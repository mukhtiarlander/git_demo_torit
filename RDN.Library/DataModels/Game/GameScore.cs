using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.Game.Members;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game
{
    [Table("RDN_Game_Score")]
    public class GameScore : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ScoreId { get; set; }
        //id of the score from the game it self generated on the client machine.
        [Required]
        public Guid GameScoreId { get; set; }
        [Required]
        public DateTime DateTimeScored { get; set; }
        [Required]
        public int Point { get; set; }
        [Required]
        public int JamNumber { get; set; }

        public Guid JamId { get; set; }
        [Required]
        public int PeriodNumber { get; set; }
        [Required]
        public long PeriodTimeRemainingMilliseconds { get; set; }



        #region References

        [Required]
        public virtual GameTeam GameTeam { get; set; }

        public virtual GameMember MemberWhoScored { get; set; }

        #endregion

    }
}
