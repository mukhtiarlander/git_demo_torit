using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Data.Contracts;
using RDN.Library.Cache;
using System.Collections.Generic;
using RDN.Portable.Config;
using System.Linq;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Settings repository
    /// </summary>
    public class SearchRepository
    {
        /// <summary>
        /// Get all settings
        /// </summary>
        /// <returns>Settings object</returns>
        public static List<SearchResultItem> Search(string searchString, string type)
        {
            List<SearchResultItem> result = new List<SearchResultItem>();
            if (string.IsNullOrEmpty(type) || type == "l")
            {
                var leagues = SiteCache.SearchAllPublicLeagues(0, 20, searchString);

                for (int i = 0; i < leagues.Count; i++)
                {
                    SearchResultItem item = new SearchResultItem();
                    item.ItemType = "League";
                    item.IdOfItem = leagues[i].LeagueId;
                    if (!string.IsNullOrEmpty(leagues[i].LogoUrlThumb))
                        item.PhotoUrl = leagues[i].LogoUrlThumb;
                    else
                        item.PhotoUrl = leagues[i].LogoUrl;
                    item.Title = leagues[i].LeagueName;
                    item.UrlOfItem = leagues[i].LeagueUrl;

                    item.Properties.Add("Founded", leagues[i].DateFounded.ToShortDateString());
                    item.Properties.Add("City", leagues[i].City);
                    item.Properties.Add("Country", leagues[i].Country);
                    item.Properties.Add("MemberCount", leagues[i].Membercount.ToString());
                    item.Properties.Add("Latitude", leagues[i].lat.ToString());
                    item.Properties.Add("Longitude", leagues[i].lon.ToString());
                    item.Properties.Add("State", leagues[i].State);
                    item.Properties.Add("Twitter", leagues[i].Twitter);
                    item.Properties.Add("WebSite", leagues[i].WebSite);
                    item.Properties.Add("Instagram", leagues[i].Instagram);
                    item.Properties.Add("Facebook", leagues[i].Facebook);
                    item.Properties.Add("RuleSetsPlayed", leagues[i].RuleSetsPlayed);

                    result.Add(item);
                }
            }
            if (string.IsNullOrEmpty(type) || type == "m")
            {
                var members = SiteCache.SearchAllPublicMembers(0, 20, searchString);

                for (int i = 0; i < members.Count; i++)
                {
                    SearchResultItem item = new SearchResultItem();
                    item.ItemType = "Member";
                    item.IdOfItem = members[i].MemberId;
                    if (!string.IsNullOrEmpty(members[i].ThumbUrl))
                        item.PhotoUrl = members[i].ThumbUrl;
                    else
                        item.PhotoUrl = members[i].photoUrl;
                    item.Title = members[i].DerbyName;
                    item.UrlOfItem = members[i].DerbyNameUrl;

                    item.Properties.Add("Bio", members[i].Bio);
                    item.Properties.Add("Age", members[i].Age.ToString());
                    item.Properties.Add("DerbyNumber", members[i].DerbyNumber);
                    item.Properties.Add("DOB", members[i].DOB.ToShortDateString());
                    item.Properties.Add("GamesPlayed", members[i].GamesCount.ToString());
                    item.Properties.Add("Gender", members[i].Gender);
                    item.Properties.Add("HeightFeet", members[i].HeightFeet.ToString());
                    item.Properties.Add("HeightInches", members[i].HeightInches.ToString());
                    item.Properties.Add("LeagueName", members[i].LeagueName);
                    item.Properties.Add("LeagueUrl", members[i].LeagueUrl);
                    item.Properties.Add("Losses", members[i].Losses.ToString());
                    item.Properties.Add("Wins", members[i].Wins.ToString());

                    result.Add(item);
                }

            }

            return result.ToList();
        }


    }
}
