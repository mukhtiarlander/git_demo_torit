using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Account.Classes;
using System.Text.RegularExpressions;
using System.Net;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Member;
using RDN.Library.DataModels.Team;
using RDN.Portable.Classes.Account.Classes;

namespace RDN.Library.Classes.Utilities
{
    public class TwoEvilsWebsite
    {
        /// <summary>
        /// scans the two evils website for all the derby profiles
        /// </summary>
        public static void ScanTwoEvilsWebsite()
        {

            //<tr class="(trc1|trc2)"><td>(&nbsp;|[a-zA-Z0-9\s�\-\.\'\%\(\),!"#$*/\?:&\+!_=~@\[\]`]+)</td><td>(&nbsp;|[0-9a-zA-Z\s\.\/\-%#:'�$"\+&/\\!_@\>=\?,\)\(\*\^;\]\[~]+(&cent;)?|&times;)</td><td>([0-9\-]+|&nbsp;)</td><td>(&nbsp;|[a-zA-Z\s\(\)0-9/\.':,\-�!_\*\?]+)</td></tr>

            //name: (&nbsp;|[a-zA-Z0-9\s�\-\.\'\%\(\),!"#$*/\?:&\+!_=~@\[\]`]+)
            //number: (&nbsp;|[0-9a-zA-Z\s\.\/\-%#:'�$"\+&/\\!_@\>=\?,\)\(\*\^;\]\[~]+(&cent;)?|&times;)
            // date: ([0-9\-]+|&nbsp;)
            //league: (&nbsp;|[a-zA-Z\s\(\)0-9/\.':,\-�!_\*\?]+)

            List<MemberDisplay> profiles = new List<MemberDisplay>();
            StringBuilder sb = new StringBuilder();
            sb.Append("<tr class=\"");
            sb.Append("(trc1|trc2)\"");
            sb.Append(@"><td>(&nbsp;|[a-zA-Z0-9\s�\-\.\'\%\(\),!");
            sb.Append(@"#$*/\?:&\+!_=~@\[\]`]+)</td><td>(&nbsp;|[0-9a-zA-Z\s\.\/\-%#:'�$");
            sb.Append(@"\+&/\\!_@\>=\?,\)\(\*\^;\]\[~]+(&cent;)?|&times;)</td><td>([0-9\-]+|&nbsp;)</td><td>(&nbsp;|[a-zA-Z\s\(\)0-9/\.':,\-�!_\*\?]+)</td></tr>");


            Regex lineItem = new Regex(sb.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled);

            Regex name = new Regex(@"(&nbsp;|[a-zA-Z0-9\s�\-\.\'\%\(\),!#$*/\?:&\+!_=~@\[\]`]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Regex number = new Regex(@"(&nbsp;|[0-9a-zA-Z\s\.\/\-%#:'�$\+&/\\!_@\>=\?,\)\(\*\^;\]\[~]+(&cent;)?|&times;)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Regex date = new Regex(@"([0-9\-]+|&nbsp;)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Regex league = new Regex(@"(&nbsp;|[a-zA-Z\s\(\)0-9/\.':,\-�!_\*\?]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Regex tdBlah = new Regex(@"</td><td>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            WebClient webClient = new WebClient();
            const string strUrl = "http://www.twoevils.org/rollergirls/";
            byte[] reqHTML;
            reqHTML = webClient.DownloadData(strUrl);
            UTF8Encoding objUTF8 = new UTF8Encoding();
            string html = objUTF8.GetString(reqHTML);
            int blah = lineItem.Matches(html).Count;
            MatchCollection matches = lineItem.Matches(html);
            foreach (Match pro in matches)
            {
                string website = "";
                string teamName = "";
                string match = pro.Value;
                match = match.Replace("<tr class=\"trc2\"><td>", "");
                match = match.Replace("<tr class=\"trc1\"><td>", "");
                MemberDisplay profile = new MemberDisplay();
                profile.DerbyName = name.Match(match).Value;
                match = match.Remove(name.Match(match).Index, profile.DerbyName.Length);
                match = tdBlah.Replace(match, "", 1);
                profile.PlayerNumber = number.Match(match).Value;
                if (profile.PlayerNumber.Contains("&nbsp;"))
                    profile.PlayerNumber = string.Empty;
                match = match.Remove(number.Match(match).Index, profile.PlayerNumber.Length);
                match = tdBlah.Replace(match, "", 1);
                website = date.Match(match).Value;
                if (website.Contains("&nbsp;"))
                    website = string.Empty;
                match = match.Remove(date.Match(match).Index, website.Length);
                match = tdBlah.Replace(match, "", 1);
                teamName = league.Match(match).Value;
                teamName = teamName.Replace("(trademarked name)", "");
                if (teamName.Contains("&nbsp;") || teamName.Contains("delete"))
                    teamName = string.Empty;
                match = match.Remove(league.Match(match).Index, teamName.Length);
                RDN.Portable.Classes.League.Classes.League l = new RDN.Portable.Classes.League.Classes.League();
                l.Website = website;
                l.Name = teamName;
                profile.Leagues.Add(l);
                if (!profile.DerbyName.Contains("Expletive Deleted"))
                    profiles.Add(profile);
            }

            foreach (var pr in profiles)
            {
                try
                {
                    Console.WriteLine("teamName:" + pr.DerbyName);
                    var dc = new ManagementContext();
                    Guid skaterId = new Guid();
                    Guid teamId = new Guid();
                    if (pr.Leagues.FirstOrDefault() != null)
                    {
                        var team = dc.LeaguesForTwoEvils.Where(x => x.Name.ToLower() == pr.Leagues.FirstOrDefault().Name.ToLower()).FirstOrDefault();
                        var teamRoster = dc.LeaguesForDerbyRoster.Where(x => x.Name.ToLower() == pr.Leagues.FirstOrDefault().Name.ToLower()).FirstOrDefault();
                        if (team == null)
                        {
                            TwoEvilsProfile skater = new TwoEvilsProfile();
                            skater.Date = pr.Leagues.FirstOrDefault().Website;
                            skater.Name = pr.DerbyName;
                            skater.Number = pr.PlayerNumber;
                            dc.ProfilesForTwoEvils.Add(skater);

                            TwoEvilsLeague leagueDb = new TwoEvilsLeague();
                            if (pr.Leagues.FirstOrDefault().Name.Length > 0)
                            {
                                leagueDb.Name = pr.Leagues.FirstOrDefault().Name;
                                leagueDb.Skaters.Add(skater);
                                dc.LeaguesForTwoEvils.Add(leagueDb);
                            }
                            if (teamRoster != null)
                            {
                                teamRoster.Skaters.Add(skater);
                            }

                            dc.SaveChanges();
                            skaterId = skater.ProfileId;
                            teamId = leagueDb.TeamId;
                        }
                        else
                        {
                            TwoEvilsProfile skater = new TwoEvilsProfile();
                            skater.Date = pr.Leagues.FirstOrDefault().Website;
                            skater.Name = pr.DerbyName;
                            skater.Number = pr.PlayerNumber;
                            dc.ProfilesForTwoEvils.Add(skater);
                            team.Skaters.Add(skater);

                            if (teamRoster != null)
                            {
                                teamRoster.Skaters.Add(skater);
                            }

                            dc.SaveChanges();
                            teamId = team.TeamId;
                            skaterId = skater.ProfileId;
                        }

                        if (skaterId != new Guid())
                        {
                            //  http://rdnation.com/roller-derby-skater/A-LO/5cb2bdb641f747f39d1745286d6a1561
                            string url = "http://rdnation.com/roller-derby-skater/1/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(pr.DerbyName) + "/" + skaterId.ToString().Replace("-", "");
                            SitemapHelper.AddNode(url, false);
                        }
                        if (teamId != new Guid())
                        {
                            //http://rdnation.com/roller-derby-league/San-Diego-Roller-Derby/99121fbf301f4c2fb4beb5c13d011088
                            string url = "http://rdnation.com/roller-derby-league/1/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(pr.Leagues.FirstOrDefault().Name) + "/" + teamId.ToString().Replace("-", "");
                            SitemapHelper.AddNode(url, false);
                        }
                    }
                }
                catch { }
            }

        }

    }
}
