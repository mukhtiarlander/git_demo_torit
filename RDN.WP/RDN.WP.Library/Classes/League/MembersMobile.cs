using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
//using RDN.Mobile.Classes.Utilities;
////using Newtonsoft.Json;
//using RDN.Mobile.Database;
//using RDN.Mobile.Database.PublicProfile;
//using RDN.Utilities.Config;
using RDN.Portable.Config;
using RDN.Portable.Models.Json.Public;
using RDN.Portable.Models.Json;
using RDN.Portable.Classes.Controls.Forum;
using RDN.Portable.Network;
using System.IO;
using RDN.Portable.Classes.Utilities;
using RDN.Portable.Classes.Account.Classes;

namespace RDN.WP.Library.Classes.League
{
    public class MembersMobile
    {

    
        public static void PullLeagueMembers(string memId, string uid, Action<List<MemberDisplayAPI>> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                var data = RDN.Portable.Classes.Utilities.Json.DeserializeObject<List<MemberDisplayAPI>>(e.Result);
                callback(data);
            };
            Random random = new Random();
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.LEAGUE_MEMBERS_URL+ "mid=" + memId.ToString() + "&uid=" + uid.ToString() + "&r=" + random.Next()));
        }
    }
}
