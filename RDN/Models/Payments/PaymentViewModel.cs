using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDN.Models.Payments
{
    public class PaymentViewModel
    {
        public string StripeKey { get; set; }

        public bool SuccessfullyCharged{ get; set; }

    }
}