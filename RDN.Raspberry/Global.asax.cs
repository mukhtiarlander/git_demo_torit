using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RDN.Library.DataModels;
using RDN.Library.DatabaseInitializers;
using RDN.Library.ViewModel;
using RDN.Library.Classes.Error;
using RDN.Utilities.Error;
using RDN.Library.Cache.Singletons;
using System.Configuration;
using RDN.Library.Classes.Payment.Enums;



namespace RDN.Raspberry
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
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

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Account", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }
        protected void Application_Error(object sender, EventArgs e)
        {
            var error = Server.GetLastError();
            string errorPrevURL = string.Empty;
            if (HttpContext.Current.Request.UrlReferrer != null)
                errorPrevURL = Server.HtmlEncode(HttpContext.Current.Request.UrlReferrer.ToString().Replace(Environment.NewLine, " "));

            ErrorDatabaseManager.AddException(error, GetType(), errorGroup:ErrorGroupEnum.Network, additionalInformation: "url:" + HttpContext.Current.Request.Url.ToString());

        }


        protected void Application_Start()
        {
            ErrorInitializer.Initialize();
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            SiteSingleton.Instance.IsProduction = Convert.ToBoolean(ConfigurationManager.AppSettings["IsProduction"].ToString());
            SiteSingleton.Instance.IsPayPalLive = (PaymentMode)Enum.Parse(typeof(PaymentMode), ConfigurationManager.AppSettings["PaymentMode"].ToString());
        }
    }
}