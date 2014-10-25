using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

using Scoreboard.Library.Static;
using Scoreboard.Library.Static.Enums;
using System.ComponentModel;
using RDN.Utilities.Config;
using Scoreboard.Library.ViewModel.Video;
using RDN.Utilities.Error;
using RDN.Utilities.Util;


namespace Scoreboard.Library.ViewModel
{
    public enum PolicyViewModelEnum
    {
        SkaterLineUpTheme, DefaultScoreboardTheme, DefaultLanguage,
        EnableAdChange, LineupClockControlsStartJam, EnableIntermissionStartOfClock,
        IntermissionStartOfClockInMilliseconds, IntermissionStopClockEnable,
        IntermissionStopClockIncrementJamNumber, IntermissionStopClockResetJamNumber,
        IntermissionStopClockResetJamTime, IntermissionStopClockIncrementPeriodNumber,
        IntermissionStopClockResetPeriodNumber, IntermissionStopClockResetPeriodTime,
        JamClockControlsLineUpClock, JamClockControlsTeamPositions,
        PenaltyBoxControlsLeadJammer, PeriodClockControlsLineupJamClock,
        AdChangeUseLineUpClock, ShowLatestPoints,
        AdChangeDisplayChangesInMilliSeconds, AdChangeAutomaticallyChangeImage,
        AdChangeShowAdsDuringIntermission, AdChangeShowAdsRandomly,
        AlwaysShowJamClock, TimeoutClockControlsLineupClock,
        EnableIntermissionNaming, HideClockTimeAfterBout,
        FirstIntermissionNameText, FirstIntermissionNameConfirmedText,
        SecondIntermissionNameText, SecondIntermissionNameConfirmedText,
        IntermissionOtherText, JamClockTimePerJam,
        LineUpClockPerJam, PeriodClock,
        NumberOfPeriods, TimeOutsPerPeriod,
        TimeOutClock, GameSelectionType,
        StartJamKeyShortcut, StopJamKeyShortcut, Team1TimeOutKeyShortcut, Team2TimeOutKeyShortcut,
        Team1ScoreUpKeyShortcut, Team2ScoreUpKeyShortcut, Team1ScoreDownKeyShortcut, Team2ScoreDownKeyShortcut, OfficialTimeOutKeyShortcut,
        Team1LeadJammerKeyShortcut, Team2LeadJammerKeyShortcut, StopLineUpClockAtZero, StopPeriodClockWhenLineUpClockHitsZero, ShowActiveClockDuringSlideShow,
        SecondsPerSlideShowSlide, RotateSlideShowSlides, ISUPDATEPROPERTY, ShowActiveJammerPictures

    }
    /// <summary>
    /// Updates the policy of the application.
    /// If you ADD ANY items in the policyViewModel, Make sure to rename the ISUPDATEPROPERTY
    /// </summary>
    public class PolicyViewModel : INotifyPropertyChanged
    {
        static PolicyViewModel instance = new PolicyViewModel();

        public static string DEFAULT_BUZZER_FILE_LOCATION = "Resources/buzzer.mp3";

        public static PolicyViewModel Instance
        {
            get
            {
                return instance;
            }
        }

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static PolicyViewModel()
        { }

        PolicyViewModel()
        { }

        public Overlay VideoOverlay { get; set; }

        private string _ISUPDATEPROPERTY;
        /// <summary>
        /// property controls if the scoreboard policy is in need of an update.
        /// We check this property to see if the default values have been set on the new update of the scoreboard.
        /// </summary>
        public string ISUPDATEPROPERTYNEW
        {
            get { return _ISUPDATEPROPERTY; }
            set
            {
                _ISUPDATEPROPERTY = value;
                OnPropertyChanged("ISUPDATEPROPERTY");
            }
        }
        private int _secondsPerSlideShowSlide;
        /// <summary>
        /// seconds per slide in the slideshow
        /// </summary>
        public int SecondsPerSlideShowSlide
        {
            get { return _secondsPerSlideShowSlide; }
            set
            {
                _secondsPerSlideShowSlide = value;
                OnPropertyChanged("SecondsPerSlide");
            }
        }
        private bool _showActiveClockDuringSlideShow;
        /// <summary>
        /// shows the active clock during slide show
        /// </summary>
        public bool ShowActiveClockDuringSlideShow
        {
            get { return _showActiveClockDuringSlideShow; }
            set
            {
                _showActiveClockDuringSlideShow = value;
                OnPropertyChanged("ShowActiveClockDuringSlideShow");
            }
        }
        private bool _rotateSlideShowSlides;
        /// <summary>
        ///Rotates the slides at the seconds specified
        /// </summary>
        public bool RotateSlideShowSlides
        {
            get { return _rotateSlideShowSlides; }
            set
            {
                _rotateSlideShowSlides = value;
                OnPropertyChanged("RotateSlideShowSlides");
            }
        }
        private string _skaterLineUpTheme;
        public string SkaterLineUpTheme
        {
            get { return _skaterLineUpTheme; }
            set
            {
                _skaterLineUpTheme = value;
                OnPropertyChanged("SkaterLineUpTheme");
            }
        }

        private string _defaultScoreboardTheme;
        /// <summary>
        /// The current theme selected by the user of the default scoreboard.
        /// </summary>
        public string DefaultScoreboardTheme
        {
            get { return _defaultScoreboardTheme; }
            set
            {
                _defaultScoreboardTheme = value;
                OnPropertyChanged("DefaultScoreboardTheme");
            }
        }
        private string _defaultLanguage;
        /// <summary>
        /// The current language selected by the user of the default scoreboard.
        /// </summary>
        public string DefaultLanguage
        {
            get { return _defaultLanguage; }
            set
            {
                _defaultLanguage = value;
                OnPropertyChanged("DefaultLanguage");
            }
        }

