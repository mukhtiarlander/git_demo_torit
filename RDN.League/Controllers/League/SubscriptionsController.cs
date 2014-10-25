using RDN.League.Models.Filters;
using RDN.Library.Cache;
using RDN.Library.Classes.EmailServer;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.EmailServer.Enums;
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
    public class SubscriptionsController : BaseController
    {
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult AddNewList()
        {
            IEnumerable<SubscriberTypeEnum> Sets = Enum.GetValues(typeof(SubscriberTypeEnum))
                                                     .Cast<SubscriberTypeEnum>();
            var list = (from a in Sets
                        select new SelectListItem
                        {
                            Text = RDN.Portable.Util.Enums.EnumExt.ToFreindlyName(a),
                            Value = ((int)a).ToString()
                        });

            ViewBag.Sets = new SelectList(list, "Value", "Text");

            return View();

        }
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult AddNewList(SubscriptionFactory subscriptionList)
        {

            try
            {
                //var memId = RDN.Library.Classes.Account.User.GetMemberId();
                //var league = MemberCache.GetLeagueOfMember(memId);
                //sponsor.SponsorForLeague = league.LeagueId;
                //sponsor.SponsorAddByMember = memId;
            
                bool execute = RDN.Library.Classes.EmailServer.SubscriptionFactory.Add_New_SubscriptionList(subscriptionList);
                return Redirect(Url.Content("~/subscriptions/Lists?u=" + SiteMessagesEnum.sac));
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

        [Authorize]
        [ValidateInput(false)]
        public ActionResult SubscriptionEdit(long listId)
        {
            try
            { 
                var Data = RDN.Library.Classes.EmailServer.SubscriptionFactory.GetData(listId);
 
                return View(Data);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Redirect(Url.Content("?u=" + SiteMessagesEnum.sac));
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult SubscriptionEdit(SubscriptionFactory subscription)
        {
            try
            {
                bool execute = RDN.Library.Classes.EmailServer.SubscriptionFactory.UpdateListInfo(subscription);
                return Redirect(Url.Content("~/subscriptions/Lists?u=" + SiteMessagesEnum.sac));

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Redirect(Url.Content("?u=" + SiteMessagesEnum.su));
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [Authorize]
        public ActionResult SubscriptionRemove(long listId)
        {
            try
            {
                //var memId = RDN.Library.Classes.Account.User.GetMemberId();
                //if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                //{
                //    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                //}

                var Data = RDN.Library.Classes.EmailServer.SubscriptionFactory.RemoveList(listId);
                return Redirect(Url.Content("~/subscriptions/Lists?u=" + SiteMessagesEnum.de));
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
        [Authorize]

        public ActionResult ViewAllSubsLists()
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "information updated.";
                    this.AddMessage(message);

                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sac.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully Added.";
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


                var itemLists = RDN.Library.Classes.EmailServer.SubscriptionFactory.GetSubscriberList();
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
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult SubscriptionListDetails(long listId,string listName)
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "information updated.";
                    this.AddMessage(message);

                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sac.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully Added.";
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

                var subscriptionData = RDN.Library.Classes.EmailServer.SubscriptionFactory.GetData(listId);
                ViewBag.SubscriptionType = subscriptionData.SubscriberTypeEnum;
                ViewBag.listId = listId;
                ViewBag.Name = listName;
                var itemLists = RDN.Library.Classes.EmailServer.SubscriptionFactory.GetSubscriberDetails(listId);
               
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
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult SubscriberAdd(long listId,string listName)
        {
            var subscriptionData = RDN.Library.Classes.EmailServer.SubscriptionFactory.GetData(listId);
            
            SubscriptionFactory initialData = new SubscriptionFactory();
            initialData.listId = listId;
            initialData.ListName = listName;
            initialData.SubscriberTypeEnum = subscriptionData.SubscriberTypeEnum;
            return View(initialData);

        }
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult SubscriberAdd(SubscriptionFactory subscriber)
        {

            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                subscriber.LeagueOwner = league.LeagueId;
                
                bool execute = RDN.Library.Classes.EmailServer.SubscriptionFactory.Add_New_Subscriber(subscriber);

                return Redirect(Url.Content("~/subscriptions/View/"+ subscriber.listId +"/"+ subscriber.ListName +"?u=" + SiteMessagesEnum.sac));

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Redirect(Url.Content("?u=" + SiteMessagesEnum.sac));
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        
        [Authorize]
        [ValidateInput(false)]
        public ActionResult SubscriberEdit(long listId, string listName, long subscriberId)
        {
              try
            {
                var subscriptionData = RDN.Library.Classes.EmailServer.SubscriptionFactory.GetData(listId);
                               
                //SubscriptionFactory initialData = new SubscriptionFactory();
                var initialData = RDN.Library.Classes.EmailServer.SubscriptionFactory.GetSubscriberData(listId, subscriberId);
                initialData.SubscriberTypeEnum = subscriptionData.SubscriberTypeEnum;
                initialData.listId = listId;
                initialData.ListName = listName;
                return View(initialData);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Redirect(Url.Content("?u=" + SiteMessagesEnum.sac));
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult SubscriberEdit(SubscriptionFactory subscriber)
        {
            try
            {
                bool execute = RDN.Library.Classes.EmailServer.SubscriptionFactory.UpdateSubscriberInfo(subscriber);
                return Redirect(Url.Content("~/subscriptions/View/" + subscriber.listId + "/" + subscriber.ListName + "?u=" + SiteMessagesEnum.su));

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Redirect(Url.Content("?u=" + SiteMessagesEnum.su));
        }
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif

        [Authorize]
        [ValidateInput(false)]
        public ActionResult SubscriberView(long listId, string listName, long subscriberId)
        {
            try
            {
                //SubscriptionFactory initialData = new SubscriptionFactory();
                var initialData = RDN.Library.Classes.EmailServer.SubscriptionFactory.GetSubscriberData(listId, subscriberId);

                initialData.listId = listId;
                initialData.ListName = listName;
                return View(initialData);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Redirect(Url.Content("?u=" + SiteMessagesEnum.sac));
        }

        [Authorize]
        public ActionResult SubscriberRemove(long listId, string listName, long subscriberId)
        {
            try
            {
                //var memId = RDN.Library.Classes.Account.User.GetMemberId();
                //if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                //{
                //    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                //}

                var Data = RDN.Library.Classes.EmailServer.SubscriptionFactory.SubscriberRemove(subscriberId);
                
                return Redirect(Url.Content("~/subscriptions/View/" + listId + "/" + listName + "?u=" + SiteMessagesEnum.de));


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

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult EmailBlast(long listId, string listName)
        {
            var subscriptions = RDN.Library.Classes.EmailServer.SubscriptionFactory.GetSubscription();
            ViewBag.subscription = new SelectList(subscriptions, "SubscriptionId", "SubscriptionName");

            return View();

        }

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult EmailBlast(SubscriptionFactory subscriber)
        {
            
            return View();

        }
    }
}
