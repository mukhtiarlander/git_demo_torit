using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Calendar
{
    [ProtoContract]
    [DataContract]
    public class CalendarEventJson
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid id { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string title { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string backColor { get; set; }
        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string start { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public DateTime StartDate { get; set; }
        //end: new Date(y, m, d, 14, 0),
        [ProtoMember(6)]
        [DataMember]
        public string end { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public DateTime EndDate { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public string url { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public bool allDay { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public Guid ReocurringId { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public bool IsRemovedFromCalendar { get; set; }
    }
}
