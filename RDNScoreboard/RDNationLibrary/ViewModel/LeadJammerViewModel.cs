using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDNationLibrary.ViewModel
{
    /// <summary>
    /// specifies the Lead Jammer for the game..
    /// </summary>
    public class LeadJammerViewModel
    {
        public TeamMembersViewModel Jammer { get; set; }
        public long GameTimeInMilliseconds { get; set; }
        public long JamTimeInMilliseconds { get; set; }
            }
}
