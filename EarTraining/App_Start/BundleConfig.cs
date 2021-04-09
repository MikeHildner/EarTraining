using System.Web;
using System.Web.Optimization;

namespace EarTraining
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                      "~/Scripts/site.js",
                      "~/Scripts/toastr.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/custom.css",
                      "~/Content/site.css",
                      "~/Content/css-toggle-switch.css",
                      "~/Content/font-awesome.css",
                      "~/Content/hover.css",
                      "~/Content/toastr.css"));

            //// Angular bundles
            //bundles.Add(new ScriptBundle("~/bundles/Angular")
            //  .Include(
            //    "~/bundles/AngularOutput/inline.*",
            //    "~/bundles/AngularOutput/polyfills.*",
            //    "~/bundles/AngularOutput/scripts.*",
            //    "~/bundles/AngularOutput/vendor.*",
            //    "~/bundles/AngularOutput/runtime.*",
            //    "~/bundles/AngularOutput/main.*"));

            //bundles.Add(new StyleBundle("~/Content/Angular")
            //  .Include("~/bundles/AngularOutput/styles.*"));

            // ETAngular bundles
            //bundles.Add(new Bundle("~/bundles/ETAngular")
            //  .Include(
            //    "~/bundles/ETAngularOutput/inline.*",
            //    "~/bundles/ETAngularOutput/polyfills.*",
            //    "~/bundles/ETAngularOutput/scripts.*",
            //    "~/bundles/ETAngularOutput/vendor.*",
            //    "~/bundles/ETAngularOutput/runtime.*",
            //    "~/bundles/ETAngularOutput/main.*"));

            //bundles.Add(new StyleBundle("~/Content/ETAngular")
            //  .Include("~/bundles/ETAngularOutput/styles.*"));
        }
    }
}
