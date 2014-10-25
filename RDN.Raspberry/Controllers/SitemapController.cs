using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Raspberry.Models.Utilities;

namespace RDN.Raspberry.Controllers
{
    public class SitemapController : Controller
    {
        [Authorize]
        public ActionResult Delete()
        {
            IdModel mod = new IdModel();
            mod.IsDeleted = false;
            return View(mod);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Delete(IdModel mod)
        {
            mod.IsDeleted = RDN.Library.Classes.Utilities.SitemapHelper.DeleteSiteMapItem(mod.Id);
            return View(mod);
        }

    }
}
