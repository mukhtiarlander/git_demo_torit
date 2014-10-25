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
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using RDNScoreboard.Code;
using Scoreboard.Library.Network;
using System.Web.Script.Serialization;
using Scoreboard.Library.Models;
using System.Collections.ObjectModel;
using Scoreboard.Library.ViewModel.Members;
using RDN.Portable.Classes.Team;

namespace RDNScoreboard.Views.TeamManagerTabs
{
    /// <summary>
    /// Interaction logic for LogoManagerTab.xaml
    /// </summary>
    public partial class LogoManagerTab : Page
    {
        private ObservableCollection<TeamLogo> _logos;
        public ObservableCollection<TeamLogo> Logos
        {
            get
            {
                if (_logos == null)
                {
                    _logos = new ObservableCollection<TeamLogo>();
                }
                return _logos;
            }
            set { _logos = value; }
        }
        public LogoManagerTab()
        {
            InitializeComponent();
            this.Title = "Logo Manager - " + ScoreboardConfig.SCOREBOARD_NAME;
            GameViewModel.Instance.OnNewGame += new EventHandler(Instance_OnNewGame);
            setupView();
        }



        void Instance_OnNewGame(object sender, EventArgs e)
        {
            setupView();
        }

        public void setupView()
        {
            try
            {
                GameViewModel.Instance.Team1.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Team1_PropertyChanged);
                GameViewModel.Instance.Team2.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Team2_PropertyChanged);

                this.Logo1.UnselectAll();

                this.Logo2.UnselectAll();

                Logos.Clear();

                foreach (var logo in LogoViewModel.Instance.DirectoryLogos)
                {
                    Logos.Add(logo);
                }
                this.Logo1.ItemsSource = Logos;
                this.Logo2.ItemsSource = Logos;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }


