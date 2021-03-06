﻿using IdelMedical.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IdelMedical.User.Kr.Controllers
{
    public class AuthController : Controller
    {
        public DatabaseContext Db { get; }
        public IConfiguration Configuration { get; }

        public AuthController(DatabaseContext db, IConfiguration Configuration)
        {
            this.Db = db;
            this.Configuration = Configuration;
        }

        public IActionResult Login()
        {
            ViewBag.NaverClientId = this.Configuration["OauthNaver:ClientId"];
            ViewBag.NaverRedirectUrl = this.Configuration["OauthNaver:RedirectUrl"];
            ViewBag.KakaoJavascriptKey = this.Configuration["OauthKakao:JavascriptKey"];
            ViewBag.FacebookAppId = this.Configuration["OauthFacebook:AppId"];
            ViewBag.FacebookApiVersion = this.Configuration["OauthFacebook:ApiVersion"];
            ViewBag.GoogleApiKey = this.Configuration["OauthGoogle:ApiKey"];
            ViewBag.GoogleClientId = this.Configuration["OauthGoogle:ClientId"];
            ViewBag.GoogleRedirectUrl = this.Configuration["OauthGoogle:RedirectUrl"];


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
                        .Where(x => x.AccountType == AccountTypes.Idel && x.UserId == userid && x.Passwd == passwd)
                        .FirstOrDefault();

                if (user == null)
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
        public async Task<IActionResult> NaverLogin(string access_token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(access_token))
                {
                    return View();
                }
                else
                {
                    using (var http = new HttpClient())
                    {
                        http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {access_token}");
                        using (var response = await http.GetAsync($"https://openapi.naver.com/v1/nid/me"))
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            content = Regex.Unescape(content);
                            var data = JsonConvert.DeserializeAnonymousType(content, new
                            {
                                resultcode = default(string),
                                message = default(string),
                                response = new
                                {
                                    id = default(string),
                                    gender = default(string),
                                    email = default(string),
                                    name = default(string),
                                    birthday = default(string)
                                }
                            });

                            HttpContext.Session.SetString("AccountType", "Naver");
                            HttpContext.Session.SetString("AccessToken", access_token);

                            if (this.Db.Users.Any(x => x.UserKey == $"naver_{data.response.id}"))
                            {
                                HttpContext.Session.SetString("UserKey", $"naver_{data.response.id}");
                                return Redirect("/");
                            }
                            else
                            {
                                HttpContext.Session.SetString("Id", data.response.id);
                                HttpContext.Session.SetString("Name", data.response.name ?? "");
                                HttpContext.Session.SetString("Email", data.response.email ?? "");
                                HttpContext.Session.SetString("Gender", data.response.gender ?? "");

                                return Redirect("/Auth/Join?accountType=naver");
                            }
                        }
                    }
                }

                throw new Exception();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> KakaoLogin(string access_token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(access_token))
                {
                    throw new Exception();
                }
                else
                {
                    using (var http = new HttpClient())
                    {
                        http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {access_token}");
                        using (var response = await http.GetAsync($"https://kapi.kakao.com/v2/user/me"))
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            content = Regex.Unescape(content);
                            var data = JsonConvert.DeserializeAnonymousType(content, new
                            {
                                id = default(int),
                                kakao_account = new
                                {
                                    profile = new
                                    {
                                        nickname = default(string)
                                    },
                                    email = default(string)
                                }
                            });

                            HttpContext.Session.SetString("AccountType", "Kakao");
                            HttpContext.Session.SetString("AccessToken", access_token);

                            if (this.Db.Users.Any(x => x.UserKey == $"kakao_{data.id}"))
                            {
                                HttpContext.Session.SetString("UserKey", $"kakao_{data.id}");
                                return Redirect("/");
                            }
                            else
                            {
                                HttpContext.Session.SetString("Id", data.id.ToString());
                                HttpContext.Session.SetString("Name", data.kakao_account.profile.nickname ?? "");
                                HttpContext.Session.SetString("Email", data.kakao_account.email ?? "");

                                return Redirect("/Auth/Join?accountType=kakao");
                            }
                        }
                    }
                }

                throw new Exception();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> FacebookLogin(string access_token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(access_token))
                {
                    throw new Exception();
                }
                else
                {
                    using (var http = new HttpClient())
                    {
                        using (var response = await http.GetAsync($"https://graph.facebook.com/me?fields=name,email&access_token={access_token}"))
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            content = Regex.Unescape(content);
                            var data = JsonConvert.DeserializeAnonymousType(content, new
                            {
                                name = default(string),
                                id = default(string),
                                email = default(string)
                            });

                            HttpContext.Session.SetString("AccountType", "Facebook");
                            HttpContext.Session.SetString("AccessToken", access_token);

                            if (this.Db.Users.Any(x => x.UserKey == $"facebook_{data.id}"))
                            {
                                HttpContext.Session.SetString("UserKey", $"facebook_{data.id}");
                                return Redirect("/");
                            }
                            else
                            {
                                HttpContext.Session.SetString("Id", data.id);
                                HttpContext.Session.SetString("Name", data.name ?? "");
                                HttpContext.Session.SetString("Email", data.email ?? "");

                                return Redirect("/Auth/Join?accountType=facebook");
                            }
                        }
                    }
                }

                throw new Exception();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GoogleLogin(string access_token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(access_token))
                {
                    throw new Exception();
                }
                else
                {
                    using (var http = new HttpClient())
                    {
                        using (var response = await http.GetAsync($"https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token={access_token}"))
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            content = Regex.Unescape(content);
                            var data = JsonConvert.DeserializeAnonymousType(content, new
                            {
                                id = default(string),
                                email = default(string),
                                name = default(string)
                            });

                            HttpContext.Session.SetString("AccountType", "Google");
                            HttpContext.Session.SetString("AccessToken", access_token);

                            if (this.Db.Users.Any(x => x.UserKey == $"google_{data.id}"))
                            {
                                HttpContext.Session.SetString("UserKey", $"google_{data.id}");
                                return Redirect("/");
                            }
                            else
                            {
                                HttpContext.Session.SetString("Id", data.id);
                                HttpContext.Session.SetString("Name", data.name ?? "");
                                HttpContext.Session.SetString("Email", data.email ?? "");

                                return Redirect("/Auth/Join?accountType=google");
                            }
                        }
                    }
                }

                throw new Exception();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserKey");

            var accountType = HttpContext.Session.GetString("AccountType");

            switch (accountType)
            {
                case "Naver":
                    {
                        
                    }
                    break;
                case "Facebook":
                    {

                    }
                    break;
                default:
                    break;
            }

            return Redirect("/");
        }

        [HttpGet]
        public IActionResult Join(string accountType)
        {
            switch (accountType)
            {
                case "naver":
                    HttpContext.Session.SetString("AccountType", "Naver");

                    ViewBag.AccountType = "Naver";
                    ViewBag.Id = HttpContext.Session.GetString("Id");
                    ViewBag.Name = HttpContext.Session.GetString("Name");
                    ViewBag.Email = HttpContext.Session.GetString("Email");
                    ViewBag.Gender = HttpContext.Session.GetString("Gender");
                    break;
                case "kakao":
                    ViewBag.AccountType = "Kakao";
                    ViewBag.Id = HttpContext.Session.GetString("Id");
                    ViewBag.Name = HttpContext.Session.GetString("Name");
                    ViewBag.Email = HttpContext.Session.GetString("Email");
                    break;
                case "facebook":
                    ViewBag.AccountType = "Facebook";
                    ViewBag.Id = HttpContext.Session.GetString("Id");
                    ViewBag.Name = HttpContext.Session.GetString("Name");
                    ViewBag.Email = HttpContext.Session.GetString("Email");
                    break;
                case "google":
                    ViewBag.AccountType = "Google";
                    ViewBag.Id = HttpContext.Session.GetString("Id");
                    ViewBag.Name = HttpContext.Session.GetString("Name");
                    ViewBag.Email = HttpContext.Session.GetString("Email");
                    break;
                default:
                    ViewBag.AccountType = "Idel";
                    HttpContext.Session.SetString("AccountType", "Idel");
                    HttpContext.Session.Remove("Id");
                    HttpContext.Session.Remove("Name");
                    HttpContext.Session.Remove("Email");
                    HttpContext.Session.Remove("Gender");
                    break;
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
                        {
                            accountType = AccountTypes.Naver;

                            var id = HttpContext.Session.GetString("Id");
                            userkey = $"naver_{id}";
                        }
                        break;
                    case "Kakao":
                        {
                            accountType = AccountTypes.Kakao;

                            var id = HttpContext.Session.GetString("Id");
                            userkey = $"kakao_{id}";
                        }
                        break;
                    case "Facebook":
                        {
                            accountType = AccountTypes.Facebook;

                            var id = HttpContext.Session.GetString("Id");
                            userkey = $"facebook_{id}";
                        }
                        break;
                    case "Google":
                        {
                            accountType = AccountTypes.Google;

                            var id = HttpContext.Session.GetString("Id");
                            userkey = $"google_{id}";
                        }
                        break;
                    case "Idel":
                    default:
                        {
                            accountType = AccountTypes.Idel;
                            userkey = Guid.NewGuid().ToString().ToLower().Replace("-", "");
                        }
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
                else
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

        [HttpGet]
        public IActionResult Find()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FindId(string name, string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new Exception("이름을 입력해 주세요");
                if (string.IsNullOrWhiteSpace(email))
                    throw new Exception("이메일을 입력해 주세요");
                if (!Regex.IsMatch(email, "^[0-9a-zA-Z]([-_.]?[0-9a-zA-Z])*@[0-9a-zA-Z]([-_.]?[0-9a-zA-Z])*.[a-zA-Z]{2,3}$"))
                    throw new Exception("이메일 형식이 잘못되었습니다");

                var user = await this.Db.Users
                    .Where(x => x.AccountType == AccountTypes.Idel && x.Name == name && x.Email == email)
                    .Select(x => new
                    {
                        x.UserId
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                    throw new Exception("일치하는 정보가 없습니다.\n간편가입 계정은 확인되지 않습니다.");

                var client = new SmtpClient
                {
                    Host = "idelmedi.com",
                    UseDefaultCredentials = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                var mail = new MailMessage
                {
                    From = new MailAddress("admin@idelmedi.com", "아이델 성형외과"),
                    SubjectEncoding = Encoding.UTF8,
                    Subject = "[발신전용] 아이델 아이디 찾기 결과",
                    Body = $"본 메일은 발신 전용 메일입니다.<br /><br />회원님의 아이디는 <strong>{user.UserId}</strong> 입니다.",
                    IsBodyHtml = true
                };

                mail.To.Add(email);

                client.Send(mail);

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> FindPw(string name, string userid, string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new Exception("이름을 입력해 주세요");
                if (string.IsNullOrWhiteSpace(userid))
                    throw new Exception("아이디를 입력해 주세요");
                if (string.IsNullOrWhiteSpace(email))
                    throw new Exception("이메일을 입력해 주세요");
                if (!Regex.IsMatch(email, "^[0-9a-zA-Z]([-_.]?[0-9a-zA-Z])*@[0-9a-zA-Z]([-_.]?[0-9a-zA-Z])*.[a-zA-Z]{2,3}$"))
                    throw new Exception("이메일 형식이 잘못되었습니다");

                var user = await this.Db.Users
                    .Where(x => x.AccountType == AccountTypes.Idel && x.Name == name && x.UserId == userid && x.Email == email)
                    .Select(x => new
                    {
                        x.Passwd
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                    throw new Exception("일치하는 정보가 없습니다.\n간편가입 계정은 확인되지 않습니다.");

                var client = new SmtpClient
                {
                    Host = "idelmedi.com",
                    UseDefaultCredentials = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                var mail = new MailMessage
                {
                    From = new MailAddress("admin@idelmedi.com", "아이델 성형외과"),
                    SubjectEncoding = Encoding.UTF8,
                    Subject = "[발신전용] 아이델 비밀번호 찾기 결과",
                    Body = $"본 메일은 발신 전용 메일입니다.<br /><br />회원님의 비밀번호는 <strong>{user.Passwd}</strong> 입니다.",
                    IsBodyHtml = true
                };

                mail.To.Add(email);

                client.Send(mail);

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }
    }
}
