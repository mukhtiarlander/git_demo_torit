using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
//using RDN.Mobile.Classes.Utilities;
////using Newtonsoft.Json;
//using RDN.Mobile.Database;
//using RDN.Mobile.Database.PublicProfile;
//using RDN.Utilities.Config;
using RDN.Portable.Config;
using RDN.Portable.Models.Json.Public;
using RDN.Portable.Models.Json;
using RDN.Portable.Classes.Controls.Forum;
using RDN.Portable.Network;
using System.IO;
using RDN.Portable.Classes.Utilities;
using RDN.Portable.Classes.Controls.Message.Enums;
using RDN.Portable.Classes.Controls.Message;

namespace RDN.WP.Library.Classes.League
{
    public class MessagesMobile
    {
        public static ConversationModel SendNewMessage(ConversationModel message)
        {
            HttpWebResponse response = null;
            if (message.MessageTypeEnum == MessageTypeEnum.Regular)
                response = Network.SendPackage(Network.ConvertObjectToStream(message), MobileConfig.MEMBER_MESSAGES_NEW_URL);
            else if (message.MessageTypeEnum == MessageTypeEnum.Text)
                response = Network.SendPackage(Network.ConvertObjectToStream(message), MobileConfig.MEMBER_MESSAGES_NEW_TEXT_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<ConversationModel>(json);
        }
        public static ConversationModel ReplyToMessage(ConversationModel message)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(message), MobileConfig.MEMBER_MESSAGES_REPLY_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<ConversationModel>(json);
        }

        public static void PullMessages(string memId, string uid, int page, int count, GroupOwnerTypeEnum type, Action<MessageModel> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                    var data = RDN.Portable.Classes.Utilities.Json.DeserializeObject<MessageModel>(e.Result);
                    callback(data);
                }
                catch (Exception exception)
                {

                }
            };
            Random random = new Random();
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.MEMBER_MESSAGES_URL + "mid=" + memId.ToString() + "&t=" + type + "&uid=" + uid.ToString() + "&p=" + page + "&c=" + count + "&r=" + random.Next()));
        }

        public static void PullMessageTopic(string memId, string uid, string groupId, Action<ConversationModel> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                    var data = RDN.Portable.Classes.Utilities.Json.DeserializeObject<ConversationModel>(e.Result);
                    callback(data);
                }
                catch
                {

                }
            };
            Random random = new Random();
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.MEMBER_MESSAGES_VIEW_URL + "mid=" + memId.ToString() + "&uid=" + uid.ToString() + "&gid=" + groupId + "&r=" + random.Next()));
        }
    }
}
