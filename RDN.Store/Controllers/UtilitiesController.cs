using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Utilities;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Store;
using RDN.Library.Classes.Store.Display;
using System.Text;
using RDN.Library.Classes.Store.Classes;
using RDN.Library.Classes.Config;

namespace RDN.Store.Controllers
{
    public class UtilitiesController : Controller
    {
        /// <summary>
        /// adds a node to the sitemap.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="modified"></param>
        /// <returns></returns>
        public ActionResult AddNodeToSiteMap(string url, bool modified)
        {
            try
            {
                //don't want the sitemap to have lostpassword links.
                //Verify  Name
                if (!url.Contains("receipt") && !url.Contains("product-review") && !url.Contains("checkout") && !url.Contains("cart") && !url.Contains("lostpassword") && !url.Contains("verifyname") && !url.Contains("returnsite=") && !url.Contains("returnurl") && !url.Contains("problem.error"))
                    SitemapHelper.AddNode(url, modified);
                return Json(new { answer = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { answer = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchStoreItem(string q, int limit)
        {
            StoreGateway sg = new StoreGateway();
            List<StoreItemJson> item = sg.SearchStoreItems(q, limit);

            System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string sJSON = oSerializer.Serialize(item);
            return Content(sJSON);
        }
        public ActionResult UpdateViewsCount(string storeItemId)
        {
            try
            {
                StoreGateway sg = new StoreGateway();
                bool done = sg.UpdateStoreItemViewCount(Convert.ToInt64(storeItemId));
                return Json(new { success = done }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { answer = false }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult Robot()
        {
            StringBuilder robotsResult = new StringBuilder();
            robotsResult.Append("Sitemap: " + LibraryConfig.PublicSite + "/sitemap" + Environment.NewLine);
            robotsResult.Append("Disallow: login" + Environment.NewLine);
            robotsResult.Append("Disallow: /signup" + Environment.NewLine);
            robotsResult.Append("Disallow: /utilities" + Environment.NewLine);
            robotsResult.Append("Disallow: /store" + Environment.NewLine);
            robotsResult.Append("Disallow: /" + LibraryConfig.SportNameForUrl + "-store" + Environment.NewLine);

            return Content(robotsResult.ToString(), "text/plain");
        }

    }
}
