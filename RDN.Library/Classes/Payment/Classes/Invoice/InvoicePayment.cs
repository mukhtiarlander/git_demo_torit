using System;

namespace RDN.Library.Classes.Payment.Classes.Invoice
{

    public class InvoicePayment
    {
        public int InvoicePaymentId { get; set; }
        // Payment provider order or id number
        public string PaymentProviderId { get; set; }
        // Amount payed        
        public decimal Amount { get; set; }
        // Payment details
        public string Details { get; set; }
        // Date the payment was registered        
        public DateTime Registered { get; set; }
    }
}
