using RDN.Library.Classes.Error;
using RDN.Library.Classes.Scoreboard;
using RDN.Portable.Classes.Utilities;
using RDN.Portable.Classes.Utilities.Enums;
using RDN.Portable.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.Api.Controllers
{
    public class FeedbackController : Controller
    {
        //
        // GET: /Feedback/

        public ActionResult SaveFeedback()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var fb = Network.LoadObject<Feedback>(ref stream);
                    ScoreboardFeedbackClass.CommitFeedback(fb.FeedbackText, fb.League, fb.Email, fb.FeedbackType);
                    fb.IsSuccessful = true;
                    return Json(fb, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());

            }
            return Json(new Feedback(), JsonRequestBehavior.AllowGet);
        }

    }
}
