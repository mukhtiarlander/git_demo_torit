using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment;

namespace RDN.Library.Classes.Billing.Classes
{
    public class LeagueBillingHistory
    {
        public Guid LeagueId { get; set; }
        public string LeagueName { get; set; }
        public List<LeagueReceipt> Receipts { get; set; }

        public static LeagueBillingHistory GetReceiptsForLeague(Guid leagueId)
        {
            LeagueBillingHistory history = new LeagueBillingHistory();
            history.LeagueId = leagueId;
            var league = League.LeagueFactory.GetLeague(leagueId);
            history.LeagueName = league.Name;
            try
            {
                PaymentGateway pg = new PaymentGateway();
                var invoices = pg.GetAllInvoiceSubscriptionsForLeague(leagueId);
                foreach (var invoice in invoices)
                {
                    LeagueReceipt re = new LeagueReceipt();
                    re.InvoiceId = invoice.InvoiceId;
                    re.LeagueId = invoice.Subscription.InternalObject;
                    re.Expires = invoice.Subscription.ValidUntil;
                    re.AmountPaid = invoice.Subscription.Price;
                    re.EmailForReceipt = league.Email;
                    re.Status = invoice.InvoiceStatus;
                    history.Receipts.Add(re);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return history;
        }
        public LeagueBillingHistory()
        {
            Receipts = new List<LeagueReceipt>();
        }

    }
}
