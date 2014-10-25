using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RDN.Library.DataModels.Imports.Rinxter
{
    
       [Table("RDN_Rinxter_Bouts")]
    public class RBouts : InheritDb
    {
           public RBouts()
        {
            this.LineUps = new HashSet<RLineUps>();
            this.Penalties = new HashSet<RPenalties>();
            this.Scores = new HashSet<RScores>();
            IsFemale = true;
            IsDeleted = false;
        }
           [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long BoutId { get; set; }
        public int ID { get; set; }
        public string Venue { get; set; }
        public System.DateTime Date { get; set; }
        public string Season { get; set; }
        public string Status { get; set; }
        public string State { get; set; }
        public int Team1ID { get; set; }
        public int Team2ID { get; set; }
        public string Sanction { get; set; }
        public string SanctionS { get; set; }
        public string Location { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public string Ruleset { get; set; }
        public Nullable<int> TournamentsID { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public bool IsFemale { get; set; }

        public virtual RTeams Teams { get; set; }
        public virtual RTeams Teams1 { get; set; }
        public virtual RTournaments Tournaments { get; set; }
        public virtual ICollection<RLineUps> LineUps { get; set; }
        public virtual ICollection<RPenalties> Penalties { get; set; }
        public virtual ICollection<RScores> Scores { get; set; }
    }
}
