using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Billing.Enums;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Location;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Classes.Invoice;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Store;
using RDN.Library.Classes.Store.Classes;
using RDN.Library.Util.Enum;
using RDN.Store.Classes.Enums;
using RDN.Utilities.Config;
using RDN.Portable.Config;
using RDN.Library.Classes.Payment.Money;
using RDN.Library.Cache;
using RDN.Portable.Classes.Payment.Enums;
using RDN.Portable.Classes.ContactCard.Enums;

namespace RDN.Store.Controllers
{
    public class CartController : BaseController
    {
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [HttpPost]
        public ActionResult CheckOut(RDN.Store.Models.CheckOut model)
        {
            try
            {
                var shoppingCartId = StoreGateway.GetShoppingCartId(HttpContext);
                if (!shoppingCartId.HasValue)
                {

                    return Redirect(Url.Action("StoreCart"));
                }

                var sg = new StoreGateway();
                var checkout = sg.GetCheckoutData(shoppingCartId.Value, model.MerchantId);
                if (checkout == null || checkout.ShoppingCart.ItemsCount == 0)
                {
                    return Redirect(Url.Action("StoreCart"));

                }

                PaymentProvider paymentProvider;
                if (!Enum.TryParse(model.PaymentProviderId, out paymentProvider))
                {
                    return Redirect(Url.Action("StoreCart"));

                }

                var pg = new PaymentGateway();

                ShippingType shipType = ShippingType.Postal;
                if (checkout.WillPickUpAtStore)
                    shipType = ShippingType.PickUp;

                var invoice = pg.StartInvoiceWizard()
                    .Initalize(checkout.MerchantId, checkout.Currency, paymentProvider, PaymentMode.Live, ChargeTypeEnum.InStorePurchase)
                    .SetShipping(checkout.TotalShipping, shipType)
                    .SetInvoiceId(Guid.NewGuid())
                    .SetNotes(model.Notes)
                    .SetUserId(RDN.Library.Classes.Account.User.GetUserId())
                    .SetShoppingCartId(shoppingCartId.Value);


                if (paymentProvider == PaymentProvider.Stripe)
                {
                    string token = Request.Form["stripeToken"].ToString();
                    invoice.SetStripeTokenId(token);
                }
                var sellersAddress = new InvoiceContactInfo();
                if (shipType == ShippingType.PickUp)
                {
                    if (checkout.SellersAddress != null)
                    {
                        sellersAddress.City = checkout.SellersAddress.City;
                        sellersAddress.Country = checkout.SellersAddress.Country;
                        sellersAddress.CompanyName = checkout.SellersAddress.CompanyName;
                        sellersAddress.State = checkout.SellersAddress.State;
                        sellersAddress.Street = checkout.SellersAddress.Street;
                        sellersAddress.Street2 = checkout.SellersAddress.Street2;
                        sellersAddress.Zip = checkout.SellersAddress.Zip;
                        invoice.SetSellersAddress(sellersAddress);
                    }
                }

                var billingInfo = new InvoiceContactInfo();
                billingInfo.City = model.BillingAddress_City;
                billingInfo.Country = model.BillingAddress_Country;
                billingInfo.Email = model.BillingAddress_Email;
                billingInfo.FirstName = model.BillingAddress_FirstName;
                billingInfo.LastName = model.BillingAddress_LastName;
                billingInfo.Phone = model.BillingAddress_Phone;
                billingInfo.State = model.BillingAddress_State;
                billingInfo.Street = model.BillingAddress_Street;
                billingInfo.Street2 = model.BillingAddress_Street2;
                billingInfo.Zip = model.BillingAddress_Zip;
                if (User.Identity.IsAuthenticated)
                    RDN.Library.Classes.Account.User.AddContactToMember(RDN.Library.Classes.Account.User.GetMemberId(), billingInfo, AddressTypeEnum.Billing);
                if (model.IsBillingDifferentFromShipping)
                {
                    var shippingInfo = new InvoiceContactInfo();
                    shippingInfo.City = model.ShippingAddress_City;
                    shippingInfo.Country = model.ShippingAddress_Country;
                    shippingInfo.Email = model.ShippingAddress_Email;
                    shippingInfo.FirstName = model.ShippingAddress_FirstName;
                    shippingInfo.LastName = model.ShippingAddress_LastName;
                    shippingInfo.Phone = model.ShippingAddress_Phone;
                    shippingInfo.State = model.ShippingAddress_State;
                    shippingInfo.Street = model.ShippingAddress_Street;
                    shippingInfo.Street2 = model.ShippingAddress_Street2;
                    shippingInfo.Zip = model.ShippingAddress_Zip;
                    invoice.SetInvoiceContactData(billingInfo, shippingInfo);

                    if (User.Identity.IsAuthenticated)
                        RDN.Library.Classes.Account.User.AddContactToMember(RDN.Library.Classes.Account.User.GetMemberId(), shippingInfo, AddressTypeEnum.Shipping);

                }
                else
                    invoice.SetInvoiceContactData(billingInfo, billingInfo);


                foreach (var cartitem in checkout.ShoppingCart.Stores.FirstOrDefault().StoreItems)
                {
                    var item = new InvoiceItem();
                    item.ArticleNumber = cartitem.ArticleNumber;
                    item.Article2Number = cartitem.Article2Number;
                    item.Description = "Tax Included in Price: " + cartitem.Description;
                    item.Name = cartitem.Name;
                    item.BasePrice = cartitem.BasePrice + cartitem.BaseTaxOnItem;
                    item.Price = cartitem.Price;
                    item.TotalShipping = cartitem.Shipping;
                    item.Quantity = cartitem.QuantityOrdered;
                    item.Weight = cartitem.Weight;
                    item.SizeOfItem = cartitem.ItemSizeEnum;
                    item.ColorOfItem = cartitem.ColorAGB;
                    item.StoreItemId = cartitem.StoreItemId;
                    invoice.AddItemTaxIncluded(item);
                }

                var requestResponse = invoice.FinalizeInvoice();
                if (requestResponse.Error != null)
                    throw new Exception(requestResponse.Error);

                this.ClearCart(HttpContext.Session);

                if (requestResponse.Status == InvoiceStatus.Payment_Successful)
                    return Redirect(Url.Content("~/receipt/" + requestResponse.InvoiceId.ToString().Replace("-", "")));
                else if (requestResponse.Status == InvoiceStatus.Pending_Payment_From_Paypal)
                    return Redirect(requestResponse.RedirectLink);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult StoreCheckOut(Guid merchantId)
        {
            var checkoutObject = new RDN.Store.Models.CheckOut();
            try
            {
                var shoppingCartId = StoreGateway.GetShoppingCartId(HttpContext);
                if (!shoppingCartId.HasValue)
                {
                    return Redirect(Url.Action("StoreCart"));

                }

                var sg = new StoreGateway();

                var checkout = sg.GetCheckoutData(shoppingCartId.Value, merchantId);
                if (checkout == null || checkout.ShoppingCart.ItemsCount == 0)
                {
                    return Redirect(Url.Action("StoreCart"));

                }
                Dictionary<int, string> countries = LocationFactory.GetCountriesDictionary();
                checkoutObject.Countries = countries.Select(item => new SelectListItem { Text = item.Value, Value = item.Key.ToString() }).ToList();
                checkoutObject.Years = EnumExt.ToSelectListId(YearsEnum.Fourteen);
                checkoutObject.Months = EnumExt.ToSelectListIdAndName(MonthsEnum.Jan);
                checkoutObject.MerchantName = checkout.MerchantName;
                checkoutObject.MerchantId = merchantId;
                checkoutObject.ShoppingCart = checkout.ShoppingCart;
                checkoutObject.Tax = checkout.Tax;
                checkoutObject.TaxRate = checkout.TaxRate;
                checkoutObject.TotalExclVat = checkout.TotalExclVat;
                checkoutObject.TotalInclVat = checkout.TotalInclVat;
                checkoutObject.Currency = checkout.Currency;
                checkoutObject.CurrencyCost = checkout.CurrencyCost;
                checkoutObject.TotalItemsCount = checkout.TotalItemsCount;
                checkoutObject.TotalShipping = checkout.TotalShipping;

                checkoutObject.AcceptPayPal = checkout.AcceptsPayPal;
                checkoutObject.AcceptStripe = checkout.AcceptsStripe;
                //need to turn stripe off if we are using another currency...
                if (checkout.Currency != "USD")
                    checkoutObject.AcceptStripe = false;

                var currencies = SiteCache.GetCurrencyExchanges();
                CurrencyConverter converter = new CurrencyConverter();
                converter.LoadCurrencies(currencies);

                foreach (var currency in currencies)
                {
                    if (currency.CurrencyAbbrName != checkoutObject.Currency)
                    {
                        checkoutObject.CurrenciesConverted.Add(currency.CurrencyAbbrName, converter.Convert(checkoutObject.Currency, currency.CurrencyAbbrName, checkoutObject.TotalInclVat));
                    }
                }

                var paymentProviders = Enum.GetNames(typeof(PaymentProvider));
                checkoutObject.PaymentProviders = paymentProviders.Select(x => new SelectListItem { Selected = false, Text = x, Value = x }).ToList();
                checkoutObject.PaymentProviders[0].Selected = true;
                //checkoutObject.ShippingAddress = new StoreShoppingCartContactInfo();
                //checkoutObject.BillingAddress = new StoreShoppingCartContactInfo();

                if (User.Identity.IsAuthenticated)
                {
                    var mem = RDN.Library.Cache.MemberCache.GetMemberDisplay(RDN.Library.Classes.Account.User.GetMemberId());
                    var billing = mem.ContactCard.Addresses.Where(x => x.Type == AddressTypeEnum.Billing).FirstOrDefault();
                    if (billing != null)
                    {
                        checkoutObject.BillingAddress_City = billing.CityRaw;
                        checkoutObject.BillingAddress_Country = billing.Country;
                        checkoutObject.BillingAddress_Email = mem.Email;
                        checkoutObject.BillingAddress_FirstName = mem.Firstname;
                        checkoutObject.BillingAddress_LastName = mem.LastName;
                        checkoutObject.BillingAddress_Phone = mem.PhoneNumber;
                        checkoutObject.BillingAddress_State = billing.StateRaw;
                        checkoutObject.BillingAddress_Street = billing.Address1;
                        checkoutObject.BillingAddress_Street2 = billing.Address2;
                        checkoutObject.BillingAddress_Zip = billing.Zip;
                    }
                    var shipping = mem.ContactCard.Addresses.Where(x => x.Type == AddressTypeEnum.Shipping).FirstOrDefault();
                    if (shipping != null)
                    {
                        checkoutObject.ShippingAddress_City = shipping.CityRaw;
                        checkoutObject.ShippingAddress_Country = shipping.Country;
                        checkoutObject.ShippingAddress_Email = mem.Email;
                        checkoutObject.ShippingAddress_FirstName = mem.Firstname;
                        checkoutObject.ShippingAddress_LastName = mem.LastName;
                        checkoutObject.ShippingAddress_Phone = mem.PhoneNumber;
                        checkoutObject.ShippingAddress_State = shipping.StateRaw;
                        checkoutObject.ShippingAddress_Street = shipping.Address1;
                        checkoutObject.ShippingAddress_Street2 = shipping.Address2;
                        checkoutObject.ShippingAddress_Zip = shipping.Zip;
                    }
                }

                //#if DEBUG
                //checkoutObject.StripeKey = "Stripe.setPublishableKey('" + ServerConfig.STRIPE_DEBUG_KEY + "');";
                //#else
                if (!String.IsNullOrEmpty(checkout.StripePublishableKey))
                    checkoutObject.StripeKey = "Stripe.setPublishableKey('" + checkout.StripePublishableKey + "');";
                else
                    checkoutObject.StripeKey = "Stripe.setPublishableKey('" + ServerConfig.STRIPE_LIVE_KEY + "');";
                //#endif
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(checkoutObject);
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult StoreCart()
        {
            try
            {
                StoreGateway sg = new StoreGateway();

                var shoppingCartId = StoreGateway.GetShoppingCartId(HttpContext);
                if (shoppingCartId != null)
                {
                    StoreShoppingCart cart = sg.GetShoppingCart(shoppingCartId.Value);
                    return View(cart);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(new StoreShoppingCart());
        }


        public ActionResult UpdateStoreQuantityItem(string storeCartItemId, string merchantId, string cartId, string quantity)
        {
            try
            {
                StoreGateway sg = new StoreGateway();
                int quan = 0;
                if (!String.IsNullOrEmpty(quantity))
                    quan = Convert.ToInt32(quantity);
                bool success = sg.UpdateStoreQuantityItem(Convert.ToInt32(storeCartItemId), new Guid(cartId), quan);
                StoreShoppingCart cart = sg.GetShoppingCart(new Guid(cartId));

                var store = cart.Stores.Where(x => x.MerchantId == new Guid(merchantId)).FirstOrDefault();
                decimal itemTotals = 0.0M;
                decimal itemShipping = 0.0M;
                decimal totalAfterShipping = 0.0M;
                decimal price = 0.0M;
                if (store != null)
                {
                    itemTotals = store.TotalPrice;
                    itemShipping = store.TotalShipping;
                    totalAfterShipping = store.TotalAfterShipping;

                    price = store.StoreItems.Where(x => x.ShoppingCartItemId == Convert.ToInt32(storeCartItemId)).FirstOrDefault().Price;
                }

                this.AddItemToCart(HttpContext.Session, Convert.ToInt32(quantity));
                return Json(new { IsSuccessful = true, itemPrice = "$" + price.ToString("N2"), subtotal = "$" + itemTotals.ToString("N2"), shipping = "$" + itemShipping.ToString("N2"), afterShipping = "$" + totalAfterShipping.ToString("N2") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: quantity);
            } return Json(new { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteItemFromCart(string storeItemCartId, string merchantId, string cartId)
        {
            try
            {
                StoreGateway sg = new StoreGateway();
                bool success = sg.DeleteItemFromCart(Convert.ToInt32(storeItemCartId), new Guid(cartId));

                StoreShoppingCart cart = sg.GetShoppingCart(new Guid(cartId));
                var store = cart.Stores.Where(x => x.MerchantId == new Guid(merchantId)).FirstOrDefault();
                decimal itemTotals = 0.0M;
                decimal itemShipping = 0.0M;
                decimal totalAfterShipping = 0.0M;
                if (store != null)
                {
                    itemTotals = store.TotalPrice;
                    itemShipping = store.TotalShipping;
                    totalAfterShipping = store.TotalAfterShipping;
                }
                this.AddItemToCart(HttpContext.Session, -1);

                return Json(new { IsSuccessful = true, subtotal = "$" + itemTotals.ToString("N2"), shipping = "$" + itemShipping.ToString("N2"), afterShipping = "$" + totalAfterShipping.ToString("N2") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            } return Json(new { IsSuccessful = false }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ToggleShipment(string storeItemCartId, string merchantId, string cartId)
        {
            try
            {
                StoreGateway sg = new StoreGateway();
                bool success = sg.ToggleShipment(Convert.ToInt32(storeItemCartId), new Guid(cartId));

                StoreShoppingCart cart = sg.GetShoppingCart(new Guid(cartId));

                var store = cart.Stores.Where(x => x.MerchantId == new Guid(merchantId)).FirstOrDefault();
                decimal itemTotals = 0.0M;
                decimal itemShipping = 0.0M;
                decimal totalAfterShipping = 0.0M;
                decimal price = 0.0M;
                decimal shippingTemp = 0.0M;
                if (store != null)
                {
                    itemTotals = store.TotalPrice;
                    itemShipping = store.TotalShipping;
                    totalAfterShipping = store.TotalAfterShipping;

                    var item = store.StoreItems.Where(x => x.ShoppingCartItemId == Convert.ToInt32(storeItemCartId)).FirstOrDefault();
                    price = item.Price;
                    shippingTemp = item.Shipping;
                }

                return Json(new { IsSuccessful = true, itemPrice = "$" + price.ToString("N2") + " + $" + shippingTemp.ToString("N2"), subtotal = "$" + itemTotals.ToString("N2"), shipping = "$" + itemShipping.ToString("N2"), afterShipping = "$" + totalAfterShipping.ToString("N2") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            } return Json(new { IsSuccessful = false }, JsonRequestBehavior.AllowGet);

        }


    }
}
