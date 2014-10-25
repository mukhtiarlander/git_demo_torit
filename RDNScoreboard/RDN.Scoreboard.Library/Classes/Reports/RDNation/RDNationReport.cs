using OfficeOpenXml;
using RDN.Utilities.Config;
using RDN.Utilities.Util;
using Scoreboard.Library.Static.Enums;
using Scoreboard.Library.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoreboard.Library.Classes.Reports.RDNation
{
    public class RDNationReport
    {

        public string fileName;
        public string openFileName;

        public RDNationReport Initialize()
        {
            return this;
        }

        public RDNationReport SetSaveFileName(string fileName)
        {
            this.fileName = fileName;
            return this;
        }
        public RDNationReport SetOpenFileName(string fileName)
        {
            this.openFileName = fileName;
            return this;
        }
        public void Export()
        {
            var saveGameTask = Task<bool>.Factory.StartNew(
                  () =>
                  {
                      try
                      {
                          ExportRDNStatsReport();

                      }
                      catch (Exception exception)
                      {
                          Logger.Instance.logMessage("tried to save the game", LoggerEnum.message);
                          ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                      }
                      return true;
                  });
        }

        private void ExportRDNStatsReport()
        {

            FileInfo newFile = new FileInfo(openFileName);

            using (ExcelPackage p = new ExcelPackage(newFile))
            {
                try
                {
                    if (GameViewModel.Instance != null)
                    {
                        ExportJams(p);
                    }
                }
                catch (Exception exception)
                {
                    ErrorViewModel.Save(exception, exception.GetType());
                }
                FileInfo fi = new FileInfo(this.fileName);
                p.SaveAs(fi);
                System.Diagnostics.Process.Start(this.fileName);
            }
        }
        private void ExportJams(ExcelPackage p)
        {
            try
            {
                var reportSheet = p.Workbook.Worksheets.Where(x => x.Name == "Jams").FirstOrDefault();

                int row = 1;
                if (GameViewModel.Instance.Team1 != null)
                    reportSheet.Cells[row, 2].Value = GameViewModel.Instance.Team1.TeamName;
                if (GameViewModel.Instance.Team2 != null)
                    reportSheet.Cells[row, 13].Value = GameViewModel.Instance.Team2.TeamName;

                row = 4;
                var periodOneJams = GameViewModel.Instance.Jams.Where(x => x.CurrentPeriod == 1).OrderBy(x => x.JamNumber).ToList();

                for (int i = 0; i < periodOneJams.Count; i++)
                {
                    if (GameViewModel.Instance.CurrentJam == null)
                    {
                        PopulateLineUpJam(reportSheet, row, periodOneJams[i]);
                        row += 1;
                    }
                    else if (periodOneJams[i].JamNumber != GameViewModel.Instance.CurrentJam.JamNumber)
                    {
                        PopulateLineUpJam(reportSheet, row, periodOneJams[i]);
                        row += 1;
                    }
                }
                row = 41;
                var periodTwoJams = GameViewModel.Instance.Jams.Where(x => x.CurrentPeriod == 2).OrderBy(x => x.JamNumber).ToList();
                for (int i = 0; i < periodTwoJams.Count; i++)
                {
                    if (GameViewModel.Instance.CurrentJam == null)
                    {
                        PopulateLineUpJam(reportSheet, row, periodTwoJams[i]);
                        row += 1;
                    }
                    else if (periodTwoJams[i].JamNumber != GameViewModel.Instance.CurrentJam.JamNumber)
                    {
                        PopulateLineUpJam(reportSheet, row, periodTwoJams[i]);
                        row += 1;
                    }

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
                reportSheet.Cells[row, 1].Value = jam.JamNumber;
                if (jam.PivotT1 == null)
                {
                    reportSheet.Cells[row, 2].Value = "X";
                    if (jam.Blocker4T1 != null)
                        reportSheet.Cells[row, 3].Value = jam.Blocker4T1.SkaterNumber;
                }
                else
                    reportSheet.Cells[row, 3].Value = jam.PivotT1.SkaterNumber;
                if (jam.Blocker1T1 != null)
                    reportSheet.Cells[row, 4].Value = jam.Blocker1T1.SkaterNumber;
                if (jam.Blocker2T1 != null)
                    reportSheet.Cells[row, 5].Value = jam.Blocker2T1.SkaterNumber;
                if (jam.Blocker3T1 != null)
                    reportSheet.Cells[row, 6].Value = jam.Blocker3T1.SkaterNumber;
                if (jam.JammerT1 != null)
                    reportSheet.Cells[row, 7].Value = jam.JammerT1.SkaterNumber;

                reportSheet.Cells[row, 8].Value = jam.TotalPointsForJamT1;
                reportSheet.Cells[row, 10].Value = jam.TotalPointsForJamT1 - jam.TotalPointsForJamT2;

                //team2
                reportSheet.Cells[row, 12].Value = jam.JamNumber;
                if (jam.PivotT2 == null)
                {
                    reportSheet.Cells[row, 13].Value = "X";
                    if (jam.Blocker4T2 != null)
                        reportSheet.Cells[row, 14].Value = jam.Blocker4T2.SkaterNumber;
                }
                else
                    reportSheet.Cells[row, 14].Value = jam.PivotT2.SkaterNumber;

                if (jam.Blocker1T2 != null)
                    reportSheet.Cells[row, 15].Value = jam.Blocker1T2.SkaterNumber;
                if (jam.Blocker2T2 != null)
                    reportSheet.Cells[row, 16].Value = jam.Blocker2T2.SkaterNumber;
                if (jam.Blocker3T2 != null)
                    reportSheet.Cells[row, 17].Value = jam.Blocker3T2.SkaterNumber;
                if (jam.JammerT2 != null)
                    reportSheet.Cells[row, 18].Value = jam.JammerT2.SkaterNumber;

                reportSheet.Cells[row, 19].Value = jam.TotalPointsForJamT2;
                reportSheet.Cells[row, 21].Value = jam.TotalPointsForJamT1 - jam.TotalPointsForJamT2;

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }


    }
}
