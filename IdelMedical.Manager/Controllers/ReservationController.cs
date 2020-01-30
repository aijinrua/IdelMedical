using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdelMedical.Manager.DataModels;
using IdelMedical.Manager.Handlers;
using IdelMedical.Database;
using Microsoft.EntityFrameworkCore;
using IdelMedical.Database.Tables;
using IdelMedical.Manager.Attributes;

namespace IdelMedical.Manager.Controllers
{
    /// <summary>
    /// 예약요청
    /// </summary>
    [Auth]
    public class ReservationController : Controller
    {
        public DatabaseContext Db { get; }

        public ReservationController(DatabaseContext db)
        {
            this.Db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int Page = 1)
        {
            var query = this.Db.Reservations.AsQueryable();

            var pager = new PageHandler(Page, await query.CountAsync(), 15);
            var items = await query
                .Select(x => new Database.Tables.Reservation
                {
                    Id = x.Id,
                    Category = x.Category,
                    ReservationTime = x.ReservationTime,
                    Name = x.Name,
                    Phone = x.Phone,
                    UseCall = x.UseCall,
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

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            var item = default(Reservation);
            if (id == null)
            {
                item = new Reservation
                {
                    Id = -1
                };
            }
            else
            {
                item = await this.Db.Reservations
                    .FirstOrDefaultAsync(x => x.Id == id.Value);
            }

            if (item == null)
                return NotFound();

            var targetWriter = default(User);
            targetWriter = await this.Db.Users.FirstOrDefaultAsync(x => x.Id == item.UserId);
            item.User = targetWriter;


            return View(item);
        }
    }
}
