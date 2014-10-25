using RDN.Library.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Game;
using System.IO;

namespace RDN.Api.Controllers
{
    public class TournamentController : Controller
    {
        public ActionResult ClearTournamentCache()
        {
            try
            {
                SiteCache.ClearTournaments();
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RenderTournament(Guid id)
        {
            try
            {
                var image = Tournament.RenderTournamentBrackets(SiteCache.GetTournament(id), false);

                MemoryStream ms = new MemoryStream();

                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                return File(ms.ToArray(), "image/png");
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public ActionResult RenderPerformanceTournament(Guid id)
        {
            try
            {
                var image = Tournament.RenderTournamentBrackets(SiteCache.GetTournament(id), true);

                MemoryStream ms = new MemoryStream();

                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                return File(ms.ToArray(), "image/png");
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }



    }
}
