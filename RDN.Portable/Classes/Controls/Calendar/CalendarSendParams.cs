using ProtoBuf;
using RDN.Portable.Classes.Controls.Calendar.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Calendar
{
    [ProtoContract]
    [DataContract]
    public class CalendarSendParams
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid CurrentMemberId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public Guid UserId { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public Guid CalendarId { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public bool IsSuccessful { get; set; }

        [ProtoMember(5)]
        [DataMember]
        public int Year { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public int  Month{ get; set; }
        [ProtoMember(7)]
        [DataMember]
        public CalendarOwnerEntityEnum CalendarType { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public Guid EventId { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public Guid MemberId { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public CalendarEventPointTypeEnum PointType { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public int AddPoints { get; set; }
        [ProtoMember(12)]
        [DataMember]
        public string Note{ get; set; }
        [ProtoMember(13)]
        [DataMember]
        public bool IsTardy{ get; set; }
        [ProtoMember(14)]
        [DataMember]
        public AvailibilityEnum Availability{ get; set; }
        [ProtoMember(15)]
        [DataMember]
        public long EventTypeId{ get; set; }

        
    }
}
