using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.League.Models.Game;
using RDN.League.Models.OutModel;
using RDN.League.Models.Utilities;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Federation.Enums;
using RDN.Library.Util.Enum;
using RDN.Library.Classes.League;
using System.Collections.Specialized;
using RDN.Library.Util;
using RDN.Portable.Classes.Federation.Enums;
using RDN.Library.Classes.Facebook;
using RDN.Library.Classes.Social.Facebook;
using System.Configuration;
using RDN.Library.Classes.Social.Twitter;
using RDN.Library.Classes.Config;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class BoutChallengeController : BaseController
    {
        /// <summary>
        /// default page size for the tables
        /// </summary>


        [Authorize]
        public ActionResult ViewAllBoutRequest()
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Saved Request.";
                    this.AddMessage(message);

                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sac.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully Added Request.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.de.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully Deleted Request.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.cl.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully Closed Request.";
                    this.AddMessage(message);
                }

                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                if (league != null)
                    SetCulture(league.CultureSelected);


                var boutLists = RDN.Library.Classes.League.BoutList.GetBoutList();
                return View(boutLists);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return View();
        }
        [Authorize]
        public ActionResult BoutChallengeRequest()
        {
            try
            {
                IEnumerable<RuleSetsUsedEnum> ruleSets = Enum.GetValues(typeof(RuleSetsUsedEnum))
                                                           .Cast<RuleSetsUsedEnum>();
                var list = (from a in ruleSets
                            select new SelectListItem
                     {
                         Text = RDN.Utilities.Enums.EnumExt.ToFreindlyName(a),
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
        [Authorize]
        [ValidateInput(false)]
        public ActionResult BoutChallengeRequest(BoutList BoutChallenge)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                var executeId = RDN.Library.Classes.League.BoutList.SaveBoutRequestList(league.LeagueId, BoutChallenge);

                var token = FacebookData.GetLatestAccessToken();

                //ConfigurationManager.AppSettings has been moved to LibraryConfig as RDN-18 request
                FacebookFactory.Initialize(token).GetPageAuthorization(LibraryConfig.FacebookPageName2, LibraryConfig.FacebookPageId2)
                       .PostToFanPage("Bout Challenge: " + league.Name + " @ " + BoutChallenge.StartDateOfEvent.Date.ToShortDateString() + "\n\n" + RDN.Portable.Config.ServerConfig.WEBSITE_DEFAULT_LOCATION + "/bout-challenge/view/" + executeId, "", "", "", "", "");
                try
                {

                    TwitterFactory.Initialize(LibraryConfig.TwitterConsumerKey, LibraryConfig.TwitterConsumerSecret, LibraryConfig.TwitterToken, LibraryConfig.TwitterTokenSecret)
                              .SendMessage("Bout Challenge: " + league.Name + " @ " + BoutChallenge.StartDateOfEvent.Date.ToShortDateString() + " #rollerderby " + RDN.Portable.Config.ServerConfig.WEBSITE_DEFAULT_LOCATION + "/bout-challenge/view/" + executeId);
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/boutchallenge/view/all?u=" + SiteMessagesEnum.sac));

        }
        [Authorize]
        public ActionResult CloseBoutRequest(long id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                {
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                }


                var Data = RDN.Library.Classes.League.BoutList.CloseEvent(id, new Guid(leagueId));//,leagueid);
                return Redirect(Url.Content("~/boutchallenge/view/all?u=" + SiteMessagesEnum.cl));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }

        [Authorize]
        public ActionResult DeleteBoutRequest(int id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                {
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                }


                var Data = RDN.Library.Classes.League.BoutList.DeleteEvent(id, new Guid(leagueId));//,leagueid);
                return Redirect(Url.Content("~/boutchallenge/view/all?u=" + SiteMessagesEnum.de));


            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }

        [Authorize]
        public ActionResult EditBoutRequest(long id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                {
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                }

                IEnumerable<RuleSetsUsedEnum> ruleSets = Enum.GetValues(typeof(RuleSetsUsedEnum))
                                                          .Cast<RuleSetsUsedEnum>();
                var list = from a in ruleSets
                           select new SelectListItem
                           {
                               Text = RDN.Portable.Util.Enums.EnumExt.ToFreindlyName(a),
                               Value = ((int)a).ToString()
                           };

                ViewBag.RuleSets = new SelectList(list, "Value", "Text");


                var Data = RDN.Library.Classes.League.BoutList.GetData(id, new Guid(leagueId));//,leagueid);

                return View(Data);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult EditBoutRequest(BoutList BoutChallenge)
        {
            try
            {
                bool execute = RDN.Library.Classes.League.BoutList.UpdateEvent(BoutChallenge);

                return Redirect(Url.Content("~/boutchallenge/view/all?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }
        [Authorize]
        public ActionResult ViewEvent(long ChallengeId, string leagueId)
        {
            try
            {
                var Data = RDN.Library.Classes.League.BoutList.GetData(ChallengeId, new Guid(leagueId));//,leagueid);

                return View(Data);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/"));
        }

    }
}