        private bool _lineupClockControlsStartJam;
        /// <summary>
        /// Countdown Clock Controls Start of Jam: Starts the Jam automatically when the time between jams has ended.
        /// </summary>
        public bool LineupClockControlsStartJam
        {
            get { return _lineupClockControlsStartJam; }
            set
            {
                _lineupClockControlsStartJam = value;
                OnPropertyChanged("LineupClockControlsStartJam");
            }
        }
        private bool _enableIntermissionStartOfClock;
        /// <summary>
        /// Enable Intermission Start Clock: Starts the intermission clock to the default or designated intermission time.
        /// </summary>
        public bool EnableIntermissionStartOfClock
        {
            get { return _enableIntermissionStartOfClock; }
            set
            {
                _enableIntermissionStartOfClock = value;
                OnPropertyChanged("EnableIntermissionStartOfClock");
            }
        }
        private long _intermissionStartOfClockInMilliseconds;
        /// <summary>
        /// Set Intermission Time: Sets the time for intermission
        /// </summary>
        public long IntermissionStartOfClockInMilliseconds
        {
            get { return _intermissionStartOfClockInMilliseconds; }
            set
            {
                _intermissionStartOfClockInMilliseconds = value;
                OnPropertyChanged("IntermissionStartOfClockInMilliseconds");
            }
        }
        private bool _IntermissionStopClockEnable;
        /// <summary>
        /// Enable Intermission Stop Clock: When the Intermission clock is over, this resets the Period and Jam clock time.
        /// </summary>
        public bool IntermissionStopClockEnable
        {
            get { return _IntermissionStopClockEnable; }
            set
            {
                _IntermissionStopClockEnable = value;
                OnPropertyChanged("IntermissionStopClockEnable");
            }
        }
        private bool _intermissionStopClockIncrementJamNumber;
        /// <summary>
        /// Increment Jam Number At Stop of Intermission: Increments the jam number automatically at stop of intermission.
        /// </summary>
        public bool IntermissionStopClockIncrementJamNumber
        {
            get { return _intermissionStopClockIncrementJamNumber; }
            set
            {
                _intermissionStopClockIncrementJamNumber = value;
                OnPropertyChanged("IntermissionStopClockIncrementJamNumber");
            }
        }
        private bool _intermissionStopClockResetJamNumber;
        /// <summary>
        /// Reset Jam Number At Stop of Intermission: Resets the jam number to zero automatically at stop of intermission.
        /// </summary>
        public bool IntermissionStopClockResetJamNumber
        {
            get { return _intermissionStopClockResetJamNumber; }
            set
            {
                _intermissionStopClockResetJamNumber = value;
                OnPropertyChanged("IntermissionStopClockResetJamNumber");
            }
        }
        private bool _intermissionStopClockResetJamTime;
        /// <summary>
        /// Reset Jam Time At Stop of Intermission: Resets the jam Time to zero automatically at stop of intermission.
        /// </summary>
        public bool IntermissionStopClockResetJamTime
        {
            get { return _intermissionStopClockResetJamTime; }
            set
            {
                _intermissionStopClockResetJamTime = value;
                OnPropertyChanged("IntermissionStopClockResetJamTime");
            }
        }
        private bool _intermissionStopClockIncrementPeriodNumber;
        /// <summary>
        /// Increment Period Number At Stop of Intermission Clock.
        /// </summary>
        public bool IntermissionStopClockIncrementPeriodNumber
        {
            get { return _intermissionStopClockIncrementPeriodNumber; }
            set
            {
                _intermissionStopClockIncrementPeriodNumber = value;
                OnPropertyChanged("IntermissionStopClockIncrementPeriodNumber");
            }
        }
        private bool _intermissionStopClockResetPeriodNumber;
        /// <summary>
        /// Reset Period Number At Stop of Intermission Clock.
        /// </summary>
        public bool IntermissionStopClockResetPeriodNumber
        {
            get { return _intermissionStopClockResetPeriodNumber; }
            set
            {
                _intermissionStopClockResetPeriodNumber = value;
                OnPropertyChanged("IntermissionStopClockResetPeriodNumber");
            }
        }
        private bool _intermissionStopClockResetPeriodTime;
        /// <summary>
        /// Reset Period Time At Stop of Intermission Clock.
        /// </summary>
        public bool IntermissionStopClockResetPeriodTime
        {
            get { return _intermissionStopClockResetPeriodTime; }
            set
            {
                _intermissionStopClockResetPeriodTime = value;
                OnPropertyChanged("IntermissionStopClockResetPeriodTime");
            }
        }
        private bool _jamClockControlsLineUpClock;
        /// <summary>
        /// Starts the line up clock when the jam clock stops.  Starts the Jam clock when the line up clock stops.
        /// </summary>
        public bool JamClockControlsLineUpClock
        {
            get { return _jamClockControlsLineUpClock; }
            set
            {
                _jamClockControlsLineUpClock = value;
                OnPropertyChanged("JamClockControlsLineUpClock");
            }
        }
        private bool _jamClockControlsTeamPositions;
        /// <summary>
        /// This clears all Team Positions (who are not in the Penalty Box) when the Jam clock is stopped, sets all Skaters to Not Lead Jammer, and sets the Team to Not Lead Jammer.
        /// </summary>
        public bool JamClockControlsTeamPositions
        {
            get { return _jamClockControlsTeamPositions; }
            set
            {
                _jamClockControlsTeamPositions = value;
                OnPropertyChanged("JamClockControlsTeamPositions");
            }
        }
        private bool _penaltyBoxControlsLeadJammer;
        /// <summary>
        /// This removes Lead Jammer from any Skater sent to the Penalty Box.
        /// </summary>
        public bool PenaltyBoxControlsLeadJammer
        {
            get { return _penaltyBoxControlsLeadJammer; }
            set
            {
                _penaltyBoxControlsLeadJammer = value;
                OnPropertyChanged("PenaltyBoxControlsLeadJammer");
            }
        }
        private bool _periodClockControlsLineupJamClock;
        /// <summary>
        /// This controls the Lineup clock based on the Period clock. When the Period clock stops and its time is equal to its 0 (i.e. its minimum), and the Jam clock is also stopped, the Lineup clock is stopped and reset.
        /// </summary>
        public bool PeriodClockControlsLineupJamClock
        {
            get { return _periodClockControlsLineupJamClock; }
            set
            {
                _periodClockControlsLineupJamClock = value;
                OnPropertyChanged("PenaltyBoxControlsLeadJammer");
            }
        }
        private bool _enableAdChange;

