using IdelMedical.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdelMedical.User.Kr.Controllers
{
    public class ServiceResultController : Controller
    {
        public DatabaseContext Db { get; }

        public ServiceResultController(DatabaseContext db)
        {
            this.Db = db;
        }

        public async Task<IActionResult> BeforeAfter(string category, int page = 1)
        {
            page = Math.Max(page, 1);

            var query = this.Db.BeforeAfters.AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query
                    .Where(x => x.Category == category);
            }

            var totalItems = await query.CountAsync();
            var totalPages = Math.Max((int)Math.Ceiling(totalItems / 3d), 1);
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
                .Skip((page - 1) * 3)
                .Take(3)
                .ToArrayAsync();

            return View();
        }

        public IActionResult RealStory()
        {
            return View();
        }
    }
}
