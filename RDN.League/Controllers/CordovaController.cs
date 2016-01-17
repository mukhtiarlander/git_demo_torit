using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Common.CordovaModules;

namespace RDN.League.Controllers
{
    public class CordovaController : Controller
    {
        public ActionResult SetPlatformCookie(string platform)
        {
            HttpContext.Response.SetCordovaCookie(platform);
            return RedirectToAction("Login","Account");
        }
    }
}