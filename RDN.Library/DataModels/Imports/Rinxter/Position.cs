using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Imports.Rinxter
{
    [Table("RDN_Rinxter_Positions")]
    public class RPosition : InheritDb
    {
        public RPosition()
        {
            this.Skaters = new List<RSkaters>();
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PositionIds { get; set; }
        public int PositionID { get; set; }
        public string Name { get; set; }

        public virtual ICollection<RSkaters> Skaters { get; set; }
    }
}
