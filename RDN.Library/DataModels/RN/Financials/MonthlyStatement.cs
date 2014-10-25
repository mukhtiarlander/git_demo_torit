using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.Library.DataModels.RN.Financials
{
    [Table("RN_Monthly_Statement")]
    public class MonthlyStatement : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long StatementId { get; set; }

        public DateTime StatementDateTime { get; set; }
        public long TotalPageViews { get; set; }
        public double TotalProfitMade { get; set; }
        public double TotalWritersPayoutProfit { get; set; }
        public long TotalWritersPaid { get; set; }

    }
}
