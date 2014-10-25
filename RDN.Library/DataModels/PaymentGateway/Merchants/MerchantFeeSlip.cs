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
    [Table("RDN_Merchant_FeeSlips")]
    public class MerchantFeeSlip : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MerchantFeeSlipId { get; set; }

        public Guid InvoiceId { get; set; }

        [Required]
        public decimal RDNFee { get; set; }
        [Required]
        public decimal CreditCompanyFee { get; set; }
        [Required]
        public decimal Tax { get; set; }        
        [Required]
        public decimal TotalInclTax { get; set; }
        [Required]
        public decimal ShippingCost { get; set; }

        // See classes -> payment -> Merchant-> slipstatus (marks if this slip has been payed for, if it still has to be paid etc)
        public byte SlipStatus { get; set; }
        public DateTime ArchivedDate { get; set; }

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
