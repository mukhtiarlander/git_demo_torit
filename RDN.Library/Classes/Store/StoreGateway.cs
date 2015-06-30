using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Context;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Store.Classes;
using RDN.Library.Classes.Store.Display;
using RDN.Library.DataModels.PaymentGateway.Merchants;
using System.IO;
using System.Drawing;
using Stripe;
using System.Configuration;
using RDN.Library.Classes.Payment.Classes.Display;
using RDN.Library.Classes.Payment;
using RDN.Utilities.Config;
using RDN.Library.Classes.Payment.Classes.Invoice;
using System.Web;
using RDN.Library.Cache;
using RDN.Utilities.Strings;
using RDN.Portable.Config;
using RDN.Portable.Classes.Store.Enums;
using RDN.Portable.Classes.Payment.Enums;
using RDN.Portable.Classes.Imaging;
using RDN.Portable.Classes.Colors;
using RDN.Library.Classes.Config;
using RDN.Portable.Classes.Url;



namespace RDN.Library.Classes.Store
{
    public class StoreGateway
    {
        public static readonly decimal RDNATION_FEE_FOR_LISTING_ITEM = .30M;

        // ToDo: GetStoreCategories
        // ToDo: GetCategoryItems        
        // ToDo: Checkout


        public static Guid? GetShoppingCartId(HttpContextBase context)
        {
            try
            {
                Guid? shoppingCartId = null;
                if (context.Session["ShoppingCartId"] != null)
                {
                    Guid cartId;
                    if (Guid.TryParse(context.Session["ShoppingCartId"].ToString(), out cartId))
                        shoppingCartId = cartId;
                }
                if (shoppingCartId == null && context.User.Identity.IsAuthenticated)
                {
                    StoreGateway sg = new StoreGateway();
                    shoppingCartId = sg.GetShoppingCartId(RDN.Library.Classes.Account.User.GetUserId());
                    context.Session["ShoppingCartId"] = shoppingCartId;
                }
                return shoppingCartId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public bool DeleteItemFromCart(int storeItemCartId, Guid cartId)
        {
            try
            {
                var dc = new ManagementContext();
                var cart = dc.ShoppingCarts.Include("Items").Include("Items.StoreItem").Where(x => x.ShoppingCartId == cartId).FirstOrDefault();
                if (cart != null)
                {
                    var item = cart.Items.Where(x => x.ShoppingCartItemId == storeItemCartId).FirstOrDefault();
                    if (item != null)
                    {
                        cart.Items.Remove(item);
                        dc.SaveChanges();
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public bool ToggleShipment(int storeItemCartId, Guid cartId)
        {
            try
            {
                var dc = new ManagementContext();
                var cart = dc.ShoppingCarts.Include("Items").Include("Items.StoreItem").Where(x => x.ShoppingCartId == cartId).FirstOrDefault();
                if (cart != null)
                {
                    var item = cart.Items.Where(x => x.ShoppingCartItemId == storeItemCartId).FirstOrDefault();
                    if (item != null)
                    {
                        item.WillPickUpLocally = !item.WillPickUpLocally;
                        int c = dc.SaveChanges();
                        return c > 0;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }


        public bool UpdateStoreQuantityItem(int storeItemCartNumber, Guid cartId, int quantity)
        {
            try
            {
                var dc = new ManagementContext();
                var cart = dc.ShoppingCarts.Include("Items").Include("Items.StoreItem").Where(x => x.ShoppingCartId == cartId).FirstOrDefault();
                if (cart != null)
                {
                    var item = cart.Items.Where(x => x.ShoppingCartItemId == storeItemCartNumber).FirstOrDefault();
                    if (item != null)
                    {
                        item.Quantity = quantity;
                        dc.SaveChanges();
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public bool DeleteStoreItemPhoto(int photoId, int storeItemNumber, Guid privateManagerId)
        {
            try
            {
                var dc = new ManagementContext();
                var memDb = dc.StoreItemPhotos.Where(x => x.ItemPhotoId == photoId && x.StoreItem.StoreItemId == storeItemNumber && x.StoreItem.Merchant.PrivateManagerId == privateManagerId).FirstOrDefault();

                dc.StoreItemPhotos.Remove(memDb);
                dc.SaveChanges();

                FileInfo info = new FileInfo(memDb.SaveLocation);
                if (info.Exists)
                    info.Delete();

                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }


        public string AddStoreItemPhoto(long storeItemNumber, Stream fileStream, string nameOfFile)
        {
            string log = string.Empty;
            try
            {

                var dc = new ManagementContext();
                var memDb = dc.StoreItems.Where(x => x.StoreItemId == storeItemNumber).FirstOrDefault();
                //time stamp for the save location
                DateTime timeOfSave = DateTime.UtcNow;
                long saveTime = timeOfSave.ToFileTimeUtc();
                FileInfo info = new FileInfo(nameOfFile);

                //the file name when we save it
                string fileName = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(memDb.Name + " buy roller derby item-") + saveTime + info.Extension;
                string fileNameThumb = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(memDb.Name + " buy roller derby item-") + saveTime + "_thumb" + info.Extension;

                string url = "http://images.rdnation.com/store/" + timeOfSave.Year + "/" + timeOfSave.Month + "/" + timeOfSave.Day + "/";
                string imageLocationToSave = @"C:\WebSites\images.rdnation.com\store\" + timeOfSave.Year + @"\" + timeOfSave.Month + @"\" + timeOfSave.Day + @"\";
                //creates the directory for the image
                if (!Directory.Exists(imageLocationToSave))
                    Directory.CreateDirectory(imageLocationToSave);



                RDN.Library.DataModels.Store.StoreItemPhoto image = new DataModels.Store.StoreItemPhoto();
                image.ImageUrl = url + fileName;
                image.SaveLocation = imageLocationToSave + fileName;
                image.ImageUrlThumb = url + fileNameThumb;
                image.SaveLocationThumb = imageLocationToSave + fileNameThumb;

                image.IsPrimaryPhoto = true;
                image.IsVisibleToPublic = true;
                image.StoreItem = memDb;
                memDb.Photos.Add(image);

                log += image.SaveLocation;

                using (var newfileStream = new FileStream(image.SaveLocation, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fileStream.CopyTo(newfileStream);
                }

                Image b = Image.FromFile(image.SaveLocation);
                Image thumb = b.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
                thumb.Save(image.SaveLocationThumb);
                //saves the photo to the DB.
                int c = dc.SaveChanges();

                return url;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: log + ":" + nameOfFile + ":" + storeItemNumber);
            }
            return string.Empty;
        }

        public bool UpdateStoreSettings(DisplayStore store)
        {
            try
            {
                var dc = new ManagementContext();
                var merc = dc.Merchants.Where(x => x.InternalReference == store.InternalReference && x.MerchantId == store.MerchantId && x.PrivateManagerId == store.PrivateManagerId).FirstOrDefault();

                if (merc != null)
                {
                    if (!String.IsNullOrEmpty(merc.OwnerName))
                        merc.OwnerName = merc.OwnerName;
                    else
                        merc.OwnerName = "Shop";
                    merc.InternalReference = merc.InternalReference;
                    merc.PrivateManagerId = merc.PrivateManagerId;
                    merc.MerchantAccountStatus = merc.MerchantAccountStatus;
                    merc.RDNFixedFee = merc.RDNFixedFee;
                    merc.RDNPercentageFee = merc.RDNPercentageFee;
                    merc.AmountOnAccount = merc.AmountOnAccount;
                    merc.PayedFeesToRDN = merc.PayedFeesToRDN;

                    merc.AutoShipWhenPaymentIsReceived = store.AutoShipWhenPaymentIsReceived;
                    merc.OrderPayedNotificationEmail = store.OrderPayedNotificationEmail;
                    merc.ShopName = store.ShopName;
                    merc.TaxRate = store.TaxRate;
                    merc.CurrencyRate = dc.ExchangeRates.Where(x => x.CurrencyAbbrName == store.Currency).FirstOrDefault();
                    merc.ShippingNotificationEmail = store.ShippingNotificationEmail;
                    merc.PaypalEmail = store.PayPalEmailAddressForPayments;
                    //need to make sure we have a paypal email.
                    if (String.IsNullOrEmpty(store.PayPalEmailAddressForPayments))
                        merc.AcceptPaymentsViaPaypal = false;
                    else
                        merc.AcceptPaymentsViaPaypal = store.AcceptPaymentsViaPaypal;
                    if (merc.AcceptPaymentsViaStripe || (store.AcceptPaymentsViaPaypal && !String.IsNullOrEmpty(store.PayPalEmailAddressForPayments)))
                        merc.IsPublished = store.IsPublished;
                    else
                        merc.IsPublished = false;

                    merc.Description = store.Description;
                    merc.WelcomeMessage = store.WelcomeMessage;
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
        public DisplayStore GetStoreSettings(Guid merchantId, Guid privateId)
        {
            DisplayStore merc = new DisplayStore();
            try
            {
                var dc = new ManagementContext();
                var store = dc.Merchants.Where(x => x.MerchantId == merchantId && x.PrivateManagerId == privateId).FirstOrDefault();

                if (store != null)
                {
                    merc.AutoShipWhenPaymentIsReceived = store.AutoShipWhenPaymentIsReceived;
                    merc.OrderPayedNotificationEmail = store.OrderPayedNotificationEmail;
                    merc.ShopName = store.ShopName;
                    merc.TaxRate = store.TaxRate;
                    if (store.CurrencyRate != null)
                        merc.Currency = store.CurrencyRate.CurrencyAbbrName;
                    else
                        merc.Currency = "USD";
                    //merc.CurrencyRate = dc.ExchangeRates.Where(x => x.CurrencyAbbrName == store.Currency).FirstOrDefault();
                    merc.ShippingNotificationEmail = store.ShippingNotificationEmail;
                    merc.PayPalEmailAddressForPayments = store.PaypalEmail;
                    //need to make sure we have a paypal email.
                    if (String.IsNullOrEmpty(store.PaypalEmail))
                        merc.AcceptPaymentsViaPaypal = false;
                    else
                        merc.AcceptPaymentsViaPaypal = store.AcceptPaymentsViaPaypal;
                    if (merc.AcceptPaymentsViaStripe || (store.AcceptPaymentsViaPaypal && !String.IsNullOrEmpty(store.PaypalEmail)))
                        merc.IsPublished = store.IsPublished;
                    else
                        merc.IsPublished = false;

                    merc.Description = store.Description;
                    merc.WelcomeMessage = store.WelcomeMessage;
                    merc.InternalReference = store.InternalReference;
                    merc.MerchantId = merchantId;
                    merc.PrivateManagerId = privateId;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return merc;
        }
        public bool UpdateStripeKey(string stripeKey, Guid privateId)
        {
            try
            {
                var stripeService = new StripeOAuthTokenService(RDN.Library.Classes.Config.LibraryConfig.StripeApiKey);
                var stripeTokenOptions = new StripeOAuthTokenCreateOptions() { Code = stripeKey, GrantType = "authorization_code" };
                var response = stripeService.Create(stripeTokenOptions);

                var dc = new ManagementContext();
                var merc = dc.Merchants.Where(x => x.PrivateManagerId == privateId).FirstOrDefault();

                if (merc != null && !String.IsNullOrEmpty(response.AccessToken))
                {
                    merc.StripeConnectToken = response.AccessToken;
                    merc.StripePublishableKey = response.StripePublishableKey;
                    merc.StripeRefreshToken = response.RefreshToken;
                    merc.StripeUserId = response.StripeUserId;
                    merc.StripeTokenType = response.TokenType;
                    merc.StripeConnectKey = stripeKey;
                    merc.AcceptPaymentsViaStripe = true;
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

        /// <summary>
        /// email payer receipt
        ///email seller receipt and orders information
        /// </summary>
        /// <param name="invoice"></param>
        private void CompileAndSendReceiptsEmailsForStore(DisplayInvoice invoice, string reportingInformation = null)
        {
            try
            {
                StringBuilder itemsSold = new StringBuilder();
                itemsSold.Append("<ul>");
                foreach (var item in invoice.InvoiceItems)
                {
                    itemsSold.Append("<li>");
                    //http://localhost:8847/roller-derby-item/Name-of-ITem-PicturesName-of-ITem-Pictures/12
                    itemsSold.Append("<a href='" +  LibraryConfig.ShopSite+ "/roller-derby-item/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(item.Name) + "/" + item.StoreItemId + "'>" + item.Name + "</a>");
                    itemsSold.Append(" - " + item.Description);
                    itemsSold.Append("</li>");
                }
                itemsSold.Append("</ul>");
                StringBuilder shippingAddress = new StringBuilder();
                StringBuilder sellersAddress = new StringBuilder();

                if (invoice.ShippingType == ShippingType.PickUp)
                {
                    shippingAddress.Append("<b>Will Pick Up Locally, Please contact buyer to know when.</b>");


                    sellersAddress.Append(invoice.SellersAddress.CompanyName + "<br/>");
                    if (!String.IsNullOrEmpty(invoice.SellersAddress.Street))
                        sellersAddress.Append(invoice.SellersAddress.Street + "<br/>");
                    if (!String.IsNullOrEmpty(invoice.SellersAddress.Street2))
                        sellersAddress.Append(invoice.SellersAddress.Street2 + "<br/>");
                    sellersAddress.Append(invoice.SellersAddress.State + " " + invoice.SellersAddress.Zip + "<br/>");
                    if (!String.IsNullOrEmpty(invoice.SellersAddress.Country))
                    {
                        var countries = Location.LocationFactory.GetCountries();
                        var count = countries.Where(x => x.CountryId == Convert.ToInt32(invoice.SellersAddress.Country)).FirstOrDefault();
                        sellersAddress.Append(count.Name + "<br/>");
                    }
                }
                else
                {
                    shippingAddress.Append("You chose to have item shipped.");
                    InvoiceContactInfo shipping = null;
                    if (invoice.InvoiceShipping == null)
                        shipping = invoice.InvoiceBilling;
                    else
                        shipping = invoice.InvoiceShipping;

                    shippingAddress.Append(shipping.FirstName + " " + shipping.LastName + "<br/>");
                    if (!String.IsNullOrEmpty(shipping.Street))
                        shippingAddress.Append(shipping.Street + "<br/>");
                    if (!String.IsNullOrEmpty(shipping.Street2))
                        shippingAddress.Append(shipping.Street2 + "<br/>");
                    shippingAddress.Append(shipping.State + " " + shipping.Zip + "<br/>");
                    if (!String.IsNullOrEmpty(shipping.Country))
                    {
                        var countries = Location.LocationFactory.GetCountries();
                        var count = countries.Where(x => x.CountryId == Convert.ToInt32(shipping.Country)).FirstOrDefault();
                        shippingAddress.Append(count.Name + "<br/>");
                    }
                }

                var emailData = new Dictionary<string, string>
                                        {
                                            { "memberName",  invoice.InvoiceBilling.FirstName +" "+ invoice.InvoiceBilling.LastName},
                                            { "shopName", "<a href='"+LibraryConfig.ShopSite+"/roller-derby-shop/"+invoice.Merchant.MerchantId.ToString().Replace("-","")+"/"+ RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(invoice.Merchant.ShopName) +"'>"+ invoice.Merchant.ShopName+"</a>"},
                                            { "invoiceId", invoice.InvoiceId.ToString().Replace("-","")},
                                            { "amountPaid", "$"+ (invoice.TotalIncludingTax + invoice.ShippingCost).ToString("N2") },
                                            { "receiptLink", "<a href='"+LibraryConfig.ShopSite+"/receipt/"+invoice.InvoiceId.ToString().Replace("-","")+"'>Your Receipt and Order Status</a>"},
                                            { "merchantLink", "<a href='"+LibraryConfig.InternalSite+"/store/orders/"+invoice.Merchant.PrivateManagerId.ToString().Replace("-","")+"/"+invoice.Merchant.MerchantId.ToString().Replace("-","")+"'>View the Order</a>"},
                                            { "shippingAddress",shippingAddress.ToString()},
                                            { "sellersAddress",sellersAddress.ToString()},
                                            { "itemsSold", itemsSold.ToString()},
                                            { "notesForPayment", invoice.Note},
                                            { "emailLink", "<a href='mailto:"+LibraryConfig.DefaultInfoEmail+"'>"+ LibraryConfig.DefaultInfoEmail+"</a>"}                                            
                                          };

                //sends email to user for their payment.
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, invoice.InvoiceBilling.Email, EmailServer.EmailServer.DEFAULT_SUBJECT + " Receipt for " + invoice.Merchant.ShopName, emailData, EmailServer.EmailServerLayoutsEnum.StoreSendReceiptForOrder);

                //sends email to the seller
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, invoice.Merchant.OrderPayedNotificationEmail, EmailServer.EmailServer.DEFAULT_SUBJECT + " New Order Bought From Shop: " + invoice.InvoiceId.ToString().Replace("-", ""), emailData, EmailServer.EmailServerLayoutsEnum.StoreSendSellerAboutOrdersBought);

                //sends email to the seller
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, EmailServer.EmailServer.DEFAULT_SUBJECT + " New Order Bought From Shop: " + invoice.InvoiceId.ToString().Replace("-", ""), emailData, EmailServer.EmailServerLayoutsEnum.StoreSendSellerAboutOrdersBought);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: reportingInformation);
            }
        }
        public void CompileAndSendShippedEmailsForStore(DisplayInvoice invoice, string reportingInformation = null)
        {
            try
            {

                StringBuilder itemsSold = new StringBuilder();
                itemsSold.Append("<ul>");
                foreach (var item in invoice.InvoiceItems)
                {
                    itemsSold.Append("<li>");
                    //http://localhost:8847/roller-derby-item/Name-of-ITem-PicturesName-of-ITem-Pictures/12
                    itemsSold.Append("<a href='" + LibraryConfig.ShopSite + "/roller-derby-item/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(item.Name) + "/" + item.StoreItemId + "'>" + item.Name + "</a>");
                    itemsSold.Append(" - " + item.Description);
                    itemsSold.Append("</li>");
                }
                itemsSold.Append("</ul>");
                StringBuilder shippingAddress = new StringBuilder();
                InvoiceContactInfo shipping = null;
                if (invoice.InvoiceShipping == null)
                    shipping = invoice.InvoiceBilling;
                else
                    shipping = invoice.InvoiceShipping;

                shippingAddress.Append(shipping.FirstName + " " + shipping.LastName + "<br/>");
                if (!String.IsNullOrEmpty(shipping.Street))
                    shippingAddress.Append(shipping.Street + "<br/>");
                if (!String.IsNullOrEmpty(shipping.Street2))
                    shippingAddress.Append(shipping.Street2 + "<br/>");
                shippingAddress.Append(shipping.State + " " + shipping.Zip + "<br/>");
                if (!String.IsNullOrEmpty(shipping.Country))
                {
                    var countries = Location.LocationFactory.GetCountries();
                    var count = countries.Where(x => x.CountryId == Convert.ToInt32(shipping.Country)).FirstOrDefault();
                    shippingAddress.Append(count.Name + "<br/>");
                }

                var emailData = new Dictionary<string, string>
                                        {
                                            { "memberName",  invoice.InvoiceBilling.FirstName +" "+ invoice.InvoiceBilling.LastName},
                                            { "shopName", "<a href='"+LibraryConfig.ShopSite+"/roller-derby-shop/"+invoice.Merchant.MerchantId.ToString().Replace("-","")+"/"+ RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(invoice.Merchant.ShopName) +"'>"+ invoice.Merchant.ShopName+"</a>"},
                                            { "invoiceId", invoice.InvoiceId.ToString().Replace("-","")},
                                            { "amountPaid", "$"+ (invoice.TotalIncludingTax + invoice.ShippingCost).ToString("N2") },
                                            { "receiptLink", "<a href='"+LibraryConfig.ShopSite+"/receipt/"+invoice.InvoiceId.ToString().Replace("-","")+"'>Your Receipt and Order Status</a>"},
                                            { "merchantLink", "<a href='"+LibraryConfig.InternalSite+"/store/orders/"+invoice.Merchant.PrivateManagerId.ToString().Replace("-","")+"/"+invoice.Merchant.MerchantId.ToString().Replace("-","")+"'>View the Order</a>"},
                                            { "shippingAddress",shippingAddress.ToString()},
                                            { "itemsSold", itemsSold.ToString()},
                                            { "notesForPayment", invoice.Note},
                                            { "emailLink", "<a href='mailto:"+LibraryConfig.DefaultInfoEmail+"'>"+ LibraryConfig.DefaultInfoEmail+"</a>"}                                            
                                          };

                //sends email to user for their payment.
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, invoice.InvoiceBilling.Email, EmailServer.EmailServer.DEFAULT_SUBJECT + " Your Items Have Shipped", emailData, EmailServer.EmailServerLayoutsEnum.StoreSendShippedItemsForOrder);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: reportingInformation);
            }
        }

        public void HandleStoreItemPaymentPending(DisplayInvoice invoice, string additionalReportingInformation = null)
        {
            try
            {
                var items = invoice.InvoiceItems;
                PaymentGateway pg = new PaymentGateway();
                //change invoice to ready to be shipped
                pg.SetInvoiceStatus(invoice.InvoiceId, InvoiceStatus.Awaiting_Payment);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: additionalReportingInformation);
            }
        }


        public void HandleStoreItemPayments(DisplayInvoice invoice, string additionalReportingInformation = null, string customerId = null)
        {
            try
            {
                var items = invoice.InvoiceItems;

                PaymentGateway pg = new PaymentGateway();
                //change invoice to ready to be shipped
                pg.SetInvoiceStatus(invoice.InvoiceId, InvoiceStatus.Awaiting_Shipping, customerId);

                CompileAndSendReceiptsEmailsForStore(invoice);

                //clear item from shooping cart.
                foreach (var item in items)
                {
                    bool isRemoved = RemoveItemFromCart(invoice.ShoppingCartId, item.StoreItemId);
                    bool isSuccess = SubtractNumberOfItemsBeingSoldByMerchant(invoice.Merchant.MerchantId, item.StoreItemId, item.Quantity);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: additionalReportingInformation);
            }
        }

        public StoreItemDisplay UpdateStoreItem(StoreItemDisplay storeItem)
        {
            try
            {
                var dc = new ManagementContext();
                var si = dc.StoreItems.Include("Colors").Include("Colors.Color").Where(x => x.StoreItemId == storeItem.StoreItemId && x.Merchant.InternalReference == storeItem.InternalId && x.Merchant.MerchantId == storeItem.MerchantId && x.Merchant.PrivateManagerId == storeItem.PrivateManagerId).FirstOrDefault();

                if (si != null)
                {
                    si.CanPickUpLocally = storeItem.CanPickUpLocally;
                    si.Merchant = si.Merchant;
                    si.ArticleNumber = storeItem.ArticleNumber;
                    si.CanRunOutOfStock = storeItem.CanRunOutOfStock;
                    si.Description = storeItem.Description;
                    si.ExemptFromShipping = false;
                    si.Name = storeItem.Name;
                    si.Note = storeItem.Note;
                    si.Price = storeItem.Price;
                    si.QuantityInStock = storeItem.QuantityInStock;
                    si.VisibleOnRdn = false;
                    si.Weight = storeItem.Weight;
                    si.ShippingCosts = storeItem.Shipping;
                    si.ShippingCostsAdditional = storeItem.ShippingAdditional;
                    si.Category = dc.StoreItemCategories.Where(x => x.StoreItemCategoryId == storeItem.ItemTypeEnum).FirstOrDefault();
                    //they are just now publishing the item.
                    if (!si.IsPublished && storeItem.IsPublished)
                    {
                        AddRDNationFeeToMerchant(si);
                        si.LastPublished = DateTime.UtcNow;
                    }
                    si.IsPublished = storeItem.IsPublished;
                    si.ItemTypeEnum = storeItem.ItemTypeEnum;
                    si.SizesEnum = 0;
                    if (storeItem.HasExtraLarge)
                        si.SizesEnum += Convert.ToInt32(StoreItemShirtSizesEnum.X_Large);
                    if (storeItem.HasExtraSmall)
                        si.SizesEnum += Convert.ToInt32(StoreItemShirtSizesEnum.X_Small);
                    if (storeItem.HasLarge)
                        si.SizesEnum += Convert.ToInt32(StoreItemShirtSizesEnum.Large);
                    if (storeItem.HasMedium)
                        si.SizesEnum += Convert.ToInt32(StoreItemShirtSizesEnum.Medium);
                    if (storeItem.HasSmall)
                        si.SizesEnum += Convert.ToInt32(StoreItemShirtSizesEnum.Small);
                    if (storeItem.HasXXLarge)
                        si.SizesEnum += Convert.ToInt32(StoreItemShirtSizesEnum.XX_Large);

                    //removes any colors not being used anymore.
                    List<int> colors = new List<int>();
                    if (!String.IsNullOrEmpty(storeItem.ColorTempSelected))
                    {
                        foreach (string color in storeItem.ColorTempSelected.Split(';'))
                        {
                            if (color.Length > 3)
                            {
                                Color c = ColorTranslator.FromHtml(color);
                                int arb = c.ToArgb();
                                colors.Add(arb);
                            }
                        }
                    }
                    var colorsNoLongerIn = si.Colors.Where(x => !colors.Contains(x.Color.ColorIdCSharp)).ToList();
                    foreach (var removeColor in colorsNoLongerIn)
                    {
                        si.Colors.Remove(removeColor);
                    }
                    //adds colors that are not currently added to the storeitem.
                    if (!String.IsNullOrEmpty(storeItem.ColorTempSelected))
                    {
                        foreach (string color in storeItem.ColorTempSelected.Split(';'))
                        {
                            if (color.Length > 3)
                            {
                                Color c = ColorTranslator.FromHtml(color);
                                int arb = c.ToArgb();
                                if (si.Colors.Where(x => x.Color.ColorIdCSharp == arb).FirstOrDefault() == null)
                                {
                                    var colorDb = dc.Colors.Where(x => x.ColorIdCSharp == arb).FirstOrDefault();
                                    if (colorDb != null)
                                    {
                                        DataModels.Store.StoreItemColor cItem = new DataModels.Store.StoreItemColor();
                                        cItem.Color = colorDb;
                                        cItem.StoreItem = si;
                                        si.Colors.Add(cItem);
                                    }
                                }
                            }
                        }
                    }



                    int ch = dc.SaveChanges();

                    storeItem.StoreItemId = si.StoreItemId;
                }
            }
            catch (System.Exception e)
            {
                var ex = e;
                ErrorDatabaseManager.AddException(ex, ex.GetType());
            }
            return storeItem;
        }
        public bool UpdateStoreItemViewCount(long storeItemId)
        {
            try
            {
                var dc = new ManagementContext();
                var si = dc.StoreItems.Where(x => x.StoreItemId == storeItemId).FirstOrDefault();

                if (si != null)
                {
                    si.Merchant = si.Merchant;
                    si.Views += 1;
                    int c = dc.SaveChanges();
                    return c > 0;
                }
            }
            catch (System.Exception e)
            {
                var ex = e;
                ErrorDatabaseManager.AddException(ex, ex.GetType());
            }
            return false;
        }

        public static bool AddRDNationFeeToMerchant(DataModels.Store.StoreItem si)
        {
            try
            {
                var dc = new ManagementContext();
                MerchantRDNationFee fee = new MerchantRDNationFee();
                fee.FeeDescription = "Published " + si.Name + " on " + DateTime.UtcNow.ToShortDateString();
                var merc = dc.Merchants.Where(x => x.MerchantId == si.Merchant.MerchantId).FirstOrDefault();
                fee.Merchant = merc;
                fee.RDNFee = RDNATION_FEE_FOR_LISTING_ITEM;
                fee.SetSlipStatus(MerchantSlipStatus.Active);
                merc.RDNationFees.Add(fee);
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (System.Exception e)
            {
                ErrorDatabaseManager.AddException(e, e.GetType());
            }
            return false;
        }

        public StoreItem GetStoreItem(int itemId, bool getReviews = false)
        {
            var item = new StoreItem();
            try
            {
                var mc = new ManagementContext();
                var storeItem = mc.StoreItems.Include("Photos").Include("Reviews").Where(x => x.StoreItemId == itemId).FirstOrDefault();
                if (storeItem == null)
                    return item;
                item = DisplayStoreItem(storeItem, getReviews);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return item;
        }


        public Classes.StoreItemDisplay GetStoreItemManager(Guid merchantId, Guid privateManagerId, int itemId)
        {
            var item = new Classes.StoreItemDisplay();
            try
            {
                var mc = new ManagementContext();
                var storeItem = mc.StoreItems.Include("Colors").Include("Colors.Color").Include("Photos").Where(x => x.StoreItemId == itemId).Where(x => x.Merchant.MerchantId == merchantId).Where(x => x.Merchant.PrivateManagerId == privateManagerId).FirstOrDefault();
                if (storeItem == null)
                    return item;

                item.CanPickUpLocally = storeItem.CanPickUpLocally;
                item.Name = storeItem.Merchant.ShopName;
                item.MerchantId = storeItem.Merchant.MerchantId;
                item.InternalId = storeItem.Merchant.InternalReference;
                item.PrivateManagerId = storeItem.Merchant.PrivateManagerId;
                item.ArticleNumber = storeItem.ArticleNumber;
                item.CanRunOutOfStock = storeItem.CanRunOutOfStock;
                if (storeItem.Merchant.CurrencyRate == null)
                {
                    item.Currency = "USD";
                    item.CurrencyCost = 1;
                }
                else
                {
                    item.Currency = storeItem.Merchant.CurrencyRate.CurrencyAbbrName;
                    item.CurrencyCost = storeItem.Merchant.CurrencyRate.CurrencyExchangePerUSD;
                }
                item.Description = storeItem.Description;
                item.Name = storeItem.Name;
                item.Price = storeItem.Price;
                item.QuantityInStock = storeItem.QuantityInStock;
                item.StoreItemId = storeItem.StoreItemId;
                item.Weight = storeItem.Weight;
                item.Note = storeItem.Note;
                item.Shipping = storeItem.ShippingCosts;
                item.ShippingAdditional = storeItem.ShippingCostsAdditional;
                item.IsPublished = storeItem.IsPublished;
                item.ItemTypeEnum = storeItem.ItemTypeEnum;
                item.ItemType = (StoreItemTypeEnum)Enum.Parse(typeof(StoreItemTypeEnum), storeItem.ItemTypeEnum.ToString());
                if (item.ItemType == StoreItemTypeEnum.Shirt)
                {
                    item.ItemSizeEnum = storeItem.SizesEnum;
                    item.ItemSize = (StoreItemShirtSizesEnum)Enum.Parse(typeof(StoreItemShirtSizesEnum), storeItem.SizesEnum.ToString());
                }
                DisplayStoreItemColors(storeItem, item);

                foreach (var photo in storeItem.Photos)
                {
                    PhotoItem p = new PhotoItem(photo.ItemPhotoId, photo.ImageUrl, photo.ImageUrlThumb, photo.IsPrimaryPhoto, photo.AlternativeText);
                    item.Photos.Add(p);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return item;
        }

        public StoreItemDisplay CreateNewStoreItem(StoreItemDisplay storeItem)
        {
            try
            {
                var dc = new ManagementContext();

                var merchant = dc.Merchants.Where(x => x.InternalReference == storeItem.InternalId && x.MerchantId == storeItem.MerchantId && x.PrivateManagerId == storeItem.PrivateManagerId).FirstOrDefault();
                if (merchant != null)
                {
                    DataModels.Store.StoreItem si = new DataModels.Store.StoreItem();
                    si.ArticleNumber = storeItem.ArticleNumber;
                    si.CanRunOutOfStock = storeItem.CanRunOutOfStock;
                    si.CanPickUpLocally = storeItem.CanPickUpLocally;
                    si.Description = storeItem.Description;
                    si.ExemptFromShipping = false;
                    si.Merchant = merchant;
                    si.Name = storeItem.Name;
                    si.Note = storeItem.Note;
                    si.Price = storeItem.Price;
                    si.QuantityInStock = storeItem.QuantityInStock;
                    si.VisibleOnRdn = false;
                    si.Weight = storeItem.Weight;
                    si.ShippingCosts = storeItem.Shipping;
                    si.IsPublished = false;
                    //si.ItemTypeEnum = storeItem.ItemTypeEnum;
                    si.ItemTypeEnum = (int)storeItem.ItemType;
                    si.Category = dc.StoreItemCategories.Where(x => x.StoreItemCategoryId == si.ItemTypeEnum).FirstOrDefault();
                    if (storeItem.HasExtraLarge)
                        si.SizesEnum += Convert.ToInt32(StoreItemShirtSizesEnum.X_Large);
                    if (storeItem.HasExtraSmall)
                        si.SizesEnum += Convert.ToInt32(StoreItemShirtSizesEnum.X_Small);
                    if (storeItem.HasLarge)
                        si.SizesEnum += Convert.ToInt32(StoreItemShirtSizesEnum.Large);
                    if (storeItem.HasMedium)
                        si.SizesEnum += Convert.ToInt32(StoreItemShirtSizesEnum.Medium);
                    if (storeItem.HasSmall)
                        si.SizesEnum += Convert.ToInt32(StoreItemShirtSizesEnum.Small);
                    if (storeItem.HasXXLarge)
                        si.SizesEnum += Convert.ToInt32(StoreItemShirtSizesEnum.XX_Large);

                    if (!String.IsNullOrEmpty(storeItem.ColorTempSelected))
                    {
                        foreach (string color in storeItem.ColorTempSelected.Split(';'))
                        {
                            if (color.Length > 3)
                            {
                                Color c = ColorTranslator.FromHtml(color);
                                int arb = c.ToArgb();
                                var colorDb = dc.Colors.Where(x => x.ColorIdCSharp == arb).FirstOrDefault();
                                if (colorDb != null)
                                {
                                    DataModels.Store.StoreItemColor cItem = new DataModels.Store.StoreItemColor();
                                    cItem.Color = colorDb;
                                    cItem.StoreItem = si;
                                    si.Colors.Add(cItem);
                                }
                            }
                        }
                    }

                    dc.StoreItems.Add(si);
                    dc.SaveChanges();

                    storeItem.StoreItemId = si.StoreItemId;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return storeItem;
        }

        public DisplayStore CreateNewStoreAndMerchantAccount(Guid internalId)
        {
            DisplayStore store = new DisplayStore();

            try
            {
                var dc = new ManagementContext();
                MerchantInternalReference internalReference = MerchantInternalReference.Member;
                string notificationEmail = String.Empty;
                string ownerName = String.Empty;
                var mem = dc.Members.Where(x => x.MemberId == internalId).FirstOrDefault();
                if (mem != null)
                {
                    internalReference = MerchantInternalReference.Member;
                    if (mem.ContactCard != null && mem.ContactCard.Emails.FirstOrDefault() != null)
                        notificationEmail = mem.ContactCard.Emails.FirstOrDefault().EmailAddress;
                    ownerName = mem.DerbyName;
                }
                var league = dc.Leagues.Where(x => x.LeagueId == internalId).FirstOrDefault();
                if (league != null)
                {
                    internalReference = MerchantInternalReference.League;
                    if (league.ContactCard != null && league.ContactCard.Emails.FirstOrDefault() != null)
                        notificationEmail = league.ContactCard.Emails.FirstOrDefault().EmailAddress;
                    ownerName = league.Name;
                }
                var federation = dc.Federations.Where(x => x.FederationId == internalId).FirstOrDefault();
                if (federation != null)
                {
                    internalReference = MerchantInternalReference.Federation;
                    if (federation.ContactCard != null && federation.ContactCard.Emails.FirstOrDefault() != null)
                        notificationEmail = federation.ContactCard.Emails.FirstOrDefault().EmailAddress;
                    ownerName = federation.Name;
                }

                Merchant merc = MerchantGateway.CreateMerchantAccount(internalId, internalReference, notificationEmail, ownerName);

                store.InternalReference = internalId;
                store.MerchantId = merc.MerchantId;
                store.PrivateManagerId = merc.PrivateManagerId;
                store.ShopName = merc.ShopName;
                store.IsPublished = false;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return store;

        }

        public static  bool ChangeOwnerOfStore(Guid internalId, Guid merchantId)
        {

            try
            {
                var dc = new ManagementContext();
                MerchantInternalReference internalReference = MerchantInternalReference.Member;
                string notificationEmail = String.Empty;
                string ownerName = String.Empty;
                var mem = dc.Members.Where(x => x.MemberId == internalId).FirstOrDefault();
                if (mem != null)
                {
                    internalReference = MerchantInternalReference.Member;
                    if (mem.ContactCard != null && mem.ContactCard.Emails.FirstOrDefault() != null)
                        notificationEmail = mem.ContactCard.Emails.FirstOrDefault().EmailAddress;
                    ownerName = mem.DerbyName;
                }
                var league = dc.Leagues.Where(x => x.LeagueId == internalId).FirstOrDefault();
                if (league != null)
                {
                    internalReference = MerchantInternalReference.League;
                    if (league.ContactCard != null && league.ContactCard.Emails.FirstOrDefault() != null)
                        notificationEmail = league.ContactCard.Emails.FirstOrDefault().EmailAddress;
                    ownerName = league.Name;
                }
                var federation = dc.Federations.Where(x => x.FederationId == internalId).FirstOrDefault();
                if (federation != null)
                {
                    internalReference = MerchantInternalReference.Federation;
                    if (federation.ContactCard != null && federation.ContactCard.Emails.FirstOrDefault() != null)
                        notificationEmail = federation.ContactCard.Emails.FirstOrDefault().EmailAddress;
                    ownerName = federation.Name;
                }

                var merch = dc.Merchants.Where(x => x.MerchantId == merchantId).FirstOrDefault();
                merch.InternalReference = internalId;
                merch.InternalReferenceType = (byte)internalReference;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;

        }


        public static Classes.Store GetStoreIdsFromInternalId(Guid ownerId)
        {
            var store = new Classes.Store();
            try
            {
                var mc = new ManagementContext();
                var merchant = mc.Merchants.FirstOrDefault(x => x.InternalReference == ownerId);

                if (merchant != null)
                {
                    store.MerchantId = merchant.MerchantId;
                    store.PrivateManagerId = merchant.PrivateManagerId;
                    store.InternalId = merchant.InternalReference;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return store;
        }


        public static string GetShopUrl(Guid ownerId)
        {
            try
            {
                var mc = new ManagementContext();
                var merchant = mc.Merchants.FirstOrDefault(x => x.InternalReference == ownerId);

                if (merchant != null)
                {
                    return LibraryConfig.ShopSite + UrlManager.STORE_MERCHANT_SHOP_URL + merchant.MerchantId.ToString().Replace("-", "") + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(merchant.ShopName);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return string.Empty;
        }

        public Classes.Store GetStore(Guid? merchantId, bool isRdn = false)
        {
            var store = new Classes.Store();
            try
            {
                if (!merchantId.HasValue && !isRdn)
                    throw new Exception("Invalid merchant id. (CreateNewShoppingCart in StoreGateway");

                var mc = new ManagementContext();
                Merchant merchant = null;
                if (merchantId.HasValue) // ToDo: When Store is updated, updated the references here to include more tables
                    merchant = mc.Merchants.Include("Items").FirstOrDefault(x => x.MerchantId.Equals(merchantId.Value));
                else
                    merchant = mc.Merchants.Include("Items").FirstOrDefault(x => x.IsRDNation.Equals(true));
                if (merchant == null)
                    return null;

                store.Name = merchant.ShopName;
                store.MerchantId = merchant.MerchantId;
                store.InternalId = merchant.InternalReference;
                foreach (var storeItem in merchant.Items)
                {
                    var item = new Classes.StoreItemDisplay();
                    item.ArticleNumber = storeItem.ArticleNumber;
                    item.CanRunOutOfStock = storeItem.CanRunOutOfStock;
                    if (storeItem.Merchant.CurrencyRate == null)
                    {
                        item.Currency = "USD";
                        item.CurrencyCost = 1;
                    }
                    else
                    {
                        item.Currency = storeItem.Merchant.CurrencyRate.CurrencyAbbrName;
                        item.CurrencyCost = storeItem.Merchant.CurrencyRate.CurrencyExchangePerUSD;
                    }
                    item.Description = storeItem.Description;
                    item.Name = storeItem.Name;
                    item.Price = storeItem.Price;
                    item.QuantityInStock = storeItem.QuantityInStock;
                    item.StoreItemId = storeItem.StoreItemId;
                    item.Weight = storeItem.Weight;
                    item.Note = storeItem.Note;
                    item.IsPublished = storeItem.IsPublished;
                    item.ItemType = (StoreItemTypeEnum)Enum.Parse(typeof(StoreItemTypeEnum), storeItem.ItemTypeEnum.ToString());
                    if (item.ItemType == StoreItemTypeEnum.Shirt)
                    {
                        item.ItemSize = (StoreItemShirtSizesEnum)Enum.Parse(typeof(StoreItemShirtSizesEnum), storeItem.SizesEnum.ToString());
                        item.ItemSizeEnum = storeItem.SizesEnum;
                    }
                    store.StoreItems.Add(item);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return store;
        }
        public List<DisplayStore> GetRandomShops()
        {
            var stores = new List<DisplayStore>();
            try
            {
                var mc = new ManagementContext();
                var shops = mc.Merchants.Where(x => x.IsPublished && x.Items.Select(y => y.IsPublished).Count() > 0).OrderBy(x => Guid.NewGuid()).Take(10);

                foreach (var shop in shops)
                {
                    var s = new DisplayStore();
                    s.MerchantId = shop.MerchantId;
                    s.ShopName = shop.ShopName;
                    s.OwnerName = shop.OwnerName;
                    s.Description = shop.Description;

                    foreach (var storeItem in shop.Items.Where(x => x.IsPublished))
                    {
                        var item = DisplayStoreItem(storeItem);
                        if (item != null)
                            s.StoreItems.Add(item);
                    }
                    stores.Add(s);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return stores;
        }
        public List<DisplayStore> GetPublicShops(int page, int count)
        {
            var stores = new List<DisplayStore>();
            try
            {
                var mc = new ManagementContext();
                var shops = mc.Merchants.Where(x => x.IsPublished && x.Items.Select(y => y.IsPublished).Count() > 0).OrderBy(x => Guid.NewGuid()).Skip(page * count).Take(count);

                foreach (var shop in shops)
                {
                    var s = new DisplayStore();
                    s.MerchantId = shop.MerchantId;
                    s.ShopName = shop.ShopName;
                    s.OwnerName = shop.OwnerName;
                    s.Description = shop.Description;

                    foreach (var storeItem in shop.Items.Where(x => x.IsPublished))
                    {
                        var item = DisplayStoreItem(storeItem);
                        if (item != null)
                            s.StoreItems.Add(item);
                    }
                    stores.Add(s);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return stores;
        }
        public DisplayCategory GetCategoryAndItems(long categoryId)
        {

            var cat = new DisplayCategory();
            try
            {
                var mc = new ManagementContext();
                var items = mc.StoreItems.Where(x => x.IsPublished && x.Category.StoreItemCategoryId == categoryId).OrderBy(x => Guid.NewGuid()).Take(40);

                if (items.Count() == 0)
                    return cat;

                cat.Name = items.FirstOrDefault().Category.Name;
                cat.Description = items.FirstOrDefault().Category.Description;
                cat.Id = items.FirstOrDefault().Category.StoreItemCategoryId;
                foreach (var storeItem in items)
                {
                    var item = DisplayStoreItem(storeItem);
                    if (item != null)
                        cat.StoreItems.Add(item);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return cat;
        }


        public DisplayStore GetRandomPublishedStoreItems(int howMany)
        {

            var store = new DisplayStore();
            try
            {
                var mc = new ManagementContext();
                var items = mc.StoreItems.Include("Photos").Include("Category").Include("Merchant").Where(x => x.IsPublished && x.Merchant.IsPublished).OrderBy(x => Guid.NewGuid()).Take(howMany);

                foreach (var storeItem in items)
                {
                    var item = DisplayStoreItem(storeItem);
                    if (item != null)
                        store.StoreItems.Add(item);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return store;
        }
        public DisplayStore GetAllPublishedStoreItems()
        {

            var store = new DisplayStore();
            try
            {
                var mc = new ManagementContext();
                var items = mc.StoreItems.Include("Photos").Include("Category").Include("Merchant").Where(x => x.IsPublished && x.Merchant.IsPublished).ToList();

                foreach (var storeItem in items)
                {
                    var item = DisplayStoreItem(storeItem);
                    if (item != null)
                        store.StoreItems.Add(item);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return store;
        }
        public int GetAllPublishedStoreItemsCount()
        {
            try
            {
                var mc = new ManagementContext();
                var items = mc.StoreItems.Include("Photos").Include("Category").Include("Merchant").Where(x => x.IsPublished && x.Merchant.IsPublished).ToList();
                return items.Count();

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }


        public List<StoreItemJson> SearchStoreItems(string keyword, int limit)
        {
            var store = new List<StoreItemJson>();
            try
            {
                var mc = new ManagementContext();
                var items = (from a in mc.StoreItems
                             where a.IsPublished && a.Merchant.IsPublished
                             where a.Name.Contains(keyword) || a.Description.Contains(keyword) || a.Note.Contains(keyword)
                             select a).Distinct().Take(limit);
                foreach (var storeItem in items)
                {
                    var item = DisplayStoreItemJson(storeItem);
                    if (item != null)
                        store.Add(item);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return store;
        }
        public List<StoreItemJson> SearchStoreItems()
        {
            var store = new List<StoreItemJson>();
            try
            {
                var mc = new ManagementContext();
                var items = (from a in mc.StoreItems
                             where a.IsPublished && a.Merchant.IsPublished
                             select a).Distinct();
                foreach (var storeItem in items)
                {
                    var item = DisplayStoreItemJson(storeItem);
                    if (item != null)
                        store.Add(item);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return store;
        }


        private static StoreItem DisplayStoreItem(DataModels.Store.StoreItem storeItem, bool getAllReviews = false)
        {
            try
            {
                var item = new Classes.StoreItem();
                item.CanPickUpLocally = storeItem.CanPickUpLocally;
                item.ArticleNumber = storeItem.ArticleNumber;
                item.CanRunOutOfStock = storeItem.CanRunOutOfStock;
                if (storeItem.Merchant.CurrencyRate == null)
                {
                    item.Currency = "USD";
                    item.CurrencyCost = 1;
                }
                else
                {
                    item.Currency = storeItem.Merchant.CurrencyRate.CurrencyAbbrName;
                    item.CurrencyCost = storeItem.Merchant.CurrencyRate.CurrencyExchangePerUSD;
                }
                if (getAllReviews)
                {
                    foreach (var b in storeItem.Reviews)
                    {
                        item.Reviews.Add(ItemReview.DisplayReviewList(b));
                    }
                }
                item.Description = storeItem.Description;
                item.Shipping = storeItem.ShippingCosts;
                item.ShippingAdditional = storeItem.ShippingCostsAdditional;
                item.Name = storeItem.Name;
                if (item.Name.Length > 21)
                    item.NameTrimmed = item.Name.Remove(21) + "...";
                else
                    item.NameTrimmed = item.Name;

                item.Price = storeItem.Price;
                item.QuantityInStock = storeItem.QuantityInStock;
                item.StoreItemId = storeItem.StoreItemId;
                item.Weight = storeItem.Weight;

                if (storeItem.LastModified > new DateTime(2013, 11, 23) || storeItem.Created > new DateTime(2013, 11, 23))
                {
                    item.NoteHtml = storeItem.Note;
                }
                else if (storeItem.Created < new DateTime(2013, 11, 23))
                {
                    RDN.Library.Util.MarkdownSharp.Markdown markdown = new RDN.Library.Util.MarkdownSharp.Markdown();
                    markdown.AutoHyperlink = true;
                    markdown.LinkEmails = true;
                    item.NoteHtml = HtmlSanitize.FilterHtmlToWhitelist(markdown.Transform(storeItem.Note)).Replace("</p>", "</p><br/>");
                }

                item.Note = storeItem.Note;
                item.IsPublished = storeItem.IsPublished;
                item.Store.MerchantId = storeItem.Merchant.MerchantId;
                item.AcceptsPayPal = storeItem.Merchant.AcceptPaymentsViaPaypal;
                item.AcceptsStripe = storeItem.Merchant.AcceptPaymentsViaStripe;
                item.Views = storeItem.Views.ToString("N0");

                if (storeItem.Category != null)
                {
                    item.Category.Name = storeItem.Category.Name;
                    item.Category.StoreItemCategoryId = storeItem.Category.StoreItemCategoryId;
                }
                item.Store.Name = storeItem.Merchant.ShopName;

                if (!String.IsNullOrEmpty(storeItem.Merchant.ShopName) && storeItem.Merchant.ShopName.Length > 15)
                    item.Store.NameTrimmed = storeItem.Merchant.ShopName.Remove(15) + "...";
                else
                    item.Store.NameTrimmed = storeItem.Merchant.ShopName;
                item.Store.Description = storeItem.Merchant.Description;
                item.Store.MerchantId = storeItem.Merchant.MerchantId;
                item.ItemType = (StoreItemTypeEnum)Enum.Parse(typeof(StoreItemTypeEnum), storeItem.ItemTypeEnum.ToString());
                if (item.ItemType == StoreItemTypeEnum.Shirt)
                {
                    item.ItemSize = (StoreItemShirtSizesEnum)Enum.Parse(typeof(StoreItemShirtSizesEnum), storeItem.SizesEnum.ToString());
                    item.ItemSizeEnum = storeItem.SizesEnum;
                }

                foreach (var photo in storeItem.Photos)
                {
                    PhotoItem p = new PhotoItem(photo.ItemPhotoId, photo.ImageUrl, photo.ImageUrlThumb, photo.IsPrimaryPhoto, photo.AlternativeText);
                    item.Photos.Add(p);
                }
                DisplayStoreItemColors(storeItem, item);

                return item;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        private static void DisplayStoreItemColors(DataModels.Store.StoreItem storeItem, StoreItem item)
        {
            try
            {
                foreach (var color in storeItem.Colors)
                {
                    ColorDisplay d = DisplayStoreItemColor(color);
                    item.ColorTempSelected += d.HexColor + ";";
                    item.Colors.Add(d);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        private static ColorDisplay DisplayStoreItemColor(DataModels.Store.StoreItemColor color)
        {
            try
            {
                if (color != null)
                {
                    var c = Color.FromArgb(color.Color.ColorIdCSharp);
                    var hex = ColorTranslator.ToHtml(c);
                    ColorDisplay d = new ColorDisplay();
                    d.ColorId = color.ColorId;
                    d.CSharpColor = color.Color.ColorIdCSharp;
                    d.HexColor = hex;
                    d.NameOfColor = color.Color.ColorName;
                    return d;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        private static StoreItemJson DisplayStoreItemJson(DataModels.Store.StoreItem storeItem)
        {
            try
            {
                var item = new Classes.StoreItemJson();
                item.Currency = Currency.USD.ToString();
                item.Description = storeItem.Description;

                item.Name = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(storeItem.Name);
                if (item.Name.Length > 22)
                    item.NameTrimmed = item.Name.Remove(22) + "...";
                else
                    item.NameTrimmed = item.Name;

                item.Price = storeItem.Price.ToString("N2");
                item.StoreItemId = storeItem.StoreItemId;
                item.ShopMerchantId = storeItem.Merchant.MerchantId.ToString().Replace("-", "");
                item.ShopName = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(storeItem.Merchant.ShopName);
                if (storeItem.Merchant.ShopName.Length > 15)
                    item.ShopNameTrimmed = storeItem.Merchant.ShopName.Remove(15) + "...";
                else
                    item.ShopNameTrimmed = storeItem.Merchant.ShopName;

                var photo = storeItem.Photos.FirstOrDefault();
                if (photo != null)
                {
                    item.PhotoUrl = photo.ImageUrl;
                    item.PhotoAlt = photo.AlternativeText;
                }
                return item;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public DisplayStore GetPublicStore(Guid merchantId, int category)
        {
            var store = new DisplayStore();
            try
            {
                var mc = new ManagementContext();
                Merchant merchant = null;

                merchant = mc.Merchants.Include("Locations").Include("Items").Where(x => x.MerchantId == merchantId).FirstOrDefault();
                if (merchant == null)
                    return null;

                store.ShopName = merchant.ShopName;
                store.MerchantId = merchant.MerchantId;
                store.InternalReference = merchant.InternalReference;
                store.PrivateManagerId = merchant.PrivateManagerId;
                store.AccountStatus = (MerchantAccountStatus)merchant.MerchantAccountStatus;
                store.AmountOnAccount = merchant.AmountOnAccount;
                store.AutoAcceptPayment = merchant.AutoAcceptPayment;
                store.AutoShipWhenPaymentIsReceived = merchant.AutoShipWhenPaymentIsReceived;
                store.Bank = merchant.Bank;
                store.BankAccount = merchant.BankAccount;
                if (merchant.CurrencyRate == null)
                {
                    store.Currency = "USD";
                    store.CurrencyCost = 1;
                }
                else
                {
                    store.Currency = merchant.CurrencyRate.CurrencyAbbrName;
                    store.CurrencyCost = merchant.CurrencyRate.CurrencyExchangePerUSD;
                }
                store.IsRDNation = merchant.IsRDNation;
                store.OrderPayedNotificationEmail = merchant.OrderPayedNotificationEmail;
                store.PayedFeesToRDN = merchant.PayedFeesToRDN;
                store.RDNFixedFee = merchant.RDNFixedFee;
                store.RDNPercentageFee = merchant.RDNPercentageFee;
                store.ShippingNotificationEmail = merchant.ShippingNotificationEmail;
                store.TaxRate = merchant.TaxRate;
                store.IsPublished = merchant.IsPublished;
                store.Description = merchant.Description;
                store.WelcomeMessage = merchant.WelcomeMessage;
                store.StripePublishableKey = merchant.StripePublishableKey;
                store.AcceptPaymentsViaStripe = merchant.AcceptPaymentsViaStripe;
                store.AcceptPaymentsViaPaypal = merchant.AcceptPaymentsViaPaypal;
                if (merchant.Locations.FirstOrDefault() != null)
                    store.Location = Location.LocationFactory.DisplayLocation(merchant.Locations.FirstOrDefault());


                var items = merchant.Items.Where(x => x.IsPublished == true);
                try
                {
                    if (category > 0)
                        items = items.Where(x => x.Category != null).Where(x => x.Category.StoreItemCategoryId == category);
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
                foreach (var storeItem in items)
                {
                    var item = DisplayStoreItem(storeItem);
                    if (item != null)
                    {
                        store.StoreItems.Add(item);

                    }
                }
                foreach (var storeItem in merchant.Items.GroupBy(x => x.Category))
                {
                    if (storeItem.Key != null)
                    {
                        var cat = store.StoreCategories.Where(x => x.StoreItemCategoryId == storeItem.Key.StoreItemCategoryId).FirstOrDefault();
                        if (cat == null && storeItem != null && storeItem.Key != null)
                        {
                            StoreCategory c = new StoreCategory();
                            c.Name = storeItem.Key.Name;
                            c.StoreItemCategoryId = storeItem.Key.StoreItemCategoryId;
                            store.StoreCategories.Add(c);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return store;
        }
        public DisplayStore GetStoreForManager(Guid privManagerId)
        {
            try
            {
                var mc = new ManagementContext();
                Merchant merchant = mc.Merchants.Include("Items").Where(x => x.PrivateManagerId == privManagerId).FirstOrDefault();
                if (merchant == null)
                    return null;
                return GetDisplayStore(merchant);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }


        public DisplayStore GetStoreForManager(Guid? merchantId, Guid? managerId, bool isRdn = false)
        {
            try
            {
                if (!merchantId.HasValue && !isRdn)
                    throw new Exception("Invalid merchant id. (CreateNewShoppingCart in StoreGateway");

                var mc = new ManagementContext();
                Merchant merchant = null;
                if (merchantId.HasValue) // ToDo: When Store is updated, updated the references here to include more tables
                    merchant = mc.Merchants.Include("Locations").Include("Items").Include("Invoices").Where(x => x.MerchantId == merchantId.Value && x.PrivateManagerId == managerId).FirstOrDefault();
                else
                    merchant = mc.Merchants.Include("Locations").Include("Items").Include("Invoices").FirstOrDefault(x => x.IsRDNation.Equals(true));
                if (merchant == null)
                    return null;
                return GetDisplayStore(merchant);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new DisplayStore();
        }
        public DisplayInvoice GetInvoiceForManager(Guid merchantId, Guid managerId, Guid invoiceId)
        {
            try
            {
                var mc = new ManagementContext();
                var invoice = mc.Invoices.Where(x => x.InvoiceId == invoiceId && x.Merchant.MerchantId == merchantId && x.Merchant.PrivateManagerId == managerId).FirstOrDefault();

                return DisplayInvoice(invoice);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new DisplayInvoice();
        }
        public DisplayInvoice GetInvoice(Guid invoiceId)
        {
            try
            {
                var mc = new ManagementContext();
                var invoice = mc.Invoices.Include("Items").Include("Items.ColorOfItem").Where(x => x.InvoiceId == invoiceId).FirstOrDefault();

                return DisplayInvoice(invoice);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new DisplayInvoice();
        }
        public bool UpdateInvoiceForStoreOrder(DisplayInvoice invoice)
        {
            try
            {
                var mc = new ManagementContext();
                var voice = mc.Invoices.Where(x => x.InvoiceId == invoice.InvoiceId).FirstOrDefault();
                voice.AdminNote = invoice.AdminNote;
                voice.Merchant = voice.Merchant;
                int c = mc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        private static DisplayStore GetDisplayStore(Merchant merchant)
        {
            try
            {
                var store = new DisplayStore();
                store.ShopName = merchant.ShopName;
                store.MerchantId = merchant.MerchantId;
                store.InternalReference = merchant.InternalReference;
                store.PrivateManagerId = merchant.PrivateManagerId;
                store.AccountStatus = (MerchantAccountStatus)merchant.MerchantAccountStatus;
                store.AmountOnAccount = merchant.AmountOnAccount;
                store.AutoAcceptPayment = merchant.AutoAcceptPayment;
                store.AutoShipWhenPaymentIsReceived = merchant.AutoShipWhenPaymentIsReceived;
                store.Bank = merchant.Bank;
                store.BankAccount = merchant.BankAccount;
                if (merchant.CurrencyRate == null)
                {
                    store.Currency = "USD";
                    store.CurrencyCost = 1;
                }
                else
                {
                    store.Currency = merchant.CurrencyRate.CurrencyAbbrName;
                    store.CurrencyCost = merchant.CurrencyRate.CurrencyExchangePerUSD;
                }
                store.IsRDNation = merchant.IsRDNation;
                store.OrderPayedNotificationEmail = merchant.OrderPayedNotificationEmail;
                store.PayedFeesToRDN = merchant.PayedFeesToRDN;
                store.RDNFixedFee = merchant.RDNFixedFee;
                store.RDNPercentageFee = merchant.RDNPercentageFee;
                store.ShippingNotificationEmail = merchant.ShippingNotificationEmail;
                store.TaxRate = merchant.TaxRate;
                store.IsPublished = merchant.IsPublished;
                store.AcceptPaymentsViaPaypal = merchant.AcceptPaymentsViaPaypal;
                store.PayPalEmailAddressForPayments = merchant.PaypalEmail;
                store.AcceptPaymentsViaStripe = merchant.AcceptPaymentsViaStripe;
                store.StripeConnectKey = merchant.StripeConnectKey;
                store.StripeConnectToken = merchant.StripeConnectToken;
                store.Description = merchant.Description;
                store.WelcomeMessage = merchant.WelcomeMessage;
                if (merchant.Locations.FirstOrDefault() != null)
                    store.Location = Location.LocationFactory.DisplayLocation(merchant.Locations.FirstOrDefault());

                foreach (var storeItem in merchant.Items)
                {
                    var item = DisplayStoreItem(storeItem);
                    if (item != null)
                        store.StoreItems.Add(item);
                }
                foreach (var voice in merchant.Invoices.Where(x => x.Items.Count > 0).OrderByDescending(x => x.Created))
                {
                    DisplayInvoice v = DisplayInvoice(voice);
                    store.Invoices.Add(v);
                }
                return store;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
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
                v.ShoppingCartId = voice.ShoppingCartId;
                v.ShippingCost = voice.Shipping;
                v.RDNDeductedFee = voice.RDNDeductedFee;
                v.CreditCardCompanyProcessorDeductedFee = voice.CreditCardCompanyProcessorDeductedFee;
                v.PaymentProvider = (PaymentProvider)voice.PaymentProvider;
                v.Note = voice.Note;
                v.InvoiceStatus = (InvoiceStatus)voice.InvoiceStatus;
                v.ShippingType = (ShippingType)voice.ShippingType;
                v.Created = voice.Created;
                v.TotalItemsBeingSold = 0;
                if (voice.InvoiceShipping != null)
                {
                    v.InvoiceShipping.City = voice.InvoiceShipping.City;
                    if (!String.IsNullOrEmpty(voice.InvoiceShipping.Country))
                    {
                        var count = SiteCache.GetCountries().Where(x => x.CountryId == Convert.ToInt32(voice.InvoiceShipping.Country)).FirstOrDefault();
                        v.InvoiceShipping.Country = count.Name;
                    }
                    v.InvoiceShipping.Email = voice.InvoiceShipping.Email;
                    v.InvoiceShipping.FirstName = voice.InvoiceShipping.FirstName;
                    v.InvoiceShipping.LastName = voice.InvoiceShipping.LastName;
                    v.InvoiceShipping.State = voice.InvoiceShipping.State;
                    v.InvoiceShipping.Street = voice.InvoiceShipping.Street;
                    v.InvoiceShipping.Street2 = voice.InvoiceShipping.Street2;
                    v.InvoiceShipping.Zip = voice.InvoiceShipping.Zip;
                }
                foreach (var item in voice.Items)
                {
                    InvoiceItem i = new InvoiceItem();
                    i.Article2Number = item.Article2Number;
                    i.ArticleNumber = item.ArticleNumber;
                    i.Description = item.Description;
                    i.Name = item.Name;
                    i.Price = item.Price;
                    i.InvoiceItemId = item.InvoiceItemId;
                    i.Quantity = item.Quantity;
                    i.SizeOfItem = item.SizeOfItem;
                    if (i.SizeOfItem > 0)
                        i.SizeOfItemName = Enum.Parse(typeof(StoreItemShirtSizesEnum), item.SizeOfItem.ToString()).ToString();
                    i.StoreItemId = item.StoreItemId;
                    i.TotalShipping = item.Shipping;
                    if (item.ColorOfItem != null)
                    {
                        i.ColorName = item.ColorOfItem.ColorName;
                        Color c = Color.FromArgb(item.ColorOfItem.ColorIdCSharp);
                        i.ColorHex = ColorTranslator.ToHtml(c);
                    }
                    v.TotalItemsBeingSold += item.Quantity;
                    v.InvoiceItems.Add(i);
                }
                v.Merchant.MerchantId = voice.Merchant.MerchantId;
                v.Merchant.PrivateManagerId = voice.Merchant.PrivateManagerId;
                v.Merchant.ShopName = voice.Merchant.ShopName;
                var loc = Location.LocationFactory.DisplayLocation(voice.Merchant.Locations.FirstOrDefault());
                var address = loc.Contact.Addresses.FirstOrDefault();
                if (address != null)
                {
                    v.SellersAddress = new InvoiceContactInfo() { City = address.CityRaw, CompanyName = loc.LocationName, Country = address.Country, State = address.StateRaw, Street = address.Address1, Street2 = address.Address2, Zip = address.Zip };
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return v;
        }

        public StoreShoppingCart CreateNewShoppingCart(Guid? merchantId, Guid userId = new Guid(), bool isRdn = false, string ip = null)
        {
            var storeCart = new StoreShoppingCart(ip);
            try
            {
                if (!merchantId.HasValue && !isRdn)
                    throw new Exception("Invalid merchant id. (CreateNewShoppingCart in StoreGateway");

                var mc = new ManagementContext();
                Merchant merchant = null;
                if (merchantId.HasValue)
                    merchant = mc.Merchants.FirstOrDefault(x => x.MerchantId.Equals(merchantId.Value));
                else
                    merchant = mc.Merchants.FirstOrDefault(x => x.IsRDNation.Equals(true));
                if (merchant == null)
                    return null;

                var cartId = Guid.NewGuid();
                var dbcart = new DataModels.Store.ShoppingCart();
                dbcart.Ip = ip;
                if (userId != new Guid())
                    dbcart.UserId = userId;
                //dbcart.Merchant = merchant;
                dbcart.ShoppingCartId = cartId;
                mc.ShoppingCarts.Add(dbcart);
                mc.SaveChanges();

                storeCart.Stores.Add(new Classes.Store { MerchantId = merchant.MerchantId, Name = merchant.ShopName });
                storeCart.ShoppingCartId = cartId;


            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return storeCart;
        }

        public static void SetShoppingCartSession(Guid cartId, HttpContextBase context)
        {
            context.Session.Add("ShoppingCartId", cartId);
        }

        public Guid? GetShoppingCartId(Guid userId)
        {
            try
            {
                var dc = new ManagementContext();
                var cart = dc.ShoppingCarts.Where(x => x.UserId == userId).OrderByDescending(x => x.Created).FirstOrDefault();
                if (cart != null)
                    return cart.ShoppingCartId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public StoreShoppingCart GetShoppingCart(Guid cartId)
        {
            var output = new StoreShoppingCart();
            try
            {
                var mc = new ManagementContext();
                DateTime now = DateTime.Now;
                var cart = mc.ShoppingCarts.Include("Items").Include("Items.StoreItem").Include("Items.StoreItem.Colors").Include("Items.StoreItem.Colors.Color").Include("Items.StoreItem.Merchant").FirstOrDefault(x => x.ShoppingCartId.Equals(cartId));
                if (cart == null) return output;

                output.ShoppingCartId = cart.ShoppingCartId;
                output.Ip = cart.Ip;

                foreach (var shoppingCartItem in cart.Items)
                {
                    output.ItemsCount += 1;
                    var item = new StoreItemDisplay();
                    item.Merchant.MerchantId = shoppingCartItem.StoreItem.Merchant.MerchantId;
                    item.Merchant.Name = shoppingCartItem.StoreItem.Merchant.ShopName;
                    item.Merchant.TaxRate = shoppingCartItem.StoreItem.Merchant.TaxRate;
                    if (shoppingCartItem.StoreItem.Merchant.CurrencyRate == null)
                    {
                        item.Merchant.Currency = "USD";
                        item.Merchant.CurrencyCost = 1;
                    }
                    else
                    {
                        item.Merchant.Currency = shoppingCartItem.StoreItem.Merchant.CurrencyRate.CurrencyAbbrName;
                        item.Merchant.CurrencyCost = shoppingCartItem.StoreItem.Merchant.CurrencyRate.CurrencyExchangePerUSD;
                    }
                    item.Name = shoppingCartItem.StoreItem.Name;
                    item.BasePrice = shoppingCartItem.StoreItem.Price;
                    item.Price = (shoppingCartItem.StoreItem.Price * shoppingCartItem.Quantity);
                    var color = DisplayStoreItemColor(shoppingCartItem.StoreItem.Colors.Where(x => x.Color.ColorIdCSharp == shoppingCartItem.Color).FirstOrDefault());
                    if (color != null)
                    {
                        item.Colors.Add(color);
                        item.ColorAGB = color.CSharpColor;
                        item.ColorHex = color.HexColor;
                    }
                    item.QuantityOrdered = shoppingCartItem.Quantity;
                    item.Weight = (shoppingCartItem.StoreItem.Weight * shoppingCartItem.Quantity);
                    item.ShoppingCartItemId = shoppingCartItem.ShoppingCartItemId;
                    item.ArticleNumber = shoppingCartItem.StoreItem.ArticleNumber;
                    if (shoppingCartItem.StoreItem.Merchant.CurrencyRate == null)
                    {
                        item.Currency = "USD";
                        item.CurrencyCost = 1;
                    }
                    else
                    {
                        item.Currency = shoppingCartItem.StoreItem.Merchant.CurrencyRate.CurrencyAbbrName;
                        item.CurrencyCost = shoppingCartItem.StoreItem.Merchant.CurrencyRate.CurrencyExchangePerUSD;
                    }
                    item.Description = shoppingCartItem.StoreItem.Description;
                    item.WillPickUpLocally = shoppingCartItem.WillPickUpLocally;
                    if (!shoppingCartItem.WillPickUpLocally)
                    {
                        item.Shipping = shoppingCartItem.StoreItem.ShippingCosts;
                        //if there are more than one of the same items being shipped, then we add on the additional fees.
                        if (shoppingCartItem.Quantity > 1)
                            item.Shipping += shoppingCartItem.StoreItem.ShippingCostsAdditional * (shoppingCartItem.Quantity - 1);
                    }
                    else
                        item.Shipping = 0;
                    item.CanRunOutOfStock = shoppingCartItem.StoreItem.CanRunOutOfStock;
                    item.StoreItemId = shoppingCartItem.StoreItem.StoreItemId;
                    item.Note = shoppingCartItem.StoreItem.Note;
                    item.BaseTaxOnItem = Math.Round((item.BasePrice * Convert.ToDecimal(shoppingCartItem.StoreItem.Merchant.TaxRate)), 2);
                    item.TotalTaxOnItem = (item.BaseTaxOnItem * shoppingCartItem.Quantity);
                    item.PriceInclTax = Math.Round((item.Price + item.TotalTaxOnItem), 2);
                    item.ItemType = (StoreItemTypeEnum)Enum.Parse(typeof(StoreItemTypeEnum), shoppingCartItem.StoreItem.ItemTypeEnum.ToString());
                    if (item.ItemType == StoreItemTypeEnum.Shirt)
                    {
                        item.ItemSize = (StoreItemShirtSizesEnum)Enum.Parse(typeof(StoreItemShirtSizesEnum), shoppingCartItem.Size.ToString());
                        item.ItemSizeEnum = shoppingCartItem.Size;
                    }
                    foreach (var photo in shoppingCartItem.StoreItem.Photos)
                    {
                        PhotoItem p = new PhotoItem(photo.ItemPhotoId, photo.ImageUrl, photo.ImageUrlThumb, photo.IsPrimaryPhoto, photo.AlternativeText);
                        item.Photos.Add(p);
                    }
                    var store = output.Stores.Where(x => x.MerchantId == item.Merchant.MerchantId).FirstOrDefault();
                    if (store != null)
                    {
                        store.TotalPrice += item.Price;
                        store.TotalShipping += item.Shipping;
                        store.TotalAfterShipping += item.Price + item.Shipping;
                        store.StoreItems.Add(item);
                    }
                    else
                    {
                        Store.Classes.Store s = new Classes.Store();
                        s.MerchantId = item.Merchant.MerchantId;
                        s.Currency = item.Merchant.Currency;
                        s.StripePublishableKey = item.Merchant.StripePublishableKey;
                        s.Name = item.Merchant.Name;
                        s.TotalPrice = item.Price;
                        s.TotalShipping = item.Shipping;
                        s.TotalAfterShipping = item.Price + item.Shipping;
                        s.StoreItems.Add(item);
                        output.Stores.Add(s);
                    }
                }
                cart.Expires = DateTime.Now.AddDays(1);
                mc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return output;
        }

        public void AddItemToCart(Guid cartId, Guid merchantId, int itemId, int quantity, long size, string hexColor)
        {
            try
            {
                var mc = new ManagementContext();
                DateTime now = DateTime.Now;
                var cart = mc.ShoppingCarts.Include("Items").Include("Items.StoreItem").FirstOrDefault(x => x.ShoppingCartId.Equals(cartId) && x.Expires.CompareTo(now) == 1);
                if (cart == null) return;
                var item = mc.StoreItems.Where(x => x.Merchant.MerchantId == merchantId && x.StoreItemId == itemId).FirstOrDefault();
                if (item == null) return;
                if (item.CanRunOutOfStock && (item.QuantityInStock - quantity) < 1) return;

                int color = 0;
                if (!String.IsNullOrEmpty(hexColor))
                {
                    Color c = ColorTranslator.FromHtml(hexColor);
                    color = c.ToArgb();
                }

                // Check if the item exists in our shopping cart
                if (cart.Items.Any(x => x.StoreItem.StoreItemId.Equals(itemId) && x.Size == size && x.Color == color))
                    if (quantity == 0) // IT exists and since quantity is 0, remove it
                        cart.Items.Remove(cart.Items.First(x => x.StoreItem.StoreItemId.Equals(itemId) && x.Size == size && x.Color == color));
                    else // It exists and quatity is over 0, add the items
                    {
                        cart.Items.First(x => x.StoreItem.StoreItemId.Equals(itemId) && x.Size == size && x.Color == color).Quantity += quantity;
                    }
                else if (quantity > 0)
                    cart.Items.Add(new DataModels.Store.ShoppingCartItem { Quantity = quantity, StoreItem = item, Size = size, Color = color });
                cart.Expires = DateTime.Now.AddDays(1);
                mc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        public bool RemoveItemFromCart(Guid cartId, long shoppingCartItemId)
        {
            try
            {
                var mc = new ManagementContext();
                var cart = mc.ShoppingCartItems.Where(x => x.Cart.ShoppingCartId == cartId && x.StoreItem.StoreItemId == shoppingCartItemId).FirstOrDefault();

                mc.ShoppingCartItems.Remove(cart);
                int c = mc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: cartId + ":" + shoppingCartItemId);
            }
            return false;
        }
        public bool SubtractNumberOfItemsBeingSoldByMerchant(Guid merchantId, long storeItemId, int numberToSubtract)
        {
            try
            {
                var mc = new ManagementContext();
                DateTime now = DateTime.Now;
                var item = mc.StoreItems.Where(x => x.Merchant.MerchantId == merchantId && x.StoreItemId == storeItemId).FirstOrDefault();
                if (item != null && item.CanRunOutOfStock)
                {
                    item.QuantityInStock -= numberToSubtract;
                    item.Merchant = item.Merchant;
                    int c = mc.SaveChanges();
                    return c > 0;
                }
                else if (item != null && !item.CanRunOutOfStock)
                    return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public Display.CheckOut GetCheckoutData(Guid cartId, Guid merchantId)
        {
            var checkout = new Display.CheckOut();
            try
            {
                var mc = new ManagementContext();
                DateTime now = DateTime.Now;
                var cart = mc.ShoppingCarts.Include("Items").Include("Items.StoreItem").Include("Items.StoreItem.Merchant").FirstOrDefault(x => x.ShoppingCartId == cartId);
                if (cart == null) return null;
                var shoppingCart = GetShoppingCart(cartId);
                if (shoppingCart.Stores.Count == 0) return null;

                var merchant = GetPublicStore(merchantId, 0);
                if (merchant.Location != null)
                {
                    var address = merchant.Location.Contact.Addresses.FirstOrDefault();
                    if (address != null)
                    {
                        checkout.SellersAddress = new StoreShoppingCartContactInfo() { City = address.CityRaw, CompanyName = merchant.Location.LocationName, Country = address.Country, State = address.StateRaw, Street = address.Address1, Street2 = address.Address2, Zip = address.Zip };
                    }
                }

                checkout.ShoppingCart = shoppingCart;
                checkout.AcceptsPayPal = merchant.AcceptPaymentsViaPaypal;
                checkout.AcceptsStripe = merchant.AcceptPaymentsViaStripe;
                checkout.StripePublishableKey = merchant.StripePublishableKey;
                var merc = shoppingCart.Stores.Where(x => x.MerchantId == merchantId).FirstOrDefault();
                if (merc == null)
                    return null;

                shoppingCart.Stores.Clear();
                shoppingCart.Stores.Add(merc);



                var totalExclVat = 0.0M;
                var weight = 0.0;
                var tax = 0.0M;

                foreach (var item in shoppingCart.Stores.FirstOrDefault().StoreItems)
                {
                    totalExclVat += item.Price;
                    weight += item.Weight;
                    tax += item.TotalTaxOnItem;
                    checkout.TotalItemsCount += item.QuantityOrdered;
                    checkout.TotalShipping += item.Shipping;
                    checkout.MerchantName = item.Merchant.Name;
                    checkout.MerchantId = merchantId;
                    checkout.TaxRate = item.Merchant.TaxRate;
                    checkout.Currency = item.Merchant.Currency;

                    checkout.Currency = item.Merchant.Currency;
                    checkout.CurrencyCost = item.Merchant.CurrencyCost;

                    if (!checkout.WillPickUpAtStore)
                        checkout.WillPickUpAtStore = item.WillPickUpLocally;
                }

                checkout.TotalExclVat = totalExclVat;
                checkout.Tax = tax;
                checkout.TotalInclVat = Math.Round((checkout.TotalExclVat), 2) + checkout.TotalShipping;
                //checkout.TotalInclVat = Math.Round((checkout.TotalExclVat + checkout.Tax), 2) + checkout.TotalShipping;

                var shippingTable = mc.ShippingTable.Where(x => x.Merchant.MerchantId.Equals(merchantId) && x.WeightFrom < weight && x.WeightUpTo > weight).ToList();
                //if (shippingTable.Count == 0)
                //    throw new Exception("No shipping method found");

                var shippingOptions = new Dictionary<int, CheckOutShippingRow>();
                foreach (var shippingOption in shippingTable)
                {
                    shippingOptions.Add(shippingOption.ShippingTableId, new CheckOutShippingRow { Name = (ShippingType)shippingOption.ShippingType, Price = shippingOption.Price });
                }
                checkout.ShippingOptionsRaw = shippingOptions;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return checkout;
        }

        public List<StoreCategory> GetStoreCategories(Guid merchantId)
        {
            var output = new List<StoreCategory>();
            try
            {
                var mc = new ManagementContext();
                var categories = mc.StoreItemCategories.Where(x => x.Merchant.MerchantId.Equals(merchantId));

                foreach (var dbcategory in categories)
                {
                    var category = new StoreCategory();
                    category.Name = dbcategory.Name;
                    category.Description = dbcategory.Description;
                    category.StoreItemCategoryId = dbcategory.StoreItemCategoryId;
                    output.Add(category);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return output;
        }

        public List<StoreItem> GetStoreItems(Guid merchantId)
        {
            var output = new List<StoreItem>();
            try
            {
                var mc = new ManagementContext();
                var items = mc.StoreItems.Where(x => x.Merchant.MerchantId.Equals(merchantId));

                foreach (var dbitem in items)
                {
                    var item = new StoreItem();
                    item.CanPickUpLocally = dbitem.CanPickUpLocally;
                    item.Name = dbitem.Name;
                    item.Description = dbitem.Description;
                    item.StoreItemId = dbitem.StoreItemId;
                    item.ArticleNumber = dbitem.ArticleNumber;
                    item.CanRunOutOfStock = dbitem.CanRunOutOfStock;
                    item.Price = dbitem.Price;
                    item.QuantityInStock = dbitem.QuantityInStock;
                    item.Weight = dbitem.Weight;
                    item.Shipping = dbitem.ShippingCosts;
                    item.IsPublished = dbitem.IsPublished;
                    item.ItemType = (StoreItemTypeEnum)Enum.Parse(typeof(StoreItemTypeEnum), dbitem.ItemTypeEnum.ToString());
                    if (item.ItemType == StoreItemTypeEnum.Shirt)
                    {
                        item.ItemSize = (StoreItemShirtSizesEnum)Enum.Parse(typeof(StoreItemShirtSizesEnum), dbitem.SizesEnum.ToString());
                        item.ItemSizeEnum = dbitem.SizesEnum;
                    }
                    output.Add(item);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return output;
        }

        public List<StoreItem> GetStoreItems(int categoryId)
        {
            var output = new List<StoreItem>();
            try
            {
                var mc = new ManagementContext();
                var items = mc.StoreItems.Where(x => x.Category.StoreItemCategoryId.Equals(categoryId));

                foreach (var dbitem in items)
                {
                    var item = new StoreItem();
                    item.CanPickUpLocally = dbitem.CanPickUpLocally;
                    item.Name = dbitem.Name;
                    item.Description = dbitem.Description;
                    item.StoreItemId = dbitem.StoreItemId;
                    item.ArticleNumber = dbitem.ArticleNumber;
                    item.CanRunOutOfStock = dbitem.CanRunOutOfStock;
                    item.Price = dbitem.Price;
                    item.QuantityInStock = dbitem.QuantityInStock;
                    item.Weight = dbitem.Weight;
                    item.IsPublished = dbitem.IsPublished;
                    item.ItemType = (StoreItemTypeEnum)Enum.Parse(typeof(StoreItemTypeEnum), dbitem.ItemTypeEnum.ToString());
                    if (item.ItemType == StoreItemTypeEnum.Shirt)
                    {
                        item.ItemSize = (StoreItemShirtSizesEnum)Enum.Parse(typeof(StoreItemShirtSizesEnum), dbitem.SizesEnum.ToString());
                        item.ItemSizeEnum = dbitem.SizesEnum;
                    }
                    output.Add(item);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return output;
        }


    }
}