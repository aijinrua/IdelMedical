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
    /// 이벤트
    /// </summary>
    [Auth]
    public class IdelEventController : Controller
    {
        public DatabaseContext Db { get; }
        public IWebHostEnvironment Env { get; }

        public IdelEventController(DatabaseContext db, IWebHostEnvironment env)
        {
            this.Db = db;
            this.Env = env;
        }

        public async Task<IActionResult> Index(int Page = 1)
        {
            var query = this.Db.IdelEvents.AsQueryable();

            var pager = new PageHandler(Page, await query.CountAsync(), 15);
            var items = await query
                .Select(x => new Database.Tables.IdelEvent
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
            var item = default(IdelEvent);
            if (id == null)
            {
                item = new IdelEvent
                {
                    Id = -1
                };
            }
            else
            {
                item = await this.Db.IdelEvents
                    .FirstOrDefaultAsync(x => x.Id == id.Value);
            }

            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, string subject, string link, string linktarget, string video, IFormFile thumb_pc, IFormFile img_pc, IFormFile thumb_mb, IFormFile img_mb)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subject))
                    throw new Exception("제목을 입력하세요");
                if (!string.IsNullOrWhiteSpace(link) && string.IsNullOrWhiteSpace(linktarget))
                    throw new Exception("링크를 입력시 링크 이동 타입을 선택해야 합니다.");

                var item = default(IdelEvent);

                if (id <= 0)
                {
                    if (thumb_pc == null || thumb_mb == null)
                        throw new Exception("섬네일은 필수로 선택해야 합니다.");

                    if (img_pc == null && img_mb == null && string.IsNullOrWhiteSpace(video))
                        throw new Exception("내용 이미지 혹은 Youtube 영상정보를 입력해야 합니다.");

                    item = new IdelEvent
                    {
                        CreateTime = DateTime.Now
                    };
                    await this.Db.AddAsync(item);
                }
                else
                {
                    item = await this.Db.IdelEvents
                        .FirstOrDefaultAsync(x => x.Id == id);
                }

                item.Subject = subject;
                item.Link = link;
                item.LinkTarget = linktarget;
                item.VideoLink = video;

                if (thumb_pc != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath,
                        "images",
                        "upload",
                        "idelevent",
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
                        "idelevent",
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
                        "idelevent",
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
                        "idelevent",
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

                if (string.IsNullOrWhiteSpace(item.ContentPC) && string.IsNullOrWhiteSpace(item.ContentMobile) && string.IsNullOrWhiteSpace(item.VideoLink))
                    throw new Exception("내용 이미지 혹은 Youtube 영상정보를 입력해야 합니다.");

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
                this.Db.IdelEvents.Remove(new IdelEvent { Id = id });
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
