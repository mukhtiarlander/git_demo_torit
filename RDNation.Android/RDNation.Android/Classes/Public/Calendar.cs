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
using RDN.Mobile.Classes.Public;
using RDN.Portable.Models.Json.Calendar;
using RDN.Portable.Config.Enums;

namespace RDNation.Droid.Classes.Public
{
    public class Calendar
    {
        public static void PullEventsByLocation(int page, int count, double longitude, double latitude, Context context, Action<EventsJson> callback)
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
                                                   CalendarMobile.PullCurrentEventsByLocation(page, count, longitude, latitude, callback);
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
        public static void PullEvents(int page, int count, Context context, Action<EventsJson> callback)
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
                                                   CalendarMobile.PullCurrentEvents(page, count, callback);

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
        public static void SearchEvents(int page, int count, string s, Context context, Action<EventsJson> callback)
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
                                                   CalendarMobile.SearchCurrentEvents(page, count, s, callback);
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