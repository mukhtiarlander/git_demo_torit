using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
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
using Microsoft.Win32;
using RDN.Utilities.Config;
using RDN.Utilities.Error;
using Scoreboard.Library.Network;
using Scoreboard.Library.Static.Enums;

using Scoreboard.Library.ViewModel;
using Scoreboard.Library.ViewModel.Members.Enums;
using Scoreboard.Library.ViewModel.Members.Officials;
using Scoreboard.Library.ViewModel.Officials.Enums;
using RDN.Utilities.Util;

namespace RDNScoreboard.Views.TeamManagerTabs
{
    /// <summary>
    /// Interaction logic for OfficialsManagerTab.xaml
    /// </summary>
    public partial class OfficialsManagerTab : Page
    {
        public OfficialsManagerTab()
        {
            InitializeComponent();
        }

        public void SetupView()
        {
            try
            {
                skaterPositionRef.ItemsSource = OfficialsHelper.RefereeEnumTypes;
                skaterPositionNso.ItemsSource = OfficialsHelper.NsoEnumTypes;
                skaterCertNso.ItemsSource = OfficialsHelper.CertificationEnumTypes;
                skaterCertRef.ItemsSource = OfficialsHelper.CertificationEnumTypes;
                if (GameViewModel.Instance.Officials != null)
                {
                    if (GameViewModel.Instance.Officials.Referees == null)
                        GameViewModel.Instance.Officials.Referees = new System.Collections.ObjectModel.ObservableCollection<RefereeMember>();
                    RefList.DataContext = GameViewModel.Instance.Officials.Referees;
                    if (GameViewModel.Instance.Officials.Nsos == null)
                        GameViewModel.Instance.Officials.Nsos = new System.Collections.ObjectModel.ObservableCollection<NSOMember>();
                    NsoList.DataContext = GameViewModel.Instance.Officials.Nsos;
                }

                GameViewModel.Instance.Officials.Referees.CollectionChanged += new NotifyCollectionChangedEventHandler(Referees_CollectionChanged);
                GameViewModel.Instance.Officials.Nsos.CollectionChanged += new NotifyCollectionChangedEventHandler(Nsos_CollectionChanged);


                GameViewModel.Instance.OnNewGame += Instance_OnNewGame;
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        void Nsos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                NsoList.UpdateLayout();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        void Referees_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                RefList.UpdateLayout();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        void Instance_OnNewGame(object sender, EventArgs e)
        {
            SetupView();
        }

        private void wikiHelpLink_Click_1(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(ScoreboardConfig.REFEREE_MANAGER_WIKI_URL);
            WebBrowser b = new WebBrowser();
            b.Navigate(ScoreboardConfig.REFEREE_MANAGER_WIKI_URL, "_blank", null, null);
        }


        private void submitSkaterRefs_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                RefereeTypeEnum refType = RefereeTypeEnum.Not_Selected;
                CertificationLevelEnum certType = CertificationLevelEnum.None;
                if (skaterPositionRef != null && skaterPositionRef.SelectedItem != null)
                    refType = (RefereeTypeEnum)Enum.Parse(typeof(RefereeTypeEnum), skaterPositionRef.SelectedItem.ToString().Replace(" ", "_"));
                if (skaterCertRef != null && skaterCertRef.SelectedItem != null)
                    certType = (CertificationLevelEnum)Enum.Parse(typeof(CertificationLevelEnum), skaterCertRef.SelectedItem.ToString().Replace(" ", "_"));

                GameViewModel.Instance.Officials.AddOfficial(skaterNameRefs.Text, refType, skaterLeagueRef.Text, certType);
                skaterNameRefs.Text = "";
                skaterLeagueRef.Text = "";

                skaterNameRefs.Focus();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void deleteRef_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                RefereeMember obj = ((FrameworkElement)sender).DataContext as RefereeMember;
                GameViewModel.Instance.Officials.RemoveSkaterFromRefs(obj);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }

        }

        private void setPictureRef_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = ((Button)sender);
                RefereeMember obj = ((FrameworkElement)sender).DataContext as RefereeMember;

                obj.SkaterPictureLocation = TeamManager.UploadSkaterPicture(obj.SkaterId, MemberTypeEnum.Referee);
            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried loading skater picture to game for team 1: " + exception.Message, LoggerEnum.error);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void deleteNso_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                NSOMember obj = ((FrameworkElement)sender).DataContext as NSOMember;
                GameViewModel.Instance.Officials.RemoveSkaterFromNsos(obj);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void setPictureNso_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = ((Button)sender);
                NSOMember obj = ((FrameworkElement)sender).DataContext as NSOMember;

