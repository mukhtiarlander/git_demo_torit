using RDN.Library.Classes.Store.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.League.Models.Store
{
    public class DisplayStoreModel : DisplayStore
    {

        public SelectList CurrencyList { get; set; }
        public DisplayStoreModel()
        { }
        public DisplayStoreModel(DisplayStore store)
        {
            this.AcceptPaymentsViaPaypal = store.AcceptPaymentsViaPaypal;
            this.AcceptPaymentsViaStripe = store.AcceptPaymentsViaStripe;
            this.AccountStatus = store.AccountStatus;
            this.AmountOnAccount = store.AmountOnAccount;
            this.AutoAcceptPayment = store.AutoAcceptPayment;
            this.AutoShipWhenPaymentIsReceived = store.AutoShipWhenPaymentIsReceived;
            this.Bank = store.Bank;
            this.BankAccount = store.BankAccount;
            this.BetaCode = store.BetaCode;
            this.Created = store.Created;
            this.Currency = store.Currency;
            this.CurrencyCost = store.CurrencyCost;
            this.Description = store.Description;
            this.InternalReference = store.InternalReference;
            this.InternalReferenceType = store.InternalReferenceType;
            this.Invoices = store.Invoices;
            this.IsPublished = store.IsPublished;
            this.IsSite = store.IsSite;
            this.Location = store.Location;
            this.MerchantId = store.MerchantId;
            this.NumberOfItemsForSale = store.NumberOfItemsForSale;
            this.OrderPayedNotificationEmail = store.OrderPayedNotificationEmail;
            this.OwnerName = store.OwnerName;
            this.PayedFeesToRDN = store.PayedFeesToRDN;
            this.PaypalEmail = store.PaypalEmail;
            this.PayPalEmailAddressForPayments = store.PayPalEmailAddressForPayments;
            this.PrivateManagerId = store.PrivateManagerId;
            this.RDNFixedFee = store.RDNFixedFee;
            this.RDNPercentageFee = store.RDNPercentageFee;
            this.ReturnUrl = store.ReturnUrl;
            this.ShippingNotificationEmail = store.ShippingNotificationEmail;
            this.ShopName = store.ShopName;
            this.ShoppingCart = store.ShoppingCart;
            this.StoreCategories = store.StoreCategories;
            this.StoreItems = store.StoreItems;
            this.StripeConnectKey = store.StripeConnectKey;
            this.StripeConnectToken = store.StripeConnectToken;
            this.StripePublishableKey = store.StripePublishableKey;
            this.StripeRefreshToken = store.StripeRefreshToken;
            this.StripeTokenType = store.StripeTokenType;
            this.StripeUserId = store.StripeUserId;
            this.TaxRate = store.TaxRate;
            this.UserId = store.UserId;
            this.WelcomeMessage = store.WelcomeMessage;
        }
    }
}