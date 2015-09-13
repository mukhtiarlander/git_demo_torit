﻿using Common.Site.Controllers;
using RDN.Library.Classes.Config;
using RDN.Library.Classes.Error;
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
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult InsertIPNNotification(PayPalMessage message)
        {
            try
            {
                if (!IsAuthenticated)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

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
                if (!IsAuthenticated)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

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
                if (!IsAuthenticated)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

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
                if (!IsAuthenticated)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

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