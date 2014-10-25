using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Mobile;
using RDN.Library.DatabaseInitializers;
using RDN.Library.Cache.Singletons;
using System.Configuration;
using RDN.Library.Classes.Payment.Enums;

namespace RDN.Api
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

        static void Broker_OnServiceException(object sender, Exception error)
        {
            ErrorDatabaseManager.AddException(error, error.GetType(), additionalInformation: sender.ToString());
        }

        static void Broker_OnNotificationRequeue(object sender, PushSharp.Core.NotificationRequeueEventArgs e)
        {
            //ErrorDatabaseManager.AddException(error, error.GetType(), additionalInformation: sender);
        }

        static void Broker_OnNotificationFailed(object sender, PushSharp.Core.INotification notification, Exception error)
        {
            ErrorDatabaseManager.AddException(error, error.GetType(), additionalInformation: notification.Tag.ToString() + sender.ToString());
        }

        static void Broker_OnDeviceSubscriptionExpired(object sender, string expiredSubscriptionId, DateTime expirationDateUtc, PushSharp.Core.INotification notification)
        {
            ErrorDatabaseManager.AddException(new Exception("Subscription Expired"), sender.GetType(), additionalInformation: notification.Tag.ToString() + sender.ToString());
        }

        static void Broker_OnDeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, PushSharp.Core.INotification notification)
        {
            ErrorDatabaseManager.AddException(new Exception("Subscription Changed"), sender.GetType(), additionalInformation: "OLD:" + oldSubscriptionId + "NEW:" + newSubscriptionId + notification.Tag.ToString() + sender.ToString());
        }

        static void Broker_OnChannelException(object sender, PushSharp.Core.IPushChannel pushChannel, Exception error)
        {
            ErrorDatabaseManager.AddException(error, sender.GetType(), additionalInformation: pushChannel.ToString() + sender.ToString());
        }

        static void Broker_OnChannelDestroyed(object sender)
        {

        }

        static void Broker_OnChannelCreated(object sender, PushSharp.Core.IPushChannel pushChannel)
        {
            ErrorDatabaseManager.AddException(new Exception("Channel Destroyed"), sender.GetType(), additionalInformation: sender.ToString());
        }

        static void Broker_OnNotificationSent(object sender, PushSharp.Core.INotification notification)
        {

        }

        protected void Application_Start()
        {
            ErrorInitializer.Initialize();
            AreaRegistration.RegisterAllAreas();

            PushSharpServer.Instance.Broker = new PushSharp.PushBroker();
            PushSharpServer.Instance.Broker.OnNotificationSent += Broker_OnNotificationSent;
            PushSharpServer.Instance.Broker.OnChannelCreated += Broker_OnChannelCreated;
            PushSharpServer.Instance.Broker.OnChannelDestroyed += Broker_OnChannelDestroyed;
            PushSharpServer.Instance.Broker.OnChannelException += Broker_OnChannelException;
            PushSharpServer.Instance.Broker.OnDeviceSubscriptionChanged += Broker_OnDeviceSubscriptionChanged;
            PushSharpServer.Instance.Broker.OnDeviceSubscriptionExpired += Broker_OnDeviceSubscriptionExpired;
            PushSharpServer.Instance.Broker.OnNotificationFailed += Broker_OnNotificationFailed;
            PushSharpServer.Instance.Broker.OnNotificationRequeue += Broker_OnNotificationRequeue;
            PushSharpServer.Instance.Broker.OnServiceException += Broker_OnServiceException;


            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            SiteSingleton.Instance.IsProduction = Convert.ToBoolean(ConfigurationManager.AppSettings["IsProduction"].ToString());
            SiteSingleton.Instance.IsPayPalLive = (PaymentMode)Enum.Parse(typeof(PaymentMode), ConfigurationManager.AppSettings["IsPayPalLive"].ToString());
        }
        protected void Application_End()
        {
            PushSharpServer.Instance.Broker.StopAllServices(false);
        }
        protected void Application_BeginRequest()
        {
            if (Request.IsLocal)
            {
                
            }
            else if(SiteSingleton.Instance.IsProduction)
            {
                if (!Context.Request.IsSecureConnection)
                    Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));

            }
        }
        protected void Application_EndRequest()
        {
        }

    }
}