        void Team2_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (TeamViewModelEnum.TeamName.ToString() == e.PropertyName)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    this.team2Logo.Text = GameViewModel.Instance.Team2.TeamName + " Logo";
                    return null;
                }, null);
            }
            else if (TeamViewModelEnum.Logo.ToString() == e.PropertyName && GameViewModel.Instance.Team2.Logo == null)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    this.Logo2.UnselectAll();
                    return null;
                }, null);
            }
        }

        void Team1_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (TeamViewModelEnum.TeamName.ToString() == e.PropertyName)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    this.team1Logo.Text = GameViewModel.Instance.Team1.TeamName + " Logo";
                    return null;
                }, null);
            }
            else if (TeamViewModelEnum.Logo.ToString() == e.PropertyName && GameViewModel.Instance.Team1.Logo == null)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    this.Logo1.UnselectAll();
                    return null;
                }, null);
            }
        }

        private void Logo2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                System.Threading.Thread thread = new System.Threading.Thread(
                    new System.Threading.ThreadStart(
                        delegate()
                        {
                            TeamLogo logo = (TeamLogo)e.AddedItems[0];
                            string location = WebUtil.getLogoOfTeam(logo, GameViewModel.Instance.Team2.TeamId);
                            logo.SaveLocation = location;

                            //if it was just uploaded to the scoreboard from a user, we need to 
                            //upload it to the server...
                            if (logo.NewlyUploaded)
                            {
                                GameViewModel.SendLogoForTeam(GameViewModel.Instance.Team2.TeamId, GameViewModel.Instance.Team2.TeamName, logo.SaveLocation);

                                logo.NewlyUploaded = false;
                            }
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object arg)
                            {
                                GameViewModel.Instance.Team2.Logo = logo;
                                return null;
                            }, null);
                        }));
                thread.Start();
            }
        }



        private void Logo1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0)
                {

                    System.Threading.Thread thread = new System.Threading.Thread(
                        new System.Threading.ThreadStart(
                            delegate()
                            {
                                try
                                {
                                    TeamLogo logo = (TeamLogo)e.AddedItems[0];
                                    string location = WebUtil.getLogoOfTeam(logo, GameViewModel.Instance.Team1.TeamId);
                                    logo.SaveLocation = location;
                                    //if it was just uploaded to the scoreboard from a user, we need to 
                                    //upload it to the server...
                                    if (logo.NewlyUploaded)
                                    {
                                        GameViewModel.SendLogoForTeam(GameViewModel.Instance.Team1.TeamId, GameViewModel.Instance.Team1.TeamName, logo.SaveLocation);
                                        logo.NewlyUploaded = false;
                                    }
                                    //once downloaded, we will add it to the team selection.
                                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object arg)
                                    {
                                        try { 
                                        GameViewModel.Instance.Team1.Logo = logo;
                                        }
                                        catch (Exception exception)
                                        {
                                            ErrorViewModel.Save(exception, exception.GetType());
                                        }
                                        return null;
                                    }, null);
                                }
                                catch (Exception exception)
                                {
                                    ErrorViewModel.Save(exception, exception.GetType());
                                }
                            }));
                    thread.Start();
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
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
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }

        private void uploadFileUpload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filename = uploadFileTextBox.Text;

                if (!String.IsNullOrEmpty(filename))
                {

                    string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_LOGOS_FOLDER, Guid.NewGuid().ToString().Replace("-", "") + System.IO.Path.GetExtension(filename));

                    DirectoryInfo dir = new DirectoryInfo(ScoreboardConfig.SAVE_LOGOS_FOLDER);
                    if (!dir.Exists)
                        dir.Create();

                    File.Copy(filename, destinationFilename, true);
                    TeamLogo logo = new TeamLogo();
                    logo.ImageUrl = destinationFilename;
                    logo.SaveLocation = destinationFilename;
                    logo.NewlyUploaded = true;
                    LogoViewModel.Instance.DirectoryLogos.Insert(0, logo);
                    Logos.Insert(0, logo);
                    uploadFileTextBox.Text = "";
                }
                //TODO:upload logo to RDNation if team name is entered..
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }

        private void feedback_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FeedbackView pop = new FeedbackView();
                pop.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                pop.Show();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }
        /// <summary>
        /// opens up the logo manager help link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wikiHelpLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //System.Diagnostics.Process.Start(ScoreboardConfig.SCOREBOARD_LOGO_MANAGER_WIKI_URL);
                WebBrowser b = new WebBrowser();
                b.Navigate(ScoreboardConfig.SCOREBOARD_LOGO_MANAGER_WIKI_URL, "_blank", null, null);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }

        private void SearchTextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                string te = ((TextBox)sender).Text;
                System.Threading.Thread thread = new System.Threading.Thread(
                           new System.Threading.ThreadStart(
                               delegate()
                               {
                                   try
                                   {

                                       if (!String.IsNullOrEmpty(te) && te.Length > 1)
                                       {

                                           var list = (from xx in LogoViewModel.Instance.WebLogos
                                                       where xx.TeamName != null && xx.TeamName.ToLower().Contains(te.ToLower())
                                                       select xx).ToList();

                                           this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object arg)
                                           {
                                               Logo1.ItemsSource = list;
                                               Logo2.ItemsSource = list;
                                               return null;
                                           }, null);
                                       }
                                       else if (te.Length == 0)
                                       {
                                           this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object arg)
                                           {
                                               Logo1.ItemsSource = Logos;
                                               Logo2.ItemsSource = Logos;
                                               return null;
                                           }, null);
                                       }
                                       //once downloaded, we will add it to the team selection.

                                   }
                                   catch (Exception exception)
                                   {
                                       ErrorViewModel.Save(exception, exception.GetType());
                                   }
                               }));
                thread.Start();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }

        }

        private void loadLogos_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                loadLogosWaiting.Visibility = System.Windows.Visibility.Visible;
                System.Threading.Thread thread = new System.Threading.Thread(
                          new System.Threading.ThreadStart(
                              delegate()
                              {
                                  try
                                  {

                                      LogoViewModel.Instance.LoadLogos(true);

                                      this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object arg)
                                      {
                                          foreach (var logo in LogoViewModel.Instance.WebLogos)
                                              Logos.Add(logo);
                                          loadLogosWaiting.Visibility = System.Windows.Visibility.Collapsed;
                                          return null;
                                      }, null);

                                      //once downloaded, we will add it to the team selection.

                                  }
                                  catch (Exception exception)
                                  {
                                      ErrorViewModel.Save(exception, exception.GetType());
                                  }
                              }));
                thread.Start();

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }



    }
}
