using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Member
{
    [Table("RDN_Member_Logs")]
    public class MemberLog : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MemberLogId { get; set; }
        [MaxLength(255)]
        public string LogTitle { get; set; }
        [MaxLength(255)]
        public string Ip { get; set; } 
        [Column(TypeName = "text")]
        public string Details { get; set; }

        #region References

        [Required]
        public virtual Member Member { get; set; }
        [Required]
        public virtual MemberLogReason LogReason { get; set; }

        #endregion
    }
}
