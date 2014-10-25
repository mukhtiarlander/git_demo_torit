using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Invoices
{
    [Table("RDN_Invoice_Paywalls")]
    public class InvoicePaywall : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long InvoicePaywallId { get; set; }
        public Guid MemberPaidId { get; set; }
        public decimal BasePrice { get; set; }
        public decimal PriceAfterFees { get; set; }
        public decimal RDNationsFees { get; set; }
        public decimal ProcessorFees { get; set; }
        public byte PaywallPriceTypeEnum { get; set; }
        public string GeneratedPassword { get;set;}
        public long SecondsViewedPaywall { get; set; }
        public long TimesUsedPassword{ get; set; }
        public DateTime? LastViewedPaywall{ get; set; }
        public Guid InternalObject { get; set; }
        
        public string Name { get; set; }
        public string Description { get; set; }
        public string PaywallLocation{ get; set; }
        /// <summary>
        /// The number of days their subscription is prolonged. Add 31 for 1 month.
        /// </summary>
        public int PaywallLengthOfDays { get; set; }
        public DateTime ValidUntil { get; set; }

        public virtual Paywall.Paywall Paywall { get; set; }
        // Reference to the invoice
        [Required]
        public virtual Invoice Invoice { get; set; }
    }
}
