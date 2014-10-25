using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Account.Classes
{
    [ProtoContract]
    [DataContract]
    public class MemberDisplayAPI : MemberDisplayBasic
    {
        [ProtoMember(101)]
        [DataMember]
        public bool HideDOBFromLeague { get; set; }
        [ProtoMember(102)]
        [DataMember]
        public bool HideEmailFromLeague { get; set; }
        [ProtoMember(103)]
        [DataMember]
        public bool HidePhoneNumberFromLeague { get; set; }
        [ProtoMember(104)]
        [DataMember]
        public bool HideDOBFromPublic { get; set; }
        [ProtoMember(105)]
        [DataMember]
        public string Job { get; set; }
        [ProtoMember(106)]
        [DataMember]
        public DateTime SkillsTestDate { get; set; }

        [ProtoMember(107)]
        [DataMember]
        public DateTime DepartureDate { get; set; }
        [ProtoMember(108)]
        [DataMember]
        public string InsuranceNumWftda { get; set; }
        [ProtoMember(109)]
        [DataMember]
        public string InsuranceNumUsars { get; set; }
        [ProtoMember(110)]
        [DataMember]
        public string InsuranceNumCRDI { get; set; }
        [ProtoMember(111)]
        [DataMember]
        public string InsuranceNumOther { get; set; }
        [ProtoMember(112)]
        [DataMember]
        public DateTime InsuranceNumWftdaExpires { get; set; }
        [ProtoMember(113)]
        [DataMember]
        public DateTime InsuranceNumUsarsExpires { get; set; }
        [ProtoMember(114)]
        [DataMember]
        public DateTime InsuranceNumCRDIExpires { get; set; }
        [ProtoMember(115)]
        [DataMember]
        public DateTime InsuranceNumOtherExpires { get; set; }
        [ProtoMember(116)]
        [DataMember]
        public string LeagueClassificationOfSkatingLevel { get; set; }

    }
}
