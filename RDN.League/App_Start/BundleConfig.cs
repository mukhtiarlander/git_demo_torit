using System.Web;
using System.Web.Optimization;

namespace RDN.League
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery/jquery-1.11.0.min.js",
                        "~/content/jquery/jquery-ui-1.10.4.custom.min.js",
                        "~/Scripts/knockout/Knockout-2.1.0.js",
                        "~/Scripts/knockout/Knockout.mapping-2.4.1.js",
                        "~/Scripts/jquery/jquery.ajaxfileupload.js",
                        "~/Scripts/jquery.numeric.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/knockout.validation.js",
                        "~/Scripts/jquery.idTabs.min.js",
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/Pages/AddOldGame.js",
                        "~/Scripts/jquery/jquery-ui-1.10.4.custom.min.js",
                        "~/Scripts/jquery/jquery.ui.timepicker.addon.js",
                        "~/Scripts/jquery.timer.js",
                        "~/Scripts/jquery.tokeninput.js",
                        "~/Scripts/jquery.dataTables.naturalSort.js",
                        "~/Scripts/colorPicker/colorpicker.js",
                        "~/Scripts/jquery.MultiFile.js",
                        "~/Scripts/jquery/jquery.ajaxfileupload.js",
                        "~/Scripts/FullCalendar/fullcalendar.min.js",
                        "~/Scripts/FullCalendar/gcal.js"
                        
                        ));

            bundles.Add(new ScriptBundle("~/bundles/viewmodels").Include(
                "~/Scripts/ViewModels/LeaguesViewModel.js",
                        "~/Scripts/ViewModels/MembersViewModel.js",        
                "~/Scripts/main.js"
                        ));


            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/bootstrap/bootstrap-notify.js",
                      "~/Scripts/jquery.doubleScroll.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/main.css",
                      "~/Content/jquery-ui-1.10.4.custom.css",
                      "~/Content/FullCalendar/fullcalendar.css"
                      //,
                      //"~/Content/FullCalendar/fullcalendar.print.css"
                      ));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}
