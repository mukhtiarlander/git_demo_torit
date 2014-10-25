using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Billing.Enums;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment;
using RDN.Library.DataModels.Context;
using RDN.Portable.Classes.Billing.Enums;

namespace RDN.Library.Classes.Billing.Classes
{
    public class LeagueBilling
    {
        public Guid LeagueId { get; set; }
        public Guid InvoiceId { get; set; }
        public string LeagueName { get; set; }
        public CurrentStatusOfBillingEnum MembershipStatus { get; set; }
        public DateTime Expires { get; set; }
        public DateTime LastBilledOn { get; set; }
        public AutoRenewalTypeEnum AutoRenewal { get; set; }
        public string StripeKey { get; set; }
        public bool IsSubscriptionCanceled { get; set; }
        public string PaymentProviderCustomerId { get; set; }

        public static LeagueBilling GetCurrentBillingStatus(Guid leagueId)
        {
            LeagueBilling bi = new LeagueBilling();
            try
            {
                var dc = new ManagementContext();
                PaymentGateway pg = new PaymentGateway();
                var invoice = pg.GetLatestInvoiceSubscriptionForLeagueId(leagueId);
                
                bi.LeagueId = leagueId;
                bi.LeagueName = dc.Leagues.Where(x => x.LeagueId == leagueId).Select(x => x.Name).FirstOrDefault();
                if (invoice != null)
                {
                    bi.InvoiceId = invoice.InvoiceId;
                    bi.Expires = invoice.Subscription.ValidUntil;
                    bi.LastBilledOn = invoice.Subscription.Created;
                    bi.MembershipStatus = CurrentStatusOfBillingEnum.League_Membership;
                    bi.PaymentProviderCustomerId = invoice.PaymentProviderCustomerId;
                }
                else
                {
                    bi.MembershipStatus = CurrentStatusOfBillingEnum.None;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return bi;
        }
    }
}
