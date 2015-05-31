using RDN.League.Models.Filters;
using RDN.Library.Cache;
using RDN.Library.Cache.ManagementSite;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Facebook;
using RDN.Library.Classes.League;
using RDN.Library.Classes.Officials;
using RDN.Library.Classes.Social.Facebook;
using RDN.Library.Classes.Social.Twitter;
using RDN.Library.Util;
using RDN.Library.Util.Enum;
using RDN.Portable.Classes.Federation.Enums;
using RDN.Portable.Classes.Games.Officiating;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.League.Controllers.League
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class OfficiatingController : BaseController
    {
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult AddRequest()
        {

            try
            {
                IEnumerable<RuleSetsUsedEnum> ruleSets = Enum.GetValues(typeof(RuleSetsUsedEnum))
                                                           .Cast<RuleSetsUsedEnum>();
                var list = (from a in ruleSets
                            select new SelectListItem
                            {
                                Text = RDN.Portable.Util.Enums.EnumExt.ToFreindlyName(a),
                                Value = ((int)a).ToString()
                            });

                ViewBag.RuleSets = new SelectList(list, "Value", "Text");

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return View();

        }

        [HttpPost]
        [ValidateInput(false)]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult AddRequest(RequestDA oRequest)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                oRequest.RequestCreator = memId;
                var executeId = RequestFactory.Add_New_Request(oRequest);
                ManagementCache.AddOfficiatingRequestsCount(1);

                
                var token = FacebookData.GetLatestAccessToken();

                FacebookFactory.Initialize(token).GetPageAuthorization(Library.Classes.Config.LibraryConfig.FacebookPageName2, Library.Classes.Config.LibraryConfig.FacebookPageId2)
                        .PostToFanPage("Officials Request: " + oRequest.TeamsPlaying + " @ " + oRequest.Date.GetValueOrDefault().ToShortDateString() + "\n\n" + RDN.Portable.Config.ServerConfig.WEBSITE_DEFAULT_LOCATION + "/officiating-requests/view/" + executeId, "", "", "", "", "");
                 
                 
                TwitterFactory.Initialize(Library.Classes.Config.LibraryConfig.TwitterConsumerKey, Library.Classes.Config.LibraryConfig.TwitterConsumerSecret, Library.Classes.Config.LibraryConfig.TwitterToken, Library.Classes.Config.LibraryConfig.TwitterTokenSecret)
                          .SendMessage("Officials Request: " + oRequest.TeamsPlaying + " @ " + oRequest.Date.GetValueOrDefault().ToShortDateString() + " #rollerderby " + RDN.Portable.Config.ServerConfig.WEBSITE_DEFAULT_LOCATION + "/officiating-requests/view/" + executeId);




            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/officiating/requests?u=" + SiteMessagesEnum.sac));

        }


#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult Requests()
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Request updated.";
                    this.AddMessage(message);

                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sac.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "New Request Successfully Added.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.de.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully Deleted.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.et.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully Used.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.cl.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully Closed.";
                    this.AddMessage(message);
                }
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                if (league != null)
                    SetCulture(league.CultureSelected);


                var requestLists = RequestFactory.GetRequestList();
                return View(requestLists);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return View();
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult EditRequest(long id, string memberId)
        {
            try
            {
                IEnumerable<RuleSetsUsedEnum> ruleSets = Enum.GetValues(typeof(RuleSetsUsedEnum))
                                                          .Cast<RuleSetsUsedEnum>();
                var list = from a in ruleSets
                           select new SelectListItem
                           {
                               Text = RDN.Portable.Util.Enums.EnumExt.ToFreindlyName(a),
                               Value = ((int)a).ToString()
                           };

                ViewBag.RuleSets = new SelectList(list, "Value", "Text");

                var Data = RequestFactory.GetData(id, new Guid(memberId));

                return View(Data);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true)]
        [ValidateInput(false)]
        public ActionResult EditRequest(RequestDA oRequest)
        {
            try
            {
                bool execute = RequestFactory.UpdateRequest(oRequest);

                return Redirect(Url.Content("~/officiating/requests?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult DeleteRequest(long id, string memberId)
        {
            try
            {

                var Data = RequestFactory.DeleteRequest(id, new Guid(memberId));
                ManagementCache.AddOfficiatingRequestsCount(-1);
                return Redirect(Url.Content("~/officiating/requests?u=" + SiteMessagesEnum.de));

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }

        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult ViewRequest(long id, string memberId)
        {
            try
            {

                var Data = RequestFactory.GetData(id, new Guid(memberId));
                if (!String.IsNullOrEmpty(Data.Description))
                {
                    Data.Description = Data.Description.Replace(Environment.NewLine, "<br/>");
                }
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
