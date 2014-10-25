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
    [Table("RDN_Game_Tournament_Pairing")]
    public class TournamentPairing: InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PairingId { get; set; }
        public int GroupId { get; set; }
        public string TrackNumber { get; set; }
        public DateTime? StartTime { get; set; }
        #region references
        [Required]
        public virtual TournamentRound Round{ get; set; }
        public virtual List<TournamentPairingTeam> Teams { get; set; }
        #endregion

        public TournamentPairing()
        {
            Teams = new List<TournamentPairingTeam>();
        }
    }
}
