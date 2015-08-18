using Common.Site.Controllers;
using RDN.Library.Classes.Payment.Paypal;
using RDN.Portable.Classes.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RDN.Library.Classes.Api.Paypal
{
    /// <summary>
    /// this is the library we use to send an email to the API of .
    /// </summary>
    public class PaypalManagerApi
    {
        RestRequest _api;
        const string InsertIPNNotificationUrl = "Paypal/InsertIPNNotification";
        const string CompletePaymentUrl = "Paypal/CompletePayment";
        const string PendingPaymentUrl = "Paypal/PendingPayment";
        const string FailedPaymentUrl = "Paypal/FailedPayment";
        string _baseUrl { get; set; }
        public PaypalManagerApi(string baseUrl, string apiKey)
        {
            _baseUrl = baseUrl;
            _api = new RestRequest(baseUrl, apiKey);
        }

        public GenericResponse InsertIPNNotification(PayPalMessage message)
        {
            return _api.ExecuteAuthenticatedJsonRequest<GenericResponse>(InsertIPNNotificationUrl, HttpMethod.Post, message);
        }

        public Task<GenericResponse> CompletePaymentAsync(Guid invoiceId, PayPalMessage message)
        {
            var dict = new Dictionary<string, object>();
            dict.Add("invoiceId", invoiceId.ToString());
            return _api.ExecuteAuthenticatedJsonRequestAsync<GenericResponse>(CompletePaymentUrl, HttpMethod.Post, message, dict);
        }
        public GenericResponse CompletePayment(Guid invoiceId, PayPalMessage message)
        {
            var dict = new Dictionary<string, object>();
            dict.Add("invoiceId", invoiceId.ToString());
            return _api.ExecuteAuthenticatedJsonRequest<GenericResponse>(CompletePaymentUrl, HttpMethod.Post, message, dict);
        }

        public Task<GenericResponse> PendingPaymentAsync(Guid invoiceId, PayPalMessage message)
        {
            var dict = new Dictionary<string, object>();
            dict.Add("invoiceId", invoiceId.ToString());
            return _api.ExecuteAuthenticatedJsonRequestAsync<GenericResponse>(PendingPaymentUrl, HttpMethod.Post, message, dict);
        }
        public GenericResponse PendingPayment(Guid invoiceId, PayPalMessage message)
        {
            var dict = new Dictionary<string, object>();
            dict.Add("invoiceId", invoiceId.ToString());
            return _api.ExecuteAuthenticatedJsonRequest<GenericResponse>(PendingPaymentUrl, HttpMethod.Post, message, dict);
        }
        public Task<GenericResponse> FailedPaymentAsync(Guid invoiceId, PayPalMessage message)
        {
            var dict = new Dictionary<string, object>();
            dict.Add("invoiceId", invoiceId.ToString());
            return _api.ExecuteAuthenticatedJsonRequestAsync<GenericResponse>(FailedPaymentUrl, HttpMethod.Post, message, dict);
        }
        public GenericResponse FailedPayment(Guid invoiceId, PayPalMessage message)
        {
            var dict = new Dictionary<string, object>();
            dict.Add("invoiceId", invoiceId.ToString());
            return _api.ExecuteAuthenticatedJsonRequest<GenericResponse>(FailedPaymentUrl, HttpMethod.Post, message, dict);
        }


    }
}
