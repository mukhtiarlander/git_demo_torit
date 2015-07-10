using RDN.Library.Cache;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Config;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.League.Classes;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Forum;
using RDN.Portable.Classes.Url;
using RDN.Portable.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Controls.Forum
{
    public class ForumNotificationFactory
    {
        public List<Guid> membersAlreadyEmailed;
        Guid ForumId;
        Guid LeagueId;
        bool IsNewPost;
        bool IsBroadcasted;
        long GroupId;
        long TopicId;
        string TopicName;
        string GroupName;
        Guid MemberIdSendingPost;
        string DerbyNameSendingPost;
        string PlainMessage;
        /// <summary>
        /// used to export all the members we send messages to.
        /// </summary>
        //public List<MemberDisplayBasic> MembersSent { get; set; }

        public ForumNotificationFactory(Guid forumId, Guid leagueId, bool isNewPost, bool isBroadcasted, long groupId, long topicId, string groupName, string topicName, string plainMessage, Guid memberIdSending, string memberDerbyName)
        {
            membersAlreadyEmailed = new List<Guid>();
            ForumId = forumId;
            LeagueId = leagueId;
            IsNewPost = isNewPost;
            IsBroadcasted = isBroadcasted;
            GroupId = groupId;
            TopicId = topicId;
            PlainMessage = plainMessage;
            MemberIdSendingPost = memberIdSending;
            DerbyNameSendingPost = memberDerbyName;
            TopicName = topicName;
            GroupName = groupName;
            //MembersSent = new List<MemberDisplayBasic>();
        }

        /// <summary>
        /// sends an email out to members apart of the forum.
        /// </summary>
        /// <param name="forum"></param>
        /// <param name="mess"></param>
        /// <param name="topic"></param>
        /// <param name="mem"></param>
        private void SendEmailAboutNewForumPost(Guid userId, string derbyName)
        {
            try
            {
                if (userId != new Guid())
                {
                    string fullMessage = "<b>" + GroupName + "</b> - " + TopicName + "<br/><br/>";
                    var emailData = new Dictionary<string, string>
                                        {
                                            { "derbyname",derbyName}, 
                                            { "FromUserName", DerbyNameSendingPost}, 
                                            { "messageBody",fullMessage + PlainMessage},
                                            { "viewConversationLink",                                               LibraryConfig.InternalSite +"/forum/post/view/" + ForumId.ToString().Replace("-","") +"/"+ TopicId},
                                            { "notificationSettings",                                               LibraryConfig.InternalSite + UrlManager.WEBSITE_MEMBER_SETTINGS}
                                        };
                    var user = System.Web.Security.Membership.GetUser((object)userId);
                    if (user != null)
                    {
                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, user.UserName, LibraryConfig.DefaultEmailSubject + " " + TopicName, emailData, EmailServer.EmailServerLayoutsEnum.SendForumBroadcastMessageToUserGroup);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        public ForumNotificationFactory LeagueEmailAboutForumPost()
        {
            try
            {
                if (GroupId > 0)
                {
                    var group = LeagueGroupFactory.GetGroup(GroupId, LeagueId);
                    var mems = group.GroupMembers;

                    foreach (var mem in mems)
                    {
                        if (MemberIdSendingPost != mem.MemberId)
                        {
                            if (!membersAlreadyEmailed.Contains(mem.MemberId))
                            {
                                //emails if its a new post, not broadcasted
                                if (IsNewPost && !IsBroadcasted && mem.DoesReceiveNewPostGroupNotifications)
                                {
                                    SendEmailAboutNewForumPost(mem.UserId, mem.DerbyName);
                                    membersAlreadyEmailed.Add(mem.MemberId);
                                }
                                else if (IsBroadcasted && mem.DoesReceiveGroupBroadcastNotifications)
                                {
                                    SendEmailAboutNewForumPost(mem.UserId, mem.DerbyName);
                                    membersAlreadyEmailed.Add(mem.MemberId);
                                }
                                //MembersSent.Add(new MemberDisplayBasic() { UserId = mem.UserId, MemberId = mem.MemberId });
                            }
                        }
                    }
                }
                else
                {
                    var members = League.LeagueFactory.GetLeagueMembersNotificationSettings(LeagueId);
                    foreach (var mem in members)
                    {
                        if (MemberIdSendingPost != mem.MemberId)
                        {
                            if (!membersAlreadyEmailed.Contains(mem.MemberId))
                            {
                                //emails if its a new post, not broadcasted
                                if (IsNewPost && !IsBroadcasted && mem.EmailForumNewPost)
                                {
                                    SendEmailAboutNewForumPost(mem.UserId, mem.DerbyName);
                                    membersAlreadyEmailed.Add(mem.MemberId);
                                }
                                else if (IsBroadcasted && mem.EmailForumBroadcasts)
                                {
                                    SendEmailAboutNewForumPost(mem.UserId, mem.DerbyName);
                                    membersAlreadyEmailed.Add(mem.MemberId);
                                }
                                //MembersSent.Add(new MemberDisplayBasic() { UserId = mem.UserId, MemberId = mem.MemberId });
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return this;
        }

        public ForumNotificationFactory EmailMembersOnWatchList()
        {
            try
            {
                var dc = new ManagementContext();
                var watches = dc.ForumWatchList.Where(x => x.Topic.TopicId == TopicId).ToList();
                for (int i = 0; i < watches.Count; i++)
                {
                    if (!membersAlreadyEmailed.Contains(watches[i].ToUser.MemberId))
                    {
                        SendEmailAboutNewForumPost(watches[i].ToUser.AspNetUserId, watches[i].ToUser.DerbyName);
                        membersAlreadyEmailed.Add(watches[i].ToUser.MemberId);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return this;
        }



    }
}
