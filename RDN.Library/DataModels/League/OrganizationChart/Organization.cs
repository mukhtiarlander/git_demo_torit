using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.League.OrganizationChart
{
     [Table("RDN_League_Organization")]
    public class Organization : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long OrganizationId { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public string Website { get; set; }
        public string Note { get; set; }
        
        #region References
        
        public virtual ICollection<Organize> Organize { get; set; }
        public virtual RDN.Library.DataModels.League.League League { get; set; }
        #endregion
    }
}
