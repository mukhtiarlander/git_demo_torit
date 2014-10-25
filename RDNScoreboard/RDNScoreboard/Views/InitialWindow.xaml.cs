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
    /// Interaction logic for InitialWindow.xaml
    /// </summary>
    public partial class InitialWindow : Window
    {
        public InitialWindow()
        {
            InitializeComponent();
            this.Title = "What To Do Today? - " + ScoreboardConfig.SCOREBOARD_NAME;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.Topmost = true;
        }


        /// <summary>
        /// user will allow live game on the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LiveGameButton_Checked(object sender, RoutedEventArgs e)
        {
            GameViewModel.Instance.SaveGameOnline = ((RadioButton)sender).IsChecked.GetValueOrDefault();
            GameViewModel.Instance.PublishGameOnline = true;
            GameViewModel.Instance.IsBeingTested = false;
            this.Close();
        }
        /// <summary>
        /// user will save stats to the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrimmageButton_Checked(object sender, RoutedEventArgs e)
        {
            GameViewModel.Instance.SaveGameOnline = ((RadioButton)sender).IsChecked.GetValueOrDefault();
            GameViewModel.Instance.PublishGameOnline = false;
            GameViewModel.Instance.IsBeingTested = false;
            this.Close();
        }
        /// <summary>
        /// user is just testing the scoreboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestingButton_Checked(object sender, RoutedEventArgs e)
        {
            GameViewModel.Instance.IsBeingTested = ((RadioButton)sender).IsChecked.GetValueOrDefault();
            GameViewModel.Instance.PublishGameOnline = false;
            GameViewModel.Instance.SaveGameOnline = false;
            this.Close();
        }
    }
}
