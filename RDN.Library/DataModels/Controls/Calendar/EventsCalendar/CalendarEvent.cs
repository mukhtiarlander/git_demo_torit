using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.Collections.ObjectModel;
using RDN.Library.DataModels.Calendar.EventsCalendar;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Calendar
{
    [Table("RDN_Calendar_Item")]
    public class CalendarEvent : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CalendarItemId { get; set; }
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// this is the id of the feed event we imported via a outside calendar.
        /// </summary>
        public string EventFeedId { get; set; }

        /// <summary>
        /// if True, allows the skater to check them selves into events.
        /// </summary>
        public bool AllowSelfCheckIn { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// we didn't use the timezone in the beginging, so we had dates
        /// without the time zone modifiers and with.  So we use this to check if this date
        /// is in UTC time or local time.
        /// </summary>
        public bool IsInUTCTime { get; set; }
       
        public bool IsRemovedFromCalendar { get; set; }

        public string Notes { get; set; }
        public string Link { get; set; }
        public bool IsPublicEvent { get; set; }
        public string TicketUrl { get; set; }
        
        public virtual Location.Location Location { get; set; }
        public virtual ICollection<EventCalendarConversation> EventConversations { get; set; }
        [Required]
        public virtual Calendar Calendar { get; set; }
        public virtual CalendarEventType EventType { get; set; }
        public virtual CalendarEventReoccuring ReocurringEvent { get; set; }
        public virtual ICollection<CalendarEventPoint> PointsForEvent { get; set; }
        public virtual ICollection<CalendarAttendance> Attendees { get; set; }
        public virtual Color.Color Color{get;set;}
        public virtual ICollection<CalendarEventGroup> Groups { get; set; }

        public CalendarEvent()
        {
            PointsForEvent = new Collection<CalendarEventPoint>();
            Attendees = new Collection<CalendarAttendance>();
            EventConversations = new Collection<EventCalendarConversation>();
            Groups = new Collection<CalendarEventGroup>();
        }
    }
}
