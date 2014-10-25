using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Calendar
{
    /// <summary>
    /// this is the type of event for each event.
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class CalendarEventType
    {
        [ProtoMember(1)]
        [DataMember]
        public CalendarEventTypeEnum EventType { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public long CalendarEventTypeId { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string EventTypeName { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public int PointsForPresent { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public int PointsForPartial { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public int PointsForNotPresent { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public int PointsForExcused { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public int PointsForTardy { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public Guid CalendarId { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public string OwnerEntity { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public string ColorTempSelected { get; set; }
        [ProtoMember(12)]
        [DataMember]
        public string ColorName { get; set; }


        [ProtoMember(13)]
        [DataMember]
        public Guid CurrentMemberId { get; set; }
        [ProtoMember(14)]
        [DataMember]
        public Guid UserId { get; set; }
        [ProtoMember(15)]
        [DataMember]
        public bool IsSuccessful { get; set; }

    }
}
