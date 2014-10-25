using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.RN.Funds.Monthly
{
   public  class Statement
    {
        public long StatementId { get; set; }

        public DateTime StatementDateTime { get; set; }
        public long TotalPageViews { get; set; }
        public double TotalProfitMade { get; set; }
        public double TotalWritersPayoutProfit { get; set; }
        public long TotalWritersPaid { get; set; }
    }
}
