using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EarTraining.Controllers
{
    public class SettingsController : Controller
    {
        // GET: Settings
        public ActionResult Index()
        {
            foreach (var sgt in Enum.GetValues(typeof(SignalGeneratorType)))
            {
                var s = sgt.ToString();
                var i = (int)sgt;
            }

            return View();
        }
    }
}