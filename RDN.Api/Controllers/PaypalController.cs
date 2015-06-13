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
    public class PaypalController : BaseController
    {
        public ActionResult InsertIPNNotification(PayPalMessage message)
        {
            if (!IsAuthenticated)
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            return Json(new GenericResponse() { IsSuccess = PaypalManagerDb.InsertIpnNotification(message) });
        }

        public ActionResult CompletePayment(Guid invoiceId, PayPalMessage message)
        {
            if (!IsAuthenticated)
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            return Json(new GenericResponse() { IsSuccess = PaypalManagerDb.CompletePayment(invoiceId, message) });
        }

        public ActionResult PendingPayment(Guid invoiceId, PayPalMessage message)
        {
            if (!IsAuthenticated)
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            return Json(new GenericResponse() { IsSuccess = PaypalManagerDb.PendingPayment(invoiceId, message) });
        }

        
        public ActionResult FailedPayment(Guid invoiceId, PayPalMessage message)
        {
            if (!IsAuthenticated)
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            return Json(new GenericResponse() { IsSuccess = PaypalManagerDb.FailedPayment(invoiceId, message) });
        }
    }
}