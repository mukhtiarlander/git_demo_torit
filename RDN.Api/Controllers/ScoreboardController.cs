using System;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Scoreboard.Library.Util;
using Scoreboard.Library.ViewModel;
using RDN.Library.ViewModel;
using RDN.Utilities.Error;
using RDN.Utilities.Config;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Scoreboard;
using Scoreboard.Library.ViewModel.Members.Enums;
using RDN.Library.Classes.Mobile;
using RDN.Portable.Config;
using RDN.Portable.Settings.Enums;
using RDN.Library.Cache;
using System.Threading.Tasks;
using RDN.Library.Classes.Config;

namespace RDN.Api.Controllers
{
    public class ScoreboardController : Controller
    {
        public ActionResult UploadMemberPictureFromGame(string k, string gameId, string memId, string fileName, string memberType)
        {
            try
            {
                MemberTypeEnum mem = MemberTypeEnum.Skater;
                if (!String.IsNullOrEmpty(memberType))
                    mem = (MemberTypeEnum)Enum.Parse(typeof(MemberTypeEnum), memberType);
                if (k == ScoreboardConfig.KEY_FOR_UPLOAD)
                {
                    foreach (string pictureFile in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[pictureFile];
                        if (file.ContentLength > 0)
                        {
                            RDN.Library.Classes.Account.User.AddMemberPhotoForGame(new Guid(gameId), file.InputStream, fileName, new Guid(memId), mem);
                        }
                    }

                }
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType(), additionalInformation: gameId);
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// httppost for feedback from the server.
        /// </summary>
        /// <param name="k">secret key</param>
        /// <param name="feedback">feedback from the user</param>
        /// <param name="league">league name</param>
        /// <param name="email">email</param>
        /// <param name="m">mac address of the scoreboard</param>
        /// <returns></returns>
        public ActionResult feedback(string k)
        {
            try
            {
                if (k == ScoreboardConfig.KEY_FOR_UPLOAD)
                {
                    foreach (string errorFile in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[errorFile];
                        if (file.ContentLength > 0)
                        {
                            try
                            {
                                DirectoryInfo dir = new DirectoryInfo(LibraryConfig.SAVE_FEEDBACK_FOLDER);
                                if (!dir.Exists)
                                    dir.Create();
                                string compressedFile = Path.Combine(LibraryConfig.SAVE_FEEDBACK_FOLDER, Path.GetFileName(file.FileName));
                                file.SaveAs(compressedFile);
                                string encryptedFile = Compression.Decompress(new FileInfo(compressedFile));
                                string finalFile = Path.Combine(LibraryConfig.SAVE_FEEDBACK_FOLDER, "feedback" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".xml");
                                Encryption.DecryptFiletoFile(encryptedFile, finalFile);

                                FileStream f = new FileStream(finalFile, FileMode.Open, FileAccess.Read);

                                var fb = FeedbackViewModel.deserialize(f);
                                ScoreboardFeedbackClass.commitScoreboardFeedback(fb.Feedback, fb.League, fb.Email, fb.ScoreboardMacId);
                                f.Close();
                                f.Dispose();
                                FileInfo f3 = new FileInfo(finalFile);

                                if (f3.Exists)
                                    f3.Delete();
                                FileInfo f1 = new FileInfo(encryptedFile);
                                if (f1.Exists)
                                    f1.Delete();
                                FileInfo f2 = new FileInfo(compressedFile);
                                if (f2.Exists)
                                    f2.Delete();
                            }
                            catch (Exception e)
                            {
                                Library.Classes.Error.ErrorDatabaseManager.AddException(e, e.GetType(), ErrorGroupEnum.Database);
                                //ErrorServerViewModel.commitScoreboardException(er, String.Empty);
                                string filePath = Path.Combine(LibraryConfig.SAVE_ERRORS_FOLDER, Path.GetFileName(file.FileName));
                                DirectoryInfo dir = new DirectoryInfo(LibraryConfig.SAVE_ERRORS_FOLDER);
                                if (!dir.Exists)
                                    dir.Create();
                                file.SaveAs(filePath);
                            }
                        }
                    }
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { result = "Key does not match." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// gets hit when ever a scoreboard is launched.  We keep a record of the active installs and active start ups.
        /// </summary>
        /// <param name="k"></param>
        /// <param name="mac"></param>
        /// <returns></returns>
        public ActionResult scoreboardLaunched(string k, string mac)
        {
            try
            {
                if (k == ScoreboardConfig.KEY_FOR_UPLOAD)
                {
                    RDN.Library.Classes.Scoreboard.Scoreboard.insertScoreboardActiveId(mac, String.Empty);
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { result = "Key does not match." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult scoreboardLaunchedNew(string k, string mac, string version)
        {
            try
            {
                if (k == ScoreboardConfig.KEY_FOR_UPLOAD && !String.IsNullOrEmpty(mac))
                {
                    RDN.Library.Classes.Scoreboard.Scoreboard.insertScoreboardActiveId(mac, version);
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { result = "Key does not match." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// just a quick check the scoreboard software can make to see if the api is running.
        /// </summary>
        /// <returns></returns>
        public ActionResult isOnline()
        {
            try
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// captures the uploading of errors from the scoreboard.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult uploadLiveGames(string k)
        {
            try
            {
                if (k == ScoreboardConfig.KEY_FOR_UPLOAD)
                {
                    Guid privatePassCode = new Guid();
                    Guid publicGameId = new Guid();
                    bool isGameOnline = false;
                    string additionalErrorInfo = String.Empty;



                    foreach (string liveGame in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[liveGame];
                        if (file.ContentLength > 0)
                        {
                            try
                            {
                                DirectoryInfo dir = new DirectoryInfo(LibraryConfig.SAVE_LIVE_GAMES_FOLDER);
                                if (!dir.Exists)
                                    dir.Create();
                                string compressedFile = Path.Combine(LibraryConfig.SAVE_LIVE_GAMES_FOLDER, Path.GetFileName(file.FileName));
                                file.SaveAs(compressedFile);
                                string encryptedFile = Compression.Decompress(new FileInfo(compressedFile));
                                string finalFile = Path.Combine(LibraryConfig.SAVE_LIVE_GAMES_FOLDER, "LiveGame" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".xml");
                                Encryption.DecryptFiletoFile(encryptedFile, finalFile);
                                additionalErrorInfo += finalFile + ";";
                                //the decryptiong didnt work.
                                if (finalFile != null)
                                {
                                    var game = GameViewModel.deserializeGame(finalFile);
                                    if (game != null)
                                    {
                                        privatePassCode = game.IdForOnlineManagementUse;
                                        publicGameId = game.GameId;
                                        Task<bool>.Factory.StartNew(() =>
                                        {
                                            var currentGame = GameServerViewModel.saveGameToDb(game);
                                            game.EmbededVideoHtml = currentGame.EmbededVideoHtml;

                                            //mobile notifies of a new game.
                                            //if (GameServerViewModel.GetGameFromCache(game.GameId) == null)
                                            //    MobileNotification.SendNotifications("New Derby Game: " + game.GameName, game.Team1.TeamName + " vs. " + game.Team2.TeamName, MobileNotificationTypeEnum.Game);
                                            return true;
                                        });
                                        var oldCachedGame = GameCache.saveGameToCurrentLiveGamesCache(game);
                                        isGameOnline = true;
                                    }
                                }
                                try
                                {
                                    FileInfo f3 = new FileInfo(finalFile);
                                    if (f3.Exists)
                                        f3.Delete();
                                }
                                catch (Exception e)
                                {
                                    ErrorDatabaseManager.AddException(e, GetType(), additionalInformation: additionalErrorInfo);
                                }

                                FileInfo f1 = new FileInfo(encryptedFile);
                                try
                                {
                                    if (f1.Exists)
                                        f1.Delete();
                                }
                                catch (Exception e)
                                {
                                    ErrorDatabaseManager.AddException(e, GetType(), additionalInformation: additionalErrorInfo);
                                }
                                try
                                {
                                    FileInfo f2 = new FileInfo(compressedFile);
                                    if (f2.Exists)
                                        f2.Delete();
                                }
                                catch (Exception e)
                                {
                                    ErrorDatabaseManager.AddException(e, GetType(), additionalInformation: additionalErrorInfo);
                                }
                            }
                            catch (Exception e)
                            {
                                Library.Classes.Error.ErrorDatabaseManager.AddException(e, e.GetType(), ErrorGroupEnum.Database, additionalInformation: additionalErrorInfo);
                                string filePath = Path.Combine(LibraryConfig.SAVE_LIVE_GAMES_FOLDER, Path.GetFileName(file.FileName));
                                DirectoryInfo dir = new DirectoryInfo(LibraryConfig.SAVE_LIVE_GAMES_FOLDER);
                                if (!dir.Exists)
                                    dir.Create();
                                file.SaveAs(filePath);
                            }
                        }
                    }

                    return Json(new { result = true, url = LibraryConfig.InternalSite +"/game/manage/" + privatePassCode.ToString().Replace("-", "") + "/" + publicGameId.ToString().Replace("-", ""), IsGameOnline = isGameOnline }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { result = "Key does not match." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// captures the uploading of errors from the scoreboard.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult uploadCompletedGames(string k)
        {
            try
            {
                if (k == ScoreboardConfig.KEY_FOR_UPLOAD)
                {
                    foreach (string errorFile in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[errorFile];
                        if (file.ContentLength > 0)
                        {
                            try
                            {
                                DirectoryInfo dir = new DirectoryInfo(LibraryConfig.SAVE_OLD_GAMES_FOLDER);
                                if (!dir.Exists)
                                    dir.Create();
                                string compressedFile = Path.Combine(LibraryConfig.SAVE_OLD_GAMES_FOLDER, Path.GetFileName(file.FileName));
                                file.SaveAs(compressedFile);
                                string encryptedFile = Compression.Decompress(new FileInfo(compressedFile));
                                string finalFile = Path.Combine(LibraryConfig.SAVE_OLD_GAMES_FOLDER, "CompletedGame" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".xml");
                                Encryption.DecryptFiletoFile(encryptedFile, finalFile);

                                var game = GameViewModel.deserializeGame(finalFile);
                                var currentGame = GameServerViewModel.saveGameToDb(game);
                                if (currentGame != null)
                                    game.EmbededVideoHtml = currentGame.EmbededVideoHtml;
                                var oldCachedGame = GameCache.saveGameToCurrentLiveGamesCache(game);

                                try
                                {
                                    FileInfo f3 = new FileInfo(finalFile);

                                    if (f3.Exists)
                                        f3.Delete();
                                }
                                catch { }
                                try
                                {
                                    FileInfo f1 = new FileInfo(encryptedFile);
                                    if (f1.Exists)
                                        f1.Delete();
                                }
                                catch { }
                                try
                                {
                                    FileInfo f2 = new FileInfo(compressedFile);
                                    if (f2.Exists)
                                        f2.Delete();
                                }
                                catch { }
                            }
                            catch (Exception e)
                            {
                                Library.Classes.Error.ErrorDatabaseManager.AddException(e, e.GetType(), ErrorGroupEnum.Database);
                                string filePath = Path.Combine(LibraryConfig.SAVE_OLD_GAMES_FOLDER, Path.GetFileName(file.FileName));
                                DirectoryInfo dir = new DirectoryInfo(LibraryConfig.SAVE_OLD_GAMES_FOLDER);
                                if (!dir.Exists)
                                    dir.Create();
                                file.SaveAs(filePath);
                            }
                        }
                    }
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { result = "Key does not match." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
