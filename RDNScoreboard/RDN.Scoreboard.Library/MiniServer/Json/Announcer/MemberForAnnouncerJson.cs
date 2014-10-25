using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scoreboard.Library.MiniServer.Json.Teams
{
    public class MemberForAnnouncerJson
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public int Jams { get; set; }
        public string PointsPerJam { get; set; }
        public int Points { get; set; }
        public string PointsPerMinute { get; set; }
        public string LeadJamPc { get; set; }
    }
}
