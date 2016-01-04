using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Error;
using RDN.Raspberry.Models.OutModel;
using RDN.Raspberry.Models.Utilities;
using RDN.Utilities.Config;
using RDN.Portable.Config;
using RDN.Portable.Classes.League.Classes;
using RDN.Portable.Classes.League.Enums;
using RDN.Library.Classes.League;
using RDN.Library.Classes.Document;
using RDN.Library.Classes.Store;
using RDN.Portable.Classes.Url;
using RDN.Library.Classes.Config;

namespace RDN.Raspberry.Controllers
{
    public class LeagueController : BaseController
    {

        [Authorize]
        [HttpPost]
        public ActionResult SetLeagueMerchantId(IdModel mod)
        {

            mod.IsSuccess = StoreGateway.ChangeOwnerOfStore(new Guid(mod.Id), new Guid(mod.Id2));
            return View(mod);
        }

        [Authorize]
        public ActionResult SetLeagueMerchantId()
        {
            IdModel mod = new IdModel();
            mod.IsDeleted = false;
            return View(mod);
        }


        [Authorize]
        [HttpPost]
        public ActionResult DeleteLogo(IdModel mod)
        {

            mod.IsDeleted = Library.Classes.Admin.League.League.DeleteLogoFromLeague(mod.Id);
            return View(mod);
        }

        [Authorize]
        public ActionResult DeleteLogo()
        {
            IdModel mod = new IdModel();
            mod.IsDeleted = false;
            return View(mod);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(IdModel mod)
        {

            mod.IsDeleted = Library.Classes.Admin.League.League.DeleteLeague(new Guid(mod.Id));
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
        public ActionResult HideLeague(IdModel mod)
        {

            mod.IsDeleted = Library.Classes.Admin.League.League.HideLeague(new Guid(mod.Id));
            return View(mod);
        }

        [Authorize]
        public ActionResult HideLeague()
        {
            IdModel mod = new IdModel();
            mod.IsDeleted = false;
            return View(mod);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SetOwner(IdModel mod)
        {
            var memberId = RDN.Library.Classes.Account.User.GetMemberId(mod.Id2);
            mod.IsSuccess = RDN.Library.Classes.League.LeagueFactory.ToggleOwnerToLeague(new Guid(mod.Id), memberId, LeagueOwnersEnum.Owner);
            try
            {
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(LibraryConfig.InternalSite + UrlManager.URL_TO_CLEAR_MEMBER_CACHE + memberId.ToString()));
                WebClient client1 = new WebClient();
                client1.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_MEMBER_CACHE_API + memberId.ToString()));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(mod);
        }

        [Authorize]
        public ActionResult SetOwner()
        {
            IdModel mod = new IdModel();
            mod.IsSuccess = false;
            return View(mod);
        }
        [Authorize]
        [HttpPost]
        public ActionResult DeleteMemberFromLeague(IdModel mod)
        {
            mod.IsSuccess = LeagueFactory.RemoveMemberFromLeague(new Guid(mod.Id), new Guid(mod.Id2));
            try
            {
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(LibraryConfig.InternalSite + UrlManager.URL_TO_CLEAR_MEMBER_CACHE + mod.Id2));
                WebClient client1 = new WebClient();
                client1.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_MEMBER_CACHE_API + mod.Id2));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(mod);
        }

        [Authorize]
        public ActionResult DeleteMemberFromLeague()
        {
            IdModel mod = new IdModel();
            mod.IsSuccess = false;
            return View(mod);
        }
        [Authorize]
        public ActionResult LeagueDocuments()
        {
            DocumentRepository repo = new DocumentRepository();
            return View(repo);

        }
        [Authorize]
        [HttpPost]
        public ActionResult LeagueDocuments(DocumentRepository repo)
        {

            repo.Documents = DocumentRepository.GetLeagueDocumentsAll(repo.OwnerId);

            return View(repo);

        }
        public ActionResult RestoreLeagueDoc(string docId, string ownerId)
        {

            DocumentRepository.RestoreDocument(Convert.ToInt64(docId));
            return LeagueDocuments(new DocumentRepository() { OwnerId = new Guid(ownerId) });

        }

