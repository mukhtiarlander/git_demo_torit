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
        public JsonResult GetAllSkaters(int? p, int? c)
        {
            List<SkaterJson> names = new List<SkaterJson>();
            try
            {
                if (p.HasValue && c.HasValue)
                {
                    names = SiteCache.GetAllPublicMembers(p.Value, c.Value);
                }
                else
                {
                    names = SiteCache.GetAllPublicMembers();
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Json(new
            {
                members = (
                       from n in names
                       select new[]
                    {
                        n.DerbyName,
                        n.DerbyNameUrl,
                        n.DerbyNumber,
                        n.Gender,
                        n.LeagueName,
                        n.LeagueUrl,
                        n.LeagueId,
                        n.photoUrl
                                            }).ToArray()
            }, JsonRequestBehavior.AllowGet);
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
