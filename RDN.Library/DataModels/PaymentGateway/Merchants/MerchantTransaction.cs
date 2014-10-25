using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Merchants
{
    [Table("RDN_Merchant_Transactions")]
    public class MerchantTransaction : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MerchantTransactionId { get; set; }

        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string Bank { get; set; }
        [Required]
        public string BankAccount { get; set; }
        
        [Required]
        public Guid TransferredByAdminLogin { get; set; }

        // Fee slips attached to this transaction. Money going out to their bank.
        public virtual IList<MerchantFeeSlip> FeeSlips { get; set; } 
    }
}
