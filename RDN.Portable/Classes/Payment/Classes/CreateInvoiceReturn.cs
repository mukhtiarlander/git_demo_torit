using ProtoBuf;
using RDN.Portable.Classes.Payment.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Payment.Classes
{
    [ProtoContract]
    [DataContract]
    public class CreateInvoiceReturn
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid InvoiceId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public InvoiceStatus Status { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string RedirectLink { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string Error { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public DateTime SubscriptionEndsOn { get; set; }
    }
}
