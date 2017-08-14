using System.Web;
using System.Web.Optimization;

namespace DotNetCoreReady
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/bootstrap3-typeahead.js",
                      "~/Scripts/bloodhound.js"));

            // http://winstrap.azurewebsites.net
            bundles.Add(new StyleBundle("~/Content/css").Include(
                        //"~/Content/bootstrap.css",
                        "~/Content/winstrap.min.css",
                        "~/Content/winjs-symbols.ttf",
                      "~/Content/site.css"));
            
            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                    "~/Scripts/knockout-3.4.2.js"));

            bundles.Add(new StyleBundle("~/fonts/glyphs/winjs-symbols.ttf").Include(
                "~/Content/winjs-symbols.ttf"));
        }
    }
}
