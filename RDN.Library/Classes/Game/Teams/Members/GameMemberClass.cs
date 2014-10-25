using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Game;
using RDN.Library.DataModels.Game.Members;
using Scoreboard.Library.ViewModel.Members;
using Scoreboard.Library.ViewModel.Members.Enums;

namespace RDN.Library.Classes.Game.Members
{
    internal class GameMemberClass
    {
        public static int insertNewSkater(Guid gameId, ManagementContext db, Guid teamId, TeamMembersViewModel member, GameTeam team)
        {
            int c = 0;
            try
            {
                GameMember mem = new GameMember();
                mem.Created = DateTime.UtcNow;
                mem.Team = team;
                //mem.MemberLinkId = member.SkaterLinkId;
                if (!String.IsNullOrEmpty(member.SkaterName))
                    mem.MemberName = member.SkaterName;
                else
                    mem.MemberName = "NA";
                mem.MemberNumber = member.SkaterNumber;
                mem.GameMemberId = member.SkaterId;
                mem.LastModified = DateTime.UtcNow;
                db.GameMembers.Add(mem);
               c=  db.SaveChanges();

                if (member.SkaterPictureCompressed != null && !String.IsNullOrEmpty(member.SkaterPictureLocation))
                {
                    try
                    {
                        Stream stream = new MemoryStream(member.SkaterPictureCompressed);
                        FileInfo file = new FileInfo(member.SkaterPictureLocation);
                        RDN.Library.Classes.Account.User.AddMemberPhotoForGame(gameId, stream, file.FullName, member.SkaterId, MemberTypeEnum.Skater);
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return c;
        }

        /// <summary>
        /// gets the member of a particular team from the game in the DB.
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="jam"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public static GameMember getMemberOfTeamInGame(Guid teamId, Guid skaterId, DataModels.Game.Game g)
        {
            try
            {
                if (g.GameTeams != null && g.GameTeams.Count > 0)
                {
                    var t = g.GameTeams.Where(x => x.TeamId == teamId).FirstOrDefault();
                    if (t == null)
                        return null;
                    var m = t.GameMembers.Where(x => x.GameMemberId == skaterId).FirstOrDefault();
                    return m;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        /// <summary>
        /// find a skater in the game within the DB.
        /// </summary>
        /// <param name="skaterId"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public static GameMember getMemberOfTeamInGame(Guid skaterId, DataModels.Game.Game g)
        {
            foreach (var team in g.GameTeams)
            {
                try
                {
                    if (team.GameMembers.Count > 0)
                    {
                        var m = team.GameMembers.Where(x => x.GameMemberId == skaterId).FirstOrDefault();
                        if (m != null)
                            return m;
                    }
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
            }
            return null;
        }



    }
}
