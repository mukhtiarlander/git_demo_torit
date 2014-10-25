using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
//using GCheckout;
//using GCheckout.AutoGen;
//using GCheckout.Checkout;
//using GCheckout.Util;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Classes.Display;
using RDN.Library.Classes.Payment.Classes.Invoice;
using RDN.Library.Classes.Payment.Enums;
using ShippingType = RDN.Library.Classes.Payment.Enums.ShippingType;

namespace RDN.Raspberry.Controllers
{
    public class PaymentController : Controller
    {
        // RDNATION MERCHANT ID: D61EB50C-E65C-11E1-84DA-AFFB6088709B

        //[Authorize]
        //[HttpPost]
        //public ActionResult ViewDeadPaypalPayments(IdModel mod)
        //{
            
        //    PaymentGateway pg = new PaymentGateway();
        //    var invoice = pg.GetDisplayInvoice(new Guid(mod.Id));
        //    mod.IsSuccess = RDN.Library.Classes.Payment.Paypal.IPNHandler.HandleDuesPayments(invoice, "From Raspberry");
        //    return View(mod);
        //}
    }
}