                obj.SkaterPictureLocation = TeamManager.UploadSkaterPicture(obj.SkaterId, MemberTypeEnum.Referee);

            }
            catch (Exception exception)
            {
                Logger.Instance.logMessage("tried loading skater picture to game for team 1: " + exception.Message, LoggerEnum.error);
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetupView();
        }

        private void submitNso_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                NSOTypeEnum refType = NSOTypeEnum.NSO;
                CertificationLevelEnum certType = CertificationLevelEnum.None;
                if (skaterPositionNso != null && skaterPositionNso.SelectedItem != null)
                    refType = (NSOTypeEnum)Enum.Parse(typeof(NSOTypeEnum), skaterPositionNso.SelectedItem.ToString().Replace(" ", "_"));
                if (skaterCertNso != null && skaterCertNso.SelectedItem != null)
                    certType = (CertificationLevelEnum)Enum.Parse(typeof(CertificationLevelEnum), skaterCertNso.SelectedItem.ToString().Replace(" ", "_"));

                GameViewModel.Instance.Officials.AddOfficial(skaterNameNso.Text, refType, skaterLeagueNso.Text, certType);
                skaterNameNso.Text = "";
                skaterLeagueNso.Text = "";
                skaterNameNso.Focus();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }
        private void loadRefs_Click_1(object sender, RoutedEventArgs e)
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

                Logger.Instance.logMessage("Loading Refs", LoggerEnum.message);
                GameViewModel.Instance.loadOfficialsRosterFromXml(dlg.FileName, TeamNumberEnum.Refs);
                SetupView();

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void saveRefs_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Instance.logMessage("Saving the Ref Rosters", LoggerEnum.message);
                if (!Directory.Exists(ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION))
                    Directory.CreateDirectory(ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION);
                var dlg = new SaveFileDialog();
                dlg.FileName = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly("Ref-Roster-" + DateTime.Now.ToString("yyyy-MMMM-dd"));
                dlg.Filter = "Xml Documents (*.xml)|*.xml";
                dlg.OverwritePrompt = true;
                dlg.RestoreDirectory = true;
                dlg.InitialDirectory = ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION;

                bool? result = dlg.ShowDialog();

                if (result == true)
                    GameViewModel.Instance.saveOfficialsAndPicturesToXml(dlg.FileName);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void loadNSOs_Click_1(object sender, RoutedEventArgs e)
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

                Logger.Instance.logMessage("Loading NSOs", LoggerEnum.message);
                GameViewModel.Instance.loadOfficialsRosterFromXml(dlg.FileName, TeamNumberEnum.NSOs);
                SetupView();

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void saveNSOs_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    Logger.Instance.logMessage("Saving the NSO Rosters", LoggerEnum.message);
                    if (!Directory.Exists(ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION))
                        Directory.CreateDirectory(ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION);
                    var dlg = new SaveFileDialog();
                    dlg.FileName = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly("NSO-Roster-" + DateTime.Now.ToString("yyyy-MMMM-dd"));
                    dlg.Filter = "Xml Documents (*.xml)|*.xml";
                    dlg.OverwritePrompt = true;
                    dlg.RestoreDirectory = true;
                    dlg.InitialDirectory = ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION;

                    bool? result = dlg.ShowDialog();

                    if (result == true)
                        GameViewModel.Instance.saveOfficialsAndPicturesToXml(dlg.FileName);
                }
                catch (Exception exception)
                {
                    ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }
        private void loadAllRefs_Click_1(object sender, RoutedEventArgs e)
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

                Logger.Instance.logMessage("Loading NSOs", LoggerEnum.message);
                GameViewModel.Instance.loadOfficialsRosterFromXml(dlg.FileName, TeamNumberEnum.AllRefsNSOs);
                SetupView();

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private void saveAllRefs_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    Logger.Instance.logMessage("Saving the NSO Rosters", LoggerEnum.message);
                    if (!Directory.Exists(ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION))
                        Directory.CreateDirectory(ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION);
                    var dlg = new SaveFileDialog();
                    dlg.FileName = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly("Ref-NSO-Roster-" + DateTime.Now.ToString("yyyy-MMMM-dd"));
                    dlg.Filter = "Xml Documents (*.xml)|*.xml";
                    dlg.OverwritePrompt = true;
                    dlg.RestoreDirectory = true;
                    dlg.InitialDirectory = ScoreboardConfig.SAVE_ROSTERS_FILE_LOCATION;

                    bool? result = dlg.ShowDialog();

                    if (result == true)
                        GameViewModel.Instance.saveOfficialsAndPicturesToXml(dlg.FileName);
                }
                catch (Exception exception)
                {
                    ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

    }
}
