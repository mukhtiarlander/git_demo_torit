using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.League
{
    [Table("RDN_League_Photos")]
    public class LeaguePhoto : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LeaguePhotoId { get; set; }
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

        #region Methods

        public LeaguePhoto()
        {
            Width = 0;
            Height = 0;
            IsVisibleToPublic = true;
        }

        #endregion

        #region References

        [Required]
        public virtual League League { get; set; }

        #endregion
    }
}
