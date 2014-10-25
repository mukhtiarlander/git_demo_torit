using RDN.Library.Classes.Calendar.Enums;
using RDN.Portable.Classes.Controls.Calendar.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Controls.Calendar.Models
{
    public class CalendarImport
    {
        public string GoogleCalendarUrl { get; set; }
        public CalendarImportTypeEnum ImportType { get; set; }
        public Guid CalendarId { get; set; }
        public CalendarOwnerEntityEnum OwnerEntity { get; set; }
        /// <summary>
        /// the offset of the timezone
        /// </summary>
        public int TimeZone { get; set; }
        /// <summary>
        /// the chosen timezone.
        /// </summary>
        public int TimeZoneId { get; set; }
        public List<RDN.Portable.Classes.Location.TimeZone> TimeZones { get; set; }
    }
}
