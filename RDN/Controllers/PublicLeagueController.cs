using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Models.Utilities;
using RDN.Models.OutModel;
using RDN.Library.Cache;
using RDN.Library.Classes.League.Classes.Json;
using RDN.Portable.Models.Json.Public;
using RDN.Library.Classes.League;

namespace RDN.Controllers
{
    /// <summary>
    /// manage team controller
    /// </summary>
    public class PublicLeagueController : Controller
    {
        private readonly int DEFAULT_PAGE_SIZE = 100;

        public ActionResult AllLeagues(int? page)
        {
            var model = new SimpleModPager<LeagueJsonDataTable>();
            if (page == null)
                model.CurrentPage = 1;
            else
                model.CurrentPage = page.Value;
            model.NumberOfRecords = SiteCache.GetNumberOfLeaguesSignedUp();
            model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / DEFAULT_PAGE_SIZE);
            model.PageSize = DEFAULT_PAGE_SIZE;
            var output = FillLeagueModel(model);
            return View(output);
        }

        
        public JsonResult AllLeagueSearching()
        {
            List<LeagueJsonDataTable> names = new List<LeagueJsonDataTable>();
            if (!String.IsNullOrEmpty(HttpContext.Request.Params["sSearch"]))
            {
                string search = HttpContext.Request.Params["sSearch"];
                names = LeagueFactory.SearchPublicLeagues(search, 100, 0);
            }
            else
            {
                names = LeagueFactory.SearchPublicLeagues("", 100, 0);
            }
            return Json(new
            {
                sEcho = HttpContext.Request.Params["sEcho"],
                iTotalRecords = names.Count,
                iTotalDisplayRecords = 400,
                aaData = ( // if you change this, make sure you change the true on the searging side as well.
                    from n in names
                    select new[]
                    {
                        n.LeagueName,
                        n.LeagueUrl,
                        n.LogoUrlThumb,
                        n.State,
                        n.Country,
                        "true",
                        n.DateFounded.Year.ToString(), 
                        n.Facebook,
                         n.Instagram,
                          n.Membercount.ToString(),
                          n.RuleSetsPlayed,
                           n.Twitter,
                           n.WebSite
                    }).ToArray()
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// converts the large league list into a single model to display on the view.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="federationId"></param>
        /// <returns></returns>
        private GenericSingleModel<SimpleModPager<LeagueJsonDataTable>> FillLeagueModel(SimpleModPager<LeagueJsonDataTable> model)
        {
            for (var i = 1; i <= model.NumberOfPages; i++)
                model.Pages.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                    Selected = i == model.CurrentPage
                });
            var output = new GenericSingleModel<SimpleModPager<LeagueJsonDataTable>> { Model = model };

            output.Model.Items = SiteCache.GetAllPublicLeagues(DEFAULT_PAGE_SIZE, (model.CurrentPage - 1));
            if (output.Model.Items == null)
                output.Model.Items = new List<LeagueJsonDataTable>();
            return output;
        }

        public ActionResult League(string name, string id)
        {
            var league = SiteCache.GetLeague(new Guid(id));
            if (league == null)
                return Redirect(Url.Content("~/"));
            return View(league);
        }
        public ActionResult LeagueDerbyRoster(string name, string id)
        {
            var league = LeagueFactory.GetLeaguePublicDerbyRoster(new Guid(id));
            if (league == null)
                return Redirect(Url.Content("~/"));
            return View(league);
        }
        public ActionResult LeagueTwoEvils(string name, string id)
        {
            var league = LeagueFactory.GetLeagueTwoEvils(new Guid(id));
            if (league == null)
                return Redirect(Url.Content("~/"));
            return View(league);
        }


    }
}
