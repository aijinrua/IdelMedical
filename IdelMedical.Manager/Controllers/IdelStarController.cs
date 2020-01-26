using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdelMedical.Manager.Controllers
{
    /// <summary>
    /// 아이델스타
    /// </summary>
    public class IdelStarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
