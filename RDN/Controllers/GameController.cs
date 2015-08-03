using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.ViewModel;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Scoreboard.Library.ViewModel;
using Scoreboard.Library.Util;
using RDN.Utilities.Config;
using RDN.Library.Classes.Game;
using RDN.Library.Util.Enum;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment.Classes.Invoice;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Cache;
using RDN.Library.Classes.Payment.Enums.Paywall;
using RDN.Library.Classes.Payment;
using RDN.Models.OutModel;
using RDN.Library.Classes.Payment.Paywall;
using System.Collections.Specialized;
using RDN.Library.Util;
using RDN.Library.Cache.Singletons;
using RDN.Portable.Config;
using RDN.Portable.Classes.Payment.Enums;
using RDN.Library.Classes.Config;
using RDN.Portable.Classes.Url;

namespace RDN.Controllers
{
    public class GameController : BaseController
    {
        [HttpPost]
        public ActionResult MakePaywallPayment(GameOutModel crap)
        {
            var memId = RDN.Library.Classes.Account.User.GetMemberId();
            GameOutModel game = new GameOutModel();
            game.Game = GameServerViewModel.GetGameFromCache(crap.Game.GameId);
            Paywall wall = new Paywall();
            game.Paywall = wall.GetPaywall(game.Game.PaywallId);

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
                    validTill = game.Paywall.EndDate.Value.AddDays(1);
                }

                PaymentGateway pg = new PaymentGateway();
                var f = pg.StartInvoiceWizard().Initalize(game.Paywall.MerchantId, "USD", provider, LibraryConfig.IsProduction, ChargeTypeEnum.Paywall)
                    .SetInvoiceId(Guid.NewGuid())
                    .SetPaywall(new InvoicePaywall
                    {
                        ValidUntil = validTill,
                        PaywallId = game.Paywall.PaywallId,
                        PriceType = sub,
                        Description = "Paid For " + game.Game.GameName + " with a " + RDN.Utilities.Enums.EnumExt.ToFreindlyName(sub) + " Pass",
                        Name = "Game Streaming Paid",
                        Price = price,
                        InternalObject = game.Game.GameId,
                        MemberPaidId = memId,
                        PaywallLocation = LibraryConfig.PublicSite + "/" + RDN.Library.Classes.Config.LibraryConfig.SportNameForUrl + "-game/" + game.Game.GameId.ToString().Replace("-", "") + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(game.Game.GameName) + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(game.Game.Team1.TeamName) + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(game.Game.Team2.TeamName)
                    })
                    .SetInvoiceContactData(new InvoiceContactInfo
                    {
                        Email = crap.Paywall.EmailAddress,
                        Phone = crap.Paywall.PhoneNumber,
                    });

                if (provider == PaymentProvider.Stripe)
                    f.SetStripeTokenId(stripeToken);

                var response = f.FinalizeInvoice();

                //succesfully charged.
                if (response.Status == InvoiceStatus.Payment_Successful)
                    return Redirect(LibraryConfig.PublicSite + UrlManager.PAYWALL_RECEIPT_URL + response.InvoiceId.ToString().Replace("-", ""));
                else if (response.Status == InvoiceStatus.Pending_Payment_From_Paypal)
                    return Redirect(response.RedirectLink);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/" + RDN.Library.Classes.Config.LibraryConfig.SportNameForUrl + "-game/" + game.Game.GameId.ToString().Replace("-", "") + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(game.Game.GameName) + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(game.Game.Team1.TeamName) + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(game.Game.Team2.TeamName) + "?u=" + SiteMessagesEnum.sww));

        }

