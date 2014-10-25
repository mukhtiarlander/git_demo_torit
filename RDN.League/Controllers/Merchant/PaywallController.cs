using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Classes.Display;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Payment.Paywall;
using RDN.Library.Util;
using RDN.Library.Util.Enum;
using RDN.Portable.Classes.Payment.Enums;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class PaywallController : BaseController
    {
        [HttpPost]
        [Authorize]
        public ActionResult PaywallOrder(DisplayInvoice invoice)
        {
            try
            {


                var sg = new Paywall();
                sg.UpdatePaywallOrder(invoice);
                return Redirect(Url.Content("~/paywall/order/" + invoice.Merchant.PrivateManagerId.ToString().Replace("-", "") + "/" + invoice.Merchant.MerchantId.ToString().Replace("-", "") + "/" + invoice.InvoiceId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [Authorize]
        public ActionResult PaywallRefundOrder(DisplayInvoice invoice)
        {
            try
            {
                bool isSuccess = false;
                var sg = new Paywall();
                PaymentGateway pg = new PaymentGateway();
                var f = pg.StartInvoiceWizard().Initalize(invoice.Merchant.MerchantId, "USD", invoice.PaymentProvider, PaymentMode.Live, ChargeTypeEnum.Refund_Paywall)
                    .SetInvoiceId(invoice.InvoiceId)
                .SetRefundAmount(invoice.RefundAmount)
                .SetNotes(null, invoice.AdminNote);

                var response = f.FinalizeInvoice();
                if (response.Status == InvoiceStatus.Refunded)
                    isSuccess = true;

                if (isSuccess)
                    return Redirect(Url.Content("~/paywall/order/" + invoice.Merchant.PrivateManagerId.ToString().Replace("-", "") + "/" + invoice.Merchant.MerchantId.ToString().Replace("-", "") + "/" + invoice.InvoiceId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.ro));
                else
                    return Redirect(Url.Content("~/paywall/order/" + invoice.Merchant.PrivateManagerId.ToString().Replace("-", "") + "/" + invoice.Merchant.MerchantId.ToString().Replace("-", "") + "/" + invoice.InvoiceId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.sww));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        public ActionResult PaywallOrder(Guid privId, Guid storeId, Guid invoiceId)
        {
            DisplayInvoice invoice = null;
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Updated.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.ro.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Refunded Order.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sww.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Error;
                    message.Message = "Order Not Refunded. Error Sent to Developers.";
                    this.AddMessage(message);
                }

                var sg = new Paywall();
                invoice = sg.GetInvoiceForManager(storeId, privId, invoiceId);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(invoice);
        }

        [Authorize]
        public ActionResult PaywallPayments(Guid privId, Guid storeId)
        {
            List<DisplayInvoice> invoices = null;
            try
            {

                Paywall pw = new Paywall();
                invoices = pw.GetPaywallInvoices(privId, storeId);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(invoices);

        }

        [Authorize]
        public ActionResult ViewPaywall(string paywallid)
        {
            try
            {
                Paywall pw = new Paywall();
                var wall = pw.GetOwnedPaywall(Convert.ToInt64(paywallid));
                if (wall == null)
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                return View(wall);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [Authorize]
        public ActionResult AddPaywall()
        {
            try
            {
                Paywall pw = new Paywall();

                return View(pw);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [Authorize]
        public ActionResult AddPaywall(Paywall pw)
        {
            try
            {
                string priceDaily = HttpContext.Request.Form["Price"];
                string priceFull = HttpContext.Request.Form["FullPrice"];
                if (!String.IsNullOrEmpty(priceDaily))
                    pw.DailyPrice = Convert.ToDecimal(priceDaily);
                if (!String.IsNullOrEmpty(priceFull))
                    pw.TimespanPrice = Convert.ToDecimal(priceFull);
                pw.OwnerId = RDN.Library.Classes.Account.User.GetMemberId();
                pw.AddPaywall(pw);
                return Redirect(Url.Content("~/paywall/all?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        public ActionResult EditPaywall(string paywallid)
        {
            try
            {
                Paywall pw = new Paywall();
                var wall = pw.GetOwnedPaywall(Convert.ToInt64(paywallid));
                if (wall == null)
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                return View(wall);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [Authorize]
        public ActionResult EditPaywall(Paywall pw)
        {
            try
            {
                string priceDaily = HttpContext.Request.Form["Price"];
                string priceFull = HttpContext.Request.Form["FullPrice"];
                if (!String.IsNullOrEmpty(priceDaily))
                    pw.DailyPrice = Convert.ToDecimal(priceDaily);
                if (!String.IsNullOrEmpty(priceFull))
                    pw.TimespanPrice = Convert.ToDecimal(priceFull);
                pw.OwnerId = RDN.Library.Classes.Account.User.GetMemberId();
                DateTime sd = new DateTime();

                bool successsd = DateTime.TryParse(pw.StartDateDisplay, out sd);
                if (successsd)
                    pw.StartDate = sd;
                else
                    pw.StartDate = null;

                DateTime ed = new DateTime();

                bool successed = DateTime.TryParse(pw.EndDateDisplay, out ed);
                if (successed)
                    pw.EndDate = ed;
                else
                    pw.EndDate = null;



                var wall = pw.UpdatePaywall(pw);

                if (wall == null)
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                return Redirect(Url.Content("~/paywall/all?u=" + SiteMessagesEnum.su));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [Authorize]
        public ActionResult Paywalls()
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string u = nameValueCollection["u"];
                if (u == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Added New Paywall.";
                    this.AddMessage(message);
                }
                if (u == SiteMessagesEnum.su.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Updated Paywall.";
                    this.AddMessage(message);
                }
                if (u == SiteMessagesEnum.sus.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Updated Settings.";
                    this.AddMessage(message);
                }
                //DisplayPaywall pw = new DisplayPaywall();
                Paywall pw = new Paywall();
                Guid ownerId = RDN.Library.Classes.Account.User.GetMemberId();
                DisplayPaywall dpw = pw.GetPaywalls(ownerId);
                dpw.InternalReference = ownerId;
                if (!dpw.AcceptPaymentsViaPaypal && !dpw.AcceptPaymentsViaStripe)
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Please setup a payment processor under Merchant Settings.";
                    this.AddMessage(message);
                }
                dpw.ReturnUrl = Url.Encode(Request.RawUrl);
                return View(dpw);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }


        [HttpPost]
        [Authorize]
        public ActionResult CreatePaywall(DisplayPaywall paywall)
        {
            try
            {
                var sg = new MerchantGateway();
                var merchant = sg.CreateMerchantAccount(paywall.InternalReference, MerchantInternalReference.Member);
                var id = RDN.Library.Classes.Account.User.GetMemberId();
                MemberCache.Clear(id);
                MemberCache.ClearApiCache(id);

                return Redirect(Url.Content("~/paywall/all"));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

    }
}
