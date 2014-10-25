using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.MemberFees
{
    /// <summary>
    /// if a different fee is required compared to the FeeItem Required Fee, then
    /// we create a fee Required Item.
    /// </summary>
    [Table("RDN_Fees_Required")]
    public class FeesRequired : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long FeeRequiredId { get; set; }

        public string Note { get; set; }

        public double FeeRequired { get; set; }
        public bool IsPaidInFull { get; set; }
        public bool IsFeeWaived { get; set; }
        public virtual FeeItem FeeItem { get; set; }
        public virtual Member.Member MemberRequiredFrom { get; set; }

    }
}
