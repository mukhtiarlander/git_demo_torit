using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment.Classes.Display;
using RDN.Library.Classes.Payment.Classes.Invoice;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Payment.Enums.Paywall;
using RDN.Library.DataModels.Context;
using RDN.Utilities.Config;
using RDN.Portable.Config;
using RDN.Portable.Classes.Payment.Enums;
using RDN.Library.Classes.Config;

namespace RDN.Library.Classes.Payment.Paywall
{
    public class Paywall
    {
        public Paywall()
        {
            Games = new List<Game.Game>();
            Invoices = new List<DisplayInvoice>();
        }
        public Guid MerchantId { get; set; }
        public long PaywallId { get; set; }
        public string DescriptionOfPaywall { get; set; }
        public string PasswordForPaywall { get; set; }
        public DateTime LastViewedPaywall { get; set; }
        /// <summary>
        /// if the start date and end date have multiple days, this is the 
        /// price for each day entered by the user if they buy just one day.
        /// </summary>
        public decimal DailyPrice { get; set; }
        /// <summary>
        /// this is the price of the full timespan.  User pays once and the entire paywall is paid.
        /// </summary>
        public decimal TimespanPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string StartDateDisplay { get; set; }
        public string EndDateDisplay { get; set; }

        public bool IsEnabled { get; set; }
        public bool IsPaid { get; set; }
        public bool IsRemoved { get; set; }
        public Guid OwnerId { get; set; }
        public bool AcceptPaypal { get; set; }
        public bool AcceptStripe { get; set; }
        public string StripePublishableKey { get; set; }
        public string CCNumber { get; set; }
        public string SecurityCode { get; set; }
        public int MonthOfExpiration { get; set; }
        public int YearOfExpiration { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public List<Game.Game> Games { get; set; }
        public List<DisplayInvoice> Invoices { get; set; }

        public DisplayInvoice GetInvoiceForManager(Guid merchantId, Guid managerId, Guid invoiceId)
        {
            try
            {
                var mc = new ManagementContext();
                var invoice = mc.Invoices.Include("Refunds").Where(x => x.InvoiceId == invoiceId && x.Merchant.MerchantId == merchantId && x.Merchant.PrivateManagerId == managerId).FirstOrDefault();

                return DisplayInvoice(invoice);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new DisplayInvoice();
        }
        public bool UpdatePaywallOrder(DisplayInvoice v)
        {
            try
            {
                var mc = new ManagementContext();
                var invoice = mc.Invoices.Where(x => x.InvoiceId == v.InvoiceId && x.Merchant.MerchantId == v.Merchant.MerchantId && x.Merchant.PrivateManagerId == v.Merchant.PrivateManagerId).FirstOrDefault();
                if (invoice.Paywall != null)
                    invoice.Paywall.ValidUntil = Convert.ToDateTime(v.Paywall.ValidUntilDisplay);
                int c = mc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool UpdatePaywallInvoiceForViewingTime(long PaywallId, string password)
        {
            try
            {

                var dc = new ManagementContext();
                var wall = dc.InvoicePaywalls.Where(x => x.Paywall.PaywallId == PaywallId && x.GeneratedPassword == password).FirstOrDefault();
                if (wall != null)
                {
                    wall.Invoice = wall.Invoice;
                    if (wall.LastViewedPaywall == null)
                        wall.LastViewedPaywall = DateTime.UtcNow;
                    TimeSpan ts = DateTime.UtcNow - wall.LastViewedPaywall.Value;
                    //if its been more than 3 minutes, they left the session.
                    //so we reset the session and make sure they have only been watching for the passed 2 minutes.
                    if (ts.TotalSeconds > 180)
                        wall.SecondsViewedPaywall += 120;
                    else
                        wall.SecondsViewedPaywall += Convert.ToInt64(ts.TotalSeconds);
                    wall.LastViewedPaywall = DateTime.UtcNow;
                    int c = dc.SaveChanges();
                    return c > 0;
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public void HandlePaywallRefund(DisplayInvoice invoice, string additionalReportingInformation = null)
        {
            try
            {
                CompileAndSendRefundEmailsForPaywall(invoice);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: additionalReportingInformation);
            }
        }
        public void HandlePaywallPayments(DisplayInvoice invoice, string additionalReportingInformation = null, string customerId = null)
        {
            try
            {
                PaymentGateway pg = new PaymentGateway();
                //change invoice to ready to be shipped
                pg.SetInvoiceStatus(invoice.InvoiceId, InvoiceStatus.Payment_Successful, customerId);

                CompileAndSendReceiptsEmailsForPaywall(invoice);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: additionalReportingInformation);
            }
        }

        private void CompileAndSendReceiptsEmailsForPaywall(DisplayInvoice invoice, string reportingInformation = null)
        {
            try
            {

                StringBuilder shippingAddress = new StringBuilder();
                InvoiceContactInfo shipping = null;
                if (invoice.InvoiceShipping == null)
                    shipping = invoice.InvoiceBilling;
                else
                    shipping = invoice.InvoiceShipping;

                var emailData = new Dictionary<string, string>
                                        {
                                                                                        { "PaywallDescription", invoice.Paywall.Description},
                                            { "invoiceId", invoice.InvoiceId.ToString().Replace("-","")},
                                            { "amountPaid", "$"+ invoice.Paywall.Price.ToString("N2") },
                                            { "receiptLink", "<a href='"+ServerConfig.WEBSITE_DEFAULT_LOCATION+"/streaming/receipt/"+invoice.InvoiceId.ToString().Replace("-","")+"'>Your Receipt and Order Status</a>"},
                                            { "buyerEmail", invoice.InvoiceBilling.Email},
                                            { "sellerMessageLink", "<a href='"+ServerConfig.WEBSITE_INTERNAL_DEFAULT_LOCATION+"/messages/new/paywall/"+ invoice.Merchant.MerchantId.ToString().Replace("-", "")+"'>send the provider a message</a>"},
                                                                                        { "sellerEmail", invoice.Merchant.OrderPayedNotificationEmail},
                                            { "paywallPassword",invoice.Paywall.PaywallPassword},
                                            { "paywallLocation", "<a href='"+invoice.Paywall.PaywallLocation+"'>Go See What you Paid For!</a>"},
                                                { "emailRDNation", "<a href='mailto:"+LibraryConfig.DefaultInfoEmail+"'>"+ LibraryConfig.DefaultInfoEmail+"</a>"}                                                                                      };

                //sends email to user for their payment.
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, invoice.InvoiceBilling.Email, EmailServer.EmailServer.DEFAULT_SUBJECT + " Receipt for " + invoice.Paywall.Name, emailData, EmailServer.EmailServerLayoutsEnum.PaywallPaid);
                if (invoice.Merchant.OrderPayedNotificationEmail != LibraryConfig.DefaultAdminEmailAdmin)
                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, invoice.Merchant.OrderPayedNotificationEmail, EmailServer.EmailServer.DEFAULT_SUBJECT + " Paywall Payment Made, " + invoice.Paywall.Name, emailData, EmailServer.EmailServerLayoutsEnum.PaywallPaidMerchant);
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmailAdmin, EmailServer.EmailServer.DEFAULT_SUBJECT + " Paywall Payment Made, " + invoice.Paywall.Name, emailData, EmailServer.EmailServerLayoutsEnum.PaywallPaidMerchant);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: reportingInformation);
            }
        }
        private void CompileAndSendRefundEmailsForPaywall(DisplayInvoice invoice, string reportingInformation = null)
        {
            try
            {

                StringBuilder shippingAddress = new StringBuilder();
                InvoiceContactInfo shipping = null;
                if (invoice.InvoiceShipping == null)
                    shipping = invoice.InvoiceBilling;
                else
                    shipping = invoice.InvoiceShipping;

                var emailData = new Dictionary<string, string>
                                        {
                                            { "PaywallDescription", invoice.Paywall.Description},
                                            { "invoiceId", invoice.InvoiceId.ToString().Replace("-","")},
                                            { "amountPaid", "$"+ invoice.RefundAmount.ToString("N2") },
                                            { "receiptLink", "<a href='"+ServerConfig.WEBSITE_DEFAULT_LOCATION+"/streaming/receipt/"+invoice.InvoiceId.ToString().Replace("-","")+"'>Your Receipt and Order Status</a>"},
                                            { "emailRDNation", "<a href='mailto:"+LibraryConfig.DefaultInfoEmail+"'>"+ LibraryConfig.DefaultInfoEmail+"</a>"}                                                                                      };

                //sends email to user for their payment.
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, invoice.InvoiceBilling.Email, EmailServer.EmailServer.DEFAULT_SUBJECT + " Refund for " + invoice.Paywall.Name, emailData, EmailServer.EmailServerLayoutsEnum.PaywallRefunded);
                if (invoice.Merchant.OrderPayedNotificationEmail != LibraryConfig.DefaultAdminEmailAdmin)
                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, invoice.Merchant.OrderPayedNotificationEmail, EmailServer.EmailServer.DEFAULT_SUBJECT + " Paywall Refund Made, " + invoice.Paywall.Name, emailData, EmailServer.EmailServerLayoutsEnum.PaywallRefundedMerchant);

                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmailAdmin, EmailServer.EmailServer.DEFAULT_SUBJECT + " Paywall Refund Made, " + invoice.Paywall.Name, emailData, EmailServer.EmailServerLayoutsEnum.PaywallRefundedMerchant);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: reportingInformation);
            }
        }

        public Paywall AddPaywall(Paywall pw)
        {
            try
            {
                ManagementContext dc = new ManagementContext();
                DataModels.PaymentGateway.Paywall.Paywall npw = new DataModels.PaymentGateway.Paywall.Paywall();
                npw.DailyPrice = pw.DailyPrice;
                npw.DescriptionOfPaywall = pw.DescriptionOfPaywall;
                npw.EndDate = pw.EndDate;
                npw.IsEnabled = pw.IsEnabled;
                npw.Merchant = dc.Merchants.Where(x => x.InternalReference == pw.OwnerId).FirstOrDefault();
                npw.StartDate = pw.StartDate;
                npw.TimespanPrice = pw.TimespanPrice;
                dc.Paywalls.Add(npw);
                dc.SaveChanges();
                pw.PaywallId = npw.PaywallId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return pw;
        }

        public Paywall GetOwnedPaywall(long paywallId)
        {
            try
            {
                ManagementContext dc = new ManagementContext();
                var pw = dc.Paywalls.Where(x => x.PaywallId == paywallId).FirstOrDefault();
                //makes sure user owns paywall.
                if (pw.Merchant.InternalReference == RDN.Library.Classes.Account.User.GetMemberId())
                {
                    return DisplayPaywall(pw);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public List<DisplayInvoice> GetPaywallInvoices(Guid privId, Guid pubId)
        {
            try
            {
                List<DisplayInvoice> invoices = new List<Classes.Display.DisplayInvoice>();
                ManagementContext dc = new ManagementContext();
                var pw = dc.Invoices.Where(x => x.Paywall != null && x.Merchant.PrivateManagerId == privId && x.Merchant.MerchantId == pubId);
                //makes sure user owns paywall.

                foreach (var v in pw)
                {
                    invoices.Add(DisplayInvoice(v));
                }
                return invoices;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public Paywall GetPaywall(long paywallId)
        {
            try
            {
                ManagementContext dc = new ManagementContext();
                var pw = dc.Paywalls.Where(x => x.PaywallId == paywallId).FirstOrDefault();
                //makes sure user owns paywall.
                if (pw != null)
                    return DisplayPaywall(pw);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        private static Paywall DisplayPaywall(DataModels.PaymentGateway.Paywall.Paywall pw)
        {
            Paywall npw = new Paywall();
            try
            {
                npw.DailyPrice = pw.DailyPrice;
                npw.DescriptionOfPaywall = pw.DescriptionOfPaywall;
                npw.EndDate = pw.EndDate;
                if (npw.EndDate.HasValue)
                    npw.EndDateDisplay = pw.EndDate.Value.ToShortDateString();
                npw.IsEnabled = pw.IsEnabled;

                npw.StartDate = pw.StartDate;
                if (npw.StartDate.HasValue)
                    npw.StartDateDisplay = pw.StartDate.Value.ToShortDateString();
                npw.TimespanPrice = pw.TimespanPrice;
                npw.PaywallId = pw.PaywallId;
                npw.AcceptPaypal = pw.Merchant.AcceptPaymentsViaPaypal;
                npw.AcceptStripe = pw.Merchant.AcceptPaymentsViaStripe;
                npw.StripePublishableKey = pw.Merchant.StripePublishableKey;
                npw.MerchantId = pw.Merchant.MerchantId;

                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                foreach (var wall in pw.PaywallInvoices)
                {
                    if (wall.MemberPaidId == memId)
                        if (wall.Invoice.InvoiceStatus == (byte)InvoiceStatus.Payment_Successful || wall.Invoice.InvoiceStatus == (byte)InvoiceStatus.Pending_Payment_From_Paypal)
                        {
                            npw.IsPaid = true;
                            break;
                        }
                }

                foreach (var voice in pw.Merchant.Invoices.Where(x => x.Items.Count > 0).OrderByDescending(x => x.Created))
                {
                    DisplayInvoice v = DisplayInvoice(voice);
                    npw.Invoices.Add(v);
                }

                var dc = new ManagementContext();
                foreach (var game in pw.Games)
                {
                    npw.Games.Add(Game.GameManager.DisplayGame(game));
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return npw;
        }

        private static DisplayInvoice DisplayInvoice(DataModels.PaymentGateway.Invoices.Invoice voice)
        {
            DisplayInvoice v = new DisplayInvoice();
            try
            {
                v.UserId = voice.UserId;
                v.AdminNote = voice.AdminNote;
                if (voice.CurrencyRate != null)
                {
                    v.Currency = voice.CurrencyRate.CurrencyAbbrName;
                    v.CurrencyCost = voice.CurrencyRate.CurrencyExchangePerUSD;
                }
                else
                {
                    v.Currency = "USD";
                    v.CurrencyCost = 1;
                }

                v.InvoiceId = voice.InvoiceId;
                v.TotalIncludingTax = voice.BasePriceForItems;
                v.RefundAmount = voice.BasePriceForItems;
                v.ShoppingCartId = voice.ShoppingCartId;
                v.ShippingCost = voice.Shipping;
                v.RDNDeductedFee = voice.RDNDeductedFee;
                v.CreditCardCompanyProcessorDeductedFee = voice.CreditCardCompanyProcessorDeductedFee;
                v.PaymentProvider = (PaymentProvider)voice.PaymentProvider;
                v.Note = voice.Note;
                v.InvoiceStatus = (InvoiceStatus)voice.InvoiceStatus;
                v.Created = voice.Created;
                v.CustomerId = voice.PaymentProviderCustomerId;
                if (!String.IsNullOrEmpty(v.CustomerId) && v.PaymentProvider == PaymentProvider.Stripe)
                    v.CanRefundCustomer = true;

                v.TotalItemsBeingSold = 0;
                foreach (var refund in voice.Refunds)
                {
                    InvoiceRefund r = new InvoiceRefund();
                    r.RefundAmount = refund.PriceRefunded;
                    r.RefundId = refund.InvoiceRefundId;
                    v.Refunds.Add(r);
                }
                v.RefundAmount -= v.Refunds.Sum(x => x.RefundAmount);
                if (voice.InvoiceBilling != null)
                {
                    try
                    {
                        v.InvoiceBilling.City = voice.InvoiceBilling.City;
                        if (!String.IsNullOrEmpty(voice.InvoiceBilling.Country))
                        {
                            var count = SiteCache.GetCountries().Where(x => x.CountryId == Convert.ToInt32(voice.InvoiceBilling.Country)).FirstOrDefault();
                            v.InvoiceBilling.Country = count.Name;
                        }
                        v.InvoiceBilling.Email = voice.InvoiceBilling.Email;
                        v.InvoiceBilling.FirstName = voice.InvoiceBilling.FirstName;
                        v.InvoiceBilling.LastName = voice.InvoiceBilling.LastName;
                        v.InvoiceBilling.State = voice.InvoiceBilling.State;
                        v.InvoiceBilling.Street = voice.InvoiceBilling.Street;
                        v.InvoiceBilling.Street2 = voice.InvoiceBilling.Street2;
                        v.InvoiceBilling.Zip = voice.InvoiceBilling.Zip;
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }
                if (voice.Paywall != null)
                {
                    InvoicePaywall i = new InvoicePaywall();
                    i.Price = voice.Paywall.BasePrice;
                    i.Description = voice.Paywall.Description;
                    i.PaywallPassword = voice.Paywall.GeneratedPassword;
                    i.PaywallId = voice.Paywall.InvoicePaywallId;
                    i.MemberPaidId = voice.Paywall.MemberPaidId;
                    i.ValidUntil = voice.Paywall.ValidUntil;
                    i.ValidUntilDisplay = voice.Paywall.ValidUntil.ToShortDateString() + " " + voice.Paywall.ValidUntil.ToShortTimeString();
                    i.PriceType = (PaywallPriceTypeEnum)voice.Paywall.PaywallPriceTypeEnum;
                    i.SecondsViewedPaywall = voice.Paywall.SecondsViewedPaywall;
                    i.TimesUsedPassword = voice.Paywall.TimesUsedPassword;
                    i.LastViewedPaywall = voice.Paywall.LastViewedPaywall;
                    v.Paywall = i;
                }

                v.Merchant.MerchantId = voice.Merchant.MerchantId;
                v.Merchant.PrivateManagerId = voice.Merchant.PrivateManagerId;
                v.Merchant.ShopName = voice.Merchant.ShopName;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return v;
        }


        public Paywall UpdatePaywall(Paywall pw)
        {
            try
            {
                ManagementContext dc = new ManagementContext();
                var npw = dc.Paywalls.Where(x => x.PaywallId == pw.PaywallId).FirstOrDefault();
                if (npw.Merchant.InternalReference == RDN.Library.Classes.Account.User.GetMemberId())
                {
                    npw.DailyPrice = pw.DailyPrice;
                    npw.DescriptionOfPaywall = pw.DescriptionOfPaywall;
                    npw.EndDate = pw.EndDate;
                    npw.IsEnabled = pw.IsEnabled;
                    npw.StartDate = pw.StartDate;
                    npw.TimespanPrice = pw.TimespanPrice;
                    npw.PaywallId = pw.PaywallId;
                    dc.SaveChanges();
                    return pw;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public DisplayPaywall GetPaywalls(Guid ownerId)
        {
            DisplayPaywall dpw = new DisplayPaywall();
            try
            {
                ManagementContext dc = new ManagementContext();
                var merchant = dc.Merchants.Where(x => x.InternalReference == ownerId).FirstOrDefault();
                if (merchant != null)
                {
                    dpw.InternalReference = ownerId;
                    dpw.MerchantId = merchant.MerchantId;
                    dpw.PrivateManagerId = merchant.PrivateManagerId;
                    dpw.AcceptPaymentsViaPaypal = merchant.AcceptPaymentsViaPaypal;
                    dpw.AcceptPaymentsViaStripe = merchant.AcceptPaymentsViaStripe;
                    if (merchant.CurrencyRate == null)
                    {
                        dpw.Currency = "USD";
                        dpw.CurrencyCost = 1;
                    }
                    else
                    {
                        dpw.Currency = merchant.CurrencyRate.CurrencyAbbrName;
                        dpw.CurrencyCost = merchant.CurrencyRate.CurrencyExchangePerUSD;
                    }
                    dpw.IsPublished = merchant.IsPublished;
                    dpw.OwnerName = merchant.OwnerName;
                    dpw.PaypalEmail = merchant.PaypalEmail;
                    dpw.OrderPayedNotificationEmail = merchant.OrderPayedNotificationEmail;
                    dpw.ShopName = merchant.ShopName;
                    dpw.StripeConnectKey = merchant.StripeConnectKey;
                    dpw.StripeConnectToken = merchant.StripeConnectToken;
                    dpw.StripePublishableKey = merchant.StripePublishableKey;
                    dpw.StripeRefreshToken = merchant.StripeRefreshToken;
                    dpw.StripeTokenType = merchant.StripeTokenType;
                    dpw.StripeUserId = merchant.StripeUserId;
                    dpw.WelcomeMessage = merchant.WelcomeMessage;
                    foreach (var paywall in merchant.Paywalls)
                    {
                        dpw.Paywalls.Add(DisplayPaywall(paywall));
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return dpw;
        }
    }
}
