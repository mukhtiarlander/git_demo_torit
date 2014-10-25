using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Classes.Invoice;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Store;
using RDN.Library.Classes.Store.Classes;
using RDN.Library.Classes.Store.Display;
using RDN.Library.DataModels.Store;
using CheckOut = RDN.Raspberry.Models.Store.CheckOut;

namespace RDN.Raspberry.Controllers
{
    public class StoreController : Controller
    {

        public ActionResult ViewRDNStore()
        {
            var cart = GetCartForRdnStore();
            var store = GetStoreForRdn();

            var displayStore = new DisplayStore();
            displayStore.ShoppingCart = cart;
            displayStore.ShopName = store.Name;
            //displayStore.StoreItems = store.StoreItems;
            displayStore.MerchantId = store.MerchantId;

            return View(displayStore);
        }

        public void RemoveItemFromCart()
        {
            var shoppingCartId = GetShoppingCartId();
            if (!shoppingCartId.HasValue)
                Response.Redirect(Url.Action("ViewRDNStore"));

            var cartItemIdRaw = Request.Form["shoppingcartitem"];

            int cartItemId;
            if (!Int32.TryParse(cartItemIdRaw, out cartItemId))
                Response.Redirect(Url.Action("ViewRDNStore"));

            var sg = new StoreGateway();
            sg.RemoveItemFromCart(shoppingCartId.Value, cartItemId);
            Response.Redirect(Url.Action("ViewRDNStore"));
        }

        public void AddItemToCart()
        {
            var shoppingCartId = GetShoppingCartId();
            if (!shoppingCartId.HasValue)
                Response.Redirect(Url.Action("ViewRDNStore"));

            var merchantIdRaw = Request.Form["merchantid"];
            var articleIdRaw = Request.Form["articleid"];
            var quantityRaw = Request.Form["quantity"];

            Guid merchantId;
            if (!Guid.TryParse(merchantIdRaw, out merchantId))
                Response.Redirect(Url.Action("ViewRDNStore"));

            int itemId, quantity;
            if (!Int32.TryParse(articleIdRaw, out itemId))
                Response.Redirect(Url.Action("ViewRDNStore"));
            if (!Int32.TryParse(quantityRaw, out quantity))
                Response.Redirect(Url.Action("ViewRDNStore"));

            var sg = new StoreGateway();
            sg.AddItemToCart(shoppingCartId.Value, merchantId, itemId, quantity, 0, null);
            Response.Redirect(Url.Action("ViewRDNStore"));
        }

        public ActionResult CheckOut()
        {
            var shoppingCartId = GetShoppingCartId();
            if (!shoppingCartId.HasValue)
            {
                Response.Redirect(Url.Action("ViewRDNStore"));
                return null;
            }

            var sg = new StoreGateway();
            var checkout = sg.GetCheckoutData(shoppingCartId.Value, new Guid());
            if (checkout == null || checkout.ShoppingCart.Stores.Count == 0)
            {
                Response.Redirect(Url.Action("ViewRDNStore"));
                return null;
            }

            var checkoutObject = new CheckOut();
            checkoutObject.MerchantId = checkout.MerchantId;
            checkoutObject.ShoppingCart = checkout.ShoppingCart;
            checkoutObject.Tax = checkout.Tax;
            checkoutObject.TaxRate = checkout.TaxRate;
            checkoutObject.TotalExclVat = checkout.TotalExclVat;
            checkoutObject.TotalInclVat = checkout.TotalInclVat;
            checkoutObject.Currency = checkout.Currency;
        
                checkoutObject.Currency =checkout.Currency;
                checkoutObject.CurrencyCost =checkout.CurrencyCost;
        
            checkoutObject.ShippingOptions =
                checkout.ShippingOptionsRaw.Select(
                    x => new SelectListItem
                    {
                        Selected = false,
                        Text = string.Format("{0} {1} {2}", x.Value.Name.ToString(), x.Value.Price, checkoutObject.Currency),
                        Value = x.Key.ToString()
                    }).ToList();
            checkoutObject.ShippingOptions[0].Selected = true;

            var paymentProviders = Enum.GetNames(typeof(PaymentProvider));
            checkoutObject.PaymentProviders = paymentProviders.Select(x => new SelectListItem { Selected = false, Text = x, Value = x }).ToList();
            checkoutObject.PaymentProviders[0].Selected = true;
            checkout.ShippingAddress = new StoreShoppingCartContactInfo();
            checkout.BillingAddress = new StoreShoppingCartContactInfo();
            return View(checkoutObject);
        }

