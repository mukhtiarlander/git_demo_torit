using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RDNationLibrary.Static.Enums;

namespace RDNationLibrary.ViewModel
{
    /// <summary>
    /// Model for when a skater is sent to the penalty box.
    /// </summary>
    public class SkaterInPenaltyBoxViewModel
    {
        public TeamMembersViewModel PlayerSentToBox { get; set; }
        public long GameTimeInMillisecondsSent { get; set; }
        public long JamTimeInMillisecondsSent { get; set; }
        public long GameTimeInMillisecondsReleased { get; set; }
        public long JamTimeInMillisecondsReleased { get; set; }
        public int JamNumberSent{ get; set; }
        public int JamNumberReleased { get; set; }
        public int PenaltyNumber { get; set; }
        public PenaltiesEnum PenaltyType { get; set; }
        public PenaltyScaleEnum PenaltyScale { get; set; }
    }
}
