using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Game;
using RDN.Raspberry.Models.Utilities;

namespace RDN.Raspberry.Controllers
{
    public class GameController : Controller
    {

        [Authorize]
        public ActionResult UnPublish()
        {
            IdModel mod = new IdModel();
            mod.IsSuccess = false;
            return View(mod);
        }
        [Authorize]
        [HttpPost]
        public ActionResult UnPublish(IdModel mod)
        {
            mod.IsSuccess = GameManager.PublishGameOnline(new Guid(mod.Id), false);
            return View(mod);
        }

        [Authorize]
        public ActionResult Delete()
        {
            IdModel mod = new IdModel();
            mod.IsDeleted = false;
            return View(mod);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Delete(IdModel mod)
        {
            mod.IsDeleted = RDN.Library.Classes.Game.Game.DeleteGame(new Guid(mod.Id));
            return View(mod);
        }
        [Authorize]
        public ActionResult DeleteTournament()
        {
            IdModel mod = new IdModel();
            mod.IsDeleted = false;
            return View(mod);
        }
        [Authorize]
        [HttpPost]
        public ActionResult DeleteTournament(IdModel mod)
        {
            mod.IsDeleted = RDN.Library.Classes.Game.Tournament.RemoveTournament(new Guid(mod.Id));
            return View(mod);
        }


        [Authorize]
        public ActionResult JoinGames()
        {
            IdModel mod = new IdModel();
            mod.IsSuccess = false;
            return View(mod);
        }
        [Authorize]
        [HttpPost]
        public ActionResult JoinGames(IdModel mod)
        {
            mod.IsSuccess = RDN.Library.Classes.Game.Game.JoinGames(new Guid(mod.Id), new Guid(mod.Id2));
            return View(mod);
        }

    }
}
