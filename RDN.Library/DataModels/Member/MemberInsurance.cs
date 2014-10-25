using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Member
{
    [Table("RDN_Member_Insurance")]
    public class MemberInsurance : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long InsuranceId { get; set; }
        [MaxLength(255)]
        public string InsuranceNumber { get; set; }
        public DateTime? Expires { get; set; }
        public Member Member { get; set; }
        public int InsuranceType { get; set; }
    }
}
