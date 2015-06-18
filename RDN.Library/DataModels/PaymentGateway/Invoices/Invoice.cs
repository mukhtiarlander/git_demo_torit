using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.PaymentGateway.Merchants;
using RDN.Library.DataModels.PaymentGateway.Money;
using RDN.Portable.Classes.Payment.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Invoices
{
    [Table("RDN_Invoices")]
    public class Invoice : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid InvoiceId { get; set; }
        // The order id number of the payment provider (ex google order number)
        [MaxLength(255)]
        public string PaymentProviderOrderId { get; set; }
        // InvoicePayment provider, stripe, google, paypal etc.  
        [Required]
        public byte PaymentProvider { get; set; }
        public string PaymentProviderChargeId { get; set; }
        public string PaymentProviderCustomerId { get; set; }
        public Guid UserId { get; set; }
        public Guid ShoppingCartId { get; set; }

        // Current status of the invoice, see enum in classes->payment->enums        
        [Required]
        public byte InvoiceStatus { get; set; }
        // Last time the invoice status was updated

        public DateTime InvoiceStatusUpdated { get; set; }
        // Note, only visible in the admin section of RDN
        public string AdminNote { get; set; }
        // Public note, visible to the user
        public string Note { get; set; }
        public bool HasNotifiedAboutReviewingProduct { get; set; }
        // Total price for the items (excluding shipping, tax and credit card fees)
        [Required]
        public decimal BasePriceForItems { get; set; }
        // Total price before tax
        public decimal TotalBeforeTax { get; set; }
        // Tax rate
        public double TaxRate { get; set; }
        // Tax
        public decimal Tax { get; set; }
        // Total price
        public decimal TotalIncludingTax { get; set; }
        // Shipping
        public decimal Shipping { get; set; }
        // Currency, see enum in classes->payment->enums   

        // Shipping type, see enum in classes->payment->enums   
        public byte ShippingType { get; set; }
        // When the base invoice is paid. Base invoice means everything except recurring subscriptions. So if the customer bought a t-shirt and an entry ticket along with a subscriptions. This is then marked completed as soon as the ticket and t-shirt has been paid for. The subscription is exluded in this variable.
        public bool InvoicePaid { get; set; }

        public string StripeTokenId { get; set; }

        // Moment in time when the possibility to charge their credit card expires. If we pass this they will not be able to charge the users credit card.
        public DateTime Expires { get; set; }

        // Credit card company static fee. Example those 30 cents always charged by google / transaction
        public decimal CreditCardCompanyProcessorDeductedFee { get; set; }
        // Credit card company percentage on transaction fee
        public decimal RDNDeductedFee { get; set; }

        public virtual IList<InvoiceItem> Items { get; set; }
        public virtual IList<InvoiceDuesItem> DuesItems { get; set; }
        public virtual IList<InvoiceWriterPayoutItem> WriterPayouts{ get; set; }
        public virtual IList<InvoicePayment> Payments { get; set; }
        public virtual IList<InvoiceLogs> Logs { get; set; }
        public virtual IList<InvoiceRefund> Refunds { get; set; }
        public virtual InvoicePaywall Paywall { get; set; }
        public virtual InvoiceSubscription Subscription { get; set; }
        public virtual InvoiceContactInfo InvoiceShipping { get; set; }
        public virtual InvoiceContactInfo InvoiceBilling { get; set; }
        public virtual CurrencyExchangeRate CurrencyRate { get; set; }
        
        // Merchant in RDN system
        [Required]
        public virtual Merchant Merchant { get; set; }

        public Invoice()
        {
            WriterPayouts = new List<InvoiceWriterPayoutItem>();
            DuesItems = new List<InvoiceDuesItem>();
            Items = new List<InvoiceItem>();
            Payments = new List<InvoicePayment>();
            Logs = new List<InvoiceLogs>();
            Refunds = new List<InvoiceRefund>();
            InvoicePaid = false;
        }

        public void SetInvoiceStatus(InvoiceStatus status)
        {
            InvoiceStatus = (byte)status;
        }

        public InvoiceStatus GetInvoiceStatus()
        {
            return (InvoiceStatus)InvoiceStatus;
        }
    }
}
