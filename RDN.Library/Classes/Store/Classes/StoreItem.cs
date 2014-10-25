using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Store.Display;
using RDN.Library.DataModels.Store;
using RDN.Portable.Classes.Store.Enums;
using RDN.Portable.Classes.Imaging;
using RDN.Portable.Classes.Colors;

namespace RDN.Library.Classes.Store.Classes
{

    public class StoreItem
    {
        public long InvoiceItemId { get; set; }
        public string rate { get; set; }
        public string ratings { get; set; }
        public string ReviewTitle { get; set; }
        public string ReviewComment{ get; set; }
        public long ReviewId { get; set; }
        public int StoreItemId { get; set; }
        public bool CanPickUpLocally { get; set; }
        public bool WillPickUpLocally { get; set; }
        public string Name { get; set; }
        public string NameTrimmed { get; set; }
        public string ArticleNumber { get; set; }
        public string Article2Number { get; set; }
        public string Description { get; set; }
        public double Weight { get; set; }
        public decimal Price { get; set; }
        public decimal PriceInclTax { get; set; }
        public decimal BaseTaxOnItem { get; set; }
        public decimal TotalTaxOnItem { get; set; }
        public decimal Shipping { get; set; }
        public decimal ShippingAdditional { get; set; }
        public string Note { get; set; }
        public string NoteHtml { get; set; }
        public bool IsPublished { get; set; }
        public StoreItemTypeEnum ItemType { get; set; }
        public int ItemTypeEnum { get; set; }
        public string ColorTempSelected { get; set; }
        public StoreItemShirtSizesEnum ItemSize { get; set; }
        public long ItemSizeEnum { get; set; }
        public int QuantityInStock { get; set; }
        public bool CanRunOutOfStock { get; set; }
        public Store Store { get; set; }
        public StoreCategory Category { get; set; }
        public Guid CartId { get; set; }
        public int CartItemsCount { get; set; }
        public bool AcceptsPayPal { get; set; }
        public bool AcceptsStripe { get; set; }
        public string Currency { get; set; }
        public decimal CurrencyCost { get; set; }
        public string Views { get; set; }
        public List<PhotoItem> Photos { get; set; }
        public List<ColorDisplay> Colors { get; set; }
        public List<ItemReview> Reviews { get; set; }

        public StoreItem()
        {
            Photos = new List<PhotoItem>();
            Store = new Store();
            Category = new StoreCategory();
            Colors = new List<ColorDisplay>();
            Reviews = new List<ItemReview>();
        }
    }
}
