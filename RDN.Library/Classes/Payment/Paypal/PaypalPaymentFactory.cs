using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment.Enums;


///https://cms.paypal.com/mx/cgi-bin/?cmd=_render-content&content_ID=developer/e_howto_api_APPayAPI
namespace RDN.Library.Classes.Payment.Paypal
{
    public class PaypalPayment
    {
        /// <summary>
        /// Compiles the paypal url so it can be response.Redirected to.
        /// </summary>

        public string SellerEmailAddress { get; set; }


        public string BuyerEmailAddress { get; set; }


        public string Code { get; set; }

        public double Amount { get; set; }

        public string ItemName { get; set; }

        public string InvoiceNumber { get; set; }

        public string ReturnUrl { get; set; }

        public string CancelUrl { get; set; }

        public string TrackingId { get; set; }

        public string LogoUrl { get; set; }

        public string IPNUrl { get; set; }

        public enum ReturnStatus
        {
            Cancel,
            Success
        }
        /// <summary>
        /// gets the paypal base url depending on the mode.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string GetBaseUrl(bool isLive)
        {
            if (isLive)
                return "https://www.paypal.com/cgi-bin/webscr";
            else
                return "https://www.sandbox.paypal.com/cgi-bin/webscr";
        }

        /// <summary>
        /// Redirects and sends the compiled variables to paypal.
        /// </summary>
        /// <param name="mode"></param>
        public string RedirectToPaypal(bool isLive)
        {
            return CompilePaypalUrl(isLive);
        }

        /// <summary>
        /// Compiles the URL to paypal standard to get ready to send to paypal.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private string CompilePaypalUrl(bool isLive)
        {

            StringBuilder url = new StringBuilder();
            try
            {
                url.Append(GetBaseUrl(isLive) + "?" + "cmd=_xclick&business=" +
                      HttpUtility.UrlEncode(SellerEmailAddress));

                url.AppendFormat("&currency_code={0}", HttpUtility.UrlEncode(Code.ToString()));

                if (!String.IsNullOrEmpty(BuyerEmailAddress))
                    url.AppendFormat("&email={0}", HttpUtility.UrlEncode(BuyerEmailAddress));

                url.AppendFormat("&amount={0:f2}", Amount);

                if (!String.IsNullOrEmpty(LogoUrl))
                    url.AppendFormat("&image_url={0}", HttpUtility.UrlEncode(LogoUrl));

                if (!String.IsNullOrEmpty(ItemName))
                    url.AppendFormat("&item_name={0}", HttpUtility.UrlEncode(ItemName));

                if (!String.IsNullOrEmpty(TrackingId))
                    url.AppendFormat("&tracking_id={0}", HttpUtility.UrlEncode(TrackingId));

                if (!String.IsNullOrEmpty(InvoiceNumber))
                    url.AppendFormat("&invoice={0}", HttpUtility.UrlEncode(InvoiceNumber));

                if (!String.IsNullOrEmpty(ReturnUrl))
                    url.AppendFormat("&return={0}", HttpUtility.UrlEncode(ReturnUrl));

                if (!String.IsNullOrEmpty(IPNUrl))
                    url.AppendFormat("&ipn_notification_url={0}", HttpUtility.UrlEncode(IPNUrl));


                if (!String.IsNullOrEmpty(CancelUrl))
                    url.AppendFormat("&cancel_return={0}", HttpUtility.UrlEncode(CancelUrl));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return url.ToString();
        }
    }
}
