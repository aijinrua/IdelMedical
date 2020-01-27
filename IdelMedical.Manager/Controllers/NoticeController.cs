using IdelMedical.Database;
using IdelMedical.Database.Tables;
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
    /// 공지사항
    /// </summary>
    public class NoticeController : Controller
    {
        public DatabaseContext Db { get; }
        public IWebHostEnvironment Env { get; }

        public NoticeController(DatabaseContext db, IWebHostEnvironment env)
        {
            this.Db = db;
            this.Env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int Page = 1)
        {
            var query = this.Db.Notices.AsQueryable();

            var pager = new PageHandler(Page, await query.CountAsync(), 15);
            var items = await query
                .Select(x => new Database.Tables.Notice
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

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            var item = default(Notice);
            if (id == null)
            {
                item = new Notice
                {
                    Id = -1
                };
            }
            else
            {
                item = await this.Db.Notices
                    .FirstOrDefaultAsync(x => x.Id == id.Value);
            }

            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, string subject, IFormFile thumb_pc, IFormFile img_pc, IFormFile thumb_mb, IFormFile img_mb, string link)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subject))
                    throw new Exception("제목을 입력하세요");

                var item = default(Notice);

                if (id <= 0)
                {
                    if (thumb_pc == null || img_pc == null || thumb_mb == null || img_mb == null)
                        throw new Exception("이미지 파일을 모두 선택해야 합니다");

                    item = new Notice
                    {
                        CreateTime = DateTime.Now
                    };
                    await this.Db.AddAsync(item);
                }
                else
                {
                    item = await this.Db.Notices
                        .FirstOrDefaultAsync(x => x.Id == id);
                }

                item.Subject = subject;
                item.Link = link;
                item.LinkTarget = "_blank";

                if (thumb_pc != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath, 
                        "images",
                        "upload", 
                        "notice", 
                        DateTime.Now.ToString("yyyyMMddHHmmss_") + new FileInfo(thumb_pc.FileName).Name));

                    var url = savefile.FullName.Replace(Env.WebRootPath, $"http://{Request.Host.ToString()}").Replace("\\", "/");

                    if (!savefile.Directory.Exists)
                        savefile.Directory.Create();

                    using (var fileStream = new FileStream(savefile.FullName, FileMode.Create))
                    {
                        await thumb_pc.OpenReadStream().CopyToAsync(fileStream);
                    }

                    item.ThumbnailPC = url;
                }

                if (img_pc != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath,
                        "images",
                        "upload",
                        "notice",
                        DateTime.Now.ToString("yyyyMMddHHmmss_") + new FileInfo(img_pc.FileName).Name));

                    var url = savefile.FullName.Replace(Env.WebRootPath, $"http://{Request.Host.ToString()}").Replace("\\", "/");

                    if (!savefile.Directory.Exists)
                        savefile.Directory.Create();

                    using (var fileStream = new FileStream(savefile.FullName, FileMode.Create))
                    {
                        await img_pc.OpenReadStream().CopyToAsync(fileStream);
                    }

                    item.ContentPC = url;
                }

                if (thumb_mb != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath,
                        "images",
                        "upload",
                        "notice",
                        DateTime.Now.ToString("yyyyMMddHHmmss_") + new FileInfo(thumb_mb.FileName).Name));

                    var url = savefile.FullName.Replace(Env.WebRootPath, $"http://{Request.Host.ToString()}").Replace("\\", "/");

                    if (!savefile.Directory.Exists)
                        savefile.Directory.Create();

                    using (var fileStream = new FileStream(savefile.FullName, FileMode.Create))
                    {
                        await thumb_mb.OpenReadStream().CopyToAsync(fileStream);
                    }

                    item.ThumbnailMobile = url;
                }

                if (img_mb != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath,
                        "images",
                        "upload",
                        "notice",
                        DateTime.Now.ToString("yyyyMMddHHmmss_") + new FileInfo(img_mb.FileName).Name));

                    var url = savefile.FullName.Replace(Env.WebRootPath, $"http://{Request.Host.ToString()}").Replace("\\", "/");

                    if (!savefile.Directory.Exists)
                        savefile.Directory.Create();

                    using (var fileStream = new FileStream(savefile.FullName, FileMode.Create))
                    {
                        await img_mb.OpenReadStream().CopyToAsync(fileStream);
                    }

                    item.ContentMobile = url;
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
                this.Db.Notices.Remove(new Notice { Id = id });
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
