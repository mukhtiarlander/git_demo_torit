using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RDN.Mobile.Classes.Account;
using RDN.Portable.Classes.Account;
using RDN.Portable.Config.Enums;

namespace RDNation.Droid.Classes.Account
{
    public class MobileNotification
    {
        public static void SendNotificationId(string memberId, string notificationId,Context context)
        {
            Task<bool>.Factory.StartNew(
                                   () =>
                                   {
                                       try
                                       {
                                           var connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
                                           var activeConnection = connectivityManager.ActiveNetworkInfo;
                                           if ((activeConnection != null) && activeConnection.IsConnected)
                                           {
                                               try
                                               {
                                                   NotificationMobileJson not = new NotificationMobileJson();
                                                   not.CanSendGameNotifications = true;
                                                   not.MemberId = memberId;
                                                   not.MobileTypeEnum = MobileTypeEnum.Android;
                                                   not.NotificationId = notificationId;
                                                   NotificationMobileJsonMb.SendNotificationId(not);
                                               }
                                               catch (Exception ex)
                                               {
                                                   ErrorHandler.Save(ex, MobileTypeEnum.Android, context);
                                               }
                                           }


                                       }
                                       catch (Exception exception)
                                       {
                                           ErrorHandler.Save(exception, MobileTypeEnum.Android, context);
                                       }
                                       return true;
                                   });

        }
    }
}