        /// <summary>
        /// Enables the Ad change policy on the board
        /// </summary>
        public bool EnableAdChange
        {
            get { return _enableAdChange; }
            set
            {
                _enableAdChange = value;
                OnPropertyChanged("EnableAdChange");
            }
        }
        private bool _adChangeUseLineUpClock;
        /// <summary>
        /// Shows the adds where the line up clock sits.
        /// </summary>
        public bool AdChangeUseLineUpClock
        {
            get { return _adChangeUseLineUpClock; }
            set
            {
                _adChangeUseLineUpClock = value;
                OnPropertyChanged("AdChangeUseLineUpClock");
            }
        }
        private bool _showLatestPoints;
        /// <summary>
        /// Shows the adds where the line up clock sits.
        /// </summary>
        public bool ShowLatestPoints
        {
            get { return _showLatestPoints; }
            set
            {
                _showLatestPoints = value;
                OnPropertyChanged("ShowLatestPoints");
            }
        }
        private bool _showActiveJammerPictures;
        /// <summary>
        /// Shows the adds where the line up clock sits.
        /// </summary>
        public bool ShowActiveJammerPictures
        {
            get { return _showActiveJammerPictures; }
            set
            {
                _showActiveJammerPictures = value;
                OnPropertyChanged("ShowActiveJammerPictures");
            }
        }
        private long _adChangeDisplayChangesInMilliSeconds;
        /// <summary>
        /// Ads change at the seconds specified
        /// </summary>
        public long AdChangeDisplayChangesInMilliSeconds
        {
            get { return _adChangeDisplayChangesInMilliSeconds; }
            set
            {
                _adChangeDisplayChangesInMilliSeconds = value;
                OnPropertyChanged("AdChangeDisplayChangesInMilliSeconds");
            }
        }
        private bool _adChangeAutomaticallyChangeImage;
        /// <summary>
        /// Automatically change the ad image
        /// </summary>
        public bool AdChangeAutomaticallyChangeImage
        {
            get { return _adChangeAutomaticallyChangeImage; }
            set
            {
                _adChangeAutomaticallyChangeImage = value;
                OnPropertyChanged("AdChangeAutomaticallyChangeImage");
            }
        }
        private bool _adChangeShowAdsDuringIntermission;
        /// <summary>
        /// Show Ads during intermission
        /// </summary>
        public bool AdChangeShowAdsDuringIntermission
        {
            get { return _adChangeShowAdsDuringIntermission; }
            set
            {
                _adChangeShowAdsDuringIntermission = value;
                OnPropertyChanged("AdChangeShowAdsDuringIntermission");
            }
        }
        private bool _adChangeShowAdsRandomly;
        /// <summary>
        /// Show Ads in Random Order
        /// </summary>
        public bool AdChangeShowAdsRandomly
        {
            get { return _adChangeShowAdsRandomly; }
            set
            {
                _adChangeShowAdsRandomly = value;
                OnPropertyChanged("AdChangeShowAdsRandomly");
            }
        }
        private bool _alwaysShowJamClock;
        /// <summary>
        /// Always Show Jam Clock
        /// </summary>
        public bool AlwaysShowJamClock
        {
            get { return _alwaysShowJamClock; }
            set
            {
                _alwaysShowJamClock = value;
                OnPropertyChanged("AlwaysShowJamClock");
            }
        }
        private bool _timeoutClockControlsLineupClock;
        /// <summary>
        /// This controls the Lineup clock based on the Timeout clock. When the Timeout clock starts, the Lineup clock is stopped then reset.
        /// </summary>
        public bool TimeoutClockControlsLineupClock
        {
            get { return _timeoutClockControlsLineupClock; }
            set
            {
                _timeoutClockControlsLineupClock = value;
                OnPropertyChanged("TimeoutClockControlsLineupClock");
            }
        }
        private bool _enableIntermissionNaming;
        /// <summary>
        /// Enable the intermission naming scheme.
        /// </summary>
        public bool EnableIntermissionNaming
        {
            get { return _enableIntermissionNaming; }
            set
            {
                _enableIntermissionNaming = value;
                OnPropertyChanged("EnableIntermissionNaming");
            }
        }
        private bool _hideClockTimeAfterBout;
        /// <summary>
        /// Hides the clocks after Bout is over.
        /// </summary>
        public bool HideClockTimeAfterBout
        {
            get { return _hideClockTimeAfterBout; }
            set
            {
                _hideClockTimeAfterBout = value;
                OnPropertyChanged("HideClockTimeAfterBout");
            }
        }
        private string _firstIntermissionNameText;
        /// <summary>
        /// Name of the First Intermission
        /// </summary>
        public string FirstIntermissionNameText
        {
            get { return _firstIntermissionNameText; }
            set
            {
                _firstIntermissionNameText = value;
                OnPropertyChanged("HideClockTimeAfterBout");
            }
        }
        private string _firstIntermissionNameConfirmedText;
        /// <summary>
        /// Intermission Name after Confirmed Points
        /// </summary>
        public string FirstIntermissionNameConfirmedText
        {
            get { return _firstIntermissionNameConfirmedText; }
            set
            {
                _firstIntermissionNameConfirmedText = value;
                OnPropertyChanged("FirstIntermissionNameConfirmedText");
            }
        }
        private string _secondIntermissionNameText;
        /// <summary>
        /// Name of Second Intermission 
        /// </summary>
        public string SecondIntermissionNameText
        {
            get { return _secondIntermissionNameText; }
            set
            {
                _secondIntermissionNameText = value;
                OnPropertyChanged("SecondIntermissionNameText");
            }
        }
        private string _secondIntermissionNameConfirmedText;
        /// <summary>
        /// Name of Second Intermission Confirmed
        /// </summary>
        public string SecondIntermissionNameConfirmedText
        {
            get { return _secondIntermissionNameConfirmedText; }
            set
            {
                _secondIntermissionNameConfirmedText = value;
                OnPropertyChanged("SecondIntermissionNameConfirmedText");
            }
        }

