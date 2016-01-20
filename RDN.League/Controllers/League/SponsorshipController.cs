using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using Common.Sponsors.Classes;
using RDN.League.Models.Filters;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Util;
using RDN.Library.Util.Enum;

namespace RDN.League.Controllers.League
{
    public class SponsorshipController : BaseController
    {

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult AddNewSponsorship()
        {

            return View();

        }

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult AddSponsorship(SponsorshipItem sponsorship)
        {

            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                sponsorship.OwnerId = league.LeagueId;
                bool execute = SponsorShipManager.Add_New_Sponsorship(sponsorship);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Redirect(Url.Content("~/league/sponsorships?u=" + SiteMessagesEnum.sac));
        }

        // GET: Sponsorships
        public ActionResult ViewSponsorships()
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Sponsorship information updated.";
                    this.AddMessage(message);

                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sac.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "New Sponsorship Successfully Added.";
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


                var sponsorshipList = SponsorShipManager.GetSponsorshipList(league.LeagueId);
                return View(sponsorshipList);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return View();
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsSecretary = true)]
        public ActionResult EditSponsorship(long id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                {
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                }

                var Data = SponsorShipManager.GetData(id, new Guid(leagueId));

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
        public ActionResult EditSponsorship(SponsorshipItem oSponsorship)
        {
            try
            {
                bool execute = SponsorShipManager.UpdateSponsorInfo(oSponsorship);

                return Redirect(Url.Content("~/league/sponsorships?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }

       
        [Authorize]
        public ActionResult ViewSponsorship(long id, string leagueId)
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

                var Data = SponsorShipManager.GetData(id, new Guid(leagueId));
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