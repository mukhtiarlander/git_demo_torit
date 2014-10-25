using RDN.Library.Classes.Payment.Enums;
using RDN.Library.DataModels.Context;
using RDN.Portable.Classes.Payment.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Payment.Paypal
{
    public class PaypalDbFactory
    {
        public static void ViewDeadPaypalPayments()
        {
            var dc = new ManagementContext();
            var status = (byte)InvoiceStatus.Not_Started;
            var invoices = dc.Invoices.Where(x => x.InvoiceStatus == status).ToList();
        }
    }
}
