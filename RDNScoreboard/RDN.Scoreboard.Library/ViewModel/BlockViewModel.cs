using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scoreboard.Library.ViewModel.Members;

namespace Scoreboard.Library.ViewModel
{
    public class BlockViewModel
    {
        public Guid BlockId { get; set; }

        /// <summary>
        /// current date time
        /// </summary>
        public DateTime CurrentDateTimeBlock { get; set; }
        /// <summary>
        /// remaining time in the period
        /// </summary>
        public long PeriodTimeRemaining { get; set; }
        /// <summary>
        /// period number
        /// </summary>
        public int Period { get; set; }
        /// <summary>
        /// jame number
        /// </summary>
        public int JamNumber { get; set; }

        public Guid JamId { get; set; }
        /// <summary>
        /// player who assisted the jam.  Can be null
        /// </summary>
        public TeamMembersViewModel PlayerWhoBlocked { get; set; }
        /// <summary>
        /// Jam time remaining when point Scored
        /// </summary>
        public long JamTimeRemaining { get; set; }

        /// <summary>
        /// dummy constructor used for Serialization
        /// </summary>
        public BlockViewModel()
        { }
        /// <summary>
        /// used to populate from the actual scoreboard
        /// </summary>
        /// <param name="playerAssisted"></param>
        /// <param name="periodTimeRemaining"></param>
        /// <param name="jamTimeInMilliseconds"></param>
        /// <param name="currentJam"></param>
        /// <param name="period"></param>
        public BlockViewModel(TeamMembersViewModel playerWhoBlocked, int currentJam, Guid currentJamId)
        {
            PlayerWhoBlocked = playerWhoBlocked;
            CurrentDateTimeBlock = DateTime.UtcNow;
            PeriodTimeRemaining = GameViewModel.Instance.PeriodClock.TimeRemaining;
            JamTimeRemaining = GameViewModel.Instance.CurrentJam.JamClock.TimeRemaining;
            Period = GameViewModel.Instance.CurrentPeriod;
            JamNumber = currentJam;
            JamId = currentJamId;
            BlockId = Guid.NewGuid();
        }
        /// <summary>
        /// used only when adding a block from the DB.
        /// </summary>
        /// <param name="periodTimeRemaining"></param>
        /// <param name="currentJam"></param>
        /// <param name="period"></param>
        /// <param name="timeBlocked"></param>
        /// <param name="playerWhoBlocked"></param>
        public BlockViewModel(long periodTimeRemaining, int currentJam, int period, DateTime timeBlocked,Guid blockId)
        {
            
            CurrentDateTimeBlock = timeBlocked;
            PeriodTimeRemaining = periodTimeRemaining;
            Period = period;
            JamNumber = currentJam;
            BlockId = blockId;
        }
    }
}
