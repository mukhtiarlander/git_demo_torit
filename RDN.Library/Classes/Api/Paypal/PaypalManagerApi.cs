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
    /// this is the library we use to send an email to the API of .
    /// </summary>
    public class PaypalManagerApi
    {
        RestRequestApi _api;
        const string InsertIPNNotificationUrl = "Paypal/InsertIPNNotification";
        const string CompletePaymentUrl = "Paypal/CompletePayment";
        const string PendingPaymentUrl = "Paypal/PendingPayment";
        const string FailedPaymentUrl = "Paypal/FailedPayment";
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

        public Task<GenericResponse> CompletePaymentAsync(Guid invoiceId, PayPalMessage message)
        {
            var dict = new Dictionary<string, object>();
            dict.Add("invoiceId", invoiceId.ToString());
            return _api.ExecuteJsonRequestAsync<GenericResponse>(CompletePaymentUrl, Method.POST, message, dict);
        }

        public Task<GenericResponse> PendingPaymentAsync(Guid invoiceId, PayPalMessage message)
        {
            var dict = new Dictionary<string, object>();
            dict.Add("invoiceId", invoiceId.ToString());
            return _api.ExecuteJsonRequestAsync<GenericResponse>(PendingPaymentUrl, Method.POST, message, dict);
        }
        public Task<GenericResponse> FailedPaymentAsync(Guid invoiceId, PayPalMessage message)
        {
            var dict = new Dictionary<string, object>();
            dict.Add("invoiceId", invoiceId.ToString());
            return _api.ExecuteJsonRequestAsync<GenericResponse>(FailedPaymentUrl, Method.POST, message, dict);
        }


    }
}
