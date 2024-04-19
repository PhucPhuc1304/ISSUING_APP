using System.Web;
using System.Web.Optimization;

namespace ISSUING_APP
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                         "~/Scripts/jquery-1.12.1.min.js", "~/Scripts/jquery-ui.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/hdbank").Include("~/Scripts/Wizard/jquery.wizard.js",
                   "~/Scripts/util.js", "~/Scripts/Common.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootbox").Include(
                        "~/Scripts/bootbox.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Styles/bootstrap-3.3.4-dist/js/bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include("~/Styles/bootstrap-3.3.4-dist/css/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/jquery-ui").Include(
                       "~/Styles/jquery-ui.css",
   "~/Styles/datepicker.min.css",
    "~/Styles/Wizard/wizard.css",
    "~/Styles/Wizard/jquery.wizard.css"));

            bundles.Add(new StyleBundle("~/Content/css-style").Include(
            "~/Styles/Style.css"));
        }
    }
}
