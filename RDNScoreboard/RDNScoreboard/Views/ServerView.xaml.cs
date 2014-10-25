using System.Windows;

using RDN.Utilities.Config;
using System.Text.RegularExpressions;
using RDNScoreboard.Server;
using System.Windows.Documents;
using System.Windows.Controls;
using RDNScoreboard.Views.ServerTabs;
using Scoreboard.Library.ViewModel;
using RDN.Utilities.Util;
using System.Diagnostics;


namespace RDNScoreboard.Views
{
    /// <summary>
    /// Interaction logic for ServerView.xaml
    /// </summary>
    public partial class ServerView : Window
    {
        OverlayTab _overlay;
        public ServerView()
        {
            InitializeComponent();
            this.Title = ScoreboardConfig.SCOREBOARD_NAME;

            lineUpTrackerControlsPoints.IsChecked = PolicyViewModel.Instance.LineUpTrackerControlsScore;

            Run runMainScreen = new Run(WebServer.Instance.IndexForShowingPages);
            this.mainScreenLink.Inlines.Clear();
            this.mainScreenLink.Inlines.Add(runMainScreen);

            Run runControlScreen = new Run(WebServer.Instance.ControlScreen);
            this.controlScreenLink.Inlines.Clear();
            this.controlScreenLink.Inlines.Add(runControlScreen);



            Run runAssistTeam1 = new Run(WebServer.Instance.AssistTeam1);
            this.assistsTeam1Link.Inlines.Clear();
            this.assistsTeam1Link.Inlines.Add(runAssistTeam1);
            Run runAssistTeam2 = new Run(WebServer.Instance.AssistTeam2);
            this.assistsTeam2Link.Inlines.Clear();
            this.assistsTeam2Link.Inlines.Add(runAssistTeam2);

            Run runBlockTeam1 = new Run(WebServer.Instance.BlockTeam1);
            this.blocksTeam1Link.Inlines.Clear();
            this.blocksTeam1Link.Inlines.Add(runBlockTeam1);
            Run runBlockTeam2 = new Run(WebServer.Instance.BlockTeam2);
            this.blocksTeam2Link.Inlines.Clear();
            this.blocksTeam2Link.Inlines.Add(runBlockTeam2);

            Run runBlockAssistTeam1 = new Run(WebServer.Instance.AssistBlockTeam1);
            this.assistBlocksTeam1Link.Inlines.Clear();
            this.assistBlocksTeam1Link.Inlines.Add(runBlockAssistTeam1);
            Run runBlockAssistTeam2 = new Run(WebServer.Instance.AssistBlockTeam2);
            this.assistsBlocksTeam2Link.Inlines.Clear();
            this.assistsBlocksTeam2Link.Inlines.Add(runBlockAssistTeam2);

            Run runPenaltyTeam1 = new Run(WebServer.Instance.PenaltyTeam1);
            this.penatlyTeam1Link.Inlines.Clear();
            this.penatlyTeam1Link.Inlines.Add(runPenaltyTeam1);
            Run runPenaltyTeam2 = new Run(WebServer.Instance.PenaltyTeam2);
            this.penaltyTeam2Link.Inlines.Clear();
            this.penaltyTeam2Link.Inlines.Add(runPenaltyTeam2);
            Run runPenaltyTeam1and2 = new Run(WebServer.Instance.PenaltyTeam1and2);
            this.penaltyTeam1and2Link.Inlines.Clear();
            this.penaltyTeam1and2Link.Inlines.Add(runPenaltyTeam1and2);

            Run runLineUpTeam1 = new Run(WebServer.Instance.LineUpTeam1);
            this.lineupTeam1Link.Inlines.Clear();
            this.lineupTeam1Link.Inlines.Add(runLineUpTeam1);
            Run runLineUpTeam2 = new Run(WebServer.Instance.LineUpTeam2);
            this.lineUpsTeam2Link.Inlines.Clear();
            this.lineUpsTeam2Link.Inlines.Add(runLineUpTeam2);

            Run runAnnouncers = new Run(WebServer.Instance.AnnouncersScreen);
            this.announcersLink.Inlines.Clear();
            this.announcersLink.Inlines.Add(runAnnouncers);

            Run runScoresTeam1 = new Run(WebServer.Instance.ScoresTeam1);
            this.scoresTeam1Link.Inlines.Clear();
            this.scoresTeam1Link.Inlines.Add(runScoresTeam1);
            Run runScoresTeam2 = new Run(WebServer.Instance.ScoresTeam2);
            this.scoresTeam2Link.Inlines.Clear();
            this.scoresTeam2Link.Inlines.Add(runScoresTeam2);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }


        private void assistsTeam1Link_Click(object sender, RoutedEventArgs e)
        {
            WebBrowser b = new WebBrowser();
            b.Navigate(WebServer.Instance.AssistTeam1, "_blank", null, null);
        }

