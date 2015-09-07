using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Calendar.Enums;
using RDN.Library.DataModels.Context;
using RDN.Library.Classes.ContactCard;
using RDN.Library.Classes.Error;
using System.Web;
using ScheduleWidget.ScheduledEvents;
using RDN.Library.Classes.Calendar.Report;
using RDN.Library.Classes.Calendar.Reports;
using RDN.Library.Classes.Controls.Calendar.Models;
using RDN.Library.Classes.Controls.Calendar;
using DDay.iCal;
using System.Drawing;
using RDN.Portable.Classes.Controls.Calendar.Enums;
using RDN.Portable.Classes.Controls.Calendar;

namespace RDN.Library.Classes.Calendar
{
    public class CalendarFactory
    {
        public CalendarFactory() { }

        /// <summary>
        /// any changes made here need to be mirrored in getcalendarschedule
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ownerEntity"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static RDN.Portable.Classes.Controls.Calendar.Calendar GetCalendarScheduleForView(Guid id, CalendarOwnerEntityEnum ownerEntity, DateTime startDate, DateTime endDate, Guid memberId)
        {
            RDN.Portable.Classes.Controls.Calendar.Calendar cal = new RDN.Portable.Classes.Controls.Calendar.Calendar();

            try
            {
                var dc = new ManagementContext();
                var calDb = (from xx in dc.Calendar.Include("CalendarEventsReocurring").Include("CalendarEvents").Include("CalendarEvents.ReocurringEvent").Include("CalendarEvents.PointsForEvent")
                             where xx.CalendarId == id
                             select new
                             {
                                 xx.IsCalendarInUTC,
                                 xx.TimeZone,
                                 xx.TimeZoneSelection,
                                 xx.DisableStartSkatingDays,
                                 xx.DisableBirthdaysFromShowing,
                                 Events = xx.CalendarEvents.Where(x => x.StartDate >= startDate && x.EndDate <= endDate && x.IsRemovedFromCalendar == false),
                                 EventsReocurring = xx.CalendarEventsReocurring.Where(x => (x.EndReocurring >= startDate || x.EndReocurring == null) && x.StartReocurring <= endDate && x.IsRemovedFromCalendar == false),
                             }).FirstOrDefault();

                string baseUrl = VirtualPathUtility.ToAbsolute("~/calendar/event/" + ownerEntity.ToString() + "/" + id.ToString().Replace("-", "") + "/");
                cal.DisableSkatingStartDates = calDb.DisableStartSkatingDays;
                cal.DisableBirthdays = calDb.DisableBirthdaysFromShowing;
                //adds all the events from the database that were in the date range selected.
                foreach (var ev in calDb.Events)
                {
                    try
                    {
                        CalendarEventJson calEvent = new CalendarEventJson();
                        if (ev.Color != null)
                        {
                            var c = Color.FromArgb(ev.Color.ColorIdCSharp);
                            calEvent.backColor = ColorTranslator.ToHtml(c);
                        }
                        calEvent.title = ev.Name;

                        calEvent.id = ev.CalendarItemId;
                        if (ev.ReocurringEvent != null)
                            calEvent.ReocurringId = ev.ReocurringEvent.CalendarItemId;
                        calEvent.url = baseUrl + ev.CalendarItemId.ToString().Replace("-", "");
                        if (!ev.IsInUTCTime)
                        {
                            calEvent.start = ev.StartDate.ToString("o");
                            calEvent.end = ev.EndDate.ToString("o");
                            calEvent.StartDate = ev.StartDate;
                            calEvent.EndDate = ev.EndDate;
                        }
                        else
                        {
                            calEvent.StartDate = ev.StartDate + new TimeSpan(calDb.TimeZone, 0, 0);
                            calEvent.EndDate = ev.EndDate + new TimeSpan(calDb.TimeZone, 0, 0);
                            if (calEvent.StartDate != null)
                                calEvent.start = calEvent.StartDate.ToString("o");
                            if (calEvent.EndDate != null)
                                calEvent.end = calEvent.EndDate.ToString("o");
                        }
                        cal.EventsJson.Add(calEvent);
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: calDb.TimeZone + ":" + ev.StartDate + ":" + ev.EndDate);
                    }
                }

                int amount = dc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return cal;
        }
        /// <summary>
        /// gets the public calendar of the league
        /// </summary>
        /// <param name="leagueId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static List<CalendarViewEventJson> GetPublicCalendarOfLeagueScheduleForView(Guid leagueId, DateTime startDate, DateTime endDate)
        {
            List<CalendarViewEventJson> events = new List<CalendarViewEventJson>();
            try
            {
                var dc = new ManagementContext();
                var calDb = (from xx in dc.CalendarLeagueOwners
                             where xx.League.LeagueId == leagueId
                             select new
                             {
                                 Events = xx.Calendar.CalendarEvents.Where(x => x.StartDate >= startDate && x.EndDate <= endDate && x.IsRemovedFromCalendar == false && x.IsPublicEvent),
                                 EventsReocurring = xx.Calendar.CalendarEventsReocurring.Where(x => (x.EndReocurring >= startDate || x.EndReocurring == null) && x.StartReocurring <= endDate && x.IsRemovedFromCalendar == false && x.IsPublic),
                             }).FirstOrDefault();

                string baseUrl = VirtualPathUtility.ToAbsolute("~/" + RDN.Library.Classes.Config.LibraryConfig.SportNameForUrl + "-event/");
                if (calDb != null)
                {
                    //adds all the events from the database that were in the date range selected.
                    foreach (var ev in calDb.Events)
                    {
                        CalendarViewEventJson calEvent = new CalendarViewEventJson();
                        if (ev.Color != null)
                        {
                            var c = Color.FromArgb(ev.Color.ColorIdCSharp);
                            calEvent.backColor = ColorTranslator.ToHtml(c);
                        }
                        calEvent.title = ev.Name;
                        //removes length less than 14 chars 
                        //because the title is too long for the calendar display.
                        if (ev.Name.Length > 10)
                            calEvent.title = calEvent.title.Remove(10);
                        calEvent.id = ev.CalendarItemId;
                        if (ev.ReocurringEvent != null)
                            calEvent.ReocurringId = ev.ReocurringEvent.CalendarItemId;
                        calEvent.url = baseUrl + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(ev.Name) + "/" + ev.CalendarItemId.ToString().Replace("-", "");
                        if (!ev.IsInUTCTime)
                        {
                            calEvent.start = ev.StartDate.ToString("o");
                            calEvent.end = ev.EndDate.ToString("o");
                        }
                        else
                        {
                            calEvent.start = (ev.StartDate + new TimeSpan(ev.Calendar.TimeZone, 0, 0)).ToString("o");
                            calEvent.end = (ev.EndDate + new TimeSpan(ev.Calendar.TimeZone, 0, 0)).ToString("o");
                        }
                        events.Add(calEvent);
                    }
                    foreach (var ev in calDb.EventsReocurring)
                    {
                        if (ev.LastDateEventsWereCreated.GetValueOrDefault() < endDate)
                        {
                            Guid locationId = new Guid();
                            if (ev.Location != null)
                            {
                                locationId = ev.Location.LocationId;
                            }
                            var aEvent = new CalendarViewEventJson()
                            {
                                id = ev.CalendarItemId,
                                Title = ev.Name,
                                Frequency = ev.FrequencyReocurring,
                                DaysOfWeek = ev.DaysOfWeekReocurring,
                                MonthlyInterval = ev.MonthlyIntervalReocurring
                            };
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

                            //iterates through all the events that were automatically generated from the calendar control
                            //then creates a json view. and adds them to the list of events for the user.
                            foreach (var date in schedule.Occurrences(range))
                            {
                                CalendarViewEventJson calEvent = new CalendarViewEventJson();
                                if (ev.Color != null)
                                {
                                    var c = Color.FromArgb(ev.Color.ColorIdCSharp);
                                    calEvent.backColor = ColorTranslator.ToHtml(c);
                                }
                                calEvent.title = ev.Name;
                                if (ev.Name.Length > 10)
                                    calEvent.title.Remove(10);
                                calEvent.id = ev.CalendarItemId;
                                calEvent.url = baseUrl + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(ev.Name) + "/" + ev.CalendarItemId.ToString().Replace("-", "");

                                if (!ev.IsInUTCTime)
                                {
                                    calEvent.start = new DateTime(date.Year, date.Month, date.Day, ev.StartDate.Hour, ev.StartDate.Minute, ev.StartDate.Second).ToString("o");
                                    calEvent.end = new DateTime(date.Year, date.Month, date.Day, ev.EndDate.Hour, ev.EndDate.Minute, ev.EndDate.Second).ToString("o");
                                    calEvent.StartDate = new DateTime(date.Year, date.Month, date.Day, ev.StartDate.Hour, ev.StartDate.Minute, ev.StartDate.Second);
                                    calEvent.EndDate = new DateTime(date.Year, date.Month, date.Day, ev.EndDate.Hour, ev.EndDate.Minute, ev.EndDate.Second);
                                }
                                else
                                {
                                    calEvent.StartDate = new DateTime(date.Year, date.Month, date.Day, ev.StartDate.Hour, ev.StartDate.Minute, ev.StartDate.Second);
                                    calEvent.EndDate = new DateTime(date.Year, date.Month, date.Day, ev.EndDate.Hour, ev.EndDate.Minute, ev.EndDate.Second);

                                    calEvent.start = (calEvent.StartDate + new TimeSpan(ev.Calendar.TimeZone, 0, 0)).ToString("o");
                                    calEvent.end = (calEvent.EndDate + new TimeSpan(ev.Calendar.TimeZone, 0, 0)).ToString("o");
                                    calEvent.StartDate = (calEvent.StartDate + new TimeSpan(ev.Calendar.TimeZone, 0, 0));
                                    calEvent.EndDate = (calEvent.EndDate + new TimeSpan(ev.Calendar.TimeZone, 0, 0));
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return events;
        }


        public static bool CheckInRemove(Guid calendarId, Guid eventId, Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var self = (from xx in dc.CalendarEvents
                            where xx.Calendar.CalendarId == calendarId
                            where xx.CalendarItemId == eventId
                            select xx).FirstOrDefault();
                if (self == null)
                    return false;

                var mem = self.Attendees.Where(x => x.Attendant.MemberId == memberId).FirstOrDefault();

                if (mem != null)
                {
                    mem.PointTypeEnum = (int)CalendarEventPointTypeEnum.None;
                    mem.Note = String.Empty;
                    mem.SecondaryPointTypeEnum = (int)CalendarEventPointTypeEnum.None;
                    int c = dc.SaveChanges();
                    return c > 0;
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        /// <summary>
        /// checks the person into event.  Allows the person to check them selves into the event.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="eventId"></param>
        /// <param name="memberId"></param>
        /// <param name="note"></param>
        /// <param name="pointType"></param>
        /// <returns></returns>

        public static CalendarEventPointTypeEnum GetEventCheckInStatus(Guid calendarId, Guid eventId, Guid memberId)
        {
            var dc = new ManagementContext();
            var self = (from xx in dc.CalendarEvents
                        where xx.Calendar.CalendarId == calendarId
                        where xx.CalendarItemId == eventId
                        select xx).FirstOrDefault();
            if (self != null)
            {
                var mem = self.Attendees.Where(x => x.Attendant.MemberId == memberId).FirstOrDefault();
                if (mem != null)
                    return (CalendarEventPointTypeEnum)mem.PointTypeEnum;
            }
            return CalendarEventPointTypeEnum.None;
        }

        public static AvailibilityEnum GetEventRSVPStatus(Guid calendarId, Guid eventId, Guid memberId)
        {
            var dc = new ManagementContext();
            var self = (from xx in dc.CalendarEvents
                        where xx.Calendar.CalendarId == calendarId
                        where xx.CalendarItemId == eventId
                        select xx).FirstOrDefault();
            if (self != null)
            {
                var mem = self.Attendees.FirstOrDefault(x => x.Attendant.MemberId == memberId);
                if (mem != null)
                    return (AvailibilityEnum)mem.AvailibityEnum;
            }
            return AvailibilityEnum.None;
        }


        public static bool CheckSelfIn(Guid calendarId, Guid eventId, Guid memberId, string note, CalendarEventPointTypeEnum pointType, bool isTardy, int additionalPoints)
        {
            try
            {
                var dc = new ManagementContext();
                var self = (from xx in dc.CalendarEvents
                            where xx.Calendar.CalendarId == calendarId
                            where xx.CalendarItemId == eventId
                            select xx).FirstOrDefault();
                if (self == null)
                    return false;

                if (self.Attendees.Where(x => x.Attendant.MemberId == memberId).FirstOrDefault() == null)
                {
                    DataModels.Calendar.CalendarAttendance att = new DataModels.Calendar.CalendarAttendance();

                    att.Attendant = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                    att.CalendarItem = self;
                    att.Note = note;
                    att.PointTypeEnum = Convert.ToInt32(pointType);
                    att.AdditionalPoints = additionalPoints;
                    if (isTardy)
                        att.SecondaryPointTypeEnum = Convert.ToInt32(CalendarEventPointTypeEnum.Tardy);
                    dc.CalendarAttendance.Add(att);
                    dc.SaveChanges();
                }
                else
                {
                    var mem = self.Attendees.Where(x => x.Attendant.MemberId == memberId).FirstOrDefault();
                    mem.PointTypeEnum = Convert.ToInt32(pointType);
                    mem.Note = note;
                    mem.AdditionalPoints = additionalPoints;
                    if (isTardy)
                        mem.SecondaryPointTypeEnum = Convert.ToInt32(CalendarEventPointTypeEnum.Tardy);
                    dc.SaveChanges();
                }
                return true;
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool SetAvailabilityForEvent(Guid calendarId, Guid eventId, Guid memberId, string note, AvailibilityEnum avail)
        {
            try
            {
                var dc = new ManagementContext();
                var self = (from xx in dc.CalendarEvents
                            where xx.Calendar.CalendarId == calendarId
                            where xx.CalendarItemId == eventId
                            select xx).FirstOrDefault();
                if (self == null)
                    return false;

                if (self.Attendees.Where(x => x.Attendant.MemberId == memberId).FirstOrDefault() == null)
                {
                    DataModels.Calendar.CalendarAttendance att = new DataModels.Calendar.CalendarAttendance();

                    att.Attendant = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                    att.CalendarItem = self;
                    att.AvailabilityNote = note;
                    att.AvailibityEnum = (byte)avail;
                    dc.CalendarAttendance.Add(att);
                    int c = dc.SaveChanges();
                    return c > 0;
                }
                else
                {
                    var mem = self.Attendees.Where(x => x.Attendant.MemberId == memberId).FirstOrDefault();
                    mem.AvailabilityNote = note;
                    mem.AvailibityEnum = (byte)avail;
                    int c = dc.SaveChanges();
                    return true;
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static CalendarEventType GetEventType(long eventTypeId)
        {
            CalendarEventType ty = new CalendarEventType();
            try
            {
                var dc = new ManagementContext();
                var locations = (from xx in dc.CalendarEventTypes
                                 where xx.CalendarEventTypeId == eventTypeId
                                 select new
                                 {
                                     xx.CalendarEventTypeId,
                                     xx.EventTypeName,
                                     xx.PointsForExcused,
                                     xx.PointsForNotPresent,
                                     xx.PointsForPartial,
                                     xx.PointsForPresent,
                                     xx.PointsForTardy,
                                     xx.DefaultColor,
                                     xx.IsRemoved

                                 }).FirstOrDefault();
                if (locations != null)
                {
                    ty.CalendarEventTypeId = locations.CalendarEventTypeId;
                    ty.EventTypeName = locations.EventTypeName;
                    ty.PointsForExcused = locations.PointsForExcused;
                    ty.PointsForNotPresent = locations.PointsForNotPresent;
                    ty.PointsForPartial = locations.PointsForPartial;
                    ty.PointsForPresent = locations.PointsForPresent;
                    ty.PointsForTardy = locations.PointsForTardy;
                    ty.IsRemoved = locations.IsRemoved;
                    if (locations.DefaultColor != null)
                    {
                        var c = Color.FromArgb(locations.DefaultColor.ColorIdCSharp);
                        ty.ColorTempSelected = ColorTranslator.ToHtml(c);
                        ty.ColorName = locations.DefaultColor.ColorName;
                    }
                    return ty;
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return ty;
        }

        public static List<CalendarEventType> GetEventTypesOfCalendar(Guid calendarId)
        {
            List<CalendarEventType> newEventTypes = new List<CalendarEventType>();

            try
            {
                var dc = new ManagementContext();
                var locations = (from xx in dc.CalendarEventTypes
                                 where xx.CalendarOwner.CalendarId == calendarId && xx.IsRemoved == false
                                 select new
                                 {
                                     xx.CalendarEventTypeId,
                                     xx.EventTypeName,
                                     xx.PointsForExcused,
                                     xx.PointsForNotPresent,
                                     xx.PointsForPartial,
                                     xx.PointsForPresent,
                                     xx.PointsForTardy,
                                     xx.DefaultColor
                                 }).ToList();
                foreach (var even in locations)
                {
                    CalendarEventType ty = new CalendarEventType();
                    ty.CalendarId = calendarId;
                    ty.CalendarEventTypeId = even.CalendarEventTypeId;
                    ty.EventTypeName = even.EventTypeName;
                    ty.PointsForExcused = even.PointsForExcused;
                    ty.PointsForNotPresent = even.PointsForNotPresent;
                    ty.PointsForPartial = even.PointsForPartial;
                    ty.PointsForPresent = even.PointsForPresent;
                    ty.PointsForTardy = even.PointsForTardy;
                    if (even.DefaultColor != null)
                    {
                        var c = Color.FromArgb(even.DefaultColor.ColorIdCSharp);
                        ty.ColorTempSelected = ColorTranslator.ToHtml(c);
                        ty.ColorName = even.DefaultColor.ColorName;
                    }
                    newEventTypes.Add(ty);
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return newEventTypes;
        }

        public static List<CalendarEventType> GetAllEventTypesOfCalendar(Guid calendarId)
        {
            List<CalendarEventType> newEventTypes = new List<CalendarEventType>();

            try
            {
                var dc = new ManagementContext();
                var locations = (from xx in dc.CalendarEventTypes
                                 where xx.CalendarOwner.CalendarId == calendarId
                                 select new
                                 {
                                     xx.CalendarEventTypeId,
                                     xx.EventTypeName,
                                     xx.PointsForExcused,
                                     xx.PointsForNotPresent,
                                     xx.PointsForPartial,
                                     xx.PointsForPresent,
                                     xx.PointsForTardy,
                                     xx.DefaultColor
                                 }).ToList();
                foreach (var even in locations)
                {
                    CalendarEventType ty = new CalendarEventType();
                    ty.CalendarId = calendarId;
                    ty.CalendarEventTypeId = even.CalendarEventTypeId;
                    ty.EventTypeName = even.EventTypeName;
                    ty.PointsForExcused = even.PointsForExcused;
                    ty.PointsForNotPresent = even.PointsForNotPresent;
                    ty.PointsForPartial = even.PointsForPartial;
                    ty.PointsForPresent = even.PointsForPresent;
                    ty.PointsForTardy = even.PointsForTardy;
                    if (even.DefaultColor != null)
                    {
                        var c = Color.FromArgb(even.DefaultColor.ColorIdCSharp);
                        ty.ColorTempSelected = ColorTranslator.ToHtml(c);
                        ty.ColorName = even.DefaultColor.ColorName;
                    }
                    newEventTypes.Add(ty);
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return newEventTypes;
        }

        public static List<RDN.Portable.Classes.Location.Location> GetLocationsOnlyOfCalendar(Guid calendarId)
        {
            List<RDN.Portable.Classes.Location.Location> newLocations = new List<RDN.Portable.Classes.Location.Location>();
            try
            {
                var dc = new ManagementContext();
                var locations = (from xx in dc.Calendar
                                 where xx.CalendarId == calendarId
                                 select new
                                 {
                                     Locations = xx.Locations.Where(x => x.IsRemoved == false)
                                 }).FirstOrDefault();

                foreach (var loc in locations.Locations)
                {
                    try
                    {
                        RDN.Portable.Classes.Location.Location l = new RDN.Portable.Classes.Location.Location();
                        l.LocationName = loc.LocationName;
                        l.LocationId = loc.LocationId;
                        if (loc.Contact != null && loc.Contact.Addresses.FirstOrDefault() != null)
                        {
                            var add = loc.Contact.Addresses.FirstOrDefault();
                            RDN.Portable.Classes.ContactCard.Address a = new RDN.Portable.Classes.ContactCard.Address();
                            a.Address1 = add.Address1;
                            a.Address2 = add.Address2;
                            a.AddressId = add.AddressId;
                            a.CityRaw = add.CityRaw;
                            if (add.Country != null)
                                a.Country = add.Country.Code;
                            a.IsDefault = add.IsDefault;
                            a.StateRaw = add.StateRaw;
                            a.Zip = add.Zip;
                            l.Contact.Addresses.Add(a);
                        }
                        if (newLocations.Where(x => x.LocationId == l.LocationId).FirstOrDefault() == null)
                            newLocations.Add(l);
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
            return newLocations;
        }

        public static List<RDN.Portable.Classes.Location.Location> GetLocationsOfCalendar(Guid calendarId)
        {
            List<RDN.Portable.Classes.Location.Location> newLocations = new List<Portable.Classes.Location.Location>();
            try
            {
                var dc = new ManagementContext();
                var locations = (from xx in dc.Calendar
                                 where xx.CalendarId == calendarId
                                 select new
                                 {
                                     xx.CalendarEvents,
                                     xx.Locations
                                 }).FirstOrDefault();

                foreach (var loc in locations.Locations.Where(x => x.IsRemoved == false))
                {
                    try
                    {
                        RDN.Portable.Classes.Location.Location l = new Portable.Classes.Location.Location();
                        l.LocationName = loc.LocationName;
                        l.LocationId = loc.LocationId;
                        if (loc.Contact != null && loc.Contact.Addresses.FirstOrDefault() != null)
                        {
                            var add = loc.Contact.Addresses.FirstOrDefault();
                            RDN.Portable.Classes.ContactCard.Address a = new RDN.Portable.Classes.ContactCard.Address();
                            a.Address1 = add.Address1;
                            a.Address2 = add.Address2;
                            a.AddressId = add.AddressId;
                            a.CityRaw = add.CityRaw;
                            if (add.Country != null)
                                a.Country = add.Country.Code;
                            a.IsDefault = add.IsDefault;
                            a.StateRaw = add.StateRaw;
                            a.Zip = add.Zip;
                            l.Contact.Addresses.Add(a);
                        }
                        if (newLocations.Where(x => x.LocationId == l.LocationId).FirstOrDefault() == null)
                            newLocations.Add(l);
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }
                foreach (var loc in locations.CalendarEvents)
                {
                    try
                    {
                        if (loc.Location != null)
                        {
                            RDN.Portable.Classes.Location.Location l = new Portable.Classes.Location.Location();
                            l.LocationName = loc.Location.LocationName;
                            l.LocationId = loc.Location.LocationId;
                            if (loc.Location.Contact != null && loc.Location.Contact.Addresses.FirstOrDefault() != null)
                            {
                                var add = loc.Location.Contact.Addresses.FirstOrDefault();
                                RDN.Portable.Classes.ContactCard.Address a = new RDN.Portable.Classes.ContactCard.Address();
                                a.Address1 = add.Address1;
                                a.Address2 = add.Address2;
                                a.AddressId = add.AddressId;
                                a.CityRaw = add.CityRaw;
                                if (add.Country != null)
                                    a.Country = add.Country.Code;
                                a.IsDefault = add.IsDefault;
                                a.StateRaw = add.StateRaw;
                                a.Zip = add.Zip;
                                l.Contact.Addresses.Add(a);
                            }
                            if (newLocations.Where(x => x.LocationId == l.LocationId).FirstOrDefault() == null)
                                newLocations.Add(l);
                        }
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
            return newLocations;
        }

        public static RDN.Portable.Classes.Controls.Calendar.Calendar CreateCalendar(Guid ownerId, CalendarOwnerEntityEnum ownerType)
        {
            RDN.Portable.Classes.Controls.Calendar.Calendar calNew = new RDN.Portable.Classes.Controls.Calendar.Calendar();
            try
            {
                var dc = new ManagementContext();
                DataModels.Calendar.Calendar cal = new DataModels.Calendar.Calendar();
                cal.AllowSelfCheckIn = false;
                cal.IsCalendarInUTC = true;
                if (ownerType == CalendarOwnerEntityEnum.federation)
                {
                    DataModels.Calendar.CalendarFederationOwnership owner = new DataModels.Calendar.CalendarFederationOwnership();
                    owner.Calendar = cal;
                    owner.Federation = dc.Federations.Where(x => x.FederationId == ownerId).FirstOrDefault();
                    owner.OwnerType = Convert.ToInt32(CalendarOwnerTypeEnum.Owner);
                    cal.FederationOwners.Add(owner);
                }
                else if (ownerType == CalendarOwnerEntityEnum.league)
                {
                    DataModels.Calendar.CalendarLeagueOwnership owner = new DataModels.Calendar.CalendarLeagueOwnership();
                    owner.Calendar = cal;
                    owner.League = dc.Leagues.Where(x => x.LeagueId == ownerId).FirstOrDefault();
                    owner.OwnerType = Convert.ToInt32(CalendarOwnerTypeEnum.Owner);
                    cal.LeagueOwners.Add(owner);
                }
                else if (ownerType == CalendarOwnerEntityEnum.member)
                {
                    DataModels.Calendar.CalendarMemberOwnership owner = new DataModels.Calendar.CalendarMemberOwnership();
                    owner.Calendar = cal;
                    owner.Member = dc.Members.Where(x => x.MemberId == ownerId).FirstOrDefault();
                    owner.OwnerType = Convert.ToInt32(CalendarOwnerTypeEnum.Owner);
                    cal.MemberOwners.Add(owner);
                }
                dc.Calendar.Add(cal);
                dc.SaveChanges();

                calNew.CalendarId = cal.CalendarId;
                calNew.OwnerEntity = ownerType;
                calNew.AllowSelfCheckIn = false;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return calNew;
        }

        public static long UpdateCalendarEventType(CalendarEventType calendarEventType)
        {
            try
            {
                var dc = new ManagementContext();
                var t = dc.CalendarEventTypes.FirstOrDefault(x => x.CalendarEventTypeId == calendarEventType.CalendarEventTypeId && x.IsRemoved == false);
                if (t != null)
                {
                    t.EventTypeName = calendarEventType.EventTypeName;
                    t.PointsForExcused = calendarEventType.PointsForExcused;
                    t.PointsForNotPresent = calendarEventType.PointsForNotPresent;
                    t.PointsForPartial = calendarEventType.PointsForPartial;
                    t.PointsForPresent = calendarEventType.PointsForPresent;
                    t.PointsForTardy = calendarEventType.PointsForTardy;
                    if (!String.IsNullOrEmpty(calendarEventType.ColorTempSelected))
                    {
                        Color c = ColorTranslator.FromHtml(calendarEventType.ColorTempSelected);
                        int arb = c.ToArgb();
                        t.DefaultColor = dc.Colors.Where(x => x.ColorIdCSharp == arb).FirstOrDefault();
                    }
                    dc.SaveChanges();
                }
                return t.CalendarEventTypeId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }

        public static bool DeleteCalendarEventType(long calendarEventTypeId)
        {
            try
            {
                var dc = new ManagementContext();
                var eventType = dc.CalendarEventTypes.FirstOrDefault(x => x.CalendarEventTypeId == calendarEventTypeId && x.IsRemoved == false);
                if (eventType != null)
                {
                    eventType.IsRemoved = true;
                    int c = dc.SaveChanges();
                    if (c > 0)
                        return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static long AddCalendarEventType(CalendarEventType calendarEventType)
        {
            try
            {
                var dc = new ManagementContext();
                RDN.Library.DataModels.Calendar.CalendarEventType t = new DataModels.Calendar.CalendarEventType();
                t.CalendarOwner = dc.Calendar.FirstOrDefault(x => x.CalendarId == calendarEventType.CalendarId);
                t.EventTypeName = calendarEventType.EventTypeName;
                t.PointsForExcused = calendarEventType.PointsForExcused;
                t.PointsForNotPresent = calendarEventType.PointsForNotPresent;
                t.PointsForPartial = calendarEventType.PointsForPartial;
                t.PointsForPresent = calendarEventType.PointsForPresent;
                t.PointsForTardy = calendarEventType.PointsForTardy;
                if (!String.IsNullOrEmpty(calendarEventType.ColorTempSelected))
                {
                    Color c = ColorTranslator.FromHtml(calendarEventType.ColorTempSelected);
                    int arb = c.ToArgb();
                    t.DefaultColor = dc.Colors.Where(x => x.ColorIdCSharp == arb).FirstOrDefault();
                }
                dc.CalendarEventTypes.Add(t);
                int cc = dc.SaveChanges();
                return t.CalendarEventTypeId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }

        public static int UpdateCalendarSettings(RDN.Portable.Classes.Controls.Calendar.Calendar calendar)
        {
            try
            {
                var dc = new ManagementContext();
                var cal = dc.Calendar.Where(x => x.CalendarId == calendar.CalendarId).FirstOrDefault();
                cal.AllowSelfCheckIn = calendar.AllowSelfCheckIn;
                cal.NotPresentCheckIn = calendar.NotPresentCheckIn;
                cal.DisableStartSkatingDays = calendar.DisableSkatingStartDates;
                cal.DisableBirthdaysFromShowing = calendar.DisableBirthdays;
                cal.TimeZoneSelection = dc.TimeZone.Where(x => x.ZoneId == calendar.TimeZoneId).FirstOrDefault();
                if (cal.TimeZoneSelection != null)
                    cal.TimeZone = cal.TimeZoneSelection.GMTOffset;

                cal.IsCalendarInUTC = true;
                return dc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }

        public static int ImportFeedFromCalendar(Guid calendarId)
        {
            try
            {
                var dc = new ManagementContext();
                var cal = dc.Calendar.Include("CalendarEvents").Where(x => x.CalendarId == calendarId).FirstOrDefault();
                if ((CalendarImportTypeEnum)cal.CalendarImportTypeEnum == CalendarImportTypeEnum.Google)
                {
                    if (!String.IsNullOrEmpty(cal.ImportFeedUrl))
                    {
                        try
                        {
                            var col = iCalendar.LoadFromUri(new Uri(cal.ImportFeedUrl)).FirstOrDefault();
                            DateTime MinDate = DateTime.UtcNow.AddYears(-1);
                            foreach (var even in col.Events)
                            {
                                try
                                {
                                    if (even.Start.Date > MinDate)
                                    {
                                        var oldEvent = cal.CalendarEvents.Where(x => x.StartDate > MinDate && x.EventFeedId == even.UID).FirstOrDefault();
                                        if (oldEvent == null)
                                        {
                                            CalendarEventFactory.CreateNewEventFromFeedUrl(cal.CalendarId, even.UID, new DateTime(even.Start.UTC.Ticks), new DateTime(even.End.UTC.Ticks), even.Location, even.Summary, string.Empty, even.Description, true);
                                        }
                                    }
                                }
                                catch (Exception exception)
                                {
                                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: cal.ImportFeedUrl);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }

        public static int UpdateCalendarFeedSettings(CalendarImport import)
        {
            try
            {
                var dc = new ManagementContext();
                var cal = dc.Calendar.Where(x => x.CalendarId == import.CalendarId).FirstOrDefault();
                cal.ImportFeedUrl = import.GoogleCalendarUrl;
                cal.TimeZoneSelection = dc.TimeZone.Where(x => x.ZoneId == import.TimeZoneId).FirstOrDefault();
                if (cal.TimeZoneSelection != null)
                    cal.TimeZone = cal.TimeZoneSelection.GMTOffset;

                if (!String.IsNullOrEmpty(import.GoogleCalendarUrl))
                    cal.CalendarImportTypeEnum = (byte)CalendarImportTypeEnum.Google;
                else
                    cal.CalendarImportTypeEnum = (byte)CalendarImportTypeEnum.None;
                cal.IsCalendarInUTC = true;
                int c = dc.SaveChanges();
                return c;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }
        /// <summary>
        /// gets the owner entity id of the league or the federation
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static Guid GetEntityOwnerId(Guid calendarId, CalendarOwnerEntityEnum owner)
        {
            try
            {
                var dc = new ManagementContext();
                var calDb = (from xx in dc.Calendar
                             where xx.CalendarId == calendarId
                             select new
                             {
                                 xx.FederationOwners,
                                 xx.LeagueOwners
                             }).FirstOrDefault();

                if (calDb == null)
                    return new Guid();

                if (calDb.FederationOwners.FirstOrDefault() != null)
                    return calDb.FederationOwners.FirstOrDefault().Federation.FederationId;
                if (calDb.LeagueOwners.FirstOrDefault() != null)
                    return calDb.LeagueOwners.FirstOrDefault().League.LeagueId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }

        public static RDN.Portable.Classes.Controls.Calendar.Calendar GetCalendar(Guid id, CalendarOwnerEntityEnum ownerEntity)
        {
            RDN.Portable.Classes.Controls.Calendar.Calendar newCal = new RDN.Portable.Classes.Controls.Calendar.Calendar();
            try
            {
                var dc = new ManagementContext();
                var calDb = (from xx in dc.Calendar
                             where xx.CalendarId == id
                             select new
                             {
                                 xx.CalendarId,
                                 xx.AllowSelfCheckIn,
                                 xx.NotPresentCheckIn,
                                 xx.DisableBirthdaysFromShowing,
                                 xx.DisableStartSkatingDays,
                                 xx.ImportFeedUrl,
                                 xx.CalendarImportTypeEnum,
                                 xx.TimeZone,
                                 xx.TimeZoneSelection,
                                 xx.IsCalendarInUTC
                             }).FirstOrDefault();

                if (calDb == null)
                    return null;

                newCal.CalendarId = id;
                newCal.OwnerEntity = ownerEntity;
                newCal.AllowSelfCheckIn = calDb.AllowSelfCheckIn;
                newCal.NotPresentCheckIn = calDb.NotPresentCheckIn;
                newCal.DisableSkatingStartDates = calDb.DisableStartSkatingDays;
                newCal.DisableBirthdays = calDb.DisableBirthdaysFromShowing;
                newCal.ImportFeedType = (CalendarImportTypeEnum)calDb.CalendarImportTypeEnum;
                newCal.ImportFeedUrl = calDb.ImportFeedUrl;
                if (calDb.TimeZoneSelection != null)
                    newCal.TimeZoneId = calDb.TimeZoneSelection.ZoneId;
                newCal.TimeZone = (int)calDb.TimeZone;

                newCal.TimeZones = Classes.Location.TimeZoneFactory.GetTimeZones();

                newCal.IsCalendarInUTC = calDb.IsCalendarInUTC;

                var types = dc.CalendarEventTypes.Include("DefaultColor").Where(x => x.CalendarOwner.CalendarId == id && x.IsRemoved == false && x.IsRemoved == false);
                foreach (var type in types)
                {
                    CalendarEventType t = new CalendarEventType();
                    t.CalendarEventTypeId = type.CalendarEventTypeId;
                    t.EventTypeName = type.EventTypeName;
                    t.PointsForExcused = type.PointsForExcused;
                    t.PointsForNotPresent = type.PointsForNotPresent;
                    t.PointsForPartial = type.PointsForPartial;
                    t.PointsForPresent = type.PointsForPresent;
                    t.PointsForTardy = type.PointsForTardy;
                    if (type.DefaultColor != null)
                    {
                        var c = Color.FromArgb(type.DefaultColor.ColorIdCSharp);
                        t.ColorTempSelected = ColorTranslator.ToHtml(c);
                        t.ColorName = type.DefaultColor.ColorName;
                    }
                    newCal.EventTypes.Add(t);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return newCal;
        }


        /// <summary>
        /// any changes made here need to be mirrored in getcalendarscheduleForView
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ownerEntity"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static RDN.Portable.Classes.Controls.Calendar.Calendar GetCalendarSchedule(Guid id, CalendarOwnerEntityEnum ownerEntity, DateTime startDate, DateTime endDate, Guid memberId, bool isAttendanceManagerOrBetter)
        {
            RDN.Portable.Classes.Controls.Calendar.Calendar newCal = new RDN.Portable.Classes.Controls.Calendar.Calendar();
            try
            {
                var dc = new ManagementContext();
                var calDb = (from xx in dc.Calendar.Include("CalendarEventsReocurring").Include("CalendarEvents").Include("CalendarEvents.ReocurringEvent").Include("CalendarEvents.PointsForEvent")
                             where xx.CalendarId == id
                             select new
                             {
                                 xx.IsCalendarInUTC,
                                 xx.TimeZone,
                                 Events = xx.CalendarEvents.Where(x => x.StartDate >= startDate && x.EndDate <= endDate && x.IsRemovedFromCalendar == false),
                                 FedOwners = xx.FederationOwners,
                                 LeagueOwners = xx.LeagueOwners,
                                 AllowSelfCheckIn = xx.AllowSelfCheckIn,
                                 xx.DisableBirthdaysFromShowing,
                                 xx.DisableStartSkatingDays,
                                 EventsReocurring = xx.CalendarEventsReocurring.Where(x => (x.EndReocurring >= startDate || x.EndReocurring == null) && x.StartReocurring <= endDate && x.IsRemovedFromCalendar == false),
                             }).FirstOrDefault();
                newCal.IsCalendarInUTC = calDb.IsCalendarInUTC;
                newCal.TimeZone = calDb.TimeZone;
                newCal.CalendarId = id;
                newCal.OwnerEntity = ownerEntity;
                newCal.AllowSelfCheckIn = calDb.AllowSelfCheckIn;
                newCal.DisableBirthdays = calDb.DisableBirthdaysFromShowing;
                newCal.DisableSkatingStartDates = calDb.DisableStartSkatingDays;
                if (ownerEntity == CalendarOwnerEntityEnum.federation)
                {
                    foreach (var own in calDb.FedOwners)
                    {
                        CalendarOwner owner = new CalendarOwner();
                        owner.OwnerId = own.Federation.FederationId;
                        owner.OwnerName = own.Federation.Name;
                        newCal.EntityName = own.Federation.Name;

                        newCal.Owners.Add(owner);
                    }
                }
                else if (ownerEntity == CalendarOwnerEntityEnum.league)
                {
                    foreach (var own in calDb.LeagueOwners)
                    {
                        CalendarOwner owner = new CalendarOwner();
                        owner.OwnerId = own.League.LeagueId;
                        owner.OwnerName = own.League.Name;
                        newCal.EntityName = own.League.Name;
                        newCal.Owners.Add(owner);
                    }
                }

                foreach (var ev in calDb.Events)
                {
                    newCal.Events.Add(CalendarEventFactory.DisplayEvent(ev, memberId, isAttendanceManagerOrBetter));
                }
                foreach (var ev in calDb.EventsReocurring)
                {
                    //used so we can refresh the last day events were created..

                    // ev.LastDateEventsWereCreated = endDate.AddMonths(-1);
                    if (ev.LastDateEventsWereCreated.GetValueOrDefault() < endDate)
                    {
                        var eventRe = dc.CalendarEventsReocurring.Where(x => x.CalendarItemId == ev.CalendarItemId && x.IsRemovedFromCalendar == false).FirstOrDefault();
                        eventRe.LastDateEventsWereCreated = endDate;
                        eventRe.Calendar = eventRe.Calendar;
                        dc.SaveChanges();

                        var aEvent = new CalendarViewEventJson()
                        {
                            id = ev.CalendarItemId,
                            Title = ev.Name,
                            Frequency = ev.FrequencyReocurring,
                            DaysOfWeek = ev.DaysOfWeekReocurring,
                            MonthlyInterval = ev.MonthlyIntervalReocurring
                        };
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
                            CalendarEvent calEvent = new CalendarEvent();
                            Guid locationId = new Guid();
                            if (ev.Location != null)
                            {
                                calEvent.Location.LocationName = ev.Location.LocationName;
                                locationId = ev.Location.LocationId;
                            }
                            calEvent.Name = ev.Name;
                            calEvent.CalendarItemId = ev.CalendarItemId;
                            if (eventRe.Color != null)
                            {
                                var c = Color.FromArgb(eventRe.Color.ColorIdCSharp);
                                calEvent.ColorTempSelected = ColorTranslator.ToHtml(c);
                            }

                            if (!ev.IsInUTCTime)
                            {
                                calEvent.StartDate = new DateTime(date.Year, date.Month, date.Day, ev.StartDate.Hour, ev.StartDate.Minute, ev.StartDate.Second);
                                calEvent.EndDate = new DateTime(date.Year, date.Month, date.Day, ev.EndDate.Hour, ev.EndDate.Minute, ev.EndDate.Second);
                            }
                            else
                            {
                                //we have to create a temp dates so we can add the timezone information without going back a day
                                //if the time being used is on the border.
                                //without the tempdates 1/4/2013 7pm turned into 1/3/2013 7pm because the timezones didn't account for the 
                                //fact the dates were already in utc.
                                var startTempDate = new DateTime(date.Year, date.Month, date.Day, ev.StartDate.Hour, ev.StartDate.Minute, ev.StartDate.Second) + new TimeSpan(ev.Calendar.TimeZone, 0, 0);
                                var endTempDate = new DateTime(date.Year, date.Month, date.Day, ev.EndDate.Hour, ev.EndDate.Minute, ev.EndDate.Second) + new TimeSpan(ev.Calendar.TimeZone, 0, 0);
                                calEvent.StartDate = new DateTime(date.Year, date.Month, date.Day, startTempDate.Hour, startTempDate.Minute, startTempDate.Second);
                                calEvent.EndDate = new DateTime(date.Year, date.Month, date.Day, endTempDate.Hour, endTempDate.Minute, endTempDate.Second);
                            }

                            calEvent.Notes = ev.Notes;
                            calEvent.AllowSelfCheckIn = ev.AllowSelfCheckIn;

                            //var evs = (from xx in newCal.Events
                            //           where xx.CalendarReoccurringId == ev.CalendarItemId
                            //           where xx.StartDate == calEvent.StartDate
                            //           where xx.EndDate == calEvent.EndDate
                            //           select xx).FirstOrDefault();
                            ////dirty bit to check if event was already in list.  If it is, we don't add it.
                            ////the date check here is to only create events within the calendar selected dates
                            //if (evs == null && calEvent.StartDate >= startDate && calEvent.EndDate <= endDate)
                            //{
                            //    string colorHex = String.Empty;
                            //    if (ev.Color != null)
                            //    {
                            //        var c = Color.FromArgb(ev.Color.ColorIdCSharp);
                            //        colorHex = ColorTranslator.ToHtml(c);
                            //    }
                            //    Guid calItemId = CalendarEventFactory.CreateNewEvent(ev.Calendar.CalendarId, calEvent.StartDate, calEvent.EndDate, locationId, ev.Name, ev.Link, ev.Notes, ev.AllowSelfCheckIn, ev.IsPublic, true, calEvent.CalendarItemId, ev.EventType.CalendarEventTypeId, false, ev.TicketUrl, colorHex, new List<long>(), memberId);
                            //    calEvent.CalendarItemId = calItemId;
                            //    newCal.Events.Add(calEvent);
                            //}
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return newCal;
        }

        public static RDN.Portable.Classes.Controls.Calendar.Calendar GetCalendarEvents(Guid id, int count, Guid currentMemberId, bool isAttendanceManagerOrBetter)
        {
            RDN.Portable.Classes.Controls.Calendar.Calendar newCal = new RDN.Portable.Classes.Controls.Calendar.Calendar();
            try
            {
                var dc = new ManagementContext();
                var events = dc.CalendarEvents.Where(x => x.Calendar.CalendarId == id && x.StartDate >= DateTime.UtcNow && x.IsRemovedFromCalendar == false).OrderBy(x => x.StartDate).Take(count).AsParallel().ToList();

                newCal.CalendarId = id;

                foreach (var ev in events)
                {
                    newCal.Events.Add(CalendarEventFactory.DisplayEvent(ev, currentMemberId, isAttendanceManagerOrBetter));
                }
                if (newCal.Events.Count > 0)
                {
                    TimeSpan ts = newCal.Events.OrderBy(x => x.StartDate).FirstOrDefault().StartDate - DateTime.UtcNow;
                    newCal.DaysToNextEvent = ts.Days;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return newCal;
        }


        public static Guid GetCalendarIdOfLeague(Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var calId = dc.CalendarLeagueOwners.Where(x => x.League.LeagueId == leagueId).FirstOrDefault();
                if (calId != null)
                    return calId.Calendar.CalendarId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }
    }
}
