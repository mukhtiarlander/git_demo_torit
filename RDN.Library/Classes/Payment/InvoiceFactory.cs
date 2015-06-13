using System;
using System.Configuration;
using System.Linq;
using System.Xml;
using RDN.Library.Classes.Payment.Classes.Invoice;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.DataModels.Context;
using ShippingType = RDN.Library.Classes.Payment.Enums.ShippingType;
using Stripe;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment.Enums.Stripe;
using System.Collections.Generic;
using RDN.Utilities.Config;
using RDN.Library.Classes.Payment.Paypal;
using System.Collections.Specialized;
using PayPal.AdaptivePayments.Model;
using RDN.Library.Classes.Payment.Enums.Paypal;
using PayPal.AdaptivePayments;
using PayPal.Authentication;
using RDN.Library.Cache;
using RDN.Portable.Config;
using PayPal.PayPalAPIInterfaceService.Model;
using PayPal.PayPalAPIInterfaceService;
using System.Text;
using RDN.Library.Classes.RN.Funds;
using RDN.Portable.Classes.Payment.Classes;
using RDN.Portable.Classes.Payment.Enums;
using RDN.Library.Classes.Config;

namespace RDN.Library.Classes.Payment
{
    /// <summary>
    /// handles invoicing for all of RDNation.
    /// </summary>
    public class InvoiceFactory
    {
        private static readonly decimal _ProcessorFeePercentage = .029M;
        private static readonly decimal _ProcessorFeeAdditionalCents = .30M;
        /// <summary>
        /// 54 cents so we get the exact amount needed to hit 50 cents profit of each dues collected.
        /// </summary>
        private static readonly decimal _RDNationDuesFees = .54M;
        //paypal takes the fees from us, so we have it at 10%.
        private static readonly decimal _RDNationInStoreFeesPercentagePaypal = .10M;
        //stripe takes the fees from the seller, so we charge 7.1%.
        private static readonly decimal _RDNationInStoreFeesPercentageStripe = .071M;

        //paypal takes the fees from us, so we have it at 25%.
        private static readonly decimal _RDNationPaywallFeesPercentagePaypal = .25M;
        //stripe takes the fees from the seller, so we charge 22.1%.
        private static readonly decimal _RDNationPaywallFeesPercentageStripe = .221M;

        private Invoice invoice = null;

        InvoiceFactory() { }

        public static InvoiceFactory CreateNew()
        {
            return new InvoiceFactory();
        }

