using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Common.Sponsors.Classes;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Account.Classes.Json;
using RDN.Library.Classes.Calendar;
using RDN.Library.Classes.Calendar.Models;
using RDN.Library.Classes.Document;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Game;
using RDN.Library.Classes.League;
using RDN.Library.Classes.League.Classes;
using RDN.Library.Classes.Mobile;
using RDN.Library.Classes.Store;
using RDN.Library.Classes.Store.Classes;
using RDN.Library.Classes.Store.Display;
using RDN.Library.Classes.Utilities;
using RDN.Mobile.Classes.Account;
using RDN.Portable.Classes.Sponsorship;
using RDN.Utilities.Config;
using RDN.Portable.Config;
using RDN.Portable.Models.Json;
using RDN.Portable.Models.Json.Public;
using RDN.Portable.Models.Json.Games;
using RDN.Portable.Classes.Account;
using RDN.Library.Cache.Singletons;
using RDN.Library.Classes.Payment.Money;
using System.Web.Security;
using RDN.Library.Classes.Site;
using RDN.Portable.Models.Json.Shop;
using RDN.Library.Classes.Social.Twitter;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Geo;
using RDN.Portable.Classes.Payment.Classes;
using RDN.Portable.Classes.League.Classes;
using RDN.Portable.Classes.Controls.Calendar;
using RDN.Library.Classes.Controls.Calendar;
using RDN.Library.Classes.Config;
using RDN.Portable.Classes.Url;
using Common.Site.Classes.Configations;
using RDN.Utilities.Error;

namespace RDN.Library.Cache
{
    public class SiteCache : CacheLock
    {
        public int NumberOfLeaguesSignedUpWithSite { get; set; }
        public int NumberOfMembersSignedUpWithSite { get; set; }
        public int NumberOfFederationsSignedUpWithSite { get; set; }
        public List<LeagueMemberClass> LeagueMemberClasses { get; set; }
        List<SkaterJson> PublicMembers { get; set; }
        SkaterJson MemberOfWeek { get; set; }
        RDN.Portable.Classes.League.Classes.League LeagueOfWeek { get; set; }
        List<MemberDisplay> PublicMemberFullProfile { get; set; }
        List<LeagueJsonDataTable> PublicLeagues { get; set; }
        List<StoreItemJson> StoreItems { get; set; }
        List<League> LeaguePages { get; set; }
        public List<CountryClass> Countries { get; set; }
        public DisplayStore ShopItems { get; set; }
        public int NumberOfItems { get; set; }
        public List<Tournament> CurrentTournaments { get; set; }
        public List<CurrentGameJson> PastGames { get; set; }
        //public List<CalendarEvent> CalendarEvents { get; set; }
        public EventsOutModel CalendarEventsModel { get; set; }
        public List<EventsForLeagueOutModel> CalendarEventsModelForLeagues { get; set; }
        public List<DataModels.Utilities.Sitemap> SiteMap { get; set; }
        public List<Document> Documents { get; set; }
        public List<NotificationMobileJson> MobileNotifications { get; set; }
        public List<CurrencyExchange> CurrencyExchangeRates { get; set; }
        public List<LeagueGroup> LeagueGroups { get; set; }
        public List<Tournament> Tournaments { get; set; }
        public List<MemberDisplayBasic> MemberIdsForUserNames { get; set; }
        public List<Common.Site.Classes.Configations.SiteConfiguration> SiteConfiguration { set; get; }
        public List<SponsorshipDisplay> Sponsorships { set; get; }
   

        public static Document GetDocument(Guid documentId)
        {
            Document doc = null;
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                doc = cached.Documents.Where(x => x.DocumentId == documentId).FirstOrDefault();
                if (doc == null)
                {
                    doc = DocumentRepository.GetDocumentLocation(documentId);
                    cached.Documents.Add(doc);
                    UpdateCache(cached);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return doc;
        }
        public static RDN.Portable.Classes.League.Classes.League GetLeagueOfWeek()
        {

            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.LeagueOfWeek == null)
                {
                    var hist = FrontPageHistorySite.GetLatestSiteHistory();
                    cached.LeagueOfWeek = GetLeague(hist.LeagueId);
                    UpdateCache(cached);
                }
                return cached.LeagueOfWeek;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new RDN.Portable.Classes.League.Classes.League();
        }
        public static SkaterJson GetSkaterOfWeek()
        {

            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.MemberOfWeek == null)
                {
                    var hist = FrontPageHistorySite.GetLatestSiteHistory();
                    cached.MemberOfWeek = RDN.Library.Classes.Account.User.GetMemberDisplayJson(hist.MemberId, true);
                    UpdateCache(cached);
                }
                return cached.MemberOfWeek;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new SkaterJson();
        }

        public static DisplayStore GetRandomPublishedStoreItems(int howMany, Guid merchantId = new Guid())
        {
            DisplayStore store = new DisplayStore();
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);

