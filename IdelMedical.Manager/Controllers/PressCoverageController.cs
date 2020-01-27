using IdelMedical.Database;
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
    /// 언론보도
    /// </summary>
    public class PressCoverageController : Controller
    {
        public DatabaseContext Db { get; }

        public PressCoverageController(DatabaseContext db)
        {
            this.Db = db;
        }

        public async Task<IActionResult> Index(int Page = 1)
        {
            var query = this.Db.PressCoverages.AsQueryable();

            var pager = new PageHandler(Page, await query.CountAsync(), 15);
            var items = await query
                .Select(x => new Database.Tables.PressCoverage
                {
                    Id = x.Id,
                    Subject = x.Subject,
                    CreateTime = x.CreateTime
                })
                .OrderByDescending(x => x.CreateTime)
                .Skip((pager.CurrentPage - 1) * pager.TakeItemCount)
                .Take(pager.TakeItemCount)
                .ToArrayAsync();

            ViewBag.Pager = pager;
            ViewBag.Items = items;

            return View();
        }

        public IActionResult Detail(string Search, int Page = 1)
        {
            var slideTest = new List<SlideModel>();


            int testItemCount = 50;



            //if (!string.IsNullOrWhiteSpace(Search))
            //{
            //    list = list
            //        .Where(x => x.NickName.IndexOf(Search) >= 0 ||
            //                    x.UserId.IndexOf(Search) >= 0 ||
            //                    x.TelegramId.IndexOf(Search) >= 0);
            //}



            for (int i = 0; i < testItemCount; i++)
            {
                var item = i + 1;
                slideTest.Add(new SlideModel()
                {
                    Number = item,
                    Title = "SlideTitle_" + (item < 10 ? ("0" + item.ToString()) : item.ToString()),
                    UploadTime = DateTime.Now.ToString("yyyy-MM-dd")
                });
            }

            //var pager = new PageHandler(Page, totalItemCount, 20);
            var pager = new PageHandler(Page, testItemCount, 15);

            ViewBag.Pager = pager;
            ViewBag.SlideData = slideTest;
            return View();
        }
    }
}
