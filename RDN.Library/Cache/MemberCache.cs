using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using RDN.Library.Classes.Federation;
using RDN.Library.Classes.Federation.Enums;
using RDN.Library.DataModels.Federation;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.DataModels.League;
using RDN.Library.Classes.League.Enums;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.League.Classes;
using RDN.Utilities.Enums;
using RDN.Library.Classes.Calendar;
using RDN.Library.Classes.Document;
using System.Net;
using RDN.Portable.Config;
using RDN.Library.Classes.Account.Classes.Json;
using RDN.Portable.Models.Json;
using RDN.Library.Cache.Singletons;
using RDN.Portable.Classes.Controls.Dues;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.League.Classes;
using RDN.Portable.Classes.League.Enums;
using RDN.Portable.Classes.Account.Enums.Settings;
using RDN.Portable.Classes.Controls.Calendar;
using RDN.Library.DataModels.Context;
using RDN.Library.Classes.Config;
using RDN.Portable.Classes.Url;

namespace RDN.Library.Cache
{
    public class MemberCache : CacheLock
    {
        public List<DataModels.Federation.FederationOwnership> OwnersOfFederation { get; set; }
        //public List<DataModels.League.LeagueOwnership> OwnersOfLeague { get; set; }
        public MemberDisplay memberDisplay { get; set; }

        public Guid CurrentLeagueForumId { get; set; }
        public Guid CurrentLeagueCalendarId { get; set; }
        public Guid CurrentFederationForumId { get; set; }
        public Classes.Store.Classes.Store CurrentFederationStoreInfo { get; set; }
        public Classes.Store.Classes.Store CurrentLeagueStoreInfo { get; set; }
        public Classes.Store.Classes.Store UserStoreInfo { get; set; }
        public int UnreadMessagesCount { get; set; }

        public int JobsCount { get; set; }
        public bool IsCurrentLeagueSubscriptionPaid { get; set; }
        public DateTime SubscriptionEndDateOfCurrentLeague { get; set; }
        public RDN.Portable.Classes.League.Classes.League CurrentLeague { get; set; }
        public DuesPortableModel Dues { get; set; }
        public List<LeagueGroup> GroupsApartOf { get; set; }
        public List<RDN.Portable.Classes.League.Classes.League> Leagues { get; set; }
        public List<RDN.Library.Classes.Federation.Federation> Federations { get; set; }
        public RDN.Library.Classes.Federation.Federation CurrentFederation { get; set; }
        public bool IsAdmin { get; set; }
        public List<Document> LeagueDocuments { get; set; }

        MemberCache()
        {
            GroupsApartOf = new List<LeagueGroup>();
        }
        /// <summary>
        /// checks if the member is an actual owner of a federation.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsOwnerOfFederation(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                var owners = cached.OwnersOfFederation;

