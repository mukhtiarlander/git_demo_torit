using System.Windows;
using Scoreboard.Library.ViewModel;
using RDN.Utilities.Config;

namespace RDNScoreboard.Views
{
    /// <summary>
    /// Interaction logic for FeedbackView.xaml
    /// </summary>
    public partial class FeedbackView : Window
    {
        public FeedbackView()
        {
            InitializeComponent();
            this.Title = "Feedback - " + ScoreboardConfig.SCOREBOARD_NAME;
        }

        private void btnPopup_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {//TODO: make this a lot nicer!!!
            //did this for speed.
            thankYouTb.Text = "";

            FeedbackViewModel fb = new FeedbackViewModel(feedbackTb.Text, leagueTb.Text, emailTb.Text);
            FeedbackViewModel.saveFeedback(fb);
            feedbackTb.Text = "";

            thankYouTb.Text = "Thank You For Your Feedback!";
        }

    }
}
