using Common.EmailServer.Library.Classes.Subscription;
using RDN.Library.Classes.EmailServer;
using RDN.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        public ActionResult UnSubscribe(string email, string listType, string id)
        {
            EmailOutModel outModel = new EmailOutModel();
            outModel.Email = email;
            var typeList = (SubscriptionServiceType)Convert.ToInt64(listType);

            if (typeList == SubscriptionServiceType.SubscriptionList)
            {
                if (EmailServer.ValidateEmailOnSubscriptionList(Convert.ToInt64(id), email.Trim()))
                    outModel.Successful = EmailServer.AddEmailToUnsubscribeList(email);
            }

            return View(outModel);
        }
    }
}