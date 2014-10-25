using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.Portable.Classes.Voting;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.WP.Library.Classes.League;
using RDN.Portable.Settings;
using System.Threading.Tasks;

namespace RDN.WP.View.MyLeague.Polls
{

    public partial class Polls : PhoneApplicationPage
    {
        PollBase polls;
        ProgressIndicator progressIndicator;
        public Polls()
        {
            InitializeComponent();

            try
            {
                progressIndicator = SystemTray.ProgressIndicator;
                progressIndicator = new ProgressIndicator();

                SystemTray.SetProgressIndicator(this, progressIndicator);
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Pulling Polls...";

                PullPolls();

            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        void PullPolls()
        {
            progressIndicator.IsVisible = true;

            try
            {
                Task.Run(new Action(() =>
                {
                    polls = PollsMobile.GetPolls(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId);

                    Dispatcher.BeginInvoke(delegate
                    {
                        PollsList.ItemsSource = polls.Polls;
                        if (polls.IsPollManager)
                        {
                            ApplicationBar.IsVisible = true;
                        }
                        progressIndicator.IsVisible = false;
                    });

                }));


            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void PollsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    var topic = (VotingClass)e.AddedItems[0];
                    if (topic.CanVotePoll)
                        NavigationService.Navigate(new Uri("/View/MyLeague/Polls/VotePoll.xaml?pid=" + topic.VotingId, UriKind.Relative));

                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        private void AddPost_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/MyLeague/Polls/CreatePoll.xaml", UriKind.Relative));

        }

        private void ViewPoll_Click(object sender, RoutedEventArgs e)
        {
            var blah = ((MenuItem)sender).DataContext as VotingClass;
            NavigationService.Navigate(new Uri("/View/MyLeague/Polls/ViewPoll.xaml?pid=" + blah.VotingId, UriKind.Relative));
        }

        private void EditPoll_Click(object sender, RoutedEventArgs e)
        {
            var blah = ((MenuItem)sender).DataContext as VotingClass;
            NavigationService.Navigate(new Uri("/View/MyLeague/Polls/EditPoll.xaml?pid=" + blah.VotingId, UriKind.Relative));

        }

        private void VotePoll_Click(object sender, RoutedEventArgs e)
        {
            var blah = ((Button)sender).DataContext as VotingClass;
            if (blah.CanVotePoll)
                NavigationService.Navigate(new Uri("/View/MyLeague/Polls/VotePoll.xaml?pid=" + blah.VotingId, UriKind.Relative));
        }

        private void VotePoll_Click_1(object sender, RoutedEventArgs e)
        {
            var blah = ((MenuItem)sender).DataContext as VotingClass;
            if (!blah.CanVotePoll)
                NavigationService.Navigate(new Uri("/View/MyLeague/Polls/VotePoll.xaml?pid=" + blah.VotingId, UriKind.Relative));


        }
    }
}