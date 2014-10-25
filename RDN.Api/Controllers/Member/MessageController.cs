using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Messages;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Controls.Message;
using RDN.Portable.Classes.Controls.Message.Enums;
using RDN.Portable.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.Api.Controllers.Member
{
    public class MessageController : Controller
    {

        public ActionResult DeleteChatMessage(string mid, string uid, string groupId)
        {
            try
            {
                var mem = MemberCache.GetMemberDisplay(new Guid(mid));
                if (new Guid(uid) == mem.UserId)
                {
                    if (MessagesCache.IsMemberOfGroup(Convert.ToInt64(groupId), mem.MemberId, HttpContext.Cache))
                    {
                        bool success = Messages.SetConversationAsDeleted(Convert.ToInt64(groupId), mem.MemberId);
                        return Json(new ConversationModel() { IsSuccessful = success }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new ConversationModel() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetConversationUpdates(string mid, string uid, string groupId, string lastMessageId)
        {
            try
            {
                var mem = MemberCache.GetMemberDisplay(new Guid(mid));
                if (new Guid(uid) == mem.UserId)
                {
                    if (MessagesCache.IsMemberOfGroup(Convert.ToInt64(groupId), mem.MemberId, HttpContext.Cache))
                    {
                        Messages.MarkGroupConversationAsRead(Convert.ToInt64(groupId), mem.MemberId);
                        var messages = Messages.GetMessageHistoryWithGroup(Convert.ToInt64(groupId), mem.MemberId, Convert.ToInt32(lastMessageId));
                        return Json(new ConversationModel() { IsSuccessful = true, Messages = messages }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType(), additionalInformation: groupId + ":" + lastMessageId);

            }
            return Json(new ConversationModel() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PostMessage()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<ConversationModel>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.MemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        if (MessagesCache.IsMemberOfGroup(ob.GroupMessageId, mem.MemberId, HttpContext.Cache))
                        {
                            Messages.AddNewMessageToGroup(ob.GroupMessageId, mem.MemberId, ob.Message);
                            ob.IsSuccessful = true;
                            return Json(ob, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new ConversationModel() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMessages(string mid, string uid, string t, string p, string c)
        {
            MessageModel mess = new MessageModel();

            try
            {
                var mem = MemberCache.GetMemberDisplay(new Guid(mid));
                if (new Guid(uid) == mem.UserId)
                {
                    mess = Messages.GetMessagesForOwner((GroupOwnerTypeEnum)Enum.Parse(typeof(GroupOwnerTypeEnum), t.ToLower()), mem.MemberId, Convert.ToInt32(p), Convert.ToInt32(c));
                    mess.IsSuccessful = true;
                    return Json(mess, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: "/type:" + t);
            }
            return Json(new MessageModel() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MessageView(string mid, string uid, string gid)
        {
            try
            {
                var mem = MemberCache.GetMemberDisplay(new Guid(mid));
                if (new Guid(uid) == mem.UserId)
                {

                    var league = MemberCache.GetLeagueOfMember(mem.MemberId);
                    //if (league != null)
                    //    SetCulture(league.CultureSelected);

                    if (MessagesCache.IsMemberOfGroup(Convert.ToInt64(gid), mem.MemberId, HttpContext.Cache))
                    {
                        Messages.MarkGroupConversationAsRead(Convert.ToInt64(gid), mem.MemberId);

                        var conversation = Messages.GetConversationFromGroup(Convert.ToInt64(gid), mem.MemberId);
                        conversation.IsSuccessful = true;
                        return Json(conversation, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new ConversationModel() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MessageNew(string mid, string uid, string type)
        {
            MessageModel model = new MessageModel();
            try
            {
                var mem = MemberCache.GetMemberDisplay(new Guid(mid));
                if (new Guid(uid) == mem.UserId)
                {
                    model.OwnerType = (GroupOwnerTypeEnum)Enum.Parse(typeof(GroupOwnerTypeEnum), type);
                    model.IdOfEntity = mem.MemberId;
                    model.SendEmailForMessage = true;
                    if (model.OwnerType == GroupOwnerTypeEnum.member)
                        model.Recipients = Messages.GetConnectedMembersOfMember(mem.MemberId);
                    else if (model.OwnerType == GroupOwnerTypeEnum.shop)
                    {
                        model.Recipients = Messages.GetConnectedShopRecipient(mem.MemberId);
                    }
                    else if (model.OwnerType == GroupOwnerTypeEnum.jobboard)
                    {
                        model.Recipients = new List<MemberDisplayBasic>();
                        model.Recipients.Add(mem);
                    }
                    else if (model.OwnerType == GroupOwnerTypeEnum.league)
                    {
                        model.Recipients = Messages.GetConnectedLeagueRecipient(mem.MemberId);
                    }
                    else if (model.OwnerType == GroupOwnerTypeEnum.paywall)
                    {
                        model.Recipients = Messages.GetConnectedShopRecipient(mem.MemberId);
                    }
                    else if (model.OwnerType == GroupOwnerTypeEnum.calevent)
                    {
                        model.Recipients = Messages.GetConnectedCalEventRecipient(mem.MemberId);
                    }
                    else
                        model.Recipients = Messages.GetConnectedMembersOfGroup(mem.MemberId);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TextMessageNew(string mid, string uid, string type)
        {
            MessageModel model = new MessageModel();
            try
            {
                var mem = MemberCache.GetMemberDisplay(new Guid(mid));
                if (new Guid(uid) == mem.UserId)
                {
                    model.OwnerType = (GroupOwnerTypeEnum)Enum.Parse(typeof(GroupOwnerTypeEnum), type);
                    model.IdOfEntity = mem.MemberId;
                    model.SendEmailForMessage = true;
                    model.IsCarrierVerified = mem.IsCarrierVerified;

                    if (model.OwnerType == GroupOwnerTypeEnum.member)
                        model.Recipients = Messages.GetConnectedMembersOfMember(mem.MemberId);
                    else
                        model.Recipients = Messages.GetConnectedMembersOfGroup(mem.MemberId);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateNewMessage()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<ConversationModel>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.MemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        Messages.CreateNewMessageForGroup(ob);
                        ob.IsSuccessful = true;
                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Json(new ConversationModel() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateNewTextMessage()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<ConversationModel>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.MemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        Messages.CreateNewTextMessageForGroup(ob);
                        ob.IsSuccessful = true;
                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new ConversationModel() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }

    }
}
