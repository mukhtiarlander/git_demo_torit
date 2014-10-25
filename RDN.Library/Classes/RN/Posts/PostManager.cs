using RDN.Library.Classes.Error;
using RDN.Library.DataModels.RN.Posts;
using RN.Library.DataModels.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.Library.Classes.RN.Posts
{
    public class PostManager
    {
        public static bool AddPostForFeed(Guid id, long feedId)
        {
            try
            {
                var dc = new RNManagementContext();
                var p = dc.Posts.Where(x => x.PostId == id).FirstOrDefault();
                if (p != null)
                {
                    p.Feed = dc.RssFeeds.Where(x => x.RssId == feedId).FirstOrDefault();
                }
                else
                {
                    p = new Post();
                    p.PostId = id;
                    p.Feed = dc.RssFeeds.Where(x => x.RssId == feedId).FirstOrDefault();
                    dc.Posts.Add(p);
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



        public static List<Classes.Post> GetAllPostsViews()
        {
            var dc = new RNManagementContext();
            var posts = (from xx in dc.Posts
                         select new
                         {
                             xx.CurrentMonthlyViews,
                             xx.PostId,
                             xx.TotalViews,
                             xx.DisabledAutoPosting,
                             xx.DisablePaymentsForPost,
                             xx.Feed
                         }).ToList();
            List<Classes.Post> postss = new List<Classes.Post>();
            for (int i = 0; i < posts.Count; i++)
            {
                Classes.Post p = new Classes.Post();
                p.TotalMonthlyViews = posts[i].CurrentMonthlyViews;
                p.Id = posts[i].PostId;
                p.TotalViews = posts[i].TotalViews;
                p.DisabledAutoPosting = posts[i].DisabledAutoPosting;
                p.DisablePaymentsForPost = posts[i].DisablePaymentsForPost;
                if (posts[i].Feed != null)
                {
                    p.FromFeed = true;
                    p.FeedName = posts[i].Feed.NameOfSite;
                    p.FeedUrl = posts[i].Feed.UrlOfSite;
                }
                postss.Add(p);
            }
            return postss;
        }
        public static bool AddViewToPost(Guid id)
        {
            try
            {
                var dc = new RNManagementContext();
                var p = dc.Posts.Where(x => x.PostId == id).FirstOrDefault();
                if (p != null)
                {
                    p.TotalViews += 1;
                    p.CurrentMonthlyViews += 1;
                }
                else
                {
                    p = new Post();
                    p.PostId = id;
                    p.CurrentMonthlyViews = 1;
                    p.TotalViews = 1;
                    dc.Posts.Add(p);
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
        public static bool SavePostToDb(Guid id, bool isAutoSharingDisabled, bool disablePaymentsForPost)
        {
            try
            {
                var dc = new RNManagementContext();
                var p = dc.Posts.Where(x => x.PostId == id).FirstOrDefault();
                if (p != null)
                {
                    p.DisabledAutoPosting = isAutoSharingDisabled;
                    p.DisablePaymentsForPost = disablePaymentsForPost;
                    p.LastTimePostedToFacebook = DateTime.UtcNow;
                    p.TotalFacebookPosts += 1;
                }
                else
                {
                    p = new Post();
                    p.PostId = id;

                    p.DisabledAutoPosting = isAutoSharingDisabled;
                    p.DisablePaymentsForPost = disablePaymentsForPost;
                    p.LastTimePostedToFacebook = DateTime.UtcNow;
                    p.TotalFacebookPosts = 1;
                    dc.Posts.Add(p);
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
    }
}
