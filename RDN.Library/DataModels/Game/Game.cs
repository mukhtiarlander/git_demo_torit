using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.Scoreboard;
using RDN.Library.DataModels.PaymentGateway.Paywall;
using RDN.Library.DataModels.Game.Members;
using RDN.Library.DataModels.Game.Tournaments;
using RDN.Library.DataModels.PaymentGateway.Merchants;
using RDN.Library.DataModels.Game.Officials;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game
{
    [Table("RDN_Game")]
    public class Game : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid GameId { get; set; }
        [Required]
        [MaxLength(200)]
        public string GameName { get; set; }
        [MaxLength(200)]
        public string VersionNumber { get; set; }
        [Required]
        public DateTime GameDate { get; set; }
        [Required]
        public bool IsGameOver { get; set; }
        [MaxLength(200)]
        public string GameType { get; set; }
        public DateTime GameEndDate { get; set; }
        [Required]
        public int ScoreboardType { get; set; }
        public Int64 ElapsedGameTimeInMilliseconds { get; set; }
        public bool HasGameStarted { get; set; }
        [MaxLength(200)]
        public string GameLocation { get; set; }
        public string GameCity{ get; set; }
        public string GameState{ get; set; }
        public bool IsGamePublishedOnline { get; set; }
        public bool IsGameLive { get; set; }
        public bool IsGameScrimmage { get; set; }
        /// <summary>
        /// the id to find the game online in the management area.
        /// </summary>
        public Guid IdForOnlineManagementUse { get; set; }

        public int IsThereVideoOfGame { get; set; }
        public string StreamingUrlOfVideo { get; set; }
        public string StreamingUrlOfVideoMobile { get; set; }
        public string EmbededVideoHtml { get; set; }
        public virtual Paywall Paywall{get;set;}
        public virtual Merchant SelectedShop { get; set; }

        #region References
        /// <summary>
        /// dont used this, deprecated use MemberOwners
        /// </summary>
        public virtual Member.Member OwnerOfGame { get; set; }
        /// <summary>
        /// dont used this, deprecated use LeagueOwners
        /// </summary>
        public virtual League.League LeagueOwner { get; set; }
        /// <summary>
        /// dont used this, deprecated use FederationOwners
        /// </summary>
        public virtual Federation.Federation FederationOwner { get; set; }
        public virtual ICollection<GameMemberOwnership> MemberOwners { get; set; }
        public virtual ICollection<GameLeagueOwnership> LeagueOwners { get; set; }
        public virtual ICollection<GameFederationOwnership> FederationOwners { get; set; }

        public virtual ScoreboardInstance ScoreboardInstance { get; set; }
        public virtual ICollection<GameJam> GameJams { get; set; }
        public virtual ICollection<GameConversation> Conversation { get; set; }
        public virtual ICollection<GameTeam> GameTeams { get; set; }
        public virtual ICollection<GameAdvertisement> GameAdvertisements { get; set; }
        public virtual ICollection<GameMemberPenaltyBox> GameMemberPenaltyBox { get; set; }
        public virtual ICollection<GameMemberPenalty> GameMemberPenalties { get; set; }
        public virtual ICollection<GameMemberAssist> GameMemberAssists { get; set; }
        public virtual ICollection<GameMemberBlock> GameMemberBlocks { get; set; }
        public virtual GamePolicy GamePolicy { get; set; }
        public virtual ICollection<GameScore> GameScores { get; set; }
        public virtual ICollection<GameTimeout> GameTimeouts { get; set; }
        public virtual ICollection<GameLink> GameLinks { get; set; }
        public virtual ICollection<GameOfficial> GameOfficials { get; set; }
        public virtual ICollection<GameOfficialReview> OfficialReviews { get; set; }
        public virtual GameTournament GameTournament { get; set; }
        #endregion

        #region Contructor

        public Game()
        {
            GameJams = new Collection<GameJam>();
            GameTeams = new Collection<GameTeam>();
            GameAdvertisements = new Collection<GameAdvertisement>();
            GameLinks = new Collection<GameLink>();
            Conversation = new Collection<GameConversation>();
            GameMemberBlocks = new Collection<GameMemberBlock>();
            GameMemberAssists = new Collection<GameMemberAssist>();
            GameMemberPenalties = new Collection<GameMemberPenalty>();
            LeagueOwners = new Collection<GameLeagueOwnership>();
            FederationOwners = new Collection<GameFederationOwnership>();
            MemberOwners = new Collection<GameMemberOwnership>();
        }

        #endregion
    }
}
