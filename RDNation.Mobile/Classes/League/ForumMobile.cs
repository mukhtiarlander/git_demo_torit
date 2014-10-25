using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using RDN.Mobile.Classes.Utilities;
//using Newtonsoft.Json;
using RDN.Mobile.Database;
using RDN.Mobile.Database.PublicProfile;
using RDN.Utilities.Config;
using RDN.Portable.Config;
using RDN.Portable.Models.Json.Public;
using RDN.Portable.Models.Json;
using RDN.Portable.Classes.Controls.Forum;
using RDN.Portable.Network;
using System.IO;

namespace RDN.Mobile.Classes.League
{
    public class ForumMobile
    {
        public static AddForumTopicModel AddNewForumTopic(AddForumTopicModel topic)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(topic), MobileConfig.LEAGUE_FORUM_ADD_TOPIC_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<AddForumTopicModel>(json);
        }
        public static AddForumTopicModel ReplyToForumTopic(AddForumTopicModel topic)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(topic), MobileConfig.LEAGUE_FORUM_REPLY_TOPIC_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<AddForumTopicModel>(json);
        }
     
        public static void PullForumTopics(string memId, string uid, string type, string groupId, string categoryId, int page, int count, Action<ForumModel> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                var data = Utilities.Json.DeserializeObject<ForumModel>(e.Result);
                callback(data);
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.LEAGUE_FORUM_TOPICS_URL + "mid=" + memId.ToString() + "&t=" + type + "&uid=" + uid.ToString() + "&gid=" + groupId + "&cid=" + categoryId + "&p=" + page + "&c=" + count));
        }
        public static void PullForumTopic(string memId, string uid, string topicId, Action<ForumTopicModel> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                var data = Utilities.Json.DeserializeObject<ForumTopicModel>(e.Result);
                callback(data);
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.LEAGUE_FORUM_TOPIC_URL + "mid=" + memId.ToString() + "&uid=" + uid.ToString() + "&tid=" + topicId));
        }
    }
}
