using RDN.Library.Cache;
using RDN.Library.Classes.Config;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Mobile;
using RDN.Library.Classes.Mobile.Enums;
using RDN.Library.Classes.Payment.Classes.Display;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Store;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.PaymentGateway.Paypal;
using RDN.Portable.Classes.Payment.Enums;
using RDN.Portable.Classes.Url;
using RDN.Portable.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public static bool PendingPayment(Guid invoiceId, PayPalMessage paypalMessage)
        {
            try
            {
                PaymentGateway pg = new PaymentGateway();
                var invoice = pg.GetDisplayInvoice(invoiceId);
                if (invoice != null)
                {
                    if (invoice.Subscription != null)
                    {
                        InvoiceFactory.EmailLeagueAboutSuccessfulSubscription(invoice.Subscription.InternalObject, invoice.InvoiceId, invoice.Subscription.Price, invoice.Subscription.ValidUntil, invoice.InvoiceBilling.Email);

                        RDN.Library.Classes.League.LeagueFactory.UpdateLeagueSubscriptionPeriod(invoice.Subscription.ValidUntil, false, invoice.Subscription.InternalObject);

                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, "Paypal: New Payment Pending!!", invoice.InvoiceId + " Amount:" + invoice.Subscription.Price + paypalMessage.ToString());
                        pg.SetInvoiceStatus(invoice.InvoiceId, InvoiceStatus.Pending_Payment_From_Paypal);
                        WebClient client = new WebClient();
                        client.DownloadDataAsync(new Uri(LibraryConfig.InternalSite + UrlManager.URL_TO_CLEAR_LEAGUE_MEMBER_CACHE + invoice.Subscription.InternalObject));
                        WebClient client1 = new WebClient();
                        client1.DownloadDataAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_LEAGUE_MEMBER_CACHE_API + invoice.Subscription.InternalObject));
                    }
                    else if (invoice.Paywall != null)
                    {
                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, "Paypal: New Paywall Payment Pending!!", invoice.InvoiceId + " Amount:" + invoice.Paywall.Price + paypalMessage.ToString());
                        pg.SetInvoiceStatus(invoice.InvoiceId, InvoiceStatus.Pending_Payment_From_Paypal);
                    }
                    else if (invoice.DuesItems.Count > 0)
                        HandleDuesPaymentPending(invoice, paypalMessage);
                    else if (invoice.InvoiceItems.Count > 0)
                    {
                        StoreGateway sg = new StoreGateway();
                        sg.HandleStoreItemPaymentPending(invoice, paypalMessage.ToString());
                    }
                    else
                    {
                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, "Paypal: Couldn't Find Subscription", paypalMessage.ToString());
                    }
                }
                else
                {
                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, "Paypal: Couldn't Find Invoice", paypalMessage.ToString());
                }
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: paypalMessage.ToString());
            }
            return false;
        }

        public static bool HandleDuesPaymentPending(DisplayInvoice invoice, PayPalMessage paypalMessage)
        {
            try
            {
                var duesItem = invoice.DuesItems.FirstOrDefault();

                PaymentGateway pg = new PaymentGateway();
                pg.SetInvoiceStatus(invoice.InvoiceId, InvoiceStatus.Pending_Payment_From_Paypal);

                //email people.
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(LibraryConfig.InternalSite + UrlManager.URL_TO_CLEAR_MEMBER_CACHE + duesItem.MemberPaidId));
                WebClient client1 = new WebClient();
                client1.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_MEMBER_CACHE_API + duesItem.MemberPaidId));

                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, "Dues Payment Pending", paypalMessage.ToString());
                var member = MemberCache.GetMemberDisplay(duesItem.MemberPaidId);
                var league = MemberCache.GetLeagueOfMember(duesItem.MemberPaidId);
                var settings = Dues.DuesFactory.GetDuesSettings(duesItem.DuesId);
                if (settings != null && member != null)
                {
                    var emailData = new Dictionary<string, string>
                                        {
                                            { "memberName",  member.DerbyName },
                                            { "leagueName", settings.LeagueOwnerName   },
                                            { "invoiceId", invoice.InvoiceId.ToString().Replace("-","")},
                                            { "amountPaid", duesItem.PriceAfterFees.ToString("N2") },
                                            { "baseAmountPaid",duesItem.BasePrice.ToString("N2")  },
                                            { "monthOfDuesPayment",duesItem.PaidForDate.ToShortDateString()},
                                            { "emailForPaypal", settings.PayPalEmailAddress},
                                            { "statusOfPayment",RDN.Portable.Util.Enums.EnumExt.ToFreindlyName( InvoiceStatus.Pending_Payment_From_Paypal)}
                                          };

                    //sends email to user for their payment.
                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, member.Email, LibraryConfig.DefaultEmailSubject + " Dues Payment Receipt", emailData, EmailServer.EmailServerLayoutsEnum.DuesPaymentMadeForUser);
                    if (league != null && !String.IsNullOrEmpty(league.Email))
                    {
                        //sends email to league for notification of their payment.
                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, league.Email, LibraryConfig.DefaultEmailSubject + " Dues Payment Made", emailData, EmailServer.EmailServerLayoutsEnum.DuesPaymentMadeForLeague);
                    }

                    MobileNotificationFactory mnf = new MobileNotificationFactory();
                    mnf.Initialize("Dues Payment Made", "Receipt For Payment", NotificationTypeEnum.DuesPaymentReceipt)
                        .AddId(invoice.InvoiceId)
                        .AddMember(duesItem.MemberPaidId)
                        .SendNotifications();
                    return true;
                }
                else
                {
                    throw new Exception("Settings or Member was null.  Can't send Receipts." + invoice.InvoiceId);
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: paypalMessage.ToString());
            }
            return false;
        }

        public static bool CompletePayment(Guid invoiceId, PayPalMessage paypalMessage)
        {
            try
            {
                PaymentGateway pg = new PaymentGateway();
                var invoice = pg.GetDisplayInvoice(invoiceId);
                if (invoice != null)
                {
                    if (invoice.Subscription != null)
                    {
                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, "Paypal: New Payment Complete!!", invoice.InvoiceId + " Amount:" + invoice.Subscription.Price + paypalMessage.ToString());

                        RDN.Library.Classes.League.LeagueFactory.UpdateLeagueSubscriptionPeriod(invoice.Subscription.ValidUntil, false, invoice.Subscription.InternalObject);
                        pg.SetInvoiceStatus(invoice.InvoiceId, InvoiceStatus.Payment_Successful, paypalMessage.PayKey);
                        WebClient client = new WebClient();
                        client.DownloadDataAsync(new Uri(LibraryConfig.InternalSite + UrlManager.URL_TO_CLEAR_LEAGUE_MEMBER_CACHE + invoice.Subscription.InternalObject));
                        WebClient client1 = new WebClient();
                        client1.DownloadDataAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_LEAGUE_MEMBER_CACHE_API + invoice.Subscription.InternalObject));
                    }
                    else if (invoice.Paywall != null)
                    {
                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, "Paypal: New Paywall Complete!!", invoice.InvoiceId + " Amount:" + invoice.Paywall.Price + paypalMessage.ToString());


                        Paywall.Paywall pw = new Paywall.Paywall();
                        pw.HandlePaywallPayments(invoice, null, paypalMessage.PayKey);
                    }
                    else if (invoice.DuesItems.Count > 0)
                        HandleDuesPayments(invoice, paypalMessage.ToString(), paypalMessage.PayKey);
                    else if (invoice.InvoiceItems.Count > 0)
                    {
                        StoreGateway sg = new StoreGateway();
                        sg.HandleStoreItemPayments(invoice, paypalMessage.ToString(), paypalMessage.PayKey);
                    }
                    else
                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, "Paypal: Haven't Found Items for the invoice", paypalMessage.ToString());
                }
                else
                {
                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, "Paypal: Couldn't Find Invoice", paypalMessage.ToString());
                }
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: paypalMessage.ToString());
            }
            return false;
        }

        public static bool HandleDuesPayments(DisplayInvoice invoice, string reportInformation, string customerId = null)
        {
            try
            {
                var duesItem = invoice.DuesItems.FirstOrDefault();
                bool success = Dues.DuesFactory.PayDuesAmount(duesItem.DuesItemId, duesItem.DuesId, (double)duesItem.BasePrice, duesItem.MemberPaidId, "Paid Via Paypal, Invoice:" + invoice.InvoiceId.ToString().Replace("-", ""));
                PaymentGateway pg = new PaymentGateway();
                pg.SetInvoiceStatus(invoice.InvoiceId, InvoiceStatus.Payment_Successful, customerId);
                if (success)
                {
                    //email people.
                    WebClient client = new WebClient();
                    WebClient client1 = new WebClient();
                    client.DownloadStringAsync(new Uri(LibraryConfig.InternalSite + UrlManager.URL_TO_CLEAR_MEMBER_CACHE + duesItem.MemberPaidId));
                    client1.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_MEMBER_CACHE_API + duesItem.MemberPaidId));

                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, "Dues Payment Made", reportInformation);
                    var member = MemberCache.GetMemberDisplay(duesItem.MemberPaidId);
                    var league = MemberCache.GetLeagueOfMember(duesItem.MemberPaidId);
                    var settings = Dues.DuesFactory.GetDuesSettings(duesItem.DuesId);
                    if (settings != null && member != null)
                    {
                        var emailData = new Dictionary<string, string>
                                        {
                                            { "memberName",  member.DerbyName },
                                            { "leagueName", settings.LeagueOwnerName   },
                                            { "invoiceId", invoice.InvoiceId.ToString().Replace("-","")},
                                            { "amountPaid", duesItem.PriceAfterFees.ToString("N2") },
                                            { "baseAmountPaid",duesItem.BasePrice.ToString("N2")  },
                                            { "monthOfDuesPayment",duesItem.PaidForDate.ToShortDateString()},
                                            { "emailForPaypal", settings.PayPalEmailAddress},
                                            { "statusOfPayment",RDN.Portable.Util.Enums.EnumExt.ToFreindlyName( InvoiceStatus.Pending_Payment_From_Paypal)}
                                          };

                        //sends email to user for their payment.
                        if (!String.IsNullOrEmpty(member.UserName))
                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, member.UserName, LibraryConfig.DefaultEmailSubject + " Dues Payment Receipt", emailData, EmailServer.EmailServerLayoutsEnum.DuesPaymentMadeForUser);
                        else if (!String.IsNullOrEmpty(member.Email))
                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, member.Email, LibraryConfig.DefaultEmailSubject + " Dues Payment Receipt", emailData, EmailServer.EmailServerLayoutsEnum.DuesPaymentMadeForUser);

                        if (league != null && !String.IsNullOrEmpty(league.Email))
                        {
                            //sends email to league for notification of their payment.
                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, league.Email, LibraryConfig.DefaultEmailSubject + " Dues Payment Made", emailData, EmailServer.EmailServerLayoutsEnum.DuesPaymentMadeForLeague);
                        }

                        MobileNotificationFactory mnf = new MobileNotificationFactory();
                        mnf.Initialize("Dues Payment Made", "Receipt For Payment", NotificationTypeEnum.DuesPaymentReceipt)
                            .AddId(invoice.InvoiceId)
                            .AddMember(duesItem.MemberPaidId)
                            .SendNotifications();
                    }
                    else
                    {
                        throw new Exception("Settings or Member was null.  Can't send Receipts." + invoice.InvoiceId);
                    }
                }
                else
                {
                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, "Paypal: Dues Updates Were not successful", reportInformation);
                }
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: reportInformation);
            }
            return false;
        }
        public static bool FailedPayment(Guid invoiceId, PayPalMessage paypalMessage)
        {
            PaymentGateway pg = new PaymentGateway();

            try
            {
                var invoice = pg.GetDisplayInvoice(invoiceId);
                if (invoice != null)
                {
                    if (invoice.Subscription != null)
                    {
                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, "Paypal: Payment Failed", paypalMessage.ToString());

                        InvoiceFactory.EmailLeagueAboutFailedSubscription(invoice.Subscription.InternalObject, invoice.InvoiceId, invoice.Subscription.Price, invoice.Subscription.ValidUntil, invoice.InvoiceBilling.Email);

                        DateTime dateToGoBackTo = invoice.Subscription.ValidUntil.AddDays(-invoice.Subscription.SubscriptionPeriodLengthInDays);
                        pg.SetInvoiceStatus(invoice.InvoiceId, InvoiceStatus.Failed);

                        RDN.Library.Classes.League.LeagueFactory.UpdateLeagueSubscriptionPeriod(dateToGoBackTo, false, invoice.Subscription.InternalObject);
                    }
                    else if (invoice.Paywall != null)
                    {
                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, "Paypal: Paywall Payment Failed", paypalMessage.ToString());

                        pg.SetInvoiceStatus(invoice.InvoiceId, InvoiceStatus.Failed);
                    }
                    else
                    {
                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, "Paypal:Failed Payment", paypalMessage.ToString());
                    }
                }
                else
                {
                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, "Paypal: Couldn't Find Invoice", paypalMessage.ToString());
                }
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: paypalMessage.ToString());
            }
            return false;
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
