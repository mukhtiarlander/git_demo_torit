using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using RDN.Utilities.Error;
using Scoreboard.Library.Classes;
using Scoreboard.Library.Static.Enums;
using Scoreboard.Library.Static;
using Scoreboard.Library.StopWatch;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Timers;
using System.Net;

using System.Threading;
using RDN.Utilities.Version;
using RDN.Utilities.Config;
using System.Web.Script.Serialization;
using Scoreboard.Library.Models;
using System.Drawing;
using Scoreboard.Library.ViewModel.Members;
using Scoreboard.Library.ViewModel.Officials;
using Scoreboard.Library.ViewModel.Members.Enums;
using Scoreboard.Library.ViewModel.Members.Officials;
using Scoreboard.Library.ViewModel.ClockView;
using Scoreboard.Library.Network;
using RDN.Utilities.Util;
using Scoreboard.Library.Util;
using RDN.Portable.Classes.Team;

namespace Scoreboard.Library.ViewModel
{


    public enum GameViewModelEnum { DisplayType, GameName, CurrentPeriod, CurrentTeam1Score, CurrentTeam2Score, CurrentTeam1JamScore, CurrentTeam2JamScore, CurrentAdvertisement, HasGameEnded, IsGameOnline, PublishGameOnline, SaveGameOnline, CurrentIntermission, CurrentSlideShowSlide, UrlForAdministeringGameOnline, NameOfIntermission, UsingServerScoring }

    public enum GameViewModelIntermissionTypeEnum { PreGameIntermission, HalfTimeIntermission, PostGameIntermission }

    public enum GameViewModelIsOnlineEnum { IsOffline = 0, IsSending = 1, IsOnline = 2, InternetProblem = 3 }

    public enum GameVideoTypeEnum
    {
        NoVideo = 0, SilverLightLive = 1, EmbededVideo = 2
    }

    /// <summary>
    /// enum for deciding which display to currently show to the fans
    /// </summary>
    public enum GameViewModelDisplayTypeEnum { Scoreboard, SlideShowForIntroductions, SlideShowForAds }
    /// <summary>
    /// debug mode is only open for developers. users and fans never see this mode.
    /// </summary>
    public enum ScoreboardModeEnum { Debug = 0, Live = 1, AddingOldGame = 2, AddedOldGame = 3 }
    public class GameViewModel : INotifyPropertyChanged
    {


        public event PropertyChangedEventHandler PropertyChanged;

        static GameViewModel instance = new GameViewModel();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static GameViewModel()
        { }

        public static GameViewModel Instance
        {
            get
            {
                return instance;
            }
        }
        public ScoreboardModeEnum ScoreboardMode { get; set; }

        /// <summary>
        /// the policy set for the game.
        /// </summary>
        public PolicyViewModel Policy { get; set; }
        private bool _isBeingTested = false;
        /// <summary>
        /// checks if user is just testing the scoreboard.  Nothing of this will be saved online.
        /// </summary>
        public bool IsBeingTested
        {
            get { return _isBeingTested; }
            set
            {
                _isBeingTested = value;
                OnPropertyChanged("IsBeingTested");
            }
        }
        private GameViewModelDisplayTypeEnum _displayType = GameViewModelDisplayTypeEnum.Scoreboard;
        /// <summary>
        /// the type described to show users during the games.  
        /// </summary>
        public GameViewModelDisplayTypeEnum DisplayType
        {
            get { return _displayType; }
            set
            {
                _displayType = value;
                OnPropertyChanged("DisplayType");
            }
        }
        /// <summary>
        /// the ID of the Federation that owns this game if any...
        /// </summary>
        public Guid FederationId { get; set; }
        public string FederationName { get; set; }

        public Guid TournamentId { get; set; }
        public string TournamentName { get; set; }
        public long PaywallId { get; set; }
        private bool _publishGameOnline;
        /// <summary>
        /// checks if user is allowing this to be a live game
        /// </summary>
        public bool PublishGameOnline
        {
            get { return _publishGameOnline; }
            set
            {
                _publishGameOnline = value;
                OnPropertyChanged("PublishGameOnline");
            }
        }
        /// <summary>
        /// if the game is actually confirmed published online by the DB.
        /// </summary>
        public bool GameIsConfirmedOnline = false;

        private bool _saveGameOnline = true;
        /// <summary>
        /// checks if user is saving this game online.
        /// </summary>
        public bool SaveGameOnline
        {
            get { return _saveGameOnline; }
            set
            {
                _saveGameOnline = value;
                OnPropertyChanged("SaveGameOnline");
            }
        }

        private GameViewModelIsOnlineEnum _isGameOnline = GameViewModelIsOnlineEnum.IsOffline;
        /// <summary>
        /// shows status that game is currently live online or if it is connecting.
        /// </summary>
        public GameViewModelIsOnlineEnum IsGameOnline
        {
            get { return _isGameOnline; }
            set
            {
                _isGameOnline = value;
                OnPropertyChanged("IsGameOnline");
            }

        }
        /// <summary>
        /// checks whether a send is currently taking place
        /// </summary>
        public bool IsCurrentlySendingOnlineGame { get; set; }

        /// <summary>
        /// MajorUpgrades.MinorUpgrades.BugFixes
        /// </summary>
        public string VersionNumber { get { return ScoreboardConfig.SCOREBOARD_VERSION_NUMBER; } }
        public bool HasGameStarted { get; set; }
        /// <summary>
        /// used to store the last modified value from the Database.
        /// </summary>
        public DateTime LastModified { get; set; }
        private bool _hasGameEnded;
        public bool HasGameEnded
        {
            get { return _hasGameEnded; }
            set
            {
                _hasGameEnded = value;
                OnPropertyChanged("HasGameEnded");
            }
        }
        /// <summary>
        /// flag for the fact there is video of the game.  Whether it be live or embeded.
        /// </summary>
        public GameVideoTypeEnum IsThereVideoOfGame { get; set; }
        public string StreamingUrlOfVideo { get; set; }
        public string StreamingUrlOfVideoMobile { get; set; }
        public string EmbededVideoHtml { get; set; }
        public string SelectedShop { get; set; }

        public bool CurrentlyInTimeOut { get; set; }

        //public StopwatchWrapper GameClock { get; set; }

        public StopwatchWrapper CurrentLineUpClock { get; set; }
        public StopwatchWrapper PeriodClock { get; set; }
        public StopwatchWrapper CurrentTimeOutClock { get; set; }
        public StopwatchWrapper IntermissionClock { get; set; }
        public StopwatchWrapper SlideShowClock { get; set; }
        //public StopwatchWrapper CountDownClock { get; set; }
        //public StopwatchWrapper AdvertisementClock { get; set; }

        public List<EditModeModel> EditModeItems { get; set; }
        //public List<StopwatchWrapper> PenaltyBoxClock { get; set; }
        public List<TimeOutViewModel> TimeOuts { get; set; }
        public List<ScoreViewModel> ScoresTeam1 { get; set; }
        public List<ScoreViewModel> ScoresTeam2 { get; set; }
        public List<AdvertisementViewModel> Advertisements { get; set; }
        public List<SlideShowViewModel> SlideShowSlides { get; set; }

        public List<AssistViewModel> AssistsForTeam1 { get; set; }
        public List<AssistViewModel> AssistsForTeam2 { get; set; }
        public List<BlockViewModel> BlocksForTeam1 { get; set; }
        public List<BlockViewModel> BlocksForTeam2 { get; set; }
        public List<PenaltyViewModel> PenaltiesForTeam1 { get; set; }
        public List<PenaltyViewModel> PenaltiesForTeam2 { get; set; }

        public List<OfficialReviewViewModel> OfficialReviews { get; set; }
        public Officials.Officials Officials { get; set; }
        public ScoreboardSettings ScoreboardSettings { get; set; }

        private string _nameOfIntermission;
        /// <summary>
        /// the current slideshow displaying on the scoreboard.
        /// </summary>
        public string NameOfIntermission
        {
            get { return _nameOfIntermission; }
            set
            {
                _nameOfIntermission = value;
                OnPropertyChanged("NameOfIntermission");
            }
        }
        private bool _usingServerScoring;
        /// <summary>
        /// boolean to know if the scorings for the game are happening with the tablets and web server.
        /// </summary>
        public bool UsingServerScoring
        {
            get { return _usingServerScoring; }
            set
            {
                _usingServerScoring = value;
                OnPropertyChanged("UsingServerScoring");
            }
        }

        /// <summary>
        /// links the user can add for the game online so users can click on them.
        /// </summary>
        public List<GameLinkViewModel> GameLinks { get; set; }

        private SlideShowViewModel _currentSlideShowSlide;
        /// <summary>
        /// the current slideshow displaying on the scoreboard.
        /// </summary>
        public SlideShowViewModel CurrentSlideShowSlide
        {
            get { return _currentSlideShowSlide; }
            set
            {
                _currentSlideShowSlide = value;
                OnPropertyChanged("CurrentSlideShowSlide");
            }
        }


        private AdvertisementViewModel _currentAdvertisement;
        /// <summary>
        /// the current advertisement displaying on the scoreboard.
        /// </summary>
        public AdvertisementViewModel CurrentAdvertisement
        {
            get { return _currentAdvertisement; }
            set
            {
                _currentAdvertisement = value;
                OnPropertyChanged("CurrentAdvertisement");
            }
        }
        private bool _isInEditMode { get; set; }
        public bool IsInEditMode
        {
            get { return _isInEditMode; }
            set
            {
                _isInEditMode = value;
                OnPropertyChanged("IsInEditMode");
            }
        }

