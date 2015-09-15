using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.League.Links;
using RDN.Library.Util;
using RDN.Library.Util.Enum;

namespace RDN.League.Controllers.League
{
    //  [RequireHttps]

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class LinksController : BaseController
    {
                
        public ActionResult ViewLinks()
        {
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
            
            string updated = nameValueCollection["u"];

            if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.s.ToString())
            {
                SiteMessage message = new SiteMessage();
                message.MessageType = SiteMessageType.Info;
                message.Message = "Item information updated.";
                this.AddMessage(message);

            }
            if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sac.ToString())
            {
                SiteMessage message = new SiteMessage();
                message.MessageType = SiteMessageType.Info;
                message.Message = "New Item Successfully Added.";
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

            var links = RDN.Library.Classes.League.Links.LinkManager.GetLeagueLinks(league.LeagueId);
            return View(links);            
        }
        
        public ActionResult AddNewLink()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddNewLink(LinkManager linkManager)
        {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);

                linkManager.LinksForLeague = league.LeagueId;
                linkManager.LinksAddByMember = memId;

                bool execute = RDN.Library.Classes.League.Links.LinkManager.AddLink(linkManager);
                
                if(execute)
                    return Redirect(Url.Content("~/league/links/all?u=" + SiteMessagesEnum.sac));

                return View();
        }

        
        public ActionResult Delete(long id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                {
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                }

                var Data = RDN.Library.Classes.League.Links.LinkManager.DeleteItem(id, new Guid(leagueId));
                return Redirect(Url.Content("~/league/links/all?u=" + SiteMessagesEnum.de));

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }


        public ActionResult LinkDetail(long id, string leagueId)
        {
            var memId = RDN.Library.Classes.Account.User.GetMemberId();
            var league = MemberCache.GetLeagueOfMember(memId);

            if (league != null)
                SetCulture(league.CultureSelected);

            var Data = RDN.Library.Classes.League.Links.LinkManager.GetLeagueLink(id, new Guid(leagueId));
           
            return View(Data);
        }

        public ActionResult EditLink(long id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();

                var Data = RDN.Library.Classes.League.Links.LinkManager.GetLeagueLink(id, new Guid(leagueId));

                return View(Data);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditLink(LinkManager linkManager)
        {
            try
            {
                bool execute = RDN.Library.Classes.League.Links.LinkManager.EditLink(linkManager);

                return Redirect(Url.Content("~/league/links/all?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

    }
}
