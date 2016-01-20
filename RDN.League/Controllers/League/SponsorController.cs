using Common.Sponsors.Classes;
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

    public class SponsorController : BaseController
    {

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult AddNewSponsor()
        {

            return View();
        
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult AddSponsor(SponsorItem sponsor)
        {

            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                sponsor.OwnerId = league.LeagueId;
                sponsor.AddedById = memId;

                bool execute = SponsorManager.Add_New_Sponsor(sponsor);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Redirect(Url.Content("~/league/sponsors?u=" + SiteMessagesEnum.sac));
        }

        [Authorize]
        public ActionResult ViewSponsors()
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Sponsor information updated.";
                    this.AddMessage(message);

                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sac.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "New Sponsor Successfully Added.";
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


                var sponsorLists = SponsorManager.GetSponsorList(league.LeagueId);
                return View(sponsorLists);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return View();
        }


        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsSecretary= true)]
        public ActionResult EditSponsor(long id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                {
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                }

                var Data = SponsorManager.GetSponsorIem(id, new Guid(leagueId));

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
        public ActionResult EditSponsor(SponsorItem oSponsor)
        {
            try
            {
                bool execute = SponsorManager.UpdateSponsorInfo(oSponsor);

                return Redirect(Url.Content("~/league/sponsors?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }

        [Authorize]
        public ActionResult DeleteSponsor(long id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                {
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                }

                var Data = SponsorManager.DeleteSponsor(id, new Guid(leagueId));
                return Redirect(Url.Content("~/league/Sponsors?u=" + SiteMessagesEnum.de));

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }
        [Authorize]
        public ActionResult ViewSponsor(long id, string leagueId)
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

                var Data = SponsorManager.GetSponsorIem(id, new Guid(leagueId));
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
        [Authorize]
        public ActionResult UseCode(long id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                {
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                }
				long usedCount;
                var Data = SponsorManager.UseCode(id, new Guid(leagueId), out usedCount);
				if (Data)
				{
					return Json(usedCount, JsonRequestBehavior.AllowGet);
				}
                return Redirect(Url.Content("~/league/Sponsors?u=" + SiteMessagesEnum.et));

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }

       
    }
}
