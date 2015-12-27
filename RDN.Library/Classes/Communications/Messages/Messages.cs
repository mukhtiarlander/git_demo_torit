using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Library.Classes.Messages.Classes;
using RDN.Utilities.Config;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Cache;
using System.Web;
using RDN.Library.DataModels.League;
using RDN.Library.Classes.League.Enums;
using RDN.Utilities.Strings;
using RDN.Portable.Config;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Controls.Message;
using RDN.Portable.Classes.Controls.Message.Enums;
using RDN.Library.Classes.Mobile;
using RDN.Portable.Classes.League.Enums;
using RDN.Library.DataModels.Messages;
using RDN.Library.Classes.Config;
using RDN.Portable.Classes.Url;

namespace RDN.Library.Classes.Messages
{
    public class Messages
    {
        public Messages()
        {

        }

        public static int UpgradeMessages()
        {
            var dc = new ManagementContext();
            DateTime dt = DateTime.UtcNow.AddDays(-30);
            var m = dc.GroupMessages.Where(x => x.Created > dt);
            foreach (var mess in m)
            {
                mess.LastModified = DateTime.UtcNow;
            }
            int c = dc.SaveChanges();
            return c;
        }

        public static List<MemberDisplayBasic> GetRecipientsOfMessageGroup(long groupId)
        {
            List<MemberDisplayBasic> members = new List<MemberDisplayBasic>();
            var dc = new ManagementContext();

            var recipients = (from xx in dc.GroupMessages
                              where xx.GroupId == groupId
                              select xx.Recipients.Where(x => x.IsRemovedFromGroup == false)).FirstOrDefault();
            if (recipients != null)
            {
                foreach (var mem in recipients)
                {
                    MemberDisplayBasic user = new MemberDisplayBasic();
                    user.MemberId = mem.Recipient.MemberId;
                    user.DerbyName = mem.Recipient.DerbyName;
                    members.Add(user);
                }
            }
            return members;

        }
        /// <summary>
        /// gets the connected members of the group message
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static List<MemberDisplayBasic> GetConnectedMembersOfGroup(Guid groupId)
        {
            List<MemberDisplayBasic> members = new List<MemberDisplayBasic>();
            List<MemberDisplayBasic> memsNew = new List<MemberDisplayBasic>();
            List<Guid> leagues = new List<Guid>();
            try
            {
                leagues.Add(groupId);
                var dc = new ManagementContext();
                ///gets the federations owned and find the leagues within the federation, 
                ///then adds them to the list of leagues.
                var membersOfFederations = (from xx in dc.Federations
                                            where xx.FederationId == groupId
                                            select xx.Members).ToList();

                var membersOfLeagues = (from xx in dc.Federations
                                        where xx.FederationId == groupId
                                        select xx.Leagues.Select(x => x.League.LeagueId)).ToList();

                foreach (var list in membersOfFederations)
                {
                    foreach (var mem in list)
                    {
                        MemberDisplayBasic m = new MemberDisplayBasic();
                        m.MemberId = mem.Member.MemberId;
                        m.DerbyName = mem.Member.DerbyName.Replace("'", "").Replace('"', ' ');
                        m.Firstname = mem.Member.Firstname;
                        m.LastName = mem.Member.Lastname;
                        members.Add(m);
                    }
                }

                foreach (var list in membersOfLeagues)
                {
                    foreach (var mem in list)
                    {
                        leagues.Add(mem);
                    }
                }
                leagues = leagues.Distinct().ToList();

                if (leagues.Count > 0)
                {
                    var membersOfLeag = (from xx in dc.Leagues
                                         where leagues.Contains(xx.LeagueId)
                                         select xx.Members).ToList();

                    foreach (var list in membersOfLeag)
                    {
                        foreach (var mem in list.Where(x => x.HasLeftLeague == false))
                        {
                            MemberDisplayBasic m = new MemberDisplayBasic();
                            m.MemberId = mem.Member.MemberId;
                            m.DerbyName = mem.Member.DerbyName.Replace("'", "").Replace('"', ' ');
                            m.Firstname = mem.Member.Firstname;
                            m.LastName = mem.Member.Lastname;
                            members.Add(m);
                        }
                    }
                }

                foreach (var mem in members)
                {
                    if (memsNew.Where(x => x.MemberId == mem.MemberId).FirstOrDefault() == null)
                        memsNew.Add(mem);
                }
                memsNew = memsNew.OrderBy(x => x.DerbyName).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return memsNew;

        }
        public static List<MemberDisplayBasic> GetConnectedMembersOfMember(Guid memberId)
        {
            List<MemberDisplayBasic> members = new List<MemberDisplayBasic>();

            var dc = new ManagementContext();
            var leagues = MemberCache.GetAllOwnedLeagues(memberId).Select(x => x.LeagueId).ToList();
            var federations = MemberCache.GetAllOwnedFederations(memberId).Select(x => x.Federation.FederationId).ToList();
            var leagueId = MemberCache.GetLeagueIdOfMember(memberId);
            leagues.Add(leagueId);
            ///gets the federations owned and find the leagues within the federation, 
            ///then adds them to the list of leagues.
            if (federations.Count > 0)
            {
                var membersOfFederations = (from xx in dc.Federations
                                            where federations.Contains(xx.FederationId)
                                            select xx.Members).ToList();

                var membersOfLeagues = (from xx in dc.Federations
                                        where federations.Contains(xx.FederationId)
                                        select xx.Leagues.Select(x => x.League.LeagueId)).ToList();

                foreach (var list in membersOfFederations)
                {
                    foreach (var mem in list)
                    {
                        MemberDisplayBasic m = new MemberDisplayBasic();
                        m.MemberId = mem.Member.MemberId;
                        m.DerbyName = mem.Member.DerbyName.Replace("'", "").Replace('"', ' ');
                        m.Firstname = mem.Member.Firstname;
                        m.LastName = mem.Member.Lastname;
                        members.Add(m);
                    }
                }

                foreach (var list in membersOfLeagues)
                {
                    foreach (var mem in list)
                    {
                        leagues.Add(mem);
                    }
                }
            }
            leagues = leagues.Distinct().ToList();

            if (leagues.Count > 0)
            {
                var membersOfLeagues = (from xx in dc.Leagues
                                        where leagues.Contains(xx.LeagueId)
                                        select xx.Members).ToList();

                foreach (var list in membersOfLeagues)
                {
                    var mems = list.Where(x => x.HasLeftLeague == false);
                    foreach (var mem in mems)
                    {
                        MemberDisplayBasic m = new MemberDisplayBasic();
                        m.MemberId = mem.Member.MemberId;
                        if (!String.IsNullOrEmpty(mem.Member.DerbyName))
                            m.DerbyName = mem.Member.DerbyName.Replace("'", "").Replace('"', ' ');
                        else
                            m.DerbyName = "Not Entered";
                        m.Firstname = mem.Member.Firstname;
                        m.LastName = mem.Member.Lastname;
                        members.Add(m);
                    }
                }
            }
            List<MemberDisplayBasic> memsNew = new List<MemberDisplayBasic>();
            foreach (var mem in members)
            {
                if (memsNew.Where(x => x.MemberId == mem.MemberId).FirstOrDefault() == null)
                    memsNew.Add(mem);
            }
            memsNew = memsNew.OrderBy(x => x.DerbyName).ToList();

            return memsNew;

        }
        /// <summary>
        /// finds the shop owner so they can be sent a message.
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public static List<MemberDisplayBasic> GetConnectedShopRecipient(Guid shopId)
        {
            List<MemberDisplayBasic> members = new List<MemberDisplayBasic>();
            try
            {
                Store.StoreGateway sg = new Store.StoreGateway();
                var store = sg.GetStore(shopId);

                var league = League.LeagueFactory.GetLeague(store.InternalId);

                if (league != null)
                {
                    foreach (var owner in league.LeagueMembers.Where(x => x.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Shops)))
                    {
                        members.Add(owner);
                    }
                    if (members.Count == 0)
                    {
                        foreach (var owner in league.LeagueMembers.Where(x => x.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner) || x.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Shops) || x.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager)))
                        {
                            owner.DerbyName = owner.DerbyName.Replace("'", "").Replace('"', ' ');
                            members.Add(owner);
                        }
                    }
                }
                else
                {
                    var member = MemberCache.GetMemberDisplay(store.InternalId);
                    if (member != null)
                    {
                        member.DerbyName = member.DerbyName.Replace("'", "").Replace('"', ' ');
                        members.Add(member);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return members;

        }
        public static List<MemberDisplayBasic> GetConnectedLeagueRecipient(Guid leagueId)
        {
            List<MemberDisplayBasic> members = new List<MemberDisplayBasic>();
            try
            {

                var league = League.LeagueFactory.GetLeague(leagueId);

                if (league != null)
                {
                    foreach (var owner in league.LeagueMembers.Where(x => x.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Events_Coord)))
                    {
                        owner.DerbyName = owner.DerbyName.Replace("'", "").Replace('"', ' ');
                        members.Add(owner);
                    }
                    if (members.Count == 0)
                    {
                        foreach (var owner in league.LeagueMembers.Where(x => x.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner) || x.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager)))
                        {
                            owner.DerbyName = owner.DerbyName.Replace("'", "").Replace('"', ' ');
                            members.Add(owner);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return members;

        }


        /// <summary>
        /// gets the owners of a calendar event so someone can send them a messages.
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<MemberDisplayBasic> GetConnectedCalEventRecipient(Guid ownerId)
        {
            List<MemberDisplayBasic> members = new List<MemberDisplayBasic>();
            try
            {
                var league = League.LeagueFactory.GetLeague(ownerId);
                if (league != null)
                {
                    foreach (var owner in league.LeagueMembers.Where(x => x.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Events_Coord)))
                    {
                        owner.DerbyName = owner.DerbyName.Replace("'", "").Replace('"', ' ');
                        members.Add(owner);
                    }
                    if (members.Count == 0)
                    {
                        foreach (var owner in league.LeagueMembers.Where(x => x.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner) || x.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager)))
                        {
                            owner.DerbyName = owner.DerbyName.Replace("'", "").Replace('"', ' ');
                            members.Add(owner);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return members;
        }

        /// <summary>
        /// gets members that are defined as Owner or Moderator
        /// </summary>
        public static List<MemberDisplayBasic> GetConnectedGroupRecipients(Guid guid)
        {  
            List<MemberDisplayBasic> members = new List<MemberDisplayBasic>();
            try
            {
                var league = League.LeagueFactory.GetLeague(guid);
                if (league != null)
                {
                    foreach (var group in league.Groups)
                    {
                        var ownersModerators = group.GroupMembers.Where(mem => mem.MemberAccessLevelEnum == GroupMemberAccessLevelEnum.Owner || mem.MemberAccessLevelEnum == GroupMemberAccessLevelEnum.Moderator);
                        foreach (var member in ownersModerators)
                        {
                            members.Add(member);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return members;
        }

        public static void AddNewMessageToGroup(long groupId, Guid ownerMemberId, string message)
        {
            try
            {
                var dc = new ManagementContext();
                DataModels.Messages.Message m = new DataModels.Messages.Message();
                m.FromUser = dc.Members.Where(x => x.MemberId == ownerMemberId).FirstOrDefault();
                m.MessageText = message;
                dc.Message.Add(m);

                var grp = dc.GroupMessages.Include("Recipients").Where(x => x.GroupId == groupId).FirstOrDefault();
                grp.LastModified = DateTime.UtcNow;
                grp.Messages.Add(m);
                dc.SaveChanges();

                var recips = grp.Recipients.Where(x => x.IsRemovedFromGroup == false);
                foreach (var mem in recips)
                {
                    try
                    {
                        DataModels.Messages.MessageInbox inbox = new DataModels.Messages.MessageInbox();
                        inbox.Message = m;
                        inbox.ToUser = dc.Members.Where(x => x.MemberId == mem.Recipient.MemberId).FirstOrDefault();
                        if (mem.Recipient.MemberId == ownerMemberId)
                            inbox.IsRead = true;
                        else
                            MemberCache.AddMessageCountToCache(+1, mem.Recipient.MemberId);
                        inbox.MessageReadDateTime = DateTime.UtcNow;
                        inbox.NotifiedEmailDateTime = DateTime.UtcNow;
                        dc.MessageInbox.Add(inbox);
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }
                dc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        public static bool CreateNewTextMessageForGroup(ConversationModel con)
        {
            try
            {
                string conversationLink = LibraryConfig.InternalSite + UrlManager.VIEW_MESSAGE_CONVERSATION;
                var member = MemberCache.GetMemberDisplay(con.MemberId);
                string ownerName = String.Empty;
                ownerName = member.DerbyName;
                var leagueMembers = MemberCache.GetCurrentLeagueMembers(con.MemberId);
                var tempGroups = MemberCache.GetLeagueGroupsOfMember();
                foreach (var g in con.GroupIds)
                {
                    var tempG = tempGroups.Where(x => x.Id == g).FirstOrDefault();
                    if (tempG != null)
                    {
                        foreach (var mTemp in tempG.GroupMembers)
                        {
                            if (con.Recipients.Where(x => x.MemberId == mTemp.MemberId).FirstOrDefault() == null)
                                con.Recipients.Add(new MemberDisplayMessage() { MemberId = mTemp.MemberId });
                        }
                    }
                }


                foreach (var mem in con.Recipients)
                {
                    try
                    {
                        var tempMem = leagueMembers.Where(x => x.MemberId == mem.MemberId).FirstOrDefault();
                        if (tempMem != null)
                        {
                            var emailData = new Dictionary<string, string>
                                        {
                                                                           { "name",tempMem.DerbyName},
                                                                           { "fromName",ownerName},
                                            { "body",ownerName+":"+ con.Message},
                                            { "setupLink",LibraryConfig.InternalSite + UrlManager .MEMBER_SETTINGS_URL},

                                        };
                            if (tempMem.IsCarrierVerified)
                            {
                                if (!String.IsNullOrEmpty(tempMem.PhoneNumber))
                                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, LibraryConfig.TextMessageEmail, tempMem.PhoneNumber, emailData, EmailServer.EmailServerLayoutsEnum.TextMessage);
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(tempMem.UserName))
                                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, tempMem.UserName, "Text Message From " + ownerName, emailData, EmailServer.EmailServerLayoutsEnum.TextMessageNotVerified);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool CreateNewMessageForGroup(ConversationModel con)
        {
            try
            {
                string conversationLink = LibraryConfig.InternalSite + UrlManager.VIEW_MESSAGE_CONVERSATION;
                var dc = new ManagementContext();
                DataModels.Messages.Message m = new DataModels.Messages.Message();
                m.FromUser = dc.Members.Where(x => x.MemberId == con.MemberId).FirstOrDefault();
                m.MessageText = con.Message;
                dc.Message.Add(m);
                dc.SaveChanges();

                string ownerName = String.Empty;
                string messageId = String.Empty;

                if (m.FromUser != null)
                    ownerName = m.FromUser.DerbyName;

                DataModels.Messages.GroupMessage group = new DataModels.Messages.GroupMessage();
                group.Messages.Add(m);
                group.TitleOfMessage = con.Title;
                group.GroupOwnerId = con.FromId;
                group.LastModified = DateTime.UtcNow;
                var fed = dc.Federations.Where(x => x.FederationId == con.FromId).FirstOrDefault();
                if (fed != null)
                    ownerName = fed.Name;
                else
                {
                    var leag = dc.Leagues.Where(x => x.LeagueId == con.FromId).FirstOrDefault();
                    if (leag != null)
                        ownerName = leag.Name;
                }
                dc.GroupMessages.Add(group);
                dc.SaveChanges();
                messageId = group.GroupId.ToString();
                var tempGroups = MemberCache.GetLeagueGroupsOfMember(con.FromId);
                //add all members of the groups
                foreach (var g in con.GroupIds)
                {
                    if (tempGroups != null && tempGroups.Count > 0)
                    {
                        var tempG = tempGroups.Where(x => x.Id == g).FirstOrDefault();
                        if (tempG != null)
                        {
                            foreach (var mTemp in tempG.GroupMembers)
                            {
                                if (con.Recipients.Where(x => x.MemberId == mTemp.MemberId).FirstOrDefault() == null)
                                    con.Recipients.Add(new MemberDisplayMessage() { MemberId = mTemp.MemberId });
                            }
                        }
                    }
                }
                foreach (var mem in con.Recipients)
                {
                    try
                    {
                        DataModels.Messages.MessageInbox inbox = new DataModels.Messages.MessageInbox();
                        inbox.Message = m;
                        inbox.ToUser = dc.Members.Where(x => x.MemberId == mem.MemberId).FirstOrDefault();
                        inbox.MessageReadDateTime = DateTime.UtcNow;
                        inbox.NotifiedEmailDateTime = DateTime.UtcNow;
                        if (inbox.ToUser.MemberId == m.FromUser.MemberId)
                            inbox.IsRead = true;
                        else
                            MemberCache.AddMessageCountToCache(+1, mem.MemberId);

                        DataModels.Messages.MessageRecipient recipient = new DataModels.Messages.MessageRecipient();
                        recipient.Group = group;
                        recipient.Recipient = inbox.ToUser;
                        group.Recipients.Add(recipient);

                        if (con.SendEmailForMessage && inbox.ToUser.AspNetUserId != new Guid())
                        {

                            var emailData = new Dictionary<string, string>
                                        {
                                            { "derbyname", inbox.ToUser.DerbyName }, 
                                            { "FromUserName", ownerName }, 
                                            { "messageBody", HtmlSanitize.ReplaceCarriageReturnsWithBreaks( m.MessageText )},
                                            { "viewConversationLink", conversationLink +  messageId}
                                        };

                            var user = System.Web.Security.Membership.GetUser((object)inbox.ToUser.AspNetUserId);
                            if (user != null)
                            {
                                inbox.UserNotifiedViaEmail = true;
                                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, user.UserName, LibraryConfig.DefaultEmailSubject + " New Message From " + ownerName, emailData, EmailServer.EmailServerLayoutsEnum.SendMessageToUserFromOtherUser);
                            }

                        }
                        dc.MessageInbox.Add(inbox);
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }

                int c = dc.SaveChanges();

                MobileNotificationFactory fact = new MobileNotificationFactory();
                fact.Initialize("New Message", m.MessageText, Mobile.Enums.NotificationTypeEnum.Message)
                    .AddId(group.GroupId)
                    .AddMembers(con.Recipients.Select(x => x.MemberId).ToList())
                    .SendNotifications();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static int GetUnreadMessagesCount(Guid ownerId)
        {
            try
            {
                var dc = new ManagementContext();
                return dc.MessageInbox.Where(x => x.ToUser.MemberId == ownerId).Select(x => x.Message.GroupBelongsTo.GroupId).Distinct().Count();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }
        public static MessageModel GetMessagesForOwner(GroupOwnerTypeEnum ownerType, Guid memId, int page, int count)
        {
            MessageModel mess = new MessageModel();
            try
            {
                mess.IdOfEntity = memId;
                mess.OwnerType = ownerType;

                var dc = new ManagementContext();
                GetGroupMessages(memId, mess, dc, page, count);
                //this is the conversation with my self.  I don't know ye why this shows up talking to one self.
                var personalConvo = mess.Conversations.Where(x => x.ConversationWithUser == memId).FirstOrDefault();
                if (personalConvo != null)
                    mess.Conversations.Remove(personalConvo);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return mess;
        }
        private static void GetGroupMessages(Guid ownerId, MessageModel mess, ManagementContext dc, int page, int take)
        {

            //TODO: add Take(50) a few days from now.
            var to = (from xx in dc.GroupMessages
                      where xx.IsDeleted == false
                      where xx.Recipients.Where(z => z.IsRemovedFromGroup == false).Select(x => x.Recipient.MemberId).Contains(ownerId)
                      select xx).AsParallel().OrderByDescending(x => x.LastModified).Skip(page * take).Take(take).ToList();
            List<RDN.Library.DataModels.Messages.GroupMessage> groups = new List<DataModels.Messages.GroupMessage>();



            //foreach (var recipient in to)
            foreach (var group in to)
            {
                try
                {
                    //var group = recipient.Group;
                    //var messages = recipient.Group.Messages;
                    foreach (var message in group.Messages)
                    {
                        MessageSingleModel ms = new MessageSingleModel();
                        if (message.FromUser == null)
                        {
                            ms.FromId = new Guid();
                        }
                        else
                        {
                            ms.FromId = message.FromUser.MemberId;
                            ms.FromName = message.FromUser.DerbyName;
                        }

                        ms.MessageCreated = message.Created;
                        ms.MessageId = message.MessageId;
                        if (!String.IsNullOrEmpty(message.MessageText))
                        {
                            ms.MessageText = RDN.Portable.Util.Strings.StringExt.HtmlDecode(message.MessageText);
                            if (ms.MessageText.Length > 20)
                                ms.MessageText = ms.MessageText.Remove(20);
                        }

                        var convo = mess.Conversations.Where(x => x.GroupMessageId == group.GroupId).FirstOrDefault();
                        if (convo == null)
                        {

                            ConversationModel con = new ConversationModel();
                            con.CanDelete = true;
                            if (message.FromUser != null)
                            {
                                con.FromName = message.FromUser.DerbyName;
                                con.FromId = message.FromUser.MemberId;
                            }
                            con.LastPostDate = ms.MessageCreated;
                            if (message.MessagesInbox.Where(x => x.ToUser.MemberId == ownerId).FirstOrDefault() == null)
                                con.IsConversationRead = true;

                            con.GroupMessageId = group.GroupId;

                            if (!String.IsNullOrEmpty(group.TitleOfMessage))
                                con.Title = group.TitleOfMessage;
                            else
                                con.Title = "Message";
                            if (message.FromUser != null)
                            {
                                con.LastPostBy = message.FromUser.DerbyName;
                            }
                            var recips = group.Recipients.Where(x => x.IsRemovedFromGroup == false);
                            foreach (var rec in recips)
                            {
                                MemberDisplayMessage mem = new MemberDisplayMessage();
                                mem.DerbyName = rec.Recipient.DerbyName;
                                mem.MemberId = rec.Recipient.MemberId;
                                mem.UserId = rec.Recipient.AspNetUserId;
                                var photo = rec.Recipient.Photos.FirstOrDefault();
                                if (photo != null)
                                    mem.ThumbUrl = photo.ImageUrlThumb;

                                con.Recipients.Add(mem);
                            }
                            con.Messages.Add(ms);
                            mess.Conversations.Add(con);
                        }
                        else
                        {
                            if (ms.MessageCreated > convo.LastPostDate)
                            {
                                convo.LastPostBy = ms.FromName;
                                convo.LastPostDate = ms.MessageCreated;
                            }
                            else
                            {
                                convo.FromId = ms.FromId;
                                convo.FromName = ms.FromName;
                            }

                            if (message.MessagesInbox.Where(x => x.ToUser.MemberId == ownerId).FirstOrDefault() != null)
                                convo.IsConversationRead = false;
                            else
                                convo.IsConversationRead = true;
                            convo.Messages.Add(ms);

                        }

                        groups.Add(group);
                    }
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
            }
            mess.Conversations = mess.Conversations.OrderByDescending(x => x.LastPostDate).ToList();
        }
        public static List<MessageSingleModel> GetMessageHistoryWithGroup(long groupId, Guid ownerId, int lastMessageId)
        {
            List<MessageSingleModel> messHistory = new List<MessageSingleModel>();
            try
            {
                var dc = new ManagementContext();
                var gro = (from xx in dc.GroupMessages.Include("Messages").Include("Message.FromUser")
                           where xx.GroupId == groupId
                           where xx.IsDeleted == false
                           select new
                           {
                               Messages = (from yy in xx.Messages
                                           where yy.MessageId > lastMessageId
                                           select new
                                           {
                                               yy.FromUser,
                                               yy.Created,
                                               yy.MessageId,
                                               yy.MessageText
                                           })
                           }).AsParallel().FirstOrDefault();
                if (gro == null)
                    return null;

                foreach (var message in gro.Messages)
                {
                    try
                    {
                        MessageSingleModel ms = new MessageSingleModel();
                        ms.FromId = message.FromUser.MemberId;
                        ms.FromName = message.FromUser.DerbyName;
                        ms.MessageCreated = message.Created;
                        ms.MessageId = message.MessageId;
                        ms.MessageText = RDN.Portable.Util.Strings.StringExt.HtmlDecode(message.MessageText);
                        if (messHistory.Where(x => x.MessageId == ms.MessageId).FirstOrDefault() == null)
                            messHistory.Add(ms);
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return messHistory;
        }


        public static ConversationModel GetConversationFromGroup(long groupId, Guid memId)
        {
            ConversationModel con = new ConversationModel();
            try
            {
                con.GroupMessageId = groupId;
                con.OwnerUserId = memId;
                var toMem = MemberCache.GetMemberDisplay(memId);
                con.FromName = toMem.DerbyName;

                var dc = new ManagementContext();
                var gro = (from xx in dc.GroupMessages.Include("Messages")
                           where xx.GroupId == groupId
                           where xx.IsDeleted == false
                           select new
                           {
                               xx.TitleOfMessage,
                               Messages = (from yy in xx.Messages
                                           select new
                                           {
                                               yy.FromUser,
                                               yy.Created,
                                               yy.MessageId,
                                               yy.MessageText,
                                               yy.MessagesInbox

                                           }).OrderBy(z => z.MessageId),
                               Recipients = xx.Recipients.Where(x => x.IsRemovedFromGroup == false),

                           }).AsParallel().FirstOrDefault();

                if (gro == null)
                    return null;

                foreach (var user in gro.Recipients)
                {
                    MemberDisplayMessage mem = new MemberDisplayMessage();
                    mem.DerbyName = user.Recipient.DerbyName;
                    mem.Firstname = user.Recipient.Firstname;
                    mem.LastName = user.Recipient.Lastname;
                    if (user.Recipient.Photos.OrderByDescending(x => x.Created).FirstOrDefault() != null)
                        mem.ThumbUrl = user.Recipient.Photos.OrderByDescending(x => x.Created).FirstOrDefault().ImageUrlThumb;
                    mem.MemberId = user.Recipient.MemberId;
                    mem.UserId = user.Recipient.AspNetUserId;
                    con.Recipients.Add(mem);
                }

                con.Title = gro.TitleOfMessage;
                foreach (var message in gro.Messages)
                {
                    try
                    {
                        MessageSingleModel ms = new MessageSingleModel();
                        ms.FromId = message.FromUser.MemberId;
                        ms.FromName = message.FromUser.DerbyName;
                        if (message.FromUser.Photos.OrderByDescending(x => x.Created).FirstOrDefault() != null)
                            ms.ThumbUrl = message.FromUser.Photos.OrderByDescending(x => x.Created).FirstOrDefault().ImageUrlThumb;
                        ms.MessageCreated = message.Created;
                        ms.MessageId = message.MessageId;
                        foreach (var inbox in message.MessagesInbox)
                        {
                            var user = con.Recipients.Where(x => x.MemberId == inbox.ToUser.MemberId).FirstOrDefault();
                            user.HasNotReadConversation = true;
                        }
                        if (!String.IsNullOrEmpty(message.MessageText))
                        {
                            ms.MessageText = message.MessageText;
                            ms.MessageTextHtml = message.MessageText;

                        }
                        if (ms.MessageCreated > con.LastPostDate)
                        {
                            con.LastPostDate = ms.MessageCreated;
                            //con.IsConversationRead = ms.IsMessageRead;
                            con.LastPostBy = ms.FromName;
                        }
                        if (con.Messages.Where(x => x.MessageId == ms.MessageId).FirstOrDefault() == null)
                            con.Messages.Add(ms);
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return con;
        }

        public static List<Guid> GetConversationMembers(long groupId)
        {
            List<Guid> memberids = new List<Guid>();
            try
            {
                var dc = new ManagementContext();
                var gro = (from xx in dc.GroupMessages.Include("Messages")
                           where xx.GroupId == groupId
                           where xx.IsDeleted == false
                           select new
                           {
                               Recipients = xx.Recipients.Where(x => x.IsRemovedFromGroup == false),
                           }).AsParallel().FirstOrDefault();

                if (gro == null)
                    return null;

                foreach (var user in gro.Recipients)
                {
                    memberids.Add(user.Recipient.MemberId);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return memberids;
        }

        public static bool MarkGroupConversationAsRead(long groupId, Guid ownerId)
        {

            try
            {
                var dc = new ManagementContext();

                var to = (from xx in dc.MessageInbox.Include("Message").Include("Message.GroupBelongsTo").Include("ToUser")
                          where xx.ToUser.MemberId == ownerId
                          where xx.Message.GroupBelongsTo.GroupId == groupId
                          select xx).ToList();
                foreach (var message in to)
                {
                    dc.MessageInbox.Remove(message);
                    MemberCache.AddMessageCountToCache(-1, ownerId);
                }

                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool SetConversationAsDeleted(long groupId, Guid ownerId)
        {
            try
            {
                var dc = new ManagementContext();
                var rec = (from xx in dc.MessagesRecipients
                           where xx.Recipient.MemberId == ownerId
                           where xx.Group.GroupId == groupId
                           select xx).FirstOrDefault();
                rec.IsRemovedFromGroup = true;

                var message = (from xx in dc.MessageInbox
                               where xx.ToUser.MemberId == ownerId
                               where xx.Message.GroupBelongsTo.GroupId == groupId
                               select xx);
                foreach (var mess in message)
                {
                    dc.MessageInbox.Remove(mess);
                }

                var group = (from xx in dc.GroupMessages
                             where xx.GroupId == groupId
                             select xx).FirstOrDefault();

                if (group.Recipients.Where(x => x.IsRemovedFromGroup == false).Count() == 0)
                    group.IsDeleted = true;
                int c = dc.SaveChanges();
                MemberCache.AddMessageCountToCache(-1, rec.Recipient.MemberId);
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool SaveMembersToMessage(List<Guid> memberids, long groupid)
        {
            try
            {
                var dc = new ManagementContext();
                var gro = (from xx in dc.GroupMessages.Include("Messages")
                           where xx.GroupId == groupid
                           where xx.IsDeleted == false
                           select xx).AsParallel().FirstOrDefault();

                foreach (Guid memberid in memberids)
                {
                    DataModels.Messages.MessageRecipient recipient = new DataModels.Messages.MessageRecipient();
                    recipient.Group = gro;

                    recipient.Recipient = dc.Members.Where(x => x.MemberId == memberid).FirstOrDefault();
                    gro.Recipients.Add(recipient);
                }
                int c = dc.SaveChanges();

                // Send Email
                foreach (Guid memberid in memberids)
                {
                    SendEmailAboutMemberAdded(memberid, groupid);
                }
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static void SendEmailAboutMemberAdded(Guid memberid, long groupid)
        {
            var member = MemberCache.GetMemberDisplay(memberid);
            var emailData = new Dictionary<string, string>
            {
                { "derbyname",member.DerbyName}, 
                { "messagelink", LibraryConfig.InternalSite+"/messages/view/"+groupid}
            };
            var user = System.Web.Security.Membership.GetUser((object)member.UserId);
            if (user != null)
            {
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, user.UserName, LibraryConfig.DefaultEmailSubject + " You are added in a message", emailData, EmailServer.EmailServerLayoutsEnum.AddedToChatMessage);
            }
        }
    }
}
