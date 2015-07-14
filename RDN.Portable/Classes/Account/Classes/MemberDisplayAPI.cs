using ProtoBuf;
using RDN.Portable.Classes.Insurance;
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
        public MemberDisplayAPI()
            : base()
        {
            InsuranceNumbers = new List<InsuranceNumber>();
        }
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
        [ProtoMember(116)]
        [DataMember]
        public string LeagueClassificationOfSkatingLevel { get; set; }

        [ProtoMember(117)]
        [DataMember]
        public List<InsuranceNumber> InsuranceNumbers { get; set; }


    }
}
