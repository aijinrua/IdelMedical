using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdelMedical.Database;
using IdelMedical.Database.Tables;
using IdelMedical.Manager.Attributes;
using IdelMedical.Manager.DataModels;
using IdelMedical.Manager.Handlers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdelMedical.Manager.Controllers
{
    [Auth]
    public class MainController : Controller
    {
        public DatabaseContext Db { get; }
        public IWebHostEnvironment Env { get; }

        public MainController(DatabaseContext db, IWebHostEnvironment env)
        {
            this.Db = db;
            this.Env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.MainItem = await this.Db.Mains
                .FirstOrDefaultAsync();

            ViewBag.MainSlides = await this.Db.MainSlides
                .ToArrayAsync();

            ViewBag.PopupNoticeItems = await this.Db.PopupNotices
                .OrderBy(x => x.OrderIdx)
                .ToArrayAsync();

            ViewBag.InstagramItems = await this.Db.MainInstagrams
                .OrderBy(x => x.OrderIdx)
                .ToArrayAsync();

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> AppendPopupNoticeItem(IFormFile image, string link, string linktarget)
        {
            try
            {
                if (image == null)
                    throw new Exception("이미지를 선택하세요");

                var mainId = await this.Db.Mains
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();

                var orderIdx = 1;

                var orderIdxQuery = this.Db.PopupNotices
                    .AsQueryable();

                if (await orderIdxQuery.CountAsync() > 0)
                {
                    orderIdx += await orderIdxQuery
                        .MaxAsync(x => x.OrderIdx);
                }

                var item = new PopupNotice
                {
                    CreateTime = DateTime.Now,
                    Link = link,
                    LinkTarget = string.IsNullOrWhiteSpace(link) ? null : linktarget,
                    OrderIdx = orderIdx
                };
                await this.Db.PopupNotices.AddAsync(item);

                if (image != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath,
                        "images",
                        "upload",
                        "popupnotice",
                        DateTime.Now.ToString("yyyyMMddHHmmss_") + new FileInfo(image.FileName).Name));

                    var url = savefile.FullName.Replace(Env.WebRootPath, $"http://{Request.Host.ToString()}").Replace("\\", "/");

                    if (!savefile.Directory.Exists)
                        savefile.Directory.Create();

                    using (var fileStream = new FileStream(savefile.FullName, FileMode.Create))
                    {
                        await image.OpenReadStream().CopyToAsync(fileStream);
                    }

                    item.Url = url;
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
        public async Task<IActionResult> UpdatePopupNoticeItem(int id, IFormFile image, string link, string linktarget)
        {
            try
            {
                var item = await this.Db.PopupNotices
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (item == null)
                    throw new Exception("잘못된 접근입니다.");

                item.Link = link;
                item.LinkTarget = string.IsNullOrWhiteSpace(link) ? null : linktarget;

                if (image != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath,
                        "images",
                        "upload",
                        "popupnotice",
                        DateTime.Now.ToString("yyyyMMddHHmmss_") + new FileInfo(image.FileName).Name));

                    var url = savefile.FullName.Replace(Env.WebRootPath, $"http://{Request.Host.ToString()}").Replace("\\", "/");

                    if (!savefile.Directory.Exists)
                        savefile.Directory.Create();

                    using (var fileStream = new FileStream(savefile.FullName, FileMode.Create))
                    {
                        await image.OpenReadStream().CopyToAsync(fileStream);
                    }

                    item.Url = url;
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
        public async Task<IActionResult> RemovePopupNoticeItem(int id)
        {
            try
            {
                var item = await this.Db.PopupNotices
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (item == null)
                    throw new Exception("잘못된 접근입니다.");

                var items = this.Db.PopupNotices
                    .Where(x => x.OrderIdx > item.OrderIdx);

                foreach (var orderItem in items)
                    orderItem.OrderIdx--;

                this.Db.PopupNotices.Remove(item);

                await this.Db.SaveChangesAsync();

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MovePrevPopupNoticeItem(int id)
        {
            try
            {
                var item = await this.Db.PopupNotices
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                var prevItem = await this.Db.PopupNotices
                    .Where(x => x.Id != id && x.OrderIdx < item.OrderIdx)
                    .OrderByDescending(x => x.OrderIdx)
                    .FirstOrDefaultAsync();

                if (prevItem != null)
                {
                    var tempIdx = item.OrderIdx;
                    item.OrderIdx = prevItem.OrderIdx;
                    prevItem.OrderIdx = tempIdx;

                    await this.Db.SaveChangesAsync();
                }

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MoveNextPopupNoticeItem(int id)
        {
            try
            {
                var item = await this.Db.PopupNotices
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                var nextItem = await this.Db.PopupNotices
                    .Where(x => x.Id != id && x.OrderIdx > item.OrderIdx)
                    .OrderBy(x => x.OrderIdx)
                    .FirstOrDefaultAsync();

                if (nextItem != null)
                {
                    var tempIdx = item.OrderIdx;
                    item.OrderIdx = nextItem.OrderIdx;
                    nextItem.OrderIdx = tempIdx;

                    await this.Db.SaveChangesAsync();
                }

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }



        [HttpPost]
        public async Task<IActionResult> AppendSlideItem(IFormFile image, bool ismobile, string link, string linktarget)
        {
            try
            {
                if (image == null)
                    throw new Exception("이미지를 선택하세요");

                var orderIdx = 1;

                var orderIdxQuery = this.Db.MainSlides
                    .Where(x => x.IsMobile == ismobile);

                if (await orderIdxQuery.CountAsync() > 0)
                {
                    orderIdx += await orderIdxQuery
                        .MaxAsync(x => x.OrderIdx);
                }

                var item = new MainSlide
                {
                    CreateTime = DateTime.Now,
                    IsMobile = ismobile,
                    Link = link,
                    LinkTarget = string.IsNullOrWhiteSpace(link) ? null : linktarget,
                    OrderIdx = orderIdx
                };
                await this.Db.MainSlides.AddAsync(item);

                if (image != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath,
                        "images",
                        "upload",
                        "mainslide",
                        DateTime.Now.ToString("yyyyMMddHHmmss_") + new FileInfo(image.FileName).Name));

                    var url = savefile.FullName.Replace(Env.WebRootPath, $"http://{Request.Host.ToString()}").Replace("\\", "/");

                    if (!savefile.Directory.Exists)
                        savefile.Directory.Create();

                    using (var fileStream = new FileStream(savefile.FullName, FileMode.Create))
                    {
                        await image.OpenReadStream().CopyToAsync(fileStream);
                    }

                    item.Url = url;
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
        public async Task<IActionResult> UpdateSlideItem(int id, IFormFile image, string link, string linktarget)
        {
            try
            {
                var mainId = await this.Db.Mains
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();

                var item = await this.Db.MainSlides
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (item == null)
                    throw new Exception("잘못된 접근입니다.");

                item.Link = link;
                item.LinkTarget = string.IsNullOrWhiteSpace(link) ? null : linktarget;

                if (image != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath,
                        "images",
                        "upload",
                        "mainslide",
                        DateTime.Now.ToString("yyyyMMddHHmmss_") + new FileInfo(image.FileName).Name));

                    var url = savefile.FullName.Replace(Env.WebRootPath, $"http://{Request.Host.ToString()}").Replace("\\", "/");

                    if (!savefile.Directory.Exists)
                        savefile.Directory.Create();

                    using (var fileStream = new FileStream(savefile.FullName, FileMode.Create))
                    {
                        await image.OpenReadStream().CopyToAsync(fileStream);
                    }

                    item.Url = url;
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
        public async Task<IActionResult> RemoveSlideItem(int id)
        {
            try
            {
                var item = await this.Db.MainSlides
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (item == null)
                    throw new Exception("잘못된 접근입니다.");

                var items = this.Db.MainSlides
                    .Where(x => x.OrderIdx > item.OrderIdx);

                foreach (var orderItem in items)
                    orderItem.OrderIdx--;

                this.Db.MainSlides.Remove(item);

                await this.Db.SaveChangesAsync();

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MovePrevSlideItem(int id)
        {
            try
            {
                var item = await this.Db.MainSlides
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                var prevItem = await this.Db.MainSlides
                    .Where(x => x.Id != id && x.OrderIdx < item.OrderIdx && x.IsMobile == item.IsMobile)
                    .OrderByDescending(x => x.OrderIdx)
                    .FirstOrDefaultAsync();

                if (prevItem != null)
                {
                    var tempIdx = item.OrderIdx;
                    item.OrderIdx = prevItem.OrderIdx;
                    prevItem.OrderIdx = tempIdx;

                    await this.Db.SaveChangesAsync();
                }

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MoveNextSlideItem(int id)
        {
            try
            {
                var item = await this.Db.MainSlides
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                var nextItem = await this.Db.MainSlides
                    .Where(x => x.Id != id && x.OrderIdx > item.OrderIdx && x.IsMobile == item.IsMobile)
                    .OrderBy(x => x.OrderIdx)
                    .FirstOrDefaultAsync();

                if (nextItem != null)
                {
                    var tempIdx = item.OrderIdx;
                    item.OrderIdx = nextItem.OrderIdx;
                    nextItem.OrderIdx = tempIdx;

                    await this.Db.SaveChangesAsync();
                }

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }



        [HttpPost]
        public async Task<IActionResult> AppendInstagramItem(IFormFile image, string link, string linktarget)
        {
            try
            {
                if (image == null)
                    throw new Exception("이미지를 선택하세요");

                var mainId = await this.Db.Mains
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();

                var orderIdx = 1;

                var orderIdxQuery = this.Db.MainInstagrams
                    .AsQueryable();

                if (await orderIdxQuery.CountAsync() > 0)
                {
                    orderIdx += await orderIdxQuery
                        .MaxAsync(x => x.OrderIdx);
                }

                var item = new MainInstagram
                {
                    CreateTime = DateTime.Now,
                    Link = link,
                    LinkTarget = string.IsNullOrWhiteSpace(link) ? null : linktarget,
                    OrderIdx = orderIdx
                };
                await this.Db.MainInstagrams.AddAsync(item);

                if (image != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath,
                        "images",
                        "upload",
                        "instagram",
                        DateTime.Now.ToString("yyyyMMddHHmmss_") + new FileInfo(image.FileName).Name));

                    var url = savefile.FullName.Replace(Env.WebRootPath, $"http://{Request.Host.ToString()}").Replace("\\", "/");

                    if (!savefile.Directory.Exists)
                        savefile.Directory.Create();

                    using (var fileStream = new FileStream(savefile.FullName, FileMode.Create))
                    {
                        await image.OpenReadStream().CopyToAsync(fileStream);
                    }

                    item.Url = url;
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
        public async Task<IActionResult> UpdateInstagramItem(int id, IFormFile image, string link, string linktarget)
        {
            try
            {
                var item = await this.Db.MainInstagrams
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (item == null)
                    throw new Exception("잘못된 접근입니다.");

                item.Link = link;
                item.LinkTarget = string.IsNullOrWhiteSpace(link) ? null : linktarget;

                if (image != null)
                {
                    var savefile = new FileInfo(Path.Combine(
                        Env.WebRootPath,
                        "images",
                        "upload",
                        "instagram",
                        DateTime.Now.ToString("yyyyMMddHHmmss_") + new FileInfo(image.FileName).Name));

                    var url = savefile.FullName.Replace(Env.WebRootPath, $"http://{Request.Host.ToString()}").Replace("\\", "/");

                    if (!savefile.Directory.Exists)
                        savefile.Directory.Create();

                    using (var fileStream = new FileStream(savefile.FullName, FileMode.Create))
                    {
                        await image.OpenReadStream().CopyToAsync(fileStream);
                    }

                    item.Url = url;
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
        public async Task<IActionResult> RemoveInstagramItem(int id)
        {
            try
            {
                var item = await this.Db.MainInstagrams
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (item == null)
                    throw new Exception("잘못된 접근입니다.");

                var items = this.Db.MainInstagrams
                    .Where(x => x.OrderIdx > item.OrderIdx);

                foreach (var orderItem in items)
                    orderItem.OrderIdx--;

                this.Db.MainInstagrams.Remove(item);

                await this.Db.SaveChangesAsync();

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MovePrevInstagramItem(int id)
        {
            try
            {
                var item = await this.Db.MainInstagrams
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                var prevItem = await this.Db.MainInstagrams
                    .Where(x => x.Id != id && x.OrderIdx < item.OrderIdx)
                    .OrderByDescending(x => x.OrderIdx)
                    .FirstOrDefaultAsync();

                if (prevItem != null)
                {
                    var tempIdx = item.OrderIdx;
                    item.OrderIdx = prevItem.OrderIdx;
                    prevItem.OrderIdx = tempIdx;

                    await this.Db.SaveChangesAsync();
                }

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MoveNextInstagramItem(int id)
        {
            try
            {
                var item = await this.Db.MainInstagrams
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                var nextItem = await this.Db.MainInstagrams
                    .Where(x => x.Id != id && x.OrderIdx > item.OrderIdx)
                    .OrderBy(x => x.OrderIdx)
                    .FirstOrDefaultAsync();

                if (nextItem != null)
                {
                    var tempIdx = item.OrderIdx;
                    item.OrderIdx = nextItem.OrderIdx;
                    nextItem.OrderIdx = tempIdx;

                    await this.Db.SaveChangesAsync();
                }

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }
    }
}
