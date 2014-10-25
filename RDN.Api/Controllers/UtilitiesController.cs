using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.ViewModel;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Controls.Calendar;
using RDN.Library.Cache;

namespace RDN.Api.Controllers
{
    public class UtilitiesController : Controller
    {
        /// <summary>
        /// secret url so admins can clear the membership cache of a user.
        /// we hit this from the administration pages.
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public ActionResult ClearMemberCache1234(string memberId)
        {
            try
            {
                MemberCache.Clear(new Guid(memberId));
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType(), additionalInformation: memberId);
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ClearLeagueMembersCache1234(string lid)
        {
            try
            {
                MemberCache.ClearLeagueMembersCache(new Guid(lid));
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType(), additionalInformation: lid);
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetCountriesInfo()
        {
            try
            {
                var countries = ApiCache.GetCountriesInfo();
                return Json(countries);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCountries()
        {
            try
            {
                var countries = ApiCache.GetCountriesInfo().ToDictionary(x => x.CountryId, x => x.Name);
                return Json(countries);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ClearLiveGameCache1234(string gameId)
        {
            try
            {
                GameCache.ClearGameFromCache(new Guid(gameId));
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType(), additionalInformation: gameId);
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ClearCurrencyExchangeRates()
        {
            try
            {
                SiteCache.ClearCurrencyExchanges();
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
