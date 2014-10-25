using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Utilities.Config;
using System.IO;
using RDN.Utilities.Error;
using RDN.Library.DataModels;
using RDN.Library.ViewModel;
using RDN.Library.Classes.Team;
using RDN.Library.Classes.Error;
using RDN.Library.Cache;

namespace RDN.Api.Controllers
{
    public class LogosController : Controller
    {
        /// <summary>
        /// gets a post from the logos scoreboard. we save and dispaly for later.
        /// </summary>
        /// <param name="k"></param>
        /// <param name="ti">team id</param>
        /// <param name="lid">team id</param>
        /// <returns></returns>
        public ActionResult uploadTeamLogo(string k, string ti, string teamName)
        {
            try
            {
                if (k == ScoreboardConfig.KEY_FOR_UPLOAD)
                {
                    foreach (string logo in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[logo];
                        if (file.ContentLength > 0)
                        {
                            try
                            {
                                var logoId = TeamFactory.SaveLogoToDbForTeam(file, new Guid(ti), teamName);
                                ApiCache.ClearCache();
                                return Json(new { result = true, id = logoId }, JsonRequestBehavior.AllowGet);
                            }
                            catch (Exception exception)
                            {
                                Library.Classes.Error.ErrorDatabaseManager.AddException(exception, GetType(),ErrorGroupEnum.Database);
                            }
                        }
                    }
                    return Json(new { result = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { result = "Key does not match." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// gets all the logos in the db
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllLogos()
        {
            try
            {
                return Json(new { logos = ApiCache.GetAllLogos() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }



    }
}
