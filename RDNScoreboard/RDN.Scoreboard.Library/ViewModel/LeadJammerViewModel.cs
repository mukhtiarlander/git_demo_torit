using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scoreboard.Library.Static.Enums;
using Scoreboard.Library.ViewModel.Members;

namespace Scoreboard.Library.ViewModel
{
    /// <summary>
    /// specifies the Lead Jammer for the game..
    /// </summary>
    public class LeadJammerViewModel
    {
        public long GameLeadJamId { get; set; }
        public TeamMembersViewModel Jammer { get; set; }
        public long GameTimeInMilliseconds { get; set; }
        public long JamTimeInMilliseconds { get; set; }
        public TeamNumberEnum Team{ get; set; }
    }
}
