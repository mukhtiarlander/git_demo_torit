using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment.Classes.Display;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.PaymentGateway.Merchants;

namespace RDN.Library.Classes.Payment
{
    public class MerchantGateway
    {
        private static OverviewMerchant GetDisplayMerchant(Merchant merchant)
        {
            try
            {
                var merch = new OverviewMerchant();
                merch.ShopName = merchant.ShopName;
                merch.MerchantId = merchant.MerchantId;
                merch.InternalReference = merchant.InternalReference;
                merch.PrivateManagerId = merchant.PrivateManagerId;
                merch.AmountOnAccount = merchant.AmountOnAccount;
                
                if (merchant.CurrencyRate == null)
                {
                    merch.Currency = "USD";
                    merch.CurrencyCost = 1;
                }
                else
                {
                    merch.Currency = merchant.CurrencyRate.CurrencyAbbrName;
                    merch.CurrencyCost = merchant.CurrencyRate.CurrencyExchangePerUSD;
                }
                merch.OrderPayedNotificationEmail = merchant.OrderPayedNotificationEmail;
                merch.PayedFeesToRDN = merchant.PayedFeesToRDN;
                merch.IsPublished = merchant.IsPublished;
                merch.AcceptPaymentsViaPaypal = merchant.AcceptPaymentsViaPaypal;
                merch.PaypalEmail = merchant.PaypalEmail;
                merch.AcceptPaymentsViaStripe = merchant.AcceptPaymentsViaStripe;
                merch.StripeConnectKey = merchant.StripeConnectKey;
                merch.StripeConnectToken = merchant.StripeConnectToken;
                merch.WelcomeMessage = merchant.WelcomeMessage;
                return merch;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public bool UpdateMerchantSettings(OverviewMerchant store)
        {
            try
            {
                var dc = new ManagementContext();
                var merc = dc.Merchants.Where(x => x.InternalReference == store.InternalReference && x.MerchantId == store.MerchantId && x.PrivateManagerId == store.PrivateManagerId).FirstOrDefault();

                if (merc != null)
                {
                    merc.OrderPayedNotificationEmail = store.OrderPayedNotificationEmail;
                    merc.PaypalEmail = store.PaypalEmail;
                    //need to make sure we have a paypal email.
                    if (String.IsNullOrEmpty(merc.PaypalEmail))
                        merc.AcceptPaymentsViaPaypal = false;
                    else
                        merc.AcceptPaymentsViaPaypal = store.AcceptPaymentsViaPaypal;
                    if (merc.AcceptPaymentsViaStripe || (store.AcceptPaymentsViaPaypal && !String.IsNullOrEmpty(store.PaypalEmail)))
                        merc.IsPublished = store.IsPublished;
                    else
                        merc.IsPublished = false;

                    dc.SaveChanges();
                    return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public OverviewMerchant GetMerchantSettings(Guid internalManagerId)
        {
            try
            {
                var mc = new ManagementContext();
                Merchant merchant = null;
                merchant = mc.Merchants.Include("Items").Include("Invoices").Where(x => x.InternalReference == internalManagerId).FirstOrDefault();

                if (merchant == null)
                    return null;
                return GetDisplayMerchant(merchant);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new OverviewMerchant();
        }
        public OverviewMerchant GetMerchant(Guid privateId)
        {
            try
            {
                var mc = new ManagementContext();
                Merchant merchant = null;
                merchant = mc.Merchants.Include("Items").Include("Invoices").Where(x => x.PrivateManagerId == privateId).FirstOrDefault();

                if (merchant == null)
                    return null;
                return GetDisplayMerchant(merchant);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new OverviewMerchant();
        }
        public List<OverviewMerchant> GetPublicMerchants()
        {
            List<OverviewMerchant> mercs = new List<OverviewMerchant>();
            try
            {
                var mc = new ManagementContext();
                var merchants = mc.Merchants.Include("Items").Include("Invoices").Where(x => x.IsPublished == true);

                foreach (var merc in merchants)
                {
                    mercs.Add(GetDisplayMerchant(merc));
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return mercs;
        }
        public OverviewMerchant CreateMerchantAccount(Guid internalId, MerchantInternalReference internalReference)
        {
            OverviewMerchant merchant = new OverviewMerchant();
            try
            {
                var dc = new ManagementContext();
                string notificationEmail = String.Empty;
                string ownerName = String.Empty;

                if (internalReference == MerchantInternalReference.Member)
                {
                    var mem = dc.Members.Where(x => x.MemberId == internalId).FirstOrDefault();
                    if (mem != null && mem.ContactCard != null && mem.ContactCard.Emails.FirstOrDefault() != null)
                        notificationEmail = mem.ContactCard.Emails.FirstOrDefault().EmailAddress;
                    ownerName = mem.DerbyName;
                }

                Merchant merc = MerchantGateway.CreateMerchantAccount(internalId, internalReference, notificationEmail, ownerName);
                merchant.InternalReference = internalId;
                merchant.MerchantId = merc.MerchantId;
                merchant.PrivateManagerId = merc.PrivateManagerId;
                merchant.ShopName = merc.ShopName;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return merchant;
        }


        public static Merchant CreateMerchantAccount(Guid internalId, MerchantInternalReference internalReference, string notificationEmail, string ownerName)
        {

            try
            {
                ManagementContext dc = new ManagementContext();
                Merchant merc = new Merchant();
                merc.AmountOnAccount = Convert.ToDecimal(0.00);
                merc.AutoAcceptPayment = true;
                merc.AutoShipWhenPaymentIsReceived = false;
                merc.Bank = "None";
                merc.BankAccount = String.Empty;
                merc.CurrencyRate = dc.ExchangeRates.Where(x => x.CurrencyAbbrName == "USD").FirstOrDefault();
                merc.InternalReference = internalId;
                merc.InternalReferenceType = Convert.ToByte(internalReference);
                merc.IsRDNation = false;
                merc.MerchantAccountStatus = Convert.ToByte(RDN.Library.Classes.Payment.Enums.MerchantAccountStatus.NotActive);
                merc.OrderPayedNotificationEmail = notificationEmail;
                if (String.IsNullOrEmpty(ownerName))
                    merc.OwnerName = "Shop";
                else
                    merc.OwnerName = ownerName;
                merc.PayedFeesToRDN = Convert.ToDecimal(0.00);
                merc.PrivateManagerId = Guid.NewGuid();
                merc.RDNFixedFee = Convert.ToDecimal(0.00);
                merc.RDNPercentageFee = 0.00;
                merc.ShippingNotificationEmail = notificationEmail;
                merc.ShopName = ownerName;
                merc.TaxRate = Convert.ToDouble(0.00);
                merc.MerchantId = Guid.NewGuid();
                merc.IsPublished = false;
                dc.Merchants.Add(merc);
                int c = dc.SaveChanges();
                if (c > 0)
                    return merc;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

    }
}
