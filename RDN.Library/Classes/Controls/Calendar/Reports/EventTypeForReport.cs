using RDN.Portable.Classes.Controls.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Calendar.Reports
{
    public class EventTypeForReport : CalendarEventType
    {
        /// <summary>
        /// total points a person has accrued for this event type.
        /// </summary>
        public int TotalPointsAccruedForType { get; set; }
        /// <summary>
        /// total times a person has been tardy to this event.
        /// </summary>
        public int TotalTimesBeenTardy { get; set; }
        /// <summary>
        /// total time the person was absent
        /// </summary>
        public int TotalTimesBeenAbsent { get; set; }
        /// <summary>
        /// total time the person was excused
        /// </summary>
        public int TotalTimesBeenExcused { get; set; }
        /// <summary>
        /// total times a person has attended the event type.
        /// </summary>
        public decimal TotalTimesAttendedEventType { get; set; }
        public double TotalHoursAttendedEventType { get; set; }
        /// <summary>
        /// total time the event type occured when reporting.
        /// </summary>
        public int TotalTimesEventTypeOccured { get; set; }
        /// <summary>
        /// total points possible for event type.
        /// </summary>
        public int TotalPointsPossible { get; set; }
        /// <summary>
        /// total times this events occured under this type.
        /// </summary>
        public decimal TotalEventOccurencesForType { get; set; }
        /// <summary>
        /// total members attended this event type.  
        /// use this to calculate average attendance per event.
        /// </summary>
        public decimal TotalMembersAttendedEventType { get; set; }


        public decimal TotalAllPointsPossiblePercentage { get; set; }
        public decimal TotalCountedPointsPossiblePercentage { get; set; }
        /// <summary>
        /// total points possible after counting up all the events for that single event type.
        /// </summary>
        public decimal TotalPointsPossibleForEventTypeFromAllEventsOfType { get; set; }
    }
}
