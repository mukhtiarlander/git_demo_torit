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
using RDN.Utilities.Enums;
using Scoreboard.Library.Static.Enums;
using Scoreboard.Library.Util;
using Scoreboard.Library.ViewModel;

namespace Scoreboard.Library.Classes.Reports.Excel
{
    public class WftdaReport
    {
        private static readonly string ScannedMessage = "A scanned IBRF with signatures and the completed stats are due within 2 weeks of bout date to: sanctioning@wftda.com";
        private static readonly string RevMessage = "IBRF Rev. 130308 © 2013 Women's Flat Track Derby Association (WFTDA)";
      private  Color _tabPink = Color.FromArgb(255, 138, 255);
        public string fileName;

        public WftdaReport Initialize()
        {
            return this;
        }

        public WftdaReport SetFileName(string fileName)
        {
            this.fileName = fileName;
            return this;
        }
        public void Export()
        {
            var saveGameTask = Task<bool>.Factory.StartNew(
                  () =>
                  {
                      try
                      {

                          ExportWFTDAStatsReport();


                      }
                      catch (Exception exception)
                      {
                          Logger.Instance.logMessage("tried to save the game", LoggerEnum.message);
                          ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                      }
                      return true;
                  });
        }
        private void ExportWFTDAStatsReport()
        {
            using (ExcelPackage p = new ExcelPackage())
            {
                try
                {
                    p.Workbook.Properties.Author = "RDNation.com v" + ScoreboardConfig.SCOREBOARD_VERSION_NUMBER;
                    p.Workbook.Properties.Title = "WFTDA Report For " + GameViewModel.Instance.GameName;
                    Color tabPink = Color.FromArgb(255, 138, 255);
                    ExportIBRF(p);
                    ExportScores(p);
                    ExportPenalties(p);
                    ExportLineups(p);
                    ExportBoutSummary(p);
                    ExportPenaltySummary(p);

                }
                catch (Exception exception)
                {
                    ErrorViewModel.Save(exception, exception.GetType());
                }
                FileInfo fi = new FileInfo(this.fileName);
                p.SaveAs(fi);
            }
        }

