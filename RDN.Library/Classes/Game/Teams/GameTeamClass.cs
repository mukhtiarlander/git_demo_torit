using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Game.Members;
using RDN.Library.DataModels.Context;
using Scoreboard.Library.Static.Enums;
using Scoreboard.Library.ViewModel;
using Scoreboard.Library.ViewModel.Members;

namespace RDN.Library.Classes.Game.Teams
{
    internal class GameTeamClass
    {
        /// <summary>
        /// inserts the team into the DB.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="team"></param>
        /// <param name="db"></param>
        public static void insertTeamIntoDb(GameViewModel game, TeamViewModel team, ManagementContext db, DataModels.Game.Game g)
        {
            try
            {
                DataModels.Game.GameTeam tm = new DataModels.Game.GameTeam();
                tm.Created = DateTime.UtcNow;
                tm.Game = g;
                if (String.IsNullOrEmpty(team.TeamName))
                    tm.TeamName = "Temp";
                else
                    tm.TeamName = team.TeamName;

                if (team.TeamId == new Guid())
                    tm.TeamId = Guid.NewGuid();
                else
                    tm.TeamId = team.TeamId;

                if (team.Logo != null && team.Logo.TeamLogoId != new Guid())
                {
                    var logo = db.TeamLogos.Where(x => x.TeamLogoId == team.Logo.TeamLogoId).FirstOrDefault();
                    if (logo != null)
                    {
                        tm.Logo = logo;
                        team.Logo.ImageUrl = logo.ImageUrl;
                        team.Logo.ImageUrlThumb = logo.ImageUrlThumb;
                    }
                }
                if (team.LineUpSettings != null)
                {
                    UpdateLineUpSettings(team, tm);
                }

                db.GameTeam.Add(tm);

                foreach (var member in team.TeamMembers)
                {
                    GameMemberClass.insertNewSkater(game.GameId, db, tm.TeamId, member, tm);
                }

                //TODO:record time outs left.
                db.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        /// <summary>
        /// entry point for updating the team.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="team"></param>
        /// <param name="db"></param>
        public static void updateTeam(GameViewModel game, TeamViewModel team, ManagementContext db, DataModels.Game.Game g)
        {
            try
            {
                var getTeam = g.GameTeams.Where(x => x.TeamId == team.TeamId).FirstOrDefault();

                if (getTeam == null)
                {
                    insertTeamIntoDb(game, team, db, g);
                }
                else
                {

                    if (String.IsNullOrEmpty(team.TeamName))
                        getTeam.TeamName = "Temp";
                    else
                        getTeam.TeamName = team.TeamName;
                    if (team.Logo != null)
                    {
                        if ((getTeam.Logo == null) || (getTeam.Logo != null && getTeam.Logo.TeamLogoId != team.Logo.TeamLogoId))
                        {
                            var logo = db.TeamLogos.Where(x => x.TeamLogoId == team.Logo.TeamLogoId).FirstOrDefault();
                            if (logo != null)
                            {
                                getTeam.Logo = logo;
                                team.Logo.ImageUrl = logo.ImageUrl;
                                team.Logo.ImageUrlThumb = logo.ImageUrlThumb;
                            }
                        }
                    }

                    getTeam.CurrentTimeouts = team.TimeOutsLeft;
                    getTeam.LeageName = team.LeagueName;
                    if (team.LineUpSettings != null)
                    {
                        UpdateLineUpSettings(team, getTeam);
                    }

                }
                db.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: "tried to update the team");
            }
        }

        private static void UpdateLineUpSettings(TeamViewModel team, DataModels.Game.GameTeam getTeam)
        {
            if (getTeam.LineupSettings == null)
                getTeam.LineupSettings = new DataModels.Game.Teams.GameTeamLineupSettings();

            getTeam.LineupSettings.LineUpTypeSelected = (byte)team.LineUpSettings.LineUpTypeSelected;
            getTeam.LineupSettings.PlainBackgroundColor = team.LineUpSettings.PlainBackgroundColor;
            getTeam.LineupSettings.PlainBorderColor = team.LineUpSettings.PlainBorderColor;
            getTeam.LineupSettings.PlainTextColor = team.LineUpSettings.PlainTextColor;
            getTeam.LineupSettings.SidebarBackgroundColor = team.LineUpSettings.SidebarBackgroundColor;
            getTeam.LineupSettings.SidebarColor = team.LineUpSettings.SidebarColor;
            getTeam.LineupSettings.SidebarSkaterTextColor = team.LineUpSettings.SidebarSkaterTextColor;
            getTeam.LineupSettings.SidebarTextColor = team.LineUpSettings.SidebarTextColor;
            getTeam.LineupSettings.Team = getTeam;
        }
        /// <summary>
        /// updates the team scores to the DB.
        /// </summary>
        /// <param name="team"></param>
        /// <param name="game"></param>
        /// <param name="cachedGame"></param>
        /// <param name="db"></param>
        public static int updateTeamScores(TeamNumberEnum team, Guid teamId, GameViewModel game, ManagementContext db, DataModels.Game.Game g)
        {
            try
            {
                List<ScoreViewModel> scoresNew = new List<ScoreViewModel>();

                if (team == TeamNumberEnum.Team1)
                    scoresNew = game.ScoresTeam1;
                else
                    scoresNew = game.ScoresTeam2;
                int scoreCount = 0;
                foreach (var score in scoresNew)
                {
                    var tdb = g.GameTeams.Where(x => x.TeamId == teamId).FirstOrDefault();
                    var scoredb = tdb.GameScores.Where(x => x.GameScoreId == score.PointId).FirstOrDefault();

                    if (scoredb == null)
                        GameScoreClass.insertScoreIntoDb(teamId, game, score, db, g);
                    else
                        scoredb.Point = score.Points;
                    scoreCount += score.Points;
                }
                db.SaveChanges();
                return scoreCount;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: "tried updating team scores");
            }
            return 0;
        }



        /// <summary>
        /// updates the team score to the DB.
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="gameId"></param>
        /// <param name="teamScore"></param>
        /// <param name="db"></param>
        public static void updateTeamScore(Guid teamId, Guid gameId, int teamScore, ManagementContext db, DataModels.Game.Game g)
        {
            try
            {
                if (g == null)
                    return;
                var getScore = g.GameTeams.Where(x => x.TeamId == teamId).FirstOrDefault();
                if (getScore != null)
                {
                    getScore.CurrentScore = teamScore;
                    db.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
    }
}
