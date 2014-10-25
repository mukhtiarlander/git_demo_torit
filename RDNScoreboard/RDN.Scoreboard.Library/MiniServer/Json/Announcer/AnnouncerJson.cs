using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scoreboard.Library.MiniServer.Json.Announcer;
using Scoreboard.Library.MiniServer.Json.Teams;

namespace Scoreboard.Library.MiniServer.Json
{
    public class AnnouncerJson
    {
        public string team1Name { get; set; }
        public string team2Name { get; set; }
        public MemberForAnnouncerJson JammerT1 { get; set; }
        public MemberForAnnouncerJson PivotT1 { get; set; }
        public MemberForAnnouncerJson Blocker1T1 { get; set; }
        public MemberForAnnouncerJson Blocker2T1 { get; set; }
        public MemberForAnnouncerJson Blocker3T1 { get; set; }
        public MemberForAnnouncerJson Blocker4T1 { get; set; }
        public MemberForAnnouncerJson JammerT2 { get; set; }
        public MemberForAnnouncerJson PivotT2 { get; set; }
        public MemberForAnnouncerJson Blocker1T2 { get; set; }
        public MemberForAnnouncerJson Blocker2T2 { get; set; }
        public MemberForAnnouncerJson Blocker3T2 { get; set; }
        public MemberForAnnouncerJson Blocker4T2 { get; set; }
        public int currentJam { get; set; }
        public Guid currentJamId { get; set; }
        public int totalJams { get; set; }
        public List<LivePlayerStat> PlayerStatsT1 { get; set; }
        public List<LivePlayerStat> PlayerStatsT2 { get; set; }

        public AnnouncerJson() { }


    }
}
