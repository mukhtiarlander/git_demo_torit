using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.MemberFees
{
    [Table("RDN_Fees_Classification")]
    public class FeeClassification : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long FeeClassificationId { get; set; }

        public string Name { get; set; }

        public double FeeRequired { get; set; }
        public bool DoesNotPayDues { get; set; }
        public virtual FeeManagement FeeItem { get; set; }
        public ICollection<FeesClassificationByMember> MembersClassified { get; set; }

        public FeeClassification()
        {
            MembersClassified = new Collection<FeesClassificationByMember>();
        }
    }
}
