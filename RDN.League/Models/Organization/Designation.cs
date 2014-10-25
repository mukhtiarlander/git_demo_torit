using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDN.League.Models.Organization
{
    //RDN_League_Organization_Dsignation
    public class Designation
    {
        public long DesignationId { get; set; }
        public string DesigTitle { get; set; }
        public string DesigLavel { get; set; }//Role President Or the most top position has 0.
        public string ShortNote { get; set; }

        #region References
        public virtual RDN.Library.DataModels.League.League League { get; set; }
        public virtual ICollection<Organize> Organization { get; set; }
        #endregion
    }
}