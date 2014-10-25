using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Payment.Enums.Stripe
{
    /// <summary>
    /// used to verify what the state is returning from Stripe.
    /// Are we using a store return or something more?
    /// </summary>
    public enum StripeStateReturnCodeEnum
    {
        store = 1,
        merchant = 2
    }
}
