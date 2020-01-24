using IdelMedical.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IdelMedical.User.Kr.Controllers
{
    public class AuthController : Controller
    {
        public DatabaseContext Db { get; }

        public AuthController(DatabaseContext db)
        {
            this.Db = db;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DoLogin(string userid, string passwd)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userid))
                    throw new Exception("아이디를 입력하세요");
                if (string.IsNullOrWhiteSpace(passwd))
                    throw new Exception("비밀번호를 입력하세요");

                var user = this.Db.Users
                        .Where(x => x.UserId == userid)
                        .FirstOrDefault();

                if (user == null)
                    throw new Exception("아이디 또는 비밀번호가 일치하지 않습니다.");

                if (user.Passwd != passwd)
                    throw new Exception("아이디 또는 비밀번호가 일치하지 않습니다.");

                HttpContext.Session.SetString("UserKey", user.UserKey);

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult NaverLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NaverLogin(string id, string name, string email, string gender)
        {
            if (this.Db.Users.Any(x => x.UserKey == $"naver_{id}"))
            {
                HttpContext.Session.SetString("UserKey", $"naver_{id}");
                return Redirect("/");
            }
            else
            {
                HttpContext.Session.SetString("Id", id);
                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Email", email);
                HttpContext.Session.SetString("Gender", gender);

                return Redirect("/Auth/Join?accountType=naver");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserKey");

            return Redirect("/");
        }

        public IActionResult Join(string accountType)
        {
            if (accountType == "naver")
            {
                HttpContext.Session.SetString("AccountType", "Naver");

                ViewBag.AccountType = "Naver";
                ViewBag.Id = HttpContext.Session.GetString("Id");
                ViewBag.Name = HttpContext.Session.GetString("Name");
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.Gender = HttpContext.Session.GetString("Gender");
            }
            else
            {
                HttpContext.Session.SetString("AccountType", "Idel");

                HttpContext.Session.Remove("Id");
                HttpContext.Session.Remove("Name");
                HttpContext.Session.Remove("Email");
                HttpContext.Session.Remove("Gender");

                ViewBag.AccountType = "Idel";
            }

            return View();
        }

        [HttpPost]
        public IActionResult CheckUserId(string userid)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userid))
                    throw new Exception("아이디를 입력하세요");
                if (Regex.IsMatch(userid, "[^a-zZ-Z0-9]"))
                    throw new Exception("아이디는 알파벳과 숫자만 가능합니다");
                if (userid.Length < 4)
                    throw new Exception("4자 이상 입력하세요");

                if (this.Db.Users.Any(x => x.UserId == userid))
                    throw new Exception("이미 사용중인 아이디 입니다.");
                else
                    return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DoJoin(
            string userid, string passwd, string passwdRe, string name, string birthday, string phone1, string phone2, string phone3, bool isreceivephone,
            string email1, string email2, bool isreceiveemail, GenderTypes gender)
        {
            try
            {
                var accountType = AccountTypes.Idel;
                var userkey = default(string);

                switch (HttpContext.Session.GetString("AccountType"))
                {
                    case "Naver":
                        accountType = AccountTypes.Naver;

                        var id = HttpContext.Session.GetString("Id");
                        userkey = $"naver_{id}";
                        break;
                    case "Idel":
                    default:
                        accountType = AccountTypes.Idel;
                        userkey = Guid.NewGuid().ToString().ToLower().Replace("-", "");
                        break;
                }

                if (accountType == AccountTypes.Idel)
                {
                    if (string.IsNullOrWhiteSpace(userid))
                        throw new Exception("아이디를 입력하세요");
                    if (Regex.IsMatch(userid, "[^a-zA-Z0-9]"))
                        throw new Exception("아이디는 영문과 숫자만 가능합니다.");
                    if (userid.Length < 4)
                        throw new Exception("아이디는 최소 4자 이상 입력해야 합니다.");
                    if (this.Db.Users.Any(x => x.UserId == userid))
                        throw new Exception("이미 사용중인 아이디 입니다.");

                    if (string.IsNullOrWhiteSpace(passwd))
                        throw new Exception("비밀번호를 입력하세요");
                    if (passwd.Length < 4)
                        throw new Exception("비밀번호는 최소 4자 이상 입력해야 합니다.");
                    if (string.IsNullOrWhiteSpace(passwdRe))
                        throw new Exception("확인용 비밀번호를 입력하세요");
                    if (passwd != passwdRe)
                        throw new Exception("비밀번호가 확인용 비밀번호와 일치하지 않습니다.");
                }
                else if (accountType == AccountTypes.Naver)
                {
                    var id = HttpContext.Session.GetString("Id");
                    userid = userkey;
                    passwd = userkey;
                }

                if (string.IsNullOrWhiteSpace(name))
                    throw new Exception("이름을 입력하세요");
                if (string.IsNullOrWhiteSpace(birthday))
                    throw new Exception("생년월일을 입력하세요");
                if (Regex.IsMatch(birthday, "[^0-9]"))
                    throw new Exception("생년월일은 숫자만 6자리로 입력해주세요");
                if (birthday.Length != 6)
                    throw new Exception("생년월일은 YYMMDD 형태로 입력해주세요");

                if (string.IsNullOrWhiteSpace(phone2) || string.IsNullOrWhiteSpace(phone3))
                    throw new Exception("휴대폰 번호를 입력해주세요");
                if (Regex.IsMatch(phone2, "[^0-9]") || Regex.IsMatch(phone3, "[^0-9]"))
                    throw new Exception("휴대폰 번호는 숫자만 입력해주세요");
                if (phone2.Length < 3 || phone2.Length > 4 || phone3.Length != 4)
                    throw new Exception("휴대폰 번호를 다시 확인해 주세요");

                if (string.IsNullOrWhiteSpace(email1) || string.IsNullOrWhiteSpace(email2))
                    throw new Exception("이메일을 입력해 주세요");

                var user = new Database.Tables.User
                {
                    AccountType = accountType,
                    Birthday = birthday,
                    CreateTime = DateTime.Now,
                    Email = $"{email1}@{email2}",
                    Gender = gender,
                    IsReceiveEmail = isreceiveemail,
                    IsReceivePhone = isreceivephone,
                    Name = name,
                    Passwd = passwd,
                    Phone = $"{phone1}-{phone2}-{phone3}",
                    UserId = userid,
                    UserKey = userkey
                };

                this.Db.Users.Add(user);

                this.Db.SaveChanges();

                HttpContext.Session.SetString("UserKey", user.UserKey);

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }
    }
}