        private string _intermissionOtherText;
        /// <summary>
        /// some other text used for intermission
        /// </summary>
        public string IntermissionOtherText
        {
            get { return _intermissionOtherText; }
            set
            {
                _intermissionOtherText = value;
                OnPropertyChanged("IntermissionOtherText");
            }
        }
        private long _jamClockTimePerJam;






        /// <summary>
        /// Jam Clock Time.
        /// </summary>
        public long JamClockTimePerJam
        {
            get { return _jamClockTimePerJam; }
            set
            {
                _jamClockTimePerJam = value;
                OnPropertyChanged("JamClockTimePerJam");
            }
        }
        private long _lineUpClockPerJam;
        public long LineUpClockPerJam
        {
            get { return _lineUpClockPerJam; }
            set
            {
                _lineUpClockPerJam = value;
                OnPropertyChanged("LineUpClockPerJam");
            }
        }
        private bool _lineUpTrackerControlsScore;
        /// <summary>
        /// line up tracker can control the score on the clock.
        /// </summary>
        public bool LineUpTrackerControlsScore
        {
            get { return _lineUpTrackerControlsScore; }
            set
            {
                _lineUpTrackerControlsScore = value;
                OnPropertyChanged("LineUpTrackerControlsScore");
            }
        }
        private bool _soundBuzzerAtEndOfPeriod;
        /// <summary>
        /// sounds a buzzer at the end of the period
        /// </summary>
        public bool SoundBuzzerAtEndOfPeriod
        {
            get { return _soundBuzzerAtEndOfPeriod; }
            set
            {
                _soundBuzzerAtEndOfPeriod = value;
                OnPropertyChanged("SoundBuzzerAtEndOfPeriod");
            }
        }
        private string _buzzerFileLocation;
        /// <summary>
        /// sounds a buzzer at the end of the period
        /// </summary>
        public string SoundBuzzerFileLocation
        {
            get { return _buzzerFileLocation; }
            set
            {
                _buzzerFileLocation = value;
                OnPropertyChanged("SoundBuzzerFileLocation");
            }
        }
        private byte[] _buzzerCompressed;
        /// <summary>
        /// sounds a buzzer at the end of the period
        /// </summary>
        public byte[] BuzzerCompressed
        {
            get { return _buzzerCompressed; }
            set
            {
                _buzzerCompressed = value;
                OnPropertyChanged("BuzzerCompressed");
            }
        }
        private long _periodClock;
        /// <summary>
        /// period clock in milliseconds
        /// </summary>
        public long PeriodClock
        {
            get { return _periodClock; }
            set
            {
                _periodClock = value;
                OnPropertyChanged("PeriodClock");
            }
        }
        private int _numberOfPeriods;
        /// <summary>
        /// total number of periods per game
        /// </summary>
        public int NumberOfPeriods
        {
            get { return _numberOfPeriods; }
            set
            {
                _numberOfPeriods = value;
                OnPropertyChanged("NumberOfPeriods");
            }
        }
        private int _timeOutsPerPeriod;
        /// <summary>
        /// number of time outs per period
        /// </summary>
        public int TimeOutsPerPeriod
        {
            get { return _timeOutsPerPeriod; }
            set
            {
                _timeOutsPerPeriod = value;
                OnPropertyChanged("TimeOutsPerPeriod");
            }
        }
        private long _timeOutClock;
        /// <summary>
        /// time out clock in milliseconds
        /// </summary>
        public long TimeOutClock
        {
            get { return _timeOutClock; }
            set
            {
                _timeOutClock = value;
                OnPropertyChanged("TimeOutClock");
            }
        }

        private GameTypeEnum _gameSelectionType;
        public GameTypeEnum GameSelectionType
        {
            get { return _gameSelectionType; }
            set
            {
                _gameSelectionType = value;
                OnPropertyChanged("GameSelectionType");
            }
        }


        private char _startJamKeyShortcut;

        public char StartJamKeyShortcut
        {
            get { return _startJamKeyShortcut; }
            set { _startJamKeyShortcut = value; OnPropertyChanged("StartJamKeyShortcut"); }
        }
        private char _stopJamKeyShortcut;

        public char StopJamKeyShortcut
        {
            get { return _stopJamKeyShortcut; }
            set { _stopJamKeyShortcut = value; OnPropertyChanged("StopJamKeyShortcut"); }
        }
        private char _team1TimeOutKeyShortcut;

        public char Team1TimeOutKeyShortcut
        {
            get { return _team1TimeOutKeyShortcut; }
            set { _team1TimeOutKeyShortcut = value; OnPropertyChanged("Team1TimeOutKeyShortcut"); }
        }
        private char _team2TimeOutKeyShortcut;

        public char Team2TimeOutKeyShortcut
        {
            get { return _team2TimeOutKeyShortcut; }
            set { _team2TimeOutKeyShortcut = value; OnPropertyChanged("Team2TimeOutKeyShortcut"); }
        }
        private char _team1ScoreUpKeyShortcut;

        public char Team1ScoreUpKeyShortcut
        {
            get { return _team1ScoreUpKeyShortcut; }
            set { _team1ScoreUpKeyShortcut = value; OnPropertyChanged("Team1ScoreUpKeyShortcut"); }
        }
        private char _team2ScoreUpKeyShortcut;

        public char Team2ScoreUpKeyShortcut
        {
            get { return _team2ScoreUpKeyShortcut; }
            set { _team2ScoreUpKeyShortcut = value; OnPropertyChanged("Team2ScoreUpKeyShortcut"); }
        }
        private char _team1ScoreDownKeyShortcut;

        public char Team1ScoreDownKeyShortcut
        {
            get { return _team1ScoreDownKeyShortcut; }
            set { _team1ScoreDownKeyShortcut = value; OnPropertyChanged("Team1ScoreDownKeyShortcut"); }
        }
        private char _team2ScoreDownKeyShortcut;

        public char Team2ScoreDownKeyShortcut
        {
            get { return _team2ScoreDownKeyShortcut; }
            set { _team2ScoreDownKeyShortcut = value; OnPropertyChanged("Team2ScoreDownKeyShortcut"); }
        }
        private char _officialTimeOutKeyShortcut;

