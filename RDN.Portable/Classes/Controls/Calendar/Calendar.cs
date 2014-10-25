using ProtoBuf;
using RDN.Portable.Classes.Controls.Calendar.Enums;
using RDN.Portable.Classes.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Calendar
{
    [ProtoContract]
    [DataContract]
    public class Calendar
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid CalendarId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string EntityName { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string ImportFeedUrl { get; set; }
        /// <summary>
        /// the offset of the timezone
        /// </summary>
        [ProtoMember(4)]
        [DataMember]
        public int TimeZone { get; set; }
        /// <summary>
        /// the chosen timezone.
        /// </summary>
        [ProtoMember(5)]
        [DataMember]
        public int TimeZoneId { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public List<TimeZone> TimeZones { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public CalendarImportTypeEnum ImportFeedType { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public bool AllowSelfCheckIn { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public bool IsCalendarInUTC { get; set; }
        /// <summary>
        /// if this is set to True, there will be no need to set a skater as NOT PRESENT during an event to get the points
        /// awarded properly.  They will just recieve zero points for not attending.
        /// </summary>
        [ProtoMember(10)]
        [DataMember]
        public bool NotPresentCheckIn { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public bool DisableBirthdays { get; set; }
        [ProtoMember(12)]
        [DataMember]
        public bool DisableSkatingStartDates { get; set; }
        [ProtoMember(13)]
        [DataMember]
        public CalendarOwnerEntityEnum OwnerEntity { get; set; }
        [ProtoMember(14)]
        [DataMember]
        public List<CalendarOwner> Owners { get; set; }
        [ProtoMember(15)]
        [DataMember]
        public List<CalendarEventPortable> Events { get; set; }
        [ProtoMember(16)]
        [DataMember]
        public List<CalendarEventJson> EventsJson { get; set; }
        [ProtoMember(17)]
        [DataMember]
        public List<CalendarEventType> EventTypes { get; set; }
        [ProtoMember(18)]
        [DataMember]
        public DateTime CurrentDateSelected { get; set; }
        [ProtoMember(19)]
        [DataMember]
        public int DaysToNextEvent { get; set; }

        [ProtoMember(20)]
        [DataMember]
        public bool  IsSuccessful{ get; set; }

        [ProtoMember(21)]
        [DataMember]
        public Guid MemberId { get; set; }

        [ProtoMember(22)]
        [DataMember]
        public Guid UserId { get; set; }

        

        public Calendar()
        {
            EventTypes = new List<CalendarEventType>();
            Owners = new List<CalendarOwner>();
            Events = new List<CalendarEventPortable>();
            EventsJson = new List<CalendarEventJson>();
        }



    }
}
