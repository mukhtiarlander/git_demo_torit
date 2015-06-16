using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.NetworkInformation;
using RDN.Portable.Config;

namespace Scoreboard.Library.Network
{
    public class Internet
    {
        public static bool CheckConnection()
        {
            Ping ping = new Ping();
            try
            {
                PingReply pingStatus = ping.Send(ScoreboardConfig.WEBSITE_PING_LOCATION);
                return pingStatus.Status == IPStatus.Success;
            }
            catch
            {

            }
            return false;

        }
        public static bool checkApi()
        {
            WebClient client = new WebClient();
            try
            {
                var st = client.DownloadString("https://api.rdnation.com/Scoreboard/isonline");
                if (st.Contains("true"))
                    return true;
                else
                    return false;
            }
            catch (WebException)
            {
                return false;
            }
        }
    }
}
