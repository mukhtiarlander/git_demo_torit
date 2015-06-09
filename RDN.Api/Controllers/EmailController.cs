using Common.EmailServer.Library.Classes.Email;
using Common.Site.Controllers;
using RDN.Library.Classes.EmailServer;
using RDN.Library.DataModels.EmailServer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace RDN.Api.Controllers
{
    public class EmailController : BaseController
    {

        public ActionResult SendEmail(EmailItem email)
        {
            if (!IsAuthenticated)
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            if (!String.IsNullOrEmpty(email.EmailLayout))
                EmailServer.SendEmail(email.From, email.DisplayNameFrom, email.Reciever, email.Subject, email.Properties, (EmailServerLayoutsEnum)Enum.Parse(typeof(EmailServerLayoutsEnum), email.EmailLayout), (EmailPriority)email.Prio);
            else
                EmailServer.SendEmail(email.From, email.DisplayNameFrom, email.Reciever, email.Subject, email.Properties, EmailServerLayoutsEnum.Default, (EmailPriority)email.Prio);
            return Json(new GenericResponse() { IsSuccess = true });
        }
    }
}