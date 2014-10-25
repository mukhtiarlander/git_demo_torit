using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Imports.Rinxter
{
    [Table("RDN_Rinxter_LineUps")]
    public class RLineUps : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LineUpIds { get; set; }
        public int LineUpID { get; set; }
        public int BoutID { get; set; }
        public string TeamName { get; set; }
        public string Jammer { get; set; }
        public string PivotBlocker { get; set; }
        public string Blocker { get; set; }
        public string Blocker1 { get; set; }
        public string Blocker2 { get; set; }
        public string Jam { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsFemale { get; set; }

        public virtual RBouts Bouts { get; set; }

        public RLineUps()
        {
            IsDeleted = false;
        }
    }
}
