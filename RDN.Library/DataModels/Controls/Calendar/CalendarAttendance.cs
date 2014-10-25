using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Calendar
{
    /// <summary>
    /// defines who is coming to the calendar event.
    /// </summary>
    [Table("RDN_Calendar_Attendance")]
    public class CalendarAttendance : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CalendarAttendanceId { get; set; }

        public string Note { get; set; }

        public int PointTypeEnum { get; set; }
        //used mostly for tardiness
        public int SecondaryPointTypeEnum { get; set; }

        public int AdditionalPoints { get; set; }

        public string AvailabilityNote { get; set; }
        public byte AvailibityEnum { get; set; }

        public virtual CalendarEvent CalendarItem { get; set; }
        //[Obsolete("Please Use Attendant Version 2")]
        public virtual Member.Member Attendant { get; set; }
        //public virtual League.LeagueMember Attendant2 { get; set; }

    }
}
