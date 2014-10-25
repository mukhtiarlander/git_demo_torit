using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Federation
{
    /// <summary>
    /// shows who the owners of the federation are.
    /// </summary>
    [Table("RDN_Federation_Owners")]
    public class FederationOwnership : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public int OwnerType { get; set; }
        [Required]
        public bool IsVerified { get; set; }

        #region references
        [Required]
        public virtual Federation Federation { get; set; }
        [Required]
        public virtual Member.Member Member { get; set; }
        #endregion

    }
}
