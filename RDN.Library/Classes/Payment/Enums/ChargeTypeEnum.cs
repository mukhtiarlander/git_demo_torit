using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Payment.Enums
{
    public enum ChargeTypeEnum
    {
        InvoiceItem = 0,
        Subscription = 1,
        DuesItem = 2,
        InStorePurchase = 3,
        SubscriptionUpdated = 4,
        Paywall = 5,
        Refund_Paywall = 6,
        Cancel_Subscription = 7,
        /// <summary>
        /// sets invoice status to set up new mass payment.
        /// </summary>
        RollinNewsWriterPrePayout = 8,
        /// <summary>
        /// sets invoice to pay out all folks that wanted to be paid out.
        /// </summary>
        RollinNewsWriterPayouts = 9,
        Stripe_Checkout = 10
    }
}
