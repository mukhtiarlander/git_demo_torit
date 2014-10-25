using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Mobile;
using RDN.Mobile.Classes.Account;
using RDN.Utilities.Account;
using RDN.Portable.Classes.Account;
using RDN.Portable.Settings.Enums;
using RDN.Portable.Network;
using RDN.Portable.Account;
using RDN.Library.Cache.ManagementSite;

namespace RDN.Api.Controllers
{
    public class AccountController : Controller
    {
        [ValidateInput(false)]
        public ActionResult CreateMobileNotificationToken()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<NotificationMobileJson>(ref stream);
                    var notifications = SiteCache.GetMobileNotifications();
                    var not = notifications.Where(x => x.NotificationId == ob.NotificationId && x.MemberId == ob.MemberId).FirstOrDefault();

                    if (not == null)
                    {
                        MobileNotificationFactory.AddMobileNotification(ob);
                        SiteCache.ClearMobileNotification();
                    }
                    return Json(new MobileNotification() { IsSuccessful = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());

            }
            return Json(new MobileNotification() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public ActionResult getNotifications()
        {
            try
            {

                return Json(SiteCache.GetMobileNotifications(), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());

            }
            return Json(new MobileNotification() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public ActionResult SendTestNotify()
        {
            try
            {
                MobileNotification.SendNotifications("New Game", "RCR vs GRG", "http://sn1.notify.live.net/throttledthirdparty/01.00/AQFgneByl872RbL3dzRlUoxQAgAAAAADAQAAAAQUZm52OkJCMjg1QTg1QkZDMkUxREQFBlVTU0MwMQ", MobileNotificationTypeEnum.Game);
                return Json(SiteCache.GetMobileNotifications(), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());

            }
            return Json(new MobileNotification() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public ActionResult DeleteMobileNotificationToken()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<NotificationMobileJson>(ref stream);

                    MobileNotificationFactory.DeleteMobileNotification(ob);
                    SiteCache.ClearMobileNotification();
                    return Json(new MobileNotification() { IsSuccessful = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());

            }
            return Json(new MobileNotification() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AccountSettings()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<UserMobile>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.MemberId);
                    if (ob.LoginId == mem.UserId)
                    {
                        AccountSettingsModel settings = new AccountSettingsModel();
                        if (mem.CurrentLeagueId != new Guid())
                        {
                            var league = MemberCache.GetLeagueOfMember(ob.MemberId);
                            if (league != null && league.Logo != null)
                                settings.LeagueLogo = league.Logo.ImageUrl;
                            settings.IsApartOfLeague = true;
                            settings.IsDuesManagementLockedDown = MemberCache.IsDuesManagementLockedDown(ob.MemberId);
                            settings.IsSubscriptionActive = MemberCache.CheckIsLeagueSubscriptionPaid(ob.MemberId);
                            settings.IsTreasurer = MemberCache.IsTreasurerOrBetterOfLeague(ob.MemberId);
                            settings.IsManagerOrBetter = MemberCache.IsManagerOrBetterOfLeague(ob.MemberId);
                            settings.IsEventsCoordinatorOrBetter = MemberCache.IsEventsCourdinatorOrBetterOfLeague(ob.MemberId);
                            settings.CalendarId = MemberCache.GetCalendarIdForMemberLeague(ob.MemberId);
                            settings.ShopEndUrl = MemberCache.GetStoreManagerKeysForUrlLeague(ob.MemberId);
                            settings.UnreadMessagesCount = MemberCache.GetUnreadMessagesCount(ob.MemberId);
                            settings.ChallengeCount = ManagementCache.GetBoutChallengesCount();
                            settings.OfficialsRequestCount = ManagementCache.GetOfficiatingRequestsCount();
                        }
                        settings.IsSuccessful = true;
                        return Json(settings, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());

            }
            return Json(new MobileNotification() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }

    }
}
