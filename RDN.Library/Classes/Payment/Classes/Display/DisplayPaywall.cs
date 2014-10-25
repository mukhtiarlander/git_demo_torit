using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Payment.Classes.Display
{
    public class DisplayPaywall : OverviewMerchant
    {


        public List<Paywall.Paywall> Paywalls { get; set; }
        /// <summary>
        /// this is the retur url for the merchant account
        /// </summary>
        public string ReturnUrl { get; set; }
        public DisplayPaywall()
        {
            Paywalls = new List<Paywall.Paywall>();
        }
    }
}
