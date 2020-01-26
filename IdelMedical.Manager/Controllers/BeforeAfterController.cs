using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdelMedical.Manager.Controllers
{
    /// <summary>
    /// 전후사진
    /// </summary>
    public class BeforeAfterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
