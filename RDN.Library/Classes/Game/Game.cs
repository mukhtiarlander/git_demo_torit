using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using RDN.Utilities.Config;
using RDN.Library.DataModels;
using RDN.Utilities.Error;
using RDN.Library.DataModels.Game;
using RDN.Library.DataModels.Context;
using RDN.Library.Classes.Error;
using Scoreboard.Library.ViewModel;
using Scoreboard.Library.StopWatch;
using RDN.Library.Cache;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Paywall;
using RDN.Library.Classes.Store.Display;
using RDN.Library.Classes.Payment.Classes.Display;
using RDN.Portable.Config;
using RDN.Portable.Models.Json.Games;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Federation;
using RDN.Library.Classes.Config;
using RDN.Portable.Classes.Url;

namespace RDN.Library.Classes.Game
{


    public class Game
    {
        private static readonly int MAX_GAMES_TO_TAKE = 8;

        public string GameName { get; set; }
        public DateTime GameDate { get; set; }
        public Guid GameId { get; set; }
        public Guid PrivateKeyForGame { get; set; }
        public string Team1Name { get; set; }
        public Guid Team1Id { get; set; }
        public Guid Team1LinkId { get; set; }
        public IEnumerable<GameScore> Team1Score { get; set; }
        public int Team1ScoreTotal { get; set; }
        public string Team2Name { get; set; }
        public int Team2ScoreTotal { get; set; }
        public IEnumerable<GameScore> Team2Score { get; set; }
        public Guid Team2Id { get; set; }
        public Guid Team2LinkId { get; set; }
        public bool HasGameStarted { get; set; }
        public bool IsGameLive { get; set; }
        public bool IsGameOver { get; set; }
        public bool IsGamePublishedOnline { get; set; }
        public bool IsGameScrimmage { get; set; }
        public string VersionNumber { get; set; }
        public List<MemberDisplayGame> MembersOfGame { get; set; }

        public GameVideoTypeEnum IsThereVideoOfGame { get; set; }
        public string EmbededVideoString { get; set; }
        public string StreamingUrlSilverlight { get; set; }
        public string StreamingMobileUrlSilverlight { get; set; }
        public long SelectedPaywall { get; set; }
        public List<Paywall> Paywalls { get; set; }

        public List<Tournament> AvailableTournaments { get; set; }
        public List<MemberDisplayBasic> MemberOwners { get; set; }
        public List<RDN.Portable.Classes.League.Classes.League> LeagueOwners { get; set; }
        public List<FederationDisplay> FederationOwners { get; set; }


        /// <summary>
        /// selected tournament for publishing game.
        /// </summary>
        public string SelectedTournament { get; set; }
        public string PassCodeEnteredForTournament { get; set; }

        public List<OverviewMerchant> AvailableShops { get; set; }
        public string SelectedShop { get; set; }

        /// <summary>
        /// the difference in percentage between the team1 score and the team 2 score.
        /// </summary>
        public double Score1Score2Delta { get; set; }

        public Game()
        {
            MembersOfGame = new List<MemberDisplayGame>();
            AvailableTournaments = new List<Tournament>();
            MemberOwners = new List<MemberDisplayBasic>();
            LeagueOwners = new List<RDN.Portable.Classes.League.Classes.League>();
            FederationOwners = new List<FederationDisplay>();
        }


