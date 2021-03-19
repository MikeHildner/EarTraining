using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WaveLibrary;

namespace EarTraining.Controllers
{
    public class SolfegController : BaseController
    {
        public SolfegController()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;
            ViewBag.ShowDo = true;
        }
        // GET: Solfeg
        public ActionResult Index(string @do)
        {
            if (!string.IsNullOrWhiteSpace(@do))
            {
                Pitch pitch = new Pitches().PitchesList.Single(s => s.PitchName.ToUpper().Split('/').Contains(@do.ToUpper()));
                ViewBag.Pitch = pitch;
            }

            return View();
        }
    }
}
