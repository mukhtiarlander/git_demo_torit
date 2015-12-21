using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Cache;
using RDN.Library.Classes.Account.Classes.Json;
using RDN.Library.Classes.Error;
using RDN.Portable.Models.Json;
using RDN.Portable.Models.Json.Public;

namespace RDN.Api.Controllers
{
    public class SkaterController : Controller
    {

        /// <summary>
        /// gets all the sakters
        /// </summary>
        /// <param name="p">page</param>
        /// <param name="c">count</param>
        /// <returns></returns>
        public JsonResult GetAllSkaters(int? p, int? c, string s)
        {
            List<SkaterJson> names = new List<SkaterJson>();
            try
            {
                names = SiteCache.SearchAllPublicMembers(p.GetValueOrDefault(), c.GetValueOrDefault(), s);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Json(new { members = names }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllSkatersStats()
        {
            List<SkaterJson> names = new List<SkaterJson>();
            var skaters = SiteCache.GetAllPublicMembers().OrderBy(x => Guid.NewGuid()).ToList();
            try
            {
                names = skaters.Where(x => !String.IsNullOrEmpty(x.photoUrl))
                        .Where(x => !x.photoUrl.Contains("roller-girl"))
                        .Where(x => !x.photoUrl.Contains("roller-person")).OrderBy(x => Guid.NewGuid()).Take(4).ToList();
                //minim 4
                if (names.Count < 4)
                {
                    names.AddRange(skaters.Where(x => !String.IsNullOrEmpty(x.photoUrl))
                        .Where(x => x.photoUrl.Contains("roller-girl") || x.photoUrl.Contains("roller-person"))
                        .OrderBy(x => Guid.NewGuid()).Take(4 - names.Count).ToList());
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Json(new { members = names }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p">page</param>
        /// <param name="c">count</param>
        /// <param name="s">ALpha Character to search by.</param>
        /// <returns></returns>
        public JsonResult GetAllSkatersMobile(int p, int c, string s, string sw)
        {
            List<SkaterJson> names = new List<SkaterJson>();
            SkatersJson skaters = new SkatersJson();
            try
            {
                if (!String.IsNullOrEmpty(s))
                    names = SiteCache.SearchAllPublicMembers(p, c, s);
                else
                    names = SiteCache.GetAllPublicMembers(p, c, sw);

                skaters.Count = names.Count;
                skaters.Page = p;
                skaters.StartsWith = s;


                for (int i = 0; i < names.Count; i++)
                {
                    skaters.Skaters.Add(new SkaterJson()
                    {
                        LeagueId = names[i].LeagueId,
                        MemberId = names[i].MemberId,
                        DerbyName = names[i].DerbyName,
                        DerbyNameUrl = names[i].DerbyNameUrl,
                        DerbyNumber = names[i].DerbyNumber,
                        Gender = names[i].Gender,
                        LeagueName = names[i].LeagueName,
                        LeagueUrl = names[i].LeagueUrl,
                        photoUrl = names[i].photoUrl,
                        ThumbUrl = names[i].ThumbUrl,
                        DOB = names[i].DOB,

                    });
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Json(skaters, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllSkatersForLeague(string lId)
        {
            List<SkaterJson> names = new List<SkaterJson>();
            SkatersJson skaters = new SkatersJson();
            try
            {
                names = SiteCache.GetAllPublicMembersInLeague(lId);
                skaters.Count = names.Count;
                for (int i = 0; i < names.Count; i++)
                {
                    skaters.Skaters.Add(new SkaterJson()
                    {
                        LeagueId = names[i].LeagueId,
                        MemberId = names[i].MemberId,
                        DerbyName = names[i].DerbyName,
                        DerbyNameUrl = names[i].DerbyNameUrl,
                        DerbyNumber = names[i].DerbyNumber,
                        Gender = names[i].Gender,
                        LeagueName = names[i].LeagueName,
                        LeagueUrl = names[i].LeagueUrl,
                        photoUrl = names[i].photoUrl,
                        ThumbUrl = names[i].ThumbUrl,
                        DOB = names[i].DOB
                    });
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Json(skaters, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSkaterMobile(string mId)
        {
            try
            {
                var mem = SiteCache.GetPublicMember(new Guid(mId));
                return Json(mem, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Json(new SkaterJson(), JsonRequestBehavior.AllowGet);
        }

    }
}
