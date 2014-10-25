using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Dues
{
    [ProtoContract]
    [DataContract]
    public class DuesCollected
    {
        [ProtoMember(1)]
        [DataMember]
        public long DuesCollectedId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string Note { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public double DuesPaid { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public DateTime PaidDate { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public bool IsPaidInFull { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public bool IsWaived { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public Guid MemberPaidId { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public string MemberPaidName { get; set; }
        /// <summary>
        /// if the item was actually cleared by someone, we need to make sure they pay.
        /// </summary>
        [ProtoMember(9)]
        [DataMember]
        public bool WasDuesClearedByUser { get; set; }
    }
}
