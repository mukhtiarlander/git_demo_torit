using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Imports.Rinxter
{
    [Table("RDN_Rinxter_Seasons")]
    public class RSeasons : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SeasonIds { get; set; }
        public int seasonID { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
    }
}
