using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Cache;
using MoreLinq;

namespace RDN.Controllers
{
    public class PublicLogosController : Controller
    {
        public ActionResult AllLogos(int? page)
        {
            var model = ApiCache.GetAllLogos();
            model.AddRange(ApiCache.GetAllScoreboardLogos());
            var list = model.DistinctBy(x => x.TeamName).OrderBy(x => x.TeamName).ToList();
            return View(list);
        }

    }
}
