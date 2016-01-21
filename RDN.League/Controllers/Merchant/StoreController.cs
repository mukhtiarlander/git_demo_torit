using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Store.Display;
using RDN.Library.Classes.Store;
using RDN.Library.Cache;
using RDN.Library.Classes.Store.Classes;
using RDN.League.Models.Store;
using RDN.Library.Classes.Error;
using RDN.Utilities.Config;
using RDN.Library.Classes.Payment.Enums.Stripe;
using RDN.League.Models.Utilities;
using RDN.League.Models.Enum;
using System.Collections.Specialized;
using RDN.Library.Util.Enum;
using RDN.Library.Util;
using RDN.Library.Classes.Payment.Classes.Display;
using RDN.League.Classes.Enums;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Colors;
using RDN.Portable.Config;
using RDN.Portable.Classes.Store.Enums;
using RDN.Portable.Classes.Payment.Enums;
using RDN.Library.Classes.Config;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class StoreController : BaseController
    {
        public JsonResult MoveInvoiceToNextStatus(string invoiceId, string status)
        {
            try
            {
                status = status.Replace("Move To", "").Trim();
                PaymentGateway pg = new PaymentGateway();
                string s = status.Replace(" ", "_");
                InvoiceStatus st = (InvoiceStatus)Enum.Parse(typeof(InvoiceStatus), s);
                pg.SetInvoiceStatus(new Guid(invoiceId), st);
                if (st == InvoiceStatus.Shipped)
                {
                    var voice = pg.GetDisplayInvoice(new Guid(invoiceId));
                    StoreGateway sg = new StoreGateway();
                    sg.CompileAndSendShippedEmailsForStore(voice);
                }
                return Json(new { isSuccess = true, status = st.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// deletes the store item.
        /// </summary>
        /// <param name="pictureId"></param>
        /// <param name="storeItemId"></param>
        /// <param name="mId"></param>
        /// <returns></returns>
        public JsonResult DeleteStoreItemPicture(string pictureId, string storeItemId, string mId)
        {
            try
            {
                StoreGateway sg = new StoreGateway();
                bool isSuccess = sg.DeleteStoreItemPhoto(Convert.ToInt32(pictureId), Convert.ToInt32(storeItemId), new Guid(mId));
                return Json(new { result = isSuccess }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// uploads the item picture
        /// </summary>
        /// <param name="display"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult UploadItemPictures(StoreItemDisplayModel display)
        {
            try
            {
                foreach (string pictureFile in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[pictureFile];
                    if (file.ContentLength > 0)
                    {
                        StoreGateway sg = new StoreGateway();
                        sg.AddStoreItemPhoto(display.StoreItemId, file.InputStream, file.FileName);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/store/item/edit/" + display.StoreItemId + "/" + display.PrivateManagerId.ToString().Replace("-", "") + "/" + display.MerchantId.ToString().Replace("-", "")));

        }
        /// <summary>
        /// posts and saves the store settings changes.
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult StoreSettings(DisplayStoreModel store)
        {
            try
            {
                ViewBag.IsSuccessful = false;
                var sg = new StoreGateway();
                ViewBag.IsSuccessful = sg.UpdateStoreSettings(store);

                return Redirect(Url.Content("~/store/settings/" + store.PrivateManagerId.ToString().Replace("-", "") + "/" + store.MerchantId.ToString().Replace("-", "")));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        /// <summary>
        /// shows the store settings view.
        /// </summary>
        /// <param name="privId"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult StoreSettings(Guid privId, Guid storeId)
        {
            DisplayStoreModel displayStore = null;
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sced.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Error;
                    message.Message = "You did not Connect to Stripe. In order to Accept Customer Credit Cards, you should connect to Stripe.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sca.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Stripe Connected Successfully.";
                    this.AddMessage(message);
                }

                ViewBag.IsSuccessful = false;
                var sg = new StoreGateway();
                var realStore = sg.GetStoreSettings(storeId, privId);
                string stripe = "https://connect.stripe.com/oauth/authorize?response_type=code&client_id=" + LibraryConfig.StripeConnectKey+ "&scope=read_write&state=" + StripeStateReturnCodeEnum.store + "-" + privId.ToString().Replace("-", "");
                


                displayStore = new DisplayStoreModel( realStore);
                displayStore.CurrencyList = new SelectList(SiteCache.GetCurrencyExchanges(), "CurrencyAbbrName", "CurrencyNameDisplay", "USD");
                ViewBag.StripeUrl = stripe;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(displayStore);

        }
        [Authorize]
        public ActionResult StoreOrders(Guid privId, Guid storeId)
        {
            DisplayStore displayStore = null;
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sced.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Error;
                    message.Message = "You did not Connect to Stripe. In order to Accept Customer Credit Cards, you should connect to Stripe.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sca.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Stripe Connected Successfully.";
                    this.AddMessage(message);
                }

                ViewBag.IsSuccessful = false;
                var sg = new StoreGateway();
                var realStore = sg.GetStoreForManager(storeId, privId, false);

                displayStore = realStore;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(displayStore);

        }
        [Authorize]
        public ActionResult StoreOrder(Guid privId, Guid storeId, Guid invoiceId)
        {
            DisplayInvoice invoice = null;
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Updated.";
                    this.AddMessage(message);
                }

                var sg = new StoreGateway();
                invoice = sg.GetInvoiceForManager(storeId, privId, invoiceId);
                if (invoice.InvoiceStatus == InvoiceStatus.Awaiting_Shipping)
                    ViewData["invoiceStatus"] = "Move To " + InvoiceStatus.Shipped;
                else if (invoice.InvoiceStatus == InvoiceStatus.Shipped)
                    ViewData["invoiceStatus"] = "Move To " + InvoiceStatus.Archived_Item_Completed;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(invoice);
        }
        [HttpPost]
        [Authorize]
        public ActionResult StoreOrder(DisplayInvoice invoice)
        {
            try
            {
                var sg = new StoreGateway();
                sg.UpdateInvoiceForStoreOrder(invoice);
                return Redirect(Url.Content("~/store/order/" + invoice.Merchant.PrivateManagerId.ToString().Replace("-", "") + "/" + invoice.Merchant.MerchantId.ToString().Replace("-", "") + "/" + invoice.InvoiceId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }



        /// <summary>
        /// posts the edit items from the store
        /// </summary>
        /// <param name="storeItem"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult StoreEditItem(StoreItemDisplayModel storeItem)
        {
            var sg = new StoreGateway();

            try
            {
                StoreItemDisplay display = new StoreItemDisplay();
                display.CanPickUpLocally = storeItem.CanPickUpLocally;
                display.ArticleNumber = storeItem.ArticleNumber;
                display.CanRunOutOfStock = storeItem.CanRunOutOfStock;
                display.Currency = storeItem.Currency;
                display.Description = storeItem.Description;
                display.InternalId = storeItem.InternalId;
                display.Note = storeItem.HtmlNote;
                display.Price = storeItem.Price;
                display.PrivateManagerId = storeItem.PrivateManagerId;
                display.QuantityInStock = storeItem.QuantityInStock;
                display.StoreItemId = storeItem.StoreItemId;
                display.Merchant.Name = storeItem.StoreName;
                display.Weight = storeItem.Weight;
                display.MerchantId = storeItem.MerchantId;
                display.Name = storeItem.Name;
                display.Shipping = storeItem.Shipping;
                display.ShippingAdditional = storeItem.ShippingAdditional;
                display.IsPublished = storeItem.IsPublished;
                display.HasExtraLarge = storeItem.HasExtraLarge;
                display.HasExtraSmall = storeItem.HasExtraSmall;
                display.HasLarge = storeItem.HasLarge;
                display.HasMedium = storeItem.HasMedium;
                display.HasSmall = storeItem.HasSmall;
                display.HasXXLarge = storeItem.HasXXLarge;
                display.HasXXXLarge= storeItem.HasXXXLarge;
                display.ItemTypeEnum = (int)storeItem.ItemType;
                display.ColorTempSelected = storeItem.ColorsSelected;

                display = sg.UpdateStoreItem(display);
                if (display.StoreItemId > 0)
                    return Redirect(Url.Content("~/store/item/edit/" + display.StoreItemId + "/" + display.PrivateManagerId.ToString().Replace("-", "") + "/" + display.MerchantId.ToString().Replace("-", "")));

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            var store = sg.GetStoreForManager(storeItem.MerchantId, storeItem.PrivateManagerId, false);
            storeItem.InternalId = store.InternalReference;
            storeItem.MerchantId = store.MerchantId;
            storeItem.PrivateManagerId = store.PrivateManagerId;
            return View(storeItem);
        }
        [Authorize]
        public ActionResult StoreEditItem(int itemId, Guid privId, Guid storeId)
        {
            StoreItemDisplayModel model = new StoreItemDisplayModel();
            try
            {
                var sg = new StoreGateway();
                var store = sg.GetStoreItemManager(storeId, privId, itemId);
                model.ArticleNumber = store.ArticleNumber;
                model.CanRunOutOfStock = store.CanRunOutOfStock;
                model.CanPickUpLocally = store.CanPickUpLocally;
                model.Currency = store.Currency;
                model.Description = store.Description;
                model.HtmlNote = store.Note;
                model.InternalId = store.InternalId;
                model.MerchantId = store.MerchantId;
                model.Name = store.Name;
                model.Price = store.Price;
                model.Shipping = store.Shipping;
                model.ShippingAdditional = store.ShippingAdditional;
                model.PrivateManagerId = store.PrivateManagerId;
                model.QuantityInStock = store.QuantityInStock;
                model.StoreItemId = store.StoreItemId;
                model.StoreName = store.Merchant.Name;
                model.Weight = store.Weight;
                model.Photos = store.Photos;
                model.IsPublished = store.IsPublished;
                model.ItemSizeEnum = store.ItemSizeEnum;
                model.ItemTypeEnum = store.ItemTypeEnum;
                model.ItemType = store.ItemType;
                model.ItemSize = store.ItemSize;
                model.ColorsSelected = store.ColorTempSelected;
                model.Colors = store.Colors;
                store.ColorTempSelected = String.Empty;
                StoreItemTypeEnum[] notInList = { StoreItemTypeEnum.None };
                model.ItemTypeSelectList = store.ItemType.ToSelectList(notInList);
                var colors = ColorDisplayFactory.GetOwnerColors(storeId);
                model.ColorList = new SelectList(colors, "HexColor", "NameOfColor");

                if (model.ItemType == StoreItemTypeEnum.Shirt)
                {
                    Dictionary<String, Int32> sizes = new Dictionary<string, int>();
                    if (model.ItemSize.HasFlag(StoreItemShirtSizesEnum.Large))
                    {
                        sizes.Add(RDN.Portable.Util.Enums.EnumExt.ToFreindlyName(StoreItemShirtSizesEnum.Large), Convert.ToInt32(StoreItemShirtSizesEnum.Large));
                        model.HasLarge = true;
                    }
                    if (model.ItemSize.HasFlag(StoreItemShirtSizesEnum.Medium))
                    {
                        sizes.Add(RDN.Portable.Util.Enums.EnumExt.ToFreindlyName(StoreItemShirtSizesEnum.Medium), Convert.ToInt32(StoreItemShirtSizesEnum.Medium));
                        model.HasMedium = true;
                    }
                    if (model.ItemSize.HasFlag(StoreItemShirtSizesEnum.Small))
                    {
                        sizes.Add(RDN.Portable.Util.Enums.EnumExt.ToFreindlyName(StoreItemShirtSizesEnum.Small), Convert.ToInt32(StoreItemShirtSizesEnum.Small));
                        model.HasSmall = true;
                    }
                    if (model.ItemSize.HasFlag(StoreItemShirtSizesEnum.X_Large))
                    {
                        sizes.Add(RDN.Portable.Util.Enums.EnumExt.ToFreindlyName(StoreItemShirtSizesEnum.X_Large), Convert.ToInt32(StoreItemShirtSizesEnum.X_Large));
                        model.HasExtraLarge = true;
                    }
                    if (model.ItemSize.HasFlag(StoreItemShirtSizesEnum.X_Small))
                    {
                        sizes.Add(RDN.Portable.Util.Enums.EnumExt.ToFreindlyName(StoreItemShirtSizesEnum.X_Small), Convert.ToInt32(StoreItemShirtSizesEnum.X_Small));
                        model.HasExtraSmall = true;
                    }
                    if (model.ItemSize.HasFlag(StoreItemShirtSizesEnum.XX_Large))
                    {
                        sizes.Add(RDN.Portable.Util.Enums.EnumExt.ToFreindlyName(StoreItemShirtSizesEnum.XX_Large), Convert.ToInt32(StoreItemShirtSizesEnum.XX_Large));
                        model.HasXXLarge = true;
                    }
                    if (model.ItemSize.HasFlag(StoreItemShirtSizesEnum.XXX_Large))
                    {
                        sizes.Add(RDN.Portable.Util.Enums.EnumExt.ToFreindlyName(StoreItemShirtSizesEnum.XXX_Large), Convert.ToInt32(StoreItemShirtSizesEnum.XXX_Large));
                        model.HasXXXLarge = true;
                    }

                    model.ItemSizes = new SelectList(sizes);
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public ActionResult StoreNewItem(StoreItemDisplayModel storeItem)
        {
            var sg = new StoreGateway();
            try
            {
                StoreItemDisplay display = new StoreItemDisplay();
                display.CanPickUpLocally = storeItem.CanPickUpLocally;
                display.ArticleNumber = storeItem.ArticleNumber;
                display.CanRunOutOfStock = storeItem.CanRunOutOfStock;
                display.Currency = storeItem.Currency;
                display.Description = storeItem.Description;
                display.InternalId = storeItem.InternalId;
                display.Note = storeItem.HtmlNote;
                display.Price = storeItem.Price;
                display.PrivateManagerId = storeItem.PrivateManagerId;
                display.QuantityInStock = storeItem.QuantityInStock;
                display.StoreItemId = storeItem.StoreItemId;
                display.Merchant.Name = storeItem.StoreName;
                display.Weight = storeItem.Weight;
                display.MerchantId = storeItem.MerchantId;
                display.Name = storeItem.Name;
                display.Shipping = storeItem.Shipping;
                display.HasExtraLarge = storeItem.HasExtraLarge;
                display.HasExtraSmall = storeItem.HasExtraSmall;
                display.HasLarge = storeItem.HasLarge;
                display.HasMedium = storeItem.HasMedium;
                display.HasSmall = storeItem.HasSmall;
                display.HasXXLarge = storeItem.HasXXLarge;
                display.HasXXXLarge = storeItem.HasXXXLarge;
                display.ItemTypeEnum = storeItem.ItemTypeEnum;
                display.ItemType = storeItem.ItemType;
                display.ColorTempSelected = storeItem.ColorsSelected;

                display = sg.CreateNewStoreItem(display);
                if (display.StoreItemId > 0)
                    return Redirect(Url.Content("~/store/item/edit/" + display.StoreItemId + "/" + display.PrivateManagerId.ToString().Replace("-", "") + "/" + display.MerchantId.ToString().Replace("-", "")));

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            //if the item didn't actually get created.
            var store = sg.GetStoreForManager(storeItem.MerchantId, storeItem.PrivateManagerId, false);
            storeItem.InternalId = store.InternalReference;
            storeItem.MerchantId = store.MerchantId;
            storeItem.PrivateManagerId = store.PrivateManagerId;
            return View(storeItem);
        }
        [Authorize]
        public ActionResult StoreNewItem(Guid privId, Guid storeId)
        {
            StoreItemDisplayModel storeItem = new StoreItemDisplayModel();
            try
            {
                var sg = new StoreGateway();
                var store = sg.GetStoreForManager(storeId, privId, false);
                storeItem.InternalId = store.InternalReference;
                storeItem.MerchantId = store.MerchantId;
                storeItem.Currency = store.Currency;
                storeItem.PrivateManagerId = store.PrivateManagerId;
                StoreItemTypeEnum[] notInList = { StoreItemTypeEnum.None };
                storeItem.ItemTypeSelectList = StoreItemTypeEnum.Item.ToSelectList(notInList);
                var colors = ColorDisplayFactory.GetOwnerColors(storeId);
                storeItem.ColorList = new SelectList(colors, "HexColor", "NameOfColor");
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(storeItem);
        }
        [HttpPost]
        [Authorize]
        public ActionResult CreateStore(DisplayStore store)
        {
            try
            {

                var sg = new StoreGateway();
                DisplayStore sto = sg.CreateNewStoreAndMerchantAccount(store.InternalReference);
                var id = RDN.Library.Classes.Account.User.GetMemberId();
                MemberCache.Clear(id);
                MemberCache.ClearApiCache(id);

                return Redirect(Url.Content("~/store/home/" + sto.PrivateManagerId.ToString().Replace("-", "") + "/" + sto.MerchantId.ToString().Replace("-", "")));

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        public ActionResult StoreHome(Guid? privId, Guid? storeId)
        {
            DisplayStore displayStore = null;
            try
            {
                if (storeId == null)
                {
                    var store = new DisplayStore();
                    //when creating a store, the store is created with the users 
                    //member id or the federation or league id.
                    if (privId == null || privId == new Guid())
                    {
                        privId = RDN.Library.Classes.Account.User.GetMemberId();
                        //check if they already have a store, if they do, we load it.
                        bool hasStore = MemberCache.HasPersonalStoreAlreadyForUser(privId.Value);
                        if (hasStore)
                            return Redirect(Url.Content("~/store/home/" + MemberCache.GetStoreManagerKeysForUrlUser(privId.Value)));
                    }
                    store.InternalReference = privId.Value;
                    NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                    string updated = nameValueCollection["u"];

                    if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.wbc.ToString())
                    {
                        SiteMessage message = new SiteMessage();
                        message.MessageType = SiteMessageType.Error;
                        message.Message = "Wrong Beta Code, Please Try Again.";
                        this.AddMessage(message);
                    }
                    return View(store);
                }
                var sg = new StoreGateway();
                displayStore = sg.GetStoreForManager(storeId, privId, false);
                if (!displayStore.AcceptPaymentsViaStripe && !displayStore.AcceptPaymentsViaPaypal)
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Please Setup a Payment Provider in the Store Settings.";
                    this.AddMessage(message);
                }
                else if (!displayStore.IsPublished)
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Your Store Is Not Yet Published. Publish the Store in the Settings.";
                    this.AddMessage(message);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(displayStore);
        }

        public ActionResult AddColor(string nameOfColor, string hexOfColor, string storeId)
        {
            try
            {
                bool re = ColorDisplayFactory.AddOwnerColor(nameOfColor, hexOfColor, new Guid(storeId));
                return Json(new { isSuccess = re }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType(), additionalInformation: nameOfColor + ":" + hexOfColor);
                return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
