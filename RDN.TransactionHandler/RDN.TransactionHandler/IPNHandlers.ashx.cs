using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment.Paypal;
using RDN.Utilities.Config;
using RDN.Library.Classes.Config;
using Common.Site.AppConfig;
using RDN.Library.Classes.Api.Email;
using RDN.Library.Classes.EmailServer;
using RDN.Portable.Config;

namespace RDN.TransactionHandler
{
    /// <summary>
    /// Summary description for IPNHandlers
    /// </summary>
    public class IPNHandlers : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var _configManager = new CustomConfigurationManager("RDN");
                var _emailManager = new EmailManagerApi(_configManager.GetSubElement("ApiBaseUrl").Value, _configManager.GetSubElement("ApiAuthenticationKey").Value);
                _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Paypal: Can't Find Invoice ID Problem", "asdf", Common.EmailServer.Library.Classes.Enums.EmailPriority.Normal);


                //IPNHandler ipn = new IPNHandler(LibraryConfig.IsProduction, HttpContext.Current);

                //ipn.CheckStatus();
                //ipn.InsertNewIPNNotification();

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}