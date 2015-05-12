using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Cache;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Calendar.Enums;
using RDN.Library.Classes.Calendar.Reports;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Library.Classes.League.Classes;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Controls.Calendar.Enums;
using RDN.Portable.Classes.League.Classes;

namespace RDN.Library.Classes.Calendar.Report
{
    public class CalendarReport
    {
        public Guid CalendarId { get; set; }
        public string EntityName { get; set; }
        public int DaysBackwards { get; set; }
        public bool PullGroupEventsOnly { get; set; }
        public bool IsSubmitted { get; set; }
        public bool NotPresentForCheckIn { get; set; }
        public DateTime StartDateSelected { get; set; }
        public string StartDateSelectedDisplay { get; set; }
        public DateTime EndDateSelected { get; set; }
        public string EndDateSelectedDisplay { get; set; }
        public string ToGroupIds { get; set; }

        public int TotalPointsAllowed { get; set; }
        public int TotalEventsCount { get; set; }

        public List<EventTypeForReport> EventTypesReport { get; set; }
        public MembersReport MemberReport { get; set; }

        public List<EventForReport> Events { get; set; }
        public List<MembersReport> Attendees { get; set; }
        public List<LeagueGroup> GroupsForReport { get; set; }

        public CalendarReport()
        {
            Attendees = new List<MembersReport>();
            Events = new List<EventForReport>();
            EventTypesReport = new List<EventTypeForReport>();
            GroupsForReport = new List<LeagueGroup>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="calendarType"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="group"></param>
        /// <param name="reportOnGroupedEventsOnly">when true, it reports on events attached to the group only.</param>
        /// <returns></returns>
        public static CalendarReport GetReportForCalendar(Guid calendarId, string calendarType, DateTime startDate, DateTime endDate, long group, bool reportOnGroupedEventsOnly)
        {
            CalendarReport report = new CalendarReport();
            var memId = RDN.Library.Classes.Account.User.GetMemberId();
            var leagueId = MemberCache.GetLeagueIdOfMember(memId);
          
            try
            {

                var dc = new ManagementContext();
                var cal = (from xx in dc.Calendar
                           where xx.CalendarId == calendarId
                           select xx).FirstOrDefault();

                DateTime enddate_utc = endDate - new TimeSpan(cal.TimeZone, 0, 0);

                report.EndDateSelected = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
                report.StartDateSelected = startDate;

                startDate = startDate - new TimeSpan(cal.TimeZone, 0, 0);
                endDate = new DateTime(enddate_utc.Year, enddate_utc.Month, enddate_utc.Day, 23, 59, 59);

                report.CalendarId = calendarId;
                report.EntityName = calendarType;
                report.IsSubmitted = true;
                report.TotalPointsAllowed = 0;
                TimeSpan daysReporting = endDate - startDate;
                report.DaysBackwards = daysReporting.Days;

              
                report.NotPresentForCheckIn = cal.NotPresentCheckIn;
                List<DataModels.Calendar.CalendarEvent> events = new List<DataModels.Calendar.CalendarEvent>();
                List<MemberDisplay> members = new List<MemberDisplay>();
                if (group == 0) //no grops were selected
                {
                    members = MemberCache.GetLeagueMembers(memId, leagueId);
                    events = cal.CalendarEvents.Where(x => x.StartDate >= startDate).Where(x => x.EndDate < endDate).Where(x => x.IsRemovedFromCalendar == false).Where(x => x.Groups.Count == 0).ToList();
                }
                else
                {
                    var groups = MemberCache.GetLeagueGroupsOfMember(memId);
                    var groupTemp = groups.Where(x => x.Id == group).FirstOrDefault();
                    if (groupTemp != null)
                        foreach (var mem in groupTemp.GroupMembers)
                        {
                            MemberDisplay m = new MemberDisplay();
                            m.DerbyName = mem.DerbyName;
                            m.Firstname = mem.Firstname;
                            m.LastName = mem.LastName;
                            m.MemberId = mem.MemberId;
                            m.PlayerNumber = mem.PlayerNumber;
                            members.Add(m);
                        }
                    if (reportOnGroupedEventsOnly)
                        events = cal.CalendarEvents.Where(x => x.StartDate >= startDate).Where(x => x.EndDate < endDate).Where(x => x.IsRemovedFromCalendar == false).Where(x => x.Groups.Select(y => y.Group.Id).Contains(group)).ToList();
                    else
                        events = cal.CalendarEvents.Where(x => x.StartDate >= startDate).Where(x => x.EndDate < endDate).Where(x => x.IsRemovedFromCalendar == false).ToList();

                }
                foreach (var calEvent in events)
                {
                    int eventTypeOccured = 0;
                    EventForReport blah = new EventForReport();
                    blah.AllowSelfCheckIn = calEvent.AllowSelfCheckIn;
                    blah.CalendarItemId = calEvent.CalendarItemId;
                    blah.Name = calEvent.Name;
                    blah.Notes = calEvent.Notes;
                    if (!calEvent.IsInUTCTime)
                    {
                        blah.StartDate = calEvent.StartDate;
                        blah.EndDate = calEvent.EndDate;
                    }
                    else
                    {
                        blah.StartDate = calEvent.StartDate + new TimeSpan(cal.TimeZone, 0, 0);
                        blah.EndDate = calEvent.EndDate + new TimeSpan(cal.TimeZone, 0, 0);
                    }
                    blah.TimeSpan = (blah.EndDate - blah.StartDate).Ticks;

                    if (calEvent.EventType != null)
                    {
                        blah.EventType.EventTypeName = calEvent.EventType.EventTypeName;
                        blah.EventType.PointsForExcused = calEvent.EventType.PointsForExcused;
                        blah.EventType.PointsForNotPresent = calEvent.EventType.PointsForNotPresent;
                        blah.EventType.PointsForPartial = calEvent.EventType.PointsForPartial;
                        blah.EventType.PointsForPresent = calEvent.EventType.PointsForPresent;
                        blah.EventType.PointsForTardy = calEvent.EventType.PointsForTardy;
                        blah.EventType.CalendarEventTypeId = calEvent.EventType.CalendarEventTypeId;
                        var eventTypeReporting = report.EventTypesReport.Where(x => x.CalendarEventTypeId == calEvent.EventType.CalendarEventTypeId).FirstOrDefault();
                        if (eventTypeReporting == null)
                        {
                            EventTypeForReport evForReport = new EventTypeForReport();
                            evForReport.EventTypeName = calEvent.EventType.EventTypeName;
                            evForReport.PointsForExcused = calEvent.EventType.PointsForExcused;
                            evForReport.PointsForNotPresent = calEvent.EventType.PointsForNotPresent;
                            evForReport.PointsForPartial = calEvent.EventType.PointsForPartial;
                            evForReport.PointsForPresent = calEvent.EventType.PointsForPresent;
                            evForReport.PointsForTardy = calEvent.EventType.PointsForTardy;
                            evForReport.CalendarEventTypeId = calEvent.EventType.CalendarEventTypeId;
                            evForReport.TotalTimesEventTypeOccured = 1;
                            eventTypeOccured = evForReport.TotalTimesEventTypeOccured;
                            report.EventTypesReport.Add(evForReport);
                        }
                        else
                        {
                            eventTypeReporting.TotalTimesEventTypeOccured += 1;
                            eventTypeOccured = eventTypeReporting.TotalTimesEventTypeOccured;
                        }
                    }

                    if (report.NotPresentForCheckIn == false)
                    {
                        SaveAttendeesWithNotPresentCheckIn(members, report, calEvent, blah);
                    }
                    else
                    {
                        SaveAttendeesWithNotPresentNeeded(members, report, calEvent, blah);
                    }
                    report.Events.Add(blah);
                }

                report.Attendees = report.Attendees.OrderBy(x => x.MemberName).ToList();
                report.TotalEventsCount = report.Events.Count;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return report;
        }
        /// <summary>
        /// used for events that require a Not Present Check in to count towards attendance percentage.
        /// Cyces through all those members that had some sort of marking on their attendance for the event. 
        /// 
        /// Not Efficient
        /// Needs to be Mirrored with SaveAttendeesWithNotPresentNeeded
        /// </summary>
        /// <param name="report"></param>
        /// <param name="calEvent"></param>
        /// <param name="blah"></param>
        private static void SaveAttendeesWithNotPresentCheckIn(List<MemberDisplay> members, CalendarReport report, DataModels.Calendar.CalendarEvent calEvent, EventForReport blah)
        {
            var attendees = calEvent.Attendees.Where(x => x.PointTypeEnum != (int)CalendarEventPointTypeEnum.None).ToList();
            int eventTypeCount = report.EventTypesReport.Where(x => x.CalendarEventTypeId == blah.EventType.CalendarEventTypeId).Sum(x => x.TotalTimesEventTypeOccured);
            foreach (var att in attendees)
            {
                // the attendee has to be in the list of members else we don't report on them.
                if (members.Select(x => x.MemberId).Contains(att.Attendant.MemberId))
                {
                    MembersReport nAtt = new MembersReport();
                    nAtt.MemberId = att.Attendant.MemberId;
                    nAtt.MemberName = att.Attendant.DerbyName;
                    nAtt.FullName = att.Attendant.Firstname + " " + att.Attendant.Lastname;
                    nAtt.MemberNumber = att.Attendant.PlayerNumber;
                    nAtt.PointType = (CalendarEventPointTypeEnum)Enum.Parse(typeof(CalendarEventPointTypeEnum), att.PointTypeEnum.ToString());
                    nAtt.SecondaryPointType = (CalendarEventPointTypeEnum)Enum.Parse(typeof(CalendarEventPointTypeEnum), att.SecondaryPointTypeEnum.ToString());
                    nAtt.Availability = (AvailibilityEnum)Enum.Parse(typeof(AvailibilityEnum), att.AvailibityEnum.ToString());
                    nAtt.AvailableNotes = att.AvailabilityNote;
                    nAtt.Note = att.Note;
                    nAtt.AdditionalPoints = att.AdditionalPoints;
                    //helps debug
                    //if (nAtt.MemberName == "Ann G")
                    //    nAtt.MemberName = att.Attendant.DerbyName;
                    //add points to this attendee thinking that they are not already in the list.
                    var evTp = new EventTypeForReport();
                    evTp.PointsForExcused = blah.EventType.PointsForExcused;
                    evTp.PointsForNotPresent = blah.EventType.PointsForNotPresent;
                    evTp.PointsForPartial = blah.EventType.PointsForPartial;
                    evTp.PointsForPresent = blah.EventType.PointsForPresent;
                    evTp.PointsForTardy = blah.EventType.PointsForTardy;

                    evTp.EventTypeName = blah.EventType.EventTypeName;
                    evTp.CalendarEventTypeId = blah.EventType.CalendarEventTypeId;
                    //the first points added to the attendee.
                    if (nAtt.PointType == CalendarEventPointTypeEnum.Excused)
                    {
                        nAtt.TotalPoints += evTp.PointsForExcused + nAtt.AdditionalPoints;
                        evTp.TotalPointsAccruedForType += evTp.PointsForExcused + nAtt.AdditionalPoints;
                        nAtt.PointsForEvent += evTp.PointsForExcused + nAtt.AdditionalPoints;
                        evTp.TotalTimesBeenExcused += 1;
                    }
                    else if (nAtt.PointType == CalendarEventPointTypeEnum.Not_Present)
                    {
                        nAtt.TotalPoints += evTp.PointsForNotPresent + nAtt.AdditionalPoints;
                        evTp.TotalPointsAccruedForType += evTp.PointsForNotPresent + nAtt.AdditionalPoints;
                        nAtt.PointsForEvent += evTp.PointsForNotPresent + nAtt.AdditionalPoints;
                        evTp.TotalTimesBeenAbsent += 1;
                    }
                    else if (nAtt.PointType == CalendarEventPointTypeEnum.Partial)
                    {
                        evTp.TotalTimesAttendedEventType = 1;
                        evTp.TotalHoursAttendedEventType = blah.TimeSpan;
                        nAtt.TotalHoursAttendedEventType = blah.TimeSpan;
                        nAtt.TotalPoints += evTp.PointsForPartial + nAtt.AdditionalPoints;
                        evTp.TotalPointsAccruedForType += evTp.PointsForPartial + nAtt.AdditionalPoints;
                        nAtt.PointsForEvent += evTp.PointsForPartial + nAtt.AdditionalPoints;
                    }
                    else if (nAtt.PointType == CalendarEventPointTypeEnum.Present)
                    {
                        nAtt.TotalPoints += evTp.PointsForPresent + nAtt.AdditionalPoints;
                        evTp.TotalPointsAccruedForType += evTp.PointsForPresent + nAtt.AdditionalPoints;
                        evTp.TotalPointsPossible += evTp.PointsForPresent;
                        nAtt.PointsForEvent += evTp.PointsForPresent + nAtt.AdditionalPoints;
                        evTp.TotalTimesAttendedEventType = 1;
                        evTp.TotalHoursAttendedEventType = blah.TimeSpan;
                        nAtt.TotalHoursAttendedEventType = blah.TimeSpan;
                    }
                    if (nAtt.SecondaryPointType == CalendarEventPointTypeEnum.Tardy)
                    {
                        nAtt.TotalPoints += evTp.PointsForTardy;
                        evTp.TotalPointsAccruedForType += evTp.PointsForTardy;
                        evTp.TotalTimesBeenTardy += 1;
                        nAtt.PointsForEvent += evTp.PointsForTardy;
                        //evTp.TotalTimesAttendedEventType = 1;
                    }

                    //this adds the events attended to this persons list.
                    nAtt.EventsAttended.Add(blah);
                    nAtt.EventTypes.Add(evTp);
                    //adds the attendee to the event.
                    blah.Attendees.Add(nAtt);
                    var getEventTypeForReport = report.EventTypesReport.Where(x => x.CalendarEventTypeId == evTp.CalendarEventTypeId).FirstOrDefault();
                    if (getEventTypeForReport != null)
                    {
                        getEventTypeForReport.TotalMembersAttendedEventType += 1;
                    }
                    //if the attendee doesn't already exist in the list of attendees.
                    var attend = report.Attendees.Where(x => x.MemberId == att.Attendant.MemberId).FirstOrDefault();
                    if (attend == null)
                    {
                        report.Attendees.Add(nAtt);
                    }
                    else
                    { //attendee exists
                        var eventType = attend.EventTypes.Where(x => x.CalendarEventTypeId == blah.EventType.CalendarEventTypeId).FirstOrDefault();
                        //this adds the events attended to this persons list.
                        attend.EventsAttended.Add(blah);
                        //if the event type doesn't actually exist in the list of events teh attendee has been to.
                        if (eventType == null)
                        {
                            if (nAtt.PointType == CalendarEventPointTypeEnum.Excused)
                            {
                                attend.TotalPoints += evTp.PointsForExcused + nAtt.AdditionalPoints;
                            }
                            else if (nAtt.PointType == CalendarEventPointTypeEnum.Not_Present)
                            {
                                attend.TotalPoints += evTp.PointsForNotPresent + nAtt.AdditionalPoints;
                            }
                            else if (nAtt.PointType == CalendarEventPointTypeEnum.Partial)
                            {
                                attend.TotalPoints += evTp.PointsForPartial + nAtt.AdditionalPoints;
                            }
                            else if (nAtt.PointType == CalendarEventPointTypeEnum.Present)
                            {
                                attend.TotalPoints += evTp.PointsForPresent + nAtt.AdditionalPoints;
                            }
                            if (nAtt.SecondaryPointType == CalendarEventPointTypeEnum.Tardy)
                            {
                                attend.TotalPoints += evTp.PointsForTardy;
                            }

                            evTp.TotalPointsPossibleForEventTypeFromAllEventsOfType = evTp.TotalPointsPossible * eventTypeCount;

                            if (evTp.TotalTimesAttendedEventType > 0 && eventTypeCount > 0)
                            {
                                evTp.TotalAllPointsPossiblePercentage = evTp.TotalTimesAttendedEventType / Convert.ToDecimal(eventTypeCount) * 100;
                                evTp.TotalCountedPointsPossiblePercentage = evTp.TotalTimesAttendedEventType / Convert.ToDecimal(eventTypeCount - evTp.TotalTimesBeenExcused) * 100;
                            }
                            else
                            {
                                evTp.TotalAllPointsPossiblePercentage = 0;
                                evTp.TotalCountedPointsPossiblePercentage = 0;
                            }
                            attend.EventTypes.Add(evTp);
                        }
                        else
                        { //if the event type exists in the attendees list of events already.
                            if (nAtt.PointType == CalendarEventPointTypeEnum.Excused)
                            {
                                attend.TotalPoints += eventType.PointsForExcused + nAtt.AdditionalPoints;
                                eventType.TotalPointsAccruedForType += eventType.PointsForExcused + nAtt.AdditionalPoints;
                                eventType.TotalTimesBeenExcused += 1;
                            }
                            else if (nAtt.PointType == CalendarEventPointTypeEnum.Not_Present)
                            {
                                attend.TotalPoints += eventType.PointsForNotPresent + nAtt.AdditionalPoints;
                                eventType.TotalPointsAccruedForType += eventType.PointsForNotPresent + nAtt.AdditionalPoints;
                                eventType.TotalTimesBeenAbsent += 1;
                            }
                            else if (nAtt.PointType == CalendarEventPointTypeEnum.Partial)
                            {
                                attend.TotalPoints += eventType.PointsForPartial + nAtt.AdditionalPoints;
                                eventType.TotalPointsAccruedForType += eventType.PointsForPartial + nAtt.AdditionalPoints;
                                eventType.TotalTimesAttendedEventType += 1;
                                eventType.TotalHoursAttendedEventType += blah.TimeSpan;
                                attend.TotalHoursAttendedEventType += blah.TimeSpan;
                            }
                            else if (nAtt.PointType == CalendarEventPointTypeEnum.Present)
                            {
                                attend.TotalPoints += eventType.PointsForPresent + nAtt.AdditionalPoints;
                                eventType.TotalPointsAccruedForType += eventType.PointsForPresent + nAtt.AdditionalPoints;
                                eventType.TotalTimesAttendedEventType += 1;
                                eventType.TotalHoursAttendedEventType += blah.TimeSpan;
                                attend.TotalHoursAttendedEventType += blah.TimeSpan;
                            }
                            if (nAtt.SecondaryPointType == CalendarEventPointTypeEnum.Tardy)
                            {
                                attend.TotalPoints += eventType.PointsForTardy;
                                eventType.TotalPointsAccruedForType += eventType.PointsForTardy;
                                eventType.TotalTimesBeenTardy += 1;
                                //eventType.TotalTimesAttendedEventType += 1;
                            }
                            eventType.TotalPointsPossibleForEventTypeFromAllEventsOfType = evTp.TotalPointsPossible * eventTypeCount;

                            //if (attend.MemberName == "Velocirapture" || attend.MemberName == "Slamtana Lopez")
                            //{ }
                            if (eventType.TotalTimesAttendedEventType > 0 && eventTypeCount > 0)
                            {
                                eventType.TotalAllPointsPossiblePercentage = eventType.TotalTimesAttendedEventType / (Convert.ToDecimal(eventTypeCount)) * 100;
                                eventType.TotalCountedPointsPossiblePercentage = eventType.TotalTimesAttendedEventType / (Convert.ToDecimal(eventTypeCount - evTp.TotalTimesBeenExcused)) * 100;
                            }
                            else
                            {
                                eventType.TotalAllPointsPossiblePercentage = 0;
                                eventType.TotalCountedPointsPossiblePercentage = 0;

                            }
                        }
                    }
                }

            }
        }
        /// <summary>
        /// for the attendees of a report where Not Present attendance checkin isn't needed
        /// to adjust attendance percentage.
        /// cycles through all the members of the league.  So that we can check who attended and who didn't.
        /// Not Efficient.
        /// Needs to be Mirrored with SaveAttendeesWithNotPresentCheckIn
        /// 
        /// for each event, we cycle through all the members.
        /// </summary>
        /// <param name="report"></param>
        /// <param name="calEvent"></param>
        /// <param name="blah"></param>
        private static void SaveAttendeesWithNotPresentNeeded(List<MemberDisplay> members, CalendarReport report, DataModels.Calendar.CalendarEvent calEvent, EventForReport blah)
        {
            int eventTypeCount = report.EventTypesReport.Where(x => x.CalendarEventTypeId == blah.EventType.CalendarEventTypeId).Sum(x => x.TotalTimesEventTypeOccured);
            foreach (var mem in members)
            {

                MembersReport nAtt = new MembersReport();
                var attendant = calEvent.Attendees.Where(x => x.Attendant.MemberId == mem.MemberId).FirstOrDefault();
                nAtt.MemberId = mem.MemberId;
                nAtt.MemberName = mem.DerbyName;
                nAtt.FullName = mem.Firstname + " " + mem.LastName;
                nAtt.MemberNumber = mem.PlayerNumber;

                if (attendant != null)
                {
                    nAtt.Note = attendant.Note;
                    nAtt.AdditionalPoints = attendant.AdditionalPoints;
                    nAtt.PointType = (CalendarEventPointTypeEnum)Enum.Parse(typeof(CalendarEventPointTypeEnum), attendant.PointTypeEnum.ToString());
                    nAtt.SecondaryPointType = (CalendarEventPointTypeEnum)Enum.Parse(typeof(CalendarEventPointTypeEnum), attendant.SecondaryPointTypeEnum.ToString());
                    nAtt.Availability = (AvailibilityEnum)Enum.Parse(typeof(AvailibilityEnum), attendant.AvailibityEnum.ToString());
                }
                else
                {
                    nAtt.PointType = CalendarEventPointTypeEnum.Not_Present;
                }

                //helps debug
                //if (nAtt.MemberName == "Ann G")
                //    nAtt.MemberName = att.Attendant.DerbyName;
                var evTp = new EventTypeForReport();
                evTp.PointsForExcused = blah.EventType.PointsForExcused;
                evTp.PointsForNotPresent = blah.EventType.PointsForNotPresent;
                evTp.PointsForPartial = blah.EventType.PointsForPartial;
                evTp.PointsForPresent = blah.EventType.PointsForPresent;
                evTp.PointsForTardy = blah.EventType.PointsForTardy;

                evTp.EventTypeName = blah.EventType.EventTypeName;
                evTp.CalendarEventTypeId = blah.EventType.CalendarEventTypeId;

                if (nAtt.PointType == CalendarEventPointTypeEnum.Excused)
                {
                    nAtt.TotalPoints += evTp.PointsForExcused + nAtt.AdditionalPoints;
                    evTp.TotalPointsAccruedForType += evTp.PointsForExcused + nAtt.AdditionalPoints;
                    nAtt.PointsForEvent += evTp.PointsForExcused + nAtt.AdditionalPoints;
                    evTp.TotalTimesBeenExcused += 1;
                }
                else if (nAtt.PointType == CalendarEventPointTypeEnum.Not_Present)
                {
                    nAtt.TotalPoints += evTp.PointsForNotPresent + nAtt.AdditionalPoints;
                    evTp.TotalPointsAccruedForType += evTp.PointsForNotPresent + nAtt.AdditionalPoints;
                    nAtt.PointsForEvent += evTp.PointsForNotPresent + nAtt.AdditionalPoints;
                    evTp.TotalTimesBeenAbsent += 1;
                }
                else if (nAtt.PointType == CalendarEventPointTypeEnum.Partial)
                {
                    evTp.TotalHoursAttendedEventType = blah.TimeSpan;
                    nAtt.TotalHoursAttendedEventType = blah.TimeSpan;
                    evTp.TotalTimesAttendedEventType = 1;
                    nAtt.TotalPoints += evTp.PointsForPartial + nAtt.AdditionalPoints;
                    evTp.TotalPointsAccruedForType += evTp.PointsForPartial + nAtt.AdditionalPoints;
                    nAtt.PointsForEvent += evTp.PointsForPartial + nAtt.AdditionalPoints;
                }
                else if (nAtt.PointType == CalendarEventPointTypeEnum.Present)
                {
                    nAtt.TotalPoints += evTp.PointsForPresent + nAtt.AdditionalPoints;
                    evTp.TotalPointsAccruedForType += evTp.PointsForPresent + nAtt.AdditionalPoints;
                    evTp.TotalPointsPossible += evTp.PointsForPresent;
                    nAtt.PointsForEvent += evTp.PointsForPresent + nAtt.AdditionalPoints;
                    evTp.TotalTimesAttendedEventType = 1;
                    evTp.TotalHoursAttendedEventType = blah.TimeSpan;
                    nAtt.TotalHoursAttendedEventType = blah.TimeSpan;
                }
                if (nAtt.SecondaryPointType == CalendarEventPointTypeEnum.Tardy)
                {
                    nAtt.TotalPoints += evTp.PointsForTardy;
                    evTp.TotalPointsAccruedForType += evTp.PointsForTardy;
                    evTp.TotalTimesBeenTardy += 1;
                    nAtt.PointsForEvent += evTp.PointsForTardy;
                    //evTp.TotalTimesAttendedEventType = 1;
                }



                //this adds the events attended to this persons list.
                nAtt.EventsAttended.Add(blah);
                nAtt.EventTypes.Add(evTp);
                //adds the attendee to the event.
                blah.Attendees.Add(nAtt);
                var getEventTypeForReport = report.EventTypesReport.Where(x => x.CalendarEventTypeId == evTp.CalendarEventTypeId).FirstOrDefault();
                if (getEventTypeForReport != null)
                {
                    getEventTypeForReport.TotalMembersAttendedEventType += 1;
                }
                //if the attendee doesn't already exist in the list of attendees.
                //we add the attendee plus the points.
                var attend = report.Attendees.Where(x => x.MemberId == mem.MemberId).FirstOrDefault();
                if (attend == null)
                {
                    report.Attendees.Add(nAtt);
                }
                else
                { //attendee exists, so we just need to add the points already earned by the attendee.
                    var eventType = attend.EventTypes.Where(x => x.CalendarEventTypeId == blah.EventType.CalendarEventTypeId).FirstOrDefault();
                    //this adds the events attended to this persons list.
                    attend.EventsAttended.Add(blah);
                    //if the event type doesn't actually exist in the list of events teh attendee has been to.
                    if (eventType == null)
                    {
                        if (nAtt.PointType == CalendarEventPointTypeEnum.Excused)
                        {
                            attend.TotalPoints += evTp.PointsForExcused + nAtt.AdditionalPoints;
                        }
                        else if (nAtt.PointType == CalendarEventPointTypeEnum.Not_Present)
                        {
                            attend.TotalPoints += evTp.PointsForNotPresent + nAtt.AdditionalPoints;
                        }
                        else if (nAtt.PointType == CalendarEventPointTypeEnum.Partial)
                        {
                            attend.TotalPoints += evTp.PointsForPartial + nAtt.AdditionalPoints;
                        }
                        else if (nAtt.PointType == CalendarEventPointTypeEnum.Present)
                        {
                            attend.TotalPoints += evTp.PointsForPresent + nAtt.AdditionalPoints;
                        }
                        if (nAtt.SecondaryPointType == CalendarEventPointTypeEnum.Tardy)
                        {
                            attend.TotalPoints += evTp.PointsForTardy;
                        }
                        evTp.TotalPointsPossibleForEventTypeFromAllEventsOfType = evTp.TotalPointsPossible * eventTypeCount;
                        if (evTp.TotalTimesAttendedEventType > 0 && eventTypeCount != 0)
                        {
                            evTp.TotalAllPointsPossiblePercentage = evTp.TotalTimesAttendedEventType / Convert.ToDecimal(eventTypeCount) * 100;
                            evTp.TotalCountedPointsPossiblePercentage = evTp.TotalTimesAttendedEventType / Convert.ToDecimal(eventTypeCount - evTp.TotalTimesBeenExcused) * 100;
                        }
                        else
                        {
                            evTp.TotalAllPointsPossiblePercentage = 0;
                            evTp.TotalCountedPointsPossiblePercentage = 0;

                        }
                        attend.EventTypes.Add(evTp);

                    }
                    else
                    {
                        if (nAtt.PointType == CalendarEventPointTypeEnum.Excused)
                        {
                            attend.TotalPoints += eventType.PointsForExcused + nAtt.AdditionalPoints;
                            eventType.TotalPointsAccruedForType += eventType.PointsForExcused + nAtt.AdditionalPoints;
                            eventType.TotalTimesBeenExcused += 1;
                        }
                        else if (nAtt.PointType == CalendarEventPointTypeEnum.Not_Present)
                        {
                            attend.TotalPoints += eventType.PointsForNotPresent + nAtt.AdditionalPoints;
                            eventType.TotalPointsAccruedForType += eventType.PointsForNotPresent + nAtt.AdditionalPoints;
                            eventType.TotalTimesBeenAbsent += 1;
                        }
                        else if (nAtt.PointType == CalendarEventPointTypeEnum.Partial)
                        {
                            attend.TotalPoints += eventType.PointsForPartial + nAtt.AdditionalPoints;
                            eventType.TotalPointsAccruedForType += eventType.PointsForPartial + nAtt.AdditionalPoints;
                            eventType.TotalTimesAttendedEventType += 1;
                            eventType.TotalHoursAttendedEventType += blah.TimeSpan;
                            attend.TotalHoursAttendedEventType += blah.TimeSpan;
                        }
                        else if (nAtt.PointType == CalendarEventPointTypeEnum.Present)
                        {
                            attend.TotalPoints += eventType.PointsForPresent + nAtt.AdditionalPoints;
                            eventType.TotalPointsAccruedForType += eventType.PointsForPresent + nAtt.AdditionalPoints;
                            eventType.TotalTimesAttendedEventType += 1;
                            eventType.TotalHoursAttendedEventType += blah.TimeSpan;
                            attend.TotalHoursAttendedEventType += blah.TimeSpan;
                        }
                        if (nAtt.SecondaryPointType == CalendarEventPointTypeEnum.Tardy)
                        {
                            attend.TotalPoints += eventType.PointsForTardy;
                            eventType.TotalPointsAccruedForType += eventType.PointsForTardy;
                            eventType.TotalTimesBeenTardy += 1;
                            //eventType.TotalTimesAttendedEventType += 1;
                        }
                        if (attend.MemberName == "Velocirapture" || attend.MemberName == "Slamtana Lopez")
                        { }
                        eventType.TotalPointsPossibleForEventTypeFromAllEventsOfType = evTp.TotalPointsPossible * eventTypeCount;
                        if (eventType.TotalTimesAttendedEventType > 0 && eventTypeCount > 0)
                        {
                            eventType.TotalAllPointsPossiblePercentage = eventType.TotalTimesAttendedEventType / Convert.ToDecimal(eventTypeCount) * 100;
                            eventType.TotalCountedPointsPossiblePercentage = eventType.TotalTimesAttendedEventType / Convert.ToDecimal(eventTypeCount - evTp.TotalTimesBeenExcused) * 100;
                        }
                        else
                        {
                            eventType.TotalAllPointsPossiblePercentage = 0;
                            eventType.TotalCountedPointsPossiblePercentage = 0;
                        }
                    }
                }
            }
        }
    }
}
