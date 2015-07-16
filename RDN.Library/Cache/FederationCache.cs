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
    public class ApiFederationCache : CacheLock
    {
        List<MemberDisplayFederation> MembersOfFederation { get; set; }
        FederationDisplay Federation { get; set; }
        public ApiFederationCache()
        {
            MembersOfFederation = new List<MemberDisplayFederation>();
        }

        public static List<MemberDisplayFederation> GetMembersOfFederation(Guid federationId)
        {
            try
            {
                var cached = GetCache(federationId, HttpContext.Current.Cache);
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
        private static ApiFederationCache GetCache(Guid federationId, System.Web.Caching.Cache cache)
        {
            try
            {
                ApiFederationCache dataObject = (ApiFederationCache)cache["FederationCache" + federationId];
                if (dataObject == null)
                {
                    lock (ThisLock)
                    {
                        dataObject = (ApiFederationCache)cache["FederationCache" + federationId];

                        if (dataObject == null)
                        {
                            dataObject = new ApiFederationCache();
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
