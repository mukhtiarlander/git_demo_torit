using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game
{

    /// <summary>
    /// shows who the owners of the league are.
    /// </summary>
    [Table("RDN_Game_Federation_Owners")]
    public class GameFederationOwnership : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public int OwnerType { get; set; }

        #region references
        [Required]
        public virtual Game Game { get; set; }
        [Required]
        public virtual Federation.Federation Federation { get; set; }
        #endregion

    }
}
