using RDN.Library.Classes.EmailServer;
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
        public ActionResult UnSubscribe(string email, string listType)
        {
            var typeList = (SubscriptionServiceType)Enum.Parse(typeof(SubscriptionServiceType), listType);

            if (typeList == SubscriptionServiceType.subscription)
            {
                ViewBag.Successful = EmailServer.AddEmailToUnsubscribeList(email);

            }



            return View();
        }
    }
}