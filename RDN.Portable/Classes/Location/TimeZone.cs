using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Location
{

    [ProtoContract]
    [DataContract]
    public class TimeZone
    {
        [ProtoMember(1)]
        [DataMember]
        public int ZoneId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string Location { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string GMT { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string LocationGMT { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public int GMTOffset { get; set; }
    }
}
