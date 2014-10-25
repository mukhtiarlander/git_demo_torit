using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Team.Classes
{
    public class Team
    {
        public Guid TeamId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Score { get; set; }
    }
}
