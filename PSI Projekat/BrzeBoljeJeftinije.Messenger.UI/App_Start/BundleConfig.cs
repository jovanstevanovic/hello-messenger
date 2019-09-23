using System.Web;
using System.Web.Optimization;

namespace BrzeBoljeJeftinije.Messenger.UI
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/jquery.BlockUI.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/cardreader").Include("~/Scripts/cardReader.js"));
            bundles.Add(new ScriptBundle("~/bundles/signalR").Include("~/Scripts/jquery.signalR-2.1.2.js"));
            bundles.Add(new ScriptBundle("~/bundles/login").Include("~/Scripts/login.js"));
            bundles.Add(new ScriptBundle("~/bundles/poruke").Include("~/Scripts/messages.js",
                                                                     "~/Scripts/FileSaver.js"));
            bundles.Add(new ScriptBundle("~/bundles/podesavanja").Include("~/Scripts/settings.js"));
            bundles.Add(new ScriptBundle("~/bundles/admin").Include("~/Scripts/admin.js"));
            bundles.Add(new ScriptBundle("~/bundles/js").Include("~/Scripts/perfect-scrollbar.min.js",
                "~/Scripts/global.js",
                "~/Scripts/jquery.ellipsis.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                        "~/Scripts/bootstrap-datepicker.js"));

            bundles.Add(new StyleBundle("~/Content/bundled_css").Include(
                      "~/Content/css/bootstrap.css",
                        "~/Content/css/CSS1.css",
                        "~/Content/css/CSS2.css",
                        "~/Content/css/CSS3.css",
                        "~/Content/css/CSS4.css",
                        "~/Content/css/poruke.css",
                        //"~/Content/css/font-awesome.min.css",
                        "~/Content/css/perfect-scrollbar.css",
                        "~/Content/css/site.css"));
        }
    }
}
