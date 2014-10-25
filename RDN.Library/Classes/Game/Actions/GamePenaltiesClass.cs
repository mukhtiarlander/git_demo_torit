using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Game.Members;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Game.Members;
using Scoreboard.Library.Static.Enums;
using Scoreboard.Library.ViewModel;

namespace RDN.Library.Classes.Game.Actions
{
    internal class GamePenaltiesClass
    {
        /// <summary>
        /// updates the penalty box DB with new information
        /// </summary>
        /// <param name="game"></param>
        /// <param name="cachedGame"></param>
        /// <param name="db"></param>
        /// <param name="pen"></param>
        public static void updatePenaltyBoxInDb(GameViewModel game, ManagementContext db, SkaterInPenaltyBoxViewModel pen, DataModels.Game.Game g)
        {
            try
            {
                var cachedPen = g.GameMemberPenaltyBox.Where(x => x.PenaltyIdFromGame == pen.PenaltyId).FirstOrDefault();
                if (cachedPen == null)
                    insertNewPenaltyIntoDb(game, db, pen, g);
                else
                {
                    cachedPen.GameTimeMilliSecondsReturned = pen.GameTimeInMillisecondsReleased;
                    cachedPen.GameTimeMilliSecondsSent = pen.GameTimeInMillisecondsSent;
                    cachedPen.JamNumberReturned = pen.JamNumberReleased;
                    cachedPen.JamNumberSent = pen.JamNumberSent;
                    cachedPen.JamIdReturned = pen.JamIdReleased;
                    cachedPen.JamIdSent = pen.JamIdSent;
                    cachedPen.JamTimeMilliSecondsReturned = pen.JamTimeInMillisecondsReleased;
                    cachedPen.JamTimeMilliSecondsSent = pen.JamTimeInMillisecondsSent;
                    cachedPen.PenaltyType = pen.PenaltyType.ToString();
                    db.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        /// <summary>
        /// updates the team penalties to the DB.
        /// </summary>
        /// <param name="team"></param>
        /// <param name="teamId"></param>
        /// <param name="game"></param>
        /// <param name="db"></param>
        /// <param name="g"></param>
        public static void updateTeamPenalties(TeamNumberEnum team, Guid teamId, GameViewModel game, ManagementContext db, DataModels.Game.Game g)
        {
            try
            {
                List<PenaltyViewModel> blocksNew = new List<PenaltyViewModel>();

                if (team == TeamNumberEnum.Team1 && game.PenaltiesForTeam1 != null)
                    blocksNew = game.PenaltiesForTeam1;
                else if (team == TeamNumberEnum.Team1 && game.PenaltiesForTeam2 != null)
                    blocksNew = game.PenaltiesForTeam2;

                for (int i = 0; i < blocksNew.Count; i++)
                {
                    var blockDb = g.GameMemberPenalties.Where(x => x.GamePenaltyId == blocksNew[i].PenaltyId).FirstOrDefault();

                    if (blockDb == null)
                        insertPenaltyIntoDb(teamId, game, blocksNew[i], db, g);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: "tried updating team scores");
            }
        }
        public static void insertNewPenaltyIntoDb(GameViewModel game, ManagementContext db, SkaterInPenaltyBoxViewModel pen, DataModels.Game.Game g)
        {
            try
            {
                var m = GameMemberClass.getMemberOfTeamInGame(pen.PlayerSentToBox.SkaterId, g);
                if (m != null)
                {
                    GameMemberPenaltyBox penalty = new GameMemberPenaltyBox();
                    penalty.PenaltyNumberForSkater = pen.PenaltyNumberForSkater;
                    penalty.Member = m;
                    penalty.GameTimeMilliSecondsSent = pen.GameTimeInMillisecondsSent;
                    penalty.GameTimeMilliSecondsReturned = pen.GameTimeInMillisecondsReleased;
                    penalty.JamNumberReturned = pen.JamNumberReleased;
                    penalty.JamIdReturned = pen.JamIdReleased;
                    penalty.JamIdSent = pen.JamIdSent;
                    penalty.JamTimeMilliSecondsReturned = pen.JamTimeInMillisecondsReleased;
                    penalty.JamTimeMilliSecondsSent = pen.JamTimeInMillisecondsSent;
                    penalty.Game = g;
                    penalty.PenaltyIdFromGame = pen.PenaltyId;
                    penalty.PenaltyType = pen.PenaltyType.ToString();
                    db.GameMemberPenaltyBox.Add(penalty);
                    db.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }


        public static void insertPenaltyIntoDb(Guid teamId, GameViewModel game, PenaltyViewModel penalty, ManagementContext db, DataModels.Game.Game g)
        {
            try
            {
                GameMemberPenalty blocks = new GameMemberPenalty();
                blocks.DateTimePenaltied = penalty.CurrentDateTimePenalty;
                blocks.GamePenaltyId = penalty.PenaltyId;
                blocks.JamNumber = penalty.JamNumber;
                blocks.JamId = penalty.JamId;
                blocks.PeriodNumber = penalty.Period;
                blocks.PeriodTimeRemainingMilliseconds = penalty.GameTimeInMilliseconds;
                blocks.PenaltyType = Convert.ToInt32(penalty.PenaltyType);
                blocks.PenaltyScale = Convert.ToInt32(penalty.PenaltyScale);
                blocks.Game = g;
                blocks.MemberWhoPenaltied = g.GameTeams.Where(x => x.TeamId == teamId).FirstOrDefault().GameMembers.Where(x => x.GameMemberId == penalty.PenaltyAgainstMember.SkaterId).FirstOrDefault();
                if (blocks.MemberWhoPenaltied != null)
                {
                    db.GameMemberPenalty.Add(blocks);
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
