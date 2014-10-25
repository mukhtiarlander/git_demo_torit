using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.League
{
    [Table("RDN_League_Jobs")]
    public class JobBoard : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long JobId { get; set; }
        public string JobTitle { get; set; }
        public string HoursPerWeek { get; set; }
        public string ReportsTo { get; set; }
        public DateTime JobEnds { get; set; }//Job Apply Dead Line
        public string JobDesc { get; set; }
        public bool IsClosed { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsFilled { get; set; }

        #region References
        public virtual RDN.Library.DataModels.Member.Member JobCreator { get; set; }
        public virtual RDN.Library.DataModels.League.League League { get; set; }
        #endregion
    }
}
