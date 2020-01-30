using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdelMedical.Manager.Attributes
{
    public class AuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var loginManager = context.HttpContext.Session.GetString("LoginManager");
            if (string.IsNullOrWhiteSpace(loginManager))
            {
                context.Result = new RedirectResult("/auth/login");
            }

            base.OnActionExecuting(context);
        }
    }
}
