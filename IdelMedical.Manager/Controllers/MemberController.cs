using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdelMedical.Manager.DataModels;
using IdelMedical.Manager.Handlers;
using IdelMedical.Database;
using Microsoft.EntityFrameworkCore;

namespace IdelMedical.Manager.Controllers
{
    /// <summary>
    /// 회원목록
    /// </summary>
    public class MemberController : Controller
    {
        public DatabaseContext Db { get; }

        public MemberController(DatabaseContext db)
        {
            this.Db = db;
        }

        public async Task<IActionResult> Index(int Page = 1)
        {
            var query = this.Db.Users.AsQueryable();

            var pager = new PageHandler(Page, await query.CountAsync(), 15);
            var items = await query
                .Select(x => new Database.Tables.User
                {
                    Id = x.Id,
                    Name = x.Name,
                    Birthday = x.Birthday,
                    Phone = x.Phone,
                    Email = x.Email,
                    Gender = x.Gender,
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

    }
}
