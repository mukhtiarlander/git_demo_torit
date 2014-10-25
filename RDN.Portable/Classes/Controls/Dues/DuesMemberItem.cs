using ProtoBuf;
using RDN.Portable.Classes.Account.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Dues
{
    [ProtoContract]
    [DataContract]
    public class DuesMemberItem
    {
        [ProtoMember(1)]
        [DataMember]
        public string DuesCostDisplay { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public double DuesCost { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public Guid DuesId { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public Guid OwnerId { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public string OwnerEntity { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public MemberDisplayBasic Member { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public DuesItem DuesItem { get; set; }
        /// <summary>
        /// classification of what group the member belongs to to pay dues.
        /// </summary>
        [ProtoMember(8)]
        [DataMember]
        public long ClassificationId { get; set; }

        [ProtoMember(9)]
        [DataMember]
        public bool IsSuccessful { get; set; }

        public DuesMemberItem()
        {
            Member = new MemberDisplayBasic();
            DuesItem = new DuesItem();
        }
    }
}
