using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Specialized;
using Scoreboard.Library.ViewModel;
using Scoreboard.Library.Static.Enums;

using RDN.Utilities.Version;
using RDN.Utilities.Config;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using RDNScoreboard.Views.TeamManagerTabs;
using Scoreboard.Library.Network;
using Microsoft.Win32;
using Scoreboard.Library.ViewModel.Members;
using Scoreboard.Library.ViewModel.Members.Enums;
using RDN.Utilities.Util;
using RDN.Utilities.Drawing;



namespace RDNScoreboard.Views
{
    /// <summary>
    /// Interaction logic for TeamManager.xaml
    /// </summary>
    public partial class TeamManager : Window
    {
        LogoManagerTab _logoManager;
        OfficialsManagerTab _officialManager;

        public TeamManager()
        {
            InitializeComponent();
            this.Title = "Team Manager - " + ScoreboardConfig.SCOREBOARD_NAME;


        }

        void Instance_OnNewGame(object sender, EventArgs e)
        {
            setupTeamManager();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }
        public void setupTeamManager()
        {
            TeamOneList.DataContext = GameViewModel.Instance.Team1.TeamMembers;
            TeamTwoList.DataContext = GameViewModel.Instance.Team2.TeamMembers;

            GameViewModel.Instance.Team2.TeamMembers.CollectionChanged += new NotifyCollectionChangedEventHandler(TeamMembers_CollectionChanged);
            GameViewModel.Instance.Team1.TeamMembers.CollectionChanged += new NotifyCollectionChangedEventHandler(TeamMembers_CollectionChanged);
            TeamName1.Text = GameViewModel.Instance.Team1.TeamName;
            TeamName2.Text = GameViewModel.Instance.Team2.TeamName;
            LeagueName1.Text = GameViewModel.Instance.Team1.LeagueName;
            LeagueName2.Text = GameViewModel.Instance.Team2.LeagueName;
            team1SkaterCount.Text = GameViewModel.Instance.Team1.TeamMembers.Count + " Skaters";
            team2SkaterCount.Text = GameViewModel.Instance.Team2.TeamMembers.Count + " Skaters";
            GameViewModel.Instance.OnNewGame += new EventHandler(Instance_OnNewGame);
        }

        void TeamMembers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TeamOneList.UpdateLayout();
            TeamTwoList.UpdateLayout();
            team1SkaterCount.Text = GameViewModel.Instance.Team1.TeamMembers.Count + " Skaters";
            team2SkaterCount.Text = GameViewModel.Instance.Team2.TeamMembers.Count + " Skaters";
        }

        private void submitSkaterTeam1_Click(object sender, RoutedEventArgs e)
        {
            GameViewModel.Instance.addTeamSkater(skaterNameTeam1.Text, skaterNumberTeam1.Text, TeamNumberEnum.Team1);
            skaterNameTeam1.Text = "";
            skaterNumberTeam1.Text = "";
            skaterNameTeam1.Focus();
            if (GameViewModel.Instance.Team1 != null)
                team1SkaterCount.Text = GameViewModel.Instance.Team1.TeamMembers.Count + " Skaters";
        }

        private void submitSkaterTeam2_Click(object sender, RoutedEventArgs e)
        {
            GameViewModel.Instance.addTeamSkater(skaterNameTeam2.Text, skaterNumberTeam2.Text, TeamNumberEnum.Team2);
            skaterNameTeam2.Text = "";
            skaterNumberTeam2.Text = "";
            skaterNameTeam2.Focus();
            if (GameViewModel.Instance.Team2 != null)
                team2SkaterCount.Text = GameViewModel.Instance.Team2.TeamMembers.Count + " Skaters";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setupTeamManager();
        }

