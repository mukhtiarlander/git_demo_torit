using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Utilities.Config;
using RDN.Library.ViewModel;
using RDN.Library.Classes.Game;
using RDN.Utilities.Error;
using RDN.Library.Classes.Error;

namespace RDN.Api.Controllers
{
    public class AdvertisementController : Controller
    {
        /// <summary>
        /// uploads the advert from the scoreboard and saves it to the db.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult uploadAdvertisement(string k, string gameId)
        {
            try { 
            if (k == ScoreboardConfig.KEY_FOR_UPLOAD)
            {
                foreach (string logo in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[logo];
                    if (file.ContentLength > 0)
                    {
                        try
                        {
                            var advertId = Game.SaveAdvertisementsToDb(new Guid(gameId), file);
                            return Json(new { result = true, id = advertId }, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception exception)
                        {
                            Library.Classes.Error.ErrorDatabaseManager.AddException(exception, GetType(),ErrorGroupEnum.Processing);
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
                ErrorDatabaseManager.AddException(exception, GetType(), additionalInformation: gameId);
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
