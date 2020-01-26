using IdelMedical.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdelMedical.User.Kr.Controllers
{
    public class CounselingController : Controller
    {
        public DatabaseContext Db { get; }

        public CounselingController(DatabaseContext db)
        {
            this.Db = db;
        }

        public IActionResult Reservation()
        {
            return View();
        }

        public async Task<IActionResult> Counseling(string category, int page = 1)
        {
            page = Math.Max(page, 1);

            var query = this.Db.Counselings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query
                    .Where(x => x.Category == category);
            }

            var totalItems = await query.CountAsync();
            var totalPages = Math.Max((int)Math.Ceiling(totalItems / 10d), 1);
            page = Math.Min(page, totalPages);
            var startPage = (((int)Math.Ceiling(page / 5d) - 1) * 5) + 1;
            var endPage = Math.Min(startPage + 4, totalPages);
            var prevPage = startPage <= 1 ? -1 : startPage - 5;
            var nextPage = endPage >= totalPages ? -1 : endPage + 1;

            ViewBag.Page = page;
            ViewBag.StartPage = startPage;
            ViewBag.EndPage = endPage;
            ViewBag.PrevPage = prevPage;
            ViewBag.NextPage = nextPage;

            ViewBag.Category = category;

            ViewBag.Items = await query
                .OrderByDescending(x => x.CreateTime)
                .Skip((page - 1) * 10)
                .Take(10)
                .ToArrayAsync();

            return View();
        }

        [HttpGet]
        public IActionResult CounselingCreate()
        {
            var userkey = HttpContext.Session.GetString("UserKey");
            if (string.IsNullOrWhiteSpace(userkey))
            {
                return Redirect("/auth/login");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CounselingCreate(string category, string subject, string substance, bool isSecret)
        {
            try
            {
                var userkey = HttpContext.Session.GetString("UserKey");
                if (string.IsNullOrWhiteSpace(userkey))
                    throw new Exception("로그인이 필요합니다.");
                if (string.IsNullOrWhiteSpace(category))
                    throw new Exception("카테고리를 선택하세요");
                if (string.IsNullOrWhiteSpace(subject))
                    throw new Exception("제목을 입력하세요");
                if (string.IsNullOrWhiteSpace(substance))
                    throw new Exception("내용을 입력하세요");

                var user = await this.Db.Users
                    .Where(x => x.UserKey == userkey)
                    .Select(x => new
                    {
                        x.Id,
                        x.Name
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                    throw new Exception("잘못된 접근입니다.");

                var newItem = new Database.Tables.Counseling
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    IsSecret = isSecret,
                    Subject = subject,
                    Substance = substance,
                    UserId = user.Id,
                    UserName = user.Name
                };
                await this.Db.Counselings.AddAsync(newItem);

                await this.Db.SaveChangesAsync();

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CounselingDetail(int id)
        {
            var item = await this.Db.Counselings
                .Where(x => x.Id == id)
                .Include(x => x.User)
                .FirstOrDefaultAsync();

            if (item == null)
                return NotFound();

            if (item.IsSecret)
            {
                var userkey = HttpContext.Session.GetString("UserKey");
                if (string.IsNullOrWhiteSpace(userkey))
                {
                    ViewBag.IsShow = false;
                }
                else if (item.User.UserKey != userkey)
                {
                    ViewBag.IsShow = false;
                }
                else
                {
                    ViewBag.IsShow = true;
                }
            }
            else
            {
                ViewBag.IsShow = false;
            }

            ViewBag.Item = item;

            return View();
        }
    }
}
