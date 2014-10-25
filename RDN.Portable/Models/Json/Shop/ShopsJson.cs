using RDN.Portable.Models.Json.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Models.Json.Calendar
{
    [DataContract]
    public class ShopsJson : DataJson
    {
        [DataMember]
        public List<ShopItemJson> Items { get; set; }
        public ShopsJson()
        {
            Items = new List<ShopItemJson>();
        }
    }
}
