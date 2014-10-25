using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Member
{
    [Table("RDN_Member_Log_Reasons")]
    public class MemberLogReason
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MemberLogReasonId { get; set; }
        [MaxLength(255)]
        public string Reason { get; set; }        
    }
}
