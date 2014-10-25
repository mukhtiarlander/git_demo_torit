using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using RDN.Portable.Config;
using RDN.Portable.Models.Json.Public;
using RDN.WP.Library.Database;
using RDN.Portable.Classes.Utilities;
using RDN.Portable.Models.Json.Calendar;
using RDN.WP.Library.Database.PublicProfile;
using RDN.WP.Library.Database.Calendar;

namespace RDN.WP.Library.Classes.Public
{
    public class LeaguesMobile
    {

        public static void PullPublicLeagues(int page, int count, string startsWith, Action<LeaguesJson> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                    var ms = new MemoryStream(Encoding.Unicode.GetBytes(e.Result));
                    var ser = new DataContractJsonSerializer(typeof(LeaguesJson));
                    LeaguesJson data = ser.ReadObject(ms) as LeaguesJson;
                    ms.Close();
                    callback(data);
                }
                catch (Exception exception)
                {

                }
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.GET_LEAGUES_BY_APLHA_URL + "p=" + page + "&c=" + count + "&sw=" + startsWith));


        }

        public static void PullPublicLeague(string leagueId, Action<LeagueJsonDataTable> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                    var data = Json.DeserializeObject<LeagueJsonDataTable>(e.Result);
                    callback(data);
                    //var fact = new SqlFactory().InsertLeagueProfile(new SqlLeagueProfile()
                    //{
                    //    Country = data.Country,
                    //    LogoUrl = data.LogoUrl,
                    //    State = data.State,
                    //    LeagueId = data.LeagueId,
                    //    LeagueName = data.LeagueName,
                    //    LeagueUrl = data.LeagueUrl
                    //});
                    
                }
                catch (Exception exception)
                {

                }
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.GET_LEAGUE_BY_LEAGUEID_URL + "lid=" + leagueId));


        }
        public static void PullPublicLeagueEvents(string leagueId, Action<EventsJson> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                  var data=  Json.DeserializeObject<EventsJson>(e.Result);
                  callback(data);
                    foreach (var ev in data.Events)
                    {
                        var sql = new SqlFactory();
                        var temp = sql.GetCalendarEvent(leagueId, ev.CalendarItemId);
                        if (temp == null)
                        {
                            sql.InsertCalendarEvent(new SqlCalendarEvent()
                               {
                                   Address = ev.Address,
                                   CalendarItemId = ev.CalendarItemId,
                                   EndDate = ev.EndDate,
                                   Location = ev.Location,
                                   Name = ev.Name,
                                   NameUrl = ev.NameUrl,
                                   StartDate = ev.StartDate,
                                   LeagueId = ev.LeagueId.ToString().Replace("-", "")
                               });
                        }
                    }
                    
                }
                catch (Exception exception)
                {

                }
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.GET_LEAGUE_EVENTS_BY_LEAGUEID_URL + "lid=" + leagueId));


        }
        public static void SearchPublicLeagues(int page, int count, string searchString, Action<LeaguesJson> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                    var data = Json.DeserializeObject<LeaguesJson>(e.Result);
                    callback(data);
                }
                catch (Exception exception)
                {

                }
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.GET_LEAGUES_BY_APLHA_URL + "p=" + page + "&c=" + count + "&s=" + searchString));


        }
    }
}
