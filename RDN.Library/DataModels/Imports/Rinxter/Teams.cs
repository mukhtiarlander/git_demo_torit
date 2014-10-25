using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Imports.Rinxter
{
    [Table("RDN_Rinxter_Teams")]
    public class RTeams : InheritDb
    {
        public RTeams()
        {
            this.Bouts = new List<RBouts>();
            this.Bouts1 = new List<RBouts>();
            this.SkaterTeam = new List<RSkaterTeam>();
            this.TeamTournaments = new List<RTeamTournaments>();
            IsFemale = true;
            IsDeleted = false;
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TeamId { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Abbr { get; set; }
        public string Image { get; set; }
        public string Color { get; set; }
        public string Letter { get; set; }
        public string Type { get; set; }
        public int PrimaryLeagueID { get; set; }
        public int TournamentsID { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public bool IsFemale { get; set; }

        public virtual ICollection<RBouts> Bouts { get; set; }
        public virtual ICollection<RBouts> Bouts1 { get; set; }
        public virtual RLeague League { get; set; }
        public virtual ICollection<RSkaterTeam> SkaterTeam { get; set; }
        public virtual ICollection<RTeamTournaments> TeamTournaments { get; set; }
    }
}
