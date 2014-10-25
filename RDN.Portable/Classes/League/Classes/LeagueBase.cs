using ProtoBuf;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Federation.Enums;
using RDN.Portable.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.League.Classes
{
    [ProtoContract]
    [DataContract]
    [ProtoInclude(200, typeof(League))]
    public class LeagueBase
    {
        [ProtoMember(1)]
        [DataMember]
        public string Name { get; set; }

        [ProtoMember(2)]
        [DataMember]
        public DateTime Founded { get; set; }

        [ProtoMember(3)]
        [DataMember]
        public string Website { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string Twitter { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public string Facebook { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public string Instagram { get; set; }

        [ProtoMember(7)]
        [DataMember]
        public RuleSetsUsedEnum RuleSetsPlayedEnum { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public string Country { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public int CountryId { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public string State { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public string Address { get; set; }
        [ProtoMember(12)]
        [DataMember]
        public string City { get; set; }
        [ProtoMember(13)]
        [DataMember]
        public string ZipCode { get; set; }
        [ProtoMember(14)]
        [DataMember]
        public string Email { get; set; }
        [ProtoMember(15)]
        [DataMember]
        public string PhoneNumber { get; set; }
        [ProtoMember(16)]
        [DataMember]
        public int CultureSelected { get; set; }


        /// <summary>
        /// the league join code given to members so that they can join the league if they aren't
        /// with them already.
        /// </summary>
        [ProtoMember(17)]
        [DataMember]
        public Guid JoinCode { get; set; }
        [ProtoMember(18)]
        [DataMember]
        public bool IsSuccessful { get; set; }
        [ProtoMember(19)]
        [DataMember]
        public Guid LeagueId { get; set; }
        [ProtoMember(20)]
        [DataMember]
        public double TimeZone { get; set; }
        [ProtoMember(21)]
        [DataMember]
        public List<Culture> Cultures { get; set; }

        public LeagueBase()
        {
            Cultures = new List<Culture>();
        }
    }
}
