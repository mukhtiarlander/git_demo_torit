using RDN.League.Models.Filters;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.League;
using RDN.Library.Util;
using RDN.Library.Util.Enum;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.League.Controllers.League
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class JobBoardController : BaseController
    {
        //
        // GET: /JobBoard/
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult AddJob()
        {
            return View();
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult AddJob(JobBoard jobBoard)
        {

            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                jobBoard.LeagueId = league.LeagueId;
                jobBoard.JobCreator = memId;

                bool execute = RDN.Library.Classes.League.JobBoard.AddNewJob(jobBoard);



            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Redirect(Url.Content("~/league/jobboard?u=" + SiteMessagesEnum.sac));
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [Authorize]
        public ActionResult ViewJobBoard()
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Job Saved.";
                    this.AddMessage(message);

                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sac.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "New Job Successfully Added.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.de.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Job Successfully Deleted.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.et.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Job Successfully Filled.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.cl.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Job Successfully Closed.";
                    this.AddMessage(message);
                }
                
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                if (league != null)
                    SetCulture(league.CultureSelected);

                var jobLists = RDN.Library.Classes.League.JobBoard.GetJobList(league.LeagueId);
                return View(jobLists);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return View();
        }

        [Authorize]
        public ActionResult EditJob(long id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                {
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                }

                var Data = RDN.Library.Classes.League.JobBoard.GetData(id, new Guid(leagueId));

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
        public ActionResult EditJob(JobBoard jobs)
        {
            try
            {
                bool execute = RDN.Library.Classes.League.JobBoard.UpdateJob(jobs);

                return Redirect(Url.Content("~/league/jobboard?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }

        [Authorize]
        public ActionResult CloseJob(long id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                {
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                }


                var Data = RDN.Library.Classes.League.JobBoard.CloseJob(id, new Guid(leagueId));
                return Redirect(Url.Content("~/league/jobboard?u=" + SiteMessagesEnum.cl));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }

        [Authorize]
        public ActionResult DeleteJob(long id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                {
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                }

                var Data = RDN.Library.Classes.League.JobBoard.DeleteJob(id, new Guid(leagueId));
                return Redirect(Url.Content("~/league/jobboard?u=" + SiteMessagesEnum.de));

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }
        [Authorize]
        public ActionResult FillJob(long id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                {
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                }

                var Data = RDN.Library.Classes.League.JobBoard.FilledJob(id, new Guid(leagueId));
                return Redirect(Url.Content("~/league/jobboard?u=" + SiteMessagesEnum.et));

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }

        [Authorize]
        public ActionResult ViewJob(long id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                {
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                }
                var league = MemberCache.GetLeagueOfMember(memId);
                if (league != null)
                    SetCulture(league.CultureSelected);

                var Data = RDN.Library.Classes.League.JobBoard.GetData(id, new Guid(leagueId));
                if (!String.IsNullOrEmpty(Data.JobDesc))
                {
                    Data.JobDesc = Data.JobDesc.Replace(Environment.NewLine, "<br/>");
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
