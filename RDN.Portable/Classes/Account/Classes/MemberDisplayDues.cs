using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Account.Classes
{
    /// <summary>
    /// basic member, so we can pass JSON back and forth with the most important information.
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class MemberDisplayDues : MemberDisplayBasic
    {
        [ProtoMember(701)]
        [DataMember]
        public double due { get; set; }
        [ProtoMember(702)]
        [DataMember]
        public double collected { get; set; }
        [ProtoMember(703)]
        [DataMember]
        public bool isPaidFull { get; set; }
        [ProtoMember(704)]
        [DataMember]
        public bool isWaived { get; set; }
        [ProtoMember(705)]
        [DataMember]
        public double TotalWithstanding { get; set; }
        [ProtoMember(706)]
        [DataMember]
        public bool DoesNotPayDues { get; set; }
        [ProtoMember(707)]
        [DataMember]
        public DateTime JoinedLeague { get; set; }
        [ProtoMember(708)]
        [DataMember]
        public double tempDue { get; set; }

        [ProtoMember(709)]
        [DataMember]
        public DateTime DatePaid{ get; set; }
    }
}
