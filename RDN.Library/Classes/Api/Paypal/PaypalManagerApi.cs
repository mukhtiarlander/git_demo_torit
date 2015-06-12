using Common.Site.Controllers;
using RDN.Library.Classes.Payment.Paypal;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.Library.Classes.Api.Paypal
{
    /// <summary>
    /// this is the library we use to send an email to the API of RDNation.
    /// </summary>
    public class PaypalManagerApi
    {
        RestRequestApi _api;
        const string InsertIPNNotificationUrl = "Paypal/InsertIPNNotification";
        const string CompletePaypalPaymentUrl = "Paypal/CompletePaypalPayment";
        string _baseUrl { get; set; }
        public PaypalManagerApi(string baseUrl, string apiKey)
        {
            _baseUrl = baseUrl;
            _api = new RestRequestApi(baseUrl, apiKey);
        }

        public Task<GenericResponse> InsertIPNNotification(PayPalMessage message)
        {
            return _api.ExecuteJsonRequestAsync<GenericResponse>(InsertIPNNotificationUrl, Method.POST, message);
        }

        public Task<GenericResponse> CompletePaypalPaymentAsync(Guid invoiceId, PayPalMessage message)
        {
            var dict = new Dictionary<string, object>();
            dict.Add("invoiceId", invoiceId.ToString());
            return _api.ExecuteJsonRequestAsync<GenericResponse>(CompletePaypalPaymentUrl, Method.POST, message, dict);
        }


    }
}
