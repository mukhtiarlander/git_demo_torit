using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Payment.Enums;

namespace RDN.Library.Classes.Store.Classes
{
    public class ShoppingCartItem
    {
        public int ShoppingCartItemId { get; set; }
        public string Name { get; set; }
        public string ArticleNumber { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal BasePrice { get; set; }
        public decimal Price { get; set; }
        public double Weight { get; set; }
        public decimal Shipping { get; set; }

        public Currency Currency { get; set; }

    }
}
