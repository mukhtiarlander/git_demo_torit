using RDN.Library.Cache;
using RDN.Library.Classes.Config;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Forum;
using RDN.Library.Util.Enum;
using RDN.Portable.Account;
using RDN.Portable.Classes.Controls.Forum;
using RDN.Portable.Classes.Forum.Enums;
using RDN.Portable.Classes.Url;
using RDN.Portable.Config;
using RDN.Portable.Network;
using RDN.Utilities.Strings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RDN.Api.Controllers.Controls
{
    public class ForumController : Controller
    {
        public ActionResult WatchTopic(string mid, string uid, string fid, string tid)
        {
            var mem = MemberCache.GetMemberDisplay(new Guid(mid));
            if (new Guid(uid) == mem.UserId)
            {
                RDN.Library.Classes.Forum.Forum.WatchTopicToggle(new Guid(fid), Convert.ToInt64(tid), mem.MemberId);
                ForumTopicCache.ClearTopicCache(new Guid(fid), Convert.ToInt64(tid));
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(LibraryConfig.InternalSite+ UrlManager.URL_TO_CLEAR_FORUM_TOPIC + "forumId=" + fid + "&topicId=" + tid));
            }
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ClearForumTopicCache(string forumId, string topicId)
        {
            RDN.Library.Cache.ForumTopicCache.ClearTopicCache(new Guid(forumId), Convert.ToInt64(topicId));
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public ActionResult ReplyTopic()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<AddForumTopicModel>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(new Guid(ob.MemberId));
                    if (new Guid(ob.UserId) == mem.UserId)
                    {
                        Forum.ReplyToPost(ob.ForumId, ob.TopicId, ob.Text, new Guid(ob.MemberId), ob.BroadcastMessage);
                        ForumTopicCache.ClearTopicCache(ob.ForumId, ob.TopicId);
                        WebClient client = new WebClient();
                        client.DownloadStringAsync(new Uri(LibraryConfig.InternalSite  + UrlManager.URL_TO_CLEAR_FORUM_TOPIC + "forumId=" + ob.ForumId + "&topicId=" + ob.TopicId));

                        return Json(new AddForumTopicModel() { IsSuccessful = true }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());

            }
            return Json(new AddForumTopicModel() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public ActionResult AddTopic()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<AddForumTopicModel>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(new Guid(ob.MemberId));
                    if (new Guid(ob.UserId) == mem.UserId)
                    {
                        var topicId = Forum.CreateNewForumTopicAndPost(ob.ForumId, (ForumOwnerTypeEnum)Enum.Parse(typeof(ForumOwnerTypeEnum), ob.ForumType), ob.TopicTitle, ob.Text, new Guid(ob.MemberId), ob.GroupId, ob.BroadcastMessage, ob.PinMessage, ob.LockMessage, ob.CategoryId, new List<Guid>());
                        if (topicId > 0)
                            return Json(new AddForumTopicModel() { IsSuccessful = true }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());

            }
            return Json(new AddForumTopicModel() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }


        [ValidateInput(false)]
        public JsonResult Posts(string mid, string uid, string t, string gid, string cid, int p, int c)
        {
            ForumModel model = new ForumModel();
            try
            {
                var mem = MemberCache.GetMemberDisplay(new Guid(mid));
                if (new Guid(uid) == mem.UserId)
                {

                    long gId = 0;
                    if (!String.IsNullOrEmpty(gid))
                        gId = Convert.ToInt64(gid);

                    Guid forumId = MemberCache.GetForumIdForMemberLeague(new Guid(mid));
                    var topics = Forum.GetForumTopics(new Guid(mid), forumId, (ForumOwnerTypeEnum)Enum.Parse(typeof(ForumOwnerTypeEnum), t), gId, c, p, false);

                    model.ForumId = forumId;
                    model.GroupId = gId;

                    for (int i = 0; i < topics.GroupTopics.Count; i++)
                    {
                        ForumGroupModel group = new ForumGroupModel();
                        group.GroupId = topics.GroupTopics[i].GroupId;
                        group.GroupName = topics.GroupTopics[i].GroupName;
                        group.Categories = topics.Categories;

                        for (int j = 0; j < topics.GroupTopics[i].Topics.Count; j++)
                        {
                            ForumTopicModel topic = new ForumTopicModel();
                            if (topics.GroupTopics[i].Topics[j].Category != null)
                            {
                                topic.Category = topics.GroupTopics[i].Topics[j].Category.CategoryName;
                                topic.CategoryId = topics.GroupTopics[i].Topics[j].Category.CategoryId;
                            }
                            if (topics.GroupTopics[i].Topics[j].LastPostByMember != null)
                            {
                                topic.LastPostById = topics.GroupTopics[i].Topics[j].LastPostByMember.MemberId;
                                topic.LastPostByName = topics.GroupTopics[i].Topics[j].LastPostByMember.DerbyName;
                            }
                            topic.PostCount = topics.GroupTopics[i].Topics[j].Replies;
                            if (topics.GroupTopics[i].Topics[j].CreatedByMember != null)
                            {
                                topic.StartedById = topics.GroupTopics[i].Topics[j].CreatedByMember.MemberId;
                                topic.StartedByName = topics.GroupTopics[i].Topics[j].CreatedByMember.DerbyName;
                            }
                            topic.StartedRelativeTime = topics.GroupTopics[i].Topics[j].CreatedHuman;
                            topic.TopicId = topics.GroupTopics[i].Topics[j].TopicId;
                            topic.ForumId = forumId;
                            topic.HasRead = topics.GroupTopics[i].Topics[j].IsRead;
                            topic.IsLocked = topics.GroupTopics[i].Topics[j].IsLocked;
                            topic.IsManagerOfTopic = topics.GroupTopics[i].Topics[j].IsManagerOfTopic;
                            topic.IsPinned = topics.GroupTopics[i].Topics[j].IsPinned;
                            topic.IsWatching = topics.GroupTopics[i].Topics[j].IsWatching;
                            topic.TopicName = topics.GroupTopics[i].Topics[j].TopicTitle;
                            topic.ViewCount = topics.GroupTopics[i].Topics[j].ViewCount;
                            topic.LastPostRelativeTime = topics.GroupTopics[i].Topics[j].LastModifiedHuman;
                            group.Topics.Add(topic);
                        }
                        group.UnreadTopicsCount = group.Topics.Where(x => x.HasRead == false).Count();
                        model.Groups.Add(group);
                    }


                    if (!String.IsNullOrEmpty(cid))
                        model.CategoryId = Convert.ToInt64(cid);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Categories(string mid, string uid, string gid)
        {
            List<ForumCategory> model = new List<ForumCategory>();
            try
            {
                var mem = MemberCache.GetMemberDisplay(new Guid(mid));
                if (new Guid(uid) == mem.UserId)
                {
                    long tId = 0;
                    if (!String.IsNullOrEmpty(gid))
                        tId = Convert.ToInt64(gid);

                    Guid forumId = MemberCache.GetForumIdForMemberLeague(new Guid(mid));
                    model = Forum.GetCategoriesOfForum(forumId, tId);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Json(model, "application/json", JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public JsonResult Topic(string mid, string uid, string tid)
        {
            ForumTopicModel model = new ForumTopicModel();
            try
            {
                var mem = MemberCache.GetMemberDisplay(new Guid(mid));
                if (new Guid(uid) == mem.UserId)
                {
                    long tId = 0;
                    if (!String.IsNullOrEmpty(tid))
                        tId = Convert.ToInt64(tid);

                    Guid forumId = MemberCache.GetForumIdForMemberLeague(new Guid(mid));
                    Forum.UpdatePostViewCount(forumId, tId, new Guid(mid));
                    Forum.MarkAsRead(forumId, tId, new Guid(mid));


                    var topic = ForumTopicCache.GetTopic(forumId, tId);

                    var isWatching = topic.Watchers.Where(x => x.MemberId == mem.MemberId).FirstOrDefault();
                    if (isWatching != null)
                        model.IsWatching = true;
                    else
                        model.IsWatching = false;

                    if (topic.Category != null)
                    {
                        model.Category = topic.Category.CategoryName;
                        model.CategoryId = topic.Category.CategoryId;
                    }
                    if (topic.LastPostByMember != null)
                    {
                        model.LastPostById = topic.LastPostByMember.MemberId;
                        model.LastPostByName = topic.LastPostByMember.DerbyName;
                    }
                    model.LastPostRelativeTime = topic.LastModifiedHuman;
                    model.PostCount = topic.Replies;
                    if (topic.CreatedByMember != null)
                    {
                        model.StartedById = topic.CreatedByMember.MemberId;
                        model.StartedByName = topic.CreatedByMember.DerbyName;
                    }
                    model.StartedRelativeTime = topic.CreatedHuman;
                    model.TopicId = topic.TopicId;
                    model.TopicName = topic.TopicTitle;
                    model.ViewCount = topic.ViewCount;
                    model.IsLocked = topic.IsLocked;
                    model.IsPinned = topic.IsPinned;
                    model.HasRead = topic.IsRead;
                    model.IsManagerOfTopic = topic.IsManagerOfTopic;
                    model.TopicInbox = topic.TopicInbox;
                    model.ForumId = forumId;

                    for (int i = 0; i < topic.Messages.Count; i++)
                    {
                        ForumPostModel post = new ForumPostModel();
                        post.DatePosted = topic.Messages[i].Created;
                        if (topic.Messages[i].Member != null)
                        {
                            post.PostedById = topic.Messages[i].Member.MemberId;
                            post.PostedByName = topic.Messages[i].Member.DerbyName;
                            if (topic.Messages[i].Member.Photos.FirstOrDefault() != null)
                                post.PostedByPictureUrl = topic.Messages[i].Member.Photos.FirstOrDefault().ImageThumbUrl;
                        }
                        post.Text = topic.Messages[i].MessagePlain;
                        post.DatePostedByHuman = topic.Messages[i].CreatedHumanRelative;
                        model.Posts.Add(post);
                    }

                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Json(model, "application/json", JsonRequestBehavior.AllowGet);
        }

    }
}