        public char OfficialTimeOutKeyShortcut
        {
            get { return _officialTimeOutKeyShortcut; }
            set { _officialTimeOutKeyShortcut = value; OnPropertyChanged("OfficialTimeOutKeyShortcut"); }
        }
        private char _team1LeadJammerKeyShortcut;

        public char Team1LeadJammerKeyShortcut
        {
            get { return _team1LeadJammerKeyShortcut; }
            set { _team1LeadJammerKeyShortcut = value; OnPropertyChanged("Team1LeadJammerKeyShortcut"); }
        }
        private char _team2LeadJammerKeyShortcut;

        public char Team2LeadJammerKeyShortcut
        {
            get { return _team2LeadJammerKeyShortcut; }
            set { _team2LeadJammerKeyShortcut = value; OnPropertyChanged("Team2LeadJammerKeyShortcut"); }
        }

        private bool _stopLineUpClockAtZero;

        /// <summary>
        /// Enables the Ad change policy on the board
        /// </summary>
        public bool StopLineUpClockAtZero
        {
            get { return _stopLineUpClockAtZero; }
            set
            {
                _stopLineUpClockAtZero = value;
                OnPropertyChanged("StopLineUpClockAtZero");
            }
        }

        private bool _stopPeriodClockWhenLineUpClockHitsZero;

        /// <summary>
        /// Enables the Ad change policy on the board
        /// </summary>
        public bool StopPeriodClockWhenLineUpClockHitsZero
        {
            get { return _stopPeriodClockWhenLineUpClockHitsZero; }
            set
            {
                _stopPeriodClockWhenLineUpClockHitsZero = value;
                OnPropertyChanged("StopPeriodClockWhenLineUpClockHitsZero");
            }
        }

