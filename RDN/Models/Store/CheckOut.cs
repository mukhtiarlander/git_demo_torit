using System;
using System.Collections.Generic;
using System.Web.Mvc;
using RDN.Library.Classes.Store.Classes;
using RDN.Library.DataModels.Store;

namespace RDN.Models.Store
{
    public class CheckOut
    {        
        public Guid MerchantId { get; set; }
        public StoreShoppingCart ShoppingCart { get; set; }        
        public List<SelectListItem> ShippingOptions { get; set; }
        // When a value is selected in the dropdown list, it ends up in this variable
        public string ShippingOptionSelectedId { get; set; }
        public List<SelectListItem> PaymentProviders { get; set; }
        public string PaymentProviderId { get; set; }
        public double TaxRate { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalShipping { get; set; }
        public decimal TotalExclVat { get; set; }
        public decimal TotalInclVat { get; set; }
        public int TotalItemsCount { get; set; }
        public RDN.Library.Classes.Payment.Enums.Currency Currency { get; set; }

        public StoreShoppingCartContactInfo BillingAddress { get; set; }
        public StoreShoppingCartContactInfo ShippingAddress { get; set; }

        
        
    }
}
