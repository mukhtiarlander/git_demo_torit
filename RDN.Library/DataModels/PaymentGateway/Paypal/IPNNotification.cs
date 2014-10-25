using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Paypal
{
    [Table("RDN_Paypal_IPN")]
    public class IPNNotification : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IPNId { get; set; }

        public System.Guid InvoiceId { get; set; }
        public string Paypal_Transaction_TXN_Id { get; set; }
        public string Post_Url { get; set; }
        public string Pay_Key { get; set; }
        public string Response { get; set; }
        public string Request_Length { get; set; }
        public string To_Email { get; set; }
        public string Business_Id { get; set; }
        public string TXN_Type { get; set; }
        public string Payment_Status { get; set; }
        public string Receiver_Email { get; set; }
        public string Receiver_Id { get; set; }
        public string Item_Name { get; set; }
        public string Item_Number { get; set; }
        public string Quantity { get; set; }
        public string Quantity_Cart_Items { get; set; }
        public string Custom { get; set; }
        public string Memo { get; set; }
        public string Tax { get; set; }
        public string Payment_Gross { get; set; }
        public string Payment_Fee { get; set; }
        public string Payment_Date { get; set; }
        public string Paypals_Payment_Fee { get; set; }
        public string Payer_Email { get; set; }
        public string Payer_Phone { get; set; }
        public string Payer_Business_Name { get; set; }
        public string Pending_Reason { get; set; }
        public string Shipping_Method { get; set; }
        public string Payer_First_Name { get; set; }
        public string Payer_Last_Name { get; set; }
        public string Payer_Address { get; set; }
        public string Payer_City { get; set; }
        public string Payer_State { get; set; }
        public string Payer_Zip { get; set; }
        public string Payer_Country { get; set; }
        public string Payer_Country_Code { get; set; }
        public string Payer_Address_Status { get; set; }
        public string Payer_Status { get; set; }
        public string Status { get; set; }
        public string Payer_Id { get; set; }
        public string Payment_Type { get; set; }
        public string Notify_Version { get; set; }
        public string Verify_Sign { get; set; }
        public string DateTime_Received { get; set; }
        public string log_default_shipping_address_in_transaction { get; set; }
        public string action_type { get; set; }
        public string ipn_notification_url { get; set; }
        public string transaction_type { get; set; }
        public string charset { get; set; }
        public string sender_email { get; set; }
        public string cancel_url { get; set; }
        public string fees_payer { get; set; }
        public string return_url { get; set; }
        public string reverse_all_parallel_payments_on_error { get; set; }
        public string payment_request_date { get; set; }

        public virtual IList<IPNNotificationTransaction> Transactions { get; set; }

        public IPNNotification()
        {
            Transactions = new List<IPNNotificationTransaction>();
        }
    }
}
