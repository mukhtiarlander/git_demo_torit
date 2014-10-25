using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Payment.Enums.Stripe
{
    //name of the actual event types in stripe
    public enum StripeEventTypeEnum
    {
        charge_failed = 1,
        charge_dispute_closed = 2,
        customer_created = 3,
        invoice_created = 4,
        charge_succeeded = 5,
        invoice_payment_succeeded = 6,
        customer_subscription_created = 7,
        customer_subscription_updated = 8,
    }


}
