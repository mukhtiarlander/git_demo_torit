using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.League
{
    [Table("RDN_League_PendingMembers")]
    public class PendingMember : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PendingMemberId { get; set; }

        #region References

        [Required]
        public virtual Member.Member Member { get; set; }
        [Required]
        public virtual League League { get; set; }                     

        #endregion
    }
}
