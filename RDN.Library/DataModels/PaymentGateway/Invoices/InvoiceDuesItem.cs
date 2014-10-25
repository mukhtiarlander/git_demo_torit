using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Invoices
{
    [Table("RDN_Invoice_Dues_Items")]
    public class InvoiceDuesItem//: InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceDuesItemId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid DuesId { get; set; }
        public long DuesItemId { get; set; }
        public Guid MemberPaidId { get; set; }
        public decimal BasePrice { get; set; }
        public decimal PriceAfterFees { get; set; }
        public int WhoPaysFees { get; set; }
        public decimal RDNationsFees { get; set; }
        public decimal ProcessorFees { get; set; }

        // Reference to the invoice
        [Required]
        public virtual Invoice Invoice { get; set; }
    }
}
