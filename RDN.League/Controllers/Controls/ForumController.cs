using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Forum;
using RDN.Library.Cache;
using RDN.League.Models.Filters;
using RDN.Library.Classes.Error;
using RDN.League.Models.Enum;
using RDN.Library.Util.Enum;
using RDN.Utilities.Strings;
using RDN.Library.Classes.Forum.Json;
using RDN.League.Models.Forum;
using System.Web.Script.Serialization;
using RDN.Library.Classes.Document;
using System.IO;
using System.Collections.Specialized;
using RDN.Library.Util;
using System.Net;
using RDN.Portable.Config;
using RDN.Portable.Classes.Forum.Enums;
using RDN.Portable.Classes.Controls.Forum;
using RDN.Portable.Classes.League.Classes;
using RDN.Library.Classes.League.Classes;
using RDN.Library.Classes.Config;
using RDN.Portable.Classes.Url;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class ForumController : BaseController
    {
        public ActionResult MarkAsRead(string forumId, string topicId)
        {
            RDN.Library.Classes.Forum.Forum.MarkAsRead(new Guid(forumId), Convert.ToInt64(topicId), RDN.Library.Classes.Account.User.GetMemberId());
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult WatchTopic(string forumId, string topicId)
        {
            var memberId = RDN.Library.Classes.Account.User.GetMemberId();
            RDN.Library.Classes.Forum.Forum.WatchTopicToggle(new Guid(forumId), Convert.ToInt64(topicId), memberId);
            ForumTopicCache.ClearTopicCache(new Guid(forumId), Convert.ToInt64(topicId));
            WebClient client = new WebClient();
            client.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_FORUM_TOPIC_API + "forumId=" + forumId + "&topicId=" + topicId));
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult UpdateTopicName(string fId, string tId, string n)
        {
            try
            {
                var memberId = RDN.Library.Classes.Account.User.GetMemberId();
                if (new Guid(fId) == MemberCache.GetForumIdForMemberLeague(memberId))
                {
                    var success = Forum.UpdateTopicName(new Guid(fId), Convert.ToInt64(tId), n);
                    ForumTopicCache.ClearTopicCache(new Guid(fId), Convert.ToInt64(tId));
                    WebClient client = new WebClient();
                    client.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_FORUM_TOPIC_API + "forumId=" + fId + "&topicId=" + tId));
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
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult AddCategoryToForum(string forumId, string categoryName, string groupId)
        {
            try
            {
                var memberId = RDN.Library.Classes.Account.User.GetMemberId();
                var success = Forum.AddCategoryToForum(new Guid(forumId), categoryName, Convert.ToInt64(groupId), memberId);
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = "false" }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult UpdateCategoryToForum(string forumId, string catagoryId, string categoryName, string groupId)
        {
            try
            {
                var memberId = RDN.Library.Classes.Account.User.GetMemberId();
                var success = Forum.UpdateCatagoryToForum(new Guid(forumId), Convert.ToInt64(catagoryId), categoryName, Convert.ToInt64(groupId));
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = "false" }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult DeleteCategoryToForum(string forumId, string catagoryId)
        {
            try
            {
                var memberId = RDN.Library.Classes.Account.User.GetMemberId();
                var success = Forum.DeleteCatagoryToForum(new Guid(forumId), Convert.ToInt64(catagoryId));
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = "false" }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult BroadcastMessageChange(string forumId, string groupId, string change)
        {
            try
            {
                var memberId = RDN.Library.Classes.Account.User.GetMemberId();
                var success = Forum.BroadcastMessageChange(new Guid(forumId), Convert.ToInt64(groupId), Convert.ToBoolean(change));
                MemberCache.Clear(memberId);
                MemberCache.ClearApiCache(memberId);
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = "false" }, JsonRequestBehavior.AllowGet);
        }

        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult ForumSettings(string forumId, string groupId)
        {
            try
            {
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfForum(memId, new Guid(forumId)))
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                var gId = Convert.ToInt64(groupId);
                if (gId > 0)
                    if (!MemberCache.IsModeratorOrBetterOfLeagueGroup(memId, gId))
                        return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                var settings = Forum.GetForumSettings(new Guid(forumId), gId);

                return View(settings);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [Authorize]
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult ReplyMessage(Models.Forum.NewPost post)
        {
            try
            {
                var memberId = RDN.Library.Classes.Account.User.GetMemberId();

                List<Guid> guids = new List<Guid>();
                if (!String.IsNullOrEmpty(post.Mentions))
                {
                    string[] ids = post.Mentions.Split(',');
                    for (int i = 0; i < ids.Count(); i++)
                    {
                        if (ids[i].ToString().Trim() != "")
                            guids.Add(new Guid(ids[i].ToString().Trim()));
                    }
                }

                Forum.ReplyToPost(post.ForumId, post.TopicId, post.Message, memberId, post.BroadcastMessage, guids);
                ForumTopicCache.ClearTopicCache(post.ForumId, post.TopicId);
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_FORUM_TOPIC_API + "forumId=" + post.ForumId + "&topicId=" + post.TopicId));

                return Redirect(Url.Content("~/forum/post/view/" + post.ForumId.ToString().Replace("-", "") + "/" + post.TopicId));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult ReplyMessage(string forumId, string topicId)
        {
            try
            {
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();

                if (!MemberCache.IsMemberApartOfForum(memId, new Guid(forumId)) && new Guid(forumId) != LibraryConfig.DEFAULT_RDN_FORUM_ID)
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                var league = MemberCache.GetLeagueOfMember(memId);
                if (league != null)
                    SetCulture(league.CultureSelected);


                Models.Forum.NewPost post = new Models.Forum.NewPost();
                var topic = ForumTopicCache.GetTopic(new Guid(forumId), Convert.ToInt64(topicId));
                var isWatching = topic.Watchers.Where(x => x.MemberId == memId).FirstOrDefault();
                if (isWatching != null)
                    topic.IsWatching = true;
                else
                    topic.IsWatching = false;

                RDN.Portable.Classes.Account.Classes.MemberDisplay setting = MemberCache.GetMemberDisplay(memId);
                if (setting.Settings.ForumDescending)
                    post.Messages = topic.Messages.OrderByDescending(x => x.Created).ToList();
                post.ForumId = topic.ForumId;
                post.ForumType = topic.ForumType;
                post.TopicId = topic.TopicId;
                post.GroupId = topic.GroupId;
                post.TopicTitle = topic.TopicTitle;
                post.BroadcastMessage = topic.BroadcastForumTopic;
                return View(post);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult EditPost(string forumId, string messageId)
        {
            try
            {
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();

                if (!MemberCache.IsMemberApartOfForum(memId, new Guid(forumId)) && !MemberCache.IsAdministrator(memId))
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                Models.Forum.NewPost post = new Models.Forum.NewPost();
                var topic = Forum.GetForumMessage(new Guid(forumId), Convert.ToInt64(messageId));
                //not the owner of the message
                if (topic.Member.MemberId != RDN.Library.Classes.Account.User.GetMemberId())
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

                post.Message = topic.MessageHTML;
                post.MessageMarkDown = topic.MessageMarkDown;
                post.TopicId = topic.TopicId;
                post.ForumId = topic.ForumId;
                post.ForumType = ForumOwnerTypeEnum.league;
                return View(post);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult QuotePost(string forumId, string topicId, string messageId)
        {
            try
            {
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();

                if (!MemberCache.IsMemberApartOfForum(memId, new Guid(forumId)) && !MemberCache.IsAdministrator(memId))
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                var league = MemberCache.GetLeagueOfMember(memId);
                if (league != null)
                    SetCulture(league.CultureSelected);


                Models.Forum.NewPost post = new Models.Forum.NewPost();
                var topic = ForumTopicCache.GetTopic(new Guid(forumId), Convert.ToInt64(topicId));
                var isWatching = topic.Watchers.Where(x => x.MemberId == memId).FirstOrDefault();
                if (isWatching != null)
                    topic.IsWatching = true;
                else
                    topic.IsWatching = false;


                var message = topic.Messages.Where(x => x.MessageId == Convert.ToInt64(messageId)).FirstOrDefault();

                post.Messages = topic.Messages.OrderByDescending(x => x.Created).ToList();

                string s = "[quote]";
                string y = "[/quote]";
                post.Message = s + message.MessagePlain + y;
                post.MessageMarkDown = s + message.MessageMarkDown + y;
                post.TopicId = topic.TopicId;
                post.ForumId = topic.ForumId;
                post.GroupId = topic.GroupId;
                post.ForumType = ForumOwnerTypeEnum.league;
                return View(post);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult QuotePost(NewPost newPost)
        {
            try
            {
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();

                if (!MemberCache.IsMemberApartOfForum(memId, newPost.ForumId) && !MemberCache.IsAdministrator(memId))
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                var topic = Forum.GetForumMessage(newPost.ForumId, newPost.MessageId);


                if (newPost.Message.Contains("[quote]") && newPost.Message.Contains("[/quote]"))
                {
                    string headQuote = "<div class='quoteForumMessage'><div class='topslice_quote'>";
                    headQuote += "<span>Quote from: " + topic.Member.DerbyName + " on " + topic.Created.ToString() + "</span>";
                    headQuote += "</div><blockquote class='bbc_standard_quote'>";
                    newPost.Message = newPost.Message.Replace("[quote]", headQuote);
                    string footQuote = "</blockquote><div class='quotefooter'><div class='botslice_quote'></div></div></div>";
                    newPost.Message = newPost.Message.Replace("[/quote]", footQuote);
                }

                List<Guid> mentions = new List<Guid>();
                if (!String.IsNullOrEmpty(newPost.Mentions))
                {
                    string[] ids = newPost.Mentions.Split(',');
                    for (int i = 0; i < ids.Count(); i++)
                    {
                        if (ids[i].ToString().Trim() != "")
                            mentions.Add(new Guid(ids[i].ToString().Trim()));
                    }
                }

                Forum.ReplyToPost(newPost.ForumId, newPost.TopicId, newPost.Message, memId, newPost.BroadcastMessage, mentions);
                ForumTopicCache.ClearTopicCache(newPost.ForumId, newPost.TopicId);
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_FORUM_TOPIC_API + "forumId=" + newPost.ForumId + "&topicId=" + newPost.TopicId));
                return Redirect(Url.Content("~/forum/post/view/" + newPost.ForumId.ToString().Replace("-", "") + "/" + newPost.TopicId));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [Authorize]
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult MovePost(Models.Forum.MovePost post)
        {
            try
            {
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();

                if (!MemberCache.IsMemberApartOfForum(memId, post.ForumId) && !MemberCache.IsAdministrator(memId))
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));


                bool topic = Forum.UpdateTopicCategoryAndGroup(post.ForumId, post.TopicId, post.ChosenForum, post.ChosenCategory);
                ForumTopicCache.ClearTopicCache(post.ForumId, post.TopicId);
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_FORUM_TOPIC_API + "forumId=" + post.ForumId + "&topicId=" + post.TopicId));
                if (topic == true)
                    return Redirect(Url.Content("~/forum/posts/" + post.ForumType.ToString() + "/" + post.ForumId.ToString().Replace("-", "") + "/" + post.ChosenForum + "?u=" + SiteMessagesEnum.s));

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult MovePost(string forumId, string messageId)
        {
            try
            {
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();

                if (!MemberCache.IsMemberApartOfForum(memId, new Guid(forumId)) && !MemberCache.IsAdministrator(memId))
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                Models.Forum.MovePost post = new Models.Forum.MovePost();

                var topic = ForumTopicCache.GetTopic(new Guid(forumId), Convert.ToInt64(messageId));
                var isWatching = topic.Watchers.Where(x => x.MemberId == memId).FirstOrDefault();
                if (isWatching != null)
                    topic.IsWatching = true;
                else
                    topic.IsWatching = false;

                var groups = LeagueGroupFactory.GetGroups(MemberCache.GetLeagueIdOfMember(memId));
                groups.Insert(0, new LeagueGroup() { GroupName = "Forum", Id = 0 });
                if (topic.GroupId > 0)
                {
                    post.MoveToForums = new SelectList(groups, "Id", "GroupName", topic.GroupId);
                    post.ChosenForum = topic.GroupId;
                }
                else
                    post.MoveToForums = new SelectList(groups, "Id", "GroupName");

                //not the owner of the message
                List<ForumCategory> categories = Forum.GetCategoriesOfForum(new Guid(forumId), topic.GroupId);
                categories.Insert(0, new ForumCategory() { CategoryId = 0, CategoryName = "None" });
                if (topic.Category != null && topic.Category.CategoryId > 0)
                {
                    post.ForumCategories = new SelectList(categories, "CategoryId", "CategoryName", topic.Category.CategoryId);
                    post.ChosenCategory = topic.Category.CategoryId;
                }
                else
                    post.ForumCategories = new SelectList(categories, "CategoryId", "CategoryName");


                post.TopicId = topic.TopicId;
                post.ForumId = topic.ForumId;
                post.TopicTitle = topic.TopicTitle;
                post.ForumType = topic.ForumType.ToString();
                return View(post);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [Authorize]
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult EditPost(Models.Forum.NewPost post)
        {
            try
            {
                var memberId = RDN.Library.Classes.Account.User.GetMemberId();
                Forum.EditPost(post.ForumId, post.MessageId, post.Message);
                ForumTopicCache.ClearTopicCache(post.ForumId, post.TopicId);
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_FORUM_TOPIC_API + "forumId=" + post.ForumId + "&topicId=" + post.TopicId));
                return Redirect(Url.Content("~/forum/post/view/" + post.ForumId.ToString().Replace("-", "") + "/" + post.TopicId));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult Post(string forumId, string topicId, string title)
        {
            try
            {
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();

                if (!MemberCache.IsMemberApartOfForum(memId, new Guid(forumId)) && new Guid(forumId) != LibraryConfig.DEFAULT_RDN_FORUM_ID)
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                var league = MemberCache.GetLeagueOfMember(memId);
                if (league != null)
                    SetCulture(league.CultureSelected);

                var topic = ForumTopicCache.GetTopic(new Guid(forumId), Convert.ToInt64(topicId));

                var display = MemberCache.GetMemberDisplay(memId);

                if (display.Settings != null && display.Settings.ForumDescending)
                    topic.Messages = topic.Messages.OrderByDescending(o => o.Created).ToList();

                var isWatching = topic.Watchers.Where(x => x.MemberId == memId).FirstOrDefault();
                if (isWatching != null)
                    topic.IsWatching = true;
                else
                    topic.IsWatching = false;

                if (topic == null)
                    return Redirect(Url.Content("~/forum/posts/league/" + forumId + "?u=" + SiteMessagesEnum.de));
                ///forum/posts/league/928750a7fb11474d904d29f5ff66b8f6
                topic.CurrentMemberId = memId;
                if (new Guid(forumId) != LibraryConfig.DEFAULT_RDN_FORUM_ID)
                {
                    topic.IsManagerOfTopic = RDN.Library.Cache.MemberCache.IsManagerOrBetterOfLeague(topic.CurrentMemberId);
                    if (!topic.IsManagerOfTopic)
                        topic.IsManagerOfTopic = RDN.Library.Cache.MemberCache.IsModeratorOrBetterOfLeagueGroup(topic.CurrentMemberId, topic.GroupId);
                }
                else
                    topic.IsManagerOfTopic = MemberCache.IsAdministrator(memId);
                return View(topic);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult NewPost(Models.Forum.NewPost post)
        {
            try
            {
                var memberId = RDN.Library.Classes.Account.User.GetMemberId();

                List<Guid> guids = new List<Guid>();
                if (!String.IsNullOrEmpty(post.Mentions))
                {
                    string[] ids = post.Mentions.Split(',');
                    for (int i = 0; i < ids.Count(); i++)
                    {
                        if (ids[i].ToString().Trim() != "")
                            guids.Add(new Guid(ids[i].ToString().Trim()));
                    }
                }
                var topicId = Forum.CreateNewForumTopicAndPost(post.ForumId, post.ForumType, post.Subject, post.Message, memberId, post.GroupId, post.BroadcastMessage, post.PinMessage, post.LockMessage, post.ChosenCategory, guids);
                if (topicId > 0)
                    return Redirect(Url.Content("~/forum/post/view/" + post.ForumId.ToString().Replace("-", "") + "/" + topicId));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult RemovePost(string forumId, string topicId, string postId)
        {
            try
            {
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfForum(memId, new Guid(forumId)) && new Guid(forumId) != LibraryConfig.DEFAULT_RDN_FORUM_ID)
                    return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);

                bool success = Forum.RemovePost(new Guid(forumId), Convert.ToInt64(topicId), Convert.ToInt64(postId));
                ForumTopicCache.ClearTopicCache(new Guid(forumId), Convert.ToInt64(topicId));
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_FORUM_TOPIC_API + "forumId=" + forumId + "&topicId=" + topicId));
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = "false" }, JsonRequestBehavior.AllowGet);
        }
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult RemoveTopic(string forumId, string topicId, string forumType)
        {
            try
            {
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                ForumOwnerTypeEnum type = (ForumOwnerTypeEnum)Enum.Parse(typeof(ForumOwnerTypeEnum), forumType);
                if (type != ForumOwnerTypeEnum.main)
                    if (!MemberCache.IsMemberApartOfForum(memId, new Guid(forumId)))
                        return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
                bool success = Forum.RemoveTopic(new Guid(forumId), Convert.ToInt64(topicId));
                ForumTopicCache.ClearTopicCache(new Guid(forumId), Convert.ToInt64(topicId));
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_FORUM_TOPIC_API + "forumId=" + forumId + "&topicId=" + topicId));
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = "false" }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult PinForumTopic(string forumId, string topicId, string pin)
        {
            try
            {
                bool success = Forum.PinTopic(new Guid(forumId), Convert.ToInt64(topicId), Convert.ToBoolean(pin));
                ForumTopicCache.ClearTopicCache(new Guid(forumId), Convert.ToInt64(topicId));
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_FORUM_TOPIC_API + "forumId=" + forumId + "&topicId=" + topicId));
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = "false" }, JsonRequestBehavior.AllowGet);
        }

        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult LockForumTopic(string forumId, string topicId, string lockTopic)
        {
            try
            {
                bool success = Forum.LockTopic(new Guid(forumId), Convert.ToInt64(topicId), Convert.ToBoolean(lockTopic));
                ForumTopicCache.ClearTopicCache(new Guid(forumId), Convert.ToInt64(topicId));
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_FORUM_TOPIC_API + "forumId=" + forumId + "&topicId=" + topicId));
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = "false" }, JsonRequestBehavior.AllowGet);
        }
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult ArchiveForumTopic(string forumId, string topicId, string lockTopic)
        {
            try
            {
                bool success = Forum.ArchiveTopic(new Guid(forumId), Convert.ToInt64(topicId), Convert.ToBoolean(lockTopic));
                ForumTopicCache.ClearTopicCache(new Guid(forumId), Convert.ToInt64(topicId));
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_FORUM_TOPIC_API + "forumId=" + forumId + "&topicId=" + topicId));
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = "false" }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult NewPost(string type, string id, string groupId)
        {
            try
            {
                var ownerType = (ForumOwnerTypeEnum)Enum.Parse(typeof(ForumOwnerTypeEnum), type);

                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (ownerType != ForumOwnerTypeEnum.main)
                    if (!MemberCache.IsMemberApartOfForum(memId, new Guid(id)))
                        return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                Models.Forum.NewPost post = new Models.Forum.NewPost();

                post.ForumId = new Guid(id);
                post.ForumType = ownerType;

                if (ownerType != ForumOwnerTypeEnum.main)
                    post.IsManagerOrBetter = MemberCache.IsSecretaryOrBetterOfLeague(memId);
                if (!String.IsNullOrEmpty(groupId))
                {
                    post.GroupId = Convert.ToInt64(groupId);
                    var groups = MemberCache.GetGroupsApartOf(memId);
                    var g = groups.Where(x => x.Id == post.GroupId).FirstOrDefault();
                    if (g != null)
                        post.GroupName = g.GroupName;
                    else
                        post.GroupName = "Forum";
                    post.ForumCategories = new SelectList(Forum.GetCategoriesOfForum(new Guid(id), post.GroupId), "CategoryId", "CategoryName");
                    if (!post.IsManagerOrBetter)
                        post.IsManagerOrBetter = MemberCache.IsModeratorOrBetterOfLeagueGroup(memId, post.GroupId);
                }
                else
                {
                    post.ForumCategories = new SelectList(Forum.GetCategoriesOfForum(new Guid(id), post.GroupId), "CategoryId", "CategoryName");
                    post.GroupName = "Forum";
                }
                var settings = Forum.GetForumSettings(post.ForumId, post.GroupId);
                post.BroadcastMessage = settings.BroadCastPostsDefault;
                return View(post);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult Index()
        {
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        /// <summary>
        /// all the posts that have been deleted from the forum.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="groupid"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult DeletedPosts(string type, string id, string groupid, string categoryId)
        {
            try
            {
                if (new Guid(id) != new Guid())
                {
                    NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                    string updated = nameValueCollection["u"];

                    if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.de.ToString())
                    {
                        SiteMessage message = new SiteMessage();
                        message.MessageType = SiteMessageType.Info;
                        message.Message = "That forum post doesn't exist.  It might have been deleted.";
                        this.AddMessage(message);
                    }

                    Guid memId = RDN.Library.Classes.Account.User.GetMemberId();

                    if (!MemberCache.IsMemberApartOfForum(memId, new Guid(id)))
                        return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                    long gId = 0;
                    if (!String.IsNullOrEmpty(groupid))
                        gId = Convert.ToInt64(groupid);

                    var topics = Forum.GetForumTopicsDeleted(new Guid(id), (ForumOwnerTypeEnum)Enum.Parse(typeof(ForumOwnerTypeEnum), type), gId, 100, Forum.DEFAULT_PAGE_SIZE, false);
                    topics.CurrentMemberId = memId;
                    if (topics.CurrentGroup == null)
                    {
                        topics.CurrentGroup = new ForumGroup();
                        topics.CurrentGroup.GroupId = gId;
                    }

                    if (!String.IsNullOrEmpty(categoryId))
                        topics.CategoryId = Convert.ToInt64(categoryId);
                    return View(topics);
                }
                Forum forum = new Forum();
                forum.ForumId = new Guid();
                return View(forum);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult Posts(string type, string id, string groupid, string categoryId)
        {
            try
            {
                ForumOwnerTypeEnum ownerType = (ForumOwnerTypeEnum)Enum.Parse(typeof(ForumOwnerTypeEnum), type);
                if (new Guid(id) != new Guid())
                {
                    NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                    string updated = nameValueCollection["u"];
                    string pageTemp = nameValueCollection["p"];
                    if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.de.ToString())
                    {
                        SiteMessage message = new SiteMessage();
                        message.MessageType = SiteMessageType.Info;
                        message.Message = "That forum post doesn't exist.  It might have been deleted.";
                        this.AddMessage(message);
                    }
                    int page = 0;
                    if (!String.IsNullOrEmpty(pageTemp))
                    {
                        Int32.TryParse(pageTemp, out page);

                    }

                    Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                    Forum topics = new Forum();

                    if (ownerType != ForumOwnerTypeEnum.main)
                        if (!MemberCache.IsMemberApartOfForum(memId, new Guid(id)))
                            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                    long gId = 0;
                    if (!String.IsNullOrEmpty(groupid))
                        gId = Convert.ToInt64(groupid);
                    if (ownerType == ForumOwnerTypeEnum.main)
                        topics = Forum.GetForumTopicsForMain(memId, LibraryConfig.DEFAULT_RDN_FORUM_ID, gId, Forum.DEFAULT_PAGE_SIZE, page, false);
                    else
                        topics = Forum.GetForumTopics(memId, new Guid(id), ownerType, gId, Forum.DEFAULT_PAGE_SIZE, page, false);



                    topics.CurrentMemberId = memId;
                    if (topics.CurrentGroup == null)
                    {
                        topics.CurrentGroup = new ForumGroup();
                        topics.CurrentGroup.GroupId = gId;
                    }

                    if (!String.IsNullOrEmpty(categoryId))
                        topics.CategoryId = Convert.ToInt64(categoryId);

                    #region order groups by user preferences
					var memberDisplay = MemberCache.GetMemberDisplay(memId);
					string groupsOrderString = memberDisplay.Settings.ForumGroupOrder; 
                    if (!string.IsNullOrWhiteSpace(groupsOrderString))
                    {
                        List<long> groupsOrder = new List<long>();
                        groupsOrder.Add(0); // default Forum
                        var ids = groupsOrderString.Split(':').Select(long.Parse).ToList();
                        foreach(var i in ids)
                        {
                            groupsOrder.Add(i);
                        }
                        var groups = topics.GroupTopics;
                        var groupsOrdered = (from i in groupsOrder
                                             join o in groups on i equals o.GroupId
                                             select o).ToList();
                        //make sure that all the groups are part of the result
                        if (groups.Count > groupsOrdered.Count)
                        {
                            // if not add the unsorted groups at the end
                            foreach (var group in groups)
                            {
                                if (!groupsOrdered.Contains(group))
                                {
                                    groupsOrdered.Add(group);
                                }
                            }
                        }

                        topics.GroupTopics = groupsOrdered;
                    }
                    #endregion

                    return View(topics);
                }
                Forum forum = new Forum();
                forum.ForumId = new Guid();
                return View(forum);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult CreateForum(string type)
        {
            try
            {
                Guid ownerId = new Guid();
                Guid forumId = new Guid();
                Guid memberId = RDN.Library.Classes.Account.User.GetMemberId();
                if (type == ForumOwnerTypeEnum.league.ToString())
                {
                    ownerId = MemberCache.GetLeagueIdOfMember(memberId);
                    forumId = Forum.CreateNewForum(ownerId, ForumOwnerTypeEnum.league, "Forum");
                }
                else if (type == ForumOwnerTypeEnum.federation.ToString())
                {
                    ownerId = MemberCache.GetFederationIdOfMember(memberId);
                    forumId = Forum.CreateNewForum(ownerId, ForumOwnerTypeEnum.federation, "Forum");
                }

                MemberCache.Clear(memberId);
                MemberCache.ClearApiCache(memberId);
                return Redirect(Url.Content("~/forum/posts/" + type + "/" + forumId.ToString().Replace("-", "")));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult GetForumPosts(string groupId, string forumId, string page, string isArchived, string pageCount, string forumType)
        {
            try
            {
                long gro = Convert.ToInt64(groupId);
                ForumSelectedJson json = new ForumSelectedJson();
                ForumOwnerTypeEnum type = ForumOwnerTypeEnum.league;
                if (Enum.TryParse<ForumOwnerTypeEnum>(forumType, out type))
                {
                    bool isArchive = false;
                    Boolean.TryParse(isArchived, out isArchive);
                    var memId = RDN.Library.Classes.Account.User.GetMemberId();

                    int pageCountTemp = Forum.DEFAULT_PAGE_SIZE;
                    Int32.TryParse(pageCount, out pageCountTemp);
                    json.Topics = Forum.GetForumTopicsJson(Convert.ToInt32(page), pageCountTemp, new Guid(forumId), gro, 0, isArchive, type);
                    json.Categories = Forum.GetCategoriesOfForum(new Guid(forumId), Convert.ToInt64(groupId));
                    var unreadTopicsCount = json.Topics.Where(x => x.IsRead == false).Count();
                    if (json.Categories.Count > 0 || unreadTopicsCount > 0)
                        json.Categories.Insert(0, new ForumCategory { CategoryName = "Latest", CategoryId = 0, UnreadTopics = unreadTopicsCount, GroupId = Convert.ToInt64(groupId) });
                    if (unreadTopicsCount > 0)
                        json.Categories.Insert(0, new ForumCategory { CategoryName = "Unread", CategoryId = -1, UnreadTopics = unreadTopicsCount, GroupId = Convert.ToInt64(groupId) });
                    if (type == ForumOwnerTypeEnum.league)
                    {
                        if (gro > 0)
                            json.IsManager = MemberCache.IsModeratorOrBetterOfLeagueGroup(memId, gro);
                        else
                            json.IsManager = MemberCache.IsManagerOrBetterOfLeague(memId);
                    }
                }
                System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string sJSON = oSerializer.Serialize(json);
                return Content(sJSON);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public JsonResult CheckIsForumPostExists(string forumId, long topicId, long messageId)
        {
            try
            {
                var isExists = Forum.CheckIsPostExists(new Guid(forumId), topicId, messageId);
                return Json(new { isSuccess = true, isExists = isExists }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult GetForumPostsCat(string groupId, string catId, string forumId, string page, string forumType)
        {
            try
            {
                var memberId = RDN.Library.Classes.Account.User.GetMemberId();
                List<ForumTopicJson> topics = new List<ForumTopicJson>();
                ForumOwnerTypeEnum type = (ForumOwnerTypeEnum)Enum.Parse(typeof(ForumOwnerTypeEnum), forumType);
                long cat = -1;
                Int64.TryParse(catId, out cat);
                if (cat != -1)
                    topics = Forum.GetForumTopicsJson(Convert.ToInt32(page), Forum.DEFAULT_PAGE_SIZE, new Guid(forumId), Convert.ToInt64(groupId), cat, false, type);
                else
                    topics = Forum.GetForumTopicsJsonUnread(new Guid(forumId), Convert.ToInt64(groupId), memberId);


                System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string sJSON = oSerializer.Serialize(topics);
                return Content(sJSON);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult SearchForumPosts(string q, int limit, string groupId, string forumId, string forumType)
        {
            try
            {
                var topics = Forum.GetForumTopicsJson(q, limit, new Guid(forumId), Convert.ToInt64(groupId));
                System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string sJSON = oSerializer.Serialize(topics);
                return Content(sJSON);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = "false" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PostImageUpload(NewPost model)
        {            
            string result;
            var serializer = new JavaScriptSerializer();
            try
            {
                if (Request.Files.Count > 0 && model.File == null)
                {
                    model.File = Request.Files[0];
                }
                // upload the file
                if (model.File != null && model.File.ContentLength > 0)
                {
                    Guid id = new Guid();
                    if (model.ForumType == ForumOwnerTypeEnum.league)
                        id = Forum.GetLeagueIdOfForum(model.ForumId);
                    else if (model.ForumType == ForumOwnerTypeEnum.federation)
                        id = Forum.GetFederatonIdOfForum(model.ForumId);
                    if (id != new Guid())
                    {
                        var doc = DocumentRepository.UploadLeagueDocument(id, model.File.InputStream, model.File.FileName, "Forum Files");
                        FileInfo fi = new FileInfo(doc.SaveLocation);
                        
                        result = string.Format("<script>top.$('.mce-btn.mce-open').parent().find('.mce-textbox').val('{0}').closest('.mce-window').find('.mce-primary').click();</script>",
                                                        RDN.Library.Classes.Config.LibraryConfig.InternalSite + "/document/view/" + doc.DocumentId.ToString().Replace("-", "") + fi.Extension);
                        
                        return Content(result); // IMPORTANT to return as HTML
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            result = serializer.Serialize(
                        new { success = false, message = "Invalid image file" });
            return Content(result);
        }

        public ActionResult ClearForumTopicCache(string forumId, string topicId)
        {
            RDN.Library.Cache.ForumTopicCache.ClearTopicCache(new Guid(forumId), Convert.ToInt64(topicId));
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }




        /// <summary>
        /// This Method will get the message id and process the count in database.
        /// For every individual message id will have one row.
        /// the new count will be updated if message id does not exist then it will be inserted as a new entry.
        /// </summary>
        /// <param name="messageId">messageId</param>
        /// <param name="memberId">memberId</param>
        /// <returns>It will return the messageid and count.
        /// Using message id it will find the button.As button id is same messageid.
        /// </returns>
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public JsonResult GetLikeCount(string messageId, string forumId, string topicId)
        {
            var memberId = RDN.Library.Classes.Account.User.GetMemberId();
            var data = Forum.ToggleMessageLike(Convert.ToInt64(messageId), memberId);


            if (data == null)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);


            ForumTopicCache.ClearTopicCache(new Guid(forumId), Convert.ToInt64(topicId));
            WebClient client = new WebClient();
            client.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_FORUM_TOPIC_API + "forumId=" + forumId + "&topicId=" + topicId));

            return Json(new { success = true, message = data }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// This Method will get the total number of I Agree
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        /// 
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public JsonResult GetIAgreeCount(string messageId, string forumId, string topicId)
        {
            var memberId = RDN.Library.Classes.Account.User.GetMemberId();
            var data = Forum.ToggleMessageAgreement(Convert.ToInt64(messageId), memberId);

            if (data == null)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);



            ForumTopicCache.ClearTopicCache(new Guid(forumId), Convert.ToInt64(topicId));
            WebClient client = new WebClient();
            client.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_FORUM_TOPIC_API + "forumId=" + forumId + "&topicId=" + topicId));


            return Json(new { success = true, message = data }, JsonRequestBehavior.AllowGet);

        }





    }
}
