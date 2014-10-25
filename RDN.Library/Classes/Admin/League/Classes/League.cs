using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Admin.League.Classes
{
    public class League
    {
        public Guid PendingLeagueId { get; set; }
        public DateTime Created { get; set; }
        public string LeagueName { get; set; }
        public string ContactTelephone { get; set; }
        public string ContactEmail { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string AdditionalInformation { get; set; }
        public string LogInformation { get; set; }
        public string MemberFirstname { get; set; }
        public string MemberDerbyname { get; set; }
        public string Federation { get; set; }
    }
}
