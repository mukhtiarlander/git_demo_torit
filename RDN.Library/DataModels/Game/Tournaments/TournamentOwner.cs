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
    /// shows who the owners of the game are.
    /// </summary>
    [Table("RDN_Game_Tournament_Owner")]
    public class TournamentOwner: InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public byte OwnerType { get; set; }

        #region references
        [Required]
        public virtual GameTournament Tournament { get; set; }
        [Required]
        public virtual Member.Member Owner { get; set; }
        #endregion

    }
}
