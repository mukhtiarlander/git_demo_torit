using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment.Classes.Display;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Store;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Messages;
using RDN.Library.DataModels.PaymentGateway.Paypal;
using RDN.Utilities.Config;
using RDN.Portable.Config;
using RDN.Portable.Classes.Payment.Enums;
using RDN.Library.Classes.Mobile;
using RDN.Library.Classes.Mobile.Enums;
using Common.Site.AppConfig;
using RDN.Library.Classes.Config;
using RDN.Library.Classes.Api.Email;

namespace RDN.Library.Classes.Payment.Paypal
{
    public class IPNHandler
    {

        public bool IsLive { get; set; }
        public string PostUrl { get; set; }

        /// <summary>
        /// This is the reponse back from the http post back to PayPal.
        /// Possible values are "VERIFIED" or "INVALID"
        /// </summary>
        public string Response { get; set; }

        public PayPalMessage PaypalMessage { get; set; }

        CustomConfigurationManager _configManager;
        EmailManagerApi _emailManager;
        /// <summary>
        /// valid strings are "TEST" for sandbox use 
        /// "LIVE" for production use
        /// </summary>
        /// <param name="mode"></param>
        public IPNHandler(bool isLive, HttpContext context)
        {
            try
            {
                IsLive = isLive;
                PostUrl = PaypalPayment.GetBaseUrl(isLive);
                PaypalMessage = this.FillIPNProperties(context);

                _configManager = new CustomConfigurationManager(PaypalMessage.ConfigurationName);
                _emailManager = new EmailManagerApi(_configManager.GetSubElement(StaticConfig.BaseApiUrl).Value, _configManager.GetSubElement(StaticConfig.ApiAuthenticationKey).Value);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        //public void InsertNewIPNNotification()
        //{
        //    try
        //    {
        //        ManagementContext db = new ManagementContext();
        //        IPNNotification paypal = new IPNNotification();

        //        try
        //        {
        //            if (PaypalMessage.Invoice != null)
        //            {
        //                if (PaypalMessage.Invoice.Contains(':'))
        //                    PaypalMessage.Invoice = PaypalMessage.Invoice.Split(':').First();
        //                paypal.InvoiceId = new Guid(PaypalMessage.Invoice);
        //            }

        //        }
        //        catch (Exception exception)
        //        {
        //            ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: PaypalMessage.ToString());
        //        }
        //        paypal.Business_Id = PaypalMessage.Business;
        //        paypal.Custom = PaypalMessage.Custom;
        //        paypal.DateTime_Received = DateTime.UtcNow.ToString();
        //        paypal.Item_Name = PaypalMessage.ItemName;
        //        paypal.Item_Number = PaypalMessage.ItemNumber;
        //        paypal.Memo = PaypalMessage.Memo;
        //        paypal.Notify_Version = PaypalMessage.NotifyVersion;
        //        paypal.Payer_Address = PaypalMessage.PayerAddress;
        //        paypal.Payer_Address_Status = PaypalMessage.PayerAddressStatus;
        //        paypal.Payer_Business_Name = PaypalMessage.PayerBusinessName;
        //        paypal.Payer_City = PaypalMessage.PayerCity;
        //        paypal.Payer_Country = PaypalMessage.PayerCountry;
        //        paypal.Pay_Key = PaypalMessage.PayKey;
        //        paypal.Payer_Country_Code = PaypalMessage.PayerCountryCode;
        //        paypal.Payer_Email = PaypalMessage.PayerEmail;
        //        paypal.Payer_First_Name = PaypalMessage.PayerFirstName;
        //        paypal.Payer_Id = PaypalMessage.PayerID;
        //        paypal.Payer_Last_Name = PaypalMessage.PayerLastName;
        //        paypal.Payer_Phone = PaypalMessage.PayerPhone;
        //        paypal.Payer_State = PaypalMessage.PayerState;
        //        paypal.Payer_Status = PaypalMessage.PayerStatus;
        //        paypal.Payer_Zip = PaypalMessage.PayerZipCode;
        //        paypal.Payment_Date = PaypalMessage.PaymentDate;
        //        paypal.Payment_Gross = PaypalMessage.PaymentGross;
        //        paypal.Payment_Status = PaypalMessage.PaymentStatus;
        //        paypal.Status = PaypalMessage.Status;
        //        paypal.Payment_Type = PaypalMessage.PaymentType;
        //        paypal.Paypal_Transaction_TXN_Id = PaypalMessage.TXN_ID;
        //        paypal.Paypals_Payment_Fee = PaypalMessage.PaymentFee;
        //        paypal.Pending_Reason = PaypalMessage.PendingReason;
        //        paypal.Post_Url = this.PostUrl;
        //        paypal.Quantity = PaypalMessage.Quantity;
        //        paypal.Quantity_Cart_Items = PaypalMessage.QuantityCartItems;
        //        paypal.Receiver_Email = PaypalMessage.ReceiverEmail;
        //        paypal.Receiver_Id = PaypalMessage.ReceiverID;
        //        paypal.Request_Length = PaypalMessage.RequestLength;
        //        paypal.Response = this.Response;
        //        paypal.Shipping_Method = PaypalMessage.ShippingMethod;
        //        paypal.Tax = PaypalMessage.Tax;
        //        paypal.To_Email = PaypalMessage.ToEmail;
        //        paypal.TXN_Type = PaypalMessage.TXN_Type;
        //        paypal.Verify_Sign = PaypalMessage.VerifySign;
        //        paypal.log_default_shipping_address_in_transaction = PaypalMessage.log_default_shipping_address_in_transaction;
        //        paypal.action_type = PaypalMessage.action_type;
        //        paypal.ipn_notification_url = PaypalMessage.ipn_notification_url;
        //        paypal.transaction_type = PaypalMessage.transaction_type;
        //        paypal.charset = PaypalMessage.charset;
        //        paypal.sender_email = PaypalMessage.sender_email;
        //        paypal.cancel_url = PaypalMessage.cancel_url;
        //        paypal.fees_payer = PaypalMessage.fees_payer;
        //        paypal.return_url = PaypalMessage.return_url;
        //        paypal.reverse_all_parallel_payments_on_error = PaypalMessage.reverse_all_parallel_payments_on_error;
        //        paypal.payment_request_date = PaypalMessage.payment_request_date;

        //        foreach (var t in PaypalMessage.Transactions)
        //        {
        //            IPNNotificationTransaction tr = new IPNNotificationTransaction();
        //            tr.is_primary_receiver = t.is_primary_receiver;
        //            tr.id_for_sender_txn = t.id_for_sender_txn;
        //            tr.receiver = t.receiver;
        //            tr.paymentType = t.paymentType;
        //            tr.invoiceId = t.invoiceId;
        //            tr.amount = t.amount;
        //            tr.status = t.status;
        //            tr.id = t.id;
        //            tr.status_for_sender_txn = t.status_for_sender_txn;
        //            tr.pending_reason = t.pending_reason;
        //            paypal.Transactions.Add(tr);
        //        }

        //        db.PaypalIPN.Add(paypal);
        //        db.SaveChanges();
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: PaypalMessage.ToString());
        //    }
        //}

        /// <summary>
        /// message checks the status of the order and notifies you via email the status.
        /// </summary>
        public void CheckStatus()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PaypalPayment.GetBaseUrl(IsLive));
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                byte[] bytes = HttpContext.Current.Request.BinaryRead(HttpContext.Current.Request.ContentLength);
                string str = Encoding.ASCII.GetString(bytes) + "&cmd=_notify-validate";
                request.ContentLength = str.Length;
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
                writer.Write(str);
                writer.Close();
                StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
                this.Response = reader.ReadToEnd();
                reader.Close();
                bool isResponseHandled = HandleResponseFromPaypal();
                if (isResponseHandled == false)
                    throw new Exception("Response Wasn't Handled");
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: this.Response);
            }
        }
        private bool HandleResponseFromPaypal()
        {
            try
            {
                CustomConfigurationManager customConfig = new CustomConfigurationManager();
                if (!String.IsNullOrEmpty(PaypalMessage.ConfigurationName))
                    customConfig.GetElement(PaypalMessage.ConfigurationName);

                //we use the adaptive payment API to handle dues and payments throught the store
                //verified items come from the subscriptions handler
                switch (this.Response)
                {
                    //only subscriptions offer a verified status.  So we leave half of this switch statement open for subscriptions.
                    case "VERIFIED":
                        //find the invoice number...
                        var trans = PaypalMessage.Transactions.FirstOrDefault();
                        string invoiceId = String.Empty;
                        if (trans != null && !String.IsNullOrEmpty(trans.invoiceId))
                            invoiceId = trans.invoiceId;
                        else if (!String.IsNullOrEmpty(PaypalMessage.Invoice))
                            invoiceId = PaypalMessage.Invoice;
                        else
                            _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Paypal: Can't Find Invoice ID Problem",PaypalMessage.ToString(), Common.EmailServer.Library.Classes.Enums.EmailPriority.Normal);

                        switch (PaypalMessage.PaymentStatus)
                        {
                            case "Completed":
                                CompletePaypalPayment(invoiceId);
                                return true;
                            case "Pending":
                                switch (PaypalMessage.PendingReason)
                                {
                                    case "address":
                                    case "authorization":
                                    case "echeck":
                                    case "intl":
                                    case "multi-currency":
                                    case "unilateral":
                                    case "upgrade":
                                    case "verify":
                                    case "other":
                                    default:
                                        PendingPaypalPayment(invoiceId);
                                        return true;
                                }
                            case "Failed":
                            case "Denied":
                                FailedPaypalPayment(invoiceId);
                                return true;
                            default:
                                _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Paypal: Status is Defaulted..??????", PaypalMessage.ToString());
                                return true;
                        }
                    //email buyer and me a reciept of order.

                    //web payments from dues or stores are considered invalid because we are using 
                    //the Paypal Adpative payments API to handle the dues and store purchases.
                    case "INVALID":
                    default:
                        _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Paypal: Can't Find Payment Problem", this.Response + " " + PaypalMessage.ToString());
                        return true;

                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: PaypalMessage.ToString());
            }
            return false;
        }

        private void HandleDuesPaymentPending(DisplayInvoice invoice)
        {
            try
            {
                var duesItem = invoice.DuesItems.FirstOrDefault();

                PaymentGateway pg = new PaymentGateway();
                pg.SetInvoiceStatus(invoice.InvoiceId, InvoiceStatus.Pending_Payment_From_Paypal);

                //email people.
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(ServerConfig.URL_TO_CLEAR_MEMBER_CACHE + duesItem.MemberPaidId));
                WebClient client1 = new WebClient();
                client1.DownloadStringAsync(new Uri(ServerConfig.URL_TO_CLEAR_MEMBER_CACHE_API + duesItem.MemberPaidId));

                _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Dues Payment Pending", PaypalMessage.ToString());
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
                    _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, member.Email, EmailServer.EmailServer.DEFAULT_SUBJECT + " Dues Payment Receipt", emailData, EmailServer.EmailServerLayoutsEnum.DuesPaymentMadeForUser.ToString());
                    if (league != null && !String.IsNullOrEmpty(league.Email))
                    {
                        //sends email to league for notification of their payment.
                        _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, league.Email, EmailServer.EmailServer.DEFAULT_SUBJECT + " Dues Payment Made", emailData, EmailServer.EmailServerLayoutsEnum.DuesPaymentMadeForLeague.ToString());
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
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: PaypalMessage.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="reportInformation"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public bool HandleDuesPayments(DisplayInvoice invoice, string reportInformation, string customerId = null)
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
                    client.DownloadStringAsync(new Uri(ServerConfig.URL_TO_CLEAR_MEMBER_CACHE + duesItem.MemberPaidId));
                    client1.DownloadStringAsync(new Uri(ServerConfig.URL_TO_CLEAR_MEMBER_CACHE_API + duesItem.MemberPaidId));

                    _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Dues Payment Made", reportInformation);
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
                            _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, member.UserName, EmailServer.EmailServer.DEFAULT_SUBJECT + " Dues Payment Receipt", emailData, EmailServer.EmailServerLayoutsEnum.DuesPaymentMadeForUser.ToString());
                        else if (!String.IsNullOrEmpty(member.Email))
                            _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, member.Email, EmailServer.EmailServer.DEFAULT_SUBJECT + " Dues Payment Receipt", emailData, EmailServer.EmailServerLayoutsEnum.DuesPaymentMadeForUser.ToString());

                        if (league != null && !String.IsNullOrEmpty(league.Email))
                        {
                            //sends email to league for notification of their payment.
                            _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, league.Email, EmailServer.EmailServer.DEFAULT_SUBJECT + " Dues Payment Made", emailData, EmailServer.EmailServerLayoutsEnum.DuesPaymentMadeForLeague.ToString());
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
                    _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Paypal: Dues Updates Were not successful", reportInformation);
                }
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: reportInformation);
            }
            return false;
        }


        private Classes.Display.DisplayInvoice CompletePaypalPayment(string invoiceId)
        {
            try
            {
                PaymentGateway pg = new PaymentGateway();
                var invoice = pg.GetDisplayInvoice(new Guid(invoiceId));
                if (invoice != null)
                {
                    if (invoice.Subscription != null)
                    {
                        _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Paypal: New Payment Complete!!", invoice.InvoiceId + " Amount:" + invoice.Subscription.Price + PaypalMessage.ToString());

                        RDN.Library.Classes.League.LeagueFactory.UpdateLeagueSubscriptionPeriod(invoice.Subscription.ValidUntil, false, invoice.Subscription.InternalObject);
                        pg.SetInvoiceStatus(invoice.InvoiceId, InvoiceStatus.Payment_Successful, PaypalMessage.PayKey);
                        WebClient client = new WebClient();
                        client.DownloadDataAsync(new Uri(ServerConfig.URL_TO_CLEAR_LEAGUE_MEMBER_CACHE + invoice.Subscription.InternalObject));
                        WebClient client1 = new WebClient();
                        client1.DownloadDataAsync(new Uri(ServerConfig.URL_TO_CLEAR_LEAGUE_MEMBER_CACHE_API + invoice.Subscription.InternalObject));
                    }
                    else if (invoice.Paywall != null)
                    {
                        _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Paypal: New Paywall Complete!!", invoice.InvoiceId + " Amount:" + invoice.Paywall.Price + PaypalMessage.ToString());


                        Paywall.Paywall pw = new Paywall.Paywall();
                        pw.HandlePaywallPayments(invoice, null, PaypalMessage.PayKey);
                    }
                    else if (invoice.DuesItems.Count > 0)
                        HandleDuesPayments(invoice, PaypalMessage.ToString(), PaypalMessage.PayKey);
                    else if (invoice.InvoiceItems.Count > 0)
                    {
                        StoreGateway sg = new StoreGateway();
                        sg.HandleStoreItemPayments(invoice, PaypalMessage.ToString(), PaypalMessage.PayKey);
                    }
                    else
                        _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Paypal: Haven't Found Items for the invoice", PaypalMessage.ToString());
                }
                else
                {
                    _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Paypal: Couldn't Find Invoice", PaypalMessage.ToString());
                }
                return invoice;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: PaypalMessage.ToString());
            }
            return null;
        }
        private Classes.Display.DisplayInvoice PendingPaypalPayment(string invoiceId)
        {
            try
            {
                PaymentGateway pg = new PaymentGateway();
                var invoice = pg.GetDisplayInvoice(new Guid(invoiceId));
                if (invoice != null)
                {
                    if (invoice.Subscription != null)
                    {
                        InvoiceFactory.EmailLeagueAboutSuccessfulSubscription(invoice.Subscription.InternalObject, invoice.InvoiceId, invoice.Subscription.Price, invoice.Subscription.ValidUntil, invoice.InvoiceBilling.Email);

                        RDN.Library.Classes.League.LeagueFactory.UpdateLeagueSubscriptionPeriod(invoice.Subscription.ValidUntil, false, invoice.Subscription.InternalObject);

                        _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Paypal: New Payment Pending!!", invoice.InvoiceId + " Amount:" + invoice.Subscription.Price + PaypalMessage.ToString());
                        pg.SetInvoiceStatus(invoice.InvoiceId, InvoiceStatus.Pending_Payment_From_Paypal);
                        WebClient client = new WebClient();
                        client.DownloadDataAsync(new Uri(ServerConfig.URL_TO_CLEAR_LEAGUE_MEMBER_CACHE + invoice.Subscription.InternalObject));
                        WebClient client1 = new WebClient();
                        client1.DownloadDataAsync(new Uri(ServerConfig.URL_TO_CLEAR_LEAGUE_MEMBER_CACHE_API + invoice.Subscription.InternalObject));
                    }
                    else if (invoice.Paywall != null)
                    {
                        _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Paypal: New Paywall Payment Pending!!", invoice.InvoiceId + " Amount:" + invoice.Paywall.Price + PaypalMessage.ToString());
                        pg.SetInvoiceStatus(invoice.InvoiceId, InvoiceStatus.Pending_Payment_From_Paypal);
                    }
                    else if (invoice.DuesItems.Count > 0)
                        HandleDuesPaymentPending(invoice);
                    else if (invoice.InvoiceItems.Count > 0)
                    {
                        StoreGateway sg = new StoreGateway();
                        sg.HandleStoreItemPaymentPending(invoice, PaypalMessage.ToString());
                    }
                    else
                    {
                        _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Paypal: Couldn't Find Subscription", PaypalMessage.ToString());
                    }
                }
                else
                {
                    _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Paypal: Couldn't Find Invoice", PaypalMessage.ToString());
                }
                return invoice;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: PaypalMessage.ToString());
            }
            return null;
        }
        private Classes.Display.DisplayInvoice FailedPaypalPayment(string invoiceId)
        {
            PaymentGateway pg = new PaymentGateway();

            try
            {
                var invoice = pg.GetDisplayInvoice(new Guid(invoiceId));
                if (invoice != null)
                {
                    if (invoice.Subscription != null)
                    {
                        _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Paypal: Payment Failed", PaypalMessage.ToString());

                        InvoiceFactory.EmailLeagueAboutFailedSubscription(invoice.Subscription.InternalObject, invoice.InvoiceId, invoice.Subscription.Price, invoice.Subscription.ValidUntil, invoice.InvoiceBilling.Email);

                        DateTime dateToGoBackTo = invoice.Subscription.ValidUntil.AddDays(-invoice.Subscription.SubscriptionPeriodLengthInDays);
                        pg.SetInvoiceStatus(invoice.InvoiceId, InvoiceStatus.Failed);

                        RDN.Library.Classes.League.LeagueFactory.UpdateLeagueSubscriptionPeriod(dateToGoBackTo, false, invoice.Subscription.InternalObject);
                    }
                    else if (invoice.Paywall != null)
                    {
                        _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Paypal: Paywall Payment Failed", PaypalMessage.ToString());

                        pg.SetInvoiceStatus(invoice.InvoiceId, InvoiceStatus.Failed);
                    }
                    else
                    {
                        _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Paypal:Failed Payment", PaypalMessage.ToString());
                    }
                }
                else
                {
                    _emailManager.SendEmailAsync(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Paypal: Couldn't Find Invoice", PaypalMessage.ToString());
                }
                return invoice;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: PaypalMessage.ToString());
            }
            return null;
        }

   
        private PayPalMessage FillIPNProperties(HttpContext context)
        {
            PayPalMessage message = new PayPalMessage();
            try
            {
                message.RequestLength = context.Request.Form.ToString();
                message.PayerCity = context.Request.Form["address_city"];
                message.PayerCountry = context.Request.Form["address_country"];
                message.PayerCountryCode = context.Request.Form["address_country_code"];
                message.PayerState = context.Request.Form["address_state"];
                message.PayerAddressStatus = context.Request.Form["address_status"];
                message.PayerAddress = context.Request.Form["address_street"];
                message.PayerZipCode = context.Request.Form["address_zip"];
                message.PayerFirstName = context.Request.Form["first_name"];
                message.PayerLastName = context.Request.Form["last_name"];
                message.PayerBusinessName = context.Request.Form["payer_business_name"];
                message.PayerEmail = context.Request.Form["payer_email"];
                message.PayerID = context.Request.Form["payer_id"];
                message.PayerStatus = context.Request.Form["payer_status"];
                message.PayerPhone = context.Request.Form["contact_phone"];
                message.Business = context.Request.Form["business"];
                message.ItemName = context.Request.Form["item_name"];
                message.ItemNumber = context.Request.Form["item_number"];
                message.Quantity = context.Request.Form["quantity"];
                message.ReceiverEmail = context.Request.Form["receiver_email"];
                message.ReceiverID = context.Request.Form["receiver_id"];
                message.Custom = context.Request.Form["custom"];
                message.Memo = context.Request.Form["memo"];
                message.Invoice = context.Request.Form["invoice"];
                if (message.Invoice != null && message.Invoice.Contains(':'))
                {
                    var messages = message.Invoice.Split(':');
                    message.Invoice = messages[0];
                    if (messages.Count() > 1)
                        message.ConfigurationName = messages[1];

                }
                message.Tax = context.Request.Form["tax"];
                message.QuantityCartItems = context.Request.Form["num_cart_items"];
                message.PaymentDate = context.Request.Form["payment_date"];
                message.PaymentStatus = context.Request.Form["payment_status"];
                message.Status = context.Request.Form["status"];
                message.PaymentType = context.Request.Form["payment_type"];
                message.PendingReason = context.Request.Form["pending_reason"];
                message.TXN_ID = context.Request.Form["txn_id"];
                message.TXN_Type = context.Request.Form["txn_type"];
                message.PaymentFee = context.Request.Form["mc_fee"];
                message.PaymentGross = context.Request.Form["mc_gross"];
                message.NotifyVersion = context.Request.Form["notify_version"];
                message.log_default_shipping_address_in_transaction = context.Request.Form["log_default_shipping_address_in_transaction"];
                message.action_type = context.Request.Form["action_type"];
                message.ipn_notification_url = context.Request.Form["ipn_notification_url"];
                message.charset = context.Request.Form["charset"];
                message.transaction_type = context.Request.Form["transaction_type"];
                message.sender_email = context.Request.Form["sender_email"];
                message.cancel_url = context.Request.Form["cancel_url"];
                message.fees_payer = context.Request.Form["fees_payer"];
                message.return_url = context.Request.Form["return_url"];
                message.reverse_all_parallel_payments_on_error = context.Request.Form["reverse_all_parallel_payments_on_error"];
                message.payment_request_date = context.Request.Form["payment_request_date"];
                if (!String.IsNullOrEmpty(context.Request.Form["transaction[0].is_primary_receiver"]))
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (!String.IsNullOrEmpty(context.Request.Form["transaction[" + i + "].is_primary_receiver"]))
                        {
                            PayPalTransaction t = new PayPalTransaction();
                            t.is_primary_receiver = context.Request.Form["transaction[" + i + "].is_primary_receiver"];
                            t.id_for_sender_txn = context.Request.Form["transaction[" + i + "].id_for_sender_txn"];
                            t.receiver = context.Request.Form["transaction[" + i + "].receiver"];
                            t.paymentType = context.Request.Form["transaction[" + i + "].paymentType"];
                            t.amount = context.Request.Form["transaction[" + i + "].amount"];
                            t.invoiceId = context.Request.Form["transaction[" + i + "].invoiceId"];
                            if (!String.IsNullOrEmpty(t.invoiceId) && t.invoiceId.Contains(':'))
                            {
                                var messages = t.invoiceId.Split(':');
                                t.invoiceId = messages[0];
                                if (messages.Count() > 1)
                                    t.configurationName = messages[1];
                            }

                            t.status = context.Request.Form["transaction[" + i + "].status"];
                            t.id = context.Request.Form["transaction[" + i + "].id"];
                            t.status_for_sender_txn = context.Request.Form["transaction[" + i + "].status_for_sender_txn"];
                            t.pending_reason = context.Request.Form["transaction[" + i + "].pending_reason"];
                            message.Transactions.Add(t);
                        }
                        else
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: PaypalMessage.ToString());
            }
            return message;
        }
    }
}
