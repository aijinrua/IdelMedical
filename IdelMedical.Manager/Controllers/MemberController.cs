using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdelMedical.Manager.DataModels;
using IdelMedical.Manager.Handlers;

namespace IdelMedical.Manager.Controllers
{
    /// <summary>
    /// 회원목록
    /// </summary>
    public class MemberController : Controller
    {
        public IActionResult Index(string Search, int Page = 1)
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
