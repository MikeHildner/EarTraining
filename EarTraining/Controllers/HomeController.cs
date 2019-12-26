using NLog;
using System.Web.Mvc;

namespace EarTraining.Controllers
{
    public class HomeController : BaseController
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            _log.Info("Entered method");

            return View();
        }

        public ActionResult About()
        {
            _log.Info("Entered method");

            ViewBag.Message = "Supplemental Eartraining Drills";

            return View();
        }
    }
}