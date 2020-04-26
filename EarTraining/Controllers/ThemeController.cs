using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EarTraining.Controllers
{
    public class ThemeController : BaseController
    {
        // GET: Theme
        public ActionResult Index()
        {
            return View();
        }
    }
}