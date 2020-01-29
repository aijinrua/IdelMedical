using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdelMedical.User.Kr.Controllers
{
    public class BodyController : Controller
    {
        public IActionResult Body()
        {
            return View();
        }

        public IActionResult Obesityinject()
        {
            return View();
        }

        public IActionResult Hip()
        {
            return View();
        }

        public IActionResult Liposuction()
        {
            return View();
        }
    }
}
