using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.Classes.Dues.Enums;
using RDN.Portable.Classes.Controls.Dues.Enums;

namespace RDN.Library.Classes.Payment.Classes.Invoice
{
    public class InvoiceDuesItem
    {
        public string Name { get; set; }
        public Guid  DuesId{ get; set; }
        public long  DuesItemId{ get; set; }
        public Guid MemberPaidId { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public decimal PriceAfterFees { get; set; }
        public WhoPaysProcessorFeesEnum WhoPaysFees { get; set; }
        public decimal SiteFees { get; set; }
        public decimal ProcessorFees { get; set; }
        public DateTime PaidForDate { get; set; }
    }
}
