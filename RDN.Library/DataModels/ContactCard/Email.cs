using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.ContactCard
{
    [Table("RDN_ContactCard_Emails")]
    public class Email : InheritDb 
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EmailId { get; set; }
        [MaxLength(500)]  
        public string EmailAddress { get; set; }
        public bool IsDefault { get; set; }

        #region References

        [Required]
        public virtual ContactCard ContactCard { get; set; }        

        #endregion
    }
}
