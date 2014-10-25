using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.Payment.Enums
{
    public enum InvoiceStatus
    {
        Not_Started = 0,
        Started = 1,
        Failed = 2,
        Awaiting_Payment = 3,
        Payment_Can_Be_Charged = 4,
        Payment_Successful = 5,
        Awaiting_Shipping = 6,
        Shipped = 7,
        Archived_Item_Completed = 8,
        Cancelled = 9,
        Subscription_Running = 10,
        Pending_Payment_From_Paypal = 11,
        //telling stripe that when the charge succeeds, go ahead and update the leagues subscription date.
        Subscription_Should_Be_Updated_On_Charge = 12,
        Paypal_Email_Not_Confirmed = 13,
        Pending_Payment_From_Paywall = 14,
        Refund_Started = 15,
        Refunded = 16,
        Partially_Refunded = 17,
        Card_Was_Declined = 18,
        Payment_Awaiting_For_Mass_Payout = 19,
        Stripe_Customer_Created_And_Charged = 20
    }
}
