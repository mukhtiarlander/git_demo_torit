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
    [Table("RDN_Federation_Members")]
    public class FederationMember : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }


        //attributes for the MADE federation
        public int MADEClassRankForMember { get; set; }
        /// <summary>
        /// declares the member type for them.  Inactive, Active etc...
        /// </summary>
        public int MemberType { get; set; }
        /// <summary>
        /// this is the id from the actual federation.
        /// </summary>
        public string FederationIdForMember { get; set; }
        /// <summary>
        /// any comments the federation has for this member.
        /// </summary>
        public string CommentsForMember { get; set; }

        public DateTime? MembershipDate { get; set; }
        public bool IsRemoved { get; set; }

        #region references
        [Required]
        public virtual Federation Federation { get; set; }
        [Required]
        public virtual Member.Member Member { get; set; }
        #endregion
    }
}
