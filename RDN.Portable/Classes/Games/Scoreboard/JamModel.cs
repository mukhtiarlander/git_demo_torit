using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Games.Scoreboard
{
    [ProtoContract]
    [DataContract]
    public class JamModel
    {
        [ProtoMember(1)]
        [DataMember]
        public int JamNumber { get; set; }

        [ProtoMember(2)]
        [DataMember]
        public TeamMembersModel PivotT1 { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public TeamMembersModel PivotT2 { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public TeamMembersModel JammerT1 { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public TeamMembersModel Blocker3T1 { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public TeamMembersModel Blocker4T1 { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public TeamMembersModel Blocker2T1 { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public TeamMembersModel Blocker1T1 { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public long GameTimeElapsedMillisecondsStart { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public long GameTimeElapsedMillisecondsEnd { get; set; }
    }
}
