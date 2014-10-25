using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Invoices
{
    [Table("RDN_Invoice_Writer_Payout")]
    public class InvoiceWriterPayoutItem//: InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long InvoiceWriterPayoutId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public Guid PayoutId { get; set; }
        public Guid UserPaidId { get; set; }
        public decimal BasePrice { get; set; }
        public decimal PriceAfterFees { get; set; }
        public int WhoPaysFees { get; set; }
        public DateTime? PaidDateTime { get; set; }
        public DateTime? PaymentRequestedDateTime { get; set; }

        // Reference to the invoice
        [Required]
        public virtual Invoice Invoice { get; set; }
    }
}
