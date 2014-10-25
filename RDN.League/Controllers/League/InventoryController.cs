using RDN.League.Models.Filters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.League;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Util.Enum;
using RDN.Library.Util;

namespace RDN.League.Controllers.League
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class InventoryController : BaseController
    {
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsInventoryTracker=true)]
        public ActionResult AddNewItem()
        {

            return View();

        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsInventoryTracker = true)]
        public ActionResult AddNewItem(ItemInfoDA Item)
        {

            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                Item.ItemsForLeague = league.LeagueId;
                Item.ItemAddByMember = memId;

                bool execute = RDN.Library.Classes.League.ItemInfoDA.Add_New_Item(Item);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Redirect(Url.Content("~/league/inventory/all?u=" + SiteMessagesEnum.sac));
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult ViewItems()
        {
            try
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


                var itemLists = RDN.Library.Classes.League.ItemInfoDA.GetItemList(league.LeagueId);
                return View(itemLists);
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
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsInventoryTracker = true)]
        public ActionResult EditItem(long id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                //if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                //{
                //    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                //}

                var Data = RDN.Library.Classes.League.ItemInfoDA.GetData(id, new Guid(leagueId));

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
        [Authorize]
        [ValidateInput(false)]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsInventoryTracker = true)]
        public ActionResult EditItem(ItemInfoDA oItem)
        {
            try
            {
                bool execute = RDN.Library.Classes.League.ItemInfoDA.UpdateItemInfo(oItem);

                return Redirect(Url.Content("~/league/inventory/all?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsInventoryTracker = true)]
        public ActionResult DeleteItem(long id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                {
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                }

                var Data = RDN.Library.Classes.League.ItemInfoDA.DeleteItem(id, new Guid(leagueId));
                return Redirect(Url.Content("~/league/Inventory/all?u=" + SiteMessagesEnum.de));

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsInventoryTracker = true)]
        public ActionResult ViewItem(long id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                //if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                //{
                //    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                //}

                var league = MemberCache.GetLeagueOfMember(memId);
                if (league != null)
                    SetCulture(league.CultureSelected);

                var Data = RDN.Library.Classes.League.ItemInfoDA.GetData(id, new Guid(leagueId));
                if (!String.IsNullOrEmpty(Data.Notes))
                {
                    Data.Notes = Data.Notes.Replace(Environment.NewLine, "<br/>");
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
