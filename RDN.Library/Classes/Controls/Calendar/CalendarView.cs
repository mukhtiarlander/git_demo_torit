using ScheduleWidget.ScheduledEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RDN.Library.Classes.Calendar
{
    /// <summary>
    /// this is for the jquery calendar we use to show the users.
    /// </summary>
    public class CalendarViewEventJson : Event
    {
        public Guid id { get; set; }
        public string title { get; set; }
        public string backColor { get; set; }
        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
        public string start { get; set; }
        public DateTime StartDate { get; set; }
        //end: new Date(y, m, d, 14, 0),
        public string end { get; set; }
        public DateTime EndDate { get; set; }
        public string url { get; set; }
        public bool allDay { get; set; }
        public Guid ReocurringId { get; set; }
        public bool IsRemovedFromCalendar { get; set; }
    }
}
