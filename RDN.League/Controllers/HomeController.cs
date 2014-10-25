using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.League.Models.Filters;
using RDN.Library.Util;
using System.Xml.Linq;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Account;
using RDN.Library.Classes.Account.Enums;
using RDN.League.Models.Utilities;
using RDN.Library.Classes.Federation;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Cache;
using RDN.Library.Classes.Federation.Enums;
using System.Collections.Specialized;
using RDN.League.Models.Enum;
using RDN.Library.Util.Enum;
using RDN.League.Models.League;
using RDN.Library.Classes.Calendar;
using RDN.Library.Classes.Calendar.Enums;
using RDN.Library.Classes.Forum;
using RDN.Library.Classes.Controls.Voting;
using System.Threading;
using RDN.Portable.Classes.Account.Enums;
using RDN.Portable.Classes.Controls.Calendar.Enums;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class HomeController : BaseController
    {
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult Index()
        {
            try
            {
                ViewData.Add("tumblr", getBlog());

                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.na.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "You do not have access to that page. Please contact info@rdnation.com if you think this is an error.";
                    this.AddMessage(message);
                }
                else if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sww.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Something went wrong, the error was sent to the developers, please try again later.";
                    this.AddMessage(message);
                }
                else if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.clus.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Error;
                    message.Message = "Changing to that league was unsuccessful.  Are you a member of that league?";
                    this.AddMessage(message);
                }
                else if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.cls.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Changing leagues was successful.";
                    this.AddMessage(message);
                }
                var memId = RDN.Library.Classes.Account.User.GetMemberId();

                var member = MemberCache.GetMemberDisplay(memId);
                //removes the user and logs them out.
                //because if member is null, they seem like they don't have an account.
                if (member == null)
                {
                    ErrorDatabaseManager.AddException(new Exception("Member Was Null"), GetType(), additionalInformation: memId.ToString());
                    return Redirect("~/account/logout");
                }
                if (member.DefaultPositionType == DefaultPositionEnum.The_Owner_of_a_Federation &&
                    !MemberCache.IsOwnerOfFederation(memId))
                {
                    SiteMessage message = new SiteMessage();
                    message.Link = UrlHelper.GenerateContentUrl("~/Federation/Setup", HttpContext);
                    message.MessageType = SiteMessageType.Info;
                    message.LinkText = "Please Setup Your Federation";
                    this.AddMessage(message);
                }
                else if (member.DefaultPositionType == DefaultPositionEnum.The_Owner_of_a_Team_League &&
           MemberCache.GetLeagueIdOfMember(memId) == new Guid())
                {
                    SiteMessage message = new SiteMessage();
                    message.Link = UrlHelper.GenerateContentUrl("~/League/Setup", HttpContext);
                    message.MessageType = SiteMessageType.Info;
                    message.LinkText = "Please Setup Your League";
                    this.AddMessage(message);
                }
                HomeModel model = new HomeModel();
                bool isAttendanceManagerOrBetter = MemberCache.IsAttendanceManagerOrBetterOfLeague(member.MemberId);
                model.League = MemberCache.GetLeagueOfMember(memId);
                if (model.League != null)
                {

                    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(model.League.CultureSelected);

                    var calId = MemberCache.GetCalendarIdForMemberLeague(memId);
                    model.Calendar = CalendarFactory.GetCalendarEvents(calId, 5, member.MemberId, isAttendanceManagerOrBetter);
                    model.Calendar.Events.AddRange(MemberCache.GetMemberBirthdays(memId, DateTime.UtcNow, DateTime.UtcNow.AddDays(2)));
                    model.Calendar.Events.AddRange(MemberCache.GetMemberStartDates(memId, DateTime.UtcNow, DateTime.UtcNow.AddDays(2)));
                    model.Calendar.OwnerEntity = CalendarOwnerEntityEnum.league;

                    model.Polls = VotingFactory.GetPollsNotVotedOn(model.League.LeagueId, memId);
                    model.ForumId = MemberCache.GetForumIdForMemberLeague(memId);
                    model.Forum = Forum.GetForumTopicsJsonUnread(model.ForumId, 0, memId, 10);
                }
                return View(model);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View();
        }

        private static Tumblr getBlog()
        {
            try
            {
                XDocument xmlDoc = XDocument.Load("http://blog.rdnation.com/api/read");
                Tumblr tm = Tumblr.GetTumblr("http://blog.rdnation.com/api/read", true);
                tm.Posts.Count = "3";


                return tm;
            }
            catch
            {
            }
            return new Tumblr();
        }


    }
}
