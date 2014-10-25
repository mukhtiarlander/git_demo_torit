using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RDN.Library.DataModels.Imports.Rinxter
{
    [Table("RDN_Rinxter_SkaterTeams")]
    public class RSkaterTeam : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SkaterTeamIds { get; set; }
        public int SkaterTeamID { get; set; }
        public int TeamID { get; set; }
        public int SkaterID { get; set; }

        public virtual RSkaters Skaters { get; set; }
        public virtual RTeams Teams { get; set; }
    }
}
