using RDN.Library.Cache;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Mobile.Enums;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Messages;
using RDN.Portable.Classes.Account;
using RDN.Portable.Config.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace RDN.Library.Classes.Mobile
{
    public class MobileNotificationFactory
    {
        string titleOfMessage { get; set; }
        string descriptionOfMessage { get; set; }
        List<NotificationMobileJson> notificationsInternal { get; set; }
        NotificationTypeEnum notifyType { get; set; }
        long Id { get; set; }
        Guid bigId { get; set; }
        Guid bigId2 { get; set; }
        string bigString { get; set; }
        public MobileNotificationFactory Initialize(string title, string description, NotificationTypeEnum type)
        {
            try
            {
                titleOfMessage = title;
                descriptionOfMessage = description;
                notifyType = type;
                notificationsInternal = new List<NotificationMobileJson>();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return this;
        }

        public MobileNotificationFactory AddId(long topicId)
        {
            Id = topicId;
            return this;
        }
        public MobileNotificationFactory AddId(Guid id)
        {
            bigId = id;
            return this;
        }
        public MobileNotificationFactory AddId2(Guid id)
        {
            bigId2 = id;
            return this;
        }
        public MobileNotificationFactory AddCalendarEvent(Guid itemId, Guid calId, string name)
        {
            bigId = itemId;
            bigId2 = calId;
            bigString = name;
            return this;
        }
        public MobileNotificationFactory AddString(string id)
        {
            bigString = id;
            return this;
        }
        public MobileNotificationFactory AddMember(Guid memberId)
        {
            try
            {
                var notify = SiteCache.GetMobileNotifications();

                var not = notify.Where(x => x.MemberId == memberId.ToString()).FirstOrDefault();
                if (not != null)
                    notificationsInternal.Add(not);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return this;
        }
        public MobileNotificationFactory AddMembers(List<Guid> members)
        {
            try
            {
                var notify = SiteCache.GetMobileNotifications();
                for (int i = 0; i < members.Count; i++)
                {
                    var not = notify.Where(x => x.MemberId == members[i].ToString());
                    if (not != null && not.Count() > 0)
                        notificationsInternal.AddRange(not);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return this;
        }
        public MobileNotificationFactory AddNotifications(List<NotificationMobileJson> notifications)
        {
            notificationsInternal = notifications;
            return this;
        }

        public MobileNotificationFactory SendNotifications()
        {
            try
            {
                for (int i = 0; i < notificationsInternal.Count; i++)
                {
                    switch (notificationsInternal[i].MobileTypeEnum)
                    {
                        case Portable.Config.Enums.MobileTypeEnum.WP8:
                            switch (notifyType)
                            {
                                case NotificationTypeEnum.Forum:
                                    SendWindowsPhoneForumNotification(notificationsInternal[i].NotificationId, Id);
                                    break;
                                case NotificationTypeEnum.Message:
                                    SendWindowsPhoneMessageNotification(notificationsInternal[i].NotificationId, Id);
                                    break;
                                case NotificationTypeEnum.NewPollCreated:
                                    SendWindowsPhoneMessageNotification(notificationsInternal[i].NotificationId, Id);
                                    break;
                                case NotificationTypeEnum.DuesPaymentReceipt:
                                    SendWindowsPhoneDuesReceiptNotification(notificationsInternal[i].NotificationId);
                                    break;
                                case NotificationTypeEnum.NewCalendarEventBroadcast:
                                    SendWindowsPhoneNewCalendarEventNotification(notificationsInternal[i].NotificationId);
                                    break;
                            }
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, new Exception().GetType());
            }
            return this;
        }
        private void SendWindowsPhoneForumNotification(string uri, long topicId)
        {
            try
            {
                // Get the URI that the Microsoft Push Notification Service returns to the push client when creating a notification channel.
                // Normally, a web service would listen for URIs coming from the web client and maintain a list of URIs to send
                // notifications out to.
                string subscriptionUri = uri;
                string toastMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<wp:Notification xmlns:wp=\"WPNotification\">" +
               "<wp:Toast>" +
                    "<wp:Text1>" + titleOfMessage + "</wp:Text1>" +
                    "<wp:Text2>" + descriptionOfMessage + "</wp:Text2>" +
                    "<wp:Param>/View/MyLeague/Forum/ViewForumTopic.xaml?tid=" + topicId + "</wp:Param>" +
               "</wp:Toast> " +
            "</wp:Notification>";
                SendWindowsPhoneNotifications(uri, toastMessage);
            }
            catch (Exception ex)
            {
                ErrorDatabaseManager.AddException(ex, new Exception().GetType());
            }
        }
        private void SendWindowsPhoneDuesReceiptNotification(string uri)
        {
            try
            {
                string subscriptionUri = uri;
                string toastMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<wp:Notification xmlns:wp=\"WPNotification\">" +
               "<wp:Toast>" +
                    "<wp:Text1>" + titleOfMessage + "</wp:Text1>" +
                    "<wp:Text2>" + descriptionOfMessage + "</wp:Text2>" +
                    "<wp:Param>/View/MyLeague/Dues/DuesReceipt.xaml?ivId=" + bigId + "</wp:Param>" +
               "</wp:Toast> " +
            "</wp:Notification>";
                SendWindowsPhoneNotifications(uri, toastMessage);
            }
            catch (Exception ex)
            {
                ErrorDatabaseManager.AddException(ex, new Exception().GetType());
            }
        }

        private void SendWindowsPhoneNewCalendarEventNotification(string uri)
        {
            try
            {
                string subscriptionUri = uri;
                string toastMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<wp:Notification xmlns:wp=\"WPNotification\">" +
               "<wp:Toast>" +
                    "<wp:Text1>" + titleOfMessage + "</wp:Text1>" +
                    "<wp:Text2>" + descriptionOfMessage + "</wp:Text2>" +
                    "<wp:Param>/View/MyLeague/Calendar/ViewCalendarEvent.xaml?evId=" + bigId + "&calId=" + bigId2 + "&name=" + bigString + "</wp:Param>" +
               "</wp:Toast> " +
            "</wp:Notification>";


                SendWindowsPhoneNotifications(uri, toastMessage);
            }
            catch (Exception ex)
            {
                ErrorDatabaseManager.AddException(ex, new Exception().GetType());
            }
        }
        private void SendWindowsPhoneMessageNotification(string uri, long topicId)
        {
            try
            {
                // Get the URI that the Microsoft Push Notification Service returns to the push client when creating a notification channel.
                // Normally, a web service would listen for URIs coming from the web client and maintain a list of URIs to send
                // notifications out to.
                string subscriptionUri = uri;
                string toastMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<wp:Notification xmlns:wp=\"WPNotification\">" +
               "<wp:Toast>" +
                    "<wp:Text1>" + titleOfMessage + "</wp:Text1>" +
                    "<wp:Text2>" + descriptionOfMessage + "</wp:Text2>" +
                    "<wp:Param>/View/MyLeague/Messages/MessagesView.xaml?mid=" + topicId + "</wp:Param>" +
               "</wp:Toast> " +
            "</wp:Notification>";
                SendWindowsPhoneNotifications(uri, toastMessage);
            }
            catch (Exception ex)
            {
                ErrorDatabaseManager.AddException(ex, new Exception().GetType());
            }
        }


        private static void SendWindowsPhoneNotifications(string uri, string toastMessage)
        {

            try
            {

                // Get the URI that the Microsoft Push Notification Service returns to the push client when creating a notification channel.
                // Normally, a web service would listen for URIs coming from the web client and maintain a list of URIs to send
                // notifications out to.
                string subscriptionUri = uri;


                HttpWebRequest sendNotificationRequest = (HttpWebRequest)WebRequest.Create(subscriptionUri);

                // Create an HTTPWebRequest that posts the toast notification to the Microsoft Push Notification Service.
                // HTTP POST is the only method allowed to send the notification.
                sendNotificationRequest.Method = "POST";

                // The optional custom header X-MessageID uniquely identifies a notification message. 
                // If it is present, the same value is returned in the notification response. It must be a string that contains a UUID.
                // sendNotificationRequest.Headers.Add("X-MessageID", "<UUID>");

                // Set the notification payload to send.
                byte[] notificationMessage = Encoding.Default.GetBytes(toastMessage);

                // Set the web request content length.
                sendNotificationRequest.ContentLength = notificationMessage.Length;
                sendNotificationRequest.ContentType = "text/xml";
                sendNotificationRequest.Headers.Add("X-WindowsPhone-Target", "toast");
                sendNotificationRequest.Headers.Add("X-NotificationClass", "2");


                using (Stream requestStream = sendNotificationRequest.GetRequestStream())
                {
                    requestStream.Write(notificationMessage, 0, notificationMessage.Length);
                }

                // Send the notification and get the response.
                HttpWebResponse response = (HttpWebResponse)sendNotificationRequest.GetResponse();
                string notificationStatus = response.Headers["X-NotificationStatus"];
                string notificationChannelStatus = response.Headers["X-SubscriptionStatus"];
                string deviceConnectionStatus = response.Headers["X-DeviceConnectionStatus"];

                // Display the response from the Microsoft Push Notification Service.  
                // Normally, error handling code would be here. In the real world, because data connections are not always available,
                // notifications may need to be throttled back if the device cannot be reached.
                //ErrorDatabaseManager.AddException(new Exception(notificationStatus + " | " + deviceConnectionStatus + " | " + notificationChannelStatus), new Exception().GetType());
            }
            catch (Exception ex)
            {
                ErrorDatabaseManager.AddException(ex, new Exception().GetType(), additionalInformation: uri);
            }
        }

        public static List<NotificationMobileJson> GetAllMobileNotificationSettings()
        {
            List<NotificationMobileJson> nots = new List<NotificationMobileJson>();
            var dc = new ManagementContext();
            var settings = dc.MobileSettings.Include("Member");
            foreach (var m in settings)
            {
                NotificationMobileJson n = new NotificationMobileJson();
                n.CanSendGameNotifications = m.AllowGameNotifications;
                //n.CanSendForumNotifications = m.CanSendForumNotifications;
                n.Id = m.Id;
                if (m.Member != null)
                {
                    n.MemberId = m.Member.MemberId.ToString();
                }
                n.NotificationId = m.NotificationId;
                n.MobileTypeEnum = (MobileTypeEnum)m.MobileTypeEnum;
                nots.Add(n);
            }
            return nots;
        }
        public static bool AddMobileNotification(NotificationMobileJson notification)
        {

            var dc = new ManagementContext();

            MobileNotificationSettings s = new MobileNotificationSettings();
            s.AllowGameNotifications = notification.CanSendGameNotifications;
            //s.CanSendForumNotifications = notification.CanSendForumNotifications;
            if (!String.IsNullOrEmpty(notification.MemberId))
                s.Member = dc.Members.Where(x => x.MemberId == new Guid(notification.MemberId)).FirstOrDefault();
            s.MobileTypeEnum = (byte)notification.MobileTypeEnum;
            s.NotificationId = notification.NotificationId;
            dc.MobileSettings.Add(s);
            int c = dc.SaveChanges();
            return true;
        }
        public static bool DeleteMobileNotification(NotificationMobileJson notification)
        {
            var dc = new ManagementContext();
            var n = dc.MobileSettings.Where(x => x.NotificationId == notification.NotificationId).FirstOrDefault();
            if (n != null)
            {
                dc.MobileSettings.Remove(n);
            }
            int c = dc.SaveChanges();
            return c > 0;
        }
        public static bool UpdateMobileNotificationForMember(string notificationId, Guid memberId)
        {
            List<MobileNotification> nots = new List<MobileNotification>();
            var dc = new ManagementContext();
            var n = dc.MobileSettings.Where(x => x.NotificationId == notificationId).FirstOrDefault();
            if (n != null)
            {
                if (memberId == new Guid())
                    n.Member = null;
                else
                    n.Member = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                dc.SaveChanges();
            }
            return true;
        }


    }
}
