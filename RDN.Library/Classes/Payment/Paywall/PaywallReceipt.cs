using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Billing.Enums;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Enums;
using RDN.Portable.Classes.Payment.Enums;

namespace RDN.Library.Classes.Payment.Paywall
{
    public class PaywallReceipt
    {
        public string NameOfPayment { get; set; }
        public string DescriptionOfPayment { get; set; }
        public long PaywallId { get; set; }
        public Guid InvoiceId { get; set; }
        public Guid MerchantId { get; set; }
        public DateTime Expires { get; set; }
        public string GeneratedPassword { get; set; }
        public string Location { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountRefunded { get; set; }
        public string EmailForReceipt { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public string EmailOfMerchant { get; set; }

        public static PaywallReceipt GetReceiptForInvoice(Guid invoiceId)
        {
            PaywallReceipt re = new PaywallReceipt();
            try
            {
                PaymentGateway pg = new PaymentGateway();
                var invoice = pg.GetDisplayInvoice(invoiceId);
                re.InvoiceStatus = invoice.InvoiceStatus;
                re.InvoiceId = invoice.InvoiceId;
                re.NameOfPayment = invoice.Paywall.Name;
                re.DescriptionOfPayment = invoice.Paywall.Description;
                re.EmailForReceipt = invoice.InvoiceBilling.Email;
                re.PaywallId = invoice.Paywall.PaywallId;
                re.Expires = invoice.Paywall.ValidUntil;
                re.GeneratedPassword = invoice.Paywall.PaywallPassword;
                re.AmountPaid = invoice.Paywall.Price;
                re.AmountRefunded = invoice.Refunds.Sum(x => x.RefundAmount);
                re.MerchantId= invoice.Merchant.MerchantId;
                re.Location = invoice.Paywall.PaywallLocation;
                            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return re;
        }
    }
}
