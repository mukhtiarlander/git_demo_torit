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
    /// Each round of the Torunament
    /// </summary>
    [Table("RDN_Game_Tournament_Round")]
    public class TournamentRound: InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long RoundNumber { get; set; }
        #region references
        [Required]
        public virtual GameTournament Tournament { get; set; }
        public virtual List<TournamentPairing> Pairings { get; set; }
        #endregion

        public TournamentRound()
        {
            Pairings = new List<TournamentPairing>();
        }
    }
}
