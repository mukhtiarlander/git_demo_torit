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
    [Table("RDN_Game_Policy")]
    public class GamePolicy : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PolicyId { get; set; }

        public int LineupClockControlsStartJam { get; set; }
        public int EnableIntermissionStartOfClock { get; set; }
        public long IntermissionStartOfClockInMilliseconds { get; set; }
        public int IntermissionStopClockEnable { get; set; }
        public int IntermissionStopClockIncrementJamNumber { get; set; }
        public int IntermissionStopClockResetJamNumber { get; set; }
        public int IntermissionStopClockResetJamTime { get; set; }
        public int IntermissionStopClockIncrementPeriodNumber { get; set; }
        public int IntermissionStopClockResetPeriodNumber { get; set; }
        public int IntermissionStopClockResetPeriodTime { get; set; }
        public int JamClockControlsLineUpClock { get; set; }
        public int JamClockControlsTeamPositions { get; set; }
        public int PenaltyBoxControlsLeadJammer { get; set; }
        public int PeriodClockControlsLineupJamClock { get; set; }
        public int EnableAdChange { get; set; }
        public int AdChangeUseLineUpClock { get; set; }
        public long AdChangeDisplayChangesInMilliSeconds { get; set; }
        public int AdChangeAutomaticallyChangeImage { get; set; }
        public int AdChangeShowAdsDuringIntermission { get; set; }
        public int AdChangeShowAdsRandomly { get; set; }
        public int AlwaysShowJamClock { get; set; }
        public int TimeoutClockControlsLineupClock { get; set; }
        public int EnableIntermissionNaming { get; set; }
        public int HideClockTimeAfterBout { get; set; }
        [MaxLength(1000)]
        public string FirstIntermissionNameText { get; set; }
        [MaxLength(1000)]
        public string FirstIntermissionNameConfirmedText { get; set; }
        [MaxLength(1000)]
        public string SecondIntermissionNameText { get; set; }
        [MaxLength(1000)]
        public string SecondIntermissionNameConfirmedText { get; set; }
        [MaxLength(1000)]
        public string IntermissionOtherText { get; set; }
        public long JamClockTimePerJam { get; set; }
        public long LineUpClockPerJam { get; set; }
        public long PeriodClock { get; set; }
        public int NumberOfPeriods { get; set; }
        public int TimeOutsPerPeriod { get; set; }
        public long TimeOutClock { get; set; }
        [MaxLength(20)]
        public string GameSelectionType { get; set; }

        [MaxLength(5)]
        public string StartJamKeyShortcut { get; set; }
        [MaxLength(5)]
        public string StopJamKeyShortcut { get; set; }
        [MaxLength(5)]
        public string Team1TimeOutKeyShortcut { get; set; }
        [MaxLength(5)]
        public string Team2TimeOutKeyShortcut { get; set; }
        [MaxLength(5)]
        public string Team1ScoreUpKeyShortcut { get; set; }
        [MaxLength(5)]
        public string Team2ScoreUpKeyShortcut { get; set; }
        [MaxLength(5)]
        public string Team1ScoreDownKeyShortcut { get; set; }
        [MaxLength(5)]
        public string Team2ScoreDownKeyShortcut { get; set; }
        [MaxLength(5)]
        public string OfficialTimeOutKeyShortcut { get; set; }
        [MaxLength(5)]
        public string Team1LeadJammerKeyShortcut { get; set; }
        [MaxLength(5)]
        public string Team2LeadJammerKeyShortcut { get; set; }
        public int StopLineUpClockAtZero { get; set; }
        public int StopPeriodClockWhenLineUpClockHitsZero { get; set; }


        #region References

        [Required]
        public virtual Game Game { get; set; }

        #endregion
    }
}
