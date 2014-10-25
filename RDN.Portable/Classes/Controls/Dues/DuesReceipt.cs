using ProtoBuf;
using RDN.Portable.Classes.Billing.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Dues
{

    [ProtoContract]
    [DataContract]
    public class DuesReceipt
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid LeagueId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string LeagueName { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public Guid InvoiceId { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public DateTime PaidDuesForMonth { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public CurrentStatusOfBillingEnum MembershipStatus { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public decimal BasePrice { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public decimal Fees { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public decimal PriceAfterFees { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public string EmailForReceipt { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public string MemberPaid { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public bool IsSuccessful{ get; set; }
        
    }
}
