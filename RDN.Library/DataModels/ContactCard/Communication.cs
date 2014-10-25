using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.ContactCard
{
    [Table("RDN_ContactCard_Communications")]
    public class Communication : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CommunicationId { get; set; }
        [MaxLength(255)]
        public string Data { get; set; }
        public bool IsDefault { get; set; }
        public byte CarrierType { get; set; }
        public string SMSVerificationCode { get; set; }
        public bool IsCarrierVerified { get; set; }
        public byte CommunicationTypeEnum { get; set; }


        #region References

        [Required]
        public virtual ContactCard ContactCard { get; set; }
        //[Required]
        //public virtual CommunicationType CommunicationType { get; set; }

        #endregion
    }
}
