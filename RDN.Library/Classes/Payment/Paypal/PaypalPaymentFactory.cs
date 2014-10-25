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
    public class PaypalPaymentFactory
    {
        /// <summary>
        /// Compiles the paypal url so it can be response.Redirected to.
        /// </summary>

        private PaypalMode _mode;

        public PaypalMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        private string _sellerEmailAddress;

        public string SellerEmailAddress
        {
            get { return _sellerEmailAddress; }
            set { _sellerEmailAddress = value; }
        }

        private string _buyerEmailAddress;

        public string BuyerEmailAddress
        {
            get { return _buyerEmailAddress; }
            set { _buyerEmailAddress = value; }
        }

        private string _code;

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }
        private double _amount;

        public double Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }
        private string _itemName;

        public string ItemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }
        private string _invoiceNumber;

        public string InvoiceNumber
        {
            get { return _invoiceNumber; }
            set { _invoiceNumber = value; }
        }
        private string _returnUrl;

        public string ReturnUrl
        {
            get { return _returnUrl; }
            set { _returnUrl = value; }
        }
        private string _cancelUrl;

        public string CancelUrl
        {
            get { return _cancelUrl; }
            set { _cancelUrl = value; }
        }
        private string _logoUrl;

        public string LogoUrl
        {
            get { return _logoUrl; }
            set { _logoUrl = value; }
        }

        public enum PaypalMode
        {
            test,
            live
        }

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
        public static string GetBaseUrl(PaypalMode mode)
        {
            switch (mode)
            {
                case PaypalMode.test:
                    return "https://www.sandbox.paypal.com/cgi-bin/webscr";
                case PaypalMode.live:
                    return "https://www.paypal.com/cgi-bin/webscr";
                default:
                    return "https://www.sandbox.paypal.com/cgi-bin/webscr";
            }
        }

        /// <summary>
        /// Redirects and sends the compiled variables to paypal.
        /// </summary>
        /// <param name="mode"></param>
        public string RedirectToPaypal()
        {
            return CompilePaypalUrl();
        }
        //public bool CheckForVerifiedEmail(string emailToVerify)
        //{

        //    PayPal.APIService ser = new PayPal.APIService();
        //    PayPal.AdaptivePayments.AdaptivePaymentsService s = new PayPal.AdaptivePayments.AdaptivePaymentsService();
        //    s.
        //}

        /// <summary>
        /// Compiles the URL to paypal standard to get ready to send to paypal.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private string CompilePaypalUrl()
        {

            StringBuilder url = new StringBuilder();
            try
            {
                url.Append(GetBaseUrl(Mode) + "?" + "cmd=_xclick&business=" +
                      HttpUtility.UrlEncode(_sellerEmailAddress));

                url.AppendFormat("&currency_code={0}", HttpUtility.UrlEncode(_code.ToString()));

                if (_buyerEmailAddress != null && _buyerEmailAddress != "")
                    url.AppendFormat("&email={0}", HttpUtility.UrlEncode(_buyerEmailAddress));

                url.AppendFormat("&amount={0:f2}", _amount);

                if (LogoUrl != null && LogoUrl != "")
                    url.AppendFormat("&image_url={0}", HttpUtility.UrlEncode(LogoUrl));

                if (_itemName != null && _itemName != "")
                    url.AppendFormat("&item_name={0}", HttpUtility.UrlEncode(_itemName));

                if (_invoiceNumber != null && _invoiceNumber != "")
                    url.AppendFormat("&invoice={0}", HttpUtility.UrlEncode(_invoiceNumber));

                if (_returnUrl != null)
                    url.AppendFormat("&return={0}", HttpUtility.UrlEncode(_returnUrl));

                if (_cancelUrl != null)
                    url.AppendFormat("&cancel_return={0}", HttpUtility.UrlEncode(_cancelUrl));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return url.ToString();
        }
    }
}
