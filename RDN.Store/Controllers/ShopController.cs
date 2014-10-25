using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Store;
using RDN.Library.Classes.Store.Display;

namespace RDN.Store.Controllers
{
    public class ShopController : BaseController
    {
        public ActionResult Shop(string id, string name)
        {
            DisplayStore item = new DisplayStore();
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string category = nameValueCollection["category"];

                int cat = 0;
                if (!String.IsNullOrEmpty(category))
                {
                    cat = Convert.ToInt32(category);
                }
                StoreGateway sg = new StoreGateway();
                item = sg.GetPublicStore(new Guid(id), cat);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(item);
        }
    }
}
