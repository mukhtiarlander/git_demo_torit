using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RDN.Library.Classes.Store.Classes;
using RDN.Library.DataModels.Store;

namespace RDN.Library.Classes.Store.Display
{
    public class CheckOut
    {        
        public Guid MerchantId { get; set; }
        public string MerchantName { get; set; }
        public StoreShoppingCart ShoppingCart { get; set; }
        public Dictionary<int, CheckOutShippingRow> ShippingOptionsRaw { get; set; }
        // When a value is selected in the dropdown list, it ends up in this variable
        public string ShippingOptionSelectedId { get; set; }
        public double TaxRate { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalShipping { get; set; }
        public decimal TotalExclVat { get; set; }
        public decimal TotalInclVat { get; set; }
        public int TotalItemsCount { get; set; }
        public string Currency { get; set; }
        public decimal CurrencyCost { get; set; }
        public bool AcceptsPayPal { get; set; }
        public bool WillPickUpAtStore { get; set; }
        public bool AcceptsStripe { get; set; }
        public string StripePublishableKey { get; set; }

        public StoreShoppingCartContactInfo BillingAddress { get; set; }
        public StoreShoppingCartContactInfo ShippingAddress { get; set; }
        public StoreShoppingCartContactInfo SellersAddress { get; set; }

        [Required]
        [MaxLength(50)]
        public string BillingAddress_FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string BillingAddress_LastName { get; set; }

        public string BillingAddress_CompanyName { get; set; }
        [Required]
        [MaxLength(50)]
        public string BillingAddress_Street { get; set; }

        public string BillingAddress_Street2 { get; set; }
        [Required]
        [MaxLength(50)]
        public string BillingAddress_State { get; set; }

        [Required]
        [MaxLength(50)]
        public string BillingAddress_Zip { get; set; }

        [Required]
        [MaxLength(50)]
        public string BillingAddress_City { get; set; }

        [Required]
        [MaxLength(50)]
        public string BillingAddress_Country { get; set; }

        [Required]
        [MaxLength(50)]
        public string BillingAddress_Email { get; set; }

        public string BillingAddress_Phone { get; set; }

        public string BillingAddress_Fax { get; set; }

        [Required]
        [MaxLength(50)]
        public string ShippingAddress_FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string ShippingAddress_LastName { get; set; }

        public string ShippingAddress_CompanyName { get; set; }
        [Required]
        [MaxLength(50)]
        public string ShippingAddress_Street { get; set; }

        public string ShippingAddress_Street2 { get; set; }
        [Required]
        [MaxLength(50)]
        public string ShippingAddress_State { get; set; }

        [Required]
        [MaxLength(50)]
        public string ShippingAddress_Zip { get; set; }

        [Required]
        [MaxLength(50)]
        public string ShippingAddress_City { get; set; }

        [Required]
        [MaxLength(50)]
        public string ShippingAddress_Country { get; set; }

        [Required]
        [MaxLength(50)]
        public string ShippingAddress_Email { get; set; }

        public string ShippingAddress_Phone { get; set; }

        public string ShippingAddress_Fax { get; set; }
    }
}
