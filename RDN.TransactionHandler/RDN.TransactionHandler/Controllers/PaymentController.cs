using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;
using GCheckout.Util;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Store;
using RDN.Library.Classes.Error;
using System.Web.Script.Serialization;

namespace RDN.TransactionHandler.Controllers
{
    public class PaymentController : Controller
    {
        public JsonResult Running()
        {
            try
            {

                return Json(RDN.Library.Classes.League.LeagueFactory.GetAllPublicLeagues(), "application/json", JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { broke = true }, "application/json", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCountries()
        {
            try
            {
                var countries = RDN.Library.Classes.Location.LocationFactory.GetCountriesDictionary();
                return Json(countries, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
