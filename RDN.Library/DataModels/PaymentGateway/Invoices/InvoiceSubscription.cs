using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Invoices
{
    [Table("RDN_Invoice_Subscriptions")]
    public class InvoiceSubscription// : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceSubscriptionId { get; set; }
        // Internal object used by RDN. When making a payment this object is set. It could be a player id, federation id, or any other identifier guid        
        public Guid InternalObject { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
        [MaxLength(255)]
        public string DigitalPurchaseText { get; set; }
        [Required]
        [MaxLength(255)]
        public string NameRecurring { get; set; }
        public string DescriptionRecurring { get; set; }
        [MaxLength(255)]
        public string ArticleNumber { get; set; }
        [Required]
        public decimal Price { get; set; }
        // Specifies the number of days that this is valid upon payment / recurring payment
        [Required]        
        public int SubscriptionPeriodLengthInDays { get; set; }
        // Checked to see if this is valid
        public DateTime ValidUntil { get; set; }
        // Recurring billing in days, see enum in classes->payment->enums   
        [Required]
        public byte SubscriptionPeriod { get; set; }
        
        // Reference to the invoice
        [Required]
        public virtual Invoice Invoice { get; set; }

    }
}
