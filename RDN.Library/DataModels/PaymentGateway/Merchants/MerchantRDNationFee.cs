using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Merchants
{
    /// <summary>
    /// this table will collect all the refererall fees the merchant owes .
    /// </summary>
    [Table("RDN_Merchant_RDNation_Fees")]
    public class MerchantRDNationFee: InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MerchantFeeId { get; set; }

        [Required]
        public decimal RDNFee { get; set; }
        public string FeeDescription{ get; set; }

        // See classes -> payment -> Merchant-> slipstatus (marks if this slip has been payed for, if it still has to be paid etc)
        public byte SlipStatus { get; set; }
        public DateTime? PaidDate { get; set; }

        [Required]
        public virtual Merchant Merchant { get; set; }

        public MerchantSlipStatus GetSlipStatus()
        {
            return (MerchantSlipStatus) SlipStatus;
        }

        public void SetSlipStatus(MerchantSlipStatus status)
        {
            SlipStatus = (byte) status;
        }
    }
}
