using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Admin.Admin.Classes
{
    public class ContactLeague
    {
        public int LeagueId { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public string HomePage { get; set; }
        public string Facebook { get; set; }
        public DateTime Created { get; set; }
        public string LeagueType { get; set; }
        public string Association { get; set; }
        public string Comment { get; set; }

        public IList<ContactLeagueAddress> Addresses { get; set; } 
    }
}