        private void BenchTeam1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;
                GameViewModel.Instance.CurrentJam.benchSkater(obj, TeamNumberEnum.Team1);

            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried benching a skater: ", LoggerEnum.error);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        /// <summary>
        /// says the team 1 blocker 1 has been choses.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void blocker1Team1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;
                GameViewModel.Instance.CurrentJam.setBlocker1(obj, TeamNumberEnum.Team1);
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried setting blocker 1 a skater: ", LoggerEnum.error);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void blocker2Team1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;
                GameViewModel.Instance.CurrentJam.setBlocker2(obj, TeamNumberEnum.Team1);
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried setting blocker 2 a skater: ", LoggerEnum.error);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void blocker3Team1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;
                GameViewModel.Instance.CurrentJam.setBlocker3(obj, TeamNumberEnum.Team1);
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried setting blocker 3 a skater: ", LoggerEnum.error);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }
        private void blocker4Team1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;
                GameViewModel.Instance.CurrentJam.setBlocker4(obj, TeamNumberEnum.Team1);
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried setting blocker 4 a skater: ", LoggerEnum.error);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void pivotTeam1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;
                GameViewModel.Instance.CurrentJam.setPivot(obj, TeamNumberEnum.Team1);
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried setting pivot a skater: ", LoggerEnum.error);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void jammerTeam1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;
                GameViewModel.Instance.CurrentJam.setJammer(obj, TeamNumberEnum.Team1);
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried setting jammer a skater: ", LoggerEnum.error);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }




        private void gameSpecialTypeBoxTeam1_Click(object sender, RoutedEventArgs e)
        {
            TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;
            if (!obj.IsInBox)
                GameViewModel.Instance.sendSkaterToPenaltyBox(obj, TeamNumberEnum.Team1);
            else
                GameViewModel.Instance.removeSkaterFromPenaltyBox(obj, TeamNumberEnum.Team1);
        }

        private void deleteSkaterTeam1_Click(object sender, RoutedEventArgs e)
        {
            TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;
            GameViewModel.Instance.removeSkaterFromTeam(obj, TeamNumberEnum.Team1);
            if (GameViewModel.Instance.Team1 != null)
                team1SkaterCount.Text = GameViewModel.Instance.Team1.TeamMembers.Count + " Skaters";
        }

        private void deleteSkaterTeam2_Click(object sender, RoutedEventArgs e)
        {
            TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;
            GameViewModel.Instance.removeSkaterFromTeam(obj, TeamNumberEnum.Team2);
            if (GameViewModel.Instance.Team2 != null)
                team2SkaterCount.Text = GameViewModel.Instance.Team2.TeamMembers.Count + " Skaters";
        }

        private void benchTeam2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;
                GameViewModel.Instance.CurrentJam.benchSkater(obj, TeamNumberEnum.Team2);
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried benching a skater: ", LoggerEnum.error);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void blocker1Team2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;
                GameViewModel.Instance.CurrentJam.setBlocker1(obj, TeamNumberEnum.Team2);
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried setting blocker 1 a skater: ", LoggerEnum.error);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void blocker2Team2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;
                GameViewModel.Instance.CurrentJam.setBlocker2(obj, TeamNumberEnum.Team2);
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried setting blocker 2 a skater: ", LoggerEnum.error);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void blocker3Team2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;
                GameViewModel.Instance.CurrentJam.setBlocker3(obj, TeamNumberEnum.Team2);
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried setting blocker 3 a skater: ", LoggerEnum.error);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }
        private void blocker4Team2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;
                GameViewModel.Instance.CurrentJam.setBlocker4(obj, TeamNumberEnum.Team2);
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried setting blocker 4 a skater: ", LoggerEnum.error);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void pivotTeam2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;
                GameViewModel.Instance.CurrentJam.setPivot(obj, TeamNumberEnum.Team2);
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried setting pivot a skater: ", LoggerEnum.error);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void jammerTeam2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;
                GameViewModel.Instance.CurrentJam.setJammer(obj, TeamNumberEnum.Team2);
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried setting jammer a skater: ", LoggerEnum.error);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }



