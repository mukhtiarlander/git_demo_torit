using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Cache;
using RDN.Library.Classes.Billing.Enums;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment;
using RDN.Portable.Classes.Controls.Dues;

namespace RDN.Library.Classes.Dues
{
    public class DuesReceiptFactory
    {
        
        public static DuesReceipt GetReceiptForDues(Guid invoiceId)
        {
            DuesReceipt re = new DuesReceipt();
            try
            {
                PaymentGateway pg = new PaymentGateway();
                var invoice = pg.GetDisplayInvoice(invoiceId);
                var duesItem = invoice.DuesItems.FirstOrDefault();
                var mem = MemberCache.GetMemberDisplay(duesItem.MemberPaidId);
                re.InvoiceId = invoice.InvoiceId;
                var league = MemberCache.GetLeagueOfMember(duesItem.MemberPaidId);
                re.LeagueId = league.LeagueId;
                re.LeagueName = league.Name;
                re.BasePrice = duesItem.BasePrice;
                re.PriceAfterFees = duesItem.PriceAfterFees;
                re.Fees = duesItem.ProcessorFees;
                re.PaidDuesForMonth = duesItem.PaidForDate;
                re.MemberPaid = mem.DerbyName;
                re.EmailForReceipt = mem.Email;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return re;
        }
    }
}
