using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using RDN.Mobile.Classes.Utilities;
using RDN.Utilities.Config;
using RDN.Portable.Config;
using RDN.Portable.Models.Json.Calendar;

namespace RDN.Mobile.Classes.Public
{
    public class CalendarMobile
    {
        public static void PullCurrentEventsByLocation(int page, int count, double longitude, double latitude, Action<EventsJson> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                    var data = Json.DeserializeObject<EventsJson>(e.Result);
                    callback(data);
                }
                catch (Exception exception)
                {

                }
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.SEARCH_CALENDAR_EVENTS_BY_LL_URL + "p=" + page + "&c=" + count +"&lat="+latitude+"&lon="+longitude));
        }
        public static void PullCurrentEvents(int page, int count, Action<EventsJson> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                    var data = Json.DeserializeObject<EventsJson>(e.Result);
                    callback(data);
                }
                catch (Exception exception)
                {

                }
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.GET_CALENDAR_EVENTS_URL + "p=" + page + "&c=" + count));
        }
        public static void SearchCurrentEvents(int page, int count, string s, Action<EventsJson> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                    var data = Json.DeserializeObject<EventsJson>(e.Result);
                    callback(data);
                }
                catch (Exception exception)
                {

                }
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.SEARCH_CALENDAR_EVENTS_URL + "p=" + page + "&c=" + count + "&s=" + s));
        }
    }
}
