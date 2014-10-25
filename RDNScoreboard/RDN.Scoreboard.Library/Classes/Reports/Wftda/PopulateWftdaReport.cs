using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using Scoreboard.Library.Static.Enums;
using Scoreboard.Library.ViewModel;
using Scoreboard.Library.Classes.Reports.Excel;
using Scoreboard.Library.ViewModel.Officials.Enums;
using Scoreboard.Library.ViewModel.Officials;

namespace Scoreboard.Library.Classes.Reports.Wftda
{
    public class PopulateWftdaReport
    {


        private ExcelPackage _excelWorkbook;
        public PopulateWftdaReport(ExcelPackage package)
        {
            _excelWorkbook = package;
        }

        public ExcelPackage PopulateReport()
        {
            try
            {
                PopulateIBRF();
                PopulateScore();
                PopulatePenalties();
                PopulateLineUps();
                PopulateOfficialReviews();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
            return _excelWorkbook;
        }

        private void PopulateOfficialReviews()
        {
            try
            {
                var reportSheet = _excelWorkbook.Workbook.Worksheets.Where(x => x.Name == "Official Reviews").FirstOrDefault();
                int row = 1;
                if (GameViewModel.Instance.Officials != null)
                {
                    var tracker1 = GameViewModel.Instance.Officials.Nsos.Where(x => x.RefereeType == NSOTypeEnum.Official_Review_Tracker).FirstOrDefault();

                    if (tracker1 != null)
                    {
                        reportSheet.Cells[row, 3].Value = tracker1.SkaterName;
                    }
                }

                row = 4;
                if (GameViewModel.Instance.OfficialReviews != null)
                {
                    var periodOneReviews = GameViewModel.Instance.OfficialReviews.Where(x => x != null && x.Period == 1);
                    foreach (var rev in periodOneReviews)
                    {
                        if (rev.TeamNumber == TeamNumberEnum.Team1 && GameViewModel.Instance.Team1 != null)
                            reportSheet.Cells[row, 3].Value = GameViewModel.Instance.Team1.TeamName;
                        else if (rev.TeamNumber == TeamNumberEnum.Team2 && GameViewModel.Instance.Team2 != null)
                            reportSheet.Cells[row, 3].Value = GameViewModel.Instance.Team2.TeamName;

                        reportSheet.Cells[row, 6].Value = TimeSpan.FromMilliseconds(rev.PeriodTimeRemaining).ToString(@"m\:ss");
                        reportSheet.Cells[row, 8].Value = rev.JamNumber;

                        reportSheet.Cells[row + 1, 2].Value = rev.Details;
                        reportSheet.Cells[row + 2, 2].Value = rev.Result;
                        row += 3;
                    }
                    row = 10;
                    var periodTwoReviews = GameViewModel.Instance.OfficialReviews.Where(x => x != null && x.Period == 2);
                    foreach (var rev in periodTwoReviews)
                    {
                        if (rev.TeamNumber == TeamNumberEnum.Team1 && GameViewModel.Instance.Team1 != null)
                            reportSheet.Cells[row, 3].Value = GameViewModel.Instance.Team1.TeamName;
                        else if (rev.TeamNumber == TeamNumberEnum.Team2 && GameViewModel.Instance.Team2 != null)
                            reportSheet.Cells[row, 3].Value = GameViewModel.Instance.Team2.TeamName;

                        reportSheet.Cells[row, 6].Value = TimeSpan.FromMilliseconds(rev.PeriodTimeRemaining).ToString(@"m\:ss");
                        reportSheet.Cells[row, 8].Value = rev.JamNumber;

                        reportSheet.Cells[row + 1, 2].Value = rev.Details;
                        reportSheet.Cells[row + 2, 2].Value = rev.Result;
                        row += 3;
                    }
                }
                row = 16;
                if (GameViewModel.Instance.Officials != null)
                {
                    var tracker1 = GameViewModel.Instance.Officials.Referees.Where(x => x.RefereeType == RefereeTypeEnum.Head_Ref).FirstOrDefault();

                    if (tracker1 != null)
                    {
                        reportSheet.Cells[row, 2].Value = tracker1.SkaterName;
                    }
                }

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }


        private void PopulatePenalties()
        {
            try
            {
                var reportSheet = _excelWorkbook.Workbook.Worksheets.Where(x => x.Name == "Penalties").FirstOrDefault();
                int row = 1;
                if (GameViewModel.Instance.Officials != null)
                {
                    var tracker1 = GameViewModel.Instance.Officials.Nsos.Where(x => x.RefereeType == NSOTypeEnum.Penalty_Tracker).FirstOrDefault();
                    if (tracker1 != null)
                    {
                        reportSheet.Cells[row, 12].Value = tracker1.SkaterName;
                        reportSheet.Cells[row, 36].Value = tracker1.SkaterName;
                    }
                }
                row = 4;
                foreach (var member in GameViewModel.Instance.Team1.TeamMembers)
                {
                    var penalties = GameViewModel.Instance.PenaltiesForTeam1.Where(x => x.PenaltyAgainstMember != null && x.PenaltyAgainstMember.SkaterId == member.SkaterId && x.Period == 1).ToList();
                    int column = 2;
                    int penCount = 0;
                    foreach (var pen in penalties)
                    {

                        string abb = PenaltyViewModel.ToAbbreviation(PenaltyViewModel.ConvertToWFTDA(pen.PenaltyType));
                        reportSheet.Cells[row, column].Value = abb;
                        reportSheet.Cells[row + 1, column].Value = pen.JamNumber;
                        column += 1;
                        penCount += 1;
                    }
                    var penaltiesP2 = GameViewModel.Instance.PenaltiesForTeam1.Where(x => x.PenaltyAgainstMember != null && x.PenaltyAgainstMember.SkaterId == member.SkaterId && x.Period == 2).ToList();
                    column = 26;
                    //we are shading the boxes here because of the penalties received in period 1
                    for (int i = 0; i < penCount; i++)
                    {
                        reportSheet.Cells[row, column].SetBackgroundColor(Color.Black);
                        reportSheet.Cells[row + 1, column].SetBackgroundColor(Color.Black);
                        column += 1;
                    }

                    foreach (var pen in penaltiesP2)
                    {
                        string abb = PenaltyViewModel.ToAbbreviation(PenaltyViewModel.ConvertToWFTDA(pen.PenaltyType));
                        reportSheet.Cells[row, column].Value = abb;
                        reportSheet.Cells[row + 1, column].Value = pen.JamNumber;
                        column += 1;
                    }
                    row += 2;
                }
                row = 4;
                foreach (var member in GameViewModel.Instance.Team2.TeamMembers)
                {
                    var penalties = GameViewModel.Instance.PenaltiesForTeam2.Where(x => x.PenaltyAgainstMember != null && x.PenaltyAgainstMember.SkaterId == member.SkaterId && x.Period == 1).ToList();
                    int column = 15;
                    int penCount = 0;
                    foreach (var pen in penalties)
                    {
                        string abb = PenaltyViewModel.ToAbbreviation(PenaltyViewModel.ConvertToWFTDA(pen.PenaltyType));
                        reportSheet.Cells[row, column].Value = abb;
                        reportSheet.Cells[row + 1, column].Value = pen.JamNumber;
                        penCount += 1;
                        column += 1;
                    }
                    var penaltiesP2 = GameViewModel.Instance.PenaltiesForTeam2.Where(x => x.PenaltyAgainstMember != null && x.PenaltyAgainstMember.SkaterId == member.SkaterId && x.Period == 2).ToList();
                    column = 39;
                    //we are shading the boxes here because of the penalties received in period 1
                    for (int i = 0; i < penCount; i++)
                    {
                        reportSheet.Cells[row, column].SetBackgroundColor(Color.Black);
                        reportSheet.Cells[row + 1, column].SetBackgroundColor(Color.Black);
                        column += 1;
                    }

                    foreach (var pen in penaltiesP2)
                    {
                        string abb = PenaltyViewModel.ToAbbreviation(PenaltyViewModel.ConvertToWFTDA(pen.PenaltyType));
                        reportSheet.Cells[row, column].Value = abb;
                        reportSheet.Cells[row + 1, column].Value = pen.JamNumber;
                        column += 1;
                    }
                    row += 2;
                }

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }
        private void PopulateLineUps()
        {
            try
            {
                var reportSheet = _excelWorkbook.Workbook.Worksheets.Where(x => x.Name == "Lineups").FirstOrDefault();
                int row = 1;
                if (GameViewModel.Instance.Officials != null)
                {
                    var tracker1 = GameViewModel.Instance.Officials.Nsos.Where(x => x.RefereeType == NSOTypeEnum.Lineup_Tracker_Team_1).FirstOrDefault();
                    var tracker2 = GameViewModel.Instance.Officials.Nsos.Where(x => x.RefereeType == NSOTypeEnum.Lineup_Tracker_Team_2).FirstOrDefault();
                    if (tracker1 != null)
                    {
                        reportSheet.Cells[row, 9].Value = tracker1.SkaterName;
                        reportSheet.Cells[row + 84, 9].Value = tracker1.SkaterName;
                    }
                    if (tracker2 != null)
                    {
                        reportSheet.Cells[row, 31].Value = tracker2.SkaterName;
                        reportSheet.Cells[row + 84, 31].Value = tracker2.SkaterName;
                    }
                }

                row = 4;
                var periodOneJams = GameViewModel.Instance.Jams.Where(x => x.CurrentPeriod == 1).OrderBy(x => x.JamNumber);
                foreach (var jam in periodOneJams)
                {
                    PopulateLineUpJam(reportSheet, row, jam);
                    row += 2;
                }
                row = 88;
                var periodTwoJams = GameViewModel.Instance.Jams.Where(x => x.CurrentPeriod == 2).OrderBy(x => x.JamNumber);
                foreach (var jam in periodTwoJams)
                {
                    PopulateLineUpJam(reportSheet, row, jam);
                    row += 2;
                }

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }

        private static void PopulateLineUpJam(ExcelWorksheet reportSheet, int row, JamViewModel jam)
        {
            try
            {
                //team1
                if (jam.PivotT1 == null)
                {
                    reportSheet.Cells[row, 2].Value = "X";
                    if (jam.Blocker4T1 != null)
                        reportSheet.Cells[row, 6].Value = jam.Blocker4T1.SkaterNumber;
                }
                else
                    reportSheet.Cells[row, 6].Value = jam.PivotT1.SkaterNumber;
                if (jam.Blocker1T1 != null)
                    reportSheet.Cells[row, 9].Value = jam.Blocker1T1.SkaterNumber;
                if (jam.Blocker2T1 != null)
                    reportSheet.Cells[row, 12].Value = jam.Blocker2T1.SkaterNumber;
                if (jam.Blocker3T1 != null)
                    reportSheet.Cells[row, 15].Value = jam.Blocker3T1.SkaterNumber;
                //team2
                if (jam.PivotT2 == null)
                {
                    reportSheet.Cells[row, 24].Value = "X";
                    if (jam.Blocker4T2 != null)
                        reportSheet.Cells[row, 28].Value = jam.Blocker4T2.SkaterNumber;
                }
                else
                    reportSheet.Cells[row, 28].Value = jam.PivotT2.SkaterNumber;

                if (jam.Blocker1T2 != null)
                    reportSheet.Cells[row, 31].Value = jam.Blocker1T2.SkaterNumber;
                if (jam.Blocker2T2 != null)
                    reportSheet.Cells[row, 34].Value = jam.Blocker2T2.SkaterNumber;
                if (jam.Blocker3T2 != null)
                    reportSheet.Cells[row, 37].Value = jam.Blocker3T2.SkaterNumber;

                string passNumbers = String.Empty;
                string passNumbersSecondRow = String.Empty;
                int allPasses = 10;
                int passCount = jam.JamPasses.Where(x => x.Team == TeamNumberEnum.Team1).Count();
                int p = allPasses - passCount;

                for (int i = 1; i < 11; i++)
                {
                    if (passCount < i)
                        if (i < 6)
                            passNumbers += i + "  ";
                        else
                            passNumbersSecondRow += i + "  ";
                }
                reportSheet.Cells[row, 18].Value = passNumbers;
                reportSheet.Cells[row + 1, 18].Value = passNumbersSecondRow;

                passNumbers = String.Empty;
                passNumbersSecondRow = String.Empty;
                allPasses = 10;
                passCount = jam.JamPasses.Where(x => x.Team == TeamNumberEnum.Team2).Count();
                p = allPasses - passCount;
                for (int i = 1; i < 11; i++)
                {
                    if (passCount < i)
                        if (i < 6)
                            passNumbers += i + "  ";
                        else
                            passNumbersSecondRow += i + "  ";
                }
                reportSheet.Cells[row, 40].Value = passNumbers;
                reportSheet.Cells[row + 1, 40].Value = passNumbersSecondRow;

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }
        private void PopulateScore()
        {
            try
            {
                var reportSheet = _excelWorkbook.Workbook.Worksheets.Where(x => x.Name == "Score").FirstOrDefault();
                int row = 1;
                if (GameViewModel.Instance.Officials != null)
                {
                    var keeper1 = GameViewModel.Instance.Officials.Nsos.Where(x => x.RefereeType == NSOTypeEnum.Scorekeeper_Team_1).FirstOrDefault();
                    var jamRef1 = GameViewModel.Instance.Officials.Referees.Where(x => x.RefereeType == RefereeTypeEnum.Jam_Ref_Team_1).FirstOrDefault();
                    if (keeper1 != null)
                    {
                        reportSheet.Cells[row, 12].Value = keeper1.SkaterName;
                        reportSheet.Cells[row + 86, 31].Value = keeper1.SkaterName;
                    }
                    if (jamRef1 != null)
                    {
                        reportSheet.Cells[row, 15].Value = jamRef1.SkaterName;
                        reportSheet.Cells[row + 86, 35].Value = jamRef1.SkaterName;
                    }
                    var keeper2 = GameViewModel.Instance.Officials.Nsos.Where(x => x.RefereeType == NSOTypeEnum.Scorekeeper_Team_2).FirstOrDefault();
                    var jamRef2 = GameViewModel.Instance.Officials.Referees.Where(x => x.RefereeType == RefereeTypeEnum.Jam_Ref_Team_2).FirstOrDefault();
                    if (keeper2 != null)
                    {
                        reportSheet.Cells[row, 31].Value = keeper2.SkaterName;
                        reportSheet.Cells[row + 86, 12].Value = keeper2.SkaterName;
                    }
                    if (jamRef2 != null)
                    {
                        reportSheet.Cells[row, 34].Value = jamRef2.SkaterName;
                        reportSheet.Cells[row + 86, 15].Value = jamRef2.SkaterName;
                    }
                }
                row = 4;
                var periodOneJams = GameViewModel.Instance.Jams.Where(x => x.CurrentPeriod == 1).OrderBy(x => x.JamNumber);
                foreach (var jam in periodOneJams)
                {
                    reportSheet.Cells[row, 1].Value = jam.JamNumber;
                    if (jam.JammerT1 != null)
                        reportSheet.Cells[row, 2].Value = jam.JammerT1.SkaterNumber;

                    reportSheet.Cells[row, 20].Value = jam.JamNumber;
                    if (jam.JammerT2 != null)
                        reportSheet.Cells[row, 21].Value = jam.JammerT2.SkaterNumber;
                    //LOST LEAD ELIGIBIITY
                    if (jam.JammerT1 != null && jam.JammerT1.LostLeadJammerEligibility)
                        reportSheet.Cells[row, 3].Value = "X";
                    if (jam.JammerT2 != null && jam.JammerT2.LostLeadJammerEligibility)
                        reportSheet.Cells[row, 22].Value = "X";

                    //LEAD
                    if (jam.JammerT1 != null && jam.JammerT1.IsLeadJammer)
                        reportSheet.Cells[row, 4].Value = "X";
                    if (jam.JammerT2 != null && jam.JammerT2.IsLeadJammer)
                        reportSheet.Cells[row, 23].Value = "X";

                    //Jam got called by jammer of Team 1
                    if (jam.DidJamGetCalledByJammerT1)
                        reportSheet.Cells[row, 5].Value = "X";
                    //Jam got called by jammer of Team 2
                    if (jam.DidJamGetCalledByJammerT2)
                        reportSheet.Cells[row, 24].Value = "X";

                    if (jam.DidJamEndWithInjury)
                    {
                        reportSheet.Cells[row, 6].Value = "X";
                        reportSheet.Cells[row, 25].Value = "X";
                    }

                    //NO PASS
                    if (jam.JamPasses.Where(x => x.Team == TeamNumberEnum.Team1).Count() == 0)
                        reportSheet.Cells[row, 7].Value = "X";
                    if (jam.JamPasses.Where(x => x.Team == TeamNumberEnum.Team2).Count() == 0)
                        reportSheet.Cells[row, 26].Value = "X";
                    foreach (var pass in jam.JamPasses)
                    {
                        if (pass.Team == TeamNumberEnum.Team1)
                        {
                            //first pass
                            if (pass.PassNumber == 1 && pass.SkaterWhoPassed != null)
                            {
                                //we have to account for star passes.  So if the pivot is the same as the person who scored
                                //it was a star pass.
                                if (jam.PivotT1 != null && jam.PivotT1.SkaterId == pass.SkaterWhoPassed.SkaterId)
                                    reportSheet.Cells[row, 1].Value = jam.JamNumber + " SP";
                                reportSheet.Cells[row, 2].Value = pass.SkaterWhoPassed.SkaterNumber;
                            }

                            //7 == first column of passes for team 1.
                            reportSheet.Cells[row, 7 + pass.PassNumber].Value = pass.PointsScoredForPass;
                        }
                        else if (pass.Team == TeamNumberEnum.Team2)
                        {
                            //first pass
                            if (pass.PassNumber == 1 && pass.SkaterWhoPassed != null)
                            {
                                //we have to account for star passes.  So if the pivot is the same as the person who scored
                                //it was a star pass.
                                if (jam.PivotT2 != null && jam.PivotT2.SkaterId == pass.SkaterWhoPassed.SkaterId)
                                    reportSheet.Cells[row, 20].Value = jam.JamNumber + " SP";

                                reportSheet.Cells[row, 21].Value = pass.SkaterWhoPassed.SkaterNumber;
                            }

                            //26== first column of passes for team 1.
                            reportSheet.Cells[row, 26 + pass.PassNumber].Value = pass.PointsScoredForPass;
                        }
                    }
                    row += 2;
                }
                row = 90;
                var periodTwoJams = GameViewModel.Instance.Jams.Where(x => x.CurrentPeriod == 2).OrderBy(x => x.JamNumber);
                foreach (var jam in periodTwoJams)
                {
                    reportSheet.Cells[row, 1].Value = jam.JamNumber;
                    if (jam.JammerT1 != null)
                        reportSheet.Cells[row, 2].Value = jam.JammerT1.SkaterNumber;

                    reportSheet.Cells[row, 20].Value = jam.JamNumber;
                    if (jam.JammerT2 != null)
                        reportSheet.Cells[row, 21].Value = jam.JammerT2.SkaterNumber;

                    //LOST LEAD ELIGIBIITY
                    if (jam.JammerT1 != null && jam.JammerT1.LostLeadJammerEligibility)
                        reportSheet.Cells[row, 3].Value = "X";
                    if (jam.JammerT2 != null && jam.JammerT2.LostLeadJammerEligibility)
                        reportSheet.Cells[row, 22].Value = "X";

                    //LEAD
                    if (jam.JammerT1 != null && jam.JammerT1.IsLeadJammer)
                        reportSheet.Cells[row, 4].Value = "X";
                    if (jam.JammerT2 != null && jam.JammerT2.IsLeadJammer)
                        reportSheet.Cells[row, 23].Value = "X";

                    //Jam got called by jammer of Team 1
                    if (jam.DidJamGetCalledByJammerT1)
                        reportSheet.Cells[row, 5].Value = "X";
                    //Jam got called by jammer of Team 2
                    if (jam.DidJamGetCalledByJammerT2)
                        reportSheet.Cells[row, 24].Value = "X";

                    if (jam.DidJamEndWithInjury)
                    {
                        reportSheet.Cells[row, 6].Value = "X";
                        reportSheet.Cells[row, 25].Value = "X";
                    }

                    //NO PASS
                    if (jam.JamPasses.Where(x => x.Team == TeamNumberEnum.Team1).Count() == 0)
                        reportSheet.Cells[row, 7].Value = "X";
                    if (jam.JamPasses.Where(x => x.Team == TeamNumberEnum.Team2).Count() == 0)
                        reportSheet.Cells[row, 26].Value = "X";
                    foreach (var pass in jam.JamPasses)
                    {
                        if (pass.Team == TeamNumberEnum.Team1)
                        {
                            //first pass
                            if (pass.PassNumber == 1 && pass.SkaterWhoPassed != null)
                            {
                                //we have to account for star passes.  So if the pivot is the same as the person who scored
                                //it was a star pass.
                                if (jam.PivotT1 != null && jam.PivotT1.SkaterId == pass.SkaterWhoPassed.SkaterId)
                                    reportSheet.Cells[row, 1].Value = jam.JamNumber + " SP";
                                reportSheet.Cells[row, 2].Value = pass.SkaterWhoPassed.SkaterNumber;
                            }

                            //7 == first column of passes for team 1.
                            reportSheet.Cells[row, 7 + pass.PassNumber].Value = pass.PointsScoredForPass;
                        }
                        else if (pass.Team == TeamNumberEnum.Team2)
                        {
                            //first pass
                            if (pass.PassNumber == 1 && pass.SkaterWhoPassed != null)
                            {
                                //we have to account for star passes.  So if the pivot is the same as the person who scored
                                //it was a star pass.
                                if (jam.PivotT2 != null && jam.PivotT2.SkaterId == pass.SkaterWhoPassed.SkaterId)
                                    reportSheet.Cells[row, 20].Value = jam.JamNumber + " SP";

                                reportSheet.Cells[row, 21].Value = pass.SkaterWhoPassed.SkaterNumber;
                            }

                            //26== first column of passes for team 1.
                            reportSheet.Cells[row, 26 + pass.PassNumber].Value = pass.PointsScoredForPass;
                        }
                    }
                    row += 2;
                }

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }
        private void PopulateIBRF()
        {
            try
            {
                var reportSheet = _excelWorkbook.Workbook.Worksheets.Where(x => x.Name == "IBRF").FirstOrDefault();
                int row = 3;
                reportSheet.Cells[3, 2].Value = GameViewModel.Instance.GameLocation;
                reportSheet.Cells[3, 8].Value = GameViewModel.Instance.GameCity;
                reportSheet.Cells[3, 10].Value = GameViewModel.Instance.GameState;
                if (GameViewModel.Instance.GameDate != null)
                    reportSheet.Cells[5, 2].Value = GameViewModel.Instance.GameDate.ToString("yyyy-MM-dd");
                if (GameViewModel.Instance.GameDate != null && GameViewModel.Instance.HasGameStarted)
                    reportSheet.Cells[5, 8].Value = GameViewModel.Instance.GameDate.ToShortTimeString();
                if (GameViewModel.Instance.GameEndDate != null && GameViewModel.Instance.HasGameEnded)
                    reportSheet.Cells[5, 11].Value = GameViewModel.Instance.GameEndDate.ToShortTimeString();

                reportSheet.Cells[8, 2].Value = GameViewModel.Instance.Team1.LeagueName;
                reportSheet.Cells[8, 8].Value = GameViewModel.Instance.Team2.LeagueName;
                reportSheet.Cells[9, 2].Value = GameViewModel.Instance.Team1.TeamName;
                reportSheet.Cells[9, 8].Value = GameViewModel.Instance.Team2.TeamName;

                row = 11;
                foreach (var member in GameViewModel.Instance.Team1.TeamMembers)
                {
                    if (!String.IsNullOrEmpty(member.SkaterNumber) && member.SkaterNumber.Length > 4)
                        reportSheet.Cells[row, 2].Value = member.SkaterNumber.Remove(3);
                    else
                        reportSheet.Cells[row, 2].Value = member.SkaterNumber;
                    reportSheet.Cells[row, 3].Value = member.SkaterName;
                    row += 1;
                }
                row = 11;
                foreach (var member in GameViewModel.Instance.Team2.TeamMembers)
                {
                    if (!String.IsNullOrEmpty(member.SkaterNumber) && member.SkaterNumber.Length > 4)
                        reportSheet.Cells[row, 8].Value = member.SkaterNumber.Remove(3);
                    else
                        reportSheet.Cells[row, 8].Value = member.SkaterNumber;
                    reportSheet.Cells[row, 9].Value = member.SkaterName;
                    row += 1;
                }
                row = 32;
                if (GameViewModel.Instance.Officials != null)
                {
                    if (GameViewModel.Instance.Officials.Referees != null)
                    {
                        foreach (var referee in GameViewModel.Instance.Officials.Referees)
                        {
                            if (row < 36)
                            {
                                reportSheet.Cells[row, 1].Value = referee.SkaterName;
                                reportSheet.Cells[row, 3].Value = RDN.Utilities.Enums.EnumExt.ToFreindlyName(referee.RefereeType);
                                reportSheet.Cells[row, 4].Value = referee.League;
                                reportSheet.Cells[row, 5].Value = RDN.Utilities.Enums.EnumExt.ToFreindlyName(referee.Cert);
                            }
                            else
                            {
                                reportSheet.Cells[row - 4, 6].Value = referee.SkaterName;
                                reportSheet.Cells[row - 4, 9].Value = RDN.Utilities.Enums.EnumExt.ToFreindlyName(referee.RefereeType);
                                reportSheet.Cells[row - 4, 10].Value = referee.League;
                                reportSheet.Cells[row - 4, 12].Value = RDN.Utilities.Enums.EnumExt.ToFreindlyName(referee.Cert);
                            }
                            row += 1;
                        }
                        row = 49;
                        var head = GameViewModel.Instance.Officials.Referees.Where(x => x.RefereeType == RefereeTypeEnum.Head_Ref).FirstOrDefault();
                        if (head != null)
                            reportSheet.Cells[row, 2].Value = head.SkaterName;
                    }
                    if (GameViewModel.Instance.Officials.Nsos != null)
                    {
                        var nsoHead = GameViewModel.Instance.Officials.Nsos.Where(x => x.RefereeType == NSOTypeEnum.Head_NSO).FirstOrDefault();
                        if (nsoHead != null)
                            reportSheet.Cells[row, 8].Value = nsoHead.SkaterName;

                        row = 56;
                        foreach (var nso in GameViewModel.Instance.Officials.Nsos)
                        {
                            reportSheet.Cells[row, 1].Value = nso.SkaterName;
                            reportSheet.Cells[row, 4].Value = RDN.Utilities.Enums.EnumExt.ToFreindlyName(nso.RefereeType);
                            reportSheet.Cells[row, 8].Value = nso.League;
                            reportSheet.Cells[row, 10].Value = RDN.Utilities.Enums.EnumExt.ToFreindlyName(nso.Cert);
                            row += 1;
                        }
                    }
                }

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }


    }
}
