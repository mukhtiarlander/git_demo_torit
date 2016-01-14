using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Site.Classes.Exception;
using Geocoding;
using RDN.Library.Cache;
using RDN.Library.Classes.League.Links;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Roster;
using RDN.Library.Util;

namespace RDN.Library.Classes.Roster
{
    public class RosterManager
    {
        public static bool CreateNewRoster(Roster roster)
        {
            try
            {
                var dc = new ManagementContext();
                DataModels.Roster.Roster rosterObj = new DataModels.Roster.Roster();
                rosterObj.RosterName = roster.RosterName;
                rosterObj.GameDate = roster.GameDate;
                rosterObj.League = dc.Leagues.Where(x => x.LeagueId == roster.LeagueId).FirstOrDefault();
                rosterObj.RuleSetsUsedEnum = roster.RuleSetsUsedEnum;
                if (!string.IsNullOrEmpty(roster.RosterMemberIds))
                {
                    var rosterMembers = roster.RosterMemberIds.Split(',');
                    foreach (var memberId in rosterMembers)
                    {
                        var rosterMember = new RosterMember();
                        rosterMember.Member = dc.Members.Where(x => x.MemberId == new Guid(memberId)).FirstOrDefault();
                        rosterMember.Roster = rosterObj;
                        rosterMember.InsuranceType = roster.InsuranceTypeId;
                        rosterObj.RosterMembers.Add(rosterMember);
                    }
                }
                dc.Rosters.Add(rosterObj);
                int c = dc.SaveChanges();
                return c > 0;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());

            }
            return false;
        }

        public static List<Roster> GetLeagueRosters(Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();

                var rosters = dc.Rosters.Where(x => x.League.LeagueId == leagueId).Select(c => new Roster()
                {
                    RosterId = c.RosterId,
                    RosterName = c.RosterName,
                    GameDate = c.GameDate,
                    LeagueId = c.League.LeagueId,
                    RuleSetsUsedEnum = c.RuleSetsUsedEnum,
                    RosterSize = c.RosterMembers.Where(x => x.IsRemoved == false).Count()
                }).ToList();


                return rosters;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public static Roster GetLeagueRoster(long rosterId, Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();

                var roster = dc.Rosters.Where(x => x.League.LeagueId == leagueId && x.RosterId == rosterId).FirstOrDefault();
                if (roster != null)
                {
                    var rosterMember = roster.RosterMembers.Where(x => x.IsRemoved == false).FirstOrDefault();
                    var item = new Roster()
                    {
                        RosterId = roster.RosterId,
                        RosterName = roster.RosterName,
                        GameDate = roster.GameDate,
                        LeagueId = roster.League.LeagueId,
                        RuleSetsUsedEnum = roster.RuleSetsUsedEnum,
                        InsuranceTypeId = rosterMember != null ? rosterMember.InsuranceType : 0,
                        RosterSize = roster.RosterMembers.Where(x => x.IsRemoved == false).Count()
                    };
                    var rosterMembers = roster.RosterMembers.Where(x => x.IsRemoved == false).Select(x => MemberCache.GetMemberDisplay(x.Member.MemberId)).Select(x => new KeyValueHelper()
                    {
                        Id = x.MemberId,
                        Name = x.SiteName
                    }).ToList();
                  
                    item.RosterMembers = rosterMembers;
                    return item;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public static bool UpdateRoster(Roster roster)
        {
            try
            {

                var dc = new ManagementContext();
                var rosterObj = dc.Rosters.Where(x => x.RosterId == roster.RosterId).FirstOrDefault();
                if (rosterObj == null)
                    return false;
                rosterObj.RosterName = roster.RosterName;
                rosterObj.GameDate = roster.GameDate;
                rosterObj.League = dc.Leagues.Where(x => x.LeagueId == roster.LeagueId).FirstOrDefault();
                rosterObj.RuleSetsUsedEnum = roster.RuleSetsUsedEnum;
                var rosterMemberIds = roster.RosterMemberIds.Split(',').ToList().ConvertAll(Guid.Parse);
                var rosterMembersToRemove = rosterObj.RosterMembers.Where(x =>  x.IsRemoved == false && !rosterMemberIds.Contains(x.Member.MemberId));
                var existingrosterMembers = rosterObj.RosterMembers.Where(x =>  x.IsRemoved == false && rosterMemberIds.Contains(x.Member.MemberId));
                rosterMembersToRemove.ForEach(x => x.IsRemoved = true);
                existingrosterMembers.ForEach(x => x.InsuranceType = roster.InsuranceTypeId);
                foreach (var memId in rosterMemberIds)
                {
                    if (!rosterObj.RosterMembers.Any(x => x.Member.MemberId == memId && x.IsRemoved == false))
                    {
                        var rosterMember = new RosterMember();
                        rosterMember.Member = dc.Members.FirstOrDefault(x => x.MemberId == memId);
                        rosterMember.Roster = rosterObj;
                        rosterMember.InsuranceType = roster.InsuranceTypeId;
                        rosterObj.RosterMembers.Add(rosterMember);
                    }
                }
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
    }
}
