using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using RDN.Mobile.Classes.Utilities;
using RDN.Portable.Config;
using RDN.Portable.Classes.Account;
using RDN.Portable.Network;

namespace RDN.Mobile.Classes.Account
{
    [DataContract]
    public class NotificationMobileJsonMb : NotificationMobileJson
    {
        public static NotificationMobileJson SendNotificationId(NotificationMobileJson notification)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(notification), MobileConfig.SEND_NOTIFICATION_CREATE_DEBUG_ANDROID);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<NotificationMobileJson>(json);
        }
    }
}
