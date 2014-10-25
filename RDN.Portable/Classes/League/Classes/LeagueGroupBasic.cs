using ProtoBuf;
using RDN.Portable.Classes.League.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.League
{
    [ProtoContract]
    [DataContract]
    [ProtoInclude(100, typeof(LeagueGroup))]
    public class LeagueGroupBasic
    {
        [ProtoMember(1)]
        [DataMember]
        public long Id { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string GroupName { get; set; }
    }
}
