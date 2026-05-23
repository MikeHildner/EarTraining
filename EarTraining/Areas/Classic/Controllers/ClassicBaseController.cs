using System;
using System.Web;
using System.Web.Mvc;

namespace EarTraining.Areas.Classic.Controllers
{
    // Base class for any Classic-specific controllers. Also exposes the static
    // cookie helper used by ClassicAreaCookieFilter so thin wrapper controllers
    // don't need to inherit this class directly.
    public class ClassicBaseController : EarTraining.Controllers.BaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            StampCookie(filterContext.HttpContext.Response);
        }

        internal static void StampCookie(HttpResponseBase response)
        {
            response.Cookies.Set(new HttpCookie("ui_area", "classic")
            {
                Expires = DateTime.UtcNow.AddHours(24),
                HttpOnly = true,
                Path = "/"
            });
        }
    }
}
