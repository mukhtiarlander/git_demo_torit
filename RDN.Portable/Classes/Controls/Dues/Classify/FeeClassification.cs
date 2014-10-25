using ProtoBuf;
using RDN.Portable.Classes.Account.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Dues.Classify
{
    [ProtoContract]
    [DataContract]
    public class FeeClassified
    {
        [ProtoMember(1)]
        [DataMember]
        public long FeeClassificationId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string Name { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public double FeeRequired { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string FeeRequiredInput { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public Guid DuesId { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public string OwnerEntity { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public Guid LeagueOwnerId { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public bool DoesNotPayDues { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public List<MemberDisplayBasic> MembersInClass { get; set; }

        public FeeClassified()
        {
            MembersInClass = new List<MemberDisplayBasic>();
        }

    }
}
