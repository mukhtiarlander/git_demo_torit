using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Game;
using RDN.Library.Classes.Error;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class ScoreboardController : Controller
    {
        //
        // GET: /Scoreboard/
        [Authorize]
        public ActionResult ManageGame(string privatePass, string gameId)
        {
            return Redirect("~/game/manage/" + privatePass + "/" + gameId);
        }
        [HttpPost]
        [Authorize]
        public ActionResult PublishGameOnline(Game game)
        {
            try
            {
                if (GameManager.PublishGameOnline(game.GameId, true))
                    return Redirect(Url.Content("~/scoreboard/manage/" + game.PrivateKeyForGame.ToString().Replace("-", "") + "/" + game.GameId.ToString().Replace("-", "")));

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Redirect(Url.Content("~/"));
        }
        [ValidateInput(false)]
        [HttpPost]
        [Authorize]
        public ActionResult EmbedVideoWithGame(Game game)
        {
            try
            {
                if (HttpContext.Request.Form["addVideo"] != null)
                {
                    if (GameManager.EmbedVideoWithGame(game.GameId, game.EmbededVideoString, Scoreboard.Library.ViewModel.GameVideoTypeEnum.EmbededVideo))
                        return Redirect(Url.Content("~/scoreboard/manage/" + game.PrivateKeyForGame.ToString().Replace("-", "") + "/" + game.GameId.ToString().Replace("-", "")));


                }
                else if (HttpContext.Request.Form["removeVideo"] != null)
                {
                    if (GameManager.RemoveEmbedVideoWithGame(game.GameId))
                        return Redirect(Url.Content("~/scoreboard/manage/" + game.PrivateKeyForGame.ToString().Replace("-", "") + "/" + game.GameId.ToString().Replace("-", "")));

                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/"));
        }

        [ValidateInput(false)]
        [HttpPost]
        [Authorize]
        public ActionResult EmbedSilverlightWithGame(Game game)
        {
            try
            {
                if (HttpContext.Request.Form["addSilverlight"] != null)
                {
                    if (GameManager.EmbedSilverlightWithGame(game.GameId, game.StreamingUrlSilverlight, game.StreamingMobileUrlSilverlight, Scoreboard.Library.ViewModel.GameVideoTypeEnum.SilverLightLive))
                        return Redirect(Url.Content("~/scoreboard/manage/" + game.PrivateKeyForGame.ToString().Replace("-", "") + "/" + game.GameId.ToString().Replace("-", "")));
                }
                else if (HttpContext.Request.Form["removeSilverlight"] != null)
                {
                    if (GameManager.RemoveSilverlightVideoWithGame(game.GameId))
                        return Redirect(Url.Content("~/scoreboard/manage/" + game.PrivateKeyForGame.ToString().Replace("-", "") + "/" + game.GameId.ToString().Replace("-", "")));
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/"));
        }


    }
}
