using RDN.iOS.Classes.Error;
using RDN.iOS.Classes.Network;
using RDN.Mobile.Classes.Public;
using RDN.Portable.Config.Enums;
using RDN.Portable.Models.Json.Calendar;
using RDN.Portable.Models.Json.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.iOS.Classes.Public
{

    public class League
    {
        public static void PullLeagues(int page, int count, string startsWith, Action<LeaguesJson> callback)
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
                                                   LeaguesMobile.PullPublicLeagues(page, count, startsWith, callback);

                                               }
                                               catch (Exception ex)
                                               {
                                                   ErrorHandler.Save(ex, MobileTypeEnum.iPhone);
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
        public static void PullLeague(string leagueId, Action<LeagueJsonDataTable> callback)
        {
            Task<bool>.Factory.StartNew(
                                   () =>
                                   {
                                       try
                                       {
                                           //var profile = new SqlFactory().GetLeagueProfile(leagueId);
                                           //if (profile != null && profile.GotExtendedContent)
                                           //{
                                           //    callback(profile);
                                           //    return true;
                                           //}
                                           var status = Reachability.IsHostReachable(Reachability.HostName);

                                           if (status)
                                           {
                                               try
                                               {
                                                   LeaguesMobile.PullPublicLeague(leagueId, callback);
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
        public static void PullLeagueEvents(string leagueId, Action<EventsJson> callback)
        {
            Task<bool>.Factory.StartNew(
                                   () =>
                                   {
                                       try
                                       {
                                           //var profile = new SqlFactory().GetCalendarEvents(leagueId);
                                           //if (profile != null && profile.Count >= 5)
                                           //{

                                           //    var evs = new EventsJson();

                                           //    evs.Events.AddRange(profile);

                                           //    callback(evs);
                                           //    return true;
                                           //}
                                           var status = Reachability.IsHostReachable(Reachability.HostName);

                                           if (status)
                                           {
                                               try
                                               {
                                                   LeaguesMobile.PullPublicLeagueEvents(leagueId, callback);
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


        public static void SearchLeagues(int page, int count, string searchString, Action<LeaguesJson> callback)
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
                                                   LeaguesMobile.SearchPublicLeagues(page, count, searchString, callback);
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
