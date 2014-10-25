using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Game.Members;
using Scoreboard.Library.Static.Enums;
using Scoreboard.Library.ViewModel;

namespace RDN.Library.Classes.Game.Actions
{
    internal class GameAssistsClass
    {

        public static int insertAssistIntoDb(Guid teamId, GameViewModel game, AssistViewModel assisted, ManagementContext db, DataModels.Game.Game g)
        {
            int c = 0;
            try
            {
                GameMemberAssist assist = new GameMemberAssist();
                assist.DateTimeAssisted = assisted.CurrentDateTimeAssisted;
                assist.GameAssistId = assisted.AssistId;
                assist.JamNumber = assisted.JamNumber;
                assist.JamId = assisted.JamId;
                assist.PeriodNumber = assisted.Period;
                assist.PeriodTimeRemainingMilliseconds = assisted.PeriodTimeRemaining;
                assist.Game = g;

                assist.MemberWhoAssisted = g.GameTeams.Where(x => x.TeamId == teamId).First().GameMembers.Where(x => x.GameMemberId == assisted.PlayerWhoAssisted.SkaterId).FirstOrDefault();
                if (assist.MemberWhoAssisted != null)
                {
                    db.GameMemberAssist.Add(assist);
                    c += db.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return c;
        }
        /// <summary>
        /// updates the team assists to the DB.
        /// </summary>
        /// <param name="team"></param>
        /// <param name="teamId"></param>
        /// <param name="game"></param>
        /// <param name="db"></param>
        /// <param name="g"></param>
        public static void updateTeamAssists(TeamNumberEnum team, Guid teamId, GameViewModel game, ManagementContext db, DataModels.Game.Game g)
        {
            try
            {
                List<AssistViewModel> assitsNew = new List<AssistViewModel>();

                if (team == TeamNumberEnum.Team1)
                    assitsNew = game.AssistsForTeam1;
                else
                    assitsNew = game.AssistsForTeam2;

                for (int i = 0; i < assitsNew.Count; i++)
                {
                    insertAssistIntoDb(teamId, game, assitsNew[i], db, g);
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: "tried updating team scores");
            }
        }



    }
}
