using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game.Members
{

    /// <summary>
    /// shows who the owners of the league are.
    /// </summary>
    [Table("RDN_Game_Member_Owners")]
    public class GameMemberOwnership : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public int OwnerType { get; set; }

        #region references
        [Required]
        public virtual Game Game { get; set; }
        [Required]
        public virtual Member.Member Member { get; set; }
        #endregion

    }
}
