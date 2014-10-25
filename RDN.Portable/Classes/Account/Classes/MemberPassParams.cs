using ProtoBuf;
using RDN.Portable.Classes.League.Classes;
using RDN.Portable.Classes.Voting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Account.Classes
{
    /// <summary>
    /// use this class solely to pass api secure parameters
    /// </summary>
    [ProtoContract]
    [DataContract]
    [ProtoInclude(200, typeof(VotingClass))]
    public class MemberPassParams
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid Uid { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public Guid Mid { get; set; }
        public LeagueBase Item { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public MemberDisplayEdit Member { get; set; }

        [ProtoMember(5)]
        [DataMember]
        public long IdOfAnySort { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public bool IsSuccessful { get; set; }

        [ProtoMember(7)]
        [DataMember]
        public string TextOfAnySort { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public Guid IdOfAnySort2 { get; set; }


    }
}
