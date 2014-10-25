using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Team
{
    /// <summary>
    /// Teams are used for the scoreboard system to save logos of the teams
    /// </summary>
    [Table("RDN_Teams")]
    public class Team : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TeamId { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string LoweredName { get; set; }
        [Column(TypeName = "text")]
        public string Description { get; set; }

        #region References

        public virtual ICollection<TeamLogo> Logos { get; set; }
        public virtual ICollection<TeamMember> Members { get; set; }
        [Required]
        public virtual League.League League { get; set; }

        #endregion

        #region Constructor

        public Team()
        {
            Members = new Collection<TeamMember>();
            Logos = new Collection<TeamLogo>();
        }

        #endregion
    }
}
