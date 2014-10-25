using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using RDNationLibrary.Static.Enums;
using RDNationLibrary.Static;
using RDNationLibrary.StopWatch;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Timers;

namespace RDNationLibrary.ViewModel
{
    public enum GameViewModelEnum { CurrentPeriod, CurrentTeam1Score, CurrentTeam2Score, CurrentAdvertisement }
    public class GameViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;



        static GameViewModel instance = new GameViewModel();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static GameViewModel()
        {
        }

        GameViewModel()
        {
        }

        public static GameViewModel Instance
        {
            get
            {
                return instance;
            }
        }


        /// <summary>
        /// MajorUpgrades.MinorUpgrades.BugFixes
        /// </summary>
        public string VersionNumber { get { return Config.SCOREBOARD_VERSION_NUMBER; } }
        public bool HasGameStarted { get; set; }
        public bool CurrentlyInTimeOut { get; set; }
        public StopwatchWrapper GameClock { get; set; }

        public StopwatchWrapper CurrentLineUpClock { get; set; }
        public StopwatchWrapper PeriodClock { get; set; }
        public StopwatchWrapper CurrentTimeOutClock { get; set; }
        public StopwatchWrapper IntermissionClock { get; set; }
        public StopwatchWrapper CountDownClock { get; set; }
        public StopwatchWrapper AdvertisementClock { get; set; }

