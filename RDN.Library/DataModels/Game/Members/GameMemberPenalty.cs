using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game.Members
{
    /// <summary>
    /// all penalties for the game.
    /// </summary>
    [Table("RDN_Game_Penalty")]
    public class GameMemberPenalty : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PenaltyId { get; set; }
        //id of the score from the game it self generated on the client machine.

        public Guid GamePenaltyId { get; set; }
  
        public DateTime DateTimePenaltied { get; set; }
    
        public int JamNumber { get; set; }
        public Guid JamId { get; set; }

        public int PeriodNumber { get; set; }
        public long PeriodTimeRemainingMilliseconds { get; set; }
        public long JamTimeRemainingMilliseconds { get; set; }
        public int PenaltyType { get; set; }
        public int PenaltyScale { get; set; }

        #region References


        [Required]
        public virtual GameMember MemberWhoPenaltied { get; set; }
        [Required]
        public virtual Game Game { get; set; }
        #endregion

    }
}
