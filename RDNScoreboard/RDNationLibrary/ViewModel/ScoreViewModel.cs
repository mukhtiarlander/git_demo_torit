using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDNationLibrary.ViewModel
{
    public class ScoreViewModel
    {
        public ScoreViewModel()
        { }

        public ScoreViewModel(int point, long periodTimeRemaining, int currentJam, int period)
        {
            Points = point;
            CurrentDateTimeScored = DateTime.UtcNow;
            PeriodTimeRemaining = periodTimeRemaining;
            Period = period;
            JamNumber = currentJam;
        }
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
        public long Period{ get; set; }
        /// <summary>
        /// jame number
        /// </summary>
        public int JamNumber { get; set; }
    }
}
