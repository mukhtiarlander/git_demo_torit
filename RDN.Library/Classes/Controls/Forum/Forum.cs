using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Context;
using System.Collections.ObjectModel;
using RDN.Library.Classes.Error;
using System.Text.RegularExpressions;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.DataModels.Member;
using RDN.Library.Cache;
using RDN.Utilities.Config;
using RDN.Utilities.Strings;
using RDN.Library.Classes.League.Classes;
using RDN.Portable.Config;
using System.Threading.Tasks;
using RDN.Library.Classes.Controls.Forum;
using RDN.Library.DataModels.Forum;
using RDN.Portable.Classes.Account.Enums;
using RDN.Portable.Classes.Forum.Enums;
using RDN.Portable.Classes.Controls.Forum;
using RDN.Library.Classes.Mobile;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Imaging;
using RDN.Portable.Classes.League.Classes;
using RDN.Library.Classes.Config;

namespace RDN.Library.Classes.Forum
{
    public class Forum
    {
        private static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static readonly int DEFAULT_PAGE_SIZE = 100;
        public Guid ForumId { get; set; }
        public string ForumName { get; set; }
        public DateTime Created { get; set; }
        public Guid FederationId { get; set; }
        public Guid LeagueId { get; set; }
        public Guid CurrentMemberId { get; set; }
        public bool IsManagerOfTopic { get; set; }
        public long CategoryId { get; set; }
        public ForumOwnerTypeEnum Type { get; set; }
        /// <summary>
        /// braodcase forum message to everyone
        /// </summary>
        public bool BroadcastToEveryone { get; set; }

        public ForumGroup CurrentGroup { get; set; }
        public List<ForumGroup> GroupTopics { get; set; }
        public List<ForumCategory> Categories { get; set; }



        public Forum()
        {
            GroupTopics = new List<ForumGroup>();
            Categories = new List<ForumCategory>();
        }

