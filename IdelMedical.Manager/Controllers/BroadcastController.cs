using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdelMedical.Manager.Controllers
{
    /// <summary>
    /// 방송출연
    /// </summary>
    public class BroadcastController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
