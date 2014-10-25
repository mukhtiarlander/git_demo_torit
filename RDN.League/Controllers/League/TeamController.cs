using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class TeamController : Controller
    {
    
        public ActionResult ViewTeam(string id, string name)
        {
            return View();
        }

        public ActionResult EditTeam(string id)
        {
            return View();
        }

        public ActionResult AddTeam(string leagueId)
        {
            return View();
        }

    }
}
