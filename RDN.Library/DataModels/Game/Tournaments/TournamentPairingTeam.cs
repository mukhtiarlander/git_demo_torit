using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.Game.Tournaments;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game.Tournaments
{

    /// <summary>
    /// The Pairing of Teams for each round in the tournament
    /// </summary>
    [Table("RDN_Game_Tournament_Pairing_Team")]
    public class TournamentPairingTeam : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PairingTeamId { get; set; }
        public long Score { get; set; }
        /// <summary>
        /// this is the id for the team in the game from the scoreboard.
        /// </summary>
        public Guid TeamIdInGame { get; set; }
        #region references
        [Required]
        public virtual TournamentPairing Pairing { get; set; }
        public virtual TournamentTeam Team { get; set; }
        #endregion

    }
}
