using Common.Site.Controllers;
using log4net;
using Newtonsoft.Json;
using RDN.Library.Classes.Config;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment.Paypal;
using RDN.Portable.Classes.API;
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
        private static readonly ILog logger = LogManager.GetLogger("PaypalLogger");

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult InsertIPNNotification(PayPalMessage message)
        {
            try
            {
                logger.Info("InsertIPNNotificationRequest" + HttpContext.Request.Headers.Get(ApiManager.ApiKey));
                if (!IsAuthenticated)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

                logger.Info("InsertIPNNotification" + JsonConvert.SerializeObject(message));

                return Json(new GenericResponse() { IsSuccess = PaypalManagerDb.InsertIpnNotification(message) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new GenericResponse() { IsSuccess = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult CompletePayment(PayPalMessage message)
        {
            try
            {
                logger.Info("CompletePaymentRequest" + HttpContext.Request.Headers.Get(ApiManager.ApiKey));
                if (!IsAuthenticated)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

                logger.Info("CompletePayment" + JsonConvert.SerializeObject(message));

                return Json(new GenericResponse() { IsSuccess = PaypalManagerDb.CompletePayment(message) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new GenericResponse() { IsSuccess = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult PendingPayment(PayPalMessage message)
        {
            try
            {
                logger.Info("PendingPaymentRequest" + HttpContext.Request.Headers.Get(ApiManager.ApiKey));
                if (!IsAuthenticated)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

                logger.Info("PendingPayment" + JsonConvert.SerializeObject(message));

                return Json(new GenericResponse() { IsSuccess = PaypalManagerDb.PendingPayment(message) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new GenericResponse() { IsSuccess = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult FailedPayment(PayPalMessage message)
        {
            try
            {
                logger.Info("FailedPaymentRequest" + HttpContext.Request.Headers.Get(ApiManager.ApiKey));
                if (!IsAuthenticated)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

                logger.Info("FailedPayment" + JsonConvert.SerializeObject(message));

                return Json(new GenericResponse() { IsSuccess = PaypalManagerDb.FailedPayment(message) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new GenericResponse() { IsSuccess = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Test()
        {
            try
            {
                logger.Info("Test");
          

                return Json(new GenericResponse() { IsSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new GenericResponse() { IsSuccess = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}