using System;
using System.Web.Mvc;

namespace EarTraining.Areas.Human.Controllers
{
    // Registered as a global filter in FilterConfig. Stamps the ui_area cookie
    // for every action in the Human area, including thin wrapper controllers
    // that don't inherit HumanBaseController directly.
    public class HumanAreaCookieFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var area = filterContext.RouteData.DataTokens["area"] as string;
            if (string.Equals(area, "Human", StringComparison.OrdinalIgnoreCase))
            {
                HumanBaseController.StampCookie(filterContext.HttpContext.Response);
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) { }
    }
}
