using System;
using System.Collections.Generic;
using RDN.Library.Classes.Payment.Enums;
using RDN.Portable.Classes.Payment.Enums;

namespace RDN.Library.Classes.Payment.Classes.Invoice
{
    public class Invoice
    {
        Invoice()
        {
            ItemsDues = new List<InvoiceDuesItem>();
            ItemsInvoice = new List<InvoiceItem>();
            FinancialData = new InvoiceFinancial();
            RNWriterPayouts = new List<InvoiceWriterPayout>();
            Currency = "USD";
            ShippingType = ShippingType.None;
        }

        public Guid InvoiceId { get; set; }
        // Merchant id in RDN system
        public Guid MerchantId { get; set; }
        // InvoicePayment provider, stripe, google, paypal etc.        
        public PaymentProvider PaymentProvider { get; set; }
        public string PaymentProviderCustomerId { get; set; }
        public string PaymentProviderRefundedId { get; set; }
        // Current status of the invoice        
        public InvoiceStatus InvoiceStatus { get; set; }
        // Note, only visible in the admin section of RDN
        public string AdminNote { get; set; }
        // Public note, visible to the user
        public string Note { get; set; }

        public PaymentMode Mode { get; set; }
        /// <summary>
        /// if the user is logged in when buying the item, this is their id.
        /// </summary>
        public Guid UserId { get; set; }
        public Guid ShoppingCartId { get; set; }
        public string StripeToken { get; set; }
        public DateTime Created { get; set; }

        public ChargeTypeEnum ChargeType { get; set; }

        // Financial stuff
        public InvoiceFinancial FinancialData { get; set; }
        // Currency
        public string Currency { get; set; }
        // Shipping type
        public ShippingType ShippingType { get; set; }

        public InvoiceSubscription Subscription { get; set; }
        public InvoicePaywall Paywall { get; set; }
        public IList<InvoiceItem> ItemsInvoice { get; set; }
        public IList<InvoiceDuesItem> ItemsDues { get; set; }
        public IList<InvoiceWriterPayout> RNWriterPayouts{ get; set; }
        public InvoiceContactInfo InvoiceShipping { get; set; }
        public InvoiceContactInfo InvoiceBilling { get; set; }
        public InvoiceContactInfo SellersAddress{ get; set; }

        public static Invoice CreateNewInvoice()
        {
            return new Invoice();
        }
    }
}
