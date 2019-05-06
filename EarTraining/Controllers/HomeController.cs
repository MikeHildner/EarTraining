using System.Web.Mvc;

namespace EarTraining.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Supplemental Eartraining Drills";

            return View();
        }
    }
}