        #region MessageLike
        /// <summary>
        /// This method will add new Forum Message like.
        /// Add if it is new entry.Otherwise it will update the Total Count Column.
        /// </summary>
        /// <param name="MessageId">long MessageId</param>
        /// <param name="memberId">Guid memberId</param>
        /// <returns></returns>
        public static ForumMessage ToggleMessageLike(long MessageId, Guid memberId)
        {
            try
            {
                long total;
                var dc = new ManagementContext();
                var check = dc.ForumMessageLike.Where(x => x.Messages.MessageId == MessageId && x.Member.MemberId == memberId).FirstOrDefault();
                if (check != null)
                {
                    if (check.TotalCount == 0)
                        check.TotalCount = 1;
                    else if (check.TotalCount == 1)
                        check.TotalCount = 0;
                }
                else
                {
                    DataModels.Controls.Forum.ForumMessageLike messageLike = new DataModels.Controls.Forum.ForumMessageLike();
                    messageLike.Member = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                    messageLike.Messages = dc.ForumMessages.Where(x => x.MessageId == MessageId).FirstOrDefault();
                    messageLike.TotalCount = 1;
                    dc.ForumMessageLike.Add(messageLike);
                }
                int c = dc.SaveChanges();
                total = dc.ForumMessageLike.Where(x => x.Messages.MessageId == MessageId).Sum(x => x.TotalCount);

                if (c > 0)
                    return new ForumMessage { MessageId = MessageId, MessageLikeCount = total };
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new ForumMessage { MessageId = MessageId, MessageAgreeCount = 0 };
        }

        /// <summary>
        /// This method willupdate the total count and id not found any will entry new I Agree
        /// </summary>
        /// <param name="MessageId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static ForumMessage ToggleMessageAgreement(long MessageId, Guid memberId)
        {
            try
            {
                long total;
                var dc = new ManagementContext();
                var check = dc.ForumMessageAgree.Where(x => x.Messages.MessageId == MessageId && x.Member.MemberId == memberId).FirstOrDefault();
                if (check != null)
                {
                    if (check.TotalCount == 0)
                        check.TotalCount = 1;
                    else if (check.TotalCount == 1)
                        check.TotalCount = 0;
                }
                else
                {

                    DataModels.Controls.Forum.ForumMessageAgree messageAgree = new DataModels.Controls.Forum.ForumMessageAgree();
                    messageAgree.Member = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                    messageAgree.Messages = dc.ForumMessages.Where(x => x.MessageId == MessageId).FirstOrDefault();
                    messageAgree.TotalCount = 1;
                    dc.ForumMessageAgree.Add(messageAgree);
                }
                int c = dc.SaveChanges();


                total = dc.ForumMessageAgree.Where(x => x.Messages.MessageId == MessageId).Sum(x => x.TotalCount);//.FirstOrDefault();

                if (c > 0)
                    return new ForumMessage { MessageId = MessageId, MessageAgreeCount = total };
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new ForumMessage { MessageId = MessageId, MessageAgreeCount = 0 };
        }


        #endregion


        public static bool UpdateTopicName(Guid forumId, long topicId, string name)
        {
            try
            {
                var dc = new ManagementContext();
                var topic = dc.ForumTopics.Where(x => x.Forum.ForumId == forumId && x.TopicId == topicId).FirstOrDefault();
                topic.LastPostByMember = topic.LastPostByMember;
                topic.Forum = topic.Forum;
                topic.CreatedByMember = topic.CreatedByMember;
                topic.TopicTitle = name;

                int c = dc.SaveChanges();
                return c > 0;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool UpdateTopicCategoryAndGroup(Guid forumId, long topicId, long groupId, long categoryId)
        {
            try
            {
                var dc = new ManagementContext();
                var topic = dc.ForumTopics.Include("Category").Where(x => x.Forum.ForumId == forumId && x.TopicId == topicId).FirstOrDefault();
                topic.LastPostByMember = topic.LastPostByMember;
                if (topic.CreatedByMember != null)
                    topic.CreatedByMember = topic.CreatedByMember;
                else
                    topic.CreatedByMember = topic.LastPostByMember;
                topic.Forum = topic.Forum;
                topic.GroupId = groupId;

                if (categoryId == 0)
                {
                    dc.Entry(topic).Reference(x => x.Category).CurrentValue = null;
                }
                else
                {
                    topic.Category = dc.ForumCetegories.Where(x => x.CategoryId == categoryId && x.Forum.ForumId == forumId).FirstOrDefault();
                    topic.Category.CreatedByMember = topic.Category.CreatedByMember;
                    topic.Category.Forum = topic.Category.Forum;
                    dc.Entry(topic.Category).State = System.Data.Entity.EntityState.Modified;
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
        public static bool AddCategoryToForum(Guid forumId, string categoryName, long groupId, Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                DataModels.Forum.ForumCategories cat = new DataModels.Forum.ForumCategories();
                cat.CreatedByMember = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                cat.Forum = dc.Forums.Where(x => x.ForumId == forumId).FirstOrDefault();
                cat.GroupId = groupId;
                cat.NameOfCategory = categoryName;
                dc.ForumCetegories.Add(cat);
                int c = dc.SaveChanges();
                if (c > 0)
                    return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static int UpdateForumPostCount()
        {
            try
            {
                var dc = new ManagementContext();
                var messages = dc.ForumMessages;
                foreach (var mess in messages)
                {
                    mess.Topic = mess.Topic;
                    mess.Member = mess.Member;
                    mess.Member.TotalForumPosts += 1;
                }
                int c = dc.SaveChanges();
                return c;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }
        public static bool UpdateCatagoryToForum(Guid forumId, long categoryId, string categoryName, long groupId)
        {
            try
            {
                var dc = new ManagementContext();
                var cat = dc.ForumCetegories.Where(x => x.Forum.ForumId == forumId && x.CategoryId == categoryId).FirstOrDefault();
                if (cat != null)
                {
                    cat.CreatedByMember = cat.CreatedByMember;
                    cat.Forum = cat.Forum;
                    cat.GroupId = groupId;
                    cat.NameOfCategory = categoryName;
                    int c = dc.SaveChanges();
                    if (c > 0)
                        return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool DeleteCatagoryToForum(Guid forumId, long categoryId)
        {
            try
            {
                var dc = new ManagementContext();
                var cat = dc.ForumCetegories.Where(x => x.Forum.ForumId == forumId && x.CategoryId == categoryId).FirstOrDefault();

                if (cat != null)
                {
                    foreach (var top in cat.Topics)
                    {
                        top.Category = null;
                    }
                    cat.IsRemoved = true;
                    cat.Forum = cat.Forum;
                    cat.CreatedByMember = cat.CreatedByMember;
                    int c = dc.SaveChanges();
                    if (c > 0)
                        return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        /// <summary>
        /// toggle if broadcast to everyone.
        /// </summary>
        /// <param name="forumId"></param>
        /// <param name="groupId"></param>
        /// <param name="WillSend"></param>
        /// <returns></returns>
        public static bool BroadcastMessageChange(Guid forumId, long groupId, bool WillSend)
        {
            try
            {
                var id = RDN.Library.Classes.Account.User.GetMemberId();
                var isMem = MemberCache.IsMemberApartOfForum(id, forumId);
                if (isMem)
                {
                    var dc = new ManagementContext();
                    if (groupId == 0)
                    {
                        var f = dc.Forums.Where(x => x.ForumId == forumId).FirstOrDefault();
                        f.BroadcastToEveryone = WillSend;
                    }
                    else
                    {
                        var g = dc.LeagueGroups.Where(x => x.Id == groupId).FirstOrDefault();
                        g.BroadcastToEveryone = WillSend;
                        g.League = g.League;
                        g.GroupMembers = g.GroupMembers;
                    }
                    int c = dc.SaveChanges();
                    return c > 0;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static void UpdatePostViewCount(Guid forumId, long topicId, Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var topic = dc.ForumTopics.Where(x => x.TopicId == topicId && x.Forum.ForumId == forumId).FirstOrDefault();
                topic.Forum = topic.Forum;
                topic.LastPostByMember = topic.LastPostByMember;
                topic.CreatedByMember = topic.CreatedByMember;
                topic.ViewCount += 1;
                if (topic.TopicsInbox != null)
                {
                    var inbox = topic.TopicsInbox.Where(x => x.ToUser.MemberId == memberId).FirstOrDefault();
                    if (inbox != null)
                        dc.ForumInbox.Remove(inbox);
                }
                dc.SaveChanges();

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        public static void MarkAsRead(Guid forumId, long topicId, Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var inbox = dc.ForumInbox.Where(x => x.Topic.TopicId == topicId && x.Topic.Forum.ForumId == forumId && x.ToUser.MemberId == memberId).FirstOrDefault();
                if (inbox != null)
                    dc.ForumInbox.Remove(inbox);
                dc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        public static bool WatchTopicToggle(Guid forumId, long topicId, Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var inbox = dc.ForumTopics.Where(x => x.TopicId == topicId && x.Forum.ForumId == forumId).FirstOrDefault();
                if (inbox != null)
                {
                    var watch = inbox.TopicWatchList.Where(x => x.ToUser.MemberId == memberId).FirstOrDefault();
                    if (watch == null)
                    {
                        ForumTopicWatchList w = new ForumTopicWatchList();
                        w.Topic = inbox;
                        w.ToUser = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                        inbox.TopicWatchList.Add(w);
                    }
                    else
                    {
                        dc.ForumWatchList.Remove(watch);
                    }
                    int c = dc.SaveChanges();
                    return c > 0;
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static ForumSettings GetForumSettings(Guid forumId, long groupId)
        {
            ForumSettings settings = new ForumSettings();
            try
            {
                settings.GroupId = groupId;
                settings.Categories = new List<ForumCategory>();
                var dc = new ManagementContext();
                var forum = dc.Forums.Where(x => x.ForumId == forumId).FirstOrDefault();
                if (forum != null)
                {
                    settings.Groups = MemberCache.GetLeagueGroupsOfMember();
                    var cats = forum.Categories.Where(x => x.IsRemoved == false && x.GroupId == groupId).OrderBy(x => x.NameOfCategory);
                    foreach (var cat in cats)
                    {
                        var c = ForumCategory.DisplayCategory(cat.CategoryId, cat.NameOfCategory, cat.GroupId);
                        if (settings.Groups != null)
                        {
                            var g = settings.Groups.Where(x => x.Id == cat.GroupId).FirstOrDefault();
                            if (g != null)
                            {
                                c.GroupName = g.GroupName;

                            }
                        }
                        //if there is a group selected and this category belongs to the group
                        //if there is a group, user should only have editing abilities for that one group.
                        // or there is no group selected which means user wants all groups and categories
                        if ((groupId > 0 && cat.GroupId == groupId) || groupId == 0)
                            settings.Categories.Add(c);
                    }

                    var gg = settings.Groups.Where(x => x.Id == groupId).FirstOrDefault();
                    if (gg != null)
                    {
                        settings.GroupName = gg.GroupName;
                        settings.BroadCastPostsDefault = gg.DefaultBroadcastMessage;
                    }
                    else
                        settings.BroadCastPostsDefault = forum.BroadcastToEveryone;
                    settings.ForumId = forum.ForumId;
                    settings.ForumName = forum.ForumName;
                    if (forum.LeagueOwner != null)
                        settings.ForumType = ForumOwnerTypeEnum.league;
                    else if (forum.FederationOwner != null)
                        settings.ForumType = ForumOwnerTypeEnum.federation;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return settings;
        }

        public static ForumMessage GetForumMessage(Guid forumId, long messageId)
        {
            try
            {
                var dc = new ManagementContext();
                var message = dc.ForumMessages.Include("Mentions").Include("Mentions.Member").Where(x => x.Topic.Forum.ForumId == forumId && x.MessageId == messageId).FirstOrDefault();
                return DisplayMessage(message);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }


        /// <summary>
        /// gets the forum topic and messages of the topic.
        /// </summary>
        /// <param name="forumId"></param>
        /// <param name="topicId"></param>
        /// <returns></returns>
        public static ForumTopic GetForumTopic(Guid forumId, long topicId)
        {
            try
            {
                var dc = new ManagementContext();
                var topic = dc.ForumTopics.Include("Forum").Include("Messages").Include("Messages.Mentions").Include("Messages.Mentions.Member").Where(x => x.Forum.ForumId == forumId && x.TopicId == topicId).FirstOrDefault();
                ForumTopic top = new ForumTopic();
                if (topic.Forum.FederationOwner != null)
                    top.ForumType = ForumOwnerTypeEnum.federation;
                else if (topic.Forum.LeagueOwner != null)
                    top.ForumType = ForumOwnerTypeEnum.league;
                else
                    top.ForumType = ForumOwnerTypeEnum.main;
                top.ForumName = topic.Forum.ForumName;
                top.ForumId = forumId;
                top.CreatedByMember = new MemberDisplay { MemberId = topic.CreatedByMember.MemberId, DerbyName = topic.CreatedByMember.DerbyName };
                top.LastPostByMember = new MemberDisplay { DerbyName = topic.LastPostByMember.DerbyName, MemberId = topic.LastPostByMember.MemberId };
                top.Replies = topic.Messages.Count - 1;
                top.TopicId = topicId;
                top.TopicTitle = topic.TopicTitle;
                top.ViewCount = topic.ViewCount;
                top.Created = topic.Created + new TimeSpan(topic.Forum.LeagueOwner.TimeZone, 0, 0);
                top.GroupId = topic.GroupId;
                foreach (var watcher in topic.TopicWatchList)
                {
                    top.Watchers.Add(new ForumTopicWatchers() { MemberId = watcher.ToUser.MemberId });
                }
                //var watch = topic.TopicWatchList.Where(x => x.ToUser.MemberId == memberId).FirstOrDefault();
                //if (watch != null)
                //    top.IsWatching = true;
                if (topic.GroupId > 0)
                    top.BroadcastForumTopic = topic.Forum.BroadcastToEveryone;
                else
                {
                    var grp = dc.LeagueGroups.Where(x => x.Id == top.GroupId).FirstOrDefault();
                    if (grp != null)
                        top.BroadcastForumTopic = grp.BroadcastToEveryone;
                    else
                        top.BroadcastForumTopic = topic.Forum.BroadcastToEveryone;
                }
                top.IsPinned = topic.IsSticky;
                top.IsLocked = topic.IsLocked;
                if (topic.Category != null)
                {
                    top.Category.CategoryId = topic.Category.CategoryId;
                    top.Category.CategoryName = topic.Category.NameOfCategory;
                    top.Category.GroupId = topic.Category.GroupId;
                }

                var mess = topic.Messages.Where(x => x.IsRemoved == false);
                foreach (var message in mess)
                {
                    top.Messages.Add(DisplayMessage(message));
                }

                foreach (var inbox in topic.TopicsInbox)
                {
                    try
                    {
                        if (inbox.ToUser != null)
                        {
                            RDN.Portable.Classes.Controls.Forum.ForumTopicInbox i = new RDN.Portable.Classes.Controls.Forum.ForumTopicInbox();
                            i.DerbyName = inbox.ToUser.DerbyName;
                            i.MemberId = inbox.ToUser.MemberId;
                            i.TopicInboxId = inbox.InboxTopicId;
                            top.TopicInbox.Add(i);
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: inbox.InboxTopicId.ToString());
                    }
                }

                return top;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        private static ForumMessage DisplayMessage(DataModels.Forum.ForumMessage message)
        {
            try
            {
                var dc = new ManagementContext();
                ForumMessage me = new ForumMessage();
                me.TopicId = message.Topic.TopicId;
                me.ForumId = message.Topic.Forum.ForumId;
                me.Created = message.Created + new TimeSpan(message.Topic.Forum.LeagueOwner.TimeZone, 0, 0);
                me.CreatedHumanRelative = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(message.Created + new TimeSpan(message.Topic.Forum.LeagueOwner.TimeZone, 0, 0));
                me.Member = new MemberDisplay { TotalForumPostsCount = message.Member.TotalForumPosts, MemberId = message.Member.MemberId, DerbyName = message.Member.DerbyName, };
                if (message.Member.Gender == Convert.ToInt32(GenderEnum.Female))
                    me.Member.Gender = GenderEnum.Female;
                else if (message.Member.Gender == Convert.ToInt32(GenderEnum.Male))
                    me.Member.Gender = GenderEnum.Male;
                else
                    me.Member.Gender = GenderEnum.None;

                if (message.Member.Photos.Count > 0)
                {
                    var photo = message.Member.Photos.OrderByDescending(x => x.Created).Where(x => x.IsPrimaryPhoto == true).FirstOrDefault();
                    if (photo != null)
                    {
                        me.Member.Photos.Add(new PhotoItem(photo.ImageUrl, true, me.Member.DerbyName));
                    }
                }
                var mentions = message.Mentions.Select(x => x.Member.MemberId);
                foreach (var MemberId in mentions)
                {
                    ForumMessageMention m = new ForumMessageMention();
                    m.memberid = MemberId;
                    me.Mentions.Add(m);
                }

                if (message.LastModified > new DateTime(2013, 11, 23) || message.Created > new DateTime(2013, 11, 23))
                {
                    me.MessageHTML = message.MessageHTML;
                }
                else if (me.Created < new DateTime(2013, 11, 23))
                {
                    RDN.Library.Util.MarkdownSharp.Markdown markdown = new RDN.Library.Util.MarkdownSharp.Markdown();
                    markdown.AutoHyperlink = true;
                    markdown.AutoNewLines = true;
                    markdown.LinkEmails = true;
                    me.MessageMarkDown = message.MessageHTML;
                    me.MessageHTML = markdown.Transform(HtmlSanitize.FilterHtmlToWhitelist(message.MessageHTML));
                    me.MessageHTML = me.MessageHTML.Replace("</p>", "</p><br/>");
                }

                me.MessageId = message.MessageId;
                me.MessagePlain = message.MessagePlain;

                me.MessageLikeCount = message.MessagesLike.Sum(x => x.TotalCount);
                me.MessageAgreeCount = message.MessagesAgree.Sum(x => x.TotalCount);


                return me;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public static bool EditPost(Guid forumId, long messageId, string message)
        {
            try
            {
                var dc = new ManagementContext();
                var topic = dc.ForumMessages.Where(x => x.Topic.Forum.ForumId == forumId && x.MessageId == messageId).FirstOrDefault();
                if (topic != null)
                {
                    topic.Member = topic.Member;
                    topic.Topic = topic.Topic;
                    topic.MessageHTML = message;
                    if (!String.IsNullOrEmpty(message))
                        topic.MessagePlain = _htmlRegex.Replace(message, " ");
                    else
                        topic.MessagePlain = message;
                    int c = dc.SaveChanges();
                    return c > 0;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static void ReplyToPost(Guid forumId, long topicId, string message, Guid memberId, bool emailGroupAboutPost, List<Guid> mentionedMemberIds)
        {
            try
            {

                var dc = new ManagementContext();
                var topic = dc.ForumTopics.Where(x => x.Forum.ForumId == forumId).Where(x => x.TopicId == topicId).FirstOrDefault();
                if (topic != null)
                {
                    var member = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                    member.TotalForumPosts = member.TotalForumPosts + 1;
                    DataModels.Forum.ForumMessage mess = new DataModels.Forum.ForumMessage();
                    mess.Member = member;
                    if (!String.IsNullOrEmpty(message))
                    {
                        message = message.Replace(Environment.NewLine, "<br/>");
                        message = message.Replace("\r", "<br/>");
                    }
                    mess.MessageHTML = message;
                    if (!String.IsNullOrEmpty(message))
                        mess.MessagePlain = _htmlRegex.Replace(message, " ");
                    mess.Topic = topic;
                    topic.Forum = topic.Forum;
                    mess.LastModified = DateTime.UtcNow;
                    topic.LastModified = DateTime.UtcNow;
                    dc.Entry(topic).Reference(c => c.LastPostByMember).Load();
                    topic.LastPostByMember = member;
                    topic.LastPostDateTime = DateTime.UtcNow;
                    topic.CreatedByMember = topic.CreatedByMember;

                    foreach (var id in mentionedMemberIds)
                    {
                        ForumMessageMentionDb mention = new ForumMessageMentionDb();
                        mention.Member = dc.Members.Where(x => x.MemberId == id).FirstOrDefault();
                        mess.Mentions.Add(mention);
                    }

                    dc.ForumMessages.Add(mess);
                    int ch = dc.SaveChanges();

                    UpdateForumInbox(dc, topic, memberId);
                    string groupName = "Forum";
                    if (topic.Forum.LeagueOwner != null)
                        groupName = topic.Forum.LeagueOwner.Name;
                    if (topic.GroupId > 0)
                    {
                        try
                        {
                            var group = SiteCache.GetAllGroups().Where(x => x != null && x.Id == topic.GroupId).FirstOrDefault();
                            if (group != null)
                                groupName = group.GroupName;
                        }
                        catch (Exception exception)
                        {
                            ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: forumId + ":" + topicId + ":" + message + ":" + memberId + ":" + emailGroupAboutPost + ":" + topic.GroupId);
                        }
                    }
                    Guid ownerId = new Guid();
                    if (topic.Forum.LeagueOwner != null)
                        ownerId = topic.Forum.LeagueOwner.LeagueId;

                    var notify = new ForumNotificationManager(forumId, ownerId, false, emailGroupAboutPost, topic.GroupId, topic.TopicId, groupName, topic.TopicTitle, message, member.MemberId, member.DerbyName, mess.MessageId)
                    .LeagueEmailAboutForumPost()
                    .EmailMembersOnWatchList()
                    .EmailMemberMentions();


                    var fact = new MobileNotificationFactory()
                           .Initialize("Forum Reply:", topic.TopicTitle, Mobile.Enums.NotificationTypeEnum.Forum)
                           .AddId(topic.TopicId)
                           .AddMembers(notify.membersAlreadyEmailed)
                           .SendNotifications();
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: forumId + ":" + topicId + ":" + message + ":" + memberId + ":" + emailGroupAboutPost);
            }
        }

        /// <summary>
        /// updates the forum No Read inbox.  Grabs all members of the topic and
        /// iterates through adding them to the HAsn't Read list.  So we can show users if topics have been read or not.
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        private static bool UpdateForumInbox(ManagementContext dc, DataModels.Forum.ForumTopic topic, Guid memberWhoPosted)
        {

            if (topic.Forum != null)
            {
                if (topic.Forum.LeagueOwner != null)
                {
                    if (topic.GroupId > 0)
                    {
                        var group = League.Classes.LeagueGroupFactory.GetGroup(topic.GroupId, topic.Forum.LeagueOwner.LeagueId);
                        foreach (var mem in group.GroupMembers)
                        {
                            //the member who posted shouldn't be getting the inbox item added for them.
                            if (mem.MemberId != memberWhoPosted)
                            {
                                var inboxItem = dc.ForumInbox.Where(x => x.ToUser.MemberId == mem.MemberId && x.Topic.TopicId == topic.TopicId).FirstOrDefault();
                                if (inboxItem == null)
                                {
                                    DataModels.Forum.ForumTopicInbox inbox = new DataModels.Forum.ForumTopicInbox();
                                    inbox.Topic = topic;
                                    inbox.ToUser = dc.Members.Where(x => x.MemberId == mem.MemberId).FirstOrDefault();
                                    topic.TopicsInbox.Add(inbox);
                                }
                            }
                        }
                    }
                    else
                    {
                        var group = League.LeagueFactory.GetActiveLeagueMembers(topic.Forum.LeagueOwner.LeagueId);
                        foreach (var mem in group)
                        {
                            //the member who posted shouldn't be getting the inbox item added for them.
                            if (mem.MemberId != memberWhoPosted)
                            {
                                var inboxItem = dc.ForumInbox.Where(x => x.ToUser.MemberId == mem.MemberId && x.Topic.TopicId == topic.TopicId).FirstOrDefault();
                                if (inboxItem == null)
                                {
                                    DataModels.Forum.ForumTopicInbox inbox = new DataModels.Forum.ForumTopicInbox();
                                    inbox.Topic = topic;
                                    inbox.ToUser = dc.Members.Where(x => x.MemberId == mem.MemberId).FirstOrDefault();
                                    topic.TopicsInbox.Add(inbox);
                                }
                            }
                        }
                    }
                }
                else if (topic.Forum.FederationOwner != null)
                {
                    var group = Federation.Federation.GetMembersOfFederation(topic.Forum.FederationOwner.FederationId);
                    foreach (var mem in group)
                    {
                        //the member who posted shouldn't be getting the inbox item added for them.
                        if (mem.MemberId != memberWhoPosted)
                        {
                            var inboxItem = dc.ForumInbox.Where(x => x.ToUser.MemberId == mem.MemberId && x.Topic.TopicId == topic.TopicId).FirstOrDefault();
                            if (inboxItem == null)
                            {
                                DataModels.Forum.ForumTopicInbox inbox = new DataModels.Forum.ForumTopicInbox();
                                inbox.Topic = topic;
                                inbox.ToUser = dc.Members.Where(x => x.MemberId == mem.MemberId).FirstOrDefault();
                                dc.ForumInbox.Add(inbox);
                            }
                        }
                    }
                }
            }
            int c = dc.SaveChanges();
            return c > 0;
        }
        public static bool RemoveTopic(Guid forumId, long topicId)
        {
            try
            {
                var dc = new ManagementContext();
                var topic = dc.ForumTopics.Where(x => x.Forum.ForumId == forumId).Where(x => x.TopicId == topicId).FirstOrDefault();
                if (topic != null)
                {
                    topic.IsRemoved = true;
                    topic.CreatedByMember = topic.CreatedByMember;
                    topic.LastPostByMember = topic.LastPostByMember;
                    topic.Forum = topic.Forum;
                    int c = dc.SaveChanges();
                    if (c > 0)
                        return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool DoUpgradeToAllTopics()
        {
            try
            {
                var dc = new ManagementContext();
                var topics = dc.ForumTopics.Where(x => x.IsRemoved == false);
                foreach (var topic in topics)
                {
                    if (topic != null)
                    {
                        topic.LastPostDateTime = topic.LastModified;
                        topic.CreatedByMember = topic.CreatedByMember;
                        topic.LastPostByMember = topic.LastPostByMember;
                        topic.Forum = topic.Forum;

                    }
                }
                int c = dc.SaveChanges();
                if (c > 0)
                    return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool PinTopic(Guid forumId, long topicId, bool pin)
        {
            try
            {
                var dc = new ManagementContext();
                var topic = dc.ForumTopics.Where(x => x.Forum.ForumId == forumId).Where(x => x.TopicId == topicId).FirstOrDefault();
                if (topic != null)
                {
                    topic.IsSticky = pin;
                    topic.CreatedByMember = topic.CreatedByMember;
                    topic.LastPostByMember = topic.LastPostByMember;
                    topic.Forum = topic.Forum;
                    int c = dc.SaveChanges();
                    if (c > 0)
                        return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool LockTopic(Guid forumId, long topicId, bool lockTopic)
        {
            try
            {
                var dc = new ManagementContext();
                var topic = dc.ForumTopics.Where(x => x.Forum.ForumId == forumId).Where(x => x.TopicId == topicId).FirstOrDefault();
                if (topic != null)
                {
                    topic.IsLocked = lockTopic;
                    topic.CreatedByMember = topic.CreatedByMember;
                    topic.LastPostByMember = topic.LastPostByMember;
                    topic.Forum = topic.Forum;
                    int c = dc.SaveChanges();
                    if (c > 0)
                        return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        /// <summary>
        /// archives the topic.
        /// </summary>
        /// <param name="forumId"></param>
        /// <param name="topicId"></param>
        /// <param name="lockTopic"></param>
        /// <returns></returns>
        public static bool ArchiveTopic(Guid forumId, long topicId, bool lockTopic)
        {
            try
            {
                var dc = new ManagementContext();
                var topic = dc.ForumTopics.Where(x => x.Forum.ForumId == forumId).Where(x => x.TopicId == topicId).FirstOrDefault();
                if (topic != null)
                {
                    topic.IsArchived = lockTopic;
                    topic.CreatedByMember = topic.CreatedByMember;
                    topic.LastPostByMember = topic.LastPostByMember;
                    topic.Forum = topic.Forum;
                    int c = dc.SaveChanges();

                    return c > 0;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool RemovePost(Guid forumId, long topicId, long postId)
        {
            try
            {
                var dc = new ManagementContext();
                var topic = dc.ForumTopics.Where(x => x.Forum.ForumId == forumId).Where(x => x.TopicId == topicId).FirstOrDefault();
                if (topic != null)
                {
                    var mess = dc.ForumMessages.Where(x => x.MessageId == postId).FirstOrDefault();
                    mess.Member = mess.Member;
                    mess.Topic = topic;
                    mess.IsRemoved = true;
                    int c = dc.SaveChanges();
                    if (c > 0)
                        return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        /// <summary>
        /// creates a new forum topic and post.
        /// </summary>
        /// <param name="forumId"></param>
        /// <param name="forumType"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static long CreateNewForumTopicAndPost(Guid forumId, ForumOwnerTypeEnum forumType, string subject, string message, Guid memberId, long groupId, bool emailGroupAboutPost, bool pinMessage, bool lockMessage, long chosenCategory, List<Guid> membersBeingMentioned)
        {
            try
            {
                var dc = new ManagementContext();
                var forum = dc.Forums.Where(x => x.ForumId == forumId).FirstOrDefault();
                if (forum != null)
                {
                    var member = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                    member.TotalForumPosts = member.TotalForumPosts + 1;
                    DataModels.Forum.ForumMessage mess = new DataModels.Forum.ForumMessage();
                    foreach (Guid id in membersBeingMentioned)
                    {
                        DataModels.Forum.ForumMessageMentionDb mention = new DataModels.Forum.ForumMessageMentionDb();
                        mention.Member = dc.Members.Where(x => x.MemberId == id).FirstOrDefault();
                        mess.Mentions.Add(mention);
                    }

                    mess.Member = member;
                    if (!String.IsNullOrEmpty(message))
                    {
                        message = message.Replace(Environment.NewLine, "<br/>");
                        message = message.Replace("\r", "<br/>");
                    }
                    mess.MessageHTML = message;
                    if (!String.IsNullOrEmpty(message))
                        mess.MessagePlain = _htmlRegex.Replace(message, " ");
                    else
                        mess.MessagePlain = message;
                    mess.LastModified = DateTime.UtcNow;


                    RDN.Library.DataModels.Forum.ForumTopic topic = new DataModels.Forum.ForumTopic();
                    topic.CreatedByMember = member;
                    topic.LastPostDateTime = DateTime.UtcNow;
                    topic.LastPostByMember = member;
                    if (groupId > 0)
                        topic.GroupId = groupId;
                    topic.Forum = forum;
                    topic.Messages.Add(mess);
                    topic.TopicTitle = subject;
                    topic.LastModified = DateTime.UtcNow;
                    topic.IsSticky = pinMessage;
                    topic.IsLocked = lockMessage;
                    if (chosenCategory > 0)
                        topic.Category = forum.Categories.Where(x => x.CategoryId == chosenCategory).FirstOrDefault();

                    forum.Topics.Add(topic);
                    int c = dc.SaveChanges();
                    UpdateForumInbox(dc, topic, memberId);

                    string groupName = "Forum";
                    if (topic.Forum.LeagueOwner != null)
                        groupName = topic.Forum.LeagueOwner.Name;
                    if (topic.GroupId > 0)
                    {
                        try
                        {
                            var group = SiteCache.GetAllGroups().Where(x => x != null && x.Id == topic.GroupId).FirstOrDefault();
                            if (group != null)
                                groupName = group.GroupName;
                        }
                        catch (Exception exception)
                        {
                            ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: forumId.ToString() + ":" + forumType + ":" + subject + ":" + message + ":" + memberId.ToString() + ":" + groupId + ":" + emailGroupAboutPost + ":" + pinMessage + ":" + lockMessage + ":" + chosenCategory);
                        }
                    }
                    //make sure the forum has an owner.
                    if (topic.Forum.LeagueOwner != null)
                    {
                        var notify = new ForumNotificationManager(forumId, topic.Forum.LeagueOwner.LeagueId, true, emailGroupAboutPost, topic.GroupId, topic.TopicId, groupName, topic.TopicTitle, mess.MessageHTML, member.MemberId, member.DerbyName, mess.MessageId)
                        .LeagueEmailAboutForumPost()
                        .EmailMembersOnWatchList()
                        .EmailMemberMentions();

                        var fact = new MobileNotificationFactory()
                            .Initialize("Forum Post:", topic.TopicTitle, Mobile.Enums.NotificationTypeEnum.Forum)
                            .AddId(topic.TopicId)
                            .AddMembers(notify.membersAlreadyEmailed)
                            .SendNotifications();
                    }
                    return topic.TopicId;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: forumId.ToString() + ":" + forumType + ":" + subject + ":" + message + ":" + memberId.ToString() + ":" + groupId + ":" + emailGroupAboutPost + ":" + pinMessage + ":" + lockMessage + ":" + chosenCategory);
            }
            return 0;
        }

        /// <summary>
        /// gets the forum id of the league.
        /// </summary>
        /// <param name="leagueId"></param>
        /// <returns></returns>
        public static Guid GetForumIdOfLeague(Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var forum = dc.Forums.Where(x => x.LeagueOwner.LeagueId == leagueId).FirstOrDefault();
                if (forum != null)
                    return forum.ForumId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }
        public static Guid GetLeagueIdOfForum(Guid forumId)
        {
            try
            {
                var dc = new ManagementContext();
                var forum = dc.Forums.Where(x => x.ForumId == forumId).FirstOrDefault();
                if (forum != null && forum.LeagueOwner != null)
                    return forum.LeagueOwner.LeagueId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }
        public static Guid GetFederatonIdOfForum(Guid forumId)
        {
            try
            {
                var dc = new ManagementContext();
                var forum = dc.Forums.Where(x => x.ForumId == forumId).FirstOrDefault();
                if (forum != null && forum.FederationOwner != null)
                    return forum.FederationOwner.FederationId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }
        public static bool DoesUserBelongToForum(Guid memId, Guid forumId)
        {
            try
            {
                var dc = new ManagementContext();
                var forum = dc.Forums.Where(x => x.ForumId == forumId).FirstOrDefault();
                if (forum.LeagueOwner != null)
                {
                    var mem = forum.LeagueOwner.Members.Where(x => x.Member.MemberId == memId && x.HasLeftLeague == false).FirstOrDefault();
                    if (mem != null)
                        return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        /// <summary>
        /// gets the forum id of the federation.
        /// </summary>
        /// <param name="federationId"></param>
        /// <returns></returns>
        public static Guid GetForumIdOfFederation(Guid federationId)
        {
            try
            {
                var dc = new ManagementContext();
                var forum = dc.Forums.Where(x => x.FederationOwner.FederationId == federationId).FirstOrDefault();
                if (forum != null)
                    return forum.ForumId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }

        /// <summary>
        /// creates a new forum.
        /// </summary>
        /// <param name="ownerId">the league or federation id</param>
        /// <param name="ownerType">type of owner so we can choose who owns it.</param>
        /// <param name="forumName">name of the forum.</param>
        public static Guid CreateNewForum(Guid ownerId, ForumOwnerTypeEnum ownerType, string forumName)
        {
            try
            {
                var dc = new ManagementContext();
                RDN.Library.DataModels.Forum.Forum forum = new DataModels.Forum.Forum();
                if (ownerType == ForumOwnerTypeEnum.federation)
                {
                    forum.FederationOwner = dc.Federations.Where(x => x.FederationId == ownerId).FirstOrDefault();
                }
                else if (ownerType == ForumOwnerTypeEnum.league)
                {
                    forum.LeagueOwner = dc.Leagues.Where(x => x.LeagueId == ownerId).FirstOrDefault();
                }
                forum.ForumName = forumName;
                dc.Forums.Add(forum);
                dc.SaveChanges();
                return forum.ForumId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="forumId"></param>
        /// <param name="groupId"></param>
        /// <param name="categoryId">id of the category selected.
        /// Category ID could be -1 which in that case the user selected just UNREAD messages.
        /// </param>
        /// <returns></returns>
        public static List<ForumTopicJson> GetForumTopicsJson(int page, int count, Guid forumId, long groupId, long categoryId, bool isArchived, ForumOwnerTypeEnum forumType)
        {

            List<ForumTopicJson> topics = new List<ForumTopicJson>();
            try
            {
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                var dc = new ManagementContext();
                int pNum = page * count;

                //because if the page is greater than 1, then the league already went through the default 100 topics.
                if (page > 0)
                    pNum = ((page - 1) * count) + Forum.DEFAULT_PAGE_SIZE;

                var groups = MemberCache.GetGroupsApartOf(memId).ToList();

                var groupIds = groups.Select(x => x.Id).ToList();
                groupIds.Add(0);
                List<DataModels.Forum.ForumTopic> dbSticky = new List<DataModels.Forum.ForumTopic>();
                List<DataModels.Forum.ForumTopic> db = new List<DataModels.Forum.ForumTopic>();

                if (groupIds.Where(x => x == groupId).FirstOrDefault() != null)
                {

                    if (categoryId == 0 || categoryId == -1)
                    {
                        //don't call stickies if we are on second page.
                        if (page == 0)
                        {
                            dbSticky = (from xx in dc.ForumTopics
                                        where xx.Forum.ForumId == forumId
                                        where xx.GroupId == groupId
                                        where xx.IsRemoved == false
                                        where xx.IsSticky == true
                                        where xx.IsArchived == isArchived
                                        select xx).OrderByDescending(x => x.LastPostDateTime).AsParallel().ToList();
                        }
                        db = (from xx in dc.ForumTopics
                              where xx.Forum.ForumId == forumId
                              where xx.GroupId == groupId
                              where xx.IsRemoved == false
                              where xx.IsArchived == isArchived
                              select xx).OrderByDescending(x => x.LastPostDateTime).Skip(pNum).Take(count).AsParallel().ToList();
                    }
                    else
                    {
                        //don't call stickies if we are on second page.
                        if (page == 0)
                        {
                            dbSticky = (from xx in dc.ForumTopics
                                        where xx.Forum.ForumId == forumId
                                        where xx.Category.CategoryId == categoryId
                                        where xx.GroupId == groupId
                                        where xx.IsRemoved == false
                                        where xx.IsArchived == isArchived
                                        where xx.IsSticky == true
                                        select xx).OrderByDescending(x => x.LastPostDateTime).AsParallel().ToList();
                        }
                        db = (from xx in dc.ForumTopics
                              where xx.Forum.ForumId == forumId
                              where xx.Category.CategoryId == categoryId
                              where xx.GroupId == groupId
                              where xx.IsRemoved == false
                              where xx.IsArchived == isArchived
                              select xx).OrderByDescending(x => x.LastPostDateTime).Skip(pNum).Take(count).AsParallel().ToList();
                    }
                    bool isManager = false;
                    bool isModerator = false;

                    if (forumType == ForumOwnerTypeEnum.league)
                    {
                        isManager = RDN.Library.Cache.MemberCache.IsManagerOrBetterOfLeague(memId);
                        isModerator = MemberCache.IsModeratorOrBetterOfLeagueGroup(memId, groupId);
                    }
                    else if (forumType == ForumOwnerTypeEnum.main)
                    {
                        isManager = MemberCache.IsAdministrator(memId);
                        isModerator = MemberCache.IsAdministrator(memId);
                    }
                    if (categoryId != -1)
                    {
                        foreach (var message in dbSticky)
                        {
                            if (topics.Where(x => x.TopicId == message.TopicId).FirstOrDefault() == null)
                            {
                                ForumTopicJson top = new ForumTopicJson();
                                if (message.Category != null)
                                {
                                    top.Category = message.Category.NameOfCategory;
                                    top.CategoryId = message.Category.CategoryId;
                                }
                                top.ForumOwnerTypeEnum = forumType.ToString();
                                top.ForumId = forumId;
                                top.IsArchived = isArchived;
                                top.LastModified = message.LastModified.GetValueOrDefault();
                                top.Created = message.Created + new TimeSpan(message.Forum.LeagueOwner.TimeZone, 0, 0);
                                top.CreatedHuman = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(message.Created + new TimeSpan(message.Forum.LeagueOwner.TimeZone, 0, 0));
                                top.CreatedByMember = new MemberDisplayBasic();
                                top.CreatedByMember.DerbyName = message.CreatedByMember.DerbyName;
                                top.CreatedByMember.DerbyNameUrl = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.CreatedByMember.DerbyName);
                                top.CreatedByMember.MemberId = message.CreatedByMember.MemberId;
                                top.LastPostByMember = new MemberDisplayBasic();
                                top.LastPostByMember.DerbyName = message.LastPostByMember.DerbyName;
                                top.LastPostByMember.MemberId = message.LastPostByMember.MemberId;
                                top.LastPostByMember.DerbyNameUrl = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.LastPostByMember.DerbyName);
                                if (message.LastPostDateTime != null)
                                    top.LastPostHuman = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(message.LastPostDateTime.GetValueOrDefault());
                                else
                                    top.LastPostHuman = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(message.LastModified.GetValueOrDefault());

                                top.TopicId = message.TopicId;
                                top.GroupId = message.GroupId;
                                top.TopicTitle = message.TopicTitle;
                                top.TopicTitleForUrl = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.TopicTitle);
                                top.ViewCount = message.ViewCount;
                                top.Replies = message.Messages.Where(x => x.IsRemoved == false).Count() - 1;
                                top.IsLocked = message.IsLocked;
                                top.IsPinned = message.IsSticky;
                                //is league manager
                                top.IsManagerOfTopic = isManager;
                                //if nt league manager and group id > 0
                                if (message.GroupId > 0 && !isManager)
                                    top.IsManagerOfTopic = isModerator;

                                if (message.TopicsInbox.Where(x => x.ToUser.MemberId == memId).FirstOrDefault() == null)
                                    top.IsRead = true;

                                if (!top.IsPinned)
                                    topics.Add(top);
                                else
                                    topics.Insert(0, top);
                            }
                        }
                    }
                    foreach (var message in db)
                    {
                        if (topics.Where(x => x.TopicId == message.TopicId).FirstOrDefault() == null)
                        {
                            ForumTopicJson top = new ForumTopicJson();
                            if (message.Category != null)
                            {
                                top.Category = message.Category.NameOfCategory;
                                top.CategoryId = message.Category.CategoryId;
                            }
                            top.ForumOwnerTypeEnum = forumType.ToString();
                            top.ForumId = forumId;
                            top.IsArchived = isArchived;
                            top.LastModified = message.LastModified.GetValueOrDefault();
                            top.Created = message.Created + new TimeSpan(message.Forum.LeagueOwner.TimeZone, 0, 0);
                            top.CreatedHuman = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(message.Created + new TimeSpan(message.Forum.LeagueOwner.TimeZone, 0, 0));
                            top.CreatedByMember = new MemberDisplayBasic();
                            top.CreatedByMember.DerbyName = message.CreatedByMember.DerbyName;
                            top.CreatedByMember.DerbyNameUrl = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.CreatedByMember.DerbyName);
                            top.CreatedByMember.MemberId = message.CreatedByMember.MemberId;
                            top.LastPostByMember = new MemberDisplayBasic();
                            top.LastPostByMember.DerbyName = message.LastPostByMember.DerbyName;
                            top.LastPostByMember.MemberId = message.LastPostByMember.MemberId;
                            top.LastPostByMember.DerbyNameUrl = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.LastPostByMember.DerbyName);
                            if (message.LastPostDateTime != null)
                                top.LastPostHuman = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(message.LastPostDateTime.GetValueOrDefault());
                            else
                                top.LastPostHuman = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(message.LastModified.GetValueOrDefault());

                            top.TopicId = message.TopicId;
                            top.GroupId = message.GroupId;
                            top.TopicTitle = message.TopicTitle;
                            top.TopicTitleForUrl = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.TopicTitle);
                            top.ViewCount = message.ViewCount;
                            top.Replies = message.Messages.Where(x => x.IsRemoved == false).Count() - 1;
                            top.IsLocked = message.IsLocked;
                            top.IsPinned = message.IsSticky;
                            //is league manager
                            top.IsManagerOfTopic = isManager;
                            //if nt league manager and group id > 0
                            if (message.GroupId > 0 && !isManager)
                                top.IsManagerOfTopic = isModerator;

                            if (message.TopicsInbox.Where(x => x.ToUser.MemberId == memId).FirstOrDefault() == null)
                                top.IsRead = true;
                            if (categoryId != -1)
                            {
                                //users selects something other than unread topics.
                                if (!top.IsPinned)
                                    topics.Add(top);
                                else
                                    topics.Insert(0, top);
                            }
                            else if (top.IsRead == false)
                            {
                                //case happens when the user selects to only see unread topics.
                                //which means the category will be -1.
                                topics.Add(top);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return topics;
        }
        public static List<ForumTopicJson> GetForumTopicsJsonUnread(Guid forumId, long groupId, Guid memId, int take = 500)
        {

            List<ForumTopicJson> topics = new List<ForumTopicJson>();
            try
            {

                var dc = new ManagementContext();
                var groups = MemberCache.GetGroupsApartOf(memId).ToList();

                var groupIds = groups.Select(x => x.Id).ToList();
                groupIds.Add(0);

                if (groupIds.Where(x => x == groupId).FirstOrDefault() != null)
                {
                    //otherwise take just the count.
                    var db = (from xx in dc.ForumInbox
                              where xx.ToUser.MemberId == memId
                              where xx.Topic.Forum.ForumId == forumId
                              where xx.Topic.IsRemoved == false
                              select new
                              {
                                  Created = xx.Topic.Created,
                                  topic = xx.Topic,
                                  LastModified = xx.LastModified
                              }).OrderByDescending(x => x.LastModified).Take(take).AsParallel().ToList();


                    bool isManager = RDN.Library.Cache.MemberCache.IsManagerOrBetterOfLeague(memId);
                    bool isModerator = MemberCache.IsModeratorOrBetterOfLeagueGroup(memId, groupId);
                    foreach (var message in db)
                    {
                        if (topics.Where(x => x.TopicId == message.topic.TopicId).FirstOrDefault() == null)
                        {
                            ForumTopicJson top = new ForumTopicJson();
                            if (message.topic.Category != null)
                            {
                                top.Category = message.topic.Category.NameOfCategory;
                                top.CategoryId = message.topic.Category.CategoryId;
                            }
                            top.ForumId = forumId;
                            top.LastModified = message.topic.LastModified.GetValueOrDefault();
                            top.Created = message.topic.Created + new TimeSpan(message.topic.Forum.LeagueOwner.TimeZone, 0, 0);
                            top.CreatedHuman = RDN.Portable.Util.DateTimes.DateTimeExt.RelativeDateTime(message.topic.Created + new TimeSpan(message.topic.Forum.LeagueOwner.TimeZone, 0, 0));
                            top.CreatedByMember = new MemberDisplayBasic();
                            top.CreatedByMember.DerbyName = message.topic.CreatedByMember.DerbyName;
                            top.CreatedByMember.DerbyNameUrl = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.topic.CreatedByMember.DerbyName);
                            top.CreatedByMember.MemberId = message.topic.CreatedByMember.MemberId;
                            top.LastPostByMember = new MemberDisplayBasic();
                            top.LastPostByMember.DerbyName = message.topic.LastPostByMember.DerbyName;
                            top.LastPostByMember.MemberId = message.topic.LastPostByMember.MemberId;
                            top.LastPostByMember.DerbyNameUrl = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.topic.LastPostByMember.DerbyName);
                            if (message.topic.LastPostDateTime != null)
                                top.LastPostHuman = RDN.Portable.Util.DateTimes.DateTimeExt.RelativeDateTime(message.topic.LastPostDateTime.GetValueOrDefault());
                            else
                                top.LastPostHuman = RDN.Portable.Util.DateTimes.DateTimeExt.RelativeDateTime(message.topic.LastModified.GetValueOrDefault());

                            top.TopicId = message.topic.TopicId;
                            top.GroupId = message.topic.GroupId;
                            top.TopicTitle = message.topic.TopicTitle;
                            top.TopicTitleForUrl = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.topic.TopicTitle);
                            top.ViewCount = message.topic.ViewCount;
                            top.Replies = message.topic.Messages.Where(x => x.IsRemoved == false).Count() - 1;
                            top.IsLocked = message.topic.IsLocked;
                            top.IsPinned = message.topic.IsSticky;
                            //is league manager
                            top.IsManagerOfTopic = isManager;
                            //if nt league manager and group id > 0
                            if (message.topic.GroupId > 0 && !isManager)
                                top.IsManagerOfTopic = isModerator;

                            var groupname = MemberCache.GetGroupsApartOf(RDN.Library.Classes.Account.User.GetMemberId()).Where(w => w.Id == message.topic.GroupId).FirstOrDefault();
                            if (groupname != null)
                                top.ForumGroup = groupname.GroupName;
                            //case happens when the user selects to only see unread topics.
                            //which means the category will be -1.
                            topics.Add(top);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return topics;
        }

        public static List<ForumTopicJson> GetForumTopicsJson(int page, int count, Guid forumId, long groupId)
        {
            List<ForumTopicJson> topics = new List<ForumTopicJson>();
            try
            {
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                var dc = new ManagementContext();
                int pNum = (page - 1) * count;
                var groups = MemberCache.GetGroupsApartOf(memId).ToList();

                var groupIds = groups.Select(x => x.Id).ToList();
                groupIds.Add(0);

                if (groupIds.Where(x => x == groupId).FirstOrDefault() != null)
                {
                    var dbSticky = (from xx in dc.ForumMessages
                                    where xx.Topic.Forum.ForumId == forumId
                                    where xx.Topic.GroupId == groupId
                                    where xx.Topic.IsRemoved == false
                                    where xx.Topic.IsSticky == true
                                    select new
                                    {
                                        Created = xx.Created,
                                        topic = xx.Topic,
                                        LastModified = xx.LastModified
                                    }).OrderByDescending(x => x.LastModified).AsParallel().ToList();

                    var db = (from xx in dc.ForumMessages
                              where xx.Topic.Forum.ForumId == forumId
                              where xx.Topic.GroupId == groupId
                              where xx.Topic.IsRemoved == false
                              select new
{
    Created = xx.Created,
    topic = xx.Topic,
    LastModified = xx.LastModified
}).OrderByDescending(x => x.LastModified).Skip(pNum).Take(count).AsParallel().ToList();

                    bool isManager = RDN.Library.Cache.MemberCache.IsManagerOrBetterOfLeague(memId);
                    bool isModerator = MemberCache.IsModeratorOrBetterOfLeagueGroup(memId, groupId);
                    foreach (var message in dbSticky)
                    {
                        if (topics.Where(x => x.TopicId == message.topic.TopicId).FirstOrDefault() == null)
                        {
                            ForumTopicJson top = new ForumTopicJson();
                            if (message.topic.Category != null)
                            {
                                top.Category = message.topic.Category.NameOfCategory;
                                top.CategoryId = message.topic.Category.CategoryId;
                            }
                            top.ForumId = forumId;
                            top.LastModified = (message.topic.LastModified + new TimeSpan(message.topic.Forum.LeagueOwner.TimeZone, 0, 0)).GetValueOrDefault();
                            top.Created = message.topic.Created + new TimeSpan(message.topic.Forum.LeagueOwner.TimeZone, 0, 0);
                            top.CreatedHuman = RDN.Portable.Util.DateTimes.DateTimeExt.RelativeDateTime(message.topic.Created + new TimeSpan(message.topic.Forum.LeagueOwner.TimeZone, 0, 0));
                            top.CreatedByMember = new MemberDisplayBasic();
                            top.CreatedByMember.DerbyName = message.topic.CreatedByMember.DerbyName;
                            top.CreatedByMember.DerbyNameUrl = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.topic.CreatedByMember.DerbyName);
                            top.CreatedByMember.MemberId = message.topic.CreatedByMember.MemberId;
                            top.LastPostByMember = new MemberDisplayBasic();
                            top.LastPostByMember.DerbyName = message.topic.LastPostByMember.DerbyName;
                            top.LastPostByMember.MemberId = message.topic.LastPostByMember.MemberId;
                            if (message.topic.LastPostDateTime != null)
                                top.LastPostHuman = RDN.Portable.Util.DateTimes.DateTimeExt.RelativeDateTime((message.topic.LastPostDateTime + new TimeSpan(message.topic.Forum.LeagueOwner.TimeZone, 0, 0)).GetValueOrDefault());
                            else
                                top.LastPostHuman = RDN.Portable.Util.DateTimes.DateTimeExt.RelativeDateTime((message.topic.LastModified + new TimeSpan(message.topic.Forum.LeagueOwner.TimeZone, 0, 0)).GetValueOrDefault());
                            top.LastPostByMember.DerbyNameUrl = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.topic.LastPostByMember.DerbyName);
                            top.TopicId = message.topic.TopicId;
                            top.GroupId = message.topic.GroupId;
                            top.TopicTitle = message.topic.TopicTitle;
                            top.TopicTitleForUrl = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.topic.TopicTitle);
                            top.ViewCount = message.topic.ViewCount;
                            top.Replies = message.topic.Messages.Where(x => x.IsRemoved == false).Count() - 1;
                            top.IsLocked = message.topic.IsLocked;
                            top.IsPinned = message.topic.IsSticky;
                            //is league manager
                            top.IsManagerOfTopic = isManager;
                            //if nt league manager and group id > 0
                            if (message.topic.GroupId > 0 && !isManager)
                                top.IsManagerOfTopic = isModerator;

                            if (message.topic.TopicsInbox.Where(x => x.ToUser.MemberId == memId).FirstOrDefault() == null)
                                top.IsRead = true;

                            if (!top.IsPinned)
                                topics.Add(top);
                            else
                                topics.Insert(0, top);
                        }
                    }
                    foreach (var message in db)
                    {
                        if (topics.Where(x => x.TopicId == message.topic.TopicId).FirstOrDefault() == null)
                        {
                            ForumTopicJson top = new ForumTopicJson();
                            if (message.topic.Category != null)
                            {
                                top.Category = message.topic.Category.NameOfCategory;
                                top.CategoryId = message.topic.Category.CategoryId;
                            }
                            top.ForumId = forumId;
                            top.LastModified = (message.topic.LastModified + new TimeSpan(message.topic.Forum.LeagueOwner.TimeZone, 0, 0)).GetValueOrDefault();
                            top.Created = message.topic.Created + new TimeSpan(message.topic.Forum.LeagueOwner.TimeZone, 0, 0);
                            top.CreatedHuman = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(message.topic.Created + new TimeSpan(message.topic.Forum.LeagueOwner.TimeZone, 0, 0));
                            top.CreatedByMember = new MemberDisplayBasic();
                            top.CreatedByMember.DerbyName = message.topic.CreatedByMember.DerbyName;
                            top.CreatedByMember.DerbyNameUrl = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.topic.CreatedByMember.DerbyName);
                            top.CreatedByMember.MemberId = message.topic.CreatedByMember.MemberId;
                            top.LastPostByMember = new MemberDisplayBasic();
                            top.LastPostByMember.DerbyName = message.topic.LastPostByMember.DerbyName;
                            top.LastPostByMember.MemberId = message.topic.LastPostByMember.MemberId;
                            top.LastPostByMember.DerbyNameUrl = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.topic.LastPostByMember.DerbyName);
                            if (message.topic.LastPostDateTime != null)
                                top.LastPostHuman = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime((message.topic.LastPostDateTime + new TimeSpan(message.topic.Forum.LeagueOwner.TimeZone, 0, 0)).GetValueOrDefault());
                            else
                                top.LastPostHuman = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime((message.topic.LastModified + new TimeSpan(message.topic.Forum.LeagueOwner.TimeZone, 0, 0)).GetValueOrDefault());

                            top.TopicId = message.topic.TopicId;
                            top.GroupId = message.topic.GroupId;
                            top.TopicTitle = message.topic.TopicTitle;
                            top.TopicTitleForUrl = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.topic.TopicTitle);
                            top.ViewCount = message.topic.ViewCount;
                            top.Replies = message.topic.Messages.Where(x => x.IsRemoved == false).Count() - 1;
                            top.IsLocked = message.topic.IsLocked;
                            top.IsPinned = message.topic.IsSticky;
                            //is league manager
                            top.IsManagerOfTopic = isManager;
                            //if nt league manager and group id > 0
                            if (message.topic.GroupId > 0 && !isManager)
                                top.IsManagerOfTopic = isModerator;

                            if (message.topic.TopicsInbox.Where(x => x.ToUser.MemberId == memId).FirstOrDefault() == null)
                                top.IsRead = true;

                            if (!top.IsPinned)
                                topics.Add(top);
                            else
                                topics.Insert(0, top);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return topics;
        }

        public static List<ForumTopicJson> GetForumTopicsJson(string q, int limit, Guid forumId, long groupId)
        {
            List<ForumTopicJson> topics = new List<ForumTopicJson>();
            try
            {
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                var dc = new ManagementContext();

                var groups = MemberCache.GetGroupsApartOf(memId).ToList();

                var groupIds = groups.Select(x => x.Id).ToList();
                groupIds.Add(0);

                if (groupIds.Where(x => x == groupId).FirstOrDefault() != null)
                {
                    var db = (from xx in dc.ForumMessages
                              where xx.Topic.Forum.ForumId == forumId
                              where xx.Topic.GroupId == groupId
                              where xx.Topic.IsRemoved == false
                              where xx.Topic.TopicTitle.Contains(q) | xx.MessagePlain.Contains(q)
                              select new
                              {
                                  Created = xx.Created,
                                  topic = xx.Topic
                              }).AsParallel().ToList();

                    bool isManager = RDN.Library.Cache.MemberCache.IsManagerOrBetterOfLeague(memId);
                    bool isModerator = MemberCache.IsModeratorOrBetterOfLeagueGroup(memId, groupId);
                    foreach (var message in db)
                    {
                        if (topics.Where(x => x.TopicId == message.topic.TopicId).FirstOrDefault() == null)
                        {
                            ForumTopicJson top = new ForumTopicJson();
                            if (message.topic.Category != null)
                            {
                                top.Category = message.topic.Category.NameOfCategory;
                                top.CategoryId = message.topic.Category.CategoryId;
                            }
                            top.ForumId = forumId;
                            top.LastModified = (message.topic.LastModified + new TimeSpan(message.topic.Forum.LeagueOwner.TimeZone, 0, 0)).GetValueOrDefault();
                            top.Created = message.topic.Created + new TimeSpan(message.topic.Forum.LeagueOwner.TimeZone, 0, 0);
                            top.CreatedHuman = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(message.topic.Created + new TimeSpan(message.topic.Forum.LeagueOwner.TimeZone, 0, 0));
                            top.CreatedByMember = new MemberDisplayBasic();
                            top.CreatedByMember.DerbyName = message.topic.CreatedByMember.DerbyName;
                            top.CreatedByMember.DerbyNameUrl = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.topic.CreatedByMember.DerbyName);
                            top.CreatedByMember.MemberId = message.topic.CreatedByMember.MemberId;
                            top.LastPostByMember = new MemberDisplayBasic();
                            top.LastPostByMember.DerbyName = message.topic.LastPostByMember.DerbyName;
                            top.LastPostByMember.MemberId = message.topic.LastPostByMember.MemberId;
                            top.LastPostByMember.DerbyNameUrl = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.topic.LastPostByMember.DerbyName);
                            if (message.topic.LastPostDateTime != null)
                                top.LastPostHuman = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(message.topic.LastPostDateTime.GetValueOrDefault());
                            else
                                top.LastPostHuman = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(message.topic.LastModified.GetValueOrDefault());

                            top.TopicId = message.topic.TopicId;
                            top.GroupId = message.topic.GroupId;
                            top.TopicTitle = message.topic.TopicTitle;
                            top.TopicTitleForUrl = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.topic.TopicTitle);
                            top.ViewCount = message.topic.ViewCount;
                            top.Replies = message.topic.Messages.Where(x => x.IsRemoved == false).Count() - 1;
                            top.IsLocked = message.topic.IsLocked;
                            top.IsPinned = message.topic.IsSticky;
                            //is league manager
                            top.IsManagerOfTopic = isManager;
                            //if nt league manager and group id > 0
                            if (message.topic.GroupId > 0 && !isManager)
                                top.IsManagerOfTopic = isModerator;

                            if (message.topic.TopicsInbox.Where(x => x.ToUser.MemberId == memId).FirstOrDefault() == null)
                                top.IsRead = true;

                            if (!top.IsPinned)
                                topics.Add(top);
                            else
                                topics.Insert(0, top);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return topics;
        }

        public static Forum GetForumTopicsDeleted(Guid forumId, ForumOwnerTypeEnum ownerType, long groupId, int count, int page, bool isArchived)
        {
            Forum forum = new Forum();
            try
            {
                var dc = new ManagementContext();
                var groups = new List<LeagueGroup>();

                var db = (from xx in dc.Forums.Include("Topics").Include("Topics.Messages").Include("Topics.TopicsInbox")
                          where xx.ForumId == forumId
                          select new
                          {
                              Created = xx.Created,
                              ForumId = xx.ForumId,
                              ForumName = xx.ForumName,
                              LeagueId = xx.LeagueOwner,
                              FederationId = xx.FederationOwner,
                              Categories = xx.Categories.OrderBy(x => x.NameOfCategory),
                              TimeZone = xx.LeagueOwner.TimeZone
                          }).FirstOrDefault();
                forum.Created = db.Created + new TimeSpan(db.TimeZone, 0, 0);
                forum.ForumId = db.ForumId;
                forum.ForumName = db.ForumName;

                bool isManager = true;
                //need to add the default forum.
                groups.Insert(0, new LeagueGroup { Id = 0, GroupName = db.ForumName });

                if (groups.Where(x => x.Id == groupId).FirstOrDefault() == null)
                    return forum;
                if (ownerType == ForumOwnerTypeEnum.federation)
                    groups = groups.Where(x => x.Id == 0).ToList();

                foreach (var g in groups)
                {
                    ForumGroup gff = new ForumGroup();
                    gff.GroupId = g.Id;
                    gff.GroupName = g.GroupName;

                    //if nt league manager and group id > 0
                    if (gff.GroupId > 0 && !isManager)
                        isManager = true;
                    //this is making sure its the actual starting group.
                    if (g.Id == groupId)
                    {
                        var topics = dc.ForumTopics.Where(x => gff.GroupId == x.GroupId && x.IsRemoved == true && x.IsArchived == isArchived && x.Forum.ForumId == forumId).OrderByDescending(x => x.LastPostDateTime).Take(count).AsParallel();

                        foreach (var topic in topics)
                        {
                            if (gff.Topics.Where(x => x.TopicId == topic.TopicId).FirstOrDefault() == null)
                            {
                                ForumTopic top = DisplayForumTopic(new Guid(), isManager, topic);
                                if (!top.IsPinned)
                                    gff.Topics.Add(top);
                                else
                                    gff.Topics.Insert(0, top);
                            }
                        }
                        var topicsSticky = dc.ForumTopics.Where(x => gff.GroupId == x.GroupId && x.IsRemoved == true && x.IsArchived == isArchived && x.IsSticky == true && x.Forum.ForumId == forumId).OrderByDescending(x => x.LastPostDateTime).Take(count).AsParallel();

                        foreach (var topic in topicsSticky)
                        {
                            if (gff.Topics.Where(x => x.TopicId == topic.TopicId).FirstOrDefault() == null)
                            {
                                ForumTopic top = DisplayForumTopic(new Guid(), isManager, topic);
                                if (!top.IsPinned)
                                    gff.Topics.Add(top);
                                else
                                    gff.Topics.Insert(0, top);
                            }
                        }

                        var topicsCount = dc.ForumTopics.Where(x => gff.GroupId == x.GroupId && x.IsRemoved == true && x.IsArchived == isArchived && x.Forum.ForumId == forumId).AsParallel().Count();
                        gff.PageSize = count;
                        gff.CurrentPage = page;
                        gff.NumberOfRecords = topicsCount;
                        gff.NumberOfPages = (int)Math.Ceiling((double)topicsCount / count);
                        forum.CurrentGroup = gff;
                    }

                    gff.UnreadTopics = dc.ForumInbox.Where(x => x.Topic.GroupId == gff.GroupId && x.Topic.IsRemoved == true && x.Topic.IsArchived == isArchived && x.Topic.Forum.ForumId == forumId && x.ToUser.MemberId == new Guid()).Count();

                    forum.GroupTopics.Add(gff);
                }

                //add the categories.
                //adds the unread category if there are unread messages
                ForumCategory unreadCat = new ForumCategory();
                unreadCat.CategoryId = -1;
                unreadCat.CategoryName = "Unread";
                unreadCat.GroupId = 0;
                var gro = forum.GroupTopics.Where(x => x.GroupId == groupId).FirstOrDefault();
                if (gro != null)
                    unreadCat.UnreadTopics = gro.UnreadTopics;

                if (unreadCat.UnreadTopics > 0)
                    forum.Categories.Add(unreadCat);

                ForumCategory latestCat = new ForumCategory();
                latestCat.CategoryId = 0;
                latestCat.CategoryName = "Latest";
                latestCat.GroupId = groupId;
                if (gro != null)
                    latestCat.UnreadTopics = gro.UnreadTopics;

                var cats = db.Categories.Where(x => x.IsRemoved == true && x.GroupId == latestCat.GroupId).OrderBy(x => x.NameOfCategory);

                if (cats.Count() > 0 || unreadCat.UnreadTopics > 0)
                    forum.Categories.Add(latestCat);

                foreach (var cat in cats)
                {
                    ForumCategory c = new ForumCategory();
                    c.CategoryId = cat.CategoryId;
                    c.CategoryName = cat.NameOfCategory;
                    c.GroupId = cat.GroupId;
                    c.UnreadTopics = dc.ForumInbox.Where(x => x.Topic.GroupId == cat.GroupId && x.Topic.Forum.ForumId == forumId && x.ToUser.MemberId == new Guid() && x.Topic.Category.CategoryId == cat.CategoryId).Count();
                    forum.Categories.Add(c);
                }

                if (ownerType == ForumOwnerTypeEnum.federation)
                {
                    forum.FederationId = db.FederationId.FederationId;
                    forum.Type = ForumOwnerTypeEnum.federation;
                }
                else if (ownerType == ForumOwnerTypeEnum.league)
                {
                    forum.LeagueId = db.LeagueId.LeagueId;
                    forum.Type = ForumOwnerTypeEnum.league;
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return forum;
        }


        /// <summary>
        /// gets all the forum topics from the DB for the forum id.
        /// </summary>
        /// <param name="forumId"></param>
        public static Forum GetForumTopics(Guid memberId, Guid forumId, ForumOwnerTypeEnum ownerType, long groupId, int count, int page, bool isArchived)
        {
            Forum forum = new Forum();
            try
            {
                var dc = new ManagementContext();
                var groups = MemberCache.GetGroupsApartOf(memberId).ToList();

                var db = (from xx in dc.Forums.Include("Topics").Include("Topics.Messages").Include("Topics.TopicsInbox")
                          where xx.ForumId == forumId
                          select new
                          {
                              Created = xx.Created,
                              ForumId = xx.ForumId,
                              ForumName = xx.ForumName,
                              LeagueId = xx.LeagueOwner,
                              FederationId = xx.FederationOwner,
                              Categories = xx.Categories.OrderBy(x => x.NameOfCategory),
                              TimeZone = xx.LeagueOwner == null ? 0 : xx.LeagueOwner.TimeZone
                          }).FirstOrDefault();
                forum.Created = db.Created + new TimeSpan(db.TimeZone, 0, 0);
                forum.ForumId = db.ForumId;
                forum.ForumName = db.ForumName;

                bool isManager = RDN.Library.Cache.MemberCache.IsManagerOrBetterOfLeague(memberId);
                //need to add the default forum.
                groups.Insert(0, new LeagueGroup { Id = 0, GroupName = db.ForumName });

                if (groups.Where(x => x.Id == groupId).FirstOrDefault() == null)
                    return forum;
                if (ownerType == ForumOwnerTypeEnum.federation)
                    groups = groups.Where(x => x.Id == 0).ToList();

                foreach (var g in groups)
                {
                    ForumGroup gff = new ForumGroup();
                    gff.GroupId = g.Id;
                    gff.GroupName = g.GroupName;

                    //if nt league manager and group id > 0
                    if (gff.GroupId > 0 && !isManager)
                        isManager = MemberCache.IsModeratorOrBetterOfLeagueGroup(memberId, gff.GroupId);
                    //this is making sure its the actual starting group.
                    if (g.Id == groupId)
                    {
                        var topics = dc.ForumTopics.Include("Forum").Include("Forum.LeagueOwner").Include("CreatedByMember").Include("Messages").Include("TopicsInbox").Include("TopicsInbox.ToUser").Where(x => gff.GroupId == x.GroupId && x.IsRemoved == false && x.IsArchived == isArchived && x.Forum.ForumId == forumId).OrderByDescending(x => x.LastPostDateTime).Skip(page * count).Take(count).AsParallel().ToList();

                        foreach (var topic in topics)
                        {
                            try
                            {
                                if (gff.Topics.Where(x => x.TopicId == topic.TopicId).FirstOrDefault() == null)
                                {
                                    ForumTopic top = DisplayForumTopic(memberId, isManager, topic);
                                    if (!top.IsPinned)
                                        gff.Topics.Add(top);
                                    else
                                        gff.Topics.Insert(0, top);
                                }
                            }
                            catch (Exception exception)
                            {
                                ErrorDatabaseManager.AddException(exception, exception.GetType());
                            }
                        }
                        var topicsSticky = dc.ForumTopics.Where(x => gff.GroupId == x.GroupId && x.IsRemoved == false && x.IsArchived == isArchived && x.IsSticky == true && x.Forum.ForumId == forumId).OrderByDescending(x => x.LastPostDateTime).Skip(page * count).Take(count).AsParallel().ToList();

                        foreach (var topic in topicsSticky)
                        {
                            if (gff.Topics.Where(x => x.TopicId == topic.TopicId).FirstOrDefault() == null)
                            {
                                ForumTopic top = DisplayForumTopic(memberId, isManager, topic);
                                if (!top.IsPinned)
                                    gff.Topics.Add(top);
                                else
                                    gff.Topics.Insert(0, top);
                            }
                        }

                        var topicsCount = dc.ForumTopics.Where(x => gff.GroupId == x.GroupId && x.IsRemoved == false && x.IsArchived == isArchived && x.Forum.ForumId == forumId).AsParallel().Count();
                        gff.PageSize = count;
                        gff.CurrentPage = page;
                        gff.NumberOfRecords = topicsCount;
                        gff.NumberOfPages = (int)Math.Ceiling((double)topicsCount / count);
                        forum.CurrentGroup = gff;
                    }

                    gff.UnreadTopics = dc.ForumInbox.Where(x => x.Topic.GroupId == gff.GroupId && x.Topic.IsRemoved == false && x.Topic.IsArchived == isArchived && x.Topic.Forum.ForumId == forumId && x.ToUser.MemberId == memberId).Count();

                    forum.GroupTopics.Add(gff);
                }

                //add the categories.
                //adds the unread category if there are unread messages
                ForumCategory unreadCat = new ForumCategory();
                unreadCat.CategoryId = -1;
                unreadCat.CategoryName = "Unread";
                unreadCat.GroupId = 0;
                var gro = forum.GroupTopics.Where(x => x.GroupId == groupId).FirstOrDefault();
                if (gro != null)
                    unreadCat.UnreadTopics = gro.UnreadTopics;

                if (unreadCat.UnreadTopics > 0)
                    forum.Categories.Add(unreadCat);

                ForumCategory latestCat = new ForumCategory();
                latestCat.CategoryId = 0;
                latestCat.CategoryName = "Latest";
                latestCat.GroupId = groupId;
                if (gro != null)
                    latestCat.UnreadTopics = gro.UnreadTopics;

                var cats = db.Categories.Where(x => x.IsRemoved == false && x.GroupId == latestCat.GroupId).OrderBy(x => x.NameOfCategory);

                if (cats.Count() > 0 || unreadCat.UnreadTopics > 0)
                    forum.Categories.Add(latestCat);

                foreach (var cat in cats)
                {
                    ForumCategory c = new ForumCategory();
                    c.CategoryId = cat.CategoryId;
                    c.CategoryName = cat.NameOfCategory;
                    c.GroupId = cat.GroupId;
                    c.UnreadTopics = dc.ForumInbox.Where(x => x.Topic.GroupId == cat.GroupId && x.Topic.Forum.ForumId == forumId && x.ToUser.MemberId == memberId && x.Topic.Category.CategoryId == cat.CategoryId).Count();
                    forum.Categories.Add(c);
                }

                if (ownerType == ForumOwnerTypeEnum.federation)
                {
                    forum.FederationId = db.FederationId.FederationId;
                    forum.Type = ForumOwnerTypeEnum.federation;
                }
                else if (ownerType == ForumOwnerTypeEnum.league)
                {
                    forum.LeagueId = db.LeagueId.LeagueId;
                    forum.Type = ForumOwnerTypeEnum.league;
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return forum;
        }
        public static Forum GetForumTopicsForMain(Guid memberId, Guid forumId, long groupId, int count, int page, bool isArchived)
        {
            Forum forum = new Forum();
            try
            {
                var dc = new ManagementContext();
                
                var db = (from xx in dc.Forums.Include("Topics").Include("Topics.Messages").Include("Topics.TopicsInbox")
                          where xx.ForumId == forumId
                          select new
                          {
                              Created = xx.Created,
                              ForumId = xx.ForumId,
                              ForumName = xx.ForumName,
                              LeagueId = xx.LeagueOwner,
                              FederationId = xx.FederationOwner,
                              Categories = xx.Categories.OrderBy(x => x.NameOfCategory),
                              TimeZone = xx.LeagueOwner == null ? 0 : xx.LeagueOwner.TimeZone
                          }).FirstOrDefault();
                
                forum.Created = db.Created + new TimeSpan(db.TimeZone, 0, 0);
                forum.ForumId = db.ForumId;
                forum.ForumName = db.ForumName;

                bool isManager = RDN.Library.Cache.MemberCache.IsAdministrator(memberId);
                //need to add the default forum.
                List<LeagueGroup> groups = new List<LeagueGroup>();
                groups.Insert(0, new LeagueGroup { Id = 0, GroupName = LibraryConfig.WebsiteShortName });

                foreach (var g in groups)
                {
                    ForumGroup gff = new ForumGroup();
                    gff.GroupId = g.Id;
                    gff.GroupName = g.GroupName;

                    //if nt league manager and group id > 0
                    if (gff.GroupId > 0 && !isManager)
                        isManager = MemberCache.IsModeratorOrBetterOfLeagueGroup(memberId, gff.GroupId);
                    //this is making sure its the actual starting group.
                    if (g.Id == groupId)
                    {
                        var topics = dc.ForumTopics.Where(x => gff.GroupId == x.GroupId && x.IsRemoved == false && x.IsArchived == isArchived && x.Forum.ForumId == forumId).OrderByDescending(x => x.LastPostDateTime).Skip(page * count).Take(count).AsParallel().ToList();

                        foreach (var topic in topics)
                        {
                            if (gff.Topics.Where(x => x.TopicId == topic.TopicId).FirstOrDefault() == null)
                            {
                                ForumTopic top = DisplayForumTopic(memberId, isManager, topic);
                                if (!top.IsPinned)
                                    gff.Topics.Add(top);
                                else
                                    gff.Topics.Insert(0, top);
                            }
                        }
                        var topicsSticky = dc.ForumTopics.Where(x => gff.GroupId == x.GroupId && x.IsRemoved == false && x.IsArchived == isArchived && x.IsSticky == true && x.Forum.ForumId == forumId).OrderByDescending(x => x.LastPostDateTime).Skip(page * count).Take(count).AsParallel().ToList();

                        foreach (var topic in topicsSticky)
                        {
                            if (gff.Topics.Where(x => x.TopicId == topic.TopicId).FirstOrDefault() == null)
                            {
                                ForumTopic top = DisplayForumTopic(memberId, isManager, topic);
                                if (!top.IsPinned)
                                    gff.Topics.Add(top);
                                else
                                    gff.Topics.Insert(0, top);
                            }
                        }

                        var topicsCount = dc.ForumTopics.Where(x => gff.GroupId == x.GroupId && x.IsRemoved == false && x.IsArchived == isArchived && x.Forum.ForumId == forumId).AsParallel().Count();
                        gff.PageSize = count;
                        gff.CurrentPage = page;
                        gff.NumberOfRecords = topicsCount;
                        gff.NumberOfPages = (int)Math.Ceiling((double)topicsCount / count);
                        forum.CurrentGroup = gff;
                    }

                    gff.UnreadTopics = dc.ForumInbox.Where(x => x.Topic.GroupId == gff.GroupId && x.Topic.IsRemoved == false && x.Topic.IsArchived == isArchived && x.Topic.Forum.ForumId == forumId && x.ToUser.MemberId == memberId).Count();

                    forum.GroupTopics.Add(gff);
                }

                //add the categories.
                //adds the unread category if there are unread messages
                ForumCategory unreadCat = new ForumCategory();
                unreadCat.CategoryId = -1;
                unreadCat.CategoryName = "Unread";
                unreadCat.GroupId = 0;
                var gro = forum.GroupTopics.Where(x => x.GroupId == groupId).FirstOrDefault();
                if (gro != null)
                    unreadCat.UnreadTopics = gro.UnreadTopics;

                if (unreadCat.UnreadTopics > 0)
                    forum.Categories.Add(unreadCat);

                ForumCategory latestCat = new ForumCategory();
                latestCat.CategoryId = 0;
                latestCat.CategoryName = "Latest";
                latestCat.GroupId = groupId;
                if (gro != null)
                    latestCat.UnreadTopics = gro.UnreadTopics;

                var cats = db.Categories.Where(x => x.IsRemoved == false && x.GroupId == latestCat.GroupId).OrderBy(x => x.NameOfCategory);

                if (cats.Count() > 0 || unreadCat.UnreadTopics > 0)
                    forum.Categories.Add(latestCat);

                foreach (var cat in cats)
                {
                    ForumCategory c = new ForumCategory();
                    c.CategoryId = cat.CategoryId;
                    c.CategoryName = cat.NameOfCategory;
                    c.GroupId = cat.GroupId;
                    c.UnreadTopics = dc.ForumInbox.Where(x => x.Topic.GroupId == cat.GroupId && x.Topic.Forum.ForumId == forumId && x.ToUser.MemberId == memberId && x.Topic.Category.CategoryId == cat.CategoryId).Count();
                    forum.Categories.Add(c);
                }


                forum.FederationId = forumId;
                forum.Type = ForumOwnerTypeEnum.main;


            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return forum;
        }

        public static List<ForumCategory> GetCategoriesOfForum(Guid forumId, long groupId = 0)
        {
            Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
            var dc = new ManagementContext();
            return (from xx in dc.ForumCetegories
                    where xx.Forum.ForumId == forumId
                    where xx.GroupId == groupId
                    where xx.IsRemoved == false
                    select new ForumCategory
                    {
                        CategoryId = xx.CategoryId,
                        CategoryName = xx.NameOfCategory,
                        GroupId = xx.GroupId,
                        UnreadTopics = dc.ForumInbox.Where(x => x.Topic.GroupId == groupId && x.Topic.Forum.ForumId == forumId && x.ToUser.MemberId == memId && x.Topic.Category.CategoryId == xx.CategoryId).Count()
                    }).AsParallel().OrderBy(x => x.CategoryName).ToList();


        }

        private static ForumTopic DisplayForumTopic(Guid memId, bool isManager, DataModels.Forum.ForumTopic topic)
        {
            ForumTopic top = new ForumTopic();
            top.LastModified = (topic.LastModified + new TimeSpan(topic.Forum.LeagueOwner != null ? topic.Forum.LeagueOwner.TimeZone : 0, 0, 0)).GetValueOrDefault();
            top.LastModifiedHuman = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(top.LastModified + new TimeSpan(topic.Forum.LeagueOwner != null ? topic.Forum.LeagueOwner.TimeZone : 0, 0, 0));
            top.Created = topic.Created + new TimeSpan(topic.Forum.LeagueOwner != null ? topic.Forum.LeagueOwner.TimeZone : 0, 0, 0);
            top.IsArchived = topic.IsArchived;
            top.CreatedHuman = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(topic.Created + new TimeSpan(topic.Forum.LeagueOwner != null ? topic.Forum.LeagueOwner.TimeZone : 0, 0, 0));
            if (topic.LastPostDateTime == null)
                top.LastPostHuman = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(topic.LastModified.GetValueOrDefault());
            else
                top.LastPostHuman = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(topic.LastPostDateTime.GetValueOrDefault());
            if (topic.Category != null)
                top.Category = new ForumCategory { CategoryId = topic.Category.CategoryId, CategoryName = topic.Category.NameOfCategory };
            top.CreatedByMember = new MemberDisplay();
            if (topic.CreatedByMember != null)
            {
                top.CreatedByMember.DerbyName = topic.CreatedByMember.DerbyName;
                top.CreatedByMember.MemberId = topic.CreatedByMember.MemberId;
            }
            top.LastPostByMember = new MemberDisplay();
            top.LastPostByMember.DerbyName = topic.LastPostByMember.DerbyName;
            top.LastPostByMember.MemberId = topic.LastPostByMember.MemberId;

            top.TopicId = topic.TopicId;
            top.GroupId = topic.GroupId;
            top.TopicTitle = topic.TopicTitle;
            top.ViewCount = topic.ViewCount;
            top.Replies = topic.Messages.Count - 1;
            top.IsLocked = topic.IsLocked;
            top.IsPinned = topic.IsSticky;
            //is league manager
            top.IsManagerOfTopic = isManager;

            if (topic.TopicsInbox.Where(x => x.ToUser != null && x.ToUser.MemberId == memId).FirstOrDefault() == null)
                top.IsRead = true;
            return top;
        }
    }
}
