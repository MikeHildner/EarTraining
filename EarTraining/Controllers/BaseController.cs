﻿using EarTraining.Authorization;
using EarTrainingLibrary.Utility;
using NLog;
using System.Collections.Generic;
using System.Linq;
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
            var stageNames = new List<string>
            {
                "Fats Treetop",
                "Keys Clamfinger",
                "Sol Dofamado"
            };

            stageNames.Shuffle();
            ViewBag.StageName = stageNames.First();
        }

        //[HttpDelete]
        public void DeleteOldFiles()
        {
            string tempFolder = HostingEnvironment.MapPath("~/Temp");
            EarTrainingLibrary.Utility.FileSystem.CleanFolder(tempFolder);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            _log.Error(filterContext.Exception, filterContext.Exception.Message);
            filterContext.Result = RedirectToAction("Index", "ErrorHandler");
        }
    }
}