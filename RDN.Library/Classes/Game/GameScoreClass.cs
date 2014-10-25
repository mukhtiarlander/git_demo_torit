using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Game;
using Scoreboard.Library.ViewModel;

namespace RDN.Library.Classes.Game
{
    internal class GameScoreClass
    {

        public static void insertScoreIntoDb(Guid teamId, GameViewModel game, ScoreViewModel score, ManagementContext db, DataModels.Game.Game g)
        {
            try
            {
                GameScore scores = new GameScore();
                if (score.CurrentDateTimeScored != new DateTime())
                    scores.DateTimeScored = score.CurrentDateTimeScored;
                else
                    scores.DateTimeScored = DateTime.UtcNow;
                scores.GameScoreId = score.PointId;
                scores.JamNumber = score.JamNumber;
                scores.JamId = score.JamId;
                scores.PeriodNumber = score.Period;
                scores.PeriodTimeRemainingMilliseconds = score.PeriodTimeRemaining;
                scores.Point = score.Points;
                scores.GameTeam = g.GameTeams.Where(x => x.TeamId == teamId).First();
                if (score.PlayerWhoScored != null && score.PlayerWhoScored.SkaterId != new Guid())
                    scores.MemberWhoScored = g.GameTeams.Where(x => x.TeamId == teamId).First().GameMembers.Where(x => x.GameMemberId == score.PlayerWhoScored.SkaterId).FirstOrDefault();
                db.GameScore.Add(scores);
                db.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: "jamId:" + score.JamId + "/ pointId:" + score.PointId);
            }
        }
    }
}
