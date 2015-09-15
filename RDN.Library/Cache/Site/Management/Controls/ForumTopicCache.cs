using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Forum;
using RDN.Portable.Config;

namespace RDN.Library.Cache
{
    /// <summary>
    /// seperating the cache out by each topic instead of by each forum.
    /// since if it was by each forum the cache could get HUGE if the forum cache item was never
    /// released from memory.
    /// </summary>
    public class ForumTopicCache : CacheLock
    {
        public Forum Forum { get; set; }
        public ForumTopic Topic { get; set; }


        public static ForumTopic GetTopic(Guid forumId, long topicId)
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache, forumId, topicId);
                if (cached.Topic.TopicId == 0)
                {
                    cached.Topic = Forum.GetForumTopic(forumId, topicId);
                    if (cached != null)
                        UpdateCache(cached);
                }
                return cached.Topic;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }


        public static void ClearTopicCache(Guid forumId, long topicId)
        {
            try
            {
                HttpContext.Current.Cache.Remove("ForumTopicCache-" + forumId + "-" + topicId);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        private static ForumTopicCache GetCache(System.Web.Caching.Cache cache, Guid forumId, long TopicId)
        {
            try
            {
                ForumTopicCache dataObject = (ForumTopicCache)cache["ForumTopicCache-" + forumId + "-" + TopicId];
                if (dataObject == null)
                {

                    dataObject = (ForumTopicCache)cache["ForumTopicCache-" + forumId + "-" + TopicId];

                    if (dataObject == null)
                    {
                        dataObject = new ForumTopicCache();
                        dataObject.Topic = new ForumTopic();
                        dataObject.Forum = new Forum();
                        dataObject.Forum.ForumId = forumId;

                        cache["ForumTopicCache-" + forumId + "-" + TopicId] = dataObject;

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
        private static ForumTopicCache UpdateCache(ForumTopicCache forumCache)
        {
            try
            {
                lock (ThisLock)
                {
                    HttpContext.Current.Cache["ForumTopicCache-" + forumCache.Forum.ForumId + "-" + forumCache.Topic.TopicId] = forumCache;
                }
                return forumCache;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
    }
}
