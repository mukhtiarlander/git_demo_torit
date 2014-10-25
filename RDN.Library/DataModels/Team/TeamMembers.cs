using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Team
{
    /// <summary>
    /// shows who the members of the federation are.
    /// which is added by the federation.
    /// </summary>
    [Table("RDN_Team_Members")]
    public class TeamMember : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        /// <summary>
        /// any comments the federation has for this member.
        /// </summary>
        public string CommentsForMember { get; set; }

        public DateTime? MembershipDate { get; set; }


        #region references
        [Required]
        public virtual Team Team { get; set; }
        [Required]
        public virtual Member.Member Member { get; set; }
        #endregion
    }
}
