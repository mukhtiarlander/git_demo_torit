using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scoreboard.Library.MiniServer.Json
{
    public class TeamMemberJson
    {
        public string memberName { get; set; }
        public string memberId { get; set; }
        public int totalBlocks { get; set; }
        public string memberNumber { get; set; }
        public int assistsForJam { get; set; }
        public int blocksForJam { get; set; }
        public int penaltiesForJam { get; set; }
        public int scoreForJam { get; set; }
        public int totalAssists { get; set; }
        public int totalPenalties { get; set; }
        public int totalScores { get; set; }
        public bool isJammer { get; set; }
        public bool isPivot { get; set; }
        public bool isBlocker1 { get; set; }
        public bool isBlocker2 { get; set; }
        public bool isBlocker4 { get; set; }
        public bool isBlocker3 { get; set; }
        public bool lostLead { get; set; }
        public bool isLead { get; set; }
        public bool linedUp { get; set; }
        public bool isPBox { get; set; }
        public int scorePass { get; set; }
        public int scoreForPass { get; set; }
        
        public TeamMemberJson()
        { }

    }
}
