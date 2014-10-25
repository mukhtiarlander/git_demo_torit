using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Scoreboard
{
    [Table("RDN_Scoreboards_Instance")]
    public class ScoreboardInstance : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long InstanceId { get; set; }
        [Required]
        [MaxLength(30)]
        public string InstanceMacAddress { get; set; }
        [Required]
        public DateTime LastUpdated { get; set; }

        public string Version { get; set; }

        public long LoadsCount { get; set; }
    }
}