        private void boxSpecialTypeTeam2_Click(object sender, RoutedEventArgs e)
        {
            TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;
            if (!obj.IsInBox)
                GameViewModel.Instance.sendSkaterToPenaltyBox(obj, TeamNumberEnum.Team2);
            else
                GameViewModel.Instance.removeSkaterFromPenaltyBox(obj, TeamNumberEnum.Team2);


        }
        private void LeagueName1_TextChanged(object sender, TextChangedEventArgs e)
        {
            GameViewModel.Instance.Team1.LeagueName = LeagueName1.Text;
        }
        private void TeamName1_TextChanged(object sender, TextChangedEventArgs e)
        {
            GameViewModel.Instance.Team1.TeamName = TeamName1.Text;
        }
        private void LeagueName2_TextChanged(object sender, TextChangedEventArgs e)
        {
            GameViewModel.Instance.Team2.LeagueName = LeagueName2.Text;
        }
        private void TeamName2_TextChanged(object sender, TextChangedEventArgs e)
        {
            GameViewModel.Instance.Team2.TeamName = TeamName2.Text;
        }

        private void wikiHelpLink_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(ScoreboardConfig.SCOREBOARD_TEAM_MANAGER_WIKI_URL);
            WebBrowser b = new WebBrowser();
            b.Navigate(ScoreboardConfig.SCOREBOARD_TEAM_MANAGER_WIKI_URL, "_blank", null, null);
        }

        private void setPictureTeam1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = ((Button)sender);
                TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;

                obj.SkaterPictureLocation = UploadSkaterPicture(obj.SkaterId, MemberTypeEnum.Skater);

            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried loading skater picture to game for team 1: " + exception.Message, LoggerEnum.error);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        //public  static void setImageOnButton(Button btn, string pictureLocation)
        //{
        //    FileStream stream = new FileStream(pictureLocation, FileMode.Open);

        //    Image image = new Image();
        //    BitmapImage bi = new BitmapImage();
        //    bi.BeginInit();
        //    bi.StreamSource = stream;
        //    bi.CacheOption = BitmapCacheOption.OnLoad;
        //    bi.EndInit();
        //    image.Source = bi;
        //    image.Height = 15;
        //    btn.Content = image;
        //    stream.Close();
        //    stream.Dispose();
        //}

        private void setPictureTeam2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = ((Button)sender);
                TeamMembersViewModel obj = ((FrameworkElement)sender).DataContext as TeamMembersViewModel;

                obj.SkaterPictureLocation = UploadSkaterPicture(obj.SkaterId, MemberTypeEnum.Skater);
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried loading skater picture to game for team 2: " + exception.Message, LoggerEnum.error);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        /// <summary>
        /// uploads the skater picture to the skater picture folder and attaches it to the skater info.
        /// </summary>
        /// <param name="skater"></param>
        public static string UploadSkaterPicture(Guid skaterId, MemberTypeEnum memType)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Images (*.JPG;*.JPEG;*.PNG)|*.JPG;*.JPEG;*.PNG";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                if (!String.IsNullOrEmpty(filename))
                {
                    try
                    {
                        Logger.Instance.logMessage("uploading ref picture: " + filename, LoggerEnum.message);
                        string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SKATERS_PICTURE_FOLDER, skaterId.ToString().Replace("-", "") + System.IO.Path.GetExtension(filename));

                        DirectoryInfo dir = new DirectoryInfo(ScoreboardConfig.SAVE_SKATERS_PICTURE_FOLDER);
                        if (!dir.Exists)
                            dir.Create();

                        using (var im = System.Drawing.Image.FromFile(filename))
                        {

                            //800x800 width and heights
                            var imageTemp = Images.ScaleDownImage(im, ScoreboardConfig.DEFAULT_SIZE_OF_RESIZED_IMAGE, ScoreboardConfig.DEFAULT_SIZE_OF_RESIZED_IMAGE);
                            imageTemp.Save(destinationFilename);
                            //File.Copy(filename, destinationFilename, true);
                        }

                        if (Internet.CheckConnection())
                            GameViewModel.SendProfilePictureForMember(GameViewModel.Instance.GameId, skaterId, filename, memType);
                        return destinationFilename;
                    }
                    catch (Exception exception)
                    {
                        Logger.Instance.logMessage("tried uploading skater picture to game: " + exception.Message, LoggerEnum.error);
                        ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
                    }

                }

            }
            return String.Empty;
        }



        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (TeamManagerTab.IsSelected)
                {
                    _logoManager = null;
                    LogoManagerFrame.Content = null;
                    System.GC.Collect();
                }
                if (LogoManager.IsSelected)
                {
                    _logoManager = new LogoManagerTab();
                    _logoManager.setupView();
                    LogoManagerFrame.Content = _logoManager;
                }
                if (RefereeManager.IsSelected)
                {
                    _officialManager = new OfficialsManagerTab();
                    _officialManager.SetupView();
                    OfficialsManagerFrame.Content = _officialManager;
                }
            }
        }

        private void loadTeam1_Click_1(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION))
                Directory.CreateDirectory(ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION);
            try
            {
                var dlg = new OpenFileDialog();
                dlg.Filter = "Xml Documents (*.xml)|*.xml";
                dlg.CheckFileExists = true;
                dlg.InitialDirectory = ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION;

                bool? result = dlg.ShowDialog();
                if (result == false)
                    return;

                Logger.Instance.logMessage("Loading Team 1", LoggerEnum.message);
                GameViewModel.Instance.loadTeamRosterFromXml(dlg.FileName, TeamNumberEnum.Team1);
                Window owner = System.Windows.Application.Current.MainWindow;
                ((MainWindow)owner).SetupViewForTeamLoad();
                foreach (var scoreboard in ((MainWindow)owner).ClockViewArray)
                    scoreboard.LoadTeamView();
                setupTeamManager();

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void saveTeam1_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Instance.logMessage("Saving the Team 1 Rosters", LoggerEnum.message);
                if (!Directory.Exists(ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION))
                    Directory.CreateDirectory(ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION);
                var dlg = new SaveFileDialog();
                dlg.FileName = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(GameViewModel.Instance.Team1.TeamName + "-Roster-" + DateTime.Now.ToString("yyyy-MMMM-dd"));
                dlg.Filter = "Xml Documents (*.xml)|*.xml";
                dlg.OverwritePrompt = true;
                dlg.RestoreDirectory = true;
                dlg.InitialDirectory = ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION;

                bool? result = dlg.ShowDialog();

                if (result == true)
                    GameViewModel.Instance.saveTeamAndPicturesToXml(dlg.FileName, TeamNumberEnum.Team1);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void loadTeam2_Click_1(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION))
                Directory.CreateDirectory(ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION);
            try
            {
                var dlg = new OpenFileDialog();
                dlg.Filter = "Xml Documents (*.xml)|*.xml";
                dlg.CheckFileExists = true;
                dlg.InitialDirectory = ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION;

                bool? result = dlg.ShowDialog();
                if (result == false)
                    return;

                Logger.Instance.logMessage("Loading Team 2", LoggerEnum.message);
                GameViewModel.Instance.loadTeamRosterFromXml(dlg.FileName, TeamNumberEnum.Team2);
                Window owner = System.Windows.Application.Current.MainWindow;
                ((MainWindow)owner).SetupViewForTeamLoad();
                foreach (var scoreboard in ((MainWindow)owner).ClockViewArray)
                    scoreboard.LoadTeamView();
                setupTeamManager();

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void saveTeam2_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Instance.logMessage("Saving the Team 2 Rosters", LoggerEnum.message);
                if (!Directory.Exists(ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION))
                    Directory.CreateDirectory(ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION);
                var dlg = new SaveFileDialog();
                dlg.FileName = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(GameViewModel.Instance.Team2.TeamName + "-Roster-" + DateTime.Now.ToString("yyyy-MMMM-dd"));
                dlg.Filter = "Xml Documents (*.xml)|*.xml";
                dlg.OverwritePrompt = true;
                dlg.RestoreDirectory = true;
                dlg.InitialDirectory = ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION;

                bool? result = dlg.ShowDialog();

                if (result == true)
                    GameViewModel.Instance.saveTeamAndPicturesToXml(dlg.FileName, TeamNumberEnum.Team2);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

    }
}
