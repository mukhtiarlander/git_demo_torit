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
    public class Shop
    {
        public static void PullShopItems(int page, int count, Context context, Action<ShopsJson> callback)
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
                                                   ShopMobile.PullShopItems(page, count, callback);

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
        public static void SearchShopItems(int page, int count, string s, Context context, Action<ShopsJson> callback)
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
                                                   ShopMobile.SearchShopItems(page, count, s, callback);
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