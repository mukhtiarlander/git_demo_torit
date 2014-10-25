using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using RDN.Library.Classes.Store.Classes;
using RDN.Library.DataModels.Store;

namespace RDN.Store.Models
{
    public class CheckOut
    {
        public CheckOut()
        {
            CurrenciesConverted = new Dictionary<string, decimal>();
        }

        public Guid MerchantId { get; set; }
        public string MerchantName { get; set; }
        public StoreShoppingCart ShoppingCart { get; set; }
        public List<SelectListItem> ShippingOptions { get; set; }
        // When a value is selected in the dropdown list, it ends up in this variable
        public string ShippingOptionSelectedId { get; set; }
        public List<SelectListItem> PaymentProviders { get; set; }
        public string PaymentProviderId { get; set; }
        public bool AcceptPayPal { get; set; }
        public bool AcceptStripe { get; set; }
        public double TaxRate { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalShipping { get; set; }
        public decimal TotalExclVat { get; set; }
        public decimal TotalInclVat { get; set; }
        public int TotalItemsCount { get; set; }
        public string Currency { get; set; }
        public decimal CurrencyCost { get; set; }
        public Dictionary<string, decimal> CurrenciesConverted { get; set; }

        public StoreShoppingCartContactInfo BillingAddress { get; set; }
        public StoreShoppingCartContactInfo ShippingAddress { get; set; }

        public string CCNumber { get; set; }
        public string SecurityCode { get; set; }
        public int MonthOfExpiration { get; set; }
        public int YearOfExpiration { get; set; }

        public List<SelectListItem> Countries { get; set; }
        public SelectList Months { get; set; }
        public SelectList Years { get; set; }
        public string StripeKey { get; set; }
        public string Notes { get; set; }
        public bool IsBillingDifferentFromShipping { get; set; }

        [Required]
        public string BillingAddress_FirstName { get; set; }

        [Required]
        public string BillingAddress_LastName { get; set; }

        public string BillingAddress_CompanyName { get; set; }
        [Required]
        public string BillingAddress_Street { get; set; }

        public string BillingAddress_Street2 { get; set; }
        [Required]
        public string BillingAddress_State { get; set; }

        [Required]
        public string BillingAddress_Zip { get; set; }

        [Required]
        public string BillingAddress_City { get; set; }

        [Required]
        public string BillingAddress_Country { get; set; }

        [Required]
        public string BillingAddress_Email { get; set; }

        public string BillingAddress_Phone { get; set; }

        public string BillingAddress_Fax { get; set; }

        [Required]
        public string ShippingAddress_FirstName { get; set; }

        [Required]
        public string ShippingAddress_LastName { get; set; }

        public string ShippingAddress_CompanyName { get; set; }
        [Required]
        public string ShippingAddress_Street { get; set; }

        public string ShippingAddress_Street2 { get; set; }
        [Required]
        public string ShippingAddress_State { get; set; }

        [Required]
        public string ShippingAddress_Zip { get; set; }

        [Required]
        public string ShippingAddress_City { get; set; }

        [Required]
        public string ShippingAddress_Country { get; set; }

        [Required]
        public string ShippingAddress_Email { get; set; }

        public string ShippingAddress_Phone { get; set; }

        public string ShippingAddress_Fax { get; set; }
    }
}
