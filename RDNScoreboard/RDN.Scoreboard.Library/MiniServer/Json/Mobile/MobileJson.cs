using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scoreboard.Library.MiniServer.Json.Mobile
{
    public class MobileJson
    {
        public string team1Name { get; set; }
        public string team2Name { get; set; }
        
        public int PeriodNumber { get; set; }
        public long PeriodTime { get; set; }
        public long LineUpTime { get; set; }
        public long JamTime { get; set; }
        public int JamNumber { get; set; }
        public long TimeOutTime { get; set; }
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
        public int Team1JamScore { get; set; }
        public int Team2JamScore { get; set; }

        public bool IsJamRunning { get; set; }
        public bool IsTimeOutRunning { get; set; }
        public bool IsPeriodRunning { get; set; }

        public MobileJson()
        { }
    }
}
