using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using RDN.Library.Classes.Error;
using RDN.Library.DatabaseInitializers;
using RDN.Library.Util.Enum;
using RDN.Utilities.Config;
using StackExchange.Profiling;
using RDN.Portable.Config;
using RDN.Library.Cache.Singletons;
using System.Configuration;
using StackExchange.Profiling.EntityFramework6;
using RDN.Shops;
using System.Web.Optimization;
using RDN.Library.Classes.Config;

namespace RDN.Store
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            MiniProfilerEF6.Initialize();

            ErrorInitializer.Initialize();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);


            BundleConfig.RegisterBundles(BundleTable.Bundles);

            log4net.Config.XmlConfigurator.Configure();

            
        }
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            try
            {
                if (!HttpContext.Current.Request.IsLocal)
                {
                    filters.Add(new RequireHttpsAttribute());
                }
            }
            catch
            { }
        }
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            #region CartController
            routes.MapRoute(
        "StoreCart", // Route name
        "cart", // URL with parameters
        new { controller = "Cart", action = "StoreCart" } // Parameter defaults
    );

            routes.MapRoute(
        "StoreCheckOut", // Route name
        "checkout/{merchantId}", // URL with parameters
        new { controller = "Cart", action = "StoreCheckOut" } // Parameter defaults
    );
            #endregion

            #region ListingController
            routes.MapRoute(
        "StoreItemListing", // Route name
        "{url}/{name}/{id}", // URL with parameters
        new { controller = "Listings", action = "ViewListing" },
            new { url = "roller-derby-item" }
    );
            #endregion

            #region ShopController
            routes.MapRoute(
          "StoreMerchant", // Route name
          "{url}/{id}/{name}", // URL with parameters
          new { controller = "Shop", action = "Shop" },
            new { url = "roller-derby-shop" }
      );
            #endregion

            #region CategoryController
            routes.MapRoute(
        "Category", // Route name
        "category/{name}/{id}", // URL with parameters
        new { controller = "Category", action = "ViewCategory" } // Parameter defaults
    );
            #endregion

            #region ReceiptController
            routes.MapRoute(
        "Receipt", // Route name
        "receipt/{invoiceId}", // URL with parameters
        new { controller = "Receipt", action = "ReceiptIndex" } // Parameter defaults
    );
            #endregion

            #region HomeController


            routes.MapRoute(
     "About", // Route name
     "about/{action}", // URL with parameters
     new { controller = "About", action = "Index" } // Parameter defaults
 );
            routes.MapRoute(
"Sell", // Route name
"sell", // URL with parameters
new { controller = "Home", action = "sell" } // Parameter defaults
);
            routes.MapRoute(
"Shops", // Route name
"{url}", // URL with parameters
new { controller = "Home", action = "Shops" },
            new { url = "(roller-derby|soccer|swim)-shops" }
);
            routes.MapRoute(
"Review", // Route name
"product-review/{name}/{id}/{invoiceId}", // URL with parameters
new { controller = "Home", action = "Review" } // Parameter defaults
);



            routes.MapRoute(
            "StoreAddToCart", // Route name
            "AddToCart", // URL with parameters
            new { controller = "Home", action = "AddToCart" } // Parameter defaults
        );

                        #endregion
            routes.MapRoute(
             "Default", // Route name
             "{action}", // URL with parameters
             new { controller = "Home", action = "Index" } // Parameter defaults
         );

            routes.MapRoute(
                "DefaultController", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );


        }
        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

            // Get the exception object.
            Exception exc = Server.GetLastError();
            ErrorDatabaseManager.AddException(exc, GetType());
            Server.ClearError();
            Response.Redirect(LibraryConfig.ShopSite+ "?u=" + SiteMessagesEnum.sww);
        }
        protected void Application_BeginRequest()
        {
            if (Request.IsLocal)
            {
                MiniProfiler.Start();
            }
            else
            {
                if (!Context.Request.IsSecureConnection)
                    Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));
            }
        }
        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }

    }
}