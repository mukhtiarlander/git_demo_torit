using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Imports.Rinxter
{
    [Table("RDN_Rinxter_Skaters")]
    public  class RSkaters : InheritDb
    {
        public RSkaters()
        {
            this.SkaterTeam = new List<RSkaterTeam>();
            IsFemale = true;
            IsDeleted = false;
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SkaterId { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Image { get; set; }
        public int TeamID { get; set; }
        public int PositionID { get; set; }
        public bool IsDeleted { get; set; }

        [Required]
        public bool IsFemale { get; set; }

        public virtual RPosition Position { get; set; }
        public virtual ICollection<RSkaterTeam> SkaterTeam { get; set; }
    }
}
