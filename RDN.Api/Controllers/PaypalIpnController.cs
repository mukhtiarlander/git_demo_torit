using RDN.Library.Classes.Config;
using RDN.Library.Classes.Payment.Paypal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.Api.Controllers
{
    public class PaypalIpnController : Controller
    {
        // GET: PaypalIpn
        public ActionResult IPN()
        {
            IPNHandler ipn = new IPNHandler(LibraryConfig.IsProduction, System.Web.HttpContext.Current);
            ipn.CheckStatus();
            ipn.InsertNewIPNNotification();

            return View();
        }
    }
}