        #region PenaltySummary
        private void ExportPenaltySummary(ExcelPackage p)
        {
            try
            {
                Color pink1 = Color.FromArgb(255, 185, 255);
                Color pink2 = Color.FromArgb(255, 232, 255);
                Color pinkFont = Color.FromArgb(255, 128, 128);
                Color yellow = Color.FromArgb(255, 255, 102);
                Color yellowBright = Color.FromArgb(255, 255, 0);
                Color offPink = Color.FromArgb(255, 204, 153);
                Color blueDark = Color.FromArgb(0, 97, 193);


                ExcelWorksheet reportSheet = p.Workbook.Worksheets.Add("Penalty Summary");
                reportSheet.Name = "Penalty Summary"; //Setting Sheet's name
                
                reportSheet.Cells.Style.Font.Size = 10; //Default font size for whole sheet
                reportSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

                int row = 1;
                int doubleColumn = 0;
                int headerColumn = 1;
                reportSheet.Cells[row, 1 + doubleColumn, row, 25 + doubleColumn].SetValue("P E N A L T I E S   S U M M A R Y").Merge().SetFontBold().SetCenterAlign().SetFontSize(14);

                row += 1;
                PenaltiesSummaryHeader("Home", pink1, reportSheet, row, doubleColumn);

                row += 1;
                PenaltiesSummaryHeaderAttributes(reportSheet, row, doubleColumn);

                row += 1;
                for (int i = row; i < row + 20; i++)
                    PenaltiesSummaryRow(pink2, pink1, yellow, yellowBright, reportSheet, i, doubleColumn);

                row += 20;
                PenaltiesSummaryFooterAttributes(pink1, pink2, reportSheet, row, doubleColumn);

                row = 30;

                reportSheet.Cells[row, 1 + doubleColumn, row, 25 + doubleColumn].SetValue("P E N A L T I E S   S U M M A R Y").Merge().SetFontBold().SetCenterAlign().SetFontSize(14);

                row += 1;
                PenaltiesSummaryHeader("Away", pink1, reportSheet, row, doubleColumn);

                row += 1;
                PenaltiesSummaryHeaderAttributes(reportSheet, row, doubleColumn);

                row += 1;
                for (int i = row; i < row + 20; i++)
                    PenaltiesSummaryRow(pink2, pink1, yellow, yellowBright, reportSheet, i, doubleColumn);

                row += 20;
                PenaltiesSummaryFooterAttributes(pink1, pink2, reportSheet, row, doubleColumn);

                ////borders
                //reportSheet.Cells[2, 1 + doubleColumn, row - 1, 3 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                //reportSheet.Cells[2, 4 + doubleColumn, row - 1, 20 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                //reportSheet.Cells[2, 21 + doubleColumn, row - 1, 21 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                //reportSheet.Cells[2, 22 + doubleColumn, row - 1, 22 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                //reportSheet.Cells[2, 23 + doubleColumn, row - 1, 25 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                //reportSheet.Cells[1, 1 + doubleColumn, row - 1, 25 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }

        private static void PenaltiesSummaryRow(Color pink2, Color pink1, Color yellow, Color yellowBright, ExcelWorksheet reportSheet, int row, int doubleColumn)
        {
            if (row % 2 == 0)
                reportSheet.Cells[row, 1 + doubleColumn, row, 25 + doubleColumn].SetBackgroundColor(pink2);
            reportSheet.Cells[row, 1 + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 2 + doubleColumn, row, 3 + doubleColumn].Merge().SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 4 + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 5 + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 6 + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 7 + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 8 + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 9 + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 10 + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 11 + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 12 + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 13 + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 14 + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 15 + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 16 + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 17 + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 18 + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 19 + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 20 + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink2);
            reportSheet.Cells[row, 21 + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink2);
            reportSheet.Cells[row, 22 + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 23 + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink2);
            reportSheet.Cells[row, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink2);
            reportSheet.Cells[row, 25 + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(yellow);

            if (row % 2 == 0)
            {
                reportSheet.Cells[row, 23 + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink1);
                reportSheet.Cells[row, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink1);
                reportSheet.Cells[row, 20 + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink1);
                reportSheet.Cells[row, 21 + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink1);
                reportSheet.Cells[row, 25 + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(yellowBright);
            }
        }
        private static void PenaltiesSummaryFooterAttributes(Color pink1, Color pink2, ExcelWorksheet reportSheet, int row, int doubleColumn)
        {
            try
            {

                reportSheet.Cells[row, 1 + doubleColumn, 2 + row, 3 + doubleColumn].Merge().SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 4 + doubleColumn, 2 + row, 4 + doubleColumn].Merge().SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Back_Block)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 5 + doubleColumn, 2 + row, 5 + doubleColumn].Merge().SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.High_Block)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 6 + doubleColumn, 2 + row, 6 + doubleColumn].Merge().SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Low_Block)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 7 + doubleColumn, 2 + row, 7 + doubleColumn].Merge().SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Elbows)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 8 + doubleColumn, 2 + row, 8 + doubleColumn].Merge().SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Forearms)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 9 + doubleColumn, 2 + row, 9 + doubleColumn].Merge().SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Blocking_With_Head)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 10 + doubleColumn, 2 + row, 10 + doubleColumn].Merge().SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Multi_Player_Block)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 11 + doubleColumn, 2 + row, 11 + doubleColumn].Merge().SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Blocking_Out_Of_Bounds)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 12 + doubleColumn, 2 + row, 12 + doubleColumn].Merge().SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Direction_Of_Gameplay)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 13 + doubleColumn, 2 + row, 13 + doubleColumn].Merge().SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Out_Of_Play)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 14 + doubleColumn, 2 + row, 14 + doubleColumn].Merge().SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Cut_Track)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 15 + doubleColumn, 2 + row, 15 + doubleColumn].Merge().SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Skating_Out_Of_Bounds)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 16 + doubleColumn, 2 + row, 16 + doubleColumn].Merge().SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Illegal_Procedure)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 17 + doubleColumn, 2 + row, 17 + doubleColumn].Merge().SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Insubordination)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 18 + doubleColumn, 2 + row, 18 + doubleColumn].Merge().SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Delay_Of_Game)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 19 + doubleColumn, 2 + row, 19 + doubleColumn].Merge().SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Misconduct_Gross)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 20 + doubleColumn, 2 + row, 20 + doubleColumn].Merge().SetValue("TOTAL").SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 21 + doubleColumn, 2 + row, 21 + doubleColumn].Merge().SetBackgroundColor(pink1);
                reportSheet.Cells[row, 22 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row + 1, 22 + doubleColumn, row + 2, 22 + doubleColumn].Merge().SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Thin).SetValue("Total Expulsions").SetCenterAlign();
                reportSheet.Cells[row, 23 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 24 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 25 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Thin);

                reportSheet.Cells[row + 1, 23 + doubleColumn, row + 2, 25 + doubleColumn].SetBackgroundColor(Color.Black).SetValue("Team Averages").SetFontColor(Color.White).Merge().SetCenterAlign();

                row += 3;
                reportSheet.Cells[row, 1 + doubleColumn, row + 2, 3 + doubleColumn].SetBackgroundColor(pink1).SetValue("MAJOR TOTALS").Merge().SetCenterAlign();
                reportSheet.Cells[row, 4 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 5 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 6 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 7 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 8 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 9 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 10 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 11 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 12 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 13 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 14 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 15 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 16 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 17 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 18 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 19 + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 20 + doubleColumn].SetBackgroundColor(pink1).SetFontBold().SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 21 + doubleColumn].SetBackgroundColor(pink1).SetFontBold();
                reportSheet.Cells[row, 22 + doubleColumn, row, 25 + doubleColumn].SetValue("Total Penalty Minutes").SetBackgroundColor(pink1).Merge().SetFontBold().SetCenterAlign();

                row += 1;
                reportSheet.Cells[row, 4 + doubleColumn, row, 9 + doubleColumn].Merge().SetValue("Avg Majors / Jam:").SetBackgroundColor(pink2).SetRightAlignment();

                reportSheet.Cells[row + 1, 4 + doubleColumn, row + 1, 9 + doubleColumn].Merge().SetValue("… vs opponent:").SetBackgroundColor(pink2).SetRightAlignment();

                reportSheet.Cells[row, 10 + doubleColumn, row, 11 + doubleColumn].Merge().SetBackgroundColor(pink2);

                reportSheet.Cells[row + 1, 10 + doubleColumn, row + 1, 11 + doubleColumn].Merge().SetBackgroundColor(pink2);

                reportSheet.Cells[row, 12 + doubleColumn, row + 1, 16 + doubleColumn].Merge().SetValue("Team Total Majors as % of Bout Total:").SetBackgroundColor(pink2).SetRightAlignment();

                reportSheet.Cells[row, 17 + doubleColumn, row + 1, 17 + doubleColumn].Merge().SetBackgroundColor(pink2);

                reportSheet.Cells[row, 18 + doubleColumn, row + 1, 19 + doubleColumn].Merge().SetBackgroundColor(pink2);
                reportSheet.Cells[row, 20 + doubleColumn, row + 1, 20 + doubleColumn].Merge().SetBackgroundColor(pink2).SetCenterAlign();
                reportSheet.Cells[row, 21 + doubleColumn, row + 1, 21 + doubleColumn].Merge().SetBackgroundColor(pink2).SetCenterAlign();

                reportSheet.Cells[row, 22 + doubleColumn, row + 1, 25 + doubleColumn].Merge().SetValue("of Bout Total Penalty Minutes").SetBackgroundColor(pink2).SetCenterAlign();
                //borders
                reportSheet.Cells[row, 21 + doubleColumn, row + 1, 25 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }

        private static void PenaltiesSummaryHeaderAttributes(ExcelWorksheet reportSheet, int row, int doubleColumn)
        {
            try
            {
                reportSheet.Cells[row, 1 + doubleColumn].SetValue("#").SetBackgroundColor(Color.Black).SetFontColor(Color.White).SetCenterBottom().SetFontSize(12);
                reportSheet.Cells[row, 2 + doubleColumn, row, 3 + doubleColumn].Merge().SetValue("SKATER").SetBackgroundColor(Color.Black).SetFontColor(Color.White).SetCenterBottom().SetFontSize(11);
                reportSheet.Cells[row, 4 + doubleColumn].SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Back_Block)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 5 + doubleColumn].SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.High_Block)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 6 + doubleColumn].SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Low_Block)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 7 + doubleColumn].SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Elbows)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 8 + doubleColumn].SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Forearms)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 9 + doubleColumn].SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Blocking_With_Head)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 10 + doubleColumn].SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Multi_Player_Block)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 11 + doubleColumn].SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Blocking_Out_Of_Bounds)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 12 + doubleColumn].SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Direction_Of_Gameplay)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 13 + doubleColumn].SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Out_Of_Play)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 14 + doubleColumn].SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Cut_Track)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 15 + doubleColumn].SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Skating_Out_Of_Bounds)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 16 + doubleColumn].SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Illegal_Procedure)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 17 + doubleColumn].SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Insubordination)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 18 + doubleColumn].SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Delay_Of_Game)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 19 + doubleColumn].SetValue(EnumExt.ToFreindlyName(PenaltiesWFTDAEnum.Misconduct_Gross)).SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 20 + doubleColumn].SetValue("TOTAL").SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 21 + doubleColumn].SetValue("Penalty Minutes").SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 22 + doubleColumn].SetValue("Expulsion").SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 23 + doubleColumn].SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 24 + doubleColumn].SetValue("Jams Skated").SetPenaltySummaryHeaderAttribute();
                reportSheet.Cells[row, 25 + doubleColumn].SetValue("PM Per Jam").SetPenaltySummaryHeaderAttribute();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }

        private static void PenaltiesSummaryHeader(string teamName, Color pink1, ExcelWorksheet reportSheet, int row, int doubleColumn)
        {
            try
            {
                reportSheet.Cells[row, 1 + doubleColumn, row, 3 + doubleColumn].SetValue(teamName + " Team").Merge().SetBackgroundColor(pink1).SetFontBold().SetCenterAlign().SetFontSize(12).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 4 + doubleColumn, row, 20 + doubleColumn].Merge().SetBackgroundColor(pink1).SetFontBold().SetCenterAlign().SetFontSize(12).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 21 + doubleColumn].SetValue("PM").SetBackgroundColor(pink1).SetCenterAlign().SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 22 + doubleColumn].SetBackgroundColor(pink1).SetCenterAlign().SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 23 + doubleColumn, row, 25 + doubleColumn].Merge().SetValue("EXTRAPOLATED").SetBackgroundColor(pink1).SetCenterAlign().SetBorder(ExcelBorderStyle.Medium);

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }

        #endregion

        #region BoutSummary
        private void ExportBoutSummary(ExcelPackage p)
        {
            try
            {
                Color pink1 = Color.FromArgb(255, 185, 255);
                Color pink2 = Color.FromArgb(255, 232, 255);
                Color pinkFont = Color.FromArgb(255, 128, 128);
                Color yellow = Color.FromArgb(255, 255, 102);
                Color offPink = Color.FromArgb(255, 204, 153);
                Color blueDark = Color.FromArgb(0, 97, 193);


                ExcelWorksheet reportSheet = p.Workbook.Worksheets.Add("Bout Summary");
                reportSheet.Name = "Bout Summary"; //Setting Sheet's name
                reportSheet.Cells.Style.Font.Size = 10; //Default font size for whole sheet
                reportSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

                int row = 1;
                int doubleColumn = 0;
                int headerColumn = 1;
                reportSheet.Cells[row, 1 + doubleColumn, row, 35 + doubleColumn].SetValue("W F T D A    R E Q U I R E D    S T A N D A R D I Z E D   S T A T S").Merge().SetFontColor(pinkFont).SetFontBold().SetCenterAlign();
                reportSheet.Cells[row, 36 + doubleColumn, row, 72 + doubleColumn].SetValue("W F T D A    E X T R A    C R E D I T   S T A T S").Merge().SetFontColor(pinkFont).SetFontBold().SetCenterAlign();

                row += 1;
                reportSheet.Cells[row, 1 + doubleColumn, row, 35 + doubleColumn].SetValue("PLEASE FILL IN THE IBRF TAB!").Merge().SetCenterAlign();
                reportSheet.Cells[row, 36 + doubleColumn, row, 72 + doubleColumn].SetValue("PLEASE FILL IN THE IBRF TAB!").Merge().SetCenterAlign();
                row += 1;
                reportSheet.Cells[row, 1 + doubleColumn, row, 35 + doubleColumn].SetValue("ENTER DATE ON IBRF TAB!").Merge().SetCenterAlign();
                reportSheet.Cells[row, 36 + doubleColumn, row, 72 + doubleColumn].SetValue("ENTER DATE ON IBRF TAB!").Merge().SetCenterAlign();

                row += 1;
                BoutSummaryHeader(reportSheet, row, doubleColumn);

                row += 1;
                BoutSummaryHeaderAttributes("Home", pink1, reportSheet, row, doubleColumn, ref headerColumn);
                reportSheet.Cells[row, 1 + doubleColumn, row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Medium);

                row += 1;
                for (int i = row; i < row + 20; i++)
                {
                    BoutSummarRow(pink1, pink2, yellow, reportSheet, i, doubleColumn, ref headerColumn);
                }
                row += 20;
                headerColumn = 1;
                BoutSummaryFooters(pink1, reportSheet, row, doubleColumn, ref headerColumn);
                //setting borders
                reportSheet.Cells[4, 1 + doubleColumn, row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[4, 8 + doubleColumn, row, 9 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[4, 17 + doubleColumn, row, 26 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[4, 34 + doubleColumn, row, 35 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[4, 36 + doubleColumn, row, 37 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[4, 38 + doubleColumn, row, 43 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[4, 44 + doubleColumn, row, 51 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[4, 52 + doubleColumn, row, 57 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[4, 58 + doubleColumn, row, 66 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[4, 67 + doubleColumn, row, 72 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);

                row += 1;
                BoutSummaryHeader(reportSheet, row, doubleColumn);

                row += 1;
                headerColumn = 1;
                BoutSummaryHeaderAttributes("Away", pink1, reportSheet, row, doubleColumn, ref headerColumn);

                row += 1;
                for (int i = row; i < row + 20; i++)
                {
                    BoutSummarRow(pink1, pink2, yellow, reportSheet, i, doubleColumn, ref headerColumn);
                }
                row += 20;
                headerColumn = 1;
                BoutSummaryFooters(pink1, reportSheet, row, doubleColumn, ref headerColumn);
                //setting borders
                reportSheet.Cells[28, 1 + doubleColumn, row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[28, 8 + doubleColumn, row, 9 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[28, 17 + doubleColumn, row, 26 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[28, 34 + doubleColumn, row, 35 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[28, 36 + doubleColumn, row, 37 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[28, 38 + doubleColumn, row, 43 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[28, 44 + doubleColumn, row, 51 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[28, 52 + doubleColumn, row, 57 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[28, 58 + doubleColumn, row, 66 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[28, 67 + doubleColumn, row, 72 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);


            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }

        private static void BoutSummaryFooters(Color pink1, ExcelWorksheet reportSheet, int row, int doubleColumn, ref int headerColumn)
        {
            try
            {
                reportSheet.Cells[row, headerColumn + doubleColumn, row, 1 + headerColumn + doubleColumn].SetValue("TEAM SUMMARIES").Merge().SetCenterAlign().SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 2;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);


                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1).SetValue("N/A");
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1).SetValue("N/A");
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1).SetValue("N/A");
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1).SetValue("N/A");
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1).SetValue("N/A");
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1).SetValue("N/A");
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1).SetValue("N/A");

                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn, row, 1 + headerColumn + doubleColumn].Merge().SetBoutSummaryFooterValues(pink1);

                headerColumn += 2;
                reportSheet.Cells[row, headerColumn + doubleColumn, row, 1 + headerColumn + doubleColumn].SetValue("TEAM SUMMARIES").Merge().SetCenterAlign().SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);

                reportSheet.Cells[row, 1 + doubleColumn, row, headerColumn + doubleColumn].SetBoutSummaryFooterValues(pink1);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }
        /// <summary>
        /// produces a row for the bout summary
        /// </summary>
        /// <param name="pink1"></param>
        /// <param name="pink2"></param>
        /// <param name="yellow"></param>
        /// <param name="reportSheet"></param>
        /// <param name="row"></param>
        /// <param name="doubleColumn"></param>
        /// <param name="headerColumn"></param>
        private static void BoutSummarRow(Color pink1, Color pink2, Color yellow, ExcelWorksheet reportSheet, int row, int doubleColumn, ref int headerColumn)
        {
            try
            {
                headerColumn = 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBackgroundColor(pink2).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink2);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBackgroundColor(pink2).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBackgroundColor(pink2).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBackgroundColor(pink2).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBackgroundColor(pink2).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBackgroundColor(yellow).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBackgroundColor(yellow).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBackgroundColor(yellow).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBackgroundColor(yellow).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn, row, 1 + headerColumn + doubleColumn].Merge().SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn, row, 1 + headerColumn + doubleColumn].SetBackgroundColor(pink2).SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 2;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink2);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink2);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink2);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink2);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink2);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(pink2);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }

        private static void BoutSummaryHeaderAttributes(string teamName, Color pink1, ExcelWorksheet reportSheet, int row, int doubleColumn, ref int headerColumn)
        {
            try
            {
                reportSheet.Cells[row, 1 + doubleColumn].SetValue("#").SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Thin).SetFontSize(11).SetCenterAlign();
                reportSheet.Cells[row, 2 + doubleColumn].SetValue(teamName + " Team").SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Thin).SetFontSize(11).SetCenterBottom();
                reportSheet.Cells[row, 3 + doubleColumn].SetValue("JAMMER").SetBackgroundColor(pink1).SetBoutSummaryAttributesHeader2();
                reportSheet.Cells[row, 4 + doubleColumn].SetValue("PIVOT").SetBackgroundColor(pink1).SetBoutSummaryAttributesHeader2();
                reportSheet.Cells[row, 5 + doubleColumn].SetValue("BLOCKER").SetBackgroundColor(pink1).SetBoutSummaryAttributesHeader2();
                reportSheet.Cells[row, 6 + doubleColumn].SetValue("TOTAL").SetBoutSummaryAttributesHeader3();
                reportSheet.Cells[row, 7 + doubleColumn].SetValue("%  JAMS SKATED").SetBoutSummaryAttributesHeader3();
                reportSheet.Cells[row, 8 + doubleColumn].SetValue("POINTS").SetBoutSummaryAttributesHeader3();
                reportSheet.Cells[row, 9 + doubleColumn].SetValue("PPJ").SetBoutSummaryAttributesHeader3();
                reportSheet.Cells[row, 10 + doubleColumn].SetValue("LOST").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                reportSheet.Cells[row, 11 + doubleColumn].SetValue("LEAD").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                reportSheet.Cells[row, 12 + doubleColumn].SetValue("CALLED").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                reportSheet.Cells[row, 13 + doubleColumn].SetValue("NO PASS").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                reportSheet.Cells[row, 14 + doubleColumn].SetValue("LEAD %").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                reportSheet.Cells[row, 15 + doubleColumn].SetValue("LEAD +/-").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                reportSheet.Cells[row, 16 + doubleColumn].SetValue("AVERAGE LEAD +/-").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                reportSheet.Cells[row, 17 + doubleColumn].SetValue("PTS  FOR").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                reportSheet.Cells[row, 18 + doubleColumn].SetValue("PTS AGAINST").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                reportSheet.Cells[row, 19 + doubleColumn].SetValue("TOTAL +/-").SetBoutSummaryAttributesHeader3();
                headerColumn = 20;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("JAMMER +/-").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("AVERAGE JAMMER +/-").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("PIVOT +/-").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("AVERAGE PIVOT +/-").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("BLOCK +/-").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("AVERAGE BLOCKER +/-").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("AVG +/-").SetBoutSummaryAttributesHeader3();
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("VTAR PTS FOR").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("VTAR  PTS AGAINST").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("VTAR  TOTAL +/-").SetBoutSummaryAttributesHeader3();
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("VTAR JAMMER AVG +/-").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("VTAR PIVOT AVG +/-").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("VTAR BLOCKER AVG +/-").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("TOTAL VTAR AVG +/-").SetBoutSummaryAttributesHeader3();
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn, row, 1 + headerColumn + doubleColumn].Merge().SetValue("PENALTY MINUTES").SetBoutSummaryAttributesHeader3();
                headerColumn += 2;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("#").SetBorder(ExcelBorderStyle.Thin).SetFontSize(11).SetBackgroundColor(pink1).SetCenterAlign();
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue(teamName + " Team").SetBorder(ExcelBorderStyle.Thin).SetFontSize(11).SetBackgroundColor(pink1).SetCenterBottom();


                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("OFF.  BLK").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("OFF.  KD").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("WHIP").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("PUSH").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("DOZER").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("ASSISTS").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("1/4 TRACK").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("FORCEOUT").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("JAMMER HIT").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("Jammer KD").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("Block Assist").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("ATTACKS").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("TOTAL").SetBoutSummaryAttributesHeader3();
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("KD's").SetBoutSummaryAttributesHeader3();


                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("ASST/JAM").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("ASSIST %").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("ATT/JAM").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("ATTACK %").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("Action/Jam").SetBoutSummaryAttributesHeader3();
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("ACTION %").SetBoutSummaryAttributesHeader3();


                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("JUKED").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("MISSED HIT").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("INEFFECTIVE").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("HIT & FALL").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("KNOCKED DN").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("TOTAL").SetBoutSummaryAttributesHeader3();
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("HITTING ACCURACY").SetBoutSummaryAttributesHeader3();
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("EFFECTIVE HITTING").SetBoutSummaryAttributesHeader3();
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("EFFECTIVE BLOCKING").SetBoutSummaryAttributesHeader3();

                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("O.B.A.").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("JUKE").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("JUMP").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("ROLLOFF").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("HIP WHIP").SetBoutSummaryAttributesHeader2().SetBackgroundColor(pink1);
                headerColumn += 1;
                reportSheet.Cells[row, headerColumn + doubleColumn].SetValue("TOTAL").SetBoutSummaryAttributesHeader3();

                reportSheet.Cells[row, 1 + doubleColumn, row, headerColumn + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 8 + doubleColumn, row, 9 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 17 + doubleColumn, row, 26 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 36 + doubleColumn, row, 37 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 38 + doubleColumn, row, 43 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 44 + doubleColumn, row, 51 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 52 + doubleColumn, row, 57 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 58 + doubleColumn, row, 66 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 67 + doubleColumn, row, 72 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }

        private static void BoutSummaryHeader(ExcelWorksheet reportSheet, int row, int doubleColumn)
        {
            try
            {
                reportSheet.Cells[row, 1 + doubleColumn, row, 2 + doubleColumn].SetValue("ROSTERS").SetBoutSummaryHeader();
                reportSheet.Cells[row, 3 + doubleColumn, row, 6 + doubleColumn].SetValue("JAMS PLAYED").SetBoutSummaryHeader();
                reportSheet.Cells[row, 7 + doubleColumn].SetBoutSummaryHeader();
                reportSheet.Cells[row, 8 + doubleColumn].SetBoutSummaryHeader();
                reportSheet.Cells[row, 9 + doubleColumn].SetBoutSummaryHeader();
                reportSheet.Cells[row, 10 + doubleColumn, row, 15 + doubleColumn].SetValue("LEAD JAMMER").SetBoutSummaryHeader();
                reportSheet.Cells[row, 16 + doubleColumn].SetBoutSummaryHeader();
                reportSheet.Cells[row, 17 + doubleColumn, row, 26 + doubleColumn].SetValue("POINTS FOR/AGAINST & PLUS/MINUS").SetBoutSummaryHeader();
                reportSheet.Cells[row, 27 + doubleColumn, row, 33 + doubleColumn].SetValue("V.T.A.R.").SetBoutSummaryHeader();
                reportSheet.Cells[row, 34 + doubleColumn, row, 35 + doubleColumn].SetValue("PENALTIES").SetBoutSummaryHeader();
                reportSheet.Cells[row, 36 + doubleColumn, row, 37 + doubleColumn].SetValue("ROSTERS").SetBoutSummaryHeader();
                reportSheet.Cells[row, 38 + doubleColumn, row, 51 + doubleColumn].SetValue("OFFENSIVE / DEFENSIVE ACTIONS").SetBoutSummaryHeader();
                reportSheet.Cells[row, 52 + doubleColumn, row, 56 + doubleColumn].SetValue("PER JAM & PERCENTAGE").SetBoutSummaryHeader();
                reportSheet.Cells[row, 57 + doubleColumn].SetBoutSummaryHeader();
                reportSheet.Cells[row, 58 + doubleColumn, row, 66 + doubleColumn].SetValue("ERRORS").SetBoutSummaryHeader();
                reportSheet.Cells[row, 67 + doubleColumn, row, 72 + doubleColumn].SetValue("JAMMER ACTIONS").SetBoutSummaryHeader();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }
        #endregion

        #region Lineups
        private void ExportLineups(ExcelPackage p)
        {
            try
            {
                Color blue1 = Color.FromArgb(230, 242, 255);
                Color blue2 = Color.FromArgb(206, 231, 255);
                Color offPink = Color.FromArgb(255, 204, 153);
                Color blueDark = Color.FromArgb(0, 97, 193);


                ExcelWorksheet reportSheet = p.Workbook.Worksheets.Add("Lineups");
                reportSheet.Name = "Lineups"; //Setting Sheet's name
                reportSheet.TabColor = _tabPink;
                reportSheet.Cells.Style.Font.Size = 10; //Default font size for whole sheet
                reportSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

                int row = 1;
                LineupsGenerateSheetPerPeriodPerTeam(blue1, blue2, blueDark, offPink, reportSheet, 1, row);
                //period 2
                row = 85;
                LineupsGenerateSheetPerPeriodPerTeam(blue1, blue2, blueDark, offPink, reportSheet, 2, row);

                reportSheet.Cells["A1:AV172"].AutoFitColumns();

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }
        private static void LineupsGenerateSheetPerPeriodPerTeam(Color blue1, Color blue2, Color blueDark, Color offPink, ExcelWorksheet reportSheet, int period, int row)
        {
            try
            {
                int doubleColumn = 0;
                reportSheet.Cells[row, 1 + doubleColumn, row + 1, 8 + doubleColumn].SetValue("Home Team").Merge().SetBackgroundColor(blue1).SetFontSize(14).SetCenterAlign();
                //line up
                reportSheet.Cells[row, 9 + doubleColumn, row, 14 + doubleColumn].Merge().SetBorderBottom(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 15 + doubleColumn, row, 17 + doubleColumn].Merge().SetBorderBottom(ExcelBorderStyle.Thin).SetBackgroundColor(blue1);
                reportSheet.Cells[row, 18 + doubleColumn].SetBackgroundColor(blueDark).SetValue(period.ToString()).SetFontSize(24).SetCenterAlign().SetFontBold().SetFontColor(Color.White);
                reportSheet.Cells[row, 21 + doubleColumn, row, 22 + doubleColumn].Merge().SetBorderBottom(ExcelBorderStyle.Thin);

                reportSheet.Cells[row, 23 + doubleColumn, row + 1, 30 + doubleColumn].SetValue("Away Team").Merge().SetBackgroundColor(blue1).SetFontSize(14).SetCenterAlign();
                //line up
                reportSheet.Cells[row, 31 + doubleColumn, row, 36 + doubleColumn].Merge().SetBorderBottom(ExcelBorderStyle.Thin);
                //date
                reportSheet.Cells[row, 37 + doubleColumn, row, 39 + doubleColumn].Merge().SetBackgroundColor(blue1).SetBorderBottom(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 40 + doubleColumn].SetBackgroundColor(blueDark).SetValue(period.ToString()).SetFontSize(24).SetFontBold().SetCenterAlign().SetFontColor(Color.White);
                reportSheet.Cells[row, 43 + doubleColumn, row, 44 + doubleColumn].Merge().SetBorderBottom(ExcelBorderStyle.Thin);

                row += 1; //2
                reportSheet.Cells[row, 9 + doubleColumn, row, 14 + doubleColumn].Merge().SetValue("Lineup Tracker").SetCenterAlign();
                reportSheet.Cells[row, 15 + doubleColumn, row, 17 + doubleColumn].SetBackgroundColor(blue1).SetValue("Date").SetCenterAlign().Merge();
                reportSheet.Cells[row, 18 + doubleColumn].SetBackgroundColor(blueDark);
                reportSheet.Cells[row, 21 + doubleColumn, row, 22 + doubleColumn].Merge().SetValue("Color").SetCenterAlign();
                reportSheet.Cells[row, 31 + doubleColumn, row, 36 + doubleColumn].Merge().SetValue("Lineup Tracker").SetCenterAlign();
                reportSheet.Cells[row, 37 + doubleColumn, row, 39 + doubleColumn].Merge().SetBackgroundColor(blue1).SetValue("Date").SetCenterAlign();
                reportSheet.Cells[row, 40 + doubleColumn].SetBackgroundColor(blueDark);
                reportSheet.Cells[row, 43 + doubleColumn, row, 44 + doubleColumn].Merge().SetValue("Color").SetCenterAlign();

                int secondSheetStart = 22;

                row += 1; //2
                LineupsSetHeaderRow(reportSheet, row, doubleColumn);
                LineupsSetHeaderRow(reportSheet, row, secondSheetStart);

                row += 1;
                int modulation = 1;
                for (int i = row; i < row + 76; i += 2)
                {
                    LineupsDisplayRow(blue1, blue2, offPink, reportSheet, i, doubleColumn, modulation % 2 == 0);
                    LineupsDisplayRow(blue1, blue2, offPink, reportSheet, i, secondSheetStart, modulation % 2 == 0);
                    modulation += 1;
                }
                row += 75;


                row += 1;//80
                LineupsBottomMessages(reportSheet, row, doubleColumn);
                LineupsBottomMessages(reportSheet, row, secondSheetStart);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }

        private static void LineupsBottomMessages(ExcelWorksheet reportSheet, int row, int doubleColumn)
        {
            reportSheet.Cells[row, 1 + doubleColumn, row + 4, 19 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
            reportSheet.Cells[row, 1 + doubleColumn, row, 19 + doubleColumn].Merge().SetValue("Write the jam number, starting from 1 each period, in the JAM column as each jam happens. Enter skater numbers by position. If no Pivot is fielded, enter an X in the noPivot box.");
            row += 1;
            reportSheet.Cells[row, 1 + doubleColumn, row, 19 + doubleColumn].Merge().SetValue("During the jam, circle the opposing jammer's current pass. When a skater sits in the penalty box, write that pass number in the left BOX column, or an S if the skater starts the jam in the box.");
            row += 1;
            reportSheet.Cells[row, 1 + doubleColumn, row, 19 + doubleColumn].Merge().SetValue("When the skater reenters the track from the box, write the current pass in the right column. If jam is called for injury, indicate it with an INJ in the BOX column of the injured skater.");
            row += 1;
            reportSheet.Cells[row, 1 + doubleColumn, row, 19 + doubleColumn].Merge().SetValue("When a star pass occurs, enter SP as in the JAM column on a new row, with jammer and pivot reversed (mark noPivot), same blockers, and maintain current pass count.");
            row += 1;
            reportSheet.Cells[row, 1 + doubleColumn, row, 19 + doubleColumn].Merge().SetValue("Note any box time carrying over from first period onto the period 2 sheet before turning in period 1 sheet.");
        }

        private static void LineupsDisplayRow(Color blue1, Color blue2, Color offPink, ExcelWorksheet reportSheet, int row, int doubleColumn, bool isAlternatingRow)
        {
            if (isAlternatingRow)
                reportSheet.Cells[row, 1 + doubleColumn, row + 1, 17 + doubleColumn].SetBackgroundColor(blue1);

            reportSheet.Cells[row, 1 + doubleColumn, row + 1, 1 + doubleColumn].Merge().SetBackgroundColor(blue1).SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 2 + doubleColumn, row + 1, 2 + doubleColumn].Merge().SetBackgroundColor(blue1).SetBorder(ExcelBorderStyle.Thin);

            //jammer
            reportSheet.Cells[row, 3 + doubleColumn, row + 1, 3 + doubleColumn].Merge().SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 4 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row + 1, 4 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 5 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row + 1, 5 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);
            //reportSheet.Column(4 + doubleColumn).Width = 3;

            //pivot
            reportSheet.Cells[row, 6 + doubleColumn, row + 1, 6 + doubleColumn].Merge().SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 7 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row + 1, 7 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 8 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row + 1, 8 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);

            //B1
            reportSheet.Cells[row, 9 + doubleColumn, row + 1, 9 + doubleColumn].Merge().SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 10 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row + 1, 10 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 11 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row + 1, 11 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);

            //B2
            reportSheet.Cells[row, 12 + doubleColumn, row + 1, 12 + doubleColumn].Merge().SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 13 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row + 1, 13 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 14 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row + 1, 14 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);

            //B3
            reportSheet.Cells[row, 15 + doubleColumn, row + 1, 15 + doubleColumn].Merge().SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 16 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row + 1, 16 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row, 17 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);
            reportSheet.Cells[row + 1, 17 + doubleColumn].SetBackgroundColor(blue2).SetBorder(ExcelBorderStyle.Thin);

            reportSheet.Cells[row, 18 + doubleColumn].SetValue("1  2  3  4  5").SetFontSize(9).SetCenterAlign();
            reportSheet.Cells[row + 1, 18 + doubleColumn].SetValue("6  7  8  9  10").SetFontSize(9).SetCenterAlign();
            reportSheet.Cells[row, 18 + doubleColumn, row + 1, 18 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);

            reportSheet.Cells[row, 19 + doubleColumn, row + 1, 19 + doubleColumn].Merge().SetBorder(ExcelBorderStyle.Medium);
            reportSheet.Cells[row, 1 + doubleColumn, row + 1, 19 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);

            if (isAlternatingRow)
            {
                reportSheet.Cells[row, 1 + doubleColumn, row + 1, 1 + doubleColumn].SetBackgroundColor(blue2);
                reportSheet.Cells[row, 2 + doubleColumn, row + 1, 2 + doubleColumn].SetBackgroundColor(blue2);
                reportSheet.Cells[row, 19 + doubleColumn, row + 1, 19 + doubleColumn].SetBackgroundColor(offPink);
            }
        }

        private static void LineupsSetHeaderRow(ExcelWorksheet reportSheet, int row, int doubleColumn)
        {
            reportSheet.Cells[row, 1 + doubleColumn].SetValue("Jam").SetTitleRowLineupPage();
            reportSheet.Cells[row, 2 + doubleColumn].SetValue("noPivot").SetTitleRowLineupPage();
            reportSheet.Cells[row, 3 + doubleColumn].SetValue("Jammer").SetTitleRowLineupPage();
            reportSheet.Cells[row, 4 + doubleColumn, row, 5 + doubleColumn].SetValue("Box").SetTitleRowLineupPage();
            reportSheet.Cells[row, 6 + doubleColumn].SetValue("Pivot").SetTitleRowLineupPage();
            reportSheet.Cells[row, 7 + doubleColumn, row, 8 + doubleColumn].SetValue("Box").SetTitleRowLineupPage();
            reportSheet.Cells[row, 9 + doubleColumn].SetValue("Blocker").SetTitleRowLineupPage();
            reportSheet.Cells[row, 10 + doubleColumn, row, 11 + doubleColumn].SetValue("Box").SetTitleRowLineupPage();
            reportSheet.Cells[row, 12 + doubleColumn].SetValue("Blocker").SetTitleRowLineupPage();
            reportSheet.Cells[row, 13 + doubleColumn, row, 14 + doubleColumn].SetValue("Box").SetTitleRowLineupPage();
            reportSheet.Cells[row, 15 + doubleColumn].SetValue("Blocker").SetTitleRowLineupPage();
            reportSheet.Cells[row, 16 + doubleColumn, row, 17 + doubleColumn].SetValue("Box").SetTitleRowLineupPage();
            reportSheet.Cells[row, 18 + doubleColumn].SetValue("Curr Pass").SetTitleRowLineupPage();
            reportSheet.Cells[row, 19 + doubleColumn].SetTitleRowLineupPage();
            reportSheet.Cells[row, 20 + doubleColumn].SetTitleRowLineupPage();
            reportSheet.Cells[row, 21 + doubleColumn, row, 22 + doubleColumn].SetValue("Team Roster").SetTitleRowLineupPage();
        }

        #endregion

        #region Penalties
        private void ExportPenalties(ExcelPackage p)
        {
            try
            {
                Color pink1 = Color.FromArgb(255, 232, 255);
                Color pink2 = Color.FromArgb(255, 208, 255);
                Color pinkDark = Color.FromArgb(196, 0, 196);
                Color lightBlue = Color.FromArgb(217, 217, 217);

                ExcelWorksheet reportSheet = p.Workbook.Worksheets.Add("Penalties");
                reportSheet.Name = "Penalties"; //Setting Sheet's name
                reportSheet.TabColor = _tabPink;
                reportSheet.Cells.Style.Font.Size = 10; //Default font size for whole sheet
                reportSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

                int row = 1;
                PenaltiesGenerateSheetPerPeriod(ref pink1, ref pink2, ref pinkDark, ref lightBlue, reportSheet, 1, row, 0);
                row = 1;
                //number of columns to skip to create the second sheet.
                int doubleColumn = 24;
                PenaltiesGenerateSheetPerPeriod(ref pink1, ref pink2, ref pinkDark, ref lightBlue, reportSheet, 2, row, doubleColumn);
                reportSheet.Cells["A1:AV172"].AutoFitColumns();

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }

        private static void PenaltiesGenerateSheetPerPeriod(ref Color pink1, ref Color pink2, ref Color pinkDark, ref Color lightBlue, ExcelWorksheet reportSheet, int period, int row, int doubleColumn)
        {
            try
            {
                reportSheet.Cells[row, 1 + doubleColumn, row + 1, 8 + doubleColumn].SetValue("Home Team").Merge().SetBackgroundColor(pink1).SetFontSize(14).SetCenterAlign();

                reportSheet.Cells[row, 9 + doubleColumn, row, 10 + doubleColumn].Merge().SetBorderBottom(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 11 + doubleColumn].SetBackgroundColor(pink1).SetBorderBottom(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 12 + doubleColumn, row, 13 + doubleColumn].Merge().SetBorderBottom(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 14 + doubleColumn, row + 1, 21 + doubleColumn].SetValue("Away Team").Merge().SetBackgroundColor(pink1).SetFontSize(14).SetCenterAlign();
                reportSheet.Cells[row, 22 + doubleColumn, row, 23 + doubleColumn].Merge().SetBorderBottom(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 24 + doubleColumn].SetFontSize(24).SetCenterAlign().SetValue(period.ToString()).SetFontBold().SetBackgroundColor(pinkDark).SetFontColor(Color.White);

                row += 1; //2
                reportSheet.Cells[row, 9 + doubleColumn, row, 10 + doubleColumn].Merge().SetValue("Color").SetCenterAlign();
                reportSheet.Cells[row, 11 + doubleColumn].SetBackgroundColor(pink1).SetValue("Date").SetCenterAlign();
                reportSheet.Cells[row, 12 + doubleColumn, row, 13 + doubleColumn].Merge().SetValue("Penalty Tracker").SetCenterAlign();
                reportSheet.Cells[row, 22 + doubleColumn, row, 23 + doubleColumn].Merge().SetValue("Color").SetCenterAlign();
                reportSheet.Cells[row, 24 + doubleColumn].SetBackgroundColor(pinkDark);


                row += 1;//3
                PenaltiesRowHeaderOfSheet(reportSheet, row, doubleColumn);

                row += 1; //4
                DisplayPenaltyCodes(reportSheet, row, doubleColumn);

                int modulateRowColor = 1;
                int notesRow = 1;
                for (int i = row; i < row + 40; i += 2)
                {
                    //we need to set the row colors first because they are overwritten by other colors after
                    // the cells.
                    //full row of home or away secition
                    if (modulateRowColor % 2 == 0)
                    {
                        reportSheet.Cells[i, 2 + doubleColumn, i + 1, 10 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                        reportSheet.Cells[i, 1 + doubleColumn, i + 1, 1 + doubleColumn].Merge().SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                    }
                    else
                    {
                        reportSheet.Cells[i, 2 + doubleColumn, i + 1, 10 + doubleColumn].SetBorder(ExcelBorderStyle.Medium).SetBackgroundColor(pink1);
                        reportSheet.Cells[i, 1 + doubleColumn, i + 1, 1 + doubleColumn].Merge().SetBackgroundColor(pink2).SetBorder(ExcelBorderStyle.Medium);
                    }
                    //full row of home or away secition
                    if (modulateRowColor % 2 == 0)
                    {
                        reportSheet.Cells[i, 14 + doubleColumn, i + 1, 23 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                        reportSheet.Cells[i, 14 + doubleColumn, i + 1, 14 + doubleColumn].Merge().SetBackgroundColor(pink1).SetBorder(ExcelBorderStyle.Medium);
                    }
                    else
                    {
                        reportSheet.Cells[i, 14 + doubleColumn, i + 1, 23 + doubleColumn].SetBorder(ExcelBorderStyle.Medium).SetBackgroundColor(pink1);
                        reportSheet.Cells[i, 14 + doubleColumn, i + 1, 14 + doubleColumn].Merge().SetBackgroundColor(pink2).SetBorder(ExcelBorderStyle.Medium);
                    }



                    //displays the penalty Jam Row
                    for (int j = 2; j < 9; j++)
                    {
                        reportSheet.Cells[i, j + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                        reportSheet.Cells[i + 1, j + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                    }
                    //Fo/EXP column
                    reportSheet.Cells[i, 9 + doubleColumn].SetBorder(ExcelBorderStyle.Dotted).SetBackgroundColor(pink2);
                    reportSheet.Cells[i + 1, 9 + doubleColumn].SetBorder(ExcelBorderStyle.Dotted).SetBackgroundColor(pink2);
                    reportSheet.Cells[i, 9 + doubleColumn, i + 1, 9 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                    //total column
                    reportSheet.Cells[i, 10 + doubleColumn, i + 1, 10 + doubleColumn].Merge().SetBorder(ExcelBorderStyle.Medium);


                    //row color sets background color
                    reportSheet.Cells[i, 11 + doubleColumn, i, 13 + doubleColumn].Merge().SetValue((notesRow).ToString()).SetCenterAlign();
                    reportSheet.Cells[i + 1, 11 + doubleColumn, i + 1, 13 + doubleColumn].Merge().SetValue((notesRow + 1).ToString()).SetBackgroundColor(lightBlue).SetCenterAlign();


                    //displays the penalty Jam Row
                    for (int j = 15; j < 22; j++)
                    {
                        reportSheet.Cells[i, j + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                        reportSheet.Cells[i + 1, j + doubleColumn].SetBorder(ExcelBorderStyle.Thin);
                    }

                    reportSheet.Cells[i, 22 + doubleColumn].SetBorder(ExcelBorderStyle.Dotted).SetBackgroundColor(pink2);
                    reportSheet.Cells[i + 1, 22 + doubleColumn].SetBorder(ExcelBorderStyle.Dotted).SetBackgroundColor(pink2);
                    //fo/exp column away team
                    reportSheet.Cells[i, 22 + doubleColumn, i + 1, 22 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                    reportSheet.Cells[i, 23 + doubleColumn, i + 1, 23 + doubleColumn].Merge().SetBorder(ExcelBorderStyle.Medium);

                    reportSheet.Cells[i, 2 + doubleColumn, i + 1, 10 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                    notesRow += 2;
                    modulateRowColor += 1;
                }

                row = 44;
                reportSheet.Cells[row, 1 + doubleColumn, row, 9 + doubleColumn].Merge().SetBackgroundColor(Color.Black).SetFontColor(Color.White).SetValue("TOTAL PENALTIES FOR PERIOD:").SetRightAlignment().SetFontBold();
                reportSheet.Cells[row, 10 + doubleColumn].Merge().SetBackgroundColor(pink2).SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 14 + doubleColumn, row, 22 + doubleColumn].Merge().SetBackgroundColor(Color.Black).SetFontColor(Color.White).SetValue("TOTAL PENALTIES FOR PERIOD:").SetRightAlignment().SetFontBold();
                reportSheet.Cells[row, 23 + doubleColumn].Merge().SetBackgroundColor(pink2).SetBorder(ExcelBorderStyle.Medium);

                row += 1;
                reportSheet.Cells[row, 1 + doubleColumn, row + 3, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
                reportSheet.Cells[row, 1 + doubleColumn, row, 24 + doubleColumn].Merge().SetValue("PENALTY / Jam #: Enter codes for penalties in the upper row and jam # in the lower row for each skater.");
                row += 1;
                reportSheet.Cells[row, 1 + doubleColumn, row, 24 + doubleColumn].Merge().SetValue("FO/EXP: Foul Outs (FO) for penalty minutes should be marked as PM. Expulsions (EXP) should be listed by the appropriate penalty code. ");
                row += 1;
                reportSheet.Cells[row, 1 + doubleColumn, row, 24 + doubleColumn].Merge().SetValue("TOTAL: At the end of each period, add the number of penalties for each skater for that period and put it in the 'TOTAL' column.");
                row += 1;
                reportSheet.Cells[row, 1 + doubleColumn, row, 24 + doubleColumn].Merge().SetValue("CARRY OVER: Before period 2, transfer the penalties from period 1 by shading in the equivalent number of boxes. ");
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }

        private static void DisplayPenaltyCodes(ExcelWorksheet reportSheet, int rowNumber, int doubleColumn)
        {
            int row = rowNumber;
            //displayCodes
            reportSheet.Cells[row, 24 + doubleColumn].SetPenaltyAbbrieviation(PenaltiesWFTDAEnum.Back_Block);
            reportSheet.Cells[row + 1, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Back_Block);
            reportSheet.Cells[row, 24 + doubleColumn, row + 1, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
            row += 2;

            reportSheet.Cells[row, 24 + doubleColumn].SetPenaltyAbbrieviation(PenaltiesWFTDAEnum.High_Block);
            reportSheet.Cells[row + 1, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.High_Block);
            reportSheet.Cells[row, 24 + doubleColumn, row + 1, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
            row += 2;

            reportSheet.Cells[row, 24 + doubleColumn].SetPenaltyAbbrieviation(PenaltiesWFTDAEnum.Low_Block);
            reportSheet.Cells[row + 1, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Low_Block);
            reportSheet.Cells[row, 24 + doubleColumn, row + 1, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
            row += 2;

            reportSheet.Cells[row, 24 + doubleColumn].SetPenaltyAbbrieviation(PenaltiesWFTDAEnum.Elbows);
            reportSheet.Cells[row + 1, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Elbows);
            reportSheet.Cells[row, 24 + doubleColumn, row + 1, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
            row += 2;

            reportSheet.Cells[row, 24 + doubleColumn].SetPenaltyAbbrieviation(PenaltiesWFTDAEnum.Forearms);
            reportSheet.Cells[row + 1, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Forearms);
            reportSheet.Cells[row, 24 + doubleColumn, row + 1, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
            row += 2;

            reportSheet.Cells[row, 24 + doubleColumn].SetPenaltyAbbrieviation(PenaltiesWFTDAEnum.Blocking_With_Head);
            reportSheet.Cells[row + 1, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Blocking_With_Head);
            reportSheet.Cells[row, 24 + doubleColumn, row + 1, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
            row += 2;

            reportSheet.Cells[row, 24 + doubleColumn].SetPenaltyAbbrieviation(PenaltiesWFTDAEnum.Multi_Player_Block);
            reportSheet.Cells[row + 1, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Multi_Player_Block);
            reportSheet.Cells[row, 24 + doubleColumn, row + 1, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
            row += 2;

            reportSheet.Cells[row, 24 + doubleColumn].SetPenaltyAbbrieviation(PenaltiesWFTDAEnum.Blocking_Out_Of_Bounds);
            reportSheet.Cells[row + 1, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Blocking_Out_Of_Bounds);
            reportSheet.Cells[row + 2, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Assisting_Out_Of_Bounds);
            reportSheet.Cells[row, 24 + doubleColumn, row + 2, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
            row += 3;

            reportSheet.Cells[row, 24 + doubleColumn].SetPenaltyAbbrieviation(PenaltiesWFTDAEnum.Clockwise_To_Block);
            reportSheet.Cells[row + 1, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Direction_Of_Gameplay);
            reportSheet.Cells[row + 2, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Clockwise_To_Block);
            reportSheet.Cells[row, 24 + doubleColumn, row + 3, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
            row += 4;

            reportSheet.Cells[row, 24 + doubleColumn].SetPenaltyAbbrieviation(PenaltiesWFTDAEnum.Out_Of_Play);
            reportSheet.Cells[row + 1, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Out_Of_Play);
            reportSheet.Cells[row + 2, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Destruction_Of_Pack);
            reportSheet.Cells[row + 3, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Failure_To_Reform);
            reportSheet.Cells[row, 24 + doubleColumn, row + 3, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
            row += 4;

            reportSheet.Cells[row, 24 + doubleColumn].SetPenaltyAbbrieviation(PenaltiesWFTDAEnum.Cut_Track);
            reportSheet.Cells[row + 1, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Cut_Track);
            reportSheet.Cells[row, 24 + doubleColumn, row + 1, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
            row += 2;

            reportSheet.Cells[row, 24 + doubleColumn].SetPenaltyAbbrieviation(PenaltiesWFTDAEnum.Skating_Out_Of_Bounds);
            reportSheet.Cells[row + 1, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Skating_Out_Of_Bounds);
            reportSheet.Cells[row, 24 + doubleColumn, row + 1, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
            row += 2;

            reportSheet.Cells[row, 24 + doubleColumn].SetPenaltyAbbrieviation(PenaltiesWFTDAEnum.Illegal_Procedure);
            reportSheet.Cells[row + 1, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Illegal_Procedure);
            reportSheet.Cells[row + 2, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.False_Start);
            reportSheet.Cells[row + 3, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Violation);
            reportSheet.Cells[row, 24 + doubleColumn, row + 3, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
            row += 4;

            reportSheet.Cells[row, 24 + doubleColumn].SetPenaltyAbbrieviation(PenaltiesWFTDAEnum.Insubordination);
            reportSheet.Cells[row + 1, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Insubordination);
            reportSheet.Cells[row, 24 + doubleColumn, row + 1, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
            row += 2;

            reportSheet.Cells[row, 24 + doubleColumn].SetPenaltyAbbrieviation(PenaltiesWFTDAEnum.Delay_Of_Game);
            reportSheet.Cells[row + 1, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Delay_Of_Game);
            reportSheet.Cells[row, 24 + doubleColumn, row + 1, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);
            row += 2;

            reportSheet.Cells[row, 24 + doubleColumn].SetPenaltyAbbrieviation(PenaltiesWFTDAEnum.Misconduct_Gross);
            reportSheet.Cells[row + 1, 24 + doubleColumn].SetPenaltyName(PenaltiesWFTDAEnum.Misconduct_Gross);
            reportSheet.Cells[row, 24 + doubleColumn, row + 2, 24 + doubleColumn].SetBorder(ExcelBorderStyle.Medium);


        }
        private static void PenaltiesRowHeaderOfSheet(ExcelWorksheet reportSheet, int row, int doubleColumn)
        {
            try
            {
                reportSheet.Cells[row, 1 + doubleColumn].SetValue("#").SetTitleRowScoresPage();
                reportSheet.Cells[row, 2 + doubleColumn, row, 8 + doubleColumn].Merge().SetValue("PENALTY / JAM #").Merge().SetTitleRowScoresPage();
                reportSheet.Cells[row, 9 + doubleColumn].SetValue("FO/EXP").SetTitleRowScoresPage();
                reportSheet.Cells[row, 10 + doubleColumn].SetValue("TOTAL").SetTitleRowScoresPage();
                reportSheet.Cells[row, 11 + doubleColumn, row, 13 + doubleColumn].Merge().SetValue("NOTES").SetTitleRowScoresPage();

                reportSheet.Cells[row, 14 + doubleColumn].SetValue("#").SetTitleRowScoresPage();
                reportSheet.Cells[row, 15 + doubleColumn, row, 21 + doubleColumn].Merge().SetValue("PENALTY / JAM #").SetTitleRowScoresPage();
                reportSheet.Cells[row, 22 + doubleColumn].SetValue("FO/EXP").SetTitleRowScoresPage();
                reportSheet.Cells[row, 23 + doubleColumn].SetValue("TOTAL").SetTitleRowScoresPage();
                reportSheet.Cells[row, 24 + doubleColumn].SetValue("Codes").SetCenterAlign().SetBackgroundColor(Color.Gray);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }

        #endregion

        #region Scores

        private void ExportScores(ExcelPackage p)
        {
            try
            {
                Color green1 = Color.FromArgb(233, 245, 218);
                Color green2 = Color.FromArgb(211, 236, 181);
                Color greenDark = Color.FromArgb(74, 112, 29);

                ExcelWorksheet reportSheet = p.Workbook.Worksheets.Add("Score");
                reportSheet.Name = "Score"; //Setting Sheet's name
                reportSheet.TabColor = _tabPink;
                reportSheet.Cells.Style.Font.Size = 10; //Default font size for whole sheet
                reportSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

                int row = 1;
                ScoresFillTeamSheet(ref green1, ref green2, ref greenDark, reportSheet, ref row, 1);
                row = 87;
                ScoresFillTeamSheet(ref green1, ref green2, ref greenDark, reportSheet, ref row, 2);
                reportSheet.Cells["A1:AM172"].AutoFitColumns();

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }

        private static void ScoresFillTeamSheet(ref Color green1, ref Color green2, ref Color greenDark, ExcelWorksheet reportSheet, ref int row, int teamNumber)
        {
            try
            {
                reportSheet.Cells[row, 1, row + 1, 8].SetValue("Home Team").Merge().SetBackgroundColor(green1).SetFontSize(14).SetCenterAlign();

                reportSheet.Cells[row, 9, row, 10].Merge().SetBorderBottom(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 11].SetBackgroundColor(green1).SetBorderBottom(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 12, row, 14].Merge().SetBorderBottom(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 15, row, 17].Merge().SetBorderBottom(ExcelBorderStyle.Thin).SetBackgroundColor(green1);
                reportSheet.Cells[row, 18].SetFontSize(24).SetCenterAlign().SetValue(teamNumber.ToString()).SetFontBold().SetBackgroundColor(greenDark).SetFontColor(Color.White);

                reportSheet.Cells[row, 20, row + 1, 27].SetValue("Away Team").Merge().SetBackgroundColor(green1).SetFontSize(14).SetCenterAlign();
                reportSheet.Cells[row, 28, row, 29].Merge().SetBorderBottom(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 30].SetBackgroundColor(green1).SetBorderBottom(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 31, row, 33].Merge().SetBorderBottom(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 34, row, 36].Merge().SetBorderBottom(ExcelBorderStyle.Thin).SetBackgroundColor(green1);

                reportSheet.Cells[row, 37].SetFontSize(24).SetCenterAlign().SetValue(teamNumber.ToString()).SetFontBold().SetBackgroundColor(greenDark).SetFontColor(Color.White);

                row += 1; //2
                reportSheet.Cells[row, 9, row, 10].Merge().SetValue("Color").SetCenterAlign();
                reportSheet.Cells[row, 11].SetBackgroundColor(green1).SetValue("Date").SetCenterAlign();
                reportSheet.Cells[row, 12, row, 14].Merge().SetValue("Scorekeeper").SetCenterAlign();
                reportSheet.Cells[row, 15, row, 17].Merge().SetValue("Jammer Ref").SetBackgroundColor(green1).SetCenterAlign();
                reportSheet.Cells[row, 18].SetBackgroundColor(greenDark);

                reportSheet.Cells[row, 28, row, 29].Merge().SetValue("Color").SetCenterAlign();
                reportSheet.Cells[row, 30].SetBackgroundColor(green1).SetValue("Date").SetCenterAlign();
                reportSheet.Cells[row, 31, row, 33].Merge().SetValue("Scorekeeper").SetCenterAlign();
                reportSheet.Cells[row, 34, row, 36].Merge().SetValue("Jammer Ref").SetBackgroundColor(green1).SetCenterAlign();
                reportSheet.Cells[row, 37].SetFontSize(24).SetBackgroundColor(greenDark);

                row += 1;//3
                int doubleColumn = 19;
                ScoresRowHeaderOfSheet(reportSheet, row, 0);
                ScoresRowHeaderOfSheet(reportSheet, row, doubleColumn);

                row += 1; //4
                int modulatorRow = 1;
                for (int i = row; i < row + 76; i += 2)
                {
                    ScoresRowQuarterOfSheet(ref green1, ref green2, reportSheet, i, 0, modulatorRow);
                    ScoresRowQuarterOfSheet(ref green1, ref green2, reportSheet, i, doubleColumn, modulatorRow);
                    modulatorRow += 1;
                }

                row += 76;

                reportSheet.Cells[row, 1, row + 1, 1].Merge().SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(green2);
                //sets the bottom row of the sheet.
                for (int i = 1; i < 38; i++)
                    reportSheet.Cells[row, i, row + 1, i].Merge().SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(green2);

                reportSheet.Cells[row, 2, row + 1, 2].Merge().SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(green2).SetValue("PERIOD TOTALS").SetFontSize(11);
                reportSheet.Cells[row, 20, row + 1, 20].Merge().SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(green2).SetValue("PERIOD TOTALS").SetFontSize(11);
                reportSheet.Cells[row, 37, row + 1, 37].SetBorder(ExcelBorderStyle.Medium);

                row += 2;
                string jamText = "JAM: Write in the jam number jam by jam, starting from 1. If there is a star pass, move to the next line, write 'SP' in the Jam # column, put in the new Jammer's number and pick up the scoring where the previous jammer left off.";
                reportSheet.Cells[row, 1, row, 18].Merge().SetValue(jamText).SetCenterAlign();
                reportSheet.Cells[row, 20, row, 37].Merge().SetValue(jamText).SetCenterAlign();
                row += 1;
                string trackingText = "Tracking:  ALL of the Lead and Call categories should be marked with an X.";
                reportSheet.Cells[row, 1, row, 18].Merge().SetValue(trackingText).SetCenterAlign();
                reportSheet.Cells[row, 20, row, 37].Merge().SetValue(trackingText).SetCenterAlign();
                row += 1;
                trackingText = " Lost = When a jammer loses the ability to become lead jammer or loses lead jammer status itself. Do not check this box if the jammer is eligible but the opposing jammer is assigned lead jammer status first.";
                reportSheet.Cells[row, 1, row, 18].Merge().SetValue(trackingText).SetCenterAlign();
                reportSheet.Cells[row, 20, row, 37].Merge().SetValue(trackingText).SetCenterAlign();
                row += 1;
                trackingText = "Lead = Lead Jammer.     Call = Called Jam, when the listed jammer successfully calls off the jam before jam time runs out.  This is marked whether or not the jam was called legally.";
                reportSheet.Cells[row, 1, row, 18].Merge().SetValue(trackingText).SetCenterAlign();
                reportSheet.Cells[row, 20, row, 37].Merge().SetValue(trackingText).SetCenterAlign();
                row += 1;
                trackingText = "INJ = Called For Injury before the natural end of the jam.           NP = First pass is not completed by the end of the jam (No Pass). ";
                reportSheet.Cells[row, 1, row, 18].Merge().SetValue(trackingText).SetCenterAlign();
                reportSheet.Cells[row, 20, row, 37].Merge().SetValue(trackingText).SetCenterAlign();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }
        /// <summary>
        /// the header of the scores sheet.
        /// </summary>
        /// <param name="reportSheet"></param>
        /// <param name="row"></param>
        /// <param name="doubleColumn"></param>
        private static void ScoresRowHeaderOfSheet(ExcelWorksheet reportSheet, int row, int doubleColumn)
        {
            try
            {
                reportSheet.Cells[row, 1 + doubleColumn].SetValue("JAM").SetTitleRowScoresPage();
                reportSheet.Cells[row, 2 + doubleColumn].SetValue("Jammer's Number").SetTitleRowScoresPage();
                reportSheet.Cells[row, 3 + doubleColumn].SetValue("LOST").SetTitleRowFlippedScoresPage();
                reportSheet.Cells[row, 4 + doubleColumn].SetValue("LEAD").SetTitleRowFlippedScoresPage();
                reportSheet.Cells[row, 5 + doubleColumn].SetValue("CALL").SetTitleRowFlippedScoresPage();
                reportSheet.Cells[row, 6 + doubleColumn].SetValue("INJ.").SetTitleRowFlippedScoresPage();
                reportSheet.Cells[row, 7 + doubleColumn].SetValue("NP").SetTitleRowFlippedScoresPage();
                reportSheet.Cells[row, 8 + doubleColumn].SetValue("Pass 2").SetTitleRowScoresPage();
                reportSheet.Cells[row, 9 + doubleColumn].SetValue("Pass 3").SetTitleRowScoresPage();
                reportSheet.Cells[row, 10 + doubleColumn].SetValue("Pass 4").SetTitleRowScoresPage();
                reportSheet.Cells[row, 11 + doubleColumn].SetValue("Pass 5").SetTitleRowScoresPage();
                reportSheet.Cells[row, 12 + doubleColumn].SetValue("Pass 6").SetTitleRowScoresPage();
                reportSheet.Cells[row, 13 + doubleColumn].SetValue("Pass 7").SetTitleRowScoresPage();
                reportSheet.Cells[row, 14 + doubleColumn].SetValue("Pass 8").SetTitleRowScoresPage();
                reportSheet.Cells[row, 15 + doubleColumn].SetValue("Pass 9").SetTitleRowScoresPage();
                reportSheet.Cells[row, 16 + doubleColumn].SetValue("Pass 10").SetTitleRowScoresPage();
                reportSheet.Cells[row, 17 + doubleColumn].SetValue("Jam Total").SetTitleRowScoresPage();
                reportSheet.Cells[row, 18 + doubleColumn].SetValue("Game Total").SetTitleRowScoresPage();
                reportSheet.Cells[row, 19 + doubleColumn].SetValue("Passes").SetTitleRowScoresPage().SetBackgroundColor(Color.Gray);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }
        /// <summary>
        /// sets the row for quarter of the sheet.  So we use an array to answer each quarter.
        /// </summary>
        /// <param name="green1"></param>
        /// <param name="green2"></param>
        /// <param name="reportSheet"></param>
        /// <param name="row"></param>
        /// <param name="quarterColumnStart"></param>
        private static void ScoresRowQuarterOfSheet(ref Color green1, ref Color green2, ExcelWorksheet reportSheet, int row, int quarterColumnStart, int modulatorRow)
        {
            try
            {
                Color firstColor = Color.White;
                Color secondColor = green1;
                if (modulatorRow % 2 == 1)
                {
                    firstColor = green1;
                    secondColor = green2;
                }
                //jam
                reportSheet.Cells[row, 1 + quarterColumnStart, row + 1, 1 + quarterColumnStart].Merge().SetBackgroundColor(secondColor).SetBorder(ExcelBorderStyle.Thin);
                //jammersColor
                reportSheet.Cells[row, 2 + quarterColumnStart, row + 1, 2 + quarterColumnStart].Merge().SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(firstColor);
                reportSheet.Cells[row, 3 + quarterColumnStart, row + 1, 3 + quarterColumnStart].Merge().SetBackgroundColor(secondColor).SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 4 + quarterColumnStart, row + 1, 4 + quarterColumnStart].Merge().SetBackgroundColor(secondColor).SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 5 + quarterColumnStart, row + 1, 5 + quarterColumnStart].Merge().SetBackgroundColor(secondColor).SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 6 + quarterColumnStart, row + 1, 6 + quarterColumnStart].Merge().SetBackgroundColor(secondColor).SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 7 + quarterColumnStart, row + 1, 7 + quarterColumnStart].Merge().SetBackgroundColor(secondColor).SetBorder(ExcelBorderStyle.Thin);
                for (int j = 8; j < 18; j++)
                    reportSheet.Cells[row, j + quarterColumnStart, row + 1, j + quarterColumnStart].Merge().SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(firstColor);
                reportSheet.Cells[row, 17 + quarterColumnStart, row + 1, 17 + quarterColumnStart].Merge().SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(firstColor);
                reportSheet.Cells[row, 18 + quarterColumnStart, row + 1, 18 + quarterColumnStart].Merge().SetBackgroundColor(green2).SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 19 + quarterColumnStart, row + 1, 19 + quarterColumnStart].Merge().SetBackgroundColor(green1).SetBorder(ExcelBorderStyle.Thin);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }
        #endregion

        #region IBRF

        private void ExportIBRF(ExcelPackage p)
        {
            try
            {
                ExcelWorksheet reportSheet = p.Workbook.Worksheets.Add("IBRF");
                reportSheet.Name = "IBRF"; //Setting Sheet's name
                reportSheet.TabColor = _tabPink;
                reportSheet.Cells.Style.Font.Size = 10; //Default font size for whole sheet
                reportSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

                IBRFCreateSectionOne(reportSheet);
                IBRFCreateSectionTwo(reportSheet);
                IBRFCreateSectionThree(reportSheet);
                IBRFCreateSectionFourNonSkatingOfficials(reportSheet);
                reportSheet.Cells["A1:M80"].AutoFitColumns();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }

        private static int IBRFCreateSectionFourNonSkatingOfficials(ExcelWorksheet reportSheet)
        {
            try
            {
                int row = 54;
                reportSheet.Cells[row, 1, row, 12].SetValue("LIST OF NON-SKATING OFFICIALS/STAT TRACKERS").SetTitleFont().SetBorder(ExcelBorderStyle.Thin);

                row += 1; //55
                reportSheet.Cells[row, 1, row, 3].SetValue("Official/Tracker's Name").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 4, row, 7].SetValue("Non-Skating Official Position").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 8, row, 9].SetValue("League").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 10, row, 12].SetValue("Certification").SetHeader1().SetBorder(ExcelBorderStyle.Thin);

                row += 1; //56
                for (int i = row; i < row + 21; i++)
                {
                    reportSheet.Cells[i, 1, i, 3].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                    reportSheet.Cells[i, 4, i, 7].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                    reportSheet.Cells[i, 8, i, 9].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                    reportSheet.Cells[i, 10, i, 12].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                }

                row = 76;

                row += 1; //77
                reportSheet.Cells[row, 1, row, 12].SetValue(ScannedMessage).SetWhiteSpace().SetCenterAlign();

                row += 1; //78
                reportSheet.Cells[row, 1, row, 12].SetValue(RevMessage).SetWhiteSpace().SetCenterAlign();

                return row;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
            return 0;
        }

        private static int IBRFCreateSectionThree(ExcelWorksheet reportSheet)
        {
            try
            {
                int row = 43;
                reportSheet.Cells[row, 1, row, 12].SetValue("Section 3. CERTIFICATION (Complete IMMEDIATELY AFTER Bout)").SetTitleFont().SetBorder(ExcelBorderStyle.Thin);

                row += 1; //44
                reportSheet.Cells[row, 1, row, 5].SetValue("Home Team Captain").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 6, row, 12].SetValue("Visiting Team Captain").SetHeader1().SetBorder(ExcelBorderStyle.Thin);

                row += 1; //45
                reportSheet.Cells[row, 1].SetValue("Skate Name:").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 2, row, 5].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 6, row, 7].SetValue("Skate Name:").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 8, row, 12].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);

                row += 1; //46
                reportSheet.Cells[row, 1].SetValue("Legal Name:").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 2, row, 5].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 6, row, 7].SetValue("Legal Name:").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 8, row, 12].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);

                row += 1; //47
                reportSheet.Cells[row, 1].SetValue("Signature:").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 2, row, 5].SetLavenderSpace().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 6, row, 7].SetValue("Signature:").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 8, row, 12].SetLavenderSpace().SetBorder(ExcelBorderStyle.Thin);

                row += 1; //48
                reportSheet.Cells[row, 1, row, 5].SetValue("Head Referee").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 6, row, 12].SetValue("Head NSO/Scorekeeper").SetHeader1().SetBorder(ExcelBorderStyle.Thin);

                row += 1; //49
                reportSheet.Cells[row, 1].SetValue("Skate Name:").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 2, row, 5].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 6, row, 7].SetValue("Skate Name:").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 8, row, 12].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);

                row += 1; //50
                reportSheet.Cells[row, 1].SetValue("Legal Name:").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 2, row, 5].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 6, row, 7].SetValue("Legal Name:").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 8, row, 12].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);

                row += 1; //51
                reportSheet.Cells[row, 1].SetValue("Signature:").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 2, row, 5].SetLavenderSpace().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 6, row, 7].SetValue("Signature:").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 8, row, 12].SetLavenderSpace().SetBorder(ExcelBorderStyle.Thin);

                row += 1; //52
                reportSheet.Cells[row, 1, row, 12].SetValue(ScannedMessage).SetWhiteSpace().SetCenterAlign();

                row += 1; //53
                reportSheet.Cells[row, 1, row, 12].SetValue(RevMessage).SetWhiteSpace().SetCenterAlign();

                return row;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
            return 0;
        }

        private static int IBRFCreateSectionTwo(ExcelWorksheet reportSheet)
        {
            try
            {
                int row = 36;
                reportSheet.Cells[row, 1, row, 12].SetValue("Section 2. SCORE (Complete DURING or IMMEDIATELY AFTER bout)").SetTitleFont().SetBorder(ExcelBorderStyle.Thin);

                row += 1;
                reportSheet.Cells[row, 1, row, 6].SetValue("HOME TEAM").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 7, row, 12].SetValue("VISITING TEAM").SetHeader1().SetBorder(ExcelBorderStyle.Thin);

                row += 1;//38
                reportSheet.Cells[row, 1].SetValue("Period 1").SetHeader2().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 2].SetValue("Points").SetHeader3().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 3].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 4].SetValue("Penalties").SetHeader3().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 5, row, 6].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 7].SetValue("Period 1").SetHeader2().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 8].SetValue("Points").SetHeader3().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 9].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 10].SetValue("Penalties").SetHeader3().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 11, row, 12].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);

                row += 1;//39
                reportSheet.Cells[row, 1].SetValue("Period 2").SetHeader2().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 2].SetValue("Points").SetHeader3().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 3].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 4].SetValue("Penalties").SetHeader3().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 5, row, 6].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 7].SetValue("Period 2").SetHeader2().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 8].SetValue("Points").SetHeader3().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 9].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 10].SetValue("Penalties").SetHeader3().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 11, row, 12].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);

                row += 1; //40  
                reportSheet.Cells[row, 1, row, 2].SetValue("TOTAL POINTS:").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 3].SetLavenderSpace().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 4].SetValue("PENALTIES:").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 5, row, 6].SetLavenderSpace().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 7, row, 8].SetValue("TOTAL POINTS:").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 9].SetLavenderSpace().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 10].SetValue("PENALTIES:").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 11, row, 12].SetLavenderSpace().SetBorder(ExcelBorderStyle.Thin);

                row += 1;//41
                reportSheet.Cells[row, 1, row, 3].SetValue("Expulsion/suspension notes:").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 4, row, 12].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);

                row += 1;//42
                reportSheet.Cells[row, 1, row, 12].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                return row;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
            return 0;
        }
        /// <summary>
        /// creates section one of the WFTDA IBRF page.
        /// </summary>
        /// <param name="reportSheet"></param>
        private static int IBRFCreateSectionOne(ExcelWorksheet reportSheet)
        {
            try
            {
                int row = 1;
                reportSheet.Cells[row, 1, row, 12].Value = "WFTDA Interleague Bout Reporting Form (IBRF)";
                reportSheet.Cells[row, 1, row, 12].SetPageTitle().SetBorder(ExcelBorderStyle.Thin);

                reportSheet.Cells[2, 1, 2, 12].Value = "Section 1. VENUE & ROSTERS";
                reportSheet.Cells[2, 1, 2, 12].SetTitleFont().SetBorder(ExcelBorderStyle.Thin);

                //location
                reportSheet.Cells[3, 1, 4, 1].Value = "Location:";
                reportSheet.Cells[3, 1, 4, 1].SetHeader1().SetBorder(ExcelBorderStyle.Thin);

                reportSheet.Cells[4, 2, 4, 7].Value = "VENUE NAME";
                reportSheet.Cells[4, 2, 4, 7].SetHeader2().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[3, 2, 3, 7].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);

                reportSheet.Cells[4, 8, 4, 9].Value = "CITY";
                reportSheet.Cells[4, 8, 4, 9].SetHeader2().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[3, 8, 3, 9].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);

                reportSheet.Cells[4, 10].Value = "ST/PRV";
                reportSheet.Cells[4, 10].SetHeader2().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[3, 10].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);

                reportSheet.Cells[4, 11, 4, 12].Value = "BOUT A/B";
                reportSheet.Cells[4, 11, 4, 12].SetHeader2().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[3, 11, 3, 12].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);

                reportSheet.Cells[5, 1].Value = "Date:";
                reportSheet.Cells[5, 1].SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[5, 2, 5, 5].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);

                reportSheet.Cells[5, 6, 5, 7].Value = "Start Time:";
                reportSheet.Cells[5, 6, 5, 7].SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[5, 8, 5, 9].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);

                reportSheet.Cells[5, 10].Value = "End Time:";
                reportSheet.Cells[5, 10].SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[5, 11, 5, 12].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);

                reportSheet.Cells[6, 1, 6, 12].Value = "TEAM ROSTERS - List in order of skater number (numeric portion)";
                reportSheet.Cells[6, 1, 6, 12].SetTitleBreak().SetBorder(ExcelBorderStyle.Thin);

                reportSheet.Cells[7, 1, 7, 5].Value = "H O M E  T E A M";
                reportSheet.Cells[7, 1, 7, 5].SetHeader1().SetBorder(ExcelBorderStyle.Thin);

                reportSheet.Cells[7, 6, 7, 12].Value = "V I S I T I N G  T E A M";
                reportSheet.Cells[7, 6, 7, 12].SetHeader1().SetBorder(ExcelBorderStyle.Thin);

                reportSheet.Cells[8, 1].Value = "LEAGUE";
                reportSheet.Cells[8, 1].SetHeader2().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[8, 2, 8, 5].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[9, 1].Value = "TEAM";
                reportSheet.Cells[9, 1].SetHeader2().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[9, 2, 9, 5].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);

                reportSheet.Cells[8, 6, 8, 7].Value = "LEAGUE";
                reportSheet.Cells[8, 6, 8, 7].SetHeader2().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[8, 8, 8, 12].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[9, 6, 9, 7].Value = "TEAM";
                reportSheet.Cells[9, 6, 9, 7].SetHeader2().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[9, 8, 9, 12].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);

                reportSheet.Cells[10, 1].Value = "# of players";
                reportSheet.Cells[10, 1].SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[10, 2].Value = "Skater #";
                reportSheet.Cells[10, 2].SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[10, 3, 10, 5].Value = "Skater Name";
                reportSheet.Cells[10, 3, 10, 5].SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[10, 6, 10, 7].Value = "# of players";
                reportSheet.Cells[10, 6, 10, 7].SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[10, 8].Value = "Skater #";
                reportSheet.Cells[10, 8].SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[10, 9, 10, 12].Value = "Skater Name";
                reportSheet.Cells[10, 9, 10, 12].SetHeader1().SetBorder(ExcelBorderStyle.Thin);

                for (int i = 1; i < 21; i++)
                {
                    reportSheet.Cells[i + 10, 1].Value = i;
                    reportSheet.Cells[i + 10, 1].SetHeader3().SetBorder(ExcelBorderStyle.Thin);
                    reportSheet.Cells[i + 10, 2].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                    reportSheet.Cells[i + 10, 3, i + 10, 5].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);

                    reportSheet.Cells[i + 10, 6, i + 10, 7].Value = i;
                    reportSheet.Cells[i + 10, 6, i + 10, 7].SetHeader3().SetBorder(ExcelBorderStyle.Thin);
                    reportSheet.Cells[i + 10, 8].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                    reportSheet.Cells[i + 10, 9, i + 10, 12].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                }

                row = 31;
                reportSheet.Cells[31, 1, 31, 2].SetValue("Referee Name").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[31, 3].SetValue("Position").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[31, 4].SetValue("League").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[31, 5].SetValue("Cert.").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[31, 6, 31, 8].SetValue("Referee Name").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[31, 9].SetValue("Position").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[31, 10, 31, 11].SetValue("League").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[31, 12].SetValue("Cert.").SetHeader1().SetBorder(ExcelBorderStyle.Thin);
                reportSheet.Cells[row, 1, row, 12].SetBorder(ExcelBorderStyle.Thick);

                row = 32;
                for (int i = row; i < row + 4; i++)
                {
                    reportSheet.Cells[i, 1, i, 2].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                    reportSheet.Cells[i, 6, i, 8].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                    reportSheet.Cells[i, 3].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                    reportSheet.Cells[i, 4].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                    reportSheet.Cells[i, 5].SetLavenderSpace().SetBorder(ExcelBorderStyle.Thin);
                    reportSheet.Cells[i, 9].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                    reportSheet.Cells[i, 10, i, 11].SetWhiteSpace().SetBorder(ExcelBorderStyle.Thin);
                    reportSheet.Cells[i, 12].SetLavenderSpace().SetBorder(ExcelBorderStyle.Thin);
                }
                row += 4;
                return row;

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
            return 0;
        }

        #endregion


    }
}
