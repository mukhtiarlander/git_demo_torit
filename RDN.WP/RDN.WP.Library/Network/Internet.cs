using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.NetworkInformation;

namespace RDN.Utilities.Network
{
    public class Internet
    {
        public static bool CheckConnection()
        {
            Ping ping = new Ping();
            try
            {
                PingReply pingStatus = ping.Send(RDN.Utilities.Config.ServerConfig.WEBSITE_PING_LOCATION);
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
