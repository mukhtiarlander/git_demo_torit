using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using RDN.League.Models.Helpers;
using RDN.League.Models.Roster;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Roster;
using RDN.Library.Util;
using RDN.Library.Util.Enum;
using RDN.Portable.Classes.Federation.Enums;
using RDN.Portable.Classes.Insurance;
using Scoreboard.Library.Classes.Reports.Excel;

namespace RDN.League.Controllers.League
{
    public class RosterController : BaseController
    {
        // GET: Roster
        [Authorize]
        public ActionResult ViewRosters()
        {
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);

            string updated = nameValueCollection["u"];

            if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.s.ToString())
            {
                SiteMessage message = new SiteMessage();
                message.MessageType = SiteMessageType.Info;
                message.Message = "Roster information updated.";
                this.AddMessage(message);

            }
            if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sac.ToString())
            {
                SiteMessage message = new SiteMessage();
                message.MessageType = SiteMessageType.Info;
                message.Message = "New Roster Successfully Added.";
                this.AddMessage(message);
            }
            if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.de.ToString())
            {
                SiteMessage message = new SiteMessage();
                message.MessageType = SiteMessageType.Info;
                message.Message = "Successfully Deleted.";
                this.AddMessage(message);
            }
            var memId = RDN.Library.Classes.Account.User.GetMemberId();
            var league = MemberCache.GetLeagueOfMember(memId);
            var lst = RosterManager.GetLeagueRosters(league.LeagueId);

            return View(lst);
        }

        [Authorize]
        public ActionResult ViewRoster(long id, string leagueId)
        {
            var memId = RDN.Library.Classes.Account.User.GetMemberId();
            var league = MemberCache.GetLeagueOfMember(memId);
            var roster = RosterManager.GetLeagueRoster(id, league.LeagueId);
            return View(roster);
        }
        [Authorize]
        public ActionResult AddNewRoster()
        {
            var memId = RDN.Library.Classes.Account.User.GetUserId();
            var model = new RosterModel();
            var members = MemberCache.GetCurrentLeagueMembers(memId).Select(x => new KeyValueHelper()
            {
                Id = x.MemberId,
                Name = x.Name
            }).ToList();
            model.Members = members;
            model.InsuranceTypes = new SelectList(EnumerationHelper.GetAll<InsuranceType>(), "Key", "Value");
            model.RuleSets = new SelectList(EnumerationHelper.GetAll<RuleSetsUsedEnum>(), "Key", "Value");

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddNewRoster(RosterModel model)
        {
            var roster = new Roster();
            roster.RosterId = model.RosterId;
            roster.RosterName = model.RosterName;
            roster.RosterMemberIds = model.RosterMemberIds;
            roster.GameDate = model.GameDate;
            roster.RosterMembers = model.RosterMembers;
            roster.InsuranceTypeId = model.InsuranceTypeId;
            roster.RuleSetsUsedEnum = model.RuleSetsUsedEnum;
            var memId = RDN.Library.Classes.Account.User.GetMemberId();
            var league = MemberCache.GetLeagueOfMember(memId);
            roster.LeagueId = league.LeagueId;
            bool isexecute = RosterManager.CreateNewRoster(roster);
            if (isexecute)
                return Redirect(Url.Content("~/league/rosters/all?u=" + SiteMessagesEnum.sac));

            return View(model);
        }

        [Authorize]
        public ActionResult EditRoster(long id, string leagueId)
        {
            var memId = RDN.Library.Classes.Account.User.GetMemberId();

            var roster = RDN.Library.Classes.Roster.RosterManager.GetLeagueRoster(id, new Guid(leagueId));
            var model = new RosterModel();
            model.RosterId = roster.RosterId;
            model.RosterName = roster.RosterName;
            model.GameDate = roster.GameDate;
            model.RosterMembers = roster.RosterMembers;
            model.InsuranceTypeId = roster.InsuranceTypeId;
            model.RuleSetsUsedEnum = roster.RuleSetsUsedEnum;
            var members = MemberCache.GetCurrentLeagueMembers(memId).Select(x => new KeyValueHelper()
            {
                Id = x.MemberId,
                Name = x.Name
            }).ToList();
            model.Members = members;
            model.RosterMemberIds = string.Join(",", roster.RosterMembers.Select(x => x.Id).ToArray());
            model.InsuranceTypes = new SelectList(EnumerationHelper.GetAll<InsuranceType>(), "Key", "Value");
            model.RuleSets = new SelectList(EnumerationHelper.GetAll<RuleSetsUsedEnum>(), "Key", "Value");

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditRoster(RosterModel model)
        {
            var roster = new Roster();
            roster.RosterId = model.RosterId;
            roster.RosterName = model.RosterName;
            roster.RosterMemberIds = model.RosterMemberIds;
            roster.GameDate = model.GameDate;
            roster.RosterMembers = model.RosterMembers;
            roster.InsuranceTypeId = model.InsuranceTypeId;
            roster.RuleSetsUsedEnum = model.RuleSetsUsedEnum;
            var memId = RDN.Library.Classes.Account.User.GetMemberId();
            var league = MemberCache.GetLeagueOfMember(memId);
            roster.LeagueId = league.LeagueId;
            bool isexecute = RosterManager.UpdateRoster(roster);
            if (isexecute)
                return Redirect(Url.Content("~/league/rosters/all?u=" + SiteMessagesEnum.sac));

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public FileResult ExportRoster(RosterModel model)
        {

            try
            {
                var roster = RosterManager.GetLeagueRoster(model.RosterId, model.LeagueId);

                if (roster != null)
                {
                    using (ExcelPackage p = new ExcelPackage())
                    {
                        ExcelWorksheet reportSheet = p.Workbook.Worksheets.Add("Roster");
                        reportSheet.Name = "Roster";
                        reportSheet.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                        reportSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
                        reportSheet.Cells[1, 1].Value = "Skater Name";
                        reportSheet.Cells[1, 2].Value = "#";
                        reportSheet.Cells[1, 3].Value = "WFTDA #";
                        reportSheet.Cells[1, 4].Value = "Real Name";
                        reportSheet.Cells[1, 5].Value = "Status";

                        reportSheet.Cells[1, 1].SetFontBold();
                        reportSheet.Cells[1, 2].SetFontBold();
                        reportSheet.Cells[1, 3].SetFontBold();
                        reportSheet.Cells[1, 4].SetFontBold();
                        reportSheet.Cells[1, 5].SetFontBold();

                        int rowReport = 2;
                        foreach (var member in roster.RosterMembers)
                        {
                            var mem = MemberCache.GetMemberDisplay(member.Id);
                            reportSheet.Cells[rowReport, 1].Value = mem.DerbyName;
                            reportSheet.Cells[rowReport, 2].Value = mem.PlayerNumber;

                            var insType = mem.InsuranceNumbers.FirstOrDefault(x => x.Type == InsuranceType.WFTDA);
                            if (insType != null)
                            {
                                reportSheet.Cells[rowReport, 3].Value = insType.Number;
                            }
                            reportSheet.Cells[rowReport, 4].Value = mem.RealName;
                            reportSheet.Cells[rowReport, 5].Value = "*";
                            rowReport += 1;
                        }
                        Byte[] bin = p.GetAsByteArray();
                        string file = "Roster_" + roster.RosterName + ".xlsx";
                        return File(bin, RDN.Utilities.IO.FileExt.GetMIMEType(file), file);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new FileStreamResult(null, "text/calendar");
        }

    }
}