using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using RDNScoreboard.Code;
using RDNScoreboard.Themes;
using Scoreboard.Library.ViewModel;
using Scoreboard.Library.StopWatch;
using RDN.Utilities.Config;

using System.Windows.Data;
using System.IO;
using RDNScoreboard.Controls;
using System.Text;
using WpfAnimatedGif;
using Scoreboard.Library.ViewModel.Members;
using Scoreboard.Library.Static.Enums;
using RDN.Utilities.Error;
using System.Windows.Media;
using RDN.Utilities.Util;


namespace RDNScoreboard.Views
{
    /// <summary>
    /// Interaction logic for ClockView.xaml
    /// </summary>
    public partial class ClockView : Window
    {
        public Guid ClockId;
        private static readonly int DEFAULT_SCOREBOARD_MIDDLE_COLUMN_ROW_COUNT = 5;
        /// <summary>
        /// last row the middle column is for making room with the advertisement.
        /// </summary>
        private static readonly int DEFAULT_SCOREBOARD_MIDDLE_COLUMN_LAST_ROW_PERCENTAGE = 130;

        public ClockView()
        {
            InitializeComponent();

            this.Title = "Game Clock - " + ScoreboardConfig.SCOREBOARD_NAME;
            ClockId = Guid.NewGuid();

            ListCollectionView lcv = new ListCollectionView(ThemeManager.GetAllScoreboardThemes());
            lcv.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
            this.ChooseThemesPanel.ItemsSource = lcv;

            setupView();
        }
        /// <summary>
        /// applies the current display.  From slideshow to scoreboard.
        /// Applies the themes. Applies the selections.
        /// </summary>
        private void ApplyDisplayType()
        {
            try
            {
                switch (GameViewModel.Instance.DisplayType)
                {
                    case GameViewModelDisplayTypeEnum.SlideShowForAds:
                        ContentControl.Visibility = System.Windows.Visibility.Collapsed;
                        ImageMain.Visibility = System.Windows.Visibility.Visible;
                        CRGScoreboard.Visibility = System.Windows.Visibility.Collapsed;
                        DefaultScoreboard.Visibility = System.Windows.Visibility.Collapsed;
                        themeTextBox.Visibility = System.Windows.Visibility.Collapsed;
                        ChooseThemesPanel.Visibility = System.Windows.Visibility.Collapsed;

                        break;
                    case GameViewModelDisplayTypeEnum.SlideShowForIntroductions:
                        if (PolicyViewModel.Instance.SkaterLineUpTheme != string.Empty)
                            this.ApplyTheme(PolicyViewModel.Instance.SkaterLineUpTheme);
                        else
                        {
                            this.ApplyTheme(ThemeManager.GetSkaterSlideshowThemes().FirstOrDefault().Name);
                            PolicyViewModel.Instance.SkaterLineUpTheme = ThemeManager.GetSkaterSlideshowThemes().FirstOrDefault().Name;
                            Logger.Instance.logMessage("setting skater line up theme", LoggerEnum.message);
                            PolicyViewModel.Instance.savePolicyToXml();
                        }
                        if (ContentControl.Visibility != System.Windows.Visibility.Visible)
                            ContentControl.Visibility = System.Windows.Visibility.Visible;

                        ImageMain.Visibility = System.Windows.Visibility.Collapsed;
                        CRGScoreboard.Visibility = System.Windows.Visibility.Collapsed;
                        DefaultScoreboard.Visibility = System.Windows.Visibility.Collapsed;
                        themeTextBox.Visibility = System.Windows.Visibility.Collapsed;
                        ChooseThemesPanel.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case GameViewModelDisplayTypeEnum.Scoreboard:
                    default:
                        ContentControl.Visibility = System.Windows.Visibility.Collapsed;
                        themeTextBox.Visibility = System.Windows.Visibility.Visible;
                        ChooseThemesPanel.Visibility = System.Windows.Visibility.Visible;
                        if (!String.IsNullOrEmpty(PolicyViewModel.Instance.DefaultScoreboardTheme))
                        {
                            this.ApplyTheme(PolicyViewModel.Instance.DefaultScoreboardTheme);
                        }
                        else
                        {
                            this.ApplyTheme(ThemeManager.GetCRGThemesScoreboard().FirstOrDefault().Name);
                            PolicyViewModel.Instance.DefaultScoreboardTheme = ThemeManager.GetCRGThemesScoreboard().FirstOrDefault().Name;
                            Logger.Instance.logMessage("setting default theme", LoggerEnum.message);
                            PolicyViewModel.Instance.savePolicyToXml();
                        }

                        if (!String.IsNullOrEmpty(PolicyViewModel.Instance.DefaultScoreboardTheme))
                        {
                            var theme = ThemeManager.GetAllScoreboardThemes().FirstOrDefault(x => x.Name == PolicyViewModel.Instance.DefaultScoreboardTheme);
                            ChooseThemesPanel.SelectedItem = theme;
                            if (theme.Type == ThemeEnum.CRGScoreboard || theme.Type == ThemeEnum.TournamentScoreboard)
                            {
                                ImageMain.Visibility = System.Windows.Visibility.Collapsed;
                                CRGScoreboard.Visibility = System.Windows.Visibility.Visible;
                                DefaultScoreboard.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            else if (theme.Type == ThemeEnum.DefaultScoreboard)
                            {
                                ImageMain.Visibility = System.Windows.Visibility.Collapsed;
                                CRGScoreboard.Visibility = System.Windows.Visibility.Collapsed;
                                DefaultScoreboard.Visibility = System.Windows.Visibility.Visible;
                            }
                            else if (theme.Type == ThemeEnum.JammersDefaultScoreboard)
                            {

                            }

                            //we need to keep this here because when we change the background image, something weird happens to the linearbackground.
                            //checking if there is a background imagae currently..
                            if (this.TryFindResource("ClockViewBackgroundBrush").GetType() == typeof(LinearGradientBrush))
                                this.Background = (LinearGradientBrush)this.TryFindResource("ClockViewBackgroundBrush");
                            else
                                this.Background = (ImageBrush)this.TryFindResource("ClockViewBackgroundBrush");
                        }
                        break;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }

        }



        /// <summary>
        /// sets up the clock view for new games and open windows.
        /// </summary>
        public void setupView()
        {
            GameViewModel.Instance.OnNewGame += new EventHandler(Instance_OnNewGame);
            GameViewModel.Instance.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Instance_PropertyChanged);
            LoadTeamView();
            GameViewModel.Instance.OnNewJam += new EventHandler(Instance_OnNewJam);
            GameViewModel.Instance.OnStopJam += new EventHandler(Instance_OnStopJam);
            GameViewModel.Instance.OnStartJam += new EventHandler(Instance_OnStartJam);
            GameViewModel.Instance.OnNewLineUp += new EventHandler(Instance_OnNewLineUp);
            GameViewModel.Instance.OnNewTimeOut += new EventHandler(Instance_OnNewTimeOut);
            GameViewModel.Instance.OnNewOfficialReview += new EventHandler(Instance_OnNewOfficialReview);
            GameViewModel.Instance.OnNewIntermission += new EventHandler(Instance_OnNewIntermission);
            GameViewModel.Instance.OnNewPeriod += new EventHandler(Instance_OnNewPeriod);
            ShowActiveJammerPictures.IsChecked = PolicyViewModel.Instance.ShowActiveJammerPictures;
            if (GameViewModel.Instance.PeriodClock != null)
                GameViewModel.Instance.PeriodClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(PeriodClock_PropertyChanged);
            if (GameViewModel.Instance.CurrentJam != null)
            {
                GameViewModel.Instance.CurrentJam.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentJam_PropertyChanged);
                if (GameViewModel.Instance.CurrentJam.JamClock != null)
                    GameViewModel.Instance.CurrentJam.JamClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(JamClock_PropertyChanged);
            }
            if (GameViewModel.Instance.CurrentLineUpClock != null)
                GameViewModel.Instance.CurrentLineUpClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentLineUpClock_PropertyChanged);
            if (GameViewModel.Instance.CurrentTimeOutClock != null)
                GameViewModel.Instance.CurrentTimeOutClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentTimeOutClock_PropertyChanged);
            if (GameViewModel.Instance.IntermissionClock != null)
                GameViewModel.Instance.IntermissionClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(IntermissionClock_PropertyChanged);



