using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Payment.Enums.Stripe
{

    /// <summary>
    ///name of the actual subscription plans within Stripe 
    /// </summary>
    public enum StripePlanNames
    {
        Monthly_Plan = 1,
        Six_Month_League_Subscription = 6,
        Three_Month_League_Subscription = 3,
        Yearly_League_Subscription = 12,
        Monthly_RN_Sponsor=1001
    }
}