        [HttpPost]
        public void CheckOut(CheckOut model)
        {
            var shoppingCartId = GetShoppingCartId();
            if (!shoppingCartId.HasValue)
            {
                Response.Redirect(Url.Action("ViewRDNStore"));
                return;
            }

            var sg = new StoreGateway();
            var checkout = sg.GetCheckoutData(shoppingCartId.Value, new Guid());
            if (checkout == null || checkout.ShoppingCart.ItemsCount == 0)
            {
                Response.Redirect(Url.Action("ViewRDNStore"));
                return;
            }

            PaymentProvider paymentProvider;
            if (!Enum.TryParse(model.PaymentProviderId, out paymentProvider))
            {
                Response.Redirect(Url.Action("ViewRDNStore"));
                return;
            }
            int shippingId;
            if (!Int32.TryParse(model.ShippingOptionSelectedId, out shippingId))
            {
                Response.Redirect(Url.Action("ViewRDNStore"));
                return;
            }

            var pg = new PaymentGateway();

            var invoice = pg.StartInvoiceWizard()
                .Initalize(checkout.MerchantId, checkout.Currency, paymentProvider, PaymentMode.Live, ChargeTypeEnum.InvoiceItem)
                .SetShipping(checkout.TotalShipping, ShippingType.Postal)
                .SetInvoiceId(checkout.ShoppingCart.ShoppingCartId);

            var billingInfo = new InvoiceContactInfo();
            billingInfo.City = model.BillingAddress.City;
            billingInfo.CompanyName = model.BillingAddress.CompanyName;
            billingInfo.Country = model.BillingAddress.Country;
            billingInfo.Email = model.BillingAddress.Email;
            billingInfo.Fax = model.BillingAddress.Fax;
            billingInfo.FirstName = model.BillingAddress.FirstName;
            billingInfo.LastName = model.BillingAddress.LastName;
            billingInfo.Phone = model.BillingAddress.Phone;
            billingInfo.State = model.BillingAddress.State;
            billingInfo.Street = model.BillingAddress.Street;
            billingInfo.Street2 = model.BillingAddress.Street2;
            billingInfo.Zip = model.BillingAddress.Zip;

            var shippingInfo = new InvoiceContactInfo();
            shippingInfo.City = model.ShippingAddress.City;
            shippingInfo.CompanyName = model.ShippingAddress.CompanyName;
            shippingInfo.Country = model.ShippingAddress.Country;
            shippingInfo.Email = model.ShippingAddress.Email;
            shippingInfo.Fax = model.ShippingAddress.Fax;
            shippingInfo.FirstName = model.ShippingAddress.FirstName;
            shippingInfo.LastName = model.ShippingAddress.LastName;
            shippingInfo.Phone = model.ShippingAddress.Phone;
            shippingInfo.State = model.ShippingAddress.State;
            shippingInfo.Street = model.ShippingAddress.Street;
            shippingInfo.Street2 = model.ShippingAddress.Street2;
            shippingInfo.Zip = model.ShippingAddress.Zip;
            invoice.SetInvoiceContactData(billingInfo, shippingInfo);

            //foreach (var cartitem in checkout.ShoppingCart.Items)
            //{
            //    var item = new InvoiceItem();
            //    item.ArticleNumber = cartitem.ArticleNumber;
            //    item.Article2Number = cartitem.Article2Number;
            //    item.Description = cartitem.Description;
            //    item.Name = cartitem.Name;
            //    item.BasePrice = cartitem.BasePrice;
            //    item.Price = cartitem.Price;
            //    item.Quantity = cartitem.QuantityOrdered;
            //    item.Weight = cartitem.Weight;
            //    invoice.AddItem(item);
            //}

            invoice.SetTax(checkout.TaxRate);

            var requestResponse = invoice.FinalizeInvoice();
            if (requestResponse.Error != null)
                throw new Exception(requestResponse.Error);

            Response.Redirect(requestResponse.RedirectLink);
        }

        // ToDo: Add code 
        private StoreShoppingCart GetCartForStore(Guid merchantId)
        {
            return null;
        }

        private StoreShoppingCart GetCartForRdnStore()
        {
            var shoppingCartId = GetShoppingCartId();
            StoreShoppingCart cart = null;
            var sg = new StoreGateway();
            if (shoppingCartId == null)
            {
                cart = sg.CreateNewShoppingCart(null, RDN.Library.Classes.Account.User.GetUserId(), true, Request.UserHostAddress);
                Session.Add("ShoppingCartId", cart.ShoppingCartId);
            }
            else
                cart = sg.GetShoppingCart(shoppingCartId.Value);
            return cart;
        }

        private Store GetStoreForRdn()
        {
            var sg = new StoreGateway();
            return sg.GetStore(null, true);
        }

        /// <summary>
        /// Get the shopping cart id from the session
        /// </summary>
        /// <returns></returns>
        private Guid? GetShoppingCartId()
        {
            Guid? shoppingCartId = null;
            if (Session["ShoppingCartId"] != null)
            {
                Guid cartId;
                if (Guid.TryParse(Session["ShoppingCartId"].ToString(), out cartId))
                    shoppingCartId = cartId;
            }
            return shoppingCartId;
        }
    }
}
