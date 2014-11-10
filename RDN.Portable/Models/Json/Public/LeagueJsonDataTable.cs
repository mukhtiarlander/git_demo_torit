using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Models.Json.Public
{
    [Obsolete("Use LeagueBase")]
    [DataContract]
    public class LeagueJsonDataTable
    {
        [DataMember]
        public string LogoUrl { get; set; }
        [DataMember]
        public string LogoUrlThumb { get; set; }
        [DataMember]
        public string LeagueUrl { get; set; }
        [DataMember]
        public string LeagueName { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public string LeagueId { get; set; }
        [DataMember]
        public bool GotExtendedContent { get; set; }
        [DataMember]
        public double lat { get; set; }
        [DataMember]
        public double lon { get; set; }
        [DataMember]
        public Int32 Membercount { get; set; }
        [DataMember]
        public DateTime DateFounded { get; set; }
        [DataMember]
        public string WebSite { get; set; }
        [DataMember]
        public string Twitter { get; set; }
        [DataMember]
        public string Facebook { get; set; }
        [DataMember]
        public string Instagram { get; set; }
        [DataMember]
        public string RuleSetsPlayed { get; set; }
        [DataMember]
        public string PublicEventCount{ get; set; } 
    }
}
