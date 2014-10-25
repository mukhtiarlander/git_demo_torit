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
using RDN.Utilities.Config;
using Scoreboard.Library.Static.Enums;

using Scoreboard.Library.ViewModel;
using RDN.Utilities.Util;

namespace RDNScoreboard.Views
{
    /// <summary>
    /// Interaction logic for OfficialReviewWindow.xaml
    /// </summary>
    public partial class OfficialReviewWindow : Window
    {
        Guid officialReviewId = new Guid();

        public OfficialReviewWindow()
        {
            InitializeComponent();
            this.Title = "Official Review - " + ScoreboardConfig.SCOREBOARD_NAME;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            this.officialReviewId = GameViewModel.Instance.createOfficialReview();

            TeamCbo.ItemsSource = TeamNumberTypes;
        }

        public static List<string> TeamNumberTypes
        {
            get
            {
                List<string> l = new List<string>();
                if (GameViewModel.Instance.Team1 != null)
                    l.Add(GameViewModel.Instance.Team1.TeamName);
                if (GameViewModel.Instance.Team2 != null)
                    l.Add(GameViewModel.Instance.Team2.TeamName);
                return l;
            }
        }

        private void saveOfficialReview_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                TeamNumberEnum team = TeamNumberEnum.None;
                var review = GameViewModel.Instance.OfficialReviews.Where(x => x.OfficialReviewId == this.officialReviewId).FirstOrDefault();
                int selectedItem = TeamCbo.SelectedIndex;

                //home team
                if (selectedItem == 0)
                    team = TeamNumberEnum.Team1;
                else if (selectedItem == 1) //away team
                    team = TeamNumberEnum.Team2;
                //team wasn't chosen.
                if (review != null && team != TeamNumberEnum.None)
                {
                    review.SetTeam(team);
                    review.Result = this.ResultsBox.Text;
                    review.Details = this.DetailsBox.Text;
                    this.Close();
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void cancelOfficialReview_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                var review = GameViewModel.Instance.OfficialReviews.Where(x => x.OfficialReviewId == this.officialReviewId).FirstOrDefault();
                if (review != null)
                {
                    GameViewModel.Instance.OfficialReviews.Remove(review);
                    this.Close();
                }
                this.Close();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }

        }

        private void StackPanel_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void ClockWindow_MouseEnter(object sender, MouseEventArgs e)
        {
        }
        /// <summary>
        /// hides the controsl of the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClockWindow_MouseLeave(object sender, MouseEventArgs e)
        {

        }
    }
}