                if (owners != null && owners.Count > 0)
                {
                    var owner = owners.Where(x => x.OwnerType == Convert.ToInt32(FederationOwnerEnum.Owner)).FirstOrDefault();
                    if (owner != null)
                        return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool IsOwnerOfLeague(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.memberDisplay != null)
                    return cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool IsManagerOfLeague(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.memberDisplay != null)
                    if (cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager))
                        return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool IsSecretaryOrBetterOfLeague(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.memberDisplay != null)
                    if (cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Secretary))
                        return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool IsPollMgrOrBetterOfLeague(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.memberDisplay != null)
                    if (cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Secretary) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Polls))
                        return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool IsAttendanceManager(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.memberDisplay != null)
                    if (cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Attendance))
                        return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool IsAttendanceManagerOrBetterOfLeague(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.memberDisplay != null)
                    if (cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Attendance) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Secretary))
                        return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool IsEventsCourdinatorOrBetterOfLeague(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.memberDisplay != null)
                    if (cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Events_Coord) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Secretary))
                        return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool IsPollManager(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.memberDisplay != null)
                    if (cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Polls))
                        return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool IsMedicalOrBetterOfLeague(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.memberDisplay != null)
                    if (cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Medical) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner))
                        return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool IsTreasurerOrBetterOfLeague(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.memberDisplay != null)
                    if (cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Treasurer))
                        return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool IsInventoryOrBetterOfLeague(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.memberDisplay != null)
                    if (cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Secretary) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Inventory))
                        return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool IsSponsorshipOrBetterOfLeague(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.memberDisplay != null)
                    if (cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Secretary) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Sponsorship))
                        return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool IsHeadRefOrBetterOfLeague(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.memberDisplay != null)
                    if (cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Head_Ref))
                        return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool IsHeadNSOOrBetterOfLeague(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.memberDisplay != null)
                    if (cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Head_NSO))
                        return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool IsShopManagerOrBetterOfLeague(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.memberDisplay != null)
                    if (cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Shops))
                        return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        /// <summary>
        /// checks if the user is a manager of the federation
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static bool IsManagerOrBetterOfFederation(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                var owners = cached.OwnersOfFederation;

                if (owners != null && owners.Count > 0)
                {
                    var owner = owners.Where(x => x.OwnerType == Convert.ToInt32(FederationOwnerEnum.Manager)).FirstOrDefault();
                    if (owner == null)
                        owner = owners.Where(x => x.OwnerType == Convert.ToInt32(FederationOwnerEnum.Owner)).FirstOrDefault();
                    if (owner != null)
                        return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool IsConnectedToDerby(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                //they are connected
                if (cached.memberDisplay != null)
                    return !cached.memberDisplay.IsNotConnectedToDerby;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            //they are not connected.
            return false;
        }


        public static bool IsManagerOrBetterOfLeague(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.memberDisplay != null)
                    if (cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner))
                        return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool IsModeratorOrBetterOfLeagueGroup(Guid memberId, long groupId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                var league = cached.CurrentLeague;
                if (league != null)
                {
                    var group = league.Groups.Where(x => x.Id == groupId).FirstOrDefault();
                    if (group != null)
                    {
                        var mem = group.GroupMembers.Where(x => x.MemberId == memberId).FirstOrDefault();
                        if (mem != null)
                        {
                            if (mem.MemberAccessLevelEnum == GroupMemberAccessLevelEnum.Moderator || mem.MemberAccessLevelEnum == GroupMemberAccessLevelEnum.Owner)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool IsManagerOrBetterOfLeague(Guid memberId, Guid leagueId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.memberDisplay != null)
                    if (cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager) || cached.memberDisplay.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner))
                        return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        /// <summary>
        /// clears all members cache attached to league.
        /// </summary>
        /// <param name="leagueId"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static bool ClearLeagueMembersCache(Guid leagueId)
        {
            try
            {
                var members = Classes.League.LeagueFactory.GetLeagueMembers(leagueId);
                foreach (var mem in members)
                {
                    Clear(mem.MemberId);
                }
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        /// <summary>
        /// gets a list of all the owned federations
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static List<FederationOwnership> GetAllOwnedFederations(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                var owners = cached.OwnersOfFederation;

                return owners;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static List<CalendarEventPortable> GetMemberBirthdays(Guid memberId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate.Month == 12)
                    startDate = new DateTime(startDate.Year + 1, 1, 1);
                if (endDate.Month == 1)
                    endDate = new DateTime(endDate.Year - 1, 12, 31);
                var cached = GetCache(memberId, true);
                List<CalendarEventPortable> birthdays = new List<CalendarEventPortable>();
                if (cached.CurrentLeague != null)
                {
                    var owners = cached.CurrentLeague.LeagueMembers.Where(x => x.DOB != null && x.DOB > DateTime.UtcNow.AddYears(-100) && x.DOB.Month >= startDate.Month && x.DOB.Month <= endDate.Month);
                    foreach (var member in owners)
                    {
                        var display = CalendarEventFactory.DisplayBirthday(member);
                        if (display.StartDate <= endDate && display.EndDate >= startDate)
                            birthdays.Add(display);
                    }
                }
                return birthdays;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<CalendarEventPortable>();
        }

        public static List<CalendarEventJson> GetMemberBirthdaysJson(Guid memberId, DateTime startDate, DateTime endDate)
        {
            try
            {
                //if its the 12th month, we need it to be the first day.
                if (startDate.Month == 12)
                    startDate = new DateTime(startDate.Year + 1, 1, 1);
                //if its the 1st month, we need it to be the 1th month 
                if (endDate.Month == 1)
                    endDate = new DateTime(endDate.Year - 1, 12, 31);
                var cached = GetCache(memberId, true);
                //we modify the dates above to deal witht eh proper mnths for the this linq query.
                // otherwise we wouldn't get any results on those edge tries.
                var owners = cached.CurrentLeague.LeagueMembers.Where(x => x.DOB != null && x.DOB > DateTime.UtcNow.AddYears(-100) && x.DOB.Month >= startDate.Month && x.DOB.Month <= endDate.Month);
                List<CalendarEventJson> birthdays = new List<CalendarEventJson>();
                foreach (var member in owners)
                {
                    var display = CalendarEventFactory.DisplayBirthdayJson(member);
                    if (display.StartDate <= endDate && display.EndDate >= startDate)
                        birthdays.Add(display);
                }
                return birthdays;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<CalendarEventJson>();
        }
        public static List<CalendarEventPortable> GetMemberStartDates(Guid memberId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate.Month == 12)
                    startDate = new DateTime(startDate.Year + 1, 1, 1);
                if (endDate.Month == 1)
                    endDate = new DateTime(endDate.Year - 1, 12, 31);
                var cached = GetCache(memberId, true);
                List<CalendarEventPortable> startDates = new List<CalendarEventPortable>();
                if (cached.CurrentLeague != null)
                {
                    var owners = cached.CurrentLeague.LeagueMembers.Where(x => x.StartedSkating.HasValue && x.StartedSkating.Value > DateTime.UtcNow.AddYears(-100) && x.StartedSkating.Value.Month >= startDate.Month && x.StartedSkating.Value.Month <= endDate.Month);

                    foreach (var member in owners)
                    {
                        var display = CalendarEventFactory.DisplayStartDate(member);
                        if (display.StartDate <= endDate && display.EndDate >= startDate)
                            startDates.Add(display);
                    }
                }
                return startDates;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<CalendarEventPortable>();
        }

        public static List<CalendarEventJson> GetMemberStartDatesJson(Guid memberId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate.Month == 12)
                    startDate = new DateTime(startDate.Year + 1, 1, 1);
                if (endDate.Month == 1)
                    endDate = new DateTime(endDate.Year - 1, 12, 31);
                var cached = GetCache(memberId, true);
                var owners = cached.CurrentLeague.LeagueMembers.Where(x => x.StartedSkating.HasValue && x.StartedSkating.Value > DateTime.UtcNow.AddYears(-100) && x.StartedSkating.Value.Month >= startDate.Month && x.StartedSkating.Value.Month <= endDate.Month);
                List<CalendarEventJson> startDates = new List<CalendarEventJson>();
                foreach (var member in owners)
                {
                    var display = CalendarEventFactory.DisplayStartDateJson(member);
                    if (display.StartDate <= endDate && display.EndDate >= startDate)
                        startDates.Add(display);
                }
                return startDates;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<CalendarEventJson>();
        }
        public static List<LeagueGroup> GetGroupsApartOf(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                var owners = cached.GroupsApartOf;
                return owners;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static LeagueGroup GetGroup(Guid memberId, long groupId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                return cached.GroupsApartOf.Where(x => x.Id == groupId).FirstOrDefault();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static List<RDN.Portable.Classes.League.Classes.League> GetAllOwnedLeagues(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);

                if (cached.memberDisplay != null)
                {
                    var leagues = cached.memberDisplay.Leagues.Where(x => x.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner)).ToList();
                    return leagues;
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public static MemberDisplay GetMemberDisplay(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                var member = cached.memberDisplay;
                return member;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static List<MemberDisplay> GetCurrentLeagueMembers(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.CurrentLeague != null)
                    return cached.CurrentLeague.LeagueMembers;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<MemberDisplay>();
        }

        public static bool IsMemberApartOfLeague(Guid memberId, Guid leagueId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.Leagues != null)
                {
                    if (cached.Leagues.Where(x => x.LeagueId == leagueId).FirstOrDefault() != null)
                        return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static string GetLeagueThemeColor(Guid leagueid)
        {
            try
            {
                var dc = new ManagementContext();
                var league = dc.Leagues.Include("Groups").Include("Federations").Include("Owners").Include("Teams").Include("Members").Include("Members.SkaterClass").Include("ContactCard").Include("Contacts").Where(x => x.LeagueId == leagueid).FirstOrDefault();
                if (league != null && league.ThemeColor != null)
                    return league.ThemeColor;
                return "";
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return "";
        }
        public static bool IsMemberApartOfForum(Guid memberId, Guid forumId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.CurrentLeagueForumId == forumId || cached.CurrentFederationForumId == forumId)
                    return true;
                return false;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static List<RDN.Portable.Classes.League.Classes.League> GetLeaguesOfMember(Guid memberId, bool removeCurrentLeague)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.Leagues != null && cached.CurrentLeague != null)
                {
                    if (removeCurrentLeague)
                        return cached.Leagues.Where(x => x.LeagueId != cached.CurrentLeague.LeagueId).ToList();
                    else
                        return cached.Leagues;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<Portable.Classes.League.Classes.League>();
        }
        public static Guid GetLeagueIdOfMember()
        {
            try
            {
                var cached = GetCache(RDN.Library.Classes.Account.User.GetMemberId(), true);

                if (cached.CurrentLeague != null)
                {
                    return cached.CurrentLeague.LeagueId;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }
        public static Guid GetLeagueIdOfMember(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);

                if (cached.CurrentLeague != null)
                {
                    return cached.CurrentLeague.LeagueId;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }
        public static string GetStoreManagerKeysForUrlUser(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.UserStoreInfo != null)
                {
                    if (cached.UserStoreInfo.MerchantId != new Guid())
                        return cached.UserStoreInfo.PrivateManagerId.ToString().Replace("-", "") + "/" + cached.UserStoreInfo.MerchantId.ToString().Replace("-", "");
                }
                return memberId.ToString().Replace("-", "");
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return String.Empty;
        }
        public static bool HasPersonalStoreAlreadyForUser(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.UserStoreInfo != null)
                {
                    if (cached.UserStoreInfo.MerchantId != new Guid())
                        return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool IsManagerOrBetterOfStore(Guid memberId, Guid merchantId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.UserStoreInfo != null)
                    if (cached.UserStoreInfo.MerchantId == merchantId)
                        return true;

                if (cached.CurrentLeagueStoreInfo != null)
                    if (cached.CurrentLeagueStoreInfo.MerchantId == merchantId)
                    {
                        return IsShopManagerOrBetterOfLeague(memberId);
                    }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static string GetStoreManagerKeysForUrlLeague(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.CurrentLeagueStoreInfo != null)
                {
                    if (cached.CurrentLeagueStoreInfo.MerchantId != new Guid())
                        return cached.CurrentLeagueStoreInfo.PrivateManagerId.ToString().Replace("-", "") + "/" + cached.CurrentLeagueStoreInfo.MerchantId.ToString().Replace("-", "");
                }
                return cached.CurrentLeague.LeagueId.ToString().Replace("-", "");
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return String.Empty;
        }
        public static string GetStoreManagerKeysForUrlFederation(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.CurrentFederationStoreInfo != null)
                {
                    if (cached.CurrentFederationStoreInfo.MerchantId != null && cached.CurrentFederationStoreInfo.PrivateManagerId != null)
                        return cached.CurrentFederationStoreInfo.PrivateManagerId.ToString().Replace("-", "") + "/" + cached.CurrentFederationStoreInfo.MerchantId.ToString().Replace("-", "");
                }
                return cached.CurrentFederation.FederationId.ToString().Replace("-", "");
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return String.Empty;
        }
        /// <summary>
        /// checks if members league has any owners yet.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static bool HasOwnersOfLeague(Guid memberId, Guid leagueId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.Leagues != null && cached.Leagues.Where(x => x.LeagueId == leagueId).FirstOrDefault() != null)
                    if (cached.Leagues.Where(x => x.LeagueId == leagueId).FirstOrDefault().LeagueMembers.Where(x => x.LeagueOwnersEnum > 0).Count() > 0)
                        return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static List<Guid> GetFederationIdsOfMember(Guid memberId)
        {
            List<Guid> fedIds = new List<Guid>();
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.Federations != null)
                {
                    foreach (var fed in cached.Federations)
                    {
                        fedIds.Add(fed.FederationId);
                    }
                }
                return fedIds;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return fedIds;
        }
        public static bool DoesBelongToFederation(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.CurrentFederation != null)
                {
                    return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool DoesBelongToFederation()
        {
            try
            {
                var cached = GetCache(RDN.Library.Classes.Account.User.GetMemberId(), true);
                if (cached.CurrentFederation != null)
                {
                    return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static Guid GetFederationIdOfMember(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.CurrentFederation != null)
                {
                    return cached.CurrentFederation.FederationId;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }
        public static List<RDN.Library.Classes.Federation.Federation> GetFederationsOfMember(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                return cached.Federations;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static RDN.Portable.Classes.League.Classes.League GetLeagueOfMember(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                return cached.CurrentLeague;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static List<MemberDisplay> GetLeagueMembers(Guid memId, Guid leagueId, int recordsToSkip, int numberOfRecordsToPull, bool hasLeftLeague = false)
        {
            try
            {
                var cached = GetCache(memId, true);
                if (cached.CurrentLeague != null && cached.CurrentLeague.LeagueId == leagueId)
                    if (!hasLeftLeague)
                        return cached.CurrentLeague.LeagueMembers.OrderBy(x => x.DerbyName).Skip(recordsToSkip).Take(numberOfRecordsToPull).ToList();
                    else
                        return RDN.Library.Classes.League.LeagueFactory.GetLeagueMembersDisplay(leagueId, hasLeftLeague);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<MemberDisplay>();
        }
        public static List<MemberDisplay> GetLeagueMembers(Guid memId, Guid leagueId)
        {
            try
            {
                var cached = GetCache(memId, true);
                if (cached.CurrentLeague != null && cached.CurrentLeague.LeagueId == leagueId)
                    return cached.CurrentLeague.LeagueMembers.OrderBy(x => x.DerbyName).ToList();

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<MemberDisplay>();
        }

        public static List<LeagueGroup> GetLeagueGroupsOfMember(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.CurrentLeague != null)
                    return cached.CurrentLeague.Groups;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static List<LeagueGroup> GetLeagueGroupsOfMember()
        {
            try
            {
                return GetLeagueGroupsOfMember(RDN.Library.Classes.Account.User.GetMemberId());
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        /// <summary>
        /// gets the forum id of the league the member is apart of.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static Guid GetForumIdForMemberLeague(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                return cached.CurrentLeagueForumId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }
        /// <summary>
        /// gets the calendar Id of the user.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static Guid GetCalendarIdForMemberLeague(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                return cached.CurrentLeagueCalendarId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }
        /// <summary>
        /// gets the forum id of the federation the member is apart of.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static Guid GetForumIdForMemberFederation(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                return cached.CurrentFederationForumId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }

        public static bool IsAdministrator(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached != null)
                    return cached.IsAdmin;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: memberId.ToString());
            }
            return false;
        }
        public static CalendarDefaultViewEnum GetCalendarDefaultView(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached != null && cached.memberDisplay != null && cached.memberDisplay.Settings != null)
                    return cached.memberDisplay.Settings.CalendarViewDefault;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: memberId.ToString());
            }
            return CalendarDefaultViewEnum.List_View;
        }

        public static int GetUnreadMessagesCount(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                return cached.UnreadMessagesCount;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }

        public static int GetJobsCount(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                return cached.JobsCount;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }
        public static void AddMessageCountToCache(int messageCount, Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, false);
                if (cached != null)
                {
                    cached.UnreadMessagesCount += messageCount;
                    UpdateCache(cached);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        public static void ResetMessageCountCache(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, false);
                if (cached != null)
                {
                    cached.UnreadMessagesCount = 0;
                    UpdateCache(cached);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        public static void UpdateCurrentLeagueMemberCache(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, false);
                if (cached != null)
                {
                    cached.CurrentLeague = RDN.Library.Classes.League.LeagueFactory.GetLeague(cached.memberDisplay.CurrentLeagueId);

                    UpdateCache(cached);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        public static List<Document> GetLeagueDocuments(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, false);
                if (cached != null)
                {
                    if (cached.LeagueDocuments.Count == 0)
                    {
                        cached.LeagueDocuments = RDN.Library.Classes.Document.DocumentRepository.GetLeagueDocuments(cached.CurrentLeague.LeagueId);
                        UpdateCache(cached);
                    }
                    return cached.LeagueDocuments;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<Document>();
        }
        public static List<Document> ClearLeagueDocument(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, false);
                if (cached != null)
                {
                    cached.LeagueDocuments = new List<Document>();
                    UpdateCache(cached);
                    return cached.LeagueDocuments;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<Document>();
        }
        public static MemberDisplay UpdateMemberDisplay(MemberDisplay memberDis)
        {
            try
            {
                var cached = GetCache(memberDis.MemberId, true);
                cached.memberDisplay = memberDis;
                UpdateCache(cached);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return memberDis;
        }
        public static RDN.Portable.Classes.League.Classes.League UpdateLeagueDisplay(RDN.Portable.Classes.League.Classes.League league, Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                cached.CurrentLeague = league;
                UpdateCache(cached);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return league;
        }
        public static bool CheckIsLeagueSubscriptionPaid(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                return cached.IsCurrentLeagueSubscriptionPaid;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static DateTime GetLeagueSubscriptionDate(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                return cached.SubscriptionEndDateOfCurrentLeague;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new DateTime();
        }
        public static bool IsDuesManagementLockedDown(Guid memberId)
        {
            try
            {
                var cached = GetCache(memberId, true);
                if (cached.Dues != null)
                    return cached.Dues.LockDownManagementToManagersOnly;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }


        /// <summary>
        /// clears the member cache
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="cache"></param>
        public static void Clear(Guid memberId)
        {
            try
            {
                if (HttpContext.Current.Cache["MemberCache" + memberId] != null)
                    HttpContext.Current.Cache.Remove("MemberCache" + memberId);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        public static void ClearApiCache(Guid memberId)
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_MEMBER_CACHE_API + memberId));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        public static void ClearLeagueMembersApiCache(Guid leagueId)
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_LEAGUE_MEMBER_CACHE_API + leagueId));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        public static void ClearWebSitesCache(Guid memberId)
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(LibraryConfig.InternalSite + UrlManager.URL_TO_CLEAR_MEMBER_CACHE + memberId));
                WebClient client1 = new WebClient();
                client1.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_MEMBER_CACHE_API + memberId));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        /// <summary>
        /// gets the cache of the member
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static MemberCache GetCache(Guid memberId, bool loadCacheIfNoneFound)
        {
            try
            {
                MemberCache dataObject = (MemberCache)HttpContext.Current.Cache["MemberCache" + memberId];
                if (dataObject == null && loadCacheIfNoneFound)
                {
                    //lock (ThisLock)
                    //{
                    dataObject = (MemberCache)HttpContext.Current.Cache["MemberCache" + memberId];

                    if (dataObject == null)
                    {
                        dataObject = new MemberCache();

                        dataObject.IsAdmin = System.Web.Security.Roles.IsUserInRole("admin");
                        dataObject.OwnersOfFederation = RDN.Library.Classes.Federation.Federation.GetAllOwnedFederations(memberId);

                        if (LibraryConfig.IsProduction)
                        {
                            dataObject.memberDisplay = RDN.Library.Classes.Account.User.GetMemberDisplay(memberId);
                        }
                        else
                        {
                            dataObject.OwnersOfFederation = new List<FederationOwnership>();
                            dataObject.memberDisplay = RDN.Library.Classes.Account.User.GetMemberDisplay(memberId, false, false);
                        }
                        dataObject.Leagues = RDN.Library.Classes.Account.User.GetLeaguesOfMember(memberId);
                        dataObject.UserStoreInfo = RDN.Library.Classes.Store.StoreGateway.GetStoreIdsFromInternalId(memberId);
                        dataObject.UnreadMessagesCount = RDN.Library.Classes.Messages.Messages.GetUnreadMessagesCount(memberId);


                        if (dataObject.memberDisplay != null)
                            dataObject.CurrentLeague = RDN.Library.Classes.League.LeagueFactory.GetLeague(dataObject.memberDisplay.CurrentLeagueId);

                        if (dataObject.CurrentLeague != null)
                        {
                            dataObject.JobsCount = RDN.Library.Classes.League.JobBoard.GetJobsCount(dataObject.CurrentLeague.LeagueId);
                            dataObject.Federations = RDN.Library.Classes.Federation.Federation.GetFederationsWithLeagueId(dataObject.CurrentLeague.LeagueId);
                            dataObject.CurrentFederation = RDN.Library.Classes.Federation.Federation.GetFederationWithLeagueId(dataObject.CurrentLeague.LeagueId);
                            for (int i = 0; i < dataObject.CurrentLeague.Groups.Count; i++)
                            {
                                var tempMem = dataObject.CurrentLeague.Groups[i].GroupMembers.Where(x => x.MemberId == memberId).FirstOrDefault();
                                if (tempMem != null)
                                {
                                    var gTemp = dataObject.CurrentLeague.Groups[i];
                                    gTemp.DoesReceiveGroupNotificationsCurrentMember = tempMem.DoesReceiveNewPostGroupNotifications;
                                    gTemp.DoesReceiveGroupBroadcastNotificationsCurrentMember = tempMem.DoesReceiveGroupBroadcastNotifications;
                                    dataObject.GroupsApartOf.Add(gTemp);
                                }
                            }
                            dataObject.Dues = Classes.Dues.DuesFactory.GetDuesSettingsByLeague(dataObject.CurrentLeague.LeagueId);
                        }
                        if (dataObject.Federations != null && dataObject.Federations.Count > 0)
                        {
                            dataObject.CurrentFederationForumId = RDN.Library.Classes.Forum.Forum.GetForumIdOfFederation(dataObject.Federations.FirstOrDefault().FederationId);
                            dataObject.CurrentFederationStoreInfo = RDN.Library.Classes.Store.StoreGateway.GetStoreIdsFromInternalId(dataObject.Federations.FirstOrDefault().FederationId);
                        }
                        else if (dataObject.OwnersOfFederation.Count > 0)
                        {
                            dataObject.CurrentFederationForumId = RDN.Library.Classes.Forum.Forum.GetForumIdOfFederation(dataObject.OwnersOfFederation.FirstOrDefault().Federation.FederationId);
                            dataObject.CurrentFederationStoreInfo = RDN.Library.Classes.Store.StoreGateway.GetStoreIdsFromInternalId(dataObject.OwnersOfFederation.FirstOrDefault().Federation.FederationId);
                        }
                        else
                            dataObject.CurrentFederationStoreInfo = new Classes.Store.Classes.Store();

                        if (dataObject.CurrentLeague != null)
                        {
                            dataObject.CurrentLeagueForumId = RDN.Library.Classes.Forum.Forum.GetForumIdOfLeague(dataObject.CurrentLeague.LeagueId);
                            dataObject.CurrentLeagueCalendarId = RDN.Library.Classes.Calendar.CalendarFactory.GetCalendarIdOfLeague(dataObject.CurrentLeague.LeagueId);
                            dataObject.CurrentLeagueStoreInfo = RDN.Library.Classes.Store.StoreGateway.GetStoreIdsFromInternalId(dataObject.CurrentLeague.LeagueId);
                            dataObject.LeagueDocuments = new List<Document>();
                            if (DateTime.UtcNow > dataObject.CurrentLeague.SubscriptionPeriodEnds)
                                dataObject.IsCurrentLeagueSubscriptionPaid = false;
                            else
                                dataObject.IsCurrentLeagueSubscriptionPaid = true;
                            dataObject.SubscriptionEndDateOfCurrentLeague = dataObject.CurrentLeague.SubscriptionPeriodEnds;
                        }
                        HttpContext.Current.Cache["MemberCache" + memberId] = dataObject;
                    }
                    //}
                }
                return dataObject;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static MemberCache UpdateCache(MemberCache memberCache)
        {
            try
            {
                if (memberCache != null && memberCache.memberDisplay != null)
                    lock (ThisLock)
                    {
                        HttpContext.Current.Cache["MemberCache" + memberCache.memberDisplay.MemberId] = memberCache;
                    }
                return memberCache;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
    }
}
