using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game
{
    /// <summary>
    /// saves the advertisements for the game.
    /// </summary>
    [Table("RDN_Game_Advertisements")]
    public class GameAdvertisement : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid GameAdvertisementId { get; set; }
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Url { get; set; }
        [Required]
        [MaxLength(1000)]
        public string SavePath { get; set; }
        [MaxLength(1000)]
        public string UrlThumb { get; set; }
        [MaxLength(1000)]
        public string SavePathThumb { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        [Required]
        public Guid Format { get; set; }

        #region References

        [Required]
        public virtual Game Game { get; set; }

        #endregion

    }
}
