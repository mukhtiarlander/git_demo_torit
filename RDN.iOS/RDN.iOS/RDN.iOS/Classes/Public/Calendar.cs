using RDN.iOS.Classes.Error;
using RDN.iOS.Classes.Network;
using RDN.Mobile.Classes.Public;
using RDN.Portable.Config.Enums;
using RDN.Portable.Models.Json.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.iOS.Classes.Public
{
    public class Calendar
    {
        public static void PullEventsByLocation(int page, int count, double longitude, double latitude, Action<EventsJson> callback)
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
                                                   CalendarMobile.PullCurrentEventsByLocation(page, count, longitude, latitude, callback);
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
        public static void PullEvents(int page, int count, Action<EventsJson> callback)
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
                                                   CalendarMobile.PullCurrentEvents(page, count, callback);

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
        public static void SearchEvents(int page, int count, string s, Action<EventsJson> callback)
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
                                                   CalendarMobile.SearchCurrentEvents(page, count, s, callback);
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
