using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Imports.Rinxter
{
 [Table("RDN_Rinxter_Tournaments")]
    public class RTournaments : InheritDb
    {
        public RTournaments()
        {
            this.Bouts = new List<RBouts>();
            this.TeamTournaments = new List<RTeamTournaments>();
            IsFemale = true;
            IsDeleted = false;
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TournamentsId { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Abbr { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Venue { get; set; }
        public string PostCode { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public System.DateTime? StartDate { get; set; }
        public System.DateTime? EndDate { get; set; }
        public int SeasonID { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public bool IsFemale { get; set; }

        public virtual ICollection<RBouts> Bouts { get; set; }
        public virtual ICollection<RTeamTournaments> TeamTournaments { get; set; }
    }
}
