using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.MemberFees
{
    [Table("RDN_Fees_Collected")]
    public class FeesCollected : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long FeeCollectionId { get; set; }

        public string Note { get; set; }

        public double FeeCollected { get; set; }
        public bool IsPaidInFull { get; set; }
        public bool IsFeeWaived { get; set; }
        /// <summary>
        /// if the actual dues collection person cleared this dues
        /// item, we will turn dues on for the person and make sure they pay.
        /// </summary>
        public bool WasClearedByUser { get; set; }
        public virtual FeeItem FeeItem { get; set; }
        public virtual Member.Member MemberPaid { get; set; }

    }
}