        /// <summary>
        /// current jam score for the current jam.  gets erased on each new jam.
        /// </summary>
        private int _currentTeam1JamScore;
        public int CurrentTeam1JamScore
        {
            get { return _currentTeam1JamScore; }
            set
            {
                _currentTeam1JamScore = value;
                OnPropertyChanged("CurrentTeam1JamScore");
            }
        }
        /// <summary>
        /// current jam score for the current jam.  gets erased on each new jam.
        /// </summary>
        private int _currentTeam2JamScore;
        public int CurrentTeam2JamScore
        {
            get { return _currentTeam2JamScore; }
            set
            {
                _currentTeam2JamScore = value;
                OnPropertyChanged("CurrentTeam2JamScore");
            }
        }

        private int _currentTeam1Score;
        public int CurrentTeam1Score
        {
            get { return _currentTeam1Score; }
            set
            {
                _currentTeam1Score = value;
                OnPropertyChanged("CurrentTeam1Score");
            }
        }
        private int _currentTeam2Score;
        public int CurrentTeam2Score
        {
            get { return _currentTeam2Score; }
            set
            {
                _currentTeam2Score = value;
                OnPropertyChanged("CurrentTeam2Score");
            }
        }

        public Guid GameId { get; set; }
        /// <summary>
        /// we use this Id to access this game online privately.  
        /// Womever has this id, will have access to this game in a restricted environment.
        /// </summary>
        public Guid IdForOnlineManagementUse { get; set; }
        private string _urlForAdministeringGameOnline { get; set; }
        /// <summary>
        /// this is used to store the url for administering the game online.
        /// </summary>
        public string UrlForAdministeringGameOnline
        {
            get { return _urlForAdministeringGameOnline; }
            set
            {
                _urlForAdministeringGameOnline = value;
                OnPropertyChanged("UrlForAdministeringGameOnline");
            }
        }

        private string _gameName;
        public string GameName
        {
            get { return _gameName; }
            set
            {
                _gameName = value;
                OnPropertyChanged("GameName");
            }
        }
        public string GameLocation { get; set; }
        public string GameCity { get; set; }
        public string GameState { get; set; }

        public DateTime GameDate { get; set; }
        public DateTime GameEndDate { get; set; }
        /// <summary>
        /// game type enum WFTDA, MADE, Etc.
        /// </summary>
        // public GameTypeEnum GameType { get; set; }

        /// <summary>
        /// elapsed time on the game clock.
        /// </summary>
        public long ElapsedTimeGameClockMilliSeconds { get; set; }

        private TeamViewModel _team1 = new TeamViewModel();
        private TeamViewModel _team2 = new TeamViewModel();

        public TeamViewModel Team1
        {
            get { return _team2; }
            set { _team2 = value; }
        }

        public TeamViewModel Team2
        {
            get { return _team1; }
            set { _team1 = value; }
        }

        public TeamViewModel Team1AttachedToGame { get; set; }
        public TeamViewModel Team2AttachedToGame { get; set; }


        private ObservableCollection<JamViewModel> _jams = new ObservableCollection<JamViewModel>();
        /// <summary>
        /// this is the list of jams posted for the games.
        /// </summary>
        public ObservableCollection<JamViewModel> Jams
        {
            get { return _jams; }
            set { _jams = value; }
        }

        private ObservableCollection<SkaterInPenaltyBoxViewModel> _penaltyBox = new ObservableCollection<SkaterInPenaltyBoxViewModel>();
        public ObservableCollection<SkaterInPenaltyBoxViewModel> PenaltyBox
        {
            get { return _penaltyBox; }
            set { _penaltyBox = value; }
        }

        private JamViewModel _currentJam;
        public JamViewModel CurrentJam
        {
            get { return _currentJam; }
            set { _currentJam = value; }
        }

        private int _currentPeriod;
        public int CurrentPeriod
        {
            get { return _currentPeriod; }
            set
            {
                _currentPeriod = value;
                OnPropertyChanged("CurrentPeriod");
            }
        }
        private GameViewModelIntermissionTypeEnum _currentIntermission;
        public GameViewModelIntermissionTypeEnum CurrentIntermission
        {
            get { return _currentIntermission; }
            set
            {
                _currentIntermission = value;
                OnPropertyChanged("CurrentIntermission");
            }
        }

