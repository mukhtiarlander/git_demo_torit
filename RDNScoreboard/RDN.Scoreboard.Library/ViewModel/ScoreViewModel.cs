using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scoreboard.Library.ViewModel.Members;

namespace Scoreboard.Library.ViewModel
{
    public class ScoreViewModel
    {
        public Guid PointId { get; set; }
        /// <summary>
        /// points retained
        /// </summary>
        public int Points { get; set; }
        /// <summary>
        /// current date time
        /// </summary>
        public DateTime CurrentDateTimeScored { get; set; }
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
        public Guid JamId { get; set; }

        public int JamNumber { get; set; }

        //public Guid JamId { get; set; }
        /// <summary>
        /// player who scored the jam.  Can be null
        /// </summary>
        public TeamMembersViewModel PlayerWhoScored { get; set; }
        /// <summary>
        /// Jam time remaining when point Scored
        /// </summary>
        public long JamTimeRemaining { get; set; }

        public ScoreViewModel()
        { }
        /// <summary>
        /// used to poulate the scoreboard.  This one is used by the API via web appliction to populate the player
        /// who scored the points.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="periodTimeRemaining"></param>
        /// <param name="currentJam"></param>
        /// <param name="period"></param>
        /// <param name="jammer"></param>
        public ScoreViewModel(TeamMembersViewModel playerScored, int point, Guid currentJamId, int currentJamNumber)
        {
            PlayerWhoScored = playerScored;
            Points = point;
            CurrentDateTimeScored = DateTime.UtcNow;
            PeriodTimeRemaining = GameViewModel.Instance.PeriodClock.TimeRemaining;
            JamTimeRemaining = GameViewModel.Instance.CurrentJam.JamClock.TimeRemaining;
            Period = GameViewModel.Instance.CurrentPeriod;
            JamNumber = currentJamNumber;
            JamId = currentJamId;
            PointId = Guid.NewGuid();
        }
        /// <summary>
        /// This one is used by the scoreboard operator. They have the choice to add scores as well.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="periodTimeRemaining"></param>
        /// <param name="currentJam"></param>
        /// <param name="period"></param>
        public ScoreViewModel(int point, long periodTimeRemaining, Guid currentJam, int currentJamNumber, int period)
        {
            Points = point;
            CurrentDateTimeScored = DateTime.UtcNow;
            PeriodTimeRemaining = periodTimeRemaining;

            Period = period;
            JamNumber = currentJamNumber;
            JamId = currentJam;
            //JamId = currentJamId;
            PointId = Guid.NewGuid();
        }
        /// <summary>
        /// used to populate from the Add Jams Page on the website
        /// </summary>
        /// <param name="currentJam"></param>
        public ScoreViewModel(Guid currentJam, int currentJamNumber)
        {
            CurrentDateTimeScored = DateTime.UtcNow;
            JamNumber = currentJamNumber;
            JamId = currentJam;
            PointId = Guid.NewGuid();
        }

        /// <summary>
        /// use to populate from the DB.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="periodTimeRemaining"></param>
        /// <param name="currentJam"></param>
        /// <param name="period"></param>
        /// <param name="timeScored"></param>
        /// <param name="pointId"></param>
        public ScoreViewModel(int point, long periodTimeRemaining, Guid currentJamId, int currentJamNumber, int period, DateTime timeScored, Guid pointId)
        {
            Points = point;
            CurrentDateTimeScored = timeScored;
            PeriodTimeRemaining = periodTimeRemaining;
            Period = period;
            JamNumber = currentJamNumber;
            JamId = currentJamId;
            PointId = pointId;
        }


    }
}
