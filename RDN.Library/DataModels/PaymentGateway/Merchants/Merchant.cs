using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.PaymentGateway.Invoices;
using RDN.Library.DataModels.Store;
using RDN.Library.DataModels.PaymentGateway.Money;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Merchants
{
    [Table("RDN_Merchants")]
    public class Merchant : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid MerchantId { get; set; }
        // Reference to for instance a league or a federation guid id
        [Required]
        public Guid InternalReference { get; set; }
        //used to manage the store.  We find the store with te internal Reference Id, but then users must use the private managerid to manage it.
        //we do this because the internal reference id is made public and we need some secret key.
        [Required]
        public Guid PrivateManagerId { get; set; }
        // Like federation or league, class->payment gateway->enums
        public byte InternalReferenceType { get; set; }
        [MaxLength(255)]
        [Required]
        public string OwnerName { get; set; }
        public string Description { get; set; }
        public string WelcomeMessage { get; set; }
        [MaxLength(255)]
        public string ShopName { get; set; }
        // Will be notified when an order goes into shipping state
        [MaxLength(255)]
        [Obsolete("Only need to use the OrderPayedNotificationEmail")]
        public string ShippingNotificationEmail { get; set; }
        // Will be notified when an order goes into paymentsuccessful state
        [MaxLength(255)]
        public string OrderPayedNotificationEmail { get; set; }

        public bool AcceptPaymentsViaPaypal { get; set; }
        public string PaypalEmail { get; set; }
        public bool AcceptPaymentsViaStripe { get; set; }
        public string StripeConnectKey { get; set; }
        public string StripeConnectToken { get; set; }
        public string StripePublishableKey { get; set; }
        public string StripeRefreshToken { get; set; }
        public string StripeUserId { get; set; }
        public string StripeTokenType { get; set; }
        public bool IsRDNation { get; set; }
        public bool IsPublished { get; set; }

        [Required]
        public byte MerchantAccountStatus { get; set; }

        // On for instance google you need to ship your items (regardless of what items). This will let you skip that state and automatically archive the order.
        public bool AutoShipWhenPaymentIsReceived { get; set; }
        // Accepts the payment automatically regardless of the fees
        public bool AutoAcceptPayment { get; set; }

        [MaxLength(255)]
        public string Bank { get; set; }
        [MaxLength(255)]
        public string BankAccount { get; set; }

        // What currency do this guy use? Classes->Payment gateway->enums -> currency
        //[Required]
        //public byte Currency { get; set; }

        [Required]
        public double TaxRate { get; set; }

        [Required]
        public decimal RDNFixedFee { get; set; }
        [Required]
        public double RDNPercentageFee { get; set; }

        [Required]
        public decimal AmountOnAccount { get; set; }
        [Required]
        public decimal PayedFeesToRDN { get; set; }

        public virtual CurrencyExchangeRate CurrencyRate { get; set; }
        public virtual IList<Invoices.Invoice> Invoices { get; set; }
        public virtual IList<MerchantFeeSlip> FeeSlips { get; set; }
        public virtual IList<MerchantTransaction> Transactions { get; set; }
        public virtual IList<Paywall.Paywall> Paywalls { get; set; }
        public virtual IList<StoreItem> Items { get; set; }
        public virtual IList<StoreItemCategory> Categories { get; set; }
        public virtual IList<ShippingTable> ShippingTable { get; set; }
        public virtual IList<MerchantRDNationFee> RDNationFees { get; set; }
        public virtual IList<Location.Location> Locations { get; set; }
        public Merchant()
        {
            MerchantAccountStatus accountStatus = Classes.Payment.Enums.MerchantAccountStatus.Active;
            MerchantAccountStatus = (byte)accountStatus;

            Invoices = new List<Invoice>();
            FeeSlips = new List<MerchantFeeSlip>();
            Transactions = new List<MerchantTransaction>();
            Items = new List<StoreItem>();
            Categories = new List<StoreItemCategory>();
            ShippingTable = new List<ShippingTable>();
            RDNationFees = new List<MerchantRDNationFee>();
            Paywalls = new List<Paywall.Paywall>();
            Locations = new List<Location.Location>();
        }
    }
}
