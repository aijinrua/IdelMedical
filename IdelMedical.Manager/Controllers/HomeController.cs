using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IdelMedical.Manager.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IdelMedical.Manager.Controllers
{
    [Auth]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var loginmanager = HttpContext.Session.GetString("LoginManager");
            var manager = JsonConvert.DeserializeObject<IdelMedical.Database.Tables.Manager>(loginmanager);

            if (manager.Type == Database.ManagerTypes.Root || manager.Type == Database.ManagerTypes.Counseler)
            {
                return Redirect("/Member");
            }
            else
            {
                return Redirect("/Main");
            }
        }
    }
}
