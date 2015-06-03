using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment.Paypal;
using RDN.Utilities.Config;
using RDN.Library.Classes.Config;

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
                RDN.Library.Classes.Payment.Paypal.PaypalPaymentFactory.PaypalMode mode = PaypalPaymentFactory.PaypalMode.test;
                if (LibraryConfig.IsProduction)
                    mode = PaypalPaymentFactory.PaypalMode.live;

                IPNHandler ipn = new IPNHandler(mode, HttpContext.Current);
                ipn.CheckStatus();
                ipn.InsertNewIPNNotification();

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