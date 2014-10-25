using ProtoBuf;
using RDN.Portable.Classes.Geo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Account.Classes
{
    [ProtoContract]
    [DataContract]
    public class MemberDisplayEdit : MemberDisplayBasic
    {
        public MemberDisplayEdit()
        {
            Countries = new List<CountryClass>();
        }
        [ProtoMember(801)]
        [DataMember]
        /// <summary>
        /// DateTime Started Skating
        /// </summary>
        public DateTime? StartedSkating { get; set; }
        [ProtoMember(802)]
        [DataMember]
        public DateTime? StoppedSkating { get; set; }

        [ProtoMember(803)]
        [DataMember]
        public string DayJob { get; set; }
        [ProtoMember(804)]
        [DataMember]
        public bool IsRetired { get; set; }
        [ProtoMember(805)]
        [DataMember]
        public bool IsProfileRemovedFromPublicView { get; set; }

        [ProtoMember(806)]
        [DataMember]
        public string InsuranceNumWftda { get; set; }
        [ProtoMember(807)]
        [DataMember]
        public string InsuranceNumUsars { get; set; }
        [ProtoMember(808)]
        [DataMember]
        public string InsuranceNumCRDI { get; set; }
        [ProtoMember(809)]
        [DataMember]
        public string InsuranceNumOther { get; set; }
        [ProtoMember(810)]
        [DataMember]
        public DateTime? InsuranceNumWftdaExpires { get; set; }
        [ProtoMember(811)]
        [DataMember]
        public DateTime? InsuranceNumUsarsExpires { get; set; }
        [ProtoMember(812)]
        [DataMember]
        public DateTime? InsuranceNumCRDIExpires { get; set; }
        [ProtoMember(813)]
        [DataMember]
        public DateTime? InsuranceNumOtherExpires { get; set; }
        [ProtoMember(814)]
        [DataMember]
        public int Country { get; set; }
        [ProtoMember(815)]
        [DataMember]
        public List<CountryClass> Countries { get; set; }

        //public string Country { get; set; }
        //public int CountryId { get; set; }
        [ProtoMember(816)]
        [DataMember]
        public string State { get; set; }
        [ProtoMember(817)]
        [DataMember]
        public string Address { get; set; }
        [ProtoMember(818)]
        [DataMember]
        public string Address2 { get; set; }
        [ProtoMember(819)]
        [DataMember]
        public string City { get; set; }
        [ProtoMember(820)]
        [DataMember]
        public string ZipCode { get; set; }

        [ProtoMember(821    )]
        [DataMember]
        public bool IsSuccessful { get; set; }
    }
}