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
    public class DuesRequired
    {
        [ProtoMember(1)]
        [DataMember]
        public long DuesRequiredId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string Note { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public double DuesRequire { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public DateTime RequiredDate { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public bool IsPaidInFull { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public bool IsWaived { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public Guid MemberRequiredId { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public string MemberRequiredName { get; set; }
    }
}
