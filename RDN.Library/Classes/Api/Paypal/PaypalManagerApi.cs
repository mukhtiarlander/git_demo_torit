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

        public Task<GenericResponse> CompletePaymentAsync(PayPalMessage message)
        {
            return _api.ExecuteAuthenticatedJsonRequestAsync<GenericResponse>(CompletePaymentUrl, HttpMethod.Post, message);
        }
        public GenericResponse CompletePayment(PayPalMessage message)
        {
            return _api.ExecuteAuthenticatedJsonRequest<GenericResponse>(CompletePaymentUrl, HttpMethod.Post, message);
        }

        public Task<GenericResponse> PendingPaymentAsync(PayPalMessage message)
        {
            return _api.ExecuteAuthenticatedJsonRequestAsync<GenericResponse>(PendingPaymentUrl, HttpMethod.Post, message);
        }
        public GenericResponse PendingPayment(PayPalMessage message)
        {
            return _api.ExecuteAuthenticatedJsonRequest<GenericResponse>(PendingPaymentUrl, HttpMethod.Post, message);
        }
        public Task<GenericResponse> FailedPaymentAsync(PayPalMessage message)
        {
            return _api.ExecuteAuthenticatedJsonRequestAsync<GenericResponse>(FailedPaymentUrl, HttpMethod.Post, message);
        }
        public GenericResponse FailedPayment(PayPalMessage message)
        {
            return _api.ExecuteAuthenticatedJsonRequest<GenericResponse>(FailedPaymentUrl, HttpMethod.Post, message);
        }


    }
}
