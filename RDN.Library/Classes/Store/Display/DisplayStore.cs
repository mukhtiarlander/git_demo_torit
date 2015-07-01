using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Store.Classes;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Payment.Classes.Display;

namespace RDN.Library.Classes.Store.Display
{
    public class DisplayStore : OverviewMerchant
    {
        
        //public Guid PrivateManagerId { get; set; }

        public string Description { get; set; }
        public int BetaCode { get; set; }
        //public string WelcomeMessage { get; set; }
        public bool AutoShipWhenPaymentIsReceived { get; set; }
        public bool AutoAcceptPayment { get; set; }
        public string ShippingNotificationEmail { get; set; }
        //public string OrderPayedNotificationEmail { get; set; }
        public double TaxRate { get; set; }
        public decimal RDNFixedFee { get; set; }
        public double RDNPercentageFee { get; set; }
        //public decimal AmountOnAccount { get; set; }
        //public decimal PayedFeesToRDN { get; set; }
        public string Bank { get; set; }
        public string BankAccount { get; set; }
        public bool IsSite { get; set; }
        public string PayPalEmailAddressForPayments { get; set; }
        //public bool AcceptPaymentsViaStripe { get; set; }
        //public string StripeConnectKey { get; set; }
        //public string StripeConnectToken { get; set; }
        public MerchantAccountStatus AccountStatus { get; set; }
        //public string Currency { get; set; }
        //public decimal CurrencyCost { get; set; }
        public List<StoreItem> StoreItems { get; set; }
        public List<StoreCategory> StoreCategories { get; set; }
        public StoreShoppingCart ShoppingCart { get; set; }
        public List<DisplayInvoice> Invoices { get; set; }
        public RDN.Portable.Classes.Location.Location Location { get; set; }

        public DisplayStore()
        {
            StoreItems = new List<StoreItem>();
            StoreCategories = new List<StoreCategory>();
            Invoices = new List<DisplayInvoice>();
        }

    }
}
