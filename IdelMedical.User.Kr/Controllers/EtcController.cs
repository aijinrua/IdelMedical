using IdelMedical.Database;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdelMedical.User.Kr.Controllers
{
    public class EtcController : Controller
    {
        public DatabaseContext Db { get; }

        public EtcController(DatabaseContext db)
        {
            this.Db = db;
        }

        [HttpPost]
        public async Task<IActionResult> ModelGathering(string name, string phone, string category, bool cbox01)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new Exception("이름을 입력하세요");
                if (string.IsNullOrWhiteSpace(phone))
                    throw new Exception("연락처를 입력하세요");
                if (string.IsNullOrWhiteSpace(category))
                    throw new Exception("모집분야를 선택하세요");
                if (!cbox01)
                    throw new Exception("약관에 동의해야 합니다.");

                await this.Db.ModelGatherings.AddAsync(new Database.Tables.ModelGathering
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    Name = name,
                    Phone = phone
                });

                await this.Db.SaveChangesAsync();

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }
    }
}
