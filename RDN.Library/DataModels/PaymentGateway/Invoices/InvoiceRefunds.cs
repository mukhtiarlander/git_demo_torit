using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Invoices
{
    [Table("RDN_Invoice_Refunds")]
    public class InvoiceRefund: InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long InvoiceRefundId { get; set; }
        public decimal PriceRefunded { get; set; }
        
        //public Guid InternalObject { get; set; }
        // Reference to the invoice
        [Required]
        public virtual Invoice Invoice { get; set; }
    }
}
