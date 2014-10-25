using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Game;

namespace RDN.Controllers
{
    public class IframeController : Controller
    {
        //
        // GET: /Iframe/

        public ActionResult StreamingVideo(string id)
        {
            var tourn = Tournament.GetPublicTournament(new Guid(id));
            ViewBag.Embed = tourn.EmbedVideoString;
            return View();
        }

    }
}
