using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using RDN.Portable.Config;
using RDN.Portable.Models.Json.Games;
using RDN.Portable.Classes.Utilities;

namespace RDN.WP.Library.Classes.Public
{
    public class GamesMobile
    {

        public static void PullCurrentGames(Action<GamesJson> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                    var data = Json.DeserializeObject<GamesJson>(e.Result);
                    callback(data);
                }
                catch (Exception exception)
                {

                }
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.GET_CURRENT_GAMES_MOBILE_URL));
        }
        public static void PullPastGames(int page, int count, Action<GamesJson> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                    var data = Json.DeserializeObject<GamesJson>(e.Result);
                    callback(data);
                }
                catch (Exception exception)
                {

                }
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.GET_PAST_GAMES_MOBILE_URL + "p=" + page + "&c=" + count));
        }


    }
}
