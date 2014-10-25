using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Game
{
    [Obsolete("Use RDN.Mobile.Model.JSON.CurrentGame")]
    public class CurrentGameObsolete
    {
        public DateTime StartTime { get; set; }
        public Guid GameId { get; set; }
        public string GameName { get; set; }
        public string Team1Name { get; set; }
        public string Team1LinkName { get; set; }
        public string Team2LinkName { get; set; }
        public string Team1LogoUrl { get; set; }
        public string Team2Name { get; set; }
        public string Team2LogoUrl { get; set; }
        public int? Team1Score { get; set; }
        public int? Team2Score { get; set; }
        public long PeriodTimeLeft { get; set; }
        public long JamTimeLeft { get; set; }
        public int? JamNumber { get; set; }
        public int? PeriodNumber { get; set; }
        public string RuleSet { get; set; }
        public bool IsLiveStreaming { get; set; }
        public bool HasGameEnded { get; set; }

    }
}