        public List<StopwatchWrapper> PenaltyBoxClock { get; set; }
        public List<TimeOutViewModel> TimeOuts { get; set; }
        public List<ScoreViewModel> ScoresTeam1 { get; set; }
        public List<ScoreViewModel> ScoresTeam2 { get; set; }
        public List<AdvertisementViewModel> Advertisements { get; set; }
        private AdvertisementViewModel _currentAdvertisement;
        public AdvertisementViewModel CurrentAdvertisement
        {
            get { return _currentAdvertisement; }
            set
            {
                _currentAdvertisement = value;
                OnPropertyChanged("CurrentAdvertisement");
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
        public string GameName { get; set; }
        public DateTime GameDate { get; set; }
        /// <summary>
        /// game type enum WFTDA, MADE, Etc.
        /// </summary>
        public GameTypeEnum GameType { get; set; }
        /// <summary>
        /// total length of game
        /// </summary>
        public long GameClockMilliSecondsLength { get; set; }
        public long PeriodClockMilliSecondsLength { get; set; }
        public long JamClockMilliSecondsLength { get; set; }
        public long LineUpClockMilliSecondsLength { get; set; }
        public long TimeOutClockMilliSecondsLength { get; set; }
        public long IntermissionClockMilliSecondsLength { get; set; }
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



        private ObservableCollection<JamViewModel> _jams = new ObservableCollection<JamViewModel>();
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


        /// <summary>
        /// starts the jam for the game
        /// </summary>
        public void startJam()
        {
            instance.HasGameStarted = true;
            GameViewModel.Instance.startPeriod();
            stopTimeOut();
            instance.CurrentJam.JamClock.Start();
            instance.CurrentLineUpClock.Stop();
            instance.CurrentJam.LineUpClockAfterJam = instance.CurrentLineUpClock;
        }


        /// <summary>
        /// saves the current jam to the history
        /// </summary>
        public void saveJam()
        {
            instance.Jams.Add(instance.CurrentJam);
        }
        /// <summary>
        /// creates and sets up a new jam.
        /// </summary>
        public void createNewJam()
        {
            instance.CurrentJam = new JamViewModel(instance.Jams.Count + 1, instance.ElapsedTimeGameClockMilliSeconds, instance.CurrentPeriod);
            this.NewJam(this, null);
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
            instance.PeriodClock.Stop();
            stopTimeOut();
            stopJam();
            instance.CurrentLineUpClock.Stop();
            GameViewModel.Instance.saveJam();
        }

        public void startIntermission()
        {
            instance.IntermissionClock.Start();
            this.NewIntermission(this, null);
        }

        public void endIntermission()
        {
            instance.IntermissionClock.Stop();
        }


        /// <summary>
        /// starts from a time out mode.
        /// </summary>
        public void startFromTimeOut()
        {
            if (instance.CurrentlyInTimeOut)
            {
                instance.CurrentlyInTimeOut = false;
                instance.PeriodClock.Resume();

            }
        }

        /// <summary>
        /// creates a new time out
        /// </summary>
        public void createTimeOutClock(TimeOutTypeEnum type)
        {
            if (type == TimeOutTypeEnum.Team1 | TimeOutTypeEnum.Team2 == type)
                instance.CurrentTimeOutClock = new StopwatchWrapper(PolicyViewModel.Instance.TimeOutClock);
            else if (type == TimeOutTypeEnum.Offical)//TODO: this clock should be counting up instead of down for the official time out
                instance.CurrentTimeOutClock = new StopwatchWrapper(PolicyViewModel.Instance.TimeOutClock);

            this.NewTimeOut(this, null);
        }
        /// <summary>
        /// starts a time out
        /// </summary>
        /// <param name="type"></param>
        public void startTimeOut(TimeOutTypeEnum type)
        {
            instance.CurrentJam.JamClock.Stop();
            instance.PeriodClock.Stop();
            instance.CurrentLineUpClock.Stop();
            instance.CurrentlyInTimeOut = true;

            TimeOutViewModel timeOut = new TimeOutViewModel(instance.CurrentTimeOutClock, type);
            instance.CurrentTimeOutClock.Start();
            if (type == TimeOutTypeEnum.Team1)
                instance.Team1.TimeOutsLeft -= 1;
            else if (type == TimeOutTypeEnum.Team2)
                instance.Team2.TimeOutsLeft -= 1;

            instance.TimeOuts.Add(timeOut);


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

        public void stopJam()
        {
            _team1.clearTeamPositions();
            _team2.clearTeamPositions();
            instance.CurrentJam.JamClock.Stop();
            this.StopJam(this, null);
        }

        public void createLineUpClock()
        {
            instance.CurrentLineUpClock = new StopwatchWrapper(PolicyViewModel.Instance.LineUpClockPerJam);
            this.NewLineUp(this, null);
        }


        /// <summary>
        /// starts the line up clock counting.
        /// </summary>
        public void startLineUpClock()
        {
            instance.CurrentLineUpClock.Start();
        }
        /// <summary>
        /// creates a new period for the game
        /// </summary>
        public void createNewPeriod()
        {

            GameViewModel.Instance.CurrentPeriod += 1;
            GameViewModel.Instance.PeriodClock = new StopwatchWrapper(PolicyViewModel.Instance.JamClockTimePerJam);
            GameViewModel.Instance.PeriodClock.PropertyChanged += new PropertyChangedEventHandler(PeriodClock_PropertyChanged);

            createNewIntermission();
            this.NewPeriod(this, null);
        }


        /// <summary>
        /// creates a new intermission clock.
        /// </summary>
        public void createNewIntermission()
        {
            GameViewModel.Instance.IntermissionClock = new StopwatchWrapper(PolicyViewModel.Instance.JamClockTimePerJam);
            GameViewModel.Instance.IntermissionClock.PropertyChanged += new PropertyChangedEventHandler(IntermissionClock_PropertyChanged);
        }

        void IntermissionClock_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (StopWatchWrapperEnum.IsClockAtZero.ToString() == e.PropertyName)
            {
                if (instance.IntermissionClock.IsClockAtZero)
                    endIntermission();
            }
        }

        /// <summary>
        /// creates a new instance and overall a brand new roller derby game.
        /// </summary>
        public void createNewGame(GameTypeEnum gameType)
        {
            instance = new GameViewModel();
            instance.GameId = Guid.NewGuid();

            //instance.Periods = Config.WFTDA_DEFAULT_PERIODS;
            instance.Team1.TimeOutsLeft = PolicyViewModel.Instance.TimeOutsPerPeriod;
            instance.Team2.TimeOutsLeft = PolicyViewModel.Instance.TimeOutsPerPeriod;
            GameViewModel.Instance.CurrentPeriod = 0;

            //TODO: Team names should be different?
            instance.Team1.TeamName = "Home";
            instance.CurrentTeam1Score = 0;
            instance.ScoresTeam1 = new List<ScoreViewModel>();
            instance.Team1.TeamId = Guid.NewGuid();
            instance.Team2.TeamName = "Away";
            instance.CurrentTeam2Score = 0;
            instance.ScoresTeam2 = new List<ScoreViewModel>();
            instance.Team2.TeamId = Guid.NewGuid();
            instance.GameDate = DateTime.UtcNow;
            instance.TimeOuts = new List<TimeOutViewModel>();
            createNewPeriod();
            createNewJam();
            createLineUpClock();
            instance.Advertisements = new List<AdvertisementViewModel>();
            AdvertisementViewModel.getAdvertsFromDirectory();
            setupAdvertisements();
            setCurrentAdvertisement();
            this.NewGame(this, null);




        }

        private void setupAdvertisements()
        {
            Timer advertTimer = new Timer();
            advertTimer.Elapsed += new ElapsedEventHandler(advertTimer_Elapsed);
            advertTimer.Interval = 14000;
            advertTimer.Enabled = true;
            advertTimer.Start();
        }

        private void setCurrentAdvertisement()
        {
            if (instance.Advertisements != null)
            {
                if (instance.CurrentAdvertisement != null)
                {
                    var current = instance.Advertisements.IndexOf(instance.CurrentAdvertisement);

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

        void advertTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            setCurrentAdvertisement();
        }
        /// <summary>
        /// hits if the period clock goes to zero...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PeriodClock_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == StopWatchWrapperEnum.IsClockAtZero.ToString())
            {
                if (instance.PeriodClock.IsClockAtZero)
                {
                    endPeriod();

                }
            }
        }
        public void removeSkaterFromTeam(TeamMembersViewModel skater, TeamNumberEnum team)
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

        /// <summary>
        /// Sends Player to Penalty Box
        /// </summary>
        /// <param name="skaterBeingSentToBox"></param>
        public void sendSkaterToPenaltyBox(TeamMembersViewModel skaterBeingSentToBox, TeamNumberEnum team)
        {
            SkaterInPenaltyBoxViewModel skater = new SkaterInPenaltyBoxViewModel();
            skater.GameTimeInMillisecondsSent = instance.ElapsedTimeGameClockMilliSeconds;
            skater.JamTimeInMillisecondsSent = instance.CurrentJam.JamClock.TimeElapsed;
            skater.PlayerSentToBox = skaterBeingSentToBox;
            skater.JamNumberSent = instance.CurrentJam.JamNumber;
            instance.PenaltyBox.Add(skater);

            if (team == TeamNumberEnum.Team1)
            {
                instance.Team1.TeamMembers.Where(x => x.SkaterId == skaterBeingSentToBox.SkaterId).FirstOrDefault().IsInBox = true;
                //any skater in box isn't allowed to be lead jammer.
                if (PolicyViewModel.Instance.PenaltyBoxControlsLeadJammer)
                    instance.Team1.TeamMembers.Where(x => x.SkaterId == skaterBeingSentToBox.SkaterId).FirstOrDefault().IsLeadJammer = false;
            }
            else if (team == TeamNumberEnum.Team2)
            {
                instance.Team2.TeamMembers.Where(x => x.SkaterId == skaterBeingSentToBox.SkaterId).FirstOrDefault().IsInBox = true;
                //any skater in box isn't allowed to be lead jammer.
                if (PolicyViewModel.Instance.PenaltyBoxControlsLeadJammer)
                    instance.Team2.TeamMembers.Where(x => x.SkaterId == skaterBeingSentToBox.SkaterId).FirstOrDefault().IsLeadJammer = false;
            }
        }

        /// <summary>
        /// removes the player from the penalty box.
        /// </summary>
        /// <param name="skaterBeingRemovedFromBox"></param>
        /// <param name="team"></param>
        public void removeSkaterFromPenaltyBox(TeamMembersViewModel skaterBeingRemovedFromBox, TeamNumberEnum team)
        {
            if (skaterBeingRemovedFromBox.IsInBox)
            {
                instance.PenaltyBox.Where(x => x.PlayerSentToBox.SkaterId == skaterBeingRemovedFromBox.SkaterId).LastOrDefault().GameTimeInMillisecondsReleased = instance.ElapsedTimeGameClockMilliSeconds;
                instance.PenaltyBox.Where(x => x.PlayerSentToBox.SkaterId == skaterBeingRemovedFromBox.SkaterId).LastOrDefault().JamTimeInMillisecondsReleased = instance.CurrentJam.JamClock.TimeElapsed;
                instance.PenaltyBox.Where(x => x.PlayerSentToBox.SkaterId == skaterBeingRemovedFromBox.SkaterId).LastOrDefault().JamNumberReleased = instance.CurrentJam.JamNumber;

                if (team == TeamNumberEnum.Team1)
                    instance.Team1.TeamMembers.Where(x => x.SkaterId == skaterBeingRemovedFromBox.SkaterId).FirstOrDefault().IsInBox = false;
                else if (team == TeamNumberEnum.Team2)
                    instance.Team2.TeamMembers.Where(x => x.SkaterId == skaterBeingRemovedFromBox.SkaterId).FirstOrDefault().IsInBox = false;
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

        public void AddScoreToGame(int point, TeamNumberEnum team)
        {
            if (team == TeamNumberEnum.Team1)
            {
                instance.CurrentTeam1Score += point;
                instance.ScoresTeam1.Add(new ScoreViewModel(point, instance.PeriodClock.TimeRemaining, instance.CurrentJam.JamNumber, instance.CurrentPeriod));
            }
            else if (team == TeamNumberEnum.Team2)
            {
                instance.CurrentTeam2Score += point;
                instance.ScoresTeam2.Add(new ScoreViewModel(point, instance.PeriodClock.TimeRemaining, instance.CurrentJam.JamNumber, instance.CurrentPeriod));
            }

        }

        /// <summary>
        /// saves the entire game to XML
        /// </summary>
        public void saveGameToXml()
        {
            var saveGameTask = Task<bool>.Factory.StartNew(
                () =>
                {
                    System.Xml.Serialization.XmlSerializer writer =
                       new System.Xml.Serialization.XmlSerializer(Instance.GetType());
                    System.IO.StreamWriter file = new System.IO.StreamWriter(Config.SAVE_GAMES_FILE);

                    writer.Serialize(file, Instance);
                    file.Close();

                    return true;
                });

            if (saveGameTask.Result)
            {
                //TODO: show popup?
            }
        }

        /// <summary>
        /// saves the entire game to XML
        /// </summary>
        public void loadGameFromXml()
        {
            XmlSerializer xs = new XmlSerializer(typeof(GameViewModel));
            if (File.Exists(Config.SAVE_GAMES_FILE))
            {
                StreamReader objReader = new StreamReader(Config.SAVE_GAMES_FILE);
                GameViewModel game = (GameViewModel)xs.Deserialize(objReader);
                instance = game;
            }
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

        public event EventHandler OnStopJam;
        protected void StopJam(object sender, EventArgs eventArgs)
        {
            if (OnStopJam != null)
            {
                OnStopJam(sender, eventArgs);
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
