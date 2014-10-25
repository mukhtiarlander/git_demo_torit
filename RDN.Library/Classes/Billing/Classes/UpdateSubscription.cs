using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Billing.Enums;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Enums;

namespace RDN.Library.Classes.Billing.Classes
{
    public class UpdateSubscription :AddSubscription
    {
        public string LeagueName { get; set; }
        public bool CancelSubscription { get; set; }
    
    }
}
