using RDN.Portable.Classes.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Store.Classes
{
    public class StoreItemDisplay : StoreItem
    {
        [Obsolete("User Merchant instead")]
        public Guid MerchantId { get; set; }
        public Guid InternalId { get; set; }
        public Guid PrivateManagerId { get; set; }
        public Guid CartId { get; set; }
        public int CartItemsCount { get; set; }
        public int ShoppingCartItemId { get; set; }
        public int QuantityOrdered { get; set; }
        public decimal BasePrice { get; set; }

        public bool HasExtraSmall { get; set; }
        public bool HasSmall { get; set; }
        public bool HasMedium { get; set; }
        public bool HasLarge { get; set; }
        public bool HasExtraLarge { get; set; }
        public bool HasXXLarge { get; set; }
        public bool HasXXXLarge { get; set; }
        public string  ColorHex { get; set; }
        public int ColorAGB { get; set; }

        public Store Merchant { get; set; }

        public StoreItemDisplay()
        {
            Merchant = new Store();
            Colors = new List<ColorDisplay>();
        }
    }
}
