using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.Collections.ObjectModel;
using RDN.Library.DataModels.PaymentGateway.Paywall;
using RDN.Library.DataModels.PaymentGateway.Merchants;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game.Tournaments
{
    [Table("RDN_Game_Tournament")]
    public class GameTournament : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TournamentId { get; set; }
        [Required]
        [MaxLength(400)]
        public string TournamentName { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public Guid PrivateTournamentId { get; set; }
        public bool IsRemoved { get; set; }
        public bool IsPublished { get; set; }
        public bool AreBracketsPublished { get; set; }
        public string TournamentWebsite { get; set; }
        /// <summary>
        /// used for adding a game to a tournament...  
        /// need to enter a passcode to connect the game to the tournament.
        /// </summary>
        public string TournamentPasscode { get; set; }
        public byte TournamentClass { get; set; }
        public string EmbedVideoString { get; set; }
        public byte TournamentTypeEnum { get; set; }
        /// <summary>
        /// true if the user selects to use the tournament for seeding too.
        /// </summary>
        public bool AreSeedingBracketsRequired { get; set; }
        public byte TournamentTypeEnumForSeedingRound { get; set; }
        /// <summary>
        /// if we are seeding, has the seeding started.
        /// </summary>
        public bool HasSeedingRoundStarted { get; set; }
        /// <summary>
        /// has the real tournamentRoundsStarted?
        /// </summary>
        public bool HasTournamentRoundsStarted { get; set; }
        
        public virtual Paywall Paywall { get; set; }
        public virtual Merchant SelectedShop { get; set; }
                /// <summary>
        /// these are the actual rounds of the tournament.
        /// </summary>
        public virtual ICollection<TournamentRound> Rounds { get; set; }
        /// <summary>
        /// these rounds are for seeding the actual tourament.
        /// </summary>
        public virtual ICollection<TournamentRound> SeedingRounds { get; set; }
        public virtual ICollection<Game> Games { get; set; }
        public virtual ICollection<TournamentTeam> Teams { get; set; }
        public virtual ICollection<TournamentConversation> Conversation { get; set; }
        [Obsolete("Please Start Using OwnersOfTournament")]
        public virtual Member.Member OwnerOfTournament { get; set; }
        public virtual List<TournamentOwner> OwnersOfTournament { get; set; }
        public virtual Federation.Federation FederationOwner { get; set; }
        public virtual ContactCard.ContactCard ContactCard { get; set; }

        public virtual GameTournamentLogo Logo { get; set; }


        public GameTournament()
        {
            OwnersOfTournament = new List<TournamentOwner>();
            Rounds = new Collection<TournamentRound>();
            SeedingRounds = new Collection<TournamentRound>();
            Games = new Collection<Game>();
            Conversation = new Collection<TournamentConversation>();
        }
    }
}