        public void setOfficialStartTime()
        {
            try
            {
                instance.GameDate = DateTime.UtcNow;
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        /// <summary>
        /// starts the jam for the game
        /// </summary>
        public void startJam()
        {
            try
            {
                instance.HasGameStarted = true;

                if (GameViewModel.instance.PeriodClock.IsPaused && PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.TEXAS_DERBY)
                    GameViewModel.Instance.PeriodClock.Resume();
                else
                    GameViewModel.Instance.startPeriod();
                //we are checking if the period was paused, because of the policy allowing the lineup clock to dictate if the period clock is stopped when the line up clock hits zero.
                if (GameViewModel.Instance.PeriodClock.IsPaused)
                    GameViewModel.Instance.PeriodClock.Resume();
                stopTimeOut();
                instance.CurrentJam.JamClock.Start();
                instance.CurrentLineUpClock.Stop();
                instance.CurrentJam.LineUpClockAfterJam = instance.CurrentLineUpClock;
                instance.CurrentTeam1JamScore = 0;
                instance.CurrentTeam2JamScore = 0;
                this.StartJam(this, null);
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }


        /// <summary>
        /// saves the current jam to the history
        /// </summary>
        public void saveJam()
        {
            try
            {
                instance.Jams.Add(instance.CurrentJam);
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }



        /// <summary>
        /// creates and sets up a new jam.
        /// </summary>
        public void createNewJam(bool afterIntermission = false)
        {
            ///checks if we are playing the WFTDA rule set.  If we are, then we need to reset the jam
            ///number after the intermission.
            if (afterIntermission && instance.CurrentJam != null && (PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.WFTDA_2010 || PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.WFTDA))
                instance.CurrentJam = new JamViewModel(1, instance.ElapsedTimeGameClockMilliSeconds, instance.CurrentPeriod);
            else if (instance.CurrentJam != null)
                instance.CurrentJam = new JamViewModel(instance.CurrentJam.JamNumber + 1, instance.ElapsedTimeGameClockMilliSeconds, instance.CurrentPeriod);
            else //if its the first time we add a jam.
                instance.CurrentJam = new JamViewModel(instance.Jams.Count + 1, instance.ElapsedTimeGameClockMilliSeconds, instance.CurrentPeriod);

            instance.CurrentJam.JamClock.PropertyChanged += new PropertyChangedEventHandler(JamClock_PropertyChanged);

            this.NewJam(this, null);
        }

        void JamClock_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == StopWatchWrapperEnum.IsClockAtZero.ToString())
            {
                if (instance.CurrentJam.JamClock.IsClockAtZero)
                {
                    instance.stopJam();
                }
            }
        }

        /// <summary>
        /// starts the period for the game if it isn't currently running.
        /// </summary>
        public void startPeriod()
        {
            if (!instance.PeriodClock.IsRunning)
                instance.PeriodClock.Start();

        }
        /// <summary>
        /// ends the period for the game.  Should be called when period should be stopped.
        /// </summary>
        public void endPeriod()
        {
            try
            {
                instance.PeriodClock.Stop();
                stopTimeOut();
                //MADE RULE SET requires the jam end when the period ends.
                if (PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.TEXAS_DERBY || PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.MADE || PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.MADE_COED)
                {
                    stopJam();

                }
                instance.CurrentLineUpClock.Stop();
                GameViewModel.Instance.saveJam();
                //if the period clock has started, no need to stay in pre game.
                // instance.createNewIntermission();
                if (instance.CurrentIntermission == GameViewModelIntermissionTypeEnum.PreGameIntermission)
                {
                    instance.CurrentIntermission = GameViewModelIntermissionTypeEnum.HalfTimeIntermission;
                    instance.NameOfIntermission = PolicyViewModel.Instance.FirstIntermissionNameText;
                }
                Logger.Instance.logMessage("Period is Over", LoggerEnum.message);
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        public void startIntermission()
        {
            instance.IntermissionClock.Start();

            this.NewIntermission(this, null);
            Logger.Instance.logMessage("Start Intermission", LoggerEnum.message);
        }
        /// <summary>
        /// ends the intermission
        /// </summary>
        public void endIntermission()
        {
            try
            {
                instance.IntermissionClock.Stop();
                //if one intermission has gone by, then we move to halftime.
                //if the second intermission has gone by, then we move to post game.
                if (instance.CurrentIntermission == GameViewModelIntermissionTypeEnum.PreGameIntermission)
                {
                    instance.CurrentIntermission = GameViewModelIntermissionTypeEnum.HalfTimeIntermission;
                    instance.NameOfIntermission = PolicyViewModel.Instance.FirstIntermissionNameText;
                }
                else if (instance.CurrentIntermission == GameViewModelIntermissionTypeEnum.HalfTimeIntermission)
                {
                    instance.CurrentIntermission = GameViewModelIntermissionTypeEnum.PostGameIntermission;
                    instance.NameOfIntermission = PolicyViewModel.Instance.SecondIntermissionNameText;
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        public void endGame()
        {
            try
            {
                instance.endPeriod();
                if (instance._onlineGameTimer != null)
                    instance._onlineGameTimer.Stop();
                instance.HasGameEnded = true;
                instance.GameEndDate = DateTime.UtcNow;

                sendCompletedGameToServer();
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }


        /// <summary>
        /// starts from a time out mode.
        /// </summary>
        public void startFromTimeOut()
        {
            try
            {
                if (instance.CurrentlyInTimeOut)
                {
                    instance.CurrentlyInTimeOut = false;
                    instance.PeriodClock.Resume();

                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        /// <summary>
        /// creates a new time out
        /// </summary>
        public void createTimeOutClock(TimeOutTypeEnum type)
        {
            if (type == TimeOutTypeEnum.Team1 || TimeOutTypeEnum.Team2 == type)
            {
                instance.CurrentTimeOutClock = new StopwatchWrapper(PolicyViewModel.Instance.TimeOutClock);
                this.NewTimeOut(this, null);
            }
            else if (type == TimeOutTypeEnum.Offical)
            {
                instance.CurrentTimeOutClock = new StopwatchWrapper(0);
                this.NewTimeOut(this, null);
            }
            else if (type == TimeOutTypeEnum.Official_Review)
            {
                instance.CurrentTimeOutClock = new StopwatchWrapper(0);
                this.OnNewOfficialReview(this, null);
            }

        }
        /// <summary>
        /// starts a time out
        /// </summary>
        /// <param name="type"></param>
        public void startTimeOut(TimeOutTypeEnum type)
        {
            try
            {
                if (instance.CurrentJam != null)
                    instance.CurrentJam.JamClock.Stop();
                instance.PeriodClock.Pause();
                instance.CurrentLineUpClock.Stop();
                instance.CurrentlyInTimeOut = true;

                instance._team1.clearTeamPositions();
                instance._team2.clearTeamPositions();

                TimeOutViewModel timeOut = new TimeOutViewModel(instance.CurrentTimeOutClock, type);
                instance.CurrentTimeOutClock.Start();
                if (type == TimeOutTypeEnum.Team1)
                    instance.Team1.TimeOutsLeft -= 1;
                else if (type == TimeOutTypeEnum.Team2)
                    instance.Team2.TimeOutsLeft -= 1;

                instance.TimeOuts.Add(timeOut);

            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }
        /// <summary>
        /// stops the current time out if any
        /// </summary>
        public void stopTimeOut()
        {
            if (instance.CurrentTimeOutClock != null && instance.CurrentTimeOutClock.IsRunning)
            {
                instance.CurrentTimeOutClock.Stop();
            }
        }
        public void stopIntermission()
        {
            if (instance.IntermissionClock != null && instance.IntermissionClock.IsRunning)
            {
                instance.IntermissionClock.Stop();
            }
        }

        public void stopJam(bool didJammerT1StopJam = false, bool didJammerT2StopJam = false, bool didJamEndwithInjury = false)
        {
            try
            {
                if (PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.TEXAS_DERBY)
                    instance.PeriodClock.Pause();

                _team1.clearTeamPositions();
                _team2.clearTeamPositions();

                instance.CurrentJam.JamClock.Stop();
                instance.CurrentJam.DidJamGetCalledByJammerT1 = didJammerT1StopJam;
                instance.CurrentJam.DidJamGetCalledByJammerT2 = didJammerT2StopJam;
                instance.CurrentJam.DidJamEndWithInjury = didJamEndwithInjury;


                this.StopJam(this, null);
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        public void createLineUpClock()
        {
            try
            {
                instance.CurrentLineUpClock = new StopwatchWrapper(PolicyViewModel.Instance.LineUpClockPerJam);
                instance.CurrentLineUpClock.PropertyChanged += new PropertyChangedEventHandler(CurrentLineUpClock_PropertyChanged);
                this.NewLineUp(this, null);
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }
        /// <summary>
        /// hits when the line up clock property changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CurrentLineUpClock_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == StopWatchWrapperEnum.IsClockAtZero.ToString())
                {
                    if (instance.CurrentLineUpClock.IsClockAtZero)
                    {
                        if (PolicyViewModel.Instance.StopLineUpClockAtZero)
                            instance.CurrentLineUpClock.Stop();

                        if (PolicyViewModel.Instance.StopPeriodClockWhenLineUpClockHitsZero)
                            pausePeriodClock();
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        public void pausePeriodClock()
        {
            instance.PeriodClock.Pause();
        }

        /// <summary>
        /// starts the line up clock counting.
        /// </summary>
        public void startLineUpClock()
        {
            try
            {
                instance.CurrentLineUpClock.Start();
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }
        /// <summary>
        /// creates a new period for the game
        /// </summary>
        public void createNewPeriod()
        {
            try
            {
                GameViewModel.instance.CurrentPeriod += 1;
                GameViewModel.Instance.PeriodClock = new StopwatchWrapper(PolicyViewModel.Instance.PeriodClock);
                GameViewModel.Instance.PeriodClock.PropertyChanged += new PropertyChangedEventHandler(PeriodClock_PropertyChanged);

                //WFTDA only allows time outs per game, not actually the period.
                if (PolicyViewModel.Instance.GameSelectionType != GameTypeEnum.WFTDA || PolicyViewModel.Instance.GameSelectionType != GameTypeEnum.USARS || PolicyViewModel.Instance.GameSelectionType != GameTypeEnum.RDCL)
                {
                    GameViewModel.Instance.Team1.TimeOutsLeft = PolicyViewModel.Instance.TimeOutsPerPeriod;
                    GameViewModel.Instance.Team2.TimeOutsLeft = PolicyViewModel.Instance.TimeOutsPerPeriod;
                }
                this.NewPeriod(this, null);
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }


        /// <summary>
        /// creates a new intermission clock.
        /// </summary>
        public void createNewIntermission()
        {
            try
            {
                if (GameViewModel.Instance.IntermissionClock == null || !GameViewModel.Instance.IntermissionClock.IsRunning)
                {
                    GameViewModel.Instance.IntermissionClock = new StopwatchWrapper(PolicyViewModel.Instance.IntermissionStartOfClockInMilliseconds);
                    GameViewModel.Instance.IntermissionClock.PropertyChanged += new PropertyChangedEventHandler(IntermissionClock_PropertyChanged);
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        public Guid createOfficialReview()
        {
            OfficialReviewViewModel or = new OfficialReviewViewModel();
            try
            {
                GameViewModel.Instance.OfficialReviews.Add(or);
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
            return or.OfficialReviewId;
        }

        void IntermissionClock_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (StopWatchWrapperEnum.IsClockAtZero.ToString() == e.PropertyName)
                {
                    if (instance.IntermissionClock.IsClockAtZero)
                    {
                        endIntermission();
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        /// <summary>
        /// creates a new instance and overall a brand new game.
        /// </summary>
        public void createNewGame(GameTypeEnum gameType)
        {
            try
            {
                //we need to save the settings from the initial window that the user selected.
                bool saveGameOnline = GameViewModel.Instance.SaveGameOnline;
                bool publishGame = GameViewModel.Instance.PublishGameOnline;
                bool beingTested = GameViewModel.Instance.IsBeingTested;

                instance = new GameViewModel();
#if DEBUG
                instance.ScoreboardMode = ScoreboardModeEnum.Debug;
#else
                instance.ScoreboardMode = ScoreboardModeEnum.Live;
#endif
                instance.SaveGameOnline = saveGameOnline;
                instance.PublishGameOnline = publishGame;
                instance.IsBeingTested = beingTested;

                instance.GameId = Guid.NewGuid();
                instance.IdForOnlineManagementUse = Guid.NewGuid();

                instance.Policy = PolicyViewModel.Instance;
                instance.Team1.TimeOutsLeft = PolicyViewModel.Instance.TimeOutsPerPeriod;
                instance.Team2.TimeOutsLeft = PolicyViewModel.Instance.TimeOutsPerPeriod;
                //must start with a negative one for pre game intermissions...
                //should prob do this a better way, but havent figured it out yet.
                GameViewModel.Instance.CurrentPeriod = 0;

                //TODO: Team names should be different?
                instance.Team1.TeamName = "Home";
                instance.CurrentTeam1Score = 0;
                instance.CurrentTeam1JamScore = 0;
                instance.ScoresTeam1 = new List<ScoreViewModel>();
                instance.Team1.TeamId = Guid.NewGuid();
                instance.Team2.TeamName = "Away";

                instance.Team1.Logo = new TeamLogo();
                instance.Team2.Logo = new TeamLogo();
                instance.CurrentTeam2Score = 0;
                instance.CurrentTeam2JamScore = 0;
                instance.ScoresTeam2 = new List<ScoreViewModel>();
                instance.Team2.TeamId = Guid.NewGuid();
                instance.GameDate = DateTime.UtcNow;
                instance.TimeOuts = new List<TimeOutViewModel>();
                instance.GameName = "Bout";
                instance.CurrentIntermission = GameViewModelIntermissionTypeEnum.PreGameIntermission;
                instance.NameOfIntermission = PolicyViewModel.Instance.IntermissionOtherText;
                createNewPeriod();
                createLineUpClock();
                instance.Jams.Clear();
                instance.CurrentJam = null;
                createNewJam();
                createNewIntermission();
                instance.Advertisements = new List<AdvertisementViewModel>();
                instance.SlideShowSlides = new List<SlideShowViewModel>();
                AdvertisementViewModel.getAdvertsFromDirectory();
                setupAdvertisements();
                setCurrentAdvertisement();
                _team1.clearTeamPositions();
                _team2.clearTeamPositions();
                setupTimerForOnlineGame();
                setupTimerForSlideShow();
                instance.EditModeItems = new List<EditModeModel>();
                instance.AssistsForTeam1 = new List<AssistViewModel>();
                instance.BlocksForTeam1 = new List<BlockViewModel>();
                instance.PenaltiesForTeam1 = new List<PenaltyViewModel>();
                instance.AssistsForTeam2 = new List<AssistViewModel>();
                instance.BlocksForTeam2 = new List<BlockViewModel>();
                instance.PenaltiesForTeam2 = new List<PenaltyViewModel>();
                instance.OfficialReviews = new List<OfficialReviewViewModel>();
                instance.UsingServerScoring = false;
                instance.Officials = new Officials.Officials();
                instance.ScoreboardSettings = new ScoreboardSettings();
                liveGameTimer_Elapsed(this, null);
                this.NewGame(this, null);
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        private System.Timers.Timer _slideShowTimer;
        /// <summary>
        /// the timer for the slide show.  Setups a brand new one and starts it.
        /// </summary>
        public void setupTimerForSlideShow()
        {
            try
            {
                if (instance._slideShowTimer != null)
                {
                    instance._slideShowTimer.Stop();
                    instance._slideShowTimer.Close();
                    instance._slideShowTimer.Dispose();
                }
                instance._slideShowTimer = new System.Timers.Timer();
                instance._slideShowTimer.Elapsed += new ElapsedEventHandler(_slideShowTimer_Elapsed);
                //converting milliseconds into seconds with default to 10 second interval
                instance._slideShowTimer.Interval = PolicyViewModel.Instance.SecondsPerSlideShowSlide * 1000;
                instance._slideShowTimer.Enabled = true;
                instance._slideShowTimer.AutoReset = false;
                instance._slideShowTimer.Start();
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }

        }
        /// <summary>
        /// every time the slide show elapsed is hit, we change the slide
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _slideShowTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (instance.SlideShowSlides != null && instance.SlideShowSlides.Count > 0 && PolicyViewModel.Instance.RotateSlideShowSlides)
                {
                    setNewSlideShowSlide();
                }
                setupTimerForSlideShow();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        private static void setNewSlideShowSlide()
        {
            try
            {
                //if there is no slide, we go ahead and add the first one

                var slides = instance.SlideShowSlides.Where(x => x.IsShowing).ToList();

                if (instance.CurrentSlideShowSlide == null && slides.Count > 0)
                {
                    var slide = slides[0];
                    if (slide != null)
                        instance.CurrentSlideShowSlide = slide;
                }
                else
                {
                    var numOfSlides = slides.Count;
                    for (int i = 0; i < numOfSlides; i++)
                    {
                        //once we find the slide, we set the next slide.
                        if (slides[i] == instance.CurrentSlideShowSlide)
                        {
                            if ((i + 1) == numOfSlides)
                                instance.CurrentSlideShowSlide = slides[0];
                            else
                                instance.CurrentSlideShowSlide = slides[i + 1];
                            break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
        }



        private System.Timers.Timer _onlineGameTimer;
        private void setupTimerForOnlineGame()
        {
            try
            {
                if (instance._onlineGameTimer != null)
                {
                    instance._onlineGameTimer.Stop();
                    instance._onlineGameTimer.Close();
                }

                instance._onlineGameTimer = new System.Timers.Timer();
                instance._onlineGameTimer.Elapsed += new ElapsedEventHandler(liveGameTimer_Elapsed);
                instance._onlineGameTimer.Interval = ScoreboardConfig.SEND_LIVE_GAMES_MILLISECOND_INTERVAL;
                instance._onlineGameTimer.Enabled = true;
                instance._onlineGameTimer.Start();
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }

        }

        void liveGameTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (this.PublishGameOnline || this.SaveGameOnline)
                {
                    sendLiveGameToServer();
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI);
            }
        }
        private System.Timers.Timer _advertisementTimer;
        private void setupAdvertisements()
        {
            try
            {
                setCurrentAdvertisement();

                instance._advertisementTimer = new System.Timers.Timer();
                instance._advertisementTimer.Elapsed += new ElapsedEventHandler(advertTimer_Elapsed);
                if (PolicyViewModel.Instance.AdChangeDisplayChangesInMilliSeconds > 0)
                    instance._advertisementTimer.Interval = PolicyViewModel.Instance.AdChangeDisplayChangesInMilliSeconds;
                else
                    instance._advertisementTimer.Interval = 10;
                instance._advertisementTimer.Enabled = true;
                instance._advertisementTimer.Start();
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }
        /// <summary>
        /// sets the current advertisement
        /// </summary>
        private void setCurrentAdvertisement()
        {
            try
            {
                if (instance.Advertisements != null)
                {
                    if (instance.CurrentAdvertisement != null)
                    {
                        var current = instance.Advertisements.IndexOf(instance.CurrentAdvertisement);
                        //we were getting out of index exceptions so current was showing 
                        //negative one, so we check if it is negative and then assign to 0 if it is.
                        //meaning there is no current advertisement.
                        if (current == -1)
                            current = 0;

                        for (int i = current; i < instance.Advertisements.Count; i++)
                        {
                            if (instance.Advertisements.Count > i + 1)
                            {
                                var ad = instance.Advertisements[i + 1];
                                if (ad.IsShowing)
                                {
                                    instance.CurrentAdvertisement = instance.Advertisements[i + 1];
                                    break;
                                }
                            }
                            else
                            {
                                instance.CurrentAdvertisement = instance.Advertisements[0];
                                break;
                            }
                        }
                    }
                    else if (instance.Advertisements.Count > 0)
                        instance.CurrentAdvertisement = instance.Advertisements[0];
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        void advertTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (PolicyViewModel.Instance.AdChangeAutomaticallyChangeImage)
                {
                    setCurrentAdvertisement();
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI);
            }
        }
        /// <summary>
        /// hits if the period clock goes to zero...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PeriodClock_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == StopWatchWrapperEnum.IsClockAtZero.ToString())
                {
                    if (instance.PeriodClock.IsClockAtZero)
                    {
                        endPeriod();
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI);
            }
        }
        public void removeSkaterFromTeam(TeamMembersViewModel skater, TeamNumberEnum team)
        {
            try
            {
                if (team == TeamNumberEnum.Team1)
                {
                    var member = this.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault();
                    instance.Team1.TeamMembers.Remove(member);
                }
                else if (team == TeamNumberEnum.Team2)
                {
                    var member = this.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault();
                    instance.Team2.TeamMembers.Remove(member);
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        /// <summary>
        /// Sends Player to Penalty Box
        /// </summary>
        /// <param name="skaterBeingSentToBox"></param>
        public void sendSkaterToPenaltyBox(TeamMembersViewModel skaterBeingSentToBox, TeamNumberEnum team)
        {
            try
            {
                SkaterInPenaltyBoxViewModel skater = new SkaterInPenaltyBoxViewModel(skaterBeingSentToBox, instance.ElapsedTimeGameClockMilliSeconds, instance.CurrentJam.JamClock.TimeElapsed, instance.CurrentJam.JamId, instance.CurrentJam.JamNumber);

                instance.PenaltyBox.Add(skater);

                if (team == TeamNumberEnum.Team1)
                {
                    instance.Team1.TeamMembers.Where(x => x.SkaterId == skaterBeingSentToBox.SkaterId).FirstOrDefault().IsInBox = true;
                    //any skater in box isn't allowed to be lead jammer.
                    if (PolicyViewModel.Instance.PenaltyBoxControlsLeadJammer)
                        instance.Team1.TeamMembers.Where(x => x.SkaterId == skaterBeingSentToBox.SkaterId).FirstOrDefault().IsLeadJammer = false;

                    if (instance.CurrentJam.JammerT1 != null && instance.CurrentJam.JammerT1.SkaterId == skaterBeingSentToBox.SkaterId)
                    {
                        instance.CurrentJam.JammerT1 = null;
                    }
                    if (instance.CurrentJam.PivotT1 != null && instance.CurrentJam.PivotT1.SkaterId == skaterBeingSentToBox.SkaterId)
                    {
                        instance.CurrentJam.PivotT1 = null;
                    }
                }
                else if (team == TeamNumberEnum.Team2)
                {
                    instance.Team2.TeamMembers.Where(x => x.SkaterId == skaterBeingSentToBox.SkaterId).FirstOrDefault().IsInBox = true;
                    //any skater in box isn't allowed to be lead jammer.
                    if (PolicyViewModel.Instance.PenaltyBoxControlsLeadJammer)
                        instance.Team2.TeamMembers.Where(x => x.SkaterId == skaterBeingSentToBox.SkaterId).FirstOrDefault().IsLeadJammer = false;

                    if (instance.CurrentJam.JammerT2 != null && instance.CurrentJam.JammerT2.SkaterId == skaterBeingSentToBox.SkaterId)
                    {
                        instance.CurrentJam.JammerT2 = null;
                    }
                    if (instance.CurrentJam.PivotT2 != null && instance.CurrentJam.PivotT2.SkaterId == skaterBeingSentToBox.SkaterId)
                    {
                        instance.CurrentJam.PivotT2 = null;
                    }
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        /// <summary>
        /// removes the player from the penalty box.
        /// </summary>
        /// <param name="skaterBeingRemovedFromBox"></param>
        /// <param name="team"></param>
        public void removeSkaterFromPenaltyBox(TeamMembersViewModel skaterBeingRemovedFromBox, TeamNumberEnum team)
        {
            try
            {
                if (skaterBeingRemovedFromBox.IsInBox)
                {
                    instance.PenaltyBox.Where(x => x.PlayerSentToBox.SkaterId == skaterBeingRemovedFromBox.SkaterId).LastOrDefault().GameTimeInMillisecondsReleased = instance.ElapsedTimeGameClockMilliSeconds;
                    instance.PenaltyBox.Where(x => x.PlayerSentToBox.SkaterId == skaterBeingRemovedFromBox.SkaterId).LastOrDefault().JamTimeInMillisecondsReleased = instance.CurrentJam.JamClock.TimeElapsed;
                    instance.PenaltyBox.Where(x => x.PlayerSentToBox.SkaterId == skaterBeingRemovedFromBox.SkaterId).LastOrDefault().JamIdReleased = instance.CurrentJam.JamId;
                    //instance.PenaltyBox.Where(x => x.PlayerSentToBox.SkaterId == skaterBeingRemovedFromBox.SkaterId).LastOrDefault().JamIdReleased = instance.CurrentJam.JamId;

                    if (team == TeamNumberEnum.Team1)
                        instance.Team1.TeamMembers.Where(x => x.SkaterId == skaterBeingRemovedFromBox.SkaterId).FirstOrDefault().IsInBox = false;
                    else if (team == TeamNumberEnum.Team2)
                        instance.Team2.TeamMembers.Where(x => x.SkaterId == skaterBeingRemovedFromBox.SkaterId).FirstOrDefault().IsInBox = false;
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        /// <summary>
        /// add a skater to a team specified.
        /// </summary>
        /// <param name="skaterName"></param>
        /// <param name="skaterNumber"></param>
        /// <param name="team"></param>
        public void addTeamSkater(string skaterName, string skaterNumber, TeamNumberEnum team)
        {
            try
            {
                TeamMembersViewModel teamMember = new TeamMembersViewModel();
                teamMember.SkaterId = Guid.NewGuid();
                teamMember.SkaterName = skaterName;
                teamMember.SkaterNumber = skaterNumber;
                teamMember.IsBenched = true;
                teamMember.IsBlocker1 = false;
                teamMember.IsBlocker2 = false;
                teamMember.IsBlocker3 = false;
                teamMember.IsInBox = false;
                teamMember.IsJammer = false;
                teamMember.IsLeadJammer = false;
                teamMember.IsPivot = false;

                if (team == TeamNumberEnum.Team1)
                {
                    instance.Team1.TeamMembers.Add(teamMember);
                }
                else
                {
                    instance.Team2.TeamMembers.Add(teamMember);
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        public void AddScoreToGame(int point, TeamNumberEnum team)
        {
            try
            {
                if (team == TeamNumberEnum.Team1)
                {
                    if ((instance.CurrentTeam1Score + point) > -1)
                    {
                        instance.CurrentTeam1Score += point;
                        //so jam scores dont go negative
                        if (instance.CurrentTeam1JamScore + point > -1)
                            instance.CurrentTeam1JamScore += point;
                        long periodTimeRemaining = 0;
                        if (instance.PeriodClock != null)
                            periodTimeRemaining = instance.PeriodClock.TimeRemaining;
                        else
                            Logger.Instance.logMessage("period Clock was NULL", LoggerEnum.message);
                        if (instance.CurrentJam != null)
                        {
                            if (instance.CurrentJam.TotalPointsForJamT1 + point > -1)
                                instance.CurrentJam.TotalPointsForJamT1 += point;
                            var lead = instance.CurrentJam.LeadJammers.Where(x => x.Team == TeamNumberEnum.Team1).LastOrDefault();
                            TeamMembersViewModel skater = null;
                            if (lead != null && lead.Jammer != null)
                                skater = lead.Jammer;
                            else if (instance.CurrentJam.JammerT1 != null)
                                skater = instance.CurrentJam.JammerT1;
                            if (skater == null)
                                instance.ScoresTeam1.Add(new ScoreViewModel(point, periodTimeRemaining, instance.CurrentJam.JamId, instance.CurrentJam.JamNumber, instance.CurrentPeriod));
                            else
                                GameViewModel.Instance.ScoresTeam1.Add(new ScoreViewModel(new TeamMembersViewModel() { SkaterId = skater.SkaterId, SkaterLinkId = skater.SkaterLinkId, IsPivot = skater.IsPivot, IsJammer = skater.IsJammer, IsLeadJammer = skater.IsLeadJammer, LostLeadJammerEligibility = skater.LostLeadJammerEligibility, SkaterName = skater.SkaterName, SkaterNumber = skater.SkaterNumber }, point, instance.CurrentJam.JamId, instance.CurrentJam.JamNumber));
                        }
                    }
                }
                else if (team == TeamNumberEnum.Team2)
                {
                    if ((instance.CurrentTeam2Score + point) > -1)
                    {
                        instance.CurrentTeam2Score += point;
                        //so jam scores dont go negative
                        if (instance.CurrentTeam2JamScore + point > -1)
                            instance.CurrentTeam2JamScore += point;

                        long periodTimeRemaining = 0;
                        if (instance.PeriodClock != null)
                            periodTimeRemaining = instance.PeriodClock.TimeRemaining;
                        else
                            Logger.Instance.logMessage("period Clock was NULL", LoggerEnum.message);
                        if (instance.CurrentJam != null)
                        {
                            if (instance.CurrentJam.TotalPointsForJamT2 + point > -1)
                                instance.CurrentJam.TotalPointsForJamT2 += point;
                            var lead = instance.CurrentJam.LeadJammers.Where(x => x.Team == TeamNumberEnum.Team2).LastOrDefault();
                            TeamMembersViewModel skater = null;
                            if (lead != null && lead.Jammer != null)
                                skater = lead.Jammer;
                            else if (instance.CurrentJam.JammerT2 != null)
                                skater = instance.CurrentJam.JammerT2;
                            if (skater == null)
                                instance.ScoresTeam2.Add(new ScoreViewModel(point, periodTimeRemaining, instance.CurrentJam.JamId, instance.CurrentJam.JamNumber, instance.CurrentPeriod));
                            else
                                GameViewModel.Instance.ScoresTeam2.Add(new ScoreViewModel(new TeamMembersViewModel() { SkaterId = skater.SkaterId, SkaterLinkId = skater.SkaterLinkId, IsPivot = skater.IsPivot, IsJammer = skater.IsJammer, IsLeadJammer = skater.IsLeadJammer, LostLeadJammerEligibility = skater.LostLeadJammerEligibility, SkaterName = skater.SkaterName, SkaterNumber = skater.SkaterNumber }, point, instance.CurrentJam.JamId, instance.CurrentJam.JamNumber));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        /// <summary>
        /// saves the entire game to XML
        /// </summary>
        public void saveGameToXml(string fileName)
        {
            var saveGameTask = Task<bool>.Factory.StartNew(
                () =>
                {
                    try
                    {

                        GameViewModel.instance.Policy = PolicyViewModel.Instance;
                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(Instance.GetType());
                        System.IO.StreamWriter file = new System.IO.StreamWriter(Path.Combine(ScoreboardConfig.SAVE_GAMES_FILE_LOCATION, fileName));

                        writer.Serialize(file, Instance);
                        file.Close();
                        file.Dispose();
                    }
                    catch (Exception exception)
                    {
                        Logger.Instance.logMessage("tried to save the game", LoggerEnum.message);
                        ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
                        //ErrorViewModel.saveError(exception);
                    }
                    return true;
                });

            if (saveGameTask.Result)
            {
                //TODO: show popup?
            }
        }


        public void saveGameAndPicturesToXml(string fileName)
        {
            var saveGameTask = Task<bool>.Factory.StartNew(
                () =>
                {
                    try
                    {
                        PolicyViewModel.Instance.CompressBuzzer();


                        if (PolicyViewModel.Instance.VideoOverlay != null && !string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.LogoPictureLocation))
                        {
                            PolicyViewModel.Instance.VideoOverlay.LogoPictureCompressed = SavePictureOfMemberToXmlFile(PolicyViewModel.Instance.VideoOverlay.LogoPictureLocation);
                        }

                        var f = Instance;
                        f.Policy = PolicyViewModel.Instance;

                        foreach (var member in Instance.Team1.TeamMembers)
                            member.SkaterPictureCompressed = SavePictureOfMemberToXmlFile(member.SkaterPictureLocation);
                        foreach (var member in Instance.Team2.TeamMembers)
                            member.SkaterPictureCompressed = SavePictureOfMemberToXmlFile(member.SkaterPictureLocation);
                        foreach (var member in Instance.Officials.Nsos)
                            member.SkaterPictureCompressed = SavePictureOfMemberToXmlFile(member.SkaterPictureLocation);
                        foreach (var member in Instance.Officials.Referees)
                            member.SkaterPictureCompressed = SavePictureOfMemberToXmlFile(member.SkaterPictureLocation);
                        foreach (var member in Instance.Advertisements)
                            member.AdvertismentPictureCompressed = SavePictureOfMemberToXmlFile(member.FileLocation);


                        if (f.ScoreboardSettings != null)
                            GameViewModel.Instance.ScoreboardSettings.BackgroundPictureCompressed = SavePictureOfMemberToXmlFile(GameViewModel.Instance.ScoreboardSettings.BackgroundPictureLocation);

                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(Instance.GetType());
                        System.IO.StreamWriter file = new System.IO.StreamWriter(fileName);

                        writer.Serialize(file, f);
                        file.Close();
                        file.Dispose();
                    }
                    catch (Exception exception)
                    {
                        Logger.Instance.logMessage("tried to save the game", LoggerEnum.message);
                        ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                        //ErrorViewModel.saveError(exception);
                    }
                    return true;
                });

            if (saveGameTask.Result)
            {
                //TODO: show popup?
            }
        }
        public void saveTeamAndPicturesToXml(string fileName, TeamNumberEnum team)
        {
            var saveGameTask = Task<bool>.Factory.StartNew(
                () =>
                {
                    try
                    {


                        var f = Instance;
                        System.IO.StreamWriter file = new System.IO.StreamWriter(fileName);
                        System.Xml.Serialization.XmlSerializer writer = null;
                        if (team == TeamNumberEnum.Team1)
                        {
                            foreach (var member in f.Team1.TeamMembers)
                            {
                                member.SkaterPictureCompressed = SavePictureOfMemberToXmlFile(member.SkaterPictureLocation);
                            }
                            writer = new System.Xml.Serialization.XmlSerializer(f.Team1.GetType());
                            writer.Serialize(file, f.Team1);
                        }
                        else if (team == TeamNumberEnum.Team2)
                        {
                            foreach (var member in f.Team2.TeamMembers)
                            {
                                member.SkaterPictureCompressed = SavePictureOfMemberToXmlFile(member.SkaterPictureLocation);
                            }
                            writer = new System.Xml.Serialization.XmlSerializer(f.Team2.GetType());
                            writer.Serialize(file, f.Team2);
                        }

                        file.Close();
                        file.Dispose();

                    }
                    catch (Exception exception)
                    {
                        Logger.Instance.logMessage("tried to team", LoggerEnum.message);
                        ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                    }
                    return true;
                });
        }

        public void saveOfficialsAndPicturesToXml(string fileName)
        {
            var saveGameTask = Task<bool>.Factory.StartNew(
                () =>
                {
                    try
                    {


                        var f = Instance;
                        System.IO.StreamWriter file = new System.IO.StreamWriter(fileName);
                        System.Xml.Serialization.XmlSerializer writer = null;

                        foreach (var member in f.Officials.Nsos)
                        {
                            member.SkaterPictureCompressed = SavePictureOfMemberToXmlFile(member.SkaterPictureLocation);
                        }
                        foreach (var member in f.Officials.Referees)
                        {
                            member.SkaterPictureCompressed = SavePictureOfMemberToXmlFile(member.SkaterPictureLocation);
                        }
                        writer = new System.Xml.Serialization.XmlSerializer(f.Officials.GetType());
                        writer.Serialize(file, f.Officials);

                        file.Close();
                        file.Dispose();

                    }
                    catch (Exception exception)
                    {
                        Logger.Instance.logMessage("tried to save officals", LoggerEnum.message);
                        ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                    }
                    return true;
                });
        }

        public static byte[] SavePictureOfMemberToXmlFile(string skaterPictureLocation)
        {
            try
            {
                if (!String.IsNullOrEmpty(skaterPictureLocation) && File.Exists(skaterPictureLocation))
                {
                    Logger.Instance.logMessage("image:" + skaterPictureLocation, LoggerEnum.message);
                    FileStream fs = File.OpenRead(skaterPictureLocation);
                    Logger.Instance.logMessage("imageSIZE:" + fs.Length, LoggerEnum.message);
                    Image image = Image.FromStream(fs);
                    //var image = Image.FromFile(skaterPictureLocation);
                    //http://stackoverflow.com/questions/1053052/a-generic-error-occurred-in-gdi-jpeg-image-to-memorystream
                    Logger.Instance.logMessage("image Height:" + image.Height + ":" + image.Width, LoggerEnum.message);
                    var byteArray = Scoreboard.Library.Util.Imaging.ImageToByte(image);
                    image.Dispose();
                    fs.Dispose();
                    return byteArray;
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried stuffing picture into byte array ", LoggerEnum.message);
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            return null;
        }


        /// <summary>
        /// loads the entire game to XML
        /// </summary>
        public void loadGameFromXml(string fileName)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(GameViewModel));
                if (File.Exists(Path.Combine(ScoreboardConfig.SAVE_GAMES_FILE_LOCATION, fileName)))
                {
                    StreamReader objReader = new StreamReader(Path.Combine(ScoreboardConfig.SAVE_GAMES_FILE_LOCATION, fileName));
                    GameViewModel game = (GameViewModel)xs.Deserialize(objReader);
                    instance = game;
                    setupTimerForOnlineGame();

                    //sets the timers on both the period clock and the jam clock
                    if (game.PeriodClock.StartTime == new DateTime())
                        instance.PeriodClock = new StopwatchWrapper(game.PeriodClock.TimerLength);
                    else
                        instance.PeriodClock = new StopwatchWrapper(game.PeriodClock.TimeRemaining);

                    if (instance.CurrentJam != null && instance.CurrentJam.JamClock != null)
                    {
                        if (game.CurrentJam.JamClock.StartTime == new DateTime())
                            instance.CurrentJam.JamClock = new StopwatchWrapper(game.CurrentJam.JamClock.TimerLength);
                        else
                            instance.CurrentJam.JamClock = new StopwatchWrapper(game.CurrentJam.JamClock.TimeRemaining);
                    }
                    instance.PeriodClock.PropertyChanged += new PropertyChangedEventHandler(PeriodClock_PropertyChanged);

                    foreach (var member in Instance.Team1.TeamMembers)
                        LoadPictureOfMemberFromSavedGame(member);

                    foreach (var member in Instance.Team2.TeamMembers)
                        LoadPictureOfMemberFromSavedGame(member);

                    foreach (var member in Instance.Officials.Nsos)
                        LoadPictureOfMemberFromSavedGame(member);
                    foreach (var member in Instance.Officials.Referees)
                        LoadPictureOfMemberFromSavedGame(member);

                    foreach (var member in Instance.Advertisements)
                    {
                        LoadPictureOAdvertismentFromSavedGame(member.FileLocation, member.AdvertGameId, member.AdvertismentPictureCompressed);
                        member.AdvertismentPictureCompressed = null;
                    }

                    if (GameViewModel.Instance.ScoreboardSettings != null)
                        LoadScoreboardBackground(fileName);

                    AdvertisementViewModel.getAdvertsFromDirectory();
                    SlideShowViewModel.getSlidesFromDirectory();

                    setupTimerForSlideShow();
                    setupAdvertisements();

                    PolicyViewModel.Instance.SetPolicy(instance.Policy);

                    PolicyViewModel.Instance.LoadBuzzerFromCompression();

                    instance.IsCurrentlySendingOnlineGame = false;
                    ///sends the first initial game to the server.
                    if (instance.PublishGameOnline || instance.SaveGameOnline)
                    {
                        sendLiveGameToServer();
                    }
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        private static void LoadScoreboardBackground(string fileName)
        {
            //skater picture found on the computer if it exists.
            if (!String.IsNullOrEmpty(GameViewModel.Instance.ScoreboardSettings.BackgroundPictureLocation) && File.Exists(GameViewModel.Instance.ScoreboardSettings.BackgroundPictureLocation))
            {
                GameViewModel.Instance.ScoreboardSettings.BackgroundPictureCompressed = null;
            }
            else if (GameViewModel.Instance.ScoreboardSettings.BackgroundPictureCompressed != null && !String.IsNullOrEmpty(GameViewModel.Instance.ScoreboardSettings.BackgroundPictureLocation))
            {
                try
                {
                    FileInfo file = new FileInfo(GameViewModel.Instance.ScoreboardSettings.BackgroundPictureLocation);
                    using (var tmpImage = Scoreboard.Library.Util.Imaging.ByteToImage(GameViewModel.Instance.ScoreboardSettings.BackgroundPictureCompressed))
                    {
                        string saveLocation = Path.Combine(ScoreboardConfig.SAVE_BACKGROUND_IMAGES_FOLDER, fileName);
                        tmpImage.Save(saveLocation);
                        GameViewModel.Instance.ScoreboardSettings.BackgroundPictureLocation = saveLocation;
                    }
                }
                catch (Exception e)
                {
                    ErrorViewModel.Save(e, e.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                }
            }
        }
        public void loadTeamRosterFromXml(string fileName, TeamNumberEnum team)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(TeamViewModel));
                if (File.Exists(fileName))
                {
                    StreamReader objReader = new StreamReader(fileName);
                    TeamViewModel teamView = (TeamViewModel)xs.Deserialize(objReader);
                    if (team == TeamNumberEnum.Team1)
                    {
                        //we define the same team ids because the online server and DB structure relies on
                        //team ids being the same through the duration of the game.
                        teamView.TeamId = instance.Team1.TeamId;
                        instance.Team1 = teamView;
                        foreach (var member in Instance.Team1.TeamMembers)
                            LoadPictureOfMemberFromSavedGame(member);
                    }
                    else if (team == TeamNumberEnum.Team2)
                    {
                        //we define the same team ids because the online server and DB structure relies on
                        //team ids being the same through the duration of the game.
                        teamView.TeamId = instance.Team2.TeamId;
                        instance.Team2 = teamView;
                        foreach (var member in Instance.Team2.TeamMembers)
                            LoadPictureOfMemberFromSavedGame(member);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }
        public void loadOfficialsRosterFromXml(string fileName, TeamNumberEnum team)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(Officials.Officials));
                if (File.Exists(fileName))
                {

                    StreamReader objReader = new StreamReader(fileName);
                    try
                    {
                        Officials.Officials refsView = (Officials.Officials)xs.Deserialize(objReader);
                        if (team == TeamNumberEnum.Refs)
                        {
                            //we define the same team ids because the online server and DB structure relies on
                            //team ids being the same through the duration of the game.
                            GameViewModel.Instance.Officials.Referees = refsView.Referees;
                            foreach (var member in Instance.Officials.Referees)
                                LoadPictureOfMemberFromSavedGame(member);
                        }
                        else if (team == TeamNumberEnum.NSOs)
                        {
                            //we define the same team ids because the online server and DB structure relies on
                            //team ids being the same through the duration of the game.
                            GameViewModel.Instance.Officials.Nsos = refsView.Nsos;
                            if (Instance.Officials.Nsos != null)
                            {
                                foreach (var member in Instance.Officials.Nsos)
                                    LoadPictureOfMemberFromSavedGame(member);
                            }
                        }
                        else if (team == TeamNumberEnum.AllRefsNSOs)
                        {
                            //we define the same team ids because the online server and DB structure relies on
                            //team ids being the same through the duration of the game.
                            GameViewModel.Instance.Officials.Nsos = refsView.Nsos;
                            GameViewModel.Instance.Officials.Referees = refsView.Referees;
                            if (Instance.Officials.Nsos != null)
                            {
                                foreach (var member in Instance.Officials.Nsos)
                                    LoadPictureOfMemberFromSavedGame(member);
                            }
                            if (Instance.Officials.Referees != null)
                            {
                                foreach (var member in Instance.Officials.Referees)
                                    LoadPictureOfMemberFromSavedGame(member);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: fileName + ":" + team.ToString() + ":" + objReader.ReadToEnd());
                    }
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: fileName + ":" + team.ToString());
            }
        }
        private static void LoadPictureOfMemberFromSavedGame(RefereeMember member)
        {
            member.SkaterPictureLocation = LoadPictureOfMemberFromSavedGame(member.SkaterPictureLocation, member.SkaterId, member.SkaterPictureCompressed);
            member.SkaterPictureCompressed = null;
        }
        private static void LoadPictureOfMemberFromSavedGame(NSOMember member)
        {
            member.SkaterPictureLocation = LoadPictureOfMemberFromSavedGame(member.SkaterPictureLocation, member.SkaterId, member.SkaterPictureCompressed);
            member.SkaterPictureCompressed = null;
        }
        private static void LoadPictureOfMemberFromSavedGame(TeamMembersViewModel member)
        {
            member.SkaterPictureLocation = LoadPictureOfMemberFromSavedGame(member.SkaterPictureLocation, member.SkaterId, member.SkaterPictureCompressed);
            member.SkaterPictureCompressed = null;
        }

        /// <summary>
        /// loads the picture of a member from a saved game.
        /// </summary>
        /// <param name="member"></param>
        private static string LoadPictureOfMemberFromSavedGame(string skaterPictureLocation, Guid skaterId, byte[] skaterPictureCompressed)
        {
            //skater picture found on the computer if it exists.
            if (!String.IsNullOrEmpty(skaterPictureLocation) && File.Exists(skaterPictureLocation))
            {
                skaterPictureCompressed = null;
            }
            else if (skaterPictureCompressed != null && !String.IsNullOrEmpty(skaterPictureLocation))
            {
                try
                {
                    FileInfo file = new FileInfo(skaterPictureLocation);
                    string pictureName = skaterId.ToString().Replace("-", "") + file.Extension;
                    using (var tmpImage = Scoreboard.Library.Util.Imaging.ByteToImage(skaterPictureCompressed))
                    {
                        string saveLocation = Path.Combine(ScoreboardConfig.SAVE_SKATERS_PICTURE_FOLDER, pictureName);
                        var tmpImage2 = RDN.Utilities.Drawing.Images.ScaleDownImage(tmpImage, ScoreboardConfig.DEFAULT_SIZE_OF_RESIZED_IMAGE, ScoreboardConfig.DEFAULT_SIZE_OF_RESIZED_IMAGE);

                        tmpImage2.Save(saveLocation);
                        skaterPictureLocation = saveLocation;
                    }
                }
                catch (Exception e)
                {
                    ErrorViewModel.Save(e, e.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                }
            }
            return skaterPictureLocation;
        }
        private static string LoadPictureOAdvertismentFromSavedGame(string skaterPictureLocation, Guid skaterId, byte[] skaterPictureCompressed)
        {
            //skater picture found on the computer if it exists.
            if (!String.IsNullOrEmpty(skaterPictureLocation) && File.Exists(skaterPictureLocation))
            {
                skaterPictureCompressed = null;
            }
            else if (skaterPictureCompressed != null && !String.IsNullOrEmpty(skaterPictureLocation))
            {
                try
                {
                    FileInfo file = new FileInfo(skaterPictureLocation);
                    string pictureName = skaterId.ToString().Replace("-", "") + file.Extension;
                    using (var tmpImage = Scoreboard.Library.Util.Imaging.ByteToImage(skaterPictureCompressed))
                    {
                        string saveLocation = Path.Combine(ScoreboardConfig.SAVE_ADVERTS_FOLDER, pictureName);
                        tmpImage.Save(saveLocation);
                        skaterPictureLocation = saveLocation;
                    }
                }
                catch (Exception e)
                {
                    ErrorViewModel.Save(e, e.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                }
            }
            return skaterPictureLocation;
        }
        /// <summary>
        /// saves the entire game to XML
        /// </summary>
        public static GameViewModel deserializeGame(string filePath)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(GameViewModel));
                if (File.Exists(filePath))
                {
                    StreamReader objReader = new StreamReader(filePath);
                    GameViewModel game = (GameViewModel)xs.Deserialize(objReader);
                    objReader.Close();
                    return game;
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, e.GetType(), ErrorGroupEnum.UI);
            }
            return null;
        }


        public static bool SendProfilePictureForMember(Guid gameId, Guid memberId, string pictureLocation, MemberTypeEnum memberType)
        {
            Task<bool>.Factory.StartNew(
                             () =>
                             {
                                 try
                                 {
                                     string urlToSend = ScoreboardConfig.SCOREBOARD_UPLOAD_MEMBER_PICTURE_URL;
                                     //urlToSend = "http://localhost:49534/scoreboard/UploadMemberPictureFromGame?k=" + ScoreboardConfig.KEY_FOR_UPLOAD;
                                     FileInfo picture = new FileInfo(pictureLocation);

                                     urlToSend += "&gameId=" + gameId.ToString();
                                     urlToSend += "&memId=" + memberId.ToString();
                                     urlToSend += "&fileName=" + picture.Name;
                                     urlToSend += "&memberType=" + memberType.ToString();

                                     WebClient client = new WebClient();
                                     client.UploadFile(urlToSend, pictureLocation);
                                     client.Dispose();

                                 }
                                 catch (Exception e)
                                 {
                                     ErrorViewModel.Save(e, e.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
                                 }
                                 return true;
                             });
            return true;
        }

        public static bool SendLogoForTeam(Guid teamId, string teamName, string pictureLocation)
        {
            Task<bool>.Factory.StartNew(
                             () =>
                             {
                                 try
                                 {
                                     string urlToSend = ScoreboardConfig.SCOREBOARD_UPLOAD_LOGO_URL;
                                     FileInfo picture = new FileInfo(pictureLocation);

                                     urlToSend += "&ti=" + teamId.ToString();
                                     urlToSend += "&teamName=" + teamName;

                                     WebClient client = new WebClient();
                                     byte[] rawResponse = client.UploadFile(urlToSend, pictureLocation);
                                     string re = System.Text.Encoding.ASCII.GetString(rawResponse);
                                     JavaScriptSerializer serializer = new JavaScriptSerializer();
                                     GameOnlineModel onlineGame = serializer.Deserialize<GameOnlineModel>(re);
                                     if (teamId == GameViewModel.Instance.Team1.TeamId)
                                     {
                                         GameViewModel.Instance.Team1.Logo.TeamLogoId = new Guid(onlineGame.id);
                                     }
                                     else if (teamId == GameViewModel.Instance.Team2.TeamId)
                                     {
                                         GameViewModel.Instance.Team2.Logo.TeamLogoId = new Guid(onlineGame.id);
                                     }

                                 }
                                 catch (Exception e)
                                 {
                                     ErrorViewModel.Save(e, e.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
                                 }
                                 return true;
                             });
            return true;
        }

        /// <summary>
        /// sends the games to the server that are live.
        /// </summary>
        public static void sendCompletedGameToServer()
        {
            Logger.Instance.logMessage("sending finished game", LoggerEnum.message);
            if (GameViewModel.Instance.IsBeingTested)
                return;

            Task<bool>.Factory.StartNew(
                             () =>
                             {
                                 try
                                 {
                                     CheckIfSendingGameOnline();
                                     GameViewModel.Instance.IsGameOnline = GameViewModelIsOnlineEnum.IsSending;

                                     if (!Network.Internet.CheckConnection())
                                     {
                                         GameViewModel.Instance.IsCurrentlySendingOnlineGame = false;
                                         GameViewModel.Instance.IsGameOnline = GameViewModelIsOnlineEnum.IsOffline;
                                         return true;
                                     }

                                     var f = Instance;


                                     System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(Instance.GetType());
                                     System.IO.StreamWriter file = new System.IO.StreamWriter(ScoreboardConfig.SAVE_TEMP_GAMES_FILE);

                                     writer.Serialize(file, f);
                                     file.Close();
                                     file.Dispose();
                                     Encryption.EncryptFiletoFile(ScoreboardConfig.SAVE_TEMP_GAMES_FILE, ScoreboardConfig.SAVE_GAMES_TEMP_ENCRYPTED_FILE);
                                     string compressedFile = Compression.Compress(new FileInfo(ScoreboardConfig.SAVE_GAMES_TEMP_ENCRYPTED_FILE));

                                     WebClientRDN client = new WebClientRDN();

                                     client.UploadFile(new Uri(ScoreboardConfig.UPLOAD_LIVE_GAMES_URL), compressedFile);

                                     Thread.Sleep(2000);
                                     FileInfo fileInfo = new FileInfo(compressedFile);
                                     if (fileInfo.Exists)
                                         fileInfo.Delete();

                                     FileInfo file1 = new FileInfo(ScoreboardConfig.SAVE_GAMES_TEMP_ENCRYPTED_FILE);
                                     if (file1.Exists)
                                         file1.Delete();

                                     if (GameViewModel.Instance.PublishGameOnline)
                                         GameViewModel.Instance.IsGameOnline = GameViewModelIsOnlineEnum.IsOnline;
                                 }
                                 catch (Exception e)
                                 {
                                     ErrorViewModel.Save(e, e.GetType(), null, null, null, Logger.Instance.getLoggedMessages());

                                 }

                                 GameViewModel.Instance.IsCurrentlySendingOnlineGame = false;
                                 return true;
                             });
        }

        private static void CheckIfSendingGameOnline()
        {
            try
            {
                if (GameViewModel.Instance.IsCurrentlySendingOnlineGame)
                {
                    Logger.Instance.logMessage("currently sending online game.", LoggerEnum.message);
                    Thread.Sleep(2000);
                    if (GameViewModel.Instance.IsCurrentlySendingOnlineGame)
                    {
                        CheckIfSendingGameOnline();
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
        }

        /// <summary>
        /// sends the games to the server that are live.
        /// </summary>
        public static void sendLiveGameToServer()
        {
            try
            {
                if (GameViewModel.Instance.IsCurrentlySendingOnlineGame)
                    return;
                //controls the fact we are actually sending the game, so we don't try and send it two times at the same time
                GameViewModel.Instance.IsCurrentlySendingOnlineGame = true;

                Task<bool>.Factory.StartNew(
                     () =>
                     {
                         try
                         {
                             System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(Instance.GetType());
                             System.IO.StreamWriter file = new System.IO.StreamWriter(ScoreboardConfig.SAVE_TEMP_GAMES_FILE);
                             Logger.Instance.logMessage("opening Stream Writer" + ScoreboardConfig.SAVE_TEMP_GAMES_FILE, LoggerEnum.message);
                             writer.Serialize(file, Instance);
                             file.Close();
                             file.Dispose();

                             if (GameViewModel.Instance.IsBeingTested)
                             {
                                 GameViewModel.Instance.IsCurrentlySendingOnlineGame = false;
                                 return true;
                             }

                             if (!GameViewModel.Instance.PublishGameOnline && !GameViewModel.Instance.SaveGameOnline)
                             {
                                 GameViewModel.Instance.IsGameOnline = GameViewModelIsOnlineEnum.IsOffline;
                                 GameViewModel.Instance.IsCurrentlySendingOnlineGame = false;
                                 return true;
                             }
                             //controls the UI for the user to tell them we are sending online.
                             GameViewModel.Instance.IsGameOnline = GameViewModelIsOnlineEnum.IsSending;

                             if (!Network.Internet.CheckConnection())
                             {
                                 GameViewModel.Instance.IsCurrentlySendingOnlineGame = false;
                                 GameViewModel.Instance.IsGameOnline = GameViewModelIsOnlineEnum.InternetProblem;
                                 return true;
                             }
                             Logger.Instance.logMessage("encrypting file" + ScoreboardConfig.SAVE_TEMP_GAMES_FILE, LoggerEnum.message);
                             Encryption.EncryptFiletoFile(ScoreboardConfig.SAVE_TEMP_GAMES_FILE, ScoreboardConfig.SAVE_GAMES_TEMP_ENCRYPTED_FILE);
                             Logger.Instance.logMessage("compressing file" + ScoreboardConfig.SAVE_GAMES_TEMP_ENCRYPTED_FILE, LoggerEnum.message);
                             string compressedFile = Compression.Compress(new FileInfo(ScoreboardConfig.SAVE_GAMES_TEMP_ENCRYPTED_FILE));


                             string gameUrl = ScoreboardConfig.UPLOAD_LIVE_GAMES_URL;
                             if (String.IsNullOrEmpty(compressedFile))
                             {
                                 GameViewModel.Instance.IsCurrentlySendingOnlineGame = false;
                                 Logger.Instance.logMessage("file doesn't exist22 " + compressedFile, LoggerEnum.message);
                                 return true;
                             }
                             FileInfo fileInfo = new FileInfo(compressedFile);
                             //we check for existance because there might have been an error compressing and encrypting
                             // the file.  So we want to make sure it exists before we try 
                             //and send it.
                             if (!fileInfo.Exists)
                             {
                                 GameViewModel.Instance.IsCurrentlySendingOnlineGame = false;
                                 Logger.Instance.logMessage("file doesn't exist " + compressedFile, LoggerEnum.message);
                                 return true;
                             }
                             Logger.Instance.logMessage("uploading file" + compressedFile, LoggerEnum.message);
                             WebClientRDN client = new WebClientRDN();

                             byte[] rawResponse = client.UploadFile(new Uri(gameUrl), compressedFile);
                             string re = System.Text.Encoding.ASCII.GetString(rawResponse);
                             JavaScriptSerializer serializer = new JavaScriptSerializer();
                             GameOnlineModel onlineGame = serializer.Deserialize<GameOnlineModel>(re);
                             GameViewModel.Instance.UrlForAdministeringGameOnline = onlineGame.url;
                             GameViewModel.Instance.GameIsConfirmedOnline = onlineGame.IsGameOnline;

                             Thread.Sleep(2000);
                             //delete the file after sending.
                             if (fileInfo.Exists)
                                 fileInfo.Delete();

                             FileInfo file1 = new FileInfo(ScoreboardConfig.SAVE_GAMES_TEMP_ENCRYPTED_FILE);
                             if (file1.Exists)
                                 file1.Delete();

                             if (GameViewModel.Instance.GameIsConfirmedOnline)
                                 GameViewModel.Instance.IsGameOnline = GameViewModelIsOnlineEnum.IsOnline;

                             GameViewModel.Instance.IsCurrentlySendingOnlineGame = false;
                         }
                         catch (Exception e)
                         {
                             ErrorViewModel.Save(e, e.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
                             GameViewModel.Instance.IsCurrentlySendingOnlineGame = false;
                         }

                         GameViewModel.Instance.IsCurrentlySendingOnlineGame = false;
                         return true;
                     });
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
                GameViewModel.Instance.IsCurrentlySendingOnlineGame = false;
            }
        }



        /// <summary>
        /// enters the Edit Mode.
        /// </summary>
        public void EnterEditMode()
        {
            try
            {
                Logger.Instance.logMessage("entering edit mode", LoggerEnum.message);
                if (instance.EditModeItems == null)
                    instance.EditModeItems = new List<EditModeModel>();

                instance.IsInEditMode = true;
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }
        /// <summary>
        /// exits Edit Mode
        /// </summary>
        public void ExitEditMode()
        {
            Logger.Instance.logMessage("exiting edit mode", LoggerEnum.message);
            instance.IsInEditMode = false;
        }


        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public event EventHandler OnNewPeriod;
        protected void NewPeriod(object sender, EventArgs eventArgs)
        {
            if (OnNewPeriod != null)
            {
                OnNewPeriod(sender, eventArgs);
            }
        }
        public event EventHandler OnNewIntermission;
        protected void NewIntermission(object sender, EventArgs eventArgs)
        {
            if (OnNewIntermission != null)
            {
                OnNewIntermission(sender, eventArgs);
            }
        }

        public event EventHandler OnNewTimeOut;
        protected void NewTimeOut(object sender, EventArgs eventArgs)
        {
            if (OnNewTimeOut != null)
            {
                OnNewTimeOut(sender, eventArgs);
            }
        }

        public event EventHandler OnNewOfficialReview;
        protected void NewOfficialReview(object sender, EventArgs eventArgs)
        {
            if (OnNewOfficialReview != null)
            {
                OnNewOfficialReview(sender, eventArgs);
            }
        }

        public event EventHandler OnStopJam;
        protected void StopJam(object sender, EventArgs eventArgs)
        {
            if (OnStopJam != null)
            {
                OnStopJam(sender, eventArgs);
            }
        }
        public event EventHandler OnStartJam;
        protected void StartJam(object sender, EventArgs eventArgs)
        {
            if (OnStartJam != null)
            {
                OnStartJam(sender, eventArgs);
            }
        }
        public event EventHandler OnNewLineUp;
        protected void NewLineUp(object sender, EventArgs eventArgs)
        {
            if (OnNewLineUp != null)
            {
                OnNewLineUp(sender, eventArgs);
            }
        }
        public event EventHandler OnNewGame;
        protected void NewGame(object sender, EventArgs eventArgs)
        {
            if (OnNewGame != null)
            {
                OnNewGame(sender, eventArgs);
            }
        }
        public event EventHandler OnNewJam;
        protected void NewJam(object sender, EventArgs eventArgs)
        {
            if (OnNewJam != null)
            {
                OnNewJam(sender, eventArgs);
            }
        }
    }
}
