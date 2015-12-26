using ProtoBuf;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Federation
{
    [ProtoContract]
    [DataContract]
    public class FederationDisplay
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid FederationId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string FederationName { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string MADEClassRank { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string MemberType { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public string OwnerType { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public string FederationComments { get; set; }

        [ProtoMember(7)]
        [DataMember]
        public DateTime? Founded { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public string Website { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public string Country { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public int CountryId { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public string State { get; set; }
        [ProtoMember(12)]
        [DataMember]
        public string Address { get; set; }
        [ProtoMember(13)]
        [DataMember]
        public string City { get; set; }
        [ProtoMember(14)]
        [DataMember]
        public string Email { get; set; }
        [ProtoMember(15)]
        [DataMember]
        public string PhoneNumber { get; set; }
        [ProtoMember(16)]
        [DataMember]
        public double TimeZone { get; set; }
        [ProtoMember(17)]
        [DataMember]
        public PhotoItem Logo { get; set; }
        [ProtoMember(18)]
        [DataMember]
        public DateTime MembershipDate { get; set; }
        [ProtoMember(19)]
        [DataMember]
        public int MembersCount { get; set; }
        [ProtoMember(20)]
        [DataMember]
        public int LeaguesCount { get; set; }
        /// <summary>
        /// internal membership id of federation
        /// </summary>
        [ProtoMember(21)]
        [DataMember]
        public string MembershipId { get; set; }
        [ProtoMember(22)]
        [DataMember]
        public List<MemberDisplayFederation> Members { get; set; }
        
        [ProtoMember(23)]
        [DataMember]
        public List<League.Classes.League> Leagues { get; set; }

        [ProtoMember(24)]
        [DataMember]
        public List<FederationDisplay> Federations { get; set; }

        [ProtoMember(25)]
        [DataMember]
        public League.Classes.League League { get; set; }

        public FederationDisplay()
        {
            Members = new List<MemberDisplayFederation>();
            Leagues = new List<League.Classes.League>();
            Federations = new List<FederationDisplay>();
            League = new League.Classes.League();
        }  
    }

}
