using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using RDN.Library.Cache;

namespace RDN.League.Models.Utilities
{
    public static class MemberStatus
    {
        public static bool IsInLeague()
        {
            var memberId = RDN.Library.Classes.Account.User.GetMemberId();
            Guid id = MemberCache.GetLeagueIdOfMember(memberId);
            if (id != new Guid())
                return true;
            return false;
        }

        public static bool IsManager(Cache cache)
        {
            var memberId = RDN.Library.Classes.Account.User.GetMemberId();
            return MemberCache.IsManagerOrBetterOfLeague(memberId);
        }

        public static bool IsPresident()
        {
            return System.Web.Security.Roles.IsUserInRole("League_President");
        }

        public static bool IsTreasurer()
        {
            return System.Web.Security.Roles.IsUserInRole("League_Treasurer");
        }

        public static bool IsInAny(MemberStatusTypes types)
        {
            var roles = System.Web.Security.Roles.GetRolesForUser();
            if (types.HasFlag(MemberStatusTypes.Manager))
                if (roles.Contains("League_Manager"))
                    return true;
            if (types.HasFlag(MemberStatusTypes.Member))
                if (roles.Contains("League_Member"))
                    return true;
            if (types.HasFlag(MemberStatusTypes.President))
                if (roles.Contains("League_President"))
                    return true;
            if (types.HasFlag(MemberStatusTypes.Treasurer))
                if (roles.Contains("League_Treasurer"))
                    return true;
            return false;
        }

        public static bool IsSelf(Guid idToCheck)
        {
            return Library.Classes.Account.User.GetMemberId().Equals(idToCheck);
        }
    }
}