using System.Web;
using System.Web.Mvc;

namespace EarTraining.Authorization
{
    public class EarTrainingAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            HttpCookie cookie = httpContext.Request.Cookies.Get("EarTraining");

            return cookie != null;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // If they are authorized, handle accordingly
            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                // Otherwise redirect to your specific authorized area
                filterContext.Result = new RedirectResult("~/Auth/Unauthorized");
            }
        }
    }
}