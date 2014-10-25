using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Payment.Enums;

namespace RDN.Library.Classes.Payment.Classes.Display
{
    public class OverviewMerchant
    {
        public Guid MerchantId { get; set; }
        public Guid InternalReference { get; set; }
        public Guid PrivateManagerId { get; set; }
        // Like federation or league or user
        public MerchantInternalReference InternalReferenceType { get; set; }
        public Guid UserId { get; set; }
        public string OwnerName { get; set; }
        public string ShopName { get; set; }
        public decimal AmountOnAccount { get; set; }
        public decimal PayedFeesToRDN { get; set; }
        public int NumberOfItemsForSale { get; set; }
        public DateTime Created { get; set; }
        public bool AcceptPaymentsViaPaypal { get; set; }
        public string PaypalEmail { get; set; }
        public bool AcceptPaymentsViaStripe { get; set; }
        public string StripeConnectKey { get; set; }
        public string StripeConnectToken { get; set; }
        public string StripePublishableKey { get; set; }
        public string StripeRefreshToken { get; set; }
        public string StripeUserId { get; set; }
        public string StripeTokenType { get; set; }
        public string OrderPayedNotificationEmail { get; set; }
        public bool IsPublished { get; set; }
        public string WelcomeMessage { get; set; }
        public string Currency { get; set; }
        public decimal CurrencyCost { get; set; }
        public string ReturnUrl { get; set; }
        
    }
}
