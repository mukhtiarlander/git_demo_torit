using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Team
{
    [ProtoContract]
    [DataContract]
    public class TeamDisplay
    {
        /// <summary>
        /// name of the team
        [ProtoMember(1)]
        [DataMember]
        public string TeamName { get; set; }
        /// <summary>
        /// id of the team
        /// </summary>
        [ProtoMember(2)]
        [DataMember]
        public Guid TeamId { get; set; }
        /// <summary>
        /// descriptin of the team.
        /// </summary>
        [ProtoMember(3)]
        [DataMember]
        public string Description { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public List<TeamLogo> ScoreboardLogos { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public List<TeamLogo> LeagueLogos { get; set; }

        public TeamDisplay()
        {
            ScoreboardLogos = new List<TeamLogo>();
            LeagueLogos = new List<TeamLogo>();
        }
    }
}