        /// <summary>
        /// gets the past 7 days of games.
        /// </summary>
        /// <returns></returns>
        public static List<CurrentGameJson> GetPastWeeksGames()
        {
            try
            {
                var dc = new ManagementContext();
                DateTime hours = DateTime.UtcNow.AddDays(-30);
                DateTime endDate = DateTime.UtcNow.AddDays(-30);
                var games = (from xx in dc.Games
                             where xx.IsGamePublishedOnline == true
                             where xx.HasGameStarted == true
                             where xx.GameEndDate > endDate
                             where xx.LastModified.Value > hours
                             select new CurrentGameJson
                             {
                                 StartTime = xx.GameDate,
                                 JamNumber = xx.GameJams.OrderByDescending(x => x.JamNumber).FirstOrDefault().JamNumber,
                                 PeriodNumber = xx.GameJams.OrderByDescending(x => x.JamNumber).FirstOrDefault().CurrentPeriod,
                                 GameId = xx.GameId,
                                 RuleSet = xx.GameType,
                                 GameName = xx.GameName,
                                 Team2Name = (dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault() == null) ? xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamName : dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault().Name,

                                 Team1Name = (dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault() == null) ? xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamName : dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault().Name,

                                 Team1Score = xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().CurrentScore,
                                 Team2Score = xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().CurrentScore,
                                 Team1LogoUrl = xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().Logo == null ? "" : xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().Logo.ImageUrl,
                                 Team2LogoUrl = xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().Logo == null ? "" : xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().Logo.ImageUrl
                             }).OrderByDescending(x => x.StartTime).ToList();

                return games;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return new List<CurrentGameJson>();
        }
        [Obsolete("use GameFactory")]
        public static List<CurrentGameJson> GetPastWeeksGames(int count, int page)
        {
            try
            {
                var dc = new ManagementContext();
                var games = (from xx in dc.Games
                             where xx.IsGamePublishedOnline == true
                             where xx.HasGameStarted == true
                             //where xx.GameEndDate > endDate
                             //where xx.LastModified.Value > hours
                             select new CurrentGameJson
                             {
                                 HasGameEnded = xx.IsGameOver,
                                 StartTime = xx.GameDate,
                                 JamNumber = xx.GameJams.OrderByDescending(x => x.JamNumber).FirstOrDefault().JamNumber,
                                 PeriodNumber = xx.GameJams.OrderByDescending(x => x.JamNumber).FirstOrDefault().CurrentPeriod,
                                 GameId = xx.GameId,
                                 RuleSet = xx.GameType,
                                 GameName = xx.GameName,
                                 Team2Name = (dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault() == null) ? xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamName : dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault().Name,

                                 Team1Name = (dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault() == null) ? xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamName : dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault().Name,

                                 Team1Score = xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().CurrentScore,
                                 Team2Score = xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().CurrentScore,
                                 Team1LogoUrl = xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().Logo == null ? "" : xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().Logo.ImageUrlThumb,
                                 Team2LogoUrl = xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().Logo == null ? "" : xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().Logo.ImageUrlThumb
                             }).OrderByDescending(x => x.StartTime).Skip(count * page).Take(count).ToList();
                return games;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return new List<CurrentGameJson>();
        }
        [Obsolete("use GameFactory")]
        public static List<CurrentGameJson> GetPastWeeksGamesFromRN(int count, int page)
        {
            try
            {
                var dc = new ManagementContext();
                DateTime hours = DateTime.UtcNow.AddDays(-16);
                DateTime endDate = DateTime.UtcNow.AddDays(-16);
                var games = (from xx in dc.RNScores
                             select new CurrentGameJson
                             {
                                 Team1Name = xx.Team1Name,
                                 Team1Score = xx.Team1Score,
                                 Team2Name = xx.Team2Name,
                                 Team2Score = xx.Team2Score,
                                 StartTime = xx.GameDateTime.HasValue ? xx.GameDateTime.Value : new DateTime()
                             }).OrderByDescending(x => x.StartTime).Skip(count * page).Take(count).ToList();
                return games;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return new List<CurrentGameJson>();
        }

        public static List<CurrentGameJson> GetCurrentGames()
        {

            var gs = GameCache.GetCurrentLiveGames();
            List<CurrentGameJson> games = new List<CurrentGameJson>();
            foreach (var game in gs)
            {
                if (game.GameIsConfirmedOnline && game.HasGameStarted)
                {
                    CurrentGameJson g = new CurrentGameJson();
                    g.StartTime = game.GameDate;
                    g.JamNumber = game.CurrentJam.JamNumber;
                    g.PeriodNumber = game.CurrentPeriod;
                    g.GameId = game.GameId;
                    g.GameName = game.GameName;
                    g.Team2Name = game.Team2.TeamName;
                    g.Team1Name = game.Team1.TeamName;
                    g.Team1Score = game.CurrentTeam1Score;
                    g.Team2Score = game.CurrentTeam2Score;
                    if (!String.IsNullOrEmpty(game.EmbededVideoHtml))
                        g.IsLiveStreaming = true;
                    games.Add(g);
                }
            }

            return games.OrderByDescending(x => x.StartTime).Take(8).ToList();
        }
        public static GamesJson GetCurrentGamesMobile()
        {
            var gs = GameCache.GetCurrentLiveGames();
            GamesJson gj = new GamesJson();
            gj.Games = new List<CurrentGameJson>();
            gj.Count = gs.Count;
            foreach (var game in gs)
            {
                if (game.GameIsConfirmedOnline && game.HasGameStarted)
                {
                    CurrentGameJson g = new CurrentGameJson();
                    g.StartTime = game.GameDate;
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
                    gj.Games.Add(g);
                }
            }

            return gj;
        }

        /// <summary>
        /// gets the current live games of the day.
        /// </summary>
        /// <returns></returns>
        private static List<CurrentGameJson> GetCurrentGamesDb()
        {


            try
            {
                var dc = new ManagementContext();
                DateTime hours = DateTime.UtcNow.AddHours(-24);
                DateTime endDate = DateTime.UtcNow.AddHours(-5);
                var games = (from xx in dc.Games
                             where xx.IsGamePublishedOnline == true
                             where xx.HasGameStarted == true
                             where xx.GameEndDate > endDate
                             where xx.LastModified.Value > hours
                             select new CurrentGameJson
                             {
                                 StartTime = xx.GameDate,
                                 JamNumber = xx.GameJams.OrderByDescending(x => x.JamNumber).FirstOrDefault().JamNumber,
                                 PeriodNumber = xx.GameJams.OrderByDescending(x => x.JamNumber).FirstOrDefault().CurrentPeriod,
                                 GameId = xx.GameId,
                                 RuleSet = xx.GameType,
                                 GameName = xx.GameName,
                                 Team2Name = (dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault() == null) ? xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamName : dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault().Name,
                                 Team1Name = (dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault() == null) ? xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamName : dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault().Name,
                                 Team1Score = xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().CurrentScore,
                                 Team2Score = xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().CurrentScore

                             }).OrderByDescending(x => x.StartTime).ToList();
                return games;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return new List<CurrentGameJson>();
        }

        public static List<Game> GetGamesPlayedByMember(Guid memberId, int recordsToSkip, int numberOfRecordsToPull)
        {
            try
            {
                var dc = new ManagementContext();
                var games = (from xx in dc.GameMembers
                             where xx.MemberLinkId == memberId
                             select new Game
                             {
                                 GameDate = xx.Team.Game.GameDate,
                                 GameId = xx.Team.Game.GameId,
                                 GameName = xx.Team.Game.GameName,
                                 Team1Id = xx.Team.Game.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamId,
                                 Team1LinkId = xx.Team.Game.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamIdLink,
                                 Team2Id = xx.Team.Game.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamId,
                                 Team2LinkId = xx.Team.Game.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamIdLink,
                                 Team2Name = (dc.Leagues.Where(g => g.LeagueId == xx.Team.Game.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault() == null) ? xx.Team.Game.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamName : dc.Leagues.Where(g => g.LeagueId == xx.Team.Game.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault().Name,
                                 Team1Name = (dc.Leagues.Where(g => g.LeagueId == xx.Team.Game.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault() == null) ? xx.Team.Game.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamName : dc.Leagues.Where(g => g.LeagueId == xx.Team.Game.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault().Name,
                                 Team1Score = dc.GameScore.Where(x => x.GameTeam == xx.Team.Game.GameTeams.OrderByDescending(z => z.TeamName).FirstOrDefault()),
                                 Team2Score = dc.GameScore.Where(x => x.GameTeam == xx.Team.Game.GameTeams.OrderBy(z => z.TeamName).FirstOrDefault())
                             }).OrderByDescending(x => x.GameDate).Skip(recordsToSkip).Take(numberOfRecordsToPull).ToList();

                foreach (var game in games)
                {
                    game.Team1ScoreTotal = 0;
                    foreach (var score in game.Team1Score)
                        game.Team1ScoreTotal += score.Point;

                    game.Team2ScoreTotal = 0;
                    foreach (var score in game.Team2Score)
                        game.Team2ScoreTotal += score.Point;
                }
                return games;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return null;
        }

        /// <summary>
        /// gets the games owned by the federation
        /// </summary>
        /// <param name="federationId"></param>
        /// <param name="recordsToSkip"></param>
        /// <param name="numberOfRecordsToPull"></param>
        /// <returns></returns>
        public static List<Game> GetGamesOwnedByFederation(Guid federationId, int recordsToSkip, int numberOfRecordsToPull)
        {
            try
            {
                var dc = new ManagementContext();

                var games = (from xx in dc.GameFederationOwners
                             where xx.Federation.FederationId == federationId
                             select new Game
                             {
                                 GameDate = xx.Game.GameDate,
                                 GameId = xx.Game.GameId,
                                 GameName = xx.Game.GameName,
                                 Team1Id = xx.Game.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamId,
                                 Team1LinkId = xx.Game.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamIdLink,
                                 Team2Id = xx.Game.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamId,
                                 Team2LinkId = xx.Game.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamIdLink,
                                 Team2Name = (dc.Leagues.Where(g => g.LeagueId == xx.Game.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault() == null) ? xx.Game.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamName : dc.Leagues.Where(g => g.LeagueId == xx.Game.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault().Name,
                                 Team1Name = (dc.Leagues.Where(g => g.LeagueId == xx.Game.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault() == null) ? xx.Game.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamName : dc.Leagues.Where(g => g.LeagueId == xx.Game.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault().Name,
                                 Team1Score = dc.GameScore.Where(x => x.GameTeam == xx.Game.GameTeams.OrderByDescending(z => z.TeamName).FirstOrDefault()),
                                 Team2Score = dc.GameScore.Where(x => x.GameTeam == xx.Game.GameTeams.OrderBy(z => z.TeamName).FirstOrDefault()),
                             }).OrderByDescending(x => x.GameDate).Skip(recordsToSkip).Take(numberOfRecordsToPull).ToList();

                foreach (var game in games)
                {
                    game.Team1ScoreTotal = 0;
                    foreach (var score in game.Team1Score)
                        game.Team1ScoreTotal += score.Point;

                    game.Team2ScoreTotal = 0;
                    foreach (var score in game.Team2Score)
                        game.Team2ScoreTotal += score.Point;
                }
                return games;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return null;
        }
        public static List<Game> GetGamesForLeague(Guid leagueId, int recordsToSkip, int numberOfRecordsToPull)
        {
            try
            {
                var dc = new ManagementContext();
                int scoreType = Convert.ToInt32(ScoreboardModeEnum.AddedOldGame);
                var gameIds = dc.GameTeam.Where(x => x.TeamIdLink == leagueId).Select(x => x.Game.GameId).ToList();
                var games = (from xx in dc.Games.Include("GameScores").Include("GameTeams")
                             where gameIds.Contains(xx.GameId)
                             where xx.ScoreboardType == scoreType
                             select new Game
                             {
                                 GameDate = xx.GameDate,
                                 GameId = xx.GameId,
                                 GameName = xx.GameName,
                                 Team1Id = xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamId,
                                 Team1LinkId = xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamIdLink,
                                 Team2Id = xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamId,
                                 Team2LinkId = xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamIdLink,
                                 Team2Name = (dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault() == null) ? xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamName : dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault().Name,
                                 Team1Name = (dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault() == null) ? xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamName : dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamIdLink).FirstOrDefault().Name,
                                 Team1Score = dc.GameScore.Where(x => x.GameTeam == xx.GameTeams.OrderByDescending(z => z.TeamName).FirstOrDefault()),
                                 Team2Score = dc.GameScore.Where(x => x.GameTeam == xx.GameTeams.OrderBy(z => z.TeamName).FirstOrDefault()),
                             }).OrderByDescending(x => x.GameDate).Skip(recordsToSkip).Take(numberOfRecordsToPull).ToList();

                foreach (var game in games)
                {
                    game.Team1ScoreTotal = 0;
                    foreach (var score in game.Team1Score)
                        game.Team1ScoreTotal += score.Point;

                    game.Team2ScoreTotal = 0;
                    foreach (var score in game.Team2Score)
                        game.Team2ScoreTotal += score.Point;
                }
                return games;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return null;
        }


        public static int GetNumberOfGamesForLeague(Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var games = (from xx in dc.GameTeam
                             where xx.TeamIdLink == leagueId
                             select xx.Game).Count();
                return games;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return -1;
        }



        public static int GetNumberOfGamesForFederation(Guid federationId)
        {
            try
            {
                var dc = new ManagementContext();
                var games = (from xx in dc.GameFederationOwners
                             where xx.Federation.FederationId == federationId
                             select xx).Count();
                return games;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return -1;
        }
        /// <summary>
        /// get number of games for the current member 
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static int GetNumberOfGamesForPlayer(Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var games = (from xx in dc.GameMembers
                             where xx.MemberLinkId == memberId
                             select xx).Count();
                return games;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return -1;
        }



        /// <summary>
        /// saves the posted file to the DB and the directory as an advertisement
        /// </summary>
        /// <param name="file"></param>
        /// <param name="context"></param>
        public static Guid SaveAdvertisementsToDb(Guid gameId, HttpPostedFileBase file)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(LibraryConfig.DataFolder + ServerManager.SAVE_ADVERTISEMENTS_FOLDER);
                if (!dir.Exists)
                    dir.Create();

                string dtSaved = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

                string extension = Path.GetExtension(file.FileName);
                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string logoFile = Path.Combine(LibraryConfig.DataFolder + ServerManager.SAVE_ADVERTISEMENTS_FOLDER, fileName + dtSaved + extension);
                string logoUrl = Path.Combine(LibraryConfig.PublicSite + UrlManager.SAVE_ADVERTISEMENTS_WEBSITE_FOLDER, fileName + dtSaved + extension);
                file.SaveAs(logoFile);



                //var advert = new GameAdvertisements
                //{
                //    Created = DateTime.UtcNow,
                //    uid = Guid.NewGuid(),
                //    Name = fileName,
                //    SavePath = logoFile,
                //    Url = logoUrl,
                //    LastModified = DateTime.UtcNow,
                //    Game = game
                //};
                //var dc = new ManagementContext();
                //dc.GameAdvertisements.Add(advert);
                //dc.SaveChanges();
                //return advert.uid;

                return new Guid();
            }
            catch (Exception e)
            {
                Library.Classes.Error.ErrorDatabaseManager.AddException(e, e.GetType(), ErrorGroupEnum.Database);
            }
            return new Guid();
        }
        /// <summary>
        /// deletes the game entirely.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteGame(Guid id)
        {
            try
            {
                var dc = new ManagementContext();
                var member = dc.Games.Where(x => x.GameId == id).FirstOrDefault();
                if (member != null && member.GamePolicy != null)
                    dc.GamePolicy.Remove(member.GamePolicy);
                var teams = member.GameTeams.ToList();
                for (int i = 0; i < teams.Count; i++)
                {
                    if (teams[i].LineupSettings != null)
                        dc.GameTeamLineUpSettings.Remove(teams[i].LineupSettings);

                    var membersOfTeam = teams[i].GameMembers.ToList();
                    for (int j = 0; j < membersOfTeam.Count; j++)
                        teams[i].GameMembers.Remove(membersOfTeam[j]);

                    dc.GameTeam.Remove(teams[i]);
                }


                var timeouts = member.GameTimeouts.ToList();
                for (int i = 0; i < timeouts.Count; i++)
                    dc.GameTimeOut.Remove(timeouts[i]);

                if (member != null)
                    dc.Games.Remove(member);
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;

        }

        /// <summary>
        /// joining games doesn't move the members or teams due to name comparisons needed, and I just don't 
        /// feel like writing all that crap right now, but it does move everything else.
        /// </summary>
        /// <param name="originalGame"></param>
        /// <param name="movingFromGame"></param>
        /// <returns></returns>
        public static bool JoinGames(Guid originalGame, Guid movingFromGame)
        {
            try
            {
                var dc = new ManagementContext();
                var original = dc.Games.Where(x => x.GameId == originalGame).FirstOrDefault();
                var movedFrom = dc.Games.Where(x => x.GameId == movingFromGame).FirstOrDefault();


                var adverts = dc.GameAdvertisements.Where(x => x.Game.GameId == movingFromGame);
                foreach (var advert in adverts)
                {
                    advert.Game = original;

                }
                dc.SaveChanges();

                var conver = dc.GameConversations.Where(x => x.Game.GameId == movingFromGame);
                foreach (var item in conver)
                {
                    item.Game = original;
                }
                dc.SaveChanges();

                var jams = dc.GameJams.Where(x => x.Game.GameId == movingFromGame);
                foreach (var item in jams)
                {
                    item.Game = original;
                }
                dc.SaveChanges();

                var links = dc.GameLinks.Where(x => x.Game.GameId == movingFromGame);
                foreach (var item in links)
                {
                    item.Game = original;
                }
                dc.SaveChanges();

                var teams = dc.GameTeam.Where(x => x.Game.GameId == movingFromGame).ToList();
                for (int i = 0; i < teams.Count(); i++)
                {
                    var team = original.GameTeams.Where(x => x.TeamName.ToLower() == teams[i].TeamName.ToLower()).FirstOrDefault();
                    if (team != null)
                    {
                        team.CurrentScore = teams[i].CurrentScore;
                        team.CurrentTimeouts = teams[i].CurrentTimeouts;
                        foreach (var sc in teams[i].GameScores)
                        {
                            sc.GameTeam = team;
                            team.GameScores.Add(sc);
                        }
                    }
                }
                dc.SaveChanges();

                var stops = dc.GameStopWatch.Where(x => x.Game.GameId == movingFromGame);
                foreach (var item in stops)
                {
                    item.Game = original;

                }
                dc.SaveChanges();

                var times = dc.GameTimeOut.Where(x => x.Game.GameId == movingFromGame);
                foreach (var item in times)
                {
                    item.Game = original;
                }
                dc.SaveChanges();
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;

        }
    }
}
