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
    public class VotesClass
    {
        [ProtoMember(1)]
        [DataMember]
        public long VoteId { get; set; }

        [ProtoMember(2)]
        [DataMember]
        public string OtherText { get; set; }

        [ProtoMember(3)]
        [DataMember]
        public bool HasVoted { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string IPAddress { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public long AnswerId { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public List<long> AnswerIds { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public Guid MemberId { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public string DerbyName { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public Guid UserId { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public DateTime Created { get; set; }
        public VotesClass()
        {
            AnswerIds = new List<long>();
        }
    }
}
