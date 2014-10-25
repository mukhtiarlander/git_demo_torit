using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Federation.Enums;
using RDN.Portable.Classes.Games.Tournament;
using RDN.Portable.Classes.League.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.Games
{
    /// <summary>
    /// used for manual entry games when we don't have a lot of stats on it.
    /// </summary>
    public class ManualGame
    {
        public long ScoreId { get; set; }
        //id of the score from the game it self generated on the client machine.
        public string Team1Name { get; set; }
        public string Team1Id{ get; set; }
        public int Team1Score { get; set; }
        public string Team2Name { get; set; }
        public string Team2Id{ get; set; }
        public int Team2Score { get; set; }
        public DateTime? TimeEntered { get; set; }

        public bool IsPublished { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? PublishDateTime { get; set; }


        public RuleSetsUsedEnum RuleSetEnum { get; set; }
        public string RuleSetEnumDisplay { get; set; }

        public FederationsEnum SanctionedByFederationEnum { get; set; }
        public string SanctionedByFederationEnumDisplay { get; set; }

        public string Notes { get; set; }

        public bool EmailWhenBoutIsPosted { get; set; }

        public MemberDisplayBasic Member { get; set; }

        public LeagueBase League1 { get; set; }
        public LeagueBase League2 { get; set; }
        public TournamentBase Tournament { get; set; }

        public ManualGame()
        { }
        public ManualGame(string team1Name, int team1Score, string team2Name, int team2Score, DateTime timeEntered)
        {
            Team1Name = team1Name;
            Team2Name = team2Name;
            Team1Score = team1Score;
            Team2Score = team2Score;
            TimeEntered = timeEntered;
        }



    }
}
