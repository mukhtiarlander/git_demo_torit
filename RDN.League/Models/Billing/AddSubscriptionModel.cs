using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Billing.Classes;

namespace RDN.League.Models.Billing
{
    public class AddSubscriptionModel : AddSubscription
    {
        public List<SelectListItem> Countries { get; set; }
        public SelectList Months { get; set; }
        public SelectList Years { get; set; }
        public string StripeKey { get; set; }
    }
}