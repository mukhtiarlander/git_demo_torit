using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.CordovaModules;

namespace RDN.Controllers
{
    public class CordovaController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SetPlatformCookie(string platform)
        {
            HttpContext.Response.SetCordovaCookie(platform);
            return RedirectToAction("Index");
        }
    }
}