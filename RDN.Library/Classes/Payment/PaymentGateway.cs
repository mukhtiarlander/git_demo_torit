using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
//using GCheckout.AutoGen;
//using GCheckout.Util;
using RDN.Library.Classes.Dues.Enums;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment.Classes.Display;
using RDN.Library.Classes.Payment.Classes.Invoice;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.PaymentGateway.Invoices;
using RDN.Library.DataModels.PaymentGateway.Merchants;
using Invoice = RDN.Library.Classes.Payment.Classes.Invoice.Invoice;
using RDN.Portable.Classes.Controls.Dues.Enums;
using RDN.Portable.Classes.Payment.Enums;

namespace RDN.Library.Classes.Payment
{
    public sealed class PaymentGateway
    {
        string _connectionStringName;

        public PaymentGateway()
        { 
        
        }

        public PaymentGateway(string connectionStringName)
        {
            _connectionStringName = connectionStringName;
        }

        //ToDo: Delete old shopping cart when payment has been recieved? or when dispatched to google?. Shopping cart will have the same id as the invoice.
        // ToDo: WHEN DELETE OLD SHOP CART. Remember to reduce the number of items in stock.
        // ToDo: Cronjob to delete old shopping cards / old invoices that has expired
        public InvoiceFactory StartInvoiceWizard()
        {
            return InvoiceFactory.CreateNew();
        }

