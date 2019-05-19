using EarTraining.Authorization;
using EarTrainingLibrary.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;

namespace EarTraining.Controllers
{
    //[EarTrainingAuthorize]
    public class BaseController : Controller
    {
        public BaseController()
        {
            //var stageNames = new List<string>
            //{
            //    "Fats Treetop",
            //    "Keys Clamfinger"
            //};

            //stageNames.Shuffle();
            //ViewBag.StageName = stageNames.First();
        }

        //[HttpDelete]
        public void DeleteOldFiles()
        {
            string tempFolder = HostingEnvironment.MapPath("~/Temp");
            EarTrainingLibrary.Utility.FileSystem.CleanFolder(tempFolder);
        }
    }
}