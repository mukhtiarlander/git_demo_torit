using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Threading.Tasks;
using RDN.Portable.Models.Json.Games;
using RDN.Mobile.Classes.Public;
using RDN.iOS.Classes.Network;
using RDN.iOS.Classes.Error;
using RDN.Portable.Config.Enums;

namespace RDN.iOS.Classes.Public
{
    public class Game
    {
        public static void PullCurrentGames(Action<GamesJson> callback)
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
                                                   GamesMobile.PullCurrentGames(callback);
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
        public static void PullPastGames(int count, int page, Action<GamesJson> callback)
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
                                                   GamesMobile.PullPastGames(page, count, callback);
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