        /// <summary>
        /// Gets a list with merchants for overview
        /// </summary>
        /// <returns></returns>
        public List<OverviewMerchant> GetMerchants()
        {
            var output = new List<OverviewMerchant>();
            try
            {
                var mc = new ManagementContext();
                var merchants = mc.Merchants.Include("Items").Where(x => x.IsRDNation.Equals(false)).Select(x => new { x.MerchantId, x.InternalReference, x.PrivateManagerId, x.InternalReferenceType, x.OwnerName, x.ShopName, x.AmountOnAccount, x.PayedFeesToRDN, x.Created, itemsInShop = x.Items.Count });

                foreach (var dbmerchant in merchants)
                {
                    var merchant = new OverviewMerchant();
                    merchant.AmountOnAccount = dbmerchant.AmountOnAccount;
                    merchant.PrivateManagerId = dbmerchant.PrivateManagerId;
                    merchant.Created = dbmerchant.Created;
                    merchant.InternalReference = dbmerchant.InternalReference;
                    merchant.InternalReferenceType = (MerchantInternalReference)dbmerchant.InternalReferenceType;
                    merchant.MerchantId = dbmerchant.MerchantId;
                    merchant.OwnerName = dbmerchant.OwnerName;
                    merchant.PayedFeesToRDN = dbmerchant.PayedFeesToRDN;
                    merchant.ShopName = dbmerchant.ShopName;
                    merchant.NumberOfItemsForSale = dbmerchant.itemsInShop;
                    output.Add(merchant);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return output;
        }
        public OverviewMerchant GetMerchant(Guid merchantId)
        {
            try
            {
                var mc = new ManagementContext();
                var dbmerchant = (from x in mc.Merchants.Include("Items")
                                  where x.MerchantId == merchantId
                                  select new OverviewMerchant
                                  {
                                      MerchantId = x.MerchantId,
                                      InternalReference = x.InternalReference,
                                      PrivateManagerId = x.PrivateManagerId,
                                      //InternalReferenceType = x.InternalReferenceType,
                                      OwnerName = x.OwnerName,
                                      ShopName = x.ShopName,
                                      AmountOnAccount = x.AmountOnAccount,
                                      PayedFeesToRDN = x.PayedFeesToRDN,
                                      Created = x.Created,
                                      AcceptPaymentsViaPaypal = x.AcceptPaymentsViaPaypal,
                                      AcceptPaymentsViaStripe = x.AcceptPaymentsViaStripe,
                                      PaypalEmail = x.PaypalEmail,
                                      OrderPayedNotificationEmail = x.OrderPayedNotificationEmail,
                                      StripeConnectKey = x.StripeConnectKey,
                                      StripeConnectToken = x.StripeConnectToken,
                                      StripePublishableKey = x.StripePublishableKey,
                                      StripeRefreshToken = x.StripeRefreshToken,
                                      StripeTokenType = x.StripeTokenType,
                                      StripeUserId = x.StripeUserId
                                  }).FirstOrDefault();
                return dbmerchant;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        private static OverviewMerchant DisplayMerchant(Merchant dbmerchant)
        {
            try
            {
                var merchant = new OverviewMerchant();
                merchant.AmountOnAccount = dbmerchant.AmountOnAccount;
                merchant.PrivateManagerId = dbmerchant.PrivateManagerId;
                merchant.Created = dbmerchant.Created;
                merchant.InternalReference = dbmerchant.InternalReference;
                merchant.InternalReferenceType = (MerchantInternalReference)dbmerchant.InternalReferenceType;
                merchant.MerchantId = dbmerchant.MerchantId;
                merchant.OwnerName = dbmerchant.OwnerName;
                merchant.PayedFeesToRDN = dbmerchant.PayedFeesToRDN;
                merchant.ShopName = dbmerchant.ShopName;
                merchant.AcceptPaymentsViaPaypal = dbmerchant.AcceptPaymentsViaPaypal;
                merchant.AcceptPaymentsViaStripe = dbmerchant.AcceptPaymentsViaStripe;
                merchant.PaypalEmail = dbmerchant.PaypalEmail;
                merchant.OrderPayedNotificationEmail = dbmerchant.OrderPayedNotificationEmail;
                merchant.StripeConnectKey = dbmerchant.StripeConnectKey;
                merchant.StripeConnectToken = dbmerchant.StripeConnectToken;
                merchant.StripePublishableKey = dbmerchant.StripePublishableKey;
                merchant.StripeRefreshToken = dbmerchant.StripeRefreshToken;
                merchant.StripeTokenType = dbmerchant.StripeTokenType;
                merchant.StripeUserId = dbmerchant.StripeUserId;
                return merchant;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }


        /*
        public List<OverviewInvoice> GetInvoiceOverviewForMerchant(Guid merchantId)
        {
            var mc = new ManagementContext();
            
            var dbinvoices = mc.Invoices.Where(x => x.Merchant.MerchantId.Equals(merchantId)).OrderByDescending(x => x.Created).Select(x => new { x.Created, x.Currency, x.InvoiceId, x.InvoicePaid, x.InvoiceStatus, x.InvoiceStatusUpdated, x.Note, x.TotalIncludingTax }).ToList();
            var invoices = new List<OverviewInvoice>();
            foreach (var dbinvoice in dbinvoices)
            {
                var invoice = new OverviewInvoice();
                invoice.Currency = (Currency)dbinvoice.Currency;
                invoice.InvoiceId = dbinvoice.InvoiceId;
                invoice.InvoiceStatus = (InvoiceStatus)dbinvoice.InvoiceStatus;
                invoice.IsPaid = dbinvoice.InvoicePaid;
                invoice.Note = dbinvoice.Note;
                invoice.TotalIncludingTax = dbinvoice.TotalIncludingTax;
                invoice.InvoiceStatusUpdated = dbinvoice.InvoiceStatusUpdated;
                invoice.Created = dbinvoice.Created;
                invoices.Add(invoice);
            }
            return invoices;
        }
        */

        /// <summary>
        /// Gets a list of invoices for rdnation
        /// </summary>
        /// <returns></returns>
        public List<OverviewInvoice> GetInvoiceOverviewForRDN()
        {
            var invoices = new List<OverviewInvoice>();
            try
            {
                var mc = new ManagementContext();

                var dbinvoices = mc.Invoices.Where(x => x.Merchant.IsRDNation.Equals(true)).OrderByDescending(x => x.Created).Select(x => new { x.Created, x.CurrencyRate, x.InvoiceId, x.InvoicePaid, x.InvoiceStatus, x.InvoiceStatusUpdated, x.Note, x.TotalIncludingTax }).ToList();

                foreach (var dbinvoice in dbinvoices)
                {
                    var invoice = new OverviewInvoice();
                    invoice.Currency = dbinvoice.CurrencyRate.CurrencyAbbrName;
                    invoice.InvoiceId = dbinvoice.InvoiceId;
                    invoice.InvoiceStatus = (InvoiceStatus)dbinvoice.InvoiceStatus;
                    invoice.IsPaid = dbinvoice.InvoicePaid;
                    invoice.Note = dbinvoice.Note;
                    invoice.TotalIncludingTax = dbinvoice.TotalIncludingTax;
                    invoice.InvoiceStatusUpdated = dbinvoice.InvoiceStatusUpdated;
                    invoice.Created = dbinvoice.Created;
                    invoices.Add(invoice);
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return invoices;
        }

        public DisplayInvoice GetDisplayInvoiceWithStripeCustomerId(string customerId)
        {
            var output = new DisplayInvoice();

            var mc = new ManagementContext();
            var invoice = GetDatabaseInvoice(ref mc, customerId);
            if (invoice == null)
                return null;
            return GetDisplayInvoice(invoice);
        }

        public DisplayInvoice GetDisplayInvoice(Guid invoiceId, bool isRdn = false)
        {
            var output = new DisplayInvoice();
            if (!ValidateInvoice(invoiceId)) return null;

            var mc = new ManagementContext();
            var invoice = GetDatabaseInvoice(ref mc, invoiceId);
            if (invoice == null)
                return null;
            return GetDisplayInvoice(invoice);
        }

        /// <summary>
        /// Gets an invoice to be displayed in the merchant admin section
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="isRdn"></param>
        /// <returns></returns>
        private DisplayInvoice GetDisplayInvoice(DataModels.PaymentGateway.Invoices.Invoice invoice)
        {
            var output = new DisplayInvoice();
            try
            {
                output.InvoiceId = invoice.InvoiceId;

                output.InvoiceStatus = (InvoiceStatus)invoice.InvoiceStatus;
                output.InvoiceStatusUpdated = invoice.InvoiceStatusUpdated;
                output.PaymentProvider = (PaymentProvider)invoice.PaymentProvider;
                output.Note = invoice.Note;
                output.AdminNote = invoice.AdminNote;
                output.TotalBeforeTax = invoice.TotalBeforeTax;
                output.TotalIncludingTax = invoice.TotalIncludingTax;
                if (output.TotalIncludingTax == 0)
                    output.TotalIncludingTax = invoice.BasePriceForItems;
                output.TaxRate = invoice.TaxRate;
                output.Tax = invoice.Tax;
                output.ShippingCost = invoice.Shipping;
                output.ShippingType = (ShippingType)invoice.ShippingType;
                output.InvoicePaid = invoice.InvoicePaid;
                output.CreditCardCompanyProcessorDeductedFee = invoice.CreditCardCompanyProcessorDeductedFee;
                output.RDNDeductedFee = invoice.RDNDeductedFee;
                if (invoice.CurrencyRate != null)
                {
                    output.Currency = invoice.CurrencyRate.CurrencyAbbrName;
                    output.CurrencyCost = invoice.CurrencyRate.CurrencyExchangePerUSD;
                }
                else
                {
                    output.Currency = "USD";
                    output.CurrencyCost = 1;
                }
                output.Created = invoice.Created;
                output.UserId = invoice.UserId;
                output.ShoppingCartId = invoice.ShoppingCartId;
                output.CustomerId = invoice.PaymentProviderCustomerId;
                if (invoice.Merchant != null)
                {
                    output.Merchant.MerchantId = invoice.Merchant.MerchantId;
                    output.Merchant.PrivateManagerId = invoice.Merchant.PrivateManagerId;
                    output.Merchant.ShopName = invoice.Merchant.ShopName;
                    output.Merchant.PaypalEmail = invoice.Merchant.PaypalEmail;
                    output.Merchant.OrderPayedNotificationEmail = invoice.Merchant.OrderPayedNotificationEmail;
                    output.Merchant.OwnerName = invoice.Merchant.OwnerName;
                }
                if (invoice.Subscription != null)
                {
                    var subscription = new Classes.Invoice.InvoiceSubscription();
                    subscription.Name = invoice.Subscription.Name;
                    subscription.Description = invoice.Subscription.Description;
                    subscription.ArticleNumber = invoice.Subscription.ArticleNumber;
                    subscription.SubscriptionPeriod = (SubscriptionPeriod)invoice.Subscription.SubscriptionPeriod;
                    subscription.Price = invoice.Subscription.Price;
                    subscription.ValidUntil = invoice.Subscription.ValidUntil;
                    subscription.InternalObject = invoice.Subscription.InternalObject;
                    subscription.SubscriptionPeriodLengthInDays = invoice.Subscription.SubscriptionPeriodLengthInDays;
                    output.Subscription = subscription;
                }
                if (invoice.Paywall != null)
                {
                    var paywall = new Classes.Invoice.InvoicePaywall();
                    paywall.Name = invoice.Paywall.Name;
                    paywall.Description = invoice.Paywall.Description;
                    paywall.PaywallLocation = invoice.Paywall.PaywallLocation;
                    paywall.PaywallId = invoice.Paywall.InvoicePaywallId;
                    paywall.MemberPaidId = invoice.Paywall.MemberPaidId;
                    paywall.PriceType = (Enums.Paywall.PaywallPriceTypeEnum)invoice.Paywall.PaywallPriceTypeEnum;
                    paywall.Price = invoice.Paywall.BasePrice;
                    paywall.ValidUntil = invoice.Paywall.ValidUntil;
                    paywall.InternalObject = invoice.Paywall.InternalObject;
                    paywall.PaywallLengthOfDays = invoice.Paywall.PaywallLengthOfDays;
                    paywall.PaywallPassword = invoice.Paywall.GeneratedPassword;

                    output.Paywall = paywall;
                }
                if (output.InvoiceItems == null)
                    output.InvoiceItems = new List<Classes.Invoice.InvoiceItem>();
                foreach (var refund in invoice.Refunds)
                {
                    var r = new RDN.Library.Classes.Payment.Classes.Invoice.InvoiceRefund();
                    r.RefundAmount = refund.PriceRefunded;
                    r.RefundId = refund.InvoiceRefundId;
                    output.Refunds.Add(r);
                }

                foreach (var dbitem in invoice.Items)
                {
                    var item = new Classes.Invoice.InvoiceItem();
                    item.ArticleNumber = dbitem.ArticleNumber;
                    item.Article2Number = dbitem.Article2Number;
                    item.Description = dbitem.Description;
                    item.Name = dbitem.Name;
                    item.StoreItemId = dbitem.StoreItemId;
                    item.Price = dbitem.Price;
                    item.TotalShipping = dbitem.Shipping;
                    item.Quantity = dbitem.Quantity;
                    item.Weight = dbitem.Weight;
                    item.SizeOfItem = dbitem.SizeOfItem;
                    if (dbitem.ColorOfItem != null)
                    {
                        item.ColorOfItem = dbitem.ColorOfItem.ColorIdCSharp;
                        item.ColorName = dbitem.ColorOfItem.ColorName;
                    }
                    //no need to use this line as it doubles the damn price.
                    // output.TotalIncludingTax += dbitem.Price + dbitem.Shipping;
                    output.InvoiceItems.Add(item);
                }

                if (output.DuesItems == null)
                    output.DuesItems = new List<Classes.Invoice.InvoiceDuesItem>();
                foreach (var dbitem in invoice.DuesItems)
                {
                    var item = new Classes.Invoice.InvoiceDuesItem();
                    item.BasePrice = dbitem.BasePrice;
                    item.DuesId = dbitem.DuesId;
                    item.DuesItemId = dbitem.DuesItemId;
                    item.MemberPaidId = dbitem.MemberPaidId;
                    item.PriceAfterFees = dbitem.PriceAfterFees;
                    item.ProcessorFees = dbitem.ProcessorFees;
                    item.RDNationsFees = dbitem.RDNationsFees;
                    item.Name = dbitem.Name;
                    item.Description = dbitem.Description;
                    item.WhoPaysFees = (WhoPaysProcessorFeesEnum)Enum.Parse(typeof(WhoPaysProcessorFeesEnum), dbitem.WhoPaysFees.ToString());
                    var d = Dues.DuesFactory.GetDuesCollectionItem(dbitem.DuesItemId, dbitem.DuesId, RDN.Library.Classes.Account.User.GetMemberId());
                    if (d.DuesFees.FirstOrDefault() != null)
                        item.PaidForDate = d.DuesFees.FirstOrDefault().PayBy;
                    output.DuesItems.Add(item);
                }

                output.InvoiceBilling = GetInvoiceAddressFromDbInvoice(invoice.InvoiceBilling);
                output.InvoiceShipping = GetInvoiceAddressFromDbInvoice(invoice.InvoiceShipping);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return output;
        }

        private Classes.Invoice.InvoiceContactInfo GetInvoiceAddressFromDbInvoice(DataModels.PaymentGateway.Invoices.InvoiceContactInfo contactInfo)
        {
            var address = new Classes.Invoice.InvoiceContactInfo();

            try
            {
                if (contactInfo == null) return null;

                address.City = contactInfo.City;
                address.CompanyName = contactInfo.CompanyName;
                address.Country = contactInfo.Country;
                address.Email = contactInfo.Email;
                address.Fax = contactInfo.Fax;
                address.FirstName = contactInfo.FirstName;
                address.LastName = contactInfo.LastName;
                address.Phone = contactInfo.Phone;
                address.State = contactInfo.State;
                address.StateCode = contactInfo.StateCode;
                address.Street = contactInfo.Street;
                address.Street2 = contactInfo.Street2;
                address.Zip = contactInfo.Zip;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return address;
        }
        public bool UpdateInvoiceForRefund(Guid invoiceId, InvoiceStatus status, decimal priceForRefund, string notes)
        {
            try
            {
                var dc = new ManagementContext();
                var invoice = dc.Invoices.Where(x => x.InvoiceId == invoiceId).FirstOrDefault();
                if (invoice != null)
                {
                    invoice.Merchant = invoice.Merchant;
                    invoice.InvoiceStatus = (byte)status;
                    invoice.InvoiceStatusUpdated = DateTime.UtcNow;
                    invoice.AdminNote = notes;
                    RDN.Library.DataModels.PaymentGateway.Invoices.InvoiceRefund refund = new RDN.Library.DataModels.PaymentGateway.Invoices.InvoiceRefund();
                    refund.Invoice = invoice;
                    refund.PriceRefunded = priceForRefund;
                    invoice.Refunds.Add(refund);
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
        public bool SetInvoiceStatus(Guid invoiceId, InvoiceStatus status, string customerId = null)
        {
            try
            {
                var dc = new ManagementContext(_connectionStringName);
                var invoice = dc.Invoices.Where(x => x.InvoiceId == invoiceId).FirstOrDefault();
                if (invoice != null)
                {
                    invoice.Merchant = invoice.Merchant;
                    invoice.InvoiceStatus = (byte)status;
                    invoice.Paywall = invoice.Paywall;
                    if (!String.IsNullOrEmpty(customerId))
                        invoice.PaymentProviderCustomerId = customerId;
                    invoice.InvoiceStatusUpdated = DateTime.UtcNow;
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

        public Invoice GetInvoice(Guid invoiceId)
        {
            return null;
        }
     
        public Invoice GetLatestInvoiceSubscriptionForLeagueId(Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var invoice = dc.Invoices.Include("Items").Include("Payments").Include("Logs").Include("Subscription").Include("InvoiceShipping").Include("InvoiceBilling").Include("Merchant").Where(x => x.InvoicePaid == true).OrderByDescending(x => x.Subscription.ValidUntil).OrderByDescending(x => x.Created).FirstOrDefault(x => x.Subscription.InternalObject.Equals(leagueId));

                if (invoice != null && invoice.Expires > DateTime.UtcNow)
                {
                    Invoice inv = Invoice.CreateNewInvoice();
                    inv.InvoiceId = invoice.InvoiceId;
                    inv.InvoiceStatus = invoice.GetInvoiceStatus();
                    inv.PaymentProviderCustomerId = invoice.PaymentProviderCustomerId;
                    if (invoice.Subscription != null)
                    {
                        inv.Subscription = new Classes.Invoice.InvoiceSubscription();
                        inv.Subscription.Name = invoice.Subscription.Name;
                        inv.Subscription.Price = invoice.Subscription.Price;
                        inv.Subscription.ValidUntil = invoice.Subscription.ValidUntil;
                        inv.Subscription.Created = invoice.Created;
                        inv.PaymentProviderCustomerId = invoice.PaymentProviderCustomerId;
                        return inv;
                    }
                    return inv;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public List<Invoice> GetAllInvoiceSubscriptionsForLeague(Guid leagueId)
        {
            try
            {
                List<Invoice> voices = new List<Invoice>();
                var dc = new ManagementContext();
                var invoice = dc.Invoices.Include("Items").Include("Payments").Include("Logs").Include("Subscription").Include("InvoiceShipping").Include("InvoiceBilling").Include("Merchant").Where(x => x.Subscription.InternalObject.Equals(leagueId)).OrderByDescending(x => x.Subscription.ValidUntil).ToList();

                if (invoice != null)
                {
                    foreach (var invoi in invoice)
                    {
                        Invoice inv = Invoice.CreateNewInvoice();
                        inv.InvoiceId = invoi.InvoiceId;
                        inv.InvoiceStatus = invoi.GetInvoiceStatus();
                        inv.PaymentProviderCustomerId = invoi.PaymentProviderCustomerId;
                        inv.Created = invoi.Created;
                        if (invoi.Subscription != null)
                        {
                            inv.Subscription = new Classes.Invoice.InvoiceSubscription();
                            inv.Subscription.Name = invoi.Subscription.Name;
                            inv.Subscription.Price = invoi.Subscription.Price;
                            inv.Subscription.ValidUntil = invoi.Subscription.ValidUntil;
                            inv.Subscription.Created = invoi.Created;
                        }
                        voices.Add(inv);
                    }
                    return voices;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        private DataModels.PaymentGateway.Invoices.Invoice GetDatabaseInvoice(ref ManagementContext mc, Guid invoiceId)
        {
            return mc.Invoices.Include("Items").Include("Payments").Include("Logs").Include("Subscription").Include("Paywall").Include("InvoiceShipping").Include("InvoiceBilling").Include("Merchant").FirstOrDefault(x => x.InvoiceId.Equals(invoiceId));
        }
        private DataModels.PaymentGateway.Invoices.Invoice GetDatabaseInvoice(ref ManagementContext mc, string customerId)
        {
            return mc.Invoices.Include("Items").Include("Payments").Include("Logs").Include("Subscription").Include("InvoiceShipping").Include("InvoiceBilling").Include("Merchant").Where(x => x.InvoicePaid == true).OrderByDescending(x => x.Created).FirstOrDefault(x => x.PaymentProviderCustomerId.Equals(customerId));
        }

        private bool ValidateInvoice(Guid invoiceId)
        {
            var mc = new ManagementContext();
            return mc.Invoices.Any(x => x.InvoiceId.Equals(invoiceId));
        }

        private void SetInvoiceStatus(ref DataModels.PaymentGateway.Invoices.Invoice invoice, InvoiceStatus status)
        {
            invoice.InvoiceStatus = (byte)status;
            invoice.InvoiceStatusUpdated = DateTime.Now;
        }

        private void SetOrderId(ref DataModels.PaymentGateway.Invoices.Invoice invoice, string orderId)
        {
            invoice.PaymentProviderOrderId = orderId;
        }

        private void AddInvoiceLog(ref DataModels.PaymentGateway.Invoices.Invoice invoice, string logToAdd)
        {
            var invoiceLog = new InvoiceLogs();
            invoiceLog.Log = logToAdd;
            invoiceLog.Created = DateTime.Now;
            invoice.Logs.Add(invoiceLog);
        }


        /// <summary>
        /// Adds the RDN fees to the recent order
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="creditCompanyFee"></param>
        /// <param name="tax"></param>
        /// <param name="totalInclTax"></param>
        private void AddSlip(ref DataModels.PaymentGateway.Invoices.Invoice invoice, decimal creditCompanyFee, decimal tax, decimal totalInclTax)
        {
            try
            {
                double rdnFeePercent = 0.0;
                decimal rdnFeeFixed = 0.0M;

                if (!invoice.Merchant.IsRDNation)
                {
                    rdnFeePercent = invoice.Merchant.RDNPercentageFee;
                    rdnFeeFixed = invoice.Merchant.RDNFixedFee;
                }

                var slip = new MerchantFeeSlip();
                slip.InvoiceId = invoice.InvoiceId;
                slip.CreditCompanyFee = creditCompanyFee;
                slip.Tax = tax;
                slip.TotalInclTax = totalInclTax;
                if (invoice.Merchant.IsRDNation)
                    slip.SetSlipStatus(MerchantSlipStatus.IsRdn);
                else
                    slip.SetSlipStatus(MerchantSlipStatus.Active);

                slip.ShippingCost = invoice.Shipping;

                var rdnFee = rdnFeeFixed;
                rdnFee += Convert.ToDecimal(rdnFeePercent) * totalInclTax;
                slip.RDNFee = rdnFee;
                invoice.Merchant.FeeSlips.Add(slip);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }


        /// <summary>
        /// Checks if the XML contains the is-subscription tag set to yes
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool ExtraxtIsSubscriptionPresentFromGoogleData(XDocument input)
        {
            bool isSubscriptionPresent = false;
            try
            {
                XNamespace ns = input.Root.Name.Namespace;
                // Get all RDN data elements
                var privateDataNodes = (from nodes in input.Descendants(ns + "merchant-private-item-data") select nodes).ToList();
                foreach (var privateDataNode in privateDataNodes)
                {
                    // For each element, make sure it exists and is not null
                    if (privateDataNode != null && privateDataNode.HasElements && privateDataNode.Element(ns + "is-subscription") != null)
                    {
                        if (!string.IsNullOrEmpty(privateDataNode.Element(ns + "is-subscription").Value))
                        {
                            // If we stumble upon a subscription object, continue. This is used to identify a subscription
                            if (privateDataNode.Element(ns + "is-subscription").Value.Equals("yes"))
                            {
                                isSubscriptionPresent = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType(), additionalInformation: input.ToString());
            }

            return isSubscriptionPresent;
        }

        /// <summary>
        /// Get the serial number from the google xml data
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ExtraxtSerialNumberFromGoogleData(XDocument input)
        {
            string serialNumber = null;
            try
            {
                serialNumber = input.Root.Attribute("serial-number").Value;
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType(), additionalInformation: input.ToString());
            }

            return serialNumber;
        }
    }
}
