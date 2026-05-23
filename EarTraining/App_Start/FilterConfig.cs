using EarTraining.Areas.Classic.Controllers;
using System.Web.Mvc;

namespace EarTraining
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ClassicAreaCookieFilter());
        }
    }
}
