using System;
using System.Web;
using System.Web.Mvc;

namespace EarTraining.Areas.Human.Controllers
{
    // Base class for any Human-specific controllers. Also exposes the static
    // cookie helper used by HumanAreaCookieFilter so thin wrapper controllers
    // don't need to inherit this class directly.
    public class HumanBaseController : EarTraining.Controllers.BaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            StampCookie(filterContext.HttpContext.Response);
        }

        internal static void StampCookie(HttpResponseBase response)
        {
            response.Cookies.Set(new HttpCookie("ui_area", "human")
            {
                Expires = DateTime.UtcNow.AddHours(24),
                HttpOnly = true,
                Path = "/"
            });
        }
    }
}
