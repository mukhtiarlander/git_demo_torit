using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.League
{

    /// <summary>
    /// shows who the owners of the league are.
    /// </summary>
    [Obsolete("ownership is now defined at the membership level of the league")]
    [Table("RDN_League_Owners")]
    public class LeagueOwnership : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public int OwnerType { get; set; }

        #region references
        [Required]
        public virtual League League { get; set; }
        [Required]
        public virtual Member.Member Member { get; set; }
        #endregion

    }
}
