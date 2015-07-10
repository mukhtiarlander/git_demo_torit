using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using Scoreboard.Library.ViewModel;
using RDN.Utilities.Error;
using RDN.Library.Classes.Account.Classes;
using RDN.Utilities.Config;
using RDN.Library.DataModels.Game;
using RDN.Library.Classes.Game.Enums;
using RDN.Library.DataModels.EmailServer.Enums;
using RDN.Library.DataModels.Game.Members;
using RDN.Library.Classes.Payment;
using RDN.Portable.Config;
using Scoreboard.Library.ViewModel.Members;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Federation;
using RDN.Library.Classes.Config;

namespace RDN.Library.Classes.Game
{
    public class GameManager
    {
        public static int DoUpdateOfGameTeamIds()
        {
            var dc = new ManagementContext();
            var teams = dc.GameMembers;
            foreach (var team in teams)
            {
                team.GameMemberDbId = team.GameMemberId;
                team.Team = team.Team;
            }
            return dc.SaveChanges();

        }
        public static void MoveOwnersOfGameOverToNewOwnershipSystem()
        {
            var dc = new ManagementContext();
            var gameDb = (from xx in dc.Games
                          select xx).ToList();

            foreach (var game in gameDb)
            {
                if (game.OwnerOfGame != null)
                {
                    GameMemberOwnership owner = new GameMemberOwnership();
                    owner.Game = game;
                    owner.Member = game.OwnerOfGame;
                    owner.OwnerType = Convert.ToInt32(GameOwnerEnum.Owner);
                    game.MemberOwners.Add(owner);
                }
                if (game.FederationOwner != null)
                {
                    GameFederationOwnership owner = new GameFederationOwnership();
                    owner.Game = game;
                    owner.Federation = game.FederationOwner;
                    owner.OwnerType = Convert.ToInt32(GameOwnerEnum.Owner);
                    game.FederationOwners.Add(owner);
                }
            }
            dc.SaveChanges();
        }

