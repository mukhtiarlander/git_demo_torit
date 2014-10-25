using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.Raspberry.Controllers
{
    public class YafController : Controller
    {
        public ActionResult SyncYafUsers()
        {
            ViewBag.Result = RDN.Library.Classes.Yaf.Yaf.SyncYafDisplayNamesWithDerbyNames();
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }
    }
}
