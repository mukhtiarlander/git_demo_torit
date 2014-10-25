using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Store.Classes;
using RDN.Library.Classes.Store.Display;
using RDN.Utilities.Config;
using RDN.Portable.Config;
using RDN.Portable.Models.Json.Calendar;
using RDN.Portable.Models.Json.Shop;


namespace RDN.Api.Controllers
{
    public class ShopsController : Controller
    {


        public JsonResult GetAllMerch(int p, int c)
        {
            List<StoreItem> names = SiteCache.GetPublishedStoreItems(c, p);
            ShopsJson shop = new ShopsJson();
            shop.Items = new List<ShopItemJson>();
            shop.Count = c;
            shop.Page = p;
            for (int i = 0; i < names.Count; i++)
            {
                ShopItemJson j = new ShopItemJson();

                j.Description = names[i].Description;
                j.Name = names[i].Name;
                j.AcceptsPayPal = names[i].AcceptsPayPal;
                j.AcceptsStripe = names[i].AcceptsStripe;
                j.CanRunOutOfStock = names[i].CanRunOutOfStock;
                for (int js = 0; js < names[i].Colors.Count; js++)
                {
                    j.Colors.Add(names[i].Colors[js].HexColor);
                }

                j.IsPublished = names[i].IsPublished;
                j.ItemSize = names[i].ItemSize;
                j.ItemType = names[i].ItemType;
                j.Notes = RDN.Utilities.Strings.StringExt.ConvertTextAreaToHtml(names[i].Note);
                for (int js = 0; js < names[i].Photos.Count; js++)
                {
                    j.PhotoUrls.Add(names[i].Photos[js].ImageUrl);
                    j.PhotoUrlsThumbs.Add(names[i].Photos[js].ImageThumbUrl);
                }
                j.Shipping = names[i].Shipping;
                j.Price = names[i].Price;
                j.QuantityInStock = names[i].QuantityInStock;
                j.SoldBy = names[i].Store.Name;
                j.SoldById = names[i].Store.MerchantId.ToString().Replace("-", "");
                j.StoreItemId = names[i].StoreItemId;
                j.Views = names[i].Views;
                j.RDNUrl = ServerConfig.WEBSITE_STORE_DEFAULT_LOCATION + "/roller-derby-item/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(j.Name) + "/" + j.StoreItemId;
                shop.Items.Add(j);
            }

            return Json(shop, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SearchAllMerch(int p, int c, string s)
        {
            List<StoreItem> names = SiteCache.SearchPublishedStoreItems(c, p, s);
            ShopsJson shop = new ShopsJson();
            shop.Items = new List<ShopItemJson>();
            shop.Count = c;
            shop.Page = p;
            for (int i = 0; i < names.Count; i++)
            {
                ShopItemJson j = new ShopItemJson();

                j.Description = names[i].Description;
                j.Name = names[i].Name;
                j.AcceptsPayPal = names[i].AcceptsPayPal;
                j.AcceptsStripe = names[i].AcceptsStripe;
                j.CanRunOutOfStock = names[i].CanRunOutOfStock;
                for (int js = 0; js < names[i].Colors.Count; js++)
                {
                    j.Colors.Add(names[i].Colors[js].HexColor);
                }

                j.IsPublished = names[i].IsPublished;
                j.ItemSize = names[i].ItemSize;
                j.ItemType = names[i].ItemType;
                j.Notes = RDN.Utilities.Strings.StringExt.ConvertTextAreaToHtml(names[i].Note);

                for (int js = 0; js < names[i].Photos.Count; js++)
                {
                    j.PhotoUrls.Add(names[i].Photos[js].ImageUrl);
                    j.PhotoUrlsThumbs.Add(names[i].Photos[js].ImageThumbUrl);
                }
                j.Shipping = names[i].Shipping;
                j.Price = names[i].Price;
                j.QuantityInStock = names[i].QuantityInStock;
                j.SoldBy = names[i].Store.Name;
                j.SoldById = names[i].Store.MerchantId.ToString().Replace("-", "");
                j.StoreItemId = names[i].StoreItemId;
                j.Views = names[i].Views;
                j.RDNUrl = ServerConfig.WEBSITE_STORE_DEFAULT_LOCATION + "/roller-derby-item/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(j.Name) + "/" + j.StoreItemId;
                shop.Items.Add(j);
            }

            return Json(shop, JsonRequestBehavior.AllowGet);
        }

    }
}
