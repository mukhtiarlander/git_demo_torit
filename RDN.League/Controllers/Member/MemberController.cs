using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.League.Models.Filters;
using RDN.Library.Classes.Account.Classes;
using System.Web.Security;
using RDN.League.Models.User;
using RDN.League.Models.Federation;
using RDN.Library.Classes.Account.Enums;
using RDN.League.Classes.Enums;
using RDN.Library.Cache;
using RDN.League.Models.Enum;
using RDN.Library.Classes.Error;
using RDN.Library.Util.Enum;
using System.Collections.Specialized;
using RDN.Library.Util;
using RDN.Library.Classes.Communications.Enums;
using System.Text.RegularExpressions;
using RDN.Library.DataModels.ContactCard;
using RDN.Portable.Classes.Account.Enums;
using RDN.Portable.Classes.Account.Enums.Settings;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Communications.Enums;
using RDN.Library.Classes.Config;
using RDN.Portable.Classes.Insurance;

namespace RDN.League.Controllers
{


#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class MemberController : BaseController
    {

        private static Regex numberRgx = new Regex(@"\d+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //
        // GET: /Member/

        public ActionResult Index()
        {
            return View();
        }
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult ChangeMemberSettingCalView(string newId)
        {
            try
            {
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                CalendarDefaultViewEnum calView = (CalendarDefaultViewEnum)Enum.Parse(typeof(CalendarDefaultViewEnum), newId);
                bool success = MemberSettingsFactory.ChangeCalendarViewSetting(calView, memId);
                RDN.Library.Cache.MemberCache.Clear(memId);
                MemberCache.ClearApiCache(memId);
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult ChangeEmailNotificationSetting(string groupLeague, string id, string checkedUnCheck)
        {
            try
            {
                bool success = false;
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                switch (groupLeague)
                {
                    case "group":
                        success = MemberSettingsFactory.ChangeEmailNotificationSettingForGroup(memId, Convert.ToInt64(id), Convert.ToBoolean(checkedUnCheck));
                        break;
                    case "league":
                        success = MemberSettingsFactory.ChangeEmailNotificationSettingForLeague(memId, new Guid(id), Convert.ToBoolean(checkedUnCheck));
                        break;
                    case "groupbroadcast":
                        success = MemberSettingsFactory.ChangeEmailNotificationSettingForGroupBroadcasted(memId, Convert.ToInt64(id), Convert.ToBoolean(checkedUnCheck));
                        break;
                    case "forumBroadcast":
                        success = MemberSettingsFactory.ChangeEmailNotificationForumBroadcastsOff(memId, Convert.ToBoolean(checkedUnCheck));
                        break;
                    case "ForumNewPost":
                        success = MemberSettingsFactory.ChangeEmailNotificationForumNewPosts(memId, Convert.ToBoolean(checkedUnCheck));
                        break;
                    case "ForumWeeklyRoundup":
                        success = MemberSettingsFactory.ChangeEmailNotificationForumWeeklyRoundUpOff(memId, Convert.ToBoolean(checkedUnCheck));
                        break;
                    case "calendarnewevent":
                        success = MemberSettingsFactory.ChangeEmailNotificationCalendarNewEventOff(memId, Convert.ToBoolean(checkedUnCheck));
                        break;
                    case "newmessage":
                        success = MemberSettingsFactory.ChangeEmailNotificationMessageNewOff(memId, Convert.ToBoolean(checkedUnCheck));
                        break;
                }

                RDN.Library.Cache.MemberCache.Clear(memId);
                MemberCache.ClearApiCache(memId);
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChangeForumMessageOrderSetting(string checkedUnCheck)
        {
            try
            {
                bool success = false;
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();

                success = MemberSettingsFactory.ChangeForumMessageOrderSetting(memId, Convert.ToBoolean(checkedUnCheck));

                RDN.Library.Cache.MemberCache.Clear(memId);
                MemberCache.ClearApiCache(memId);
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveResortedOrderOfGroups(string newIds)
        {
            try
            {
                bool success = false;
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                Guid leagueId = MemberCache.GetLeagueIdOfMember(memId);

                var strings = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(newIds);
                var id = new long[strings.Length];
                string newOrder = "";
                for (int i = 0; i < strings.Length; i++)
                {
                    newOrder += Convert.ToInt64(numberRgx.Match(strings[i]).Value);
                    if (i + 1 != strings.Length)
                    {
                        newOrder += ":";
                    }
                }
                success = MemberSettingsFactory.ChangeForumGroupsOrder(memId, leagueId, newOrder);
                
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult MemberSetting()
        {
            try
            {
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                Guid leagueId = MemberCache.GetLeagueIdOfMember(memId);
                var display = MemberCache.GetMemberDisplay(memId);

                if (display.Settings == null)
                {
                    display.Settings = new MemberSettingsClass();
                    display.Settings.CalendarViewDefault = CalendarDefaultViewEnum.List_View;
                }
                display.Settings.DerbyName = display.DerbyName;
                display.Settings.MemberId = display.MemberId;
                display.Settings.PhoneNumber = display.PhoneNumber;
                display.Settings.ServiceProvider = display.Carrier;
                display.Settings.IsCarrierVerified = display.IsCarrierVerified;
                display.Settings.DoesReceiveLeagueNotifications = display.DoesReceiveLeagueNotifications;
                display.Settings.EmailCalendarNewEventBroadcast = display.EmailCalendarNewEventBroadcast;
                display.Settings.EmailForumBroadcasts = display.EmailForumBroadcasts;
                display.Settings.EmailForumNewPost = display.EmailForumNewPost;
                display.Settings.EmailForumWeeklyRoundup = display.EmailForumWeeklyRoundup;
                display.Settings.EmailMessagesReceived = display.EmailMessagesReceived;                
                display.Settings.CurrentLeagueId = display.CurrentLeagueId;
                display.Settings.Hide_DOB_From_League = display.Settings.Hide_DOB_From_League;
                display.Settings.Hide_DOB_From_Public = display.Settings.Hide_DOB_From_Public;
                display.Settings.Hide_Email_From_League = display.Settings.Hide_Email_From_League;
                display.Settings.Hide_Phone_Number_From_League = display.Settings.Hide_Phone_Number_From_League;
                display.Settings.Hide_Address_From_League = display.Settings.Hide_Address_From_League;
                display.Settings.DoYouDerby = !display.IsNotConnectedToDerby;
                display.Settings.ForumDescending = display.Settings.ForumDescending;
                ViewBag.CalendarView = RDN.League.Classes.Enums.EnumExt.ToSelectList(display.Settings.CalendarViewDefault);
                ViewBag.ServiceProviders = RDN.League.Classes.Enums.EnumExt.ToSelectListValue(display.Settings.ServiceProvider);

                //order groups by user preferences
                string groupsOrderString = MemberSettingsFactory.GetForumGroupsOrder(memId, leagueId);
                if (!string.IsNullOrWhiteSpace(groupsOrderString))
                {
                    List<long> groupsOrder = groupsOrderString.Split(':').Select(long.Parse).ToList();
                    var groups = MemberCache.GetGroupsApartOf(memId);
                    var groupsOrdered = (from i in groupsOrder
                                        join o in groups on i equals o.Id
                                        select o).ToList();
                    //make sure that all the groups are part of the result
                    if (groups.Count > groupsOrdered.Count)
                    {
                        // if not add the unsorted groups at the end
                        foreach (var group in groups)
                        {
                            if (!groupsOrdered.Contains(group))
                            {
                                groupsOrdered.Add(group);
                            }
                        }
                    }
                    display.Settings.GroupsApartOf = groupsOrdered;
                }
                else
                {
                    display.Settings.GroupsApartOf = MemberCache.GetGroupsApartOf(memId);
                }
                
                return View(display.Settings);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [LeagueAuthorize(EmailVerification = true, IsInLeague = false, IsManager = false)]
        public ActionResult MemberContacts(string id)
        {
            try
            {
                ViewBag.Saved = false;
                ViewBag.Retired = false;
                MemberDisplay member = null;
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                //if an admin wants to edit the member.
                if (id != null && (MemberCache.IsAdministrator(memId) || MemberCache.IsManagerOrBetterOfLeague(memId) && RDN.Library.Cache.MemberCache.CheckIsLeagueSubscriptionPaid(memId)))
                {
                    member = RDN.Library.Cache.MemberCache.GetMemberDisplay(new Guid(id));
                }
                else
                {
                    //for everyone else.
                    member = RDN.Library.Cache.MemberCache.GetMemberDisplay(memId);
                }
                ViewBag.Contacts = RDN.League.Classes.Enums.EnumExt.ToSelectList(MemberContactTypeEnum.UnSpecified);
                var countries = RDN.Library.Classes.Location.LocationFactory.GetCountries();
                ViewBag.Countries = new SelectList(countries, "CountryId", "Name");
                return View(member);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }


        [LeagueAuthorize(EmailVerification = true, IsInLeague = false, IsManager = false)]
        public ActionResult AddMemberContact(string memberId, string first, string last, string type, string email, string phone, string address1, string address2, string city, string state, string zip, string country)
        {
            try
            {
                var t = (MemberContactTypeEnum)Enum.Parse(typeof(MemberContactTypeEnum), type);
                bool success = RDN.Library.Classes.Account.Classes.MemberContactFactory.AddContactToMember(new Guid(memberId), first, last, t, email, phone, address1, address2, city, state, zip, country);

                RDN.Library.Cache.MemberCache.Clear(new Guid(memberId));
                MemberCache.ClearApiCache(new Guid(memberId));
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false, IsManager = false)]
        public ActionResult RemoveContact(string memberId, string contactId)
        {
            try
            {
                bool success = RDN.Library.Classes.Account.Classes.MemberContactFactory.RemoveContact(new Guid(memberId), Convert.ToInt64(contactId));

                RDN.Library.Cache.MemberCache.Clear(new Guid(memberId));
                MemberCache.ClearApiCache(new Guid(memberId));
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false, IsManager = false)]
        public ActionResult MemberMedical(string id)
        {
            try
            {
                ViewBag.Saved = false;
                ViewBag.Retired = false;
                MemberDisplay member = null;
                //if an admin wants to edit the member.
                if (id != null && MemberCache.IsAdministrator(RDN.Library.Classes.Account.User.GetMemberId()))
                {
                    member = RDN.Library.Cache.MemberCache.GetMemberDisplay(new Guid(id));
                }
                else
                {
                    //for everyone else.
                    member = RDN.Library.Cache.MemberCache.GetMemberDisplay(RDN.Library.Classes.Account.User.GetMemberId());
                }
                ViewBag.ContactLenses = RDN.League.Classes.Enums.EnumExt.ToSelectList(member.Medical.HardSoftLensesEnum);
                return View(member);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false, IsManager = false)]
        public ActionResult MemberMedical(MemberDisplay member)
        {
            try
            {
                ViewBag.Saved = true;
                ViewBag.Retired = false;
                var id = RDN.Library.Classes.Account.User.GetMemberId();
                if (Request.Form["clearRecord"] != null)
                {
                    RDN.Library.Classes.Account.User.ClearMedicalRecords(id);
                }
                else if (Request.Form["saveRecords"] != null)
                {
                    RDN.Library.Classes.Account.User.UpdateMedicalRecords(member);
                }
                MemberCache.Clear(id);
                MemberCache.ClearApiCache(id);

                return Redirect(Url.Content("~/member/medical"));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false, IsManager = false)]
        public ActionResult EditMemberMySelf(string id)
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string u = nameValueCollection["u"];
                if (u == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Updated";
                    this.AddMessage(message);
                }
                var memid = RDN.Library.Classes.Account.User.GetMemberId();
                ViewBag.Retired = false;
                MemberDisplay member = null;
                //if an admin wants to edit the member.
                if (id != null && MemberCache.IsAdministrator(memid))
                {
                    member = RDN.Library.Cache.MemberCache.GetMemberDisplay(new Guid(id));
                }
                else
                {
                    //for everyone else.
                    member = RDN.Library.Cache.MemberCache.GetMemberDisplay(memid);
                }

                EditMember edit = new EditMember();
                edit.Photos = member.Photos;
                edit.IsProfileRemovedFromPublicView = member.IsProfileRemovedFromPublicView;
                edit.IsRetired = member.IsRetired;
                edit.Bio = member.Bio;
                edit.DerbyName = member.DerbyName;
                edit.DOB = member.DOB;
                edit.Email = member.Email;
                edit.DayJob = member.DayJob;
                edit.StartedSkating = member.StartedSkating;
                edit.StoppedSkating = member.StoppedSkating;
                edit.InsuranceNumbers = member.InsuranceNumbers;

                if (LibraryConfig.SiteType == Library.Classes.Site.Enums.SiteType.RollerDerby)
                {
                    if (edit.InsuranceNumbers.Where(x => x.Type == InsuranceType.CRDI).FirstOrDefault() == null)
                    {
                        edit.InsuranceNumbers.Add(new InsuranceNumber()
                        {
                            Type = InsuranceType.CRDI
                        });
                    }
                    if (edit.InsuranceNumbers.Where(x => x.Type == InsuranceType.USARS).FirstOrDefault() == null)
                    {
                        edit.InsuranceNumbers.Add(new InsuranceNumber()
                        {
                            Type = InsuranceType.USARS
                        });
                    }
                    if (edit.InsuranceNumbers.Where(x => x.Type == InsuranceType.WFTDA).FirstOrDefault() == null)
                    {
                        edit.InsuranceNumbers.Add(new InsuranceNumber()
                        {
                            Type = InsuranceType.WFTDA
                        });
                    }
                }
                if (edit.InsuranceNumbers.Where(x => x.Type == InsuranceType.Other).FirstOrDefault() == null)
                {
                    edit.InsuranceNumbers.Add(new InsuranceNumber()
                    {
                        Type = InsuranceType.Other
                    });
                }


                foreach (var fed in member.FederationsApartOf)
                {
                    FederationDisplay fe = new FederationDisplay();
                    fe.FederationComments = fed.FederationComments;
                    fe.FederationId = fed.FederationId;
                    fe.FederationName = fed.FederationName;
                    fe.MADEClassRank = fed.MADEClassRank;
                    fe.MemberType = fed.MemberType;
                    fe.OwnerType = fed.OwnerType;
                    edit.FederationsApartOf.Add(fe);
                }

                edit.Leagues = member.Leagues;
                edit.Firstname = member.Firstname;
                edit.Gender = member.Gender;
                edit.HeightFeet = member.HeightFeet;
                edit.HeightInches = member.HeightInches;
                edit.LastName = member.LastName;
                edit.MemberId = member.MemberId;
                edit.UserId = member.UserId;
                edit.PhoneNumber = member.PhoneNumber;
             

                edit.PlayerNumber = member.PlayerNumber;
                edit.WeightLbs = member.WeightLbs;
                ViewData["genderSelectList"] = member.Gender.ToSelectList();
                var countries = RDN.Library.Classes.Location.LocationFactory.GetCountries();
                edit.Countries = new SelectList(countries, "CountryId", "Name");
                if (member.ContactCard.Addresses.Count > 0)
                {
                    foreach (var add in member.ContactCard.Addresses)
                    {
                        edit.Address = add.Address1;
                        edit.Address2 = add.Address2;
                        edit.City = add.CityRaw;
                        edit.State = add.StateRaw;
                        edit.ZipCode = add.Zip;
                        if (countries.Where(x => x.Code == add.Country).FirstOrDefault() != null)
                            edit.Country = countries.Where(x => x.Code == add.Country).FirstOrDefault().CountryId;
                    }
                }

                return View(edit);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false, IsManager = false)]
        public ActionResult EditMemberMySelf(EditMember member, HttpPostedFileBase file)
        {
            try
            {
                ViewBag.Saved = true;
                ViewBag.Retired = false;
                Guid id = member.MemberId;
                if (member.MemberId == new Guid())
                    id = RDN.Library.Classes.Account.User.GetMemberId();
                var edit = RDN.Library.Cache.MemberCache.GetMemberDisplay(id);

                MemberDisplay newMemberWithImage = null;

                edit.Bio = member.Bio;
                if (!String.IsNullOrEmpty(member.DerbyName))
                    edit.DerbyName = member.DerbyName.Trim();
                if (member.DOB != null && member.DOB > DateTime.Now.AddYears(-100))
                    edit.DOB = member.DOB;
                else if (member.DOB == new DateTime())
                    edit.DOB = new DateTime();
                edit.Email = member.Email;
                if (!String.IsNullOrEmpty(member.Email))
                    edit.Email = member.Email.Trim();
                edit.DayJob = member.DayJob;
                if (member.StartedSkating != null && member.StartedSkating > DateTime.Now.AddYears(-100))
                    edit.StartedSkating = member.StartedSkating;
                if (member.StoppedSkating != null && member.StoppedSkating > DateTime.Now.AddYears(-100))
                    edit.StoppedSkating = member.StoppedSkating;
                edit.IsProfileRemovedFromPublicView = member.IsProfileRemovedFromPublicView;
                if (!String.IsNullOrEmpty(member.Firstname))
                    edit.Firstname = member.Firstname.Trim();
                else
                    edit.Firstname = member.Firstname;
                edit.Gender = member.Gender;
                edit.HeightFeet = member.HeightFeet;
                edit.HeightInches = member.HeightInches;
                edit.LastName = member.LastName;

                edit.MemberId = member.MemberId;
                edit.PhoneNumber = member.PhoneNumber;
                edit.Photos = member.Photos;
                edit.PlayerNumber = member.PlayerNumber;
                edit.WeightLbs = member.WeightLbs;
                edit.IsRetired = member.IsRetired;

                edit.Address = member.Address;
                edit.Address2 = member.Address2;
                edit.City = member.City;
                edit.State = member.State;
                edit.ZipCode = member.ZipCode;
                edit.InsuranceNumbers.Clear();
                var values = Enum.GetValues(typeof(InsuranceType));
                foreach (var insurance in values)
                {
                    if (!String.IsNullOrEmpty(Request.Form["insuranceNumber_" + insurance]))
                    {
                        var insuranceType = (InsuranceType)Enum.Parse(typeof(InsuranceType), insurance.ToString());
                        var number = new InsuranceNumber()
                        {
                            Type = insuranceType,
                            Number = Request.Form["insuranceNumber_" + insurance]
                        };

                        DateTime expires = new DateTime();
                        if (DateTime.TryParse(Request.Form["insuranceNumberExpires_" + insurance], out expires))
                            number.Expires = expires;

                        edit.InsuranceNumbers.Add(number);
                    }
                }


                ViewData["genderSelectList"] = member.Gender.ToSelectList();
                var countries = RDN.Library.Classes.Location.LocationFactory.GetCountries();
                member.Countries = new SelectList(countries, "CountryId", "Name");
                edit.CountryId = member.Country;

                foreach (var leag in edit.Leagues)
                {
                    if (!String.IsNullOrEmpty(HttpContext.Request.Form[leag.LeagueId + "-LEAGUEDepartureDate"]))
                    {
                        DateTime outDT;
                        bool success = DateTime.TryParse(HttpContext.Request.Form[leag.LeagueId + "-LEAGUEDepartureDate"], out outDT);
                        if (success)
                            leag.DepartureDate = outDT;
                    }
                }

                if (file != null)
                    newMemberWithImage = RDN.Library.Classes.Account.User.UpdateMemberDisplayForMember(edit, file.InputStream, file.FileName);
                else
                    newMemberWithImage = RDN.Library.Classes.Account.User.UpdateMemberDisplayForMember(edit);

                RDN.Library.Cache.MemberCache.Clear(id);
                MemberCache.ClearApiCache(id);

                return Redirect(Url.Content("~/member/edit?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: member.MemberId.ToString());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false, IsManager = false)]
        public ActionResult RetireSelf()
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                RDN.Library.Classes.Account.User.RetireMember(memId);
                RDN.Library.Cache.MemberCache.Clear(memId);
                MemberCache.ClearApiCache(memId);

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false, IsManager = false)]
        public ActionResult UnRetireSelf()
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                RDN.Library.Classes.Account.User.UnRetireMember(memId);
                RDN.Library.Cache.MemberCache.Clear(memId);
                MemberCache.ClearApiCache(memId);

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult ViewMember(string id, string name)
        {
            try
            {
                var mem = RDN.Library.Cache.MemberCache.GetMemberDisplay(new Guid(id));
                return View(mem);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult ChangeEmail()
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string u = nameValueCollection["u"];
                if (u == SiteMessagesEnum.sww.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Warning;
                    message.Message = "Username Change was NOT successful. Someone else has that username.";
                    this.AddMessage(message);
                }
                if (u == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Username was changed Successfully.";
                    this.AddMessage(message);
                }


                Guid memberId = RDN.Library.Classes.Account.User.GetMemberId();

                var mem = RDN.Library.Cache.MemberCache.GetMemberDisplay(memberId);
                return View(mem);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult ChangeEmail(MemberDisplay member)
        {
            try
            {
                ViewBag.Updated = false;
                bool changed = RDN.Library.Classes.Account.User.ChangeUserName(member.UserId, member.UserName, member.DerbyName);
                if (changed)
                {

                    MemberCache.Clear(member.MemberId);
                    MemberCache.ClearApiCache(member.MemberId);
                    return Redirect(Url.Content("~/member/username?u=" + SiteMessagesEnum.s));
                }
                else
                    return Redirect(Url.Content("~/member/username?u=" + SiteMessagesEnum.sww));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult ChangePassword()
        {
            try
            {
                ViewBag.Updated = false;
                Guid memberId = RDN.Library.Classes.Account.User.GetMemberId();

                var mem = RDN.Library.Cache.MemberCache.GetMemberDisplay(memberId);
                return View(mem);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult ChangePassword(MemberDisplay member)
        {
            try
            {
                string possibleErrorMsg;
                ViewBag.Updated = false;
                if (member.NewPassword == member.Password)
                {
                    bool changed = RDN.Library.Classes.Account.User.ChangeUserPassword(member.UserId, member.OldPassword, member.NewPassword, out possibleErrorMsg);
                    ViewBag.Updated = changed;
                    if (changed != true && string.IsNullOrWhiteSpace(possibleErrorMsg))
                    {
                        ViewBag.Message = "Something is wrong with your passwords, please try again. If it continues, feel free to contact " + LibraryConfig.DefaultInfoEmail;
                    }
                    else if (!changed)
                    {
                        ViewBag.Message = possibleErrorMsg;
                    }
                }
                else
                {
                    ViewBag.Message = "New Passwords were not the same.";
                }
                var mem = RDN.Library.Cache.MemberCache.GetMemberDisplay(member.MemberId);
                return View(mem);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());

            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [LeagueAuthorize(EmailVerification = true, IsInLeague = false, IsManager = false)]
        public ActionResult VerifySMS(string number)
        {
            try
            {
                string newNumber = String.Empty;

                var coll = numberRgx.Matches(number);
                foreach (Match m in coll)
                {
                    newNumber += m.Value;
                }

                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                //MobileServiceProvider provider = (MobileServiceProvider)Enum.Parse(typeof(MobileServiceProvider), carrier);
                bool success = MemberSettingsFactory.VerifiySMSCarrier(Convert.ToInt64(newNumber), MobileServiceProvider.None, memId);
                RDN.Library.Cache.MemberCache.Clear(memId);
                MemberCache.ClearApiCache(memId);
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false, IsManager = false)]
        public JsonResult RemoveMemberFromLeague(string memberId, string leagueId)
        {
            try
            {
                ViewBag.Saved = false;

                RDN.Library.Classes.League.LeagueFactory.DisconnectMemberFromLeague(new Guid(leagueId), new Guid(memberId));
                RDN.Library.Cache.MemberCache.Clear(new Guid(memberId));
                MemberCache.ClearApiCache(new Guid(memberId));

                return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public JsonResult SetPrivacySetting(string setting)
        {
            try
            {
                var id = RDN.Library.Classes.Account.User.GetMemberId();
                var temp = (MemberPrivacySettingsEnum)Enum.Parse(typeof(MemberPrivacySettingsEnum), setting);
                if (temp == MemberPrivacySettingsEnum.Do_You_Derby)
                    RDN.Library.Classes.Account.User.ToggleConnectionToDerby(id);
                else
                    RDN.Library.Classes.Account.Classes.MemberSettingsFactory.TogglePrivacySettingsForMember(id, temp);
                RDN.Library.Cache.MemberCache.Clear(id);
                MemberCache.ClearApiCache(id);

                return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false, IsManager = false)]
        public ActionResult VerifySMSCode(string number, string code)
        {
            try
            {
                string newNumber = String.Empty;
                var coll = numberRgx.Matches(number);
                foreach (Match m in coll)
                {
                    newNumber += m.Value;
                }
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                //MobileServiceProvider provider = (MobileServiceProvider)Enum.Parse(typeof(MobileServiceProvider), carrier);
                bool success = MemberSettingsFactory.VerifiySMSCarrierCode(Convert.ToInt64(newNumber), MobileServiceProvider.None, code, memId);
                RDN.Library.Cache.MemberCache.Clear(memId);
                MemberCache.ClearApiCache(memId);
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
       
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult RetireProfile()
        {
            try
            {
                Guid memberId = RDN.Library.Classes.Account.User.GetMemberId();

                var mem = RDN.Library.Cache.MemberCache.GetMemberDisplay(memberId);
                return View(mem);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public ActionResult UnRetireProfile()
        {
            try
            {
                Guid memberId = RDN.Library.Classes.Account.User.GetMemberId();

                var mem = RDN.Library.Cache.MemberCache.GetMemberDisplay(memberId);
                return View(mem);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
    }
}
