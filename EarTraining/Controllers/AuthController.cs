using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EarTraining.Controllers
{
    public class AuthController : Controller
    {
        public ActionResult Unauthorized()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Unauthorized(string userName, string password)
        {
            if (userName.ToUpper() == "DOREMI" && password.ToUpper() == "IIMIN7V7IMAJ7")
            {
                this.Response.Cookies.Add(new HttpCookie("EarTraining"));
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Message = "Invalid Credentials";
                return View();
            }
        }
    }
}