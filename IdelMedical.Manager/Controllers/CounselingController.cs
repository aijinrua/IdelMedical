using IdelMedical.Database;
using IdelMedical.Database.Tables;
using IdelMedical.Manager.Attributes;
using IdelMedical.Manager.DataModels;
using IdelMedical.Manager.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdelMedical.Manager.Controllers
{
    /// <summary>
    /// 상담요청
    /// </summary>
    [Auth]
    public class CounselingController : Controller
    {

        public DatabaseContext Db { get; }

        public CounselingController(DatabaseContext db)
        {
            this.Db = db;
        }

        public async Task<IActionResult> Index(int Page = 1, string search = null, string categoryFilter = null, string applyFilter = null)
        {
            var query = this.Db.Counselings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Subject.Contains(search));

            if (!string.IsNullOrWhiteSpace(categoryFilter) && categoryFilter != "전체")
                query = query.Where(x => x.Category == categoryFilter);


            if (!string.IsNullOrWhiteSpace(applyFilter) && applyFilter != "전체")
            {
                switch (applyFilter)
                {
                    case "처리완료":
                        query = query.Where(x => !string.IsNullOrWhiteSpace(x.Answer));
                        break;
                    case "미처리":
                        query = query.Where(x => string.IsNullOrWhiteSpace(x.Answer));
                        break;
                    default:
                        break;
                }
            }


            var pager = new PageHandler(Page, await query.CountAsync(), 15);
            var items = await query
                .Select(x => new Database.Tables.Counseling
                {
                    Id = x.Id,
                    Category = x.Category,
                    Subject = x.Subject,
                    Answer = x.Answer,
                    UserName = x.UserName,
                    CreateTime = x.CreateTime
                })
                .OrderByDescending(x => x.CreateTime)
                .Skip((pager.CurrentPage - 1) * pager.TakeItemCount)
                .Take(pager.TakeItemCount)
                .ToArrayAsync();

            ViewBag.Pager = pager;
            ViewBag.Search = search;
            ViewBag.Items = items;
            ViewBag.CategoryFilter = categoryFilter;
            ViewBag.ApplyFilter = applyFilter;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            var item = default(Counseling);
            if (id == null)
            {
                item = new Counseling
                {
                    Id = -1
                };
            }
            else
            {
                item = await this.Db.Counselings
                    .FirstOrDefaultAsync(x => x.Id == id.Value);
            }

            if (item == null)
                return NotFound();

            var targetWriter = default(User);
            targetWriter = await this.Db.Users.FirstOrDefaultAsync(x => x.Id == item.UserId);
            item.User = targetWriter;


            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, string answer, string memo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(answer))
                    throw new Exception("답변을 입력하세요");

                var item = default(Counseling);

                item = await this.Db.Counselings
                    .FirstOrDefaultAsync(x => x.Id == id);

                item.Answer = answer;


                await this.Db.SaveChangesAsync();

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

    }
}
