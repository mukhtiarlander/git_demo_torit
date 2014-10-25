using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Invoices
{
    [Table("RDN_Invoice_Items")]
    public class InvoiceItem //: InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long InvoiceItemId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string ArticleNumber { get; set; }
        [MaxLength(255)]
        public string Article2Number { get; set; }
        public string Description { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }
        public decimal Shipping { get; set; }
        [Required]
        public double Weight { get; set; }

        public long StoreItemId { get; set; }

        public long SizeOfItem { get; set; }
        public virtual Color.Color ColorOfItem { get; set; }

        // Reference to the invoice
        [Required]
        public virtual Invoice Invoice { get; set; }
    }
}