                if (merchantId == new Guid())
                    store.StoreItems = cached.ShopItems.StoreItems.OrderBy(x => Guid.NewGuid()).Take(howMany).ToList();
                else
                    store.StoreItems = cached.ShopItems.StoreItems.Where(x => x.Store.MerchantId == merchantId).OrderBy(x => Guid.NewGuid()).Take(howMany).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return store;
        }

        public static List<StoreItem> GetPublishedStoreItems(int count, int page)
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                return cached.ShopItems.StoreItems.OrderBy(x => x.StoreItemId).Skip(page * count).Take(count).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<StoreItem>();
        }
        public static List<StoreItem> SearchPublishedStoreItems(int count, int page, string searchString)
        {
            try
            {
                if (!string.IsNullOrEmpty(searchString))
                    searchString = searchString.ToLower();
                var cached = GetCache(HttpContext.Current.Cache);
                return cached.ShopItems.StoreItems.Where(x => (x.Store != null && x.Store.Name.ToLower().Contains(searchString)) || (x.Name != null && x.Name.ToLower().Contains(searchString)) || (x.Note != null && x.Note.ToLower().Contains(searchString))).Skip(page * count).Take(count).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<StoreItem>();
        }
        public static List<DataModels.Utilities.Sitemap> GetSiteMap()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);

                if (cached.SiteMap == null)
                {
                    cached.SiteMap = SitemapHelper.GetSitemap();
                    UpdateCache(cached);
                }
                return cached.SiteMap;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static List<NotificationMobileJson> GetMobileNotifications()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.MobileNotifications == null)
                {
                    cached.MobileNotifications = MobileNotificationFactory.GetAllMobileNotificationSettings();
                    UpdateCache(cached);
                }
                return cached.MobileNotifications;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static MemberDisplay GetPublicMemberFullWithUserId(Guid userId)
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.PublicMemberFullProfile == null)
                    cached.PublicMemberFullProfile = new List<MemberDisplay>();
                var mem = cached.PublicMemberFullProfile.Where(x => x.UserId == userId).FirstOrDefault();
                if (mem == null)
                {
                    var user = Membership.GetUser(userId);
                    if (user != null)
                    {
                        var member = RDN.Library.Classes.Account.User.GetMemberDisplay(user.UserName, true);
                        if (cached.PublicMemberFullProfile.Where(x => x.UserName == user.UserName).FirstOrDefault() == null && member != null)
                        {
                            cached.PublicMemberFullProfile.Add(member);
                            UpdateCache(cached);
                        }
                        return member;
                    }
                }
                return mem;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static MemberDisplay GetPublicMemberFull(string userName)
        {
            try
            {
                if (!String.IsNullOrEmpty(userName))
                {
                    var cached = GetCache(HttpContext.Current.Cache);
                    if (cached.PublicMemberFullProfile == null)
                        cached.PublicMemberFullProfile = new List<MemberDisplay>();

                    var mem = cached.PublicMemberFullProfile.Where(x => x.UserName == userName).FirstOrDefault();
                    if (mem == null)
                    {
                        var member = RDN.Library.Classes.Account.User.GetMemberDisplay(userName, true);
                        if (cached.PublicMemberFullProfile.Where(x => x.UserName == userName).FirstOrDefault() == null && member != null)
                        {
                            cached.PublicMemberFullProfile.Add(member);
                            UpdateCache(cached);
                        }
                        return member;
                    }
                    return mem;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: userName);
            }
            return null;
        }
        public static List<CurrencyExchange> ClearCurrencyExchanges()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                cached.CurrencyExchangeRates = CurrencyExchangeFactory.GetCurrencyExchangeRates();
                UpdateCache(cached);
                return cached.CurrencyExchangeRates;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static List<LeagueGroup> GetAllGroups()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.LeagueGroups.Count > 0)
                    return cached.LeagueGroups;

                cached.LeagueGroups = LeagueGroupFactory.GetGroups();
                UpdateCache(cached);
                return cached.LeagueGroups;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static List<LeagueGroup> ResetGroups()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);

                cached.LeagueGroups = LeagueGroupFactory.GetGroups();
                UpdateCache(cached);
                return cached.LeagueGroups;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static List<Tournament> GetTournaments()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.Tournaments.Count > 0)
                    return cached.Tournaments;

                cached.Tournaments = Tournament.GetLastCountTournaments(20);
                UpdateCache(cached);
                return cached.Tournaments;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static Tournament GetTournament(Guid id)
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                var tourn = cached.Tournaments.Where(x => x.Id == id).FirstOrDefault();
                if (tourn != null)
                    return tourn;

                tourn = Tournament.GetTournament(id);

                cached.Tournaments.Add(tourn);
                UpdateCache(cached);
                return tourn;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static List<Tournament> ClearTournaments()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);

                cached.Tournaments = new List<Tournament>();
                UpdateCache(cached);
                return cached.Tournaments;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static List<CurrencyExchange> GetCurrencyExchanges()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.CurrencyExchangeRates.Count > 0)
                    return cached.CurrencyExchangeRates;

                cached.CurrencyExchangeRates = CurrencyExchangeFactory.GetCurrencyExchangeRates();
                UpdateCache(cached);
                return cached.CurrencyExchangeRates;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static List<NotificationMobileJson> ClearMobileNotification()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                cached.MobileNotifications = MobileNotificationFactory.GetAllMobileNotificationSettings();
                UpdateCache(cached);
                return cached.MobileNotifications;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static RDN.Portable.Classes.League.Classes.League GetLeague(Guid leagueId)
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.LeaguePages == null)
                    cached.LeaguePages = new List<RDN.Portable.Classes.League.Classes.League>();
                RDN.Portable.Classes.League.Classes.League league = null;
                if (cached.LeaguePages.Count > 0)
                    league = cached.LeaguePages.Where(x => x.LeagueId == leagueId).FirstOrDefault();

                if (league == null)
                {
                    league = LeagueFactory.GetLeague(leagueId);
                    if (league != null)
                    {
                        cached.LeaguePages.Add(league);
                        UpdateCache(cached);
                    }
                }
                return league;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: leagueId.ToString());
            }
            return null;
        }
        public static SponsorshipDisplay GetSponsorship(Guid leagueId, long Id)
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.Sponsorships == null)
                    cached.Sponsorships = new List<SponsorshipDisplay>();
                SponsorshipDisplay sponsorshipDisplay = null;
                if (cached.Sponsorships.Count > 0)
                    sponsorshipDisplay = cached.Sponsorships.Where(x => x.OwnerId == leagueId && x.SponsorshipId == Id).FirstOrDefault();

                if (sponsorshipDisplay == null)
                {
                    var sponsorship = SponsorShipManager.GetData(Id, leagueId);
                    if (sponsorship != null)
                    {
                        sponsorshipDisplay = new SponsorshipDisplay();
                        sponsorshipDisplay.Description = sponsorship.Description;
                        sponsorshipDisplay.ExpiresDate = sponsorship.ExpiresDate;
                        sponsorshipDisplay.IsRemoved = sponsorship.IsRemoved;
                        sponsorshipDisplay.Price = sponsorship.Price;
                        sponsorshipDisplay.SponsorshipId = sponsorship.SponsorshipId;
                        sponsorshipDisplay.SponsorshipName = sponsorship.SponsorshipName;
                        sponsorshipDisplay.League = SiteCache.GetLeague(leagueId);
                        cached.Sponsorships.Add(sponsorshipDisplay);
                        UpdateCache(cached);
                        return sponsorshipDisplay;
                    }
                }
                return new SponsorshipDisplay();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: leagueId.ToString());
            }
            return null;
        }


        public static CalendarEventPortable GetCalendarEvent(Guid eventId)
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                var calEvent = cached.CalendarEventsModel.Events.Where(x => x.CalendarItemId == eventId).FirstOrDefault();
                if (calEvent == null)
                {
                    calEvent = CalendarEventFactory.GetEvent(eventId, new Guid());
                    if (calEvent != null && cached.CalendarEventsModel.Events.Where(x => x.CalendarItemId == eventId).FirstOrDefault() == null)
                    {
                        cached.CalendarEventsModel.Events.Add(calEvent);
                        UpdateCache(cached);
                    }
                }
                return calEvent;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static List<CalendarEvent> GetCalendarEvents(DateTime start, DateTime end)
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);

                EventsOutModel mod = new EventsOutModel();
                mod.StartDate = start;
                mod.EndDate = end;
                if (cached.CalendarEventsModel.StartDate < mod.StartDate && cached.CalendarEventsModel.EndDate > mod.EndDate)
                    return cached.CalendarEventsModel.Events.Where(x => x.StartDate > mod.StartDate && x.EndDate < mod.EndDate).OrderBy(x => x.StartDate).ToList();
                else
                {
                    if (cached.CalendarEventsModel.StartDate == new DateTime() || cached.CalendarEventsModel.StartDate > mod.StartDate)
                        cached.CalendarEventsModel.StartDate = mod.StartDate;
                    if (cached.CalendarEventsModel.EndDate == new DateTime() || cached.CalendarEventsModel.EndDate < mod.EndDate)
                        cached.CalendarEventsModel.EndDate = mod.EndDate;
                    mod.Events = CalendarEventFactory.GetPublicEvents(cached.CalendarEventsModel.StartDate, cached.CalendarEventsModel.EndDate, new Guid(), false);
                    cached.CalendarEventsModel.Events = mod.Events;
                    UpdateCache(cached);
                    return cached.CalendarEventsModel.Events.Where(x => x.StartDate > mod.StartDate && x.EndDate < mod.EndDate).OrderBy(x => x.StartDate).ToList();
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<CalendarEvent>();
        }
        public static EventsForLeagueOutModel GetCalendarEvents(Guid leagueId, int count, DateTime startDateToCheck)
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                var cal = cached.CalendarEventsModelForLeagues.Where(x => x.LeagueId == leagueId).FirstOrDefault();
                if (cal == null)
                {
                    EventsForLeagueOutModel mod = new EventsForLeagueOutModel();
                    mod.LeagueId = leagueId;
                    mod.StartDate = startDateToCheck;

                    mod.Events = CalendarEventFactory.GetPublicEvents(leagueId, count, startDateToCheck, new Guid(), false);
                    cached.CalendarEventsModelForLeagues.Add(mod);
                    UpdateCache(cached);
                    return mod;
                }
                return cal;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new EventsForLeagueOutModel();
        }
        public static List<CalendarEvent> SearchCalendarEvents(DateTime start, double longitude, double latitude, int page, int take)
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);

                EventsOutModel mod = new EventsOutModel();
                mod.StartDate = start;
                mod.EndDate = start.AddMonths(4);
                if (cached.CalendarEventsModel.StartDate < mod.StartDate && cached.CalendarEventsModel.EndDate > mod.EndDate)
                {
                    var coord = new RDN.Portable.Classes.Location.GeoCoordinate(latitude, longitude);
                    var nearest = cached.CalendarEventsModel.Events.Where(x => x.Location.Contact.Addresses.Count > 0).OrderBy(x => x.Location.Contact.Addresses.FirstOrDefault().Coords.GetDistanceTo(coord)).ToList();
                }
                else
                {
                    if (cached.CalendarEventsModel.StartDate == new DateTime() || cached.CalendarEventsModel.StartDate > mod.StartDate)
                        cached.CalendarEventsModel.StartDate = mod.StartDate;
                    if (cached.CalendarEventsModel.EndDate == new DateTime() || cached.CalendarEventsModel.EndDate < mod.EndDate)
                        cached.CalendarEventsModel.EndDate = mod.EndDate;
                    mod.Events = CalendarEventFactory.GetPublicEvents(cached.CalendarEventsModel.StartDate, cached.CalendarEventsModel.EndDate, new Guid(), false);
                    cached.CalendarEventsModel.Events = mod.Events;
                    UpdateCache(cached);
                    var coord = new RDN.Portable.Classes.Location.GeoCoordinate(latitude, longitude);
                    var nearest = cached.CalendarEventsModel.Events.Where(x => x.Location.Contact.Addresses.Count > 0).OrderBy(x => x.Location.Contact.Addresses.FirstOrDefault().Coords.GetDistanceTo(coord)).Skip(page * take).Take(take).ToList();

                    return nearest;
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<CalendarEvent>();
        }
        public static List<CalendarEvent> SearchCalendarEvents(DateTime start, string s, int page, int take)
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                if (String.IsNullOrEmpty(s))
                    s = "";
                s = s.ToLower();
                EventsOutModel mod = new EventsOutModel();
                mod.StartDate = start;
                mod.EndDate = start.AddMonths(4);
                if (cached.CalendarEventsModel.StartDate < mod.StartDate && cached.CalendarEventsModel.EndDate > mod.EndDate.AddDays(-10))
                    return cached.CalendarEventsModel.Events.Where(x => x.StartDate > mod.StartDate && x.EndDate < mod.EndDate).Where(x => x.Name != null).Where(x => x.Name.ToLower().Contains(s) || x.OrganizersName.Contains(s)).OrderBy(x => x.StartDate).Skip(page * take).Take(take).ToList();
                else
                {
                    if (cached.CalendarEventsModel.StartDate == new DateTime() || cached.CalendarEventsModel.StartDate > mod.StartDate)
                        cached.CalendarEventsModel.StartDate = mod.StartDate;
                    if (cached.CalendarEventsModel.EndDate == new DateTime() || cached.CalendarEventsModel.EndDate < mod.EndDate)
                        cached.CalendarEventsModel.EndDate = mod.EndDate;
                    mod.Events = CalendarEventFactory.GetPublicEvents(cached.CalendarEventsModel.StartDate, cached.CalendarEventsModel.EndDate, new Guid(), false);
                    cached.CalendarEventsModel.Events = mod.Events;
                    UpdateCache(cached);
                    return cached.CalendarEventsModel.Events.Where(x => x.StartDate > mod.StartDate && x.EndDate < mod.EndDate).Where(x => x.Name != null).Where(x => x.Name.ToLower().Contains(s) || x.OrganizersName.Contains(s)).OrderBy(x => x.StartDate).Skip(page * take).Take(take).ToList();
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<CalendarEvent>();
        }
        public static List<CalendarEvent> GetCalendarEvents(DateTime start, int page, int take)
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                EventsOutModel mod = new EventsOutModel();
                mod.StartDate = start;
                mod.EndDate = start.AddMonths(4);
                if (cached.CalendarEventsModel.StartDate < mod.StartDate && cached.CalendarEventsModel.EndDate > mod.EndDate)
                    return cached.CalendarEventsModel.Events.Where(x => x.StartDate > mod.StartDate && x.EndDate < mod.EndDate).Where(x => x.Name != null).OrderBy(x => x.StartDate).Skip(page * take).Take(take).ToList();
                else
                {
                    if (cached.CalendarEventsModel.StartDate == new DateTime() || cached.CalendarEventsModel.StartDate > mod.StartDate)
                        cached.CalendarEventsModel.StartDate = mod.StartDate;
                    if (cached.CalendarEventsModel.EndDate == new DateTime() || cached.CalendarEventsModel.EndDate < mod.EndDate)
                        cached.CalendarEventsModel.EndDate = mod.EndDate;
                    mod.Events = CalendarEventFactory.GetPublicEvents(cached.CalendarEventsModel.StartDate, cached.CalendarEventsModel.EndDate, new Guid(), false);
                    cached.CalendarEventsModel.Events = mod.Events;
                    UpdateCache(cached);
                    return cached.CalendarEventsModel.Events.Where(x => x.StartDate > mod.StartDate && x.EndDate < mod.EndDate).Where(x => x.Name != null).OrderBy(x => x.StartDate).Skip(page * take).Take(take).ToList();
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<CalendarEvent>();
        }
        public static List<SkaterJson> GetAllPublicMembers()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.PublicMembers == null)
                {
                    cached.PublicMembers = Classes.Account.User.GetAllPublicMembers();
                    cached.NumberOfMembersSignedUpWithSite = cached.PublicMembers.Count;
                    UpdateCache(cached);
                }
                return cached.PublicMembers;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<SkaterJson>();
        }
        public static MemberDisplay GetPublicMemberFull(Guid memberId)
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.PublicMemberFullProfile == null)
                    cached.PublicMemberFullProfile = new List<MemberDisplay>();
                var mem = cached.PublicMemberFullProfile.Where(x => x.MemberId == memberId).FirstOrDefault();
                if (mem == null)
                {
                    var member = RDN.Library.Classes.Account.User.GetMemberDisplay(memberId, true);
                    if (cached.PublicMemberFullProfile.Where(x => x.MemberId == memberId).FirstOrDefault() == null && member != null)
                        cached.PublicMemberFullProfile.Add(member);
                    return member;
                }
                return mem;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static List<SkaterJson> GetAllPublicMembersInLeague(string leagueId)
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.PublicMembers == null)
                {
                    cached.PublicMembers = Classes.Account.User.GetAllPublicMembers();
                    cached.NumberOfMembersSignedUpWithSite = cached.PublicMembers.Count;
                    UpdateCache(cached);
                }
                return cached.PublicMembers.Where(x => x.LeagueId == leagueId).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<SkaterJson>();
        }
        public static List<SkaterJson> GetAllPublicMembers(int page, int count)
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.PublicMembers == null)
                {
                    cached.PublicMembers = Classes.Account.User.GetAllPublicMembers();
                    cached.NumberOfMembersSignedUpWithSite = cached.PublicMembers.Count;
                    UpdateCache(cached);
                }
                return cached.PublicMembers.Skip(page * count).Take(count).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<SkaterJson>();
        }
        public static List<SkaterJson> GetAllPublicMembers(int page, int count, string alphaCharacter)
        {
            try
            {
                if (!String.IsNullOrEmpty(alphaCharacter))
                    alphaCharacter = alphaCharacter.ToLower();
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.PublicMembers == null)
                {
                    cached.PublicMembers = Classes.Account.User.GetAllPublicMembers();
                    cached.NumberOfMembersSignedUpWithSite = cached.PublicMembers.Count;
                    UpdateCache(cached);
                }
                if (!String.IsNullOrEmpty(alphaCharacter))
                    return cached.PublicMembers.Where(x => x.DerbyName.ToLower().StartsWith(alphaCharacter)).Skip(page * count).Take(count).ToList();

                return cached.PublicMembers.Skip(page * count).Take(count).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<SkaterJson>();
        }
        public static List<LeagueJsonDataTable> GetAllPublicLeagues(int page, int count, string alphaCharacter)
        {
            try
            {
                if (!String.IsNullOrEmpty(alphaCharacter))
                    alphaCharacter = alphaCharacter.ToLower();
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.PublicLeagues == null)
                {
                    cached.PublicLeagues = Classes.League.LeagueFactory.GetAllPublicLeagues();
                    cached.NumberOfLeaguesSignedUpWithSite = cached.PublicLeagues.Count;
                    UpdateCache(cached);
                }
                if (!String.IsNullOrEmpty(alphaCharacter))
                    return cached.PublicLeagues.Where(x => string.IsNullOrEmpty(alphaCharacter) || x.LeagueName.ToLower().StartsWith(alphaCharacter) || x.RuleSetsPlayed.ToLower().StartsWith(alphaCharacter)).Skip(page * count).Take(count).ToList();

                return cached.PublicLeagues.Skip(page * count).Take(count).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: alphaCharacter + ":" + page + ":" + count);
            }
            return new List<LeagueJsonDataTable>();
        }
        public static Guid GetMemberId(string userName)
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                var item = cached.MemberIdsForUserNames.Where(x => x.UserName != null && x.UserName == userName).FirstOrDefault();

                if (item == null)
                {
                    var user = Membership.GetUser(userName);
                    if (user == null)
                        return new Guid();
                    var member = RDN.Library.Classes.Account.User.GetMemberWithUserId((Guid)user.ProviderUserKey);
                    if (member != null)
                    {
                        cached.MemberIdsForUserNames.Add(new MemberDisplayBasic() { UserId = (Guid)user.ProviderUserKey, MemberId = member.MemberId, UserName = userName });
                        UpdateCache(cached);
                        return member.MemberId;
                    }
                }
                else
                    return item.MemberId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: userName);
            }
            return new Guid();
        }
        public static Guid GetUserId(string userName)
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                var item = cached.MemberIdsForUserNames.Where(x => x.UserName == userName).FirstOrDefault();

                if (item == null)
                {
                    var user = Membership.GetUser(userName);
                    if (user == null)
                        return new Guid();
                    var member = RDN.Library.Classes.Account.User.GetMemberWithUserId((Guid)user.ProviderUserKey);
                    if (member != null)
                    {
                        cached.MemberIdsForUserNames.Add(new MemberDisplayBasic() { UserId = (Guid)user.ProviderUserKey, MemberId = member.MemberId, UserName = userName });
                        UpdateCache(cached);
                        return member.MemberId;
                    }
                }
                else
                    return item.UserId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }

        public static SkaterJson GetPublicMember(Guid memberId)
        {
            try
            {
                string memId = memberId.ToString().Replace("-", "");
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.PublicMembers == null)
                {
                    cached.PublicMembers = Classes.Account.User.GetAllPublicMembers();
                    cached.NumberOfMembersSignedUpWithSite = cached.PublicMembers.Count;
                    UpdateCache(cached);
                }
                var mem = cached.PublicMembers.Where(x => x.MemberId == memId).FirstOrDefault();

                if (mem != null && mem.GotExtendedContent)
                    return mem;
                else if (mem != null && !mem.GotExtendedContent)
                {
                    var member = RDN.Library.Classes.Account.User.GetMemberDisplay(memberId);
                    mem.Wins = member.GamesWon;
                    mem.GamesCount = member.TotalGamesPlayed;
                    mem.Losses = member.GamesLost;
                    mem.Weight = member.WeightLbs.ToString();

                    mem.GotExtendedContent = true;
                    UpdateCache(cached);
                    return mem;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static LeagueJsonDataTable GetPublicLeague(Guid leagueId)
        {
            try
            {
                string memId = leagueId.ToString().Replace("-", "");
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.PublicLeagues == null)
                {
                    cached.PublicLeagues = Classes.League.LeagueFactory.GetAllPublicLeagues();
                    cached.NumberOfLeaguesSignedUpWithSite = cached.PublicLeagues.Count;
                    UpdateCache(cached);
                }
                var mem = cached.PublicLeagues.Where(x => x.LeagueId == memId).FirstOrDefault();

                if (mem != null && mem.GotExtendedContent)
                    return mem;
                else if (mem != null && !mem.GotExtendedContent)
                {
                    var member = RDN.Library.Classes.League.LeagueFactory.GetLeague(leagueId);
                    //mem.Wins = member.GamesWon;
                    //mem.GamesCount = member.TotalGamesPlayed;
                    //mem.Losses = member.GamesLost;
                    //mem.Weight = member.WeightLbs.ToString();

                    mem.GotExtendedContent = true;
                    UpdateCache(cached);
                    return mem;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new LeagueJsonDataTable();
        }
        public static List<SkaterJson> SearchAllPublicMembers(int page, int count, string searchString)
        {
            try
            {
                if (!String.IsNullOrEmpty(searchString))
                    searchString = searchString.ToLower();
                else
                    searchString = "";
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.PublicMembers == null)
                {
                    cached.PublicMembers = Classes.Account.User.GetAllPublicMembers();
                    cached.NumberOfMembersSignedUpWithSite = cached.PublicMembers.Count;
                    UpdateCache(cached);
                }
                return cached.PublicMembers.Where(x =>
                   (x.DerbyName != null && x.DerbyName.ToLower().Contains(searchString))
                    || (x.Bio != null && x.Bio.ToLower().Contains(searchString))
                    || (x.DerbyNumber != null && x.DerbyNumber.ToLower().Contains(searchString))
                    || (x.LeagueName != null && x.LeagueName.ToLower().Contains(searchString))
                    ).Skip(page * count).Take(count).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<SkaterJson>();
        }
        public static bool ClearPastGames()
        {
            var cached = GetCache(HttpContext.Current.Cache);
            cached.PastGames = new List<CurrentGameJson>();
            UpdateCache(cached);
            return true;
        }
        public static List<CurrentGameJson> GetPastGames(int page, int count)
        {
            try
            {

                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.PastGames != null && cached.PastGames.Count == 0)
                {
                    try
                    {
                        var games = Game.GetPastWeeksGames(100, 0);
                        if (games != null)
                        {
                            for (int i = 0; i < games.Count; i++)
                            {
                                CurrentGameJson c = new CurrentGameJson();
                                c.GameLocationFrom = Portable.Models.Json.Games.Enums.GameLocationFromEnum.SCOREBOARD;
                                c.StartTime = games[i].StartTime;
                                c.JamNumber = games[i].JamNumber;
                                c.PeriodNumber = games[i].PeriodNumber;
                                c.GameId = games[i].GameId;
                                c.RuleSet = games[i].RuleSet;
                                c.GameName = games[i].GameName;
                                c.Team2Name = games[i].Team2Name;
                                c.Team1Name = games[i].Team1Name;
                                c.Team1Score = games[i].Team1Score;
                                c.Team2Score = games[i].Team2Score;
                                c.Team1LogoUrl = games[i].Team1LogoUrl;
                                c.Team2LogoUrl = games[i].Team2LogoUrl;
                                c.HasGameEnded = games[i].HasGameEnded;
                                c.GameUrl = LibraryConfig.PublicSite + UrlManager.PublicSite_FOR_PAST_GAMES + "/" + c.GameId + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(games[i].GameName) + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(games[i].Team1Name) + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(games[i].Team2Name);
                                cached.PastGames.Add(c);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                    try
                    {
                        var games1 = Game.GetPastWeeksGamesFromRN(100, 0);
                        if (games1 != null)
                        {
                            for (int i = 0; i < games1.Count; i++)
                            {
                                CurrentGameJson c = new CurrentGameJson();
                                c.GameLocationFrom = Portable.Models.Json.Games.Enums.GameLocationFromEnum.ROLLINNEWS;
                                c.StartTime = games1[i].StartTime;
                                c.Team2Name = games1[i].Team2Name;
                                c.Team1Name = games1[i].Team1Name;
                                c.Team1Score = games1[i].Team1Score;
                                c.Team2Score = games1[i].Team2Score;
                                cached.PastGames.Add(c);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                    UpdateCache(cached);
                }

                return cached.PastGames.OrderByDescending(x => x.StartTime).Skip(page * count).Take(count).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<CurrentGameJson>();
        }
        public static List<LeagueJsonDataTable> SearchAllPublicLeagues(int page, int count, string searchString)
        {
            try
            {
                searchString = searchString.ToLower();
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.PublicLeagues == null)
                {
                    cached.PublicLeagues = Classes.League.LeagueFactory.GetAllPublicLeagues();
                    cached.NumberOfLeaguesSignedUpWithSite = cached.PublicLeagues.Count;
                    UpdateCache(cached);
                }
                return cached.PublicLeagues.Where(x =>
                    (x.LeagueName != null && x.LeagueName.ToLower().Contains(searchString))
                    || (x.State != null && x.State.ToLower().Contains(searchString))
                    || (x.City != null && x.City.ToLower().Contains(searchString))
                    || (x.RuleSetsPlayed != null && x.RuleSetsPlayed.ToLower().Contains(searchString))
                    ).Skip(page * count).Take(count).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<LeagueJsonDataTable>();
        }

        public static List<LeagueJsonDataTable> GetAllPublicLeagues()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.PublicLeagues == null)
                {
                    cached.PublicLeagues = Classes.League.LeagueFactory.GetAllPublicLeagues();
                    cached.NumberOfLeaguesSignedUpWithSite = cached.PublicLeagues.Count;
                    UpdateCache(cached);
                }

                return cached.PublicLeagues;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<LeagueJsonDataTable>();
        }
        public static List<LeagueJsonDataTable> GetAllPublicLeagues(int count, int page)
        {
            try
            {
                var leagues = GetAllPublicLeagues();
                return leagues.Skip(count * page).Take(count).ToList();

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<LeagueJsonDataTable>();
        }
        public static List<StoreItemJson> GetAllPublishedItems()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.StoreItems == null)
                {
                    StoreGateway sg = new StoreGateway();
                    var storeItems = sg.SearchStoreItems();

                    cached.StoreItems = storeItems;
                    UpdateCache(cached);
                }

                return cached.StoreItems;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<StoreItemJson>();
        }
        public static List<StoreItemJson> GetAllPublishedItems(int count, int page)
        {
            try
            {
                var storeItems = GetAllPublishedItems();
                return storeItems.Skip(count * page).Take(count).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<StoreItemJson>();
        }
        public static List<LeagueMemberClass> GetLeagueMemberClasses()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                return cached.LeagueMemberClasses;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<LeagueMemberClass>();
        }
        public static List<CountryClass> GetCountries()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                return cached.Countries;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<CountryClass>();
        }
        public static List<Tournament> GetCurrentTournaments()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                return cached.CurrentTournaments;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<Tournament>();
        }
        public static List<LeagueMemberClass> UpdateLeagueMemberClasses()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                cached.LeagueMemberClasses = RDN.Library.Classes.League.Classes.LeagueMemberClass.GetAllMemberClasses();
                UpdateCache(cached);
                return cached.LeagueMemberClasses;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<LeagueMemberClass>();
        }
        public static int GetNumberOfLeaguesSignedUp()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                return cached.NumberOfLeaguesSignedUpWithSite;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }
        public static int GetNumberOfItemsForSale()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                return cached.NumberOfItems;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;

        }
        public static int GetNumberOfMembersSignedUp()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                return cached.NumberOfMembersSignedUpWithSite;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }
        public static int GetNumberOfFederationsSignedUp()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                return cached.NumberOfFederationsSignedUpWithSite;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }
        public static void StartSite(System.Web.Caching.Cache cache)
        {
            try
            {
                GetCache(cache);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

        }

        private static SiteCache GetCache(System.Web.Caching.Cache cache)
        {
            try
            {
                SiteCache dataObject = (SiteCache)cache["SiteCache"];
                if (dataObject == null)
                {
                    lock (ThisLock)
                    {
                        dataObject = (SiteCache)cache["SiteCache"];
                        if (dataObject == null)
                        {
                            dataObject = new SiteCache();

                            dataObject.LeagueMemberClasses = LeagueMemberClass.GetAllMemberClasses();

                            if (LibraryConfig.IsProduction)
                            {
                                StoreGateway sg = new StoreGateway();
                                dataObject.ShopItems = sg.GetAllPublishedStoreItems();
                                dataObject.NumberOfItems = sg.GetAllPublishedStoreItemsCount();
                                dataObject.CurrentTournaments = Classes.Game.Tournament.GetCurrentTournaments();
                            }
                            else
                            {
                                //dataObject.PublicMembers = new List<SkaterJson>();
                                //dataObject.PublicLeagues = new List<LeagueJsonDataTable>();
                                dataObject.ShopItems = new DisplayStore();

                                //dataObject.PublicMembers = Classes.Account.User.GetAllPublicMembers();
                                //dataObject.PublicLeagues = Classes.League.League.GetAllPublicLeagues();
                                //dataObject.NumberOfLeaguesSignedUpWithRDNation = dataObject.PublicLeagues.Count;
                                //dataObject.NumberOfMembersSignedUpWithRDNation = dataObject.PublicMembers.Count;
                                StoreGateway sg = new StoreGateway();
                                dataObject.ShopItems = sg.GetAllPublishedStoreItems();
                                //dataObject.StoreItems = sg.SearchStoreItems();
                                //dataObject.NumberOfItems = sg.GetAllPublishedStoreItemsCount();
                                //dataObject.NumberOfLeaguesSignedUpWithRDNation = 0;
                                //dataObject.NumberOfMembersSignedUpWithRDNation = 0;

                                dataObject.CurrentTournaments = new List<Tournament>();
                            }

                            dataObject.MemberIdsForUserNames = new List<MemberDisplayBasic>();
                            dataObject.CurrencyExchangeRates = new List<CurrencyExchange>();
                            dataObject.PastGames = new List<CurrentGameJson>();
                            dataObject.NumberOfFederationsSignedUpWithSite = Classes.Federation.Federation.GetNumberOfFederations();

                            dataObject.Countries = Classes.Location.LocationFactory.GetCountries();
                            dataObject.PublicMemberFullProfile = new List<MemberDisplay>();
                            dataObject.LeaguePages = new List<RDN.Portable.Classes.League.Classes.League>();
                            //dataObject.CalendarEvents = new List<CalendarEvent>();
                            dataObject.CalendarEventsModel = new EventsOutModel();
                            dataObject.CalendarEventsModelForLeagues = new List<EventsForLeagueOutModel>();


                            dataObject.LeagueGroups = new List<LeagueGroup>();
                            dataObject.Documents = new List<Document>();
                            dataObject.Tournaments = new List<Tournament>();
                            dataObject.SiteConfiguration = new List<SiteConfiguration>();
                            //dataObject.MobileNotifications = new List<MobileNotification>();
                            cache["SiteCache"] = dataObject;
                        }
                    }

                }
                return dataObject;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        private static SiteCache UpdateCache(SiteCache siteCache)
        {
            try
            {
                lock (ThisLock)
                {
                    HttpContext.Current.Cache["SiteCache"] = siteCache;
                }
                return siteCache;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public static List<SiteConfiguration> GetConfiguration()
        {
            try
            {
                if (HttpContext.Current != null)
                {
                    var cached = GetCache(HttpContext.Current.Cache);
                    if (cached.SiteConfiguration == null || cached.SiteConfiguration.Count() == 0)
                    {
                        Common.Site.Classes.Configations.ConfigurationManager configManager = new ConfigurationManager();
                        cached.SiteConfiguration = configManager.GetConfigurations();
                        UpdateCache(cached);
                    }
                    return cached.SiteConfiguration;
                }
                else
                {
                    Common.Site.Classes.Configations.ConfigurationManager configManager = new ConfigurationManager();
                    return configManager.GetConfigurations();
                }
            }
            catch (Exception e)
            {
                Library.Classes.Error.ErrorDatabaseManager.AddException(e, e.GetType(), ErrorGroupEnum.Database);
            }
            return null;
        }

        public static string GetConfigurationValue(string key)
        {
            return GetConfiguration().Where(item => item.Key == key).FirstOrDefault().Value;
        }
    }
}
