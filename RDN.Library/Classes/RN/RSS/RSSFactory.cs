using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.RN.Posts;
using RDN.Portable.Classes.RN;
using RN.Library.DataModels.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;

namespace RDN.Library.Classes.RN.RSS
{


    //XmlReader reader = XmlReader.Create("http://www.postsecret.com/feeds/posts/default?alt=rss");
    //        SyndicationFeed feed = SyndicationFeed.Load(reader);

    public class RSSFactory
    {


        public static RSSFactory Initilize()
        {
            return new RSSFactory();
        }
        public RSSFeedItem PullFeed(long id)
        {
            try
            {
                var dc = new RNManagementContext();
                var feeds = (from xx in dc.RssFeeds
                             where xx.RssId == id
                             select new RSSFeedItem
                             {
                                 MainUrl = xx.UrlOfSite,
                                 NameOfOrganization = xx.NameOfSite,
                                 Created = xx.Created,
                                 FeedId = xx.RssId,
                                 LastChecked = xx.LastCheck,
                                 TotalPostsPulled = xx.TotalPostsImported,
                                 RSSUrl = xx.UrlOfRss,
                                 InitialImageUrl = xx.InitialImageUrl,
                                 MainImageUrl = xx.MainImageUrl,
                                 CanNotScanFeed = xx.CanNotScanFeed,
                                 AuthorUserId = xx.UserIdToAssignPostsTo,
                                 Categories = (from yy in xx.InitialCategories
                                               select new RSSFeedCategory
                                               {
                                                   CategoryId = yy.CategoryId,
                                                   CategoryRNId = yy.CategoryRNId
                                               }),
                                 Tags = (from yy in xx.InitialTags
                                         select new RSSFeedTag
                                         {
                                             TagName = yy.TagName,
                                             TagId = yy.TagId
                                         })

                             }).FirstOrDefault();

                if (feeds.AuthorUserId != new Guid())
                    feeds.AuthorUserName = Membership.GetUser(feeds.AuthorUserId).UserName;
                return feeds;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return new RSSFeedItem();

        }

        public RSSFeedItem UpdateFeed(RSSFeedItem item)
        {
            try
            {
                var dc = new RNManagementContext();
                var feeds = (from xx in dc.RssFeeds
                             where xx.RssId == item.FeedId
                             select xx).FirstOrDefault();
                feeds.UrlOfRss = item.RSSUrl;
                feeds.NameOfSite = item.NameOfOrganization;
                feeds.UrlOfSite = item.MainUrl;
                feeds.InitialImageUrl = item.InitialImageUrl;
                feeds.MainImageUrl = item.MainImageUrl;
                feeds.CanNotScanFeed = item.CanNotScanFeed;
                if (!String.IsNullOrEmpty(item.AuthorUserName))
                    feeds.UserIdToAssignPostsTo = (Guid)Membership.GetUser(item.AuthorUserName).ProviderUserKey;
                else
                    feeds.UserIdToAssignPostsTo = new Guid();

                var catsInDb = feeds.InitialCategories.ToList();
                var tagsInDb = feeds.InitialTags.ToList();

                foreach (var cat in catsInDb)
                {
                    var c = item.Categories.Where(x => x.CategoryRNId == cat.CategoryRNId).FirstOrDefault();
                    if (c == null)
                        feeds.InitialCategories.Remove(cat);
                }
                foreach (var cat in item.Categories)
                {
                    var c = feeds.InitialCategories.Where(x => x.CategoryRNId == cat.CategoryRNId).FirstOrDefault();
                    if (c == null)
                        feeds.InitialCategories.Add(new DataModels.RN.RSS.RSSFeedCategory() { CategoryRNId = cat.CategoryRNId });
                }
                foreach (var cat in tagsInDb)
                {
                    var c = item.Tags.Where(x => x.TagName == cat.TagName).FirstOrDefault();
                    if (c == null)
                        feeds.InitialTags.Remove(cat);
                }
                foreach (var cat in item.Tags)
                {
                    var c = feeds.InitialTags.Where(x => x.TagName == cat.TagName).FirstOrDefault();
                    if (c == null)
                        feeds.InitialTags.Add(new DataModels.RN.RSS.RSSFeedTag() { TagName = cat.TagName });
                }


                int count = dc.SaveChanges();
                return item;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return new RSSFeedItem();

        }
        public bool RemoveFeed(long id)
        {
            try
            {
                var dc = new RNManagementContext();
                var feeds = (from xx in dc.RssFeeds
                             where xx.RssId == id
                             select xx).FirstOrDefault();
                feeds.IsRemoved = true;
                dc.SaveChanges();
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return false;

        }
        public bool FinishFeedPolling(long id, int count)
        {
            try
            {
                if (count > 0)
                {
                    var dc = new RNManagementContext();
                    var feeds = (from xx in dc.RssFeeds
                                 where xx.RssId == id
                                 select xx).FirstOrDefault();
                    feeds.TotalPostsImported += count;
                    feeds.LastCheck = DateTime.UtcNow;
                    dc.SaveChanges();
                }
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return false;

        }

        public List<RSSFeedItem> PullAllFeedsToScan(int take, int page)
        {
            try
            {
                var dc = new RNManagementContext();
                var feeds = (from xx in dc.RssFeeds
                             where xx.IsRemoved == false
                             where xx.CanNotScanFeed == false
                             select new RSSFeedItem
                             {
                                 Created = xx.Created,
                                 FeedId = xx.RssId,
                                 LastChecked = xx.LastCheck,
                                 TotalPostsPulled = xx.TotalPostsImported,
                                 RSSUrl = xx.UrlOfRss,
                                 NameOfOrganization = xx.NameOfSite,
                                 MainUrl = xx.UrlOfSite,
                                 InitialImageUrl = xx.InitialImageUrl,
                                 MainImageUrl = xx.MainImageUrl,
                                 CanNotScanFeed = xx.CanNotScanFeed,
                                 AuthorUserId = xx.UserIdToAssignPostsTo,
                                 Categories = (from yy in xx.InitialCategories
                                               select new RSSFeedCategory
                                               {
                                                   CategoryId = yy.CategoryId,
                                                   CategoryRNId = yy.CategoryRNId
                                               }),
                                 Tags = (from yy in xx.InitialTags
                                         select new RSSFeedTag
                                         {
                                             TagName = yy.TagName,
                                             TagId = yy.TagId
                                         })

                             }).OrderBy(x => x.NameOfOrganization).Skip(take * page).Take(take).ToList();
                for (int i = 0; i < feeds.Count; i++)
                {
                    if (feeds[i].AuthorUserId != new Guid())
                        feeds[i].AuthorUserName = Membership.GetUser(feeds[i].AuthorUserId).UserName;
                }

                return feeds;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return new List<RSSFeedItem>();

        }

        public List<RSSFeedItem> PullAllFeeds(int take, int page)
        {
            try
            {
                var dc = new RNManagementContext();
                var feeds = (from xx in dc.RssFeeds
                             where xx.IsRemoved == false
                             select new RSSFeedItem
                             {
                                 Created = xx.Created,
                                 FeedId = xx.RssId,
                                 LastChecked = xx.LastCheck,
                                 TotalPostsPulled = xx.TotalPostsImported,
                                 RSSUrl = xx.UrlOfRss,
                                 NameOfOrganization = xx.NameOfSite,
                                 MainUrl = xx.UrlOfSite,
                                 InitialImageUrl = xx.InitialImageUrl,
                                 MainImageUrl = xx.MainImageUrl,
                                 CanNotScanFeed = xx.CanNotScanFeed,
                                 AuthorUserId = xx.UserIdToAssignPostsTo,
                                 Categories = (from yy in xx.InitialCategories
                                               select new RSSFeedCategory
                                               {
                                                   CategoryId = yy.CategoryId,
                                                   CategoryRNId = yy.CategoryRNId
                                               }),
                                 Tags = (from yy in xx.InitialTags
                                         select new RSSFeedTag
                                         {
                                             TagName = yy.TagName,
                                             TagId = yy.TagId
                                         })

                             }).OrderBy(x => x.NameOfOrganization).Skip(take * page).Take(take).ToList();

                for (int i = 0; i < feeds.Count; i++)
                {
                    if (feeds[i].AuthorUserId != new Guid())
                        feeds[i].AuthorUserName = Membership.GetUser(feeds[i].AuthorUserId).UserName;
                }
                return feeds;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return new List<RSSFeedItem>();

        }

        public RSSFactory AddNewFeed(RSSFeedItem item)
        {
            try
            {
                var dc = new RNManagementContext();
                var ff = dc.RssFeeds.Where(x => x.UrlOfRss == item.RSSUrl).FirstOrDefault();
                if (ff == null)
                {
                    RSSFeed feed = new RSSFeed();
                    feed.UrlOfRss = item.RSSUrl;
                    feed.UrlOfSite = item.MainUrl;
                    feed.NameOfSite = item.NameOfOrganization;
                    feed.TotalPostsImported = 0;
                    feed.LastCheck = DateTime.UtcNow.AddYears(-100);
                    feed.Created = DateTime.UtcNow;
                    feed.InitialImageUrl = item.InitialImageUrl;
                    feed.MainImageUrl = item.MainImageUrl;
                    feed.CanNotScanFeed = item.CanNotScanFeed;
                    if (!String.IsNullOrEmpty(item.AuthorUserName))
                        feed.UserIdToAssignPostsTo = (Guid)Membership.GetUser(item.AuthorUserName).ProviderUserKey;
                    foreach (var c in item.Categories)
                    {
                        DataModels.RN.RSS.RSSFeedCategory cat = new DataModels.RN.RSS.RSSFeedCategory();
                        cat.CategoryRNId = c.CategoryRNId;
                        feed.InitialCategories.Add(cat);
                    }
                    foreach (var t in item.Tags)
                    {
                        DataModels.RN.RSS.RSSFeedTag tag = new DataModels.RN.RSS.RSSFeedTag();
                        tag.TagName = t.TagName;
                        feed.InitialTags.Add(tag);
                    }
                    dc.RssFeeds.Add(feed);
                    dc.SaveChanges();
                }
                else
                    ff.IsRemoved = false;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }

            return this;
        }

    }
}
