using System.ComponentModel.DataAnnotations;

namespace RDN.Library.Classes.Payment.Classes.Invoice
{
    public class InvoiceRefund
    {
        public long  RefundId{ get; set; }
        public decimal RefundAmount { get; set; }
    }
}
