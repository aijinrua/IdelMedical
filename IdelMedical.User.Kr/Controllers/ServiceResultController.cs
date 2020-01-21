using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdelMedical.User.Kr.Controllers
{
    public class ServiceResultController : Controller
    {
        public IActionResult BeforeAfter()
        {
            return View();
        }

        public IActionResult RealStory()
        {
            return View();
        }
    }
}
