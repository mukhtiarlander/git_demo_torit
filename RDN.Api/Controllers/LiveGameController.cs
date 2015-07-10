using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.ViewModel;
using RDN.Api.Mvc;
using Scoreboard.Library.Util;
using RDN.Utilities.Config;
using RDN.Library.Classes.Error;
using RDN.Library.Cache;
using RDN.Portable.Models.Json.Games;
using RDN.Library.Classes.Game;
using RDN.Portable.Config;
using RDN.Library.Classes.Config;
using RDN.Portable.Classes.Url;

namespace RDN.Api.Controllers
{
    public class LiveGameController : Controller
    {

        public JsonResult Tournaments()
        {
            try
            {
                List<TournamentJson> js = new List<TournamentJson>();
                var tourns = SiteCache.GetTournaments();
                for (int i = 0; i < tourns.Count; i++)
                {
                    js.Add(new TournamentJson(tourns[i]));
                }
                return Json(new { Tournaments = js }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Tournament(string id)
        {
            try
            {
                Guid gameId = new Guid();
                if (Guid.TryParse(id, out gameId))
                {
                    var tourn = SiteCache.GetTournament(gameId);
                    tourn.RenderUrl =  LibraryConfig.ApiSite + UrlManager.WEBSITE_TOURNAMENT_RENDER_URL + tourn.Id.ToString().Replace("-", "");
                    tourn.RenderPerformanceUrl =LibraryConfig.ApiSite+ UrlManager.WEBSITE_TOURNAMENT_RENDER_PERFORMANCE_URL + tourn.Id.ToString().Replace("-", "");
                    return Json(new { Tournament = new TournamentJson(tourn) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Index()
        {


            return Json(new { Working = true });
        }
        /// <summary>
        /// this is the game that gets pulled from the api to show off the main game page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Game(string id)
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
                return new JsonpResult(new { game = game });
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GameJsonMin(string id)
        {
            try
            {
                Guid gameId = new Guid();
                if (Guid.TryParse(id, out gameId))
                {
                    var game = GameServerViewModel.GetGameFromCacheApi(gameId);

                    CurrentGameJson g = new CurrentGameJson();

                    g.StartTime = game.GameDate;
                    if (game.CurrentJam != null)
                        g.JamNumber = game.CurrentJam.JamNumber;
                    g.PeriodNumber = game.CurrentPeriod;
                    g.GameId = game.GameId;
                    g.GameName = game.GameName;
                    g.Team2Name = game.Team2.TeamName;
                    g.Team1Name = game.Team1.TeamName;
                    g.Team1Score = game.CurrentTeam1Score;
                    g.Team2Score = game.CurrentTeam2Score;
                    g.GameUrl = LibraryConfig.PublicSite + UrlManager.PublicSite_FOR_PAST_GAMES + "/" + g.GameId + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(g.GameName) + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(g.Team1Name) + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(g.Team2Name);
                    g.HasGameEnded = game.HasGameEnded;
                    if (!g.HasGameEnded)
                    {
                        if (game.CurrentJam != null && game.CurrentJam.JamClock != null)
                            g.JamTimeLeft = game.CurrentJam.JamClock.TimeRemaining;
                        if (game.PeriodClock != null)
                            g.PeriodTimeLeft = game.PeriodClock.TimeRemaining;
                    }
                    g.RuleSet = game.Policy.GameSelectionType.ToString();
                    if (game.Team1 != null && game.Team1.Logo != null)
                        g.Team1LogoUrl = game.Team1.Logo.ImageUrlThumb;
                    if (game.Team2 != null && game.Team2.Logo != null)
                        g.Team2LogoUrl = game.Team2.Logo.ImageUrlThumb;

                    if (!String.IsNullOrEmpty(game.EmbededVideoHtml))
                        g.IsLiveStreaming = true;

                    return Json(new { game = g }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { result = "Error, Sent to Developers." }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GameJson(string id)
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
                return Json(new { game = game }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Gamexml(string id, string k)
        {
            try
            {
                if (k == ScoreboardConfig.KEY_FOR_UPLOAD)
                {
                    var game = GameServerViewModel.GetGameFromCacheApi(new Guid(id));
                    return Content(Xml.SerializeObject(game));
                }
                else
                {
                    return Json(new { result = "Key does not match." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult CurrentGames()
        {
            try
            {
                var games = RDN.Library.Classes.Game.Game.GetCurrentGames();
                return new JsonpResult(new { games = games });
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());
                return new JsonpResult(new { games = "" });
            }
        }
        public ActionResult CurrentGamesMobile()
        {
            try
            {
                var games = RDN.Library.Classes.Game.Game.GetCurrentGamesMobile();
                return Json(games, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());

            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PastGames(string p, string c)
        {
            try
            {
                GamesJson gj = new GamesJson();

                gj.Games = SiteCache.GetPastGames(Convert.ToInt32(p), Convert.ToInt32(c));
                gj.Count = gj.Games.Count;
                gj.Page = Convert.ToInt32(p);
                return Json(gj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
    }
}
