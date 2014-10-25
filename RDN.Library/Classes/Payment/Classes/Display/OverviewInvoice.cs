using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Payment.Classes.Invoice;
using RDN.Library.Classes.Payment.Enums;
using RDN.Portable.Classes.Payment.Enums;

namespace RDN.Library.Classes.Payment.Classes.Display
{
    public class OverviewInvoice
    {
        public Guid InvoiceId { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public string Note { get; set; }
        public decimal TotalIncludingTax { get; set; }
        public string Currency { get; set; }
        public bool IsPaid { get; set; }
        public DateTime InvoiceStatusUpdated { get; set; }
        public DateTime Created { get; set; }
    }
}
