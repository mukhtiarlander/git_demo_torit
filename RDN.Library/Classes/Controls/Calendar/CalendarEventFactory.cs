using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Context;
using RDN.Library.Classes.Calendar.Enums;
using RDN.Library.Classes.ContactCard;
using RDN.Library.Classes.Error;
using ScheduleWidget.ScheduledEvents;
using ScheduleWidget.Enums;
using RDN.Utilities.Strings;
using RDN.Utilities.Config;
using RDN.Library.Cache;
using DDay.iCal;
using DDay.iCal.Serialization.iCalendar;
using System.IO;
using RDN.Library.Classes.Game;
using RDN.Library.DataModels.Calendar.EventsCalendar;
using RDN.Library.Classes.Communications;
using RDN.Library.Classes.Account.Classes;
using System.Web;
using System.Device.Location;
using RDN.Utilities.Company.Google;
using RDN.Portable.Config;
using System.Drawing;
using MoreLinq;
using RDN.Library.Classes.League.Classes;
using RDN.Portable.Classes.Controls.Calendar.Enums;
using RDN.Portable.Classes.Controls.Calendar;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.ContactCard;
using RDN.Library.Classes.Controls.Calendar;
using RDN.Library.Classes.Mobile;
using System.Configuration;
using RDN.Library.Classes.Config;
using RDN.Portable.Classes.Url;
namespace RDN.Library.Classes.Calendar
{
    public class CalendarEventFactory
    {

        public CalendarEventFactory()
        {
        }

        public static Guid UpdateEventReOcurring(Guid calId, Guid reoccurringEventId, DateTime startDate, DateTime endDate, Guid locationId, string eventName, string link, string notes, bool AllowSelfCheckIn, FrequencyTypeEnum repeatFrequencySelected, bool sunday, bool monday, bool tuesday, bool wednesday, bool thursday, bool friday, bool saturday, EndsWhenReoccuringEnum endsWhen, DateTime endsOnDateReoccuring, long selectedEventTypeId, int monthlyIntervalId, string hexColor, bool isEventPublic, List<long> groupIds, Guid memId)
        {
            bool editColorsOfAllEvents = false;
            bool editGroupsOfAllEvents = false;
            bool editAllEventDates = false;
            ManagementContext dc = new ManagementContext();
            var ev = dc.CalendarEventsReocurring.Where(x => x.CalendarItemId == reoccurringEventId && x.Calendar.CalendarId == calId).FirstOrDefault();
            if (ev != null)
            {
                ev.IsPublic = isEventPublic;
                ev.LastDateEventsWereCreated = startDate.AddDays(-1);
                try
                {
                    int daysOfWeek = 0;
                    if (sunday)
                        daysOfWeek += (int)DayOfWeekEnum.Sun;
                    if (monday)
                        daysOfWeek += (int)DayOfWeekEnum.Mon;
                    if (tuesday)
                        daysOfWeek += (int)DayOfWeekEnum.Tue;
                    if (wednesday)
                        daysOfWeek += (int)DayOfWeekEnum.Wed;
                    if (thursday)
                        daysOfWeek += (int)DayOfWeekEnum.Thu;
                    if (friday)
                        daysOfWeek += (int)DayOfWeekEnum.Fri;
                    if (saturday)
                        daysOfWeek += (int)DayOfWeekEnum.Sat;

                    ScheduleWidget.ScheduledEvents.Event aEvent = null;
                    if (repeatFrequencySelected == FrequencyTypeEnum.Monthly)
                    {
                        aEvent = new ScheduleWidget.ScheduledEvents.Event()
                        {
                            Title = eventName,
                            FrequencyTypeOptions = repeatFrequencySelected,
                            DaysOfWeek = daysOfWeek,
                            MonthlyInterval = monthlyIntervalId
                        };
                    }
                    else
                    {
                        aEvent = new ScheduleWidget.ScheduledEvents.Event()
                        {
                            Title = eventName,
                            FrequencyTypeOptions = repeatFrequencySelected,
                            DaysOfWeek = daysOfWeek
                        };
                    }


                    if (endsWhen == EndsWhenReoccuringEnum.Never)
                    {
                        ev.EndReocurring = DateTime.UtcNow.AddYears(1);
                    }
                    else if (endsWhen == EndsWhenReoccuringEnum.On)
                    {
                        if (endsOnDateReoccuring != new DateTime())
                            ev.EndReocurring = endsOnDateReoccuring;
                        else
                        {
                            //if this breaks, then the if statement above is wrong... Must change to match
                            //1/1/0001 12:00:00 AM
                            ErrorDatabaseManager.AddException(new Exception("NewDateTime" + new DateTime()), new Exception().GetType(), additionalInformation: startDate + " " + endDate + " " + endsOnDateReoccuring + " " + endsWhen.ToString());
                            ev.EndReocurring = null;
                        }
                    }
                    // we delete all the events if the reoccuring event is modified.
                    //so that we have a fresh date.
                    if (ev.DaysOfWeekReocurring != aEvent.DaysOfWeek)
                        editAllEventDates = true;
                    ev.DaysOfWeekReocurring = aEvent.DaysOfWeek;

                    if (ev.FrequencyReocurring != aEvent.Frequency)
                        editAllEventDates = true;
                    ev.FrequencyReocurring = aEvent.Frequency;

                    if (ev.MonthlyIntervalReocurring != aEvent.MonthlyInterval)
                        editAllEventDates = true;
                    ev.MonthlyIntervalReocurring = aEvent.MonthlyInterval;

                    if (ev.StartReocurring != startDate)
                        editAllEventDates = true;
                    ev.StartReocurring = startDate;
                    ev.Calendar = dc.Calendar.Where(x => x.CalendarId == calId).FirstOrDefault();

                    if (ev.EndDate != endDate)
                        editAllEventDates = true;


                    DateTimeOffset dtOffEnd = new DateTimeOffset(endDate.Ticks, new TimeSpan(ev.Calendar.TimeZone, 0, 0));
                    DateTimeOffset dtOffStart = new DateTimeOffset(startDate.Ticks, new TimeSpan(ev.Calendar.TimeZone, 0, 0));

                    ev.StartDate = dtOffStart.UtcDateTime;
                    ev.EndDate = dtOffEnd.UtcDateTime;
                    ev.IsInUTCTime = true;

                    if (!String.IsNullOrEmpty(hexColor))
                    {
                        Color cc = ColorTranslator.FromHtml(hexColor);
                        int arb = cc.ToArgb();

                        var color = dc.Colors.Where(x => x.ColorIdCSharp == arb).FirstOrDefault();
                        if (ev.Color != color)
                            editColorsOfAllEvents = true;
                        ev.Color = color;
                    }
                    else
                    {
                        if (ev.Color != null)
                            editColorsOfAllEvents = true;
                        ev.Color = null;
                    }
                    if (ev.AllowSelfCheckIn != AllowSelfCheckIn)
                        editAllEventDates = true;
                    ev.AllowSelfCheckIn = AllowSelfCheckIn;
                    ev.Location = dc.Locations.Include("Contact").Include("Contact.Addresses").Include("Contact.Communications").Where(x => x.LocationId == locationId).FirstOrDefault();
                    ev.Name = eventName;
                    ev.Notes = notes;
                    ev.Link = link;

                    //removes any groups not in list.
                    var tempGroups = ev.Groups.ToList();
                    foreach (var group in tempGroups)
                    {
                        if (!groupIds.Contains(group.Group.Id))
                        {
                            ev.Groups.Remove(group);
                            editGroupsOfAllEvents = true;
                        }
                    }

                    //adds any groups not in list.
                    foreach (var id in groupIds)
                    {
                        var group = dc.LeagueGroups.Where(x => x.Id == id).FirstOrDefault();
                        if (group != null)
                        {
                            if (ev.Groups.Where(x => x.Group.Id == id).FirstOrDefault() == null)
                            {
                                RDN.Library.DataModels.Calendar.CalendarEventReoccuringGroup newGroup = new RDN.Library.DataModels.Calendar.CalendarEventReoccuringGroup();
                                newGroup.Group = group;
                                newGroup.Event = ev;
                                ev.Groups.Add(newGroup);
                                editGroupsOfAllEvents = true;
                            }
                        }
                    }

                    ev.EventType = dc.CalendarEventTypes.Where(x => x.CalendarEventTypeId == selectedEventTypeId).FirstOrDefault();





                    if (editColorsOfAllEvents)
                    {
                        foreach (var eventReoccure in ev.ReoccuringEvents.Where(x => x.IsRemovedFromCalendar == false))
                        {
                            eventReoccure.Color = ev.Color;
                        }
                    }
                    if (editGroupsOfAllEvents)
                    {
                        foreach (var eventReoccure in ev.ReoccuringEvents.Where(x => x.IsRemovedFromCalendar == false))
                        {
                            UpdateGroupsOfEvent(groupIds, dc, eventReoccure);
                        }
                    }

                    int c = dc.SaveChanges();

                    if (editAllEventDates)
                    {
                        var eventsReoccur = ev.ReoccuringEvents.Where(x => x.Attendees.Count == 0 && x.IsRemovedFromCalendar == false).ToList();
                        foreach (var eve in eventsReoccur)
                        {
                            eve.IsRemovedFromCalendar = true;
                        }

                        var schedule = new Schedule(aEvent);
                        var range = new DateRange()
                        {
                            StartDateTime = ev.StartReocurring
                        };
                        //date is null if the event is never ending.
                        if (ev.EndReocurring.HasValue)
                            range.EndDateTime = ev.EndReocurring.Value;
                        else
                            range.EndDateTime = endDate.AddMonths(1);

                        foreach (var date in schedule.Occurrences(range))
                        {

                            DateTime endDateEv = new DateTime();
                            DateTime startDateEv = new DateTime();

                            if (!ev.IsInUTCTime)
                            {
                                startDateEv = new DateTime(date.Year, date.Month, date.Day, ev.StartDate.Hour, ev.StartDate.Minute, ev.StartDate.Second);
                                endDateEv = new DateTime(date.Year, date.Month, date.Day, ev.EndDate.Hour, ev.EndDate.Minute, ev.EndDate.Second);
                            }
                            else
                            {
                                //we have to create a temp dates so we can add the timezone information without going back a day
                                //if the time being used is on the border.
                                //without the tempdates 1/4/2013 7pm turned into 1/3/2013 7pm because the timezones didn't account for the 
                                //fact the dates were already in utc.
                                var startTempDate = new DateTime(date.Year, date.Month, date.Day, ev.StartDate.Hour, ev.StartDate.Minute, ev.StartDate.Second) + new TimeSpan(ev.Calendar.TimeZone, 0, 0);
                                var endTempDate = new DateTime(date.Year, date.Month, date.Day, ev.EndDate.Hour, ev.EndDate.Minute, ev.EndDate.Second) + new TimeSpan(ev.Calendar.TimeZone, 0, 0);
                                startDateEv = new DateTime(date.Year, date.Month, date.Day, startTempDate.Hour, startTempDate.Minute, startTempDate.Second);
                                endDateEv = new DateTime(date.Year, date.Month, date.Day, endTempDate.Hour, endTempDate.Minute, endTempDate.Second);
                            }

                            //only add more dates to dates in the future.  No need to go back in time.
                            if (startDateEv > DateTime.UtcNow)
                                CalendarEventFactory.CreateNewEvent(ev.Calendar.CalendarId, startDateEv, endDateEv, locationId, ev.Name, ev.Link, ev.Notes, ev.AllowSelfCheckIn, ev.IsPublic, true, ev.CalendarItemId, ev.EventType.CalendarEventTypeId, false, ev.TicketUrl, hexColor, new List<long>(), memId);

                        }
                    }
                    c = dc.SaveChanges();

                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: startDate + " " + endDate + " " + endsOnDateReoccuring + " " + endsWhen.ToString());
                }
            }
            return reoccurringEventId;
        }
        /// <summary>
        /// updates the groups for the calendar event.
        /// </summary>
        /// <param name="groupIds"></param>
        /// <param name="dc"></param>
        /// <param name="calevent"></param>
        private static void UpdateGroupsOfEvent(List<long> groupIds, ManagementContext dc, DataModels.Calendar.CalendarEvent calevent)
        {
            //removes any groups not in list.
            var tempGroupsForEachEvent = calevent.Groups.ToList();
            foreach (var group in tempGroupsForEachEvent)
            {
                if (!groupIds.Contains(group.Group.Id))
                {
                    calevent.Groups.Remove(group);
                }
            }

            //adds any groups not in list.
            foreach (var id in groupIds)
            {
                var group = dc.LeagueGroups.Where(x => x.Id == id).FirstOrDefault();
                if (group != null)
                {
                    if (calevent.Groups.Where(x => x.Group.Id == id).FirstOrDefault() == null)
                    {
                        RDN.Library.DataModels.Calendar.CalendarEventGroup newGroup = new RDN.Library.DataModels.Calendar.CalendarEventGroup();
                        newGroup.Group = group;
                        newGroup.Event = calevent;
                        calevent.Groups.Add(newGroup);
                    }
                }
            }
        }


