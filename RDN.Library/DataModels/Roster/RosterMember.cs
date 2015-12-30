using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDN.Library.DataModels.Base;

namespace RDN.Library.DataModels.Roster
{
     [Table("RDN_Roster_Member")]
    public class RosterMember : InheritDb
    {
         [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
         public long Id { get; set; }

         public int InsuranceType { get; set; }

         public bool IsRemoved { get; set; }

         #region references
         [Required]
         public virtual Roster Roster { get; set; }
         [Required]
         public virtual Member.Member Member { get; set; }
         #endregion
    }
}
