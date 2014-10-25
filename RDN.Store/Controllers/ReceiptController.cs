using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment.Classes.Display;
using RDN.Library.Classes.Store;

namespace RDN.Store.Controllers
{
    public class ReceiptController : BaseController
    {
        //
        // GET: /Receipt/

        public ActionResult ReceiptIndex(Guid invoiceId)
        {
            DisplayInvoice invoice = null;
            try
            {
                var sg = new StoreGateway();
                invoice = sg.GetInvoice(invoiceId);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(invoice);
        }

    }
}
