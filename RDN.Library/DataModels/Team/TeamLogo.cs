using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Team
{
    /// <summary>
    /// logos uploaded by teams.
    /// </summary>
        [Table("RDN_Team_Logos")]
    public class TeamLogo : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TeamLogoId { get; set; }

        public string TeamName { get; set; }

        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public string SaveLocation { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string ImageUrlThumb { get; set; }

        public string SaveLocationThumb { get; set; }

        #region references

        public TeamLogo()
        {
            Width = 0;
            Height = 0;
        }
   

        #endregion
    }
}
