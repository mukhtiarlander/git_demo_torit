using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Calendar
{
    /// <summary>
    /// used to capture all reccurring events, so that we have a template to generate real events from.
    /// </summary>
    [Table("RDN_Calendar_Item_Reoccuring")]
    public class CalendarEventReoccuring : InheritDb
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
        /// we didn't use the timezone in the beginging, so we had dates
        /// without the time zone modifiers and with.  So we use this to check if this date
        /// is in UTC time or local time.
        /// </summary>
        public bool IsInUTCTime { get; set; }
       
        /// <summary>
        /// if True, allows the skater to check them selves into events.
        /// </summary>
        public bool AllowSelfCheckIn { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        public string Notes { get; set; }
        public string Link { get; set; }

        public virtual Location.Location Location { get; set; }

        //used for the Recorrurring events
        public int FrequencyReocurring { get; set; }
        public int DaysOfWeekReocurring { get; set; }
        public int MonthlyIntervalReocurring { get; set; }
        public DateTime StartReocurring { get; set; }
        public DateTime? EndReocurring { get; set; }

        public DateTime? LastDateEventsWereCreated { get; set; }

        public bool IsRemovedFromCalendar { get; set; }
        public bool IsPublic { get; set; }

        public string TicketUrl { get; set; }
       
        [Required]
        public virtual Calendar Calendar { get; set; }
        public virtual CalendarEventType EventType { get; set; }
        public virtual ICollection<CalendarEventPoint> PointsForEvent { get; set; }
        public virtual ICollection<CalendarEvent> ReoccuringEvents { get; set; }
        public virtual Color.Color Color { get; set; }
        public virtual ICollection<CalendarEventReoccuringGroup> Groups { get; set; }

        public CalendarEventReoccuring()
        {
            ReoccuringEvents = new Collection<CalendarEvent>();
            PointsForEvent = new Collection<CalendarEventPoint>();
            Groups = new Collection<CalendarEventReoccuringGroup>();
        }
    }
}
