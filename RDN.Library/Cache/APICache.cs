using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using RDN.Library.Classes.Account.Classes.Json;
using RDN.Library.Classes.Calendar;
using RDN.Library.Classes.Calendar.Models;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Game;
using RDN.Library.Classes.League;
using RDN.Library.Classes.League.Classes;
using RDN.Library.Classes.League.Classes.Json;
using RDN.Library.Classes.Store;
using RDN.Library.Classes.Store.Display;
using RDN.Library.Classes.Team;
using RDN.Portable.Classes.Geo;
using RDN.Portable.Classes.Team;


namespace RDN.Library.Cache
{
    public class ApiCache : CacheLock
    {

        List<TeamLogo> Logos { get; set; }
        List<TeamLogo> ScoreboardLogos { get; set; }
        List<CountryClass> Countries { get; set; }

        public static List<TeamLogo> GetAllLogos()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.Logos == null || cached.Logos.Count == 0)
                {
                    TeamFactory lo = new TeamFactory();
                    var logos = lo.GetAllLogos(new TeamDisplay());
                    foreach (var l in logos)
                        l.SaveLocation = null;
                    cached.Logos = logos;
                    UpdateCache(cached);
                }

                return cached.Logos;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<TeamLogo>();
        }


        public static List<CountryClass> GetCountriesInfo()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                if (cached.Countries.Count == 0)
                {
                    cached.Countries = RDN.Library.Classes.Location.LocationFactory.GetCountries();
                    UpdateCache(cached);
                }
                return cached.Countries;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<CountryClass>();
        }

        public static List<RDN.Portable.Classes.Team.TeamLogo> GetAllScoreboardLogos()
        {
            try
            {
                var cached = GetCache(HttpContext.Current.Cache);
                return cached.ScoreboardLogos;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<RDN.Portable.Classes.Team.TeamLogo>();
        }


        private static ApiCache GetCache(System.Web.Caching.Cache cache)
        {
            try
            {
                ApiCache dataObject = (ApiCache)cache["ApiCache"];
                if (dataObject == null)
                {

                    dataObject = (ApiCache)cache["ApiCache"];

                    if (dataObject == null)
                    {
                        dataObject = new ApiCache();
                        TeamFactory lo = new TeamFactory();
                        dataObject.Countries = new List<CountryClass>();
                        TeamDisplay team = new TeamDisplay();
                        var logos = lo.GetAllLogos(team);
                        foreach (var l in logos)
                            l.SaveLocation = null;

                        dataObject.Logos = logos;
                        var scoreboardLogos = team.ScoreboardLogos;
                        foreach (var l in scoreboardLogos)
                            l.SaveLocation = null;

                        dataObject.ScoreboardLogos = scoreboardLogos;
                        cache["ApiCache"] = dataObject;

                    }
                }
                return dataObject;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static void ClearCache()
        {
            try
            {
                HttpContext.Current.Cache.Remove("ApiCache");
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

        }
        private static ApiCache UpdateCache(ApiCache siteCache)
        {
            try
            {
                lock (ThisLock)
                {
                    HttpContext.Current.Cache["ApiCache"] = siteCache;
                }
                return siteCache;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
    }
}
