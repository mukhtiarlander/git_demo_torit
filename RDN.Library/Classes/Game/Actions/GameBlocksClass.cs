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
    internal class GameBlocksClass
    {
        public static void insertBlockIntoDb(Guid teamId, GameViewModel game, BlockViewModel block, ManagementContext db, DataModels.Game.Game g)
        {
            try
            {
                GameMemberBlock blocks = new GameMemberBlock();
                blocks.DateTimeBlocked = block.CurrentDateTimeBlock;
                blocks.GameBlockId = block.BlockId;
                blocks.JamNumber = block.JamNumber;
                blocks.JamId = block.JamId;
                blocks.PeriodNumber = block.Period;
                blocks.PeriodTimeRemainingMilliseconds = block.PeriodTimeRemaining;
                blocks.Game = g;

                blocks.MemberWhoBlocked = g.GameTeams.Where(x => x.TeamId == teamId).First().GameMembers.Where(x => x.GameMemberId == block.PlayerWhoBlocked.SkaterId).FirstOrDefault();
                if (blocks.MemberWhoBlocked != null)
                {
                    db.GameMemberBlock.Add(blocks);
                    db.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        /// <summary>
        /// updates the team blocks to the Db.
        /// </summary>
        /// <param name="team"></param>
        /// <param name="teamId"></param>
        /// <param name="game"></param>
        /// <param name="db"></param>
        /// <param name="g"></param>
        public static void updateTeamBlocks(TeamNumberEnum team, Guid teamId, GameViewModel game, ManagementContext db, DataModels.Game.Game g)
        {
            try
            {
                List<BlockViewModel> blocksNew = new List<BlockViewModel>();

                if (team == TeamNumberEnum.Team1)
                    blocksNew = game.BlocksForTeam1;
                else
                    blocksNew = game.BlocksForTeam2;

                for (int i = 0; i < blocksNew.Count; i++)
                {
                    var blockDb = g.GameMemberBlocks.Where(x => x.GameBlockId == new Guid("c88dd0c2-2e9e-4e0f-b6db-98d39b913291")).FirstOrDefault();

                    if (blockDb == null)
                        insertBlockIntoDb(teamId, game, blocksNew[i], db, g);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: "tried updating team scores");
            }
        }
    }
}
