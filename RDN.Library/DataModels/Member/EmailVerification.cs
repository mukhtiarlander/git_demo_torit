using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Member
{    
    [Table("RDN_Member_EmailVerifications")]
    public class EmailVerification : InheritDb
    {        
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid VerificationId { get; set; }
        [MaxLength(500)]
        public string EmailAddress { get; set; }
        public DateTime Expires { get; set; }

        #region References

        [Required]
        public virtual Member Member { get; set; }        

        #endregion
    }
}
