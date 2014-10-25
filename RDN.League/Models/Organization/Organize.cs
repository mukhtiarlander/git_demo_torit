using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDN.League.Models.Organization
{
    //Table:: RDN_League_Organization_Organize
    public class Organize
    {
        public long OrganizeId { get; set; }
        public string ReportsToRoleId { get; set; } //Designation Level/Role Id
        public string Comment { get; set; }

        #region References
        public virtual RDN.Library.DataModels.Member.Member StaffId { get; set; }
        public virtual RDN.Library.DataModels.Member.Member OrganizedBy { get; set; }
        public virtual Designation Designation { get; set; }
        #endregion
    }
}