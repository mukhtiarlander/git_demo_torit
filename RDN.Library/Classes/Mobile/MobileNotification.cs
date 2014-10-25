using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Messages;
using PushSharp.Android;
using RDN.Library.Cache;
using RDN.Mobile.Classes.Account;

using RDN.Portable.Config.Enums;
using RDN.Portable.Classes.Account;
using RDN.Portable.Settings.Enums;
using System.Net;
using RDN.Library.Classes.Error;

namespace RDN.Library.Classes.Mobile
{
    [Obsolete("Use MobileNotificationFactory")]
    public class MobileNotification
    {
        public long Id { get; set; }
        public string NotificationId { get; set; }
        public bool CanSendGameNotifications { get; set; }
        public Guid MemberId { get; set; }
        public Guid UserId { get; set; }
        public MobileTypeEnum MobileTypeEnum { get; set; }
        public bool IsSuccessful { get; set; }

        public static void SendNotifications(string header, string message, string url, MobileNotificationTypeEnum notificationType)
        {
            string json = "{\"head\":\"" + header + "\",\"message\":\"" + message + "\",\"nottype\":\"" + notificationType.ToString() + "\"}";
            //SendWindowsNotifications(url);
            SendAndroidNotifications(json);
        }
        private static void SendAndroidNotifications(string json)
        {
            PushSharpServer.Instance.Broker.RegisterService<GcmNotification>(new GcmPushService(new GcmPushChannelSettings("493035910025", "AIzaSyDDqTCHFCp0eoXIp5BXKhLi0YTG6Ez8aFQ", "com.rdnation.droid")));

            var n = new GcmNotification();
            n.CollapseKey = "NONE";
            n.JsonData = json;

            var not = SiteCache.GetMobileNotifications().Where(x => x.MobileTypeEnum == MobileTypeEnum.Android).ToList();
            for (int i = 0; i < not.Count; i++)
                PushSharpServer.Instance.Broker.QueueNotification(GcmNotification.ForSingleRegistrationId(n, not[i].NotificationId));
        }
  

       }
}
