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
using System.Windows.Shapes;
using Scoreboard.Library.ViewModel;
using RDN.Utilities.Config;

namespace RDNScoreboard.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();

            this.Title = "Settings - " + ScoreboardConfig.SCOREBOARD_NAME;

            JamStartKeyBoardShortcut.Text = PolicyViewModel.Instance.StartJamKeyShortcut.ToString();
            JamEndKeyBoardShortcut.Text = PolicyViewModel.Instance.StopJamKeyShortcut.ToString();
            OfficialTimeoutKeyBoardShortcut.Text = PolicyViewModel.Instance.OfficialTimeOutKeyShortcut.ToString();
            Team1ScoreUpKeyBoardShortcut.Text = PolicyViewModel.Instance.Team1ScoreUpKeyShortcut.ToString();
            Team1ScoreDownKeyBoardShortcut.Text = PolicyViewModel.Instance.Team1ScoreDownKeyShortcut.ToString();
            Team1TimeOutKeyBoardShortcut.Text = PolicyViewModel.Instance.Team1TimeOutKeyShortcut.ToString();
            Team1JammerLeadKeyBoardShortcut.Text = PolicyViewModel.Instance.Team1LeadJammerKeyShortcut.ToString();
            Team2ScoreUpKeyBoardShortcut.Text = PolicyViewModel.Instance.Team2ScoreUpKeyShortcut.ToString();
            Team2ScoreDownKeyBoardShortcut.Text = PolicyViewModel.Instance.Team2ScoreDownKeyShortcut.ToString();
            Team2TimeOutKeyBoardShortcut.Text = PolicyViewModel.Instance.Team2TimeOutKeyShortcut.ToString();
            Team2LeadJammerKeyBoardShortcut.Text = PolicyViewModel.Instance.Team2LeadJammerKeyShortcut.ToString();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void JamStartKeyBoardShortcut_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (JamStartKeyBoardShortcut.Text.Length > 0)
                PolicyViewModel.Instance.StartJamKeyShortcut = Char.Parse(JamStartKeyBoardShortcut.Text);
        }

        private void JamEndKeyBoardShortcut_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (JamEndKeyBoardShortcut.Text.Length > 0)
                PolicyViewModel.Instance.StopJamKeyShortcut = Char.Parse(JamEndKeyBoardShortcut.Text);
        }

        private void OfficialTimeoutKeyBoardShortcut_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (OfficialTimeoutKeyBoardShortcut.Text.Length > 0)
                PolicyViewModel.Instance.OfficialTimeOutKeyShortcut = Char.Parse(OfficialTimeoutKeyBoardShortcut.Text);
        }

        private void Team1ScoreUpKeyBoardShortcut_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Team1ScoreUpKeyBoardShortcut.Text.Length > 0)
                PolicyViewModel.Instance.Team1ScoreUpKeyShortcut = Char.Parse(Team1ScoreUpKeyBoardShortcut.Text);
        }

        private void Team1ScoreDownKeyBoardShortcut_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Team1ScoreDownKeyBoardShortcut.Text.Length > 0)
                PolicyViewModel.Instance.Team1ScoreDownKeyShortcut = Char.Parse(Team1ScoreDownKeyBoardShortcut.Text);
        }

        private void Team1TimeOutKeyBoardShortcut_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Team1TimeOutKeyBoardShortcut.Text.Length > 0)
                PolicyViewModel.Instance.Team1TimeOutKeyShortcut = Char.Parse(Team1TimeOutKeyBoardShortcut.Text);
        }

        private void Team1JammerLeadKeyBoardShortcut_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Team1JammerLeadKeyBoardShortcut.Text.Length > 0)
                PolicyViewModel.Instance.Team1LeadJammerKeyShortcut = Char.Parse(Team1JammerLeadKeyBoardShortcut.Text);
        }

        private void Team2ScoreUpKeyBoardShortcut_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Team2ScoreUpKeyBoardShortcut.Text.Length > 0)
                PolicyViewModel.Instance.Team2ScoreUpKeyShortcut = Char.Parse(Team2ScoreUpKeyBoardShortcut.Text);
        }

        private void Team2ScoreDownKeyBoardShortcut_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Team2ScoreDownKeyBoardShortcut.Text.Length > 0)
                PolicyViewModel.Instance.Team2ScoreDownKeyShortcut = Char.Parse(Team2ScoreDownKeyBoardShortcut.Text);
        }

        private void Team2TimeOutKeyBoardShortcut_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Team2TimeOutKeyBoardShortcut.Text.Length > 0)
                PolicyViewModel.Instance.Team2TimeOutKeyShortcut = Char.Parse(Team2TimeOutKeyBoardShortcut.Text);
        }

        private void Team2LeadJammerKeyBoardShortcut_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Team2LeadJammerKeyBoardShortcut.Text.Length > 0)
                PolicyViewModel.Instance.Team2LeadJammerKeyShortcut = Char.Parse(Team2LeadJammerKeyBoardShortcut.Text);
        }
    }
}
