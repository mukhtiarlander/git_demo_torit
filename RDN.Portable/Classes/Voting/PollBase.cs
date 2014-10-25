using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Voting
{
    [ProtoContract]
    [DataContract]
    public class PollBase
    {
        [ProtoMember(1)]
        [DataMember]
        public string LeagueName { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public Guid LeagueId { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public List<VotingClass> Polls { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public bool IsPollManager { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public bool IsSuccessful { get; set; }

        public PollBase()
        {
            Polls = new List<VotingClass>();
        }
    }
}
