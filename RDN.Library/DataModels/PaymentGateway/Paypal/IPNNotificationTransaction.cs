using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Paypal
{
    [Table("RDN_Paypal_IPN_Transaction")]
    public class IPNNotificationTransaction : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IPNTransactionId { get; set; }

        public string is_primary_receiver { get; set; }
        public string id_for_sender_txn { get; set; }
        public string receiver { get; set; }
        public string paymentType { get; set; }
        public string amount { get; set; }
        public string invoiceId { get; set; }
        public string status { get; set; }
        public string id { get; set; }
        public string status_for_sender_txn { get; set; }
        public string pending_reason { get; set; }
        [Required]
        public virtual IPNNotification IPNNotification{get;set;}

    }
}
