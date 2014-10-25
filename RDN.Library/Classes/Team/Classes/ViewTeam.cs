using System;
using System.Collections.Generic;
using RDN.Library.Classes.Account.Classes;

namespace RDN.Library.Classes.Team.Classes
{
    public class ViewTeam : Team
    {        
        public List<ViewMember> Members { get; set; }

        public ViewTeam()
        {
            Members = new List<ViewMember>();
        }
    }
}
