using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.Classes.Dues.Enums;
using RDN.Portable.Classes.Controls.Dues.Enums;

namespace RDN.Library.Classes.Payment.Classes.Invoice
{
    public class InvoiceWriterPayout
    {
        public string Name { get; set; }
        public Guid PayoutId { get; set; }
        public Guid UserPaidId { get; set; }
        public decimal BasePrice { get; set; }
        public decimal PriceAfterFeeDeduction { get; set; }
        public WhoPaysProcessorFeesEnum WhoPaysFees { get; set; }
        public DateTime PaidDateTime { get; set; }
        public DateTime PaymentRequestedDateTime { get; set; }
    }
}
