using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.League.OrganizationChart
{
    [Table("RDN_League_Organization_Organize")]
    public class Organize : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long OrganizeId { get; set; }
        //public string ReportsToRoleId { get; set; } 
        public string Comment { get; set; }
        public string ReportsToDesignation { get; set; } //A member works under which designation . Designation Level/Role Id
        public string OrganizedBy { get; set; }

        #region References
        public virtual RDN.Library.DataModels.Member.Member ManagerId { get; set; }
        public virtual RDN.Library.DataModels.Member.Member StaffId { get; set; }
        public virtual Designation Designation { get; set; } //Member Designation    
        public virtual RDN.Library.DataModels.League.League League { get; set; }
        public virtual Organization Organization { get; set; }

        #endregion
    }
}
