using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Payment.Classes.Invoice;
using RDN.Library.Classes.Payment.Enums;
using RDN.Portable.Classes.Payment.Enums;

namespace RDN.Library.Classes.Payment.Classes.Display
{
    public class DisplayInvoice
    {
        public Guid InvoiceId { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public DateTime InvoiceStatusUpdated { get; set; }
        public PaymentProvider PaymentProvider { get; set; }
        public Guid ShoppingCartId { get; set; }
        // Public note, visible to the user
        public string Note { get; set; }
        public string AdminNote { get; set; }
        // Total price before tax
        public decimal TotalBeforeTax { get; set; }
        // Tax rate
        public double TaxRate { get; set; }
        // Tax
        public decimal Tax { get; set; }
        // Total price
        public decimal TotalIncludingTax { get; set; }
        // Shipping
        public decimal ShippingCost { get; set; }
        public string CustomerId { get; set; }
        public bool CanRefundCustomer { get; set; }

        // Currency
        public string Currency { get; set; }
        public decimal CurrencyCost { get; set; }
        // Shipping type
        public ShippingType ShippingType { get; set; }
        public bool InvoicePaid { get; set; }
        // Credit card company static fee. Example those 30 cents always charged by google / transaction
        public decimal CreditCardCompanyProcessorDeductedFee { get; set; }
        // Credit card company percentage on transaction fee
        public decimal RDNDeductedFee { get; set; }
        public int TotalItemsBeingSold { get; set; }

        public InvoiceSubscription Subscription { get; set; }
        public InvoicePaywall Paywall { get; set; }
        public IList<InvoiceItem> InvoiceItems { get; set; }
        public IList<InvoiceDuesItem> DuesItems { get; set; }
        public IList<InvoiceRefund> Refunds { get; set; }
        public InvoiceContactInfo InvoiceBilling { get; set; }
        public InvoiceContactInfo InvoiceShipping { get; set; }
        public InvoiceContactInfo SellersAddress { get; set; }

        public DateTime Created { get; set; }
        public decimal RefundAmount { get; set; }

        public Guid UserId { get; set; }

        public OverviewMerchant Merchant { get; set; }

        public DisplayInvoice()
        {
            Refunds = new List<InvoiceRefund>();
            Merchant = new OverviewMerchant();
            InvoiceItems = new List<InvoiceItem>();
            InvoiceShipping = new InvoiceContactInfo();
            InvoiceBilling = new InvoiceContactInfo();
            SellersAddress = new InvoiceContactInfo();
        }
    }
}
