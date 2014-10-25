using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Cache;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Error;
using RDN.Portable.Classes.Account.Classes;

namespace RDN.Library.Classes.Messages
{
    public class MessagesCache : CacheLock
    {
        public List<MemberDisplayBasic> ListOfRecipients { get; set; }
        public long GroupIdOfMessages { get; set; }

        public static bool IsMemberOfGroup(long groupId, Guid memberId, System.Web.Caching.Cache cache)
        {
            try
            {
                var ob = GetCache(groupId, cache);
                if (ob != null)
                {
                    var mem = ob.ListOfRecipients.Where(x => x.MemberId == memberId).FirstOrDefault();
                    if (mem != null)
                        return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static MessagesCache GetCache(long groupId, System.Web.Caching.Cache cache)
        {
            try
            {
                MessagesCache dataObject = (MessagesCache)cache["MessagesCache" + groupId];
                if (dataObject == null)
                {
                    lock (ThisLock)
                    {
                 
                        dataObject = (MessagesCache)cache["MessagesCache" + groupId];

                        if (dataObject == null)
                        {
                            dataObject = new MessagesCache();
                            dataObject.GroupIdOfMessages = groupId;
                            dataObject.ListOfRecipients = RDN.Library.Classes.Messages.Messages.GetRecipientsOfMessageGroup(groupId);
                            cache["MessagesCache" + groupId] = dataObject;
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
        public static MessagesCache UpdateCache(MessagesCache messageCache, System.Web.Caching.Cache cache)
        {
            try
            {
                lock (ThisLock)
                {
                    cache["MessagesCache" + messageCache.GroupIdOfMessages] = messageCache;
                }
                return messageCache;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

    }
}
