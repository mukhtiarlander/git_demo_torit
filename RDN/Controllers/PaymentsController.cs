using RDN.Models.Payments;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Payment;
using RDN.Portable.Config;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Payment.Classes.Invoice;
using RDN.Portable.Classes.Payment.Enums;
using RDN.Library.Classes.Config;

namespace RDN.Controllers
{
    public class PaymentsController : Controller
    {
        //
        // GET: /Payments/

        public ActionResult RNContent()
        {
            PaymentViewModel model = new PaymentViewModel();

            model.StripeKey = Library.Classes.Config.LibraryConfig.StripeApiPublicKey;
            return View(model);
        }
        [HttpPost]
        public ActionResult RNContent(PaymentViewModel model)
        {
            var options = InvoiceFactory.CreateNew().Initalize(LibraryConfig.SiteStoreID, "GBP", PaymentProvider.Stripe, LibraryConfig.IsProduction, ChargeTypeEnum.Subscription)
             .SetStripeTokenId(HttpContext.Request.Form["stripeToken"].ToString())
            .SetInvoiceId(Guid.NewGuid())
                    .SetSubscription(new InvoiceSubscription
                    {
                        ArticleNumber = HttpContext.Request.Form["stripeToken"].ToString(),
                        DescriptionRecurring = "Charge for Advertising on RN",
                        Name = "RN Advertising Content",
                        Description = "Charge for Advertising on RN",
                        NameRecurring = "RN Advertising Content",
                        DigitalPurchaseText = "You Can Now Advertise Content on RN",
                        Price = (decimal)20.00,
                        SubscriptionPeriodStripe = SubscriptionPeriodStripe.Monthly_RN_Sponsor,
                        SubscriptionPeriodLengthInDays = 30,
                        //ValidUntil = subScriptionDate.AddDays(lengthOfDays),
                        //league id is the ownerId
                    })
             .FinalizeInvoice();
            if (options.Status == InvoiceStatus.Payment_Successful)
                model.SuccessfullyCharged = true;

            return View(model);
        }

    }
}
