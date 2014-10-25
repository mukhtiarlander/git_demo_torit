using RDN.Portable.Classes.Controls.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Calendar.Reports
{
    public class MembersReport : CalendarAttendance
    {
        public List<EventTypeForReport> EventTypes { get; set; }
        public List<Report.EventForReport> EventsAttended { get; set; }

        public MembersReport()
        {
            EventTypes = new List<EventTypeForReport>();
            EventsAttended = new List<Report.EventForReport>();
        }
    }
}
