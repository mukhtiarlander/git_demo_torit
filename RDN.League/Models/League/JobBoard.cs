using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDN.League.Models.League
{
    public class JobBoard
    {
        public long JobId { get; set; }
        public string JobTitle { get; set; }
        public double HoursPerWeek { get; set; } 
        public string ReportsTo { get; set; }
        public DateTime JobEnds { get; set; }//Job Apply Dead Line
        public string JobDesc { get; set; }

        #region References
        public virtual RDN.Library.DataModels.Member.Member JobCreator { get; set; }
        public virtual RDN.Library.DataModels.League.League League { get; set; }
        #endregion
    }
}