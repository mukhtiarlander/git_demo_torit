using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.MemberFees
{
    [Table("RDN_Fees_Classification_By_Member")]
    public class FeesClassificationByMember: InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long FeeClassificationByMemberId { get; set; }

        public virtual FeeClassification FeeItem { get; set; }
        public virtual Member.Member Member{ get; set; }
        
    }
}
