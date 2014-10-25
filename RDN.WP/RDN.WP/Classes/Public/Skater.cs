using Microsoft.Phone.Net.NetworkInformation;
using RDN.Portable.Config.Enums;
using RDN.Portable.Models.Json;
using RDN.Portable.Models.Json.Public;
using RDN.WP.Classes.Error;
using RDN.WP.Library.Classes.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.WP.Classes.Public
{
    public class Skater
    {

        public static void PullSkater(string memberId, Action<SkaterJson> callback)
        {
            Task<bool>.Factory.StartNew(
                                   () =>
                                   {
                                       try
                                       {
                                           if (NetworkInterface.GetIsNetworkAvailable())
                                           {
                                               try
                                               {
                                                   SkatersMobile.PullPublicSkater(memberId, callback);
                                               }
                                               catch (Exception ex)
                                               {
                                                   ErrorHandler.Save(ex, MobileTypeEnum.Android);
                                               }
                                           }
                                       }
                                       catch (Exception exception)
                                       {
                                           ErrorHandler.Save(exception, MobileTypeEnum.Android);
                                       }
                                       return true;
                                   });
        }

        public static void PullSkaters(int page, int count, string startsWith, Action<SkatersJson> callback)
        {
            Task<bool>.Factory.StartNew(
                                   () =>
                                   {
                                       try
                                       {
                                           if (NetworkInterface.GetIsNetworkAvailable())
                                           {
                                               try
                                               {
                                                   SkatersMobile.PullPublicSkaters(page, count, startsWith, callback);

                                               }
                                               catch (Exception ex)
                                               {
                                                   ErrorHandler.Save(ex, MobileTypeEnum.Android);
                                               }
                                           }


                                       }
                                       catch (Exception exception)
                                       {
                                           ErrorHandler.Save(exception, MobileTypeEnum.Android);
                                       }
                                       return true;
                                   });

        }
        public static void PullSkaters(string leagueId, Action<SkatersJson> callback)
        {
            Task<bool>.Factory.StartNew(
                                   () =>
                                   {
                                       try
                                       {
                                           if (NetworkInterface.GetIsNetworkAvailable())
                                           {
                                               try
                                               {
                                                   SkatersMobile.PullPublicSkatersByLeague(leagueId, callback);
                                               }
                                               catch (Exception ex)
                                               {
                                                   ErrorHandler.Save(ex, MobileTypeEnum.Android);
                                               }
                                           }
                                       }
                                       catch (Exception exception)
                                       {
                                           ErrorHandler.Save(exception, MobileTypeEnum.Android);
                                       }
                                       return true;
                                   });
        }

        public static void SearchSkaters(int page, int count, string searchString, Action<SkatersJson> callback)
        {
            Task<bool>.Factory.StartNew(
                                   () =>
                                   {
                                       try
                                       {
                                           if (NetworkInterface.GetIsNetworkAvailable())
                                           {
                                               try
                                               {
                                                   SkatersMobile.SearchPublicSkaters(page, count, searchString, callback);
                                               }
                                               catch (Exception ex)
                                               {
                                                   ErrorHandler.Save(ex, MobileTypeEnum.Android);
                                               }
                                           }
                                       }
                                       catch (Exception exception)
                                       {
                                           ErrorHandler.Save(exception, MobileTypeEnum.Android);
                                       }
                                       return true;
                                   });
        }
    }

}
