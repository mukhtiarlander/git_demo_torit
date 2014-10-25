using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.Game.Members;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game
{
    /// <summary>
    /// tells who the lead jammers are for the game.
    /// </summary>
    [Table("RDN_Game_Jams_Passes")]
    public class GameJamPasses : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long JamPassId { get; set; }
        public int PassNumber { get; set; }
        public Guid GamePassId { get; set; }

        public int PointsScoredForPass { get; set; }
        public long JamTimeMilliseconds { get; set; }
        public byte TeamNumberEnum { get; set; }

        #region References
        [Required]
        public virtual GameMember SkaterWhoPassed { get; set; }

        [Required]
        public virtual GameJam GameJam { get; set; }

        #endregion
    }
}
