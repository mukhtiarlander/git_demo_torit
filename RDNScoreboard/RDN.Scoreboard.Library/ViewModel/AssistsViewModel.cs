using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scoreboard.Library.ViewModel.Members;

namespace Scoreboard.Library.ViewModel
{
    public class AssistViewModel
    {
        public Guid AssistId { get; set; }

        /// <summary>
        /// current date time
        /// </summary>
        public DateTime CurrentDateTimeAssisted { get; set; }
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
        public TeamMembersViewModel PlayerWhoAssisted { get; set; }
        /// <summary>
        /// Jam time remaining when point Scored
        /// </summary>
        public long JamTimeRemaining { get; set; }

        /// <summary>
        /// dummy constructor used for Serialization
        /// </summary>
        public AssistViewModel()
        { }
        /// <summary>
        /// used to populate from the actual scoreboard
        /// </summary>
        /// <param name="playerAssisted"></param>
        /// <param name="periodTimeRemaining"></param>
        /// <param name="jamTimeInMilliseconds"></param>
        /// <param name="currentJam"></param>
        /// <param name="period"></param>
        public AssistViewModel(TeamMembersViewModel playerAssisted, long periodTimeRemaining, long jamTimeInMilliseconds, int currentJam, int period)
        {
            PlayerWhoAssisted = playerAssisted;
            CurrentDateTimeAssisted = DateTime.UtcNow;
            PeriodTimeRemaining = periodTimeRemaining;
            JamTimeRemaining = jamTimeInMilliseconds;
            Period = period;
            JamNumber = currentJam;
            AssistId = Guid.NewGuid();
        }
        public AssistViewModel(TeamMembersViewModel playerAssisted, int currentJam, Guid currentJamId)
        {
            PlayerWhoAssisted = playerAssisted;
            CurrentDateTimeAssisted = DateTime.UtcNow;
            PeriodTimeRemaining = GameViewModel.Instance.PeriodClock.TimeRemaining;
            JamTimeRemaining = GameViewModel.Instance.CurrentJam.JamClock.TimeRemaining;
            Period = GameViewModel.Instance.CurrentPeriod;
            JamNumber = currentJam;
            JamId = currentJamId;
            AssistId = Guid.NewGuid();
        }
        /// <summary>
        /// used when adding an assist from the DB.
        /// </summary>
        /// <param name="periodTimeRemaining"></param>
        /// <param name="currentJam"></param>
        /// <param name="period"></param>
        /// <param name="timeAssisted"></param>
        /// <param name="assistId"></param>
        public AssistViewModel(long periodTimeRemaining, int currentJam, int period, DateTime timeAssisted, Guid assistId)
        {

            CurrentDateTimeAssisted = timeAssisted;
            PeriodTimeRemaining = periodTimeRemaining;
            Period = period;
            JamNumber = currentJam;
            AssistId = assistId;
        }
    }
}
