using EarTraining.Authorization;
using EarTrainingLibrary.Utility;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;

namespace EarTraining.Controllers
{
    //[EarTrainingAuthorize]
    public class BaseController : Controller
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public BaseController()
        {
            DeleteOldFiles();

            var stageNames = new List<string>
            {
                "Fats Treetop",
                "Keys Clamfinger",
                "Sol Dofamedo"
            };

            stageNames.Shuffle();
            ViewBag.StageName = stageNames.First();
        }

        //[HttpDelete]
        public void DeleteOldFiles()
        {
            string tempFolder = HostingEnvironment.MapPath("~/Temp");
            //Task.Run(() => FileSystem.CleanFolder(tempFolder));
            new Task(() => new FileSystem().CleanFolder(tempFolder)).Start();
            //FileSystem.CleanFolder(tempFolder);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            _log.Error(filterContext.Exception, filterContext.Exception.Message);
            filterContext.Result = RedirectToAction("Index", "ErrorHandler");
        }
    }
}