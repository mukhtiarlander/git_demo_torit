using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Payment.Enums;

namespace RDN.Library.Classes.Store.Classes
{
    public class Store
    {
        public Guid MerchantId { get; set; }
        public Guid PrivateManagerId { get; set; }
        public Guid InternalId { get; set; }

        public string Name { get; set; }
        public string NameTrimmed { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public decimal CurrencyCost { get; set; }
        public List<StoreItemDisplay> StoreItems { get; set; }
        public string  StripePublishableKey{ get; set; }
        //price for the shopping cart store.
        public decimal TotalPrice { get; set; }
        public decimal TotalShipping { get; set; }
        public decimal TotalAfterShipping { get; set; }
        public double TaxRate{ get; set; }
        // ToDo: Add mroe store details such as shipping table and such
        // Easy way done to get the t-shirt sales for rdn running

        public Store()
        {
            StoreItems = new List<StoreItemDisplay>();
        }
    }
}
