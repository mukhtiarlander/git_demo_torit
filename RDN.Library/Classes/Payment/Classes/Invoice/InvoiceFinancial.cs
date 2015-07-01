using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Payment.Enums;

namespace RDN.Library.Classes.Payment.Classes.Invoice
{
    public class InvoiceFinancial
    {
        // Total price for the items (excluding shipping, tax and credit card fees)
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
        public decimal ShippingCost { get; set; }

        public decimal SiteFees { get; set; }
        public decimal PriceSubtractingSiteFees { get; set; }

        public decimal CCProcessorFees { get; set; }

        public decimal RefundAmount { get; set; }
    }
}
