using System.Web.Mvc;

namespace EarTraining.Areas.Classic
{
    public class ClassicAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Classic";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Classic_default",
                "Classic/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