        public static bool UpdateEvent(Guid calId, Guid eventId, DateTime startDate, DateTime endDate, Guid locationId, string eventName, string link, string notes, bool AllowSelfCheckIn, long selectedEventTypeId, string ticketUrl, string hexColor, bool isEventPublic, List<long> groupIds)
        {
            ManagementContext dc = new ManagementContext();
            var ev = dc.CalendarEvents.Where(x => x.CalendarItemId == eventId && x.Calendar.CalendarId == calId).FirstOrDefault();
            if (ev != null)
            {
                try
                {
                    if (!String.IsNullOrEmpty(hexColor))
                    {
                        Color cc = ColorTranslator.FromHtml(hexColor);
                        int arb = cc.ToArgb();
                        ev.Color = dc.Colors.Where(x => x.ColorIdCSharp == arb).FirstOrDefault();
                    }
                    else
                        ev.Color = null;

                    ev.TicketUrl = ticketUrl;
                    ev.Calendar = dc.Calendar.Where(x => x.CalendarId == calId).FirstOrDefault();
                    ev.AllowSelfCheckIn = AllowSelfCheckIn;
                    ev.Location = dc.Locations.Include("Contact").Include("Contact.Addresses").Include("Contact.Communications").Where(x => x.LocationId == locationId).FirstOrDefault();
                    ev.Name = eventName;
                    ev.Notes = notes;
                    ev.Link = link;
                    ev.IsPublicEvent = isEventPublic;
                    ev.EventType = dc.CalendarEventTypes.Where(x => x.CalendarEventTypeId == selectedEventTypeId).FirstOrDefault();

                    DateTimeOffset dtOffEnd = new DateTimeOffset(endDate.Ticks, new TimeSpan(ev.Calendar.TimeZone, 0, 0));
                    DateTimeOffset dtOffStart = new DateTimeOffset(startDate.Ticks, new TimeSpan(ev.Calendar.TimeZone, 0, 0));
                    ev.StartDate = dtOffStart.UtcDateTime;
                    ev.EndDate = dtOffEnd.UtcDateTime;
                    ev.IsInUTCTime = true;


                    UpdateGroupsOfEvent(groupIds, dc, ev);

                    int c = dc.SaveChanges();
                    return c > 0;
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: startDate + " " + endDate + " ");
                }
            }
            return false;
        }


        public static bool DeleteEvent(Guid calendarId, Guid eventId)
        {
            try
            {
                var dc = new ManagementContext();
                var e = (from xx in dc.CalendarEvents
                         where xx.Calendar.CalendarId == calendarId
                         where xx.CalendarItemId == eventId
                         select xx).FirstOrDefault();
                if (e != null)
                {
                    e.Calendar = e.Calendar;
                    e.IsRemovedFromCalendar = true;
                    int c = dc.SaveChanges();
                    return c > 0;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool DeleteEventReccurring(Guid calendarId, Guid eventId)
        {
            try
            {
                var dc = new ManagementContext();
                var e = (from xx in dc.CalendarEventsReocurring
                         where xx.Calendar.CalendarId == calendarId
                         where xx.CalendarItemId == eventId
                         select xx).FirstOrDefault();
                if (e != null)
                {
                    e.Calendar = e.Calendar;
                    e.IsRemovedFromCalendar = true;
               
                    var evs = e.ReoccuringEvents.Where(x => x.Attendees.Count == 0 && x.IsRemovedFromCalendar == false);
                    foreach (var ev in evs)
                    {
                        ev.IsRemovedFromCalendar = true;
                    }
                    int c = dc.SaveChanges();
                    return c > 0;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static RDN.Library.Classes.Controls.Calendar.CalendarEvent GetEventReocurring(Guid eventId, Guid calendarId = new Guid())
        {
            RDN.Library.Classes.Controls.Calendar.CalendarEvent ev = new RDN.Library.Classes.Controls.Calendar.CalendarEvent();
            try
            {
                var dc = new ManagementContext();
                var e = (from xx in dc.CalendarEventsReocurring.Include("Location").Include("Location.Contact").Include("Location.Contact.Addresses").Include("Location.Contact.Communications")
                         where xx.Calendar.CalendarId == calendarId
                         where xx.CalendarItemId == eventId
                         where xx.IsRemovedFromCalendar == false
                         select new
                         {
                             xx.TicketUrl,
                             xx.IsInUTCTime,
                             xx.Calendar.IsCalendarInUTC,
                             xx.Calendar.TimeZone,
                             xx.AllowSelfCheckIn,
                             xx.CalendarItemId,
                             xx.EndDate,
                             xx.Link,
                             xx.Calendar.LeagueOwners,
                             xx.Name,
                             xx.Notes,
                             xx.StartDate,
                             xx.Location,
                             contactCart = xx.Location.Contact,
                             addresses = xx.Location.Contact.Addresses,
                             coms = xx.Location.Contact.Communications,
                             xx.PointsForEvent,
                             xx.EventType,
                             xx.DaysOfWeekReocurring,
                             xx.StartReocurring,
                             xx.EndReocurring,
                             xx.FrequencyReocurring,
                             xx.IsPublic,
                             xx.MonthlyIntervalReocurring,
                             xx.LastModified,
                             xx.Created,
                             xx.Color,
                             xx.Groups
                         }).FirstOrDefault();
                if (calendarId == new Guid() && e == null)
                {
                    e = (from xx in dc.CalendarEventsReocurring.Include("Location").Include("Location.Contact").Include("Location.Contact.Addresses").Include("Location.Contact.Communications")
                         where xx.Calendar.CalendarId == calendarId
                         where xx.CalendarItemId == eventId
                         where xx.IsRemovedFromCalendar == false
                         select new
                         {
                             xx.TicketUrl,
                             xx.IsInUTCTime,
                             xx.Calendar.IsCalendarInUTC,
                             xx.Calendar.TimeZone,
                             xx.AllowSelfCheckIn,
                             xx.CalendarItemId,
                             xx.EndDate,
                             xx.Link,
                             xx.Calendar.LeagueOwners,
                             xx.Name,
                             xx.Notes,
                             xx.StartDate,
                             xx.Location,
                             contactCart = xx.Location.Contact,
                             addresses = xx.Location.Contact.Addresses,
                             coms = xx.Location.Contact.Communications,
                             xx.PointsForEvent,
                             xx.EventType,
                             xx.DaysOfWeekReocurring,
                             xx.StartReocurring,
                             xx.EndReocurring,
                             xx.FrequencyReocurring,
                             xx.IsPublic,
                             xx.MonthlyIntervalReocurring,
                             xx.LastModified,
                             xx.Created,
                             xx.Color,
                             xx.Groups
                         }).FirstOrDefault();
                }

                if (e == null)
                    return null;

                ev.AllowSelfCheckIn = e.AllowSelfCheckIn;
                ev.CalendarItemId = e.CalendarItemId;
                if (e.Color != null)
                {
                    var c = Color.FromArgb(e.Color.ColorIdCSharp);
                    ev.ColorTempSelected = ColorTranslator.ToHtml(c);
                }

                ev.Link = e.Link;

                var aEvent = new ScheduleWidget.ScheduledEvents.Event()
                {
                    Title = e.Name,
                    FrequencyTypeOptions = (FrequencyTypeEnum)Enum.Parse(typeof(FrequencyTypeEnum), e.FrequencyReocurring.ToString()),
                    DaysOfWeek = e.DaysOfWeekReocurring,
                    MonthlyInterval = e.MonthlyIntervalReocurring
                };
                if (e.EndReocurring.HasValue)
                {
                    ev.EndDateReoccurring = e.EndReocurring.GetValueOrDefault();
                    ev.EndDateReoccurringDisplay = e.EndReocurring.GetValueOrDefault().ToShortDateString();
                }
                ev.StartDateReoccurring = e.StartReocurring;
                ev.StartDateReoccurringDisplay = e.StartReocurring.ToShortDateString() + " " + e.StartReocurring.ToShortTimeString();
                ev.EventReoccurring = aEvent;
                ev.TicketUrl = e.TicketUrl;
                ev.SiteUrl = LibraryConfig.PublicSite  + UrlManager.WEBSITE_EVENT_URL + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(e.Name) + "/" + e.CalendarItemId.ToString().Replace("-", "");

                foreach (var owner in e.LeagueOwners)
                {
                    if (owner.League != null)
                    {
                        ev.OrganizersId = owner.League.LeagueId;
                        ev.OrganizersName = owner.League.Name;
                        break;
                    }
                }
                GoogleCalendar gc = new GoogleCalendar();
                if (e.Location != null)
                {
                    ev.Location.LocationName = e.Location.LocationName;
                    ev.Location.LocationId = e.Location.LocationId;
                }
                gc.Location = ev.Location.LocationName + ", ";
                if (e.addresses.FirstOrDefault() != null)
                {
                    RDN.Portable.Classes.ContactCard.Address address = new RDN.Portable.Classes.ContactCard.Address();
                    address.Address1 = e.addresses.FirstOrDefault().Address1;
                    address.Address2 = e.addresses.FirstOrDefault().Address2;
                    address.AddressId = e.addresses.FirstOrDefault().AddressId;
                    address.CityRaw = e.addresses.FirstOrDefault().CityRaw;
                    if (e.addresses.FirstOrDefault().Country != null)
                        address.Country = e.addresses.FirstOrDefault().Country.Code;
                    address.StateRaw = e.addresses.FirstOrDefault().StateRaw;
                    address.Zip = e.addresses.FirstOrDefault().Zip;
                    gc.Location += address.Address1 + " " + address.Address2 + ", " + address.CityRaw + ", " + address.StateRaw + " " + address.Zip;
                    ev.Location.Contact.Addresses.Add(address);
                }
                ev.EventType = new Portable.Classes.Controls.Calendar.CalendarEventType();
                if (e.EventType != null)
                {
                    ev.EventType.EventTypeName = e.EventType.EventTypeName;
                    ev.EventType.PointsForExcused = e.EventType.PointsForExcused;
                    ev.EventType.PointsForNotPresent = e.EventType.PointsForNotPresent;
                    ev.EventType.PointsForPartial = e.EventType.PointsForPartial;
                    ev.EventType.PointsForPresent = e.EventType.PointsForPresent;
                    ev.EventType.PointsForTardy = e.EventType.PointsForTardy;
                    ev.EventType.CalendarEventTypeId = e.EventType.CalendarEventTypeId;
                }
                ev.Name = e.Name;
                ev.Notes = e.Notes;

                foreach (var g in e.Groups)
                {
                    var gTemp = League.Classes.LeagueGroupFactory.DisplayGroup(g.Group);
                    ev.GroupsForEvent.Add(gTemp);
                    foreach (var mem in gTemp.GroupMembers)
                    {
                        if (ev.MembersApartOfEvent.Where(x => x.MemberId == mem.MemberId).FirstOrDefault() == null)
                        {
                            RDN.Portable.Classes.Controls.Calendar.CalendarAttendance a = new Portable.Classes.Controls.Calendar.CalendarAttendance();
                            a.MemberId = mem.MemberId;
                            a.MemberName = mem.DerbyName;
                            a.IsCheckedIn = false;
                            a.FullName = mem.Firstname + " " + mem.LastName;
                            a.MemberNumber = mem.PlayerNumber;
                            ev.MembersApartOfEvent.Add(a);
                            ev.MembersToCheckIn.Add(a);
                        }
                    }
                }

                if (e.LastModified > new DateTime(2013, 11, 23) || e.Created > new DateTime(2013, 11, 23))
                {
                    ev.NotesHtml = e.Notes;
                }
                else if (e.Created < new DateTime(2013, 11, 23))
                {
                    if (!String.IsNullOrEmpty(e.Notes))
                    {
                        RDN.Library.Util.MarkdownSharp.Markdown markdown = new RDN.Library.Util.MarkdownSharp.Markdown();
                        markdown.AutoHyperlink = true;
                        markdown.LinkEmails = true;
                        ev.NotesHtml = HtmlSanitize.FilterHtmlToWhitelist(markdown.Transform(e.Notes)).Replace("</p>", "</p><br/>");
                    }
                }
                ev.IsPublicEvent = e.IsPublic;

                gc.Description = ev.Notes;
                if (!e.IsInUTCTime)
                {
                    ev.EndDate = e.EndDate;
                    ev.StartDate = e.StartDate;
                }
                else
                {
                    ev.StartDate = (e.StartDate + new TimeSpan(e.TimeZone, 0, 0));
                    ev.EndDate = (e.EndDate + new TimeSpan(e.TimeZone, 0, 0));
                }
                gc.EndDate = ev.EndDate.ToUniversalTime();
                gc.StartDate = ev.StartDate.ToUniversalTime();
                gc.Website = ev.Link;
                gc.WebsiteName = ev.Link;
                gc.What = ev.Name;

                ev.GoogleCalendarUrl = gc.GetGoogleUrl();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return ev;
        }

        public static MemoryStream ExportEvent(Guid eventId, Guid currentMemberId, Guid calendarId = new Guid())
        {

            try
            {
                var ev = GetEvent(eventId, currentMemberId, calendarId);
                iCalendar iCal = new iCalendar();
                DDay.iCal.Event e = iCal.Create<DDay.iCal.Event>();

                iCal.Method = CalendarMethods.Publish;

                e.Start = new iCalDateTime(ev.StartDate);
                e.End = new iCalDateTime(ev.EndDate);
                string address = String.Empty;

                if (ev.Location != null)
                {
                    e.Location = ev.Location.LocationName;
                    if (ev.Location.Contact.Addresses.FirstOrDefault() != null)
                    {
                        if (!String.IsNullOrEmpty(ev.Location.Contact.Addresses.FirstOrDefault().Address1))
                            address += ev.Location.Contact.Addresses.FirstOrDefault().Address1 + ", ";
                        if (!String.IsNullOrEmpty(ev.Location.Contact.Addresses.FirstOrDefault().Address2))
                            address += ev.Location.Contact.Addresses.FirstOrDefault().Address2 + ", ";

                        if (!String.IsNullOrEmpty(ev.Location.Contact.Addresses.FirstOrDefault().CityRaw))
                            address += ev.Location.Contact.Addresses.FirstOrDefault().CityRaw + ", ";

                        if (!String.IsNullOrEmpty(ev.Location.Contact.Addresses.FirstOrDefault().StateRaw))
                            address += ev.Location.Contact.Addresses.FirstOrDefault().StateRaw + ", ";

                        if (!String.IsNullOrEmpty(ev.Location.Contact.Addresses.FirstOrDefault().Zip))
                            address += ev.Location.Contact.Addresses.FirstOrDefault().Zip + ", ";

                        if (!String.IsNullOrEmpty(ev.Location.Contact.Addresses.FirstOrDefault().Country))
                            address += ev.Location.Contact.Addresses.FirstOrDefault().Country;
                    }
                    e.Location += " " + address;

                }
                //e.Name = ev.Name;
                if (!String.IsNullOrEmpty(ev.OrganizersName))
                    e.Description = "By " + ev.OrganizersName;

                e.Description += " " + ev.Notes;
                e.Summary = ev.Name;
                e.UID = ev.CalendarItemId.ToString().Replace("-", "");

                MemoryStream stream = new MemoryStream();
                var icalSerializer = new iCalendarSerializer(iCal);
                icalSerializer.Serialize(iCal, stream, System.Text.Encoding.Default);
                stream.Position = 0;
                return stream;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static List<CalendarEvent> GetPublicEvents(DateTime start, DateTime end, Guid currentMemberId, bool isAttendanceManagerOrBetter)
        {
            List<CalendarEvent> evs = new List<CalendarEvent>();
            try
            {
                var dc = new ManagementContext();
                var e = (from xx in dc.CalendarEvents.Include("Calendar.LeagueOwners").Include("Calendar.LeagueOwners.League").Include("Calendar.LeagueOwners.League.Logo").Include("Location").Include("Location.Contact").Include("Location.Contact.Addresses").Include("Location.Contact.Communications")
                         where xx.IsPublicEvent == true
                         where xx.IsRemovedFromCalendar == false
                         where xx.StartDate > start
                         where xx.EndDate < end
                         select xx).AsParallel().OrderBy(x => x.StartDate).ToList();

                foreach (var ev in e)
                {
                    evs.Add(DisplayEvent(ev, currentMemberId, isAttendanceManagerOrBetter));
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return evs;
        }
        public static List<CalendarEvent> GetDeletedEvents()
        {
            List<CalendarEvent> evs = new List<CalendarEvent>();
            try
            {
                var dc = new ManagementContext();
                var e = (from xx in dc.CalendarEvents.Include("Calendar.LeagueOwners").Include("Calendar.LeagueOwners.League").Include("Calendar.LeagueOwners.League.Logo").Include("Location").Include("Location.Contact").Include("Location.Contact.Addresses").Include("Location.Contact.Communications")

                         where xx.IsRemovedFromCalendar == true
                         where xx.Attendees.Count > 0
                         select xx);

                foreach (var ev in e)
                {
                    evs.Add(DisplayEvent(ev, new Guid(), true));
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return evs;
        }
        public static List<CalendarEvent> UnDeleteAttendanceEvents(Guid calId)
        {
            List<CalendarEvent> evs = new List<CalendarEvent>();
            try
            {
                var dc = new ManagementContext();
                var e = (from xx in dc.CalendarEvents

                         where xx.IsRemovedFromCalendar == true
                         where xx.Attendees.Count > 0
                         where xx.Calendar.CalendarId == calId
                         select xx);



                foreach (var item in e)
                {
                    item.Calendar = item.Calendar;
                    item.IsRemovedFromCalendar = false;
                }

                int c = dc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return evs;
        }
        public static List<CalendarEvent> GetPublicEvents(Guid leagueId, int count, DateTime startDateToCheck, Guid currentMemberId, bool isAttendanceManagerOrBetter)
        {
            List<CalendarEvent> evs = new List<CalendarEvent>();
            try
            {
                var dc = new ManagementContext();
                var cal = dc.CalendarLeagueOwners.Where(x => x.League.LeagueId == leagueId).FirstOrDefault();
                if (cal != null && cal.Calendar != null)
                {
                    var e = (from xx in dc.CalendarEvents.Include("Location").Include("Location.Contact").Include("Location.Contact.Addresses").Include("Location.Contact.Communications")
                             where xx.IsPublicEvent == true
                             where xx.IsRemovedFromCalendar == false
                             where xx.StartDate > startDateToCheck
                             where xx.Calendar.CalendarId == cal.Calendar.CalendarId
                             select xx).Take(count).AsParallel().OrderBy(x => x.StartDate).ToList();

                    foreach (var ev in e)
                    {
                        evs.Add(DisplayEvent(ev, currentMemberId, isAttendanceManagerOrBetter));
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return evs;
        }

        public static List<CalendarEventPortable> SearchPublicEvents(DateTime start, string s, int take, Guid currentMemberId, bool isAttendanceManagerOrBetter)
        {
            List<CalendarEventPortable> evs = new List<CalendarEventPortable>();
            try
            {
                var dc = new ManagementContext();
                var e = (from xx in dc.CalendarEvents.Include("Location").Include("Location.Contact").Include("Location.Contact.Addresses").Include("Location.Contact.Communications")
                         where xx.IsPublicEvent == true
                         where xx.IsRemovedFromCalendar == false
                         where xx.Name.Contains(s)
                         where xx.StartDate > start
                         select xx).AsParallel().OrderBy(x => x.StartDate).Take(take).ToList();

                foreach (var ev in e)
                {
                    evs.Add(DisplayEvent(ev, currentMemberId, isAttendanceManagerOrBetter));
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return evs;
        }
        public static RDN.Library.Classes.Controls.Calendar.CalendarEvent GetBirthdayEvent(Guid memberId)
        {
            RDN.Library.Classes.Controls.Calendar.CalendarEvent ev = new RDN.Library.Classes.Controls.Calendar.CalendarEvent();
            try
            {
                var dc = new ManagementContext();
                var mem = MemberCache.GetMemberDisplay(memberId);

                ev.EndDate = mem.DOB;
                ev.StartDate = mem.DOB;
                ev.EventType = new Portable.Classes.Controls.Calendar.CalendarEventType();
                ev.EventType.EventType = CalendarEventTypeEnum.Birthday;
                ev.Name = mem.DerbyName;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return ev;
        }
        public static RDN.Library.Classes.Controls.Calendar.CalendarEvent GetStartedSkatingEvent(Guid memberId)
        {
            RDN.Library.Classes.Controls.Calendar.CalendarEvent ev = new RDN.Library.Classes.Controls.Calendar.CalendarEvent();
            try
            {
                var dc = new ManagementContext();
                var mem = MemberCache.GetMemberDisplay(memberId);

                ev.EndDate = mem.StartedSkating.GetValueOrDefault();
                ev.StartDate = mem.StartedSkating.GetValueOrDefault();
                ev.EventType = new Portable.Classes.Controls.Calendar.CalendarEventType();
                ev.EventType.EventType = CalendarEventTypeEnum.StartSkatingDate;
                ev.Name = mem.DerbyName;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return ev;
        }

        public static RDN.Library.Classes.Controls.Calendar.CalendarEvent GetEvent(Guid eventId, Guid currentMemberId, Guid calendarId = new Guid())
        {
            RDN.Library.Classes.Controls.Calendar.CalendarEvent ev = new RDN.Library.Classes.Controls.Calendar.CalendarEvent();
            try
            {
                var dc = new ManagementContext();
                var e = (from xx in dc.CalendarEvents.Include("Location").Include("Location.Contact").Include("Location.Contact.Addresses").Include("Location.Contact.Communications")
                         where xx.Calendar.CalendarId == calendarId
                         where xx.CalendarItemId == eventId
                         where xx.IsRemovedFromCalendar == false
                         select new
                         {
                             xx.TicketUrl,
                             xx.IsInUTCTime,
                             xx.Calendar.TimeZone,
                             xx.Calendar.IsCalendarInUTC,
                             xx.AllowSelfCheckIn,
                             xx.CalendarItemId,
                             xx.Calendar.LeagueOwners,
                             xx.EndDate,
                             xx.Link,
                             xx.Name,
                             xx.Notes,
                             xx.StartDate,
                             xx.Attendees,
                             xx.Location,
                             xx.IsPublicEvent,
                             contactCart = xx.Location.Contact,
                             addresses = xx.Location.Contact.Addresses,
                             coms = xx.Location.Contact.Communications,
                             xx.PointsForEvent,
                             xx.EventType,
                             xx.ReocurringEvent,
                             nextEventId = dc.CalendarEvents.Where(x => x.Calendar.CalendarId == calendarId && x.IsRemovedFromCalendar == false && x.StartDate >= xx.StartDate && x.CalendarItemId != eventId).OrderBy(x => x.StartDate).Select(x => x.CalendarItemId).FirstOrDefault(),
                             previousEventId = dc.CalendarEvents.Where(x => x.Calendar.CalendarId == calendarId && x.IsRemovedFromCalendar == false && x.StartDate <= xx.StartDate && x.CalendarItemId != eventId).OrderByDescending(x => x.StartDate).Select(x => x.CalendarItemId).FirstOrDefault(),
                             nextStartDate = dc.CalendarEvents.Where(x => x.Calendar.CalendarId == calendarId && x.IsRemovedFromCalendar == false && x.StartDate >= xx.StartDate && x.CalendarItemId != eventId).OrderBy(x => x.StartDate).Select(x => x.StartDate).FirstOrDefault(),
                             previousStartDate = dc.CalendarEvents.Where(x => x.Calendar.CalendarId == calendarId && x.IsRemovedFromCalendar == false && x.StartDate <= xx.StartDate && x.CalendarItemId != eventId).OrderByDescending(x => x.StartDate).Select(x => x.StartDate).FirstOrDefault(),
                             xx.LastModified,
                             xx.Created,
                             xx.Color,
                             xx.Groups
                         }).FirstOrDefault();
                if (calendarId == new Guid() && e == null)
                {
                    e = (from xx in dc.CalendarEvents.Include("Location").Include("Location.Contact").Include("Location.Contact.Addresses").Include("Location.Contact.Communications")
                         where xx.CalendarItemId == eventId
                         where xx.IsRemovedFromCalendar == false
                         select new
                         {
                             xx.TicketUrl,
                             xx.IsInUTCTime,
                             xx.Calendar.TimeZone,
                             xx.Calendar.IsCalendarInUTC,
                             xx.AllowSelfCheckIn,
                             xx.CalendarItemId,
                             xx.Calendar.LeagueOwners,
                             xx.EndDate,
                             xx.Link,
                             xx.Name,
                             xx.Notes,
                             xx.StartDate,
                             xx.Attendees,
                             xx.Location,
                             xx.IsPublicEvent,
                             contactCart = xx.Location.Contact,
                             addresses = xx.Location.Contact.Addresses,
                             coms = xx.Location.Contact.Communications,
                             xx.PointsForEvent,
                             xx.EventType,
                             xx.ReocurringEvent,
                             nextEventId = dc.CalendarEvents.Where(x => x.Calendar.CalendarId == calendarId && x.IsRemovedFromCalendar == false && x.StartDate >= xx.StartDate && x.CalendarItemId != eventId).OrderBy(x => x.StartDate).Select(x => x.CalendarItemId).FirstOrDefault(),
                             previousEventId = dc.CalendarEvents.Where(x => x.Calendar.CalendarId == calendarId && x.IsRemovedFromCalendar == false && x.StartDate <= xx.StartDate && x.CalendarItemId != eventId).OrderByDescending(x => x.StartDate).Select(x => x.CalendarItemId).FirstOrDefault(),
                             nextStartDate = dc.CalendarEvents.Where(x => x.Calendar.CalendarId == calendarId && x.IsRemovedFromCalendar == false && x.StartDate >= xx.StartDate && x.CalendarItemId != eventId).OrderBy(x => x.StartDate).Select(x => x.StartDate).FirstOrDefault(),
                             previousStartDate = dc.CalendarEvents.Where(x => x.Calendar.CalendarId == calendarId && x.IsRemovedFromCalendar == false && x.StartDate <= xx.StartDate && x.CalendarItemId != eventId).OrderByDescending(x => x.StartDate).Select(x => x.StartDate).FirstOrDefault(),
                             xx.LastModified,
                             xx.Created,
                             xx.Color,
                             xx.Groups
                         }).FirstOrDefault();

                }

                if (e == null)
                {
                    return GetEventReocurring(calendarId, eventId);
                }
                //need to get the league first for the calendar.
                foreach (var owner in e.LeagueOwners)
                {
                    if (owner.League != null)
                    {
                        ev.OrganizersId = owner.League.LeagueId;
                        ev.OrganizersName = owner.League.Name;

                        if (owner.League.Logo != null)
                            ev.LogoUrl = owner.League.Logo.ImageUrl;
                        break;
                    }
                }
                //adding the members of the event to it.
                foreach (var g in e.Groups)
                {
                    var gTemp = League.Classes.LeagueGroupFactory.DisplayGroup(g.Group);
                    ev.GroupsForEvent.Add(gTemp);
                    foreach (var mem in gTemp.GroupMembers)
                    {
                        if (ev.MembersApartOfEvent.Where(x => x.MemberId == mem.MemberId).FirstOrDefault() == null)
                        {
                            RDN.Portable.Classes.Controls.Calendar.CalendarAttendance a = new RDN.Portable.Classes.Controls.Calendar.CalendarAttendance();
                            a.MemberId = mem.MemberId;
                            a.MemberName = mem.DerbyName;
                            a.IsCheckedIn = false;
                            a.FullName = mem.Firstname + " " + mem.LastName;
                            a.MemberNumber = mem.PlayerNumber;
                            ev.MembersApartOfEvent.Add(a);
                            ev.MembersToCheckIn.Add(a);
                        }
                    }
                }
                //if the event had no groups attached, we add the league members to the event.
                if (ev.MembersApartOfEvent.Count == 0)
                {
                    var members = League.LeagueFactory.GetLeagueMembers(ev.OrganizersId);
                    for (int i = 0; i < members.Count; i++)
                    {
                        RDN.Portable.Classes.Controls.Calendar.CalendarAttendance a = new Portable.Classes.Controls.Calendar.CalendarAttendance();
                        a.MemberId = members[i].MemberId;
                        a.MemberName = members[i].DerbyName;
                        a.IsCheckedIn = false;
                        a.FullName = members[i].Firstname + " " + members[i].LastName;
                        a.MemberNumber = members[i].PlayerNumber;
                        ev.MembersApartOfEvent.Add(a);
                        ev.MembersToCheckIn.Add(a);
                    }
                }
                //event type needs to stay above attendees since attendees relies on event type.
                ev.EventType = new Portable.Classes.Controls.Calendar.CalendarEventType();
                if (e.EventType != null)
                {
                    ev.EventType.EventTypeName = e.EventType.EventTypeName;
                    ev.EventType.PointsForExcused = e.EventType.PointsForExcused;
                    ev.EventType.PointsForNotPresent = e.EventType.PointsForNotPresent;
                    ev.EventType.PointsForPartial = e.EventType.PointsForPartial;
                    ev.EventType.PointsForPresent = e.EventType.PointsForPresent;
                    ev.EventType.PointsForTardy = e.EventType.PointsForTardy;
                    ev.EventType.CalendarEventTypeId = e.EventType.CalendarEventTypeId;
                }


                //add the attendees to the event.
                foreach (var att in e.Attendees)
                {
                    RDN.Portable.Classes.Controls.Calendar.CalendarAttendance a = new Portable.Classes.Controls.Calendar.CalendarAttendance()
                    {
                        AttedanceId = att.CalendarAttendanceId,
                        MemberId = att.Attendant.MemberId,
                        FullName = att.Attendant.Firstname + " " + att.Attendant.Lastname,
                        MemberName = att.Attendant.DerbyName,
                        Note = att.Note,
                        PointType = (CalendarEventPointTypeEnum)Enum.Parse(typeof(CalendarEventPointTypeEnum), att.PointTypeEnum.ToString()),
                        SecondaryPointType = (CalendarEventPointTypeEnum)Enum.Parse(typeof(CalendarEventPointTypeEnum), att.SecondaryPointTypeEnum.ToString()),
                        MemberNumber = att.Attendant.PlayerNumber,
                        AdditionalPoints = att.AdditionalPoints,
                        Availability = (AvailibilityEnum)Enum.Parse(typeof(AvailibilityEnum), att.AvailibityEnum.ToString()),
                        AvailableNotes = att.AvailabilityNote,
                    };

                    switch (a.PointType)
                    {
                        case CalendarEventPointTypeEnum.Present:
                            if (a.SecondaryPointType == CalendarEventPointTypeEnum.Tardy && a.AdditionalPoints == 0)
                                a.PointsStringForReading = ev.EventType.PointsForPresent + " + " + ev.EventType.PointsForTardy + " = " + (ev.EventType.PointsForPresent + ev.EventType.PointsForTardy);
                            else if (a.SecondaryPointType == CalendarEventPointTypeEnum.Tardy)
                                a.PointsStringForReading = ev.EventType.PointsForPresent + " + " + a.AdditionalPoints + " + " + ev.EventType.PointsForTardy + " = " + (ev.EventType.PointsForPresent + ev.EventType.PointsForTardy + a.AdditionalPoints);
                            else if (a.AdditionalPoints != 0)
                                a.PointsStringForReading = ev.EventType.PointsForPresent + " + " + a.AdditionalPoints + " = " + (ev.EventType.PointsForPresent + a.AdditionalPoints);
                            else
                                a.PointsStringForReading = ev.EventType.PointsForPresent.ToString();
                            break;
                        case CalendarEventPointTypeEnum.Partial:
                            if (a.SecondaryPointType == CalendarEventPointTypeEnum.Tardy && a.AdditionalPoints == 0)
                                a.PointsStringForReading = ev.EventType.PointsForPartial + " + " + ev.EventType.PointsForTardy + " = " + (ev.EventType.PointsForPartial + ev.EventType.PointsForTardy);
                            else if (a.SecondaryPointType == CalendarEventPointTypeEnum.Tardy)
                                a.PointsStringForReading = ev.EventType.PointsForPartial + " + " + a.AdditionalPoints + " + " + ev.EventType.PointsForTardy + " = " + (ev.EventType.PointsForPartial + ev.EventType.PointsForTardy + a.AdditionalPoints);
                            else if (a.AdditionalPoints != 0)
                                a.PointsStringForReading = ev.EventType.PointsForPartial + " + " + a.AdditionalPoints + " = " + (ev.EventType.PointsForPartial + a.AdditionalPoints);
                            else
                                a.PointsStringForReading = ev.EventType.PointsForPartial.ToString();
                            break;
                        case CalendarEventPointTypeEnum.Not_Present:
                            if (a.SecondaryPointType == CalendarEventPointTypeEnum.Tardy && a.AdditionalPoints == 0)
                                a.PointsStringForReading = ev.EventType.PointsForNotPresent + " + " + ev.EventType.PointsForTardy + " = " + (ev.EventType.PointsForNotPresent + ev.EventType.PointsForTardy);
                            else if (a.SecondaryPointType == CalendarEventPointTypeEnum.Tardy)
                                a.PointsStringForReading = ev.EventType.PointsForNotPresent + " + " + a.AdditionalPoints + " + " + ev.EventType.PointsForTardy + " = " + (ev.EventType.PointsForNotPresent + ev.EventType.PointsForTardy + a.AdditionalPoints);
                            else if (a.AdditionalPoints != 0)
                                a.PointsStringForReading = ev.EventType.PointsForNotPresent + " + " + a.AdditionalPoints + " = " + (ev.EventType.PointsForNotPresent + a.AdditionalPoints);
                            else
                                a.PointsStringForReading = ev.EventType.PointsForNotPresent.ToString();
                            break;
                        case CalendarEventPointTypeEnum.Excused:
                            if (a.SecondaryPointType == CalendarEventPointTypeEnum.Tardy && a.AdditionalPoints == 0)
                                a.PointsStringForReading = ev.EventType.PointsForExcused + " + " + ev.EventType.PointsForTardy + " = " + (ev.EventType.PointsForExcused + ev.EventType.PointsForTardy);
                            else if (a.SecondaryPointType == CalendarEventPointTypeEnum.Tardy)
                                a.PointsStringForReading = ev.EventType.PointsForExcused + " + " + a.AdditionalPoints + " + " + ev.EventType.PointsForTardy + " = " + (ev.EventType.PointsForExcused + ev.EventType.PointsForTardy + a.AdditionalPoints);
                            else if (a.AdditionalPoints != 0)
                                a.PointsStringForReading = ev.EventType.PointsForExcused + " + " + a.AdditionalPoints + " = " + (ev.EventType.PointsForExcused + a.AdditionalPoints);
                            else
                                a.PointsStringForReading = ev.EventType.PointsForExcused.ToString();
                            break;
                    }

                    if (a.PointType != CalendarEventPointTypeEnum.None)
                        a.IsCheckedIn = true;

                    //since the member is attending the event, we remove them from having members to check in.
                    var checkedInMember = ev.MembersToCheckIn.Where(x => x.MemberId == a.MemberId).FirstOrDefault();
                    if (checkedInMember != null)
                        ev.MembersToCheckIn.Remove(checkedInMember);

                    ev.Attendees.Add(a);
                }

                var avail = ev.Attendees.Where(x => x.MemberId == currentMemberId && x.Availability != AvailibilityEnum.None).FirstOrDefault();
                if (avail != null)
                    ev.HasCurrentMemberSetAvailability = true;
                var attend = ev.Attendees.Where(x => x.MemberId == currentMemberId && x.IsCheckedIn == true).FirstOrDefault();
                if (attend != null)
                    ev.IsCurrentMemberCheckedIn = true;
                if (e.Color != null)
                {
                    var c = Color.FromArgb(e.Color.ColorIdCSharp);
                    ev.ColorTempSelected = ColorTranslator.ToHtml(c);
                }
                ev.IsCalendarInUTC = e.IsCalendarInUTC;
                ev.IsInUTCTime = e.IsInUTCTime;
                ev.CalendarTimeZone = e.TimeZone;
                ev.AllowSelfCheckIn = e.AllowSelfCheckIn;
                ev.CalendarItemId = e.CalendarItemId;
                ev.CalendarId = calendarId;
                ev.SiteUrl = LibraryConfig.PublicSite + UrlManager.WEBSITE_EVENT_URL + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(e.Name) + "/" + e.CalendarItemId.ToString().Replace("-", "");
                if (!ev.IsInUTCTime)
                {
                    ev.EndDate = e.EndDate;
                    ev.StartDate = e.StartDate;
                }
                else
                {
                    ev.StartDate = (e.StartDate + new TimeSpan(e.TimeZone, 0, 0));
                    ev.EndDate = (e.EndDate + new TimeSpan(e.TimeZone, 0, 0));
                }
                if (!String.IsNullOrEmpty(e.Link) && !e.Link.Contains("http://"))
                    ev.Link = "http://" + e.Link;
                else
                    ev.Link = e.Link;
                ev.IsPublicEvent = e.IsPublicEvent;
                ev.TicketUrl = e.TicketUrl;
                if (e.ReocurringEvent != null)
                {
                    ev.CalendarReoccurringId = e.ReocurringEvent.CalendarItemId;
                    ev.IsReoccurring = true;
                }

                GoogleCalendar gc = new GoogleCalendar();
                if (e.Location != null)
                {
                    ev.Location.LocationName = e.Location.LocationName;
                    ev.Location.LocationId = e.Location.LocationId;
                    gc.Location = ev.Location.LocationName + ", ";
                }

                if (e.addresses.FirstOrDefault() != null)
                {
                    Address address = new Address();

                    address.Address1 = e.addresses.FirstOrDefault().Address1;
                    address.Address2 = e.addresses.FirstOrDefault().Address2;
                    address.AddressId = e.addresses.FirstOrDefault().AddressId;
                    address.CityRaw = e.addresses.FirstOrDefault().CityRaw;
                    if (e.addresses.FirstOrDefault().Country != null)
                        address.Country = e.addresses.FirstOrDefault().Country.Code;
                    address.StateRaw = e.addresses.FirstOrDefault().StateRaw;
                    address.Zip = e.addresses.FirstOrDefault().Zip;
                    gc.Location += address.Address1 + " " + address.Address2 + ", " + address.CityRaw + ", " + address.StateRaw + " " + address.Zip;
                    ev.Location.Contact.Addresses.Add(address);
                }
                ev.Name = e.Name;
                ev.NextEventId = e.nextEventId;
                ev.PreviousEventId = e.previousEventId;

                //if two or more events lay on the same StartDate and Time, we do this to Sort them
                //into previous and next so that we can walk through them one by one.
                //before this, if we hit next, we would skip over dates with the same datetime.
                if (ev.NextEventId != ev.PreviousEventId)
                {
                    var oneDateManyEvents = (from xx in dc.CalendarEvents
                                             where xx.Calendar.CalendarId == calendarId
                                             where xx.IsRemovedFromCalendar == false
                                             where xx.StartDate == e.previousStartDate
                                             select xx.CalendarItemId).OrderBy(x => x).ToList();
                    if (oneDateManyEvents.Count > 1)
                    {
                        var indexOfCurrentEvent = oneDateManyEvents.IndexOf(oneDateManyEvents.Where(x => x == ev.PreviousEventId).FirstOrDefault());
                        //if we get to this point, the event always gets the first event with the same times.  
                        //We we need to reverse it to make it get the last event in the sequence.
                        if (indexOfCurrentEvent == 0)
                        {
                            ev.NextEventId = dc.CalendarEvents.Where(x => x.Calendar.CalendarId == calendarId && x.IsRemovedFromCalendar == false && x.StartDate > e.StartDate).OrderBy(x => x.StartDate).Select(x => x.CalendarItemId).FirstOrDefault();
                            ev.PreviousEventId = oneDateManyEvents[oneDateManyEvents.Count - 1];
                        }
                    }
                }
                else
                {
                    try
                    {
                        var oneDateManyEvents = (from xx in dc.CalendarEvents
                                                 where xx.Calendar.CalendarId == calendarId
                                                 where xx.IsRemovedFromCalendar == false
                                                 where xx.StartDate == e.StartDate
                                                 select xx.CalendarItemId).OrderBy(x => x).ToList();
                        var indexOfCurrentEvent = oneDateManyEvents.IndexOf(oneDateManyEvents.Where(x => x == eventId).FirstOrDefault());
                        if (indexOfCurrentEvent == 0)
                        {
                            try
                            {
                                ev.PreviousEventId = dc.CalendarEvents.Where(x => x.Calendar.CalendarId == calendarId && x.IsRemovedFromCalendar == false && x.StartDate < e.StartDate && x.CalendarItemId != eventId).OrderByDescending(x => x.StartDate).Select(x => x.CalendarItemId).FirstOrDefault();
                                if (oneDateManyEvents.Count > indexOfCurrentEvent && oneDateManyEvents.Count > 1)
                                    ev.NextEventId = oneDateManyEvents[indexOfCurrentEvent + 1];
                            }
                            catch (Exception exception)
                            {
                                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: indexOfCurrentEvent + " " + oneDateManyEvents.Count);
                            }
                        }
                        else if (indexOfCurrentEvent >= oneDateManyEvents.Count - 1)
                        {
                            if (oneDateManyEvents.Count > 1)
                                ev.PreviousEventId = oneDateManyEvents[indexOfCurrentEvent - 1];
                            ev.NextEventId = dc.CalendarEvents.Where(x => x.Calendar.CalendarId == calendarId && x.IsRemovedFromCalendar == false && x.StartDate > e.StartDate && x.CalendarItemId != eventId).OrderBy(x => x.StartDate).Select(x => x.CalendarItemId).FirstOrDefault();
                        }
                        else
                        {
                            ev.PreviousEventId = oneDateManyEvents[indexOfCurrentEvent - 1];
                            ev.NextEventId = oneDateManyEvents[indexOfCurrentEvent + 1];
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }

                ev.Notes = e.Notes;

                if (e.LastModified > new DateTime(2013, 11, 23) || e.Created > new DateTime(2013, 11, 23))
                {
                    ev.NotesHtml = e.Notes;
                }
                else if (e.Created < new DateTime(2013, 11, 23))
                {
                    if (!String.IsNullOrEmpty(e.Notes))
                    {
                        RDN.Library.Util.MarkdownSharp.Markdown markdown = new RDN.Library.Util.MarkdownSharp.Markdown();
                        markdown.AutoHyperlink = true;
                        markdown.LinkEmails = true;
                        ev.NotesHtml = HtmlSanitize.FilterHtmlToWhitelist(markdown.Transform(e.Notes)).Replace("</p>", "</p><br/>");
                    }
                }

                gc.Description = ev.Notes;
                gc.EndDate = ev.EndDate.ToUniversalTime();
                gc.StartDate = ev.StartDate.ToUniversalTime();
                gc.Website = ev.Link;
                gc.WebsiteName = ev.Link;
                gc.What = ev.Name;


                ev.GoogleCalendarUrl = gc.GetGoogleUrl();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return ev;
        }
        private static void SendEmailAboutNewEvent(Guid calId, DataModels.Calendar.CalendarEvent calEvent, DataModels.Calendar.CalendarEventReoccuring calEventReoccur, string createdByMemberName, Guid toUserId, string toDerbyName, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (toUserId != new Guid())
                {
                    if (calEvent != null)
                    {
                        string points = String.Empty;
                        if (calEvent.EventType != null)
                            points = "<span><b>" + calEvent.EventType.PointsForPresent + "</b> Pre, <b>" + calEvent.EventType.PointsForPartial + "</b>Par, <b>" + calEvent.EventType.PointsForNotPresent + "</b> Not, <b>" + calEvent.EventType.PointsForExcused + "</b> Exc, <b>" + calEvent.EventType.PointsForTardy + "</b> Tar</span>";
                        else
                            points = "<span></span>";
                        string location = String.Empty;
                        string address = string.Empty;
                        if (calEvent.Location.Contact.Addresses.FirstOrDefault() != null)
                        {
                            address = "<a href='http://www.bing.com/maps/default.aspx?q=" + calEvent.Location.Contact.Addresses.FirstOrDefault().Address1 + calEvent.Location.Contact.Addresses.FirstOrDefault().Address2 + "," + calEvent.Location.Contact.Addresses.FirstOrDefault().CityRaw + "," + calEvent.Location.Contact.Addresses.FirstOrDefault().StateRaw + calEvent.Location.Contact.Addresses.FirstOrDefault().Zip + calEvent.Location.Contact.Addresses.FirstOrDefault().Country + "' target='_blank'>View Map</a>";
                        }
                        if (calEvent.Location != null)
                            location = calEvent.Location.LocationName;

                        var emailData = new Dictionary<string, string>();
                        emailData.Add("derbyname", toDerbyName);
                        emailData.Add("FromUserName", createdByMemberName);
                        emailData.Add("eventName", calEvent.Name);
                        if (calEvent.EventType != null)
                            emailData.Add("eventType", calEvent.EventType.EventTypeName);
                        else
                            emailData.Add("eventType", "None");
                        emailData.Add("points", points);
                        emailData.Add("location", location);
                        emailData.Add("locationMap", address);
                        emailData.Add("eventNotes", calEvent.Notes);
                        emailData.Add("viewEventLink", LibraryConfig.InternalSite + "/calendar/event/league/" + calId.ToString().Replace("-", "") + "/" + calEvent.CalendarItemId.ToString().Replace("-", ""));


                        emailData.Add("eventDateTime", startDate.ToString("ddd") + ", " + startDate.ToShortDateString() + " " + startDate.ToShortTimeString() + " - " + endDate.ToShortTimeString());
                        var user = System.Web.Security.Membership.GetUser((object)toUserId);
                        if (user != null)
                        {
                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, user.UserName, LibraryConfig.DefaultEmailSubject + " New Event Created", emailData, EmailServer.EmailServerLayoutsEnum.CalendarSendNewEvent);
                        }
                    }
                    else if (calEventReoccur != null)
                    {
                        try
                        {
                            string points = "<span><b>" + calEventReoccur.EventType.PointsForPresent + "</b> Pre, <b>" + calEventReoccur.EventType.PointsForPartial + "</b>Par, <b>" + calEventReoccur.EventType.PointsForNotPresent + "</b> Not, <b>" + calEventReoccur.EventType.PointsForExcused + "</b> Exc, <b>" + calEventReoccur.EventType.PointsForTardy + "</b> Tar</span>";
                            string location = String.Empty;
                            string type = string.Empty;
                            string address = string.Empty;
                            if (calEventReoccur.Location.Contact.Addresses.FirstOrDefault() != null)
                            {
                                address = "<a href='http://www.bing.com/maps/default.aspx?q=" + calEventReoccur.Location.Contact.Addresses.FirstOrDefault().Address1 + calEventReoccur.Location.Contact.Addresses.FirstOrDefault().Address2 + "," + calEventReoccur.Location.Contact.Addresses.FirstOrDefault().CityRaw + "," + calEventReoccur.Location.Contact.Addresses.FirstOrDefault().StateRaw + calEventReoccur.Location.Contact.Addresses.FirstOrDefault().Zip + calEventReoccur.Location.Contact.Addresses.FirstOrDefault().Country + "' target='_blank'>View Map</a>";
                            }
                            if (calEventReoccur.Location != null)
                                location = calEventReoccur.Location.LocationName;
                            if (calEventReoccur.EventType != null)
                                type = calEventReoccur.EventType.EventTypeName;
                            var emailData = new Dictionary<string, string>();
                            emailData.Add("derbyname", toDerbyName);
                            emailData.Add("FromUserName", createdByMemberName);
                            emailData.Add("eventName", calEventReoccur.Name);
                            emailData.Add("eventType", type);
                            emailData.Add("points", points);
                            emailData.Add("location", location);
                            emailData.Add("locationMap", address);
                            emailData.Add("eventNotes", calEventReoccur.Notes);

                            emailData.Add("viewEventLink", LibraryConfig.InternalSite + "/calendar/event/league/" + calId.ToString().Replace("-", "") + "/" + calEventReoccur.CalendarItemId.ToString().Replace("-", ""));

                            emailData.Add("eventDateTime", startDate.ToString("ddd") + ", " + startDate.ToShortDateString() + " " + startDate.ToShortTimeString() + " - " + endDate.ToShortTimeString());

                            var user = System.Web.Security.Membership.GetUser((object)toUserId);
                            if (user != null)
                            {
                                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, user.UserName, LibraryConfig.DefaultEmailSubject + " New Event Created", emailData, EmailServer.EmailServerLayoutsEnum.CalendarSendNewEvent);
                            }
                        }
                        catch (Exception exception)
                        {
                            ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: calId + ":" + calEventReoccur.CalendarItemId);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        public static Guid CreateNewEventFromFeedUrl(Guid calId, string UID, DateTime startDate, DateTime endDate, string location, string eventName, string link, string notes, bool AllowSelfCheckIn)
        {
            DataModels.Calendar.CalendarEvent ev = new DataModels.Calendar.CalendarEvent();
            try
            {
                var dc = new ManagementContext();
                ev.Calendar = dc.Calendar.Where(x => x.CalendarId == calId).FirstOrDefault();
                ev.IsInUTCTime = true;
                ev.EndDate = endDate;
                ev.StartDate = startDate;
                ev.AllowSelfCheckIn = AllowSelfCheckIn;
                ev.EventFeedId = UID;
                ev.Name = eventName;
                ev.Notes = location + " " + notes;
                ev.Link = link;
                dc.CalendarEvents.Add(ev);
                int c = dc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: endDate + ":" + startDate);
            }
            return ev.CalendarItemId;
        }

        public static Guid CreateNewEvent(Guid calId, DateTime startDate, DateTime endDate, Guid locationId, string eventName, string link, string notes, bool AllowSelfCheckIn, bool isPublicEvent, bool isReocurring, Guid reocurringEventId, long selectedEventTypeId, bool broadcastEvent, string ticketUrl, string hexColor, List<long> groupIds, Guid memId)
        {
            DataModels.Calendar.CalendarEvent ev = new DataModels.Calendar.CalendarEvent();
            try
            {
                var dc = new ManagementContext();
                ev.Calendar = dc.Calendar.Where(x => x.CalendarId == calId).FirstOrDefault();
                if (ev.Calendar.IsCalendarInUTC)
                {
                    DateTimeOffset dtOffEnd = new DateTimeOffset(endDate.Ticks, new TimeSpan(ev.Calendar.TimeZone, 0, 0));
                    DateTimeOffset dtOffStart = new DateTimeOffset(startDate.Ticks, new TimeSpan(ev.Calendar.TimeZone, 0, 0));

                    ev.EndDate = dtOffEnd.UtcDateTime;
                    ev.StartDate = dtOffStart.UtcDateTime;
                    ev.IsInUTCTime = true;
                }
                else
                {
                    ev.EndDate = endDate;
                    ev.StartDate = startDate;
                }
                ev.AllowSelfCheckIn = AllowSelfCheckIn;
                ev.Location = dc.Locations.Include("Contact").Include("Contact.Addresses").Include("Contact.Communications").Where(x => x.LocationId == locationId).FirstOrDefault();
                ev.Name = eventName;
                ev.Notes = notes;
                ev.Link = link;
                ev.IsPublicEvent = isPublicEvent;
                ev.TicketUrl = ticketUrl;
                if (ev.EndDate < DateTime.UtcNow.AddYears(-1))
                    ev.EndDate = ev.StartDate;


                if (!String.IsNullOrEmpty(hexColor))
                {
                    Color cc = ColorTranslator.FromHtml(hexColor);
                    int arb = cc.ToArgb();
                    ev.Color = dc.Colors.Where(x => x.ColorIdCSharp == arb).FirstOrDefault();
                }
                else
                    ev.Color = null;
                ev.EventType = dc.CalendarEventTypes.Where(x => x.CalendarEventTypeId == selectedEventTypeId).FirstOrDefault();

                //we create a event.  Gotta check if its already in the db.
                //we had an instance where a league deleted an old reoccuring event
                //and they used the same name.  So we had to check if the event had the same reoccuring event
                //id.  
                DataModels.Calendar.CalendarEvent checkEventExists = null;
                if (isReocurring)
                {
                    ev.ReocurringEvent = dc.CalendarEventsReocurring.Where(x => x.CalendarItemId == reocurringEventId).FirstOrDefault();
                    //taking the groups from the reoccurence and adding them to the list of groups.
                    foreach (var g in ev.ReocurringEvent.Groups)
                        groupIds.Add(g.Group.Id);

                    checkEventExists = ev.Calendar.CalendarEvents.Where(x => x.EndDate == endDate && x.StartDate == startDate && x.Name == eventName && x.ReocurringEvent != null && x.ReocurringEvent.CalendarItemId == reocurringEventId).FirstOrDefault();
                }

                //keep under isrecocuring question.
                foreach (var id in groupIds)
                {
                    var group = dc.LeagueGroups.Where(x => x.Id == id).FirstOrDefault();
                    if (group != null)
                    {
                        if (ev.Groups.Where(x => x.Group.Id == id).FirstOrDefault() == null)
                        {
                            RDN.Library.DataModels.Calendar.CalendarEventGroup newGroup = new RDN.Library.DataModels.Calendar.CalendarEventGroup();
                            newGroup.Group = group;
                            newGroup.Event = ev;
                            ev.Groups.Add(newGroup);
                        }
                    }
                }


                if (checkEventExists == null)
                    dc.CalendarEvents.Add(ev);
                else
                    ev.CalendarItemId = checkEventExists.CalendarItemId;
                int c = dc.SaveChanges();

                if (broadcastEvent)
                {
                    List<Guid> memIds = new List<Guid>();
                    var memberCreated = MemberCache.GetMemberDisplay(memId);
                    if (ev.Groups.Count == 0)
                    {
                        //sends broadcast to all league members
                        var members = MemberCache.GetCurrentLeagueMembers(memId);
                        foreach (var mem in members)
                        {
                            SendEmailAboutNewEvent(calId, ev, null, memberCreated.DerbyName, mem.UserId, mem.DerbyName, startDate, endDate);
                            memIds.Add(mem.MemberId);
                        }
                    }
                    else
                    {
                        //gets all the members of the groups selected and sends an email broadcast to those members.
                        List<MemberDisplay> memsToSend = new List<MemberDisplay>();
                        var groups = MemberCache.GetLeagueGroupsOfMember(memId);
                        foreach (var group in ev.Groups)
                        {
                            var g = groups.Where(x => x.Id == group.Group.Id).FirstOrDefault();
                            if (g != null)
                            {
                                foreach (var temp in g.GroupMembers)
                                {
                                    MemberDisplay mtemp = new MemberDisplay();
                                    mtemp.DerbyName = temp.DerbyName;
                                    mtemp.UserId = temp.UserId;
                                    if (memsToSend.Where(x => x.UserId == mtemp.UserId).FirstOrDefault() == null)
                                        memsToSend.Add(mtemp);
                                }
                            }
                        }
                        var members = MemberCache.GetCurrentLeagueMembers(memId);
                        foreach (var mem in memsToSend)
                        {
                            SendEmailAboutNewEvent(calId, ev, null, memberCreated.DerbyName, mem.UserId, mem.DerbyName, startDate, endDate);
                            memIds.Add(mem.MemberId);
                        }

                    }
                    var fact = new MobileNotificationFactory()
                         .Initialize("New Event Created:", ev.Name, Mobile.Enums.NotificationTypeEnum.NewCalendarEventBroadcast)
                         .AddCalendarEvent(ev.CalendarItemId, calId, ev.Name)
                         .AddMembers(memIds)
                         .SendNotifications();
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: endDate + ":" + startDate);
            }
            return ev.CalendarItemId;
        }

        public static Guid CreateNewEventReOcurring(Guid calId, DateTime startDate, DateTime endDate, Guid locationId, string eventName, string link, string notes, bool AllowSelfCheckIn, FrequencyTypeEnum repeatFrequencySelected, bool sunday, bool monday, bool tuesday, bool wednesday, bool thursday, bool friday, bool saturday, EndsWhenReoccuringEnum endsWhen, int endsOnOcurrences, DateTime endsOnDateReoccuring, long selectedEventTypeId, bool broadcastEvent, bool isEventPublic, int monthlyIntervalId, string ticketUrl, string hexColor, List<long> groupIds, Guid memberId)
        {
            DataModels.Calendar.CalendarEventReoccuring ev = new DataModels.Calendar.CalendarEventReoccuring();

            try
            {

                //need to know how many days so we can know when to end the reoccuring event in the calendar
                int howManyDays = 0;
                int daysOfWeek = 0;
                if (sunday)
                {
                    howManyDays += 1;
                    daysOfWeek += (int)DayOfWeekEnum.Sun;
                }
                if (monday)
                {
                    howManyDays += 1;
                    daysOfWeek += (int)DayOfWeekEnum.Mon;
                }
                if (tuesday)
                {
                    howManyDays += 1;
                    daysOfWeek += (int)DayOfWeekEnum.Tue;
                }
                if (wednesday)
                {
                    howManyDays += 1;
                    daysOfWeek += (int)DayOfWeekEnum.Wed;
                }
                if (thursday)
                {
                    howManyDays += 1;
                    daysOfWeek += (int)DayOfWeekEnum.Thu;
                }
                if (friday)
                {
                    howManyDays += 1;
                    daysOfWeek += (int)DayOfWeekEnum.Fri;
                }
                if (saturday)
                {
                    howManyDays += 1;
                    daysOfWeek += (int)DayOfWeekEnum.Sat;
                }
                ScheduleWidget.ScheduledEvents.Event aEvent = null;
                if (repeatFrequencySelected == FrequencyTypeEnum.Monthly)
                {
                    aEvent = new ScheduleWidget.ScheduledEvents.Event()
                   {
                       Title = eventName,
                       FrequencyTypeOptions = repeatFrequencySelected,
                       DaysOfWeek = daysOfWeek,
                       MonthlyInterval = monthlyIntervalId
                   };
                }
                else
                {
                    aEvent = new ScheduleWidget.ScheduledEvents.Event()
                   {
                       Title = eventName,
                       FrequencyTypeOptions = repeatFrequencySelected,
                       DaysOfWeek = daysOfWeek
                   };
                }

                var dc = new ManagementContext();
                if (endsWhen == EndsWhenReoccuringEnum.Never)
                {
                    ev.EndReocurring = DateTime.UtcNow.AddYears(2);
                }
                else if (endsWhen == EndsWhenReoccuringEnum.After)
                {
                    if (aEvent.FrequencyTypeOptions == FrequencyTypeEnum.Daily)
                        ev.EndReocurring = startDate.AddDays(endsOnOcurrences);
                    else if (aEvent.FrequencyTypeOptions == FrequencyTypeEnum.Monthly)
                        ev.EndReocurring = startDate.AddMonths(endsOnOcurrences);
                    else if (aEvent.FrequencyTypeOptions == FrequencyTypeEnum.Weekly)
                    {
                        int daysToAdd = (endsOnOcurrences / howManyDays) * 7;
                        ev.EndReocurring = startDate.AddDays(daysToAdd);
                    }
                }
                else if (endsWhen == EndsWhenReoccuringEnum.On)
                {
                    ev.EndReocurring = endsOnDateReoccuring;
                }
                if (!String.IsNullOrEmpty(hexColor))
                {
                    Color c = ColorTranslator.FromHtml(hexColor);
                    int arb = c.ToArgb();
                    ev.Color = dc.Colors.Where(x => x.ColorIdCSharp == arb).FirstOrDefault();
                }
                else
                    ev.Color = null;

                ev.DaysOfWeekReocurring = aEvent.DaysOfWeek;
                ev.FrequencyReocurring = aEvent.Frequency;
                ev.MonthlyIntervalReocurring = aEvent.MonthlyInterval;
                ev.StartReocurring = startDate;
                ev.Calendar = dc.Calendar.Where(x => x.CalendarId == calId).FirstOrDefault();
                if (ev.Calendar.IsCalendarInUTC)
                {
                    DateTimeOffset dtOffEnd = new DateTimeOffset(endDate.Ticks, new TimeSpan(ev.Calendar.TimeZone, 0, 0));
                    DateTimeOffset dtOffStart = new DateTimeOffset(startDate.Ticks, new TimeSpan(ev.Calendar.TimeZone, 0, 0));

                    ev.EndDate = dtOffEnd.UtcDateTime;
                    ev.StartDate = dtOffStart.UtcDateTime;
                    ev.IsInUTCTime = true;
                }
                else
                {
                    ev.EndDate = endDate;
                    ev.StartDate = startDate;
                }
                //keep under isrecocuring question.
                foreach (var id in groupIds)
                {
                    var group = dc.LeagueGroups.Where(x => x.Id == id).FirstOrDefault();
                    if (group != null)
                    {
                        RDN.Library.DataModels.Calendar.CalendarEventReoccuringGroup newGroup = new RDN.Library.DataModels.Calendar.CalendarEventReoccuringGroup();
                        newGroup.Group = group;
                        newGroup.Event = ev;
                        if (ev.Groups.Where(x => x.Group.Id == id).FirstOrDefault() == null)
                            ev.Groups.Add(newGroup);
                    }
                }

                ev.AllowSelfCheckIn = AllowSelfCheckIn;
                ev.Location = dc.Locations.Include("Contact").Include("Contact.Addresses").Include("Contact.Communications").Where(x => x.LocationId == locationId).FirstOrDefault();
                ev.Name = eventName;
                ev.Notes = notes;
                ev.TicketUrl = ticketUrl;
                ev.Link = link;
                ev.IsPublic = isEventPublic;
                ev.LastDateEventsWereCreated = DateTime.UtcNow.AddMonths(-3);
                ev.EventType = dc.CalendarEventTypes.Where(x => x.CalendarEventTypeId == selectedEventTypeId).FirstOrDefault();
                dc.CalendarEventsReocurring.Add(ev);
                ev.Calendar.CalendarEventsReocurring.Add(ev);
                int cc = dc.SaveChanges();

                var schedule = new Schedule(aEvent);
                var range = new DateRange()
                {
                    StartDateTime = ev.StartReocurring
                };
                //date is null if the event is never ending.
                if (ev.EndReocurring.HasValue)
                    range.EndDateTime = ev.EndReocurring.Value;
                else
                    range.EndDateTime = endDate.AddMonths(1);

                foreach (var date in schedule.Occurrences(range))
                {


                    DateTime endDateEv = new DateTime();
                    DateTime startDateEv = new DateTime();

                    if (!ev.IsInUTCTime)
                    {
                        startDateEv = new DateTime(date.Year, date.Month, date.Day, ev.StartDate.Hour, ev.StartDate.Minute, ev.StartDate.Second);
                        endDateEv = new DateTime(date.Year, date.Month, date.Day, ev.EndDate.Hour, ev.EndDate.Minute, ev.EndDate.Second);
                    }
                    else
                    {
                        //we have to create a temp dates so we can add the timezone information without going back a day
                        //if the time being used is on the border.
                        //without the tempdates 1/4/2013 7pm turned into 1/3/2013 7pm because the timezones didn't account for the 
                        //fact the dates were already in utc.
                        var startTempDate = new DateTime(date.Year, date.Month, date.Day, ev.StartDate.Hour, ev.StartDate.Minute, ev.StartDate.Second) + new TimeSpan(ev.Calendar.TimeZone, 0, 0);
                        var endTempDate = new DateTime(date.Year, date.Month, date.Day, ev.EndDate.Hour, ev.EndDate.Minute, ev.EndDate.Second) + new TimeSpan(ev.Calendar.TimeZone, 0, 0);
                        startDateEv = new DateTime(date.Year, date.Month, date.Day, startTempDate.Hour, startTempDate.Minute, startTempDate.Second);
                        endDateEv = new DateTime(date.Year, date.Month, date.Day, endTempDate.Hour, endTempDate.Minute, endTempDate.Second);
                    }


                    Guid calItemId = CalendarEventFactory.CreateNewEvent(ev.Calendar.CalendarId, startDateEv, endDateEv, locationId, ev.Name, ev.Link, ev.Notes, ev.AllowSelfCheckIn, ev.IsPublic, true, ev.CalendarItemId, ev.EventType.CalendarEventTypeId, false, ev.TicketUrl, hexColor, new List<long>(), memberId);

                }

                if (broadcastEvent)
                {
                    List<Guid> memIds = new List<Guid>();
                    var memberCreated = MemberCache.GetMemberDisplay(memberId);
                    if (ev.Groups.Count == 0)
                    {
                        //sends broadcast to all league members
                        var members = MemberCache.GetCurrentLeagueMembers(memberId);
                        foreach (var mem in members)
                        {
                            SendEmailAboutNewEvent(calId, null, ev, memberCreated.DerbyName, mem.UserId, mem.DerbyName, startDate, endDate);
                            memIds.Add(mem.MemberId);
                        }
                    }
                    else
                    {
                        //gets all the members of the groups selected and sends an email broadcast to those members.
                        List<MemberDisplay> memsToSend = new List<MemberDisplay>();
                        var groups = MemberCache.GetLeagueGroupsOfMember(memberId);
                        foreach (var group in ev.Groups)
                        {
                            var g = groups.Where(x => x.Id == group.Group.Id).FirstOrDefault();
                            if (g != null)
                            {
                                foreach (var temp in g.GroupMembers)
                                {
                                    MemberDisplay mtemp = new MemberDisplay();
                                    mtemp.DerbyName = temp.DerbyName;
                                    mtemp.UserId = temp.UserId;
                                    if (memsToSend.Where(x => x.UserId == mtemp.UserId).FirstOrDefault() == null)
                                        memsToSend.Add(mtemp);
                                }
                            }
                        }
                        var members = MemberCache.GetCurrentLeagueMembers(memberId);
                        foreach (var mem in memsToSend)
                        {
                            SendEmailAboutNewEvent(calId, null, ev, memberCreated.DerbyName, mem.UserId, mem.DerbyName, startDate, endDate);
                            memIds.Add(mem.MemberId);
                        }
                    }
                    var fact = new MobileNotificationFactory()
                       .Initialize("New Event Created:", ev.Name, Mobile.Enums.NotificationTypeEnum.NewCalendarEventBroadcast)
                       .AddCalendarEvent(ev.CalendarItemId, calId, ev.Name)
                       .AddMembers(memIds)
                       .SendNotifications();
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return ev.CalendarItemId;
        }


        private static DataModels.Calendar.CalendarEventPoint CreateNewPointType(int points, CalendarPointTypeEnum pointType, ManagementContext dc)
        {
            DataModels.Calendar.CalendarEventPoint point = new DataModels.Calendar.CalendarEventPoint();
            try
            {
                point.PointsForEvent = points;
                point.PointTypeEnum = Convert.ToInt32(pointType);
                dc.CalendarEventPoints.Add(point);
                dc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return point;
        }
        public static List<Conversation> GetConversation(Guid eventId)
        {
            try
            {
                var dc = new ManagementContext();
                var convo = (from xx in dc.CalendarEventsConversation
                             where xx.CalEvent.CalendarItemId == eventId
                             select new Conversation
                             {
                                 Chat = xx.Text,
                                 Id = xx.ConversationId,
                                 MemberName = xx.Owner == null ? "Anonymous" : xx.Owner.DerbyName,
                                 MemberProfilePicUrl = xx.Owner.Photos.OrderByDescending(x=>x.Created).FirstOrDefault() == null? "": xx.Owner.Photos.OrderByDescending(x=>x.Created).FirstOrDefault().ImageUrlThumb, 
                                 Created = xx.Created,
                                 OwnerId = eventId,
                                 
                             }).OrderByDescending(x => x.Id).Take(60).AsParallel().ToList();
                foreach (var con in convo)
                {
                    con.Time = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(con.Created);
                }
                return convo;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<Classes.Communications.Conversation>();
        }

        public static Conversation PostConversationText(Guid eventId, string text, Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var ev = dc.CalendarEvents.Where(x => x.CalendarItemId == eventId).FirstOrDefault();
                EventCalendarConversation sation = new EventCalendarConversation();
                sation.CalEvent = ev;
                sation.Text = text;
                if (memberId != new Guid())
                    sation.Owner = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                dc.CalendarEventsConversation.Add(sation);
                dc.SaveChanges();
                Conversation s = new Conversation();
                s.Chat = text;
                s.Created = sation.Created;
                s.OwnerId = eventId;
                s.Id = sation.ConversationId;
                if (sation.Owner != null)
                    s.MemberName = sation.Owner.DerbyName;
                else
                    s.MemberName = "Anonymous";
                return s;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public static CalendarEvent DisplayEvent(DataModels.Calendar.CalendarEvent ev, Guid currentMemberId, bool isAttendanceManagerOrBetter)
        {
            CalendarEvent calEvent = new CalendarEvent();
            try
            {
                calEvent.IsAttendanceManagerOrBetter = isAttendanceManagerOrBetter;
                foreach (var att in ev.Attendees)
                {
                    RDN.Portable.Classes.Controls.Calendar.CalendarAttendance a = new Portable.Classes.Controls.Calendar.CalendarAttendance();
                    a.MemberId = att.Attendant.MemberId;
                    a.MemberName = att.Attendant.DerbyName;
                    a.FullName = att.Attendant.Firstname + " " + att.Attendant.Lastname;
                    a.AttedanceId = att.CalendarAttendanceId;
                    a.Note = att.Note;
                    a.PointType = (CalendarEventPointTypeEnum)Enum.Parse(typeof(CalendarEventPointTypeEnum), att.PointTypeEnum.ToString());
                    a.SecondaryPointType = (CalendarEventPointTypeEnum)Enum.Parse(typeof(CalendarEventPointTypeEnum), att.SecondaryPointTypeEnum.ToString());
                    a.Availability = (AvailibilityEnum)Enum.Parse(typeof(AvailibilityEnum), att.AvailibityEnum.ToString());
                    a.AvailableNotes = att.AvailabilityNote;
                    if (a.PointType != CalendarEventPointTypeEnum.None)
                        a.IsCheckedIn = true;
                    calEvent.Attendees.Add(a);
                }

                if (ev.Groups.Count == 0)
                    calEvent.IsCurrentMemberApartOfEvent = true;
                else
                {
                    foreach (var g in ev.Groups)
                    {
                        var gTemp = League.Classes.LeagueGroupFactory.DisplayGroup(g.Group);
                        foreach (var mem in gTemp.GroupMembers)
                        {
                            RDN.Portable.Classes.Controls.Calendar.CalendarAttendance a = new RDN.Portable.Classes.Controls.Calendar.CalendarAttendance();
                            a.MemberId = mem.MemberId;
                            a.MemberName = mem.DerbyName;
                            if (calEvent.MembersApartOfEvent.Where(x => x.MemberId == a.MemberId).FirstOrDefault() == null)
                                calEvent.MembersApartOfEvent.Add(a);
                        }
                    }
                }

                if (ev.EventType != null)
                {
                    calEvent.EventType.CalendarEventTypeId = ev.EventType.CalendarEventTypeId;
                    calEvent.EventType.PointsForExcused = ev.EventType.PointsForExcused;
                    calEvent.EventType.PointsForNotPresent = ev.EventType.PointsForNotPresent;
                    calEvent.EventType.PointsForPartial = ev.EventType.PointsForPartial;
                    calEvent.EventType.PointsForPresent = ev.EventType.PointsForPresent;
                    calEvent.EventType.PointsForTardy = ev.EventType.PointsForTardy;
                }
                foreach (var owner in ev.Calendar.LeagueOwners)
                {
                    if (owner.League != null)
                    {
                        calEvent.OrganizersId = owner.League.LeagueId;
                        calEvent.OrganizersName = owner.League.Name;
                        if (owner.League.Logo != null)
                            calEvent.ImageUrl = owner.League.Logo.ImageUrlThumb;
                        calEvent.OrganizerUrl = RDN.Library.Classes.Config.LibraryConfig.LeagueUrl + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(calEvent.OrganizersName) + "/" + calEvent.OrganizersId.ToString().Replace("-", "");
                        break;
                    }
                }
                GoogleCalendar gc = new GoogleCalendar();
                if (ev.Location != null)
                {
                    calEvent.Location.LocationName = ev.Location.LocationName;
                    gc.Location = calEvent.Location.LocationName + " ";
                }
                if (ev.Location != null)
                {
                    calEvent.Location.LocationName = ev.Location.LocationName;
                    calEvent.Location.LocationId = ev.Location.LocationId;

                    if (ev.Location.Contact != null && ev.Location.Contact.Addresses.FirstOrDefault() != null)
                    {
                        Address address = new Address();
                        address.Address1 = ev.Location.Contact.Addresses.FirstOrDefault().Address1;
                        address.Address2 = ev.Location.Contact.Addresses.FirstOrDefault().Address2;
                        address.AddressId = ev.Location.Contact.Addresses.FirstOrDefault().AddressId;
                        address.CityRaw = ev.Location.Contact.Addresses.FirstOrDefault().CityRaw;
                        if (ev.Location.Contact.Addresses.FirstOrDefault().Country != null)
                            address.Country = ev.Location.Contact.Addresses.FirstOrDefault().Country.Code;
                        address.StateRaw = ev.Location.Contact.Addresses.FirstOrDefault().StateRaw;
                        address.Zip = ev.Location.Contact.Addresses.FirstOrDefault().Zip;
                        gc.Location += address.Address1 + " " + address.Address2 + ", " + address.CityRaw + ", " + address.StateRaw + " " + address.Zip;
                        address.Coords = new Portable.Classes.Location.GeoCoordinate();
                        address.Coords.Longitude = ev.Location.Contact.Addresses.FirstOrDefault().Coords.Longitude;
                        address.Coords.Latitude = ev.Location.Contact.Addresses.FirstOrDefault().Coords.Latitude;

                        if (!String.IsNullOrEmpty(address.CityRaw))
                            calEvent.Address += address.CityRaw + ", ";
                        if (!String.IsNullOrEmpty(address.StateRaw))
                            calEvent.Address += address.StateRaw + ",";
                        calEvent.Address += address.Country;
                        calEvent.AddressUrl += "http://www.bing.com/maps/default.aspx?q=";
                        calEvent.AddressUrl += address.Address1 + address.Address2;

                        calEvent.AddressUrl += " ," + address.CityRaw;
                        calEvent.AddressUrl += "," + address.StateRaw;
                        calEvent.AddressUrl += " " + address.Zip;
                        calEvent.AddressUrl += address.Country;

                        calEvent.Location.Contact.Addresses.Add(address);
                    }
                }
                if (ev.ReocurringEvent != null)
                    calEvent.CalendarReoccurringId = ev.ReocurringEvent.CalendarItemId;
                calEvent.Name = ev.Name;
                calEvent.NameUrl =RDN.Library.Classes.Config.LibraryConfig.EventPublicUrl + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(ev.Name) + "/" + ev.CalendarItemId.ToString().Replace("-", "");
                calEvent.TicketUrl = ev.TicketUrl;
                calEvent.CalendarItemId = ev.CalendarItemId;

                var avail = calEvent.Attendees.Where(x => x.MemberId == currentMemberId && x.Availability != AvailibilityEnum.None).FirstOrDefault();
                if (avail != null)
                    calEvent.HasCurrentMemberSetAvailability = true;
                var attend = calEvent.Attendees.Where(x => x.MemberId == currentMemberId && x.IsCheckedIn == true).FirstOrDefault();
                if (attend != null)
                    calEvent.IsCurrentMemberCheckedIn = true;

                if (ev.Color != null)
                {
                    var c = Color.FromArgb(ev.Color.ColorIdCSharp);
                    calEvent.ColorTempSelected = ColorTranslator.ToHtml(c);
                }

                if (!ev.IsInUTCTime)
                {
                    calEvent.StartDate = ev.StartDate;
                    calEvent.EndDate = ev.EndDate;
                }
                else
                {
                    calEvent.StartDate = (ev.StartDate + new TimeSpan(ev.Calendar.TimeZone, 0, 0));
                    calEvent.EndDate = (ev.EndDate + new TimeSpan(ev.Calendar.TimeZone, 0, 0));
                }

                calEvent.StartDateDisplay = calEvent.StartDate.ToShortDateString();
                calEvent.EndDateDisplay = calEvent.EndDate.ToShortDateString();
                calEvent.Link = ev.Link;
                if (!String.IsNullOrEmpty(ev.Link) && !ev.Link.Contains("http://") && !ev.Link.Contains("https://"))
                    calEvent.Link = "http://" + ev.Link;
                else
                    calEvent.Link = ev.Link;
                calEvent.Notes = ev.Notes;

                if (ev.LastModified > new DateTime(2013, 11, 23) || ev.Created > new DateTime(2013, 11, 23))
                {
                    calEvent.NotesHtml = ev.Notes;
                }
                else if (ev.Created < new DateTime(2013, 11, 23))
                {
                    if (!String.IsNullOrEmpty(ev.Notes))
                    {
                        RDN.Library.Util.MarkdownSharp.Markdown markdown = new RDN.Library.Util.MarkdownSharp.Markdown();
                        markdown.AutoHyperlink = true;
                        markdown.LinkEmails = true;
                        calEvent.NotesHtml = HtmlSanitize.FilterHtmlToWhitelist(markdown.Transform(ev.Notes)).Replace("</p>", "</p><br/>");
                    }
                }

                //gc.Description = calEvent.Notes;
                gc.EndDate = calEvent.EndDate.ToUniversalTime();
                gc.StartDate = calEvent.StartDate.ToUniversalTime();
                gc.Website = calEvent.Link;
                gc.WebsiteName = calEvent.Link;
                gc.What = calEvent.Name;

                calEvent.AllowSelfCheckIn = ev.AllowSelfCheckIn;
                var isApartOfEvent = calEvent.MembersApartOfEvent.Where(x => x.MemberId == currentMemberId).FirstOrDefault();
                if (calEvent.AllowSelfCheckIn && calEvent.IsCurrentMemberApartOfEvent || (calEvent.AllowSelfCheckIn && isApartOfEvent != null))
                    calEvent.CanCurrentUserCheckIn = true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return calEvent;
        }

        public static CalendarEventPortable DisplayBirthday(MemberDisplay member)
        {
            CalendarEventPortable calEvent = new CalendarEventPortable();
            try
            {

                calEvent.EventType.EventType = CalendarEventTypeEnum.Birthday;

                calEvent.Name = member.DerbyName + " Birthday";
                calEvent.NameUrl = LibraryConfig.PublicSite + "/" + RDN.Library.Classes.Config.LibraryConfig.SportNameForUrl + "-birthday/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(member.DerbyName) + "/" + member.MemberId.ToString().Replace("-", "");
                calEvent.CalendarItemId = member.MemberId;
                DateTime dt = new DateTime(DateTime.UtcNow.Year, member.DOB.Month, member.DOB.Day);

                calEvent.StartDate = dt;
                calEvent.StartDateDisplay = dt.ToShortDateString();
                calEvent.EndDate = dt;
                calEvent.EndDateDisplay = dt.ToShortDateString();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return calEvent;
        }

        public static CalendarEventJson DisplayBirthdayJson(MemberDisplay member)
        {
            CalendarEventJson calEvent = new CalendarEventJson();
            try
            {

                calEvent.title = "BDay " + member.DerbyName;
                //removes length less than 14 chars 
                //because the title is too long for the calendar display.
                
                calEvent.id = member.MemberId;
                calEvent.url = VirtualPathUtility.ToAbsolute("~/calendar/birthday/" + member.MemberId.ToString().Replace("-", "") + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(member.DerbyName));
                DateTime dt = new DateTime(DateTime.UtcNow.Year, member.DOB.Month, member.DOB.Day);
                calEvent.StartDate = dt;
                calEvent.EndDate = dt;
                calEvent.start = dt.ToString("o");
                calEvent.end = dt.ToString("o");
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return calEvent;
        }
        public static CalendarEventPortable DisplayStartDate(MemberDisplay member)
        {
            CalendarEventPortable calEvent = new CalendarEventPortable();
            try
            {

                calEvent.EventType.EventType = CalendarEventTypeEnum.StartSkatingDate;

                calEvent.Name = member.DerbyName + " Started Skating Today";
                calEvent.NameUrl = LibraryConfig.PublicSite + "/started-skating-" + RDN.Library.Classes.Config.LibraryConfig.SportNameForUrl + "/" + member.MemberId.ToString().Replace("-", "") + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(member.DerbyName);
                calEvent.CalendarItemId = member.MemberId;
                try
                {
                    //put this because some years don't allow a leap year for Feb.
                    DateTime dt = new DateTime(DateTime.UtcNow.Year, member.StartedSkating.Value.Month, member.StartedSkating.Value.Day);
                    calEvent.StartDate = dt;
                    calEvent.EndDate = dt;
                }
                catch { }
                calEvent.StartDateDisplay = member.StartedSkating.Value.Month + "/" + member.StartedSkating.Value.Day + "/" + DateTime.UtcNow.Year;
                calEvent.EndDateDisplay = member.StartedSkating.Value.Month + "/" + member.StartedSkating.Value.Day + "/" + DateTime.UtcNow.Year;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: member.StartedSkating.Value.Month + ":" + member.StartedSkating.Value.Day);
            }
            return calEvent;
        }

        public static CalendarEventJson DisplayStartDateJson(MemberDisplay member)
        {
            CalendarEventJson calEvent = new CalendarEventJson();
            try
            {
                calEvent.title = "SS:" + member.DerbyName;
                //removes length less than 14 chars 
                //because the title is too long for the calendar display.
                
                calEvent.id = member.MemberId;
                calEvent.url = VirtualPathUtility.ToAbsolute("~/calendar/started-skating/" + member.MemberId.ToString().Replace("-", "") + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(member.DerbyName));
                DateTime dt = new DateTime(DateTime.UtcNow.Year, member.StartedSkating.Value.Month, member.StartedSkating.Value.Day);
                calEvent.StartDate = dt;
                calEvent.EndDate = dt;
                calEvent.start = dt.ToString("o");
                calEvent.end = dt.ToString("o");
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return calEvent;
        }




    }
}
