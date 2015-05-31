using RDN.Library.Cache.Singletons;
using RDN.Library.Classes.Error;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RDN.TransactionHandler
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            Library.DatabaseInitializers.ManagementInitializer.Initialize(); // Remove the validation on the database. Meaning that we can update the database without having to update the payments

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            //ErrorDatabaseManager.AddException(new Exception("Transactions Just Started Up"), GetType());

            SiteSingleton.Instance.IsProduction = Convert.ToBoolean(Library.Classes.Config.LibraryConfig.IsProduction);
        }
    }
}