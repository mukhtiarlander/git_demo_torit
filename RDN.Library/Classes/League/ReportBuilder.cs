using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using RDN.Library.Classes.League.Reports;
using RDN.Portable.Classes.Account.Enums;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Portable.Classes.Insurance;

namespace RDN.Library.Classes.League
{
    public class ReportBuilder
    {
        public static ExcelPackage PrepareExcelWorkBook(string SelectedColumns, RDN.Portable.Classes.League.Classes.League league)
        {
            ExcelPackage excel = new ExcelPackage();
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                bool isSec = MemberCache.IsSecretaryOrBetterOfLeague(memId);
                var items = SelectedColumns.Split(',');
                //we create the first sheet.
                ExcelWorksheet reportSheet = excel.Workbook.Worksheets.Add("Report");
                reportSheet.Name = "Report"; //Setting Sheet's name
                reportSheet.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                reportSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

                int column = 1;
                int row = 1;
                foreach (var item in items)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        MembersReportEnum temp = (MembersReportEnum)Enum.Parse(typeof(MembersReportEnum), item);

                        if (Enum.GetNames(typeof(MemberContactTypeEnum)).Contains(item))
                        {
                            MemberContactTypeEnum contacttype = (MemberContactTypeEnum)Enum.Parse(typeof(MemberContactTypeEnum), item);
                            ExcelWorksheet reportSheet_contact = excel.Workbook.Worksheets.Add(item);
                            reportSheet_contact = PrepareContactTypeSheet(reportSheet_contact, contacttype);
                        }
                        else
                        {
                            reportSheet.Cells[row, column].Value = RDN.Portable.Util.Enums.EnumExt.ToFreindlyName(temp);
                            row += 1;
                            for (int i = 0; i < league.LeagueMembers.Count; i++)
                            {
                                switch (temp)
                                {
                                    case MembersReportEnum.Phone_Number:
                                        if (!league.LeagueMembers[i].Settings.Hide_Phone_Number_From_League || (league.LeagueMembers[i].Settings.Hide_Phone_Number_From_League && isSec))
                                            reportSheet.Cells[row, column].Value = league.LeagueMembers[i].PhoneNumber;
                                        else
                                            reportSheet.Cells[row, column].Value = "hidden";
                                        break;
                                    case MembersReportEnum.Email:
                                        if (!league.LeagueMembers[i].Settings.Hide_Email_From_League || (league.LeagueMembers[i].Settings.Hide_Email_From_League && isSec))
                                            reportSheet.Cells[row, column].Value = league.LeagueMembers[i].Email;
                                        else
                                            reportSheet.Cells[row, column].Value = "hidden";
                                        break;
                                    case MembersReportEnum.Birthday:
                                        if (!league.LeagueMembers[i].Settings.Hide_DOB_From_League || (league.LeagueMembers[i].Settings.Hide_DOB_From_League && isSec))
                                            reportSheet.Cells[row, column].Value = league.LeagueMembers[i].DOB.ToShortDateString();
                                        else
                                            reportSheet.Cells[row, column].Value = "hidden";
                                        break;
                                    case MembersReportEnum.CRDI_Insurance_Number:
                                        var crdNumber = league.LeagueMembers[i].InsuranceNumbers.Where(x => x.Type == InsuranceType.CRDI).FirstOrDefault();
                                        if (crdNumber != null)
                                            reportSheet.Cells[row, column].Value = crdNumber.Number;
                                        break;
                                    case MembersReportEnum.Nick_Name:
                                        reportSheet.Cells[row, column].Value = league.LeagueMembers[i].DerbyName;
                                        break;
                                    case MembersReportEnum.Number:
                                        reportSheet.Cells[row, column].Value = league.LeagueMembers[i].PlayerNumber;
                                        break;
                                    case MembersReportEnum.First_Name:
                                        reportSheet.Cells[row, column].Value = league.LeagueMembers[i].Firstname;
                                        break;
                                    case MembersReportEnum.Inactive_Indicator:
                                        reportSheet.Cells[row, column].Value = league.LeagueMembers[i].IsInactiveFromCurrentLeague;
                                        break;
                                    case MembersReportEnum.Last_Name:
                                        reportSheet.Cells[row, column].Value = league.LeagueMembers[i].LastName;
                                        break;
                                    case MembersReportEnum.Other_Insurance_Number:
                                        var otherNumber = league.LeagueMembers[i].InsuranceNumbers.Where(x => x.Type == InsuranceType.Other).FirstOrDefault();
                                        if (otherNumber != null)
                                            reportSheet.Cells[row, column].Value = otherNumber.Number;
                                        break;
                                    case MembersReportEnum.Skill_Level:
                                        reportSheet.Cells[row, column].Value = league.LeagueMembers[i].LeagueClassificationOfSkatingLevel;
                                        break;
                                    case MembersReportEnum.USARS_Insurance_Number:
                                        var usarNumber = league.LeagueMembers[i].InsuranceNumbers.Where(x => x.Type == InsuranceType.USARS).FirstOrDefault();
                                        if (usarNumber != null)
                                            reportSheet.Cells[row, column].Value = usarNumber.Number;
                                        break;
                                    case MembersReportEnum.WFTDA_Insurance_Number:
                                        var wftdaNumber = league.LeagueMembers[i].InsuranceNumbers.Where(x => x.Type == InsuranceType.WFTDA).FirstOrDefault();
                                        if (wftdaNumber != null)
                                            reportSheet.Cells[row, column].Value = wftdaNumber.Number;
                                        break;
                                    case MembersReportEnum.Skills_Test_Date:
                                        if (league.LeagueMembers[i].Leagues.FirstOrDefault() != null && league.LeagueMembers[i].Leagues.FirstOrDefault().SkillsTestDate.HasValue)
                                            reportSheet.Cells[row, column].Value = league.LeagueMembers[i].Leagues.FirstOrDefault().SkillsTestDate.GetValueOrDefault().ToShortDateString();
                                        break;
                                    case MembersReportEnum.League_Departure_Date:
                                        if (league.LeagueMembers[i].Leagues.FirstOrDefault() != null && league.LeagueMembers[i].Leagues.FirstOrDefault().DepartureDate.HasValue)
                                            reportSheet.Cells[row, column].Value = league.LeagueMembers[i].Leagues.FirstOrDefault().DepartureDate.GetValueOrDefault().ToShortDateString();
                                        break;
                                    case MembersReportEnum.Day_Job:
                                        reportSheet.Cells[row, column].Value = league.LeagueMembers[i].DayJob;
                                        break;

                                }
                                row += 1;
                            }
                            row = 1;
                            column += 1;
                        }

                    }
                }
                reportSheet.Cells["A1:Z100"].AutoFitColumns();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return excel;
        }

        public static ExcelWorksheet PrepareContactTypeSheet(ExcelWorksheet contactSheet, MemberContactTypeEnum contacttype)
        {
            contactSheet.Name = RDN.Portable.Util.Enums.EnumExt.ToFreindlyName(contacttype); //Setting Sheet's name
            contactSheet.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            contactSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

            int column = 1;
            int row = 1;

            var memberid = RDN.Library.Classes.Account.User.GetMemberId();
            var leagueid = MemberCache.GetLeagueIdOfMember(memberid);
            var members = MemberCache.GetLeagueMembers(memberid, leagueid);

            for (int i = 0; i < members.Count(); i++)
            {
                var contacts = RDN.Library.Cache.MemberCache.GetMemberDisplay(members[i].MemberId).MemberContacts.Where(w => w.ContactType.ToString() == contacttype.ToString())
               .Select(s => new
               {
                   Member_First_Name = members[i].Firstname,
                   Member_Last_Name = members[i].LastName,
                   First_Name = s.Firstname,
                   Last_Name = s.Lastname,
                   Email = s.Email,
                   Phone_Number = s.PhoneNumber,
                   Address_Line_1 = s.Addresses.FirstOrDefault() != null ? s.Addresses.FirstOrDefault().Address1 : "",
                   Address_Line_2 = s.Addresses.FirstOrDefault() != null ? s.Addresses.FirstOrDefault().Address2 : "",
                   City = s.Addresses.FirstOrDefault() != null ? s.Addresses.FirstOrDefault().CityRaw : "",
                   State = s.Addresses.FirstOrDefault() != null ? s.Addresses.FirstOrDefault().StateRaw : "",
                   Zip = s.Addresses.FirstOrDefault() != null ? s.Addresses.FirstOrDefault().Zip : "",
                   Country = s.Addresses.FirstOrDefault() != null ? s.Addresses.FirstOrDefault().Country : ""
               }).ToList();

                if (contacts.Count() > 0)
                {
                    foreach (string item in Enum.GetNames(typeof(MembersReportContactEnum)))
                    {
                        if (!String.IsNullOrEmpty(item))
                        {
                            MembersReportContactEnum temp = (MembersReportContactEnum)Enum.Parse(typeof(MembersReportContactEnum), item);
                            contactSheet.Cells[row, column].Value = RDN.Portable.Util.Enums.EnumExt.ToFreindlyName(temp);
                            row += 1;
                            for (int j = 0; j < contacts.Count; j++)
                            {
                                switch (temp)
                                {
                                    case MembersReportContactEnum.Member_First_Name:
                                        contactSheet.Cells[row, column].Value = contacts[j].Member_First_Name;
                                        break;
                                    case MembersReportContactEnum.Member_Last_Name:
                                        contactSheet.Cells[row, column].Value = contacts[j].Member_Last_Name;
                                        break;
                                    case MembersReportContactEnum.First_Name:
                                        contactSheet.Cells[row, column].Value = contacts[j].First_Name;
                                        break;
                                    case MembersReportContactEnum.Last_Name:
                                        contactSheet.Cells[row, column].Value = contacts[j].Last_Name;
                                        break;
                                    case MembersReportContactEnum.Email:
                                        contactSheet.Cells[row, column].Value = contacts[j].Email;
                                        break;
                                    case MembersReportContactEnum.Phone_Number:
                                        contactSheet.Cells[row, column].Value = contacts[j].Phone_Number;
                                        break;
                                    case MembersReportContactEnum.Address_Line_1:
                                        contactSheet.Cells[row, column].Value = contacts[j].Address_Line_1;
                                        break;
                                    case MembersReportContactEnum.Address_Line_2:
                                        contactSheet.Cells[row, column].Value = contacts[j].Address_Line_2;
                                        break;
                                    case MembersReportContactEnum.City:
                                        contactSheet.Cells[row, column].Value = contacts[j].City;
                                        break;
                                    case MembersReportContactEnum.State:
                                        contactSheet.Cells[row, column].Value = contacts[j].State;
                                        break;
                                    case MembersReportContactEnum.Zip:
                                        contactSheet.Cells[row, column].Value = contacts[j].Zip;
                                        break;
                                    case MembersReportContactEnum.Country:
                                        contactSheet.Cells[row, column].Value = contacts[j].Country;
                                        break;
                                }
                                row += 1;
                            }
                            row = 1;
                            column += 1;

                        }
                    }
                }
            }

            contactSheet.Cells["A1:Z100"].AutoFitColumns();
            return contactSheet;
        }
    }
}
