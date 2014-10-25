using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Member
{
    [Table("RDN_Member_Photos")]
    public class MemberPhoto : InheritDb
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

        public string AlternativeText { get; set; }
        #region Methods

        public MemberPhoto()
        {
            Width = 0;
            Height = 0;
            IsVisibleToPublic = true;
        }

        #endregion

        #region References

        [Required]
        public virtual Member Member { get; set; }

        #endregion
    }
}
