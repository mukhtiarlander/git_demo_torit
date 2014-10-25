using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Payment.Paywall;

namespace RDN.Library.Cache.Singletons
{
    public class PaywallViewers
    {

        public List<Paywall> Paywalls { get; set; }

        static PaywallViewers instance = new PaywallViewers();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static PaywallViewers()
        {

        }

        public static PaywallViewers Instance
        {
            get
            {
                return instance;
            }
        }


        public bool IsCurrentlyViewingPaywall(long paywallId, string password)
        {
            if (instance.Paywalls == null)
                instance.Paywalls = new List<Paywall>();

            var wall = instance.Paywalls.Where(x => x.PaywallId == paywallId && x.PasswordForPaywall == password).FirstOrDefault();
            if (wall != null)
            {
                if (wall.LastViewedPaywall > DateTime.UtcNow)
                    return true;
            }
            else
            {
                Paywall w = new Paywall();
                w.PaywallId = paywallId;
                w.PasswordForPaywall = password;
                w.LastViewedPaywall = DateTime.UtcNow.AddMinutes(2);
                instance.Paywalls.Add(w);
            }
            return false;
        }
        public bool UpdateCurrentlyViewing(long paywallId, string password)
        {
            if (instance.Paywalls == null)
                instance.Paywalls = new List<Paywall>();

            var wall = instance.Paywalls.Where(x => x.PaywallId == paywallId && x.PasswordForPaywall == password).FirstOrDefault();
            if (wall != null)
            {
                wall.LastViewedPaywall = DateTime.UtcNow.AddMinutes(2);
            }
            else
            {
                Paywall w = new Paywall();
                w.PaywallId = paywallId;
                w.PasswordForPaywall = password;
                w.LastViewedPaywall = DateTime.UtcNow.AddMinutes(2);
                instance.Paywalls.Add(w);
            }
            return true;
        }



    }
}
