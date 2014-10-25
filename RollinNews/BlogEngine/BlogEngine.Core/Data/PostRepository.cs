using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using RDN.Library.Classes.EmailServer;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.RN.Posts;
using RDN.Portable.Config;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Security;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Post repository
    /// </summary>
    public class PostRepository : IPostRepository
    {
        private int postCount {  get; set; }

        /// <summary>
        /// Post list
        /// </summary>
        /// <param name="filter">Filter expression</param>
        /// <param name="order">Order expression</param>
        /// <param name="skip">Records to skip</param>
        /// <param name="take">Records to take</param>
        /// <returns>List of posts</returns>
        public IEnumerable<PostItem> Find(int take = 10, int skip = 0, string filter = "", string order = "", string published = "")
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.ViewPublicPosts))
                throw new System.UnauthorizedAccessException();

            var posts = new List<PostItem>();
            //remove isvisible since we were not seeing any posts for unpublished for the author.
            //var postList = Post.ApplicablePosts.Where(p => p.IsVisible).ToList();
            var postList = Post.ApplicablePosts;

            if (!Security.IsAuthorizedTo(Rights.EditOtherUsersPosts))
                postList = postList.Where(p => p.Author.ToLower() == Security.CurrentUser.Identity.Name.ToLower()).ToList();

            if (!String.IsNullOrEmpty(published))
            {
                var p = Convert.ToBoolean(published);
                postList = postList.Where(x => x.IsPublished == p).ToList();
            }

            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                postList = (from xx in postList
                            where (String.IsNullOrEmpty(xx.Author) && xx.Author.ToLower().Contains(filter)) || xx.Title.ToLower().Contains(filter)
                            select xx).ToList();
            }

            if (take == 0) take = postList.Count;
            //if (string.IsNullOrEmpty(filter)) 
            filter = "1==1";
            if (string.IsNullOrEmpty(order)) order = "DateCreated desc";

            var query = postList.AsQueryable().Where(filter);

            postCount = postList.Count;

            foreach (var item in query.OrderBy(order).Skip(skip).Take(take))
                posts.Add(ToJson((BlogEngine.Core.Post)item));

            return posts;
        }
        public long PostCount()
        {
            return postCount;
        }
        public IEnumerable<PostItem> FindPublic(int take = 10, int skip = 0, string filter = "", string order = "")
        {

            var posts = new List<PostItem>();
            //visible is visible with the current datetime that the post was published.
            var postList = Post.ApplicablePosts.Where(p => p.IsPublished && p.IsVisible).ToList();


            if (take == 0) take = postList.Count;
            if (string.IsNullOrEmpty(filter)) filter = "1==1";
            if (string.IsNullOrEmpty(order)) order = "DateCreated desc";

            var query = postList.AsQueryable().Where(filter);

            foreach (var item in query.OrderBy(order).Skip(skip).Take(take))
                posts.Add(ToJson((BlogEngine.Core.Post)item));

            return posts;
        }
        public IEnumerable<PostItem> FindPublic(DateTime createdAfter)
        {

            var posts = new List<PostItem>();
            //visible is visible with the current datetime that the post was published.
            var postList = Post.ApplicablePosts.Where(p => p.IsPublished && p.IsVisible && p.DateCreated > createdAfter).ToList();

            foreach (var item in postList)
                posts.Add(ToJson((BlogEngine.Core.Post)item));

            return posts;
        }
        /// <summary>
        /// AuthorUserName, PostId
        /// </summary>
        /// <returns></returns>
        public List<Post> GetPostsAndAuthors()
        {

            var postList = Post.ApplicablePosts.Where(p => p.IsVisible && p.IsPublished).ToList();



            return postList;
        }

        /// <summary>
        /// Get single post
        /// </summary>
        /// <param name="id">Post id</param>
        /// <returns>Post object</returns>
        public PostDetail FindById(Guid id)
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.ViewPublicPosts))
                throw new System.UnauthorizedAccessException();
            try
            {
                return ToJsonDetail((from p in Post.Posts.ToList() where p.Id == id select p).FirstOrDefault());
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Add new post
        /// </summary>
        /// <param name="detail">Post</param>
        /// <returns>Saved post with new ID</returns>
        public PostDetail Add(PostDetail detail, string action)
        {

            var post = new Post();

            try
            {

                if (action == "RNFeedSubmittal")
                {
                    detail.IsSavedForApproval = true;
                    post.DateModified = DateTime.Now;


                    Save(post, detail);
                    SendEmailAboutNewPostWaitingApprovalChiefs(post.AbsoluteLink.ToString());
                    return ToJsonDetail(post);
                }
                if (action == "DNNSubmittal")
                {
                    detail.IsSavedForApproval = true;
                    post.DateModified = DateTime.Now;


                    Save(post, detail);
                    return ToJsonDetail(post);
                }

                if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.CreateNewPosts))
                    throw new System.UnauthorizedAccessException();

                if (action == "publishSelf")
                {
                    detail.IsPublished = true;
                    post.DateModified = DateTime.Now;


                    Save(post, detail);


                    SendEmailAboutNewPostPublishedToAuthor(post.Author, post.AbsoluteLink.ToString(), post.DateCreated.ToUniversalTime());
                    SendEmailAboutNewPostPublishedToChiefs(post.AbsoluteLink.ToString(), post.DateCreated);

                }
                if (action == "publish")
                {
                    post.IsPublished = true;
                    post.DateModified = DateTime.Now;

                    //publish to facebook.


                    Save(post, detail);
                    SendEmailAboutNewPostPublishedToAuthor(post.Author, post.AbsoluteLink.ToString(), post.DateCreated.ToUniversalTime());
                    SendEmailAboutNewPostPublishedToChiefs(post.AbsoluteLink.ToString(), post.DateCreated);

                }
                else if (action == "unpublish")
                {
                    post.IsPublished = false;
                    post.DateModified = DateTime.Now;
                    Save(post, detail);
                }
                else if (action == "unpublishSelf")
                {
                    detail.IsPublished = false;
                    post.DateModified = DateTime.Now;
                    Save(post, detail);
                }
                else if (action == "submitForApproval")
                {
                    detail.IsSavedForApproval = true;
                    post.DateModified = DateTime.Now;


                    Save(post, detail);
                    SendEmailAboutNewPostWaitingApprovalChiefs(post.AbsoluteLink.ToString());

                }
                else
                {
                    Save(post, detail);
                }
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());
            }

            return ToJsonDetail(post);
        }

        /// <summary>
        /// Update post
        /// </summary>
        /// <param name="detail">Post to update</param>
        /// <param name="action">Action</param>
        /// <returns>True on success</returns>
        public bool Update(PostDetail detail, string action)
        {
            try
            {
                if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.EditOwnPosts))
                    throw new System.UnauthorizedAccessException();

                var post = (from p in Post.Posts.ToList() where p.Id == detail.Id select p).FirstOrDefault();

                if (post != null)
                {
                    if (action == "publishSelf")
                    {
                        detail.IsPublished = true;
                        post.DateModified = DateTime.Now;

                        Save(post, detail);

                        SendEmailAboutNewPostPublishedToAuthor(post.Author, post.AbsoluteLink.ToString(), post.DateCreated.ToUniversalTime());
                        SendEmailAboutNewPostPublishedToChiefs(post.AbsoluteLink.ToString(), post.DateCreated);

                    }
                    if (action == "publish")
                    {
                        post.IsPublished = true;
                        post.DateModified = DateTime.Now;


                        //publish to facebook.

                        post.Save();

                        SendEmailAboutNewPostPublishedToAuthor(post.Author, post.AbsoluteLink.ToString(), post.DateCreated.ToUniversalTime());
                        SendEmailAboutNewPostPublishedToChiefs(post.AbsoluteLink.ToString(), post.DateCreated);
                    }
                    else if (action == "unpublish")
                    {
                        post.IsPublished = false;
                        post.DateModified = DateTime.Now;
                        post.Save();
                    }
                    else if (action == "unpublishSelf")
                    {
                        detail.IsPublished = false;
                        post.DateModified = DateTime.Now;
                        Save(post, detail);
                    }
                    else if (action == "submitForApproval")
                    {
                        detail.IsSavedForApproval = true;
                        post.DateModified = DateTime.Now;

                        SendEmailAboutNewPostWaitingApprovalChiefs(post.AbsoluteLink.ToString());


                        Save(post, detail);
                    }
                    else
                    {
                        Save(post, detail);
                    }
                    return true;
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return false;
        }

        private static void SendEmailAboutNewPostPublishedToChiefs(string link, DateTime UTCDateTime)
        {
            //admin/editor/post.cshtml?id=c5646b46-3163-4368-a424-cbd083abc1fb
            try
            {

                var users = Roles.GetUsersInRole("Chiefs");
                foreach (var user in users)
                {
                    var id = RDN.Library.Classes.Account.User.GetMemberId(user);
                    var member = RDN.Library.Cache.SiteCache.GetPublicMember(id);
                    if (member != null)
                    {
                        var emailData = new Dictionary<string, string>
                                            {
                                                { "derbyname",member.DerbyName},
                                                {"link",link},
                                                {"datetime",UTCDateTime.ToString()}
                                              };

                        EmailServer.SendEmail(RollinNewsConfig.DEFAULT_EMAIL, RollinNewsConfig.DEFAULT_EMAIL_FROM_NAME, user, EmailServer.DEFAULT_SUBJECT_ROLLIN_NEWS + " New Post Published!", emailData, EmailServerLayoutsEnum.RNNewPostPublished);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        private static void SendEmailAboutNewPostWaitingApprovalChiefs(string link)
        {
            //admin/editor/post.cshtml?id=c5646b46-3163-4368-a424-cbd083abc1fb
            try
            {

                var users = Roles.GetUsersInRole("Chiefs");
                foreach (var user in users)
                {
                    var id = RDN.Library.Classes.Account.User.GetMemberId(user);
                    var member = RDN.Library.Cache.SiteCache.GetPublicMember(id);
                    if (member != null)
                    {
                        var emailData = new Dictionary<string, string>
                                            {
                                                { "derbyname",member.DerbyName},
                                                {"link",link}
                                              };

                        EmailServer.SendEmail(RollinNewsConfig.DEFAULT_EMAIL, RollinNewsConfig.DEFAULT_EMAIL_FROM_NAME, user, EmailServer.DEFAULT_SUBJECT_ROLLIN_NEWS + " Post Awaiting Approval", emailData, EmailServerLayoutsEnum.RNPostAwaitingApproval);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        private static void SendEmailAboutNewPostPublishedToAuthor(string userName, string linkToPost, DateTime UTCPublishedDate)
        {
            //admin/editor/post.cshtml?id=c5646b46-3163-4368-a424-cbd083abc1fb

            try
            {
                var id = RDN.Library.Classes.Account.User.GetMemberId(userName);
                var member = RDN.Library.Cache.SiteCache.GetPublicMember(id);
                if (member != null)
                {
                    var emailData = new Dictionary<string, string>
                                            {
                                                { "derbyname",member.DerbyName},
                                                {"link",linkToPost},
                                                {"datetime",UTCPublishedDate.ToString()},
                                              };

                    EmailServer.SendEmail(RollinNewsConfig.DEFAULT_EMAIL, RollinNewsConfig.DEFAULT_EMAIL_FROM_NAME, userName, EmailServer.DEFAULT_SUBJECT_ROLLIN_NEWS + " Your Post was Published!", emailData, EmailServerLayoutsEnum.RNNewPostPublished);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        /// <summary>
        /// Delete post
        /// </summary>
        /// <param name="id">Post ID</param>
        /// <returns>True on success</returns>
        public bool Remove(Guid id)
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.DeleteOwnPosts))
                throw new System.UnauthorizedAccessException();
            try
            {
                var post = (from p in Post.Posts.ToList() where p.Id == id select p).FirstOrDefault();
                post.Delete();
                post.Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region Private Methods

        public static PostItem ToJson(Post post)
        {
            var p = new PostItem
            {
                Id = post.Id,
                Author = post.Author,
                AuthorUserId = post.AuthorUserId,
                Title = post.Title,
                Slug = post.Slug,
                RelativeLink = post.RelativeLink,
                DateCreated = post.DateCreated.ToString("yyyy-MM-dd HH:mm"),
                Categories = GetCategories(post.Categories),
                Tags = GetTags(post.Tags),
                Comments = GetComments(post),
                IsPublished = post.IsPublished,
                InitialImageUrl = post.InitialImageUrl,
                MainImageUrl = post.MainImageUrl,
                AuthorDerbyId = post.AuthorDerbyId,
                AuthorDerbyName = post.AuthorDerbyName,
                TotalMonthsViews = post.TotalMonthsViews,
                TotalViews = post.TotalViews,
                DisabledAutoPosting = post.DisabledAutoPosting,
                DisablePaymentsForPost = post.DisablePaymentsForPost
            };
            if (!String.IsNullOrEmpty(p.InitialImageUrl))
                p.InitialImageUrl = p.InitialImageUrl.Replace("\"", "");
            if (!String.IsNullOrEmpty(p.MainImageUrl))
                p.MainImageUrl = p.MainImageUrl.Replace("\"", "");
            return p;
        }

        static PostDetail ToJsonDetail(Post post)
        {
            return new PostDetail
            {
                Id = post.Id,
                Author = post.Author,
                AuthorUserId = post.AuthorUserId,
                Title = post.Title,
                Slug = post.Slug,
                Description = post.Description,
                RelativeLink = post.RelativeLink,
                Content = post.Content,
                DateCreated = post.DateCreated.ToString("yyyy-MM-dd HH:mm"),
                Categories = GetCategories(post.Categories),
                Tags = GetTags(post.Tags),
                Comments = GetComments(post),
                HasCommentsEnabled = post.HasCommentsEnabled,
                IsPublished = post.IsPublished,
                IsDeleted = post.IsDeleted,
                CanUserEdit = post.CanUserEdit,
                CanUserDelete = post.CanUserDelete,
                InitialImageUrl = post.InitialImageUrl,
                MainImageUrl = post.MainImageUrl,
                AuthorDerbyId = post.AuthorDerbyId,
                AuthorDerbyName = post.AuthorDerbyName,
                DisabledAutoPosting = post.DisabledAutoPosting,
                DisablePaymentsForPost = post.DisablePaymentsForPost,
                BottomLineForConstribution = post.BottomLineForConstribution
            };
        }

        static void Save(Post post, PostDetail detail)
        {
            post.Title = detail.Title;
            //post.BottomLineForConstribution = detail.BottomLineForConstribution;
            post.Author = detail.Author;
            //if we can't find author anymore, at least their userId stays in tact so we can pay them.
            if (detail.Author != ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN)
            {
                var user = Membership.GetUser(detail.Author);
                if (user != null)
                    post.AuthorUserId = (Guid)user.ProviderUserKey;
                var Member = RDN.Library.Cache.SiteCache.GetPublicMemberFull(post.Author);
                if (Member != null)
                {
                    post.AuthorDerbyName = Member.DerbyName;
                    post.AuthorDerbyId = Member.MemberId;
                }
            }
            else
            {
                post.AuthorUserId = new Guid("885e1e16-0f8e-44a8-bb36-4fce12ef6360");
            }
            post.Description = detail.Description;
            post.Content = detail.Content;
            if (String.IsNullOrEmpty(post.Content))
                post.Content = "Please Add Content";
            post.Slug = detail.Slug;
            post.IsPublished = detail.IsPublished;
            post.HasCommentsEnabled = detail.HasCommentsEnabled;
            post.IsDeleted = detail.IsDeleted;
            post.IsSavedForApproval = detail.IsSavedForApproval;
            post.DisabledAutoPosting = detail.DisabledAutoPosting;
            post.DisablePaymentsForPost = detail.DisablePaymentsForPost;
            DateTime posted = DateTime.UtcNow;
            DateTime.TryParseExact(detail.DateCreated, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out posted);
            post.DateCreated = posted;
            if (!String.IsNullOrEmpty(detail.InitialImageUrl))
                post.InitialImageUrl = detail.InitialImageUrl.Replace("\"", "");
            if (!String.IsNullOrEmpty(detail.MainImageUrl))
                post.MainImageUrl = detail.MainImageUrl.Replace("\"", "");

            UpdatePostCategories(post, detail.Categories);
            UpdatePostTags(post, FilterTags(detail.Tags));

            post.Save();

            PostManager.SavePostToDb(post.Id, post.DisabledAutoPosting, post.DisablePaymentsForPost);
        }

        static List<CategoryItem> GetCategories(ICollection<Category> categories)
        {
            if (categories == null || categories.Count == 0)
                return new List<CategoryItem>();

            //var html = categories.Aggregate("", (current, cat) => current + string.Format
            //("<a href='#' onclick=\"ChangePostFilter('Category','{0}','{1}')\">{1}</a>, ", cat.Id, cat.Title));
            var categoryList = new List<CategoryItem>();
            foreach (var coreCategory in categories)
            {
                var item = new CategoryItem();
                item.Id = coreCategory.Id;
                item.Title = coreCategory.Title;
                item.Description = coreCategory.Description;
                item.Parent = ItemParent(coreCategory.Parent);
                categoryList.Add(item);
            }
            return categoryList;
        }

        static SelectOption ItemParent(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return null;

            var item = Category.Categories.Where(c => c.Id == id).FirstOrDefault();
            return new SelectOption { OptionName = item.Title, OptionValue = item.Id.ToString() };
        }

        static List<TagItem> GetTags(ICollection<string> tags)
        {
            if (tags == null || tags.Count == 0)
                return new List<TagItem>();

            var items = new List<TagItem>();
            foreach (var item in tags)
            {
                items.Add(new TagItem { TagName = item });
            }
            return items;
        }

        static string[] GetComments(Post post)
        {
            if (post.Comments == null || post.Comments.Count == 0)
            {
                string[] commentsCheck = new string[3];
                commentsCheck[0] = "0";
                commentsCheck[1] = "0";
                commentsCheck[2] = "0";
                return commentsCheck;
            }

            string[] comments = new string[3];
            comments[0] = post.NotApprovedComments.Count.ToString();
            comments[1] = post.ApprovedComments.Count.ToString();
            comments[2] = post.SpamComments.Count.ToString();
            return comments;
        }

        static void UpdatePostCategories(Post post, List<CategoryItem> categories)
        {
            post.Categories.Clear();
            foreach (var cat in categories)
            {
                // add if category does not exist
                var existingCat = Category.Categories.Where(c => c.Title == cat.Title).FirstOrDefault();
                if (existingCat == null)
                {
                    var repo = new CategoryRepository();
                    post.Categories.Add(Category.GetCategory(repo.Add(cat).Id));
                }
                else
                {
                    post.Categories.Add(Category.GetCategory(existingCat.Id));
                }

            }
        }

        static void UpdatePostTags(Post post, List<TagItem> tags)
        {
            post.Tags.Clear();
            foreach (var t in tags)
            {
                post.Tags.Add(t.TagName);
            }
        }

        static List<TagItem> FilterTags(List<TagItem> tags)
        {
            var uniqueTags = new List<TagItem>();
            foreach (var t in tags)
            {
                if (!uniqueTags.Any(u => u.TagName == t.TagName))
                {
                    uniqueTags.Add(t);
                }
            }
            return uniqueTags;
        }

        #endregion
    }
}