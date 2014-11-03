using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Cache;
using RDN.Library.Cache.Singletons;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Game;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Classes.Invoice;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Payment.Enums.Paywall;
using RDN.Library.Util;
using RDN.Library.Util.Enum;
using RDN.Utilities.Config;
using RDN.Portable.Config;
using System.IO;
using RDN.Portable.Classes.Payment.Enums;

namespace RDN.Controllers
{

    public class TournamentController : BaseController
    {
        public ActionResult RenderTournament(Guid id)
        {
            try
            {
                var image = Tournament.RenderTournamentBrackets(SiteCache.GetTournament(id), false);

                MemoryStream ms = new MemoryStream();

                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                return File(ms.ToArray(), "image/png");
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public ActionResult RenderPerformanceTournament(Guid id)
        {
            try
            {
                var image = Tournament.RenderTournamentBrackets(SiteCache.GetTournament(id), true);

                MemoryStream ms = new MemoryStream();

                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                return File(ms.ToArray(), "image/png");
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        [HttpPost]
        public ActionResult MakePaywallPayment(Tournament tourny)
        {
            var memId = RDN.Library.Classes.Account.User.GetMemberId();
            var to = Tournament.GetPublicTournament(tourny.Id);
            try
            {
                string paymentProvider = Request.Form["PaymentType"].ToString();
                string stripeToken = "none";

                PaymentProvider provider = PaymentProvider.None;
                if (paymentProvider == PaymentProvider.Stripe.ToString())
                {
                    provider = PaymentProvider.Stripe;
                    stripeToken = Request.Form["stripeToken"].ToString();
                }
                else if (paymentProvider == PaymentProvider.Paypal.ToString())
                {
                    provider = PaymentProvider.Paypal;
                }
                string subType = Request.Form["PaymentCost"].ToString();
                PaywallPriceTypeEnum sub = (PaywallPriceTypeEnum)Enum.Parse(typeof(PaywallPriceTypeEnum), subType);
                decimal price = .99M;


                DateTime now = DateTime.UtcNow;
                DateTime validTill = DateTime.UtcNow.AddDays(1);

                if (sub == PaywallPriceTypeEnum.Daily_Payment)
                {
                    validTill = DateTime.UtcNow.AddDays(1);
                }
                else if (sub == PaywallPriceTypeEnum.Full_Payment)
                {
                    validTill = to.Paywall.EndDate.Value.AddDays(1);
                }

                PaymentGateway pg = new PaymentGateway();
                var f = pg.StartInvoiceWizard().Initalize(to.Paywall.MerchantId, "USD", provider, PaymentMode.Live, ChargeTypeEnum.Paywall)
                    .SetInvoiceId(Guid.NewGuid())
                    .SetPaywall(new InvoicePaywall
                    {
                        ValidUntil = validTill,
                        PaywallId = to.Paywall.PaywallId,

                        PriceType = sub,
                        Description = "Paid For " + to.Name + " with a " + RDN.Utilities.Enums.EnumExt.ToFreindlyName(sub) + " Pass",
                        Name = "Tournament Streaming Paid",
                        Price = price,
                        InternalObject = to.Id,
                        MemberPaidId = memId,
                        PaywallLocation = ServerConfig.WEBSITE_DEFAULT_LOCATION + "/roller-derby-tournament/" + to.Id.ToString().Replace("-", "") + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(to.Name)
                    })
                    .SetInvoiceContactData(new InvoiceContactInfo
                    {
                        Email = tourny.Paywall.EmailAddress,
                        Phone = tourny.Paywall.PhoneNumber,
                    });

                if (provider == PaymentProvider.Stripe)
                    f.SetStripeTokenId(stripeToken);

                var response = f.FinalizeInvoice();

                //succesfully charged.
                if (response.Status == InvoiceStatus.Payment_Successful)
                    return Redirect(ServerConfig.PAYWALL_RECEIPT_URL + response.InvoiceId.ToString().Replace("-", ""));
                else if (response.Status == InvoiceStatus.Pending_Payment_From_Paypal)
                    return Redirect(response.RedirectLink);


            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/roller-derby-tournament/" + to.Id.ToString().Replace("-", "") + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(to.Name) + "?u=" + SiteMessagesEnum.sww));
        }


        public ActionResult TournamentHome(string id, string name)
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string u = nameValueCollection["u"];
                if (u == SiteMessagesEnum.sww.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Error;
                    message.Message = "Something Went Wrong, Problem Sent to the Developers.";
                    this.AddMessage(message);
                }
                var tourny = Tournament.GetPublicTournament(new Guid(id));

                if (!String.IsNullOrEmpty(tourny.Paywall.StripePublishableKey))
                    tourny.StripeKey = "Stripe.setPublishableKey('" + tourny.Paywall.StripePublishableKey + "');";
                else
                    tourny.StripeKey = "Stripe.setPublishableKey('" + ServerConfig.STRIPE_LIVE_KEY + "');";
                ViewData["merchantId"] = tourny.SelectedShop;
                tourny.PensAbbre = (from Scoreboard.Library.Static.Enums.PenaltiesEnum d in Enum.GetValues(typeof(Scoreboard.Library.Static.Enums.PenaltiesEnum))
                                    select new { ID = (int)d, Name = RDN.Utilities.Enums.EnumExt.ToFreindlyName(d), Abbre = Scoreboard.Library.ViewModel.PenaltyViewModel.ToAbbreviation(d) });


                return View(tourny);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        public ActionResult TournamentHome(Tournament tourny)
        {
            try
            {
                if (!String.IsNullOrEmpty(tourny.Paywall.PasswordForPaywall))
                    tourny.Paywall.PasswordForPaywall = tourny.Paywall.PasswordForPaywall.Trim();
                var tourn = Tournament.GetPublicTournament(tourny.Id);
                bool isPaid = Tournament.CheckTournamentPaywallIsPaid(tourny.Id, tourny.Paywall.PasswordForPaywall);
                tourn.Paywall.IsPaid = isPaid;

                bool isCurrentlyViewing = PaywallViewers.Instance.IsCurrentlyViewingPaywall(tourny.Paywall.PaywallId, tourny.Paywall.PasswordForPaywall);
                if (isCurrentlyViewing)
                {
                    tourn.Paywall.IsPaid = false;
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Warning;
                    message.Message = "You are currently viewing this from another device. You can only view this from one device at a time.";
                    this.AddMessage(message);
                }


                tourny.StripeKey = "Stripe.setPublishableKey('" + ServerConfig.STRIPE_LIVE_KEY + "');";
                return View(tourn);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        public ActionResult Tournaments()
        {
            return View(SiteCache.GetTournaments());
        }

    }
}
