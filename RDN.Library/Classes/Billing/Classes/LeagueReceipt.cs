using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Billing.Enums;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Enums;
using RDN.Portable.Classes.Payment.Enums;
using RDN.Portable.Classes.Billing.Enums;

namespace RDN.Library.Classes.Billing.Classes
{
    public class LeagueReceipt
    {
        public Guid LeagueId { get; set; }
        public string LeagueName { get; set; }
        public Guid InvoiceId { get; set; }
        public DateTime Expires { get; set; }
        public CurrentStatusOfBillingEnum MembershipStatus { get; set; }
        public decimal AmountPaid { get; set; }
        public string EmailForReceipt { get; set; }
        public InvoiceStatus Status { get; set; }

        public static LeagueReceipt GetReceiptForInvoice(Guid invoiceId)
        {
            LeagueReceipt re = new LeagueReceipt();
            try
            {
                PaymentGateway pg = new PaymentGateway();
                var invoice = pg.GetDisplayInvoice(invoiceId);
                re.InvoiceId = invoice.InvoiceId;
                re.LeagueId = invoice.Subscription.InternalObject;
                var league = League.LeagueFactory.GetLeague(re.LeagueId);
                re.LeagueName = league.Name;
                re.Expires = invoice.Subscription.ValidUntil;
                re.MembershipStatus = CurrentStatusOfBillingEnum.League_Membership;
                re.AmountPaid = invoice.Subscription.Price;
                re.EmailForReceipt = league.Email;
                re.Status = invoice.InvoiceStatus;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return re;
        }
    }
}
