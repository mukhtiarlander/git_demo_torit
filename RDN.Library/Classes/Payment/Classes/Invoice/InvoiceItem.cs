using System.ComponentModel.DataAnnotations;

namespace RDN.Library.Classes.Payment.Classes.Invoice
{
    public class InvoiceItem
    {
        public string Name { get; set; }
        public string ArticleNumber { get; set; }
        public string Article2Number { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal BasePrice { get; set; }
        public decimal BaseShipping { get; set; }
        public decimal Price { get; set; }
        public decimal TotalShipping { get; set; }
        public double Weight { get; set; }
        public long SizeOfItem { get; set; }
        public string  SizeOfItemName{ get; set; }
        public int ColorOfItem { get; set; }
        public string ColorName { get; set; }
        public string ColorHex { get; set; }
        public long StoreItemId { get; set; }
        public long InvoiceItemId { get; set; }
    }
}
