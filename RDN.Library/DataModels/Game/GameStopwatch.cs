using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;
namespace RDN.Library.DataModels.Game
{
    /// <summary>
    /// used for all types of stop watches, the game clock, jam clock or penalty clock.
    /// </summary>
    [Table("RDN_Game_Stopwatch")]
    public class GameStopwatch:InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long StopWatchId { get; set; }
        //we use this to decide which stop watch this is for.  For the game, jam, penalty, time out clock... This makes this table
        // a many to many relationship.
        public Guid StopwatchForId { get; set; }
        public int JamNumber { get; set; }
        public Guid JamId { get; set; }
        public int IsRunning { get; set; }
        public long Length { get; set; }

        public DateTime StartDateTime { get; set; }
        public long TimeElapsed { get; set; }
        public int IsClockAtZero { get; set; }
        public long TimeRemaining { get; set; }
        public int Type { get; set; }


        #region References

        [Required]
        public virtual Game Game { get; set; }
        #endregion

    }
}
