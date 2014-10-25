using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using RDN.Portable.Config;
using RDN.Portable.Config.Enums;
using RDN.Portable.Classes.Account;
using RDN.Portable.Classes.Utilities;
using RDN.Portable.Network;

namespace RDN.WP.Library.Classes.Account
{

    [DataContract]
    public class NotificationMobileJsonWP : NotificationMobileJson
    {

        public static NotificationMobileJson SendNotificationId(NotificationMobileJson notification)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(notification), MobileConfig.SEND_NOTIFICATION_CREATE);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<NotificationMobileJson>(json);
        }
    }
}
