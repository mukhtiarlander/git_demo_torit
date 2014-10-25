using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RDN.Library.Classes.Calendar;
using RDN.Library.Classes.Payment.Paywall;
using Scoreboard.Library.ViewModel;
using RDN.Portable.Classes.Controls.Calendar;
using RDN.Library.Classes.Controls.Calendar;

namespace RDN.Library.Classes.Calendar.Models
{
    public class EventsForLeagueOutModel
    {
        public List<CalendarEvent> Events { get; set; }
        
        public Guid  LeagueId{ get; set; }
        public DateTime StartDate { get; set; }
        public EventsForLeagueOutModel()
        {
            Events = new List<CalendarEvent>();
        }
    }
}