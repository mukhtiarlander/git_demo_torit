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
using RDN.Portable.Models.Json.Public;
using RDN.Portable.Config.Enums;
using RDN.Portable.Models.Json;

namespace RDNation.Droid.Classes.Public
{
    public class Skater
    {
        public static void PullSkater(string memberId, Context context, Action<SkaterJson> callback)
        {
            Task<bool>.Factory.StartNew(
                                   () =>
                                   {
                                       try
                                       {
                                           var profile = new SqlFactory().GetSkaterProfile(memberId);
                                           if (profile != null && profile.GotExtendedContent)
                                           {
                                               callback(profile);
                                               return true;
                                           }
                                           var connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
                                           var activeConnection = connectivityManager.ActiveNetworkInfo;
                                           if ((activeConnection != null) && activeConnection.IsConnected)
                                           {
                                               try
                                               {
                                                   SkatersMobile.PullPublicSkater(memberId, callback);
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

        public static void PullSkaters(int page, int count, string startsWith, Context context, Action<SkatersJson> callback)
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
                                                   SkatersMobile.PullPublicSkaters(page, count, startsWith, callback);

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
        public static void PullSkaters(string leagueId, Context context, Action<SkatersJson> callback)
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
                                                   SkatersMobile.PullPublicSkatersByLeague(leagueId, callback);
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

        public static void SearchSkaters(int page, int count, string searchString, Context context, Action<SkatersJson> callback)
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
                                                   SkatersMobile.SearchPublicSkaters(page, count, searchString, callback);
                                               }
                                               catch (Exception ex)
                                               {
                                                   ErrorHandler.Save(ex,MobileTypeEnum.Android, context);
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