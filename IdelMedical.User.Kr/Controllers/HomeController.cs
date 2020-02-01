using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdelMedical.Database;
using IdelMedical.User.Kr.Extensions;
using IdelMedical.User.Kr.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdelMedical.User.Kr.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public DatabaseContext Db { get; }

        public HomeController(ILogger<HomeController> logger, DatabaseContext db)
        {
            _logger = logger;
            this.Db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.PopupNoticeItems = await this.Db.PopupNotices
                .OrderBy(x => x.OrderIdx)
                .ToArrayAsync();

            ViewBag.YoutubeItems = await this.Db.IdelTVs
                .OrderByDescending(x => x.CreateTime)
                .Take(3)
                .ToArrayAsync();

            if (this.IsMobile())
            {
                ViewBag.SlideItems = await this.Db.MainSlides
                    .Where(x => x.IsMobile)
                    .OrderBy(x => x.OrderIdx)
                    .ToArrayAsync();

                ViewBag.MainInstagrams = await this.Db.MainInstagrams
                    .OrderBy(x => x.OrderIdx)
                    .Take(9)
                    .ToArrayAsync();

                return View("MainMobile");
            }
            else
            {
                ViewBag.SlideItems = await this.Db.MainSlides
                    .Where(x => !x.IsMobile)
                    .OrderBy(x => x.OrderIdx)
                    .ToArrayAsync();

                ViewBag.MainInstagrams = await this.Db.MainInstagrams
                    .OrderBy(x => x.OrderIdx)
                    .Take(18)
                    .ToArrayAsync();

                return View("Main");
            }
        }

        public IActionResult Popup(string title, string img)
        {
            ViewBag.Title = title;
            ViewBag.Image = img;

            return View();
        }

        [Route("manager/6cc57165b5de4b71b4f4e31d24409263")]
        public IActionResult LandingRequestList(int page = 1, string search = null)
        {
            var query = this.Db.LandingRequests
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query
                    .Where(x => x.Phone.Contains(search) ||
                                x.UserName.Contains(search) ||
                                x.Question.Contains(search) ||
                                x.Memo.Contains(search));
            }

            var totalItemCount = query.Count();
            var totalPageCount = (int)Math.Ceiling(totalItemCount / 10d);

            page = Math.Max(page, 1);
            page = Math.Min(page, totalPageCount);

            var items = query
                .OrderByDescending(x => x.CreateTime)
                .Skip((page - 1) * 10)
                .Take(10)
                .Select(x => new Home_LandingRequestList_Response.Item
                {
                    CreateTime = x.CreateTime,
                    Id = x.Id,
                    Memo = x.Memo,
                    Phone = x.Phone,
                    Question = x.Question,
                    UserName = x.UserName
                })
                .ToArray();

            var model = new Home_LandingRequestList_Response
            {
                Search = search,
                TotalItemCount = totalItemCount,
                Page = page,
                Items = items
            };

            return View("LandingRequestList", model);
        }

        [HttpPost]
        public IActionResult RequestAdd(string username, string phone1, string phone2, string phone3, string question, bool confirmpolicy)
        {
            try
            {
                if (!confirmpolicy)
                    throw new Exception("개인정보 수집에 동의해 주세요.");
                if (string.IsNullOrWhiteSpace(username))
                    throw new Exception("이름을 입력해 주세요.");
                if (string.IsNullOrWhiteSpace(phone1) ||
                    string.IsNullOrWhiteSpace(phone2) ||
                    string.IsNullOrWhiteSpace(phone3))
                    throw new Exception("휴대폰 번호를 입력해 주세요.");
                if (string.IsNullOrWhiteSpace(question))
                    throw new Exception("상담내용을 입력해 주세요.");
                if (username.Length < 1 ||
                    username.Length > 10)
                    throw new Exception("이름은 1~10자 이내로 입력해 주세요");
                if (phone1.Length != 3 ||
                    phone2.Length < 3 ||
                    phone2.Length > 4 ||
                    phone3.Length != 4 ||
                    !int.TryParse(phone1, out int p1) ||
                    !int.TryParse(phone2, out int p2) ||
                    !int.TryParse(phone3, out int p3))
                    throw new Exception("휴대폰 번호가 유효하지 않습니다.");

                var phone = $"{phone1}-{phone2}-{phone3}";

                if (this.Db.LandingRequests.Any(x => x.Phone == phone))
                    throw new Exception("해당 휴대폰으로 상담이 접수되어있습니다.");

                var newEntity = new Database.Tables.LandingRequest
                {
                    UserName = username,
                    Phone = phone,
                    Question = question,
                };

                this.Db.LandingRequests.Add(newEntity);

                this.Db.SaveChanges();

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message, log = ExceptionToLog(ex) });
            }
        }

        [Route("manager/71e836bb058041b2a2f3438a92581701"), HttpPost]
        public IActionResult UpdateMemo(int id, string memo)
        {
            try
            {
                this.Db.Database.BeginTransaction();

                var acceptCount = this.Db.Database.ExecuteSqlInterpolated($"update landingrequest set memo = {memo} where id = {id}");

                if (acceptCount == 0)
                    throw new Exception("존재하지 않는 데이터 입니다.\r\n새로고침 후 다시 시도해 주세요.");

                this.Db.Database.CommitTransaction();

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                this.Db.Database.RollbackTransaction();

                return Json(new { status = false, message = ex.Message, log = ExceptionToLog(ex) });
            }
        }

        [NonAction]
        private string ExceptionToLog(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            while (ex != null)
            {
                sb.AppendLine(ex.Message);
                sb.AppendLine(ex.StackTrace);
                ex = ex.InnerException;
            }

            return sb.ToString();
        }
    }
}
