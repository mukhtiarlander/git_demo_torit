using RDN.Library.Classes.Error;
using RDN.Library.Classes.Game;
using RDN.Library.Classes.RN.RSS;
using RDN.Portable.Classes.RN;
using RDN.Portable.Models.Json.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace RDN.Library.Cache.Site
{
    public class RNCache : CacheLock
    {
        public List<RSSFeedItem> RssFeeds { get; set; }

        public static List<RSSFeedItem> GetRSSFeeds()
        {
            var cached = GetCache(HttpContext.Current.Cache);
            if (cached.RssFeeds == null)
            {

                cached.RssFeeds = RSSFactory.Initilize().PullAllFeeds(1000, 0).OrderBy(x=>x.NameOfOrganization).ToList();
                UpdateCache(cached);
            }
            return cached.RssFeeds;
        }

        private static RNCache GetCache(System.Web.Caching.Cache cache)
        {
            try
            {
                RNCache dataObject = (RNCache)cache["RNCache"];
                if (dataObject == null)
                {
                    lock (ThisLock)
                    {
                        dataObject = (RNCache)cache["RNCache"];
                        if (dataObject == null)
                        {
                            dataObject = new RNCache();
                            dataObject.RssFeeds = null;

                            cache["RNCache"] = dataObject;
                        }
                    }

                }
                return dataObject;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        private static RNCache UpdateCache(RNCache siteCache)
        {
            try
            {
                lock (ThisLock)
                {
                    HttpContext.Current.Cache["RNCache"] = siteCache;
                }
                return siteCache;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
    }
}
