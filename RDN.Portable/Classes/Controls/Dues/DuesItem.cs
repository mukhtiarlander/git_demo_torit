using ProtoBuf;
using RDN.Portable.Classes.Controls.Dues.Classify;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Dues
{
    [ProtoContract]
    [DataContract]
    [DebuggerDisplay("[{this.ClassificationName}]")]
    public class DuesItem
    {
        [ProtoMember(1)]
        [DataMember]
        public long DuesItemId { get; set; }

        [ProtoMember(2)]
        [DataMember]
        public double CostOfDues { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string ClassificationName { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public DateTime PayBy { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public double TotalPaid { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public double TotalWithstanding { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public List<DuesCollected> DuesCollected { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public List<DuesRequired> DuesRequired { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public List<FeeClassified> Classifications { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public bool IsCurrentMemberPaidOrWaivedInFull { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public double TotalPaymentNeededFromMember { get; set; }
        [ProtoMember(12)]
        [DataMember]
        public double CostOfDuesFromMember { get; set; }
        [ProtoMember(13)]
        [DataMember]
        public double TotalPaidFromMember { get; set; }
        [ProtoMember(14)]
        [DataMember]
        public string MemberClassificationName { get; set; }

        [ProtoMember(15)]
        [DataMember]
        public Guid DuesId{ get; set; }
        

        public DuesItem()
        {
            DuesCollected = new List<DuesCollected>();
            DuesRequired = new List<DuesRequired>();
            Classifications = new List<FeeClassified>();
        }
    }
}
