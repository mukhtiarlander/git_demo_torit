using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Error;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Federation;

namespace RDN.Library.Cache
{
    public class FederationCache : CacheLock
    {
        List<MemberDisplayFederation> MembersOfFederation { get; set; }
        FederationDisplay Federation { get; set; }
        public FederationCache()
        {
            MembersOfFederation = new List<MemberDisplayFederation>();
        }

        public static List<MemberDisplayFederation> GetMembersOfFederation(Guid federationId, System.Web.Caching.Cache cache)
        {
            try
            {
                var cached = GetCache(federationId, cache);
                return cached.MembersOfFederation;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static FederationDisplay GetFederation(Guid federationId)
        {
            try
            {
                var cached = GetCache(federationId, HttpContext.Current.Cache);
                return cached.Federation;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public static void Clear(Guid federationId)
        {
            if (HttpContext.Current.Cache["FederationCache" + federationId] != null)
                HttpContext.Current.Cache.Remove("FederationCache" + federationId);
        }

        /// <summary>
        /// gets the cache of the member
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static FederationCache GetCache(Guid federationId, System.Web.Caching.Cache cache)
        {
            try
            {
                FederationCache dataObject = (FederationCache)cache["FederationCache" + federationId];
                if (dataObject == null)
                {
                    lock (ThisLock)
                    {
                        dataObject = (FederationCache)cache["FederationCache" + federationId];

                        if (dataObject == null)
                        {
                            dataObject = new FederationCache();
                            dataObject.MembersOfFederation = RDN.Library.Classes.Federation.Federation.GetMembersOfFederation(federationId);
                            dataObject.Federation = Classes.Federation.Federation.GetFederationDisplay(federationId);

                            cache["FederationCache" + federationId] = dataObject;
                        }
                    }
                }
                return dataObject;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            } return null;
        }


    }
}
