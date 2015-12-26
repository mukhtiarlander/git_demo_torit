using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Federation
{
    /// <summary>
    /// shows who the members of the federation are.
    /// which is added by the federation.
    /// </summary>
    [Table("RDN_Federation_Leagues")]
    public class FederationLeague : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }


        /// <summary>
        /// any comments the federation has for this member.
        /// </summary>
        public string CommentsForLeague{ get; set; }

        public string InternalLeagueIdForFederation { get; set; }

        public DateTime? MembershipDate { get; set; }

        public bool  IsRemoved { get; set; }

        #region references
        [Required]
        public virtual Federation Federation { get; set; }
        [Required]
        public virtual League.League League{ get; set; }
        #endregion
    }
}
