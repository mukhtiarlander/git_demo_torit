using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment.Paywall;

namespace RDN.Controllers
{
    public class PaywallController : Controller
    {
        public ActionResult ViewPaywallReceipt(string id)
        {
            PaywallReceipt receipt = new PaywallReceipt();
            try
            {
                receipt = PaywallReceipt.GetReceiptForInvoice(new Guid(id));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(receipt);
        }

    }
}
