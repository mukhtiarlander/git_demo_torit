using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using RDN.Library.Classes.EmailServer;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Enums.Stripe;
using RDN.Utilities.Config;
using RDN.Utilities.Enums;
using Stripe;
using RDN.Portable.Config;
using RDN.Library.Classes.Config;
using RDN.Library.Classes.Api.Email;

namespace RDN.TransactionHandler
{
    /// <summary>
    /// Summary description for StripeEventHandler
    /// </summary>
    public class StripeEventHandler : IHttpHandler
    {

        EmailManagerApi _api;
        public bool IsReusable
        {
            get { return true; }
        }

        private static readonly string path = @"C:\temp\stripe.com\";


        public void ProcessRequest(HttpContext context)
        {
            try
            {


                Random rand = new Random();
                var filename = "stripe-json " + DateTime.Now.ToString("yyyyMMdd-HHmmssfff") + "-" + rand.Next(100) + ".txt";
                FileInfo file = new FileInfo(path + filename);

                string json = new StreamReader(context.Request.InputStream).ReadToEnd();

                //TODO:
                //get stripe event.
                //use unique id within stripe to iterate through databases and check which database payment was made in.
                //update the database based on that information.

                var stripeEvent = StripeEventUtility.ParseEvent(json);

                switch (stripeEvent.Type)
                {
                    // take a look at all the types here: https://stripe.com/docs/api#event_types
                    case "customer.created":
                        StripeHandler.CustomerCreated(stripeEvent, json);
                        break;
                    case "invoice.created":
                        StripeHandler.InvoiceCreated(stripeEvent, json);
                        break;
                    case "charge.succeeded":
                        StripeHandler.ChargeSucceeded(stripeEvent, json);
                        break;
                    case "charge.failed":
                        StripeHandler.ChargeFailed(stripeEvent, json);
                        break;
                    case "invoice.payment_succeeded":
                        StripeHandler.InvoicePaymentSucceeded(stripeEvent, json);
                        break;
                    case "customer.subscription.created":
                        StripeHandler.SubscriptionCreated(stripeEvent, json);
                        break;
                    case "customer.subscription.updated":
                        StripeHandler.SubscriptionUpdated(stripeEvent, json);
                        break;
                    default:
                        EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, "New Stripe Type Found", json);
                        break;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
    }
}