using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels;
using RDN.Library.Cache;
using Scoreboard.Library.ViewModel;
using Scoreboard.Library.StopWatch;
using Scoreboard.Library.Static.Enums;
using RDN.Utilities.Error;
using RDN.Utilities.Config;
using RDN.Library.DataModels.Team;
using RDN.Library.DataModels.Context;

using RDN.Library.DataModels.Game;
using RDN.Library.Classes.Error;
using System.Collections;
using RDN.Library.Classes.Federation;
using RDN.Library.Classes.Account.Classes;
using System.IO;
using RDN.Library.Classes.Game.Enums;
using Scoreboard.Library.ViewModel.Members;
using RDN.Library.DataModels.Game.Members;
using RDN.Library.Classes.Communications;
using Scoreboard.Library.Models;
using RDN.Library.Classes.Game;
using RDN.Library.Classes.Game.Teams;
using RDN.Library.Classes.Game.Actions;
using RDN.Library.Classes.Game.Officials;
using RDN.Library.Classes.Game.Members;
using Scoreboard.Library.ViewModel.Members.Enums;
using RDN.Library.DataModels.Game.Officials;
using RDN.Portable.Classes.Imaging;




namespace RDN.Library.ViewModel
{
    public class GameServerViewModel
    {
        public static bool CheckGamePaywallIsPaid(Guid gameId, string password)
        {
            try
            {
                var dc = new ManagementContext();
                var tourny = dc.Games.Where(x => x.GameId == gameId).FirstOrDefault();
                if (tourny == null)
                    return false;
                if (tourny.Paywall != null && tourny.Paywall.PaywallInvoices.Count > 0)
                {
                    foreach (var wall in tourny.Paywall.PaywallInvoices)
                    {
                        if (wall.GeneratedPassword == password)
                            return true;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static List<Conversation> GetConversation(Guid gameId)
        {
            try
            {
                var dc = new ManagementContext();
                var convo = (from xx in dc.GameConversations
                             where xx.Game.GameId == gameId

                             select new Conversation
                             {
                                 Chat = xx.Text,
                                 Id = xx.ConversationId,
                                 MemberName = xx.Owner == null ? "Anonymous" : xx.Owner.DerbyName,
                                 Created = xx.Created,
                                 OwnerId = gameId
                             }).OrderByDescending(x => x.Id).Take(30).AsParallel().ToList();
                foreach (var con in convo)
                {
                    con.Time = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(con.Created);
                }
                return convo;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<Conversation>();
        }

        public static Conversation PostConversationText(Guid gameId, string text, Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var game = dc.Games.Where(x => x.GameId == gameId).FirstOrDefault();
                GameConversation sation = new GameConversation();
                sation.Game = game;
                sation.Text = text;
                if (memberId != new Guid())
                    sation.Owner = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                dc.GameConversations.Add(sation);
                dc.SaveChanges();
                Conversation s = new Conversation();
                s.Chat = text;
                s.Created = sation.Created;
                s.OwnerId = gameId;
                s.Id = sation.ConversationId;
                if (sation.Owner != null)
                    s.MemberName = sation.Owner.DerbyName;
                else
                    s.MemberName = "Anonymous";
                return s;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }


        /// <summary>
        /// this checks the cache first and doesn't set an expiration date on the cached game.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static GameViewModel GetGameFromCacheApi(Guid id)
        {
            try
            {

                var cachedGame = GameCache.GetCurrentLiveGame(id);

                if (cachedGame == null)
                {
                    var game = getGameFromDb(id);
                    //doing a second check just because pulling from the DB takes a few seconds and someone else could have done it.
                    GameCache.saveGameToCurrentLiveGamesCache(game);
                    return game;
                }
                return cachedGame;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        /// <summary>
        /// gets the game from cache, and sets a 30 second expiration date.  
        /// If its an old Game by 30 seconds, updates the cache from the Db.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static GameViewModel GetGameFromCache(Guid id)
        {
            var cachedGame = GameCache.GetCurrentLiveGame(id);
            if (cachedGame != null)
            {
                if (cachedGame.LastModified < DateTime.UtcNow.AddSeconds(-60) && cachedGame.HasGameEnded == false)
                {
                    GameCache.ClearGameFromCache(cachedGame.GameId);
                    cachedGame = null;
                }
            }

            if (cachedGame == null)
            {
                var game = getGameFromDb(id);
                if (game != null)
                {
                    //doing a second check just because pulling from the DB takes a few seconds and someone else could have done it.
                    GameCache.saveGameToCurrentLiveGamesCache(game);

                    return game;
                }
            }
            return cachedGame;
        }

        public static List<GameViewModel> getCurrentLiveGames()
        {
            return GameCache.GetCurrentLiveGames();
        }
        public static List<GameViewModel> getCurrentDebugGames()
        {
            return GameCache.GetCurrentLiveDebuggingGames();
        }

        private static void getCurrentLiveDebuggedGames()
        {
            ManagementContext db = new ManagementContext();
            var getG = (from xx in db.Games
                        where xx.ScoreboardType == 0
                        select xx.GameId).ToList();
            foreach (var game in getG)
            {
                var g = getGameFromDb(game);
                GameCache.saveGameToDebuggingGamesCache(g);
            }
        }
        private static void getCurrentLiveDbGames()
        {
            ManagementContext db = new ManagementContext();

            var getG = (from xx in db.Games
                        where xx.ScoreboardType == 1
                        where xx.GameDate > DateTime.UtcNow.AddHours(-3)
                        select xx.GameId).ToList();
            foreach (var game in getG)
            {
                var g = getGameFromDb(game);
                GameCache.saveGameToCurrentLiveGamesCache(g);
            }
        }

        public static GameViewModel getGameForRollerDerby(Guid id)
        {
            return GameCache.GetGameFromRollerDerby(id);
        }

        /// <summary>
        /// finds the game being added to the list of games in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static GameViewModel getGameBeingAdded(Guid id)
        {
            var game = GameCache.GetCurrentGamesGettingAdded(id);
            if (game == null)
            {
                game = getGameFromDb(id);
                GameCache.saveGameToGamesGettingAddedCache(game);
                return game;
            }
            return game;
        }

        public static void addGameToBeingAddedList(GameViewModel game)
        {
            GameCache.saveGameToGamesGettingAddedCache(game);
        }
        /// <summary>
        /// pulls the pictures of the game from the DB.
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public static List<PhotoItem> GetPicturesOfGameFromDb(Guid gameId)
        {
            List<PhotoItem> photos = new List<PhotoItem>();
            List<Guid> memberIds = new List<Guid>();
            List<Guid> memberLinkIds = new List<Guid>();
            var dc = new ManagementContext();
            var teams = dc.Games.Where(x => x.GameId == gameId).FirstOrDefault().GameTeams;
            foreach (var team in teams)
            {
                foreach (var member in team.GameMembers)
                {
                    if (member.MemberLinkId != new Guid())
                        memberLinkIds.Add(member.MemberLinkId);
                    memberIds.Add(member.GameMemberId);
                }
                if (team.Logo != null)
                {
                    photos.Add(new PhotoItem(team.Logo.ImageUrl, team.Logo.ImageUrlThumb, true, team.TeamName));
                }
            }

            var photosOfMembers = (from xx in dc.MemberPhotos
                                   where memberLinkIds.Contains(xx.Member.MemberId)
                                   select xx).ToList();

            var photosOfGameMembers = (from xx in dc.GameMemberPhotos
                                       where memberIds.Contains(xx.Member.GameMemberId)
                                       select xx).ToList();
            foreach (var photo in photosOfMembers)
            {
                photos.Add(new PhotoItem(photo.ImageUrl, photo.ImageUrlThumb, photo.IsPrimaryPhoto, "picture of skater"));
            }
            foreach (var photo in photosOfGameMembers)
            {
                photos.Add(new PhotoItem(photo.ImageUrl, photo.ImageUrlThumb, photo.IsPrimaryPhoto, "picture of skater"));
            }
            return photos;
        }

        public static GameViewModel getGameFromDb(Guid id)
        {
            try
            {
                ManagementContext db = new ManagementContext();
                var getGame = (from xx in db.Games.Include("GameLinks").Include("GameMemberPenalties").Include("GameMemberAssists").Include("GameMemberBlocks").Include("GameTeams").Include("GameJams").Include("GamePolicy").Include("GameScores").Include("GameTimeouts").Include("GameTeams.Logo")
                               where xx.GameId == id
                               //game must be published to be used.

                               //where xx.IsGamePublishedOnline == true
                               select xx).FirstOrDefault();

                if (getGame == null)
                    return null;
                GameViewModel game = new GameViewModel();

                if (getGame.GameTournament != null)
                {
                    game.TournamentId = getGame.GameTournament.TournamentId;
                    game.TournamentName = getGame.GameTournament.TournamentName;
                }
                if (getGame.FederationOwners.Count > 0)
                {
                    game.FederationId = getGame.FederationOwners.FirstOrDefault().Federation.FederationId;
                    game.FederationName = getGame.FederationOwners.FirstOrDefault().Federation.Name;
                }
                if (getGame.SelectedShop != null)
                    game.SelectedShop = getGame.SelectedShop.MerchantId.ToString();
                game.IsThereVideoOfGame = (GameVideoTypeEnum)Enum.Parse(typeof(GameVideoTypeEnum), getGame.IsThereVideoOfGame.ToString());
                game.StreamingUrlOfVideo = getGame.StreamingUrlOfVideo;
                game.StreamingUrlOfVideoMobile = getGame.StreamingUrlOfVideoMobile;
                game.EmbededVideoHtml = getGame.EmbededVideoHtml;
                game.LastModified = DateTime.UtcNow;
                game.IdForOnlineManagementUse = getGame.IdForOnlineManagementUse;
                game.ElapsedTimeGameClockMilliSeconds = (long)getGame.ElapsedGameTimeInMilliseconds;
                game.GameDate = getGame.GameDate;
                game.GameEndDate = getGame.GameEndDate;
                game.GameId = id;
                game.GameName = getGame.GameName;
                game.HasGameEnded = getGame.IsGameOver;
                game.HasGameStarted = getGame.HasGameStarted;
                game.GameLocation = getGame.GameLocation;
                game.PublishGameOnline = getGame.IsGameLive;
                game.SaveGameOnline = getGame.IsGameScrimmage;
                GameClock.getPeriodClock(id, game);
                GameClock.getIntermissionClock(id, game);
                GameClock.getLineUpClock(id, game);
                GameClock.getTimeOutClock(id, game);
                if (getGame.Paywall != null)
                    game.PaywallId = getGame.Paywall.PaywallId;


                if (getGame.ScoreboardType == 0)
                    game.ScoreboardMode = ScoreboardModeEnum.Debug;
                else if (getGame.ScoreboardType == 1)
                    game.ScoreboardMode = ScoreboardModeEnum.Live;

                //we order the teams so the first one in, is also the first one out.
                var getTeams = getGame.GameTeams.OrderByDescending(x => x.Created).ToList();
                if (game.ScoresTeam1 == null)
                    game.ScoresTeam1 = new List<ScoreViewModel>();
                if (game.ScoresTeam2 == null)
                    game.ScoresTeam2 = new List<ScoreViewModel>();

                game.GameLinks = new List<GameLinkViewModel>();
                var gameLinkss = getGame.GameLinks.ToList();
                for (int i = 0; i < getGame.GameLinks.Count; i++)
                {
                    GameLinkViewModel gameLink = new GameLinkViewModel();
                    gameLink.GameLink = gameLinkss[i].Link;
                    gameLink.LinkId = gameLinkss[i].GameLinkId;
                    gameLink.LinkType = (GameLinkTypeEnum)Enum.ToObject(typeof(GameLinkTypeEnum), gameLinkss[i].LinkType);
                    game.GameLinks.Add(gameLink);
                }


                for (int i = 0; i < getTeams.Count; i++)
                {
                    TeamViewModel tvm = new TeamViewModel();
                    tvm.TeamId = getTeams[i].TeamId;
                    tvm.Logo = new Portable.Classes.Team.TeamLogo();

                    tvm.TeamLinkId = getTeams[i].TeamIdLink;
                    var dc = new ManagementContext();
                    if (getTeams[i].Logo != null)
                    {
                        tvm.Logo.ImageUrl = getTeams[i].Logo.ImageUrl;
                        tvm.Logo.TeamLogoId = getTeams[i].Logo.TeamLogoId;
                    }

                    tvm.TeamName = getTeams[i].TeamName;
                    tvm.TimeOutsLeft = getTeams[i].CurrentTimeouts;
                    tvm.TeamMembers = new System.Collections.ObjectModel.ObservableCollection<TeamMembersViewModel>();
                    if (i == 0)
                    {
                        game.CurrentTeam1Score = getTeams[i].CurrentScore;
                        game.Team1 = tvm;
                    }
                    else if (i == 1)
                    {
                        game.CurrentTeam2Score = getTeams[i].CurrentScore;
                        game.Team2 = tvm;
                    }


                    foreach (var mem in getTeams[i].GameMembers)
                    {
                        TeamMembersViewModel mvm = new TeamMembersViewModel();
                        mvm.SkaterId = mem.GameMemberId;
                        mvm.SkaterName = mem.MemberName;
                        mvm.SkaterNumber = mem.MemberNumber;
                        mvm.SkaterLinkId = mem.MemberLinkId;

                        if (getTeams[i].TeamId == game.Team1.TeamId)
                            game.Team1.TeamMembers.Add(mvm);
                        else if (getTeams[i].TeamId == game.Team2.TeamId)
                            game.Team2.TeamMembers.Add(mvm);
                    }
                    //scores must come after teams  members get added.
                    var scores = getTeams[i].GameScores.OrderBy(x => x.JamNumber);
                    foreach (var score in scores)
                    {
                        try
                        {
                            ScoreViewModel svm = new ScoreViewModel(score.Point, score.PeriodTimeRemainingMilliseconds, score.JamId, score.JamNumber, score.PeriodNumber, score.DateTimeScored, score.GameScoreId);

                            if (getTeams[i].TeamId == game.Team1.TeamId)
                            {
                                if (score.MemberWhoScored != null)
                                    svm.PlayerWhoScored = game.Team1.TeamMembers.Where(x => x.SkaterId == score.MemberWhoScored.GameMemberId).FirstOrDefault();
                                game.ScoresTeam1.Add(svm);
                            }
                            else if (getTeams[i].TeamId == game.Team2.TeamId)
                            {
                                if (score.MemberWhoScored != null)
                                    svm.PlayerWhoScored = game.Team2.TeamMembers.Where(x => x.SkaterId == score.MemberWhoScored.GameMemberId).FirstOrDefault();
                                game.ScoresTeam2.Add(svm);
                            }
                        }
                        catch (Exception exception)
                        {
                            ErrorDatabaseManager.AddException(exception, exception.GetType());
                        }
                    }


                }
                game.BlocksForTeam1 = new List<BlockViewModel>();
                game.BlocksForTeam2 = new List<BlockViewModel>();
                game.AssistsForTeam1 = new List<AssistViewModel>();
                game.AssistsForTeam2 = new List<AssistViewModel>();
                game.PenaltiesForTeam1 = new List<PenaltyViewModel>();
                game.PenaltiesForTeam2 = new List<PenaltyViewModel>();
                //blocks must come after teams  members get added.
                var blocks = getGame.GameMemberBlocks.OrderBy(x => x.JamNumber);
                foreach (var block in blocks)
                {
                    try
                    {
                        BlockViewModel svm = new BlockViewModel(block.PeriodTimeRemainingMilliseconds, block.JamNumber, block.PeriodNumber, block.DateTimeBlocked, block.GameBlockId);

                        var member = game.Team1.TeamMembers.Where(x => x.SkaterId == block.MemberWhoBlocked.GameMemberId).FirstOrDefault();
                        if (member == null)
                        {
                            member = game.Team2.TeamMembers.Where(x => x.SkaterId == block.MemberWhoBlocked.GameMemberId).FirstOrDefault();
                            svm.PlayerWhoBlocked = member;
                            game.BlocksForTeam2.Add(svm);
                        }
                        else
                        {
                            svm.PlayerWhoBlocked = member;
                            game.BlocksForTeam1.Add(svm);
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }

                //blocks must come after teams  members get added.
                var assists = getGame.GameMemberAssists.OrderBy(x => x.JamNumber);
                foreach (var assist in assists)
                {
                    try
                    {
                        AssistViewModel svm = new AssistViewModel(assist.PeriodTimeRemainingMilliseconds, assist.JamNumber, assist.PeriodNumber, assist.DateTimeAssisted, assist.GameAssistId);

                        var member = game.Team1.TeamMembers.Where(x => x.SkaterId == assist.MemberWhoAssisted.GameMemberId).FirstOrDefault();
                        if (member == null)
                        {
                            member = game.Team2.TeamMembers.Where(x => x.SkaterId == assist.MemberWhoAssisted.GameMemberId).FirstOrDefault();
                            svm.PlayerWhoAssisted = member;
                            game.AssistsForTeam2.Add(svm);
                        }
                        else
                        {
                            svm.PlayerWhoAssisted = member;
                            game.AssistsForTeam1.Add(svm);
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }

                var penal = getGame.GameMemberPenalties.OrderBy(x => x.JamNumber);
                foreach (var pen in penal)
                {
                    try
                    {
                        PenaltyViewModel svm = new PenaltyViewModel((PenaltiesEnum)Enum.Parse(typeof(PenaltiesEnum), pen.PenaltyType.ToString()), pen.PeriodTimeRemainingMilliseconds, pen.JamNumber, pen.PeriodNumber, pen.DateTimePenaltied, pen.GamePenaltyId);

                        var member = game.Team1.TeamMembers.Where(x => x.SkaterId == pen.MemberWhoPenaltied.GameMemberId).FirstOrDefault();
                        if (member == null)
                        {
                            member = game.Team2.TeamMembers.Where(x => x.SkaterId == pen.MemberWhoPenaltied.GameMemberId).FirstOrDefault();
                            svm.PenaltyAgainstMember = member;
                            game.PenaltiesForTeam2.Add(svm);
                        }
                        else
                        {
                            svm.PenaltyAgainstMember = member;
                            game.PenaltiesForTeam1.Add(svm);
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }

                var getJams = getGame.GameJams.OrderBy(x => x.JamNumber).ToList();
                var getJamClocks = (from xx in db.GameStopWatch
                                    where xx.StopwatchForId == id
                                    where xx.Type == (int)StopWatchTypeEnum.JamClock
                                    select xx).ToList();

                foreach (var jam in getJams)
                {
                    try
                    {
                        JamViewModel jvm = new JamViewModel(jam.JamNumber, jam.GameTimeElapsedMillisecondsStart, jam.CurrentPeriod);
                        var scoresT1 = game.ScoresTeam1.Where(x => x.JamNumber == jam.JamNumber);
                        foreach (var score in scoresT1)
                            jvm.TotalPointsForJamT1 += score.Points;

                        var scoresT2 = game.ScoresTeam2.Where(x => x.JamNumber == jam.JamNumber);
                        foreach (var score in scoresT2)
                            jvm.TotalPointsForJamT2 += score.Points;

                        var getClock = getJamClocks.Where(x => x.JamNumber == jam.JamNumber).FirstOrDefault();
                        if (getClock != null)
                        {
                            StopwatchWrapper stop = new StopwatchWrapper();
                            stop.IsClockAtZero = getClock.IsClockAtZero == 1 ? true : false;
                            stop.IsRunning = getClock.IsRunning == 1 ? true : false;
                            stop.StartTime = getClock.StartDateTime;
                            stop.TimeElapsed = getClock.TimeElapsed;
                            stop.TimeRemaining = getClock.TimeRemaining;
                            stop.TimerLength = getClock.Length;
                            jvm.JamClock = stop;
                        }

                        if (jam.Blocker1Team1 != null)
                        {
                            TeamMembersViewModel tmvm = new TeamMembersViewModel();
                            tmvm.SkaterId = jam.Blocker1Team1.GameMemberId;
                            tmvm.SkaterLinkId = jam.Blocker1Team1.MemberLinkId;
                            tmvm.SkaterName = jam.Blocker1Team1.MemberName;
                            tmvm.SkaterNumber = jam.Blocker1Team1.MemberNumber;
                            jvm.Blocker1T1 = tmvm;
                        }
                        if (jam.Blocker2Team1 != null)
                        {
                            TeamMembersViewModel tmvm = new TeamMembersViewModel();
                            tmvm.SkaterId = jam.Blocker2Team1.GameMemberId;
                            tmvm.SkaterLinkId = jam.Blocker2Team1.MemberLinkId;
                            tmvm.SkaterName = jam.Blocker2Team1.MemberName;
                            tmvm.SkaterNumber = jam.Blocker2Team1.MemberNumber;
                            jvm.Blocker2T1 = tmvm;
                        }
                        if (jam.Blocker3Team1 != null)
                        {
                            TeamMembersViewModel tmvm = new TeamMembersViewModel();
                            tmvm.SkaterId = jam.Blocker3Team1.GameMemberId;
                            tmvm.SkaterLinkId = jam.Blocker3Team1.MemberLinkId;
                            tmvm.SkaterName = jam.Blocker3Team1.MemberName;
                            tmvm.SkaterNumber = jam.Blocker3Team1.MemberNumber;
                            jvm.Blocker3T1 = tmvm;
                        }
                        if (jam.Blocker4Team1 != null)
                        {
                            TeamMembersViewModel tmvm = new TeamMembersViewModel();
                            tmvm.SkaterId = jam.Blocker4Team1.GameMemberId;
                            tmvm.SkaterLinkId = jam.Blocker4Team1.MemberLinkId;
                            tmvm.SkaterName = jam.Blocker4Team1.MemberName;
                            tmvm.SkaterNumber = jam.Blocker4Team1.MemberNumber;
                            jvm.Blocker4T1 = tmvm;
                        }
                        if (jam.PivotTeam1 != null)
                        {
                            TeamMembersViewModel tmvm = new TeamMembersViewModel();
                            tmvm.SkaterId = jam.PivotTeam1.GameMemberId;
                            tmvm.SkaterLinkId = jam.PivotTeam1.MemberLinkId;
                            tmvm.SkaterName = jam.PivotTeam1.MemberName;
                            tmvm.SkaterNumber = jam.PivotTeam1.MemberNumber;
                            jvm.PivotT1 = tmvm;
                        }
                        if (jam.JammerTeam1 != null)
                        {
                            TeamMembersViewModel tmvm = new TeamMembersViewModel();
                            tmvm.SkaterId = jam.JammerTeam1.GameMemberId;
                            tmvm.SkaterLinkId = jam.JammerTeam1.MemberLinkId;
                            tmvm.SkaterName = jam.JammerTeam1.MemberName;
                            tmvm.SkaterNumber = jam.JammerTeam1.MemberNumber;
                            jvm.JammerT1 = tmvm;
                        }
                        if (jam.Blocker1Team2 != null)
                        {
                            TeamMembersViewModel tmvm = new TeamMembersViewModel();
                            tmvm.SkaterId = jam.Blocker1Team2.GameMemberId;
                            tmvm.SkaterLinkId = jam.Blocker1Team2.MemberLinkId;
                            tmvm.SkaterName = jam.Blocker1Team2.MemberName;
                            tmvm.SkaterNumber = jam.Blocker1Team2.MemberNumber;
                            jvm.Blocker1T2 = tmvm;
                        }
                        if (jam.Blocker2Team2 != null)
                        {
                            TeamMembersViewModel tmvm = new TeamMembersViewModel();
                            tmvm.SkaterId = jam.Blocker2Team2.GameMemberId;
                            tmvm.SkaterLinkId = jam.Blocker2Team2.MemberLinkId;
                            tmvm.SkaterName = jam.Blocker2Team2.MemberName;
                            tmvm.SkaterNumber = jam.Blocker2Team2.MemberNumber;
                            jvm.Blocker2T2 = tmvm;
                        }
                        if (jam.Blocker3Team2 != null)
                        {
                            TeamMembersViewModel tmvm = new TeamMembersViewModel();
                            tmvm.SkaterId = jam.Blocker3Team2.GameMemberId;
                            tmvm.SkaterLinkId = jam.Blocker3Team2.MemberLinkId;
                            tmvm.SkaterName = jam.Blocker3Team2.MemberName;
                            tmvm.SkaterNumber = jam.Blocker3Team2.MemberNumber;
                            jvm.Blocker3T2 = tmvm;
                        }
                        if (jam.Blocker4Team2 != null)
                        {
                            TeamMembersViewModel tmvm = new TeamMembersViewModel();
                            tmvm.SkaterId = jam.Blocker4Team2.GameMemberId;
                            tmvm.SkaterLinkId = jam.Blocker4Team2.MemberLinkId;
                            tmvm.SkaterName = jam.Blocker4Team2.MemberName;
                            tmvm.SkaterNumber = jam.Blocker4Team2.MemberNumber;
                            jvm.Blocker4T2 = tmvm;
                        }
                        if (jam.PivotTeam2 != null)
                        {
                            TeamMembersViewModel tmvm = new TeamMembersViewModel();
                            tmvm.SkaterId = jam.PivotTeam2.GameMemberId;
                            tmvm.SkaterLinkId = jam.PivotTeam2.MemberLinkId;
                            tmvm.SkaterName = jam.PivotTeam2.MemberName;
                            tmvm.SkaterNumber = jam.PivotTeam2.MemberNumber;
                            jvm.PivotT2 = tmvm;
                        }
                        if (jam.JammerTeam2 != null)
                        {
                            TeamMembersViewModel tmvm = new TeamMembersViewModel();
                            tmvm.SkaterId = jam.JammerTeam2.GameMemberId;
                            tmvm.SkaterLinkId = jam.JammerTeam2.MemberLinkId;
                            tmvm.SkaterName = jam.JammerTeam2.MemberName;
                            tmvm.SkaterNumber = jam.JammerTeam2.MemberNumber;
                            jvm.JammerT2 = tmvm;
                        }

                        //gets all the lead jammers for this particular jam.
                        var getLeadJammers = jam.LeadJammers.ToList();
                        foreach (var lJam in getLeadJammers)
                        {
                            try
                            {
                                if (lJam.GameMemberId != new Guid())
                                {
                                    LeadJammerViewModel ljvm = new LeadJammerViewModel();
                                    ljvm.GameTimeInMilliseconds = lJam.GameTimeInMilliseconds;
                                    ljvm.Jammer = new TeamMembersViewModel();
                                    ljvm.Jammer.SkaterId = lJam.GameMemberId;
                                    ljvm.JamTimeInMilliseconds = lJam.JamTimeInMilliseconds;
                                    ljvm.GameLeadJamId = lJam.GameJamLeadId;
                                    jvm.LeadJammers.Add(ljvm);
                                }
                            }
                            catch (Exception exception)
                            { ErrorDatabaseManager.AddException(exception, exception.GetType()); }
                        }

                        game.Jams.Add(jvm);
                    }
                    catch (Exception exception)
                    { ErrorDatabaseManager.AddException(exception, exception.GetType()); }
                }


                var Penalties = getGame.GameMemberPenaltyBox.ToList();

                for (int i = 0; i < Penalties.Count; i++)
                {
                    SkaterInPenaltyBoxViewModel skater = new SkaterInPenaltyBoxViewModel();
                    skater.GameTimeInMillisecondsReleased = Penalties[i].GameTimeMilliSecondsReturned;
                    skater.GameTimeInMillisecondsSent = Penalties[i].GameTimeMilliSecondsSent;
                    skater.JamNumberReleased = Penalties[i].JamNumberReturned;
                    skater.JamNumberSent = Penalties[i].JamNumberSent;
                    skater.JamTimeInMillisecondsReleased = Penalties[i].JamTimeMilliSecondsReturned;
                    skater.JamTimeInMillisecondsSent = Penalties[i].JamTimeMilliSecondsSent;
                    skater.PenaltyId = Penalties[i].PenaltyIdFromGame;
                    skater.PenaltyScale = (PenaltyScaleEnum)Enum.ToObject(typeof(PenaltyScaleEnum), (int)Penalties[i].PenaltyScale);
                    skater.PenaltyType = (PenaltiesEnum)Enum.Parse(typeof(PenaltiesEnum), Penalties[i].PenaltyType);
                    skater.PenaltyNumberForSkater = Penalties[i].PenaltyNumberForSkater;
                    bool checkIfPenaltyIsAssigned = false;


                    for (int j = 0; j < game.Team1.TeamMembers.Count; j++)
                    {
                        if (game.Team1.TeamMembers[j].SkaterId == Penalties[i].Member.GameMemberId)
                        {
                            skater.PlayerSentToBox = game.Team1.TeamMembers[j];
                            game.Team1.TeamMembers[j].Penalties.Add(new PenaltyViewModel(skater.PenaltyType));
                            checkIfPenaltyIsAssigned = true;
                            break;
                        }
                    }
                    if (!checkIfPenaltyIsAssigned)
                    {
                        for (int j = 0; j < game.Team2.TeamMembers.Count; j++)
                        {
                            if (game.Team2.TeamMembers[j].SkaterId == Penalties[i].Member.GameMemberId)
                            {
                                skater.PlayerSentToBox = game.Team2.TeamMembers[j];
                                game.Team2.TeamMembers[j].Penalties.Add(new PenaltyViewModel(skater.PenaltyType));
                                checkIfPenaltyIsAssigned = true;
                                break;
                            }
                        }
                    }

                    game.PenaltyBox.Add(skater);
                }

                var getPolicies = getGame.GamePolicy;
                if (getPolicies != null)
                {
                    if (game.Policy == null)
                        game.Policy = PolicyViewModel.Instance;
                    game.Policy.AdChangeAutomaticallyChangeImage = getPolicies.AdChangeAutomaticallyChangeImage == 0 ? false : true;
                    game.Policy.AdChangeDisplayChangesInMilliSeconds = (long)getPolicies.AdChangeDisplayChangesInMilliSeconds;
                    game.Policy.AdChangeShowAdsDuringIntermission = getPolicies.AdChangeShowAdsDuringIntermission == 0 ? false : true;
                    game.Policy.AdChangeShowAdsRandomly = getPolicies.AdChangeShowAdsRandomly == 0 ? false : true;
                    game.Policy.AdChangeUseLineUpClock = getPolicies.AdChangeUseLineUpClock == 0 ? false : true;
                    game.Policy.AlwaysShowJamClock = getPolicies.AlwaysShowJamClock == 0 ? false : true;
                    game.Policy.EnableAdChange = getPolicies.EnableAdChange == 0 ? false : true;
                    game.Policy.EnableIntermissionNaming = getPolicies.EnableIntermissionNaming == 0 ? false : true;
                    game.Policy.EnableIntermissionStartOfClock = getPolicies.EnableIntermissionStartOfClock == 0 ? false : true;
                    game.Policy.FirstIntermissionNameConfirmedText = getPolicies.FirstIntermissionNameConfirmedText;
                    game.Policy.FirstIntermissionNameText = getPolicies.FirstIntermissionNameText;
                    game.Policy.GameSelectionType = (GameTypeEnum)Enum.Parse(typeof(GameTypeEnum), getPolicies.GameSelectionType);
                    game.Policy.HideClockTimeAfterBout = getPolicies.HideClockTimeAfterBout == 0 ? false : true;
                    game.Policy.IntermissionOtherText = getPolicies.IntermissionOtherText;
                    game.Policy.IntermissionStartOfClockInMilliseconds = (long)getPolicies.IntermissionStartOfClockInMilliseconds;
                    game.Policy.IntermissionStopClockEnable = getPolicies.IntermissionStopClockEnable == 0 ? false : true;
                    game.Policy.IntermissionStopClockIncrementJamNumber = getPolicies.IntermissionStopClockIncrementJamNumber == 0 ? false : true;
                    game.Policy.IntermissionStopClockIncrementPeriodNumber = getPolicies.IntermissionStopClockIncrementPeriodNumber == 0 ? false : true;
                    game.Policy.IntermissionStopClockResetJamNumber = getPolicies.IntermissionStopClockResetJamNumber == 0 ? false : true;
                    game.Policy.IntermissionStopClockResetJamTime = getPolicies.IntermissionStopClockResetJamTime == 0 ? false : true;
                    game.Policy.IntermissionStopClockResetPeriodNumber = getPolicies.IntermissionStopClockResetPeriodNumber == 0 ? false : true;
                    game.Policy.IntermissionStopClockResetPeriodTime = getPolicies.IntermissionStopClockResetPeriodTime == 0 ? false : true;
                    game.Policy.JamClockControlsLineUpClock = getPolicies.JamClockControlsLineUpClock == 0 ? false : true;
                    game.Policy.JamClockControlsTeamPositions = getPolicies.JamClockControlsTeamPositions == 0 ? false : true;
                    game.Policy.JamClockTimePerJam = (long)getPolicies.JamClockTimePerJam;
                    game.Policy.LineupClockControlsStartJam = getPolicies.LineupClockControlsStartJam == 0 ? false : true;
                    game.Policy.LineUpClockPerJam = (long)getPolicies.LineUpClockPerJam;
                    game.Policy.NumberOfPeriods = (int)getPolicies.NumberOfPeriods;
                    game.Policy.PenaltyBoxControlsLeadJammer = getPolicies.PenaltyBoxControlsLeadJammer == 0 ? false : true;
                    game.Policy.PeriodClock = (long)getPolicies.PeriodClock;
                    game.Policy.PeriodClockControlsLineupJamClock = getPolicies.PeriodClockControlsLineupJamClock == 0 ? false : true;
                    game.Policy.SecondIntermissionNameConfirmedText = getPolicies.SecondIntermissionNameConfirmedText;
                    game.Policy.SecondIntermissionNameText = getPolicies.SecondIntermissionNameText;
                    game.Policy.TimeOutClock = (long)getPolicies.TimeOutClock;
                    game.Policy.TimeoutClockControlsLineupClock = getPolicies.TimeoutClockControlsLineupClock == 0 ? false : true;
                    game.Policy.TimeOutsPerPeriod = (int)getPolicies.TimeOutsPerPeriod;
                }




                //TODO: get game adverts.


                var getTimeOuts = getGame.GameTimeouts.ToList();
                foreach (var time in getTimeOuts)
                {
                    TimeOutViewModel tvm = new TimeOutViewModel();

                    tvm.TimeoutId = time.TimeOutId;
                    if (time.GameTeam != null && time.GameTeam.TeamId == game.Team1.TeamId)
                        tvm.TimeOutType = TimeOutTypeEnum.Team1;
                    else if (time.GameTeam != null && time.GameTeam.TeamId == game.Team2.TeamId)
                        tvm.TimeOutType = TimeOutTypeEnum.Team2;
                    else
                        tvm.TimeOutType = TimeOutTypeEnum.Offical;

                    if (game.TimeOuts == null)
                        game.TimeOuts = new List<TimeOutViewModel>();

                    game.TimeOuts.Add(tvm);
                }

                return game;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }


        private static void insertNewPoliciesIntoDb(GameViewModel game, ManagementContext db)
        {
            try
            {
                var g = db.Games.Where(x => x.GameId == game.GameId).First();

                if (g.GamePolicy == null)
                    g.GamePolicy = new GamePolicy();
                g.GamePolicy.AdChangeAutomaticallyChangeImage = game.Policy.AdChangeAutomaticallyChangeImage ? 1 : 0;
                g.GamePolicy.AdChangeDisplayChangesInMilliSeconds = game.Policy.AdChangeDisplayChangesInMilliSeconds;
                g.GamePolicy.AdChangeShowAdsDuringIntermission = game.Policy.AdChangeShowAdsDuringIntermission ? 1 : 0;
                g.GamePolicy.AdChangeShowAdsRandomly = game.Policy.AdChangeShowAdsRandomly ? 1 : 0;
                g.GamePolicy.AdChangeUseLineUpClock = game.Policy.AdChangeUseLineUpClock ? 1 : 0;
                g.GamePolicy.AlwaysShowJamClock = game.Policy.AlwaysShowJamClock ? 1 : 0;
                g.GamePolicy.Created = DateTime.UtcNow;
                g.GamePolicy.EnableAdChange = game.Policy.EnableAdChange ? 1 : 0;
                g.GamePolicy.EnableIntermissionNaming = game.Policy.EnableIntermissionNaming ? 1 : 0;
                g.GamePolicy.EnableIntermissionStartOfClock = game.Policy.EnableIntermissionStartOfClock ? 1 : 0;
                g.GamePolicy.FirstIntermissionNameConfirmedText = game.Policy.FirstIntermissionNameConfirmedText;
                g.GamePolicy.FirstIntermissionNameText = game.Policy.FirstIntermissionNameText;
                g.GamePolicy.GameSelectionType = game.Policy.GameSelectionType.ToString();
                g.GamePolicy.HideClockTimeAfterBout = game.Policy.HideClockTimeAfterBout ? 1 : 0;
                g.GamePolicy.IntermissionOtherText = game.Policy.IntermissionOtherText;
                g.GamePolicy.IntermissionStartOfClockInMilliseconds = game.Policy.IntermissionStartOfClockInMilliseconds;
                g.GamePolicy.IntermissionStopClockEnable = game.Policy.IntermissionStopClockEnable ? 1 : 0;
                g.GamePolicy.OfficialTimeOutKeyShortcut = game.Policy.OfficialTimeOutKeyShortcut.ToString();
                g.GamePolicy.StartJamKeyShortcut = game.Policy.StartJamKeyShortcut.ToString();
                g.GamePolicy.StopJamKeyShortcut = game.Policy.StopJamKeyShortcut.ToString();
                g.GamePolicy.Team1LeadJammerKeyShortcut = game.Policy.Team1LeadJammerKeyShortcut.ToString();
                g.GamePolicy.Team1ScoreDownKeyShortcut = game.Policy.Team1ScoreDownKeyShortcut.ToString();
                g.GamePolicy.Team1ScoreUpKeyShortcut = game.Policy.Team1ScoreUpKeyShortcut.ToString();
                g.GamePolicy.Team1TimeOutKeyShortcut = game.Policy.Team1TimeOutKeyShortcut.ToString();
                g.GamePolicy.Team2LeadJammerKeyShortcut = game.Policy.Team2LeadJammerKeyShortcut.ToString();
                g.GamePolicy.Team2ScoreDownKeyShortcut = game.Policy.Team2ScoreDownKeyShortcut.ToString();
                g.GamePolicy.Team2ScoreUpKeyShortcut = game.Policy.Team2ScoreUpKeyShortcut.ToString();
                g.GamePolicy.Team2TimeOutKeyShortcut = game.Policy.Team2TimeOutKeyShortcut.ToString();
                g.GamePolicy.StopLineUpClockAtZero = game.Policy.StopLineUpClockAtZero ? 1 : 0;
                g.GamePolicy.StopPeriodClockWhenLineUpClockHitsZero = game.Policy.StopPeriodClockWhenLineUpClockHitsZero ? 1 : 0;
                db.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        /// <summary>
        /// inserts game links into the DB.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="db"></param>
        /// <param name="gameNew"></param>
        private static void InsertGameLinks(GameViewModel game, ManagementContext db, DataModels.Game.Game gameNew)
        {
            if (game.GameLinks != null && game.GameLinks.Count > 0)
            {
                foreach (var link in game.GameLinks)
                {
                    var linkDb = db.GameLinks.Where(x => x.GameLinkId == link.LinkId).FirstOrDefault();

                    if (linkDb == null)
                    {
                        GameLink links = new GameLink();
                        links.Game = gameNew;
                        links.GameLinkId = link.LinkId;
                        links.Link = link.GameLink;
                        int type = Convert.ToInt32(link.LinkType);
                        links.LinkType = type;
                        db.GameLinks.Add(links);
                        db.SaveChanges();
                    }
                    else
                    {
                        linkDb.Link = link.GameLink;
                        int type = Convert.ToInt32(link.LinkType);
                        linkDb.LinkType = type;
                        db.SaveChanges();
                    }
                }
            }
            db.SaveChanges();
        }
        /// <summary>
        ///         /// entry point for saving a game to the DB.
        /// returns if the game is published online.
        /// </summary>
        /// <param name="game"></param>
        /// <returns>the fact the game was actually saved.</returns>
        public static DataModels.Game.Game saveGameToDb(GameViewModel game)
        {
            try
            {
                if (game != null)
                {
                    ManagementContext db = new ManagementContext();
                    var gameId = (from xx in db.Games
                                  where xx.GameId == game.GameId
                                  select xx).FirstOrDefault();
                    if (gameId == null)
                    {
                        var gameNew = insertNewGameToDb(game, db);
                        return gameNew;
                    }
                    else
                    {
                        deepCompareAndInsertDb(game, db, gameId);
                        return gameId;

                    }
                }
            }
            catch (Exception e)
            {
                Library.Classes.Error.ErrorDatabaseManager.AddException(e, e.GetType(), ErrorGroupEnum.Database);
            }
            return null;
        }
        private static DataModels.Game.Game insertNewGameToDb(GameViewModel game, ManagementContext db)
        {
            DataModels.Game.Game gameNew = new DataModels.Game.Game();
            gameNew.IdForOnlineManagementUse = game.IdForOnlineManagementUse;
            gameNew.GameDate = game.GameDate;
            gameNew.LastModified = DateTime.UtcNow;

            if (game.GameEndDate != DateTime.MinValue)
                gameNew.GameEndDate = game.GameEndDate;
            else
                gameNew.GameEndDate = game.GameDate;

            //debug or live mode.  Debug is for developing it, so users never see this mode and only open to developers.
            gameNew.ScoreboardType = Convert.ToInt32(game.ScoreboardMode);
            if (!String.IsNullOrEmpty(game.GameName))
                gameNew.GameName = game.GameName;
            else
                gameNew.GameName = ScoreboardConfig.DEFAULT_GAME_NAME;
            if (game.Policy != null)
                gameNew.GameType = game.Policy.GameSelectionType.ToString();
            gameNew.IsGameOver = game.HasGameEnded;
            gameNew.HasGameStarted = game.HasGameStarted;
            gameNew.IsGameLive = game.PublishGameOnline;
            gameNew.IsGameScrimmage = game.SaveGameOnline;
            gameNew.GameLocation = game.GameLocation;
            gameNew.GameCity = game.GameCity;
            gameNew.GameState = game.GameState;
            gameNew.GameId = game.GameId;
            gameNew.VersionNumber = game.VersionNumber;


            gameNew.IsGamePublishedOnline = game.PublishGameOnline;
            gameNew.Created = DateTime.UtcNow;
            if (game.FederationId != new Guid())
            {
                GameFederationOwnership fedOwner = new GameFederationOwnership();
                fedOwner.Federation = db.Federations.Where(x => x.FederationId == game.FederationId).FirstOrDefault();
                fedOwner.Game = gameNew;
                fedOwner.OwnerType = Convert.ToInt32(GameOwnerEnum.Owner);
                gameNew.FederationOwners.Add(fedOwner);
            }
            db.Games.Add(gameNew);
            int c = db.SaveChanges();

            InsertGameLinks(game, db, gameNew);

            //inserts the current jam into the DB.
            if (game.CurrentJam != null)
            {
                GameJamClass.insertNewJamIntoDb(game.GameId, game.Team1.TeamId, game.Team2.TeamId, game.CurrentJam, db, gameNew);
            }
            c += db.SaveChanges();

            if (game.CurrentLineUpClock != null && game.CurrentLineUpClock.StartTime != new DateTime())
            {
                GameStopwatch stop = new GameStopwatch();
                stop.Created = DateTime.UtcNow;
                stop.IsClockAtZero = game.CurrentLineUpClock.IsClockAtZero == true ? 1 : 0;
                stop.IsRunning = game.CurrentLineUpClock.IsRunning == true ? 1 : 0;
                stop.Length = game.CurrentLineUpClock.TimerLength;
                stop.StartDateTime = game.CurrentLineUpClock.StartTime;
                stop.StopwatchForId = game.GameId;
                stop.TimeElapsed = game.CurrentLineUpClock.TimeElapsed;
                stop.TimeRemaining = game.CurrentLineUpClock.TimeRemaining;
                stop.Type = (int)StopWatchTypeEnum.LineUpClock;
                stop.Game = gameNew;
                db.GameStopWatch.Add(stop);
                c += db.SaveChanges();
            }
            if (game.CurrentTimeOutClock != null && game.CurrentTimeOutClock.StartTime != new DateTime())
            {
                GameStopwatch stop = new GameStopwatch();
                stop.Created = DateTime.UtcNow;
                stop.IsClockAtZero = game.CurrentTimeOutClock.IsClockAtZero == true ? 1 : 0;
                stop.IsRunning = game.CurrentTimeOutClock.IsRunning == true ? 1 : 0;
                stop.Length = game.CurrentTimeOutClock.TimerLength;
                stop.StartDateTime = game.CurrentTimeOutClock.StartTime;
                stop.StopwatchForId = game.GameId;
                stop.TimeElapsed = game.CurrentTimeOutClock.TimeElapsed;
                stop.TimeRemaining = game.CurrentTimeOutClock.TimeRemaining;
                stop.Type = (int)StopWatchTypeEnum.TimeOutClock;
                stop.Game = gameNew;
                db.GameStopWatch.Add(stop);
                c += db.SaveChanges();
            }
            if (game.IntermissionClock != null && game.IntermissionClock.StartTime != new DateTime())
            {
                GameStopwatch stop = new GameStopwatch();
                stop.Created = DateTime.UtcNow;
                stop.IsClockAtZero = game.IntermissionClock.IsClockAtZero == true ? 1 : 0;
                stop.IsRunning = game.IntermissionClock.IsRunning == true ? 1 : 0;
                stop.Length = game.IntermissionClock.TimerLength;
                stop.StartDateTime = game.IntermissionClock.StartTime;
                stop.StopwatchForId = game.GameId;
                stop.TimeElapsed = game.IntermissionClock.TimeElapsed;
                stop.TimeRemaining = game.IntermissionClock.TimeRemaining;
                stop.Type = (int)StopWatchTypeEnum.IntermissionClock;
                stop.Game = gameNew;
                db.GameStopWatch.Add(stop);
                c += db.SaveChanges();
            }
            try
            {
                if (game.PeriodClock != null && game.PeriodClock.StartTime != new DateTime())
                {
                    GameStopwatch stop = new GameStopwatch();
                    stop.Created = DateTime.UtcNow;
                    stop.IsClockAtZero = game.PeriodClock.IsClockAtZero == true ? 1 : 0;
                    stop.IsRunning = game.PeriodClock.IsRunning == true ? 1 : 0;
                    stop.Length = game.PeriodClock.TimerLength;
                    stop.StartDateTime = game.PeriodClock.StartTime;
                    stop.StopwatchForId = game.GameId;
                    stop.TimeElapsed = game.PeriodClock.TimeElapsed;
                    stop.TimeRemaining = game.PeriodClock.TimeRemaining;
                    stop.Type = (int)StopWatchTypeEnum.PeriodClock;
                    stop.Game = gameNew;
                    db.GameStopWatch.Add(stop);
                    c += db.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            //teams need to be inserted first before the jams...
            GameTeamClass.insertTeamIntoDb(game, game.Team1, db, gameNew);

            GameTeamClass.insertTeamIntoDb(game, game.Team2, db, gameNew);

            foreach (var jam in game.Jams)
            {
                GameJamClass.insertNewJamIntoDb(game.GameId, game.Team1.TeamId, game.Team2.TeamId, jam, db, gameNew);
            }
            foreach (var advert in game.Advertisements)
            {
                //TODO: save adverts, but we need to upload these from the scoreboard.
            }
            foreach (var timeout in game.TimeOuts)
            {
                insertTimeoutsIntoDb(game, timeout, db, gameNew);
            }
            foreach (var score in game.ScoresTeam1)
            {
                GameScoreClass.insertScoreIntoDb(game.Team1.TeamId, game, score, db, gameNew);
            }
            foreach (var score in game.ScoresTeam2)
            {
                GameScoreClass.insertScoreIntoDb(game.Team2.TeamId, game, score, db, gameNew);
            }
            for (int i = 0; i < game.AssistsForTeam1.Count; i++)
            {
                GameAssistsClass.insertAssistIntoDb(game.Team1.TeamId, game, game.AssistsForTeam1[i], db, gameNew);
            }
            for (int i = 0; i < game.AssistsForTeam2.Count; i++)
            {
                GameAssistsClass.insertAssistIntoDb(game.Team2.TeamId, game, game.AssistsForTeam2[i], db, gameNew);
            }
            for (int i = 0; i < game.BlocksForTeam1.Count; i++)
            {
                GameBlocksClass.insertBlockIntoDb(game.Team1.TeamId, game, game.BlocksForTeam1[i], db, gameNew);
            }
            for (int i = 0; i < game.BlocksForTeam2.Count; i++)
            {
                GameBlocksClass.insertBlockIntoDb(game.Team2.TeamId, game, game.BlocksForTeam2[i], db, gameNew);
            }
            for (int i = 0; i < game.OfficialReviews.Count; i++)
            {
                OfficialReviewsClass.AddOfficialReview(game, gameNew, game.OfficialReviews[i]);
            }
            c += db.SaveChanges();
            //penalties added for the team.
            for (int i = 0; i < game.PenaltiesForTeam1.Count; i++)
            {
                GamePenaltiesClass.insertPenaltyIntoDb(game.Team1.TeamId, game, game.PenaltiesForTeam1[i], db, gameNew);
            }
            for (int i = 0; i < game.PenaltiesForTeam2.Count; i++)
            {
                GamePenaltiesClass.insertPenaltyIntoDb(game.Team2.TeamId, game, game.PenaltiesForTeam2[i], db, gameNew);
            }
            //times the skater went to the box.
            foreach (var pen in game.PenaltyBox)
            {
                GamePenaltiesClass.insertNewPenaltyIntoDb(game, db, pen, gameNew);
            }
            if (game.Policy != null)
            {
                insertNewPoliciesIntoDb(game, db);
            }
            if (game.Officials != null)
            {
                GameOfficialsClass.AddOfficialsToDb(game, db, gameNew);
            }

            c += db.SaveChanges();
            return gameNew;
        }



        /// <summary>
        /// does a deep compare of all the values between the cached game and the not cached game.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="cachedGame"></param>
        /// <param name="db"></param>
        /// <param name="gdb"></param>
        private static void deepCompareAndInsertDb(GameViewModel game, ManagementContext db, DataModels.Game.Game gdb)
        {
            gdb = (from xx in db.Games.Include("GameOfficials").Include("GameLinks").Include("GameMemberPenalties").Include("GameMemberAssists").Include("GameMemberBlocks").Include("GameTeams").Include("GameTeams.LineupSettings").Include("GameJams").Include("GameJams.LeadJammers").Include("GameJams.JamPasses").Include("GamePolicy").Include("GameScores").Include("GameTimeouts")
                   where xx.GameId == game.GameId
                   select xx).FirstOrDefault();

            //todo:refactor, its too big of a method
            //todo: compare adverts
            //todo save all the currents....

            GameTeamClass.updateTeam(game, game.Team1, db, gdb);
            GameTeamClass.updateTeam(game, game.Team2, db, gdb);

            InsertGameLinks(game, db, gdb);

            gdb.IdForOnlineManagementUse = game.IdForOnlineManagementUse;
            gdb.GameDate = game.GameDate;
            if (game.GameEndDate != new DateTime())
                gdb.GameEndDate = game.GameEndDate;
            gdb.ScoreboardType = Convert.ToInt32(game.ScoreboardMode);
            gdb.ElapsedGameTimeInMilliseconds = game.ElapsedTimeGameClockMilliSeconds;
            if (!String.IsNullOrEmpty(game.GameName))
                gdb.GameName = game.GameName;
            else
                gdb.GameName = ScoreboardConfig.DEFAULT_GAME_NAME;
            gdb.HasGameStarted = game.HasGameStarted;
            gdb.IsGameOver = game.HasGameEnded;
            gdb.IsGameScrimmage = game.SaveGameOnline;
            gdb.GameLocation = game.GameLocation;
            if (!gdb.IsGamePublishedOnline)
                gdb.IsGamePublishedOnline = game.PublishGameOnline;
            gdb.LastModified = DateTime.UtcNow;
            int c = db.SaveChanges();

            //updates the team members for the game

            foreach (var mem in game.Team1.TeamMembers)
            {
                try
                {
                    var team = gdb.GameTeams.Where(x => x.TeamId == game.Team1.TeamId).FirstOrDefault();
                    if (team == null)
                        GameTeamClass.insertTeamIntoDb(game, game.Team1, db, gdb);
                    var skater = team.GameMembers.Where(x => x.GameMemberId == mem.SkaterId).FirstOrDefault();

                    if (skater == null)
                        GameMemberClass.insertNewSkater(game.GameId, db, game.Team1.TeamId, mem, gdb.GameTeams.Where(x => x.TeamId == game.Team1.TeamId).First());
                    else
                    {
                        if (skater.MemberName != mem.SkaterName || skater.MemberNumber != mem.SkaterNumber)
                            skater.LastModified = DateTime.UtcNow;
                        if (!String.IsNullOrEmpty(mem.SkaterName))
                            skater.MemberName = mem.SkaterName;
                        else
                            skater.MemberName = "NA";
                        skater.MemberNumber = mem.SkaterNumber;

                        if (mem.SkaterPictureCompressed != null && !String.IsNullOrEmpty(mem.SkaterPictureLocation))
                        {
                            if (skater.Photos.Count == 0)
                            {
                                try
                                {
                                    Stream stream = new MemoryStream(mem.SkaterPictureCompressed);
                                    FileInfo file = new FileInfo(mem.SkaterPictureLocation);
                                    RDN.Library.Classes.Account.User.AddMemberPhotoForGame(game.GameId, stream, file.FullName, mem.SkaterId, MemberTypeEnum.Skater);
                                }
                                catch (Exception exception)
                                {
                                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                                }
                            }
                        }
                        c += db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Library.Classes.Error.ErrorDatabaseManager.AddException(e, e.GetType(), ErrorGroupEnum.Database);
                }
            }
            foreach (var mem in game.Team2.TeamMembers)
            {
                try
                {
                    var team = gdb.GameTeams.Where(x => x.TeamId == game.Team2.TeamId).FirstOrDefault();
                    if (team != null)
                    {
                        var skater = team.GameMembers.Where(x => x.GameMemberId == mem.SkaterId).FirstOrDefault();

                        if (skater == null)
                            GameMemberClass.insertNewSkater(game.GameId, db, game.Team2.TeamId, mem, gdb.GameTeams.Where(x => x.TeamId == game.Team2.TeamId).First());
                        else
                        {
                            if (skater.MemberName != mem.SkaterName || skater.MemberNumber != mem.SkaterNumber)
                                skater.LastModified = DateTime.UtcNow;
                            if (!String.IsNullOrEmpty(mem.SkaterName))
                                skater.MemberName = mem.SkaterName;
                            else
                                skater.MemberName = "NA";

                            skater.MemberNumber = mem.SkaterNumber;

                            if (mem.SkaterPictureCompressed != null && !String.IsNullOrEmpty(mem.SkaterPictureLocation))
                            {
                                if (skater.Photos.Count == 0)
                                {
                                    try
                                    {
                                        Stream stream = new MemoryStream(mem.SkaterPictureCompressed);
                                        FileInfo file = new FileInfo(mem.SkaterPictureLocation);
                                        RDN.Library.Classes.Account.User.AddMemberPhotoForGame(game.GameId, stream, file.FullName, mem.SkaterId, MemberTypeEnum.Skater);
                                    }
                                    catch (Exception exception)
                                    {
                                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                                    }
                                }
                            }
                            c += db.SaveChanges();
                        }
                    }
                }
                catch (Exception e)
                {
                    Library.Classes.Error.ErrorDatabaseManager.AddException(e, e.GetType(), ErrorGroupEnum.Database);
                }
            }

            if (game.CurrentJam != null)
            {
                GameJamClass.updateJamToDb(game, db, game.CurrentJam, gdb);
            }
            if (game.PeriodClock != null)
                updateClock(game.GameId, db, game.PeriodClock, StopWatchTypeEnum.PeriodClock, gdb);

            if (game.CurrentLineUpClock != null)
                updateClock(game.GameId, db, game.CurrentLineUpClock, StopWatchTypeEnum.LineUpClock, gdb);

            if (game.IntermissionClock != null)
                updateClock(game.GameId, db, game.IntermissionClock, StopWatchTypeEnum.IntermissionClock, gdb);

            if (game.CurrentTimeOutClock != null)
                updateClock(game.GameId, db, game.CurrentTimeOutClock, StopWatchTypeEnum.TimeOutClock, gdb);

            for (int i = 0; i < game.Jams.Count; i++)
            {
                GameJamClass.updateJamToDb(game, db, game.Jams[i], gdb);
            }
            for (int i = 0; i < game.PenaltyBox.Count; i++)
                GamePenaltiesClass.updatePenaltyBoxInDb(game, db, game.PenaltyBox[i], gdb);

            int scoreTeam1 = GameTeamClass.updateTeamScores(TeamNumberEnum.Team1, game.Team1.TeamId, game, db, gdb);
            int scoreTeam2 = GameTeamClass.updateTeamScores(TeamNumberEnum.Team2, game.Team2.TeamId, game, db, gdb);
            GameTeamClass.updateTeamScore(game.Team1.TeamId, game.GameId, game.CurrentTeam1Score, db, gdb);
            GameTeamClass.updateTeamScore(game.Team2.TeamId, game.GameId, game.CurrentTeam2Score, db, gdb);
            GameBlocksClass.updateTeamBlocks(TeamNumberEnum.Team1, game.Team1.TeamId, game, db, gdb);
            GameBlocksClass.updateTeamBlocks(TeamNumberEnum.Team2, game.Team2.TeamId, game, db, gdb);
            GamePenaltiesClass.updateTeamPenalties(TeamNumberEnum.Team1, game.Team1.TeamId, game, db, gdb);
            GamePenaltiesClass.updateTeamPenalties(TeamNumberEnum.Team2, game.Team2.TeamId, game, db, gdb);
            GameAssistsClass.updateTeamAssists(TeamNumberEnum.Team1, game.Team1.TeamId, game, db, gdb);
            GameAssistsClass.updateTeamAssists(TeamNumberEnum.Team2, game.Team2.TeamId, game, db, gdb);
            db.SaveChanges();
            GameOfficialsClass.DeepCompareOfficialsToDb(game, db, gdb);


            try
            {
                foreach (var time in game.TimeOuts)
                {
                    var temp = gdb.GameTimeouts.Where(x => x.TimeOutId == time.TimeoutId).FirstOrDefault();
                    if (temp == null)
                        insertTimeoutsIntoDb(game, time, db, gdb);
                    else
                    {
                        temp.TimeOutTime = time.TimeOutClock.TimeRemaining;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, e.GetType(), ErrorGroupEnum.Database, additionalInformation: "trying to save time outs");
            }
            db.SaveChanges();
        }
        /// <summary>
        /// removes a member from the game
        /// </summary>
        /// <param name="db"></param>
        /// <param name="teamId"></param>
        /// <param name="skaterId"></param>
        /// <param name="gameId"></param>
        public static int RemoveSkaterFromGame(Guid teamId, Guid skaterId)
        {
            ManagementContext db = new ManagementContext();
            var team = db.GameTeam.Where(x => x.TeamId == teamId).First();
            var skater = team.GameMembers.Where(x => x.GameMemberId == skaterId).FirstOrDefault();
            db.GameMembers.Remove(skater);
            return db.SaveChanges();
        }


        /// <summary>
        /// updates the stop watch clock for a chosen clock.
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="db"></param>
        /// <param name="watch"></param>
        /// <param name="type"></param>
        private static bool updateClock(Guid gameId, ManagementContext db, StopwatchWrapper watch, StopWatchTypeEnum type, DataModels.Game.Game g)
        {
            try
            {
                if (watch.StartTime != new DateTime())
                {
                    var findPeriod = (from xx in db.GameStopWatch
                                      where xx.StopwatchForId == gameId
                                      where xx.Type == (int)type
                                      select xx).FirstOrDefault();
                    if (findPeriod == null)
                    {
                        GameStopwatch stop = new GameStopwatch();
                        stop.Created = DateTime.UtcNow;
                        stop.IsClockAtZero = watch.IsClockAtZero == true ? 1 : 0;
                        stop.IsRunning = watch.IsRunning == true ? 1 : 0;
                        stop.Length = watch.TimerLength;
                        stop.StartDateTime = watch.StartTime;
                        stop.StopwatchForId = gameId;
                        stop.TimeElapsed = watch.TimeElapsed;
                        stop.TimeRemaining = watch.TimeRemaining;
                        stop.Type = (int)type;
                        stop.Game = g;
                        db.GameStopWatch.Add(stop);
                        int c = db.SaveChanges();
                        return c > 0;
                    }
                    else
                    {
                        findPeriod.Game = findPeriod.Game;
                        findPeriod.IsClockAtZero = watch.IsClockAtZero == true ? 1 : 0;
                        findPeriod.IsRunning = watch.IsRunning == true ? 1 : 0;
                        findPeriod.Length = watch.TimerLength;
                        findPeriod.StartDateTime = watch.StartTime;
                        findPeriod.TimeElapsed = watch.TimeElapsed;
                        findPeriod.TimeRemaining = watch.TimeRemaining;
                        int c = db.SaveChanges();
                        return c > 0;
                    }
                }
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, e.GetType(), ErrorGroupEnum.Database);
            }
            return false;

        }







        private static void insertTimeoutsIntoDb(GameViewModel game, TimeOutViewModel timeOuts, ManagementContext db, DataModels.Game.Game g)
        {
            try
            {
                GameTimeout time = new GameTimeout();
                time.Game = g;
                if (timeOuts.TimeOutType == TimeOutTypeEnum.Team1)
                    time.GameTeam = g.GameTeams.Where(x => x.TeamId == game.Team1.TeamId).First();
                else if (timeOuts.TimeOutType == TimeOutTypeEnum.Team2)
                    time.GameTeam = g.GameTeams.Where(x => x.TeamId == game.Team2.TeamId).First();
                time.TimeOutTime = timeOuts.TimeOutClock.TimeRemaining;
                time.TimeOutId = timeOuts.TimeoutId;
                db.GameTimeOut.Add(time);
                db.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }





    }
}
