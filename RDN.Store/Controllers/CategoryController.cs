using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Store;
using RDN.Library.Classes.Store.Display;

namespace RDN.Store.Controllers
{
    public class CategoryController : BaseController
    {

        public ActionResult ViewCategory(string name, string id)
        {
            StoreGateway sg = new StoreGateway();
            DisplayCategory item = sg.GetCategoryAndItems(Convert.ToInt64(id));
            return View(item);
        }

    }
}
