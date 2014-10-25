using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using RDN.Portable.Config;
using RDN.Portable.Models.Json.Calendar;
using RDN.Portable.Classes.Utilities;

namespace RDN.WP.Library.Classes.Public
{
    public class ShopMobile
    {
        public static void PullShopItems(int page, int count, Action<ShopsJson> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                    var data = Json.DeserializeObject<ShopsJson>(e.Result);
                    //var data = JsonConvert.DeserializeObject<ShopsJson>(e.Result);
                    callback(data);
                }
                catch (Exception exception)
                {

                }
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.GET_SHOP_ITEMS_URL + "p=" + page + "&c=" + count));
        }
        public static void SearchShopItems(int page, int count, string search, Action<ShopsJson> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                    var data = Json.DeserializeObject<ShopsJson>(e.Result);
                    callback(data);
                }
                catch (Exception exception)
                {

                }
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.SEARCH_SHOP_ITEMS_URL + "p=" + page + "&c=" + count + "&s=" + search));
        }
    }
}
