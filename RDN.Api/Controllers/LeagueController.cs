using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Portable.Models.Json.Public;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.League;
using System.IO;
using RDN.Portable.Network;
using RDN.Portable.Classes.League.Classes;
using System.Globalization;
using RDN.Portable.Util;
using RDN.Library.Classes.League;
using RDN.Portable.Classes.Location;
using RDN.Library.Classes.Location;
using RDN.Library.Classes.Calendar;
using RDN.Portable.Classes.Controls.Calendar.Enums;
using RDN.Library.Classes.Controls.Voting;
using RDN.Library.Classes.Forum;
using RDN.Portable.Classes.Controls.Forum;
using RDN.Api.Mvc;


namespace RDN.Api.Controllers
{
    public class LeagueController : Controller
    {
        public ActionResult LoadInitial()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<MemberPassParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.Mid);
                    if (ob.Uid == mem.UserId)
                    {
                        LeagueStartModel model = new LeagueStartModel();

                        if (mem.CurrentLeagueId != new Guid())
                        {
                            model.CurrentLeagueId = mem.CurrentLeagueId;
                            bool isAttendanceManager = MemberCache.IsAttendanceManagerOrBetterOfLeague(mem.MemberId);
                            var calId = MemberCache.GetCalendarIdForMemberLeague(mem.MemberId);
                            var forumId = MemberCache.GetForumIdForMemberLeague(mem.MemberId);
                            model.Calendar = CalendarFactory.GetCalendarEvents(calId, 5, mem.MemberId, isAttendanceManager);
                            model.Calendar.Events.AddRange(MemberCache.GetMemberBirthdays(mem.MemberId, DateTime.UtcNow, DateTime.UtcNow.AddDays(2)));
                            model.Calendar.Events.AddRange(MemberCache.GetMemberStartDates(mem.MemberId, DateTime.UtcNow, DateTime.UtcNow.AddDays(2)));

                            model.Polls = VotingFactory.GetPollsNotVotedOn(mem.CurrentLeagueId, mem.MemberId);
                            var topics = Forum.GetForumTopicsJsonUnread(forumId, 0, mem.MemberId, 5);
                            for (int i = 0; i < topics.Count; i++)
                                model.ForumTopics.Add(new ForumTopicModel() { TopicId = topics[i].TopicId, ForumId = topics[i].ForumId, TopicName = topics[i].TopicTitle, PostCount = topics[i].Replies, ViewCount = topics[i].ViewCount });
                            return Json(model, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new LeagueStartModel(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocations()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<MemberPassParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.Mid);
                    if (ob.Uid == mem.UserId)
                    {
                        var locs = CalendarFactory.GetLocationsOfCalendar(ob.IdOfAnySort2);
                        return Json(locs, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new List<Location>(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditLeague()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<MemberPassParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.Mid);
                    if (ob.Uid == mem.UserId)
                    {

                        LeagueBase leag = new LeagueBase();
                        bool isAdmin = MemberCache.IsManagerOrBetterOfLeague(ob.Mid);
                        if (!isAdmin)
                        {
                            leag.IsSuccessful = false;
                            return Json(leag, JsonRequestBehavior.AllowGet);
                        }

                        var league = RDN.Library.Classes.League.LeagueFactory.GetLeague(MemberCache.GetLeagueIdOfMember(ob.Mid));

                        leag.Address = league.Address;
                        leag.ZipCode = league.ZipCode;
                        leag.City = league.City;
                        leag.Country = league.Country;
                        leag.CountryId = league.CountryId;
                        leag.Email = league.Email;
                        leag.Founded = league.Founded;
                        leag.LeagueId = league.LeagueId;
                        leag.Name = league.Name;
                        leag.PhoneNumber = league.PhoneNumber;
                        leag.State = league.State;
                        leag.TimeZone = league.TimeZone;
                        leag.Website = league.Website;
                        leag.Twitter = league.Twitter;
                        leag.Instagram = league.Instagram;
                        leag.Facebook = league.Facebook;
                        leag.RuleSetsPlayedEnum = league.RuleSetsPlayedEnum;
                        leag.CultureSelected = league.CultureSelected;

                        var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(x => x.Name.Contains("en")).OrderBy(x => x.DisplayName).ToList();
                        for (int i = 0; i < cultures.Count; i++)
                        {
                            Culture c = new Culture();
                            c.Name = cultures[i].DisplayName;
                            c.LCID = cultures[i].LCID;
                            leag.Cultures.Add(c);
                        }
                        leag.IsSuccessful = true;
                        return Json(leag, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new LeagueBase() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveLeague()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<MemberPassParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.Mid);
                    if (ob.Uid == mem.UserId)
                    {
                        var leag = (LeagueBase)ob.Item;
                        RDN.Library.Classes.League.LeagueFactory.UpdateLeagueFromMobile(leag);
                        leag.IsSuccessful = true;
                        MemberCache.ClearWebSitesCache(ob.Mid);
                        return Json(leag, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new LeagueBase() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }


        [ValidateInput(false)]
        public JsonResult ViewMemberRoster(string mid, string uid)
        {
            List<MemberDisplayAPI> mems = new List<MemberDisplayAPI>();
            try
            {
                var mem = MemberCache.GetMemberDisplay(new Guid(mid));
                if (new Guid(uid) == mem.UserId)
                {
                    var members = MemberCache.GetLeagueMembers(mem.MemberId, mem.CurrentLeagueId);
                    for (int i = 0; i < members.Count; i++)
                    {
                        MemberDisplayAPI m = new MemberDisplayAPI();
                        if (members[i].Photos.FirstOrDefault() != null)
                            m.ThumbUrl = members[i].Photos.FirstOrDefault().ImageThumbUrl;
                        
                        m.DerbyName = members[i].DerbyName;
                        m.DerbyNameUrl = members[i].DerbyNameUrl;
                        m.DOB = members[i].DOB;
                        m.Email = members[i].Email;
                        m.Firstname = members[i].Firstname;
                        m.Gender = members[i].Gender;
                        m.HeightFeet = members[i].HeightFeet;
                        m.HeightInches = members[i].HeightInches;
                        m.LastName = members[i].LastName;
                        m.MemberId = members[i].MemberId;
                        m.PhoneNumber = members[i].PhoneNumber;
                        m.PlayerNumber = members[i].PlayerNumber;
                        m.UserId = members[i].UserId;
                        m.UserName = members[i].UserName;
                        m.WeightLbs = members[i].WeightLbs;
                        m.HideDOBFromLeague = members[i].Settings.Hide_DOB_From_League;
                        m.HideDOBFromPublic = members[i].Settings.Hide_DOB_From_Public;
                        m.HideEmailFromLeague = members[i].Settings.Hide_Email_From_League;
                        m.HidePhoneNumberFromLeague = members[i].Settings.Hide_Phone_Number_From_League;
                        m.Job = members[i].DayJob;
                        if (members[i].Leagues.FirstOrDefault() != null)
                        {
                            m.SkillsTestDate = members[i].Leagues.FirstOrDefault().SkillsTestDate.GetValueOrDefault();
                            m.DepartureDate = members[i].Leagues.FirstOrDefault().DepartureDate.GetValueOrDefault();
                        }
                        m.InsuranceNumCRDI = members[i].InsuranceNumCRDI;
                        m.InsuranceNumCRDIExpires = members[i].InsuranceNumCRDIExpires.GetValueOrDefault();
                        m.InsuranceNumOther = members[i].InsuranceNumOther;
                        m.InsuranceNumOtherExpires = members[i].InsuranceNumOtherExpires.GetValueOrDefault();
                        m.InsuranceNumUsars = members[i].InsuranceNumUsars;
                        m.InsuranceNumUsarsExpires = members[i].InsuranceNumUsarsExpires.GetValueOrDefault();
                        m.InsuranceNumWftda = members[i].InsuranceNumWftda;
                        m.InsuranceNumWftdaExpires = members[i].InsuranceNumWftdaExpires.GetValueOrDefault();
                        m.LeagueClassificationOfSkatingLevel = members[i].LeagueClassificationOfSkatingLevel;
                        mems.Add(m);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Json(mems, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public JsonResult Members(string mid, string uid)
        {
            List<MemberDisplayBasic> mems = new List<MemberDisplayBasic>();
            try
            {
                var mem = MemberCache.GetMemberDisplay(new Guid(mid));
                if (new Guid(uid) == mem.UserId)
                {
                    var members = MemberCache.GetLeagueMembers(mem.MemberId, mem.CurrentLeagueId);
                    for (int i = 0; i < members.Count; i++)
                    {
                        MemberDisplayBasic m = new MemberDisplayBasic();
                        if (members[i].Photos.FirstOrDefault() != null)
                            m.ThumbUrl = members[i].Photos.FirstOrDefault().ImageThumbUrl;
                        
                        m.DerbyName = members[i].DerbyName;
                        m.Firstname = members[i].Firstname;
                        m.LastName = members[i].LastName;
                        m.MemberId = members[i].MemberId;
                        mems.Add(m);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Json(mems, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public JsonResult Groups(string mid, string uid)
        {
            List<LeagueGroupBasic> mems = new List<LeagueGroupBasic>();
            try
            {
                var mem = MemberCache.GetMemberDisplay(new Guid(mid));
                if (new Guid(uid) == mem.UserId)
                {
                    var members = MemberCache.GetGroupsApartOf(mem.MemberId);
                    for (int i = 0; i < members.Count; i++)
                    {
                        LeagueGroupBasic m = new LeagueGroupBasic();
                        m.GroupName = members[i].GroupName;
                        m.Id = members[i].Id;
                        mems.Add(m);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Json(mems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllLeagues(int? p, int? c, string s)
        {
            List<LeagueJsonDataTable> names = new List<LeagueJsonDataTable>();

            if (!String.IsNullOrEmpty(s))
            {
                names = LeagueFactory.SearchPublicLeagues(s, c.GetValueOrDefault(), p.GetValueOrDefault());
            }
            else
            {
                names = LeagueFactory.SearchPublicLeagues("", c.GetValueOrDefault(), p.GetValueOrDefault());
            }

            return Json(new { leagues = names }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLeagesformap()
        {
            List<LeagueJsonDataTable> leages = new List<LeagueJsonDataTable>();
            leages = SiteCache.GetAllPublicLeagues();
            return Json(new
            {
                leageData = (from l in leages
                             select new[]
                        {
                        Convert.ToString(l.lon),
                        Convert.ToString(l.lat),
                        l.LeagueName,
                        l.City,
                        l.State,
                        l.Country,
                        l.LogoUrlThumb,
                        Convert.ToString(l.Membercount),
                        l.LeagueUrl
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
        public JsonResult GetAllLeaguesMobile(int p, int c, string s, string sw)
        {
            List<LeagueJsonDataTable> names = new List<LeagueJsonDataTable>();
            LeaguesJson leagues = new LeaguesJson();
            try
            {
                if (!String.IsNullOrEmpty(s))
                    names = SiteCache.SearchAllPublicLeagues(p, c, s);
                else
                    names = SiteCache.GetAllPublicLeagues(p, c, sw);

                leagues.Count = names.Count;
                leagues.Page = p;
                leagues.StartsWith = s;


                for (int i = 0; i < names.Count; i++)
                {
                    leagues.Leagues.Add(names[i]);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Json(leagues, JsonRequestBehavior.AllowGet);
        }
        public JsonpResult GetAllLeaguesJsonP(int p, int c, string s, string sw)
        {
            List<LeagueJsonDataTable> names = new List<LeagueJsonDataTable>();
            LeaguesJson leagues = new LeaguesJson();
            try
            {
                if (!String.IsNullOrEmpty(s))
                    names = SiteCache.SearchAllPublicLeagues(p, c, s);
                else
                    names = SiteCache.GetAllPublicLeagues(p, c, sw);

                leagues.Count = names.Count;
                leagues.Page = p;
                leagues.StartsWith = s;


                for (int i = 0; i < names.Count; i++)
                {
                    leagues.Leagues.Add(names[i]);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return new JsonpResult(leagues);
        }

        public JsonResult GetLeagueMobile(string lId)
        {

            try
            {
                var mem = SiteCache.GetPublicLeague(new Guid(lId));
                return Json(mem, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Json(new LeagueJsonDataTable(), JsonRequestBehavior.AllowGet);
        }

    }
}
