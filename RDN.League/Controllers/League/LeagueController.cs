using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.League.Models.Filters;
using RDN.League.Models.League;
using RDN.Library.Classes.Federation;
using RDN.Library.Classes.Utilities;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Calendar;
using RDN.Library.Cache;
using RDN.Library.Classes.Account.Classes;
using RDN.League.Models.Utilities;
using RDN.League.Models.OutModel;
using RDN.Library.Classes.Federation.Enums;
using RDN.Library.Classes.Location;
using System.IO;
using RDN.Library.Classes.League.Classes;
using System.Collections.Specialized;
using RDN.League.Models.Enum;
using RDN.League.Classes.Enums;
using RDN.Library.Classes.EmailServer;
using RDN.Utilities.Config;
using RDN.Library.DataModels.EmailServer.Enums;
using RDN.Library.Classes.League.Enums;
using RDN.Library.Util.Enum;
using RDN.Library.Util;
using RDN.Library.Classes.Document;
using OfficeOpenXml;
using System.Data;
using RDN.Library.Classes.League.Reports;
using RDN.Portable.Models.Json.Public;
using RDN.Library.Classes.Account.Classes.Json;
using RDN.Library.Classes.Colors;
using RDN.Portable.Models.Json;
using System.Globalization;
using System.Threading;
using RDN.Portable.Classes.Federation.Enums;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.League.Classes;
using RDN.Portable.Classes.League.Enums;
using RDN.Library.Classes.League;
using RDN.Portable.Classes.Account.Enums;



namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class LeagueController : BaseController
    {
        /// <summary>
        /// default page size for the tables
        /// </summary>
        private int PAGE_SIZE = 150;

        #region groups

        public JsonResult GetGroupsOfCurrentMember()
        {

            var g = MemberCache.GetLeagueGroupsOfMember();

            if (g == null)
                return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
            return Json(new
            {
                groups = (from n in g
                          select new[]
                    {
                        n.GroupName,
                        n.Id.ToString()
                        }).ToArray(),
                isSuccess = true
            }, JsonRequestBehavior.AllowGet);
        }

        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult RemoveGroupFromLeague(string groupId)
        {
            try
            {
                var member = RDN.Library.Classes.Account.User.GetMemberId();
                if (MemberCache.IsModeratorOrBetterOfLeagueGroup(member
, Convert.ToInt64(groupId)) || MemberCache.IsSecretaryOrBetterOfLeague(member))
                {
                    bool success = LeagueGroupFactory.RemoveGroup(Convert.ToInt64(groupId));
                    MemberCache.Clear(member);
                    MemberCache.ClearApiCache(member);
                    return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = "false" }, JsonRequestBehavior.AllowGet);
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult ChangeToLeague(string leagueId)
        {
            try
            {
                var member = RDN.Library.Classes.Account.User.GetMemberId();
                if (MemberCache.IsMemberApartOfLeague(member
, new Guid(leagueId)))
                {
                    bool success = RDN.Library.Classes.League.LeagueFactory.ChangeCurrentLeague(member, new Guid(leagueId));

                    MemberCache.Clear(member);
                    MemberCache.ClearApiCache(member);
                    if (success)
                        return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.cls));
                }
                return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.clus));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        //        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        //        public JsonResult RemoveLeagueGroupMember(string groupId, string memberId)
        //        {
        //            try
        //            {
        //                var member = RDN.Library.Classes.Account.User.GetMemberId();
        //                if (MemberCache.IsModeratorOrBetterOfLeagueGroup(member
        //, Convert.ToInt64(groupId)) || MemberCache.IsManagerOrBetterOfLeague(member))
        //                {
        //                    bool success = LeagueGroup.RemoveMemberToGroup(Convert.ToInt64(groupId), new Guid(memberId));
        //                    MemberCache.UpdateCurrentLeagueMemberCache(member);
        //                    return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //            catch (Exception exception)
        //            {
        //                ErrorDatabaseManager.AddException(exception, exception.GetType());
        //            }
        //            return Json(new { isSuccess = "false" }, JsonRequestBehavior.AllowGet);
        //        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public JsonResult UpdateLeagueGroupMember(string groupId, string memberId, string memberType, string isApartOfG)
        {
            try
            {
                var member = RDN.Library.Classes.Account.User.GetMemberId();
                if (MemberCache.IsModeratorOrBetterOfLeagueGroup(member, Convert.ToInt64(groupId)) || MemberCache.IsManagerOrBetterOfLeague(member))
                {
                    bool isApart = Convert.ToBoolean(isApartOfG);
                    bool success = false;
                    if (isApart)
                        success = LeagueGroupFactory.UpdateMemberToGroup(Convert.ToInt64(groupId), new Guid(memberId), memberType);
                    else
                        success = LeagueGroupFactory.RemoveMemberToGroup(Convert.ToInt64(groupId), new Guid(memberId));
                    MemberCache.UpdateCurrentLeagueMemberCache(member);
                    return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = "false" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// adds a group member to the league group.
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="leagueId"></param>
        /// <param name="memberId"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public JsonResult AddLeagueGroupMember(string groupId, string leagueId, string memberId, string memberType)
        {
            try
            {
                var member = RDN.Library.Classes.Account.User.GetMemberId();
                if (MemberCache.IsModeratorOrBetterOfLeagueGroup(member, Convert.ToInt64(groupId)) || MemberCache.IsManagerOrBetterOfLeague(member))
                {
                    bool success = LeagueGroupFactory.AddMemberToGroup(Convert.ToInt64(groupId), new Guid(leagueId), new Guid(memberId), memberType);
                    MemberCache.UpdateCurrentLeagueMemberCache(member);
                    return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = "false" }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public ActionResult Groups()
        {
            try
            {
                var league = MemberCache.GetLeagueOfMember(RDN.Library.Classes.Account.User.GetMemberId());
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string u = nameValueCollection["u"];
                if (u == SiteMessagesEnum.sag.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Added New Group.";
                    this.AddMessage(message);
                }
                else if (u == SiteMessagesEnum.srg.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Removed Group.";
                    this.AddMessage(message);
                }
                else if (u == SiteMessagesEnum.sww.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Error;
                    message.Message = "Something went wrong, error sent, please try again later.";
                    this.AddMessage(message);
                }
                else if (u == SiteMessagesEnum.na.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Warning;
                    message.Message = "You do not have access to that page.";
                    this.AddMessage(message);
                }
                return View(league);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, IsSecretary = true)]
        public ActionResult GroupsAdd()
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var mems = MemberCache.GetCurrentLeagueMembers(memId);
                ViewData["GroupMembers"] = new SelectList(mems, "MemberId", "DerbyName");

                LeagueGroup group = new LeagueGroup();
                group.League = MemberCache.GetLeagueOfMember(memId);
                ViewData["groupTypeSelectList"] = group.GroupTypeEnum.ToSelectList();
                return View(group);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, IsSecretary = true)]
        public ActionResult GroupsAdd(LeagueGroup group)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                foreach (var mem in league.LeagueMembers)
                {
                    if (!String.IsNullOrEmpty(Request.Form[mem.MemberId + "-check"]))
                    {
                        bool cb = Request.Form[mem.MemberId + "-check"].ToString().Contains("true");
                        if (cb)
                        {
                            LeagueGroupMember m = new LeagueGroupMember();
                            m.MemberId = mem.MemberId;
                            if (!String.IsNullOrEmpty(HttpContext.Request.Form[mem.MemberId + "-memType"]))
                                m.MemberAccessLevelEnum = (GroupMemberAccessLevelEnum)Enum.Parse(typeof(GroupMemberAccessLevelEnum), HttpContext.Request.Form[mem.MemberId + "-memType"].ToString());

                            group.GroupMembers.Add(m);
                        }
                    }
                }


                if (LeagueGroupFactory.AddGroup(group))
                {
                    foreach (var mem in group.GroupMembers)
                    {
                        LeagueGroupFactory.EmailMemberAboutAddedToGroup(group.GroupName, mem.MemberId);
                        MemberCache.Clear(mem.MemberId);
                        MemberCache.ClearApiCache(mem.MemberId);
                        SiteCache.ResetGroups();
                    }
                    MemberCache.Clear(RDN.Library.Classes.Account.User.GetMemberId());
                    return Redirect(Url.Content("~/league/groups?u=" + SiteMessagesEnum.sag));
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/league/groups?u=" + SiteMessagesEnum.sww));
        }

        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult GroupsSettings(LeagueGroup group)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                foreach (var mem in league.LeagueMembers)
                {
                    if (!String.IsNullOrEmpty(Request.Form[mem.MemberId + "-check"]))
                    {
                        bool cb = Request.Form[mem.MemberId + "-check"].ToString().Contains("true");

                        LeagueGroupMember m = new LeagueGroupMember();
                        m.MemberId = mem.MemberId;
                        if (!String.IsNullOrEmpty(HttpContext.Request.Form[mem.MemberId + "-memType"]))
                            m.MemberAccessLevelEnum = (GroupMemberAccessLevelEnum)Enum.Parse(typeof(GroupMemberAccessLevelEnum), HttpContext.Request.Form[mem.MemberId + "-memType"].ToString());
                        if (cb)
                        {
                            m.IsApartOfGroup = true;
                        }
                        group.GroupMembers.Add(m);

                    }
                }


                bool updated = LeagueGroupFactory.UpdateGroup(group);
                MemberCache.Clear(memId);
                MemberCache.ClearApiCache(memId);

                MemberCache.UpdateCurrentLeagueMemberCache(memId);
                if (updated)
                    return Redirect(Url.Content("~/league/groups?u=" + SiteMessagesEnum.sag));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/league/groups?u=" + SiteMessagesEnum.sww));
        }


        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult GroupSettings(string id)
        {
            var gId = Convert.ToInt64(id);
            var memId = RDN.Library.Classes.Account.User.GetMemberId();
            if (RDN.Library.Cache.MemberCache.IsModeratorOrBetterOfLeagueGroup(memId, gId) || RDN.Library.Cache.MemberCache.IsSecretaryOrBetterOfLeague(memId))
            {
                var mems = MemberCache.GetCurrentLeagueMembers(memId);
                var group = LeagueGroupFactory.GetGroup(gId, MemberCache.GetLeagueIdOfMember(memId));
                if (group == null)
                    return Redirect(Url.Content("~/league/groups?u=" + SiteMessagesEnum.na));
                group.League = MemberCache.GetLeagueOfMember(memId);
                for (int i = 0; i < group.League.LeagueMembers.Count; i++)
                {
                    if (group.GroupMembers.Where(x => x.MemberId == group.League.LeagueMembers[i].MemberId).FirstOrDefault() == null)
                    {
                        group.GroupMembers.Add(new LeagueGroupMember() { MemberId = group.League.LeagueMembers[i].MemberId, IsApartOfGroup = false, DerbyName = group.League.LeagueMembers[i].DerbyName, UserId = group.League.LeagueMembers[i].UserId });
                    }
                }

                ViewData["GroupMembers"] = new SelectList(mems, "MemberId", "DerbyName");
                ViewData["groupTypeSelectList"] = group.GroupTypeEnum.ToSelectList();
                return View(group);
            }
            return Redirect(Url.Content("~/league/groups?u=" + SiteMessagesEnum.na));
        }

        #endregion
        #region creation

        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult Setup(string id, string name)
        {
            try
            {
                //Dictionary<Guid, string> federations = Federation.GetFederations();
                Dictionary<int, string> countries = LocationFactory.GetCountriesDictionary();

                RegisterLeague league = new RegisterLeague();
                //league.Federations = federations.Select(item => new SelectListItem { Text = item.Value, Value = item.Key.ToString() }).ToList();
                league.Countries = countries.Select(item => new SelectListItem { Text = item.Value, Value = item.Key.ToString() }).ToList();
                league.Name = name;
                if (!String.IsNullOrEmpty(id))
                {
                    var leagueDb = RDN.Library.Classes.League.LeagueFactory.GetLeague(new Guid(id));
                    if (leagueDb.LeagueMembers.Where(x => x.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager) || x.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager)).Count() > 0)
                        return View(league);
                    league.LeagueId = leagueDb.LeagueId;
                    league.Name = leagueDb.Name;
                    league.Email = leagueDb.Email;
                    league.PhoneNumber = leagueDb.PhoneNumber;
                    league.City = leagueDb.City;
                    league.State = leagueDb.State;
                    if (!String.IsNullOrEmpty(leagueDb.Country))
                    {
                        var cou = league.Countries.Where(x => x.Value == leagueDb.CountryId.ToString()).FirstOrDefault();
                        if (cou != null)
                            league.Countries.Where(x => x.Value == leagueDb.CountryId.ToString()).FirstOrDefault().Selected = true;
                    }
                    else
                        league.Country = leagueDb.Country;

                    if (leagueDb.TimeZone != 0)
                    {
                        league.TimeZones.Where(x => x.Value == leagueDb.TimeZone.ToString("N0")).FirstOrDefault().Selected = true;
                    }


                    league.TimeZone = leagueDb.TimeZone;

                }

                return View(league);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/"));
        }
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult Setup(RegisterLeague model)
        {
            try
            {
                Dictionary<int, string> countries = LocationFactory.GetCountriesDictionary();

                //league.Federations = federations.Select(item => new SelectListItem { Text = item.Value, Value = item.Key.ToString() }).ToList();
                model.Countries = countries.Select(item => new SelectListItem { Text = item.Value, Value = item.Key.ToString() }).ToList();

                if (model.LeagueId != new Guid())
                {
                    var memberId = RDN.Library.Classes.Account.User.GetMemberId();
                    //updates the league and attaches the owner to it.
                    bool isUpdated = RDN.Library.Classes.League.LeagueFactory.UpdateLeagueForOwner(model.LeagueId, model.Name, model.Email, model.PhoneNumber, model.City, Convert.ToInt32(model.Country), model.State, model.TimeZone);

                    // RDN.Library.Classes.League.League.AttachOwnerToLeague(model.LeagueId, memberId);
                    RDN.Library.Classes.Account.User.AddMemberToLeague(memberId, model.LeagueId);

                    model.Updated = true;
                    MemberCache.Clear(memberId);
                    MemberCache.ClearApiCache(memberId);
                }
                else
                {
                    var memberId = RDN.Library.Classes.Account.User.GetMemberId();
                    var errors = RDN.Library.Classes.League.LeagueFactory.CreateLeague((new Guid()).ToString(), model.Name, model.PhoneNumber, model.Email, string.Empty, model.Country, model.State, model.City, model.TimeZone);
                    if (errors.Count == 0)
                    {
                        model.Created = true;
                        MemberCache.Clear(memberId);
                        MemberCache.ClearApiCache(memberId);
                    }
                    else
                    {
                        model.Created = false;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return View(model);
            }

            return View(model);
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult Join()
        {
            JoinLeague league = new JoinLeague();
            return View(league);
        }
        [Authorize]
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult Join(JoinLeague league)
        {
            try
            {
                Guid leagueCode;
                league.IsSuccess = Guid.TryParse(league.JoinCode, out leagueCode);
                if (league.IsSuccess)
                {
                    Guid leagueId = RDN.Library.Classes.League.LeagueFactory.GetLeagueAndJoinWithJoinCode(new Guid(league.JoinCode));
                    if (leagueId != new Guid())
                    {
                        league.IsSuccess = true;
                        MemberCache.ClearLeagueMembersCache(leagueId);
                        MemberCache.ClearLeagueMembersApiCache(leagueId);
                    }
                    else
                        league.IsSuccess = false;
                }
                else
                {
                    league.Message = "Join code doesn't look accurate.  Please make sure its the full code.";
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(league);
        }

        #endregion
        #region members
        /// <summary>
        /// exports an excel sheet of the members roster for the league.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsSecretary = true, IsManager = true)]
        public ActionResult ExportMembersRoster()
        {
            var memId = RDN.Library.Classes.Account.User.GetMemberId();
            var league = MemberCache.GetLeagueOfMember(memId);
            ViewBag.LeagueName = league.Name;

            using (ExcelPackage p = new ExcelPackage())
            {
                try
                {
                    p.Workbook.Properties.Author = "RDNation.com";
                    p.Workbook.Properties.Title = "Roster For " + league.Name;

                    //we create the first sheet.
                    ExcelWorksheet reportSheet = p.Workbook.Worksheets.Add("Roster");
                    reportSheet.Name = "Roster"; //Setting Sheet's name
                    reportSheet.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                    reportSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
                    reportSheet.Cells[1, 1].Value = "Derby Name";
                    reportSheet.Cells[1, 2].Value = "#";
                    reportSheet.Cells[1, 3].Value = "First Name";
                    reportSheet.Cells[1, 4].Value = "Last Name";
                    reportSheet.Cells[1, 5].Value = "Phone Number";
                    reportSheet.Cells[1, 6].Value = "Email";
                    reportSheet.Cells[1, 7].Value = "M/F";
                    reportSheet.Cells[1, 8].Value = "Inactive";
                    reportSheet.Cells[1, 9].Value = "Joined";

                    int rowReport = 2;
                    //create the remaining sheets with the names.
                    foreach (var attendee in league.LeagueMembers)
                    {
                        try
                        {

                            reportSheet.Cells[rowReport, 1].Value = attendee.DerbyName;
                            reportSheet.Cells[rowReport, 2].Value = attendee.PlayerNumber;
                            reportSheet.Cells[rowReport, 3].Value = attendee.Firstname;
                            reportSheet.Cells[rowReport, 4].Value = attendee.LastName;
                            reportSheet.Cells[rowReport, 5].Value = attendee.PhoneNumber;
                            reportSheet.Cells[rowReport, 6].Value = attendee.Email;
                            reportSheet.Cells[rowReport, 7].Value = attendee.Gender;
                            reportSheet.Cells[rowReport, 8].Value = attendee.IsInactiveFromCurrentLeague;
                            reportSheet.Cells[rowReport, 9].Value = attendee.IsConnected;

                            rowReport += 1;
                        }
                        catch (Exception exception)
                        {
                            ErrorDatabaseManager.AddException(exception, exception.GetType());
                        }
                    }
                    reportSheet.Cells["A1:K20"].AutoFitColumns();
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
                //Generate A File with Random name
                Byte[] bin = p.GetAsByteArray();
                string file = "RosterReport_" + DateTime.UtcNow.ToString("yyyyMMdd") + ".xlsx";
                return File(bin, RDN.Utilities.IO.FileExt.GetMIMEType(file), file);
            }
        }

        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsSecretary = true)]
        public ActionResult ExportGroupsOfLeague()
        {
            var memId = RDN.Library.Classes.Account.User.GetMemberId();
            var league = MemberCache.GetLeagueOfMember(memId);

            ViewBag.LeagueName = league.Name;

            using (ExcelPackage p = new ExcelPackage())
            {
                try
                {
                    p.Workbook.Properties.Author = "RDNation.com";
                    p.Workbook.Properties.Title = "Groups For " + league.Name;

                    //we create the first sheet.
                    ExcelWorksheet reportSheet = p.Workbook.Worksheets.Add("Roster");
                    reportSheet.Name = "Groups"; //Setting Sheet's name
                    reportSheet.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                    reportSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
                    reportSheet.Cells[1, 1].Value = "Group";
                    reportSheet.Cells[1, 2].Value = "Members";
                    reportSheet.Cells[1, 3].Value = "Type";
                    reportSheet.Cells[1, 4].Value = "Email";
                    reportSheet.Cells[1, 5].Value = "Is Public";

                    int rowReport = 2;
                    //create the remaining sheets with the names.
                    foreach (var group in league.Groups)
                    {
                        try
                        {

                            reportSheet.Cells[rowReport, 1].Value = group.GroupName;
                            reportSheet.Cells[rowReport, 2].Value = group.GroupMembers.Count;
                            reportSheet.Cells[rowReport, 3].Value = group.GroupTypeEnum.ToString();
                            reportSheet.Cells[rowReport, 4].Value = group.EmailAddress;
                            reportSheet.Cells[rowReport, 5].Value = group.IsPublicToWorld;

                            ExcelWorksheet groupSheet = p.Workbook.Worksheets.Add(RDN.Utilities.Strings.StringExt.ToExcelFriendly(group.GroupName));
                            groupSheet.Name = RDN.Utilities.Strings.StringExt.ToExcelFriendly(group.GroupName); //Setting Sheet's name
                            groupSheet.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                            groupSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
                            groupSheet.Cells[1, 1].Value = "Derby Name";
                            groupSheet.Cells[1, 2].Value = "Number";
                            groupSheet.Cells[1, 3].Value = "First Name";
                            groupSheet.Cells[1, 4].Value = "Last Name";
                            groupSheet.Cells[1, 5].Value = "Phone Number";
                            groupSheet.Cells[1, 6].Value = "Email";

                            int groupRow = 2;
                            foreach (var member in group.GroupMembers)
                            {
                                groupSheet.Cells[groupRow, 1].Value = member.DerbyName;
                                groupSheet.Cells[groupRow, 2].Value = member.PlayerNumber;
                                groupSheet.Cells[groupRow, 3].Value = member.Firstname;
                                groupSheet.Cells[groupRow, 4].Value = member.LastName;
                                groupSheet.Cells[groupRow, 5].Value = member.PhoneNumber;
                                groupSheet.Cells[groupRow, 6].Value = member.Email;
                                groupRow += 1;
                            }
                            groupSheet.Cells["A1:K20"].AutoFitColumns();
                            rowReport += 1;
                        }
                        catch (Exception exception)
                        {
                            ErrorDatabaseManager.AddException(exception, exception.GetType());
                        }
                    }
                    reportSheet.Cells["A1:K20"].AutoFitColumns();
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
                //Generate A File with Random name
                Byte[] bin = p.GetAsByteArray();
                string file = "GroupsReport_" + DateTime.UtcNow.ToString("yyyyMMdd") + ".xlsx";
                return File(bin, RDN.Utilities.IO.FileExt.GetMIMEType(file), file);
            }
        }


        /// <summary>
        /// adds an owner type to the league.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="ownerType"></param>
        /// <returns></returns>
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true)]
        public ActionResult AddOwnerType(string memberId, string ownerType)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var leagueId = MemberCache.GetLeagueIdOfMember(memId);

                LeagueOwnersEnum oe = LeagueOwnersEnum.None;
                if (ownerType == "o")
                    oe = LeagueOwnersEnum.Owner;
                else if (ownerType == "m")
                    oe = LeagueOwnersEnum.Manager;
                else if (ownerType == "t")
                    oe = LeagueOwnersEnum.Treasurer;
                else if (ownerType == "s")
                    oe = LeagueOwnersEnum.Secretary;
                else if (ownerType == "r")
                    oe = LeagueOwnersEnum.Head_Ref;
                else if (ownerType == "sm")
                    oe = LeagueOwnersEnum.Shops;
                else if (ownerType == "ec")
                    oe = LeagueOwnersEnum.Events_Coord;
                else if (ownerType == "med")
                    oe = LeagueOwnersEnum.Medical;
                else if (ownerType == "attn")
                    oe = LeagueOwnersEnum.Attendance;
                else if (ownerType == "poll")
                    oe = LeagueOwnersEnum.Polls;
                else if (ownerType == "inventory")
                    oe = LeagueOwnersEnum.Inventory;
                else if (ownerType == "sponsor")
                    oe = LeagueOwnersEnum.Sponsorship;

                //can't assign an owner if you are not the owner.
                if (oe == LeagueOwnersEnum.Owner)
                    if (!MemberCache.IsOwnerOfLeague(memId))
                        return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);


                //can't assign a manager if you are not already a manager or owner.
                if (oe == LeagueOwnersEnum.Manager)
                    if (!MemberCache.IsManagerOrBetterOfLeague(memId))
                        return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);


                bool worked = RDN.Library.Classes.League.LeagueFactory.ToggleOwnerToLeague(leagueId, new Guid(memberId), oe);

                RDN.Library.Cache.MemberCache.Clear(new Guid(memberId));
                MemberCache.ClearApiCache(new Guid(memberId));
                if (new Guid(memberId) != memId)
                    RDN.Library.Cache.MemberCache.Clear(memId);
                return Json(new { isSuccess = worked }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, IsSecretary = true)]
        public ActionResult RemoveMember(MemberDisplay model)
        {
            try
            {
                //current member who is removing the other member from the league
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var currentLeagueId = MemberCache.GetLeagueIdOfMember(memId);
                RDN.Library.Classes.League.LeagueFactory.DisconnectMemberFromLeague(currentLeagueId, model.MemberId);
                RDN.Library.Cache.MemberCache.Clear(model.MemberId);
                RDN.Library.Cache.MemberCache.Clear(memId);
                MemberCache.ClearApiCache(model.MemberId);

                MemberCache.ClearApiCache(memId);
                ViewBag.Removed = true;
                ViewBag.Saved = false;
                return Redirect(Url.Content("~/league/members/view/all"));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/"));
        }

        /// <summary>
        /// allows the federation to edit certain things about this member.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, IsSecretary = true)]
        public ActionResult EditMember(string id, string name)
        {
            try
            {
                //this is the member that is currently managing this league.
                var usersLeague = MemberCache.GetMemberDisplay(RDN.Library.Classes.Account.User.GetMemberId());
                var mem = MemberCache.GetMemberDisplay(new Guid(id));
                mem.MemberId = new Guid(id);
                var league = mem.Leagues.Where(x => x.LeagueId == usersLeague.CurrentLeagueId).FirstOrDefault();
                if (league != null)
                {
                    mem.Leagues.Clear();
                    mem.Leagues.Add(league);
                }

                ViewBag.Saved = false;
                ViewBag.Removed = false;
                return View(mem);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/"));
        }

        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, IsSecretary = true)]
        public ActionResult AddSkaterClass()
        {
            return View(new LeagueMemberClass());
        }

        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, IsSecretary = true)]
        public ActionResult AddSkaterClass(LeagueMemberClass memClass)
        {
            LeagueMemberClass.CreateNewClass(memClass.NameOfClass);
            //SiteCache.UpdateLeagueMemberClasses();
            NameValueCollection nameValueCollection;
            if (Request.UrlReferrer != null)
                nameValueCollection = HttpUtility.ParseQueryString(Request.UrlReferrer.Query);
            else
                nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
            string returnUrl = nameValueCollection["returnUrl"];
            SiteMessage message = new SiteMessage();
            message.MessageType = SiteMessageType.Success;
            message.Message = "Successfully Added New Class.";
            this.AddMessage(message);
            memClass.NameOfClass = "";

            // if a create another was clicked instead of just submitting the event.
            if (Request.Form["addAnother"] != null)
                return View(memClass);

            if (!String.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return View(memClass);
        }

        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false, IsManager = true, IsSecretary = true)]
        public ActionResult EditMember(MemberDisplay model, HttpPostedFileBase file)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                //this is the member that is currently managing this league.
                var usersLeague = MemberCache.GetMemberDisplay(memId);
                if (model == null)
                {
                    throw new Exception("Model is Null");
                }
                var memTemp = RDN.Library.Cache.MemberCache.GetMemberDisplay(model.MemberId);

                ViewBag.Saved = true;
                ViewBag.Removed = false;
                MemberDisplay newMemberWithImage = null;

                model.Leagues = memTemp.Leagues;

                foreach (var leag in model.Leagues)
                {
                    try
                    {
                        if (HttpContext.Request.Form["LEAGUEIsInactive[" + leag.LeagueId + "]"] != null)
                        {
                            if (HttpContext.Request.Form["LEAGUEIsInactive[" + leag.LeagueId + "]"].ToString() == "false")
                                leag.IsInactiveInLeague = false;
                            else
                                leag.IsInactiveInLeague = true;
                        }
                        if (!String.IsNullOrEmpty(HttpContext.Request.Form[leag.LeagueId + "-LEAGUEMembershipDate"]))
                        {
                            DateTime outDT;
                            bool success = DateTime.TryParse(HttpContext.Request.Form[leag.LeagueId + "-LEAGUEMembershipDate"], out outDT);
                            if (success)
                                leag.MembershipDate = outDT;
                            else
                                leag.MembershipDate = null;
                        }

                        if (!String.IsNullOrEmpty(HttpContext.Request.Form[leag.LeagueId + "-LEAGUEDepartureDate"]))
                        {
                            DateTime outDT;
                            bool success = DateTime.TryParse(HttpContext.Request.Form[leag.LeagueId + "-LEAGUEDepartureDate"], out outDT);
                            if (success)
                                leag.DepartureDate = outDT;
                            else
                                leag.DepartureDate = null;
                        }

                        if (!String.IsNullOrEmpty(HttpContext.Request.Form[leag.LeagueId + "-LEAGUEPassedWrittenExam"]))
                        {
                            DateTime outDT;
                            bool success = DateTime.TryParse(HttpContext.Request.Form[leag.LeagueId + "-LEAGUEPassedWrittenExam"], out outDT);
                            if (success)
                                leag.PassedWrittenExam = outDT;
                            else
                                leag.PassedWrittenExam = null;
                        }

                        if (!String.IsNullOrEmpty(HttpContext.Request.Form[leag.LeagueId + "-LEAGUESkillsDate"]))
                        {
                            DateTime outDT;
                            bool success = DateTime.TryParse(HttpContext.Request.Form[leag.LeagueId + "-LEAGUESkillsDate"], out outDT);
                            if (success)
                                leag.SkillsTestDate = outDT;
                            else
                                leag.SkillsTestDate = null;
                        }

                        if (HttpContext.Request.Form[leag.LeagueId + "-LEAGUEMemberNotes"] != null)
                            leag.NotesAboutMember = HttpContext.Request.Form[leag.LeagueId + "-LEAGUEMemberNotes"];

                        if (!String.IsNullOrEmpty(HttpContext.Request.Form["LEAGUESkaterClass[" + leag.LeagueId + "]"]))
                            leag.SkaterClass = Convert.ToInt64(HttpContext.Request.Form["LEAGUESkaterClass[" + leag.LeagueId + "]"]);
                        else
                            leag.SkaterClass = 0;
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: model.MemberId.ToString());
                    }
                }

                if (file != null)
                    newMemberWithImage = RDN.Library.Classes.Account.User.UpdateMemberDisplayForLeague(model, file.InputStream, file.FileName);
                else
                    newMemberWithImage = RDN.Library.Classes.Account.User.UpdateMemberDisplayForLeague(model);
                //clear users cache
                RDN.Library.Cache.MemberCache.Clear(memId);
                MemberCache.ClearApiCache(memId);
                //clears members cache
                RDN.Library.Cache.MemberCache.Clear(newMemberWithImage.MemberId);
                MemberCache.ClearApiCache(newMemberWithImage.MemberId);
                var mem = RDN.Library.Cache.MemberCache.GetMemberDisplay(model.MemberId);
                var league = mem.Leagues.Where(x => x.LeagueId == usersLeague.CurrentLeagueId).FirstOrDefault();
                if (league != null)
                {
                    mem.Leagues.Clear();
                    mem.Leagues.Add(league);
                }
                return View(mem);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: model.MemberId.ToString());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult ViewMembers(string refresh = "")
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);

                if (refresh.Length > 1)
                {
                    LeagueFactory.RefreshLeagueJoinCode(league.LeagueId);
                    MemberCache.Clear(memId);
                    league = MemberCache.GetLeagueOfMember(memId);

                }



                ViewBag.LeagueName = league.Name;
                ViewBag.JoinCode = league.JoinCode.ToString().Replace("-", "");
                ViewBag.LeagueId = league.LeagueId.ToString().Replace("-", "");


                var model = new SimpleModPager<MemberDisplay>();
                model.CurrentPage = 1;
                model.NumberOfRecords = RDN.Library.Classes.League.LeagueFactory.GetNumberOfMembersInLeague(league.LeagueId);
                model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / PAGE_SIZE);

                var output = FillMembersModel(model, league.LeagueId, memId);
                return View(output);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww.ToString()));
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult ViewMembersRemoved()
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);

                ViewBag.LeagueName = league.Name;
                ViewBag.JoinCode = league.JoinCode.ToString().Replace("-", "");
                ViewBag.LeagueId = league.LeagueId.ToString().Replace("-", "");

                var model = new SimpleModPager<MemberDisplay>();
                model.CurrentPage = 1;
                model.NumberOfRecords = RDN.Library.Classes.League.LeagueFactory.GetNumberOfMembersInLeague(league.LeagueId);
                model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / PAGE_SIZE);

                var output = FillMembersModel(model, league.LeagueId, memId, true);
                return View(output);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww.ToString()));
        }

        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult ViewMembersMap()
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);

                ViewBag.LeagueName = league.Name;
                ViewBag.LeagueId = league.LeagueId.ToString().Replace("-", "");
                List<SkaterJson> Members = new List<SkaterJson>();
                var mems = MemberCache.GetLeagueMembers(memId, league.LeagueId);

                foreach (var mem in mems)
                {

                    var memObj = new SkaterJson { DerbyName = mem.DerbyName };

                    if (mem.ContactCard != null && mem.ContactCard.Addresses.FirstOrDefault() != null)
                    {
                        var add = mem.ContactCard.Addresses.FirstOrDefault();
                        memObj.Address1 = add.Address1;
                        memObj.Address2 = add.Address2;
                        memObj.City = add.CityRaw;
                        memObj.State = add.StateRaw;
                        memObj.Zip = add.Zip;
                        memObj.Country = add.Country;
                        if (add.Coords != null)
                        {
                            memObj.Latitude = add.Coords.Latitude.ToString();
                            memObj.Longitude = add.Coords.Longitude.ToString();
                        }
                    }
                    var photo = mem.Photos.Where(x => x.IsPrimaryPhoto == true).FirstOrDefault();
                    if (photo != null)
                    {
                        memObj.photoUrl = photo.ImageUrl;
                        memObj.ThumbUrl = photo.ImageThumbUrl;
                    }
                    Members.Add(memObj);


                }

                return View(Members);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww.ToString()));
        }

        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public ActionResult ViewMembersReportBuilder()
        {
            try
            {

                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                LeagueReportBuilderModel model = new LeagueReportBuilderModel();
                model.LeagueName = league.Name;
                model.LeagueId = league.LeagueId;
                model.SavedReports = new SelectList(LeagueReportBuilder.GetReports(league.LeagueId), "ReportId", "Name");
                model.ColumnsAvailable = Enum.GetValues(typeof(MembersReportEnum)).Cast<MembersReportEnum>().OrderBy(x => x.ToString()).ToList();
                return View(model);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww.ToString()));
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, IsSecretary = true)]
        public JsonResult RemoveLeagueReport(string reportId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                var success = LeagueReportBuilder.DeleteReport(league.LeagueId, Convert.ToInt64(reportId));

                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = "false" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public ActionResult ViewMembersReportBuild(LeagueReportBuilderModel model)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
               
                var league = MemberCache.GetLeagueOfMember(memId);
                //MembersReportEnum en = new MembersReportEnum();
                long reportSelected = 0;

                if (Int64.TryParse(model.SelectedReport, out reportSelected))
                {
                    var oldReport = LeagueReportBuilder.GetReport(league.LeagueId, reportSelected);
                    model.SelectedColumnsHidden = oldReport.leagueReportEnums;
                    model.SaveReport = false;
                    model.SavedReportName = oldReport.Name;
                }

               

                if (String.IsNullOrEmpty(model.SavedReportName))
                    model.SavedReportName = "ReportBuilder";
                if (model.SaveReport)
                    LeagueReportBuilder.SaveReport(league.LeagueId, model.SelectedColumnsHidden, model.SavedReportName);

                using (ExcelPackage p = RDN.Library.Classes.League.ReportBuilder.PrepareExcelWorkBook(model.SelectedColumnsHidden, league))
                {
                    p.Workbook.Properties.Author = "RDNation.com";
                    p.Workbook.Properties.Title = "Report For " + league.Name;
                    //Generate A File with Random name
                    Byte[] bin = p.GetAsByteArray();
                    if (String.IsNullOrEmpty(model.SavedReportName))
                        model.SavedReportName = "ReportBuilder";
                    string file = model.SavedReportName + "_" + DateTime.UtcNow.ToString("yyyyMMdd") + ".xlsx";
                    Response.Headers.Add("Content-Type", RDN.Utilities.IO.FileExt.GetMIMEType(file));
                    Response.AddHeader("Content-Length", bin.Length.ToString());
                    Response.ContentType = RDN.Utilities.IO.FileExt.GetMIMEType(file);
                    
                    return File(bin, RDN.Utilities.IO.FileExt.GetMIMEType(file), file);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: "columns:" + model.SelectedColumnsHidden);
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww.ToString()));
        }

       

        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public ActionResult ViewMembersInsurance()
        {
            try
            {

                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                ViewBag.LeagueName = league.Name;
                ViewBag.JoinCode = league.JoinCode.ToString().Replace("-", "");
                ViewBag.LeagueId = league.LeagueId.ToString().Replace("-", "");

                var model = new SimpleModPager<MemberDisplay>();
                model.CurrentPage = 1;
                model.NumberOfRecords = RDN.Library.Classes.League.LeagueFactory.GetNumberOfMembersInLeague(league.LeagueId);
                model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / PAGE_SIZE);

                var output = FillMembersModel(model, league.LeagueId, memId);
                return View(output);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww.ToString()));
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsSecretary = true)]
        public ActionResult ViewMembersDates()
        {
            try
            {

                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                ViewBag.LeagueName = league.Name;
                ViewBag.JoinCode = league.JoinCode.ToString().Replace("-", "");
                ViewBag.LeagueId = league.LeagueId.ToString().Replace("-", "");

                var model = new SimpleModPager<MemberDisplay>();
                model.CurrentPage = 1;
                model.NumberOfRecords = RDN.Library.Classes.League.LeagueFactory.GetNumberOfMembersInLeague(league.LeagueId);
                model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / PAGE_SIZE);

                var output = FillMembersModel(model, league.LeagueId, memId);
                return View(output);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww.ToString()));
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult ViewMembersJobs()
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                ViewBag.LeagueName = league.Name;
                ViewBag.LeagueId = league.LeagueId.ToString().Replace("-", "");

                var model = new SimpleModPager<MemberDisplay>();
                model.CurrentPage = 1;
                model.NumberOfRecords = RDN.Library.Classes.League.LeagueFactory.GetNumberOfMembersInLeague(league.LeagueId);
                model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / PAGE_SIZE);

                var output = FillMembersModel(model, league.LeagueId, memId);
                return View(output);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww.ToString()));
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true, IsMedical = true)]
        public ActionResult ViewMembersMedical()
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                ViewBag.LeagueName = league.Name;
                ViewBag.JoinCode = league.JoinCode.ToString().Replace("-", "");
                ViewBag.LeagueId = league.LeagueId.ToString().Replace("-", "");

                var model = new SimpleModPager<MemberDisplay>();
                model.CurrentPage = 1;
                model.NumberOfRecords = RDN.Library.Classes.League.LeagueFactory.GetNumberOfMembersInLeague(league.LeagueId);
                model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / PAGE_SIZE);

                var output = FillMembersModel(model, league.LeagueId, memId);
                return View(output);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww.ToString()));
        }




        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult ViewMembersClassification()
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                ViewBag.LeagueName = league.Name;
                ViewBag.JoinCode = league.JoinCode.ToString().Replace("-", "");
                ViewBag.LeagueId = league.LeagueId.ToString().Replace("-", "");

                var model = new SimpleModPager<MemberDisplay>();
                model.CurrentPage = 1;
                model.NumberOfRecords = RDN.Library.Classes.League.LeagueFactory.GetNumberOfMembersInLeague(league.LeagueId);
                model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / PAGE_SIZE);

                var output = FillMembersModel(model, league.LeagueId, memId);
                return View(output);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww.ToString()));
        }

        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true)]
        public ActionResult ViewMembersPermissions()
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                ViewBag.LeagueName = league.Name;
                ViewBag.JoinCode = league.JoinCode.ToString().Replace("-", "");
                ViewBag.LeagueId = league.LeagueId.ToString().Replace("-", "");

                var model = new SimpleModPager<MemberDisplay>();
                model.CurrentPage = 1;
                model.NumberOfRecords = RDN.Library.Classes.League.LeagueFactory.GetNumberOfMembersInLeague(league.LeagueId);
                model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / PAGE_SIZE);

                var output = FillMembersModel(model, league.LeagueId, memId);
                return View(output);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww.ToString()));
        }


        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public FileContentResult ViewMembersDatesExportExcel()
        {
            try
            {
                var league = RDN.Library.Classes.League.LeagueFactory.GetLeague(MemberCache.GetLeagueIdOfMember(RDN.Library.Classes.Account.User.GetMemberId()));
                var members = RDN.Library.Classes.League.LeagueFactory.GetLeagueMembersDisplay(league.LeagueId);

                using (ExcelPackage p = new ExcelPackage())
                {
                    p.Workbook.Properties.Author = "RDNation";
                    p.Workbook.Properties.Title = "Date For Member with " + league.Name;
                    p.Workbook.Worksheets.Add("Dates For Members");
                    ExcelWorksheet ws = p.Workbook.Worksheets[1];
                    ws.Name = "Dates For Members"; //Setting Sheet's name
                    ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                    ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

                    DataTable dt = new DataTable();
                    for (int i = 0; i < 7; i++)
                    {
                        dt.Columns.Add(i.ToString());
                    }

                    DataRow dr0 = dt.NewRow();
                    dr0[0] = "First Name";
                    dr0[1] = "Last Name";
                    dr0[2] = "Derby Name";
                    dr0[3] = "Derby Number";
                    dr0[4] = "Skills Date";
                    dr0[5] = "Date of Birth";
                    dr0[6] = "Departure Date";
                    dt.Rows.Add(dr0);
                    // write the details
                    foreach (var person in members)
                    {
                        DataRow dr1 = dt.NewRow();
                        dr1[0] = person.Firstname;
                        dr1[1] = person.LastName;
                        dr1[2] = person.DerbyName;
                        dr1[3] = person.PlayerNumber;
                        if (person.Leagues.FirstOrDefault().SkillsTestDate.HasValue)
                            dr1[4] = person.Leagues.FirstOrDefault().SkillsTestDate.Value.ToShortDateString();
                        if (person.DOB > new DateTime())
                            dr1[5] = person.DOB.ToShortDateString();
                        if (person.Leagues.FirstOrDefault().DepartureDate.HasValue)
                            dr1[6] = person.Leagues.FirstOrDefault().DepartureDate.Value.ToShortDateString();
                        dt.Rows.Add(dr1);
                    }
                    int colIndex = 0;
                    int rowIndex = 0;
                    foreach (DataRow dr in dt.Rows) // Adding Data into rows
                    {
                        colIndex = 1;
                        rowIndex++;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            var cell = ws.Cells[rowIndex, colIndex];
                            //Setting Value in cell
                            cell.Value = dr[dc.ColumnName];
                            colIndex++;
                        }
                    }
                    ws.Cells["A1:K20"].AutoFitColumns();
                    //Generate A File with Random name
                    Byte[] bin = p.GetAsByteArray();
                    string file = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(league.Name) + "_Dates_" + DateTime.UtcNow.ToString("yyyymmdd") + ".xlsx";

                    return File(bin, RDN.Utilities.IO.FileExt.GetMIMEType(file), file);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public FileContentResult ViewMembersJobsExportExcel()
        {
            try
            {
                var league = RDN.Library.Classes.League.LeagueFactory.GetLeague(MemberCache.GetLeagueIdOfMember(RDN.Library.Classes.Account.User.GetMemberId()));
                var members = RDN.Library.Classes.League.LeagueFactory.GetLeagueMembersDisplay(league.LeagueId);

                using (ExcelPackage p = new ExcelPackage())
                {
                    p.Workbook.Properties.Author = "RDNation";
                    p.Workbook.Properties.Title = "Jobs For Member with " + league.Name;
                    p.Workbook.Worksheets.Add("Jobs For Members");
                    ExcelWorksheet ws = p.Workbook.Worksheets[1];
                    ws.Name = "Jobs For Members"; //Setting Sheet's name
                    ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                    ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

                    DataTable dt = new DataTable();
                    for (int i = 0; i < 7; i++)
                    {
                        dt.Columns.Add(i.ToString());
                    }

                    DataRow dr0 = dt.NewRow();
                    dr0[0] = "First Name";
                    dr0[1] = "Last Name";
                    dr0[2] = "Derby Name";
                    dr0[3] = "Derby Number";
                    dr0[4] = "Job";

                    dt.Rows.Add(dr0);
                    // write the details
                    foreach (var person in members)
                    {
                        DataRow dr1 = dt.NewRow();
                        dr1[0] = person.Firstname;
                        dr1[1] = person.LastName;
                        dr1[2] = person.DerbyName;
                        dr1[3] = person.PlayerNumber;
                        dr1[4] = person.DayJob;
                        dt.Rows.Add(dr1);
                    }
                    int colIndex = 0;
                    int rowIndex = 0;
                    foreach (DataRow dr in dt.Rows) // Adding Data into rows
                    {
                        colIndex = 1;
                        rowIndex++;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            var cell = ws.Cells[rowIndex, colIndex];
                            //Setting Value in cell
                            cell.Value = dr[dc.ColumnName];
                            colIndex++;
                        }
                    }
                    ws.Cells["A1:K20"].AutoFitColumns();
                    //Generate A File with Random name
                    Byte[] bin = p.GetAsByteArray();
                    string file = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(league.Name) + "_Jobs_" + DateTime.UtcNow.ToString("yyyymmdd") + ".xlsx";

                    return File(bin, RDN.Utilities.IO.FileExt.GetMIMEType(file), file);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public FileContentResult ViewMembersInsuranceExportExcel()
        {
            try
            {
                var league = RDN.Library.Classes.League.LeagueFactory.GetLeague(MemberCache.GetLeagueIdOfMember(RDN.Library.Classes.Account.User.GetMemberId()));
                var members = RDN.Library.Classes.League.LeagueFactory.GetLeagueMembersDisplay(league.LeagueId);
                using (ExcelPackage p = new ExcelPackage())
                {
                    p.Workbook.Properties.Author = "RDNation";
                    p.Workbook.Properties.Title = "Insurance For Member with " + league.Name;
                    p.Workbook.Worksheets.Add("Jobs For Members");
                    ExcelWorksheet ws = p.Workbook.Worksheets[1];
                    ws.Name = "Insurance For Members"; //Setting Sheet's name
                    ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                    ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

                    DataTable dt = new DataTable();
                    for (int i = 0; i < 12; i++)
                    {
                        dt.Columns.Add(i.ToString());
                    }

                    DataRow dr0 = dt.NewRow();
                    dr0[0] = "First Name";
                    dr0[1] = "Last Name";
                    dr0[2] = "Derby Name";
                    dr0[3] = "Derby Number";
                    dr0[4] = "WFTDA";
                    dr0[5] = "WFTDA Expires";
                    dr0[6] = "USARS";
                    dr0[7] = "USARS Expires";
                    dr0[8] = "CRDI";
                    dr0[9] = "CRDI Expires";
                    dr0[10] = "Other";
                    dr0[11] = "Other Expires";

                    dt.Rows.Add(dr0);
                    // write the details
                    foreach (var person in members)
                    {
                        DataRow dr1 = dt.NewRow();
                        dr1[0] = person.Firstname;
                        dr1[1] = person.LastName;
                        dr1[2] = person.DerbyName;
                        dr1[3] = person.PlayerNumber;
                        dr1[4] = person.InsuranceNumWftda;
                        if (person.InsuranceNumWftdaExpires.HasValue)
                            dr1[5] = person.InsuranceNumWftdaExpires.Value.ToShortDateString();
                        dr1[6] = person.InsuranceNumUsars;
                        if (person.InsuranceNumUsarsExpires.HasValue)
                            dr1[7] = person.InsuranceNumUsarsExpires.Value.ToShortDateString();
                        dr1[8] = person.InsuranceNumCRDI;
                        if (person.InsuranceNumCRDIExpires.HasValue)
                            dr1[9] = person.InsuranceNumCRDIExpires.Value.ToShortDateString();
                        dr1[10] = person.InsuranceNumOther;
                        if (person.InsuranceNumOtherExpires.HasValue)
                            dr1[11] = person.InsuranceNumOtherExpires.Value.ToShortDateString();

                        dt.Rows.Add(dr1);
                    }
                    int colIndex = 0;
                    int rowIndex = 0;
                    foreach (DataRow dr in dt.Rows) // Adding Data into rows
                    {
                        colIndex = 1;
                        rowIndex++;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            var cell = ws.Cells[rowIndex, colIndex];
                            //Setting Value in cell
                            cell.Value = dr[dc.ColumnName];
                            colIndex++;
                        }
                    }
                    ws.Cells["A1:K20"].AutoFitColumns();
                    //Generate A File with Random name
                    Byte[] bin = p.GetAsByteArray();
                    string file = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(league.Name) + "_Insurance_" + DateTime.UtcNow.ToString("yyyymmdd") + ".xlsx";

                    return File(bin, RDN.Utilities.IO.FileExt.GetMIMEType(file), file);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;

        }

        /// <summary>
        /// creates a post from the page and allows us to page the members page.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult ViewMembers(SimpleModPager<MemberDisplay> model)
        {
            try
            {
                Guid memId = RDN.Library.Classes.Account.User.GetMemberId();
                var l = MemberCache.GetLeagueOfMember(memId);

                var output = FillMembersModel(model, l.LeagueId, memId);
                return View(output);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/"));
        }

        /// <summary>
        /// the view for adding members to the federation.
        /// </summary>
        /// <returns></returns>
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, IsSecretary = true)]
        public ActionResult AddMembers()
        {
            try
            {
                var league = RDN.Library.Classes.League.LeagueFactory.GetLeague(MemberCache.GetLeagueIdOfMember(RDN.Library.Classes.Account.User.GetMemberId()));
                ViewBag.League = league;
                return View(league);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/"));
        }
        /// <summary>
        /// bulk adds members to the federation.
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, IsSecretary = true)]
        public JsonResult AddMembers(List<MemberDisplay> members)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var leagueId = MemberCache.GetLeagueIdOfMember(memId);
                if (members != null)
                {
                    foreach (var mem in members)
                        RDN.Library.Classes.Account.User.CreateMemberForLeague(mem, leagueId);

                    MemberCache.Clear(memId);
                    MemberCache.ClearApiCache(memId);
                    var url = Url.Content("~/League/Home");
                    return Json(new { result = "true", url = url });
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { result = "false", url = "" });
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, IsSecretary = true)]
        public JsonResult SendEmailInviteToMemberAgain(string leagueName, string memberId)
        {
            try
            {
                string email = RDN.Library.Classes.Account.User.SendEmailInviteOnProfileCreation(leagueName, new Guid(memberId));
                if (!String.IsNullOrEmpty(email))
                    return Json(new { isSuccess = true, emailSent = email }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { isSuccess = false, emailSent = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = "false" }, JsonRequestBehavior.AllowGet);
        }



        #endregion
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true)]
        public ActionResult ViewTeams()
        {
            try
            {
                var league = RDN.Library.Classes.League.LeagueFactory.GetLeague(MemberCache.GetLeagueIdOfMember(RDN.Library.Classes.Account.User.GetMemberId()));
                ViewBag.LeagueName = league.Name;

                return View(league.Teams);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/"));
        }

        #region league
        /// <summary>
        /// gets the members of a league.
        /// </summary>
        /// <param name="leagueId"></param>
        /// <returns></returns>
        public JsonResult GetMembersOfLeague(string leagueId)
        {
            try
            {
                var members = RDN.Library.Classes.League.LeagueFactory.GetLeagueMembers(new Guid(leagueId));
                return Json(members, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// converts the large members list into a single model to display on the view.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="federationId"></param>
        /// <returns></returns>
        private GenericSingleModel<SimpleModPager<MemberDisplay>> FillMembersModel(SimpleModPager<MemberDisplay> model, Guid leagueId, Guid memId, bool hasLeftLeague = false)
        {
            for (var i = 1; i <= model.NumberOfPages; i++)
                model.Pages.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                    Selected = i == model.CurrentPage
                });
            var output = new GenericSingleModel<SimpleModPager<MemberDisplay>> { Model = model };
            output.Model.Items = MemberCache.GetLeagueMembers(memId, leagueId, (model.CurrentPage - 1) * PAGE_SIZE, PAGE_SIZE, hasLeftLeague);
            return output;
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, IsSecretary = true)]
        public ActionResult EditLeague(string id)
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully Updated League.";
                    this.AddMessage(message);
                }

                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var leagues = MemberCache.GetAllOwnedLeagues(memId);

                bool isAdmin = MemberCache.IsManagerOrBetterOfLeague(memId);
                if (!isAdmin)
                    if (leagues == null || leagues.Where(x => x.LeagueId == new Guid(id)).FirstOrDefault() == null)
                        return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                var league = RDN.Library.Classes.League.LeagueFactory.GetLeague(new Guid(id));
                SetCulture(league.CultureSelected);

                LeagueModel leag = new LeagueModel();
                leag.InternalWelcomeMessageModel = league.InternalWelcomeMessage;
                leag.Address = league.Address;
                leag.ZipCode = league.ZipCode;
                leag.City = league.City;
                leag.Country = league.Country;
                leag.CountryId = league.CountryId;
                leag.Email = league.Email;
                leag.Founded = league.Founded;
                leag.FoundedCultureString = league.Founded.ToShortDateString();
                leag.InternalWelcomeImage = league.InternalWelcomeImage;
                leag.LeagueId = league.LeagueId;
                leag.LeagueMembers = league.LeagueMembers;
                leag.Logo = league.Logo;
                leag.Name = league.Name;
                //leag.Owners = league.Owners;
                leag.PhoneNumber = league.PhoneNumber;
                leag.State = league.State;
                leag.Teams = league.Teams;
                leag.TimeZone = league.TimeZone;
                if (leag.TimeZone != null && league.TimeZoneSelection != null)
                    leag.TimeZoneId = league.TimeZoneSelection.ZoneId;
                leag.TimeZones = RDN.Library.Classes.Location.TimeZoneFactory.GetTimeZones();

                leag.Website = league.Website;
                leag.Twitter = league.Twitter;
                leag.Instagram = league.Instagram;
                leag.Facebook = league.Facebook;
                leag.RuleSetsPlayedEnum = league.RuleSetsPlayedEnum;
                leag.CultureSelected = league.CultureSelected;
                leag.ThemeColor = league.ThemeColor;
                ViewBag.Saved = false;
                var colors = ColorDisplayFactory.GetColors();
                leag.ColorList = new SelectList(colors, "HexColor", "NameOfColor");
                leag.Colors = league.Colors;
                leag.ColorsSelected = league.ColorsSelected;

                //https://github.com/jquery/jquery-ui/tree/master/ui/i18n
                var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(x => x.Name.Contains("en")).OrderBy(x => x.DisplayName);
                leag.CultureList = new SelectList(cultures, "LCID", "DisplayName");
                @ViewBag.ThemeList = new SelectList(System.Configuration.ConfigurationManager.AppSettings["ThemeColors"].ToString().Split(','));

                return View(leag);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/"));
        }
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true)]
        public ActionResult EditLeague(RDN.League.Models.League.LeagueModel league, HttpPostedFileBase file)
        {
            try
            {

                var memid = RDN.Library.Classes.Account.User.GetMemberId();
                var isManager = MemberCache.IsManagerOrBetterOfLeague(memid);

                bool isAdmin = MemberCache.IsAdministrator(memid);
                if (!isAdmin && !isManager)
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                RDN.Portable.Classes.League.Classes.League leag = league;
                leag.InternalWelcomeMessage = league.InternalWelcomeMessageModel;
                bool made = false;
                if (Request.Form[RuleSetsUsedEnum.MADE.ToString()] != null)
                    made = Request.Form[RuleSetsUsedEnum.MADE.ToString()].Contains("true");
                bool OSDA = false;
                if (Request.Form[RuleSetsUsedEnum.OSDA.ToString()] != null)
                    OSDA = Request.Form[RuleSetsUsedEnum.OSDA.ToString()].Contains("true");
                bool RDCL = false;
                if (Request.Form[RuleSetsUsedEnum.RDCL.ToString()] != null)
                    RDCL = Request.Form[RuleSetsUsedEnum.RDCL.ToString()].Contains("true");
                bool Renegade = false;
                if (Request.Form[RuleSetsUsedEnum.Renegade.ToString()] != null)
                    Renegade = Request.Form[RuleSetsUsedEnum.Renegade.ToString()].Contains("true");
                bool Texas_Derby = false;
                if (Request.Form[RuleSetsUsedEnum.Texas_Derby.ToString()] != null)
                    Texas_Derby = Request.Form[RuleSetsUsedEnum.Texas_Derby.ToString()].Contains("true");
                bool The_WFTDA = false;
                if (Request.Form[RuleSetsUsedEnum.The_WFTDA.ToString()] != null)
                    The_WFTDA = Request.Form[RuleSetsUsedEnum.The_WFTDA.ToString()].Contains("true");
                bool USARS = false;
                if (Request.Form[RuleSetsUsedEnum.USARS.ToString()] != null)
                    USARS = Request.Form[RuleSetsUsedEnum.USARS.ToString()].Contains("true");

                if (made)
                    league.RuleSetsPlayedEnum |= RuleSetsUsedEnum.MADE;
                if (OSDA)
                    league.RuleSetsPlayedEnum |= RuleSetsUsedEnum.OSDA;
                if (RDCL)
                    league.RuleSetsPlayedEnum |= RuleSetsUsedEnum.RDCL;
                if (Renegade)
                    league.RuleSetsPlayedEnum |= RuleSetsUsedEnum.Renegade;
                if (Texas_Derby)
                    league.RuleSetsPlayedEnum |= RuleSetsUsedEnum.Texas_Derby;
                if (The_WFTDA)
                    league.RuleSetsPlayedEnum |= RuleSetsUsedEnum.The_WFTDA;
                if (USARS)
                    league.RuleSetsPlayedEnum |= RuleSetsUsedEnum.USARS;


                if (file == null)
                    RDN.Library.Classes.League.LeagueFactory.UpdateLeague(league, null, null);
                else
                    RDN.Library.Classes.League.LeagueFactory.UpdateLeague(league, file.InputStream, file.FileName);

                MemberCache.Clear(memid);
                MemberCache.ClearApiCache(memid);

                return Redirect(Url.Content("~/league/edit/" + league.LeagueId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }

            return Redirect(Url.Content("~/league/edit/" + league.LeagueId.ToString().Replace("-", "")));
        }

        #endregion

        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult Home()
        {
            try
            {
                var league = RDN.Library.Classes.League.LeagueFactory.GetLeague(MemberCache.GetLeagueIdOfMember(RDN.Library.Classes.Account.User.GetMemberId()));

                return View(league);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        //
        // GET: /League/
        public ActionResult Index()
        {
            return View();
        }

        #region Federations
        /// <summary>
        /// gets the leagues of the federation.
        /// </summary>
        /// <param name="federationId"></param>
        /// <returns></returns>
        public JsonResult GetLeagues(string federationId)
        {
            try
            {
                var leagues = RDN.Library.Classes.League.LeagueFactory.GetLeaguesInFederation(new Guid(federationId));
                return Json(leagues, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// manly for allowing federation owners to add multiple leagues at once.
        /// </summary>
        /// <returns></returns>
        [FederationAuthorize(RequireFederationManagerStatus = true)]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult Adds()
        {
            try
            {
                ViewBag.FederationName = RDN.Library.Classes.Federation.Federation.GetAllOwnedFederations(RDN.Library.Classes.Account.User.GetMemberId()).FirstOrDefault().Federation.Name;

                return View();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/"));
        }
        /// <summary>
        /// creates the leagues from the federation add leagues page.
        /// </summary>
        /// <param name="leagues"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Adds(List<CreateLeague> leagues)
        {
            try
            {
                var feds = Federation.GetAllOwnedFederations(RDN.Library.Classes.Account.User.GetMemberId());

                foreach (var league in leagues)
                {
                    RDN.Library.Classes.League.LeagueFactory.CreateLeague(feds.FirstOrDefault().Federation.FederationId.ToString(), league.LeagueName, league.ContactPhone, league.ContactEmail, String.Empty, league.Country, league.State, league.City, -5);
                }

                var url = Url.Content("~/Federation/Home");
                return Json(new { result = "true", url = url });
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { result = "false", url = "" });
        }

        #endregion

        #region Documents
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult Documents(Guid leagueId)
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string u = nameValueCollection["u"];
                //folder selected
                string f = nameValueCollection["f"];
                //group selected
                string g = nameValueCollection["g"];
                long folderId = 0;
                long groupId = 0;
                if (u == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Added Documents.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(f))
                {
                    folderId = Convert.ToInt64(f);
                }
                if (!String.IsNullOrEmpty(g))
                {
                    groupId = Convert.ToInt64(g);
                }
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var repo = DocumentRepository.GetLeagueDocumentRepository(leagueId, memId, folderId, groupId);
                repo.GroupsApartOf = MemberCache.GetGroupsApartOf(memId);

                //hides the folders not apart of within league
                var foldersCom = new List<Folder>();
                var foldersApartOf = new List<Folder>();
                foldersCom.AddRange(repo.Folders);
                //adds the groups to the folder list so we can add to a group.
                for (int i = 0; i < repo.Groups.Count; i++)
                {
                    foldersCom.Add(new Folder() { FolderId = repo.Groups[i].Id, FolderName = "G- " + repo.Groups[i].GroupName });
                }
                //checking folders if they belong to any of the groups the current user is apart of..
                for (int i = 0; i < repo.Folders.Count; i++)
                {
                    if (repo.Folders[i].GroupId > 0)
                    {
                        if (repo.GroupsApartOf.Where(x => x.Id == repo.Folders[i].GroupId).FirstOrDefault() != null)
                        {
                            foldersApartOf.Add(new Folder() { GroupFolderId = repo.Folders[i].FolderId.ToString(), FolderName = repo.Folders[i].FolderName });
                        }
                    }
                    else
                        foldersApartOf.Add(new Folder() { GroupFolderId = repo.Folders[i].FolderId.ToString(), FolderName = repo.Folders[i].FolderName });
                }
                for (int i = 0; i < repo.GroupsApartOf.Count; i++)
                    foldersApartOf.Add(new Folder() { GroupFolderId = "G-" + repo.GroupsApartOf[i].Id, FolderName = "G- " + repo.GroupsApartOf[i].GroupName });

                //folder to move to for Manager
                ViewBag.FoldersSelect = new SelectList(foldersCom.OrderBy(x => x.FolderName), "FolderId", "FolderName");
                //folder to move to for UPLOADER.
                ViewBag.FoldersApartOf = new SelectList(foldersApartOf.OrderBy(x => x.FolderName), "GroupFolderId", "FolderName");

                return View(repo);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult DocumentComments(Guid documentId, long leagueDocumentId)
        {
            try
            {
                var doc = RDN.Library.Classes.Document.DocumentRepository.GetLeagueDocument(documentId, leagueDocumentId);

                return View(doc);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, IsSecretary = true)]
        public ActionResult DocumentSettings(Guid leagueId)
        {
            try
            {
                var repo = DocumentRepository.GetLeagueDocumentRepository(leagueId, RDN.Library.Classes.Account.User.GetMemberId());

                return View(repo);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        #endregion

        #region Contacts
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public ActionResult ContactsLeague()
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string u = nameValueCollection["u"];
                if (u == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Added New Contact.";
                    this.AddMessage(message);
                }
                else if (u == SiteMessagesEnum.su.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Edited Contact.";
                    this.AddMessage(message);
                }
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                return View(MemberCache.GetLeagueOfMember(memId));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        #endregion

    }
}
