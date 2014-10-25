using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Cache;
using RDN.Raspberry.Models.Team;

namespace RDN.Raspberry.Controllers
{
    public class TeamController : Controller
    {
        //
        // GET: /Team/
        [Authorize]
        public ActionResult UploadLogo()
        {
            AddLogo logo = new AddLogo();
            ViewBag.Saved = false;
            return View(logo);
        }
        [Authorize]
        [HttpPost]
        public ActionResult UploadLogo(AddLogo logo, HttpPostedFileBase file)
        {
            RDN.Library.Classes.Team.TeamFactory.SaveLogoToDbForTeam(file, new Guid(), logo.TeamName);
            ViewBag.Saved = true;
            ApiCache.ClearCache();
            return View(logo);
        }

        public ActionResult AllLogos()
        {
            var logos = ApiCache.GetAllScoreboardLogos();
            return View(logos);
        }

        public ActionResult UpdateLogo()
        {
            ApiCache.ClearCache();
            if (HttpContext.Request.Form["Update"] != null)
            {
                string LogoId = HttpContext.Request.Form["Update"].ToString();
                LogoId = LogoId.Replace("Update-", "");
                string teamName = HttpContext.Request.Form["TeamName-" + LogoId].ToString();

                RDN.Library.Classes.Team.TeamFactory.UpdateTeamNameLogo(new Guid(LogoId), teamName);
            }
            else if (HttpContext.Request.Form["Delete"] != null)
            {

                string LogoId = HttpContext.Request.Form["Delete"].ToString();
                LogoId = LogoId.Replace("Delete-", "");
                RDN.Library.Classes.Team.TeamFactory.DeleteLogoFromLogos(new Guid(LogoId));
            }
            else if (HttpContext.Request.Form["Merge"] != null)
            {
                var names = HttpContext.Request.Form.AllKeys.Where(c => c.StartsWith("checkbox")).ToList();
                for (int i = 0; i < names.Count(); i++)
                    names[i] = names[i].Replace("checkbox-", "");
                RDN.Library.Classes.Team.TeamFactory.MergeLogos(names);

            }
            
            return Redirect(Url.Content("~/team/alllogos"));
        }


    }
}
