using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdelMedical.Manager.Controllers
{
    /// <summary>
    /// 언론보도
    /// </summary>
    public class PressCoverageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
