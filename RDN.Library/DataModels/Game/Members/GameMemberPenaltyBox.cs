using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game.Members
{
    /// <summary>
    /// all penalties for the box
    /// </summary>
    [Table("RDN_Game_Members_Penalty_Box")]
    public class GameMemberPenaltyBox : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PenaltyId { get; set; }

        [Required]
        [MaxLength(20)]
        public string PenaltyType { get; set; }
        [Required]
        public int PenaltyScale { get; set; }
        [Required]
        public long GameTimeMilliSecondsSent { get; set; }
        [Required]
        public long JamTimeMilliSecondsSent { get; set; }
        [Required]
        public int JamNumberSent { get; set; }
        public Guid JamIdSent { get; set; }
        [Required]
        public Guid PenaltyIdFromGame { get; set; }
        /// <summary>
        /// the number of penalties received by the skater.
        /// </summary>
        public int PenaltyNumberForSkater { get; set; }
        public long GameTimeMilliSecondsReturned { get; set; }
        public long JamTimeMilliSecondsReturned { get; set; }
        public int JamNumberReturned { get; set; }
        public Guid JamIdReturned { get; set; }


        #region References

        [Required]
        public virtual GameMember Member { get; set; }

        public virtual Game Game { get; set; }
        #endregion

    }
}
