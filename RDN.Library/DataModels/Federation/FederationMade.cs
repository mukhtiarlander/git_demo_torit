using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels;

namespace RDN.Library.DataModels.Federation
{
    /// <summary>
    /// a table exclusivley for the MADE federation and certain member features.
    /// </summary>
    [Table("RDN_Federation_Made")]
    public class FederationMade : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        
        public int ClassRankForMember { get; set; }

        #region references
        [Required]
        public virtual FederationMember FederationMember { get; set; }
        #endregion
    }
}
