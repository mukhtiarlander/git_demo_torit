using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Payment.Paypal
{
    public class PayPalTransaction
    {
        public string is_primary_receiver { get; set; }
        public string id_for_sender_txn { get; set; }
        public string receiver { get; set; }
        public string paymentType { get; set; }
        public string invoiceId { get; set; }
        public string configurationName { get; set; }
        public string amount { get; set; }
        public string status { get; set; }
        public string id { get; set; }
        public string status_for_sender_txn { get; set; }
        public string pending_reason { get; set; }
    }
}
