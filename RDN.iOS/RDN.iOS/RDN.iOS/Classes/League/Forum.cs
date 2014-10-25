using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using RDN.Portable.Classes.Controls.Forum;
using System.Threading.Tasks;
using RDN.iOS.Classes.Network;
using RDN.Mobile.Classes.Public;
using RDN.iOS.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.Mobile.Classes.League;

namespace RDN.iOS.Classes.League
{
    public class Forum
    {

        public static bool ReplyToTopic(AddForumTopicModel topic)
        {

            var status = Reachability.IsHostReachable(Reachability.HostName);
            if (status)
            {
                try
                {
                    return ForumMobile.ReplyToForumTopic(topic).IsSuccessful;

                }
                catch (Exception ex)
                {
                    ErrorHandler.Save(ex, MobileTypeEnum.iPhone);
                }
            }
            return false;

        }

        public static bool AddNewTopic(AddForumTopicModel topic)
        {

            var status = Reachability.IsHostReachable(Reachability.HostName);
            if (status)
            {
                try
                {
                    return ForumMobile.AddNewForumTopic(topic).IsSuccessful;

                }
                catch (Exception ex)
                {
                    ErrorHandler.Save(ex, MobileTypeEnum.iPhone);
                }
            }
            return false;

        }
        public static void PullForumTopics(string memberId, string userId, string type, long groupId, long categoryId, int page, int count, Action<ForumModel> callback)
        {
            Task<bool>.Factory.StartNew(
                                   () =>
                                   {
                                       try
                                       {
                                           var status = Reachability.IsHostReachable(Reachability.HostName);

                                           if (status)
                                           {
                                               try
                                               {
                                                   ForumMobile.PullForumTopics(memberId, userId, type, groupId.ToString(), categoryId.ToString(), page, count, callback);
                                               }
                                               catch (Exception ex)
                                               {
                                                   ErrorHandler.Save(ex, MobileTypeEnum.iPhone);
                                               }
                                           }
                                       }
                                       catch (Exception exception)
                                       {
                                           ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
                                       }
                                       return true;
                                   });

        }

        public static void PullForumTopic(string memberId, string userId, long topicId, Action<ForumTopicModel> callback)
        {
            Task<bool>.Factory.StartNew(
                                   () =>
                                   {
                                       try
                                       {
                                           var status = Reachability.IsHostReachable(Reachability.HostName);

                                           if (status)
                                           {
                                               try
                                               {
                                                   ForumMobile.PullForumTopic(memberId, userId, topicId.ToString(), callback);
                                               }
                                               catch (Exception ex)
                                               {
                                                   ErrorHandler.Save(ex, MobileTypeEnum.iPhone);
                                               }
                                           }
                                       }
                                       catch (Exception exception)
                                       {
                                           ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
                                       }
                                       return true;
                                   });

        }


    }
}