using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Imports.Rinxter
{
    [Table("RDN_Rinxter_Regions")]
    public class RRegions : InheritDb
    {
        public RRegions()
        {
            this.League = new List<RLeague>();
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RegionIds { get; set; }
        public int RegionID { get; set; }
        public string Name { get; set; }

        public virtual ICollection<RLeague> League { get; set; }
    }
}
