using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Account.Classes;
using System.Text.RegularExpressions;
using System.Net;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Team;
using RDN.Portable.Classes.Account.Classes;

namespace RDN.Library.Classes.Utilities
{
    public class DerbyRosterWebsite
    {
        /// <summary>
        /// scans derby roster.com for us for the shadow leagues.
        /// </summary>
        public static void ScanDerbyRosterWebsite()
        {

            //<tr class="(trc1|trc2)"><td>(&nbsp;|[a-zA-Z0-9\s�\-\.\'\%\(\),!"#$*/\?:&\+!_=~@\[\]`]+)</td><td>(&nbsp;|[0-9a-zA-Z\s\.\/\-%#:'�$"\+&/\\!_@\>=\?,\)\(\*\^;\]\[~]+(&cent;)?|&times;)</td><td>([0-9\-]+|&nbsp;)</td><td>(&nbsp;|[a-zA-Z\s\(\)0-9/\.':,\-�!_\*\?]+)</td></tr>

            //name: (&nbsp;|[a-zA-Z0-9\s�\-\.\'\%\(\),!"#$*/\?:&\+!_=~@\[\]`]+)
            //number: (&nbsp;|[0-9a-zA-Z\s\.\/\-%#:'�$"\+&/\\!_@\>=\?,\)\(\*\^;\]\[~]+(&cent;)?|&times;)
            // date: ([0-9\-]+|&nbsp;)
            //league: (&nbsp;|[a-zA-Z\s\(\)0-9/\.':,\-�!_\*\?]+)
            List<ItemInMatches> countries = new List<ItemInMatches>();
            List<ItemInMatches> states = new List<ItemInMatches>();
            List<MemberDisplay> profiles = new List<MemberDisplay>();
            //<a href="http(s)?://[a-zA-Z\.\-/0-9\?\=\_%&]+" target="_blank">[a-zA-Z0-9\s\-\'\.\(\)\!"/]+</a>
            StringBuilder sb = new StringBuilder();
            sb.Append("<a href=\"");
            sb.Append(@"http://[a-zA-Z\.\-/0-9\?\=_%&]+");
            sb.Append("\" target=\"");
            sb.Append("_blank\"");
            sb.Append(@">[a-zA-Z0-9\s\-\'\.\(\)\!");
            sb.Append("\"/]+</a>");
            Regex lineItem = new Regex(sb.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled);

            StringBuilder sbState = new StringBuilder();
            sbState.Append("class=\"state\" style=\"padding-left: 20px;\" >[a-zA-Z");
            sbState.Append(@"\s]+</td>");
            Regex stateItem = new Regex(sbState.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled);

            StringBuilder sbCountry = new StringBuilder();
            sbCountry.Append("<h7 class=\"expand\">[a-zA-Z");
            sbCountry.Append(@"\s]+</h7>");
            Regex countryItem = new Regex(sbCountry.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled);

            StringBuilder sbName = new StringBuilder();
            sbName.Append(@"[a-zA-Z0-9\s\-\'\.\(\)\!");
            sbName.Append("\"/]+");
            Regex name = new Regex(sbName.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Regex state = new Regex(@"[a-zA-Z\s]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Regex country = new Regex(@"[a-zA-Z\s]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Regex webSite = new Regex(@"http://[a-zA-Z\.\-/0-9\?\=_%&]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            WebClient webClient = new WebClient();
            const string strUrl = "http://derbyroster.com/";
            byte[] reqHTML;
            reqHTML = webClient.DownloadData(strUrl);
            UTF8Encoding objUTF8 = new UTF8Encoding();
            string html = objUTF8.GetString(reqHTML);
            html = html.Replace("�", "");

            int blah = lineItem.Matches(html).Count;

            foreach (Match stMatch in stateItem.Matches(html))
            {
                ItemInMatches item = new ItemInMatches();
                item.index = stMatch.Index;
                item.name = stMatch.Value;
                item.name = item.name.Remove(0, item.name.IndexOf(">"));
                item.name = country.Match(item.name).Value;
                states.Add(item);
            }

            foreach (Match stMatch in countryItem.Matches(html))
            {
                ItemInMatches item = new ItemInMatches();
                item.index = stMatch.Index;
                item.name = stMatch.Value;
                item.name = item.name.Remove(0, item.name.IndexOf(">"));
                item.name = state.Match(item.name).Value;
                countries.Add(item);
            }

            MatchCollection matches = lineItem.Matches(html);
            foreach (Match pro in matches)
            {
                string match = pro.Value;

                MemberDisplay profile = new MemberDisplay();
                //website
                RDN.Portable.Classes.League.Classes.League l = new Portable.Classes.League.Classes.League();
                l.Website = webSite.Match(match).Value;
                match = match.Remove(0, match.IndexOf(">"));
                //name
                l.Name = name.Match(match).Value;
                profile.Leagues.Add(l);
                //state
                var stat = states.Where(x => x.index < pro.Index).LastOrDefault();
                profile.PhoneNumber = stat.name;
                //country
                var count = countries.Where(x => x.index < pro.Index).LastOrDefault();
                profile.PlayerNumber = count.name;

                profiles.Add(profile);
            }

            foreach (var pr in profiles)
            {
                try
                {
                    var dc = new ManagementContext();
                    if (pr.Leagues.FirstOrDefault() != null)
                    {
                        Console.WriteLine("teamName:" + pr.Leagues.FirstOrDefault().Name);
                        var team = dc.LeaguesForDerbyRoster.Where(x => x.Name.ToLower() == pr.Leagues.FirstOrDefault().Website.ToLower()).FirstOrDefault();
                        if (team == null)
                        {
                            DerbyRosterLeague roster = new DerbyRosterLeague();
                            roster.Name = pr.Leagues.FirstOrDefault().Name;
                            roster.WebSite = pr.Leagues.FirstOrDefault().Website;
                            roster.State = pr.PhoneNumber;
                            roster.Country = pr.PlayerNumber;

                            dc.LeaguesForDerbyRoster.Add(roster);
                            dc.SaveChanges();


                            string url = RDN.Library.Classes.Config.LibraryConfig.PublicSite + "/" + RDN.Library.Classes.Config.LibraryConfig.SportNameForUrl + "-league/2/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(pr.Leagues.FirstOrDefault().Name) + "/" + roster.TeamId.ToString().Replace("-", "");
                            Console.WriteLine("sitemap:" + url);
                            SitemapHelper.AddNode(url, false);

                        }
                    }
                }
                catch { }
            }

        }


    }

    public class ItemInMatches
    {
        public int index { get; set; }
        public string name { get; set; }
    }
}
