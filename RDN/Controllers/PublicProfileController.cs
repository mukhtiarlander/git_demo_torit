using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Models.Utilities;
using RDN.Models.OutModel;
using RDN.Library.Cache;
using RDN.Library.Classes.Account.Classes.Json;
using RDN.Portable.Models.Json;

namespace RDN.Controllers
{
    public class PublicProfileController : Controller
    {
        private readonly int DEFAULT_PAGE_SIZE = 100;

        public ActionResult AllSkaters()
        {
            return View();
        }


        public JsonResult AllSkatersSearching()
        {
            List<SkaterJson> names = new List<SkaterJson>();
            if (!String.IsNullOrEmpty(HttpContext.Request.Params["sSearch"]))
            {
                string search = HttpContext.Request.Params["sSearch"];
                names = RDN.Library.Classes.Account.User.SearchPublicMembers(search, 100);
            }
            else
            {
                names = RDN.Library.Classes.Account.User.SearchPublicMembers("", 100);
            }
            return Json(new
            {
                sEcho = HttpContext.Request.Params["sEcho"],
                iTotalRecords = names.Count,
                iTotalDisplayRecords = names.Count,
                aaData = (
        from n in names
        select new[]
                    {
                        n.DerbyName,
                        n.DerbyNameUrl,
                        n.DerbyNumber,
                        n.Gender,
                        n.LeagueName,
                        n.LeagueUrl,
                        n.photoUrl,
                        "true"
                    }).ToArray()
            }, JsonRequestBehavior.AllowGet);
        }






        public ActionResult AllRefs()
        {
            return View();
        }

        public ActionResult Skater(string name, string id)
        {
            var member = SiteCache.GetPublicMemberFull(new Guid(id));
            if (member == null)
                return Redirect(Url.Content("~/"));
            return View(member);
        }
        public ActionResult SkaterRedirect(string id)
        {
            var member = SiteCache.GetPublicMemberFull(new Guid(id));
            if (member == null)
                return Redirect(Url.Content("~/"));
            return RedirectPermanent(Url.Content("~/" + RDN.Library.Classes.Config.LibraryConfig.SportNameForUrl + "-" + RDN.Library.Classes.Config.LibraryConfig.NameOfMember.ToLower() + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(member.DerbyName)) + "/" + id);

        }
        public ActionResult SkaterTwoEvils(string name, string id)
        {
            var member = RDN.Library.Classes.Account.User.GetMemberDisplayTwoEvils(new Guid(id));
            if (member == null)
                return Redirect(Url.Content("~/"));
            return View(member);
        }
        public ActionResult Ref(string name, string id)
        {
            return View();
        }

    }
}
