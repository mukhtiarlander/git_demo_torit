using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.League.Models.Game;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Game;
using RDN.Library.Classes.Game.Enums;
using RDN.Library.Classes.Location;
using RDN.Library.Classes.Payment;
using RDN.Library.Util;
using RDN.Library.Util.Enum;
using RDN.League.Classes.Enums;
using RDN.Library.Classes.Payment.Paywall;
using System.Web.Script.Serialization;
using System.IO;
using System.Net;
using RDN.Portable.Config;
using RDN.Library.Classes.Account.Classes;
using RDN.Portable.Classes.Account.Classes;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class TournamentController : BaseController
    {
        //
        // GET: /Tournament/
        [Authorize]
        public ActionResult CreateTournament()
        {
            RegisterTournamentModel model = new RegisterTournamentModel();
            model.StartDate = DateTime.UtcNow;
            model.EndDate = DateTime.UtcNow;

            return View(model);
        }
        [Authorize]
        [HttpPost]
        public ActionResult CreateTournament(RegisterTournamentModel model)
        {
            var tourny = Tournament.CreateTournament(model.TournamentName, model.StartDate, model.EndDate, model.Passcode, RDN.Library.Classes.Account.User.GetMemberId());
            return Redirect(Url.Content("~/tournament/view/" + tourny.PrivateKey.ToString().Replace("-", "") + "/" + tourny.Id.ToString().Replace("-", "")));
        }

        [Authorize]
        public ActionResult ViewTournaments()
        {
            try
            {
                return View(Tournament.GetAllOwnedTournaments(RDN.Library.Classes.Account.User.GetMemberId()));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        public ActionResult ViewTournament(string pid, string id)
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string u = nameValueCollection["u"];
                if (u == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Updated Tournament.";
                    this.AddMessage(message);
                }
                if (u == SiteMessagesEnum.sup.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Updated Logo.";
                    this.AddMessage(message);
                }
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                var tourny = Tournament.GetTournamentToManage(new Guid(id), new Guid(pid));
                Paywall pw = new Paywall();
                var pws = pw.GetPaywalls(memId);
                Dictionary<int, string> countries = LocationFactory.GetCountriesDictionary();
                var counTemp = countries.Select(item => new SelectListItem { Text = item.Value, Value = item.Key.ToString() }).ToList();
                //ViewData["TournamentClass"] = tourny.TournamentClass.ToSelectListDropDown();
                //var list =    Enum.GetValues(typeof(TournamentTypeEnum)).Cast<TournamentTypeEnum>().ToList();
                MerchantGateway mg = new MerchantGateway();
                tourny.AvailableShops = mg.GetPublicMerchants();
                ViewBag.Countries = new SelectList(counTemp, "Value", "Text", tourny.CountryId);
                ViewBag.PayWalls = new SelectList(pws.Paywalls, "PaywallId", "DescriptionOfPaywall", tourny.PaywallId);
                return View(tourny);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        public ActionResult TournamentOwners(string pid, string id)
        {
            try
            {

                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                var tourny = Tournament.GetTournamentToManage(new Guid(id), new Guid(pid));
                if (tourny.Owners.Count < 10)
                {
                    MemberDisplayBasic mem = new MemberDisplayBasic();
                    tourny.Owners.Add(mem);
                    MemberDisplayBasic mem1 = new MemberDisplayBasic();
                    tourny.Owners.Add(mem1);
                    MemberDisplayBasic mem2 = new MemberDisplayBasic();
                    tourny.Owners.Add(mem2);
                    MemberDisplayBasic mem3 = new MemberDisplayBasic();
                    tourny.Owners.Add(mem3);
                }
                return View(tourny);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [Authorize]
        public ActionResult TournamentOwners(Tournament tournament)
        {
            try
            {

                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                var tourny = Tournament.GetTournamentToManage(tournament.Id, tournament.PrivateKey);

                for (int i = 0; i < tourny.Owners.Count + 10; i++)
                {
                    if (HttpContext.Request.Form["MemberOwner" + i + "hidden"] != null && !String.IsNullOrEmpty(HttpContext.Request.Form["MemberOwner" + i + "hidden"].ToString()))
                    {
                        MemberDisplayBasic mem = new MemberDisplayBasic();
                        mem.MemberId = new Guid(HttpContext.Request.Form["MemberOwner" + i + "hidden"].ToString());
                        tourny.Owners.Add(mem);
                    }

                }
                Tournament.UpdateTournamentOwners(tourny);
                return View(tourny);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [HttpPost]
        [Authorize]
        public ActionResult UploadItemPictures(Tournament display)
        {
            try
            {
                foreach (string pictureFile in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[pictureFile];
                    if (file.ContentLength > 0)
                    {
                        Tournament.AddLogo(display.Id, file.InputStream, file.FileName);
                    }
                }
                return Redirect(Url.Content("~/tournament/view/" + display.PrivateKey.ToString().Replace("-", "") + "/" + display.Id.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.sup));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/u=" + SiteMessagesEnum.sww));

        }
        [ValidateInput(false)]
        [HttpPost]
        [Authorize]
        public ActionResult UpdateTournament(Tournament tourny)
        {
            try
            {
                tourny.EndDate = Convert.ToDateTime(tourny.EndDateDisplay);
                tourny.StartDate = Convert.ToDateTime(tourny.StartDateDisplay);

                Tournament.UpdateTournament(tourny);
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE));
                WebClient client1 = new WebClient();
                client1.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE_API));
                return Redirect(Url.Content("~/tournament/view/" + tourny.PrivateKey.ToString().Replace("-", "") + "/" + tourny.Id.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }
        #region Brackets
        [ValidateInput(false)]
        [Authorize]
        public ActionResult SaveTeamsToTournament(string tournId, string teams)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var list = js.Deserialize<List<string>>(teams);

                Tournament.AddTeamsToTournament(new Guid(tournId), list);
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE));
                WebClient client1 = new WebClient();
                client1.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE_API));
                return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);

        }
        [ValidateInput(false)]
        [Authorize]
        public ActionResult SaveTeamToTournament(string tournId, string team, string rating, string pool)
        {
            try
            {
                if (!String.IsNullOrEmpty(team))
                {
                    Guid teamId = Tournament.AddTeamToTournament(new Guid(tournId), team, Convert.ToInt32(rating), Convert.ToInt32(pool));
                    WebClient client = new WebClient();
                    client.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE));
                    WebClient client1 = new WebClient();
                    client1.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE_API));
                    if (teamId != new Guid())
                        return Json(new { isSuccess = true, id = teamId }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult RenderTournament(Guid id)
        {
            try
            {
                var image = Tournament.RenderTournamentBrackets(id, false);

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
                var image = Tournament.RenderTournamentBrackets(id, true);

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

        [ValidateInput(false)]
        [Authorize]
        public ActionResult StartNextPerformanceRound(string tournId)
        {
            try
            {

                bool succ = Tournament.StartNextRound(new Guid(tournId), true);
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE));
                WebClient client1 = new WebClient();
                client1.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE_API));
                return Json(new { isSuccess = succ }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);

        }
        [ValidateInput(false)]
        [Authorize]
        public ActionResult RollBackPerformanceRound(string tournId)
        {
            try
            {

                bool succ = Tournament.RollBackRound(new Guid(tournId), true);
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE));
                WebClient client1 = new WebClient();
                client1.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE_API));
                return Json(new { isSuccess = succ }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);

        }
        [ValidateInput(false)]
        [Authorize]
        public ActionResult StartNextRound(string tournId)
        {
            try
            {

                bool succ = Tournament.StartNextRound(new Guid(tournId), false);
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE));
                WebClient client1 = new WebClient();
                client1.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE_API));
                return Json(new { isSuccess = succ }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);

        }
        [ValidateInput(false)]
        [Authorize]
        public ActionResult RollBackRound(string tournId)
        {
            try
            {

                bool succ = Tournament.RollBackRound(new Guid(tournId), false);
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE));
                WebClient client1 = new WebClient();
                client1.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE_API));
                return Json(new { isSuccess = succ }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);

        }
        [ValidateInput(false)]
        [Authorize]
        public ActionResult SavePairingOfTournament(string tournId, string pairingId, string gameId, string team1Id, string team2Id, string team1Score, string team2Score, string trackNumber, string trackTime)
        {
            try
            {
                Guid gId = new Guid();
                Guid.TryParse(gameId, out gId);

                Guid t1Id = new Guid();
                Guid.TryParse(team1Id, out t1Id);

                Guid t2Id = new Guid();
                Guid.TryParse(team2Id, out t2Id);

                int t1S = 0;
                Int32.TryParse(team1Score, out t1S);

                int t2S = 0;
                Int32.TryParse(team2Score, out t2S);

                DateTime tt = new DateTime();
                DateTime.TryParse(trackTime, out tt);

                bool succ = Tournament.SavePairing(new Guid(tournId), Convert.ToInt64(pairingId), gId, t1Id, t2Id, t1S, t2S, trackNumber, tt);
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE));
                WebClient client1 = new WebClient();
                client1.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE_API));
                return Json(new { isSuccess = succ }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);

        }

        [ValidateInput(false)]
        [Authorize]
        public ActionResult PublishTournament(string tournId)
        {
            try
            {

                bool succ = Tournament.PublishTournament(new Guid(tournId));
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE));
                WebClient client1 = new WebClient();
                client1.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE_API));
                return Json(new { isSuccess = succ }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);

        }
        [ValidateInput(false)]
        [Authorize]
        public ActionResult PublishTournamentBrackets(string tournId)
        {
            try
            {
                bool succ = Tournament.PublishTournamentBrackets(new Guid(tournId));
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE));
                WebClient client1 = new WebClient();
                client1.DownloadStringAsync(new Uri(ServerConfig.WEBSITE_CLEAR_TOURNAMENT_CACHE_API));
                return Json(new { isSuccess = succ }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);

        }

        [ValidateInput(false)]
        [Authorize]
        public ActionResult RemoveTeamFromTournament(string tournId, string teamId)
        {
            try
            {

                var success = Tournament.RemoveTeamFromTournament(new Guid(tournId), new Guid(teamId));
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);

        }

        [Authorize]
        public ActionResult BracketsForTournament(string pid, string id)
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string u = nameValueCollection["u"];
                if (u == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Updated Tournament.";
                    this.AddMessage(message);
                }
                if (u == SiteMessagesEnum.sup.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Updated Logo.";
                    this.AddMessage(message);
                }
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                var tourny = Tournament.GetTournamentToManage(new Guid(id), new Guid(pid));



                return View(tourny);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }


        #endregion

    }
}
