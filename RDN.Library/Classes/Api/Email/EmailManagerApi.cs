using Common.EmailServer.Library.Classes.Email;
using Common.EmailServer.Library.Classes.Enums;
using Common.Site.Controllers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.Library.Classes.Api.Email
{
    /// <summary>
    /// this is the library we use to send an email to the API of RDNation.
    /// </summary>
    public class EmailManagerApi
    {
        RestRequestApi _api;
        const string SendEmailUrl = "Email/SendEmail";
        string _baseUrl { get; set; }
        public EmailManagerApi(string baseUrl, string apiKey)
        {
            _baseUrl = baseUrl;
            _api = new RestRequestApi(baseUrl, apiKey);
        }

        public Task<GenericResponse> SendEmailAsync(string from, string displayNameFrom, string to, string subject, string body, EmailPriority priority = EmailPriority.Important)
        {

            EmailItem email = new EmailItem();
            email.From = from;
            email.DisplayNameFrom = displayNameFrom;
            email.Reciever = to;
            email.Subject = subject;
            email.Properties.Add("body", body);
            email.Prio = (byte)priority;
            return _api.ExecuteJsonRequestAsync<GenericResponse>(SendEmailUrl, Method.POST, email);
        }
        public Task<GenericResponse> SendEmailAsync(string from, string displayNameFrom, string to, string subject, Dictionary<string, string> properties, string emailLayout, EmailPriority priority = EmailPriority.Important)
        {
            EmailItem email = new EmailItem();
            email.From = from;
            email.DisplayNameFrom = displayNameFrom;
            email.Reciever = to;
            email.Subject = subject;
            email.Properties = properties;
            email.Prio = (byte)priority;
            email.EmailLayout = emailLayout;
            return _api.ExecuteJsonRequestAsync<GenericResponse>(SendEmailUrl, Method.POST, email);
        }


    }
}
