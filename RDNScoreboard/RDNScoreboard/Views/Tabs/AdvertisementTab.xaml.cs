using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RDN.Utilities.Config;
using Scoreboard.Library.ViewModel;

using System.IO;
using RDN.Utilities.Error;
using RDN.Utilities.Util;
using Microsoft.VisualBasic.FileIO;

namespace RDNScoreboard.Views.Tabs
{
    /// <summary>
    /// Interaction logic for AdvertisementTab.xaml
    /// </summary>
    public partial class AdvertisementTab : Page
    {
        public AdvertisementTab()
        {
            try
            {
                InitializeComponent();
                this.Title = "Advertisement Manager - " + ScoreboardConfig.SCOREBOARD_NAME;
                if (GameViewModel.Instance.Advertisements == null)
                    GameViewModel.Instance.Advertisements = new List<AdvertisementViewModel>();

                GameViewModel.Instance.OnNewGame += new EventHandler(Instance_OnNewGame);
                AdvertisementViewModel.getAdvertsFromDirectory();

                AdvertList.ItemsSource = GameViewModel.Instance.Advertisements;
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        public void Instance_OnNewGame(object sender, EventArgs e)
        {
            try
            {
                AdvertisementViewModel.getAdvertsFromDirectory();
                AdvertList.ItemsSource = null;
                AdvertList.ItemsSource = GameViewModel.Instance.Advertisements;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        private void AdvertList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    var ad = (AdvertisementViewModel)e.AddedItems[0];
                    var listAd = GameViewModel.Instance.Advertisements.Where(x => x.FileLocation == ad.FileLocation).FirstOrDefault();
                    if (listAd != null)
                    {
                        listAd.IsShowing = !listAd.IsShowing;

                        AdvertList.ItemsSource = null;
                        AdvertList.ItemsSource = GameViewModel.Instance.Advertisements;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
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
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
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
                    Guid advertID = Guid.NewGuid();
                    string destinationFilePath = ScoreboardConfig.SAVE_ADVERTS_FOLDER;
                    string destinationFileName = advertID.ToString().Replace("-", "") + System.IO.Path.GetExtension(filename);

                    DirectoryInfo dir = new DirectoryInfo(ScoreboardConfig.SAVE_ADVERTS_FOLDER);
                    if (!dir.Exists)
                        dir.Create();

                    File.Copy(filename, System.IO.Path.Combine(destinationFilePath, destinationFileName), true);
                    GameViewModel.Instance.Advertisements.Insert(0, new AdvertisementViewModel(advertID, true, destinationFilePath, destinationFileName));
                    AdvertList.ItemsSource = null;
                    AdvertList.ItemsSource = GameViewModel.Instance.Advertisements;

                    uploadFileTextBox.Text = "";
                }
                catch (Exception exception)
                {
                    Logger.Instance.logMessage("tried uploading advertisment to game", LoggerEnum.error);
                    ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
                }
            }
        }
        /// <summary>
        /// deletes teh advert from the folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void advertisementDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you want to delete this advertisement?",
  "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // Close the window
            
            try
            {
                Button cmd = (Button)sender;
                FileInfo file = new FileInfo(((AdvertisementViewModel)cmd.DataContext).FileLocation);

                if (file.Exists)
                {
                    try
                    {                        
                        FileSystem.DeleteFile(file.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    }
                    catch (Exception exception)
                    {
                        Logger.Instance.logMessage("File Delete: " + file.FullName, LoggerEnum.error);
                        ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
                    }
                    var advert = GameViewModel.Instance.Advertisements.Where(x => x.FileLocation == file.FullName).FirstOrDefault();
                    if (advert != null)
                        GameViewModel.Instance.Advertisements.Remove(advert);
                }
                AdvertList.ItemsSource = null;
                AdvertList.ItemsSource = GameViewModel.Instance.Advertisements;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
            }
            
        }

        private void advertismentCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox cmd = (CheckBox)sender;
                var item = ((AdvertisementViewModel)cmd.DataContext);

                var listAd = GameViewModel.Instance.Advertisements.Where(x => x.FileLocation == item.FileLocation).FirstOrDefault();
                if (listAd != null)
                {
                    listAd.IsShowing = true;

                    AdvertList.ItemsSource = null;
                    AdvertList.ItemsSource = GameViewModel.Instance.Advertisements;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void advertismentCheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox cmd = (CheckBox)sender;
                var item = ((AdvertisementViewModel)cmd.DataContext);
                if (item != null)
                {
                    if (GameViewModel.Instance.Advertisements == null)
                        GameViewModel.Instance.Advertisements = new List<AdvertisementViewModel>();

                    var listAd = GameViewModel.Instance.Advertisements.Where(x => x != null && x.FileLocation == item.FileLocation).FirstOrDefault();
                    if (listAd != null)
                    {
                        listAd.IsShowing = false;

                        AdvertList.ItemsSource = null;
                        AdvertList.ItemsSource = GameViewModel.Instance.Advertisements;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
    }
}
