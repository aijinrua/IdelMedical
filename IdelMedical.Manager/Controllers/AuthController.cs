using IdelMedical.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdelMedical.Manager.Controllers
{
    public class AuthController : Controller
    {
        public DatabaseContext Db { get; }

        public AuthController(DatabaseContext db)
        {
            this.Db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userid, string passwd)
        {
            try
            {
                var item = await this.Db.Managers
                    .Where(x => x.UserId == userid && x.Passwd == passwd)
                    .FirstOrDefaultAsync();

                if (item == null)
                    throw new Exception("아이디 또는 비밀번호가 일치하지 않습니다.");

                HttpContext.Session.SetString("LoginManager", JsonConvert.SerializeObject(item));

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("LoginManager");

            return Redirect("/Auth/Login");
        }
    }
}