using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.PaymentGateway.Paypal;
using RDN.Portable.Classes.Payment.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Payment.Paypal
{
    public class PaypalManagerDb
    {
        public static void ViewDeadPaypalPayments()
        {
            var dc = new ManagementContext();
            var status = (byte)InvoiceStatus.Not_Started;
            var invoices = dc.Invoices.Where(x => x.InvoiceStatus == status).ToList();
        }

        public static bool InsertIpnNotification(PayPalMessage paypalMessage)
        {
            try
            {
                ManagementContext db = new ManagementContext();
                IPNNotification paypal = new IPNNotification();

                try
                {
                    if (paypalMessage.Invoice != null)
                    {
                        if (paypalMessage.Invoice.Contains(':'))
                            paypalMessage.Invoice = paypalMessage.Invoice.Split(':').First();
                        paypal.InvoiceId = new Guid(paypalMessage.Invoice);
                    }

                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: paypalMessage.ToString());
                }
                paypal.Business_Id = paypalMessage.Business;
                paypal.Custom = paypalMessage.Custom;
                paypal.DateTime_Received = DateTime.UtcNow.ToString();
                paypal.Item_Name = paypalMessage.ItemName;
                paypal.Item_Number = paypalMessage.ItemNumber;
                paypal.Memo = paypalMessage.Memo;
                paypal.Notify_Version = paypalMessage.NotifyVersion;
                paypal.Payer_Address = paypalMessage.PayerAddress;
                paypal.Payer_Address_Status = paypalMessage.PayerAddressStatus;
                paypal.Payer_Business_Name = paypalMessage.PayerBusinessName;
                paypal.Payer_City = paypalMessage.PayerCity;
                paypal.Payer_Country = paypalMessage.PayerCountry;
                paypal.Pay_Key = paypalMessage.PayKey;
                paypal.Payer_Country_Code = paypalMessage.PayerCountryCode;
                paypal.Payer_Email = paypalMessage.PayerEmail;
                paypal.Payer_First_Name = paypalMessage.PayerFirstName;
                paypal.Payer_Id = paypalMessage.PayerID;
                paypal.Payer_Last_Name = paypalMessage.PayerLastName;
                paypal.Payer_Phone = paypalMessage.PayerPhone;
                paypal.Payer_State = paypalMessage.PayerState;
                paypal.Payer_Status = paypalMessage.PayerStatus;
                paypal.Payer_Zip = paypalMessage.PayerZipCode;
                paypal.Payment_Date = paypalMessage.PaymentDate;
                paypal.Payment_Gross = paypalMessage.PaymentGross;
                paypal.Payment_Status = paypalMessage.PaymentStatus;
                paypal.Status = paypalMessage.Status;
                paypal.Payment_Type = paypalMessage.PaymentType;
                paypal.Paypal_Transaction_TXN_Id = paypalMessage.TXN_ID;
                paypal.Paypals_Payment_Fee = paypalMessage.PaymentFee;
                paypal.Pending_Reason = paypalMessage.PendingReason;
                paypal.Post_Url = paypalMessage.PostUrl;
                paypal.Quantity = paypalMessage.Quantity;
                paypal.Quantity_Cart_Items = paypalMessage.QuantityCartItems;
                paypal.Receiver_Email = paypalMessage.ReceiverEmail;
                paypal.Receiver_Id = paypalMessage.ReceiverID;
                paypal.Request_Length = paypalMessage.RequestLength;
                paypal.Response = paypalMessage.Response;
                paypal.Shipping_Method = paypalMessage.ShippingMethod;
                paypal.Tax = paypalMessage.Tax;
                paypal.To_Email = paypalMessage.ToEmail;
                paypal.TXN_Type = paypalMessage.TXN_Type;
                paypal.Verify_Sign = paypalMessage.VerifySign;
                paypal.log_default_shipping_address_in_transaction = paypalMessage.log_default_shipping_address_in_transaction;
                paypal.action_type = paypalMessage.action_type;
                paypal.ipn_notification_url = paypalMessage.ipn_notification_url;
                paypal.transaction_type = paypalMessage.transaction_type;
                paypal.charset = paypalMessage.charset;
                paypal.sender_email = paypalMessage.sender_email;
                paypal.cancel_url = paypalMessage.cancel_url;
                paypal.fees_payer = paypalMessage.fees_payer;
                paypal.return_url = paypalMessage.return_url;
                paypal.reverse_all_parallel_payments_on_error = paypalMessage.reverse_all_parallel_payments_on_error;
                paypal.payment_request_date = paypalMessage.payment_request_date;

                foreach (var t in paypalMessage.Transactions)
                {
                    IPNNotificationTransaction tr = new IPNNotificationTransaction();
                    tr.is_primary_receiver = t.is_primary_receiver;
                    tr.id_for_sender_txn = t.id_for_sender_txn;
                    tr.receiver = t.receiver;
                    tr.paymentType = t.paymentType;
                    tr.invoiceId = t.invoiceId;
                    tr.amount = t.amount;
                    tr.status = t.status;
                    tr.id = t.id;
                    tr.status_for_sender_txn = t.status_for_sender_txn;
                    tr.pending_reason = t.pending_reason;
                    paypal.Transactions.Add(tr);
                }

                db.PaypalIPN.Add(paypal);
                int c = db.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: paypalMessage.ToString());
            }
            return false;
        }
    }
}
