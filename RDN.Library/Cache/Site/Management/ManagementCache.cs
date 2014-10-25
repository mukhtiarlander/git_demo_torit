using RDN.Library.Classes.Error;
using RDN.Library.Classes.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace RDN.Library.Cache.ManagementSite
{
    public class ManagementCache : CacheLock
    {
        public int BoutChallengesCount { get; set; }
        public int OfficiatingRequestsCount { get; set; }

        public static int GetBoutChallengesCount()
        {
            try
            {
                var cached = GetCache();
                return cached.BoutChallengesCount;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }
        public static void AddOfficiatingRequestsCount(int count)
        {
            try
            {
                var cached = GetCache();
                cached.OfficiatingRequestsCount += count;
                UpdateCache(cached);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        public static int GetOfficiatingRequestsCount()
        {
            try
            {
                var cached = GetCache();
                return cached.OfficiatingRequestsCount;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }
        private static ManagementCache GetCache()
        {
            try
            {
                ManagementCache dataObject = (ManagementCache)HttpContext.Current.Cache["ManagementCache"];
                if (dataObject == null)
                {
                    lock (ThisLock)
                    {
                        if (dataObject == null)
                        {
                            dataObject = new ManagementCache();
                            dataObject.BoutChallengesCount = RDN.Library.Classes.League.BoutList.GetBoutListCount();
                            dataObject.OfficiatingRequestsCount = RDN.Library.Classes.Officials.RequestFactory.GetRequestCount();

                            HttpContext.Current.Cache["ManagementCache"] = dataObject;
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
        private static ManagementCache UpdateCache(ManagementCache siteCache)
        {
            try
            {
                lock (ThisLock)
                {
                    HttpContext.Current.Cache["ManagementCache"] = siteCache;
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