        private void assistsTeam2Link_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(WebServer.Instance.AssistTeam2);
            WebBrowser b = new WebBrowser();
            b.Navigate(WebServer.Instance.AssistTeam2, "_blank", null, null);
        }

        private void blocksTeam1Link_Click(object sender, RoutedEventArgs e)
        {
            WebBrowser b = new WebBrowser();
            b.Navigate(WebServer.Instance.BlockTeam1, "_blank", null, null);
        }

        private void blocksTeam2Link_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(WebServer.Instance.BlockTeam2);
            WebBrowser b = new WebBrowser();
            b.Navigate(WebServer.Instance.BlockTeam2, "_blank", null, null);
        }

        private void penaltyTeam2Link_Click(object sender, RoutedEventArgs e)
        {
            WebBrowser b = new WebBrowser();
            b.Navigate(WebServer.Instance.PenaltyTeam2, "_blank", null, null);
        }

        private void penatlyTeam1Link_Click(object sender, RoutedEventArgs e)
        {
            WebBrowser b = new WebBrowser();
            b.Navigate(WebServer.Instance.PenaltyTeam1, "_blank", null, null);
        }

        private void lineupTeam1Link_Click(object sender, RoutedEventArgs e)
        {
            WebBrowser b = new WebBrowser();
            b.Navigate(WebServer.Instance.LineUpTeam1, "_blank", null, null);
        }

        private void lineUpsTeam2Link_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(WebServer.Instance.LineUpTeam2);
            WebBrowser b = new WebBrowser();
            b.Navigate(WebServer.Instance.LineUpTeam2, "_blank", null, null);
        }

        private void mainScreenLink_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(WebServer.Instance.IndexForShowingPages);
            WebBrowser b = new WebBrowser();
            b.Navigate(WebServer.Instance.IndexForShowingPages, "_blank", null, null);
        }

        private void assistBlocksTeam1Link_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(WebServer.Instance.AssistBlockTeam1);
            WebBrowser b = new WebBrowser();
            b.Navigate(WebServer.Instance.AssistBlockTeam1, "_blank", null, null);
        }

        private void assistsBlocksTeam2Link_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(WebServer.Instance.AssistBlockTeam2);
            WebBrowser b = new WebBrowser();
            b.Navigate(WebServer.Instance.AssistBlockTeam2, "_blank", null, null);
        }

        private void penaltyTeam1and2Link_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(WebServer.Instance.PenaltyTeam1and2);
            WebBrowser b = new WebBrowser();
            b.Navigate(WebServer.Instance.PenaltyTeam1and2, "_blank", null, null);
        }

        private void wikiHelpLink_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(ScoreboardConfig.SCOREBOARD_STATS_COLLECTION_WIKI_URL);
            WebBrowser b = new WebBrowser();
            b.Navigate(ScoreboardConfig.SCOREBOARD_STATS_COLLECTION_WIKI_URL, "_blank", null, null);
        }

        private void controlScreenLink_Click_1(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(WebServer.Instance.ControlScreen);
            WebBrowser b = new WebBrowser();
            b.Navigate(WebServer.Instance.ControlScreen, "_blank", null, null);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (ServerTab.IsSelected)
                {
                    _overlay = null;
                    System.GC.Collect();
                }
                if (VideoOverlay.IsSelected)
                {
                    _overlay = new OverlayTab();
                    VideoOverlayFrame.Content = _overlay;
                }
            }
        }

        private void announcersLink_Click_1(object sender, RoutedEventArgs e)
        {
                      WebBrowser b = new WebBrowser();
                    b.Navigate(WebServer.Instance.AnnouncersScreen, "_blank", null, null);
        }

        private void scoresTeam1Link_Click_1(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(WebServer.Instance.ScoresTeam1);
            WebBrowser b = new WebBrowser();
            b.Navigate(WebServer.Instance.AnnouncersScreen, "_blank", null, null);
        }

        private void scoresTeam2Link_Click_1(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(WebServer.Instance.ScoresTeam2);
            WebBrowser b = new WebBrowser();
            b.Navigate(WebServer.Instance.AnnouncersScreen, "_blank", null, null);

        }

        private void lineUpTrackerHelp_Click_1(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(ScoreboardConfig.SCOREBOARD_STATS_COLLECTION_SERVER_SETTINGS_WIKI_URL);
            WebBrowser b = new WebBrowser();
            b.Navigate(WebServer.Instance.AnnouncersScreen, "_blank", null, null);
        }


        private void lineUpTrackerControlsPoints_Click_1(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.LineUpTrackerControlsScore = !PolicyViewModel.Instance.LineUpTrackerControlsScore;
            Logger.Instance.logMessage("setting line up tracker control", LoggerEnum.message);
            PolicyViewModel.Instance.savePolicyToXml();
        }
    }

}
