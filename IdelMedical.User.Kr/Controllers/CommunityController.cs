using IdelMedical.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdelMedical.User.Kr.Controllers
{
    public class CommunityController : Controller
    {
        public DatabaseContext Db { get; }

        public CommunityController(DatabaseContext db)
        {
            this.Db = db;
        }

        public async Task<IActionResult> NoticeList(string search, int page = 1)
        {
            page = Math.Max(page, 1);

            var query = this.Db.Notices.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query
                    .Where(x => x.Subject.Contains(search));
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

            ViewBag.Search = search;

            ViewBag.Items = await query
                .OrderByDescending(x => x.CreateTime)
                .Skip((page - 1) * 3)
                .Take(3)
                .ToArrayAsync();

            return View();
        }

        public async Task<IActionResult> NoticeDetail(int id)
        {
            var item = await this.Db.Notices
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Item = item;

            return View();
        }

        public async Task<IActionResult> BroadcastList(string search, int page = 1)
        {
            page = Math.Max(page, 1);

            var query = this.Db.Broadcasts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query
                    .Where(x => x.Subject.Contains(search));
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

            ViewBag.Search = search;

            ViewBag.Items = await query
                .OrderByDescending(x => x.CreateTime)
                .Skip((page - 1) * 3)
                .Take(3)
                .ToArrayAsync();

            return View();
        }

        public async Task<IActionResult> BroadcastDetail(int id)
        {
            var item = await this.Db.Broadcasts
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Item = item;

            return View();
        }

        public async Task<IActionResult> IdelStarList(string search, int page = 1)
        {
            page = Math.Max(page, 1);

            var query = this.Db.IdelStars.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query
                    .Where(x => x.Subject.Contains(search));
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

            ViewBag.Search = search;

            ViewBag.Items = await query
                .OrderByDescending(x => x.CreateTime)
                .Skip((page - 1) * 3)
                .Take(3)
                .ToArrayAsync();

            return View();
        }

        public async Task<IActionResult> IdelStarDetail(int id)
        {
            var item = await this.Db.IdelStars
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Item = item;

            return View();
        }

        public async Task<IActionResult> PressCoverageList(string search, int page = 1)
        {
            page = Math.Max(page, 1);

            var query = this.Db.PressCoverages.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query
                    .Where(x => x.Subject.Contains(search));
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

            ViewBag.Search = search;

            ViewBag.Items = await query
                .OrderByDescending(x => x.CreateTime)
                .Skip((page - 1) * 10)
                .Take(10)
                .ToArrayAsync();

            return View();
        }

        public async Task<IActionResult> PressCoverageDetail(int id)
        {
            var item = await this.Db.PressCoverages
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Item = item;

            return View();
        }

        public async Task<IActionResult> IdelTVList(string search, int page = 1)
        {
            page = Math.Max(page, 1);

            var query = this.Db.IdelTVs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query
                    .Where(x => x.Subject.Contains(search) ||
                                x.SubjectComment.Contains(search));
            }

            var totalItems = await query.CountAsync();
            var totalPages = Math.Max((int)Math.Ceiling(totalItems / 9d), 1);
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

            ViewBag.Search = search;

            ViewBag.Items = await query
                .OrderByDescending(x => x.CreateTime)
                .Skip((page - 1) * 9)
                .Take(9)
                .ToArrayAsync();

            return View();
        }

        public async Task<IActionResult> IdelEventList(string search, int page = 1)
        {
            page = Math.Max(page, 1);

            var query = this.Db.IdelEvents.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query
                    .Where(x => x.Subject.Contains(search) ||
                                x.SubjectComment.Contains(search));
            }

            var totalItems = await query.CountAsync();
            var totalPages = Math.Max((int)Math.Ceiling(totalItems / 6d), 1);
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

            ViewBag.Search = search;

            ViewBag.Items = await query
                .OrderByDescending(x => x.CreateTime)
                .Skip((page - 1) * 6)
                .Take(6)
                .ToArrayAsync();

            return View();
        }

        public async Task<IActionResult> IdelEventDetail(int id)
        {
            var item = await this.Db.IdelEvents
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Item = item;

            return View();
        }
    }
}
