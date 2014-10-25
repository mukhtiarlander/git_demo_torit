using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using Microsoft.Windows.Controls.Ribbon;
using RDNScoreboard.Views;
using System.Windows.Threading;
using RDNScoreboard.Themes;
using Scoreboard.Library.ViewModel;
using Scoreboard.Library.Static.Enums;
using Scoreboard.Library.StopWatch;

using System.Text.RegularExpressions;
using RDN.Utilities.Config;
using RDNScoreboard.Code;
using System.Windows.Documents;
using RDNScoreboard.Views.Tabs;
using System.Diagnostics;
using RDNScoreboard.Server;
using Scoreboard.Library.ViewModel.Members;
using Scoreboard.Library.Classes.Reports.Excel;
using WPFLocalizeExtension.Engine;
using System.Globalization;
using RDN.Portable.Util;
using Scoreboard.Library.Classes.Reports.RDNation;
using RDN.Utilities.Util;
using Scoreboard.Library.Util;
using RDN.Utilities.Strings;
using NAudio.Wave;

namespace RDNScoreboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        /// <summary>
        /// have to make sure we sound the buzzer for each rule set, so if the period ends and the game doesn't
        /// we set this flag to make sure we sound the buzzer at the end of the jam.
        /// </summary>
        bool _soundBuzzerAtEndOfJam;
        SlideShowTab _slideShow;
        AdvertisementTab _advertTab;
        TeamManager teamManager;
        PolicyView _policyWindow;
        ServerView _serverWindow;
        public List<ClockView> ClockViewArray;
        //public ClockView ClockView;

        public WebServer _WebServer;
        Regex findIpAddress = new Regex(@"^(([01]?\d\d?|2[0-4]\d|25[0-5])\.){3}([01]?\d\d?|25[0-5]|2[0-4]\d)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        System.Net.IPAddress _IpAddress;

        SettingsView settingsView;

        /// <summary>
        /// actual position of the cursor when it moves inside the texts boxes on edit mode. 
        /// Had to save it as an int because the clocks when they go up or down
        /// change the position when the count changes.
        /// </summary>
        int _currentCursorPosition;
        /// <summary>
        /// enum for checking which value is actually selected in edit mode
        /// </summary>
        enum _TextBoxEnum { None, PeriodTime, JamTime, LineUpTime, IntermissionTime, IntermissionName };
        _TextBoxEnum _currentTextBoxSelected;
        private static bool _canExecuteAnyKeyBinding = true;

        private static RoutedUICommand _startJam = new RoutedUICommand();
        private static RoutedUICommand _endJam = new RoutedUICommand();
        private static RoutedUICommand _officialTimeOut = new RoutedUICommand();
        private static RoutedUICommand _team1ScoreUp = new RoutedUICommand();
        private static RoutedUICommand _team1ScoreDown = new RoutedUICommand();
        private static RoutedUICommand _team1TimeOut = new RoutedUICommand();
        private static RoutedUICommand _team1JammerLead = new RoutedUICommand();
        private static RoutedUICommand _team2ScoreUp = new RoutedUICommand();
        private static RoutedUICommand _team2ScoreDown = new RoutedUICommand();
        private static RoutedUICommand _team2TimeOut = new RoutedUICommand();
        private static RoutedUICommand _team2JammerLead = new RoutedUICommand();


        public MainWindow()
        {
            try
            {
                InitializeComponent();

                Window.Title = "Control Panel - " + ScoreboardConfig.SCOREBOARD_NAME;

                Languages.ItemsSource = LanguagesUtil.Languages;
                LanguagesHolder.SelectedItem = LanguagesUtil.Languages[0];
                LanguagesHolder.SelectionChanged += LanguagesHolder_SelectionChanged;
                //ThemesInBox.ItemsSource = ThemeManager.GetThemes();
                Application.Current.ApplyTheme("WhistlerBlue");

                setupViewForGame();
                //must open after policys because the keyboard shortcuts are controled from the policys view.
                teamManager = new TeamManager();

                ClockView c = new ClockView();
                ClockViewArray = new List<ClockView>();
                ClockViewArray.Add(c);

                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

                //opens the window to ask if they are testing, going live etc.
                InitialWindow askIfTestingOnline = new InitialWindow();
                askIfTestingOnline.Show();
                _slideShow = new Views.Tabs.SlideShowTab();
                SlideShowFrame.Content = _slideShow;
                _advertTab = new Views.Tabs.AdvertisementTab();
                AdvertisementFrame.Content = _advertTab;



                CreateAndStartWebServer();

                if (ScoreboardCrashed.DidScoreboardJustCrash())
                {
                    if (MessageBox.Show("It seems the Scoreboard just crashed.  Would you like to load the last game?", "Scoreboard Crashed?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            ScoreboardCrashed.LoadTempGameFile();
                            setupViewForGame();
                            enableButtonsToOnMode();
                            teamManager.setupTeamManager();
                            foreach (var scoreboard in ClockViewArray)
                                scoreboard.setupView();
                            SaveGame.IsEnabled = true;
                        }
                        catch (Exception exception)
                        {
                            ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                        }
                    }
                    else
                        return;
                }

                ScoreboardCrashed.Opened();
                if (!String.IsNullOrEmpty(PolicyViewModel.Instance.DefaultLanguage))
                {
                    LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(PolicyViewModel.Instance.DefaultLanguage);
                }
                else
                {
                    LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo("en-US");
                    PolicyViewModel.Instance.DefaultLanguage = "en-US";
                    Logger.Instance.logMessage("setting language", LoggerEnum.message);
                    PolicyViewModel.Instance.savePolicyToXml();
                }




            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        void LanguagesHolder_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(LanguagesUtil.ConvertToAbbreviation(e.NewValue.ToString()));
                PolicyViewModel.Instance.DefaultLanguage = LanguagesUtil.ConvertToAbbreviation(e.NewValue.ToString());
                Logger.Instance.logMessage("setting language2", LoggerEnum.message);
                PolicyViewModel.Instance.savePolicyToXml();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void CreateAndStartWebServer()
        {
            try
            {
                string myHost = System.Net.Dns.GetHostName();
                System.Net.IPHostEntry myIPs = System.Net.Dns.GetHostEntry(myHost);
                foreach (System.Net.IPAddress myIP in myIPs.AddressList)
                {
                    //http://packetlife.net/blog/2008/sep/24/169-254-0-0-addresses-explained/
                    if (findIpAddress.IsMatch(myIP.ToString()) && !myIP.ToString().Contains("169.254"))
                    {
                        _IpAddress = myIP;
                        break;
                    }
                }
                WebServer.Instance.IpAddress = _IpAddress;
                WebServer.Instance.Setup();
                WebServer.Instance.Start();
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, GetType());
            }
        }

        /// <summary>
        /// loads the main view from the chosen policies
        /// </summary>
        private void createViewFromPolicies()
        {
            try
            {
                PolicyViewModel.Instance.loadFromXmlPolicy();
                this.jamClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"mm\:ss");
                this.editJamClockText.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"mm\:ss");
                this.LineUpClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.LineUpClockPerJam).ToString(@"mm\:ss");
                this.editLineUpClockText.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.LineUpClockPerJam).ToString(@"mm\:ss");
                this.PeriodClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.PeriodClock).ToString(@"mm\:ss");
                this.editPeriodClockText.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.PeriodClock).ToString(@"mm\:ss");
                this.TimeOutsRemainingTeam1.Text = PolicyViewModel.Instance.TimeOutsPerPeriod.ToString();
                this.editTimeOutsRemainingTeam1Text.Text = PolicyViewModel.Instance.TimeOutsPerPeriod.ToString();
                this.TimeOutsRemainingTeam2.Text = PolicyViewModel.Instance.TimeOutsPerPeriod.ToString();
                this.editTimeOutsRemainingTeam2Text.Text = PolicyViewModel.Instance.TimeOutsPerPeriod.ToString();
                this.TimeOutClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.TimeOutClock).ToString(@"mm\:ss");
                this.IntermissionClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.IntermissionStartOfClockInMilliseconds).ToString(@"mm\:ss");
                this.editIntermissionClockText.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.IntermissionStartOfClockInMilliseconds).ToString(@"mm\:ss");

                setKeyboardShortcuts();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }

        }

        private void clearKeyboardShortcuts()
        {
            CommandBindings.Clear();
        }

        /// <summary>
        /// sets all the keyboard shortcuts for the window.
        /// </summary>
        private void setKeyboardShortcuts()
        {

            CommandBindings.Clear();
            shortcutsTextBlock.Inlines.Clear();
            Run startText = new Run("Shortcuts: ");
            startText.FontWeight = FontWeights.Bold;
            shortcutsTextBlock.Inlines.Add(startText);
            try
            {
                _startJam.Text = "Starts the Jam for the Game.";
                _startJam.InputGestures.Clear();
                _startJam.InputGestures.Add(new AnyKeyGesture(StringUtil.ResolveKey(PolicyViewModel.Instance.StartJamKeyShortcut)));
                startJamBtn.Command = _startJam;
                CommandBindings.Add(new CommandBinding(_startJam, startJamBtn_Click, CanExecuteKeyBindingsHandler));

                Run text = new Run("Start-");
                Run key = new Run(PolicyViewModel.Instance.StartJamKeyShortcut.ToString());
                key.FontWeight = FontWeights.Bold;
                shortcutsTextBlock.Inlines.Add(text);
                shortcutsTextBlock.Inlines.Add(key);

                Logger.Instance.logMessage("adding start jam shortcut", LoggerEnum.message);

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            try
            {
                _endJam.InputGestures.Clear();
                _endJam.InputGestures.Add(new AnyKeyGesture(StringUtil.ResolveKey(PolicyViewModel.Instance.StopJamKeyShortcut)));
                stopJamBtn.Command = _endJam;
                CommandBindings.Add(new CommandBinding(_endJam, stopJamBtn_Click));

                Run text = new Run(" Stop-");
                Run key = new Run(PolicyViewModel.Instance.StopJamKeyShortcut.ToString());
                key.FontWeight = FontWeights.Bold;
                shortcutsTextBlock.Inlines.Add(text);
                shortcutsTextBlock.Inlines.Add(key);
                Logger.Instance.logMessage("adding stop jam shortcut", LoggerEnum.message);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            try
            {
                _officialTimeOut.InputGestures.Clear();
                _officialTimeOut.InputGestures.Add(new AnyKeyGesture(StringUtil.ResolveKey(PolicyViewModel.Instance.OfficialTimeOutKeyShortcut)));
                timeOutBtn.Command = _officialTimeOut;
                CommandBindings.Add(new CommandBinding(_officialTimeOut, timeOutBtn_Click));

                Run text = new Run(" OffiTO-");
                Run key = new Run(PolicyViewModel.Instance.OfficialTimeOutKeyShortcut.ToString());
                key.FontWeight = FontWeights.Bold;
                shortcutsTextBlock.Inlines.Add(text);
                shortcutsTextBlock.Inlines.Add(key);
                Logger.Instance.logMessage("adding official time out shortcut", LoggerEnum.message);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            try
            {
                _team1JammerLead.InputGestures.Clear();
                _team1JammerLead.InputGestures.Add(new AnyKeyGesture(StringUtil.ResolveKey(PolicyViewModel.Instance.Team1LeadJammerKeyShortcut)));
                team1JammerLead.Command = _team1JammerLead;
                CommandBindings.Add(new CommandBinding(_team1JammerLead, team1JammerLead_Click));

                Run text = new Run(" T1LeadJam-");
                Run key = new Run(PolicyViewModel.Instance.Team1LeadJammerKeyShortcut.ToString());
                key.FontWeight = FontWeights.Bold;
                shortcutsTextBlock.Inlines.Add(text);
                shortcutsTextBlock.Inlines.Add(key);
                Logger.Instance.logMessage("adding jammer1 lead shortcut", LoggerEnum.message);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            try
            {
                _team1ScoreDown.InputGestures.Clear();
                _team1ScoreDown.InputGestures.Add(new AnyKeyGesture(StringUtil.ResolveKey(PolicyViewModel.Instance.Team1ScoreDownKeyShortcut)));
                Team1ScoreDownBtn.Command = _team1ScoreDown;
                CommandBindings.Add(new CommandBinding(_team1ScoreDown, Team1ScoreDownBtn_Click));

                Run text = new Run(" T1-1-");
                Run key = new Run(PolicyViewModel.Instance.Team1ScoreDownKeyShortcut.ToString());
                key.FontWeight = FontWeights.Bold;
                shortcutsTextBlock.Inlines.Add(text);
                shortcutsTextBlock.Inlines.Add(key);
                Logger.Instance.logMessage("adding team1 score down shortcut", LoggerEnum.message);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            try
            {
                _team1ScoreUp.InputGestures.Clear();
                _team1ScoreUp.InputGestures.Add(new AnyKeyGesture(StringUtil.ResolveKey(PolicyViewModel.Instance.Team1ScoreUpKeyShortcut)));
                Team1ScoreUpBtn.Command = _team1ScoreUp;
                CommandBindings.Add(new CommandBinding(_team1ScoreUp, Team1ScoreUpBtn_Click));

                Run text = new Run(" T1+1-");
                Run key = new Run(PolicyViewModel.Instance.Team1ScoreUpKeyShortcut.ToString());
                key.FontWeight = FontWeights.Bold;
                shortcutsTextBlock.Inlines.Add(text);
                shortcutsTextBlock.Inlines.Add(key);
                Logger.Instance.logMessage("adding team 1 score up shortcut", LoggerEnum.message);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            try
            {
                _team1TimeOut.InputGestures.Clear();
                _team1TimeOut.InputGestures.Add(new AnyKeyGesture(StringUtil.ResolveKey(PolicyViewModel.Instance.Team1TimeOutKeyShortcut)));
                timeOutTeam1Btn.Command = _team1TimeOut;
                CommandBindings.Add(new CommandBinding(_team1TimeOut, timeOutTeam1Btn_Click));

                Run text = new Run(" T1TO-");
                Run key = new Run(PolicyViewModel.Instance.Team1TimeOutKeyShortcut.ToString());
                key.FontWeight = FontWeights.Bold;
                shortcutsTextBlock.Inlines.Add(text);
                shortcutsTextBlock.Inlines.Add(key);
                Logger.Instance.logMessage("adding team 1 timeout shortcut", LoggerEnum.message);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            try
            {
                _team2JammerLead.InputGestures.Clear();
                _team2JammerLead.InputGestures.Add(new AnyKeyGesture(StringUtil.ResolveKey(PolicyViewModel.Instance.Team2LeadJammerKeyShortcut)));
                team2JammerLead.Command = _team2JammerLead;
                CommandBindings.Add(new CommandBinding(_team2JammerLead, team2JammerLead_Click));

                Run text = new Run(" T2LeadJam-");
                Run key = new Run(PolicyViewModel.Instance.Team2LeadJammerKeyShortcut.ToString());
                key.FontWeight = FontWeights.Bold;
                shortcutsTextBlock.Inlines.Add(text);
                shortcutsTextBlock.Inlines.Add(key);
                Logger.Instance.logMessage("adding team 2 lead jammer shortcut", LoggerEnum.message);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            try
            {
                _team2ScoreDown.InputGestures.Clear();
                _team2ScoreDown.InputGestures.Add(new AnyKeyGesture(StringUtil.ResolveKey(PolicyViewModel.Instance.Team2ScoreDownKeyShortcut)));
                Team2ScoreDownBtn.Command = _team2ScoreDown;
                CommandBindings.Add(new CommandBinding(_team2ScoreDown, Team2ScoreDownBtn_Click));
                Run text = new Run(" T2-1-");
                Run key = new Run(PolicyViewModel.Instance.Team2ScoreDownKeyShortcut.ToString());
                key.FontWeight = FontWeights.Bold;
                shortcutsTextBlock.Inlines.Add(text);
                shortcutsTextBlock.Inlines.Add(key);

                Logger.Instance.logMessage("adding team 2 score down shortcut", LoggerEnum.message);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            try
            {
                _team2ScoreUp.InputGestures.Clear();
                _team2ScoreUp.InputGestures.Add(new AnyKeyGesture(StringUtil.ResolveKey(PolicyViewModel.Instance.Team2ScoreUpKeyShortcut)));
                Team2ScoreUpBtn.Command = _team2ScoreUp;
                CommandBindings.Add(new CommandBinding(_team2ScoreUp, Team2ScoreUpBtn_Click));
                Run text = new Run(" T2+1-");
                Run key = new Run(PolicyViewModel.Instance.Team2ScoreUpKeyShortcut.ToString());
                key.FontWeight = FontWeights.Bold;
                shortcutsTextBlock.Inlines.Add(text);
                shortcutsTextBlock.Inlines.Add(key);

                Logger.Instance.logMessage("adding team 2 score up shortcut", LoggerEnum.message);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            try
            {
                _team2TimeOut.InputGestures.Clear();
                _team2TimeOut.InputGestures.Add(new AnyKeyGesture(StringUtil.ResolveKey(PolicyViewModel.Instance.Team2TimeOutKeyShortcut)));
                timeOutTeam2Btn.Command = _team2TimeOut;
                CommandBindings.Add(new CommandBinding(_team2TimeOut, timeOutTeam2Btn_Click));
                Run text = new Run(" T2TO-");
                Run key = new Run(PolicyViewModel.Instance.Team2TimeOutKeyShortcut.ToString());
                key.FontWeight = FontWeights.Bold;
                shortcutsTextBlock.Inlines.Add(text);
                shortcutsTextBlock.Inlines.Add(key);

                Logger.Instance.logMessage("adding team 2 timeout shortcut", LoggerEnum.message);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        /// <summary>
        /// checks to see if the keybindings are allowed to be executed or not.
        /// Mainly say we can't execute keybindings if user is typing something on main screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanExecuteKeyBindingsHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_canExecuteAnyKeyBinding)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }



        /// <summary>
        /// loads the game on click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadGame_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(ScoreboardConfig.SAVE_GAMES_FILE_LOCATION))
                Directory.CreateDirectory(ScoreboardConfig.SAVE_GAMES_FILE_LOCATION);
            try
            {
                var dlg = new OpenFileDialog();
                dlg.Filter = "Xml Documents (*.xml)|*.xml";
                dlg.CheckFileExists = true;
                dlg.InitialDirectory = ScoreboardConfig.SAVE_GAMES_FILE_LOCATION;

                bool? result = dlg.ShowDialog();
                if (result == false)
                    return;

                Logger.Instance.logMessage("Loading game from Old", LoggerEnum.message);
                GameViewModel.Instance.loadGameFromXml(dlg.FileName);
                setupViewForGame();
                enableButtonsToOnMode();
                teamManager.setupTeamManager();
                _advertTab.Instance_OnNewGame(null, null);
                foreach (var scoreboard in ClockViewArray)
                    scoreboard.setupView();
                SaveGame.IsEnabled = true;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }
        private void SaveSendGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Instance.logMessage("Saving the Game", LoggerEnum.message);
                if (!Directory.Exists(ScoreboardConfig.SAVE_GAMES_FILE_LOCATION))
                    Directory.CreateDirectory(ScoreboardConfig.SAVE_GAMES_FILE_LOCATION);
                var dlg = new SaveFileDialog();
                //changes the game name since its final.
                if (GameViewModel.Instance.HasGameEnded)
                    dlg.FileName = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly("Game Final " + GameViewModel.Instance.Team1.TeamName + "-" + GameViewModel.Instance.Team2.TeamName + " " + DateTime.Now.ToString("yyyy-MMMM-dd"));
                else
                    dlg.FileName = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly("Game " + GameViewModel.Instance.Team1.TeamName + "-" + GameViewModel.Instance.Team2.TeamName + " " + DateTime.Now.ToString("yyyy-MMMM-dd"));
                dlg.Filter = "Xml Documents (*.xml)|*.xml";
                dlg.OverwritePrompt = true;
                dlg.RestoreDirectory = true;
                dlg.InitialDirectory = ScoreboardConfig.SAVE_GAMES_FILE_LOCATION;

                bool? result = dlg.ShowDialog();

                if (result == true)
                    GameViewModel.Instance.saveGameAndPicturesToXml(dlg.FileName);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// creates a new game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateNewGame_Click(object sender, RoutedEventArgs e)
        {
            if (GameViewModel.Instance.HasGameStarted)
            {
                if (MessageBox.Show("Do you want to save this game?", "Save Game?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    SaveSendGame_Click(null, null);
                }
            }

            //game exists
            Logger.Instance.logMessage("creating new game", LoggerEnum.message);
            if (GameViewModel.Instance.GameId == new Guid())
            {
                //TODO:Show popup and ask if they want to create a new game
            }

            GameViewModel.Instance.createNewGame(PolicyViewModel.Instance.GameSelectionType);
            setupViewForGame();
            //buttons were enabled here because the edit mode also calls setup view for game and we don't want it to reenable buttons it disables.
            enableButtonsToOnMode();
            //disable the new and load game buttons so we don't accidentally do something.
            CreateNewGame.IsEnabled = false;
            LoadGame.IsEnabled = false;
            SaveGame.IsEnabled = true;
        }
        /// <summary>
        /// enables the default buttons to the on mode when we setup the game.
        /// </summary>
        private void enableButtonsToOnMode()
        {
            try
            {
                this.Team1ScoreDownBtn.IsEnabled = true;
                this.Team1ScoreUpBtn.IsEnabled = true;
                this.Team1ScoreUpBtn4.IsEnabled = true;
                this.Team1ScoreUpBtn5.IsEnabled = true;
                this.Team2ScoreDownBtn.IsEnabled = true;
                this.Team2ScoreUpBtn.IsEnabled = true;
                this.Team2ScoreUpBtn4.IsEnabled = true;
                this.Team2ScoreUpBtn5.IsEnabled = true;
                this.timeOutTeam1Btn.IsEnabled = true;
                this.timeOutTeam2Btn.IsEnabled = true;
                this.timeOutBtn.IsEnabled = true;
                this.officialReviewBtn.IsEnabled = true;
                this.startEditMode.IsEnabled = true;
                this.teamManagerButton.IsEnabled = true;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// sets up the game forthe view.
        /// </summary>
        private void setupViewForGame()
        {
            try
            {
                createViewFromPolicies();

                if (GameViewModel.Instance.CurrentJam != null)
                {
                    GameViewModel.Instance.CurrentJam.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentJam_PropertyChanged);
                    GameViewModel.Instance.CurrentJam.JamClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(JamClock_PropertyChanged);
                    this.LineUpNumberLbl.Text = GameViewModel.Instance.CurrentJam.JamNumber.ToString();
                    this.JamNumberLbl.Text = GameViewModel.Instance.CurrentJam.JamNumber.ToString();
                    this.editJamNumberText.Text = GameViewModel.Instance.CurrentJam.JamNumber.ToString();
                    //if its the very first jam of the day.
                    if (GameViewModel.Instance.CurrentJam.JamNumber == 1)
                    {
                        this.startIntermissionBtn.IsEnabled = true;
                    }
                    else if (GameViewModel.Instance.CurrentIntermission == GameViewModelIntermissionTypeEnum.HalfTimeIntermission || GameViewModel.Instance.CurrentIntermission == GameViewModelIntermissionTypeEnum.PostGameIntermission)
                    {
                        this.startIntermissionBtn.IsEnabled = false;
                    }
                }

                if (GameViewModel.Instance.CurrentLineUpClock != null)
                    GameViewModel.Instance.CurrentLineUpClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentLineUpClock_PropertyChanged);

                if (GameViewModel.Instance.IntermissionClock != null)
                    GameViewModel.Instance.IntermissionClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(IntermissionClock_PropertyChanged);

                //sets up the intermission button and labels.
                IntermissionLbl.Text = GameViewModel.Instance.NameOfIntermission;
                editIntermissionName.Text = GameViewModel.Instance.NameOfIntermission;
                startIntermissionBtn.Content = "Start " + GameViewModel.Instance.NameOfIntermission;

                if (GameViewModel.Instance.PeriodClock != null)
                    GameViewModel.Instance.PeriodClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(PeriodClock_PropertyChanged);

                GameViewModel.Instance.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Instance_PropertyChanged);
                SetupViewForTeamLoad();
                this.Team2Scorelbl.Text = GameViewModel.Instance.CurrentTeam2Score.ToString();
                this.Team1Scorelbl.Text = GameViewModel.Instance.CurrentTeam1Score.ToString();
                this.PeriodNumberLbl.Text = GameViewModel.Instance.CurrentPeriod.ToString();
                this.editPeriodNumberText.Text = GameViewModel.Instance.CurrentPeriod.ToString();
                this.gameNameText.Text = GameViewModel.Instance.GameName;
                Window.Title = GameViewModel.Instance.GameName + " - " + ScoreboardConfig.SCOREBOARD_NAME;
                this.gameLocationText.Text = GameViewModel.Instance.GameLocation;
                this.gameCityText.Text = GameViewModel.Instance.GameCity;
                this.gameStateText.Text = GameViewModel.Instance.GameState;

                Jammer1Lbl.Text = "";
                Jammer2Lbl.Text = "";
                Pivot1Lbl.Text = "";
                Pivot2Lbl.Text = "";

                if (GameViewModel.Instance.CurrentJam != null)
                {
                    if (GameViewModel.Instance.CurrentJam.PivotT1 != null)
                    {
                        this.Pivot1Lbl.Text = GameViewModel.Instance.CurrentJam.PivotT1.SkaterName;
                        this.team1PivotLead.IsEnabled = true;
                    }

                    if (GameViewModel.Instance.CurrentJam.PivotT2 != null)
                    {
                        this.team2PivotLead.IsEnabled = true;
                        this.Pivot2Lbl.Text = GameViewModel.Instance.CurrentJam.PivotT2.SkaterName;
                    }
                    if (GameViewModel.Instance.CurrentJam.JammerT1 != null)
                    {
                        this.team1JammerLead.IsEnabled = true;
                        this.Jammer1Lbl.Text = GameViewModel.Instance.CurrentJam.JammerT1.SkaterName;
                    }
                    if (GameViewModel.Instance.CurrentJam.JammerT2 != null)
                    {
                        this.team2JammerLead.IsEnabled = true;
                        this.Jammer2Lbl.Text = GameViewModel.Instance.CurrentJam.JammerT2.SkaterName;
                    }
                }
                this.jamClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"mm\:ss");
                this.editJamClockText.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"mm\:ss");
                this.LineUpClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.LineUpClockPerJam).ToString(@"mm\:ss");
                this.editLineUpClockText.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.LineUpClockPerJam).ToString(@"mm\:ss");
                this.PeriodClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.PeriodClock).ToString(@"mm\:ss");
                this.editPeriodClockText.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.PeriodClock).ToString(@"mm\:ss");
                this.TimeOutsRemainingTeam1.Text = PolicyViewModel.Instance.TimeOutsPerPeriod.ToString();
                this.editTimeOutsRemainingTeam1Text.Text = PolicyViewModel.Instance.TimeOutsPerPeriod.ToString();
                this.TimeOutsRemainingTeam2.Text = PolicyViewModel.Instance.TimeOutsPerPeriod.ToString();
                this.editTimeOutsRemainingTeam2Text.Text = PolicyViewModel.Instance.TimeOutsPerPeriod.ToString();
                this.TimeOutClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.TimeOutClock).ToString(@"mm\:ss");
                this.IntermissionClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.IntermissionStartOfClockInMilliseconds).ToString(@"mm\:ss");
                this.editIntermissionClockText.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.IntermissionStartOfClockInMilliseconds).ToString(@"mm\:ss");

                this.startJamBtn.IsEnabled = true;
                PolicyViewModel.Instance.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(PolicyInstance_PropertyChanged);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        public void SetupViewForTeamLoad()
        {
            try
            {
                GameViewModel.Instance.Team1.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Team1_PropertyChanged);
                GameViewModel.Instance.Team2.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Team2_PropertyChanged);
                this.team1Label.Text = GameViewModel.Instance.Team1.TeamName;
                this.team2Label.Text = GameViewModel.Instance.Team2.TeamName;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        void Team2_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (TeamViewModelEnum.TeamName.ToString() == e.PropertyName)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        this.team2Label.Text = GameViewModel.Instance.Team2.TeamName;
                        return null;
                    }, null);
                }
                else if (TeamViewModelEnum.TimeOutsLeft.ToString() == e.PropertyName)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        this.TimeOutsRemainingTeam2.Text = GameViewModel.Instance.Team2.TimeOutsLeft.ToString();
                        this.editTimeOutsRemainingTeam2Text.Text = GameViewModel.Instance.Team2.TimeOutsLeft.ToString();
                        return null;
                    }, null);

                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        void Team1_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (TeamViewModelEnum.TeamName.ToString() == e.PropertyName)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        this.team1Label.Text = GameViewModel.Instance.Team1.TeamName;
                        return null;
                    }, null);
                }
                else if (TeamViewModelEnum.TimeOutsLeft.ToString() == e.PropertyName)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        this.TimeOutsRemainingTeam1.Text = GameViewModel.Instance.Team1.TimeOutsLeft.ToString();
                        this.editTimeOutsRemainingTeam1Text.Text = GameViewModel.Instance.Team1.TimeOutsLeft.ToString();
                        return null;
                    }, null);

                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }



        private void FeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FeedbackView pop = new FeedbackView();
                pop.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                pop.Show();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }


        private void PolicyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_policyWindow == null || _policyWindow.IsLoaded == false)
                    _policyWindow = new PolicyView();
                _policyWindow.Show();
                _policyWindow.Activate();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void OpenServerWindow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_serverWindow == null || _serverWindow.IsLoaded == false)
                    _serverWindow = new ServerView();
                _serverWindow.Show();
                _serverWindow.Activate();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void teamManagerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                teamManager.Show();
                teamManager.Activate();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="validateNewGame">if true, will throw a message box asking if a new game should be created</param>
        public void startJamWithButtonClick(bool validateNewGame)
        {
            try
            {
                //since we start a new jam, make sure the buzzer doesn't hit.  
                //this mainly resets because when the period restarts we don't want the buzzer going off after each jam.
                _soundBuzzerAtEndOfJam = false;

                if (GameViewModel.Instance.ScoresTeam1 == null)
                {
                    Logger.Instance.logMessage("starting Jam without new game", LoggerEnum.message);
                    //no need to ask t start a new game, just do it.
                    if (!validateNewGame)
                        CreateNewGame_Click(null, null);
                    else if (MessageBox.Show("There is no current game, Would you like to create a new game?", "New Game?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        CreateNewGame_Click(null, null);
                    else
                        return;
                }
                //jam isn't running.
                if (!GameViewModel.Instance.CurrentJam.JamClock.IsRunning)
                {
                    Logger.Instance.logMessage("starting Jam without jam clock running", LoggerEnum.message);
                    //this is checking if its the first jam.
                    if (GameViewModel.Instance.HasGameStarted)
                    {
                        //if the period clock stoped because of a timeout.
                        GameViewModel.Instance.startFromTimeOut();
                    }
                    else //game triggers start here
                    {
                        GameViewModel.Instance.setOfficialStartTime();
                        GameViewModel.Instance.PeriodClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(PeriodClock_PropertyChanged);
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object arg)
                        {
                            this.PeriodClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.PeriodClock).ToString(@"mm\:ss");
                            this.editPeriodClockText.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.PeriodClock).ToString(@"mm\:ss");
                            return null;
                        }, null);
                    }
                    //create a new intermission.
                    //we haveto force a new intermission incase the jam started before the intermission was actually over.
                    if (GameViewModel.Instance.IntermissionClock.IsRunning)
                    {
                        GameViewModel.Instance.stopIntermission();

                        GameViewModel.Instance.createNewIntermission();
                        GameViewModel.Instance.IntermissionClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(IntermissionClock_PropertyChanged);

                        GameViewModel.Instance.startJam();
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object arg)
                        {
                            this.IntermissionClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.IntermissionStartOfClockInMilliseconds).ToString(@"mm\:ss");
                            this.editIntermissionClockText.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.IntermissionStartOfClockInMilliseconds).ToString(@"mm\:ss");
                            startIntermissionBtn.IsEnabled = false;
                            return null;
                        }, null);
                    }

                    GameViewModel.Instance.startJam();
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object arg)
                    {
                        this.LineUpClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.LineUpClockPerJam).ToString(@"mm\:ss");
                        startIntermissionBtn.IsEnabled = false;
                        gameOverBtn.IsEnabled = false;
                        stopJamBtn.IsEnabled = true;
                        stopJambyInjuryBtn.IsEnabled = true;
                        stopJamT1Btn.IsEnabled = true;
                        stopJamT2Btn.IsEnabled = true;

                        startJamBtn.IsEnabled = false;
                        timeOutTeam2Btn.IsEnabled = false;
                        timeOutTeam1Btn.IsEnabled = false;

                        team2PivotLead.IsEnabled = true;
                        team2JammerLead.IsEnabled = true;
                        team1PivotLead.IsEnabled = true;
                        team1JammerLead.IsEnabled = true;

                        Team1ScoreUpBtn.IsEnabled = true;
                        Team1ScoreUpBtn4.IsEnabled = true;
                        Team1ScoreUpBtn5.IsEnabled = true;
                        Team2ScoreUpBtn.IsEnabled = true;
                        Team2ScoreUpBtn4.IsEnabled = true;
                        Team2ScoreUpBtn5.IsEnabled = true;


                        Team1ScoreDownBtn.IsEnabled = true;
                        Team2ScoreDownBtn.IsEnabled = true;

                        return null;
                    }, null);
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// starts the jam with a button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startJamBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //need to make sure we see if its enabled so just in case this is a keyboard shortcut.
                if (startJamBtn.IsEnabled)
                {

                    CreateNewGame.IsEnabled = false;
                    LoadGame.IsEnabled = false;
                    startJamWithButtonClick(true);
                    Logger.Instance.logMessage("starting jam", LoggerEnum.message);
                    System.GC.Collect();
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// stops the jam
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void stopJamBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //because of commands, we must first see if the button is enabled through our logic.  If not, the button and command don't work.
                if (!stopJamBtn.IsEnabled)
                    return;
                //we are checking if a jam ended with another button rather than the general button.
                if (sender == null)
                    GameViewModel.Instance.stopJam();
                else if (sender == this)
                    GameViewModel.Instance.stopJam();
                else
                {
                    Button btn = (Button)sender;

                    if (btn.Name == "stopJamT1Btn")
                        GameViewModel.Instance.stopJam(didJammerT1StopJam: true);
                    else if (btn.Name == "stopJamT2Btn")
                        GameViewModel.Instance.stopJam(didJammerT2StopJam: true);
                    else if (btn.Name == "stopJambyInjuryBtn")
                        GameViewModel.Instance.stopJam(didJamEndwithInjury: true);
                    else
                        GameViewModel.Instance.stopJam();
                }

                GameViewModel.Instance.createLineUpClock();
                GameViewModel.Instance.CurrentLineUpClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentLineUpClock_PropertyChanged);
                this.LineUpNumberLbl.Text = GameViewModel.Instance.CurrentJam.JamNumber.ToString();
                this.LineUpClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.LineUpClockPerJam).ToString(@"mm\:ss");
                //period clock should be running if we were to start up the line up clock.
                //we don't start it because the jam clock could still be counting when the period has ended.
                if (PolicyViewModel.Instance.JamClockControlsLineUpClock && GameViewModel.Instance.PeriodClock.IsRunning)
                    GameViewModel.Instance.startLineUpClock();
                GameViewModel.Instance.saveJam();

                GameViewModel.Instance.createNewJam();
                GameViewModel.Instance.CurrentJam.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentJam_PropertyChanged);
                GameViewModel.Instance.CurrentJam.JamClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(JamClock_PropertyChanged);

                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object arg)
                {
                    this.jamClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"mm\:ss");
                    this.editJamClockText.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"mm\:ss");
                    this.JamNumberLbl.Text = GameViewModel.Instance.CurrentJam.JamNumber.ToString();
                    this.editJamNumberText.Text = GameViewModel.Instance.CurrentJam.JamNumber.ToString();
                    return null;
                }, null);
                if (FindResource("SkaterLabelsMainGrey") as Style != null)
                {
                    Jammer1Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                    Jammer2Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                    Pivot1Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                    Pivot2Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                }

                Jammer1Lbl.Text = "";
                Jammer2Lbl.Text = "";
                Pivot1Lbl.Text = "";
                Pivot2Lbl.Text = "";

                stopJamBtn.IsEnabled = false;
                stopJambyInjuryBtn.IsEnabled = false;
                stopJamT1Btn.IsEnabled = false;
                stopJamT2Btn.IsEnabled = false;
                startJamBtn.IsEnabled = true;

                timeOutTeam2Btn.IsEnabled = true;
                timeOutTeam1Btn.IsEnabled = true;

                team2PivotLead.IsChecked = false;
                team2JammerLead.IsChecked = false;
                team1PivotLead.IsChecked = false;
                team1JammerLead.IsChecked = false;

                team2PivotLead.IsEnabled = false;
                team2JammerLead.IsEnabled = false;
                team1PivotLead.IsEnabled = false;
                team1JammerLead.IsEnabled = false;

                Team1ScoreUpBtn.IsEnabled = true;
                Team2ScoreUpBtn.IsEnabled = true;
                Team1ScoreUpBtn4.IsEnabled = true;
                Team2ScoreUpBtn4.IsEnabled = true;
                Team1ScoreUpBtn5.IsEnabled = true;
                Team2ScoreUpBtn5.IsEnabled = true;
                Team1ScoreDownBtn.IsEnabled = true;
                Team2ScoreDownBtn.IsEnabled = true;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// clicks the official time out button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void timeOutBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //because of commands, we must first see if the button is enabled through our logic.  If not, the button and command don't work.
                if (!timeOutBtn.IsEnabled)
                    return;

                GameViewModel.Instance.createTimeOutClock(TimeOutTypeEnum.Offical);
                GameViewModel.Instance.CurrentTimeOutClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentTimeOutClock_PropertyChanged);
                GameViewModel.Instance.startTimeOut(TimeOutTypeEnum.Offical);
                TimeOutName.Text = "Time Out";

                if (stopJamBtn.IsEnabled)
                {
                    GameViewModel.Instance.createLineUpClock();
                    GameViewModel.Instance.CurrentLineUpClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentLineUpClock_PropertyChanged);
                    this.LineUpNumberLbl.Text = GameViewModel.Instance.CurrentJam.JamNumber.ToString();
                    this.LineUpClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.LineUpClockPerJam).ToString(@"mm\:ss");
                    //period clock should be running if we were to start up the line up clock.
                    //we don't start it because the jam clock could still be counting when the period has ended.
                    GameViewModel.Instance.saveJam();

                    GameViewModel.Instance.createNewJam();
                    GameViewModel.Instance.CurrentJam.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentJam_PropertyChanged);
                    GameViewModel.Instance.CurrentJam.JamClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(JamClock_PropertyChanged);

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object arg)
                    {
                        this.jamClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"mm\:ss");
                        this.editJamClockText.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"mm\:ss");
                        this.JamNumberLbl.Text = GameViewModel.Instance.CurrentJam.JamNumber.ToString();
                        this.editJamNumberText.Text = GameViewModel.Instance.CurrentJam.JamNumber.ToString();
                        return null;
                    }, null);
                    if (FindResource("SkaterLabelsMainGrey") as Style != null)
                    {
                        Jammer1Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                        Jammer2Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                        Pivot1Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                        Pivot2Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                    }

                    timeOutTeam2Btn.IsEnabled = true;
                    timeOutTeam1Btn.IsEnabled = true;

                    team2PivotLead.IsChecked = false;
                    team2JammerLead.IsChecked = false;
                    team1PivotLead.IsChecked = false;
                    team1JammerLead.IsChecked = false;

                    team2PivotLead.IsEnabled = false;
                    team2JammerLead.IsEnabled = false;
                    team1PivotLead.IsEnabled = false;
                    team1JammerLead.IsEnabled = false;

                    Team1ScoreUpBtn.IsEnabled = true;
                    Team2ScoreUpBtn.IsEnabled = true;
                    Team1ScoreUpBtn4.IsEnabled = true;
                    Team2ScoreUpBtn4.IsEnabled = true;
                    Team1ScoreUpBtn5.IsEnabled = true;
                    Team2ScoreUpBtn5.IsEnabled = true;
                    Team1ScoreDownBtn.IsEnabled = true;
                    Team2ScoreDownBtn.IsEnabled = true;
                    this.TimeOutClockLbl.Text = TimeSpan.FromMilliseconds(GameViewModel.Instance.CurrentTimeOutClock.TimeRemaining).ToString(@"mm\:ss");
                }
                startJamBtn.IsEnabled = true;
                stopJamBtn.IsEnabled = false;
                stopJambyInjuryBtn.IsEnabled = false;
                stopJamT1Btn.IsEnabled = false;
                stopJamT2Btn.IsEnabled = false;

                timeOutTeam1Btn.IsEnabled = false;
                timeOutTeam2Btn.IsEnabled = false;

                Jammer1Lbl.Text = "";
                Jammer2Lbl.Text = "";
                Pivot1Lbl.Text = "";
                Pivot2Lbl.Text = "";

                team2PivotLead.IsChecked = false;
                team2JammerLead.IsChecked = false;
                team1PivotLead.IsChecked = false;
                team1JammerLead.IsChecked = false;

                team2PivotLead.IsEnabled = false;
                team2JammerLead.IsEnabled = false;
                team1PivotLead.IsEnabled = false;
                team1JammerLead.IsEnabled = false;

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }


        private void Team1ScoreUpBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //because of commands, we must first see if the button is enabled through our logic.  If not, the button and command don't work.
                if (!Team1ScoreUpBtn.IsEnabled)
                    return;

                GameViewModel.Instance.AddScoreToGame(1, TeamNumberEnum.Team1);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        private void Team1ScoreUpBtn4_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //because of commands, we must first see if the button is enabled through our logic.  If not, the button and command don't work.
                if (!Team1ScoreUpBtn4.IsEnabled)
                    return;

                GameViewModel.Instance.AddScoreToGame(4, TeamNumberEnum.Team1);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        private void Team1ScoreUpBtn5_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //because of commands, we must first see if the button is enabled through our logic.  If not, the button and command don't work.
                if (!Team1ScoreUpBtn5.IsEnabled)
                    return;

                GameViewModel.Instance.AddScoreToGame(5, TeamNumberEnum.Team1);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void Team2ScoreUpBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //because of commands, we must first see if the button is enabled through our logic.  If not, the button and command don't work.
                if (!Team2ScoreUpBtn.IsEnabled)
                    return;

                GameViewModel.Instance.AddScoreToGame(1, TeamNumberEnum.Team2);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        private void Team2ScoreUpBtn4_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //because of commands, we must first see if the button is enabled through our logic.  If not, the button and command don't work.
                if (!Team2ScoreUpBtn4.IsEnabled)
                    return;

                GameViewModel.Instance.AddScoreToGame(4, TeamNumberEnum.Team2);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        private void Team2ScoreUpBtn5_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //because of commands, we must first see if the button is enabled through our logic.  If not, the button and command don't work.
                if (!Team2ScoreUpBtn5.IsEnabled)
                    return;

                GameViewModel.Instance.AddScoreToGame(5, TeamNumberEnum.Team2);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void Team1ScoreDownBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {            //because of commands, we must first see if the button is enabled through our logic.  If not, the button and command don't work.
                if (!Team1ScoreDownBtn.IsEnabled)
                    return;

                GameViewModel.Instance.AddScoreToGame(-1, TeamNumberEnum.Team1);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void Team2ScoreDownBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //because of commands, we must first see if the button is enabled through our logic.  If not, the button and command don't work.
                if (!Team2ScoreDownBtn.IsEnabled)
                    return;

                GameViewModel.Instance.AddScoreToGame(-1, TeamNumberEnum.Team2);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        public void timeOutTeam1Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //because of commands, we must first see if the button is enabled through our logic.  If not, the button and command don't work.
                if (!timeOutTeam1Btn.IsEnabled)
                    return;

                if (GameViewModel.Instance.Team1.TimeOutsLeft > 0)
                {
                    this.TimeOutClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.TimeOutClock).ToString(@"mm\:ss");
                    GameViewModel.Instance.createTimeOutClock(TimeOutTypeEnum.Team1);
                    GameViewModel.Instance.CurrentTimeOutClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentTimeOutClock_PropertyChanged);
                    GameViewModel.Instance.startTimeOut(TimeOutTypeEnum.Team1);
                    TimeOutName.Text = "Time Out";


                    TimeOutsRemainingTeam1.Text = GameViewModel.Instance.Team1.TimeOutsLeft.ToString();
                    editTimeOutsRemainingTeam1Text.Text = GameViewModel.Instance.Team1.TimeOutsLeft.ToString();

                    startJamBtn.IsEnabled = true;
                    stopJamBtn.IsEnabled = false;
                    stopJambyInjuryBtn.IsEnabled = false;
                    stopJamT1Btn.IsEnabled = false;
                    stopJamT2Btn.IsEnabled = false;

                    timeOutTeam1Btn.IsEnabled = false;
                    timeOutTeam2Btn.IsEnabled = false;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// timeout for team 2 button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void timeOutTeam2Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //because of commands, we must first see if the button is enabled through our logic.  If not, the button and command don't work.
                if (!timeOutTeam2Btn.IsEnabled)
                    return;

                if (GameViewModel.Instance.Team2.TimeOutsLeft > 0)
                {
                    this.TimeOutClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.TimeOutClock).ToString(@"mm\:ss");
                    GameViewModel.Instance.createTimeOutClock(TimeOutTypeEnum.Team2);
                    GameViewModel.Instance.CurrentTimeOutClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentTimeOutClock_PropertyChanged);
                    GameViewModel.Instance.startTimeOut(TimeOutTypeEnum.Team2);
                    TimeOutName.Text = "Time Out";

                    TimeOutsRemainingTeam2.Text = GameViewModel.Instance.Team2.TimeOutsLeft.ToString();
                    this.editTimeOutsRemainingTeam2Text.Text = GameViewModel.Instance.Team2.TimeOutsLeft.ToString();

                    startJamBtn.IsEnabled = true;
                    stopJamBtn.IsEnabled = false;
                    stopJambyInjuryBtn.IsEnabled = false;
                    stopJamT1Btn.IsEnabled = false;
                    stopJamT2Btn.IsEnabled = false;

                    timeOutTeam1Btn.IsEnabled = false;
                    timeOutTeam2Btn.IsEnabled = false;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void team1JammerLead_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //because of commands, we must first see if the button is enabled through our logic.  If not, the button and command don't work.
                if (!team1JammerLead.IsEnabled)
                    return;

                //if ((bool)team1JammerLead.IsChecked)
                //{
                if (GameViewModel.Instance.CurrentJam.JammerT1 != null)
                {//TODO: add a notification saying we need a lead jammer here.
                    GameViewModel.Instance.CurrentJam.setLeadJammer(GameViewModel.Instance.CurrentJam.JammerT1, GameViewModel.Instance.ElapsedTimeGameClockMilliSeconds, TeamNumberEnum.Team1);
                    team1JammerLead.IsChecked = true;
                    team1PivotLead.IsChecked = false;
                    team2PivotLead.IsChecked = false;
                    team2JammerLead.IsChecked = false;

                }
                if (FindResource("SkaterLabelsMainGreen") as Style != null)
                    Jammer1Lbl.Style = FindResource("SkaterLabelsMainGreen") as Style;

                if (FindResource("SkaterLabelsMainGrey") as Style != null)
                {
                    Pivot1Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                    Pivot2Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                    Jammer2Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                }
                //}
                //else
                //{
                //    team1JammerLead.IsChecked = false;
                //    if (FindResource("SkaterLabelsMainGrey") as Style != null)
                //        Jammer1Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                //}
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void team1PivotLead_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GameViewModel.Instance.CurrentJam.PivotT1 != null)
                {//TODO: add a notification saying we need a lead jammer here.
                    GameViewModel.Instance.CurrentJam.setLeadJammer(GameViewModel.Instance.CurrentJam.PivotT1, GameViewModel.Instance.ElapsedTimeGameClockMilliSeconds, TeamNumberEnum.Team1);
                    team1PivotLead.IsChecked = true;
                    team1JammerLead.IsChecked = false;
                    team2PivotLead.IsChecked = false;
                    team2JammerLead.IsChecked = false;

                }
                if (FindResource("SkaterLabelsMainGreen") as Style != null)
                    Pivot1Lbl.Style = FindResource("SkaterLabelsMainGreen") as Style;

                if (FindResource("SkaterLabelsMainGrey") as Style != null)
                {
                    Jammer1Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                    Pivot2Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                    Jammer2Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                }

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void team2JammerLead_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //because of commands, we must first see if the button is enabled through our logic.  If not, the button and command don't work.
                if (!team2JammerLead.IsEnabled)
                    return;


                if (GameViewModel.Instance.CurrentJam.JammerT2 != null)
                {
                    //TODO: add a notification saying we need a lead jammer here.
                    GameViewModel.Instance.CurrentJam.setLeadJammer(GameViewModel.Instance.CurrentJam.JammerT2, GameViewModel.Instance.ElapsedTimeGameClockMilliSeconds, TeamNumberEnum.Team2);
                    team2JammerLead.IsChecked = true;
                    team2PivotLead.IsChecked = false;

                    team1JammerLead.IsChecked = false;
                    team1PivotLead.IsChecked = false;


                }
                if (FindResource("SkaterLabelsMainGreen") as Style != null)
                    Jammer2Lbl.Style = FindResource("SkaterLabelsMainGreen") as Style;

                if (FindResource("SkaterLabelsMainGrey") as Style != null)
                {
                    Jammer1Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                    Pivot2Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                    Pivot1Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                }
                //if (!(bool)team2JammerLead.IsChecked)
                //{
                //    team2JammerLead.IsChecked = false;
                //    if (FindResource("SkaterLabelsMainGrey") as Style != null)
                //        Jammer2Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                //}
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void team2PivotLead_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (GameViewModel.Instance.CurrentJam.PivotT2 != null)
                {//TODO: add a notification saying we need a lead jammer here.
                    GameViewModel.Instance.CurrentJam.setLeadJammer(GameViewModel.Instance.CurrentJam.PivotT2, GameViewModel.Instance.ElapsedTimeGameClockMilliSeconds, TeamNumberEnum.Team2);
                    team2PivotLead.IsChecked = true;
                    team2JammerLead.IsChecked = false;

                    team1JammerLead.IsChecked = false;
                    team1PivotLead.IsChecked = false;

                }
                if (FindResource("SkaterLabelsMainGreen") as Style != null)
                    Pivot2Lbl.Style = FindResource("SkaterLabelsMainGreen") as Style;

                if (FindResource("SkaterLabelsMainGrey") as Style != null)
                {
                    Jammer1Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                    Jammer2Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                    Pivot1Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                }

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        public void officialReviewBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //because of commands, we must first see if the button is enabled through our logic.  If not, the button and command don't work.
                if (!officialReviewBtn.IsEnabled)
                    return;



                OfficialReviewWindow window = new OfficialReviewWindow();
                window.Show();
                GameViewModel.Instance.createTimeOutClock(TimeOutTypeEnum.Official_Review);
                GameViewModel.Instance.CurrentTimeOutClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentTimeOutClock_PropertyChanged);
                GameViewModel.Instance.startTimeOut(TimeOutTypeEnum.Official_Review);
                TimeOutName.Text = "Official Review";

                if (stopJamBtn.IsEnabled)
                {
                    GameViewModel.Instance.createLineUpClock();
                    GameViewModel.Instance.CurrentLineUpClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentLineUpClock_PropertyChanged);
                    this.LineUpNumberLbl.Text = GameViewModel.Instance.CurrentJam.JamNumber.ToString();
                    this.LineUpClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.LineUpClockPerJam).ToString(@"mm\:ss");
                    //period clock should be running if we were to start up the line up clock.
                    //we don't start it because the jam clock could still be counting when the period has ended.
                    GameViewModel.Instance.saveJam();

                    GameViewModel.Instance.createNewJam();
                    GameViewModel.Instance.CurrentJam.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentJam_PropertyChanged);
                    GameViewModel.Instance.CurrentJam.JamClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(JamClock_PropertyChanged);

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object arg)
                    {
                        this.jamClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"mm\:ss");
                        this.editJamClockText.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"mm\:ss");
                        this.JamNumberLbl.Text = GameViewModel.Instance.CurrentJam.JamNumber.ToString();
                        this.editJamNumberText.Text = GameViewModel.Instance.CurrentJam.JamNumber.ToString();
                        return null;
                    }, null);
                    if (FindResource("SkaterLabelsMainGrey") as Style != null)
                    {
                        Jammer1Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                        Jammer2Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                        Pivot1Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                        Pivot2Lbl.Style = FindResource("SkaterLabelsMainGrey") as Style;
                    }

                    timeOutTeam2Btn.IsEnabled = true;
                    timeOutTeam1Btn.IsEnabled = true;

                    team2PivotLead.IsChecked = false;
                    team2JammerLead.IsChecked = false;
                    team1PivotLead.IsChecked = false;
                    team1JammerLead.IsChecked = false;

                    team2PivotLead.IsEnabled = false;
                    team2JammerLead.IsEnabled = false;
                    team1PivotLead.IsEnabled = false;
                    team1JammerLead.IsEnabled = false;

                    Team1ScoreUpBtn.IsEnabled = true;
                    Team2ScoreUpBtn.IsEnabled = true;
                    Team1ScoreUpBtn4.IsEnabled = true;
                    Team2ScoreUpBtn4.IsEnabled = true;
                    Team1ScoreUpBtn5.IsEnabled = true;
                    Team2ScoreUpBtn5.IsEnabled = true;
                    Team1ScoreDownBtn.IsEnabled = true;
                    Team2ScoreDownBtn.IsEnabled = true;
                    this.TimeOutClockLbl.Text = TimeSpan.FromMilliseconds(GameViewModel.Instance.CurrentTimeOutClock.TimeRemaining).ToString(@"mm\:ss");
                }
                startJamBtn.IsEnabled = true;
                stopJamBtn.IsEnabled = false;
                stopJambyInjuryBtn.IsEnabled = false;
                stopJamT1Btn.IsEnabled = false;
                stopJamT2Btn.IsEnabled = false;

                timeOutTeam1Btn.IsEnabled = false;
                timeOutTeam2Btn.IsEnabled = false;

                Jammer1Lbl.Text = "";
                Jammer2Lbl.Text = "";
                Pivot1Lbl.Text = "";
                Pivot2Lbl.Text = "";

                team2PivotLead.IsChecked = false;
                team2JammerLead.IsChecked = false;
                team1PivotLead.IsChecked = false;
                team1JammerLead.IsChecked = false;

                team2PivotLead.IsEnabled = false;
                team2JammerLead.IsEnabled = false;
                team1PivotLead.IsEnabled = false;
                team1JammerLead.IsEnabled = false;



            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// starts the intermission
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startIntermissionBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                GameViewModel.Instance.createNewIntermission();
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object arg)
                {
                    this.IntermissionClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.IntermissionStartOfClockInMilliseconds).ToString(@"mm\:ss");
                    this.editIntermissionClockText.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.IntermissionStartOfClockInMilliseconds).ToString(@"mm\:ss");

                    gameOverBtn.IsEnabled = false;
                    startJamBtn.IsEnabled = true;
                    return null;
                }, null);

                Logger.Instance.logMessage("starting a new intermission", LoggerEnum.message);
                GameViewModel.Instance.IntermissionClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(IntermissionClock_PropertyChanged);
                GameViewModel.Instance.startIntermission();

                GameViewModel.Instance.createNewJam(true);
                GameViewModel.Instance.CurrentJam.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentJam_PropertyChanged);
                GameViewModel.Instance.CurrentJam.JamClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(JamClock_PropertyChanged);

                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object arg)
                {
                    this.jamClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"mm\:ss");
                    this.editJamClockText.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam).ToString(@"mm\:ss");

                    this.JamNumberLbl.Text = GameViewModel.Instance.CurrentJam.JamNumber.ToString();
                    this.editJamNumberText.Text = GameViewModel.Instance.CurrentJam.JamNumber.ToString();
                    return null;
                }, null);

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        void CurrentLineUpClock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (StopWatchWrapperEnum.TimeRemaining.ToString() == e.PropertyName)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        this.LineUpClockLbl.Text = TimeSpan.FromMilliseconds(GameViewModel.Instance.CurrentLineUpClock.TimeRemaining).ToString(@"mm\:ss");
                        this.editLineUpClockText.Text = TimeSpan.FromMilliseconds(GameViewModel.Instance.CurrentLineUpClock.TimeRemaining).ToString(@"mm\:ss");
                        if (_currentTextBoxSelected == _TextBoxEnum.LineUpTime)
                            this.editLineUpClockText.SelectionStart = _currentCursorPosition;
                        return null;
                    }, null);
                }
                else if (StopWatchWrapperEnum.IsClockAtZero.ToString() == e.PropertyName)
                {
                    if (GameViewModel.Instance.CurrentLineUpClock.IsClockAtZero && PolicyViewModel.Instance.LineupClockControlsStartJam)
                    {
                        startJamWithButtonClick(true);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }

        }
        void CurrentJam_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (JamViewModelEnum.JamNumber.ToString() == e.PropertyName ||
                    JamViewModelEnum.JammerT1.ToString() == e.PropertyName ||
                    JamViewModelEnum.JammerT2.ToString() == e.PropertyName ||
                    JamViewModelEnum.PivotT1.ToString() == e.PropertyName ||
                    JamViewModelEnum.PivotT2.ToString() == e.PropertyName)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        this.JamNumberLbl.Text = GameViewModel.Instance.CurrentJam.JamNumber.ToString();
                        this.editJamNumberText.Text = GameViewModel.Instance.CurrentJam.JamNumber.ToString();
                        if (GameViewModel.Instance.CurrentJam.JammerT1 != null)
                        {
                            this.Jammer1Lbl.Text = GameViewModel.Instance.CurrentJam.JammerT1.SkaterName;
                            this.team1JammerLead.IsEnabled = true;
                        }
                        else
                        {
                            this.Jammer1Lbl.Text = "";
                            this.team1JammerLead.IsEnabled = false;
                        }
                        if (GameViewModel.Instance.CurrentJam.JammerT2 != null)
                        {
                            this.Jammer2Lbl.Text = GameViewModel.Instance.CurrentJam.JammerT2.SkaterName;
                            this.team2JammerLead.IsEnabled = true;
                        }
                        else
                        {
                            this.Jammer2Lbl.Text = "";
                            this.team2JammerLead.IsEnabled = false;
                        }
                        if (GameViewModel.Instance.CurrentJam.PivotT1 != null)
                        {
                            this.Pivot1Lbl.Text = GameViewModel.Instance.CurrentJam.PivotT1.SkaterName;
                            this.team1PivotLead.IsEnabled = true;
                        }
                        else
                        {
                            this.Pivot1Lbl.Text = "";
                            this.team1PivotLead.IsEnabled = false;
                        }
                        if (GameViewModel.Instance.CurrentJam.PivotT2 != null)
                        {
                            this.Pivot2Lbl.Text = GameViewModel.Instance.CurrentJam.PivotT2.SkaterName;
                            this.team2PivotLead.IsEnabled = true;
                        }
                        else
                        {
                            this.Pivot2Lbl.Text = "";
                            this.team2PivotLead.IsEnabled = false;
                        }
                        return null;
                    }, null);
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        void JamClock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (StopWatchWrapperEnum.TimeRemaining.ToString() == e.PropertyName)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        this.jamClockLbl.Text = TimeSpan.FromMilliseconds(GameViewModel.Instance.CurrentJam.JamClock.TimeRemaining).ToString(@"mm\:ss");
                        this.editJamClockText.Text = TimeSpan.FromMilliseconds(GameViewModel.Instance.CurrentJam.JamClock.TimeRemaining).ToString(@"mm\:ss");
                        if (_currentTextBoxSelected == _TextBoxEnum.JamTime)
                            this.editJamClockText.SelectionStart = _currentCursorPosition;
                        return null;
                    }, null);
                }
                else if (StopWatchWrapperEnum.IsClockAtZero.ToString() == e.PropertyName)
                {
                    //buzzer sounds if the jam clock and period clock are at zero.
                    if (_soundBuzzerAtEndOfJam)
                        SoundBuzzer();
                }

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// when the period clock changes properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PeriodClock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (StopWatchWrapperEnum.TimeRemaining.ToString() == e.PropertyName)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        this.PeriodClockLbl.Text = TimeSpan.FromMilliseconds(GameViewModel.Instance.PeriodClock.TimeRemaining).ToString(@"mm\:ss");
                        this.editPeriodClockText.Text = TimeSpan.FromMilliseconds(GameViewModel.Instance.PeriodClock.TimeRemaining).ToString(@"mm\:ss");
                        if (_currentTextBoxSelected == _TextBoxEnum.PeriodTime)
                        {
                            this.editPeriodClockText.SelectionStart = _currentCursorPosition;
                            Console.WriteLine(_currentCursorPosition);
                        }
                        return null;
                    }, null);
                }
                else if (StopWatchWrapperEnum.IsClockAtZero.ToString() == e.PropertyName)
                {
                    if (GameViewModel.Instance.PeriodClock.IsClockAtZero)
                    {
                        if (PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.TEXAS_DERBY || PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.MADE || PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.MADE_COED)
                        {
                            SoundBuzzer();
                        }
                        else
                        {
                            _soundBuzzerAtEndOfJam = true;
                        }

                        //WFTDA stops the game after the second period, so we need to make sure we don't create another one 
                        //if its on the second period under the WFTDA rule set.
                        if (GameViewModel.Instance.CurrentPeriod == 2)
                        {
                            if (PolicyViewModel.Instance.GameSelectionType != GameTypeEnum.WFTDA && PolicyViewModel.Instance.GameSelectionType != GameTypeEnum.WFTDA_2010)
                            {
                                GameViewModel.Instance.createNewPeriod();
                                GameViewModel.Instance.PeriodClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(PeriodClock_PropertyChanged);
                            }
                            else
                            {
                                //WFTDA games want the unofficial final name to show.
                                GameViewModel.Instance.NameOfIntermission = "UnOfficial Final";

                            }
                        }
                        else
                        {
                            GameViewModel.Instance.createNewPeriod();
                            GameViewModel.Instance.PeriodClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(PeriodClock_PropertyChanged);
                        }
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (DispatcherOperationCallback)delegate(object arg)
                        {
                            this.PeriodClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.PeriodClock).ToString(@"mm\:ss");
                            this.editPeriodClockText.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.PeriodClock).ToString(@"mm\:ss");

                            this.startIntermissionBtn.IsEnabled = true;
                            this.gameOverBtn.IsEnabled = true;
                            //we turn off the jam button here, because we don't create a new jam yet.  We create the new jam when we start an intermission.
                            this.startJamBtn.IsEnabled = false;
                            if (!GameViewModel.Instance.CurrentJam.JamClock.IsRunning)
                            {
                                this.stopJamBtn.IsEnabled = false;
                                stopJambyInjuryBtn.IsEnabled = false;
                                stopJamT1Btn.IsEnabled = false;
                                stopJamT2Btn.IsEnabled = false;
                            }
                            this.timeOutTeam1Btn.IsEnabled = false;
                            this.timeOutTeam2Btn.IsEnabled = false;
                            return null;
                        }, null);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// sounds period end buzzer.
        /// </summary>
        private void SoundBuzzer()
        {
            if (PolicyViewModel.Instance.SoundBuzzerAtEndOfPeriod && !String.IsNullOrEmpty(PolicyViewModel.Instance.SoundBuzzerFileLocation))
            {
                try
                {
                    var reader = new Mp3FileReader(PolicyViewModel.Instance.SoundBuzzerFileLocation);
                    var buzzer = new WaveOut();
                    buzzer.Init(reader);
                    buzzer.Play();
                }
                catch (Exception exception)
                {
                    ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                }
            }
        }

        void IntermissionClock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (StopWatchWrapperEnum.TimeRemaining.ToString() == e.PropertyName)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        this.IntermissionClockLbl.Text = TimeSpan.FromMilliseconds(GameViewModel.Instance.IntermissionClock.TimeRemaining).ToString(@"mm\:ss");
                        this.editIntermissionClockText.Text = TimeSpan.FromMilliseconds(GameViewModel.Instance.IntermissionClock.TimeRemaining).ToString(@"mm\:ss");
                        if (_currentTextBoxSelected == _TextBoxEnum.IntermissionTime)
                            this.editIntermissionClockText.SelectionStart = _currentCursorPosition;
                        return null;
                    }, null);
                }
                else if (StopWatchWrapperEnum.IsClockAtZero.ToString() == e.PropertyName)
                {
                    if (GameViewModel.Instance.IntermissionClock.IsClockAtZero)
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                         (DispatcherOperationCallback)delegate(object arg)
                         {
                             this.startJamBtn.IsEnabled = true;
                             this.timeOutTeam1Btn.IsEnabled = true;
                             this.timeOutTeam2Btn.IsEnabled = true;
                             this.timeOutBtn.IsEnabled = true;
                             this.officialReviewBtn.IsEnabled = true;

                             if (_currentTextBoxSelected == _TextBoxEnum.PeriodTime)
                                 this.editPeriodClockText.SelectionStart = _currentCursorPosition;


                             GameViewModel.Instance.createNewIntermission();
                             GameViewModel.Instance.IntermissionClock.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(IntermissionClock_PropertyChanged);


                             this.IntermissionClockLbl.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.IntermissionStartOfClockInMilliseconds).ToString(@"mm\:ss");
                             this.editIntermissionClockText.Text = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.IntermissionStartOfClockInMilliseconds).ToString(@"mm\:ss");
                             return null;
                         }, null);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        void PolicyInstance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (PolicyViewModelEnum.OfficialTimeOutKeyShortcut.ToString() == e.PropertyName ||
                    PolicyViewModelEnum.StartJamKeyShortcut.ToString() == e.PropertyName ||
                    PolicyViewModelEnum.StopJamKeyShortcut.ToString() == e.PropertyName ||
                    PolicyViewModelEnum.Team1LeadJammerKeyShortcut.ToString() == e.PropertyName ||
                    PolicyViewModelEnum.Team1ScoreDownKeyShortcut.ToString() == e.PropertyName ||
                    PolicyViewModelEnum.Team1ScoreUpKeyShortcut.ToString() == e.PropertyName ||
                    PolicyViewModelEnum.Team1TimeOutKeyShortcut.ToString() == e.PropertyName ||
                    PolicyViewModelEnum.Team2LeadJammerKeyShortcut.ToString() == e.PropertyName ||
                    PolicyViewModelEnum.Team2ScoreDownKeyShortcut.ToString() == e.PropertyName ||
                    PolicyViewModelEnum.Team2ScoreUpKeyShortcut.ToString() == e.PropertyName ||
                    PolicyViewModelEnum.Team2TimeOutKeyShortcut.ToString() == e.PropertyName)
                {
                    setKeyboardShortcuts();
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (GameViewModelEnum.CurrentPeriod.ToString() == e.PropertyName ||
                    GameViewModelEnum.CurrentTeam1Score.ToString() == e.PropertyName ||
                    GameViewModelEnum.CurrentTeam2Score.ToString() == e.PropertyName
                    )
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        this.editPeriodNumberText.Text = GameViewModel.Instance.CurrentPeriod.ToString();
                        this.PeriodNumberLbl.Text = GameViewModel.Instance.CurrentPeriod.ToString();
                        this.Team2Scorelbl.Text = GameViewModel.Instance.CurrentTeam2Score.ToString();
                        this.Team1Scorelbl.Text = GameViewModel.Instance.CurrentTeam1Score.ToString();
                        return null;
                    }, null);
                }
                else if (GameViewModelEnum.IsGameOnline.ToString() == e.PropertyName)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        switch (GameViewModel.Instance.IsGameOnline)
                        {
                            case GameViewModelIsOnlineEnum.IsOnline:
                                this.onOffImage.Source = (ImageSource)FindResource("ImageGreenOn");
                                this.onOffLabel.Text = "Online";
                                break;
                            case GameViewModelIsOnlineEnum.IsSending:
                                this.onOffLabel.Text = "Sending Update...";
                                break;
                            case GameViewModelIsOnlineEnum.IsOffline:
                                this.onOffImage.Source = (ImageSource)FindResource("ImageRedOff");
                                this.onOffLabel.Text = "Offline";
                                break;
                            case GameViewModelIsOnlineEnum.InternetProblem:
                                this.onOffImage.Source = (ImageSource)FindResource("ImageRedOff");
                                this.onOffLabel.Text = "Internet Dropped, retrying...";
                                break;
                        }

                        return null;
                    }, null);
                }
                else if (GameViewModelEnum.CurrentIntermission.ToString() == e.PropertyName)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        IntermissionLbl.Text = GameViewModel.Instance.NameOfIntermission;
                        startIntermissionBtn.Content = "Start " + GameViewModel.Instance.NameOfIntermission;
                        editIntermissionName.Text = GameViewModel.Instance.NameOfIntermission;
                        return null;
                    }, null);
                }
                else if (GameViewModelEnum.NameOfIntermission.ToString() == e.PropertyName)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        IntermissionLbl.Text = GameViewModel.Instance.NameOfIntermission;
                        startIntermissionBtn.Content = "Start " + GameViewModel.Instance.NameOfIntermission;
                        return null;
                    }, null);
                }
                else if (GameViewModelEnum.UrlForAdministeringGameOnline.ToString() == e.PropertyName)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        if (!String.IsNullOrEmpty(GameViewModel.Instance.UrlForAdministeringGameOnline))
                            OnlineAdminLink.Visibility = System.Windows.Visibility.Visible;
                        else
                            OnlineAdminLink.Visibility = System.Windows.Visibility.Collapsed;
                        return null;
                    }, null);
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        void CurrentTimeOutClock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (StopWatchWrapperEnum.TimeRemaining.ToString() == e.PropertyName && GameViewModel.Instance.CurrentTimeOutClock != null)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        if (GameViewModel.Instance.CurrentTimeOutClock != null)
                        {
                            this.TimeOutClockLbl.Text = TimeSpan.FromMilliseconds(GameViewModel.Instance.CurrentTimeOutClock.TimeRemaining).ToString(@"mm\:ss");
                            if (GameViewModel.Instance.CurrentTimeOutClock.TimeRemaining <= 0)
                            {
                                this.timeOutTeam1Btn.IsEnabled = true;
                                this.timeOutTeam2Btn.IsEnabled = true;
                            }
                        }
                        return null;
                    }, null);
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }



        private void themeHolder_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                Application.Current.ApplyTheme(e.NewValue.ToString());
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void gameOverBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Clicking Yes Will Finalize Scores & Will End The Game, Are You Sure?", "End Game?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    GameViewModel.Instance.endGame();
                    LoadGame.IsEnabled = true;
                    CreateNewGame.IsEnabled = true;
                    SaveGame.IsEnabled = true;
                    SaveSendGame_Click(null, null);

                    if (MessageBox.Show("Please be sure to Upload & Publish your Game to RDNation." + Environment.NewLine + Environment.NewLine + "Would you like to go there now?", "Upload Your Game?", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        //System.Diagnostics.Process.Start(ScoreboardConfig.SCOREBOARD_UPLOAD_AND_PUBLISH_GAME);
                        WebBrowser b = new WebBrowser();
                        b.Navigate(ScoreboardConfig.SCOREBOARD_UPLOAD_AND_PUBLISH_GAME, "_blank", null, null);
                    }
                }
                else
                    return;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }


        private void gameNameText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                GameViewModel.Instance.GameName = ((RibbonTextBox)sender).Text;
                Window.Title = ((RibbonTextBox)sender).Text + " - " + ScoreboardConfig.SCOREBOARD_NAME;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void gameLocationText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                GameViewModel.Instance.GameLocation = ((TextBox)sender).Text;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// hits when window is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                Logger.Instance.logMessage("main window closed.", LoggerEnum.message);
                PolicyViewModel.Instance.savePolicyToXml();
                var result = MessageBox.Show("Please help us improve the scoreboard, take a few minutes and submit some feedback to us. Can you spare a few minutes and send some feedback to us?", "Feedback wanted!", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    FeedbackView pop = new FeedbackView();
                    pop.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    pop.ShowDialog();
                }
                Logger.Instance.logMessage("scoreboard closed", LoggerEnum.message);
                if (_WebServer != null)
                    _WebServer.Stop();

                ScoreboardCrashed.ClosedProperly();

                Application.Current.Shutdown();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        /// <summary>
        /// enters or exits edit mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startEditMode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!GameViewModel.Instance.IsInEditMode)
                {
                    // we are going into the Edit Mode UI.
                    GameViewModel.Instance.EnterEditMode();
                    editModeTeam1TimeOut.Visibility = System.Windows.Visibility.Visible;
                    editModeTeam2TimeOut.Visibility = System.Windows.Visibility.Visible;
                    editModePeriodNumber.Visibility = System.Windows.Visibility.Visible;
                    editModeJamNumber.Visibility = System.Windows.Visibility.Visible;
                    editModePeriodTime.Visibility = System.Windows.Visibility.Visible;
                    editModeJamTime.Visibility = System.Windows.Visibility.Visible;
                    editModeLineUpTime.Visibility = System.Windows.Visibility.Visible;
                    editPeriodNumberText.Visibility = System.Windows.Visibility.Visible;
                    PeriodNumberLbl.Visibility = System.Windows.Visibility.Collapsed;
                    editTimeOutsRemainingTeam1Text.Visibility = System.Windows.Visibility.Visible;
                    TimeOutsRemainingTeam1.Visibility = System.Windows.Visibility.Collapsed;
                    editJamNumberText.Visibility = System.Windows.Visibility.Visible;
                    JamNumberLbl.Visibility = System.Windows.Visibility.Collapsed;
                    editTimeOutsRemainingTeam2Text.Visibility = System.Windows.Visibility.Visible;
                    TimeOutsRemainingTeam2.Visibility = System.Windows.Visibility.Collapsed;
                    editLineUpClockText.Visibility = System.Windows.Visibility.Visible;
                    LineUpClockLbl.Visibility = System.Windows.Visibility.Collapsed;
                    editJamClockText.Visibility = System.Windows.Visibility.Visible;
                    jamClockLbl.Visibility = System.Windows.Visibility.Collapsed;
                    editPeriodClockText.Visibility = System.Windows.Visibility.Visible;
                    PeriodClockLbl.Visibility = System.Windows.Visibility.Collapsed;
                    IntermissionClockLbl.Visibility = System.Windows.Visibility.Collapsed;
                    editIntermissionClockText.Visibility = System.Windows.Visibility.Visible;
                    editModeIntermissionTime.Visibility = System.Windows.Visibility.Visible;
                    editIntermissionName.Visibility = System.Windows.Visibility.Visible;
                    IntermissionLbl.Visibility = System.Windows.Visibility.Collapsed;
                    Team1ScoreUpBtn.IsEnabled = true;
                    Team2ScoreUpBtn.IsEnabled = true;
                    Team1ScoreUpBtn4.IsEnabled = true;
                    Team2ScoreUpBtn4.IsEnabled = true;
                    Team1ScoreUpBtn5.IsEnabled = true;
                    Team2ScoreUpBtn5.IsEnabled = true;
                    Team1ScoreDownBtn.IsEnabled = true;
                    Team2ScoreDownBtn.IsEnabled = true;
                }
                else
                {
                    //we remove all the edit mode ui.
                    GameViewModel.Instance.ExitEditMode();
                    editModeTeam1TimeOut.Visibility = System.Windows.Visibility.Collapsed;
                    editModeTeam2TimeOut.Visibility = System.Windows.Visibility.Collapsed;
                    editModePeriodNumber.Visibility = System.Windows.Visibility.Collapsed;
                    editModeJamNumber.Visibility = System.Windows.Visibility.Collapsed;
                    editModePeriodTime.Visibility = System.Windows.Visibility.Collapsed;
                    editModeJamTime.Visibility = System.Windows.Visibility.Collapsed;
                    editModeLineUpTime.Visibility = System.Windows.Visibility.Collapsed;
                    editPeriodNumberText.Visibility = System.Windows.Visibility.Collapsed;
                    PeriodNumberLbl.Visibility = System.Windows.Visibility.Visible;
                    editTimeOutsRemainingTeam1Text.Visibility = System.Windows.Visibility.Collapsed;
                    TimeOutsRemainingTeam1.Visibility = System.Windows.Visibility.Visible;
                    editJamNumberText.Visibility = System.Windows.Visibility.Collapsed;
                    JamNumberLbl.Visibility = System.Windows.Visibility.Visible;
                    editTimeOutsRemainingTeam2Text.Visibility = System.Windows.Visibility.Collapsed;
                    TimeOutsRemainingTeam2.Visibility = System.Windows.Visibility.Visible;
                    editLineUpClockText.Visibility = System.Windows.Visibility.Collapsed;
                    LineUpClockLbl.Visibility = System.Windows.Visibility.Visible;
                    editJamClockText.Visibility = System.Windows.Visibility.Collapsed;
                    jamClockLbl.Visibility = System.Windows.Visibility.Visible;
                    editPeriodClockText.Visibility = System.Windows.Visibility.Collapsed;
                    PeriodClockLbl.Visibility = System.Windows.Visibility.Visible;
                    IntermissionClockLbl.Visibility = System.Windows.Visibility.Visible;
                    editIntermissionClockText.Visibility = System.Windows.Visibility.Collapsed;
                    editModeIntermissionTime.Visibility = System.Windows.Visibility.Collapsed;
                    editIntermissionName.Visibility = System.Windows.Visibility.Collapsed;
                    IntermissionLbl.Visibility = System.Windows.Visibility.Visible;

                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void upTeam1TimeOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditModeModel mode = new EditModeModel();
                mode.EditModeType = EditModeEnum.upTeam1TimeOut;
                GameViewModel.Instance.EditModeItems.Add(mode);
                GameViewModel.Instance.Team1.TimeOutsLeft += 1;
                TimeOutsRemainingTeam1.Text = GameViewModel.Instance.Team1.TimeOutsLeft.ToString();
                editTimeOutsRemainingTeam1Text.Text = GameViewModel.Instance.Team1.TimeOutsLeft.ToString();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void downTeam1TimeOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditModeModel mode = new EditModeModel();
                mode.EditModeType = EditModeEnum.downTeam1TimeOut;
                GameViewModel.Instance.EditModeItems.Add(mode);
                GameViewModel.Instance.Team1.TimeOutsLeft -= 1;
                TimeOutsRemainingTeam1.Text = GameViewModel.Instance.Team1.TimeOutsLeft.ToString();
                editTimeOutsRemainingTeam1Text.Text = GameViewModel.Instance.Team1.TimeOutsLeft.ToString();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// users clicks the period up time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void upPeriodTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditModeModel mode = new EditModeModel();
                mode.EditModeType = EditModeEnum.upPeriodTime;
                GameViewModel.Instance.EditModeItems.Add(mode);
                GameViewModel.Instance.PeriodClock.AddSecondsToClock(1);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void downPeriodTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditModeModel mode = new EditModeModel();
                mode.EditModeType = EditModeEnum.downPeriodTime;
                GameViewModel.Instance.EditModeItems.Add(mode);
                GameViewModel.Instance.PeriodClock.AddSecondsToClock(-1);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void upJamTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                EditModeModel mode = new EditModeModel();
                mode.EditModeType = EditModeEnum.upJamTime;
                GameViewModel.Instance.EditModeItems.Add(mode);
                GameViewModel.Instance.CurrentJam.JamClock.AddSecondsToClock(1);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void downJamTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditModeModel mode = new EditModeModel();
                mode.EditModeType = EditModeEnum.downJamTime;
                GameViewModel.Instance.EditModeItems.Add(mode);
                GameViewModel.Instance.CurrentJam.JamClock.AddSecondsToClock(-1);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void upLineUpTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditModeModel mode = new EditModeModel();
                mode.EditModeType = EditModeEnum.upLineUpTime;
                GameViewModel.Instance.EditModeItems.Add(mode);
                GameViewModel.Instance.CurrentLineUpClock.AddSecondsToClock(1);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void downLineUpTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditModeModel mode = new EditModeModel();
                mode.EditModeType = EditModeEnum.downLineUpTime;
                GameViewModel.Instance.EditModeItems.Add(mode);
                GameViewModel.Instance.CurrentLineUpClock.AddSecondsToClock(-1);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }


        private void downPeriodNumber_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditModeModel mode = new EditModeModel();
                mode.EditModeType = EditModeEnum.downPeriodNumber;
                GameViewModel.Instance.EditModeItems.Add(mode);
                GameViewModel.Instance.CurrentPeriod -= 1;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void upPeriodNumber_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditModeModel mode = new EditModeModel();
                mode.EditModeType = EditModeEnum.upPeriodNumber;
                GameViewModel.Instance.EditModeItems.Add(mode);
                GameViewModel.Instance.CurrentPeriod += 1;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void upJamNumber_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                EditModeModel mode = new EditModeModel();
                mode.EditModeType = EditModeEnum.upJamNumber;
                GameViewModel.Instance.EditModeItems.Add(mode);
                GameViewModel.Instance.CurrentJam.JamNumber += 1;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void downJamNumber_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditModeModel mode = new EditModeModel();
                mode.EditModeType = EditModeEnum.downJamNumber;
                GameViewModel.Instance.EditModeItems.Add(mode);
                GameViewModel.Instance.CurrentJam.JamNumber -= 1;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }


        private void upTeam2TimeOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditModeModel mode = new EditModeModel();
                mode.EditModeType = EditModeEnum.upTeam2TimeOut;
                GameViewModel.Instance.EditModeItems.Add(mode);
                GameViewModel.Instance.Team2.TimeOutsLeft += 1;
                TimeOutsRemainingTeam2.Text = GameViewModel.Instance.Team2.TimeOutsLeft.ToString();
                editTimeOutsRemainingTeam2Text.Text = GameViewModel.Instance.Team2.TimeOutsLeft.ToString();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void downTeam2TimeOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditModeModel mode = new EditModeModel();
                mode.EditModeType = EditModeEnum.downTeam2TimeOut;
                GameViewModel.Instance.EditModeItems.Add(mode);
                GameViewModel.Instance.Team2.TimeOutsLeft -= 1;
                TimeOutsRemainingTeam2.Text = GameViewModel.Instance.Team2.TimeOutsLeft.ToString();
                editTimeOutsRemainingTeam2Text.Text = GameViewModel.Instance.Team2.TimeOutsLeft.ToString();
                Logger.Instance.logMessage("time outs down team 2", LoggerEnum.message);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void wikiHelpLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //System.Diagnostics.Process.Start(ScoreboardConfig.SCOREBOARD_WIKI_URL);
                WebBrowser b = new WebBrowser();
                b.Navigate(ScoreboardConfig.SCOREBOARD_WIKI_URL, "_blank", null, null);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        private void onlineSupportLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //System.Diagnostics.Process.Start(ScoreboardConfig.SCOREBOARD_ONLINE_HELP_URL);
                WebBrowser b = new WebBrowser();
                b.Navigate(ScoreboardConfig.SCOREBOARD_ONLINE_HELP_URL, "_blank", null, null);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void CRGWindow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClockView cv = new Views.ClockView();
                ClockViewArray.Add(cv);
                cv.Show();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }



        /// <summary>
        /// user entered a number to change the team 1 time outs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editTimeOutsRemainingTeam1Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string text = editTimeOutsRemainingTeam1Text.Text;
                if (StringExt.IsNumber(text) && GameViewModel.Instance.IsInEditMode)
                {
                    EditModeModel mode = new EditModeModel();
                    mode.EditModeType = EditModeEnum.Team1TimeOutChange;
                    mode.additionalInformation = StringExt.NumberRegex.Match(text).Value;
                    GameViewModel.Instance.EditModeItems.Add(mode);
                    GameViewModel.Instance.Team1.TimeOutsLeft = Convert.ToInt32(StringExt.NumberRegex.Match(text).Value);
                    TimeOutsRemainingTeam1.Text = GameViewModel.Instance.Team1.TimeOutsLeft.ToString();
                    editTimeOutsRemainingTeam1Text.Text = GameViewModel.Instance.Team1.TimeOutsLeft.ToString();
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// user entered a number to change the team 2 time outs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editTimeOutsRemainingTeam2Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string text = editTimeOutsRemainingTeam2Text.Text;
                if (StringExt.IsNumber(text) && GameViewModel.Instance.IsInEditMode)
                {
                    EditModeModel mode = new EditModeModel();
                    mode.EditModeType = EditModeEnum.Team2TimeOutChange;
                    mode.additionalInformation = StringExt.NumberRegex.Match(text).Value;
                    GameViewModel.Instance.EditModeItems.Add(mode);
                    GameViewModel.Instance.Team2.TimeOutsLeft = Convert.ToInt32(StringExt.NumberRegex.Match(text).Value);
                    TimeOutsRemainingTeam1.Text = GameViewModel.Instance.Team1.TimeOutsLeft.ToString();
                    editTimeOutsRemainingTeam1Text.Text = GameViewModel.Instance.Team1.TimeOutsLeft.ToString();
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// changes the period number on the user typing something different.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editPeriodNumberText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string text = editPeriodNumberText.Text;
                if (StringExt.IsNumber(text) && GameViewModel.Instance.IsInEditMode)
                {
                    EditModeModel mode = new EditModeModel();
                    mode.EditModeType = EditModeEnum.periodNumberChanged;
                    GameViewModel.Instance.EditModeItems.Add(mode);
                    GameViewModel.Instance.CurrentPeriod = Convert.ToInt32(StringExt.NumberRegex.Match(text).Value);
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// changes the jam number on user input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editJamNumberText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string text = editJamNumberText.Text;
                if (StringExt.IsNumber(text) && GameViewModel.Instance.IsInEditMode)
                {
                    EditModeModel mode = new EditModeModel();
                    mode.EditModeType = EditModeEnum.jamNumberChanged;
                    if (GameViewModel.Instance.EditModeItems == null)
                        GameViewModel.Instance.EditModeItems = new List<EditModeModel>();

                    GameViewModel.Instance.EditModeItems.Add(mode);
                    GameViewModel.Instance.CurrentJam.JamNumber = Convert.ToInt32(StringExt.NumberRegex.Match(text).Value);
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        #region Edit Mode for the scoreboard
        private void editLineUpClockText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    e.Handled = true;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: _currentCursorPosition + ":" + e.Key + ":" + editPeriodClockText.Text + ":" + Logger.Instance.getLoggedMessages());
            }
        }

        /// <summary>
        /// changes teh character in the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editLineUpClockText_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    e.Handled = true;
                    return;
                }

                chooseCaretIndexPosition(e, editLineUpClockText);
                _currentTextBoxSelected = _TextBoxEnum.LineUpTime;
                if (IsEditTextKey(e))
                {
                    string text = "";
                    Logger.Instance.logMessage("textbox Length:" + editLineUpClockText.Text, LoggerEnum.message);
                    if (editLineUpClockText.Text.Length > 0 && _currentCursorPosition <= editLineUpClockText.Text.Length)
                        text = editLineUpClockText.Text.Remove(_currentCursorPosition - 1, 1).Insert(_currentCursorPosition - 1, getKeyString(e.Key));

                    if (Clocks.CLOCK_CHECK.IsMatch(text) && GameViewModel.Instance.IsInEditMode)
                    {
                        EditModeModel mode = new EditModeModel();
                        mode.EditModeType = EditModeEnum.lineUpClockChanged;
                        mode.additionalInformation = Clocks.CLOCK_CHECK.Match(text).Value;
                        GameViewModel.Instance.EditModeItems.Add(mode);
                        GameViewModel.Instance.CurrentLineUpClock.changeSecondsOfClock(Convert.ToInt32(Clocks.convertTimeDisplayToSeconds(Clocks.CLOCK_CHECK.Match(text).Value)));
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            //positions the cursor
            e.Handled = true;
        }

        private void editIntermissionClockText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    e.Handled = true;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: _currentCursorPosition + ":" + e.Key + ":" + editPeriodClockText.Text + ":" + Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// eidts the intermission value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editIntermissionClockText_PreviewKeyUp(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    e.Handled = true;
                    return;
                }

                chooseCaretIndexPosition(e, editIntermissionClockText);
                _currentTextBoxSelected = _TextBoxEnum.IntermissionTime;
                if (IsEditTextKey(e))
                {
                    string text = "";
                    Logger.Instance.logMessage("textbox Length:" + editIntermissionClockText.Text, LoggerEnum.message);
                    if (editIntermissionClockText.Text.Length > 0 && _currentCursorPosition <= editIntermissionClockText.Text.Length)
                        text = editIntermissionClockText.Text.Remove(_currentCursorPosition - 1, 1).Insert(_currentCursorPosition - 1, getKeyString(e.Key));

                    if (Clocks.CLOCK_CHECK.IsMatch(text) && GameViewModel.Instance.IsInEditMode)
                    {
                        EditModeModel mode = new EditModeModel();
                        mode.EditModeType = EditModeEnum.intermissionClockChanged;
                        mode.additionalInformation = Clocks.CLOCK_CHECK.Match(text).Value;
                        GameViewModel.Instance.EditModeItems.Add(mode);
                        GameViewModel.Instance.IntermissionClock.changeSecondsOfClock(Convert.ToInt32(Clocks.convertTimeDisplayToSeconds(Clocks.CLOCK_CHECK.Match(text).Value)));
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: _currentCursorPosition + ":" + e.Key + ":" + editIntermissionClockText.Text + ":" + Logger.Instance.getLoggedMessages());
            }
            e.Handled = true;
        }

        private void editJamClockText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    e.Handled = true;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: _currentCursorPosition + ":" + e.Key + ":" + editPeriodClockText.Text + ":" + Logger.Instance.getLoggedMessages());
            }
        }

        private void editJamClockText_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    e.Handled = true;
                    return;
                }

                //positions the cursor
                chooseCaretIndexPosition(e, editJamClockText);
                _currentTextBoxSelected = _TextBoxEnum.JamTime;

                if (IsEditTextKey(e))
                {
                    string text = "";
                    //check length bedcause of this error
                    //Index and count must refer to a location within the string. Parameter name: count
                    Logger.Instance.logMessage("textbox Length:" + editJamClockText.Text, LoggerEnum.message);
                    if (editJamClockText.Text.Length > 0 && _currentCursorPosition <= editJamClockText.Text.Length)
                        text = editJamClockText.Text.Remove(_currentCursorPosition - 1, 1).Insert(_currentCursorPosition - 1, getKeyString(e.Key));

                    if (Clocks.CLOCK_CHECK.IsMatch(text) && GameViewModel.Instance.IsInEditMode)
                    {
                        EditModeModel mode = new EditModeModel();
                        mode.EditModeType = EditModeEnum.jamClockChanged;
                        mode.additionalInformation = Clocks.CLOCK_CHECK.Match(text).Value;
                        GameViewModel.Instance.EditModeItems.Add(mode);
                        GameViewModel.Instance.CurrentJam.JamClock.changeSecondsOfClock(Convert.ToInt32(Clocks.convertTimeDisplayToSeconds(Clocks.CLOCK_CHECK.Match(text).Value)));
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }

            e.Handled = true;
        }

        /// <summary>
        /// chooses where the cursor sits based on the key pressed.
        /// We had to do this because when the clock ticks, it moves the caret back to its start and we cant
        /// have that whent he user clicks on this box to edit it.
        /// </summary>
        /// <param name="e"></param>
        private void chooseCaretIndexPosition(KeyEventArgs e, TextBox textBox)
        {
            try
            {
                if (e.Key == Key.Right || e.Key == Key.Left)
                    _currentCursorPosition = textBox.CaretIndex;
                else
                    _currentCursorPosition = textBox.CaretIndex + 1;
                Logger.Instance.logMessage("current Caret Position: " + _currentCursorPosition, LoggerEnum.message);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        /// <summary>
        /// the edit text box for period clock to replace the character in the box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editPeriodClockText_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    e.Handled = true;
                    return;
                }
                chooseCaretIndexPosition(e, editPeriodClockText);
                _currentTextBoxSelected = _TextBoxEnum.PeriodTime;

                if (IsEditTextKey(e))
                {
                    string text = string.Empty;
                    Logger.Instance.logMessage("textbox Length:" + editPeriodClockText.Text, LoggerEnum.message);
                    if (editPeriodClockText.Text.Length > 0 && _currentCursorPosition <= editPeriodClockText.Text.Length)
                        text = editPeriodClockText.Text.Remove(_currentCursorPosition - 1, 1).Insert(_currentCursorPosition - 1, getKeyString(e.Key));

                    if (Clocks.CLOCK_CHECK.IsMatch(text) && GameViewModel.Instance.IsInEditMode)
                    {
                        EditModeModel mode = new EditModeModel();
                        mode.EditModeType = EditModeEnum.jamClockChanged;
                        mode.additionalInformation = Clocks.CLOCK_CHECK.Match(text).Value;
                        GameViewModel.Instance.EditModeItems.Add(mode);
                        GameViewModel.Instance.PeriodClock.changeSecondsOfClock(Convert.ToInt32(Clocks.convertTimeDisplayToSeconds(Clocks.CLOCK_CHECK.Match(text).Value)));
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: _currentCursorPosition + ":" + e.Key + ":" + editPeriodClockText.Text + ":" + Logger.Instance.getLoggedMessages());
            }

            e.Handled = true;
        }
        private void editPeriodClockText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    e.Handled = true;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: _currentCursorPosition + ":" + e.Key + ":" + editPeriodClockText.Text + ":" + Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// turns the edit mode off for the textbox.
        /// we use the edit mode to place characters and to move the caret around
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editJamClockText_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {

                ClockEditWarning.IsOpen = false;
                if (_currentTextBoxSelected == _TextBoxEnum.JamTime)
                    _currentTextBoxSelected = _TextBoxEnum.None;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        private void editJamClockText_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ClockEditWarning.PlacementTarget = editJamClockText;
                ClockEditWarning.IsOpen = true;

                _currentTextBoxSelected = _TextBoxEnum.JamTime;
                _currentCursorPosition = ((TextBox)sender).CaretIndex;
                Console.WriteLine(_currentCursorPosition);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void editJamClockText_GotMouseCapture(object sender, RoutedEventArgs e)
        {
            try
            {
                _currentTextBoxSelected = _TextBoxEnum.JamTime;
                _currentCursorPosition = ((TextBox)sender).CaretIndex;
                Console.WriteLine(_currentCursorPosition);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// turns the edit mode off for the textbox.
        /// we use the edit mode to place characters and to move the caret around
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editPeriodClockText_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {

                ClockEditWarning.IsOpen = false;
                if (_currentTextBoxSelected == _TextBoxEnum.PeriodTime)
                    _currentTextBoxSelected = _TextBoxEnum.None;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        private void editPeriodClockText_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ClockEditWarning.PlacementTarget = editPeriodClockText;
                ClockEditWarning.IsOpen = true;

                _currentTextBoxSelected = _TextBoxEnum.PeriodTime;
                _currentCursorPosition = ((TextBox)sender).CaretIndex;
                Console.WriteLine(_currentCursorPosition);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void editPeriodClockText_GotMouseCapture(object sender, RoutedEventArgs e)
        {
            try
            {
                _currentTextBoxSelected = _TextBoxEnum.PeriodTime;
                _currentCursorPosition = ((TextBox)sender).CaretIndex;
                Console.WriteLine(_currentCursorPosition);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// turns the edit mode off for the textbox.
        /// we use the edit mode to place characters and to move the caret around
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editLineUpClockText_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {

                ClockEditWarning.IsOpen = false;

                if (_currentTextBoxSelected == _TextBoxEnum.LineUpTime)
                    _currentTextBoxSelected = _TextBoxEnum.None;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        private void editLineUpClockText_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ClockEditWarning.PlacementTarget = editLineUpClockText;
                ClockEditWarning.IsOpen = true;

                _currentTextBoxSelected = _TextBoxEnum.LineUpTime;
                _currentCursorPosition = ((TextBox)sender).CaretIndex;
                Console.WriteLine(_currentCursorPosition);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void editLineUpClockText_GotMouseCapture(object sender, RoutedEventArgs e)
        {
            try
            {
                _currentTextBoxSelected = _TextBoxEnum.LineUpTime;
                _currentCursorPosition = ((TextBox)sender).CaretIndex;
                Console.WriteLine(_currentCursorPosition);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        /// <summary>
        /// we are making sure that a user can type all letters even with keyboard bindings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gameNameText_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                //we remove the key bindings when the cursor is inside the text box.
                //keyboard is focused on textbox
                if ((bool)e.NewValue)
                    clearKeyboardShortcuts();
                else
                    setKeyboardShortcuts();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// we are making sure that a user can type all letters even with keyboard bindings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gameLocationText_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                //we remove the key bindings when the cursor is inside the text box.
                if ((bool)e.NewValue)
                    clearKeyboardShortcuts();
                else
                    setKeyboardShortcuts();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void upIntermissionTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditModeModel mode = new EditModeModel();
                mode.EditModeType = EditModeEnum.upIntermissionTime;
                GameViewModel.Instance.EditModeItems.Add(mode);
                GameViewModel.Instance.IntermissionClock.AddSecondsToClock(1);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void downIntermissionTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditModeModel mode = new EditModeModel();
                mode.EditModeType = EditModeEnum.downIntermissionTime;
                GameViewModel.Instance.EditModeItems.Add(mode);
                GameViewModel.Instance.IntermissionClock.AddSecondsToClock(-1);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void editIntermissionClockText_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {

                ClockEditWarning.IsOpen = false;

                if (_currentTextBoxSelected == _TextBoxEnum.IntermissionTime)
                    _currentTextBoxSelected = _TextBoxEnum.None;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        private void editIntermissionClockText_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ClockEditWarning.PlacementTarget = editIntermissionClockText;
                ClockEditWarning.IsOpen = true;

                _currentTextBoxSelected = _TextBoxEnum.IntermissionTime;
                _currentCursorPosition = ((TextBox)sender).CaretIndex;
                Console.WriteLine(_currentCursorPosition);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void editIntermissionClockText_GotMouseCapture(object sender, RoutedEventArgs e)
        {
            try
            {
                _currentTextBoxSelected = _TextBoxEnum.IntermissionTime;
                _currentCursorPosition = ((TextBox)sender).CaretIndex;
                Console.WriteLine(_currentCursorPosition);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }


        /// <summary>
        /// checks if the values of 123456789DeleteBackSpace have been pressed.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static bool IsEditTextKey(KeyEventArgs e)
        {
            try
            {
                //123456789DeleteBackSpace
                if (e.Key == Key.NumPad0 || e.Key == Key.NumPad1 || e.Key == Key.NumPad2 || e.Key == Key.NumPad3 || e.Key == Key.NumPad4 || e.Key == Key.NumPad5 || e.Key == Key.NumPad6 || e.Key == Key.NumPad7 || e.Key == Key.NumPad8 || e.Key == Key.NumPad9 || e.Key == Key.D0 || e.Key == Key.D1 || e.Key == Key.D2 || e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5 || e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9)
                {
                    return true;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            return false;
        }

        /// <summary>
        /// gets the string representation of the ENUM key value.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string getKeyString(Key key)
        {
            switch (key)
            {
                case Key.NumPad0:
                case Key.D0:
                    return "0";
                case Key.NumPad1:
                case Key.D1:
                    return "1";
                case Key.NumPad2:
                case Key.D2:
                    return "2";
                case Key.NumPad3:
                case Key.D3:
                    return "3";
                case Key.NumPad4:
                case Key.D4:
                    return "4";
                case Key.NumPad5:
                case Key.D5:
                    return "5";
                case Key.NumPad6:
                case Key.D6:
                    return "6";
                case Key.NumPad7:
                case Key.D7:
                    return "7";
                case Key.NumPad8:
                case Key.D8:
                    return "8";
                case Key.NumPad9:
                case Key.D9:
                    return "9";
                default:
                    return string.Empty;
            }
        }



        #endregion


        private void SettingsView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                settingsView = new SettingsView();
                settingsView.Show();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {


        }

        private void ReleaseNotesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var releaseNotes = new ReleaseNotes();
                releaseNotes.Show();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// this click will bring the user to a online page so they can manage the game online.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlineAdministration_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //System.Diagnostics.Process.Start(GameViewModel.Instance.UrlForAdministeringGameOnline);
                WebBrowser b = new WebBrowser();
                b.Navigate(GameViewModel.Instance.UrlForAdministeringGameOnline, "_blank", null, null);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void editIntermissionName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                GameViewModel.Instance.NameOfIntermission = ((TextBox)sender).Text;
                EditModeModel mode = new EditModeModel();
                mode.EditModeType = EditModeEnum.IntermissionName;
                mode.additionalInformation = GameViewModel.Instance.NameOfIntermission;
                GameViewModel.Instance.EditModeItems.Add(mode);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }


        private void editIntermissionName_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                //we remove the key bindings when the cursor is inside the text box.
                if ((bool)e.NewValue)
                    clearKeyboardShortcuts();
                else
                    setKeyboardShortcuts();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void team1JammerLead_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void ExportMadeReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Instance.logMessage("Saving the MADE Report", LoggerEnum.message);
                if (!Directory.Exists(ScoreboardConfig.SAVE_REPORTS_FILE_LOCATION))
                    Directory.CreateDirectory(ScoreboardConfig.SAVE_REPORTS_FILE_LOCATION);
                var dlg = new SaveFileDialog();
                //changes the game name since its final.

                dlg.FileName = "MADE_Report_" + RDN.Utilities.Strings.StringExt.ToExcelFriendly(GameViewModel.Instance.GameName) + "_" + DateTime.UtcNow.ToString("yyyy-MMMM-dd");

                dlg.Filter = "Excel Documents (*.xlsx)|*.xlsx";
                dlg.OverwritePrompt = true;
                dlg.RestoreDirectory = true;
                dlg.InitialDirectory = ScoreboardConfig.SAVE_REPORTS_FILE_LOCATION;

                bool? result = dlg.ShowDialog();

                if (result == true)
                    MadeReportExport.SaveMadeReport(dlg.FileName);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }
        private void ExportWFTDAReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Instance.logMessage("Saving the WFTDA Report", LoggerEnum.message);
                if (!Directory.Exists(ScoreboardConfig.SAVE_REPORTS_FILE_LOCATION))
                    Directory.CreateDirectory(ScoreboardConfig.SAVE_REPORTS_FILE_LOCATION);
                var dlg = new SaveFileDialog();
                //changes the game name since its final.

                dlg.FileName = "STATS-" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "_" + RDN.Utilities.Strings.StringExt.ToExcelFriendly(GameViewModel.Instance.Team1.TeamName) + "_vs_" + RDN.Utilities.Strings.StringExt.ToExcelFriendly(GameViewModel.Instance.Team2.TeamName);

                dlg.Filter = "Excel Documents (*.xlsx)|*.xlsx";
                dlg.OverwritePrompt = true;
                dlg.RestoreDirectory = true;
                dlg.InitialDirectory = ScoreboardConfig.SAVE_REPORTS_FILE_LOCATION;

                bool? result = dlg.ShowDialog();

                if (result == true)
                {
                    WftdaReport wftda = new WftdaReport();
                    wftda.Initialize().SetSaveFileName(dlg.FileName).SetOpenFileName(@"Resources/stats-book-03-2013.xlsx").Export();
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }
        private void ExportRDNationReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Instance.logMessage("Saving the RDNation Report", LoggerEnum.message);
                if (!Directory.Exists(ScoreboardConfig.SAVE_REPORTS_FILE_LOCATION))
                    Directory.CreateDirectory(ScoreboardConfig.SAVE_REPORTS_FILE_LOCATION);
                var dlg = new SaveFileDialog();
                //changes the game name since its final.

                dlg.FileName = "RDN-STATS-" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "_" + RDN.Utilities.Strings.StringExt.ToExcelFriendly(GameViewModel.Instance.Team1.TeamName) + "_vs_" + RDN.Utilities.Strings.StringExt.ToExcelFriendly(GameViewModel.Instance.Team2.TeamName);

                dlg.Filter = "Excel Documents (*.xlsx)|*.xlsx";
                dlg.OverwritePrompt = true;
                dlg.RestoreDirectory = true;
                dlg.InitialDirectory = ScoreboardConfig.SAVE_REPORTS_FILE_LOCATION;

                bool? result = dlg.ShowDialog();

                if (result == true)
                {
                    RDNationReport rdn = new RDNationReport();
                    rdn.Initialize().SetSaveFileName(dlg.FileName).SetOpenFileName(@"Resources/RDNBook.xlsx").Export();
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void gameCityText_IsKeyboardFocusedChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                //we remove the key bindings when the cursor is inside the text box.
                if ((bool)e.NewValue)
                    clearKeyboardShortcuts();
                else
                    setKeyboardShortcuts();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void gameCityText_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                GameViewModel.Instance.GameCity = ((TextBox)sender).Text;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void gameStateText_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                GameViewModel.Instance.GameState = ((TextBox)sender).Text;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void gameStateText_IsKeyboardFocusedChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                //we remove the key bindings when the cursor is inside the text box.
                if ((bool)e.NewValue)
                    clearKeyboardShortcuts();
                else
                    setKeyboardShortcuts();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
    }
}
