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
using RDN.Mobile.Database;
using RDN.Portable.Models.Json.Games;
using RDN.Portable.Config.Enums;

namespace RDNation.Droid.Classes.Public
{
    public class Game
    {
        public static void PullCurrentGames(Context context, Action<GamesJson> callback)
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
                                                   GamesMobile.PullCurrentGames(callback);
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
        public static void PullPastGames(int count, int page,Context context, Action<GamesJson> callback)
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
                                                   GamesMobile.PullPastGames(page, count,callback);
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