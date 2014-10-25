using RDN.Library.Classes.Error;
using RDN.Library.Classes.Game;
using RDN.Portable.Models.Json.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace RDN.Library.Cache.Site
{
    public class PublicSiteCache : CacheLock
    {
        public List<CurrentGameJson> PastGames { get; set; }

        public static List<CurrentGameJson> GetPastGames()
        {
            var cached = GetCache(HttpContext.Current.Cache);
            if (cached.PastGames == null)
            {
                cached.PastGames = RDN.Library.Classes.Game.Game.GetPastWeeksGames();
                UpdateCache(cached);
            }
            return cached.PastGames;
        }

        private static PublicSiteCache GetCache(System.Web.Caching.Cache cache)
        {
            try
            {
                PublicSiteCache dataObject = (PublicSiteCache)cache["PublicSiteCache"];
                if (dataObject == null)
                {
                    lock (ThisLock)
                    {
                        dataObject = (PublicSiteCache)cache["PublicSiteCache"];
                        if (dataObject == null)
                        {
                            dataObject = new PublicSiteCache();
                            dataObject.PastGames = null;

                            cache["PublicSiteCache"] = dataObject;
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
        private static PublicSiteCache UpdateCache(PublicSiteCache siteCache)
        {
            try
            {
                lock (ThisLock)
                {
                    HttpContext.Current.Cache["PublicSiteCache"] = siteCache;
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
