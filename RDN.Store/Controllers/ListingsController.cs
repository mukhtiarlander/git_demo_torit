using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Store;
using RDN.Library.Classes.Store.Classes;
using RDN.Portable.Classes.Store.Enums;
using RDN.Library.Classes.Config;

namespace RDN.Store.Controllers
{
    public class ListingsController : BaseController
    {
        [HttpPost]
        public ActionResult AddToCart(StoreItem storeItem)
        {
            try
            {
                var sg = new StoreGateway();
                var shoppingCartId = StoreGateway.GetShoppingCartId(HttpContext);
                StoreShoppingCart cart = null;

                if (shoppingCartId == null)
                {
                    cart = sg.CreateNewShoppingCart(storeItem.Store.MerchantId, RDN.Library.Classes.Account.User.GetUserId(), true, Request.UserHostAddress);
                    StoreGateway.SetShoppingCartSession(cart.ShoppingCartId, HttpContext);
                }
                else
                    cart = sg.GetShoppingCart(shoppingCartId.Value);

                int quantity = Convert.ToInt32(Request.Form["quantityToBuy"]);

                sg.AddItemToCart(cart.ShoppingCartId, storeItem.Store.MerchantId, storeItem.StoreItemId, quantity, storeItem.ItemSizeEnum, storeItem.ColorTempSelected);
                this.AddItemToCart(HttpContext.Session, quantity);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/"+LibraryConfig.SportNameForUrl+"-item/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(storeItem.Name) + "/" + storeItem.StoreItemId));

        }

        public ActionResult ViewListing(string name, string id)
        {
            StoreItem item = null;
            try
            {
                StoreGateway sg = new StoreGateway();
                item = sg.GetStoreItem(Convert.ToInt32(id), true);
                item.Note = RDN.Utilities.Strings.StringExt.ConvertTextAreaToHtml(item.Note);
                Dictionary<String, Int32> sizes = new Dictionary<string, int>();
                if (item.ItemSize.HasFlag(StoreItemShirtSizesEnum.X_Small))
                {
                    sizes.Add(RDN.Utilities.Enums.EnumExt.ToFreindlyName(StoreItemShirtSizesEnum.X_Small), Convert.ToInt32(StoreItemShirtSizesEnum.X_Small));
                }
                if (item.ItemSize.HasFlag(StoreItemShirtSizesEnum.Small))
                {
                    sizes.Add(RDN.Utilities.Enums.EnumExt.ToFreindlyName(StoreItemShirtSizesEnum.Small), Convert.ToInt32(StoreItemShirtSizesEnum.Small));
                }
                if (item.ItemSize.HasFlag(StoreItemShirtSizesEnum.Medium))
                {
                    sizes.Add(RDN.Utilities.Enums.EnumExt.ToFreindlyName(StoreItemShirtSizesEnum.Medium), Convert.ToInt32(StoreItemShirtSizesEnum.Medium));
                }
                if (item.ItemSize.HasFlag(StoreItemShirtSizesEnum.Large))
                {
                    sizes.Add(RDN.Utilities.Enums.EnumExt.ToFreindlyName(StoreItemShirtSizesEnum.Large), Convert.ToInt32(StoreItemShirtSizesEnum.Large));
                }
                if (item.ItemSize.HasFlag(StoreItemShirtSizesEnum.X_Large))
                {
                    sizes.Add(RDN.Utilities.Enums.EnumExt.ToFreindlyName(StoreItemShirtSizesEnum.X_Large), Convert.ToInt32(StoreItemShirtSizesEnum.X_Large));
                }
                if (item.ItemSize.HasFlag(StoreItemShirtSizesEnum.XX_Large))
                {
                    sizes.Add(RDN.Utilities.Enums.EnumExt.ToFreindlyName(StoreItemShirtSizesEnum.XX_Large), Convert.ToInt32(StoreItemShirtSizesEnum.XX_Large));
                }
                ViewBag.ItemSizes = new SelectList(sizes, "value", "key");
                ViewBag.Colors = new SelectList(item.Colors, "HexColor", "NameOfColor");
                var shoppingCartId = StoreGateway.GetShoppingCartId(HttpContext);
                if (shoppingCartId != null)
                {
                    StoreShoppingCart cart = sg.GetShoppingCart(shoppingCartId.Value);
                    item.CartItemsCount = cart.ItemsCount;
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(item);
        }

    }
}
