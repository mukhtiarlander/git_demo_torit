using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Imports.Rinxter
{
    [Table("RDN_Rinxter_Tournament_Logos")]
    public class RTournamentPhoto : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TournamentPhotoId { get; set; }
        public bool IsVisibleToPublic { get; set; }
        public bool IsPrimaryPhoto { get; set; }
        public int Width { get; set; }
        //public byte LeaguePhotoTypeEnum { get; set; }
        public int Height { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public string SaveLocation { get; set; }

        public string ImageUrlThumb { get; set; }

        public string SaveLocationThumb { get; set; }

        public string AlternativeText { get; set; }

        public long TournamentId { get; set; }

        #region Methods

        public RTournamentPhoto()
        {
            Width = 0;
            Height = 0;
            IsVisibleToPublic = true;

        }

        #endregion

    }
}
