using Common.EmailServer.Library.Classes.Email;
using Common.EmailServer.Library.Classes.Enums;
using Common.Site.Controllers;
using RDN.Portable.Classes.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RDN.Library.Classes.Api.Email
{
    /// <summary>
    /// this is the library we use to send an email to the API of .
    /// </summary>
    public class EmailManagerApi
    {
        RestRequest _api;
        const string SendEmailUrl = "Email/SendEmail";
        string _baseUrl { get; set; }
        public EmailManagerApi(string baseUrl, string apiKey)
        {
            _baseUrl = baseUrl;
            _api = new RestRequest(baseUrl, apiKey);
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
            return _api.ExecuteAuthenticatedJsonRequestAsync<GenericResponse>(SendEmailUrl,HttpMethod.Post, email);
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
            return _api.ExecuteAuthenticatedJsonRequestAsync<GenericResponse>(SendEmailUrl, HttpMethod.Post, email);
        }


    }
}