            if (PolicyViewModel.Instance.EnableAdChange && PolicyViewModel.Instance.AdChangeUseLineUpClock)
            {
                if (GameViewModel.Instance.CurrentAdvertisement != null && (new FileInfo(GameViewModel.Instance.CurrentAdvertisement.FileLocation)).Exists)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
               (DispatcherOperationCallback)delegate(object arg)
               {
                   try
                   {
                       Logger.Instance.logMessage("loading advert bitmap: " + GameViewModel.Instance.CurrentAdvertisement.FileLocation, LoggerEnum.message);

                       BitmapImage bi = new BitmapImage();
                       bi.BeginInit();
                       bi.UriSource = new Uri(GameViewModel.Instance.CurrentAdvertisement.FileLocation);
                       bi.CacheOption = BitmapCacheOption.OnLoad;
                       bi.EndInit();
                       lineUpImage.Source = bi;
                       lineUpImageDefault.Source = bi;
                   }
                   catch (Exception exception)
                   {
                       ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                   }
                   return null;
               }, null);
                }
            }

            PolicyViewModel.Instance.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(InstancePolicyViewModel_PropertyChanged);

            if (GameViewModel.Instance.ScoreboardSettings != null && !String.IsNullOrEmpty(GameViewModel.Instance.ScoreboardSettings.BackgroundPictureLocation))
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object arg)
                {
                    try
                    {
                        var brush = new ImageBrush();
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.UriSource = new Uri(GameViewModel.Instance.ScoreboardSettings.BackgroundPictureLocation);
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.EndInit();
                        brush.ImageSource = bi;
                        this.Background = brush;
                        uploadFileTextBox.Text = GameViewModel.Instance.ScoreboardSettings.BackgroundPictureLocation;
                    }
                    catch (Exception exception)
                    {
                        ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                    }
                    return null;
                }, null);
            }

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            (DispatcherOperationCallback)delegate(object arg)
            {
                try
                {
                    setupPeriodClocks();
                    this.TimeOutClock.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.TimeOutClock).ToString(@"m\:ss");
                    this.bottomLeftTimeLbl.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.PeriodClock).ToString(@"m\:ss");
                    this.bottomRightTimeLbl.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"m\:ss");

                    this.JamPeriodShowingGrid.Visibility = System.Windows.Visibility.Visible;
                    this.FinalGameOverGrid.Visibility = System.Windows.Visibility.Hidden;
                    this.LineUpGrid.Visibility = System.Windows.Visibility.Hidden;
                    this.IntermissionGrid.Visibility = System.Windows.Visibility.Hidden;
                    this.TimeOutShowingGrid.Visibility = System.Windows.Visibility.Hidden;

                    this.team1JamScoreLbl.Content = GameViewModel.Instance.CurrentTeam1JamScore;
                    this.team2JamScoreLbl.Content = GameViewModel.Instance.CurrentTeam2JamScore;

                    this.team1JamScoreLblDefault.Content = GameViewModel.Instance.CurrentTeam1JamScore;
                    this.team2JamScoreLblDefault.Content = GameViewModel.Instance.CurrentTeam2JamScore;

                    this.team1ScoreLbl.Content = GameViewModel.Instance.CurrentTeam1Score;
                    this.team2ScoreLbl.Content = GameViewModel.Instance.CurrentTeam2Score;

                    this.team1ScoreLblDefault.Content = GameViewModel.Instance.CurrentTeam1Score;
                    this.team2ScoreLblDefault.Content = GameViewModel.Instance.CurrentTeam2Score;


                    //for DefaultClock
                    this.PeriodTimeLblDefault.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.PeriodClock).ToString(@"m\:ss");
                    this.ImageMainClock.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.PeriodClock).ToString(@"m\:ss");
                    this.JamTimeLblDefault.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"m\:ss");
                    //must set period name because we change it when the final intermission hits.
                    this.PeriodLblDefault.Content = "Period 1";
                    if (GameViewModel.Instance.Team1 != null)
                    {
                        this.team1TimeoutsLbl.Content = GameViewModel.Instance.Team1.TimeOutsLeft;
                        this.timeOutsTeam1Default.Content = GameViewModel.Instance.Team1.TimeOutsLeft;
                        this.team1LblDefault.Content = GameViewModel.Instance.Team1.TeamName;
                        this.team1Lbl.Content = GameViewModel.Instance.Team1.TeamName;
                    }
                    if (GameViewModel.Instance.Team2 != null)
                    {
                        this.team2TimeoutsLbl.Content = GameViewModel.Instance.Team2.TimeOutsLeft;
                        this.timeOutsTeam2Default.Content = GameViewModel.Instance.Team2.TimeOutsLeft;
                        this.team2LblDefault.Content = GameViewModel.Instance.Team2.TeamName;
                        this.team2Lbl.Content = GameViewModel.Instance.Team2.TeamName;
                    }
                }
                catch (Exception exception)
                {
                    ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                }
                return null;
            }, null);


            ApplyDisplayType();
        }

        void Instance_OnNewOfficialReview(object sender, EventArgs e)
        {

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
             (DispatcherOperationCallback)delegate(object arg)
             {
                 this.JamPeriodShowingGrid.Visibility = System.Windows.Visibility.Hidden;
                 this.LineUpGrid.Visibility = System.Windows.Visibility.Hidden;
                 this.IntermissionGrid.Visibility = System.Windows.Visibility.Hidden;
                 this.TimeOutShowingGrid.Visibility = System.Windows.Visibility.Visible;
                 this.FinalGameOverGrid.Visibility = System.Windows.Visibility.Hidden;
                 this.TimeOutClock.Content = TimeSpan.FromMilliseconds(GameViewModel.Instance.CurrentTimeOutClock.TimeRemaining).ToString(@"m\:ss");

                 this.JamLblDefault.Content = "Official Review";

                 this.timeOutLbl.Visibility = System.Windows.Visibility.Hidden;
                 this.officialReviewLbl.Visibility = System.Windows.Visibility.Visible;
                 this.JamTimeLblDefault.Content = TimeSpan.FromMilliseconds(GameViewModel.Instance.CurrentTimeOutClock.TimeRemaining).ToString(@"m\:ss");
                 return null;
             }, null);
            if (GameViewModel.Instance.CurrentTimeOutClock != null)
                GameViewModel.Instance.CurrentTimeOutClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentTimeOutClock_PropertyChanged);


        }

        public void LoadTeamView()
        {
            GameViewModel.Instance.Team1.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Team1_PropertyChanged);
            GameViewModel.Instance.Team2.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Team2_PropertyChanged);

            loadTeam1Image();
            loadTeam2Image();


            if (GameViewModel.Instance.Team1 != null)
            {
                this.team1TimeoutsLbl.Content = GameViewModel.Instance.Team1.TimeOutsLeft;
                this.timeOutsTeam1Default.Content = GameViewModel.Instance.Team1.TimeOutsLeft;
                this.team1LblDefault.Content = GameViewModel.Instance.Team1.TeamName;
                this.team1Lbl.Content = GameViewModel.Instance.Team1.TeamName;
            }
            if (GameViewModel.Instance.Team2 != null)
            {
                this.team2TimeoutsLbl.Content = GameViewModel.Instance.Team2.TimeOutsLeft;
                this.timeOutsTeam2Default.Content = GameViewModel.Instance.Team2.TimeOutsLeft;
                this.team2LblDefault.Content = GameViewModel.Instance.Team2.TeamName;
                this.team2Lbl.Content = GameViewModel.Instance.Team2.TeamName;
            }

        }
        /// <summary>
        /// gets hit when a new period is created.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Instance_OnNewPeriod(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
           (DispatcherOperationCallback)delegate(object arg)
           {

               this.JamPeriodShowingGrid.Visibility = System.Windows.Visibility.Visible;
               this.LineUpGrid.Visibility = System.Windows.Visibility.Hidden;
               this.FinalGameOverGrid.Visibility = System.Windows.Visibility.Hidden;
               this.IntermissionGrid.Visibility = System.Windows.Visibility.Hidden;
               this.TimeOutShowingGrid.Visibility = System.Windows.Visibility.Hidden;
               this.PeriodTimeOutClock.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.PeriodClock).ToString(@"m\:ss");

               //this.PeriodLblDefault.Content = "Period " + GameViewModel.Instance.CurrentPeriod;
               this.PeriodTimeLblDefault.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.PeriodClock).ToString(@"m\:ss");
               this.ImageMainClock.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.PeriodClock).ToString(@"m\:ss");

               this.bottomRightTimeLbl.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"m\:ss");
               this.JamTimeLblDefault.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"m\:ss");
               return null;
           }, null);
            if (GameViewModel.Instance.PeriodClock != null)
                GameViewModel.Instance.PeriodClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(PeriodClock_PropertyChanged);

        }
        /// <summary>
        /// gets hit when a new intermission is created.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Instance_OnNewIntermission(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            (DispatcherOperationCallback)delegate(object arg)
            {
                this.bottomRightTimeLbl.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"m\:ss");
                this.JamTimeLblDefault.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"m\:ss");


                this.JamPeriodShowingGrid.Visibility = System.Windows.Visibility.Hidden;
                this.LineUpGrid.Visibility = System.Windows.Visibility.Hidden;
                this.IntermissionGrid.Visibility = System.Windows.Visibility.Visible;
                this.FinalGameOverGrid.Visibility = System.Windows.Visibility.Hidden;
                this.TimeOutShowingGrid.Visibility = System.Windows.Visibility.Hidden;
                this.IntermissionClock.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.IntermissionStartOfClockInMilliseconds).ToString(@"m\:ss");

                //checking what intermission number it is declares what text shows.
                this.PeriodLblDefault.Content = GameViewModel.Instance.NameOfIntermission;
                this.IntermissionLbl.Content = GameViewModel.Instance.NameOfIntermission;

                this.PeriodTimeLblDefault.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.IntermissionStartOfClockInMilliseconds).ToString(@"m\:ss");
                this.ImageMainClock.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.IntermissionStartOfClockInMilliseconds).ToString(@"m\:ss");
                return null;
            }, null);
            if (GameViewModel.Instance.IntermissionClock != null)
                GameViewModel.Instance.IntermissionClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(IntermissionClock_PropertyChanged);

        }
        /// <summary>
        /// gets hit when the intemermission clock counts down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void IntermissionClock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (StopWatchWrapperEnum.TimeRemaining.ToString() == e.PropertyName)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    this.IntermissionClock.Content = TimeSpan.FromMilliseconds(GameViewModel.Instance.IntermissionClock.TimeRemaining).ToString(@"m\:ss");

                    this.PeriodTimeLblDefault.Content = TimeSpan.FromMilliseconds(GameViewModel.Instance.IntermissionClock.TimeRemaining).ToString(@"m\:ss");
                    this.ImageMainClock.Content = TimeSpan.FromMilliseconds(GameViewModel.Instance.IntermissionClock.TimeRemaining).ToString(@"m\:ss");
                    return null;
                }, null);
            }
            else if (StopWatchWrapperEnum.IsClockAtZero.ToString() == e.PropertyName)
            {
                //make sure the intermission clock is at zero.
                if (GameViewModel.Instance.IntermissionClock.IsClockAtZero == true)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        //need to reset the clocks for period view on the default clocks.
                        setupPeriodClocks();
                        this.PeriodTimeLblDefault.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.PeriodClock).ToString(@"m\:ss");

                        //need to reset the clocks for period view on the CRG clock
                        this.bottomLeftTimeLbl.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.PeriodClock).ToString(@"m\:ss");
                        IntermissionGrid.Visibility = System.Windows.Visibility.Hidden;
                        JamPeriodShowingGrid.Visibility = System.Windows.Visibility.Visible;
                        return null;
                    }, null);
                }
            }
        }
        /// <summary>
        /// gets hit on new time out
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Instance_OnNewTimeOut(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
             (DispatcherOperationCallback)delegate(object arg)
             {
                 this.JamPeriodShowingGrid.Visibility = System.Windows.Visibility.Hidden;
                 this.LineUpGrid.Visibility = System.Windows.Visibility.Hidden;
                 this.IntermissionGrid.Visibility = System.Windows.Visibility.Hidden;
                 this.TimeOutShowingGrid.Visibility = System.Windows.Visibility.Visible;
                 this.FinalGameOverGrid.Visibility = System.Windows.Visibility.Hidden;
                 this.TimeOutClock.Content = TimeSpan.FromMilliseconds(GameViewModel.Instance.CurrentTimeOutClock.TimeRemaining).ToString(@"m\:ss");

                 this.JamLblDefault.Content = "Time Out";
                 this.timeOutLbl.Visibility = System.Windows.Visibility.Visible;
                 this.officialReviewLbl.Visibility = System.Windows.Visibility.Hidden;
                 this.JamTimeLblDefault.Content = TimeSpan.FromMilliseconds(GameViewModel.Instance.CurrentTimeOutClock.TimeRemaining).ToString(@"m\:ss");
                 return null;
             }, null);
            if (GameViewModel.Instance.CurrentTimeOutClock != null)
                GameViewModel.Instance.CurrentTimeOutClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentTimeOutClock_PropertyChanged);

        }
        /// <summary>
        /// created a new line up clock.
        /// </summary>
        /// <param name="sender"></param>Team2ScoreLis
        /// <param name="e"></param>
        void Instance_OnNewLineUp(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
              (DispatcherOperationCallback)delegate(object arg)
              {
                  this.jammerTeam2.Content = "";
                  this.jammerTeam2.Visibility = System.Windows.Visibility.Hidden;
                  this.jammerTeam1.Content = "";
                  this.jammerTeam1.Visibility = System.Windows.Visibility.Hidden;
                  //clears the lead jammer arrows.
                  this.JammerLeadLeft.Visibility = System.Windows.Visibility.Hidden;
                  this.JammerLeadRight.Visibility = System.Windows.Visibility.Hidden;
                  this.JammerLeadLeftDefault.Visibility = System.Windows.Visibility.Hidden;
                  this.JammerLeadRightDefault.Visibility = System.Windows.Visibility.Hidden;

                  this.JamPeriodShowingGrid.Visibility = System.Windows.Visibility.Hidden;
                  this.LineUpGrid.Visibility = System.Windows.Visibility.Visible;
                  this.IntermissionGrid.Visibility = System.Windows.Visibility.Hidden;
                  this.TimeOutShowingGrid.Visibility = System.Windows.Visibility.Hidden;
                  this.FinalGameOverGrid.Visibility = System.Windows.Visibility.Hidden;

                  if (PolicyViewModel.Instance.ShowLatestPoints)
                  {
                      team1LblDefault.Visibility = Visibility.Hidden;
                      team1ScoreLblDefault.Visibility = Visibility.Hidden;
                      team1JamScoreLblDefault.Visibility = Visibility.Hidden;
                      if (PolicyViewModel.Instance.ShowActiveJammerPictures)
                          team1JammerimgDefault.Visibility = Visibility.Hidden;
                      team1JammerLblDefault.Visibility = Visibility.Hidden;

                      team2LblDefault.Visibility = Visibility.Hidden;
                      team2ScoreLblDefault.Visibility = Visibility.Hidden;
                      team2JamScoreLblDefault.Visibility = Visibility.Hidden;
                      if (PolicyViewModel.Instance.ShowActiveJammerPictures)
                          team2JammerimgDefault.Visibility = Visibility.Hidden;
                      team2JammerLblDefault.Visibility = Visibility.Hidden;

                      team1ScoreLbl.Visibility = Visibility.Hidden;
                      team1JamScoreLbl.Visibility = Visibility.Hidden;
                      team2ScoreLbl.Visibility = Visibility.Hidden;
                      team2JamScoreLbl.Visibility = Visibility.Hidden;
                      TimeOutsGrid.Visibility = Visibility.Hidden;

                      if (PolicyViewModel.Instance.EnableAdChange && PolicyViewModel.Instance.AdChangeUseLineUpClock)
                      {
                          team1imgDefault.Visibility = Visibility.Hidden;
                          team2imgDefault.Visibility = Visibility.Hidden;
                      }
                      else
                      {
                          team1imgDefault.Visibility = Visibility.Visible;
                          team2imgDefault.Visibility = Visibility.Visible;
                      }

                  }
                  this.JamLblDefault.Content = "Line Up";

                  //5 is the current line up row count.  If it only has 5, then we can add a new 
                  //row depending on if the line clock is now counting down.
                  //we add this row as a ghost row.  We don't actually put anything in it, we
                  //just use its space to make room for the rotating ad.
                  if (PolicyViewModel.Instance.EnableAdChange && PolicyViewModel.Instance.AdChangeUseLineUpClock)
                  {
                      Grid.SetRowSpan(MiddleGrid, 1);
                      lineUpImageDefault.Visibility = System.Windows.Visibility.Visible;
                  }

                  return null;
              }, null);
            if (GameViewModel.Instance.CurrentLineUpClock != null)
                GameViewModel.Instance.CurrentLineUpClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentLineUpClock_PropertyChanged);
        }
        /// <summary>
        /// gets hit on stopping of the jam
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Instance_OnStopJam(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
              (DispatcherOperationCallback)delegate(object arg)
              {
                  this.JamPeriodShowingGrid.Visibility = System.Windows.Visibility.Hidden;
                  this.LineUpGrid.Visibility = System.Windows.Visibility.Visible;
                  this.IntermissionGrid.Visibility = System.Windows.Visibility.Hidden;
                  this.TimeOutShowingGrid.Visibility = System.Windows.Visibility.Hidden;
                  this.FinalGameOverGrid.Visibility = System.Windows.Visibility.Hidden;
                  this.LineUpClock.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.LineUpClockPerJam).ToString(@"m\:ss");

                  this.JamTimeLblDefault.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.LineUpClockPerJam).ToString(@"m\:ss");
                  this.team1JammerLblDefault.Content = "";
                  this.team1JammerLblDefault.Visibility = System.Windows.Visibility.Hidden;
                  this.team2JammerLblDefault.Content = "";
                  this.team2JammerLblDefault.Visibility = System.Windows.Visibility.Hidden;
                  if (PolicyViewModel.Instance.ShowActiveJammerPictures)
                  {
                      this.team1JammerimgDefault.Source = null;
                      this.team2JammerimgDefault.Source = null;
                  }
                  return null;
              }, null);
        }
        /// <summary>
        /// gets hit on new jam
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Instance_OnNewJam(object sender, EventArgs e)
        {
            if (GameViewModel.Instance.CurrentJam != null)
            {
                GameViewModel.Instance.CurrentJam.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentJam_PropertyChanged);

                if (GameViewModel.Instance.CurrentJam.JamClock != null)
                    GameViewModel.Instance.CurrentJam.JamClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(JamClock_PropertyChanged);

                //if (GameViewModel.Instance.CurrentLineUpClock != null)
                //    GameViewModel.Instance.CurrentLineUpClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentLineUpClock_PropertyChanged);
            }
        }

        void Instance_OnStartJam(object sender, EventArgs e)
        {
            if (GameViewModel.Instance.CurrentJam != null)
            {

                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
              (DispatcherOperationCallback)delegate(object arg)
              {
                  this.bottomRightNameLbl.Content = "Jam " + GameViewModel.Instance.CurrentJam.JamNumber.ToString();
                  setupPeriodClocks();
                  //for the crg scoreboard
                  this.JamPeriodShowingGrid.Visibility = System.Windows.Visibility.Visible;
                  this.LineUpGrid.Visibility = System.Windows.Visibility.Hidden;
                  this.IntermissionGrid.Visibility = System.Windows.Visibility.Hidden;
                  this.TimeOutShowingGrid.Visibility = System.Windows.Visibility.Hidden;
                  this.FinalGameOverGrid.Visibility = System.Windows.Visibility.Hidden;
                  this.bottomRightTimeLbl.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"m\:ss");
                  this.JammerLeadLeft.Visibility = System.Windows.Visibility.Hidden;
                  this.JammerLeadRight.Visibility = System.Windows.Visibility.Hidden;

                  //this is the default scoreboard
                  this.JamLblDefault.Content = "  Jam " + GameViewModel.Instance.CurrentJam.JamNumber.ToString() + " ";
                  this.JamTimeLblDefault.Content = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"m\:ss");
                  this.JammerLeadLeftDefault.Visibility = System.Windows.Visibility.Hidden;
                  this.JammerLeadRightDefault.Visibility = System.Windows.Visibility.Hidden;
                  this.team2JammerLblDefault.Visibility = System.Windows.Visibility.Hidden;
                  this.team1JammerLblDefault.Visibility = System.Windows.Visibility.Hidden;

                  team1LblDefault.Visibility = Visibility.Visible;
                  team1ScoreLblDefault.Visibility = Visibility.Visible;
                  team1JamScoreLblDefault.Visibility = Visibility.Visible;
                  if (PolicyViewModel.Instance.ShowActiveJammerPictures)
                      team1JammerimgDefault.Visibility = Visibility.Visible;

                  team2LblDefault.Visibility = Visibility.Visible;
                  team2ScoreLblDefault.Visibility = Visibility.Visible;
                  team2JamScoreLblDefault.Visibility = Visibility.Visible;
                  if (PolicyViewModel.Instance.ShowActiveJammerPictures)
                      team2JammerimgDefault.Visibility = Visibility.Visible;

                  team1imgDefault.Visibility = Visibility.Visible;
                  team2imgDefault.Visibility = Visibility.Visible;

                  team1ScoreLbl.Visibility = Visibility.Visible;
                  team1JamScoreLbl.Visibility = Visibility.Visible;
                  team2ScoreLbl.Visibility = Visibility.Visible;
                  team2JamScoreLbl.Visibility = Visibility.Visible;
                  TimeOutsGrid.Visibility = Visibility.Visible;


                  //removes the extra row we made for the advert.
                  Grid.SetRowSpan(MiddleGrid, 2);
                  lineUpImageDefault.Visibility = System.Windows.Visibility.Hidden;
                  return null;
              }, null);

            }
        }


        /// <summary>
        /// gets hit when time out clock counts down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CurrentTimeOutClock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (StopWatchWrapperEnum.TimeRemaining.ToString() == e.PropertyName)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    try
                    {
                        if (GameViewModel.Instance.CurrentTimeOutClock != null && GameViewModel.Instance.CurrentTimeOutClock.IsRunning)
                        {
                            this.TimeOutClock.Content = TimeSpan.FromMilliseconds(GameViewModel.Instance.CurrentTimeOutClock.TimeRemaining).ToString(@"m\:ss");
                            this.JamTimeLblDefault.Content = TimeSpan.FromMilliseconds(GameViewModel.Instance.CurrentTimeOutClock.TimeRemaining).ToString(@"m\:ss");
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                    }
                    return null;
                }, null);
            }
        }
        /// <summary>
        /// on start of time outs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //void Instance_OnStartTimeOut(object sender, EventArgs e)
        //{
        //    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
        //    (DispatcherOperationCallback)delegate(object arg)
        //    {
        //        this.JamPeriodShowingGrid.Visibility = System.Windows.Visibility.Hidden;
        //        this.LineUpGrid.Visibility = System.Windows.Visibility.Hidden;
        //        this.IntermissionGrid.Visibility = System.Windows.Visibility.Hidden;
        //        this.TimeOutShowingGrid.Visibility = System.Windows.Visibility.Visible;
        //        this.FinalGameOverGrid.Visibility = System.Windows.Visibility.Hidden;
        //        this.JamLblDefault.Content = "Time Out";
        //        return null;
        //    }, null);
        //}
        /// <summary>
        /// shows text in all the controls period labels.
        /// </summary>
        private void setupPeriodClocks()
        {
            this.bottomLeftNameLbl.Content = "Period " + GameViewModel.Instance.CurrentPeriod;
            this.PeriodLineUpLbl.Content = "Period " + GameViewModel.Instance.CurrentPeriod;
            this.PeriodTimeOutLbl.Content = "Period " + GameViewModel.Instance.CurrentPeriod;

            this.PeriodLblDefault.Content = "Period " + GameViewModel.Instance.CurrentPeriod;
        }
        /// <summary>
        /// current line up clocks counts down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CurrentLineUpClock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (StopWatchWrapperEnum.TimeRemaining.ToString() == e.PropertyName)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    this.LineUpClock.Content = TimeSpan.FromMilliseconds(GameViewModel.Instance.CurrentLineUpClock.TimeRemaining).ToString(@"m\:ss");

                    this.JamTimeLblDefault.Content = TimeSpan.FromMilliseconds(GameViewModel.Instance.CurrentLineUpClock.TimeRemaining).ToString(@"m\:ss");
                    return null;
                }, null);
            }
        }
        /// <summary>
        /// when the jam number increases
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CurrentJam_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (JamViewModelEnum.JamNumber.ToString() == e.PropertyName)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    if (GameViewModel.Instance.CurrentJam != null)
                    {
                        this.bottomRightNameLbl.Content = "Jam " + GameViewModel.Instance.CurrentJam.JamNumber.ToString();
                        this.JamLblDefault.Content = "  Jam " + GameViewModel.Instance.CurrentJam.JamNumber.ToString() + " ";
                    }
                    return null;
                }, null);
            }
            else if (JamViewModelEnum.TeamLeadingJam.ToString() == e.PropertyName)
            {
                //hides the arrow showing what team has lead jammer.
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    if (GameViewModel.Instance.CurrentJam != null)
                    {
                        if (GameViewModel.Instance.CurrentJam.TeamLeadingJam == Scoreboard.Library.Static.Enums.TeamNumberEnum.Team1)
                        {
                            this.JammerLeadLeft.Visibility = System.Windows.Visibility.Visible;
                            this.JammerLeadRight.Visibility = System.Windows.Visibility.Hidden;

                            this.JammerLeadLeftDefault.Visibility = System.Windows.Visibility.Visible;
                            this.JammerLeadRightDefault.Visibility = System.Windows.Visibility.Hidden;
                        }
                        else if (GameViewModel.Instance.CurrentJam.TeamLeadingJam == Scoreboard.Library.Static.Enums.TeamNumberEnum.Team2)
                        {
                            this.JammerLeadRight.Visibility = System.Windows.Visibility.Visible;
                            this.JammerLeadLeft.Visibility = System.Windows.Visibility.Hidden;

                            this.JammerLeadRightDefault.Visibility = System.Windows.Visibility.Visible;
                            this.JammerLeadLeftDefault.Visibility = System.Windows.Visibility.Hidden;
                        }
                    }
                    return null;
                }, null);
            }
            else if (JamViewModelEnum.JammerT1.ToString() == e.PropertyName ||
              JamViewModelEnum.JammerT2.ToString() == e.PropertyName)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    try
                    {
                        if (GameViewModel.Instance.CurrentJam.JammerT1 != null)
                        {
                            this.jammerTeam1.Content = GameViewModel.Instance.CurrentJam.JammerT1.SkaterName;
                            this.jammerTeam1.Visibility = System.Windows.Visibility.Visible;

                            this.team1JammerLblDefault.Content = GameViewModel.Instance.CurrentJam.JammerT1.SkaterName;
                            this.team1JammerLblDefault.Visibility = System.Windows.Visibility.Visible;
                            //loading picture
                            if (PolicyViewModel.Instance.ShowActiveJammerPictures)
                            {
                                if (!String.IsNullOrEmpty(GameViewModel.Instance.CurrentJam.JammerT1.SkaterPictureLocation))
                                {
                                    BitmapImage bi = new BitmapImage();
                                    bi.BeginInit();
                                    bi.UriSource = new Uri(GameViewModel.Instance.CurrentJam.JammerT1.SkaterPictureLocation);
                                    bi.CacheOption = BitmapCacheOption.OnLoad;
                                    bi.EndInit();
                                    this.team1JammerimgDefault.Source = bi;
                                }
                            }
                        }
                        else
                        {
                            this.jammerTeam1.Content = "";
                            this.jammerTeam1.Visibility = System.Windows.Visibility.Hidden;

                            this.team1JammerLblDefault.Content = "";
                            if (PolicyViewModel.Instance.ShowActiveJammerPictures)
                                this.team1JammerimgDefault.Source = null;
                            this.team1JammerLblDefault.Visibility = System.Windows.Visibility.Hidden;
                        }
                        if (GameViewModel.Instance.CurrentJam.JammerT2 != null)
                        {
                            this.jammerTeam2.Content = GameViewModel.Instance.CurrentJam.JammerT2.SkaterName;
                            this.jammerTeam2.Visibility = System.Windows.Visibility.Visible;

                            this.team2JammerLblDefault.Content = GameViewModel.Instance.CurrentJam.JammerT2.SkaterName;
                            this.team2JammerLblDefault.Visibility = System.Windows.Visibility.Visible;
                            //loading picture
                            if (PolicyViewModel.Instance.ShowActiveJammerPictures)
                            {
                                if (!String.IsNullOrEmpty(GameViewModel.Instance.CurrentJam.JammerT2.SkaterPictureLocation))
                                {
                                    BitmapImage bi = new BitmapImage();
                                    bi.BeginInit();
                                    bi.UriSource = new Uri(GameViewModel.Instance.CurrentJam.JammerT2.SkaterPictureLocation);
                                    bi.CacheOption = BitmapCacheOption.OnLoad;
                                    bi.EndInit();
                                    team2JammerimgDefault.Source = bi;
                                }
                            }
                        }
                        else
                        {
                            this.jammerTeam2.Content = "";
                            this.jammerTeam2.Visibility = System.Windows.Visibility.Hidden;

                            this.team2JammerLblDefault.Content = "";
                            this.team2JammerLblDefault.Visibility = System.Windows.Visibility.Hidden;
                            if (PolicyViewModel.Instance.ShowActiveJammerPictures)
                                this.team2JammerimgDefault.Source = null;
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorViewModel.Save(exception, GetType());
                    }
                    return null;
                }, null);
            }

        }
        /// <summary>
        /// jamclock iscoutnring down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void JamClock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (StopWatchWrapperEnum.TimeRemaining.ToString() == e.PropertyName)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    this.bottomRightTimeLbl.Content = TimeSpan.FromMilliseconds(GameViewModel.Instance.CurrentJam.JamClock.TimeRemaining).ToString(@"m\:ss");

                    this.JamTimeLblDefault.Content = TimeSpan.FromMilliseconds(GameViewModel.Instance.CurrentJam.JamClock.TimeRemaining).ToString(@"m\:ss");
                    return null;
                }, null);
            }
            else if (StopWatchWrapperEnum.IsClockAtZero.ToString() == e.PropertyName)
            {
                if (GameViewModel.Instance.CurrentJam != null && GameViewModel.Instance.CurrentJam.JamClock.IsClockAtZero)
                    if (GameViewModel.Instance.CurrentPeriod == 2 && GameViewModel.Instance.PeriodClock.IsClockAtZero)
                    {
                        if (PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.WFTDA || PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.WFTDA_2010)
                        {
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                   (DispatcherOperationCallback)delegate(object arg)
                   {
                       this.bottomLeftNameLbl.Content = PolicyViewModel.Instance.SecondIntermissionNameText;
                       this.PeriodLineUpLbl.Content = PolicyViewModel.Instance.SecondIntermissionNameText;
                       this.PeriodTimeOutLbl.Content = PolicyViewModel.Instance.SecondIntermissionNameText;
                       this.PeriodLblDefault.Content = PolicyViewModel.Instance.SecondIntermissionNameText;
                       this.JamPeriodShowingGrid.Visibility = System.Windows.Visibility.Hidden;
                       this.FinalGameOverGrid.Visibility = System.Windows.Visibility.Visible;
                       this.LineUpGrid.Visibility = System.Windows.Visibility.Hidden;
                       this.IntermissionGrid.Visibility = System.Windows.Visibility.Hidden;
                       this.TimeOutShowingGrid.Visibility = System.Windows.Visibility.Hidden;
                       this.PeriodLblDefault.Content = PolicyViewModel.Instance.SecondIntermissionNameText;
                       this.FinalLbl.Content = PolicyViewModel.Instance.SecondIntermissionNameText;
                       return null;
                   }, null);

                        }
                    }
            }
        }
        /// <summary>
        /// period clock is counting down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PeriodClock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (StopWatchWrapperEnum.TimeRemaining.ToString() == e.PropertyName)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    this.PeriodTimeOutClock.Content = TimeSpan.FromMilliseconds(GameViewModel.Instance.PeriodClock.TimeRemaining).ToString(@"m\:ss");
                    this.PeriodLineUpClock.Content = TimeSpan.FromMilliseconds(GameViewModel.Instance.PeriodClock.TimeRemaining).ToString(@"m\:ss");
                    this.bottomLeftTimeLbl.Content = TimeSpan.FromMilliseconds(GameViewModel.Instance.PeriodClock.TimeRemaining).ToString(@"m\:ss");

                    this.PeriodTimeLblDefault.Content = TimeSpan.FromMilliseconds(GameViewModel.Instance.PeriodClock.TimeRemaining).ToString(@"m\:ss");
                    this.ImageMainClock.Content = TimeSpan.FromMilliseconds(GameViewModel.Instance.PeriodClock.TimeRemaining).ToString(@"m\:ss");
                    return null;
                }, null);
            }

        }
        /// <summary>
        /// sets up the view ona new game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Instance_OnNewGame(object sender, EventArgs e)
        {
            setupView();
        }
        /// <summary>
        /// team 2 property changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Team2_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (TeamViewModelEnum.TeamName.ToString() == e.PropertyName ||
                TeamViewModelEnum.TimeOutsLeft.ToString() == e.PropertyName)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    this.team2Lbl.Content = GameViewModel.Instance.Team2.TeamName;
                    this.team2TimeoutsLbl.Content = GameViewModel.Instance.Team2.TimeOutsLeft;

                    this.team2LblDefault.Content = GameViewModel.Instance.Team2.TeamName;
                    this.timeOutsTeam2Default.Content = GameViewModel.Instance.Team2.TimeOutsLeft;
                    return null;
                }, null);
            }
            else if (TeamViewModelEnum.Logo.ToString() == e.PropertyName)
            {
                loadTeam2Image();
            }
        }

        private void loadTeam2Image()
        {
            try
            {
                if (GameViewModel.Instance.Team2.Logo != null && File.Exists(GameViewModel.Instance.Team2.Logo.SaveLocation))
                {
                    BitmapImage image = new BitmapImage(new Uri(GameViewModel.Instance.Team2.Logo.SaveLocation));
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object arg)
                    {
                        this.team2img.Source = image;
                        this.team2imgDefault.Source = image;
                        return null;
                    }, null);
                }
                else
                {
                    this.team2img.Source = null;

                    this.team2imgDefault.Source = null;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// team 1 property changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Team1_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (TeamViewModelEnum.TeamName.ToString() == e.PropertyName ||
                TeamViewModelEnum.TimeOutsLeft.ToString() == e.PropertyName
                )
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    this.team1Lbl.Content = GameViewModel.Instance.Team1.TeamName;
                    this.team1TimeoutsLbl.Content = GameViewModel.Instance.Team1.TimeOutsLeft;

                    this.team1LblDefault.Content = GameViewModel.Instance.Team1.TeamName;
                    this.timeOutsTeam1Default.Content = GameViewModel.Instance.Team1.TimeOutsLeft;
                    return null;
                }, null);
            }
            else if (TeamViewModelEnum.Logo.ToString() == e.PropertyName)
            {
                loadTeam1Image();
            }
        }

        private void loadTeam1Image()
        {
            try
            {
                if (GameViewModel.Instance.Team1.Logo != null && File.Exists(GameViewModel.Instance.Team1.Logo.SaveLocation))
                {

                    BitmapImage image = new BitmapImage(new Uri(GameViewModel.Instance.Team1.Logo.SaveLocation));
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object arg)
                    {
                        try {
                        this.team1img.Source = image;
                        this.team1imgDefault.Source = image;
                                     }
                        catch (Exception exception)
                        {
                            ErrorViewModel.Save(exception, GetType());
                        }
                        return null;
                    }, null);
                }
                else
                {
                    this.team1img.Source = null;

                    this.team1imgDefault.Source = null;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// the pocliy view model gets changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void InstancePolicyViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PolicyViewModelEnum.EnableAdChange.ToString() == e.PropertyName)
            {
                if (PolicyViewModel.Instance.EnableAdChange && PolicyViewModel.Instance.AdChangeUseLineUpClock)
                {
                    if (GameViewModel.Instance.CurrentAdvertisement != null)
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                   (DispatcherOperationCallback)delegate(object arg)
                   {
                       try
                       {
                           BitmapImage bi = new BitmapImage();
                           bi.BeginInit();
                           bi.UriSource = new Uri(GameViewModel.Instance.CurrentAdvertisement.FileLocation);
                           bi.CacheOption = BitmapCacheOption.OnLoad;
                           bi.EndInit();
                           lineUpImage.Source = bi;
                           lineUpImageDefault.Source = bi;
                       }
                       catch (Exception exception)
                       {
                           ErrorViewModel.Save(exception, GetType());
                       }
                       return null;
                   }, null);
                    }
                }
                else
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (DispatcherOperationCallback)delegate(object arg)
                       {
                           lineUpImage.Source = null;
                           lineUpImageDefault.Source = null;
                           return null;
                       }, null);
                }
            }
            else if (PolicyViewModelEnum.SecondIntermissionNameConfirmedText.ToString() == e.PropertyName)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        FinalLbl.Content = PolicyViewModel.Instance.SecondIntermissionNameConfirmedText;
                        return null;
                    }, null);
            }
            else if (PolicyViewModelEnum.ShowActiveClockDuringSlideShow.ToString() == e.PropertyName)
            {//shows the clock during the slide show.  Incase a clock is wanted or needed.
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (DispatcherOperationCallback)delegate(object arg)
                       {
                           if (PolicyViewModel.Instance.ShowActiveClockDuringSlideShow)
                               this.ImageMainClock.Visibility = System.Windows.Visibility.Visible;
                           else
                               this.ImageMainClock.Visibility = System.Windows.Visibility.Hidden;
                           return null;
                       }, null);
            }
        }
        /// <summary>
        /// instance property changed for the gameviewmodel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (GameViewModelEnum.CurrentTeam1Score.ToString() == e.PropertyName ||
                GameViewModelEnum.CurrentTeam2Score.ToString() == e.PropertyName ||
                GameViewModelEnum.CurrentPeriod.ToString() == e.PropertyName)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
               (DispatcherOperationCallback)delegate(object arg)
               {
                   try
                   {
                       this.team1ScoreLbl.Content = GameViewModel.Instance.CurrentTeam1Score;
                       this.team2ScoreLbl.Content = GameViewModel.Instance.CurrentTeam2Score;

                       this.team1ScoreLblDefault.Content = GameViewModel.Instance.CurrentTeam1Score;
                       this.team2ScoreLblDefault.Content = GameViewModel.Instance.CurrentTeam2Score;


                       StringBuilder sb = new StringBuilder();
                       if (!String.IsNullOrEmpty(GameViewModel.Instance.Team1.TeamName))
                           sb.Append(GameViewModel.Instance.Team1.TeamName[0] + ":");
                       else
                           sb.Append("H:");

                       sb.Append(GameViewModel.Instance.CurrentTeam1Score + "/");
                       if (!String.IsNullOrEmpty(GameViewModel.Instance.Team2.TeamName))
                           sb.Append(GameViewModel.Instance.Team2.TeamName[0] + ":");
                       else
                           sb.Append("A:");
                       sb.Append(GameViewModel.Instance.CurrentTeam2Score);
                       this.ImageScores.Content = sb.ToString();
                       setupPeriodClocks();
                   }
                   catch (Exception exception)
                   {
                       ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                   }
                   return null;
               }, null);
            }
            else if (GameViewModelEnum.CurrentTeam1JamScore.ToString() == e.PropertyName ||
           GameViewModelEnum.CurrentTeam2JamScore.ToString() == e.PropertyName)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object arg)
                {
                    try
                    {
                        this.team1JamScoreLbl.Content = GameViewModel.Instance.CurrentTeam1JamScore;
                        this.team2JamScoreLbl.Content = GameViewModel.Instance.CurrentTeam2JamScore;

                        this.team1JamScoreLblDefault.Content = GameViewModel.Instance.CurrentTeam1JamScore;
                        this.team2JamScoreLblDefault.Content = GameViewModel.Instance.CurrentTeam2JamScore;
                    }
                    catch (Exception exception)
                    {
                        ErrorViewModel.Save(exception, GetType());
                    }
                    return null;
                }, null);
            }
            else if (GameViewModelEnum.CurrentAdvertisement.ToString() == e.PropertyName)
            {
                if (PolicyViewModel.Instance.EnableAdChange && PolicyViewModel.Instance.AdChangeUseLineUpClock)
                {
                    if (GameViewModel.Instance.CurrentAdvertisement != null)
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        try
                        {
                            BitmapImage bi = new BitmapImage();
                            bi.BeginInit();
                            bi.UriSource = new Uri(GameViewModel.Instance.CurrentAdvertisement.FileLocation);
                            bi.CacheOption = BitmapCacheOption.OnLoad;
                            bi.EndInit();
                            lineUpImage.Source = bi;
                            lineUpImageDefault.Source = bi;
                        }
                        catch (Exception exception)
                        {
                            ErrorViewModel.Save(exception, GetType());
                            if (exception.Message.Contains("Could not find file"))
                            {
                                var tempAd = GameViewModel.Instance.Advertisements.Where(x => x.AdvertGameId == GameViewModel.Instance.CurrentAdvertisement.AdvertGameId).FirstOrDefault();
                                if (tempAd != null)
                                    tempAd.IsShowing = false;
                            }
                        }
                        return null;
                    }, null);
                    }
                }
            }
            else if (GameViewModelEnum.CurrentSlideShowSlide.ToString() == e.PropertyName)
            {
                if (GameViewModel.Instance.CurrentSlideShowSlide != null && !String.IsNullOrEmpty(GameViewModel.Instance.CurrentSlideShowSlide.FileLocation) && File.Exists(GameViewModel.Instance.CurrentSlideShowSlide.FileLocation))
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    try
                    {
                        if (GameViewModel.Instance.CurrentSlideShowSlide != null && File.Exists(GameViewModel.Instance.CurrentSlideShowSlide.FileLocation))
                        {
                            BitmapImage bi = new BitmapImage();
                            bi.BeginInit();
                            bi.UriSource = new Uri(GameViewModel.Instance.CurrentSlideShowSlide.FileLocation);
                            bi.CacheOption = BitmapCacheOption.OnLoad;
                            bi.EndInit();
                            //if its animated, we will use a different type of control
                            if (GameViewModel.Instance.CurrentSlideShowSlide.FileLocation.Contains(".gif"))
                            {
                                try
                                {
                                    ImageBehavior.SetAnimatedSource(MediaElementMainForSlideshow, bi);
                                    ImageMainForSlideshow.Visibility = System.Windows.Visibility.Hidden;
                                    MediaElementMainForSlideshow.Visibility = System.Windows.Visibility.Visible;
                                }
                                catch (Exception exception)
                                {
                                    ErrorViewModel.Save(exception, GetType());
                                }
                            }
                            else
                            {
                                ImageMainForSlideshow.Source = bi;
                                ImageMainForSlideshow.Visibility = System.Windows.Visibility.Visible;
                                MediaElementMainForSlideshow.Visibility = System.Windows.Visibility.Hidden;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorViewModel.Save(exception, GetType());
                    }
                    return null;
                }, null);
                }
            }
            else if (GameViewModelEnum.NameOfIntermission.ToString() == e.PropertyName)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (DispatcherOperationCallback)delegate(object arg)
                        {
                            this.PeriodLblDefault.Content = GameViewModel.Instance.NameOfIntermission;
                            this.IntermissionLbl.Content = GameViewModel.Instance.NameOfIntermission;
                            return null;
                        }, null);
            }

            else if (e.PropertyName == GameViewModelEnum.DisplayType.ToString())
            {
                //hits when the display type of the scoreboard changes.  
                //current is the slide show and the scoreboard.
                ApplyDisplayType();
            }
            else if (e.PropertyName == GameViewModelEnum.HasGameEnded.ToString())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (DispatcherOperationCallback)delegate(object arg)
                       {
                           this.JamPeriodShowingGrid.Visibility = System.Windows.Visibility.Hidden;
                           this.FinalGameOverGrid.Visibility = System.Windows.Visibility.Visible;
                           this.LineUpGrid.Visibility = System.Windows.Visibility.Hidden;
                           this.IntermissionGrid.Visibility = System.Windows.Visibility.Hidden;
                           this.TimeOutShowingGrid.Visibility = System.Windows.Visibility.Hidden;
                           this.PeriodLblDefault.Content = PolicyViewModel.Instance.SecondIntermissionNameConfirmedText;
                           this.FinalLbl.Content = PolicyViewModel.Instance.SecondIntermissionNameConfirmedText;
                           return null;
                       }, null);
            }

        }

        #region WindowEventsButtonClicks
        private void ClockWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                //removes the reference in the main window so we can spawn a lot of the clock views.
                Window owner = System.Windows.Application.Current.MainWindow;
                if (owner != null)
                    foreach (var scoreboard in ((MainWindow)owner).ClockViewArray)
                        if (scoreboard.ClockId == this.ClockId)
                        {
                            ((MainWindow)owner).ClockViewArray.Remove(scoreboard);
                            break;
                        }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// 2 clicks maximimizes or normal sizes window or allows for a drag and move.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleBarGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (WindowState == System.Windows.WindowState.Maximized)
                    WindowState = WindowState.Normal;
                else if (WindowState == System.Windows.WindowState.Normal)
                    WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    DragMove();
            }
        }
        /// <summary>
        /// shows controls of window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClockWindow_MouseEnter(object sender, MouseEventArgs e)
        {
            OpenCloseControls.Visibility = System.Windows.Visibility.Visible;
        }
        /// <summary>
        /// hides the controsl of the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClockWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            OpenCloseControls.Visibility = System.Windows.Visibility.Hidden;
        }
        /// <summary>
        /// changes view to normal of window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeViewButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Normal;
        }
        /// <summary>
        /// minimizeswindow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// closes window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
        /// <summary>
        /// maximises window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaximizeButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }
        #endregion


        /// <summary>
        /// changes the theme of the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseThemesPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var theme = ((Theme)e.AddedItems[0]);
                if (GameViewModel.Instance.DisplayType == GameViewModelDisplayTypeEnum.Scoreboard)
                    PolicyViewModel.Instance.DefaultScoreboardTheme = theme.Name;
                else if (GameViewModel.Instance.DisplayType == GameViewModelDisplayTypeEnum.SlideShowForIntroductions)
                    PolicyViewModel.Instance.SkaterLineUpTheme = theme.Name;

                Logger.Instance.logMessage("setting choosing theme panel", LoggerEnum.message);
                PolicyViewModel.Instance.savePolicyToXml();
                ApplyDisplayType();
            }
        }



        private void wikiHelpLink_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(ScoreboardConfig.SCOREBOARD_SCOREBOARD_WIKI_URL);
            WebBrowser b = new WebBrowser();
            b.Navigate(ScoreboardConfig.SCOREBOARD_SCOREBOARD_WIKI_URL, "_blank", null, null);
        }

        private void uploadFileBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "Images (*.JPG;*.JPEG;*.PNG)|*.JPG;*.JPEG;*.PNG";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string filename = dlg.FileName;
                    uploadFileTextBox.Text = filename;
                    var info = new FileInfo(filename);
                    if (!String.IsNullOrEmpty(filename) && info.Exists)
                    {
                        try
                        {
                            Guid advertID = Guid.NewGuid();
                            string destinationFilePath = ScoreboardConfig.SAVE_BACKGROUND_IMAGES_FOLDER;
                            string destinationFileName = advertID.ToString().Replace("-", "") + System.IO.Path.GetExtension(filename);

                            DirectoryInfo dir = new DirectoryInfo(ScoreboardConfig.SAVE_BACKGROUND_IMAGES_FOLDER);
                            if (!dir.Exists)
                                dir.Create();
                            Logger.Instance.logMessage("opening backgroundImage:" + destinationFilePath + destinationFileName, LoggerEnum.message);
                            Logger.Instance.logMessage("filename:" + filename, LoggerEnum.message);
                            File.Copy(filename, System.IO.Path.Combine(destinationFilePath, destinationFileName), true);
                            if (GameViewModel.Instance.ScoreboardSettings == null)
                            {
                                MessageBox.Show("Please create a new game before adding this background.", "Create New Game");
                            }
                            else
                                GameViewModel.Instance.ScoreboardSettings.BackgroundPictureLocation = destinationFilePath + destinationFileName;
                            var brush = new ImageBrush();
                            BitmapImage bi = new BitmapImage();
                            bi.BeginInit();
                            bi.UriSource = new Uri(destinationFilePath + destinationFileName);
                            bi.CacheOption = BitmapCacheOption.OnLoad;
                            bi.EndInit();
                            brush.ImageSource = bi;
                            this.Background = brush;
                        }
                        catch (Exception exception)
                        {
                            ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void deleteFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var theme = ThemeManager.GetAllScoreboardThemes().FirstOrDefault(x => x.Name == PolicyViewModel.Instance.DefaultScoreboardTheme);
                ChooseThemesPanel.SelectedItem = theme;
                if (this.TryFindResource("ClockViewBackgroundBrush").GetType() == typeof(LinearGradientBrush))
                    this.Background = (LinearGradientBrush)this.TryFindResource("ClockViewBackgroundBrush");
                else
                    this.Background = (ImageBrush)this.TryFindResource("ClockViewBackgroundBrush");

                uploadFileTextBox.Text = String.Empty;
                if (GameViewModel.Instance.ScoreboardSettings != null)
                {
                    GameViewModel.Instance.ScoreboardSettings.BackgroundPictureLocation = String.Empty;
                    GameViewModel.Instance.ScoreboardSettings.BackgroundPictureCompressed = null;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void ShowActiveJammerPictures_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.ShowActiveJammerPictures = !PolicyViewModel.Instance.ShowActiveJammerPictures;
            PolicyViewModel.Instance.savePolicyToXml();
        }

    }
}