using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdelMedical.User.Kr.Controllers
{
    public class InformationController : Controller
    {
        public IActionResult Introduce()
        {
            return View();
        }

        public IActionResult Staff()
        {
            return View();
        }

        public IActionResult Tour()
        {
            return View();
        }
    }
}
