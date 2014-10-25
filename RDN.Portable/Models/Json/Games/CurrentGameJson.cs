using RDN.Portable.Models.Json.Games.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Models.Json.Games
{
    [DataContract]
    public class CurrentGameJson
    {
        [DataMember]
        public DateTime StartTime { get; set; }
        [DataMember]
        public Guid GameId { get; set; }
        [DataMember]
        public long ScoreId { get; set; }
        [DataMember]
        public string GameName { get; set; }
        [DataMember]
        public string Team1Name { get; set; }
        [DataMember]
        public string Team1LinkName { get; set; }
        [DataMember]
        public string Team2LinkName { get; set; }
        [DataMember]
        public string Team1LogoUrl { get; set; }
        [DataMember]
        public string Team2Name { get; set; }
        [DataMember]
        public string Team2LogoUrl { get; set; }
        [DataMember]
        public int Team1Score { get; set; }
        [DataMember]
        public int Team2Score { get; set; }
        [DataMember]
        public long PeriodTimeLeft { get; set; }
        [DataMember]
        public long JamTimeLeft { get; set; }
        public string JamNumberHuman { get { return "J" + JamNumber; } }
        [DataMember]
        public int JamNumber { get; set; }

        public string PeriodNumberHuman { get { return "P" + PeriodNumber; } }
        [DataMember]
        public int PeriodNumber { get; set; }
        [DataMember]
        public string RuleSet { get; set; }
        [DataMember]
        public string SanctioningFederationType { get; set; }

        [DataMember]
        public bool IsLiveStreaming { get; set; }
        [DataMember]
        public string GameUrl { get; set; }
        /// <summary>
        /// from SCOREBOARD, ROLLINNEWS
        /// </summary>
        [DataMember]
        public GameLocationFromEnum GameLocationFrom { get; set; }


        public string GameHeader
        {
            get
            {
                if (HasGameEnded || StartTime < DateTime.UtcNow.AddDays(-1))
                {
                    return StartTime.Year + "/" + StartTime.Month + "/" + StartTime.Day + ": Final";
                }
                else
                {
                    string time = string.Empty;
                    var tsPeriod = TimeSpan.FromMilliseconds(PeriodTimeLeft);
                    string answerPeriod = string.Format("{0:D2}:{1:D2}", tsPeriod.Minutes, tsPeriod.Seconds);
                    var tsJam = TimeSpan.FromMilliseconds(JamTimeLeft);
                    string answerJam = string.Format("{0:D1}:{1:D2}", tsJam.Minutes, tsJam.Seconds);
                    return "P" + PeriodNumber + " " + answerPeriod + ": J" + JamNumber + " " + answerJam;
                }
            }
        }

        [DataMember]
        public bool HasGameEnded { get; set; }
    }
}
