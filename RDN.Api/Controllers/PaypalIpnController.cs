using Common.Site.Controllers;
using RDN.Library.Classes.Config;
using RDN.Library.Classes.Payment.Paypal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RDN.Api.Controllers
{
    public class PaypalIpnController : BaseController
    {
        public ActionResult InsertNewIPNNotification(PayPalMessage message)
        {
            if (!IsAuthenticated)
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            return Json(new GenericResponse() { IsSuccess = PaypalManagerDb.InsertIpnNotification(message) });
        }
    }
}