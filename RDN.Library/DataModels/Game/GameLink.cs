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
    /// links for the user to add to the games, like youtube, flickr etc...
    /// </summary>
    [Table("RDN_Game_Links")]
    public class GameLink : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LinkId { get; set; }

        public Guid GameLinkId { get; set; }

        [Required]
        public string Link { get; set; }
        [Required]
        public int LinkType { get; set; }

        #region References
        [Required]
        public virtual Game Game { get; set; }
        #endregion

    }
}
