using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.League.OrganizationChart
{
    [Table("RDN_League_Organization_Designation")]
    public class Designation : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long DesignationId { get; set; }
        public string DesignationTitle { get; set; }
        public string DesignationLevel { get; set; }//Role President Or the most top position has 0.
        public string ShortNote { get; set; }

        #region References
        public virtual RDN.Library.DataModels.League.League League { get; set; }
        public virtual ICollection<Organize> Organization { get; set; }
        #endregion
    }
}
