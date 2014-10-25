using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Billing.Classes;

namespace RDN.League.Models.Billing
{
    public class UpdateSubscriptionModel : UpdateSubscription
    {
        public bool CancelSubscription { get; set; }
        public string StripeKey { get; set; }
    }
}