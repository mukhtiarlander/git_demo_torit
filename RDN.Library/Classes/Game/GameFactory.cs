using RDN.Library.Classes.Error;
using RDN.Library.Classes.Federation.Enums;
using RDN.Library.DataModels.Context;
using RDN.Portable.Classes.Federation.Enums;
using RDN.Portable.Classes.Games;
using RDN.Portable.Models.Json.Games;
using RDN.Portable.Models.Json.Games.Enums;
using RDN.Utilities.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Game
{
    public class GameFactory
    {
        ManualGame rnScore = null;

        public static GameFactory Initialize()
        {
            return new GameFactory();
        }

        public GameFactory AddRNScore(ManualGame score)
        {
            this.rnScore = score;
            return this;
        }

        public void SaveChanges()
        {
            if (this.rnScore != null)
            {
                if (this.rnScore.ScoreId > 0)
                    UpdateScore();
                else
                    InsertScore();
            }
        }
        private ManualGame UpdateScore()
        {
            try
            {
                var dc = new ManagementContext();
                var s = dc.RNScores.Where(x => x.ScoreId == this.rnScore.ScoreId).FirstOrDefault();
                s.IsApproved = this.rnScore.IsApproved;
                s.IsPublished = this.rnScore.IsPublished;
                s.Team1Name = this.rnScore.Team1Name;
                s.Team1Score = this.rnScore.Team1Score;
                s.Team2Name = this.rnScore.Team2Name;
                s.Team2Score = this.rnScore.Team2Score;
                s.GameDateTime = this.rnScore.TimeEntered;
                s.RuleSetEnum = (long)this.rnScore.RuleSetEnum;
                s.SanctionedByFederationEnum = (long)this.rnScore.SanctionedByFederationEnum;
                s.EmailWhenBoutIsPosted = this.rnScore.EmailWhenBoutIsPosted;
                if (!String.IsNullOrEmpty(this.rnScore.Team1Id))
                {
                    Guid id = new Guid(this.rnScore.Team1Id);
                    s.League1 = dc.Leagues.Where(x => x.LeagueId == id).FirstOrDefault();
                }
                if (!String.IsNullOrEmpty(this.rnScore.Team2Id))
                {
                    Guid id = new Guid(this.rnScore.Team2Id);
                    s.League2 = dc.Leagues.Where(x => x.LeagueId == id).FirstOrDefault();
                }
                s.Notes = this.rnScore.Notes;
                if (this.rnScore.Tournament != null)
                    s.Tournament = dc.GameTournaments.Where(x => x.TournamentId == this.rnScore.Tournament.Id).FirstOrDefault();
                
                int c = dc.SaveChanges();
                this.rnScore.ScoreId = s.ScoreId;

                return this.rnScore;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return this.rnScore;
        }

        private ManualGame InsertScore()
        {
            try
            {
                var dc = new ManagementContext();
                DataModels.Game.RNScore s = new DataModels.Game.RNScore();
                s.IsApproved = this.rnScore.IsApproved;
                s.IsPublished = this.rnScore.IsPublished;
                s.Team1Name = this.rnScore.Team1Name;
                s.Team1Score = this.rnScore.Team1Score;
                s.Team2Name = this.rnScore.Team2Name;
                s.Team2Score = this.rnScore.Team2Score;
                s.GameDateTime = this.rnScore.TimeEntered;
                s.RuleSetEnum = (long)this.rnScore.RuleSetEnum;
                s.SanctionedByFederationEnum = (long)this.rnScore.SanctionedByFederationEnum;
                s.EmailWhenBoutIsPosted = this.rnScore.EmailWhenBoutIsPosted;
                if (!String.IsNullOrEmpty(this.rnScore.Team1Id))
                {
                    Guid id = new Guid(this.rnScore.Team1Id);
                    s.League1 = dc.Leagues.Where(x => x.LeagueId == id).FirstOrDefault();
                }
                if (!String.IsNullOrEmpty(this.rnScore.Team2Id))
                {
                    Guid id = new Guid(this.rnScore.Team2Id);
                    s.League2 = dc.Leagues.Where(x => x.LeagueId == id).FirstOrDefault();
                }
                s.Notes = this.rnScore.Notes;
                if (this.rnScore.Tournament != null)
                    s.Tournament = dc.GameTournaments.Where(x => x.TournamentId == this.rnScore.Tournament.Id).FirstOrDefault();
                dc.RNScores.Add(s);
                int c = dc.SaveChanges();
                this.rnScore.ScoreId = s.ScoreId;

                return this.rnScore;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return this.rnScore;
        }

        public List<CurrentGameJson> GetPastWeeksGames(int count, int page)
        {
            try
            {
                var dc = new ManagementContext();
                DateTime hours = DateTime.UtcNow.AddDays(-16);
                DateTime endDate = DateTime.UtcNow.AddDays(-16);
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
                                 Team2LogoUrl = xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().Logo == null ? "" : xx.GameTeams.OrderBy(x => x.TeamName).FirstOrDefault().Logo.ImageUrlThumb,
                                 GameLocationFrom = GameLocationFromEnum.SCOREBOARD
                             }).OrderByDescending(x => x.StartTime).Skip(count * page).Take(count).ToList();
                return games;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return new List<CurrentGameJson>();
        }
        public List<CurrentGameJson> GetPastWeeksGamesFromRN(int count, int page)
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
                                 StartTime = xx.GameDateTime.GetValueOrDefault(),
                                 GameLocationFrom = GameLocationFromEnum.ROLLINNEWS,
                                 ScoreId = xx.ScoreId,
                                 RuleSet = xx.RuleSetEnum.ToString(),
                                 SanctioningFederationType = xx.SanctionedByFederationEnum.ToString(),

                             }).OrderByDescending(x => x.StartTime).Skip(count * page).Take(count).ToList();
                return games;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return new List<CurrentGameJson>();
        }
        /// <summary>
        /// get games that were not entered by scoreboard.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public List<ManualGame> GetManualGames(int count, int skip)
        {
            try
            {
                if (count == 0) count = 10000;
                var dc = new ManagementContext();
                var games = (from xx in dc.RNScores
                             select new ManualGame
                             {
                                 Team1Name = xx.Team1Name,
                                 Team1Score = xx.Team1Score,
                                 Team2Name = xx.Team2Name,
                                 Team2Score = xx.Team2Score,
                                 ScoreId = xx.ScoreId,
                                 EmailWhenBoutIsPosted = xx.EmailWhenBoutIsPosted,
                                 Notes = xx.Notes,
                                 RuleSetEnum = (RuleSetsUsedEnum)xx.RuleSetEnum,
                                 SanctionedByFederationEnum = (FederationsEnum)xx.SanctionedByFederationEnum,
                                 TimeEntered = xx.GameDateTime,
                                 IsPublished = xx.IsPublished,
                                 IsApproved = xx.IsApproved,
                                 PublishDateTime = xx.PublishDateTime
                             }).OrderByDescending(x => x.TimeEntered).Skip(skip).Take(count).AsParallel().ToList();
                for (int i = 0; i < games.Count; i++)
                {
                    games[i].RuleSetEnumDisplay = games[i].RuleSetEnum.ToString();
                    games[i].SanctionedByFederationEnumDisplay = games[i].SanctionedByFederationEnum.ToString();
                }
                return games;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return new List<ManualGame>();
        }
        public ManualGame GetManualGame(long gameId)
        {
            try
            {
                var dc = new ManagementContext();
                DateTime hours = DateTime.UtcNow.AddDays(-16);
                DateTime endDate = DateTime.UtcNow.AddDays(-16);
                var games = dc.RNScores.Where(x => x.ScoreId == gameId).FirstOrDefault();

                var game = new ManualGame()
                              {
                                  Team1Name = games.Team1Name,
                                  Team1Score = games.Team1Score,

                                  Team2Name = games.Team2Name,
                                  Team2Score = games.Team2Score,
                                  ScoreId = games.ScoreId,
                                  EmailWhenBoutIsPosted = games.EmailWhenBoutIsPosted,
                                  Notes = games.Notes,
                                  RuleSetEnum = (RuleSetsUsedEnum)games.RuleSetEnum,
                                  SanctionedByFederationEnum = (FederationsEnum)games.SanctionedByFederationEnum,
                                  TimeEntered = games.GameDateTime,
                                  IsPublished = games.IsPublished,
                                  IsApproved = games.IsApproved,
                                  PublishDateTime = games.PublishDateTime,
                                  Team1Id = games.League1 == null ? "" : games.League1.LeagueId.ToString(),
                                  Team2Id = games.League2 == null ? "" : games.League2.LeagueId.ToString()
                              };
                game.RuleSetEnumDisplay = game.RuleSetEnum.ToString();
                game.SanctionedByFederationEnumDisplay = game.SanctionedByFederationEnum.ToString();
                return game;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), ErrorGroupEnum.Database);
            }
            return new ManualGame();
        }

    }
}
