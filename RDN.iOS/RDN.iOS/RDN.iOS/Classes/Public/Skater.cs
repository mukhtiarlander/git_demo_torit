using RDN.iOS.Classes.Error;
using RDN.iOS.Classes.Network;
using RDN.Mobile.Classes.Public;
using RDN.Portable.Config.Enums;
using RDN.Portable.Models.Json;
using RDN.Portable.Models.Json.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.iOS.Classes.Public
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
                                           var status = Reachability.IsHostReachable(Reachability.HostName);

                                           if (status)
                                           {
                                               try
                                               {
                                                   SkatersMobile.PullPublicSkater(memberId, callback);
                                               }
                                               catch (Exception ex)
                                               {
                                                   ErrorHandler.Save(ex, MobileTypeEnum.iPhone);
                                               }
                                           }
                                       }
                                       catch (Exception exception)
                                       {
                                           ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
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


                                           var status = Reachability.IsHostReachable(Reachability.HostName);

                                           if (status)
                                           {
                                               try
                                               {
                                                   SkatersMobile.PullPublicSkaters(page, count, startsWith, callback);

                                               }
                                               catch (Exception ex)
                                               {
                                                   ErrorHandler.Save(ex, MobileTypeEnum.iPhone);
                                               }
                                           }


                                       }
                                       catch (Exception exception)
                                       {
                                           ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
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
                                           var status = Reachability.IsHostReachable(Reachability.HostName);

                                           if (status)
                                           {
                                               try
                                               {
                                                   SkatersMobile.PullPublicSkatersByLeague(leagueId, callback);
                                               }
                                               catch (Exception ex)
                                               {
                                                   ErrorHandler.Save(ex, MobileTypeEnum.iPhone);
                                               }
                                           }
                                       }
                                       catch (Exception exception)
                                       {
                                           ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
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
                                           var status = Reachability.IsHostReachable(Reachability.HostName);

                                           if (status)
                                           {
                                               try
                                               {
                                                   SkatersMobile.SearchPublicSkaters(page, count, searchString, callback);
                                               }
                                               catch (Exception ex)
                                               {
                                                   ErrorHandler.Save(ex, MobileTypeEnum.iPhone);
                                               }
                                           }


                                       }
                                       catch (Exception exception)
                                       {
                                           ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
                                       }
                                       return true;
                                   });

        }
    }
}
