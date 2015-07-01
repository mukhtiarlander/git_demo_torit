using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RDN.Utilities.Config;
using Scoreboard.Library.Static.Enums;

using Scoreboard.Library.ViewModel;
using Scoreboard.Library.ViewModel.Members;
using RDN.Utilities.Util;

namespace Scoreboard.Library.Classes.Reports.Excel
{
    public class MadeReportExport
    {
        public static void SaveMadeReport(string fileName)
        {
            var saveGameTask = Task<bool>.Factory.StartNew(
                  () =>
                  {
                      try
                      {

                          ExportMadeStatsReport(fileName);


                      }
                      catch (Exception exception)
                      {
                          Logger.Instance.logMessage("tried to save the game", LoggerEnum.message);
                          ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                          //ErrorViewModel.saveError(exception);
                      }
                      return true;
                  });

        }

        private static void ExportMadeStatsReport(string fileName)
        {
            using (ExcelPackage p = new ExcelPackage())
            {
                try
                {
                    p.Workbook.Properties.Author =  "RDNation.com v" + ScoreboardConfig.SCOREBOARD_VERSION_NUMBER;
                    p.Workbook.Properties.Title = "MADE Report For " + GameViewModel.Instance.GameName;

                    //first sheet for Team 1
                    ExportAssistBlocks(p, GameViewModel.Instance.Team1, 1);
                    ExportAssistBlocks(p, GameViewModel.Instance.Team2, 2);
                    ExportScores(p);
                    ExportPenalties(p);
                }
                catch (Exception exception)
                {
                    ErrorViewModel.Save(exception, exception.GetType());
                }
                FileInfo fi = new FileInfo(fileName);
                p.SaveAs(fi);
            }
        }

