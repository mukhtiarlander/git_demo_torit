using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Scoreboard.Library.ViewModel;
using RDN.Utilities.Config;
using System.ComponentModel;
using System.IO;

using RDNScoreboard.Views.LineupTemplates;
using System.Diagnostics;
using Scoreboard.Library.ViewModel.Members;
using RDN.Utilities.Util;

namespace RDNScoreboard.Views.Tabs
{
    /// <summary>
    /// Interaction logic for SlideShowTab.xaml
    /// </summary>
    public partial class SlideShowTab : Page
    {

        private static readonly string MUST_SELECT_TEMPLATE = "Be Sure to Set a Template and Have started a New Game";
        private static readonly string MUST_SELECT_LOGO = "Please Select a Logo For This Team.";
        LargeSlideshowFrame _largeSlideShow = new LargeSlideshowFrame();

        MainWindow _window;
        public SlideShowTab()
        {
            InitializeComponent();
            this.Title = "SlideShow Manager - " + ScoreboardConfig.SCOREBOARD_NAME;

            LargeSlideShowFrame.Content = _largeSlideShow;

        }


        private void setupView()
        {

            GameViewModel.Instance.Team2.TeamMembers.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(TeamMembers_CollectionChanged);
            GameViewModel.Instance.Team1.TeamMembers.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(TeamMembers_CollectionChanged);
            Team1List.ItemsSource = GameViewModel.Instance.Team1.TeamMembers;
            Team2List.ItemsSource = GameViewModel.Instance.Team2.TeamMembers;

            GameViewModel.Instance.PropertyChanged += new PropertyChangedEventHandler(Instance_PropertyChanged);
            GameViewModel.Instance.OnNewGame += new EventHandler(Instance_OnNewGame);

            GameViewModel.Instance.Team1.PropertyChanged += new PropertyChangedEventHandler(Team1_PropertyChanged);
            GameViewModel.Instance.Team2.PropertyChanged += new PropertyChangedEventHandler(Team2_PropertyChanged);
        }

