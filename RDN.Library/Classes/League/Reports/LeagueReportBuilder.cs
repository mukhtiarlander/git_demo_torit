using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.League.Reports
{
    public class LeagueReportBuilder
    {
        public string Name { get; set; }
        public string leagueReportEnums { get; set; }
        public long ReportId { get; set; }

        public static bool SaveReport(Guid leagueId, string leagueReportEnums, string name)
        {
            try
            {
                int c = 0;
                var dc = new ManagementContext();
                DataModels.League.Report.LeagueReport report = new DataModels.League.Report.LeagueReport();
                report.Description = "";
                report.League = dc.Leagues.Where(x => x.LeagueId == leagueId).FirstOrDefault();
                report.LeagueReportItems = leagueReportEnums;
                report.Name = name;
                dc.LeagueReports.Add(report);
                c += dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static List<LeagueReportBuilder> GetReports(Guid leagueId)
        {
            List<LeagueReportBuilder> reports = new List<LeagueReportBuilder>();
            try
            {
                int c = 0;
                var dc = new ManagementContext();
                var list = dc.LeagueReports.Where(x => x.League.LeagueId == leagueId && x.IsRemoved == false).ToList();

                foreach (var re in list)
                {
                    reports.Add(DisplayReport(re));
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return reports;
        }
        public static LeagueReportBuilder GetReport(Guid leagueId, long reportId)
        {
            List<LeagueReportBuilder> reports = new List<LeagueReportBuilder>();
            try
            {
                int c = 0;
                var dc = new ManagementContext();
                var list = dc.LeagueReports.Where(x => x.League.LeagueId == leagueId && x.ReportId == reportId).FirstOrDefault();

                return DisplayReport(list);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new LeagueReportBuilder();
        }
        public static bool DeleteReport(Guid leagueId, long reportId)
        {
            try
            {
                var dc = new ManagementContext();
                var list = dc.LeagueReports.Where(x => x.League.LeagueId == leagueId && x.ReportId == reportId).FirstOrDefault();
                list.IsRemoved = true;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        private static LeagueReportBuilder DisplayReport(DataModels.League.Report.LeagueReport re)
        {
            LeagueReportBuilder temp = new LeagueReportBuilder();
            temp.leagueReportEnums = re.LeagueReportItems;
            temp.Name = re.Name;
            temp.ReportId = re.ReportId;
            return temp;
        }
    }
}
