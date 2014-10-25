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
using Scoreboard.Library.Network;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using RDN.Utilities.Config;
using System.Windows.Threading;
using RDNScoreboard.Code;
using RDN.Utilities.Util;


namespace RDNScoreboard.Views.Tabs
{
    /// <summary>
    /// Interaction logic for LargeSlideshowFrame.xaml
    /// </summary>
    public partial class LargeSlideshowFrame : Page
    {
        Regex _checkNumber = new Regex(@"[\d]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public LargeSlideshowFrame()
        {
            InitializeComponent();
            if (GameViewModel.Instance.SlideShowSlides == null)
                GameViewModel.Instance.SlideShowSlides = new List<SlideShowViewModel>();

            GameViewModel.Instance.OnNewGame += new EventHandler(Instance_OnNewGame);
            SlideShowViewModel.getSlidesFromDirectory();

            SlideList.ItemsSource = GameViewModel.Instance.SlideShowSlides;

            SecondsPerSlide.Text = PolicyViewModel.Instance.SecondsPerSlideShowSlide.ToString();
            ShowActiveClock.IsChecked = PolicyViewModel.Instance.ShowActiveClockDuringSlideShow;
            enableRotationOfSlides.IsChecked = PolicyViewModel.Instance.RotateSlideShowSlides;

        }

        void Instance_OnNewGame(object sender, EventArgs e)
        {
            SlideShowViewModel.getSlidesFromDirectory();
            SlideList.ItemsSource = null;
            SlideList.ItemsSource = GameViewModel.Instance.SlideShowSlides;
            GameViewModel.Instance.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(InstanceGame_PropertyChanged);
            GameViewModel.Instance.OnNewGame += new EventHandler(Instance_OnNewGame);
        }

        void InstanceGame_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == GameViewModelEnum.CurrentSlideShowSlide.ToString())
            {
                if (GameViewModel.Instance.CurrentSlideShowSlide != null)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                      (DispatcherOperationCallback)delegate(object arg)
                      {
                          SlideList.SelectedItem = GameViewModel.Instance.CurrentSlideShowSlide;
                          return null;
                      }, null);
                }
            }
        }


        private void uploadFileBrowse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Images (*.JPG;*.JPEG;*.PNG;*.GIF)|*.JPG;*.JPEG;*.PNG;*.GIF";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                uploadFileTextBox.Text = filename;
            }
        }

        private void uploadFileUpload_Click(object sender, RoutedEventArgs e)
        {
            string filename = uploadFileTextBox.Text;
            if (!String.IsNullOrEmpty(filename))
            {
                try
                {
                    Logger.Instance.logMessage("uploading file: " + filename, LoggerEnum.message);
                    Guid slideID = Guid.NewGuid();
                    string destinationFilePath = ScoreboardConfig.SAVE_SLIDESHOW_FOLDER;
                    string destinationFileName = slideID.ToString().Replace("-", "") + System.IO.Path.GetExtension(filename);

                    DirectoryInfo dir = new DirectoryInfo(ScoreboardConfig.SAVE_SLIDESHOW_FOLDER);
                    if (!dir.Exists)
                        dir.Create();

                    File.Copy(filename, System.IO.Path.Combine(destinationFilePath, destinationFileName), true);
                    GameViewModel.Instance.SlideShowSlides.Insert(0, new SlideShowViewModel(true, destinationFilePath, destinationFileName));
                    SlideList.ItemsSource = null;
                    SlideList.ItemsSource = GameViewModel.Instance.SlideShowSlides;

                    uploadFileTextBox.Text = "";
                }
                catch (Exception exception)
                {
                    Logger.Instance.logMessage("tried uploading slide to game", LoggerEnum.error);
                    ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
                }
            }
        }

        private void feedback_Click(object sender, RoutedEventArgs e)
        {
            FeedbackView pop = new FeedbackView();
            pop.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            pop.Show();
        }
        /// <summary>
        /// opens up the logo manager help link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wikiHelpLink_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(ScoreboardConfig.SCOREBOARD_SLIDESHOW_MANAGER_WIKI_URL);
            WebBrowser b = new WebBrowser();
            b.Navigate(ScoreboardConfig.SCOREBOARD_SLIDESHOW_MANAGER_WIKI_URL, "_blank", null, null);
        }
        
        private void SlideList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                GameViewModel.Instance.CurrentSlideShowSlide = (SlideShowViewModel)e.AddedItems[0];
            }
        }

        private void SlideCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //CheckBox cmd = (CheckBox)sender;
            //var item = ((SlideShowViewModel)cmd.DataContext);

            //var listAd = GameViewModel.Instance.SlideShowSlides.Where(x => x.FileLocation == item.FileLocation).FirstOrDefault();
            //if (listAd != null)
            //{
            //    //listAd.IsShowing = !listAd.IsShowing;

            //    //SlideList.ItemsSource = null;
            //    //SlideList.ItemsSource = GameViewModel.Instance.SlideShowSlides;
            //}
        }

        private void slideDelete_Click(object sender, RoutedEventArgs e)
        {
            Button cmd = (Button)sender;
            FileInfo file = new FileInfo(((SlideShowViewModel)cmd.DataContext).FileLocation);

            if (file.Exists)
            {
                file.Delete();
                var advert = GameViewModel.Instance.SlideShowSlides.Where(x => x.FileLocation == file.FullName).FirstOrDefault();
                if (advert != null)
                    GameViewModel.Instance.SlideShowSlides.Remove(advert);
            }
            SlideList.ItemsSource = null;
            SlideList.ItemsSource = GameViewModel.Instance.SlideShowSlides;
        }

        private void SecondsPerSlide_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = ((TextBox)sender).Text;
            //text can't be zero because of our timers...
            if (_checkNumber.IsMatch(text) && Convert.ToInt32(text) > 0)
            {
                PolicyViewModel.Instance.SecondsPerSlideShowSlide = Convert.ToInt32(_checkNumber.Match(text).Value);
                Logger.Instance.logMessage("setting seconds per slideshow slide.", LoggerEnum.message);
                PolicyViewModel.Instance.savePolicyToXml();
                GameViewModel.Instance.setupTimerForSlideShow();
            }

        }

        private void ShowActiveClock_Checked(object sender, RoutedEventArgs e)
        {
            bool isChecked = ((CheckBox)sender).IsChecked.Value;
            PolicyViewModel.Instance.ShowActiveClockDuringSlideShow = isChecked;
            Logger.Instance.logMessage("setting show actice clock during slideshow", LoggerEnum.message);
            PolicyViewModel.Instance.savePolicyToXml();
        }

        private void enableRotationOfSlides_Checked(object sender, RoutedEventArgs e)
        {
            bool isChecked = ((CheckBox)sender).IsChecked.Value;
            if (PolicyViewModel.Instance.RotateSlideShowSlides != isChecked)
            {
                PolicyViewModel.Instance.RotateSlideShowSlides = isChecked;
                Logger.Instance.logMessage("setting rotate slide show slides", LoggerEnum.message);
                PolicyViewModel.Instance.savePolicyToXml();
                if (isChecked)
                    GameViewModel.Instance.setupTimerForSlideShow();
            }
        }

        private void SlideCheckBox_Click(object sender, RoutedEventArgs e)
        {

        }


        private void SelectAll_Checked_1(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < GameViewModel.Instance.SlideShowSlides.Count; i++)
                GameViewModel.Instance.SlideShowSlides[i].IsShowing = true;

            SlideList.ItemsSource = null;
            SlideList.ItemsSource = GameViewModel.Instance.SlideShowSlides;
        }

        private void SelectAll_Unchecked_1(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < GameViewModel.Instance.SlideShowSlides.Count; i++)
                GameViewModel.Instance.SlideShowSlides[i].IsShowing = false;

            SlideList.ItemsSource = null;
            SlideList.ItemsSource = GameViewModel.Instance.SlideShowSlides;
        }

    }
}
