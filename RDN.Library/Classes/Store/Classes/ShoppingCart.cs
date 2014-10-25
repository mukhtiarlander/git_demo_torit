using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Store.Classes
{
    public class StoreShoppingCart
    {
        public StoreShoppingCart()
        {
            ShoppingCartId = Guid.NewGuid();
            Stores = new List<Store>();
            //Items = new List<StoreItemDisplay>();
        }
        public StoreShoppingCart(string ip)
        {
            ShoppingCartId = Guid.NewGuid();
            Ip = ip;
            Stores = new List<Store>();
            //Items = new List<StoreItemDisplay>();
        }
        public int ItemsCount { get; set; }
        public Guid ShoppingCartId { get; set; }
        //[Obsolete("Use Stores")]
        //public Guid MerchantId { get; set; }

        public List<Store> Stores { get; set; }

        public string Ip { get; set; }

        public virtual StoreShoppingCartContactInfo BillingAddress { get; set; }
        public virtual StoreShoppingCartContactInfo ShippingAddress { get; set; }

        //public virtual decimal TotalPrice { get; set; }
        //public virtual IList<StoreItemDisplay> Items { get; set; }

    }
}
