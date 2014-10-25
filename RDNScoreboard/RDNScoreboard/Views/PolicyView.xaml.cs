using System;
using System.Windows;
using System.Windows.Controls;
using Scoreboard.Library.ViewModel;
using Scoreboard.Library.Static.Enums;
using RDN.Utilities.Config;
using RDN.Utilities.Error;

using System.Linq;
using RDN.Utilities.Util;
using NAudio.Wave;
using System.IO;
namespace RDNScoreboard.Views
{

    /// <summary>
    /// Interaction logic for PolicyView.xaml
    /// </summary>
    public partial class PolicyView : Window
    {

        private static readonly string MADE_TIMEOUTS_PER_PERIOD_TEXT = "Each Team gets a certain number of time outs per period";
        private static readonly string WFTDA_TIMEOUTS_PER_PERIOD_TEXT = "Each Team gets a certain number of time outs per game";
        WaveOut buzzer;
        public PolicyView()
        {
            try
            {
                InitializeComponent();
                Title = "Policy Controller - " + ScoreboardConfig.SCOREBOARD_NAME;
                PolicyViewModel.Instance.loadFromXmlPolicy();
                GameTypeCbo.ItemsSource = Enum.GetValues(typeof(GameTypeEnum));
                loadsavedSettings();
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// loads the saved settings from the polciy object
        /// </summary>
        private void loadsavedSettings()
        {
            try
            {
                AdChangeAutomaticallyChangeImageButton.IsChecked = PolicyViewModel.Instance.AdChangeAutomaticallyChangeImage;
                AdChangeShowAdsDuringIntermissionButton.IsChecked = PolicyViewModel.Instance.AdChangeShowAdsDuringIntermission;
                AdChangeUseLineUpClockButton.IsChecked = PolicyViewModel.Instance.AdChangeUseLineUpClock;
                AlwaysShowJamClockButton.IsChecked = PolicyViewModel.Instance.AlwaysShowJamClock;
                LineupClockControlsStartJamPolicyButton.IsChecked = PolicyViewModel.Instance.LineupClockControlsStartJam;
                TimeSpan adChangeSeconds = TimeSpan.FromSeconds(PolicyViewModel.Instance.AdChangeDisplayChangesInMilliSeconds / 1000);
                if (adChangeSeconds.Seconds < 10)
                {
                    AdChangeDisplayChangesInSecondsText.Text = "_" + adChangeSeconds.Seconds;
                }
                else
                {
                    AdChangeDisplayChangesInSecondsText.Text = adChangeSeconds.Seconds.ToString();
                }

                EnableAdChangeButton.IsChecked = PolicyViewModel.Instance.EnableAdChange;
                EnableIntermissionNamingButton.IsChecked = PolicyViewModel.Instance.EnableIntermissionNaming;
                ShowActiveJammerPictures.IsChecked = PolicyViewModel.Instance.ShowActiveJammerPictures;
                FirstIntermissionNameConfirmedText.Text = PolicyViewModel.Instance.FirstIntermissionNameConfirmedText;
                FirstIntermissionNameText.Text = PolicyViewModel.Instance.FirstIntermissionNameText;
                HideClockTimeAfterBoutButton.IsChecked = PolicyViewModel.Instance.HideClockTimeAfterBout;
                IntermissionOtherText.Text = PolicyViewModel.Instance.IntermissionOtherText;
                TimeSpan clockTime = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.IntermissionStartOfClockInMilliseconds);
                IntermissionClockTimeText.Text = formatClockTime(clockTime);

                TimeSpan jamTime = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.JamClockTimePerJam);
                JamClockTimePerJamText.Text = formatClockTime(jamTime);

                TimeSpan periodTime = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.PeriodClock);
                PeriodClockText.Text = formatClockTime(periodTime);

                TimeSpan timeOutTime = TimeSpan.FromMilliseconds(PolicyViewModel.Instance.TimeOutClock);
                TimeOutClockText.Text = formatClockTime(timeOutTime);

                JamClockControlsLineUpClockButton.IsChecked = PolicyViewModel.Instance.JamClockControlsLineUpClock;
                JamClockControlsTeamPositionsButton.IsChecked = PolicyViewModel.Instance.JamClockControlsTeamPositions;
                PenaltyBoxControlsLeadJammerButton.IsChecked = PolicyViewModel.Instance.PenaltyBoxControlsLeadJammer;
                SecondIntermissionNameConfirmedText.Text = PolicyViewModel.Instance.SecondIntermissionNameConfirmedText;
                SecondIntermissionNameText.Text = PolicyViewModel.Instance.SecondIntermissionNameText;
                NumberOfPeriodsText.Text = PolicyViewModel.Instance.NumberOfPeriods.ToString();

                for (int i = 0; i < GameTypeCbo.Items.Count; i++)
                    if (GameTypeCbo.Items[i].ToString() == PolicyViewModel.Instance.GameSelectionType.ToString())
                        GameTypeCbo.SelectedIndex = i;

                NumberOfTimeOutsPerPeriodText.Text = PolicyViewModel.Instance.TimeOutsPerPeriod.ToString();

                StopPeriodClockOnZeroLineUp.IsChecked = PolicyViewModel.Instance.StopPeriodClockWhenLineUpClockHitsZero;
                StopLineUpClockOnZeroLineUp.IsChecked = PolicyViewModel.Instance.StopLineUpClockAtZero;
                lineUpTrackerControlsPoints.IsChecked = PolicyViewModel.Instance.LineUpTrackerControlsScore;
                SoundBuzzerAtEndOfPeriod.IsChecked = PolicyViewModel.Instance.SoundBuzzerAtEndOfPeriod;

                EnableDisableAds();
                enableDisableIntermissionStopClock();
                EnableDisableIntermissionClock();


                setTextForGameSelectionType();
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// sets the text in the view for the game type chosen.
        /// </summary>
        private void setTextForGameSelectionType()
        {
            try
            {
                if (PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.WFTDA || PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.WFTDA_2010 || PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.RDCL || PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.USARS)
                    timeOutsPerPeriodText.Text = WFTDA_TIMEOUTS_PER_PERIOD_TEXT;
                else if (PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.TEXAS_DERBY || PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.MADE || PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.MADE_COED)
                    timeOutsPerPeriodText.Text = MADE_TIMEOUTS_PER_PERIOD_TEXT;
                else
                    timeOutsPerPeriodText.Text = MADE_TIMEOUTS_PER_PERIOD_TEXT;
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private static string formatClockTime(TimeSpan clockTime)
        {
            string clockText = String.Empty;
            try
            {
                if (clockTime.Minutes < 10)
                { clockText = clockTime.Minutes.ToString(); }
                else
                {
                    clockText = clockTime.Minutes.ToString();
                }

                if (clockTime.Seconds < 10)
                {
                    if (clockTime.Seconds == 0)
                    {
                        clockText += ":00";
                    }
                    else
                    {
                        clockText += ":0" + clockTime.Seconds;
                    }
                }
                else { clockText += ":" + clockTime.Seconds; }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, e.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
            return clockText;
        }
        /// <summary>
        /// Countdown Clock Controls Start of Jam: Starts the Jam automatically when the time between jams has ended.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LineupClockControlsStartJamPolicyButton_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.LineupClockControlsStartJam = !PolicyViewModel.Instance.LineupClockControlsStartJam;
        }
        /// <summary>
        /// Enable Intermission Start Clock: Starts the intermission clock to the default or designated intermission time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IntermissionClockStartEnableButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PolicyViewModel.Instance.EnableIntermissionStartOfClock = !PolicyViewModel.Instance.EnableIntermissionStartOfClock;
                EnableDisableIntermissionClock();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void EnableDisableIntermissionClock()
        {
            IntermissionClockTimeText.IsEnabled = PolicyViewModel.Instance.EnableIntermissionStartOfClock;
        }

        /// <summary>
        /// Enable Intermission Stop Clock: When the Intermission clock is over, this resets the Period and Jam clock time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IntermissionStopClockEnableButton_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.IntermissionStopClockEnable = !PolicyViewModel.Instance.IntermissionStopClockEnable;


        }

        private void enableDisableIntermissionStopClock()
        {
        }
        /// <summary>
        /// Reset Jam Time At Stop of Intermission: Resets the jam Time to zero automatically at stop of intermission.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IntermissionStopClockResetJamTimeButton_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.IntermissionStopClockResetJamTime = !PolicyViewModel.Instance.IntermissionStopClockResetJamTime;
        }
        /// <summary>
        /// Reset Period Time At Stop of Intermission Clock.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IntermissionStopClockResetPeriodTimeButton_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.IntermissionStopClockResetPeriodTime = !PolicyViewModel.Instance.IntermissionStopClockResetPeriodTime;
        }
        /// <summary>
        /// Starts the line up clock when the jam clock stops.  Starts the Jam clock when the line up clock stops.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JamClockControlsLineUpClockButton_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.JamClockControlsLineUpClock = !PolicyViewModel.Instance.JamClockControlsLineUpClock;
        }
        /// <summary>
        /// This clears all Team Positions (who are not in the Penalty Box) when the Jam clock is stopped, sets all Skaters to Not Lead Jammer, and sets the Team to Not Lead Jammer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JamClockControlsTeamPositionsButton_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.JamClockControlsTeamPositions = !PolicyViewModel.Instance.JamClockControlsTeamPositions;
        }
        /// <summary>
        /// This removes Lead Jammer from any Skater sent to the Penalty Box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PenaltyBoxControlsLeadJammerButton_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.PenaltyBoxControlsLeadJammer = !PolicyViewModel.Instance.PenaltyBoxControlsLeadJammer;
        }
        /// <summary>
        /// This controls the Lineup clock based on the Period clock. When the Period clock stops and its time is equal to its 0 (i.e. its minimum), and the Jam clock is also stopped, the Lineup clock is stopped and reset.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PeriodClockControlsLineupJamClockButton_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.PeriodClockControlsLineupJamClock = !PolicyViewModel.Instance.PeriodClockControlsLineupJamClock;
        }
        /// <summary>
        /// Enables the Ad change policy on the board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnableAdChangeButton_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.EnableAdChange = !PolicyViewModel.Instance.EnableAdChange;

            EnableDisableAds();
        }
        /// <summary>
        /// Enables and disables ad controls when ad policy changes
        /// </summary>
        private void EnableDisableAds()
        {
            AdChangeUseLineUpClockButton.IsEnabled = PolicyViewModel.Instance.EnableAdChange;
            AdChangeDisplayChangesInSecondsText.IsEnabled = PolicyViewModel.Instance.EnableAdChange;
            AdChangeAutomaticallyChangeImageButton.IsEnabled = PolicyViewModel.Instance.EnableAdChange;
            AdChangeShowAdsDuringIntermissionButton.IsEnabled = PolicyViewModel.Instance.EnableAdChange;
        }
        /// <summary>
        /// Shows the adds where the line up clock sits.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdChangeUseLineUpClockButton_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.AdChangeUseLineUpClock = !PolicyViewModel.Instance.AdChangeUseLineUpClock;
        }
        /// <summary>
        /// Shows the latest points from the previous 5 jams between jams
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowActiveJammerPictures_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.ShowActiveJammerPictures = !PolicyViewModel.Instance.ShowActiveJammerPictures;
        }
        /// <summary>
        /// Ads change at the seconds specified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdChangeDisplayChangesInSecondsText_TextChanged(object sender, TextChangedEventArgs e)
        {
            //simple try catch to not invoke errors when in the process of entering the number
            try
            {
                long seconds = int.Parse(AdChangeDisplayChangesInSecondsText.Text.Replace("_", "")) * 1000;
                PolicyViewModel.Instance.AdChangeDisplayChangesInMilliSeconds = seconds;
            }
            catch { }
        }
        /// <summary>
        /// Automatically change the ad image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdChangeAutomaticallyChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.AdChangeAutomaticallyChangeImage = !PolicyViewModel.Instance.AdChangeAutomaticallyChangeImage;
        }
        /// <summary>
        /// Show Ads during intermission
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdChangeShowAdsDuringIntermissionButton_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.AdChangeShowAdsDuringIntermission = !PolicyViewModel.Instance.AdChangeShowAdsDuringIntermission;
        }
        /// <summary>
        /// Show Ads in Random Order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdChangeShowAdsRandomlyButton_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.AdChangeShowAdsRandomly = !PolicyViewModel.Instance.AdChangeShowAdsRandomly;
        }
        /// <summary>
        /// Always Show Jam Clock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlwaysShowJamClockButton_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.AlwaysShowJamClock = !PolicyViewModel.Instance.AlwaysShowJamClock;
        }
        /// <summary>
        /// This controls the Lineup clock based on the Timeout clock. When the Timeout clock starts, the Lineup clock is stopped then reset.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeoutClockControlsLineupClockButton_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.TimeoutClockControlsLineupClock = !PolicyViewModel.Instance.TimeoutClockControlsLineupClock;
        }
        /// <summary>
        /// Enable the intermission naming scheme.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnableIntermissionNamingButton_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.EnableIntermissionNaming = !PolicyViewModel.Instance.EnableIntermissionNaming;
        }
        /// <summary>
        /// Hides the clocks after Bout is over.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideClockTimeAfterBoutButton_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.HideClockTimeAfterBout = !PolicyViewModel.Instance.HideClockTimeAfterBout;
        }

        /// <summary>
        /// Name of the First Intermission
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FirstIntermissionNameText_TextChanged(object sender, TextChangedEventArgs e)
        {
            PolicyViewModel.Instance.FirstIntermissionNameText = FirstIntermissionNameText.Text;
        }
        /// <summary>
        /// Intermission Name after Confirmed Points
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FirstIntermissionNameConfirmedText_TextChanged(object sender, TextChangedEventArgs e)
        {
            PolicyViewModel.Instance.FirstIntermissionNameConfirmedText = FirstIntermissionNameConfirmedText.Text;
        }


        /// <summary>
        /// Name of Second Intermission 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SecondIntermissionNameText_TextChanged(object sender, TextChangedEventArgs e)
        {
            PolicyViewModel.Instance.SecondIntermissionNameText = SecondIntermissionNameText.Text;
        }
        /// <summary>
        /// Name of Second Intermission Confirmed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SecondIntermissionNameConfirmedText_TextChanged(object sender, TextChangedEventArgs e)
        {
            PolicyViewModel.Instance.SecondIntermissionNameConfirmedText = SecondIntermissionNameConfirmedText.Text;
        }
        /// <summary>
        /// some other text used for intermission
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IntermissionOtherText_TextChanged(object sender, TextChangedEventArgs e)
        {
            PolicyViewModel.Instance.IntermissionOtherText = IntermissionOtherText.Text;
        }
        /// <summary>
        /// this is the default clock time for intermission.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IntermissionClockTimeText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!this.IsLoaded) return;
                string clock = IntermissionClockTimeText.Text.Replace("_", "");

                PolicyViewModel.Instance.IntermissionStartOfClockInMilliseconds = ConvertInputClockToMinuteSeconds(clock);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }

        }
        /// <summary>
        /// executes when window closes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //saves settings when the window closes.
            Logger.Instance.logMessage("policy view closing.", LoggerEnum.message);
            PolicyViewModel.Instance.savePolicyToXml();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //mainScroll.Height = windowMain.Height - 75;
        }

        private void JamClockTimePerJamText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!this.IsLoaded) return;

                string clock = JamClockTimePerJamText.Text.Replace("_", "");

                PolicyViewModel.Instance.JamClockTimePerJam = ConvertInputClockToMinuteSeconds(clock);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages() + JamClockTimePerJamText.Text);
            }
        }

        private static long ConvertInputClockToMinuteSeconds(string clock)
        {
            long minutes = 0;
            long seconds = 0;
            try
            {
                bool isMinutes = long.TryParse(clock.Split(':')[0], out minutes);
                if (isMinutes)
                    minutes = minutes * 60 * 1000;

                bool isSeconds = long.TryParse(clock.Split(':')[1], out seconds);
                if (isSeconds)
                    seconds = seconds * 1000;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages() + clock);
            }
            return minutes + seconds;
        }

        private void PeriodClockText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!this.IsLoaded) return;
                string clock = PeriodClockText.Text.Replace("_", "");

                PolicyViewModel.Instance.PeriodClock = ConvertInputClockToMinuteSeconds(clock);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void NumberOfPeriodsText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(NumberOfPeriodsText.Text))
                {
                    Logger.Instance.logMessage(NumberOfPeriodsText.Text, LoggerEnum.message);
                    int periods = 4;
                    bool converted = Int32.TryParse(NumberOfPeriodsText.Text, out periods);
                    if (converted)
                        PolicyViewModel.Instance.NumberOfPeriods = periods;
                    else
                        PolicyViewModel.Instance.NumberOfPeriods = 4;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void GameTypeCbo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PolicyViewModel.Instance.changeGameSelectionType((GameTypeEnum)Enum.Parse(typeof(GameTypeEnum), e.AddedItems[0].ToString()));
            setTextForGameSelectionType();
        }

        private void NumberOfTimeOutsPerPeriodText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(NumberOfTimeOutsPerPeriodText.Text))
                {
                    Logger.Instance.logMessage(NumberOfTimeOutsPerPeriodText.Text, LoggerEnum.message);

                    int periods = 2;
                    bool converted = Int32.TryParse(NumberOfTimeOutsPerPeriodText.Text, out periods);
                    if (converted)
                        PolicyViewModel.Instance.TimeOutsPerPeriod = periods;
                    else
                        PolicyViewModel.Instance.TimeOutsPerPeriod = 2;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void TimeOutClockText_TextChanged(object sender, TextChangedEventArgs e)
        {


            try
            {
                if (!this.IsLoaded) return;
                if (!String.IsNullOrEmpty(TimeOutClockText.Text))
                {
                    Logger.Instance.logMessage("clock:" + TimeOutClockText.Text, LoggerEnum.message);
                    string clock = TimeOutClockText.Text.Replace("_", "");
                    long minutes = Convert.ToInt64(clock.Split(':')[0]) * 60 * 1000;
                    long seconds = Convert.ToInt64(clock.Split(':')[1]) * 1000;
                    PolicyViewModel.Instance.TimeOutClock = minutes + seconds;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages() + RDN.Utilities.Util.Logger.Instance.getLoggedMessages());
            }

        }
        /// <summary>
        /// stop the period clock when the line up clock hits zero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopPeriodClockOnZeroLineUp_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.StopPeriodClockWhenLineUpClockHitsZero = !PolicyViewModel.Instance.StopPeriodClockWhenLineUpClockHitsZero;
        }
        /// <summary>
        /// stops the line up clock at zero.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopLineUpClockOnZeroLineUp_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.StopLineUpClockAtZero = !PolicyViewModel.Instance.StopLineUpClockAtZero;
        }

        private void lineUpTrackerControlsPoints_Click_1(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.LineUpTrackerControlsScore = !PolicyViewModel.Instance.LineUpTrackerControlsScore;
        }

        private void lineUpTrackerHelp_Click_1(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(ScoreboardConfig.SCOREBOARD_STATS_COLLECTION_SERVER_SETTINGS_WIKI_URL);
            WebBrowser b = new WebBrowser();
            b.Navigate(ScoreboardConfig.SCOREBOARD_STATS_COLLECTION_SERVER_SETTINGS_WIKI_URL, "_blank", null, null);
        }

        private void SoundBuzzerAtEndOfPeriod_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.SoundBuzzerAtEndOfPeriod = !PolicyViewModel.Instance.SoundBuzzerAtEndOfPeriod;
        }

        private void PlayBuzzerBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Instance.logMessage(PolicyViewModel.Instance.SoundBuzzerFileLocation, LoggerEnum.message);
                //string fileLocation = new Uri(PolicyViewModel.Instance.SoundBuzzerFileLocation, UriKind.RelativeOrAbsolute).ToString();
                var reader = new Mp3FileReader(PolicyViewModel.Instance.SoundBuzzerFileLocation);
                buzzer = new WaveOut();
                buzzer.Init(reader);
                buzzer.Play();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages() + RDN.Utilities.Util.Logger.Instance.getLoggedMessages());
            }

        }

        private void UploadBuzzerBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "Audio (*.MP3;*.WAV)|*.MP3;*.WAV";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string filename = dlg.FileName;
                    var info = new FileInfo(filename);
                    if (!String.IsNullOrEmpty(filename) && info.Exists)
                    {
                        try
                        {
                            string destinationFilePath = ScoreboardConfig.SAVE_FILES_FOLDER;
                            string destinationFileName = info.Name;

                            DirectoryInfo dir = new DirectoryInfo(ScoreboardConfig.SAVE_FILES_FOLDER);
                            if (!dir.Exists)
                                dir.Create();
                            Logger.Instance.logMessage("opening buzzer File:" + destinationFilePath + destinationFileName, LoggerEnum.message);
                            Logger.Instance.logMessage("filename:" + filename, LoggerEnum.message);
                            File.Copy(filename, System.IO.Path.Combine(destinationFilePath, destinationFileName), true);
                            PolicyViewModel.Instance.SoundBuzzerFileLocation = destinationFilePath + destinationFileName;
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

        private void ResetBuzzerDefault_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.ResetSoundBuzzer();
        }

        private void StopBuzzerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (buzzer != null)
                buzzer.Stop();
        }










    }
}
