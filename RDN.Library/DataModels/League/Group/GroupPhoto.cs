using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.League.Group
{
    [Table("RDN_League_Group_Photo")]
    public class GroupPhoto : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PhotoId { get; set; }
        public bool IsVisibleToPublic { get; set; }
        public bool IsPrimaryPhoto { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public string SaveLocation { get; set; }

        public string AlternativeText { get; set; }

        #region Methods

        public GroupPhoto()
        {
            Width = 0;
            Height = 0;
            IsVisibleToPublic = true;
        }

        #endregion

        #region References

        [Required]
        public virtual Group Group{ get; set; }

        #endregion
    }
}
