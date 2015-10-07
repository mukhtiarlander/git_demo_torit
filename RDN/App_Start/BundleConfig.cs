using RDN.Library.Classes.Config;
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
                        "~/Scripts/FullCalendar/gcal.js",
                        "~/Scripts/Utilities/Countdown.js",
                        "~/Scripts/masonry/imagesloaded.min.js",
                        "~/Scripts/masonry/masonry.min.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/viewmodels").Include(
                "~/scripts/OpenLayers.js",
                "~/Scripts/main.js",
                        "~/Scripts/ViewModels/LeagueViewModel.js",
                        "~/Scripts/ViewModels/MemberViewModel.js",
                        "~/Scripts/ViewModels/CalendarViewModel.js",
                        "~/Scripts/ViewModels/EventViewModel.js",
                        "~/Scripts/Models/ChatModel.js",
                        "~/Scripts/game.js",
                        "~/Scripts/ViewModels/TournamentViewModel.js"));


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

            if (LibraryConfig.SiteType == Library.Classes.Site.Enums.SiteType.RollerDerby)
                bundles.Add(new ScriptBundle("~/bundles/properties").Include("~/scripts/Sites/rdnation.js"));
            else if (LibraryConfig.SiteType == Library.Classes.Site.Enums.SiteType.Soccer)
                bundles.Add(new ScriptBundle("~/bundles/properties").Include("~/scripts/Sites/snation.js"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}