        public ActionResult CurrentGames()
        {
            try
            {
                var games = RDN.Library.Classes.Game.Game.GetCurrentGames();
                return Json(new { games = games }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { games = false }, JsonRequestBehavior.AllowGet);
        }

        // GET: /Game/
        public ActionResult Index(string id, string gameName, string team1, string team2)
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


                GameOutModel game = new GameOutModel();
                game.Game = GameServerViewModel.GetGameFromCache(new Guid(id));
                if (game.Game == null)
                    return Redirect(Url.Content("~/" + RDN.Library.Classes.Config.LibraryConfig.SportNameForUrl + "-games?u=" + SiteMessagesEnum.dex));

                Paywall wall = new Paywall();
                game.Paywall = wall.GetPaywall(game.Game.PaywallId);
                if (game.Paywall == null)
                    game.Paywall = new Paywall();

                ViewData["merchantId"] = game.Game.SelectedShop;
                if (!String.IsNullOrEmpty(game.Paywall.StripePublishableKey))
                    game.StripeKey = "Stripe.setPublishableKey('" + game.Paywall.StripePublishableKey + "');";
                else
                    game.StripeKey = "Stripe.setPublishableKey('" + LibraryConfig.StripeApiPublicKey + "');";
                if (game != null)
                    return View(game);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/" + RDN.Library.Classes.Config.LibraryConfig.SportNameForUrl + "-games?u=" + SiteMessagesEnum.dex));
        }
        [HttpPost]
        public ActionResult Index(GameOutModel crap)
        {
            try
            {
                if (!String.IsNullOrEmpty(crap.Paywall.PasswordForPaywall))
                    crap.Paywall.PasswordForPaywall = crap.Paywall.PasswordForPaywall.Trim();
                GameOutModel game = new GameOutModel();
                game.Game = GameServerViewModel.GetGameFromCache(crap.Game.GameId);
                Paywall wall = new Paywall();
                game.Paywall = wall.GetPaywall(game.Game.PaywallId);
                if (game.Paywall == null)
                    game.Paywall = new Paywall();

                bool isPaid = GameServerViewModel.CheckGamePaywallIsPaid(game.Game.GameId, crap.Paywall.PasswordForPaywall);
                game.Paywall.IsPaid = isPaid;
                if (PaywallViewers.Instance.Paywalls == null)
                    PaywallViewers.Instance.Paywalls = new List<Paywall>();
                bool isCurrentlyViewing = PaywallViewers.Instance.IsCurrentlyViewingPaywall(crap.Paywall.PaywallId, crap.Paywall.PasswordForPaywall);
                if (isCurrentlyViewing)
                {
                    game.Paywall.IsPaid = false;
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Warning;
                    message.Message = "You are currently viewing this from another device. You can only view this from one device at a time.";
                    this.AddMessage(message);
                }

                ViewData["merchantId"] = game.Game.SelectedShop;
                game.StripeKey = "Stripe.setPublishableKey('" + LibraryConfig.StripeApiPublicKey + "');";
                if (game != null)
                    return View(game);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        //replacing this method with an actual API call.
        public ActionResult GameApi(string id)
        {
            try
            {
                var game = GameServerViewModel.GetGameFromCacheApi(new Guid(id));
                game.IdForOnlineManagementUse = new Guid();

                // need to define the scores for the players.
                foreach (var jam in game.Jams)
                {
                    var scoresT1 = game.ScoresTeam1.Where(x => x.JamNumber == jam.JamNumber);
                    jam.TotalPointsForJamT1 = 0;
                    foreach (var score in scoresT1)
                        jam.TotalPointsForJamT1 += score.Points;

                    var scoresT2 = game.ScoresTeam2.Where(x => x.JamNumber == jam.JamNumber);
                    jam.TotalPointsForJamT2 = 0;
                    foreach (var score in scoresT2)
                        jam.TotalPointsForJamT2 += score.Points;

                }
                return Json(new { game = game });
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/" + RDN.Library.Classes.Config.LibraryConfig.SportNameForUrl + "-games"));
        }

        public ActionResult PicturesOfGame(string id)
        {
            try
            {
                var pictures = GameServerViewModel.GetPicturesOfGameFromDb(new Guid(id));
                return Json(new { pictures = pictures }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { pictures = false }, JsonRequestBehavior.AllowGet);
        }



    }

}
