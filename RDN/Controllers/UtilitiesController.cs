using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Utilities;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Game.Enums;
using RDN.Library.ViewModel;
using RDN.Library.Classes.Game;
using RDN.Library.Cache.Singletons;
using RDN.Library.Classes.Payment.Paywall;
using RDN.Library.Classes.Calendar;
using RDN.Library.Classes.Calendar.Enums;
using RDN.Utilities.Dates;
using RDN.Library.Classes.Communications;
using RDN.Library.Classes.Communications.Enums;
using RDN.Library.Cache;
using RDN.Portable.Classes.Controls.Calendar;
using System.Text;
using RDN.Library.Classes.Config;

namespace RDN.Controllers
{
    public class UtilitiesController : Controller
    {
        public ActionResult ClearTournamentCache()
        {
            try
            {
                SiteCache.ClearTournaments();
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadCalendarView(string lid, string startDt, string endDt)
        {
            Calendar cal = new Calendar();

            try
            {
                cal.CalendarId = new Guid();

                if (new Guid(lid) != new Guid())
                {
                    DateTime st = DateTimeExt.FromUnixTime(Convert.ToInt64(startDt));
                    DateTime en = DateTimeExt.FromUnixTime(Convert.ToInt64(endDt));
                    var topics = CalendarFactory.GetPublicCalendarOfLeagueScheduleForView(new Guid(lid), st, en);
                    return Json(new { events = topics }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Json(new { view = "" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CurrentlyViewingPaywall(string pId, string p)
        {
            try
            {
                PaywallViewers.Instance.UpdateCurrentlyViewing(Convert.ToInt64(pId), p);
                Paywall.UpdatePaywallInvoiceForViewingTime(Convert.ToInt64(pId), p);
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PostConversation(string id, string convoType, string chat)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                Conversation postedId = null;
                ConversationTypeEnum convType = (ConversationTypeEnum)Enum.Parse(typeof(ConversationTypeEnum), convoType);
                if (convType == ConversationTypeEnum.Game)
                    postedId = GameServerViewModel.PostConversationText(new Guid(id), chat, memId);
                else if (convType == ConversationTypeEnum.Tournament)
                    postedId = Tournament.PostConversationText(new Guid(id), chat, memId);
                else if (convType == ConversationTypeEnum.Event)
                    postedId = CalendarEventFactory.PostConversationText(new Guid(id), chat, memId);
                if (postedId != null)
                    Conversation.Instance.AddConversation(postedId);
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetConversation(string id, string convoType)
        {
            try
            {
                Guid idd = new Guid(id);
                List<Conversation> con = new List<Conversation>();
                if (Conversation.Instance.Conversations == null)
                    Conversation.Instance.Conversations = new List<Conversation>();
                var conTemp = Conversation.Instance.Conversations.Where(x => x.OwnerId == idd);

                if (conTemp.Count() > 0)
                {
                    con = conTemp.OrderByDescending(x => x.Created).Take(30).OrderByDescending(x => x.Created).ToList();
                    return Json(new { convo = con }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ConversationTypeEnum convType = (ConversationTypeEnum)Enum.Parse(typeof(ConversationTypeEnum), convoType);
                    if (convType == ConversationTypeEnum.Game)
                        con = GameServerViewModel.GetConversation(new Guid(id));
                    else if (convType == ConversationTypeEnum.Tournament)
                        con = Tournament.GetConversation(new Guid(id));
                    else if (convType == ConversationTypeEnum.Event)
                        con = CalendarEventFactory.GetConversation(new Guid(id));
                    foreach (var c in con)
                    {
                        Conversation.Instance.AddConversation(c.OwnerId, c.Id, c.MemberName, c.Chat, c.Created);
                    }
                }
                return Json(new { convo = con }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: id + ":" + convoType);
            }
            return Json(new { convo = false }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// adds a node to the sitemap.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="modified"></param>
        /// <returns></returns>
        public ActionResult AddNodeToSiteMap(string url, bool modified)
        {
            //don't want the sitemap to have lostpassword links.
            //Verify  Name
            if (!url.Contains("%") && !url.Contains("c=") && !url.Contains("lostpassword") && !url.Contains("verifyname") && !url.Contains("receipt") && !url.Contains("returnsite") && !url.Contains("returnurl") && !url.Contains("problem.error") && !url.Contains("returnsite") && !url.Contains("login") && !url.Contains("email") && !url.Contains("@"))
                SitemapHelper.AddNode(url, modified);
            return Json(new { answer = true }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult Robot()
        {
            StringBuilder robotsResult = new StringBuilder();
            robotsResult.Append("Sitemap: " + LibraryConfig.PublicSite + "/sitemap" + Environment.NewLine);
            robotsResult.Append("Disallow: login" + Environment.NewLine);
            robotsResult.Append("Disallow: /signup" + Environment.NewLine);
            robotsResult.Append("Disallow: /utilities" + Environment.NewLine);
            robotsResult.Append("Disallow: /store" + Environment.NewLine);
            robotsResult.Append("Disallow: /" + RDN.Library.Classes.Config.LibraryConfig.SportNameForUrl + "-store" + Environment.NewLine);

            return Content(robotsResult.ToString(), "text/plain");
        }

        public ActionResult SearchForDerbyName(string name)
        {
            var derbyNames = RDN.Library.Classes.Account.User.SearchForDerbyName(name);
            return Json(new { result = true, names = derbyNames }, JsonRequestBehavior.AllowGet);
        }
    }
}
