using ProtoBuf;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Games.Scoreboard.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Games
{
    [ProtoContract]
    [DataContract]
    public class Game
    {
        //      [ProtoMember(1)]
        //      [DataMember]
        //      private static readonly int MAX_GAMES_TO_TAKE = 8;
        [ProtoMember(2)]
        [DataMember]
        public string GameName { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public DateTime GameDate { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public Guid GameId { get; set; }
        //      [ProtoMember(5)]
        //      [DataMember]
        //      public Guid PrivateKeyForGame { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public string Team1Name { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public Guid Team1Id { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public Guid Team1LinkId { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public IEnumerable<GameScore> Team1Score { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public int Team1ScoreTotal { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public string Team2Name { get; set; }
        [ProtoMember(12)]
        [DataMember]
        public int Team2ScoreTotal { get; set; }
        [ProtoMember(13)]
        [DataMember]
        public IEnumerable<GameScore> Team2Score { get; set; }
        [ProtoMember(14)]
        [DataMember]
        public Guid Team2Id { get; set; }
        [ProtoMember(15)]
        [DataMember]
        public Guid Team2LinkId { get; set; }
        //      [ProtoMember(16)]
        //      [DataMember]
        //      public bool HasGameStarted { get; set; }
        //      [ProtoMember(17)]
        //      [DataMember]
        //      public bool IsGameLive { get; set; }
        //      [ProtoMember(18)]
        //      [DataMember]
        //      public bool IsGameOver { get; set; }
        //      [ProtoMember(19)]
        //      [DataMember]
        //      public bool IsGamePublishedOnline { get; set; }
        //      [ProtoMember(20)]
        //      [DataMember]
        //      public bool IsGameScrimmage { get; set; }
        //      [ProtoMember(21)]
        //      [DataMember]
        //      public string VersionNumber { get; set; }
        //      [ProtoMember(22)]
        //      [DataMember]
        //      public List<MemberDisplayGame> MembersOfGame { get; set; }
        //      [ProtoMember(23)]
        //      [DataMember]
        //      public GameVideoTypeEnum IsThereVideoOfGame { get; set; }
        //      [ProtoMember(24)]
        //      [DataMember]
        //      public string EmbededVideoString { get; set; }
        //      [ProtoMember(25)]
        //      [DataMember]
        //      public string StreamingUrlSilverlight { get; set; }
        //      [ProtoMember(26)]
        //      [DataMember]
        //      public string StreamingMobileUrlSilverlight { get; set; }
        //      [ProtoMember(27)]
        //      [DataMember]
        //      public long SelectedPaywall { get; set; }
        //      [ProtoMember(28)]
        //      [DataMember]
        //      public List<Paywall> Paywalls { get; set; }
        //      [ProtoMember(29)]
        //      [DataMember]
        //      public List<Tournament> AvailableTournaments { get; set; }
        //      [ProtoMember(30)]
        //      [DataMember]
        //      public List<MemberDisplayBasic> MemberOwners { get; set; }
        //      [ProtoMember(31)]
        //      [DataMember]
        //      public List<League.LeagueFactory> LeagueOwners { get; set; }
        //      [ProtoMember(32)]
        //      [DataMember]
        //      public List<Federation.Classes.FederationDisplay> FederationOwners { get; set; }


        //      /// <summary>
        //      /// selected tournament for publishing game.
        //      /// </summary>
        //      [ProtoMember(33)]
        //      [DataMember]
        //      public string SelectedTournament { get; set; }
        //      [ProtoMember(34)]
        //      [DataMember]
        //      public string PassCodeEnteredForTournament { get; set; }

        //      [ProtoMember(35)]
        //      [DataMember]
        //      public List<OverviewMerchant> AvailableShops { get; set; }
        //      [ProtoMember(36)]
        //      [DataMember]
        //      public string SelectedShop { get; set; }

        //      /// <summary>
        //      /// the difference in percentage between the team1 score and the team 2 score.
        //      /// </summary>
        [ProtoMember(37)]
        [DataMember]
        public double Score1Score2Delta { get; set; }

        //      public Game()
        //      {
        //          MembersOfGame = new List<MemberDisplayGame>();
        //          AvailableTournaments = new List<Tournament>();
        //          MemberOwners = new List<MemberDisplayBasic>();
        //          LeagueOwners = new List<League.LeagueFactory>();
        //          FederationOwners = new List<Federation.Classes.FederationDisplay>();
        //      }

    }
}
