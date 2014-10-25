using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scoreboard.Library.Static.Enums;

namespace Scoreboard.Library.ViewModel.Officials
{
    public class OfficialReviewViewModel
    {
        public Guid OfficialReviewId { get; set; }

        /// <summary>
        /// current date time
        /// </summary>
        public DateTime CurrentDateTimeReviewed { get; set; }
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

        public TeamNumberEnum TeamNumber { get; set; }

        public string Details { get; set; }
        public string Result { get; set; }

        public OfficialReviewViewModel()
        {
            OfficialReviewId = Guid.NewGuid();
            CurrentDateTimeReviewed = DateTime.UtcNow;
            if (GameViewModel.Instance.PeriodClock != null)
                PeriodTimeRemaining = GameViewModel.Instance.PeriodClock.TimeRemaining;
            Period = GameViewModel.Instance.CurrentPeriod;
            if (GameViewModel.Instance.CurrentJam != null)
            {
                JamNumber = GameViewModel.Instance.CurrentJam.JamNumber;
                JamId = GameViewModel.Instance.CurrentJam.JamId;
            }
        }
        public OfficialReviewViewModel SetTeam(TeamNumberEnum team)
        {
            TeamNumber = team;
            return this;
        }

    }
}
