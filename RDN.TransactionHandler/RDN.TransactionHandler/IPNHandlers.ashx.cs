using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment.Paypal;
using RDN.Utilities.Config;

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
                string mode = ConfigurationManager.AppSettings["PaypalLiveTest"];
                PaypalPaymentFactory.PaypalMode setting = (PaypalPaymentFactory.PaypalMode)Enum.Parse(typeof(RDN.Library.Classes.Payment.Paypal.PaypalPaymentFactory.PaypalMode), mode);
                IPNHandler ipn = new IPNHandler(setting, HttpContext.Current);
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