        /// <summary>
        /// makes the proper game policy changes to the game
        /// </summary>
        /// <param name="gameType"></param>
        public void changeGameSelectionType(GameTypeEnum gameType)
        {
            try
            {
                GameSelectionType = gameType;
                if (gameType == GameTypeEnum.MADE)
                {
                    instance.NumberOfPeriods = 4;
                    instance.PeriodClock = (long)TimeSpan.FromMinutes(15).TotalMilliseconds;
                    instance.JamClockTimePerJam = (long)TimeSpan.FromSeconds(90).TotalMilliseconds;
                    instance.LineUpClockPerJam = (long)TimeSpan.FromSeconds(30).TotalMilliseconds;
                    instance.TimeOutsPerPeriod = 1;
                }
                else if (gameType == GameTypeEnum.TEXAS_DERBY)
                {
                    instance.NumberOfPeriods = 4;
                    instance.PeriodClock = (long)TimeSpan.FromMinutes(8).TotalMilliseconds;
                    instance.JamClockTimePerJam = (long)TimeSpan.FromSeconds(60).TotalMilliseconds;
                    instance.LineUpClockPerJam = (long)TimeSpan.FromSeconds(30).TotalMilliseconds;
                    instance.TimeOutsPerPeriod = 1;
                    instance.TimeOutClock = (long)TimeSpan.FromSeconds(120).TotalMilliseconds;
                }
                else if (gameType == GameTypeEnum.MADE_COED)
                {
                    instance.NumberOfPeriods = 8;
                    instance.PeriodClock = (long)TimeSpan.FromMinutes(15).TotalMilliseconds;
                    instance.JamClockTimePerJam = (long)TimeSpan.FromSeconds(90).TotalMilliseconds;
                    instance.LineUpClockPerJam = (long)TimeSpan.FromSeconds(30).TotalMilliseconds;
                    instance.TimeOutsPerPeriod = 1;
                }
                else if (gameType == GameTypeEnum.RDCL)
                {
                    instance.NumberOfPeriods = 4;
                    instance.PeriodClock = (long)TimeSpan.FromMinutes(15).TotalMilliseconds;
                    instance.JamClockTimePerJam = (long)TimeSpan.FromSeconds(60).TotalMilliseconds;
                    instance.LineUpClockPerJam = (long)TimeSpan.FromSeconds(30).TotalMilliseconds;
                    instance.TimeOutsPerPeriod = 3;

                }
                else if (gameType == GameTypeEnum.WFTDA_2010)
                {
                    instance.NumberOfPeriods = 2;
                    instance.PeriodClock = (long)TimeSpan.FromMinutes(30).TotalMilliseconds;
                    instance.JamClockTimePerJam = (long)TimeSpan.FromMinutes(2).TotalMilliseconds;
                    instance.LineUpClockPerJam = (long)TimeSpan.FromSeconds(30).TotalMilliseconds;
                    instance.TimeOutsPerPeriod = 2;
                    instance.StopPeriodClockWhenLineUpClockHitsZero = false;
                }
                else if (gameType == GameTypeEnum.WFTDA)
                {
                    instance.NumberOfPeriods = 2;
                    instance.PeriodClock = (long)TimeSpan.FromMinutes(30).TotalMilliseconds;
                    instance.JamClockTimePerJam = (long)TimeSpan.FromMinutes(2).TotalMilliseconds;
                    instance.LineUpClockPerJam = (long)TimeSpan.FromSeconds(30).TotalMilliseconds;
                    instance.TimeOutsPerPeriod = 3;
                    instance.StopPeriodClockWhenLineUpClockHitsZero = false;
                }
                else if (gameType == GameTypeEnum.USARS)
                {
                    instance.NumberOfPeriods = 2;
                    instance.PeriodClock = (long)TimeSpan.FromMinutes(30).TotalMilliseconds;
                    instance.JamClockTimePerJam = (long)TimeSpan.FromMinutes(2).TotalMilliseconds;
                    instance.LineUpClockPerJam = (long)TimeSpan.FromSeconds(30).TotalMilliseconds;
                    instance.TimeOutsPerPeriod = 3;
                    instance.TimeOutClock = (long)TimeSpan.FromSeconds(90).TotalMilliseconds;
                }
                else if (gameType == GameTypeEnum.RENEGADE)
                {//TODO: find renenegade rule set
                }
                else if (gameType == GameTypeEnum.OSDA)
                {
                    instance.NumberOfPeriods = 4;
                    instance.PeriodClock = (long)TimeSpan.FromMinutes(15).TotalMilliseconds;
                    instance.JamClockTimePerJam = (long)TimeSpan.FromMinutes(90).TotalMilliseconds;
                    instance.LineUpClockPerJam = (long)TimeSpan.FromSeconds(30).TotalMilliseconds;
                    instance.TimeOutsPerPeriod = 1;
                }
                else if (gameType == GameTypeEnum.OSDA_COED)
                {
                    instance.NumberOfPeriods = 8;
                    instance.PeriodClock = (long)TimeSpan.FromMinutes(10).TotalMilliseconds;
                    instance.JamClockTimePerJam = (long)TimeSpan.FromSeconds(90).TotalMilliseconds;
                    instance.LineUpClockPerJam = (long)TimeSpan.FromSeconds(30).TotalMilliseconds;
                    instance.TimeOutsPerPeriod = 1;
                }
                else if (gameType == GameTypeEnum.CUSTOM)
                {
                    //because its custom game, we let the user decide all the presets.
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        public void SetPolicy(PolicyViewModel policy)
        {
            instance = policy;
        }

        public void loadFromXmlPolicy()
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(PolicyViewModel));

                //loads policy from a saved settings file.
                if (File.Exists(ScoreboardConfig.SAVE_POLICY_FILE))
                {
                    StreamReader objReader = new StreamReader(ScoreboardConfig.SAVE_POLICY_FILE);
                    try
                    {
                        PolicyViewModel policy = (PolicyViewModel)xs.Deserialize(objReader);
                        instance.AdChangeAutomaticallyChangeImage = policy.AdChangeAutomaticallyChangeImage;
                        instance.AdChangeDisplayChangesInMilliSeconds = policy.AdChangeDisplayChangesInMilliSeconds;
                        instance.AdChangeShowAdsDuringIntermission = policy.AdChangeShowAdsDuringIntermission;
                        instance.AdChangeShowAdsRandomly = policy.AdChangeShowAdsRandomly;
                        instance.AdChangeUseLineUpClock = policy.AdChangeUseLineUpClock;
                        instance.AlwaysShowJamClock = policy.AlwaysShowJamClock;
                        instance.LineupClockControlsStartJam = policy.LineupClockControlsStartJam;
                        instance.EnableAdChange = policy.EnableAdChange;
                        instance.EnableIntermissionNaming = policy.EnableIntermissionNaming;
                        instance.EnableIntermissionStartOfClock = policy.EnableIntermissionStartOfClock;
                        instance.FirstIntermissionNameConfirmedText = policy.FirstIntermissionNameConfirmedText;
                        instance.FirstIntermissionNameText = policy.FirstIntermissionNameText;
                        instance.HideClockTimeAfterBout = policy.HideClockTimeAfterBout;
                        instance.IntermissionOtherText = policy.IntermissionOtherText;
                        instance.IntermissionStartOfClockInMilliseconds = policy.IntermissionStartOfClockInMilliseconds;
                        instance.IntermissionStopClockEnable = policy.IntermissionStopClockEnable;
                        instance.IntermissionStopClockIncrementJamNumber = policy.IntermissionStopClockIncrementJamNumber;
                        instance.IntermissionStopClockIncrementPeriodNumber = policy.IntermissionStopClockIncrementPeriodNumber;
                        instance.IntermissionStopClockResetJamNumber = policy.IntermissionStopClockResetJamNumber;
                        instance.IntermissionStopClockResetJamTime = policy.IntermissionStopClockResetJamTime;
                        instance.IntermissionStopClockResetPeriodNumber = policy.IntermissionStopClockResetPeriodNumber;
                        instance.IntermissionStopClockResetPeriodTime = policy.IntermissionStopClockResetPeriodTime;
                        instance.JamClockControlsLineUpClock = policy.JamClockControlsLineUpClock;
                        instance.JamClockControlsTeamPositions = policy.JamClockControlsTeamPositions;
                        instance.PenaltyBoxControlsLeadJammer = policy.PenaltyBoxControlsLeadJammer;
                        instance.PeriodClockControlsLineupJamClock = policy.PeriodClockControlsLineupJamClock;
                        instance.SecondIntermissionNameConfirmedText = policy.SecondIntermissionNameConfirmedText;

                        instance.SecondIntermissionNameText = policy.SecondIntermissionNameText;
                        instance.TimeoutClockControlsLineupClock = policy.TimeoutClockControlsLineupClock;
                        instance.JamClockTimePerJam = policy.JamClockTimePerJam;
                        instance.LineUpClockPerJam = policy.LineUpClockPerJam;
                        instance.PeriodClock = policy.PeriodClock;
                        instance.NumberOfPeriods = policy.NumberOfPeriods;
                        instance.GameSelectionType = policy.GameSelectionType;
                        instance.TimeOutsPerPeriod = policy.TimeOutsPerPeriod;
                        instance.TimeOutClock = policy.TimeOutClock;
                        instance.DefaultScoreboardTheme = policy.DefaultScoreboardTheme;
                        instance.SkaterLineUpTheme = policy.SkaterLineUpTheme;
                        instance.ShowLatestPoints = false;
                        if (policy.VideoOverlay == null)
                            policy.VideoOverlay = new Overlay();
                        instance.VideoOverlay = policy.VideoOverlay;
                        instance.ShowActiveJammerPictures = policy.ShowActiveJammerPictures;

                        //TODO: figure out a better way to update properties when the scoreboard is updated.
                        if (policy.ISUPDATEPROPERTYNEW != ScoreboardConfig.SCOREBOARD_VERSION_NUMBER)
                        {
                        }

                        //these are the default already selectedvalues.
                        instance.StopLineUpClockAtZero = policy.StopLineUpClockAtZero;
                        instance.StopPeriodClockWhenLineUpClockHitsZero = policy.StopPeriodClockWhenLineUpClockHitsZero;
                        instance.OfficialTimeOutKeyShortcut = policy.OfficialTimeOutKeyShortcut;
                        instance.StartJamKeyShortcut = policy.StartJamKeyShortcut;
                        instance.StopJamKeyShortcut = policy.StopJamKeyShortcut;
                        instance.Team1LeadJammerKeyShortcut = policy.Team1LeadJammerKeyShortcut;
                        instance.Team1ScoreDownKeyShortcut = policy.Team1ScoreDownKeyShortcut;
                        instance.Team1ScoreUpKeyShortcut = policy.Team1ScoreUpKeyShortcut;
                        instance.Team1TimeOutKeyShortcut = policy.Team1TimeOutKeyShortcut;
                        instance.Team2LeadJammerKeyShortcut = policy.Team2LeadJammerKeyShortcut;
                        instance.Team2ScoreDownKeyShortcut = policy.Team2ScoreDownKeyShortcut;
                        instance.Team2ScoreUpKeyShortcut = policy.Team2ScoreUpKeyShortcut;
                        instance.Team2TimeOutKeyShortcut = policy.Team2TimeOutKeyShortcut;
                        instance.ShowActiveClockDuringSlideShow = policy.ShowActiveClockDuringSlideShow;
                        instance.SecondsPerSlideShowSlide = policy.SecondsPerSlideShowSlide;
                        instance.RotateSlideShowSlides = policy.RotateSlideShowSlides;
                        instance.ISUPDATEPROPERTYNEW = ScoreboardConfig.SCOREBOARD_VERSION_NUMBER;
                        if (String.IsNullOrEmpty(policy.DefaultLanguage))
                            instance.DefaultLanguage = "en-US";
                        else
                            instance.DefaultLanguage = policy.DefaultLanguage;
                        instance.LineUpTrackerControlsScore = policy.LineUpTrackerControlsScore;
                        instance.SoundBuzzerAtEndOfPeriod = policy.SoundBuzzerAtEndOfPeriod;
                        if (String.IsNullOrEmpty(policy.SoundBuzzerFileLocation))
                            instance.SoundBuzzerFileLocation = DEFAULT_BUZZER_FILE_LOCATION;
                        else
                            instance.SoundBuzzerFileLocation = policy.SoundBuzzerFileLocation;


                    }
                    catch
                    {
                        SetDefaultPolicySettings();
                    }
                    finally
                    {
                        objReader.Close();
                    }

                }
                else
                {
                    SetDefaultPolicySettings();

                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI);
            }
        }
        /// <summary>
        /// sets the default settings of the scoreboard if there are any current settings saved.
        /// </summary>
        private void SetDefaultPolicySettings()
        {
            try
            {
                //loads defaults for loading the policy for the first time.
                instance.AdChangeAutomaticallyChangeImage = true;
                //TODO: place 5 seconds somewhere else in default
                instance.AdChangeDisplayChangesInMilliSeconds = (long)TimeSpan.FromSeconds(7).TotalMilliseconds;
                instance.AdChangeShowAdsDuringIntermission = false;
                instance.AdChangeShowAdsRandomly = true;
                instance.AdChangeUseLineUpClock = true;
                instance.AlwaysShowJamClock = true;

                instance.LineupClockControlsStartJam = true;
                instance.EnableAdChange = true;
                instance.EnableIntermissionNaming = true;
                instance.EnableIntermissionStartOfClock = true;
                //TODO: place this text in the correct place when we work on languages.
                instance.FirstIntermissionNameConfirmedText = "Halftime";
                instance.FirstIntermissionNameText = "Halftime";
                instance.HideClockTimeAfterBout = true;
                instance.IntermissionOtherText = "Pre Game";
                instance.IntermissionStartOfClockInMilliseconds = (long)TimeSpan.FromMinutes(15).TotalMilliseconds;
                instance.IntermissionStopClockEnable = true;
                instance.IntermissionStopClockIncrementJamNumber = true;
                instance.IntermissionStopClockIncrementPeriodNumber = true;
                instance.IntermissionStopClockResetJamNumber = false;
                instance.IntermissionStopClockResetJamTime = true;
                instance.IntermissionStopClockResetPeriodNumber = false;
                instance.IntermissionStopClockResetPeriodTime = true;
                instance.JamClockControlsLineUpClock = true;
                instance.JamClockControlsTeamPositions = true;
                instance.PenaltyBoxControlsLeadJammer = true;
                instance.PeriodClockControlsLineupJamClock = true;
                instance.SecondIntermissionNameConfirmedText = "Final";
                instance.SecondIntermissionNameText = "UnOfficial Final";
                instance.TimeoutClockControlsLineupClock = true;
                instance.NumberOfPeriods = 2;
                instance.PeriodClock = (long)TimeSpan.FromMinutes(30).TotalMilliseconds;
                instance.JamClockTimePerJam = (long)TimeSpan.FromMinutes(2).TotalMilliseconds;
                instance.LineUpClockPerJam = (long)TimeSpan.FromSeconds(30).TotalMilliseconds;
                instance.TimeOutsPerPeriod = 3;
                instance.GameSelectionType = GameTypeEnum.WFTDA;
                instance.TimeOutClock = (long)TimeSpan.FromSeconds(60).TotalMilliseconds;
                instance.StopLineUpClockAtZero = true;
                instance.StopPeriodClockWhenLineUpClockHitsZero = false;
                instance.ShowLatestPoints = false;

                instance.DefaultScoreboardTheme = string.Empty;
                instance.SkaterLineUpTheme = string.Empty;

                instance.ShowActiveClockDuringSlideShow = true;
                instance.SecondsPerSlideShowSlide = 10;
                instance.RotateSlideShowSlides = true;
                instance.LineUpTrackerControlsScore = false;
                instance.SoundBuzzerAtEndOfPeriod = false;
                instance.SoundBuzzerFileLocation = DEFAULT_BUZZER_FILE_LOCATION;
                instance.DefaultLanguage = "en-US";
                instance.ShowActiveJammerPictures = false;
                instance.VideoOverlay = new Overlay();
                //30 pixels from the bottom
                instance.VideoOverlay.VerticalPosition = 30;
                instance.VideoOverlay.TextSizePosition = 1.0;
                setDefaultKeyboardShortcuts();

                changeGameSelectionType(GameTypeEnum.WFTDA);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        public void CompressBuzzer()
        {
            try
            {
                if (!String.IsNullOrEmpty(PolicyViewModel.Instance.SoundBuzzerFileLocation) && !PolicyViewModel.Instance.SoundBuzzerFileLocation.Contains(PolicyViewModel.DEFAULT_BUZZER_FILE_LOCATION) && File.Exists(PolicyViewModel.Instance.SoundBuzzerFileLocation))
                {
                    var memStream = new MemoryStream();
                    FileStream fs = File.OpenRead(PolicyViewModel.Instance.SoundBuzzerFileLocation);
                    fs.CopyTo(memStream);
                    PolicyViewModel.Instance.BuzzerCompressed = memStream.ToArray();
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried to save buzzer game", LoggerEnum.message);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
                //ErrorViewModel.saveError(exception);
            }
        }

        public void LoadBuzzerFromCompression()
        {
            
                if (!String.IsNullOrEmpty(instance.SoundBuzzerFileLocation) && File.Exists(instance.SoundBuzzerFileLocation))
                {
                    instance.BuzzerCompressed = null;
                }
                else if (instance.BuzzerCompressed != null && !instance.SoundBuzzerFileLocation.Contains(DEFAULT_BUZZER_FILE_LOCATION) && !String.IsNullOrEmpty(instance.SoundBuzzerFileLocation))
                {
                    try
                    {
                        FileInfo tempFile = new FileInfo(instance.SoundBuzzerFileLocation);
                        string destinationFilePath = ScoreboardConfig.SAVE_FILES_FOLDER;
                        string destinationFileName = tempFile.Name;

                        string saveLocation = Path.Combine(destinationFilePath, destinationFileName);

                        using (MemoryStream stream = new MemoryStream(instance.BuzzerCompressed))
                        {
                            using (FileStream fileStream = new FileStream(saveLocation, FileMode.Create, System.IO.FileAccess.Write))
                            {
                                byte[] bytes = new byte[stream.Length];
                                stream.Read(bytes, 0, (int)stream.Length);
                                fileStream.Write(bytes, 0, bytes.Length);
                                stream.Close();
                            }
                            instance.SoundBuzzerFileLocation = saveLocation;
                        }
                    }
                    catch (Exception e)
                    {
                        ErrorViewModel.Save(e, e.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                    }
                }
                instance.BuzzerCompressed = null;
            
        }

        private static void LoadPictureOfLogoForVideoOverlay()
        {
            if (!String.IsNullOrEmpty(instance.VideoOverlay.LogoPictureLocation))
            {
                if (!String.IsNullOrEmpty(instance.VideoOverlay.LogoPictureLocation) && File.Exists(instance.VideoOverlay.LogoPictureLocation))
                {
                    instance.VideoOverlay.LogoPictureCompressed = null;
                }
                else if (instance.VideoOverlay.LogoPictureCompressed != null && !String.IsNullOrEmpty(instance.VideoOverlay.LogoPictureLocation))
                {
                    try
                    {
                        FileInfo file = new FileInfo(instance.VideoOverlay.LogoPictureLocation);
                        using (var tmpImage = Scoreboard.Library.Util.Imaging.ByteToImage(instance.VideoOverlay.LogoPictureCompressed))
                        {

                            string destinationFilePath = ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG;
                            string destinationFileName = file.Name;

                            string saveLocation = Path.Combine(destinationFilePath, destinationFileName);
                            tmpImage.Save(saveLocation);
                            instance.VideoOverlay.LogoPictureLocation = saveLocation;
                        }
                    }
                    catch (Exception e)
                    {
                        ErrorViewModel.Save(e, e.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                    }
                }
                instance.VideoOverlay.LogoPictureCompressed = null;
            }
        }

        private static void setDefaultKeyboardShortcuts()
        {
            try
            {
                instance.OfficialTimeOutKeyShortcut = 'e';
                instance.StartJamKeyShortcut = 'q';
                instance.StopJamKeyShortcut = 'w';
                instance.Team1LeadJammerKeyShortcut = 'x';
                instance.Team1ScoreDownKeyShortcut = 'z';
                instance.Team1ScoreUpKeyShortcut = 'a';
                instance.Team1TimeOutKeyShortcut = 's';
                instance.Team2LeadJammerKeyShortcut = '.';
                instance.Team2ScoreDownKeyShortcut = '/';
                instance.Team2ScoreUpKeyShortcut = '\'';
                instance.Team2TimeOutKeyShortcut = ';';
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
        }

        public void ResetSoundBuzzer()
        {
            PolicyViewModel.Instance.SoundBuzzerFileLocation = @"Resources/buzzer.mp3";
            savePolicyToXml();
        }

        /// <summary>
        /// 
        /// </summary>
        public void savePolicyToXml(bool SaveBuzzer = false, bool SaveVideoOverlayPicture = false)
        {
            var saveGameTask = Task<bool>.Factory.StartNew(
                () =>
                {
                    try
                    {
                        if (PolicyViewModel.Instance.VideoOverlay != null && !string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.LogoPictureLocation))
                        {
                            PolicyViewModel.Instance.VideoOverlay.LogoPictureCompressed = GameViewModel.SavePictureOfMemberToXmlFile(PolicyViewModel.Instance.VideoOverlay.LogoPictureLocation);
                        }
                        if (SaveBuzzer)
                            CompressBuzzer();

                        GameViewModel.Instance.Policy = PolicyViewModel.Instance;
                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(Instance.GetType());
                        System.IO.StreamWriter file = new System.IO.StreamWriter(ScoreboardConfig.SAVE_POLICY_FILE);

                        writer.Serialize(file, Instance);
                        file.Close();

                        PolicyViewModel.Instance.VideoOverlay.LogoPictureCompressed = null;
                    }
                    catch (Exception exception)
                    {
                        ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
                    }
                    return true;
                });

            //if (saveGameTask.Result)
            //{

            //}
        }

        public string GetStringValue(string key)
        {
            var doc = XDocument.Load("dic");
            return doc.Element("settings") == null || doc.Element("settings").Element(key) == null ? string.Empty : doc.Element("settings").Element(key).Value;
        }

        public bool GetBoolValue(string key)
        {
            var doc = XDocument.Load("dic");
            return Convert.ToBoolean(doc.Element("settings") == null || doc.Element("settings").Element(key) == null ? "False" : doc.Element("settings").Element(key).Value);
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
