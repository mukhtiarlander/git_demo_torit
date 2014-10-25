using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game.Officials
{
    [Table("RDN_Game_Official_Photos")]
    public class GameOfficialPhoto : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MemberPhotoId { get; set; }
        public bool IsVisibleToPublic { get; set; }
        public bool IsPrimaryPhoto { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public string SaveLocation { get; set; }
        public string ImageUrlThumb { get; set; }

        public string SaveLocationThumb { get; set; }

        #region Methods

        public GameOfficialPhoto()
        {
            Width = 0;
            Height = 0;
            IsVisibleToPublic = true;
        }

        #endregion

        #region References

        [Required]
        public virtual GameOfficial Official { get; set; }

        #endregion
    }
}
