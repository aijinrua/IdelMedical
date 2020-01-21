using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdelMedical.User.Kr.Extensions
{
    public static class UtilitiesExtension
    {
        public static bool IsMobile(this Controller controller)
        {
            string useragent = controller.Request.Headers["user-agent"];
            return (useragent?.ToLower().IndexOf("mobile") ?? -1) >= 0;
        }
    }
}
