using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.League.Report
{
    [Table("RDN_League_Reports")]
    public class LeagueReport : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ReportId { get; set; }

        public string LeagueReportItems { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsRemoved { get; set; }
        public virtual League League { get; set; }

        public LeagueReport()
        {

        }
    }
}
