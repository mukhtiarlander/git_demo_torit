using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Store.Classes
{
    public class StoreItemJson
    {
        public int StoreItemId { get; set; }

        public string Name { get; set; }
        public string NameTrimmed { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Currency { get; set; }
        public string PhotoUrl { get; set; }
        public string PhotoAlt { get; set; }
        public string ShopName { get; set; }
        public string ShopMerchantId { get; set; }
        public string ShopNameTrimmed { get; set; }
    }
}
