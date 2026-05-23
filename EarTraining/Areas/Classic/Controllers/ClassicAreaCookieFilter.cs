using System;
using System.Web.Mvc;

namespace EarTraining.Areas.Classic.Controllers
{
    // Registered as a global filter in FilterConfig. Stamps the ui_area cookie
    // for every action in the Classic area, including thin wrapper controllers
    // that don't inherit ClassicBaseController directly.
    public class ClassicAreaCookieFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var area = filterContext.RouteData.DataTokens["area"] as string;
            if (string.Equals(area, "Classic", StringComparison.OrdinalIgnoreCase))
            {
                ClassicBaseController.StampCookie(filterContext.HttpContext.Response);
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) { }
    }
}
