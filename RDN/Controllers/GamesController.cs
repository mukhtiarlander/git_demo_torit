using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Util;
using RDN.Library.Util.Enum;
using RDN.Library.Cache.Site;
using RDN.Library.Classes.Officials;
using RDN.Library.Classes.Error;
using RDN.Portable.Classes.Games.Officiating;
using System.Configuration;
using RDN.Portable.Classes.Controls.Message.Enums;


namespace RDN.Controllers
{
    public class GamesController : BaseController
    {
        //
        // GET: /Games/

        public ActionResult Index()
        {
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
            string updated = nameValueCollection["u"];

            if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.dex.ToString())
            {
                SiteMessage message = new SiteMessage();
                message.MessageType = SiteMessageType.Info;
                message.Message = "Couldn't Find Game.  Was It Published?";
                this.AddMessage(message);
            }
            var games = PublicSiteCache.GetPastGames();

            return View(games);
        }


        public ActionResult OfficiatingRequests()
        {
            RequestsModel model = new RequestsModel();
            model.Requests = RequestFactory.GetRequestList();
            //model.UrlToRequest = ConfigurationManager.AppSettings["InternalSite"] + "officiating/requests";
            model.UrlToRequest = Library.Classes.Config.LibraryConfig.InternalSite + "officiating/requests";
            return View(model);

        }

        public ActionResult ViewRequest(long id)
        {
            try
            {

                var Data = RequestFactory.GetData(id);
                if (!String.IsNullOrEmpty(Data.Description))
                {
                    Data.Description = Data.Description.Replace(Environment.NewLine, "<br/>");
                }

               // Data.UrlToRequest = ConfigurationManager.AppSettings["InternalSite"] + "officiating/requests";
                Data.UrlToRequest = Library.Classes.Config.LibraryConfig.InternalSite + "officiating/requests";
                Data.UrlToContact = Library.Classes.Config.LibraryConfig.InternalSite + "messages/new/" + GroupOwnerTypeEnum.officiating.ToString() + "/" + Data.RequestCreator.ToString().Replace("-", "");
                return View(Data);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        public ActionResult AllBoutList()
        {
            try
            {

                var boutLists = RDN.Library.Classes.League.BoutList.GetBoutList();
                return View(boutLists);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        public ActionResult ViewBoutEvent(long ChallengeId)
        {
            try
            {
                var Data = RDN.Library.Classes.League.BoutList.GetData(ChallengeId);//,leagueid);

                return View(Data);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }


    }
}