        private static void ExportAssistBlocks(ExcelPackage p, TeamViewModel team, int teamNumber)
        {
            try
            {
                ExcelWorksheet reportSheet = p.Workbook.Worksheets.Add("T" + teamNumber + " Assists & Blocks");
                reportSheet.Name = "T" + teamNumber + " Assists & Blocks"; //Setting Sheet's name
                reportSheet.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                reportSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
                reportSheet.Cells[1, 1].Value = team.TeamName;
                CreateHeaderStyle(reportSheet.Cells[1, 1]);
                SetBorderCell(reportSheet.Cells[1, 1]);
                reportSheet.Cells[1, 3].Value = "Blocks";
                CreateHeaderStyle(reportSheet.Cells[1, 3]);
                SetBorderCell(reportSheet.Cells[1, 3]);
                reportSheet.Cells[1, 4].Value = "Assists";
                CreateHeaderStyle(reportSheet.Cells[1, 4]);
                SetBorderCell(reportSheet.Cells[1, 4]);

                reportSheet.Cells[2, 1].Value = "Skater Name";
                CreateHeaderStyle2(reportSheet.Cells[2, 1]);
                SetBorderCell(reportSheet.Cells[2, 1]);
                reportSheet.Cells[2, 2].Value = "#";
                CreateHeaderStyle2(reportSheet.Cells[2, 2]);
                SetBorderCell(reportSheet.Cells[2, 2]);
                reportSheet.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                SetBorderCell(reportSheet.Cells[2, 3]);
                SetBorderCell(reportSheet.Cells[2, 4]);

                int rowReport = 3;
                //creates a row for each member
                foreach (var mems in team.TeamMembers)
                {
                    try
                    {
                        SetColorCell(reportSheet.Cells[rowReport, 1], Color.Yellow, rowReport);
                        SetColorCell(reportSheet.Cells[rowReport, 2], Color.Yellow, rowReport);
                        SetColorCell(reportSheet.Cells[rowReport, 3], Color.Yellow, rowReport);
                        SetColorCell(reportSheet.Cells[rowReport, 4], Color.Yellow, rowReport);
                        SetBorderCell(reportSheet.Cells[rowReport, 1]);
                        SetBorderCell(reportSheet.Cells[rowReport, 2]);
                        SetBorderCell(reportSheet.Cells[rowReport, 3]);
                        SetBorderCell(reportSheet.Cells[rowReport, 4]);
                        reportSheet.Cells[rowReport, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        reportSheet.Cells[rowReport, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        reportSheet.Cells[rowReport, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        reportSheet.Cells[rowReport, 1].Value = mems.SkaterName;
                        reportSheet.Cells[rowReport, 2].Value = mems.SkaterNumber;
                        if (teamNumber == 1)
                        {
                            reportSheet.Cells[rowReport, 3].Value = GameViewModel.Instance.BlocksForTeam1.Where(x => x.PlayerWhoBlocked.SkaterId == mems.SkaterId).Count();
                            reportSheet.Cells[rowReport, 4].Value = GameViewModel.Instance.AssistsForTeam1.Where(x => x.PlayerWhoAssisted.SkaterId == mems.SkaterId).Count();
                        }
                        else if (teamNumber == 2)
                        {
                            reportSheet.Cells[rowReport, 3].Value = GameViewModel.Instance.BlocksForTeam2.Where(x => x.PlayerWhoBlocked.SkaterId == mems.SkaterId).Count();
                            reportSheet.Cells[rowReport, 4].Value = GameViewModel.Instance.AssistsForTeam2.Where(x => x.PlayerWhoAssisted.SkaterId == mems.SkaterId).Count();
                        }
                        rowReport += 1;
                    }
                    catch (Exception exception)
                    {
                        ErrorViewModel.Save(exception, exception.GetType());
                    }
                }
                reportSheet.Cells["A1:K20"].AutoFitColumns();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }

        }



        private static void ExportScores(ExcelPackage p)
        {
            try
            {
                ExcelWorksheet reportSheet = p.Workbook.Worksheets.Add("Scores");
                reportSheet.Name = "Scores"; //Setting Sheet's name
                reportSheet.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                reportSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
                //blank,T1,T2, blank, blank, T1, T2
                reportSheet.Cells[1, 2, 1, 4].Merge = true;
                reportSheet.Cells[1, 2, 1, 4].Value = GameViewModel.Instance.Team1.TeamName;
                reportSheet.Cells[1, 5, 1, 7].Merge = true;
                reportSheet.Cells[1, 5, 1, 7].Value = GameViewModel.Instance.Team2.TeamName;
                SetBorderCell(reportSheet.Cells[1, 2, 1, 4]);
                SetBorderCell(reportSheet.Cells[1, 5, 1, 7]);
                reportSheet.Cells[2, 1].Value = "Jam";
                SetScoresHeader(reportSheet.Cells[2, 1]);

                reportSheet.Cells[2, 2].Value = "Scorer's Name";
                reportSheet.Cells[2, 5].Value = "Scorer's Name";
                SetScoresHeader(reportSheet.Cells[2, 2]);
                SetScoresHeader(reportSheet.Cells[2, 5]);

                reportSheet.Cells[2, 3].Value = "Scorer's Number";
                reportSheet.Cells[2, 6].Value = "Scorer's Number";
                SetScoresHeader(reportSheet.Cells[2, 3]);
                SetScoresHeader(reportSheet.Cells[2, 6]);

                reportSheet.Cells[2, 4].Value = "Points";
                reportSheet.Cells[2, 7].Value = "Points";
                SetScoresHeader(reportSheet.Cells[2, 4]);
                SetScoresHeader(reportSheet.Cells[2, 7]);


                int rowReport = 3;
                foreach (var jam in GameViewModel.Instance.Jams)
                {
                    try
                    {
                        SetColorCell(reportSheet.Cells[rowReport, 1], Color.Yellow);
                        SetColorCell(reportSheet.Cells[rowReport, 2], Color.Yellow, rowReport);
                        SetColorCell(reportSheet.Cells[rowReport, 3], Color.Yellow, rowReport);
                        SetColorCell(reportSheet.Cells[rowReport, 4], Color.Yellow, rowReport);
                        SetColorCell(reportSheet.Cells[rowReport, 5], Color.Yellow, rowReport);
                        SetColorCell(reportSheet.Cells[rowReport, 6], Color.Yellow, rowReport);
                        SetColorCell(reportSheet.Cells[rowReport, 7], Color.Yellow, rowReport);
                        SetBorderCell(reportSheet.Cells[rowReport, 1]);
                        SetBorderCell(reportSheet.Cells[rowReport, 2]);
                        SetBorderCell(reportSheet.Cells[rowReport, 3]);
                        SetBorderCell(reportSheet.Cells[rowReport, 4]);
                        SetBorderCell(reportSheet.Cells[rowReport, 5]);
                        SetBorderCell(reportSheet.Cells[rowReport, 6]);
                        SetBorderCell(reportSheet.Cells[rowReport, 7]);

                        reportSheet.Cells[rowReport, 1].Value = jam.JamNumber;

                        reportSheet.Cells[rowReport, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        //finding the point scorer for team 1
                        var score1 = GameViewModel.Instance.ScoresTeam1.Where(x => x.JamNumber == jam.JamNumber).FirstOrDefault();
                        if (score1 != null)
                        {
                            if (score1.PlayerWhoScored != null)
                            {
                                reportSheet.Cells[rowReport, 2].Value = score1.PlayerWhoScored.SkaterName;
                                reportSheet.Cells[rowReport, 3].Value = score1.PlayerWhoScored.SkaterNumber;
                                reportSheet.Cells[rowReport, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }
                        }
                        var score2 = GameViewModel.Instance.ScoresTeam2.Where(x => x.JamNumber == jam.JamNumber).FirstOrDefault();
                        if (score2 != null)
                        {
                            if (score2.PlayerWhoScored != null)
                            {
                                reportSheet.Cells[rowReport, 5].Value = score2.PlayerWhoScored.SkaterName;
                                reportSheet.Cells[rowReport, 6].Value = score2.PlayerWhoScored.SkaterNumber;
                                reportSheet.Cells[rowReport, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }
                        }

                        reportSheet.Cells[rowReport, 4].Value = GameViewModel.Instance.ScoresTeam1.Where(x => x.JamNumber == jam.JamNumber).Count();
                        reportSheet.Cells[rowReport, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        reportSheet.Cells[rowReport, 7].Value = GameViewModel.Instance.ScoresTeam2.Where(x => x.JamNumber == jam.JamNumber).Count();
                        reportSheet.Cells[rowReport, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    catch (Exception exception)
                    {
                        ErrorViewModel.Save(exception, exception.GetType());
                    }
                    rowReport += 1;

                }
                reportSheet.Cells[rowReport, 4].Formula = "=sum(d3:d" + (rowReport - 1) + ")";
                reportSheet.Cells[rowReport, 7].Formula = "=sum(g3:g" + (rowReport - 1) + ")";
                reportSheet.Cells["A1:K20"].AutoFitColumns();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }



        private static void ExportPenalties(ExcelPackage p)
        {
            try
            {
                ExcelWorksheet reportSheet = p.Workbook.Worksheets.Add("Penalties");
                reportSheet.Name = "Penalties"; //Setting Sheet's name
                reportSheet.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                reportSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
                reportSheet.Cells[1, 1, 1, 8].Merge = true;
                reportSheet.Cells[1, 1, 1, 8].Value = GameViewModel.Instance.Team1.TeamName;

                reportSheet.Cells[1, 10, 1, 17].Merge = true;
                reportSheet.Cells[1, 10, 1, 17].Value = GameViewModel.Instance.Team2.TeamName;

                reportSheet.Cells[2, 1].Value = "Name";
                reportSheet.Cells[2, 2].Value = "#";
                reportSheet.Cells[2, 8].Value = "TOT";
                reportSheet.Cells[2, 10].Value = "Name";
                reportSheet.Cells[2, 11].Value = "#";
                reportSheet.Cells[2, 17].Value = "TOT";
                SetScoresHeader(reportSheet.Cells[2, 1]);
                SetScoresHeader(reportSheet.Cells[2, 2]);
                SetScoresHeader(reportSheet.Cells[2, 3]);
                SetScoresHeader(reportSheet.Cells[2, 4]);
                SetScoresHeader(reportSheet.Cells[2, 5]);
                SetScoresHeader(reportSheet.Cells[2, 6]);
                SetScoresHeader(reportSheet.Cells[2, 7]);
                SetScoresHeader(reportSheet.Cells[2, 8]);
                SetScoresHeader(reportSheet.Cells[2, 10]);
                SetScoresHeader(reportSheet.Cells[2, 11]);
                SetScoresHeader(reportSheet.Cells[2, 12]);
                SetScoresHeader(reportSheet.Cells[2, 13]);
                SetScoresHeader(reportSheet.Cells[2, 14]);
                SetScoresHeader(reportSheet.Cells[2, 15]);
                SetScoresHeader(reportSheet.Cells[2, 16]);
                SetScoresHeader(reportSheet.Cells[2, 17]);

                //penalties column
                reportSheet.Cells[2, 9, 3, 9].Merge = true;
                reportSheet.Cells[2, 9, 3, 9].Value = "Minors";
                SetPenaltyTitle(reportSheet.Cells[2, 9, 3, 9]);

                reportSheet.Cells[4, 9].Value = PenaltyViewModel.ToAbbreviation(PenaltiesMADEEnum.Cutting_Track);
                reportSheet.Cells[5, 9].Value = RDN.Utilities.Enums.EnumExt.ToFreindlyName(PenaltiesMADEEnum.Cutting_Track);
                SetPenaltyInitials(reportSheet.Cells[4, 9]);
                SetPenaltyName(reportSheet.Cells[5, 9]);
                SetBorderCell(reportSheet.Cells[4, 9, 5, 9]);
                reportSheet.Cells[6, 9].Value = PenaltyViewModel.ToAbbreviation(PenaltiesMADEEnum.Blocking_Out_Of_Play);
                reportSheet.Cells[7, 9].Value = RDN.Utilities.Enums.EnumExt.ToFreindlyName(PenaltiesMADEEnum.Blocking_Out_Of_Play);
                SetPenaltyInitials(reportSheet.Cells[6, 9]);
                SetPenaltyName(reportSheet.Cells[7, 9]);
                SetBorderCell(reportSheet.Cells[6, 9, 7, 9]);
                reportSheet.Cells[8, 9].Value = PenaltyViewModel.ToAbbreviation(PenaltiesMADEEnum.Blocking_Out_Of_Bounds);
                reportSheet.Cells[9, 9].Value = RDN.Utilities.Enums.EnumExt.ToFreindlyName(PenaltiesMADEEnum.Blocking_Out_Of_Bounds);
                SetPenaltyInitials(reportSheet.Cells[8, 9]);
                SetPenaltyName(reportSheet.Cells[9, 9]);
                SetBorderCell(reportSheet.Cells[8, 9, 9, 9]);
                reportSheet.Cells[10, 9].Value = PenaltyViewModel.ToAbbreviation(PenaltiesMADEEnum.False_Start);
                reportSheet.Cells[11, 9].Value = RDN.Utilities.Enums.EnumExt.ToFreindlyName(PenaltiesMADEEnum.False_Start);
                SetPenaltyInitials(reportSheet.Cells[10, 9]);
                SetPenaltyName(reportSheet.Cells[11, 9]);
                SetBorderCell(reportSheet.Cells[10, 9, 11, 9]);
                reportSheet.Cells[12, 9].Value = PenaltyViewModel.ToAbbreviation(PenaltiesMADEEnum.Illegal_Contact);
                reportSheet.Cells[13, 9].Value = RDN.Utilities.Enums.EnumExt.ToFreindlyName(PenaltiesMADEEnum.Illegal_Contact);
                SetPenaltyInitials(reportSheet.Cells[12, 9]);
                SetPenaltyName(reportSheet.Cells[13, 9]);
                SetBorderCell(reportSheet.Cells[12, 9, 13, 9]);
                reportSheet.Cells[14, 9].Value = PenaltyViewModel.ToAbbreviation(PenaltiesMADEEnum.Clockwise_Skating);
                reportSheet.Cells[15, 9].Value = RDN.Utilities.Enums.EnumExt.ToFreindlyName(PenaltiesMADEEnum.Clockwise_Skating);
                SetPenaltyInitials(reportSheet.Cells[14, 9]);
                SetPenaltyName(reportSheet.Cells[15, 9]);
                SetBorderCell(reportSheet.Cells[14, 9, 15, 9]);
                reportSheet.Cells[16, 9, 17, 9].Merge = true;
                reportSheet.Cells[16, 9, 17, 9].Value = "Majors";
                SetPenaltyTitle(reportSheet.Cells[16, 9, 17, 9]);

                reportSheet.Cells[18, 9].Value = PenaltyViewModel.ToAbbreviation(PenaltiesMADEEnum.Excessive_Force);
                reportSheet.Cells[19, 9].Value = RDN.Utilities.Enums.EnumExt.ToFreindlyName(PenaltiesMADEEnum.Excessive_Force);
                SetBorderCell(reportSheet.Cells[18, 9, 19, 9]);
                SetPenaltyInitials(reportSheet.Cells[18, 9]);
                SetPenaltyName(reportSheet.Cells[19, 9]);
                reportSheet.Cells[20, 9].Value = PenaltyViewModel.ToAbbreviation(PenaltiesMADEEnum.Team_Penalty);
                reportSheet.Cells[21, 9].Value = RDN.Utilities.Enums.EnumExt.ToFreindlyName(PenaltiesMADEEnum.Team_Penalty);
                SetBorderCell(reportSheet.Cells[20, 9, 21, 9]);
                SetPenaltyInitials(reportSheet.Cells[20, 9]);
                SetPenaltyName(reportSheet.Cells[21, 9]);

                reportSheet.Cells[22, 9, 23, 9].Merge = true;
                reportSheet.Cells[22, 9, 23, 9].Value = "Ejection";
                SetPenaltyTitle(reportSheet.Cells[22, 9, 23, 9]);

                reportSheet.Cells[24, 9].Value = PenaltyViewModel.ToAbbreviation(PenaltiesMADEEnum.Abusive_Language);
                reportSheet.Cells[25, 9].Value = RDN.Utilities.Enums.EnumExt.ToFreindlyName(PenaltiesMADEEnum.Abusive_Language);
                SetBorderCell(reportSheet.Cells[24, 9, 25, 9]);
                SetPenaltyInitials(reportSheet.Cells[24, 9]);
                SetPenaltyName(reportSheet.Cells[25, 9]);

                int rowReport = 3;
                //creates a row for each member
                foreach (var mems in GameViewModel.Instance.Team1.TeamMembers)
                {
                    try
                    {
                        SetPenaltyRow(reportSheet.Cells[rowReport, 1], rowReport);
                        SetPenaltyRow(reportSheet.Cells[rowReport, 2], rowReport);

                        reportSheet.Cells[rowReport, 1].Value = mems.SkaterName;
                        reportSheet.Cells[rowReport, 2].Value = mems.SkaterNumber;
                        reportSheet.Cells[rowReport, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        var pens = GameViewModel.Instance.PenaltiesForTeam1.Where(x => x.PenaltyAgainstMember.SkaterId == mems.SkaterId).ToList();
                        int penColumn = 3;
                        foreach (var pen in pens)
                        {
                            reportSheet.Cells[rowReport, penColumn].Value = PenaltyViewModel.ToAbbreviation(pen.PenaltyType);
                            penColumn += 1;
                        }
                        for (int i = 3; i < 8; i++)
                            SetPenaltyRow(reportSheet.Cells[rowReport, i], rowReport);

                        reportSheet.Cells[rowReport, 8].Value = pens.Count;
                        SetPenaltyRow(reportSheet.Cells[rowReport, 8], rowReport);
                        reportSheet.Cells[rowReport, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        rowReport += 1;
                    }
                    catch (Exception exception)
                    {
                        ErrorViewModel.Save(exception, exception.GetType());
                    }
                }
                rowReport = 3;
                foreach (var mems in GameViewModel.Instance.Team2.TeamMembers)
                {
                    try
                    {
                        SetPenaltyRow(reportSheet.Cells[rowReport, 10], rowReport);
                        SetPenaltyRow(reportSheet.Cells[rowReport, 11], rowReport);

                        reportSheet.Cells[rowReport, 10].Value = mems.SkaterName;
                        reportSheet.Cells[rowReport, 11].Value = mems.SkaterNumber;
                        reportSheet.Cells[rowReport, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        var pens = GameViewModel.Instance.PenaltiesForTeam2.Where(x => x.PenaltyAgainstMember.SkaterId == mems.SkaterId).ToList();
                        int penColumn = 12;

                        foreach (var pen in pens)
                        {
                            reportSheet.Cells[rowReport, penColumn].Value = PenaltyViewModel.ToAbbreviation(pen.PenaltyType);
                            penColumn += 1;
                        }
                        for (int i = 12; i < 17; i++)
                            SetPenaltyRow(reportSheet.Cells[rowReport, i], rowReport);

                        reportSheet.Cells[rowReport, 17].Value = pens.Count;
                        SetPenaltyRow(reportSheet.Cells[rowReport, 17], rowReport);
                        reportSheet.Cells[rowReport, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        rowReport += 1;
                    }
                    catch (Exception exception)
                    {
                        ErrorViewModel.Save(exception, exception.GetType());
                    }
                }
                reportSheet.Cells["A1:K20"].AutoFitColumns();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }

        }

        private static void SetPenaltyRow(ExcelRange cell, int rowReport)
        {
            SetColorCell(cell, Color.Yellow, rowReport);
            SetBorderCell(cell);
        }

        private static void SetPenaltyTitle(ExcelRange cell)
        {
            SetColorCell(cell, Color.Gray);
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.Font.Size = 14;
            cell.Style.Font.Bold = true;
            SetBorderCell(cell);
        }
        private static void SetPenaltyInitials(ExcelRange cell)
        {
            cell.Style.Font.Bold = true;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.Font.Size = 14;
        }
        private static void SetPenaltyName(ExcelRange cell)
        {

            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        }


        private static void SetScoresHeader(ExcelRange cell)
        {
            SetColorCell(cell, Color.Black);
            SetFontColorCell(cell, Color.White);
            SetBorderCell(cell);
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.Font.Bold = true;
        }

        private static void SetColorCell(ExcelRange cell, Color color, int rowReport)
        {
            if (rowReport % 2 == 0)
                SetColorCell(cell, color);
        }

        private static void SetColorCell(ExcelRange cell, Color color)
        {
            var fill = cell.Style.Fill;
            fill.PatternType = ExcelFillStyle.Solid;
            fill.BackgroundColor.SetColor(color);
        }

        private static void SetFontColorCell(ExcelRange cell, Color color)
        {
            cell.Style.Font.Color.SetColor(color);
        }

        private static void SetBorderCell(ExcelRange cell)
        {
            var border = cell.Style.Border;
            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
        }

        private static void CreateHeaderStyle(ExcelRange cell)
        {
            cell.Style.Font.Bold = true;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.Font.Size = 18;
            cell.Style.Font.Name = "Arial Black";
        }
        private static void CreateHeaderStyle2(ExcelRange cell)
        {
            cell.Style.Font.Bold = true;
            cell.Style.Font.Size = 12;
            cell.Style.Font.Name = "Arial Black";
        }
    }
}