        [Authorize]
        public ActionResult LeagueJoinCodes()
        {
            var leangueJoinCodes =
                Library.Classes.Admin.League.Classes.LeagueJoinCodes.GetAllLeagueJoinCodes().OrderBy(x => x.Name);
            return View(leangueJoinCodes);
        }

        [Authorize]
        public ActionResult SubscriptionsOfAllLeagues()
        {
            var leagues = Library.Classes.Admin.League.League.GetSubscriptionInformationOfAllLeagues().OrderBy(x => x.SubscriptionPeriodEnds);
            return View(leagues);
        }
        [HttpPost]
        [Authorize]
        public ActionResult SubscriptionsOfAllLeagues(List<League> l)
        {
            Guid leagueId = new Guid();

            leagueId = new Guid(Request.Form["updateLeague"]);
            var dt = Request.Form["sub-" + leagueId.ToString()].Trim();
            try
            {
                Library.Classes.League.LeagueFactory.UpdateLeagueSubscriptionPeriod(Convert.ToDateTime(dt), leagueId);
                WebClient client = new WebClient();
                client.DownloadData(LibraryConfig.InternalSite + UrlManager.URL_TO_CLEAR_LEAGUE_MEMBER_CACHE + leagueId);

                client.DownloadData(new Uri(LibraryConfig.ApiSite+ UrlManager.URL_TO_CLEAR_LEAGUE_MEMBER_CACHE_API + leagueId));
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType(), additionalInformation: dt);
            }
            var leagues = Library.Classes.Admin.League.League.GetSubscriptionInformationOfAllLeagues().OrderBy(x => x.SubscriptionPeriodEnds);
            return View(leagues);
        }

        [Authorize]
        public ActionResult Pendings()
        {
            var output = new GenericEnumerableModel<RDN.Library.Classes.Admin.League.Classes.League>();
            output.Model = Library.Classes.Admin.League.League.GetPendingLeagues();
            return View(output);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ApproveLeague()
        {
            var leagueId = Request.Form["Id"];
            var result = Library.Classes.Admin.League.League.ApproveLeague(leagueId);
            AddMessage(result
                           ? new SiteMessage
                                 {
                                     MessageType = SiteMessageType.Success,
                                     Message = "The league has been approved successfully"
                                 }
                           : new SiteMessage
                                 {
                                     MessageType = SiteMessageType.Error,
                                     Message = "An error occured and the league could not be approved"
                                 });

            return RedirectToAction("Pendings");
        }
        [Authorize]
        [HttpPost]
        public ActionResult ApproveLeagueForFederation()
        {
            var leagueId = Request.Form["Id"];
            var result = Library.Classes.Admin.League.League.ApproveLeagueForFederation(leagueId);
            AddMessage(result
                           ? new SiteMessage
                           {
                               MessageType = SiteMessageType.Success,
                               Message = "The league has been approved successfully"
                           }
                           : new SiteMessage
                           {
                               MessageType = SiteMessageType.Error,
                               Message = "An error occured and the league could not be approved"
                           });
            return RedirectToAction("Pendings");
        }

        [Authorize]
        [HttpPost]
        public ActionResult RejectLeague()
        {
            var leagueId = Request.Form["Id"];
            var rejectMessage = Request.Form["rejectMessage"];
            var result = Library.Classes.Admin.League.League.RejectLeague(leagueId, rejectMessage);
            AddMessage(result
                           ? new SiteMessage
                                 {
                                     MessageType = SiteMessageType.Success,
                                     Message = "The league has been rejected successfully"
                                 }
                           : new SiteMessage
                                 {
                                     MessageType = SiteMessageType.Error,
                                     Message = "An error occured and the league could not be approved"
                                 });
            return RedirectToAction("Pendings");
        }

        [Authorize]
        public ActionResult DeleteOldLeagueDocuments()
        {
            IdModel repo = new IdModel();

            return View(repo);
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteOldLeagueDocuments(IdModel model)
        {
            model.IsDeleted = DocumentRepository.DeleteOldLeagueDocuments();

            return View(model);
        }

     

    }
}
