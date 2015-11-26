using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.League.Models.Billing;
using RDN.Library.Classes.Billing.Classes;
using RDN.Library.Classes.Billing.Enums;
using RDN.Library.Classes.Location;
using RDN.League.Classes.Enums;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Payment.Classes.Invoice;
using RDN.Library.Classes.Error;
using RDN.Utilities.Config;
using RDN.Library.Cache;
using RDN.League.Models.Filters;
using RDN.League.Models.Enum;
using System.Collections.Specialized;
using RDN.Library.Util.Enum;
using RDN.Library.Util;
using RDN.Portable.Config;
using RDN.Library.Cache.Singletons;
using RDN.Portable.Classes.Payment.Enums;
using RDN.Library.Classes.Config;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class BillingController : BaseController
    {
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult LeagueBilling(string leagueId)
        {
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
            string updated = nameValueCollection["u"];

            if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.pp.ToString())
            {
                SiteMessage message = new SiteMessage();
                message.MessageType = SiteMessageType.Error;
                message.Message = "Those features require a subscription to " + LibraryConfig.WebsiteShortName + ". Please subscribe to Enable those features.";
                this.AddMessage(message);
            }
            else if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sc.ToString())
            {
                SiteMessage message = new SiteMessage();
                message.MessageType = SiteMessageType.Info;
                message.Message = "Your subscription has successfully been canceled.";
                this.AddMessage(message);
            }
            var bi = RDN.Library.Classes.Billing.Classes.LeagueBilling.GetCurrentBillingStatus(new Guid(leagueId));

            return View(bi);
        }


        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult LeagueAddSubscription(string leagueId)
        {


            AddSubscriptionModel add = new AddSubscriptionModel();
            try
            {
                add.AddSubscriptionOwnerId = new Guid(leagueId);
                var league = RDN.Library.Classes.League.LeagueFactory.GetLeague(new Guid(leagueId));
                if (league != null)
                    add.AddSubscriptionOwnerName = league.Name;
                Dictionary<int, string> countries = LocationFactory.GetCountriesDictionary();
                add.Countries = countries.Select(item => new SelectListItem { Text = item.Value, Value = item.Key.ToString() }).ToList();

                List<SelectListItem> years = new List<SelectListItem>();
                for (int i = 0; i < 10; i++)
                {
                    var item = new SelectListItem();
                    item.Value = DateTime.Now.AddYears(i).Year.ToString();
                    item.Text = DateTime.Now.AddYears(i).Year.ToString();
                    years.Add(item);
                }
                add.Years = years;
                add.Months = EnumExt.ToSelectListIdAndName(MonthsEnum.Jan);

                add.StripeKey = "Stripe.setPublishableKey('" + LibraryConfig.StripeApiPublicKey + "');";

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(add);
        }
        [Authorize]
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult LeagueAddSubscription(AddSubscriptionModel add)
        {
            var memId = RDN.Library.Classes.Account.User.GetMemberId();

            try
            {
                PaymentGateway pg = new PaymentGateway();



                string paymentProvider = Request.Form["PaymentType"].ToString();
                string articleNumberForSubscription = "none";

                PaymentProvider provider = PaymentProvider.None;
                if (paymentProvider == PaymentProvider.Stripe.ToString())
                {
                    provider = PaymentProvider.Stripe;
                    articleNumberForSubscription = Request.Form["stripeToken"].ToString();
                }
                else if (paymentProvider == PaymentProvider.Paypal.ToString())
                {
                    provider = PaymentProvider.Paypal;
                }
                string subType = Request.Form["SubscriptionType"].ToString();
                LeaguePlanTypesEnum sub = (LeaguePlanTypesEnum)Enum.Parse(typeof(LeaguePlanTypesEnum), subType);
                decimal price = 15.99M;
                SubscriptionPeriodStripe subStripe = SubscriptionPeriodStripe.Monthly;
                int lengthOfDays = 31;
                DateTime now = DateTime.UtcNow;
                TimeSpan ts = new TimeSpan();
                if (sub == LeaguePlanTypesEnum.One_Month)
                {
                    price = 15.99M;
                    subStripe = SubscriptionPeriodStripe.Monthly;
                    ts = now.AddMonths(1) - DateTime.UtcNow;
                    lengthOfDays = ts.Days;
                }
                else if (sub == LeaguePlanTypesEnum.Six_Months)
                {
                    price = 71.99M;
                    subStripe = SubscriptionPeriodStripe.Six_Months;
                    ts = now.AddMonths(6) - DateTime.UtcNow;
                    lengthOfDays = ts.Days;
                }
                else if (sub == LeaguePlanTypesEnum.Three_Months)
                {
                    price = 41.99M;
                    subStripe = SubscriptionPeriodStripe.Three_Months;
                    ts = now.AddMonths(3) - DateTime.UtcNow;
                    lengthOfDays = ts.Days;
                }
                else if (sub == LeaguePlanTypesEnum.Twelve_Months)
                {
                    price = 131.99M;
                    subStripe = SubscriptionPeriodStripe.Yearly;
                    ts = now.AddMonths(12) - DateTime.UtcNow;
                    lengthOfDays = ts.Days;
                }
                try
                {
                    var bi = RDN.Library.Classes.Billing.Classes.LeagueBilling.GetCurrentBillingStatus(add.AddSubscriptionOwnerId);

                    if (bi != null && bi.InvoiceId != Guid.Empty)
                    {
                        var result = pg.StartInvoiceWizard()
                                .Initalize(LibraryConfig.SiteStoreID, "USD", provider, LibraryConfig.IsProduction, ChargeTypeEnum.Cancel_Subscription)
                                .SetInvoiceId(bi.InvoiceId)
                                .FinalizeInvoice();
                    }
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }

                var f = pg.StartInvoiceWizard().Initalize(LibraryConfig.SiteStoreID, "USD", provider, LibraryConfig.IsProduction, ChargeTypeEnum.Subscription)
                    .SetInvoiceId(Guid.NewGuid())
                    .SetSubscription(new InvoiceSubscription
                    {
                        ArticleNumber = articleNumberForSubscription,
                        Description = LibraryConfig.WebsiteShortName + " League portal subscription",
                        DescriptionRecurring = "Fee for " + LibraryConfig.WebsiteShortName + " League portal subscription",
                        Name = LibraryConfig.WebsiteShortName + " Member portal",
                        NameRecurring = LibraryConfig.WebsiteShortName + " Member portal recurring",
                        DigitalPurchaseText = "You have now access to " + LibraryConfig.WebsiteShortName + " League portal",
                        Price = price,
                        SubscriptionPeriodStripe = subStripe,
                        SubscriptionPeriodLengthInDays = lengthOfDays,
                        //ValidUntil = subScriptionDate.AddDays(lengthOfDays),
                        //league id is the ownerId
                        InternalObject = add.AddSubscriptionOwnerId
                    })
                    .SetInvoiceContactData(new InvoiceContactInfo
                    {
                        City = add.City,
                        Country = add.Country,
                        Email = add.EmailAddress,
                        FirstName = add.FirstName,
                        LastName = add.LastName,
                        Phone = add.PhoneNumber,
                        State = add.State,
                        Street = add.Address,
                        Zip = add.ZipCode
                    })
                        .FinalizeInvoice();

                //succesfully charged.

                //clears the member cache to update all cache repos of the league members.
                MemberCache.ClearLeagueMembersCache(add.AddSubscriptionOwnerId);
                MemberCache.ClearLeagueMembersApiCache(add.AddSubscriptionOwnerId);

                if (f.Status == InvoiceStatus.Subscription_Running)
                    return Redirect(Url.Content("~/billing/league/receipt/" + f.InvoiceId.ToString().Replace("-", "")));
                else if (f.Status == InvoiceStatus.Pending_Payment_From_Paypal)
                    return Redirect(f.RedirectLink);
                else if (f.Status == InvoiceStatus.Card_Was_Declined)
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Error;
                    message.Message = "Your Card has been Declined.  Please check the information and try again.";
                    this.AddMessage(message);
                }


            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            add.AddSubscriptionOwnerId = add.AddSubscriptionOwnerId;
            var league = RDN.Library.Classes.League.LeagueFactory.GetLeague(add.AddSubscriptionOwnerId);
            if (league != null)
                add.AddSubscriptionOwnerName = league.Name;
            Dictionary<int, string> countries = LocationFactory.GetCountriesDictionary();
            add.Countries = countries.Select(item => new SelectListItem { Text = item.Value, Value = item.Key.ToString() }).ToList();
            //add.Years = EnumExt.ToSelectListId(YearsEnum.Fourteen);
            List<SelectListItem> years = new List<SelectListItem>();
            for (int i = 0; i < 10; i++)
            {
                var item = new SelectListItem();
                item.Value = DateTime.Now.AddYears(i).Year.ToString();
                item.Text = DateTime.Now.AddYears(i).Year.ToString();
                years.Add(item);
            }
            add.Years = years;
            add.Months = EnumExt.ToSelectListIdAndName(MonthsEnum.Jan);
            //#if DEBUG
            //                add.StripeKey = "Stripe.setPublishableKey('" + LibraryConfig.STRIPE_DEBUG_KEY + "');";
            //#else
            add.StripeKey = "Stripe.setPublishableKey('" + LibraryConfig.StripeApiPublicKey + "');";
            //#endif
            return View(add);
        }
        [Authorize]
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult LeagueCancelSubscription(LeagueBilling add)
        {
            var memId = RDN.Library.Classes.Account.User.GetMemberId();

            try
            {
                var bi = RDN.Library.Classes.Billing.Classes.LeagueBilling.GetCurrentBillingStatus(add.LeagueId);

                PaymentGateway pg = new PaymentGateway();
                var f = pg.StartInvoiceWizard().Initalize(LibraryConfig.SiteStoreID, "USD", PaymentProvider.Stripe, LibraryConfig.IsProduction, ChargeTypeEnum.Cancel_Subscription)
                   .SetInvoiceId(bi.InvoiceId)
                        .FinalizeInvoice();

                //clears the member cache to update all cache repos of the league members.
                MemberCache.ClearLeagueMembersCache(add.LeagueId);
                MemberCache.ClearLeagueMembersApiCache(add.LeagueId);
                //succesfully charged.
                if (f.Status == InvoiceStatus.Cancelled)
                    return Redirect(Url.Content("~/billing/league/" + add.LeagueId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.sc));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult LeagueReceipt(string invoiceId)
        {
            RDN.Library.Classes.Billing.Classes.LeagueReceipt receipt = new LeagueReceipt();
            try
            {
                receipt = RDN.Library.Classes.Billing.Classes.LeagueReceipt.GetReceiptForInvoice(new Guid(invoiceId));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(receipt);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult LeagueBillingHistory(string leagueId)
        {
            var memId = RDN.Library.Classes.Account.User.GetMemberId();
            if (!MemberCache.IsManagerOrBetterOfLeague(memId) && !MemberCache.IsAdministrator(memId))
                return Redirect("~/");

            RDN.Library.Classes.Billing.Classes.LeagueBillingHistory history = new LeagueBillingHistory();
            try
            {
                history = RDN.Library.Classes.Billing.Classes.LeagueBillingHistory.GetReceiptsForLeague(new Guid(leagueId));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(history);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult LeagueBillingUpdateInfo(string leagueId)
        {
            //CC#
            // CC Info.
            // Cancel subscription.


            var bi = RDN.Library.Classes.Billing.Classes.LeagueBilling.GetCurrentBillingStatus(new Guid(leagueId));

            bi.StripeKey = "Stripe.setPublishableKey('" + LibraryConfig.StripeApiPublicKey + "');";


            return View(bi);
        }
    }
}