        /// <summary>
        /// updates the game from the management view
        /// Currently updates game name, date, and its members.
        /// </summary>
        /// <param name="game"></param>
        public static bool UpdateGameFromManagement(Game game)
        {
            try
            {
                var dc = new ManagementContext();
                var gameDb = (from xx in dc.Games
                              where xx.GameId == game.GameId
                              where xx.IdForOnlineManagementUse == game.PrivateKeyForGame
                              select xx).FirstOrDefault();
                if (gameDb != null)
                {
                    gameDb.ScoreboardType = Convert.ToInt32(ScoreboardModeEnum.Live);
                    gameDb.GameDate = game.GameDate;
                    gameDb.GameName = game.GameName;
                    gameDb.EmbededVideoHtml = game.EmbededVideoString;
                    gameDb.IsThereVideoOfGame = Convert.ToInt32(game.IsThereVideoOfGame);
                    gameDb.StreamingUrlOfVideo = game.StreamingUrlSilverlight;
                    gameDb.StreamingUrlOfVideoMobile = game.StreamingMobileUrlSilverlight;
                    if (game.SelectedPaywall > 0)
                        gameDb.Paywall = dc.Paywalls.Where(x => x.PaywallId == game.SelectedPaywall).FirstOrDefault();
                    else
                        gameDb.Paywall = null;
                    //iterating through all memebers in DB
                    foreach (var team in gameDb.GameTeams)
                    {
                        foreach (var memDb in team.GameMembers)
                        {
                            //finding the member in local game
                            var mem = game.MembersOfGame.Where(x => x.MemberId == memDb.GameMemberId).FirstOrDefault();
                            //if we find member in local game, we can set the member link, otherwise we set it to nothing.
                            if (mem != null)
                            {
                                //if the member id is not the same as it is in the DB.
                                if (mem.MemberLinkId != memDb.MemberLinkId)
                                {
                                    //gets the user from the members table.
                                    var memberUser = dc.Members.Where(x => x.MemberId == mem.MemberLinkId).FirstOrDefault();
                                    //if member exists, we need to find user.
                                    if (memberUser != null)
                                    {
                                        //finds user to email.
                                        var userAccount = System.Web.Security.Membership.GetUser((object)memberUser.AspNetUserId);
                                        if (userAccount != null)
                                        {
                                            //collects the data to email the user.
                                            var emailData = new Dictionary<string, string>
                                        {
                                            { "derbyname", memberUser.DerbyName }, 
                                            { "email", userAccount.Email },
                                            { "gamename", game.GameName},
                                            { "link", "http://rdnation.com/roller-derby-game/" + game.GameId.ToString().Replace("-","")+"/"+ RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(game.GameName) +"/"+ RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(game.Team1Name) +"/"+RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(game.Team2Name)  }
                                        };
                                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, userAccount.Email, EmailServer.EmailServer.DEFAULT_SUBJECT + " Added To A Derby Game", emailData, EmailServer.EmailServerLayoutsEnum.AddedToANewGame);
                                        }
                                    }
                                }
                                memDb.MemberLinkId = mem.MemberLinkId;
                            }
                            else
                                memDb.MemberLinkId = new Guid();
                        }
                    }

                    if (!String.IsNullOrEmpty(game.SelectedTournament))
                    {
                        var tourny = dc.GameTournaments.Where(x => x.TournamentId == new Guid(game.SelectedTournament)).FirstOrDefault();
                        if (game.PassCodeEnteredForTournament == tourny.TournamentPasscode)
                        {
                            tourny.Games.Add(gameDb);
                        }
                    }
                    else
                        gameDb.GameTournament = null;
                    if (!String.IsNullOrEmpty(game.SelectedShop))
                    {
                        gameDb.SelectedShop = dc.Merchants.Where(x => x.MerchantId == new Guid(game.SelectedShop)).FirstOrDefault();
                    }
                    else
                        gameDb.SelectedShop = null;

                    foreach (var mem in game.MemberOwners)
                    {
                        if (mem.MemberId != new Guid())
                        {
                            var member = gameDb.MemberOwners.Where(x => x.Member.MemberId == mem.MemberId).FirstOrDefault();
                            if (member == null)
                            {
                                var memDb = dc.Members.Where(x => x.MemberId == mem.MemberId).FirstOrDefault();
                                if (memDb != null)
                                {
                                    GameMemberOwnership m = new GameMemberOwnership();
                                    m.Game = gameDb;
                                    m.Member = memDb;
                                    m.OwnerType = Convert.ToInt32(GameOwnerEnum.Manager);
                                    gameDb.MemberOwners.Add(m);

                                    if (memDb.AspNetUserId != new Guid())
                                    {
                                        var user = System.Web.Security.Membership.GetUser((object)memDb.AspNetUserId);
                                        EmailAccountAddedToManageDerbyGame(gameDb, memDb.DerbyName, user.Email);
                                    }
                                }
                            }
                        }
                    }
                    foreach (var mem in game.FederationOwners)
                    {
                        if (mem.FederationId != new Guid())
                        {
                            var member = gameDb.FederationOwners.Where(x => x.Federation.FederationId == mem.FederationId).FirstOrDefault();
                            if (member == null)
                            {
                                var memDb = dc.Federations.Where(x => x.FederationId == mem.FederationId).FirstOrDefault();
                                if (memDb != null)
                                {
                                    GameFederationOwnership m = new GameFederationOwnership();
                                    m.Game = gameDb;
                                    m.Federation = memDb;
                                    m.OwnerType = Convert.ToInt32(GameOwnerEnum.Manager);
                                    gameDb.FederationOwners.Add(m);
                                    if (memDb.ContactCard != null && memDb.ContactCard.Emails.Count > 0)
                                    {
                                        var emails = memDb.ContactCard.Emails.FirstOrDefault();
                                        EmailAccountAddedToManageDerbyGame(gameDb, memDb.Name, emails.EmailAddress);
                                    }
                                }
                            }
                        }
                    }
                    foreach (var mem in game.LeagueOwners)
                    {
                        if (mem.LeagueId != new Guid())
                        {
                            var member = gameDb.LeagueOwners.Where(x => x.League.LeagueId == mem.LeagueId).FirstOrDefault();
                            if (member == null)
                            {
                                var memDb = dc.Leagues.Where(x => x.LeagueId == mem.LeagueId).FirstOrDefault();
                                if (memDb != null)
                                {
                                    GameLeagueOwnership m = new GameLeagueOwnership();
                                    m.Game = gameDb;
                                    m.League = memDb;
                                    m.OwnerType = Convert.ToInt32(GameOwnerEnum.Manager);
                                    gameDb.LeagueOwners.Add(m);
                                    if (memDb.ContactCard != null && memDb.ContactCard.Emails.Count > 0)
                                    {
                                        var emails = memDb.ContactCard.Emails.FirstOrDefault();
                                        EmailAccountAddedToManageDerbyGame(gameDb, memDb.Name, emails.EmailAddress);
                                    }
                                }
                            }
                        }
                    }


                    int c = dc.SaveChanges();
                    return c > 0;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return false;
        }
        /// <summary>
        /// sends an email to the account added to help manage Bout.
        /// </summary>
        /// <param name="gameDb"></param>
        /// <param name="name"></param>
        /// <param name="emailToSendTo"></param>
        private static void EmailAccountAddedToManageDerbyGame(DataModels.Game.Game gameDb, string name, string emailToSendTo)
        {
            var emailData = new Dictionary<string, string> { 
                        { "derbyname", name}, 
                        { "gameName", gameDb.GameName}, 
                        { "link", "http://league.rdnation.com/game/manage/"+ gameDb.IdForOnlineManagementUse.ToString().Replace("-","") +"/"+ gameDb.GameId.ToString().Replace("-","") } };

            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, emailToSendTo, EmailServer.EmailServer.DEFAULT_SUBJECT + " Added To Manage Bout", emailData, layout: EmailServer.EmailServerLayoutsEnum.MemberAddedToManageDerbyGame, priority: EmailPriority.Normal);
        }

