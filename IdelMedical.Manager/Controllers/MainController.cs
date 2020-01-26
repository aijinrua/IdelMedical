﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IdelMedical.Manager.DataModels;
using IdelMedical.Manager.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdelMedical.Manager.Controllers
{
    public class MainController : Controller
    {

        [HttpGet]
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
        public IActionResult Slide(string Search, int Page = 1)
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
        public IActionResult Youtube(string Search, int Page = 1)
        {
            return View();
        }
        public IActionResult Instagram(string Search, int Page = 1)
        {
            return View();
        }
    }
}
