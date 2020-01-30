using IdelMedical.Database;
using IdelMedical.Database.Tables;
using IdelMedical.Manager.Attributes;
using IdelMedical.Manager.DataModels;
using IdelMedical.Manager.Handlers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IdelMedical.Manager.Controllers
{
    /// <summary>
    /// 전후사진
    /// </summary>
    [Auth]
    public class BeforeAfterController : Controller
    {
        public DatabaseContext Db { get; }
        public IWebHostEnvironment Env { get; }

        public BeforeAfterController(DatabaseContext db, IWebHostEnvironment env)
        {
            this.Db = db;
            this.Env = env;
        }

        public async Task<IActionResult> Index(int Page = 1)
        {
            var query = this.Db.BeforeAfters.AsQueryable();

            var pager = new PageHandler(Page, await query.CountAsync(), 15);
            var items = await query
                .Select(x => new Database.Tables.BeforeAfter
                {
                    Id = x.Id,
                    Title = x.Title,
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
            var item = default(BeforeAfter);
            if (id == null)
            {
                item = new BeforeAfter
                {
                    Id = -1
                };
            }
            else
            {
                item = await this.Db.BeforeAfters
                    .FirstOrDefaultAsync(x => x.Id == id.Value);
            }

            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Update(
            int id, string category, string title, string comment,
            IFormFile beforefront, IFormFile beforehalf, IFormFile beforeside,
            IFormFile afterfront, IFormFile afterhalf, IFormFile afterside)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(category))
                    throw new Exception("카테고리를 선택하세요");
                if (string.IsNullOrWhiteSpace(title))
                    throw new Exception("제목을 입력하세요");
                if (string.IsNullOrWhiteSpace(comment))
                    throw new Exception("간략소개글을 입력하세요");

                var item = default(BeforeAfter);

                if (id <= 0)
                {
                    if (beforefront == null ||
                        beforehalf == null ||
                        beforeside == null ||
                        afterfront == null ||
                        afterhalf == null ||
                        afterside == null)
                        throw new Exception("이미지는 모두 선택해야 합니다.");

                    item = new BeforeAfter
                    {
                        CreateTime = DateTime.Now
                    };
                    await this.Db.AddAsync(item);
                }
                else
                {
                    item = await this.Db.BeforeAfters
                        .FirstOrDefaultAsync(x => x.Id == id);
                }

                item.Category = category;
                item.Title = title;
                item.Comment = comment;

                if (beforefront != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath,
                        "images",
                        "upload",
                        "beforeafter",
                        DateTime.Now.ToString("yyyyMMddHHmmss_") + new FileInfo(beforefront.FileName).Name));

                    var url = savefile.FullName.Replace(Env.WebRootPath, $"http://{Request.Host.ToString()}").Replace("\\", "/");

                    if (!savefile.Directory.Exists)
                        savefile.Directory.Create();

                    using (var fileStream = new FileStream(savefile.FullName, FileMode.Create))
                    {
                        await beforefront.OpenReadStream().CopyToAsync(fileStream);
                    }

                    item.BeforeFront = url;
                }

                if (beforehalf != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath,
                        "images",
                        "upload",
                        "beforeafter",
                        DateTime.Now.ToString("yyyyMMddHHmmss_") + new FileInfo(beforehalf.FileName).Name));

                    var url = savefile.FullName.Replace(Env.WebRootPath, $"http://{Request.Host.ToString()}").Replace("\\", "/");

                    if (!savefile.Directory.Exists)
                        savefile.Directory.Create();

                    using (var fileStream = new FileStream(savefile.FullName, FileMode.Create))
                    {
                        await beforehalf.OpenReadStream().CopyToAsync(fileStream);
                    }

                    item.BeforeHalf = url;
                }

                if (beforeside != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath,
                        "images",
                        "upload",
                        "beforeafter",
                        DateTime.Now.ToString("yyyyMMddHHmmss_") + new FileInfo(beforeside.FileName).Name));

                    var url = savefile.FullName.Replace(Env.WebRootPath, $"http://{Request.Host.ToString()}").Replace("\\", "/");

                    if (!savefile.Directory.Exists)
                        savefile.Directory.Create();

                    using (var fileStream = new FileStream(savefile.FullName, FileMode.Create))
                    {
                        await beforeside.OpenReadStream().CopyToAsync(fileStream);
                    }

                    item.BeforeSide = url;
                }

                if (afterfront != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath,
                        "images",
                        "upload",
                        "beforeafter",
                        DateTime.Now.ToString("yyyyMMddHHmmss_") + new FileInfo(afterfront.FileName).Name));

                    var url = savefile.FullName.Replace(Env.WebRootPath, $"http://{Request.Host.ToString()}").Replace("\\", "/");

                    if (!savefile.Directory.Exists)
                        savefile.Directory.Create();

                    using (var fileStream = new FileStream(savefile.FullName, FileMode.Create))
                    {
                        await afterfront.OpenReadStream().CopyToAsync(fileStream);
                    }

                    item.AfterFront = url;
                }

                if (afterhalf != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath,
                        "images",
                        "upload",
                        "beforeafter",
                        DateTime.Now.ToString("yyyyMMddHHmmss_") + new FileInfo(afterhalf.FileName).Name));

                    var url = savefile.FullName.Replace(Env.WebRootPath, $"http://{Request.Host.ToString()}").Replace("\\", "/");

                    if (!savefile.Directory.Exists)
                        savefile.Directory.Create();

                    using (var fileStream = new FileStream(savefile.FullName, FileMode.Create))
                    {
                        await afterhalf.OpenReadStream().CopyToAsync(fileStream);
                    }

                    item.AfterHalf = url;
                }

                if (afterside != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath,
                        "images",
                        "upload",
                        "beforeafter",
                        DateTime.Now.ToString("yyyyMMddHHmmss_") + new FileInfo(afterside.FileName).Name));

                    var url = savefile.FullName.Replace(Env.WebRootPath, $"http://{Request.Host.ToString()}").Replace("\\", "/");

                    if (!savefile.Directory.Exists)
                        savefile.Directory.Create();

                    using (var fileStream = new FileStream(savefile.FullName, FileMode.Create))
                    {
                        await afterside.OpenReadStream().CopyToAsync(fileStream);
                    }

                    item.AfterSide = url;
                }

                await this.Db.SaveChangesAsync();

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            try
            {
                this.Db.BeforeAfters.Remove(new BeforeAfter { Id = id });
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