        /// <summary>   
        /// gets the number of games owned by the member.
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static int GetNumberOfGamesForMemberOwner(Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var games = (from xx in dc.GameMemberOwners
                             where xx.Member.MemberId == memberId
                             select xx).Count();
                return games;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return -1;
        }
        public static int GetNumberOfGamesForLeagueOwner(Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var games = (from xx in dc.GameLeagueOwners
                             where xx.League.LeagueId == leagueId
                             select xx).Count();
                return games;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return -1;
        }
        public static int GetNumberOfGamesForFederationOwner(Guid federationId)
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
        //public static int GetNumberOfGamesForOwner(Guid memberId)
        //{
        //    try
        //    {
        //        var dc = new ManagementContext();
        //        var games = (from xx in dc.Games
        //                     where xx.OwnerOfGame.MemberId == memberId
        //                     select xx).Count();
        //        return games;

        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType(),ErrorGroupEnum.Database);
        //    }
        //    return -1;
        //}
        /// <summary>
        /// gets all the games owned by the member.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="recordsToSkip"></param>
        /// <param name="numberOfRecordsToPull"></param>
        /// <returns></returns>
        public static List<Game> GetGamesOwnedByMember(Guid memberId, int recordsToSkip, int numberOfRecordsToPull)
        {
            try
            {
                var dc = new ManagementContext();

                var games = (from xx in dc.GameMemberOwners
                             where xx.Member.MemberId == memberId
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
                                 StreamingUrlSilverlight = xx.Game.StreamingUrlOfVideo,
                                 StreamingMobileUrlSilverlight = xx.Game.StreamingUrlOfVideoMobile,
                                 EmbededVideoString = xx.Game.EmbededVideoHtml,
                                 PrivateKeyForGame = xx.Game.IdForOnlineManagementUse,
                                 HasGameStarted = xx.Game.HasGameStarted,
                                 IsGameLive = xx.Game.IsGameLive,
                                 IsGameOver = xx.Game.IsGameOver,
                                 IsGamePublishedOnline = xx.Game.IsGamePublishedOnline,
                                 IsGameScrimmage = xx.Game.IsGameScrimmage,
                                 VersionNumber = xx.Game.VersionNumber,
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
        public static List<Game> GetGamesOwnedByFederation(List<Guid> fedIds, int recordsToSkip, int numberOfRecordsToPull)
        {
            try
            {
                var dc = new ManagementContext();

                var games = (from xx in dc.GameFederationOwners
                             where fedIds.Contains(xx.Federation.FederationId)
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
                                 StreamingUrlSilverlight = xx.Game.StreamingUrlOfVideo,
                                 StreamingMobileUrlSilverlight = xx.Game.StreamingUrlOfVideoMobile,
                                 EmbededVideoString = xx.Game.EmbededVideoHtml,
                                 PrivateKeyForGame = xx.Game.IdForOnlineManagementUse,
                                 HasGameStarted = xx.Game.HasGameStarted,
                                 IsGameLive = xx.Game.IsGameLive,
                                 IsGameOver = xx.Game.IsGameOver,
                                 IsGamePublishedOnline = xx.Game.IsGamePublishedOnline,
                                 IsGameScrimmage = xx.Game.IsGameScrimmage,
                                 VersionNumber = xx.Game.VersionNumber,
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
        public static List<Game> GetGamesOwnedByLeague(List<Guid> leagueIds, int recordsToSkip, int numberOfRecordsToPull)
        {
            try
            {
                var dc = new ManagementContext();

                var games = (from xx in dc.GameLeagueOwners
                             where leagueIds.Contains(xx.League.LeagueId)
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
                                 StreamingUrlSilverlight = xx.Game.StreamingUrlOfVideo,
                                 StreamingMobileUrlSilverlight = xx.Game.StreamingUrlOfVideoMobile,
                                 EmbededVideoString = xx.Game.EmbededVideoHtml,
                                 PrivateKeyForGame = xx.Game.IdForOnlineManagementUse,
                                 HasGameStarted = xx.Game.HasGameStarted,
                                 IsGameLive = xx.Game.IsGameLive,
                                 IsGameOver = xx.Game.IsGameOver,
                                 IsGamePublishedOnline = xx.Game.IsGamePublishedOnline,
                                 IsGameScrimmage = xx.Game.IsGameScrimmage,
                                 VersionNumber = xx.Game.VersionNumber,
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
        /// gets the game to manage it live.
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="privatePassCode"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static Game GetGameForManagement(Guid gameId, Guid privatePassCode, Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var game = (from xx in dc.Games
                            where xx.GameId == gameId
                            where xx.IdForOnlineManagementUse == privatePassCode
                            select xx).FirstOrDefault();
                if (game != null)
                {
                    if (game.MemberOwners.Count == 0)
                    {
                        var mem = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                        if (mem != null)
                        {
                            GameMemberOwnership owner = new GameMemberOwnership();
                            owner.Game = game;
                            owner.Member = mem;
                            owner.OwnerType = Convert.ToInt32(GameOwnerEnum.Owner);
                            game.MemberOwners.Add(owner);
                            dc.SaveChanges();
                        }
                    }
                    return DisplayGame(game);


                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return null;
        }

        public static Game DisplayGame(DataModels.Game.Game game)
        {
            ManagementContext dc = new ManagementContext();
            Game tempGame = new Game();
            tempGame.IsThereVideoOfGame = (GameVideoTypeEnum)Enum.Parse(typeof(GameVideoTypeEnum), game.IsThereVideoOfGame.ToString());
            tempGame.StreamingUrlSilverlight = game.StreamingUrlOfVideo;
            tempGame.StreamingMobileUrlSilverlight = game.StreamingUrlOfVideoMobile;
            tempGame.EmbededVideoString = game.EmbededVideoHtml;
            if (game.Paywall != null)
                tempGame.SelectedPaywall = game.Paywall.PaywallId;
            tempGame.GameDate = game.GameDate;
            tempGame.GameId = game.GameId;
            tempGame.PrivateKeyForGame = game.IdForOnlineManagementUse;
            tempGame.GameName = game.GameName;
            if (game.SelectedShop != null)
                tempGame.SelectedShop = game.SelectedShop.MerchantId.ToString();
            var games = game.GameTeams.OrderByDescending(x => x.Created).Take(2);
            var team2 = games.OrderBy(x => x.TeamName).FirstOrDefault();
            var team1 = games.OrderByDescending(x => x.TeamName).FirstOrDefault();

            if (team1 != null)
                tempGame.Team1Name = team1.TeamName;
            else
                tempGame.Team1Name = "Home";
            if (team2 != null)
                tempGame.Team2Name = team2.TeamName;
            else
                tempGame.Team2Name = "Away";
            tempGame.HasGameStarted = game.HasGameStarted;
            tempGame.IsGameLive = game.IsGameLive;
            tempGame.IsGameOver = game.IsGameOver;
            tempGame.IsGamePublishedOnline = game.IsGamePublishedOnline;
            tempGame.IsGameScrimmage = game.IsGameScrimmage;
            tempGame.VersionNumber = game.VersionNumber;

            foreach (var team in game.GameTeams)
            {
                foreach (var mem in team.GameMembers)
                {
                    MemberDisplayGame member = new MemberDisplayGame();
                    member.DerbyName = mem.MemberName;
                    member.MemberId = mem.GameMemberId;
                    if (mem.MemberLinkId != new Guid())
                    {
                        var memDb = (from xx in dc.Members
                                     where xx.MemberId == mem.MemberLinkId
                                     select xx.DerbyName).FirstOrDefault();
                        if (!String.IsNullOrEmpty(memDb))
                        {
                            member.MemberLinkId = mem.MemberLinkId;
                            member.DerbyLinkName = memDb;
                        }
                    }
                    tempGame.MembersOfGame.Add(member);
                }
            }
            var endDate = DateTime.UtcNow.AddDays(5);
            var tournys = dc.GameTournaments.Where(x => x.StartDate < DateTime.UtcNow && x.EndDate < endDate).ToList();
            if (game.GameTournament != null)
            {
                tempGame.SelectedTournament = game.GameTournament.TournamentId.ToString();
                tempGame.PassCodeEnteredForTournament = game.GameTournament.TournamentPasscode;
                Tournament t = new Tournament();
                t.Name = game.GameTournament.TournamentName;
                t.EndDate = game.GameTournament.EndDate;
                t.Id = game.GameTournament.TournamentId;
                t.StartDate = game.GameTournament.StartDate;
                tempGame.AvailableTournaments.Add(t);
            }

            foreach (var owner in game.MemberOwners)
            {
                MemberDisplayBasic mem = new MemberDisplayBasic();
                mem.MemberId = owner.Member.MemberId;
                mem.DerbyName = owner.Member.DerbyName;
                tempGame.MemberOwners.Add(mem);
            }
            if (tempGame.MemberOwners.Count < 5)
            {
                MemberDisplayBasic mem = new MemberDisplayBasic();
                tempGame.MemberOwners.Add(mem);
                MemberDisplayBasic mem1 = new MemberDisplayBasic();
                tempGame.MemberOwners.Add(mem1);
                MemberDisplayBasic mem2 = new MemberDisplayBasic();
                tempGame.MemberOwners.Add(mem2);
                MemberDisplayBasic mem3 = new MemberDisplayBasic();
                tempGame.MemberOwners.Add(mem3);
            }

            foreach (var owner in game.LeagueOwners)
            {
                RDN.Portable.Classes.League.Classes.League mem = new RDN.Portable.Classes.League.Classes.League();
                mem.LeagueId = owner.League.LeagueId;
                mem.Name = owner.League.Name;
                tempGame.LeagueOwners.Add(mem);
            }
            if (tempGame.LeagueOwners.Count < 2)
            {
                RDN.Portable.Classes.League.Classes.League mem = new RDN.Portable.Classes.League.Classes.League();
                tempGame.LeagueOwners.Add(mem);
                RDN.Portable.Classes.League.Classes.League mem1 = new RDN.Portable.Classes.League.Classes.League();
                tempGame.LeagueOwners.Add(mem1);
            }
            foreach (var owner in game.FederationOwners)
            {
                FederationDisplay mem = new FederationDisplay();
                mem.FederationId = owner.Federation.FederationId;
                mem.FederationName = owner.Federation.Name;
                tempGame.FederationOwners.Add(mem);
            }
            if (tempGame.FederationOwners.Count == 0)
            {
                FederationDisplay mem = new FederationDisplay();
                tempGame.FederationOwners.Add(mem);
            }

            foreach (var tourn in tournys)
            {
                var gameT = tourn.Games.Where(x => x.GameId == tempGame.GameId).FirstOrDefault();
                if (gameT != null)
                {
                    tempGame.SelectedTournament = tourn.TournamentId.ToString();
                    tempGame.PassCodeEnteredForTournament = tourn.TournamentPasscode;
                }
                Tournament t = new Tournament();
                t.Name = tourn.TournamentName;
                t.EndDate = tourn.EndDate;
                t.Id = tourn.TournamentId;
                t.StartDate = tourn.StartDate;
                if (tempGame.AvailableTournaments.Where(x => x.Id == t.Id).FirstOrDefault() == null)
                    tempGame.AvailableTournaments.Add(t);
            }
            return tempGame;
        }


        /// <summary>
        /// publishes the game online.
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public static bool PublishGameOnline(Guid gameId, bool publishGame)
        {
            try
            {
                var dc = new ManagementContext();
                var game = (from xx in dc.Games
                            where xx.GameId == gameId
                            select xx).FirstOrDefault();
                if (game != null)
                {
                    game.IsGamePublishedOnline = publishGame;
                    dc.SaveChanges();
                    return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return false;
        }
        public static List<Team.Classes.Team> GetTeamsOfGame(Guid gameId)
        {
            List<Team.Classes.Team> teams = new List<Team.Classes.Team>();
            try
            {
                var dc = new ManagementContext();
                var game = (from xx in dc.Games
                            where xx.GameId == gameId
                            select new
                            {
                                Team1Name = xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamName,
                                Team1Id = xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().TeamId,
                                Team1Score = xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().CurrentScore,
                                Team2Name = xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamName,
                                Team2Id = xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().TeamId,
                                Team2Score = xx.GameTeams.OrderByDescending(x => x.TeamName).FirstOrDefault().CurrentScore,
                            }).FirstOrDefault();

                if (game != null)
                {
                    Team.Classes.Team t = new Team.Classes.Team();
                    t.TeamId = game.Team1Id;
                    t.Name = game.Team1Name;
                    t.Score = game.Team1Score;
                    teams.Add(t);
                    Team.Classes.Team t1 = new Team.Classes.Team();
                    t1.TeamId = game.Team2Id;
                    t1.Name = game.Team2Name;
                    t1.Score = game.Team2Score;
                    teams.Add(t1);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return teams;
        }

        /// <summary>
        /// embeds the video of the game so users can see it on the live game page.
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="htmlString"></param>
        /// <param name="videoType"></param>
        /// <returns></returns>
        public static bool EmbedVideoWithGame(Guid gameId, string htmlString, GameVideoTypeEnum videoType)
        {
            try
            {
                var dc = new ManagementContext();
                var game = (from xx in dc.Games
                            where xx.GameId == gameId
                            select xx).FirstOrDefault();
                if (game != null)
                {
                    game.EmbededVideoHtml = htmlString;
                    game.IsThereVideoOfGame = Convert.ToInt32(videoType);
                    dc.SaveChanges();
                    return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return false;
        }


        /// <summary>
        /// removes the embeded video from the game.
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public static bool RemoveEmbedVideoWithGame(Guid gameId)
        {
            try
            {
                var dc = new ManagementContext();
                var game = (from xx in dc.Games
                            where xx.GameId == gameId
                            select xx).FirstOrDefault();
                if (game != null)
                {
                    game.EmbededVideoHtml = "";
                    game.IsThereVideoOfGame = Convert.ToInt32(GameVideoTypeEnum.NoVideo);
                    dc.SaveChanges();
                    return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return false;
        }


        public static bool EmbedSilverlightWithGame(Guid gameId, string urlOfSilverlightVideo, string urlOfSilverlightMobile, GameVideoTypeEnum videoType)
        {
            try
            {
                var dc = new ManagementContext();
                var game = (from xx in dc.Games
                            where xx.GameId == gameId
                            select xx).FirstOrDefault();
                if (game != null)
                {
                    game.StreamingUrlOfVideo = urlOfSilverlightVideo;
                    game.StreamingUrlOfVideoMobile = urlOfSilverlightMobile;
                    game.IsThereVideoOfGame = Convert.ToInt32(videoType);
                    dc.SaveChanges();
                    return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return false;
        }

        public static bool RemoveSilverlightVideoWithGame(Guid gameId)
        {
            try
            {
                var dc = new ManagementContext();
                var game = (from xx in dc.Games
                            where xx.GameId == gameId
                            select xx).FirstOrDefault();
                if (game != null)
                {
                    game.StreamingUrlOfVideo = "";
                    game.StreamingUrlOfVideoMobile = "";
                    game.IsThereVideoOfGame = Convert.ToInt32(GameVideoTypeEnum.NoVideo);
                    dc.SaveChanges();
                    return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return false;
        }
    }
}