        void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == GameViewModelEnum.DisplayType.ToString())
            {
                ToggleEnabled();
            }

        }

        private void ToggleEnabled()
        {
            if (GameViewModel.Instance.DisplayType == GameViewModelDisplayTypeEnum.SlideShowForIntroductions ||
                GameViewModel.Instance.DisplayType == GameViewModelDisplayTypeEnum.SlideShowForAds)
            {
                EnableText.Text = "Turn Off";
                EnableImage.Source = (ImageSource)FindResource("ImageGreenOn");
            }
            else
            {
                EnableText.Text = "Turn On";
                EnableImage.Source = (ImageSource)FindResource("ImageRedOff");
            }
        }

        void TeamMembers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Team1List.UpdateLayout();
            Team2List.UpdateLayout();
        }

        void Team2_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == TeamViewModelEnum.TeamName.ToString())
            {
                Team2Name.Text = GameViewModel.Instance.Team2.TeamName;
            }
        }

        void Team1_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == TeamViewModelEnum.TeamName.ToString())
            {
                Team1Name.Text = GameViewModel.Instance.Team1.TeamName;
            }
        }



        //private void LoadImages()
        //{
        //    GameViewModel.Instance.AdvertisementSlides = getSlideShowFromDirectory(ScoreboardConfig.SAVE_ADVERT_SLIDESHOW_FOLDER);
        //    GameViewModel.Instance.TeamIntroductionSlides = getSlideShowFromDirectory(ScoreboardConfig.SAVE_INTRODUCTIONS_SLIDESHOW_FOLDER);

        //    // SlideShowImages.ItemsSource = GameViewModel.Instance.AdvertisementSlides.ToArray();
        //}

        void Instance_OnNewGame(object sender, EventArgs e)
        {
            setupView();

        }

        private void SlideShowImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //private static List<SlideShowViewModel> getSlideShowFromDirectory(string directory)
        //{
        //    DirectoryInfo dir = new DirectoryInfo(directory);
        //    if (dir.Exists)
        //    {
        //        var files = dir.GetFiles();
        //        int fileCount = files.Count();
        //        List<SlideShowViewModel> urls = new List<SlideShowViewModel>();

        //        for (int i = 0; i < fileCount; i++)
        //            if (files[i].ToString().Contains(".jpg") || files[i].ToString().Contains(".png"))
        //                urls.Add(new SlideShowViewModel(files[i].ToString().Replace(".jpg", "").Replace(".png", ""), directory + files[i].ToString()));

        //        return urls;
        //    }
        //    return new List<SlideShowViewModel>();
        //}
        /// <summary>
        /// enables the slideshow view in inside the scoreboard layouts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnableSlideShow_Click(object sender, RoutedEventArgs e)
        {
            Logger.Instance.logMessage("enabling slideshow", LoggerEnum.message);
            if (GameViewModel.Instance.DisplayType == GameViewModelDisplayTypeEnum.SlideShowForIntroductions ||
                GameViewModel.Instance.DisplayType == GameViewModelDisplayTypeEnum.SlideShowForAds)
                GameViewModel.Instance.DisplayType = GameViewModelDisplayTypeEnum.Scoreboard;
            else
            {//chooses the display type depending on which tab is selected.
                if (SkaterTab.IsSelected)
                    GameViewModel.Instance.DisplayType = GameViewModelDisplayTypeEnum.SlideShowForIntroductions;
                else if (LargeSlideShowTab.IsSelected)
                    GameViewModel.Instance.DisplayType = GameViewModelDisplayTypeEnum.SlideShowForAds;
                Logger.Instance.logMessage("enabling slideshow Type: " + GameViewModel.Instance.DisplayType.ToString(), LoggerEnum.message);
            }
        }


        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
            GameViewModel.Instance.DisplayType = GameViewModelDisplayTypeEnum.Scoreboard;
        }
        /// <summary>
        /// sets up the scoreboard to show selected player in the scoreboard view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Team1List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (_window == null)
                    _window = (MainWindow)Window.GetWindow(this);

                Logger.Instance.logMessage("select slideshow team 1", LoggerEnum.message);
                if (GameViewModel.Instance.Team1.LineUpSettings == null || GameViewModel.Instance.Team1.LineUpSettings.LineUpTypeSelected == LineUpTypesEnum.NONE)
                {
                    MessageBox.Show(MUST_SELECT_TEMPLATE);
                    return;
                }
                if (e.AddedItems.Count > 0)
                {
                    TeamMembersViewModel member = (TeamMembersViewModel)e.AddedItems[0];
                    if (GameViewModel.Instance.Team1.LineUpSettings.LineUpTypeSelected == LineUpTypesEnum.PlainLineUp)
                        setUpPlainProfilePage(_window, member, GameViewModel.Instance.Team1.LineUpSettings, GameViewModel.Instance.Team1.Logo.SaveLocation);
                    else if (GameViewModel.Instance.Team1.LineUpSettings.LineUpTypeSelected == LineUpTypesEnum.SideBarLineUp)
                        setUpSideBarProfilePage(_window, member, GameViewModel.Instance.Team1.LineUpSettings, GameViewModel.Instance.Team1.Logo.SaveLocation, GameViewModel.Instance.Team1.TeamName);

                }
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("select slideshow team 1 Exception", LoggerEnum.message);
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }

        }
        /// <summary>
        /// sets up to show the player in the scoreboard view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Team2List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (_window == null)
                    _window = (MainWindow)Window.GetWindow(this);

                Logger.Instance.logMessage("select slideshow team 2", LoggerEnum.message);
                if (GameViewModel.Instance.Team2.LineUpSettings == null || GameViewModel.Instance.Team2.LineUpSettings.LineUpTypeSelected == LineUpTypesEnum.NONE)
                {
                    MessageBox.Show(MUST_SELECT_TEMPLATE);
                    return;
                }
                if (e.AddedItems.Count > 0)
                {
                    TeamMembersViewModel member = (TeamMembersViewModel)e.AddedItems[0];
                    if (GameViewModel.Instance.Team2.LineUpSettings.LineUpTypeSelected == LineUpTypesEnum.PlainLineUp)
                        setUpPlainProfilePage(_window, member, GameViewModel.Instance.Team2.LineUpSettings, GameViewModel.Instance.Team2.Logo.SaveLocation);
                    else if (GameViewModel.Instance.Team2.LineUpSettings.LineUpTypeSelected == LineUpTypesEnum.SideBarLineUp)
                        setUpSideBarProfilePage(_window, member, GameViewModel.Instance.Team2.LineUpSettings, GameViewModel.Instance.Team2.Logo.SaveLocation, GameViewModel.Instance.Team2.TeamName);
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("select slideshow team 1 Exception", LoggerEnum.message);
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// sets up the plain profile page for plain profiles.This will show the page on the scoreboard
        /// </summary>
        /// <param name="window"></param>
        /// <param name="member"></param>
        /// <param name="lineUpSettings"></param>
        /// <param name="logoLocation"></param>
        private static void setUpPlainProfilePage(MainWindow window, TeamMembersViewModel member, LineUpViewModel lineUpSettings, string logoLocation)
        {
            PlainProfile profile = new PlainProfile();
            profile.setBackgroundColor(lineUpSettings.PlainBackgroundColor);
            profile.setBorderColor(lineUpSettings.PlainBorderColor);
            profile.setLogo(logoLocation);
            profile.setSkaterName(member.SkaterName);
            profile.setSkaterNumber(member.SkaterNumber);
            profile.setSkaterPicture(member.SkaterPictureLocation);
            profile.setSkaterTextColor(lineUpSettings.PlainTextColor);
            foreach (var scoreboard in window.ClockViewArray)
                scoreboard.ContentControl.Content = profile;
        }
        /// <summary>
        /// sets the introduction slide.This will show the page on the scoreboard
        /// </summary>
        /// <param name="window"></param>
        /// <param name="lineUpSettings"></param>
        /// <param name="logoLocation"></param>
        private static void setUpPlainIntroPage(MainWindow window, LineUpViewModel lineUpSettings, string logoLocation)
        {
            PlainLogo profile = new PlainLogo();
            profile.setBackgroundColor(lineUpSettings.PlainBackgroundColor);
            profile.setBorderColor(lineUpSettings.PlainBorderColor);
            profile.setLogo(logoLocation);
            foreach (var scoreboard in window.ClockViewArray)
                scoreboard.ContentControl.Content = profile;
        }
        /// <summary>
        /// sets up the side bar profile page.This will show the page on the scoreboard
        /// </summary>
        /// <param name="window"></param>
        /// <param name="member"></param>
        /// <param name="lineUpSettings"></param>
        /// <param name="logoLocation"></param>
        /// <param name="teamName"></param>
        private static void setUpSideBarProfilePage(MainWindow window, TeamMembersViewModel member, LineUpViewModel lineUpSettings, string logoLocation, string teamName)
        {
            SideBarProfile profile = new SideBarProfile();
            profile.setBackgroundColor(lineUpSettings.SidebarBackgroundColor);
            profile.setLogo(logoLocation);
            profile.setSkaterName(member.SkaterName);
            profile.setSkaterNumber(member.SkaterNumber);
            profile.setSkaterPicture(member.SkaterPictureLocation);
            profile.setSkaterTextColor(lineUpSettings.SidebarSkaterTextColor);
            profile.setSidebarColor(lineUpSettings.SidebarColor);
            profile.setSideBarPanelText(teamName);
            profile.setSidebarTextColor(lineUpSettings.SidebarTextColor);
            foreach (var scoreboard in window.ClockViewArray)
                scoreboard.ContentControl.Content = profile;

        }
        /// <summary>
        /// sets the sidebar intro page.  This will show the page on the scoreboard
        /// </summary>
        /// <param name="window"></param>
        /// <param name="lineUpSettings"></param>
        /// <param name="logoLocation"></param>
        /// <param name="teamName"></param>
        private static void setUpSideBarIntroPage(MainWindow window, LineUpViewModel lineUpSettings, string logoLocation, string teamName)
        {
            SideBarLogo profile = new SideBarLogo();
            profile.setBackgroundColor(lineUpSettings.SidebarBackgroundColor);
            profile.setLogo(logoLocation);
            profile.setSidebarColor(lineUpSettings.SidebarColor);
            profile.setSideBarPanelText(teamName);
            profile.setSidebarTextColor(lineUpSettings.SidebarTextColor);
            foreach (var scoreboard in window.ClockViewArray)
                scoreboard.ContentControl.Content = profile;
        }

        private void setTeam1Template_Click(object sender, RoutedEventArgs e)
        {
            TemplateChooserView chooser = new TemplateChooserView(GameViewModel.Instance.Team1);
            chooser.Show();
        }

        private void setTeam2Template_Click(object sender, RoutedEventArgs e)
        {
            TemplateChooserView chooser = new TemplateChooserView(GameViewModel.Instance.Team2);
            chooser.Show();
        }

        private void showTeam2Introduction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_window == null)
                    _window = (MainWindow)Window.GetWindow(this);

                Logger.Instance.logMessage("select  team 2 Intros", LoggerEnum.message);
                if (GameViewModel.Instance.Team2 == null || GameViewModel.Instance.Team2.LineUpSettings == null || GameViewModel.Instance.Team2.LineUpSettings.LineUpTypeSelected == LineUpTypesEnum.NONE)
                {
                    MessageBox.Show(MUST_SELECT_TEMPLATE);
                    return;
                }
                if (GameViewModel.Instance.Team2.Logo == null)
                {
                    MessageBox.Show(MUST_SELECT_LOGO);
                    return;
                }

                if (GameViewModel.Instance.Team2.LineUpSettings.LineUpTypeSelected == LineUpTypesEnum.PlainLineUp)
                    setUpPlainIntroPage(_window, GameViewModel.Instance.Team2.LineUpSettings, GameViewModel.Instance.Team2.Logo.SaveLocation);
                else if (GameViewModel.Instance.Team2.LineUpSettings.LineUpTypeSelected == LineUpTypesEnum.SideBarLineUp)
                    setUpSideBarIntroPage(_window, GameViewModel.Instance.Team2.LineUpSettings, GameViewModel.Instance.Team2.Logo.SaveLocation, GameViewModel.Instance.Team2.TeamName);
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("select intro team 2 Exception", LoggerEnum.message);
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void showTeam1Introduction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_window == null)
                    _window = (MainWindow)Window.GetWindow(this);

                Logger.Instance.logMessage("select  team 1 Intros", LoggerEnum.message);
                if (GameViewModel.Instance.Team1 == null || GameViewModel.Instance.Team1.LineUpSettings == null || GameViewModel.Instance.Team1.LineUpSettings.LineUpTypeSelected == LineUpTypesEnum.NONE)
                {
                    MessageBox.Show(MUST_SELECT_TEMPLATE);
                    return;
                }

                if (GameViewModel.Instance.Team1.Logo == null)
                {
                    MessageBox.Show(MUST_SELECT_LOGO);
                    return;
                }

                if (GameViewModel.Instance.Team1.LineUpSettings.LineUpTypeSelected == LineUpTypesEnum.PlainLineUp)
                    setUpPlainIntroPage(_window, GameViewModel.Instance.Team1.LineUpSettings, GameViewModel.Instance.Team1.Logo.SaveLocation);
                else if (GameViewModel.Instance.Team1.LineUpSettings.LineUpTypeSelected == LineUpTypesEnum.SideBarLineUp)
                    setUpSideBarIntroPage(_window, GameViewModel.Instance.Team1.LineUpSettings, GameViewModel.Instance.Team1.Logo.SaveLocation, GameViewModel.Instance.Team1.TeamName);
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("select intro team 1 Exception", LoggerEnum.message);
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            setupView();
        }

        /// <summary>
        /// on tab change, and the button is clicked on, we change the slides showing.
        /// realizing that the user will only keep one right tab open for the current show.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slideShowControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if the button is already on for either the slideshow or the introductions
            //then we can change the display type
            //TODO: maybe we should add a message box the user changes the selection?
            if (GameViewModel.Instance.DisplayType == GameViewModelDisplayTypeEnum.SlideShowForAds ||
                GameViewModel.Instance.DisplayType == GameViewModelDisplayTypeEnum.SlideShowForIntroductions)
            {
                if (SkaterTab.IsSelected)
                    GameViewModel.Instance.DisplayType = GameViewModelDisplayTypeEnum.SlideShowForIntroductions;
                else if (LargeSlideShowTab.IsSelected)
                    GameViewModel.Instance.DisplayType = GameViewModelDisplayTypeEnum.SlideShowForAds;
            }
        }
    }
}
