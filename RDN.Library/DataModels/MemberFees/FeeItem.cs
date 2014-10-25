using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.MemberFees
{
    [Table("RDN_Fees_Items")]
    public class FeeItem : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long FeeCollectionId { get; set; }

        public double CostOfFee { get; set; }
        public int DaysBeforeDeadlineToNotify { get; set; }
        public DateTime PayBy { get; set; }
        /// <summary>
        /// members notified about payment due.
        /// </summary>
        public bool Notified { get; set; }

        public virtual FeeManagement FeeManagedBy { get; set; }

        public virtual ICollection<FeesRequired> FeesRequired { get; set; }
        public virtual ICollection<FeesCollected> FeesCollected { get; set; }

        public FeeItem()
        {
            FeesCollected = new Collection<FeesCollected>();
            FeesRequired = new Collection<FeesRequired>();
        }
    }
}
