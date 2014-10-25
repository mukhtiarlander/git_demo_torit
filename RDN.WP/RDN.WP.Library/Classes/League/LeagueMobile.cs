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
using RDN.Portable.Classes.League;
using RDN.Portable.Classes.League.Classes;
using RDN.Portable.Classes.Colors;
using RDN.Portable.Classes.Location;

namespace RDN.WP.Library.Classes.League
{
    public class LeagueMobile
    {
        public static LeagueStartModel GetMyLeague(Guid memId, Guid uid)
        {
            Random r = new Random();
            var response = Network.SendPackage(Network.ConvertObjectToStream(new MemberPassParams() { Mid = memId, Uid = uid }), MobileConfig.LEAGUE_INITIAL_LOAD_URL + "?r=" + r.Next());
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<LeagueStartModel>(json);
        }
        public static List<Location> GetLocations(Guid memId, Guid uid, Guid calendarId)
        {
            Random r = new Random();
            var response = Network.SendPackage(Network.ConvertObjectToStream(new MemberPassParams() { Mid = memId, Uid = uid, IdOfAnySort2 = calendarId }), MobileConfig.LEAGUE_LOCATIONS_URL+ "?r=" + r.Next());
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<List<Location>>(json);
        }
        public static List<ColorDisplay> GetLeagueColors(Guid memId, Guid uid)
        {
            Random r = new Random();
            var response = Network.SendPackage(Network.ConvertObjectToStream(new MemberPassParams() { Mid = memId, Uid = uid }), MobileConfig.LEAGUE_COLORS_URL+"?r="+r.Next());
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<List<ColorDisplay>>(json);
        }
        public static LeagueBase GetLeagueEdit(Guid memId, Guid uid)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new MemberPassParams() { Mid = memId, Uid = uid }), MobileConfig.LEAGUE_EDIT_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<LeagueBase>(json);
        }
        public static LeagueBase SaveLeagueEdit(Guid memId, Guid uid, LeagueBase league)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new MemberPassParams() { Mid = memId, Uid = uid, Item = league }), MobileConfig.LEAGUE_EDIT_SAVE_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<LeagueBase>(json);
        }
        public static void PullLeagueMembers(string memId, string uid, Action<List<MemberDisplayBasic>> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                var data = RDN.Portable.Classes.Utilities.Json.DeserializeObject<List<MemberDisplayBasic>>(e.Result);
                callback(data);
            };
            Random random = new Random();
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.LEAGUE_MEMBERS_BASIC_URL+ "mid=" + memId.ToString() + "&uid=" + uid.ToString() + "&r=" + random.Next()));
        }
        public static void PullLeagueGroups(string memId, string uid, Action<List<LeagueGroupBasic>> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                var data = RDN.Portable.Classes.Utilities.Json.DeserializeObject<List<LeagueGroupBasic>>(e.Result);
                callback(data);
            };
            Random random = new Random();
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.LEAGUE_GROUPS_URL+ "mid=" + memId.ToString() + "&uid=" + uid.ToString() + "&r=" + random.Next()));
        }
    }
}