        /// <summary>
        /// Initializes the wizard
        /// </summary>
        /// <param name="merchantId">The merchant id (DB Table RDN_Merchants)</param>
        /// <param name="currency"></param>
        /// <param name="paymentProvider"></param>
        /// <returns></returns>
        public InvoiceFactory Initalize(Guid merchantId, string currency, PaymentProvider paymentProvider, bool isLive, ChargeTypeEnum chargeType)
        {
            invoice = Invoice.CreateNewInvoice();
            try
            {
                invoice.FinancialData.BasePriceForItems = 0;
                invoice.FinancialData.TotalBeforeTax = 0;
                invoice.FinancialData.ShippingCost = 0.0M;
                invoice.FinancialData.TaxRate = 0;
                invoice.FinancialData.Tax = 0;
                invoice.FinancialData.TotalIncludingTax = 0;

                invoice.ChargeType = chargeType;
                invoice.IsLive = isLive;
                invoice.Currency = currency;
                invoice.ShippingType = ShippingType.None;
                invoice.InvoiceId = Guid.NewGuid();
                invoice.PaymentProvider = paymentProvider;
                invoice.InvoiceStatus = InvoiceStatus.Not_Started;
                invoice.MerchantId = merchantId;

                RecalculateTotalAfterTax();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return this;
        }
        public InvoiceFactory SetInvoiceId(Guid invoiceId)
        {
            invoice.InvoiceId = invoiceId;
            return this;
        }


        public InvoiceFactory SetRefundAmount(decimal refundAmount)
        {
            invoice.FinancialData.RefundAmount = refundAmount;
            return this;
        }
        /// <summary>
        /// for Stripe, this is the customer Id
        /// for Paypal, this is the users email
        /// </summary>
        /// <param name="paymentProviderId"></param>
        /// <returns></returns>
        public InvoiceFactory SetPaymentProviderId(string paymentProviderId)
        {
            invoice.PaymentProviderCustomerId = paymentProviderId;
            return this;
        }
        public InvoiceFactory SetInvoiceStatus(InvoiceStatus status)
        {
            invoice.InvoiceStatus = status;
            return this;
        }

        private void RecalculateTotalBeforeTax()
        {
            var totalBeforeTax = 0.0M;
            //taxes are included in shipping costs. so we comment this out.
            //totalBeforeTax += invoice.FinancialData.ShippingCost;
            totalBeforeTax += invoice.FinancialData.BasePriceForItems;
            invoice.FinancialData.TotalBeforeTax = totalBeforeTax;
        }

        private void CalculateRDNationFeesForInStorePurchasesPaypal()
        {
            //taxes are included in the base price.
            var totalPrice = invoice.FinancialData.BasePriceForItems + invoice.FinancialData.ShippingCost;
            var RDNationFees = totalPrice * _RDNationInStoreFeesPercentagePaypal;
            invoice.FinancialData.RDNationFees = Math.Round(RDNationFees + _ProcessorFeeAdditionalCents, 2);
            invoice.FinancialData.CCProcessorFees = Math.Round((totalPrice + _ProcessorFeeAdditionalCents) * _ProcessorFeePercentage, 2) + _ProcessorFeeAdditionalCents;
            invoice.FinancialData.PriceSubtractingRDNationFees = Math.Round(totalPrice - invoice.FinancialData.RDNationFees, 2);
        }

        private void CalculateRDNationFeesForInStorePurchasesStripe()
        {
            //taxes are included in the base price.
            var totalPrice = invoice.FinancialData.BasePriceForItems + invoice.FinancialData.ShippingCost;
            var RDNationFees = totalPrice * _RDNationInStoreFeesPercentageStripe;
            invoice.FinancialData.RDNationFees = Math.Round(RDNationFees, 2);
            invoice.FinancialData.CCProcessorFees = Math.Round((totalPrice + _ProcessorFeeAdditionalCents) * _ProcessorFeePercentage, 2) + _ProcessorFeeAdditionalCents;
            invoice.FinancialData.PriceSubtractingRDNationFees = Math.Round(totalPrice - invoice.FinancialData.RDNationFees, 2);
        }
        private void CalculateRDNationFeesForPaywallPurchasesPaypal()
        {
            //taxes are included in the base price.
            var totalPrice = invoice.FinancialData.BasePriceForItems + invoice.FinancialData.ShippingCost;
            var RDNationFees = totalPrice * _RDNationPaywallFeesPercentagePaypal;
            invoice.FinancialData.RDNationFees = Math.Round(RDNationFees + _ProcessorFeeAdditionalCents, 2);
            invoice.FinancialData.CCProcessorFees = Math.Round((totalPrice + _ProcessorFeeAdditionalCents) * _ProcessorFeePercentage, 2) + _ProcessorFeeAdditionalCents;
            invoice.FinancialData.PriceSubtractingRDNationFees = Math.Round(totalPrice - invoice.FinancialData.RDNationFees, 2);
        }

        private void CalculateRDNationFeesForPaywallPurchasesStripe()
        {
            //taxes are included in the base price.
            var totalPrice = invoice.FinancialData.BasePriceForItems;
            var RDNationFees = totalPrice * _RDNationPaywallFeesPercentageStripe;
            invoice.FinancialData.RDNationFees = Math.Round(RDNationFees, 2);
            invoice.FinancialData.CCProcessorFees = Math.Round((totalPrice + _ProcessorFeeAdditionalCents) * _ProcessorFeePercentage, 2) + _ProcessorFeeAdditionalCents;
            invoice.FinancialData.PriceSubtractingRDNationFees = Math.Round(totalPrice - invoice.FinancialData.RDNationFees, 2);
        }

        private void RecalculateTotalAfterTax()
        {
            RecalculateTotalBeforeTax();
            var totalAfterTax = 0.0M;
            totalAfterTax += invoice.FinancialData.TotalBeforeTax;
            invoice.FinancialData.Tax = Math.Round(Convert.ToDecimal(invoice.FinancialData.TaxRate) * totalAfterTax, 2);
            totalAfterTax += invoice.FinancialData.Tax;
            invoice.FinancialData.TotalIncludingTax = Math.Round(totalAfterTax, 2);
        }


        /// <summary>
        /// Add an invoice item to the order
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public InvoiceFactory AddItem(RDN.Library.Classes.Payment.Classes.Invoice.InvoiceItem item)
        {
            if (string.IsNullOrEmpty(item.ArticleNumber))
                throw new Exception("Invalid item article number");
            if (string.IsNullOrEmpty(item.Description))
                throw new Exception("Invalid item description");
            if (string.IsNullOrEmpty(item.Name))
                throw new Exception("Invalid item name");
            if (item.Price < 0)
                throw new Exception("Item price can not be negative");
            if (item.Quantity < 1)
                throw new Exception("Item quantity can not be below 1");
            if (item.Weight < 0)
                throw new Exception("Item weight can not be negative");
            if (item.BasePrice < 0)
                throw new Exception("Item base price can not be negative");

            invoice.ItemsInvoice.Add(item);
            invoice.FinancialData.BasePriceForItems += item.Price;
            RecalculateTotalAfterTax();
            return this;
        }

        public InvoiceFactory AddItemTaxIncluded(RDN.Library.Classes.Payment.Classes.Invoice.InvoiceItem item)
        {
            if (string.IsNullOrEmpty(item.Description))
                throw new Exception("Invalid item description");
            if (string.IsNullOrEmpty(item.Name))
                throw new Exception("Invalid item name");
            if (item.Price < 0)
                throw new Exception("Item price can not be negative");
            //if (item.Quantity < 1)
            //    throw new Exception("Item quantity can not be below 1");
            if (item.Weight < 0)
                throw new Exception("Item weight can not be negative");
            if (item.BasePrice < 0)
                throw new Exception("Item base price can not be negative");
            if (item.Quantity > 0)
            {
                invoice.ItemsInvoice.Add(item);
                invoice.FinancialData.BasePriceForItems += item.Price;
            }
            return this;
        }
        public InvoiceFactory AddWriterPayout(InvoiceWriterPayout item)
        {
            if (item.PayoutId == new Guid())
                throw new Exception("Payout Id Is New Guid");
            if (item.UserPaidId == new Guid())
                throw new Exception("User Id Is New Guid");
            if (string.IsNullOrEmpty(item.Name))
                throw new Exception("Invalid item name");
            if (item.BasePrice < 0)
                throw new Exception("Item base price can not be negative");

            if (invoice.PaymentProvider == PaymentProvider.Paypal)
                item.PriceAfterFeeDeduction = item.BasePrice - 1;
            else
                item.PriceAfterFeeDeduction = item.BasePrice;

            invoice.RNWriterPayouts.Add(item);
            invoice.FinancialData.BasePriceForItems += item.BasePrice;

            return this;
        }
        public InvoiceFactory AddDuesItem(InvoiceDuesItem item)
        {
            if (item.DuesId == new Guid())
                throw new Exception("Dues Id Is New Guid");
            if (item.MemberPaidId == new Guid())
                throw new Exception("Member Id Is New Guid");
            if (item.DuesItemId <= 0)
                throw new Exception("Dues Item Id Doesn't Exist");
            if (string.IsNullOrEmpty(item.Description))
                throw new Exception("Invalid item description");
            if (string.IsNullOrEmpty(item.Name))
                throw new Exception("Invalid item name");
            if (item.BasePrice < 0)
                throw new Exception("Item base price can not be negative");

            //add processor fees
            //add RDNation fees
            decimal paypalPlusRDNationFees = Math.Round(((item.BasePrice + _RDNationDuesFees + _ProcessorFeeAdditionalCents) * _ProcessorFeePercentage), 2);
            item.PriceAfterFees = item.BasePrice + paypalPlusRDNationFees + _RDNationDuesFees + _ProcessorFeeAdditionalCents;
            item.ProcessorFees = Math.Round((item.BasePrice + _RDNationDuesFees + _ProcessorFeeAdditionalCents) * _ProcessorFeePercentage, 2) + _RDNationDuesFees + _ProcessorFeeAdditionalCents;
            item.RDNationsFees = _RDNationDuesFees;

            invoice.ItemsDues.Add(item);
            invoice.FinancialData.BasePriceForItems += item.PriceAfterFees;

            return this;
        }

        /// <summary>
        /// Sets a subscription to the invoice. Only one subscription per invoice is allowed.
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        public InvoiceFactory SetSubscription(InvoiceSubscription subscription)
        {
            if (string.IsNullOrEmpty(subscription.Description))
                throw new Exception("Invalid subscription description");
            if (string.IsNullOrEmpty(subscription.Name))
                throw new Exception("Invalid subscription name");
            if (string.IsNullOrEmpty(subscription.DigitalPurchaseText))
                throw new Exception("Invalid subscription digital purchase text");
            if (string.IsNullOrEmpty(subscription.DescriptionRecurring))
                throw new Exception("Invalid subscription description recurring");
            if (string.IsNullOrEmpty(subscription.NameRecurring))
                throw new Exception("Invalid subscription name recurring");
            if (subscription.Price < 0)
                throw new Exception("Item price can not be negative");
            if (subscription.SubscriptionPeriodLengthInDays < 1)
                throw new Exception("Item subscription period length in day can not be below 1");

            invoice.Subscription = subscription;
            //gets the league subscriptio date.
            //if the league date is in the fiture, then we use that date, otherwise we use todas date.
            DateTime? subScriptionDate = League.LeagueFactory.GetLeagueSubscriptionDate(invoice.Subscription.InternalObject);
            if (subScriptionDate == null || subScriptionDate == new DateTime() || subScriptionDate < DateTime.UtcNow)
                subScriptionDate = DateTime.UtcNow;

            //checks the last subscription to see if one exists.  
            invoice.Subscription.ValidUntil = subScriptionDate.Value.AddDays(invoice.Subscription.SubscriptionPeriodLengthInDays);
            return this;
        }
        public InvoiceFactory SetPaywall(InvoicePaywall paywall)
        {
            if (string.IsNullOrEmpty(paywall.Description))
                throw new Exception("Invalid paywall description");
            if (string.IsNullOrEmpty(paywall.Name))
                throw new Exception("Invalid paywall name");
            if (paywall.Price < 0)
                throw new Exception("Item price can not be negative");
            if (paywall.PaywallLengthOfDays < 1)
                paywall.PaywallLengthOfDays *= -1;

            invoice.Paywall = paywall;
            //gets the league subscriptio date.
            //if the league date is in the fiture, then we use that date, otherwise we use todas date.

            //checks the last subscription to see if one exists.  
            //invoice.Paywall.ValidUntil = DateTime.UtcNow.AddDays(paywall.PaywallLengthOfDays);
            invoice.FinancialData.BasePriceForItems += paywall.Price;
            invoice.FinancialData.TotalIncludingTax += paywall.Price;
            return this;
        }

        /// <summary>
        /// 1.0 = 100%, 0.05 = 5%
        /// </summary>
        /// <param name="tax"></param>
        /// <returns></returns>
        public InvoiceFactory SetTax(double tax)
        {
            if (tax < 0 || tax > 0.5)
                throw new Exception("Tax can not be below 0 and also not above 50%");

            invoice.FinancialData.TaxRate = tax;
            RecalculateTotalAfterTax();
            return this;
        }

        public InvoiceFactory SetTaxTotalCosts(double tax, decimal totalAfterTaxCost, decimal totalTaxesCost)
        {
            if (tax < 0 || tax > 0.5)
                throw new Exception("Tax can not be below 0 and also not above 50%");

            invoice.FinancialData.TaxRate = tax;
            invoice.FinancialData.Tax = totalTaxesCost;
            invoice.FinancialData.TotalIncludingTax = totalAfterTaxCost;
            return this;
        }


        public InvoiceFactory SetShipping(decimal shipping, ShippingType shippingType)
        {
            if (shipping < 0)
                throw new Exception("Shipping can not be below 0");
            invoice.FinancialData.ShippingCost = shipping;
            invoice.ShippingType = shippingType;
            RecalculateTotalAfterTax();
            return this;
        }

        public InvoiceFactory SetNotes(string note = null, string adminNote = null)
        {
            invoice.Note = note;
            invoice.AdminNote = adminNote;
            return this;
        }
        public InvoiceFactory SetUserId(Guid userId)
        {
            invoice.UserId = userId;
            return this;
        }
        public InvoiceFactory SetShoppingCartId(Guid cartId)
        {
            invoice.ShoppingCartId = cartId;
            return this;
        }
        public InvoiceFactory SetStripeTokenId(string stripeToken)
        {
            invoice.StripeToken = stripeToken;
            return this;
        }

        public InvoiceFactory SetInvoiceContactData(InvoiceContactInfo invoiceBillingContactInfo = null, InvoiceContactInfo invoiceShippingContactInfo = null)
        {
            invoice.InvoiceShipping = invoiceShippingContactInfo;
            invoice.InvoiceBilling = invoiceBillingContactInfo;
            return this;
        }
        public InvoiceFactory SetSellersAddress(InvoiceContactInfo sellersAddress)
        {
            invoice.SellersAddress = sellersAddress;
            return this;
        }

        /// <summary>
        /// Creates the invoice in the system and makes it ready to be processed
        /// </summary>
        /// <returns>Return object containing details needed for the user to continue with the process</returns>
        public CreateInvoiceReturn FinalizeInvoice()
        {
            var output = new CreateInvoiceReturn();
            try
            {
                //if (invoice.ItemsInvoice.Count == 0 && invoice.Subscription == null && invoice.ItemsDues.Count == 0 && invoice.Paywall == null)
                //{
                //    output.Error = "No items, no subscription and no paywall present in the invoice";
                //    return output;
                //}
                //remove when we added a paywall.

                if (invoice.PaymentProvider == PaymentProvider.Stripe)
                {
                    HandleStripePayments(output);
                }
                else if (invoice.PaymentProvider == PaymentProvider.Paypal)
                {
                    HandlePaypalPayments(output);
                    ///add the paypal to the DB.
                    //but don't add if we are just paying out writers.
                    if (invoice.ChargeType != ChargeTypeEnum.RollinNewsWriterPayouts)
                        AddInvoiceToDatabase();
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return output;
        }

        private void HandlePaypalPayments(CreateInvoiceReturn output)
        {
            if (invoice.ChargeType == ChargeTypeEnum.Subscription)
            {
                output.RedirectLink = PerformPaypalSubscriptionCheckout();
                output.Status = InvoiceStatus.Pending_Payment_From_Paypal;
            }
            else if (invoice.ChargeType == ChargeTypeEnum.DuesItem)
            {
                //performs checkout, along with describing the output status.
                PerformPaypalDuesPaymentCheckout(output);
            }
            else if (invoice.ChargeType == ChargeTypeEnum.RollinNewsWriterPrePayout)
            {
                //performs checkout, along with describing the output status.
                PerformPaypalPrePayoutWriterContent(output);
            }
            else if (invoice.ChargeType == ChargeTypeEnum.RollinNewsWriterPayouts)
            {
                //performs checkout, along with describing the output status.
                PerformPaypalRollinNewsWritersPayment(output);
            }
            else if (invoice.ChargeType == ChargeTypeEnum.Paywall)
            {
                CalculateRDNationFeesForPaywallPurchasesPaypal();
                invoice.InvoiceStatus = InvoiceStatus.Awaiting_Payment;
                // If it was successful we will now have a redirect link
                output.RedirectLink = PerformPaypalPaywallPayment();
                if (output.RedirectLink != String.Empty)
                    output.Status = InvoiceStatus.Pending_Payment_From_Paypal;
                else
                    output.Status = InvoiceStatus.Failed;
            }
            //else if (invoice.ChargeType == ChargeTypeEnum.Refund_Paywall)
            //{

            //    invoice.InvoiceStatus = InvoiceStatus.Refund_Started;
            //    // If it was successful we will now have a redirect link
            //    output.RedirectLink = PerformPaypalPaywallRefund();

            //}
            else if (invoice.ChargeType == ChargeTypeEnum.InStorePurchase)
            {
                CalculateRDNationFeesForInStorePurchasesPaypal();
                output.RedirectLink = PerformPaypalInStorePaymentCheckout();
                if (output.RedirectLink != String.Empty)
                    output.Status = InvoiceStatus.Pending_Payment_From_Paypal;
                else
                    output.Status = InvoiceStatus.Failed;
            }
        }



        private void HandleStripePayments(CreateInvoiceReturn output)
        {
            if (invoice.ChargeType == ChargeTypeEnum.Refund_Paywall)
            {
                invoice.InvoiceStatus = InvoiceStatus.Refund_Started;

                // Create the Stripe Request and get the data back.
                var response = PerformStripeRefund();
                // If it was successful we will now have a redirect link
                if (response != null)
                {
                    // Try to save the data to our database
                    if (response.AmountInCentsRefunded.HasValue && response.AmountInCentsRefunded.Value > 0)
                    {
                        PaymentGateway pg = new PaymentGateway();
                        var voice = pg.GetDisplayInvoice(invoice.InvoiceId);
                        if (voice.Refunds.Sum(x => x.RefundAmount) + invoice.FinancialData.RefundAmount == voice.TotalIncludingTax)
                            pg.UpdateInvoiceForRefund(invoice.InvoiceId, InvoiceStatus.Refunded, invoice.FinancialData.RefundAmount, invoice.AdminNote);
                        else
                            pg.UpdateInvoiceForRefund(invoice.InvoiceId, InvoiceStatus.Partially_Refunded, invoice.FinancialData.RefundAmount, invoice.AdminNote);

                        voice.RefundAmount = invoice.FinancialData.RefundAmount;
                        Paywall.Paywall pw = new Paywall.Paywall();
                        pw.HandlePaywallRefund(voice);
                        output.Status = InvoiceStatus.Refunded;
                    }
                    else
                    {
                        // Could not save to our database
                        output.Error = "Failed to save to RDN database";
                        output.Status = InvoiceStatus.Failed;
                    }
                }
                else
                    output.Status = InvoiceStatus.Failed;
            }
            else if (invoice.ChargeType == ChargeTypeEnum.Subscription)
            {
                CompleteNewStripeSubscription(output);
            }
            else if (invoice.ChargeType == ChargeTypeEnum.Paywall)
            {
                CalculateRDNationFeesForPaywallPurchasesStripe();
                // Create the Stripe Request and get the data back.
                var response = PerformStripePaywallPurchaseCheckout();
                // If it was successful we will now have a redirect link
                if (response != null)
                {
                    // Try to save the data to our database
                    var result = AddInvoiceToDatabase();

                    if (result)
                    {
                        output.InvoiceId = invoice.InvoiceId;
                        output.Status = InvoiceStatus.Payment_Successful;

                        PaymentGateway pg = new PaymentGateway();
                        var voice = pg.GetDisplayInvoice(invoice.InvoiceId);
                        Paywall.Paywall pw = new Paywall.Paywall();
                        pw.HandlePaywallPayments(voice);
                    }
                    else
                    {
                        // Could not save to our database
                        output.Error = "Failed to save to RDN database";
                        output.Status = InvoiceStatus.Failed;
                    }
                }
                else
                    output.Status = InvoiceStatus.Failed;
            }
            else if (invoice.ChargeType == ChargeTypeEnum.Cancel_Subscription)
            {
                CancelStripeSubscription(output);
            }
            else if (invoice.ChargeType == ChargeTypeEnum.SubscriptionUpdated)
            {
                CompleteStripeSubscriptionUpdated(output);
            }
            else if (invoice.ChargeType == ChargeTypeEnum.InStorePurchase)
            {
                CalculateRDNationFeesForInStorePurchasesStripe();
                // Create the Stripe Request and get the data back.
                var response = PerformStripeInStorePurchaseCheckout();
                // If it was successful we will now have a redirect link
                if (response != null)
                {
                    // Try to save the data to our database
                    var result = AddInvoiceToDatabase();

                    if (result)
                    {
                        output.InvoiceId = invoice.InvoiceId;
                        output.Status = InvoiceStatus.Payment_Successful;

                        PaymentGateway pg = new PaymentGateway();
                        var voice = pg.GetDisplayInvoice(invoice.InvoiceId);
                        Store.StoreGateway sg = new Store.StoreGateway();
                        sg.HandleStoreItemPayments(voice);
                    }
                    else
                    {
                        // Could not save to our database
                        output.Error = "Failed to save to RDN database";
                        output.Status = InvoiceStatus.Failed;
                    }
                }
                else
                {
                    // Could not create a google link
                    //output.Error = response.Error;
                    output.Status = InvoiceStatus.Failed;
                }
            }
            else if (invoice.ChargeType == ChargeTypeEnum.Stripe_Checkout)
            {

                // Create the Stripe Request and get the data back.
                var response = ChargeStripeCheckoutPayment();
                // If it was successful we will now have a redirect link
                if (response != null)
                {
                    // Try to save the data to our database
                    var result = AddInvoiceToDatabase();

                    if (result)
                    {
                        output.InvoiceId = invoice.InvoiceId;
                        output.Status = InvoiceStatus.Payment_Successful;

                        var emailData = new Dictionary<string, string>
                                        {
                                            { "Why: ", invoice.Note }, 
                                            { "invoiceId",invoice.InvoiceId.ToString().Replace("-","") },
                                            { "Paid",invoice.FinancialData.TotalIncludingTax.ToString("N2")}
                                        };
                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmailAdmin, EmailServer.EmailServer.DEFAULT_SUBJECT + " Receipt For League Subscription", emailData, EmailServer.EmailServerLayoutsEnum.Default);
                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultKrisWorlidgeEmailAdmin, EmailServer.EmailServer.DEFAULT_SUBJECT + " New Payment Made", emailData, EmailServer.EmailServerLayoutsEnum.Default);

                    }
                    else
                    {
                        // Could not save to our database
                        output.Error = "Failed to save to RDN database";
                        output.Status = InvoiceStatus.Failed;
                    }
                }
                else
                {
                    // Could not create a google link
                    //output.Error = response.Error;
                    output.Status = InvoiceStatus.Failed;
                }
            }
        }
        private void CancelStripeSubscription(CreateInvoiceReturn output)
        {
            try
            {
                // Create the Stripe Request and get the data back.
                output.InvoiceId = invoice.InvoiceId;
                var response = PerformStripeSubscriptionCancellation(output);
                // If it was successful we will now have a redirect link
                if (response == true)
                {
                    ManagementContext dc = new ManagementContext();
                    var invoiceDb = dc.Invoices.Include("Items")
                        .Include("Payments").Include("Logs")
                        .Include("Subscription").Include("InvoiceShipping")
                        .Include("InvoiceBilling").Include("Merchant").Where(x => x.InvoiceId == invoice.InvoiceId).FirstOrDefault();


                    output.InvoiceId = invoice.InvoiceId;
                    output.Status = InvoiceStatus.Cancelled;

                    EmailLeagueAboutCancelledSubscription(invoiceDb.Subscription.InternalObject, invoice.InvoiceId, invoiceDb.Subscription.ValidUntil, invoiceDb.InvoiceBilling.Email);

                }
                else
                {
                    output.Status = InvoiceStatus.Failed;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        private void CompleteNewStripeSubscription(CreateInvoiceReturn output)
        {
            try
            {
                // Create the Stripe Request and get the data back.
                var response = PerformStripeSubscriptionCheckout(output);
                // If it was successful we will now have a redirect link
                if (response != null)
                {
                    // Try to save the data to our database
                    var result = AddInvoiceToDatabase();

                    if (result)
                    {
                        // Everything was successful
                        if (invoice.Subscription != null)
                        {
                            output.InvoiceId = invoice.InvoiceId;
                            output.Status = InvoiceStatus.Subscription_Running;
                            output.SubscriptionEndsOn = invoice.Subscription.ValidUntil;
                            if (invoice.Subscription.SubscriptionPeriodStripe != SubscriptionPeriodStripe.Monthly_RN_Sponsor)
                            {
                                EmailLeagueAboutSuccessfulSubscription(invoice.Subscription.InternalObject, invoice.InvoiceId, invoice.Subscription.Price, invoice.Subscription.ValidUntil, invoice.InvoiceBilling.Email);

                                RDN.Library.Classes.League.LeagueFactory.UpdateLeagueSubscriptionPeriod(output.SubscriptionEndsOn, true, invoice.Subscription.InternalObject);
                            }
                            else
                            {

                                var emailData = new Dictionary<string, string>
                                        {
                                            { "Why: ", invoice.Note }, 
                                            { "invoiceId",invoice.InvoiceId.ToString().Replace("-","") },
                                            { "Paid",invoice.FinancialData.TotalIncludingTax.ToString("N2")}
                                        };
                                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmailAdmin, EmailServer.EmailServer.DEFAULT_SUBJECT + " Receipt For RN Subscription", emailData, EmailServer.EmailServerLayoutsEnum.Default);
                                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultKrisWorlidgeEmailAdmin, EmailServer.EmailServer.DEFAULT_SUBJECT + " New Payment Made", emailData, EmailServer.EmailServerLayoutsEnum.Default);

                            }
                        }
                    }
                    else
                    {
                        // Could not save to our database
                        output.Error = "Failed to save to RDN database";
                        output.Status = InvoiceStatus.Failed;
                    }
                }
                else
                {
                    // Could not create a google link
                    //output.Error = response.Error;
                    if (output.Status != InvoiceStatus.Card_Was_Declined)
                        output.Status = InvoiceStatus.Failed;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        /// <summary>
        /// stripe is just telling us they are about to make a new charge and we need to create an invoice.
        /// </summary>
        /// <param name="output"></param>
        private void CompleteStripeSubscriptionUpdated(CreateInvoiceReturn output)
        {
            try
            {
                // Create the Stripe Request and get the data back.
                // Try to save the data to our database
                var result = AddInvoiceToDatabase();

                if (result)
                {
                    // Everything was successful
                    //we just added a new invoice.  Thats all that happened.
                }
                else
                {
                    // Could not save to our database
                    output.Error = "Failed to save to RDN database";
                    output.Status = InvoiceStatus.Failed;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        public static void EmailLeagueAboutCancelledSubscription(Guid leagueId, Guid invoiceId, DateTime validUntil, string secondEmail)
        {
            try
            {
                var league = League.LeagueFactory.GetLeague(leagueId);
                var emailData = new Dictionary<string, string>
                                        {
                                            { "leaguename",league.Name}, 
                                            { "invoiceId", invoiceId.ToString().Replace("-","") },
                                            { "expires", validUntil.ToShortDateString()}
                                        };
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, league.Email, EmailServer.EmailServer.DEFAULT_SUBJECT + " League Subscription was Canceled", emailData, EmailServer.EmailServerLayoutsEnum.SubscriptionWasCancelled);
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmailAdmin, EmailServer.EmailServer.DEFAULT_SUBJECT + " League Subscription was Canceled", emailData, EmailServer.EmailServerLayoutsEnum.SubscriptionWasCancelled);
                if (league.Email != secondEmail && !String.IsNullOrEmpty(secondEmail))
                {
                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, secondEmail, EmailServer.EmailServer.DEFAULT_SUBJECT + " League Subscription was Canceled", emailData, EmailServer.EmailServerLayoutsEnum.SubscriptionWasCancelled);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        public static void EmailLeagueAboutSuccessfulSubscription(Guid leagueId, Guid invoiceId, decimal price, DateTime validUntil, string secondEmail)
        {
            try
            {
                var league = League.LeagueFactory.GetLeague(leagueId);
                var emailData = new Dictionary<string, string>
                                        {
                                            { "leaguename",league.Name}, 
                                            { "invoiceId", invoiceId.ToString().Replace("-","") },
                                            { "amountPaid", price.ToString("N2")},
                                            { "expires", validUntil.ToShortDateString()}
                                        };
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, league.Email, EmailServer.EmailServer.DEFAULT_SUBJECT + " Receipt For League Subscription", emailData, EmailServer.EmailServerLayoutsEnum.ReceiptForLeagueSubscription);
                if (league.Email != secondEmail && !String.IsNullOrEmpty(secondEmail))
                {
                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, secondEmail, EmailServer.EmailServer.DEFAULT_SUBJECT + " Receipt For League Subscription", emailData, EmailServer.EmailServerLayoutsEnum.ReceiptForLeagueSubscription);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        public static void EmailLeagueAboutFailedSubscription(Guid leagueId, Guid invoiceId, decimal price, DateTime validUntil, string secondEmail)
        {
            try
            {
                var league = League.LeagueFactory.GetLeague(leagueId);
                var emailData = new Dictionary<string, string>
                                        {
                                            { "emailAddress",LibraryConfig.DefaultInfoEmail}
                                        };
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, league.Email, EmailServer.EmailServer.DEFAULT_SUBJECT + " League Subscription Failed Payment", emailData, EmailServer.EmailServerLayoutsEnum.LeagueSubscriptionFailedPayment);
                if (league.Email != secondEmail && !String.IsNullOrEmpty(secondEmail))
                {
                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, secondEmail, EmailServer.EmailServer.DEFAULT_SUBJECT + " League Subscription Failed Payment", emailData, EmailServer.EmailServerLayoutsEnum.LeagueSubscriptionFailedPayment);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }




        private bool AddInvoiceToDatabase()
        {
            try
            {
                var mc = new ManagementContext();

                // Create a new invoice db object
                var dbinvoice = new DataModels.PaymentGateway.Invoices.Invoice();
                dbinvoice.Expires = DateTime.Now.AddDays(14);
                dbinvoice.InvoiceId = invoice.InvoiceId;
                dbinvoice.PaymentProvider = (byte)invoice.PaymentProvider;
                dbinvoice.PaymentProviderCustomerId = invoice.PaymentProviderCustomerId;
                dbinvoice.InvoiceStatus = (byte)invoice.InvoiceStatus;
                dbinvoice.InvoiceStatusUpdated = DateTime.Now;
                dbinvoice.AdminNote = invoice.AdminNote;
                dbinvoice.Note = invoice.Note;
                dbinvoice.UserId = invoice.UserId;
                dbinvoice.ShoppingCartId = invoice.ShoppingCartId;
                dbinvoice.StripeTokenId = invoice.StripeToken;

                if (invoice.Subscription == null)
                {
                    dbinvoice.BasePriceForItems = invoice.FinancialData.BasePriceForItems;
                    dbinvoice.TotalBeforeTax = invoice.FinancialData.TotalBeforeTax;
                    dbinvoice.TaxRate = invoice.FinancialData.TaxRate;
                    dbinvoice.Tax = invoice.FinancialData.Tax;
                    dbinvoice.TotalIncludingTax = invoice.FinancialData.TotalIncludingTax;
                    dbinvoice.Shipping = invoice.FinancialData.ShippingCost;
                    dbinvoice.ShippingType = (byte)invoice.ShippingType;
                    dbinvoice.CurrencyRate = mc.ExchangeRates.Where(x => x.CurrencyAbbrName == invoice.Currency).FirstOrDefault();

                    dbinvoice.CreditCardCompanyProcessorDeductedFee = invoice.FinancialData.CCProcessorFees;
                    dbinvoice.RDNDeductedFee = invoice.FinancialData.RDNationFees;
                }
                else
                {
                    dbinvoice.BasePriceForItems = invoice.Subscription.Price;
                    dbinvoice.TotalBeforeTax = invoice.Subscription.Price;
                    dbinvoice.TaxRate = 0;
                    dbinvoice.Tax = 0;
                    dbinvoice.TotalIncludingTax = invoice.Subscription.Price;
                    dbinvoice.Shipping = 0;
                    dbinvoice.ShippingType = (byte)ShippingType.None;
                    dbinvoice.CurrencyRate = mc.ExchangeRates.Where(x => x.CurrencyAbbrName == invoice.Currency).FirstOrDefault();
                    dbinvoice.CreditCardCompanyProcessorDeductedFee = 0.0M;
                    dbinvoice.RDNDeductedFee = 0.0M;
                }

                // Assume that the merchant is in the database, will throw an error otherwise
                var merchant = mc.Merchants.First(x => x.MerchantId.Equals(invoice.MerchantId));
                dbinvoice.Merchant = merchant;

                // Add invoice contact and billing addresses
                dbinvoice.InvoiceShipping = ConvertInvoiceContactToDbInvoiceContact(invoice.InvoiceShipping);
                dbinvoice.InvoiceBilling = ConvertInvoiceContactToDbInvoiceContact(invoice.InvoiceBilling);

                // Add subscription if we have one
                if (invoice.Subscription != null)
                {
                    var subscription = new DataModels.PaymentGateway.Invoices.InvoiceSubscription();
                    subscription.ArticleNumber = invoice.Subscription.ArticleNumber;
                    subscription.Description = invoice.Subscription.Description;
                    subscription.DescriptionRecurring = invoice.Subscription.DescriptionRecurring;
                    subscription.DigitalPurchaseText = invoice.Subscription.DigitalPurchaseText;
                    subscription.InternalObject = invoice.Subscription.InternalObject;
                    subscription.Name = invoice.Subscription.Name;
                    subscription.NameRecurring = invoice.Subscription.NameRecurring;
                    subscription.Price = invoice.Subscription.Price;
                    if (invoice.PaymentProvider == PaymentProvider.Paypal)
                    {
                        subscription.SubscriptionPeriod = (byte)invoice.Subscription.SubscriptionPeriod;
                    }
                    else if (invoice.PaymentProvider == PaymentProvider.Stripe)
                    {
                        subscription.SubscriptionPeriod = (byte)invoice.Subscription.SubscriptionPeriodStripe;
                    }

                    subscription.SubscriptionPeriodLengthInDays = invoice.Subscription.SubscriptionPeriodLengthInDays;
                    subscription.ValidUntil = invoice.Subscription.ValidUntil;
                    dbinvoice.Expires = invoice.Subscription.ValidUntil;
                    dbinvoice.Subscription = subscription;
                }
                if (invoice.Paywall != null)
                {
                    var paywall = new DataModels.PaymentGateway.Invoices.InvoicePaywall();

                    paywall.Description = invoice.Paywall.Description;
                    paywall.InternalObject = invoice.Paywall.InternalObject;
                    paywall.Name = invoice.Paywall.Name;
                    paywall.BasePrice = invoice.Paywall.Price;
                    Random rnd = new Random();
                    paywall.GeneratedPassword = rnd.Next(100000, 999999).ToString();
                    paywall.PaywallLocation = invoice.Paywall.PaywallLocation;
                    paywall.MemberPaidId = invoice.Paywall.MemberPaidId;
                    paywall.PaywallPriceTypeEnum = (byte)invoice.Paywall.PriceType;
                    paywall.ValidUntil = invoice.Paywall.ValidUntil;
                    paywall.Paywall = mc.Paywalls.Where(x => x.PaywallId == invoice.Paywall.PaywallId).FirstOrDefault();
                    dbinvoice.Expires = invoice.Paywall.ValidUntil;
                    dbinvoice.Paywall = paywall;
                }

                // Add db items
                foreach (var invoiceItem in invoice.ItemsInvoice)
                {
                    var dbitem = new DataModels.PaymentGateway.Invoices.InvoiceItem();
                    dbitem.ArticleNumber = invoiceItem.ArticleNumber;
                    dbitem.ArticleNumber = invoiceItem.Article2Number;
                    dbitem.Description = invoiceItem.Description;
                    dbitem.Name = invoiceItem.Name;
                    dbitem.Price = invoiceItem.Price;
                    dbitem.Shipping = invoiceItem.TotalShipping;
                    dbitem.Quantity = invoiceItem.Quantity;
                    dbitem.Weight = invoiceItem.Weight;
                    dbitem.SizeOfItem = invoiceItem.SizeOfItem;
                    if (invoiceItem.ColorOfItem != null)
                        dbitem.ColorOfItem = mc.Colors.Where(x => x.ColorIdCSharp == invoiceItem.ColorOfItem).FirstOrDefault();
                    dbitem.StoreItemId = invoiceItem.StoreItemId;
                    dbinvoice.Items.Add(dbitem);
                }
                foreach (var invoiceItem in invoice.ItemsDues)
                {
                    var dbitem = new DataModels.PaymentGateway.Invoices.InvoiceDuesItem();
                    dbitem.BasePrice = invoiceItem.BasePrice;
                    dbitem.DuesId = invoiceItem.DuesId;
                    dbitem.DuesItemId = invoiceItem.DuesItemId;
                    dbitem.MemberPaidId = invoiceItem.MemberPaidId;
                    dbitem.PriceAfterFees = invoiceItem.PriceAfterFees;
                    dbitem.ProcessorFees = invoiceItem.ProcessorFees;
                    dbitem.RDNationsFees = invoiceItem.RDNationsFees;
                    dbitem.WhoPaysFees = Convert.ToInt32(invoiceItem.WhoPaysFees);
                    dbitem.Description = invoiceItem.Description;
                    dbitem.Name = invoiceItem.Name;
                    dbinvoice.DuesItems.Add(dbitem);
                }

                foreach (var invoiceItem in invoice.RNWriterPayouts)
                {
                    var dbitem = new DataModels.PaymentGateway.Invoices.InvoiceWriterPayoutItem();
                    dbitem.BasePrice = invoiceItem.BasePrice;
                    dbitem.Name = invoiceItem.Name;
                    dbitem.PaymentRequestedDateTime = invoiceItem.PaymentRequestedDateTime;
                    dbitem.PayoutId = invoiceItem.PayoutId;
                    dbitem.PriceAfterFees = invoiceItem.PriceAfterFeeDeduction;
                    dbitem.UserPaidId = invoiceItem.UserPaidId;
                    dbitem.WhoPaysFees = Convert.ToInt32(invoiceItem.WhoPaysFees);
                    dbinvoice.WriterPayouts.Add(dbitem);
                }

                mc.Invoices.Add(dbinvoice);

                // Save it and return if the save was successful
                int result = mc.SaveChanges();
                return result > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        private DataModels.PaymentGateway.Invoices.InvoiceContactInfo ConvertInvoiceContactToDbInvoiceContact(InvoiceContactInfo invoiceContact)
        {
            // If not null then turn a class contact info to a db contact info
            if (invoiceContact == null) return null;
            var output = new DataModels.PaymentGateway.Invoices.InvoiceContactInfo();
            try
            {
                output.CompanyName = invoiceContact.CompanyName;
                output.Country = invoiceContact.Country;
                output.FirstName = invoiceContact.FirstName;
                output.LastName = invoiceContact.LastName;
                output.State = invoiceContact.State;
                output.StateCode = invoiceContact.StateCode;
                output.Street = invoiceContact.Street;
                output.Street2 = invoiceContact.Street2;
                output.City = invoiceContact.City;
                output.Zip = invoiceContact.Zip;
                output.Email = invoiceContact.Email;
                output.Phone = invoiceContact.Phone;
                output.Fax = invoiceContact.Fax;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return output;
        }
        private StripeCustomer PerformStripeSubscriptionCheckout(CreateInvoiceReturn invoiceReturn)
        {
            try
            {
                var myCustomer = new StripeCustomerCreateOptions();
                if (invoice.InvoiceBilling != null)
                {
                    myCustomer.CardAddressCity = invoice.InvoiceBilling.City;
                    myCustomer.CardAddressCountry = invoice.InvoiceBilling.Country;
                    myCustomer.CardAddressLine1 = invoice.InvoiceBilling.Street;
                    myCustomer.CardAddressState = invoice.InvoiceBilling.State;
                    myCustomer.CardAddressZip = invoice.InvoiceBilling.Zip;
                    myCustomer.Email = invoice.InvoiceBilling.Email;
                    myCustomer.Description = LibraryConfig.ConnectionStringName;
                }
                if (invoice.Subscription != null)
                {
                    myCustomer.TokenId = invoice.Subscription.ArticleNumber;
                    if (invoice.Subscription.SubscriptionPeriodStripe == SubscriptionPeriodStripe.Monthly)
                    {
                        myCustomer.PlanId = StripePlanNames.Monthly_Plan.ToString();
                    }
                    else if (invoice.Subscription.SubscriptionPeriodStripe == SubscriptionPeriodStripe.Six_Months)
                    {
                        myCustomer.PlanId = StripePlanNames.Six_Month_League_Subscription.ToString();
                    }
                    else if (invoice.Subscription.SubscriptionPeriodStripe == SubscriptionPeriodStripe.Three_Months)
                    {
                        myCustomer.PlanId = StripePlanNames.Three_Month_League_Subscription.ToString();
                    }
                    else if (invoice.Subscription.SubscriptionPeriodStripe == SubscriptionPeriodStripe.Yearly)
                    {
                        myCustomer.PlanId = StripePlanNames.Yearly_League_Subscription.ToString();
                    }
                    else if (invoice.Subscription.SubscriptionPeriodStripe == SubscriptionPeriodStripe.Monthly_RN_Sponsor)
                    {
                        myCustomer.PlanId = StripePlanNames.Monthly_RN_Sponsor.ToString();
                    }
                }
                //creates the customer
                //adds the subscription
                //charges the customer.
                var customerService = new StripeCustomerService();
                StripeCustomer stripeCustomer = customerService.Create(myCustomer);
                invoice.PaymentProviderCustomerId = stripeCustomer.Id;

                return stripeCustomer;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
                if (exception.Message.Contains("Your card was declined"))
                    invoiceReturn.Status = InvoiceStatus.Card_Was_Declined;
            }
            return null;
        }
        private bool PerformStripeSubscriptionCancellation(CreateInvoiceReturn invoiceReturn)
        {
            try
            {
                ManagementContext dc = new ManagementContext();
                var invoice = dc.Invoices.Include("Items")
                    .Include("Payments").Include("Logs")
                    .Include("Subscription").Include("InvoiceShipping")
                    .Include("InvoiceBilling").Include("Merchant").Where(x => x.InvoiceId == invoiceReturn.InvoiceId).FirstOrDefault();

                if (invoice != null)
                {
                    invoice.InvoiceStatus = (byte)InvoiceStatus.Cancelled;
                    invoice.PaymentProvider = invoice.PaymentProvider;
                    invoice.BasePriceForItems = invoice.BasePriceForItems;
                    invoice.Merchant = invoice.Merchant;
                    var customerService = new StripeCustomerService();
                    StripeSubscription subscription = customerService.CancelSubscription(invoice.PaymentProviderCustomerId);
                    int c = dc.SaveChanges();
                    return c > 0;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
                if (exception.Message.Contains("Your card was declined"))
                    invoiceReturn.Status = InvoiceStatus.Card_Was_Declined;
            }
            return false;
        }

        private CreateInvoiceReturn ChargeStripeCheckoutPayment()
        {
            // Create the Stripe Request and get the data back.
            CreateInvoiceReturn output = new CreateInvoiceReturn();

            var myCustomer = new StripeCustomerCreateOptions();
            if (invoice.InvoiceBilling != null)
            {
                myCustomer.CardAddressCity = invoice.InvoiceBilling.City;
                myCustomer.CardAddressCountry = invoice.InvoiceBilling.Country;
                myCustomer.CardAddressLine1 = invoice.InvoiceBilling.Street;
                myCustomer.CardAddressState = invoice.InvoiceBilling.State;
                myCustomer.CardAddressZip = invoice.InvoiceBilling.Zip;
                myCustomer.Email = invoice.InvoiceBilling.Email;
            }

            myCustomer.TokenId = invoice.StripeToken;

            var myCharge = new StripeChargeCreateOptions();

            var customerService = new StripeCustomerService();

            StripeCustomer stripeCustomer = customerService.Create(myCustomer);

            // always set these properties
            //need to convert to cents because thats what stripe uses.
            myCharge.AmountInCents = (int)(invoice.FinancialData.TotalIncludingTax * 100);
            myCharge.Currency = invoice.Currency.ToString();

            // set this if you want to
            myCharge.Description = invoice.Note;

            // set this property if using a token
            myCharge.CustomerId = stripeCustomer.Id;
            myCharge.Capture = true;

            var chargeService = new StripeChargeService();
            StripeCharge stripeCharge = chargeService.Create(myCharge);

            output.InvoiceId = invoice.InvoiceId;
            output.Status = InvoiceStatus.Stripe_Customer_Created_And_Charged;
            invoice.InvoiceStatus = InvoiceStatus.Stripe_Customer_Created_And_Charged;
            return output;
        }


        private StripeCharge PerformStripeInStorePurchaseCheckout()
        {
            try
            {
                PaymentGateway pg = new PaymentGateway();
                var merchant = pg.GetMerchant(invoice.MerchantId);
                int amountInCentsTotalPayment = Convert.ToInt32((invoice.FinancialData.BasePriceForItems + invoice.FinancialData.ShippingCost) * 100);
                int RDNationsCut = Convert.ToInt32(amountInCentsTotalPayment - (invoice.FinancialData.PriceSubtractingRDNationFees * 100));
                var stripeService = new StripeChargeService(merchant.StripeConnectToken); //The token returned from the above method
                var stripeChargeOption = new StripeChargeCreateOptions() { AmountInCents = amountInCentsTotalPayment, Currency = "usd", Description = invoice.InvoiceId.ToString().Replace("-", "") + ": Payment to " + merchant.ShopName, TokenId = invoice.StripeToken, ApplicationFeeInCents = RDNationsCut };

                if (invoice.InvoiceBilling != null)
                {
                    stripeChargeOption.CardAddressCity = invoice.InvoiceBilling.City;
                    stripeChargeOption.CardAddressCountry = invoice.InvoiceBilling.Country;
                    stripeChargeOption.CardAddressLine1 = invoice.InvoiceBilling.Street;
                    stripeChargeOption.CardAddressState = invoice.InvoiceBilling.State;
                    stripeChargeOption.CardAddressZip = invoice.InvoiceBilling.Zip;
                }
                var response = stripeService.Create(stripeChargeOption);
                invoice.PaymentProviderCustomerId = response.Id;

                return response;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        private StripeCharge PerformStripePaywallPurchaseCheckout()
        {
            try
            {
                PaymentGateway pg = new PaymentGateway();
                var merchant = pg.GetMerchant(invoice.MerchantId);
                int amountInCentsTotalPayment = Convert.ToInt32((invoice.FinancialData.BasePriceForItems) * 100);
                int RDNationsCut = Convert.ToInt32(amountInCentsTotalPayment - (invoice.FinancialData.PriceSubtractingRDNationFees * 100));
                var stripeService = new StripeChargeService(merchant.StripeConnectToken); //The token returned from the above method
                var stripeChargeOption = new StripeChargeCreateOptions() { AmountInCents = amountInCentsTotalPayment, Currency = "usd", Description = invoice.InvoiceId.ToString().Replace("-", "") + ": Payment to " + merchant.OwnerName, TokenId = invoice.StripeToken, ApplicationFeeInCents = RDNationsCut };

                var response = stripeService.Create(stripeChargeOption);
                invoice.PaymentProviderCustomerId = response.Id;

                return response;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        private StripeCharge PerformStripeRefund()
        {
            try
            {
                PaymentGateway pg = new PaymentGateway();
                var merchant = pg.GetMerchant(invoice.MerchantId);
                var voice = pg.GetDisplayInvoice(invoice.InvoiceId);
                int amountInCentsTotalRefund = Convert.ToInt32((invoice.FinancialData.RefundAmount) * 100);
                var chargeService = new StripeChargeService(merchant.StripeConnectToken);
                StripeCharge stripeCharge = chargeService.Refund(voice.CustomerId, amountInCentsTotalRefund);
                invoice.PaymentProviderRefundedId = stripeCharge.Id;

                return stripeCharge;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        #region paypal
        /// <summary>
        /// 
        /// </summary>
        /// <returns>url to redirect to paypal</returns>
        private string PerformPaypalSubscriptionCheckout()
        {
            try
            {
                PaypalPayment sendingPayPal = new PaypalPayment();
                sendingPayPal.Amount = (double)invoice.Subscription.Price;
                sendingPayPal.BuyerEmailAddress = invoice.InvoiceBilling.Email;

                sendingPayPal.Code = invoice.Currency;
                sendingPayPal.ItemName = invoice.Subscription.Description;
                if (invoice.IsLive)
                {

                    sendingPayPal.ReturnUrl = ServerConfig.LEAGUE_SUBSCRIPTION_RECIEPT + invoice.InvoiceId.ToString().Replace("-", "");
                    sendingPayPal.SellerEmailAddress = LibraryConfig.DefaultAdminEmailAdmin;
                    sendingPayPal.CancelUrl = ServerConfig.LEAGUE_SUBSCRIPTION_ADDSUBSUBSCRIBE + invoice.Subscription.InternalObject.ToString().Replace("-", "");
                }
                else
                {

                    sendingPayPal.ReturnUrl = ServerConfig.LEAGUE_SUBSCRIPTION_RECIEPT_DEBUG + invoice.InvoiceId.ToString().Replace("-", "");
                    sendingPayPal.SellerEmailAddress = ServerConfig.PAYPAL_SELLER_DEBUG_ADDRESS;
                    sendingPayPal.CancelUrl = ServerConfig.LEAGUE_SUBSCRIPTION_ADDSUBSUBSCRIBE_DEBUG + invoice.Subscription.InternalObject.ToString().Replace("-", "");
                }

                sendingPayPal.InvoiceNumber = invoice.InvoiceId.ToString();
                sendingPayPal.LogoUrl = LibraryConfig.LogoURL;

                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmailAdmin, "Paypal Payment Sent To Paypal", invoice.InvoiceId + " Amount:" + invoice.Subscription.Price);

                return sendingPayPal.RedirectToPaypal(invoice.IsLive);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return String.Empty;
        }

        private string PerformPaypalPaywallPayment()
        {
            try
            {
                Payment.PaymentGateway pg = new Payment.PaymentGateway();
                var merchant = pg.GetMerchant(invoice.MerchantId);

                if (merchant != null)
                {
                    ReceiverList receiverList = new ReceiverList();
                    //RDNation as a reciever
                    Receiver recRDNation = new Receiver(invoice.FinancialData.BasePriceForItems);
                    if (invoice.IsLive)
                        recRDNation.email = LibraryConfig.DefaultAdminEmailAdmin;
                    else
                        recRDNation.email = ServerConfig.PAYPAL_SELLER_DEBUG_ADDRESS;
                    //make sure RDNation can be paid.
                    if (LibraryConfig.DefaultAdminEmailAdmin != merchant.PaypalEmail)
                        recRDNation.primary = true;

                    recRDNation.invoiceId = invoice.InvoiceId.ToString().Replace("-", "") + ": " + invoice.Paywall.Description;
                    recRDNation.paymentType = PaymentTypeEnum.SERVICE.ToString();
                    receiverList.receiver.Add(recRDNation);
                    //no need to add a second receiver if the seller is RDNation
                    if (LibraryConfig.DefaultAdminEmailAdmin != merchant.PaypalEmail)
                    {
                        Receiver recLeague = new Receiver(invoice.FinancialData.PriceSubtractingRDNationFees);
                        recLeague.amount = invoice.FinancialData.PriceSubtractingRDNationFees;
                        if (invoice.IsLive)
                            recLeague.email = merchant.PaypalEmail;
                        else
                            recLeague.email = "cheeta_1359429163_per@gmail.com";

                        recLeague.primary = false;
                        recLeague.invoiceId = invoice.InvoiceId.ToString().Replace("-", "") + ": " + invoice.Paywall.Name;
                        recLeague.paymentType = PaymentTypeEnum.GOODS.ToString();
                        receiverList.receiver.Add(recLeague);
                    }
                    PayRequest req = new PayRequest(new RequestEnvelope("en_US"), ActionTypeEnum.PAY.ToString(), invoice.Paywall.PaywallLocation, Currency.USD.ToString(), receiverList, ServerConfig.PAYWALL_RECEIPT_URL + invoice.InvoiceId.ToString().Replace("-", ""));
                    //no need to note primary if RDNation is the seller.
                    if (LibraryConfig.DefaultAdminEmailAdmin != merchant.PaypalEmail)
                        req.feesPayer = FeesPayerEnum.PRIMARYRECEIVER.ToString();

                    req.memo = invoice.Paywall.Description;
                    req.reverseAllParallelPaymentsOnError = false;
                    if (invoice.IsLive)
                        req.ipnNotificationUrl = LibraryConfig.PaypalIPNHandler;
                    else
                        req.ipnNotificationUrl = LibraryConfig.PaypalIPNHandlerDebug;

                    // All set. Fire the request            
                    AdaptivePaymentsService service = new AdaptivePaymentsService();
                    PayResponse resp = service.Pay(req);

                    // Display response values. 
                    Dictionary<string, string> keyResponseParams = new Dictionary<string, string>();
                    string redirectUrl = null;
                    if (!(resp.responseEnvelope.ack == AckCode.FAILURE) &&
                        !(resp.responseEnvelope.ack == AckCode.FAILUREWITHWARNING))
                    {
                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmailAdmin, "Paypal Paywall Wating to be Purchased", invoice.InvoiceId + " Amount:" + invoice.FinancialData.BasePriceForItems + " :" + merchant.PaypalEmail);

                        redirectUrl = PaypalPayment.GetBaseUrl(invoice.IsLive);

                        redirectUrl += "?cmd=_ap-payment&paykey=" + resp.payKey;
                        keyResponseParams.Add("Pay key", resp.payKey);
                        keyResponseParams.Add("Payment execution status", resp.paymentExecStatus);
                        if (resp.defaultFundingPlan != null && resp.defaultFundingPlan.senderFees != null)
                        {
                            keyResponseParams.Add("Sender fees", resp.defaultFundingPlan.senderFees.amount +
                                                        resp.defaultFundingPlan.senderFees.code);
                        }

                        //Selenium Test Case
                        keyResponseParams.Add("Acknowledgement", resp.responseEnvelope.ack.ToString());
                        return redirectUrl;
                    }
                    else
                    {

                        throw new Exception("Failure Payment " + merchant.PaypalEmail + ":" + invoice.InvoiceId + ":" + resp.error.FirstOrDefault().message);
                    }

                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return String.Empty;
        }

        private string PerformPaypalInStorePaymentCheckout()
        {
            try
            {
                var items = invoice.ItemsInvoice;

                if (items.Count > 0)
                {
                    Payment.PaymentGateway pg = new Payment.PaymentGateway();
                    var merchant = pg.GetMerchant(invoice.MerchantId);

                    if (merchant != null)
                    {
                        ReceiverList receiverList = new ReceiverList();
                        //RDNation as a reciever
                        Receiver recRDNation = new Receiver(invoice.FinancialData.BasePriceForItems + invoice.FinancialData.ShippingCost);
                        if (invoice.IsLive)
                            recRDNation.email = LibraryConfig.DefaultAdminEmailAdmin;
                        else
                            recRDNation.email = ServerConfig.PAYPAL_SELLER_DEBUG_ADDRESS;
                        if (LibraryConfig.DefaultAdminEmailAdmin != merchant.PaypalEmail)
                            recRDNation.primary = true;
                        //if we modify this invoiceID, 
                        //you need to modify this code here: 
                        recRDNation.invoiceId = invoice.InvoiceId.ToString().Replace("-", "") + ":" + LibraryConfig.ConnectionStringName + ": Payment to " + merchant.ShopName;
                        recRDNation.paymentType = PaymentTypeEnum.GOODS.ToString();
                        receiverList.receiver.Add(recRDNation);
                        if (LibraryConfig.DefaultAdminEmailAdmin != merchant.PaypalEmail)
                        {
                            Receiver recLeague = new Receiver(invoice.FinancialData.PriceSubtractingRDNationFees);
                            recLeague.amount = invoice.FinancialData.PriceSubtractingRDNationFees;
                            if (invoice.IsLive)
                                recLeague.email = merchant.PaypalEmail;
                            else
                                recLeague.email = "cheeta_1359429163_per@gmail.com";
                            recLeague.primary = false;
                            //if we modify this invoiceID, 
                            //you need to modify this code here: 
                            recLeague.invoiceId = invoice.InvoiceId.ToString().Replace("-", "") + ":" + LibraryConfig.ConnectionStringName + ": Payment to " + merchant.ShopName;
                            recLeague.paymentType = PaymentTypeEnum.GOODS.ToString();
                            receiverList.receiver.Add(recLeague);
                        }

                        PayRequest req = new PayRequest(new RequestEnvelope("en_US"), ActionTypeEnum.PAY.ToString(), ServerConfig.STORE_MERCHANT_CART_URL + merchant.MerchantId.ToString().Replace("-", ""), invoice.Currency, receiverList, ServerConfig.STORE_MERCHANT_RECEIPT_URL + invoice.InvoiceId.ToString().Replace("-", ""));
                        if (LibraryConfig.DefaultAdminEmailAdmin != merchant.PaypalEmail)
                            req.feesPayer = FeesPayerEnum.PRIMARYRECEIVER.ToString();
                        req.memo = "Payment to " + merchant.ShopName + ": " + invoice.InvoiceId.ToString().Replace("-", "");
                        req.reverseAllParallelPaymentsOnError = false;
                        if (invoice.IsLive)
                            req.ipnNotificationUrl = LibraryConfig.PaypalIPNHandler;
                        else
                            req.ipnNotificationUrl = LibraryConfig.PaypalIPNHandlerDebug;

                        // All set. Fire the request            
                        AdaptivePaymentsService service = new AdaptivePaymentsService();
                        PayResponse resp = service.Pay(req);

                        // Display response values. 
                        Dictionary<string, string> keyResponseParams = new Dictionary<string, string>();
                        string redirectUrl = null;
                        if (!(resp.responseEnvelope.ack == AckCode.FAILURE) &&
                            !(resp.responseEnvelope.ack == AckCode.FAILUREWITHWARNING))
                        {
                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmailAdmin, "Paypal Store Item Waiting To be Purchased", invoice.InvoiceId + " Amount:" + invoice.FinancialData.BasePriceForItems + ":" + merchant.PaypalEmail);

                            redirectUrl = PaypalPayment.GetBaseUrl(invoice.IsLive);

                            redirectUrl += "?cmd=_ap-payment&paykey=" + resp.payKey;
                            keyResponseParams.Add("Pay key", resp.payKey);
                            keyResponseParams.Add("Payment execution status", resp.paymentExecStatus);
                            if (resp.defaultFundingPlan != null && resp.defaultFundingPlan.senderFees != null)
                            {
                                keyResponseParams.Add("Sender fees", resp.defaultFundingPlan.senderFees.amount +
                                                            resp.defaultFundingPlan.senderFees.code);
                            }

                            //Selenium Test Case
                            keyResponseParams.Add("Acknowledgement", resp.responseEnvelope.ack.ToString());
                            return redirectUrl;
                        }
                        else
                        {

                            throw new Exception("Failure Payment " + merchant.PaypalEmail + ":" + invoice.InvoiceId + ":" + resp.error.FirstOrDefault().message);
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return String.Empty;
        }

        private void PerformPaypalRollinNewsWritersPayment(CreateInvoiceReturn output)
        {
            try
            {
                var status = (byte)InvoiceStatus.Payment_Awaiting_For_Mass_Payout;
                var dc = new ManagementContext();
                var invoices = dc.Invoices.Include("InvoiceBilling").Include("WriterPayouts").Where(x => x.InvoiceStatus == status).ToList();


                // Create request object
                List<Fund> tempFundsBeingPaid = new List<Fund>();
                MassPayRequestType request = new MassPayRequestType();
                ReceiverInfoCodeType receiverInfoType = ReceiverInfoCodeType.EMAILADDRESS;

                request.ReceiverType = receiverInfoType;
                // (Optional) The subject line of the email that PayPal sends when the transaction completes. The subject line is the same for all recipients.
                request.EmailSubject = "Payment For Rollin News Content " + DateTime.UtcNow.ToString("yyyy/MM/dd");

                foreach (var invoice in invoices)
                {
                    var payout = invoice.WriterPayouts.FirstOrDefault();

                    //gotta check if we are already paying this user.

                    if (payout != null && tempFundsBeingPaid.Where(x => x.UserId == payout.UserPaidId).FirstOrDefault() != null)
                    {
                        var f = tempFundsBeingPaid.Where(x => x.UserId == payout.UserPaidId).FirstOrDefault();
                        if (f.ActiveInUserAccount >= (f.AmountToWithdraw + (double)invoice.BasePriceForItems))
                        {
                            f.AmountToWithdraw += (double)payout.PriceAfterFees;
                            f.AmountToDeductFromTotal += (double)invoice.BasePriceForItems;

                        }
                    }
                    else
                    {
                        var fundSettings = Fund.GetCurrentFundsInformation(payout.UserPaidId);
                        if ((double)invoice.BasePriceForItems <= fundSettings.ActiveInUserAccount)
                        {
                            fundSettings.AmountToWithdraw += (double)payout.PriceAfterFees;
                            fundSettings.AmountToDeductFromTotal += (double)invoice.BasePriceForItems;
                            tempFundsBeingPaid.Add(fundSettings);
                        }
                    }

                }
                for (int i = 0; i < tempFundsBeingPaid.Count; i++)
                {
                    // (Required) Details of each payment.
                    // Note:
                    // A single MassPayRequest can include up to 250 MassPayItems.
                    MassPayRequestItemType massPayItem = new MassPayRequestItemType();
                    CurrencyCodeType currency = CurrencyCodeType.USD;
                    massPayItem.Amount = new BasicAmountType(currency, tempFundsBeingPaid[i].AmountToWithdraw.ToString("N2"));
                    massPayItem.ReceiverEmail = tempFundsBeingPaid[i].PaypalAddress;
                    massPayItem.Note = "Thanks for writing for Rollin News!  We appreciate your content. ID:" + invoice.InvoiceId.ToString().Replace("-", "");
                    if (!String.IsNullOrEmpty(tempFundsBeingPaid[i].PaypalAddress))
                        request.MassPayItem.Add(massPayItem);

                }

                // Invoke the API
                MassPayReq wrapper = new MassPayReq();
                wrapper.MassPayRequest = request;


                // Create the PayPalAPIInterfaceServiceService service object to make the API call
                PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService();

                // # API call 
                // Invoke the MassPay method in service wrapper object  
                MassPayResponseType massPayResponse = service.MassPay(wrapper);

                // Display response values. 
                Dictionary<string, string> keyResponseParams = new Dictionary<string, string>();
                if (!(massPayResponse.Ack == AckCodeType.FAILURE) &&
                    !(massPayResponse.Ack == AckCodeType.FAILUREWITHWARNING))
                {


                    if (!(massPayResponse.Ack == AckCodeType.SUCCESS))
                    {

                        SendErrorsOfMassPayToAdmins(output, massPayResponse);
                        ErrorDatabaseManager.AddException(new Exception("MASSPAYERROR"), GetType(), additionalInformation: Newtonsoft.Json.JsonConvert.SerializeObject(tempFundsBeingPaid) + "______" + Newtonsoft.Json.JsonConvert.SerializeObject(massPayResponse));

                    }

                    for (int i = 0; i < tempFundsBeingPaid.Count; i++)
                    {
                        try
                        {
                            var user = SiteCache.GetPublicMemberFullWithUserId(tempFundsBeingPaid[i].UserId);
                            bool success = Fund.UpdateAmounts(tempFundsBeingPaid[i].UserId, tempFundsBeingPaid[i].TotalPaidToUser);

                            var emailData = new Dictionary<string, string> 
                            { 
                            { "derbyName", user.DerbyName},
                            {"amountPaid", tempFundsBeingPaid[i].AmountToDeductFromTotal.ToString("N2")}};
                            EmailServer.EmailServer.SendEmail(RollinNewsConfig.DEFAULT_EMAIL, RollinNewsConfig.DEFAULT_EMAIL_FROM_NAME, user.UserName, EmailServer.EmailServer.DEFAULT_SUBJECT_ROLLIN_NEWS + " You Were Just Paid!", emailData, EmailServer.EmailServerLayoutsEnum.RNPaymentJustPaid);
                        }
                        catch (Exception exception)
                        {
                            ErrorDatabaseManager.AddException(exception, exception.GetType());
                        }
                    }
                    var emailDataComplete = new Dictionary<string, string> { { "totalPaid", tempFundsBeingPaid.Sum(x=>x.AmountToDeductFromTotal).ToString("N2") },
                    {"totalUsersPaid", tempFundsBeingPaid.Count.ToString()}};
                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultAdminEmailAdmin, RollinNewsConfig.DEFAULT_EMAIL_FROM_NAME, LibraryConfig.DefaultAdminEmailAdmin, EmailServer.EmailServer.DEFAULT_SUBJECT_ROLLIN_NEWS + " Mass Pay Completed!", emailDataComplete, EmailServer.EmailServerLayoutsEnum.RNPaymentJustCompleted);

                    for (int i = 0; i < invoices.Count; i++)
                    {
                        Payment.PaymentGateway pg = new PaymentGateway();
                        pg.SetInvoiceStatus(invoices[i].InvoiceId, InvoiceStatus.Payment_Successful);
                    }
                }
                else
                {
                    SendErrorsOfMassPayToAdmins(output, massPayResponse);
                }


            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        private static void SendErrorsOfMassPayToAdmins(CreateInvoiceReturn output, MassPayResponseType massPayResponse)
        {
            if (massPayResponse.Errors != null && massPayResponse.Errors.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var error in massPayResponse.Errors)
                {
                    sb.Append("code:" + error.ErrorCode);
                    sb.Append(",long:" + error.LongMessage);
                    sb.Append(",severity:" + error.SeverityCode);
                    sb.Append(",shortMessage:" + error.ShortMessage);
                    foreach (var item in error.ErrorParameters)
                    {
                        sb.Append(",param:" + item.ParamID + ";" + item.Value);
                    }
                    sb.Append("________");
                }

                output.Status = InvoiceStatus.Paypal_Email_Not_Confirmed;

                var emailData = new Dictionary<string, string> { { "body", sb.ToString() } };
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmailAdmin, " Mass Pay Problem", emailData, EmailServer.EmailServerLayoutsEnum.Blank);
            }
            else
            {
                output.Status = InvoiceStatus.Failed;
                throw new Exception("Failure MASS Payment ");
            }
        }

        private void PerformPaypalDuesPaymentCheckout(CreateInvoiceReturn output)
        {
            try
            {
                var duesItem = invoice.ItemsDues.FirstOrDefault();

                if (duesItem != null)
                {
                    var memberPaying = MemberCache.GetMemberDisplay(duesItem.MemberPaidId);
                    var leagueSettings = Dues.DuesFactory.GetDuesSettings(duesItem.DuesId);
                    if (leagueSettings != null)
                    {
                        ReceiverList receiverList = new ReceiverList();
                        //RDNation as a reciever
                        Receiver recRDNation = new Receiver(duesItem.PriceAfterFees);
                        if (invoice.IsLive)
                            recRDNation.email = LibraryConfig.DefaultAdminEmailAdmin;
                        else
                            recRDNation.email = ServerConfig.PAYPAL_SELLER_DEBUG_ADDRESS;
                        recRDNation.primary = true;

                        //if we modify this invoiceID, 
                        //you need to modify this code here: 
                        recRDNation.invoiceId = invoice.InvoiceId.ToString().Replace("-", "") + ":" + LibraryConfig.ConnectionStringName + ": " + leagueSettings.LeagueOwnerName + " Dues Payment";
                        recRDNation.paymentType = PaymentTypeEnum.SERVICE.ToString();
                        receiverList.receiver.Add(recRDNation);

                        Receiver recLeague = new Receiver(duesItem.BasePrice);
                        recLeague.amount = duesItem.BasePrice;
                        if (invoice.IsLive)
                            recLeague.email = leagueSettings.PayPalEmailAddress;
                        else
                            recLeague.email = "cheeta_1359429163_per@gmail.com";

                        recLeague.primary = false;
                        //if we modify this invoiceID, 
                        //you need to modify this code here: 
                        recLeague.invoiceId = invoice.InvoiceId.ToString().Replace("-", "") + ":" + LibraryConfig.ConnectionStringName + ": " + leagueSettings.LeagueOwnerName + " Dues Payment";
                        recLeague.paymentType = PaymentTypeEnum.SERVICE.ToString();
                        receiverList.receiver.Add(recLeague);

                        PayRequest req = new PayRequest(new RequestEnvelope("en_US"), ActionTypeEnum.PAY.ToString(), ServerConfig.LEAGUE_DUES_MANAGEMENT_URL + leagueSettings.LeagueOwnerId.ToString().Replace("-", ""), invoice.Currency, receiverList, ServerConfig.LEAGUE_DUES_RECEIPT_URL + invoice.InvoiceId.ToString().Replace("-", ""));
                        req.feesPayer = FeesPayerEnum.PRIMARYRECEIVER.ToString();
                        req.memo = "Dues payment for " + leagueSettings.LeagueOwnerName + " from " + memberPaying.DerbyName + " for " + duesItem.PaidForDate.ToShortDateString();
                        req.reverseAllParallelPaymentsOnError = false;
                        req.trackingId = invoice.InvoiceId.ToString().Replace("-", "");
                        if (invoice.IsLive)
                            req.ipnNotificationUrl = LibraryConfig.PaypalIPNHandler;
                        else
                            req.ipnNotificationUrl = LibraryConfig.PaypalIPNHandlerDebug;

                        // All set. Fire the request            
                        AdaptivePaymentsService service = new AdaptivePaymentsService();
                        PayResponse resp = service.Pay(req);



                        // Display response values. 
                        Dictionary<string, string> keyResponseParams = new Dictionary<string, string>();
                        string redirectUrl = null;
                        if (!(resp.responseEnvelope.ack == AckCode.FAILURE) &&
                            !(resp.responseEnvelope.ack == AckCode.FAILUREWITHWARNING))
                        {
                            //EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmailAdmin, "Paypal Dues Payment Waiting To be Finished", invoice.InvoiceId + " Amount:" + duesItem.PriceAfterFees + ":" + leagueSettings.PayPalEmailAddress);

                            redirectUrl = PaypalPayment.GetBaseUrl(invoice.IsLive);

                            redirectUrl += "?cmd=_ap-payment&paykey=" + resp.payKey;
                            keyResponseParams.Add("Pay key", resp.payKey);
                            keyResponseParams.Add("Payment execution status", resp.paymentExecStatus);
                            if (resp.defaultFundingPlan != null && resp.defaultFundingPlan.senderFees != null)
                            {
                                keyResponseParams.Add("Sender fees", resp.defaultFundingPlan.senderFees.amount +
                                                            resp.defaultFundingPlan.senderFees.code);
                            }

                            //Selenium Test Case
                            keyResponseParams.Add("Acknowledgement", resp.responseEnvelope.ack.ToString());
                            output.RedirectLink = redirectUrl;
                            output.Status = InvoiceStatus.Pending_Payment_From_Paypal;
                        }
                        else
                        {
                            if (resp.error.FirstOrDefault().message.Contains(LibraryConfig.DefaultAdminEmailAdmin + " is restricted"))
                            {
                                output.Status = InvoiceStatus.Paypal_Email_Not_Confirmed;

                                var emailData = new Dictionary<string, string>
                                        {
                                            { "confirmPaypalAccountLink",ServerConfig.WIKI_URL_FOR_CONFIRMED_PAYPAL_ACCOUNT},
                                            { "paypalEmailAccount", leagueSettings.PayPalEmailAddress},
                                            { "duesSettingsLink", ServerConfig.LEAGUE_DUES_SETTINGS_URL +leagueSettings.LeagueOwnerId.ToString().Replace("-", "") + "/" + leagueSettings.DuesId.ToString().Replace("-", "")}
                                                                                    };

                                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultInfoEmail, EmailServer.EmailServer.DEFAULT_SUBJECT + " Paypal Email Is Restricted: " + resp.error.FirstOrDefault().message, emailData, EmailServer.EmailServerLayoutsEnum.PaypalEmailIsRestricted);

                            }
                            //paypal account hasn't been confirmed by the league.
                            else if (resp.error.FirstOrDefault().message.Contains("isn't confirmed by PayPal") || resp.error.FirstOrDefault().message.Contains("is restricted") || resp.error.FirstOrDefault().message.Contains("fields are specified to identify a receiver"))
                            {
                                //if the paypal account hasn't been confirmed, we send the league an email
                                //and disable their paypal account for dues.
                                output.Status = InvoiceStatus.Paypal_Email_Not_Confirmed;

                                var emailData = new Dictionary<string, string>
                                        {
                                            { "confirmPaypalAccountLink",ServerConfig.WIKI_URL_FOR_CONFIRMED_PAYPAL_ACCOUNT},
                                            { "paypalEmailAccount", leagueSettings.PayPalEmailAddress},
                                            { "duesSettingsLink", ServerConfig.LEAGUE_DUES_SETTINGS_URL +leagueSettings.LeagueOwnerId.ToString().Replace("-", "") + "/" + leagueSettings.DuesId.ToString().Replace("-", "")}
                                                                                    };
                                if (resp.error.FirstOrDefault().message.Contains("isn't confirmed by PayPal"))
                                {
                                    if (!String.IsNullOrEmpty(leagueSettings.PayPalEmailAddress))
                                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, leagueSettings.PayPalEmailAddress, EmailServer.EmailServer.DEFAULT_SUBJECT + " Paypal Email Isn't Confirmed", emailData, EmailServer.EmailServerLayoutsEnum.PayPalEmailIsNotConfirmed);
                                    if (!String.IsNullOrEmpty(leagueSettings.LeagueEmailAddress))
                                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, leagueSettings.LeagueEmailAddress, EmailServer.EmailServer.DEFAULT_SUBJECT + " Paypal Email Isn't Confirmed", emailData, EmailServer.EmailServerLayoutsEnum.PayPalEmailIsNotConfirmed);
                                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultInfoEmail, EmailServer.EmailServer.DEFAULT_SUBJECT + " Paypal Email Isn't Confirmed: " + resp.error.FirstOrDefault().message, emailData, EmailServer.EmailServerLayoutsEnum.PayPalEmailIsNotConfirmed);
                                }
                                if (resp.error.FirstOrDefault().message.Contains("is restricted"))
                                {
                                    if (!String.IsNullOrEmpty(leagueSettings.PayPalEmailAddress))
                                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, leagueSettings.PayPalEmailAddress, EmailServer.EmailServer.DEFAULT_SUBJECT + " Paypal Email Is Restricted", emailData, EmailServer.EmailServerLayoutsEnum.PaypalEmailIsRestricted);
                                    if (!String.IsNullOrEmpty(leagueSettings.LeagueEmailAddress))
                                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, leagueSettings.LeagueEmailAddress, EmailServer.EmailServer.DEFAULT_SUBJECT + " Paypal Email Is Restricted", emailData, EmailServer.EmailServerLayoutsEnum.PaypalEmailIsRestricted);
                                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultInfoEmail, EmailServer.EmailServer.DEFAULT_SUBJECT + " Paypal Email Is Restricted: " + resp.error.FirstOrDefault().message, emailData, EmailServer.EmailServerLayoutsEnum.PaypalEmailIsRestricted);
                                }
                                if (resp.error.FirstOrDefault().message.Contains("specified to identify a receiver"))
                                {
                                    if (!String.IsNullOrEmpty(leagueSettings.PayPalEmailAddress))
                                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, leagueSettings.PayPalEmailAddress, EmailServer.EmailServer.DEFAULT_SUBJECT + " Paypal Email Not Specified", emailData, EmailServer.EmailServerLayoutsEnum.PaypalEmailIsRestricted);
                                    if (!String.IsNullOrEmpty(leagueSettings.LeagueEmailAddress))
                                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, leagueSettings.LeagueEmailAddress, EmailServer.EmailServer.DEFAULT_SUBJECT + " Paypal Email Not Specified", emailData, EmailServer.EmailServerLayoutsEnum.PaypalEmailIsRestricted);
                                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultInfoEmail, EmailServer.EmailServer.DEFAULT_SUBJECT + " Paypal Email Not Specified: " + resp.error.FirstOrDefault().message, emailData, EmailServer.EmailServerLayoutsEnum.PaypalEmailIsRestricted);
                                }
                                Dues.DuesFactory.DisablePaypalDuesAccountForLeague(leagueSettings.DuesId);

                            }
                            else
                            {
                                output.Status = InvoiceStatus.Failed;
                                throw new Exception("Failure Payment " + leagueSettings.PayPalEmailAddress + ":" + resp.error.FirstOrDefault().message + ":" + Newtonsoft.Json.JsonConvert.SerializeObject(req));
                            }
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        /// <summary>
        /// perform any prepayout writers content
        /// </summary>
        /// <param name="output"></param>
        private void PerformPaypalPrePayoutWriterContent(CreateInvoiceReturn output)
        {
            try
            {
                var payout = invoice.RNWriterPayouts.FirstOrDefault();

                var fundInfo = Fund.GetCurrentFundsInformation(payout.UserPaidId);

                var emailData = new Dictionary<string, string>
                                        {
                                            { "emailForPayment",fundInfo.PaypalAddress},
                                            { "amountForPayment", payout.BasePrice.ToString()},
                                            { "amountForPaymentAfterFee", invoice.FinancialData.BasePriceForItems.ToString()},
                                        };

                EmailServer.EmailServer.SendEmail(RollinNewsConfig.DEFAULT_EMAIL, RollinNewsConfig.DEFAULT_EMAIL_FROM_NAME, LibraryConfig.DefaultAdminEmailAdmin, EmailServer.EmailServer.DEFAULT_SUBJECT_ROLLIN_NEWS + " Payment Requested", emailData, EmailServer.EmailServerLayoutsEnum.RNPaymentRequested);
                EmailServer.EmailServer.SendEmail(RollinNewsConfig.DEFAULT_EMAIL, RollinNewsConfig.DEFAULT_EMAIL_FROM_NAME, RollinNewsConfig.DEFAULT_MRX_EMAIL_ADMIN, EmailServer.EmailServer.DEFAULT_SUBJECT_ROLLIN_NEWS + " Payment Requested", emailData, EmailServer.EmailServerLayoutsEnum.RNPaymentRequested);

                invoice.InvoiceStatus = InvoiceStatus.Payment_Awaiting_For_Mass_Payout;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        #endregion



        private class GoogleCheckoutReturn
        {
            public string Error { get; set; }
            public string RedirectLink { get; set; }
        }
    }
}
