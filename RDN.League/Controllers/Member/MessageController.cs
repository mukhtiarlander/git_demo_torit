using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using RDN.Library.Classes.Messages;
using RDN.Library.Classes.Error;
using RDN.League.Models.Messages;
using RDN.Library.Classes.Messages.Classes;
using System.Collections.Specialized;
using RDN.League.Models.Utilities;
using RDN.Library.Classes.Account.Classes;
using RDN.League.Models.Enum;
using RDN.Library.Cache;
using RDN.League.Models.Filters;
using RDN.Library.Util;
using RDN.Library.Util.Enum;
using RDN.Library.Classes.League.Enums;
using RDN.Library.Classes.League.Classes;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Controls.Message.Enums;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class MessageController : BaseController
    {

        [Authorize]
        public ActionResult DeleteChatMessage(string groupId, string memId)
        {
            try
            {
                if (MessagesCache.IsMemberOfGroup(Convert.ToInt64(groupId), RDN.Library.Classes.Account.User.GetMemberId(), HttpContext.Cache))
                {
                    bool success = Messages.SetConversationAsDeleted(Convert.ToInt64(groupId), new Guid(memId));
                    return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = "false" }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult GetConversationUpdates(string groupId, string ownerUserId, string lastMessageId)
        {
            try
            {
                if (MessagesCache.IsMemberOfGroup(Convert.ToInt64(groupId), RDN.Library.Classes.Account.User.GetMemberId(), HttpContext.Cache))
                {
                    Messages.MarkGroupConversationAsRead(Convert.ToInt64(groupId), new Guid(ownerUserId));
                    var messages = Messages.GetMessageHistoryWithGroup(Convert.ToInt64(groupId), new Guid(ownerUserId), Convert.ToInt32(lastMessageId));
                    return Json(new { isSuccess = true, message = messages }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType(), additionalInformation: groupId + ":" + ownerUserId + ":" + ownerUserId + ":" + lastMessageId);

            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Message/
        [ValidateInput(false)]
        [HttpPost]
        [Authorize]
        public ActionResult PostMessage(string groupId, string ownerUserId, string mess)
        {
            try
            {
                if (MessagesCache.IsMemberOfGroup(Convert.ToInt64(groupId), RDN.Library.Classes.Account.User.GetMemberId(), HttpContext.Cache))
                {
                    Messages.AddNewMessageToGroup(Convert.ToInt64(groupId), new Guid(ownerUserId), mess);
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType(), additionalInformation: groupId + ":" + ownerUserId + ":" + mess);
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false, IsManager = false)]
        public ActionResult MessageHome(string type, string idOfGroup)
        {
            RDN.Portable.Classes.Controls.Message.MessageModel mess = new Portable.Classes.Controls.Message.MessageModel();

            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.ms.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Message Sent";
                    this.AddMessage(message);
                }
                else if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.mns.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Message Not Sent, An error occurred and is being looked into.";
                    this.AddMessage(message);
                }
                else if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.na.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "You do not have access to that page.";
                    this.AddMessage(message);
                }
                else if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.de.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "That message seems to been removed by the owner.";
                    this.AddMessage(message);
                }
                mess = Messages.GetMessagesForOwner((GroupOwnerTypeEnum)Enum.Parse(typeof(GroupOwnerTypeEnum), type.ToLower()), new Guid(idOfGroup), 0, 50);
                //MemberCache.ResetMessageCountCache(new Guid(idOfGroup));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: "ownerID:" + idOfGroup + "/type:" + type);
            }
            return View(mess);
        }
        [Authorize]
        public ActionResult MessageView(string groupId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                if (league != null)
                    SetCulture(league.CultureSelected);

                if (MessagesCache.IsMemberOfGroup(Convert.ToInt64(groupId), memId, HttpContext.Cache))
                {
                    Messages.MarkGroupConversationAsRead(Convert.ToInt64(groupId), memId);

                    var conversation = Messages.GetConversationFromGroup(Convert.ToInt64(groupId), memId);
                    if (conversation != null)
                        return View(conversation);
                    else
                        return Redirect("~/messages/" + GroupOwnerTypeEnum.member.ToString() + "/" + memId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.de.ToString());
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect("~/messages/" + GroupOwnerTypeEnum.member.ToString() + "/" + RDN.Library.Classes.Account.User.GetMemberId().ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.na.ToString());
        }

        [Authorize]
        public ActionResult MessageNew(string type, string id)
        {
            MessageModel model = new MessageModel();
            try
            {
                model.OwnerType = (GroupOwnerTypeEnum)Enum.Parse(typeof(GroupOwnerTypeEnum), type);
                model.OwnerId = new Guid(id);
                model.SendEmailForMessage = true;
                if (model.OwnerType == GroupOwnerTypeEnum.member)
                {
                    model.Recipients = Messages.GetConnectedMembersOfMember(new Guid(id));
                    model.Groups = MemberCache.GetGroupsApartOf(new Guid(id));
                }
                else if (model.OwnerType == GroupOwnerTypeEnum.shop)
                {
                    model.Recipients = Messages.GetConnectedShopRecipient(new Guid(id));
                    model.Title = "Shop Message: ";
                }
                else if (model.OwnerType == GroupOwnerTypeEnum.jobboard)
                {
                    var mem = MemberCache.GetMemberDisplay(model.OwnerId);
                    model.Recipients = new List<MemberDisplayBasic>();
                    model.Recipients.Add(mem);
                    model.Title = "Jobs: ";
                }
                else if (model.OwnerType == GroupOwnerTypeEnum.person)
                {
                    var mem = MemberCache.GetMemberDisplay(model.OwnerId);
                    model.Recipients = new List<MemberDisplayBasic>();
                    model.Recipients.Add(mem);

                }
                else if (model.OwnerType == GroupOwnerTypeEnum.officiating)
                {
                    var mem = MemberCache.GetMemberDisplay(model.OwnerId);
                    model.Recipients = new List<MemberDisplayBasic>();
                    model.Recipients.Add(mem);
                    model.Title = "Officiating: ";
                }
                else if (model.OwnerType == GroupOwnerTypeEnum.league)
                {
                    model.Recipients = Messages.GetConnectedLeagueRecipient(new Guid(id));
                    model.Title = "League Message: ";
                }
                else if (model.OwnerType == GroupOwnerTypeEnum.paywall)
                {
                    model.Recipients = Messages.GetConnectedShopRecipient(new Guid(id));
                    model.Title = "Paywall Message: ";
                }
                else if (model.OwnerType == GroupOwnerTypeEnum.calevent)
                {
                    model.Recipients = Messages.GetConnectedCalEventRecipient(new Guid(id));
                    model.Title = "Public Event: ";
                }
                else
                {
                    model.Recipients = Messages.GetConnectedMembersOfGroup(new Guid(id));
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(model);
        }
        [Authorize]
        public ActionResult TextMessageNew(string type, string id)
        {
            MessageModel model = new MessageModel();
            try
            {
                model.OwnerType = (GroupOwnerTypeEnum)Enum.Parse(typeof(GroupOwnerTypeEnum), type);
                model.OwnerId = new Guid(id);
                model.SendEmailForMessage = true;
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var mem = MemberCache.GetMemberDisplay(memId);
                model.IsCarrierVerified = mem.IsCarrierVerified;
                if (model.OwnerType == GroupOwnerTypeEnum.member)
                {
                    model.Recipients = Messages.GetConnectedMembersOfMember(new Guid(id));
                    model.Groups = MemberCache.GetGroupsApartOf(new Guid(id));
                }
                else
                    model.Recipients = Messages.GetConnectedMembersOfGroup(new Guid(id));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(model);
        }

        [Authorize]
        public JsonResult GetConnectedRecepients(string memberId, string type)
        {
            try
            {
                var OwnerType = (GroupOwnerTypeEnum)Enum.Parse(typeof(GroupOwnerTypeEnum), type);
                var recList = new List<KeyValuePair<Guid, string>>();
                if (OwnerType == GroupOwnerTypeEnum.shop)
                {
                    recList = Messages.GetConnectedShopRecipient(new Guid(memberId)).Select(x => new KeyValuePair<Guid, string>(x.MemberId, x.Name)).ToList();

                }
                else if (OwnerType == GroupOwnerTypeEnum.jobboard)
                {
                    var mem = MemberCache.GetMemberDisplay(new Guid(memberId));
                    recList.Add(new KeyValuePair<Guid, string>(mem.MemberId, mem.Name));
                }
                else if (OwnerType == GroupOwnerTypeEnum.person)
                {
                    var mem = MemberCache.GetMemberDisplay(new Guid(memberId));
                    recList.Add(new KeyValuePair<Guid, string>(mem.MemberId, mem.Name));
                }
                else if (OwnerType == GroupOwnerTypeEnum.officiating)
                {
                    var mem = MemberCache.GetMemberDisplay(new Guid(memberId));
                    recList.Add(new KeyValuePair<Guid, string>(mem.MemberId, mem.Name));

                }
                else if (OwnerType == GroupOwnerTypeEnum.paywall)
                {
                    recList = Messages.GetConnectedShopRecipient(new Guid(memberId)).Select(x => new KeyValuePair<Guid, string>(x.MemberId, x.Name)).ToList();

                }
                else if (OwnerType == GroupOwnerTypeEnum.calevent)
                {
                    recList = Messages.GetConnectedCalEventRecipient(new Guid(memberId)).Select(x => new KeyValuePair<Guid, string>(x.MemberId, x.Name)).ToList();
                }
                //var memId = new Guid(memberId);
                //var member = RDN.Library.Classes.Account.User.GetMemberWithMemberId(memId);
                var json = JsonConvert.SerializeObject(recList);
                return Json(new { success = true, Recipients = json }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);

        }
        [Authorize]
        public ActionResult CreateNewMessage(MessageModel model)
        {
            RDN.Portable.Classes.Controls.Message.ConversationModel mess = new RDN.Portable.Classes.Controls.Message.ConversationModel();
            mess.MemberId = RDN.Library.Classes.Account.User.GetMemberId();
            try
            {
                List<Guid> listOfGuids = new List<Guid>();
                List<long> listOfGroupIds = new List<long>();
                mess.FromId = model.OwnerId;
                mess.Message = model.MessageTextWriting;
                listOfGuids.Add(mess.MemberId);

                mess.Title = model.Title;
                mess.SendEmailForMessage = model.SendEmailForMessage;
                mess.OwnerType = model.OwnerType;

                if (!String.IsNullOrEmpty(model.ToMemberIds))
                {
                    foreach (string guid in model.ToMemberIds.Split(','))
                    {
                        long groupId = new long();
                        Guid memberId = new Guid();
                        if (Guid.TryParse(guid, out memberId))
                            listOfGuids.Add(memberId);
                        else if (Int64.TryParse(guid, out groupId))
                            listOfGroupIds.Add(groupId);
                    }
                }

                listOfGuids = listOfGuids.Distinct().ToList();
                foreach (var guid in listOfGuids)
                {
                    MemberDisplayMessage mem = new MemberDisplayMessage();
                    mem.MemberId = guid;
                    mess.Recipients.Add(mem);
                }
                listOfGroupIds = listOfGroupIds.Distinct().ToList();
                foreach (var guid in listOfGroupIds)
                {
                    mess.GroupIds.Add(guid);
                }

                Messages.CreateNewMessageForGroup(mess);
                return Redirect("~/messages/" + GroupOwnerTypeEnum.member.ToString() + "/" + mess.MemberId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.ms.ToString());
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect("~/messages/" + GroupOwnerTypeEnum.member.ToString() + "/" + mess.MemberId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.mns.ToString());
        }
        [HttpPost]
        [Authorize]
        public ActionResult CreateNewTextMessage(MessageModel model)
        {
            try
            {
                RDN.Portable.Classes.Controls.Message.ConversationModel mess = new Portable.Classes.Controls.Message.ConversationModel();
                List<Guid> listOfGuids = new List<Guid>();
                List<long> listOfGroupIds = new List<long>();
                mess.MemberId = model.OwnerId;
                mess.FromId = model.OwnerId;
                mess.Message = model.MessageTextWriting;

                listOfGuids.Add(RDN.Library.Classes.Account.User.GetMemberId());
                mess.OwnerType = model.OwnerType;

                if (!String.IsNullOrEmpty(model.ToMemberIds))
                {
                    foreach (string guid in model.ToMemberIds.Split(','))
                    {
                        long groupId = new long();
                        Guid memberId = new Guid();
                        if (Guid.TryParse(guid, out memberId))
                            listOfGuids.Add(memberId);
                        else if (Int64.TryParse(guid, out groupId))
                            listOfGroupIds.Add(groupId);
                    }
                }


                listOfGuids = listOfGuids.Distinct().ToList();
                foreach (var guid in listOfGuids)
                {
                    MemberDisplayMessage mem = new MemberDisplayMessage();
                    mem.MemberId = guid;
                    mess.Recipients.Add(mem);
                }
                listOfGroupIds = listOfGroupIds.Distinct().ToList();
                foreach (var guid in listOfGroupIds)
                {
                    mess.GroupIds.Add(guid);
                }

                Messages.CreateNewTextMessageForGroup(mess);
                return Redirect("~/messages/" + GroupOwnerTypeEnum.member.ToString() + "/" + model.OwnerId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.ms.ToString());
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect("~/messages/" + GroupOwnerTypeEnum.member.ToString() + "/" + model.OwnerId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.mns.ToString());
        }


        [Authorize]
        public ActionResult SaveMembersToMessage(string memberids, string groupid)
        {
            List<Guid> member_ids = new List<Guid>();
            if (!String.IsNullOrEmpty(memberids))
            {
                foreach (string guid in memberids.Split(','))
                {
                    Guid temp = new Guid();
                    bool didWork = Guid.TryParse(guid, out temp);
                    if (didWork)
                        member_ids.Add(temp);
                }
            }

            return Json(Messages.SaveMembersToMessage(member_ids, Convert.ToInt64(groupid)));


        }
    }
}
