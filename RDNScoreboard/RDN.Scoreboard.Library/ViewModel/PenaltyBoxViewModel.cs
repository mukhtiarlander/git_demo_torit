using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Scoreboard.Library.Static.Enums;
using Scoreboard.Library.ViewModel.Members;

namespace Scoreboard.Library.ViewModel
{
    /// <summary>
    /// Model for when a skater is sent to the penalty box.
    /// </summary>
    public class SkaterInPenaltyBoxViewModel
    {
        public SkaterInPenaltyBoxViewModel(TeamMembersViewModel skaterSent, long gameTimeInMillisecondsSent, long jamTimeInMillisecondsSent, Guid jamIdSent, int jamNumberSent)
        {
            PlayerSentToBox = skaterSent;
            GameTimeInMillisecondsSent = gameTimeInMillisecondsSent;
            JamTimeInMillisecondsSent = jamTimeInMillisecondsSent;
            JamIdSent = jamIdSent;
            JamNumberSent = jamNumberSent;
            PenaltyId = Guid.NewGuid();
        }
        /// <summary>
        /// Dummy constructor for serialization.  Do not use.
        /// </summary>
        public SkaterInPenaltyBoxViewModel()
        {
            PenaltyScale = PenaltyScaleEnum.Unknown;
        }
        /// <summary>
        /// the actual penalty count that this skater has received for the game
        /// main reason we use this, is for adding old games from the web site.
        /// </summary>
        public int PenaltyNumberForSkater { get; set; }
        public TeamMembersViewModel PlayerSentToBox { get; set; }
        public long GameTimeInMillisecondsSent { get; set; }
        public long JamTimeInMillisecondsSent { get; set; }
        public long GameTimeInMillisecondsReleased { get; set; }
        public long JamTimeInMillisecondsReleased { get; set; }
        public int JamNumberSent { get; set; }
        public int JamNumberReleased { get; set; }
        public Guid JamIdSent { get; set; }
        public Guid JamIdReleased { get; set; }
        public Guid PenaltyId { get; set; }
        public PenaltiesEnum PenaltyType { get; set; }
        public PenaltyScaleEnum PenaltyScale { get; set; }
    }
}
