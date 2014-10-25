using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RDN.Library.Classes.Calendar;
using Scoreboard.Library.ViewModel;
using RDN.Portable.Classes.Controls.Calendar;
using RDN.Library.Classes.Controls.Calendar;

namespace RDN.Library.Classes.Calendar.Models
{
    public class EventsOutModel
    {
        public List<CalendarEvent> Events { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public EventsOutModel()
        {
            Events = new List<CalendarEvent>();
        }
    }
}