using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdelMedical.Manager.DataModels;
using IdelMedical.Manager.Handlers;
using IdelMedical.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using IdelMedical.Database.Tables;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace IdelMedical.Manager.Controllers
{
    /// <summary>
    /// 아이델TV
    /// </summary>
    public class IdelTVController : Controller
    {
        public DatabaseContext Db { get; }
        public IWebHostEnvironment Env { get; }

        public IdelTVController(DatabaseContext db, IWebHostEnvironment env)
        {
            this.Db = db;
            this.Env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int Page = 1)
        {
            var query = this.Db.IdelTVs.AsQueryable();

            var pager = new PageHandler(Page, await query.CountAsync(), 15);
            var items = await query
                .Select(x => new Database.Tables.IdelTV
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
            var item = default(IdelTV);
            if (id == null)
            {
                item = new IdelTV
                {
                    Id = -1
                };
            }
            else
            {
                item = await this.Db.IdelTVs
                    .FirstOrDefaultAsync(x => x.Id == id.Value);
            }

            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, string subject, string video, string subjectcomment, IFormFile thumb_pc, IFormFile thumb_mb)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subject))
                    throw new Exception("제목을 입력하세요");
                if (string.IsNullOrWhiteSpace(subjectcomment))
                    throw new Exception("부제목을 입력하세요");
                if (string.IsNullOrWhiteSpace(video))
                    throw new Exception("Youtube 영상 정보를 입력하세요");

                var item = default(IdelTV);

                if (id <= 0)
                {
                    if (thumb_pc == null || thumb_mb == null)
                        throw new Exception("이미지 파일을 모두 선택해야 합니다");

                    item = new IdelTV
                    {
                        CreateTime = DateTime.Now
                    };
                    await this.Db.AddAsync(item);
                }
                else
                {
                    item = await this.Db.IdelTVs
                        .FirstOrDefaultAsync(x => x.Id == id);
                }

                item.Subject = subject;
                item.SubjectComment = subjectcomment;
                item.VideoLink = video;

                if (thumb_pc != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath,
                        "images",
                        "upload",
                        "ideltv",
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

                if (thumb_mb != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath,
                        "images",
                        "upload",
                        "ideltv",
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
                this.Db.IdelTVs.Remove(new IdelTV { Id = id });
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
