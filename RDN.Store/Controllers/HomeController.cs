using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Store;
using RDN.Library.Classes.Store.Classes;
using RDN.Library.Classes.Store.Display;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Classes.Invoice;
using RDN.Library.Classes.Error;
using System.Collections.Specialized;
using RDN.Library.Util;
using RDN.Library.Util.Enum;
using RDN.Portable.Classes.Store.Enums;
using RDN.Store.Models.Utilities;
using RDN.Portable.Models.Json.Shop;
using RDN.Library.Cache;
using RDN.Store.Models;
using RDN.Store.Models.OutModel;

namespace RDN.Store.Controllers
{
    public class HomeController : BaseController
    {


        private readonly int DEFAULT_PAGE_SIZE = 40;// 100;
        public ActionResult Index(int? page)
        {
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
            string updated = nameValueCollection["u"];

            if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sww.ToString())
            {
                SiteMessage message = new SiteMessage();
                message.MessageType = SiteMessageType.Success;
                message.Message = "Something went wrong. Error sent to developers, please try again later.";
                this.AddMessage(message);
            }
            if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.re.ToString())
            {
                SiteMessage message = new SiteMessage();
                message.MessageType = SiteMessageType.Success;
                message.Message = "Successfully added the Review.";
                this.AddMessage(message);
            }

            var model = new SimpleModPager<StoreItemJson>();
            if (page == null)
                model.CurrentPage = 1;
            else
                model.CurrentPage = page.Value;
            model.NumberOfRecords = SiteCache.GetNumberOfItemsForSale();
            model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / DEFAULT_PAGE_SIZE);
            model.PageSize = DEFAULT_PAGE_SIZE;
            var output = FillStoreModel(model);
            return View(output);
        }

        /// <summary>
        /// converts the large items list into a single model to display on the view.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="federationId"></param>
        /// <returns></returns>
        private GenericSingleModel<SimpleModPager<StoreItemJson>> FillStoreModel(SimpleModPager<StoreItemJson> model)
        {
            for (var i = 1; i <= model.NumberOfPages; i++)
                model.Pages.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                    Selected = i == model.CurrentPage
                });
            var output = new GenericSingleModel<SimpleModPager<StoreItemJson>> { Model = model };

            output.Model.Items = SiteCache.GetAllPublishedItems(DEFAULT_PAGE_SIZE, (model.CurrentPage - 1)); //* DEFAULT_PAGE_SIZE);
            if (output.Model.Items == null)
                output.Model.Items = new List<StoreItemJson>();
            return output;
        }

#if !DEBUG
[RequireHttps] 
#endif
        public ActionResult Review(string name, string id, string invoiceId)
        {
            StoreItem item = null;
            try
            {
                StoreGateway sg = new StoreGateway();
                item = sg.GetStoreItem(Convert.ToInt32(id));
                var invoice = sg.GetInvoice(new Guid(invoiceId));
                var storeItem = invoice.InvoiceItems.Where(x => x.StoreItemId == Convert.ToInt32(id)).FirstOrDefault();
                if (storeItem == null)
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
                var review = ItemReview.GetItemReviewForInvoiceItem(storeItem.InvoiceItemId, Convert.ToInt32(id));
                if (review != null)
                {
                    item.ReviewTitle = review.title;
                    item.ReviewComment = review.comment;
                    item.rate = review.rate;
                    item.ReviewId = review.ReviewId;

                }
                item.InvoiceItemId = storeItem.InvoiceItemId;
                return View(item);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }

#if !DEBUG
[RequireHttps] 
#endif
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult Review(StoreItem form)//RDN.Library.DataModels.Store.Review review
        {
            //http://localhost:8847/product-review/tdp1medium/10/e6cab01ce0044bd6a5d31970a7fe8dc0
            ItemReview oReview = new ItemReview();
            var memId = RDN.Library.Classes.Account.User.GetMemberId();
            oReview.MemberId = memId;
            oReview.rate = form.ratings;
            oReview.StoreItemId = form.StoreItemId;
            oReview.title = form.ReviewTitle;
            oReview.comment = form.ReviewComment;
            oReview.InvoiceItemId = form.InvoiceItemId;
            oReview.ReviewId = form.ReviewId;

            bool execute = RDN.Library.Classes.Store.ItemReview.AddReview(oReview);
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.re));

        }


#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult Sell()
        {

            return View();
        }
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult Shops()
        {
            StoreGateway sg = new StoreGateway();
            List<DisplayStore> shops = sg.GetPublicShops(0, 30);
            return View(shops);
        }
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult StoreItem(string name, string id)
        {
            StoreItem item = null;
            try
            {
                StoreGateway sg = new StoreGateway();
                item = sg.GetStoreItem(Convert.ToInt32(id));
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