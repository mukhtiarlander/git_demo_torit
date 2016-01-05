using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Calendar
{
    [Table("RDN_Calendar")]
    public class Calendar : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CalendarId { get; set; }

        public int TimeZone { get; set; }
        
        public bool IsCalendarInUTC { get; set; }
        public byte CalendarImportTypeEnum { get; set; }
        /// <summary>
        /// this is the ICS url of the calendar.
        /// </summary>
        public string ImportFeedUrl { get; set; }
        /// <summary>
        /// if True, allows the skater to check them selves into events.
        /// </summary>
        public bool AllowSelfCheckIn { get; set; }
        /// <summary>
        /// When Enabled, leagues don't need to set a skater as "Not Present" in 
        /// order for their attendance percentage to be effected.
        /// </summary>
        public bool NotPresentCheckIn { get; set; }

        public bool DisableBirthdaysFromShowing { get; set; }
        public bool DisableStartSkatingDays { get; set; }

        public bool HideReport { get; set; }

        public virtual TimeZone.TimeZone TimeZoneSelection { get; set; }
        public virtual ICollection<CalendarEvent> CalendarEvents { get; set; }
        public virtual ICollection<CalendarEventReoccuring> CalendarEventsReocurring { get; set; }

        public virtual ICollection<CalendarMemberOwnership> MemberOwners { get; set; }
        public virtual ICollection<CalendarLeagueOwnership> LeagueOwners { get; set; }
        public virtual ICollection<CalendarFederationOwnership> FederationOwners { get; set; }
        public virtual ICollection<Location.Location> Locations { get; set; }

        public Calendar()
        {
            CalendarEventsReocurring = new Collection<CalendarEventReoccuring>();
            CalendarEvents = new Collection<CalendarEvent>();
            MemberOwners = new Collection<CalendarMemberOwnership>();
            LeagueOwners = new Collection<CalendarLeagueOwnership>();
            FederationOwners = new Collection<CalendarFederationOwnership>();
        }

    }
}
