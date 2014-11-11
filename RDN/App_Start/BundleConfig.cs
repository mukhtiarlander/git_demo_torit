using System.Web;
using System.Web.Optimization;

namespace RDN
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                //"~/content/jquery-ui.min.js",
                        "~/Scripts/Knockout-{version}.js",
                        "~/Scripts/Knockout.mapping-{version}.js",
                        "~/Scripts/jquery.idtabs.min.js",
                        "~/Scripts/jquery.timer.js",
                        "~/Scripts/jquery.easydate.js",
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/menucool.js",
                        "~/Scripts/FullCalendar/fullcalendar.min.js",
                        "~/Scripts/FullCalendar/gcal.js"

                        ));

            bundles.Add(new ScriptBundle("~/bundles/viewmodels").Include(
                        "~/Scripts/main.js",
                        "~/scripts/OpenLayers.js",
                        "~/Scripts/ViewModels/LeagueViewModel.js"));


            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/bootstrap/bootstrap-notify.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                //"~/Content/jquery-ui.min.css",
                      "~/Content/font-awesome.css",
                      "~/Content/site.css",
                      "~/Content/jquery.datatables.min.css",
                      "~/Content/bootstrap-extensions.css",
                      "~/Content/FullCalendar/fullcalendar.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}
