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
using log4net;

namespace RDN.TransactionHandler
{
    /// <summary>
    /// Summary description for IPNHandlers
    /// </summary>
    public class IPNHandlers : IHttpHandler
    {
        private static readonly ILog logger = LogManager.GetLogger("PaypalLogger");
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string values = string.Empty;
                foreach (var value in context.Request.Form.Keys)
                {
                    values+= value + "=" + context.Request.Form[value.ToString()].ToString();
                }
                logger.Info(values);

                IPNHandler ipn = new IPNHandler(LibraryConfig.IsProduction, HttpContext.Current);

                ipn.CheckStatus();
                ipn.SendIPNNotificationToApi();
            }
            catch (Exception exception)
            {
                logger.Error(context.Request.Form);
                logger.Error("Exception", exception);
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