using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game.Members
{
    [Table("RDN_Game_Block")]
    public class GameMemberBlock : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long BlockId { get; set; }
        //id of the score from the game it self generated on the client machine.
        [Required]
        public Guid GameBlockId { get; set; }
        [Required]
        public DateTime DateTimeBlocked { get; set; }
        [Required]
        public int JamNumber { get; set; }
        public Guid JamId { get; set; }
        [Required]
        public int PeriodNumber { get; set; }
        [Required]
        public long PeriodTimeRemainingMilliseconds { get; set; }

        #region References


        [Required]
        public virtual GameMember MemberWhoBlocked { get; set; }
        [Required]
        public virtual Game Game { get; set; }
        #endregion

    }
}
