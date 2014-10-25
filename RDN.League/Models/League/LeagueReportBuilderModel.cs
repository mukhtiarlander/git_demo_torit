using RDN.Library.Classes.League.Reports;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace RDN.League.Models.League
{
    public class LeagueReportBuilderModel
    {
        public LeagueReportBuilderModel() { }
        public string LeagueName { get; set; }
        public Guid LeagueId { get; set; }
        public string SavedReportName { get; set; }
        public string SelectedColumnsHidden { get; set; }
        public bool SaveReport { get; set; }
        public string SelectedReport { get; set; }
        public SelectList SavedReports { get; set; }
        public List<MembersReportEnum> ColumnsAvailable { get; set; }
        public List<MembersReportEnum> ColumnsSelected { get; set; }
    }
}