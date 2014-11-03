using System.Web;
using System.Web.Optimization;

namespace RDN.Shops
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/content/jquery-ui.min.js",
                        "~/Scripts/Knockout-{version}.js",
                        "~/Scripts/Knockout.mapping-{version}.js",
                        "~/Scripts/jquery/jquery.fileupload.js",
                        "~/Scripts/jquery/jquery.numeric.js"

                        ));

            bundles.Add(new ScriptBundle("~/bundles/viewmodels").Include(
                        "~/Scripts/main.js",
                        "~/Scripts/ViewModels/Payments.js",
                        "~/Scripts/ViewModels/AdModel.js",
                        "~/Scripts/ViewModels/PropertiesModel.js",
                        "~/Scripts/ViewModels/BuyModel.js",
                        "~/Scripts/ViewModels/SubscriptionsModel.js",
                        "~/Scripts/ViewModels/ShoppingCartModel.js",
                        "~/Scripts/ViewModels/ReceiptModel.js"));


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
                      "~/Content/jquery-ui.min.css",
                      "~/Content/font-awesome.css",
                      "~/Content/site.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}
