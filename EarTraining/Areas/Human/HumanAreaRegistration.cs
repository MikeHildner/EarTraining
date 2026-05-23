using System.Web.Mvc;

namespace EarTraining.Areas.Human
{
    public class HumanAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Human";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Human_default",
                "Human/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
