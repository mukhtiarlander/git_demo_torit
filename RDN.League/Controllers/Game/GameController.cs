using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Game.Enums;
using RDN.League.Models.Game;
using System.Collections.ObjectModel;
using Scoreboard.Library.ViewModel;
using RDN.Library.ViewModel;
using Scoreboard.Library.Static.Enums;
using RDN.Library.Classes.Error;
using System.Threading;
using System.Threading.Tasks;
using RDN.Library.Classes.Federation;
using RDN.Library.Cache;
using RDN.Library.Classes.Game;
using RDN.League.Models.Filters;
using RDN.League.Models.Utilities;
using RDN.League.Models.OutModel;
using RDN.Utilities.Config;
using System.Net;
using System.IO;
using RDN.Utilities.Error;
using RDN.Library.Classes.Account.Classes;
using RDN.League.Models.Enum;
using RDN.Library.Util.Enum;
using Scoreboard.Library.ViewModel.Members;
using System.Collections.Specialized;
using RDN.Library.Util;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Paywall;
using RDN.Portable.Config;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Federation;
using RDN.Library.Classes.Config;
using RDN.Portable.Classes.Url;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class GameController : BaseController
    {
        private static readonly int DEFAULT_PAGE_SIZE = 100;

        [HttpPost]
        [Authorize]
        public ActionResult PublishGameOnline(Game game)
        {
            try
            {
                if (GameManager.PublishGameOnline(game.GameId, true))
                    return Redirect(Url.Content("~/game/manage/" + game.PrivateKeyForGame.ToString().Replace("-", "") + "/" + game.GameId.ToString().Replace("-", "")));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

        }
        [Authorize]
        public ActionResult Upload()
        {
            VerifyModel model = new VerifyModel();
            model.IsSuccessful = true;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false, IsManager = false)]
        public ActionResult Upload(VerifyModel model, HttpPostedFileBase file)
        {

            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(LibraryConfig.DataFolder + ServerManager.SAVE_OLD_GAMES_FOLDER);
                    if (!dir.Exists)
                        dir.Create();
                    string finalFile = Path.Combine(LibraryConfig.DataFolder + ServerManager.SAVE_OLD_GAMES_FOLDER, "CompletedGame" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".xml");
                    file.SaveAs(finalFile);
                    var game = GameViewModel.deserializeGame(finalFile);
                    if (game != null)
                    {
                        GameServerViewModel.saveGameToDb(game);
                        try
                        {
                            FileInfo f3 = new FileInfo(finalFile);
                            //I want to keep a few uploaded games.
                            //if (f3.Exists)
                            //    f3.Delete();
                        }
                        catch { }
                        return Redirect("~/game/manage/" + game.IdForOnlineManagementUse.ToString().Replace("-", "") + "/" + game.GameId.ToString().Replace("-", ""));
                    }
                    else
                    {
                        model.Message = "No " + RDN.Library.Classes.Config.LibraryConfig.GameName + " was uploaded.  Please make sure this file was generated from the " + RDN.Library.Classes.Config.LibraryConfig.WebsiteShortName+" Scoreboard.";
                        model.IsSuccessful = false;
                        return View(model);
                    }
                }
                catch (Exception e)
                {
                    Library.Classes.Error.ErrorDatabaseManager.AddException(e, e.GetType(), ErrorGroupEnum.Database);
                    string filePath = Path.Combine(LibraryConfig.DataFolder + ServerManager.SAVE_OLD_GAMES_FOLDER, Path.GetFileName(file.FileName));
                    DirectoryInfo dir = new DirectoryInfo(LibraryConfig.DataFolder + ServerManager.SAVE_OLD_GAMES_FOLDER);
                    if (!dir.Exists)
                        dir.Create();
                    file.SaveAs(filePath);
                }
            }
            model.Message = "Something happened with the intake of the game. Please try again later.";
            model.IsSuccessful = false;
            return View(model);
        }
        [Authorize]
        public ActionResult ViewGame(string id, string name)
        {
            var game = GameServerViewModel.getGameForRollerDerby(new Guid(id));
            try
            {
                if (game.Team1.TeamLinkId != new Guid())
                    game.Team1AttachedToGame = RDN.Library.Classes.League.LeagueFactory.GetLeagueInTeamViewModel(game.Team1.TeamLinkId);
                if (game.Team2.TeamLinkId != new Guid())
                    game.Team2AttachedToGame = RDN.Library.Classes.League.LeagueFactory.GetLeagueInTeamViewModel(game.Team2.TeamLinkId);
                return View(game);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
        }



        /// <summary>
        /// allows the user to add old games
        /// </summary>
        /// <param name="id">the id of the league or federation adding the old game</param>
        /// <param name="name">the type of entity adding.  league Or federation</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult AddOld(string name, string id, string g)
        {
            GameModel model = new GameModel();
            try
            {
                if (g == null)
                {
                    model.CreateNewGameForAddingToDb();
                    model.FederationId = new Guid(id);
                }
                else
                    model = (GameModel)GameServerViewModel.getGameBeingAdded(new Guid(g));

                if (name == GameAddedByEnum.federation.ToString())
                {
                    model.ListOfTeams = new SelectList(RDN.Library.Classes.League.LeagueFactory.GetLeaguesInFederation(new Guid(id)), "LeagueId", "Name");

                    model.TempTeam1Members = new ObservableCollection<TeamMembersViewModel>();
                    model.TempTeam2Members = new ObservableCollection<TeamMembersViewModel>();

                    //model.Jams.Add(new JamViewModel(0, 0, 0));
                    model.ScoresTeam1.Add(new ScoreViewModel(0, 0, new Guid(), 0, 0));
                    model.ScoresTeam2.Add(new ScoreViewModel(0, 0, new Guid(), 0, 0));

                    Task<bool>.Factory.StartNew(() =>
                    {
                        GameServerViewModel.addGameToBeingAddedList(model);
                        GameServerViewModel.saveGameToDb(model);

                        return true;
                    });
                }
                else if (name == GameAddedByEnum.league.ToString())
                { }
                return View(model);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
        }
        /// <summary>
        /// gets all the games the member owns..
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult ManageAll()
        {
            try
            {
                var memberId = RDN.Library.Classes.Account.User.GetMemberId();
                var member = MemberCache.GetMemberDisplay(memberId);

                ViewBag.Name = member.DerbyName;

                var model = new SimpleModPager<Library.Classes.Game.Game>();
                model.CurrentPage = 1;
                model.NumberOfRecords = GameManager.GetNumberOfGamesForMemberOwner(memberId);
                model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / DEFAULT_PAGE_SIZE);


                var output = FillGamesModelForOwner(model, memberId);
                return View(output);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
        }
        /// <summary>
        /// gets the game to allow the user to manage it.
        /// </summary>
        /// <param name="pid">theprivate game key</param>
        /// <param name="id">the public game key</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Manage(string pid, string id)
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string u = nameValueCollection["u"];
                if (u == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Updated Game.";
                    this.AddMessage(message);
                }
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var game = GameManager.GetGameForManagement(new Guid(id), new Guid(pid), memId);
                Paywall pw = new Paywall();
                var pws = pw.GetPaywalls(memId);
                game.Paywalls = pws.Paywalls;
                MerchantGateway mg = new MerchantGateway();
                game.AvailableShops = mg.GetPublicMerchants();



                ViewBag.PayWalls = new SelectList(game.Paywalls, "PaywallId", "DescriptionOfPaywall", game.SelectedPaywall);
                if (game != null)
                    return View(game);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
        }
        [ValidateInput(false)]
        [HttpPost]
        [Authorize]
        public ActionResult ManagePost(Game game)
        {
            try
            {
                var gameDb = GameManager.GetGameForManagement(game.GameId, game.PrivateKeyForGame, RDN.Library.Classes.Account.User.GetMemberId());
                if (game == null)
                    return Redirect(Url.Content("~/"));

                gameDb.GameName = game.GameName;
                gameDb.GameDate = game.GameDate;

                for (int i = 0; i < gameDb.MembersOfGame.Count; i++)
                {
                    string linkId = HttpContext.Request.Form[gameDb.MembersOfGame[i].MemberId.ToString().Replace("-", "") + "hidden"];
                    if (String.IsNullOrEmpty(linkId))
                        gameDb.MembersOfGame[i].MemberLinkId = new Guid();
                    else
                    {
                        if (new Guid(linkId) != gameDb.MembersOfGame[i].MemberLinkId)
                        {
                            gameDb.MembersOfGame[i].MemberLinkId = new Guid(linkId);
                        }
                    }
                }
                gameDb.SelectedShop = game.SelectedShop;
                gameDb.SelectedTournament = game.SelectedTournament;
                gameDb.PassCodeEnteredForTournament = game.PassCodeEnteredForTournament;

                for (int i = 0; i < gameDb.MemberOwners.Count + 5; i++)
                {
                    if (HttpContext.Request.Form["MemberOwner" + i + "hidden"] != null && !String.IsNullOrEmpty(HttpContext.Request.Form["MemberOwner" + i + "hidden"].ToString()))
                    {
                        MemberDisplayBasic mem = new MemberDisplayBasic();
                        mem.MemberId = new Guid(HttpContext.Request.Form["MemberOwner" + i + "hidden"].ToString());
                        gameDb.MemberOwners.Add(mem);
                    }
                    if (HttpContext.Request.Form["LeagueOwner" + i + "hidden"] != null && !String.IsNullOrEmpty(HttpContext.Request.Form["LeagueOwner" + i + "hidden"].ToString()))
                    {
                        RDN.Portable.Classes.League.Classes.League mem = new Portable.Classes.League.Classes.League();
                        mem.LeagueId = new Guid(HttpContext.Request.Form["LeagueOwner" + i + "hidden"].ToString());
                        gameDb.LeagueOwners.Add(mem);
                    }
                    if (HttpContext.Request.Form["FederationOwner" + i + "hidden"] != null && !String.IsNullOrEmpty(HttpContext.Request.Form["FederationOwner" + i + "hidden"].ToString()))
                    {
                        FederationDisplay mem = new FederationDisplay();
                        mem.FederationId = new Guid(HttpContext.Request.Form["FederationOwner" + i + "hidden"].ToString());
                        gameDb.FederationOwners.Add(mem);
                    }
                }
                if (!String.IsNullOrEmpty(game.EmbededVideoString))
                    gameDb.IsThereVideoOfGame = GameVideoTypeEnum.EmbededVideo;
                else if (!String.IsNullOrEmpty(game.StreamingUrlSilverlight))
                    gameDb.IsThereVideoOfGame = GameVideoTypeEnum.SilverLightLive;

                gameDb.StreamingMobileUrlSilverlight = game.StreamingMobileUrlSilverlight;
                gameDb.StreamingUrlSilverlight = game.StreamingUrlSilverlight;
                gameDb.EmbededVideoString = game.EmbededVideoString;
                gameDb.SelectedPaywall = game.SelectedPaywall;

                GameManager.UpdateGameFromManagement(gameDb);
                try
                {
                    //clears the live game once we complete the update.
                    WebClient client = new WebClient();
                    client.DownloadString(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_LIVE_GAME_CACHE + game.GameId.ToString());
                }
                catch { }
                try
                {
                    //clears the live game once we complete the update.
                    WebClient client = new WebClient();
                    client.DownloadString(LibraryConfig.ApiSite + UrlManager.WEBSITE_CLEAR_TOURNAMENT_CACHE_API);
                }
                catch { }
                return Redirect(Url.Content("~/game/manage/" + game.PrivateKeyForGame.ToString().Replace("-", "") + "/" + game.GameId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
        }

        /// <summary>
        /// allows the user to view all the games for their specific type.  We will use this for federations and leagues...
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        [Authorize]
        public ActionResult ViewAll(string type)
        {
            try
            {
                if (type == GameAddedByEnum.federation.ToString())
                {
                    var fed = MemberCache.GetAllOwnedFederations(RDN.Library.Classes.Account.User.GetMemberId()).FirstOrDefault().Federation;
                    ViewBag.Name = fed.Name;

                    var model = new SimpleModPager<Library.Classes.Game.Game>();
                    model.CurrentPage = 1;
                    model.NumberOfRecords = Game.GetNumberOfGamesForFederation(fed.FederationId);
                    model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / DEFAULT_PAGE_SIZE);

                    var output = FillGamesModelForFederation(model, fed.FederationId);
                    return View(output);
                }
                else if (type == GameAddedByEnum.league.ToString())
                {
                    var league = MemberCache.GetLeagueOfMember(RDN.Library.Classes.Account.User.GetMemberId());

                    ViewBag.Name = league.Name;

                    var model = new SimpleModPager<Library.Classes.Game.Game>();
                    model.CurrentPage = 1;
                    model.NumberOfRecords = Game.GetNumberOfGamesForLeague(league.LeagueId);
                    model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / DEFAULT_PAGE_SIZE);

                    var output = FillGamesModelForLeague(model, league.LeagueId);
                    return View(output);
                }
                else if (type == GameAddedByEnum.member.ToString())
                {
                    var memberId = RDN.Library.Classes.Account.User.GetMemberId();
                    var member = MemberCache.GetMemberDisplay(memberId);
                    if (member != null)
                        ViewBag.Name = member.DerbyName;

                    var model = new SimpleModPager<Library.Classes.Game.Game>();
                    model.CurrentPage = 1;
                    model.NumberOfRecords = Game.GetNumberOfGamesForPlayer(memberId);
                    model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / DEFAULT_PAGE_SIZE);

                    var output = FillGamesModelForPlayer(model, memberId);
                    return View(output);
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
        }
        /// <summary>
        /// the pager method to view all the games.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="federationId"></param>
        /// <returns></returns>
        private GenericSingleModel<SimpleModPager<Library.Classes.Game.Game>> FillGamesModelForFederation(SimpleModPager<Library.Classes.Game.Game> model, Guid federationId)
        {
            var output = new GenericSingleModel<SimpleModPager<Library.Classes.Game.Game>> { Model = model };

            try
            {
                for (var i = 1; i <= model.NumberOfPages; i++)
                    model.Pages.Add(new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString(),
                        Selected = i == model.CurrentPage
                    });

                output.Model.Items = Game.GetGamesOwnedByFederation(federationId, (model.CurrentPage - 1) * DEFAULT_PAGE_SIZE, DEFAULT_PAGE_SIZE);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return output;
        }
        private GenericSingleModel<SimpleModPager<Library.Classes.Game.Game>> FillGamesModelForLeague(SimpleModPager<Library.Classes.Game.Game> model, Guid leagueId)
        {
            var output = new GenericSingleModel<SimpleModPager<Library.Classes.Game.Game>> { Model = model };
            try
            {
                for (var i = 1; i <= model.NumberOfPages; i++)
                    model.Pages.Add(new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString(),
                        Selected = i == model.CurrentPage
                    });


                output.Model.Items = Game.GetGamesForLeague(leagueId, (model.CurrentPage - 1) * DEFAULT_PAGE_SIZE, DEFAULT_PAGE_SIZE);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }

            return output;
        }
        private GenericSingleModel<SimpleModPager<Library.Classes.Game.Game>> FillGamesModelForPlayer(SimpleModPager<Library.Classes.Game.Game> model, Guid memberId)
        {
            var output = new GenericSingleModel<SimpleModPager<Library.Classes.Game.Game>> { Model = model };

            try
            {
                for (var i = 1; i <= model.NumberOfPages; i++)
                    model.Pages.Add(new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString(),
                        Selected = i == model.CurrentPage
                    });

                output.Model.Items = Game.GetGamesPlayedByMember(memberId, (model.CurrentPage - 1) * DEFAULT_PAGE_SIZE, DEFAULT_PAGE_SIZE);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return output;
        }

        private GenericSingleModel<SimpleModPager<Library.Classes.Game.Game>> FillGamesModelForOwner(SimpleModPager<Library.Classes.Game.Game> model, Guid memberId)
        {
            var output = new GenericSingleModel<SimpleModPager<Library.Classes.Game.Game>> { Model = model };
            try
            {
                for (var i = 1; i <= model.NumberOfPages; i++)
                    model.Pages.Add(new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString(),
                        Selected = i == model.CurrentPage
                    });

                var fed = MemberCache.GetAllOwnedFederations(memberId);
                var leg = MemberCache.GetAllOwnedLeagues(memberId);
                output.Model.Items = GameManager.GetGamesOwnedByMember(memberId, (model.CurrentPage - 1) * DEFAULT_PAGE_SIZE, DEFAULT_PAGE_SIZE);
                if (output.Model.Items == null)
                    output.Model.Items = new List<Game>();
                if (leg.Count > 0)
                {
                    List<Guid> ids = new List<Guid>();
                    foreach (var le in leg)
                        ids.Add(le.LeagueId);
                    var games = GameManager.GetGamesOwnedByLeague(ids, (model.CurrentPage - 1) * DEFAULT_PAGE_SIZE, DEFAULT_PAGE_SIZE);

                    foreach (var game in games)
                    {
                        if (output.Model.Items.Where(x => x.GameId == game.GameId).FirstOrDefault() == null)
                            output.Model.Items.Add(game);
                    }
                }
                if (fed.Count > 0)
                {
                    List<Guid> ids = new List<Guid>();
                    foreach (var le in fed)
                        ids.Add(le.Federation.FederationId);
                    var games = GameManager.GetGamesOwnedByFederation(ids, (model.CurrentPage - 1) * DEFAULT_PAGE_SIZE, DEFAULT_PAGE_SIZE);
                    foreach (var game in games)
                    {
                        if (output.Model.Items.Where(x => x.GameId == game.GameId).FirstOrDefault() == null)
                            output.Model.Items.Add(game);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }

            return output;
        }

        /// <summary>
        /// updates any posted details of the game.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult AddOld_UpdateDetails()
        {
            try
            {
                Guid gameId = new Guid(HttpContext.Request.Form["GameId"].ToString());
                GameViewModel game = GameServerViewModel.getGameBeingAdded(gameId);

                if (HttpContext.Request.Form["GameName"] != null)
                {
                    game.GameName = HttpContext.Request.Form["GameName"].ToString();
                }
                if (HttpContext.Request.Form["GameDate"] != null)
                {
                    game.GameDate = DateTime.Parse(HttpContext.Request.Form["GameDate"].ToString());
                    game.GameEndDate = game.GameDate;
                }
                if (HttpContext.Request.Form["Finalize"] != null)
                {
                    //we finalize the game this way.
                    game.ScoreboardMode = ScoreboardModeEnum.AddedOldGame;
                    Task<bool>.Factory.StartNew(() =>
                    {
                        GameServerViewModel.addGameToBeingAddedList(game);
                        GameServerViewModel.saveGameToDb(game);
                        return true;
                    });
                    Response.Redirect(Url.Content("~/Game/" + game.GameId.ToString().Replace("-", "") + "/" + game.GameName));
                }
                Task<bool>.Factory.StartNew(() =>
                {
                    GameServerViewModel.addGameToBeingAddedList(game);
                    GameServerViewModel.saveGameToDb(game);
                    return true;
                });
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { result = true });
        }
        [Authorize]
        public ActionResult AddOld_LinksToGame()
        {
            try
            {
                Guid gameId = new Guid(HttpContext.Request.Form["GameId"].ToString());
                GameViewModel game = GameServerViewModel.getGameBeingAdded(gameId);

                for (int i = 0; i < game.GameLinks.Count; i++)
                {
                    var link = HttpContext.Request.Form["Link[" + game.GameLinks[i].LinkId + "]"].ToString();
                    var linkId = HttpContext.Request.Form["LinkType[" + game.GameLinks[i].LinkId + "]"].ToString();

                    var oldLink = game.GameLinks.Where(x => x.LinkId == game.GameLinks[i].LinkId).FirstOrDefault();
                    oldLink.GameLink = link;
                    oldLink.LinkType = (GameLinkTypeEnum)Enum.ToObject(typeof(GameLinkTypeEnum), Convert.ToInt32(linkId));
                }
                GameLinkViewModel gameLink = new GameLinkViewModel();
                gameLink.GameLink = LibraryConfig.PublicSite;
                gameLink.LinkId = Guid.NewGuid();

                game.GameLinks.Add(gameLink);

                Task<bool>.Factory.StartNew(() =>
                          {
                              try
                              {
                                  GameServerViewModel.addGameToBeingAddedList(game);
                                  GameServerViewModel.saveGameToDb(game);
                              }
                              catch (Exception exception)
                              {
                                  ErrorDatabaseManager.AddException(exception, GetType());
                              }

                              return true;
                          });



                return PartialView("AddOldLinksPartial", (GameViewModel)game);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
        }
        [Authorize]
        public ActionResult AddOld_AddPenaltiesToGame()
        {
            try
            {

                Guid gameId = new Guid(HttpContext.Request.Form["GameId"].ToString());
                GameViewModel game = GameServerViewModel.getGameBeingAdded(gameId);

                if (HttpContext.Request.Form["savePenalties"] != null)
                {
                    //first scan all the team members.
                    for (int i = 0; i < game.Team1.TeamMembers.Count; i++)
                    {
                        //since we put 5 dropdowns out, we also have to collect all 5 dropdowns back, so we iterate through those dropdowns.
                        for (int j = 0; j < 5; j++)
                        {
                            //finding the penalty selected for the dropdown.
                            string item = HttpContext.Request.Form["Team1." + j + "[" + game.Team1.TeamMembers[i].SkaterId + "]"].ToString();
                            //if a penalty actually is selected.
                            if (!String.IsNullOrEmpty(item))
                            {
                                int penalty = Convert.ToInt32(item);
                                //find the penalty in the box if it exists already.  If it doesn't we will add it.
                                var penaltyModel = game.PenaltyBox.Where(x => x.PenaltyNumberForSkater == j).Where(x => x.PlayerSentToBox.SkaterId == game.Team1.TeamMembers[i].SkaterId).FirstOrDefault();
                                //if it does exist, we make any updates to the selected penalty.
                                if (penaltyModel != null)
                                {
                                    if (penaltyModel.PenaltyType != (PenaltiesEnum)Enum.ToObject(typeof(PenaltiesEnum), penalty))
                                    {
                                        if (penalty == Convert.ToInt32(PenaltiesMADEEnum.Excessive_Force) || penalty == Convert.ToInt32(PenaltiesMADEEnum.Expulsion) || penalty == Convert.ToInt32(PenaltiesMADEEnum.Hitting) || penalty == Convert.ToInt32(PenaltiesMADEEnum.Team_Penalty))
                                            penaltyModel.PenaltyScale = PenaltyScaleEnum.Major;
                                        else
                                            penaltyModel.PenaltyScale = PenaltyScaleEnum.Minor;
                                        penaltyModel.PenaltyType = (PenaltiesEnum)Enum.ToObject(typeof(PenaltiesEnum), penalty);
                                    }
                                }
                                else
                                {//if it doesn't exist, we go ahead and add it.
                                    SkaterInPenaltyBoxViewModel skater = new SkaterInPenaltyBoxViewModel();
                                    //we declare the penalty number and stick it in the DB because just in case the user saves the penalties twice
                                    //we want to define what the penalties were from the last save and saving the
                                    //integer of the penalties works just about right for that.
                                    skater.PenaltyNumberForSkater = j;
                                    skater.PenaltyId = Guid.NewGuid();
                                    if (penalty == Convert.ToInt32(PenaltiesMADEEnum.Excessive_Force) || penalty == Convert.ToInt32(PenaltiesMADEEnum.Expulsion) || penalty == Convert.ToInt32(PenaltiesMADEEnum.Hitting) || penalty == Convert.ToInt32(PenaltiesMADEEnum.Team_Penalty))
                                        skater.PenaltyScale = PenaltyScaleEnum.Major;
                                    else
                                        skater.PenaltyScale = PenaltyScaleEnum.Minor;
                                    skater.PenaltyType = (PenaltiesEnum)Enum.ToObject(typeof(PenaltiesEnum), penalty);
                                    skater.PlayerSentToBox = game.Team1.TeamMembers[i];
                                    game.PenaltyBox.Add(skater);
                                }
                            }
                        }
                    }

                    for (int i = 0; i < game.Team2.TeamMembers.Count; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            string item = HttpContext.Request.Form["Team2." + j + "[" + game.Team2.TeamMembers[i].SkaterId + "]"].ToString();
                            if (!String.IsNullOrEmpty(item))
                            {
                                int penalty = Convert.ToInt32(item);
                                var penaltyModel = game.PenaltyBox.Where(x => x.PenaltyNumberForSkater == j).Where(x => x.PlayerSentToBox.SkaterId == game.Team2.TeamMembers[i].SkaterId).FirstOrDefault();

                                if (penaltyModel != null)
                                {
                                    if (penaltyModel.PenaltyType != (PenaltiesEnum)Enum.ToObject(typeof(PenaltiesEnum), penalty))
                                    {
                                        if (penalty == Convert.ToInt32(PenaltiesMADEEnum.Excessive_Force) || penalty == Convert.ToInt32(PenaltiesMADEEnum.Expulsion) || penalty == Convert.ToInt32(PenaltiesMADEEnum.Hitting) || penalty == Convert.ToInt32(PenaltiesMADEEnum.Team_Penalty))
                                            penaltyModel.PenaltyScale = PenaltyScaleEnum.Major;
                                        else
                                            penaltyModel.PenaltyScale = PenaltyScaleEnum.Minor;
                                        penaltyModel.PenaltyType = (PenaltiesEnum)Enum.ToObject(typeof(PenaltiesEnum), penalty);
                                    }
                                }
                                else
                                {
                                    SkaterInPenaltyBoxViewModel skater = new SkaterInPenaltyBoxViewModel();
                                    skater.PenaltyNumberForSkater = j;
                                    skater.PenaltyId = Guid.NewGuid();
                                    if (penalty == Convert.ToInt32(PenaltiesMADEEnum.Excessive_Force) || penalty == Convert.ToInt32(PenaltiesMADEEnum.Expulsion) || penalty == Convert.ToInt32(PenaltiesMADEEnum.Hitting) || penalty == Convert.ToInt32(PenaltiesMADEEnum.Team_Penalty))
                                        skater.PenaltyScale = PenaltyScaleEnum.Major;
                                    else
                                        skater.PenaltyScale = PenaltyScaleEnum.Minor;
                                    skater.PenaltyType = (PenaltiesEnum)Enum.ToObject(typeof(PenaltiesEnum), penalty);
                                    skater.PlayerSentToBox = game.Team2.TeamMembers[i];
                                    game.PenaltyBox.Add(skater);
                                }
                            }
                        }
                    }
                    Task<bool>.Factory.StartNew(() =>
                    {
                        GameServerViewModel.addGameToBeingAddedList(game);
                        GameServerViewModel.saveGameToDb(game);
                        return true;
                    });
                }


                return PartialView("PenaltiesStatsMADEPartial", (GameViewModel)game);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

        }

        /// <summary>
        /// adds a new jam to the game.  But cycles through all the old jams to make sure they are up to date with what the user entered.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult AddOld_AddJamToGame()
        {
            try
            {
                Guid gameId = new Guid(HttpContext.Request.Form["GameId"].ToString());

                GameViewModel game = GameServerViewModel.getGameBeingAdded(gameId);

                for (int i = 0; i < game.Jams.Count; i++)
                {
                    //fills out the scores for the jam.
                    int scoreT1 = Convert.ToInt32(HttpContext.Request.Form["ScoresTeam1[" + i + "].Points"].ToString());
                    int scoreT2 = Convert.ToInt32(HttpContext.Request.Form["ScoresTeam2[" + i + "].Points"].ToString());
                    var jamNumber = game.Jams[i].JamNumber;
                    var scoreT1DB = game.ScoresTeam1.Where(x => x.JamNumber == jamNumber).FirstOrDefault();
                    var scoreT2DB = game.ScoresTeam2.Where(x => x.JamNumber == jamNumber).FirstOrDefault();
                    scoreT1DB.Points = scoreT1;
                    scoreT2DB.Points = scoreT2;

                    //fills out the teams for the jam.
                    string b1t2 = HttpContext.Request.Form["Jams[" + i + "].Blocker1T2"].ToString();
                    if (!String.IsNullOrEmpty(b1t2))
                        game.Jams[i].Blocker1T2 = game.Team2.TeamMembers.Where(x => x.SkaterId == new Guid(b1t2)).FirstOrDefault();
                    string b2t2 = HttpContext.Request.Form["Jams[" + i + "].Blocker2T2"].ToString();
                    if (!String.IsNullOrEmpty(b2t2))
                        game.Jams[i].Blocker2T2 = game.Team2.TeamMembers.Where(x => x.SkaterId == new Guid(b2t2)).FirstOrDefault();
                    string b3t2 = HttpContext.Request.Form["Jams[" + i + "].Blocker3T2"].ToString();
                    if (!String.IsNullOrEmpty(b3t2))
                        game.Jams[i].Blocker3T2 = game.Team2.TeamMembers.Where(x => x.SkaterId == new Guid(b3t2)).FirstOrDefault();
                    string pt2 = HttpContext.Request.Form["Jams[" + i + "].PivotT2"].ToString();
                    if (!String.IsNullOrEmpty(pt2))
                        game.Jams[i].PivotT2 = game.Team2.TeamMembers.Where(x => x.SkaterId == new Guid(pt2)).FirstOrDefault();
                    string jt2 = HttpContext.Request.Form["Jams[" + i + "].JammerT2"].ToString();
                    if (!String.IsNullOrEmpty(jt2))
                        game.Jams[i].JammerT2 = game.Team2.TeamMembers.Where(x => x.SkaterId == new Guid(jt2)).FirstOrDefault();
                    string b1t1 = HttpContext.Request.Form["Jams[" + i + "].Blocker1T1"].ToString();
                    if (!String.IsNullOrEmpty(b1t1))
                        game.Jams[i].Blocker1T1 = game.Team1.TeamMembers.Where(x => x.SkaterId == new Guid(b1t1)).FirstOrDefault();
                    string b2t1 = HttpContext.Request.Form["Jams[" + i + "].Blocker2T1"].ToString();
                    if (!String.IsNullOrEmpty(b2t1))
                        game.Jams[i].Blocker2T1 = game.Team1.TeamMembers.Where(x => x.SkaterId == new Guid(b2t1)).FirstOrDefault();
                    string b3t1 = HttpContext.Request.Form["Jams[" + i + "].Blocker3T1"].ToString();
                    if (!String.IsNullOrEmpty(b3t1))
                        game.Jams[i].Blocker3T1 = game.Team1.TeamMembers.Where(x => x.SkaterId == new Guid(b3t1)).FirstOrDefault();
                    string pt1 = HttpContext.Request.Form["Jams[" + i + "].PivotT1"].ToString();
                    if (!String.IsNullOrEmpty(pt1))
                        game.Jams[i].PivotT1 = game.Team1.TeamMembers.Where(x => x.SkaterId == new Guid(pt1)).FirstOrDefault();
                    string jt1 = HttpContext.Request.Form["Jams[" + i + "].JammerT1"].ToString();
                    if (!String.IsNullOrEmpty(jt1))
                        game.Jams[i].JammerT1 = game.Team1.TeamMembers.Where(x => x.SkaterId == new Guid(jt1)).FirstOrDefault();
                }

                //must add team scores before adding jam or the jams will be a bit off.
                game.ScoresTeam1.Add(new ScoreViewModel(0, 0, new Guid(), game.Jams.Count, 0));
                game.ScoresTeam2.Add(new ScoreViewModel(0, 0, new Guid(), game.Jams.Count, 0));
                game.Jams.Add(new JamViewModel(game.Jams.Count, 0, 0));

                Task<bool>.Factory.StartNew(
                            () =>
                            {
                                try
                                {
                                    GameServerViewModel.addGameToBeingAddedList(game);
                                    GameServerViewModel.saveGameToDb(game);
                                }
                                catch (Exception exception)
                                {
                                    ErrorDatabaseManager.AddException(exception, GetType());
                                }

                                return true;
                            });

                return PartialView("JamsListPartial", (GameViewModel)game);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
        }

        /// <summary>
        /// gets list of members in the league.
        /// </summary>
        /// <param name="leagueId"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult AddOld_GetListLeagueMembers()
        {
            try
            {
                ObservableCollection<TeamMembersViewModel> members = null;

                Guid gameId = new Guid(HttpContext.Request.Form["gameId"].ToString());

                //grabbing the game so we can set the league ids for the game.
                GameViewModel game = GameServerViewModel.getGameBeingAdded(gameId);

                if (game.Team1 == null)
                    game.Team1 = new TeamViewModel();
                if (game.Team2 == null)
                    game.Team2 = new TeamViewModel();

                if (HttpContext.Request.Form["Team1.TeamId"] != null)
                {
                    Guid teamId = new Guid(HttpContext.Request.Form["Team1.TeamId"].ToString());
                    members = RDN.Library.Classes.League.LeagueFactory.GetLeagueMembersForGame(teamId);
                    game.Team1.TeamLinkId = teamId;
                    ViewBag.LeagueId = game.Team1.TeamId;
                }
                else if (HttpContext.Request.Form["Team2.TeamId"] != null)
                {
                    Guid teamId = new Guid(HttpContext.Request.Form["Team2.TeamId"].ToString());
                    members = RDN.Library.Classes.League.LeagueFactory.GetLeagueMembersForGame(teamId);
                    game.Team2.TeamLinkId = teamId;
                    ViewBag.LeagueId = game.Team2.TeamId;
                }
                Task<bool>.Factory.StartNew(
                       () =>
                       {
                           GameServerViewModel.addGameToBeingAddedList(game);
                           GameServerViewModel.saveGameToDb(game);

                           return true;
                       });

                ViewBag.GameId = game.GameId;


                return PartialView("MemberListTempPartial", members);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View();
        }

        /// <summary>
        /// adds a player from the league to the list of players in the game.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult AddOld_AddSkaterToPlayedTeam()
        {
            try
            {
                Guid gameId = new Guid(HttpContext.Request.QueryString["GameId"].ToString());
                Guid leagueId = new Guid(HttpContext.Request.QueryString["LeagueId"].ToString());
                Guid skaterId = new Guid(HttpContext.Request.QueryString["SkaterId"].ToString());
                ViewBag.GameId = gameId;

                GameViewModel game = (GameViewModel)GameServerViewModel.getGameBeingAdded(gameId);
                if (game.Team1.TeamId == leagueId)
                {
                    if (game.Team1.TeamMembers == null)
                        game.Team1.TeamMembers = new ObservableCollection<TeamMembersViewModel>();

                    var skater = RDN.Library.Classes.Account.User.GetMemberWithMemberId(skaterId);
                    game.Team1.TeamMembers.Add(new TeamMembersViewModel { SkaterId = Guid.NewGuid(), SkaterLinkId = skater.MemberId, SkaterName = skater.DerbyName, SkaterNumber = skater.PlayerNumber });
                    Task<bool>.Factory.StartNew(
                            () =>
                            {
                                GameServerViewModel.saveGameToDb(game);
                                return true;
                            });
                    ViewBag.LeagueId = game.Team1.TeamId;
                    return PartialView("MemberListPlayedPartial", game.Team1.TeamMembers);
                }
                else if (game.Team2.TeamId == leagueId)
                {
                    if (game.Team2.TeamMembers == null)
                        game.Team2.TeamMembers = new ObservableCollection<TeamMembersViewModel>();

                    var skater = RDN.Library.Classes.Account.User.GetMemberWithMemberId(skaterId);
                    game.Team2.TeamMembers.Add(new TeamMembersViewModel { SkaterId = Guid.NewGuid(), SkaterLinkId = skater.MemberId, SkaterName = skater.DerbyName, SkaterNumber = skater.PlayerNumber });
                    Task<bool>.Factory.StartNew(
                                           () =>
                                           {
                                               GameServerViewModel.addGameToBeingAddedList(game);
                                               GameServerViewModel.saveGameToDb(game);
                                               return true;
                                           });
                    ViewBag.LeagueId = game.Team2.TeamId;
                    return PartialView("MemberListPlayedPartial", game.Team2.TeamMembers);
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
        }
        [Authorize]
        public ActionResult AddOld_RemoveSkaterFromPlayedTeam()
        {
            try
            {
                Guid gameId = new Guid(HttpContext.Request.QueryString["GameId"].ToString());
                Guid leagueId = new Guid(HttpContext.Request.QueryString["LeagueId"].ToString());
                Guid skaterId = new Guid(HttpContext.Request.QueryString["SkaterId"].ToString());

                GameViewModel game = (GameViewModel)GameServerViewModel.getGameBeingAdded(gameId);
                if (game.Team1.TeamId == leagueId)
                {
                    game.Team1.TeamMembers.Remove(game.Team1.TeamMembers.Where(x => x.SkaterId == skaterId).FirstOrDefault());
                    Task<bool>.Factory.StartNew(
                             () =>
                             {
                                 GameServerViewModel.addGameToBeingAddedList(game);
                                 GameServerViewModel.RemoveSkaterFromGame(leagueId, skaterId);
                                 return true;
                             });
                    return PartialView("MemberListPlayedPartial", game.Team1.TeamMembers);
                }
                else if (game.Team2.TeamId == leagueId)
                {
                    game.Team2.TeamMembers.Remove(game.Team2.TeamMembers.Where(x => x.SkaterLinkId == skaterId).FirstOrDefault());
                    Task<bool>.Factory.StartNew(
                            () =>
                            {
                                GameServerViewModel.addGameToBeingAddedList(game);
                                GameServerViewModel.RemoveSkaterFromGame(leagueId, skaterId);
                                return true;
                            });
                    return PartialView("MemberListPlayedPartial", game.Team2.TeamMembers);
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
        }

        [Authorize]
        public ActionResult PublishGame(string gId, string pId, string isPub)
        {
            try
            {
                bool succ = GameManager.PublishGameOnline(new Guid(gId), Convert.ToBoolean(isPub));
                return Json(new { isSuccess = succ }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        [Authorize]
        public ActionResult GetTeamsOfGame(string gameId)
        {
            try
            {
                var teams = GameManager.GetTeamsOfGame(new Guid(gameId));

                return Json(new { isSuccess = true, teams = teams }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);

        }
        [ValidateInput(false)]
        [Authorize]
        public ActionResult GetTeamScoreOfGame(string gameId, string teamId)
        {
            try
            {
                var id = new Guid(teamId);
                var teams = GameManager.GetTeamsOfGame(new Guid(gameId));
                var team = teams.Where(x => x.TeamId == id).FirstOrDefault();
                return Json(new { isSuccess = true, score = team.Score }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);

        }
    }
}
