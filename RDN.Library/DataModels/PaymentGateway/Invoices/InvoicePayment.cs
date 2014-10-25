using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Invoices
{
    [Table("RDN_Invoice_Payments")]
    public class InvoicePayment //: InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoicePaymentId { get; set; }        
        // Amount payed
        [Required]
        public decimal Amount { get; set; }
        // Payment details
        public string Details { get; set; }
        // Date the payment was registered
        [Required]
        public DateTime Registered { get; set; }
        [Required]
        public bool ManuallyRegistered { get; set; }
        public Guid ManuallyRegisteredByAdminLogin { get; set; }

        // Credit card company static fee. Example those 30 cents always charged by google / transaction
        public decimal CreditCardCompanyProcessorDeductedFee { get; set; }

        // Reference to the invoice
        [Required]
        public virtual Invoice Invoice { get; set; }

        public InvoicePayment()
        {
            ManuallyRegistered = false;
            Registered = DateTime.Now;
        }
    }
}
