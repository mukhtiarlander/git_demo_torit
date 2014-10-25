using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using RDN.League.Models.Utilities;
using RDN.Utilities.Config;
using RDN.Library.Cache;
using RDN.Portable.Config;
using RDN.Library.Util.Enum;

namespace RDN.League.Models.Filters
{
    public class LeagueAuthorize : AuthorizeAttribute
    {
        public bool EmailVerification { get; set; }
        public bool IsInLeague { get; set; }
        public bool IsManager { get; set; }
        public bool IsMedical { get; set; }
        public bool IsOwner { get; set; }
        public bool IsTreasurer { get; set; }
        public bool IsSecretary { get; set; }
        public bool IsAttendanceManager { get; set; }
        public bool IsEventCourdinator { get; set; }
        public bool IsPollManager { get; set; }
        public bool IsHeadRef { get; set; }
        public bool IsHeadNSO { get; set; }
        public bool IsShopManager { get; set; }
        //public bool RequirePresidentStatus { get; set; }
        public bool HasPaidSubscription { get; set; }
        public bool IsGroupModerator { get; set; }
        public bool IsInventoryTracker { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                return;

            Guid memberId = RDN.Library.Classes.Account.User.GetMemberId();

            if (MemberCache.IsAdministrator(memberId))
                return;

            if (IsGroupModerator)
            {

            }

            if (EmailVerification)
            {
                var isEmailVerified = IsEmailValidated(ref filterContext);
                if (!isEmailVerified)
                {
                    filterContext.Result = new RedirectResult(ServerConfig.WEBSITE_VERIFY_EMAIL_LOGGED_IN_LOCATION);
                    return;
                }
            }

            if (IsInLeague)
            {
                if (!MemberStatus.IsInLeague())
                {
                    filterContext.Result = new RedirectResult(ServerConfig.WEBSITE_INTERNAL_DEFAULT_LOCATION);
                    return;
                }
            }

            if (HasPaidSubscription)
            {
                bool isPaid = MemberCache.CheckIsLeagueSubscriptionPaid(memberId);
                if (!isPaid)
                {
                    filterContext.Result = new RedirectResult(ServerConfig.LEAGUE_SUBSCRIPTION_RESUBSUBSCRIBE + MemberCache.GetLeagueIdOfMember(memberId).ToString().Replace("-", "") + "?u=" + RDN.Library.Util.Enum.SiteMessagesEnum.pp);
                    return;
                }
            }

            //if any of the roles hit, we just return properly.
            if (IsTreasurer)
                if (MemberCache.IsTreasurerOrBetterOfLeague(memberId))
                    return;


            if (IsManager)
                if (MemberCache.IsManagerOrBetterOfLeague(memberId))
                    return;

            if (IsMedical)
                if (MemberCache.IsMedicalOrBetterOfLeague(memberId))
                    return;

            if (IsSecretary)
                if (MemberCache.IsSecretaryOrBetterOfLeague(memberId))
                    return;

            if (IsEventCourdinator)
                if (MemberCache.IsEventsCourdinatorOrBetterOfLeague(memberId))
                    return;

            if (IsHeadRef)
                if (MemberCache.IsHeadRefOrBetterOfLeague(memberId))
                    return;

            if (IsHeadNSO)
                if (MemberCache.IsHeadNSOOrBetterOfLeague(memberId))
                    return;

            if (IsShopManager)
                if (MemberCache.IsShopManagerOrBetterOfLeague(memberId))
                    return;

            if (IsAttendanceManager)
                if (MemberCache.IsAttendanceManager(memberId))
                    return;

            if (IsPollManager)
                if (MemberCache.IsPollManager(memberId))
                    return;

            if (IsInventoryTracker)
                if (MemberCache.IsInventoryOrBetterOfLeague(memberId))
                    return;

            //we redirect here because they weren't the required above managers...
            //so we check if any of these are true, and if they are, that means the above rules didn't return.
            if (IsHeadNSO || IsHeadRef || IsSecretary || IsTreasurer || IsManager || IsShopManager || IsMedical || IsInventoryTracker || IsAttendanceManager || IsPollManager)
            {
                filterContext.Result = new RedirectResult(ServerConfig.WEBSITE_INTERNAL_DEFAULT_LOCATION + "?u=" + SiteMessagesEnum.na.ToString());
                return;
            }

        }

        private bool IsEmailValidated(ref AuthorizationContext filterContext)
        {
            bool? isVerified = filterContext.HttpContext.Session == null || filterContext.HttpContext.Session["IsVerified"] == null || string.IsNullOrEmpty(filterContext.HttpContext.Session["IsVerified"].ToString()) ? (bool?)null : (bool)filterContext.HttpContext.Session["IsVerified"];
            isVerified = true;
            if (!isVerified.HasValue)
            {
                var userId = (Guid)Membership.GetUser().ProviderUserKey;
                var member = Library.Classes.Account.User.GetMemberWithUserId(userId);
                if (member == null)
                    throw new Exception("LeagueAuthorize -> Invalid member value");

                isVerified = member.IsVerified;
                filterContext.HttpContext.Session.Add("IsVerified", isVerified.Value);
            }
            return isVerified.Value;
        }
    }
}