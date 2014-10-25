using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using RDN.Mobile.Classes.Utilities;
//using Newtonsoft.Json;
using RDN.Mobile.Database;
using RDN.Mobile.Database.PublicProfile;
using RDN.Utilities.Config;
using RDN.Portable.Config;
using RDN.Portable.Models.Json.Public;
using RDN.Portable.Models.Json;

namespace RDN.Mobile.Classes.Public
{
    public class SkatersMobile
    {

        public static void PullPublicSkaters(int page, int count, string startsWith, Action<SkatersJson> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                    var data = Utilities.Json.DeserializeObject<SkatersJson>(e.Result);
                    callback(data);
                }
                catch (Exception exception)
                {

                }
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.GET_SKATERS_BY_APLHA_URL + "p=" + page + "&c=" + count + "&sw=" + startsWith));
        }
        public static void PullPublicSkatersByLeague(string leagueId, Action<SkatersJson> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                    var data = Json.DeserializeObject<SkatersJson>(e.Result);
                    callback(data);
                    for (int i = 0; i < data.Skaters.Count; i++)
                    {
                        var fact = new SqlFactory();
                        var sk = fact.GetSkaterProfile(data.Skaters[i].MemberId);
                        if (sk == null)
                        {
                            fact.InsertSkaterProfile(new SqlSkaterProfile()
                              {
                                  Bio = data.Skaters[i].Bio,
                                  DerbyName = data.Skaters[i].DerbyName,
                                  DerbyNameUrl = data.Skaters[i].DerbyNameUrl,
                                  DerbyNumber = data.Skaters[i].DerbyNumber,
                                  DOB = data.Skaters[i].DOB,
                                  GamesCount = data.Skaters[i].GamesCount,
                                  Gender = data.Skaters[i].Gender,
                                  GotExtendedContent = data.Skaters[i].GotExtendedContent,
                                  MemberId = data.Skaters[i].MemberId,
                                  HeightFeet = data.Skaters[i].HeightFeet,
                                  HeightInches = data.Skaters[i].HeightInches,
                                  LeagueId = data.Skaters[i].LeagueId,
                                  LeagueLogo = data.Skaters[i].LeagueLogo,
                                  LeagueName = data.Skaters[i].LeagueName,
                                  LeagueUrl = data.Skaters[i].LeagueUrl,
                                  Losses = data.Skaters[i].Losses,
                                  photoUrl = data.Skaters[i].photoUrl,
                                  ThumbUrl = data.Skaters[i].ThumbUrl,
                                  Wins = data.Skaters[i].Wins
                              });
                        }
                    }
                    
                }
                catch (Exception exception)
                {

                }
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.GET_SKATERS_BY_LEAGUEID_URL + "lId=" + leagueId));
        }
        public static void PullPublicSkater(string memberId, Action<SkaterJson> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {

                    var data = Utilities.Json.DeserializeObject<SkaterJson>(e.Result);

                    callback(data);
                    //var data = JsonConvert.DeserializeObject<SkaterJson>(e.Result);
                    var fact = new SqlFactory().InsertSkaterProfile(new SqlSkaterProfile()
                    {
                        Bio = data.Bio,
                        DerbyName = data.DerbyName,
                        DerbyNameUrl = data.DerbyNameUrl,
                        DerbyNumber = data.DerbyNumber,
                        DOB = data.DOB,
                        GamesCount = data.GamesCount,
                        Gender = data.Gender,
                        GotExtendedContent = data.GotExtendedContent,
                        MemberId = data.MemberId,
                        HeightFeet = data.HeightFeet,
                        HeightInches = data.HeightInches,
                        LeagueId = data.LeagueId,
                        LeagueLogo = data.LeagueLogo,
                        LeagueName = data.LeagueName,
                        LeagueUrl = data.LeagueUrl,
                        Losses = data.Losses,
                        photoUrl = data.photoUrl,
                        ThumbUrl = data.ThumbUrl,
                        Wins = data.Wins
                    });
                }
                catch (Exception exception)
                {

                }
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.GET_SKATER_BY_MEMBERID_URL + "mid=" + memberId));


        }
        public static void SearchPublicSkaters(int page, int count, string searchString, Action<SkatersJson> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                    var data = Utilities.Json.DeserializeObject<SkatersJson>(e.Result);
                    //var data = JsonConvert.DeserializeObject<SkatersJson>(e.Result);

                    callback(data);
                }
                catch (Exception exception)
                {

                }
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.GET_SKATERS_BY_APLHA_URL + "p=" + page + "&c=" + count + "&s=" + searchString));


        }
    }
}
