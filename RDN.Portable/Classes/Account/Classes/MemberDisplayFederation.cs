using ProtoBuf;
using RDN.Portable.Classes.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Account.Classes
{
    [ProtoContract]
    [DataContract]
    public class MemberDisplayFederation : MemberDisplayBasic
    {
        public MemberDisplayFederation()
        {
            Leagues = new List<League.Classes.League>();
            Teams = new List<Team.TeamDisplay>();
            Photos = new List<PhotoItem>();
        }

        [ProtoMember(1)]
        [DataMember]
        public List<League.Classes.League> Leagues { get; set; }

        [ProtoMember(2)]
        [DataMember]
        public List<Team.TeamDisplay> Teams { get; set; }

        [ProtoMember(3)]
        [DataMember]
        public string MemberUrl { get; set; }


        [ProtoMember(4)]
        [DataMember]
        public string Edit { get; set; }
        /// <summary>
        /// MADE federation attribute for this member
        /// </summary>

        [ProtoMember(5)]
        [DataMember]
        public string MadeClassRank { get; set; }
        /// <summary>
        /// member type of the federation
        /// </summary>

        [ProtoMember(6)]
        [DataMember]
        public string FederationMemberType { get; set; }

        [ProtoMember(7)]
        [DataMember]
        public string MembershipId { get; set; }


        [ProtoMember(8)]
        [DataMember]
        public List<PhotoItem> Photos { get; set; }
    }
}
