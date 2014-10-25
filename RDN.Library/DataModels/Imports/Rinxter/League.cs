
using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Imports.Rinxter
{
    [Table("RDN_Rinxter_Leagues")]
    public class RLeague : InheritDb
    {
        
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LeagueId { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Abbr { get; set; }
        public string Url { get; set; }
        public string Location { get; set; }
        public System.DateTime? JoinDate { get; set; }
        public string Image { get; set; }
        public int RegionID { get; set; }

        public bool IsDeleted { get; set; }
        [Required]
        public bool IsFemale { get; set; }

       // public long LeaguePhotoId { get; set; }

       // public virtual RLeaguePhoto LeaguePhoto { get; set; }
        public virtual RRegions Regions { get; set; }
        public virtual ICollection<RTeams> Teams { get; set; }

        public RLeague()
        {
            this.Teams = new List<RTeams>();
            IsFemale = true;
            IsDeleted = false;
        }
    }
}
