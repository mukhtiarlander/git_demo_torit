using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Classes.Display;
using RDN.Library.Classes.Payment.Enums.Stripe;
using RDN.Library.Util;
using RDN.Library.Util.Enum;
using RDN.Utilities.Config;
using RDN.Portable.Config;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class MerchantController : BaseController
    {
        [Authorize]
        public ActionResult MerchantSettings()
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sced.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Error;
                    message.Message = "You did not Connect to Stripe. In order to Accept Customer Credit Cards, you should connect to Stripe.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sca.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Stripe Connected Successfully.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sus.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Updated Settings Successfully.";
                    this.AddMessage(message);
                }

                MerchantGateway mg = new MerchantGateway();
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var merchant = mg.GetMerchantSettings(memId);
                string stripe = "https://connect.stripe.com/oauth/authorize?response_type=code&client_id=" + LibraryConfig.STRIPE_CONNECT_LIVE_KEY + "&scope=read_write&state=" + StripeStateReturnCodeEnum.merchant + "-" + merchant.PrivateManagerId.ToString().Replace("-", "");
                
                string u = nameValueCollection["return"];
                if (!String.IsNullOrEmpty(u))
                {
                    merchant.ReturnUrl = u;
                }

                ViewBag.StripeUrl = stripe;
                return View(merchant);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [Authorize]
        public ActionResult MerchantSettings(OverviewMerchant merchant)
        {
            try
            {
                MerchantGateway mg = new MerchantGateway();
                mg.UpdateMerchantSettings(merchant);
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.UrlReferrer.Query);
                string u = nameValueCollection["return"];
                if (!String.IsNullOrEmpty(u))
                {
                    return Redirect(u + "?u=" + SiteMessagesEnum.sus);
                }
                return Redirect(Url.Content("~/merchant/settings?u=" + SiteMessagesEnum.sus));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

    }
}
