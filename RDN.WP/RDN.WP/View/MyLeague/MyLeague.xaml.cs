using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.Portable.Account;
using RDN.WP.Library.Account;
using RDN.Portable.Settings;
using System.Threading.Tasks;
using RDN.WP.Library.Classes.League;
using RDN.Portable.Classes.League.Classes;
using RDN.Portable.Classes.Controls.Calendar;
using RDN.Portable.Classes.Voting;
using RDN.Portable.Classes.Controls.Forum;
using Microsoft.Phone.Tasks;
using RDN.Portable.Config;
using RDN.WP.Library.Database;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.IO;

namespace RDN.WP.View.MyLeague
{
    public partial class MyLeague : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        private static string BillingAddress = "";
        LeagueStartModel startModel;
        public MyLeague()
        {
            InitializeComponent();
            try
            {
                progressIndicator = SystemTray.ProgressIndicator;
                progressIndicator = new ProgressIndicator();
                SystemTray.SetProgressIndicator(this, progressIndicator);
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Loading...";
                PullSettings();
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        void PullSettings()
        {
            Dispatcher.BeginInvoke(delegate
            {
                progressIndicator.IsVisible = true;
            });
            try
            {
                Task.Run(new Action(() =>
                {
                    SettingsMobile.Instance.AccountSettings = UserMobileWP.AccountSettings(new UserMobile() { MemberId = SettingsMobile.Instance.User.MemberId, LoginId = SettingsMobile.Instance.User.LoginId });
                    if (SettingsMobile.Instance.AccountSettings.IsApartOfLeague)
                        PullInitialLeague();
                                        
                    //SettingsMobile.Instance.AccountSettings.LeagueLogo = "https://rdnation.com/Content/Rollerball_pink_s100.png";
                    if (!String.IsNullOrEmpty((SettingsMobile.Instance.AccountSettings.LeagueLogo)))
                    {
                        Dispatcher.BeginInvoke(delegate
                        {
                            BitmapImage image = new BitmapImage(new Uri(SettingsMobile.Instance.AccountSettings.LeagueLogo, UriKind.RelativeOrAbsolute));
                            panoImage.ImageSource = image;
                        });
                    }

                    Dispatcher.BeginInvoke(delegate
                    {
                        if (SettingsMobile.Instance.AccountSettings.IsApartOfLeague)
                        {
                            if (SettingsMobile.Instance.AccountSettings.IsManagerOrBetter)
                                EditLeague.Visibility = System.Windows.Visibility.Visible;

                            CalendarButton.Visibility = System.Windows.Visibility.Visible;
                            ContactsButton.Visibility = System.Windows.Visibility.Visible;
                            DocsButton.Visibility = System.Windows.Visibility.Visible;
                            DuesButton.Visibility = System.Windows.Visibility.Visible;
                            PollsButton.Visibility = System.Windows.Visibility.Visible;
                            JobsButton.Visibility = System.Windows.Visibility.Visible;
                            SponsorsButton.Visibility = System.Windows.Visibility.Visible;
                            ForumButton.Visibility = System.Windows.Visibility.Visible;
                            MembersButton.Visibility = System.Windows.Visibility.Visible;
                            InventoryButton.Visibility = System.Windows.Visibility.Visible;
                            SubscribeButton.Visibility = System.Windows.Visibility.Visible;
                            ShopButton.Visibility = System.Windows.Visibility.Visible;

                        }
                        else
                        {
                            SetupButton.Visibility = System.Windows.Visibility.Visible;
                            JoinButton.Visibility = System.Windows.Visibility.Visible;
                            progressIndicator.IsVisible = false;
                        }
                        if (SettingsMobile.Instance.AccountSettings.UnreadMessagesCount > 0)
                            MessagesTextBox.Text += " (" + SettingsMobile.Instance.AccountSettings.UnreadMessagesCount + ")";
                        if (SettingsMobile.Instance.AccountSettings.ChallengeCount > 0)
                            ChallengesTextBox.Text += " (" + SettingsMobile.Instance.AccountSettings.ChallengeCount + ")";
                        if (SettingsMobile.Instance.AccountSettings.OfficialsRequestCount > 0)
                            OfficialsRequests.Text += " (" + SettingsMobile.Instance.AccountSettings.OfficialsRequestCount + ")";
                    });
                }));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        void PullInitialLeague()
        {
            Dispatcher.BeginInvoke(delegate
            {
                progressIndicator.IsVisible = true;
            });
            try
            {
                Task.Run(new Action(() =>
                {
                    startModel = LeagueMobile.GetMyLeague(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId);
                    Dispatcher.BeginInvoke(delegate
                    {
                        if (SettingsMobile.Instance.AccountSettings.IsSubscriptionActive)
                        {
                            if (startModel.Calendar != null && startModel.Calendar.Events.Count > 0)
                            {
                                EventListText.Visibility = System.Windows.Visibility.Visible;
                                EventList.ItemsSource = startModel.Calendar.Events;
                            }
                            else
                            {
                                EventListText.Visibility = System.Windows.Visibility.Collapsed;
                                EventList.Visibility = System.Windows.Visibility.Collapsed;
                            }
                        }
                        if (startModel.ForumTopics.Count > 0)
                        {
                            UnreadForumTopics.Visibility = System.Windows.Visibility.Visible;
                            TopicsList.ItemsSource = startModel.ForumTopics;
                        }
                        else
                        {
                            UnreadForumTopics.Visibility = System.Windows.Visibility.Collapsed;
                            TopicsList.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        if (SettingsMobile.Instance.AccountSettings.IsSubscriptionActive)
                        {
                            if (startModel.Polls != null && startModel.Polls.Polls.Count > 0)
                            {
                                VotesNeeded.Visibility = System.Windows.Visibility.Visible;
                                PollsList.ItemsSource = startModel.Polls.Polls;
                            }
                            else
                            {
                                VotesNeeded.Visibility = System.Windows.Visibility.Collapsed;
                                PollsList.Visibility = System.Windows.Visibility.Collapsed;
                            }
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

        private void ForumButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new Uri("/View/MyLeague/ForumMain.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void MembersButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new Uri("/View/MyLeague/Members/Roster.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void MessagesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new Uri("/View/MyLeague/Messages/MessagesHome.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void EditLeague_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new Uri("/View/MyLeague/EditLeague.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void MyProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                NavigationService.Navigate(new Uri("/View/MyProfile/MyProfile.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void PollsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SettingsMobile.Instance.AccountSettings.IsSubscriptionActive)
                {
                    NavigationService.Navigate(new Uri("/View/MyLeague/Polls/Polls.xaml", UriKind.Relative));
                }
                else
                {
                    WebBrowserTask wbt = new WebBrowserTask();
                    wbt.Uri = new Uri(ServerConfig.LEAGUE_SUBSCRIPTION_RESUBSUBSCRIBE + SettingsMobile.Instance.User.CurrentLeagueId.ToString().Replace("-", ""));
                    wbt.Show();
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void DuesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((SettingsMobile.Instance.AccountSettings.IsTreasurer || SettingsMobile.Instance.AccountSettings.IsManagerOrBetter) && SettingsMobile.Instance.AccountSettings.IsSubscriptionActive)
                    NavigationService.Navigate(new Uri("/View/MyLeague/Dues/Dues.xaml", UriKind.Relative));
                else if (SettingsMobile.Instance.AccountSettings.IsSubscriptionActive && !SettingsMobile.Instance.AccountSettings.IsDuesManagementLockedDown)
                    NavigationService.Navigate(new Uri("/View/MyLeague/Dues/Dues.xaml", UriKind.Relative));
                else if (SettingsMobile.Instance.AccountSettings.IsSubscriptionActive && SettingsMobile.Instance.AccountSettings.IsDuesManagementLockedDown)
                    NavigationService.Navigate(new Uri("/View/MyLeague/Dues/DuesForMember.xaml", UriKind.Relative));
                else
                {
                    WebBrowserTask wbt = new WebBrowserTask();
                    wbt.Uri = new Uri(ServerConfig.LEAGUE_SUBSCRIPTION_RESUBSUBSCRIBE + SettingsMobile.Instance.User.CurrentLeagueId.ToString().Replace("-", ""));
                    wbt.Show();
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SettingsMobile.Instance.AccountSettings.IsSubscriptionActive)
                {
                    NavigationService.Navigate(new Uri("/View/MyLeague/Calendar/CalendarList.xaml", UriKind.Relative));
                }
                else
                {
                    WebBrowserTask wbt = new WebBrowserTask();
                    wbt.Uri = new Uri(ServerConfig.LEAGUE_SUBSCRIPTION_RESUBSUBSCRIBE + SettingsMobile.Instance.User.CurrentLeagueId.ToString().Replace("-", ""));
                    wbt.Show();
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }


        private void RSVP_Click(object sender, RoutedEventArgs e)
        {
            var blah = ((MenuItem)sender).DataContext as CalendarEventPortable;
            NavigationService.Navigate(new Uri("/View/MyLeague/Calendar/SetAvailabilityForEvent.xaml?evId=" + blah.CalendarItemId + "&calId=" + SettingsMobile.Instance.AccountSettings.CalendarId + "&name=" + blah.Name, UriKind.Relative));

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

        private void EventList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    var ev = (CalendarEventPortable)e.AddedItems[0];
                    NavigationService.Navigate(new Uri("/View/MyLeague/Calendar/ViewCalendarEvent.xaml?evId=" + ev.CalendarItemId + "&calId=" + SettingsMobile.Instance.AccountSettings.CalendarId + "&name=" + ev.Name, UriKind.Relative));

                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void TopicsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    var topic = (ForumTopicModel)e.AddedItems[0];
                    NavigationService.Navigate(new Uri("/View/MyLeague/Forum/ViewForumTopic.xaml?tid=" + topic.TopicId, UriKind.Relative));

                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void VotePollItem1_Click(object sender, RoutedEventArgs e)
        {
            var blah = ((MenuItem)sender).DataContext as VotingClass;
            if (!blah.CanVotePoll)
                NavigationService.Navigate(new Uri("/View/MyLeague/Polls/VotePoll.xaml?pid=" + blah.VotingId, UriKind.Relative));

        }

        private void CheckIn_Click(object sender, RoutedEventArgs e)
        {
            var blah = ((MenuItem)sender).DataContext as CalendarEventPortable;
            NavigationService.Navigate(new Uri("/View/MyLeague/Calendar/CheckSelfIntoEvent.xaml?evId=" + blah.CalendarItemId + "&calId=" + SettingsMobile.Instance.AccountSettings.CalendarId + "&name=" + blah.Name, UriKind.Relative));

        }

        private void ContactsButton_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsMobile.Instance.AccountSettings.IsSubscriptionActive)
            {
                WebBrowserTask wbt = new WebBrowserTask();
                wbt.Uri = new Uri(ServerConfig.LEAGUE_CONTACTS_URL);
                wbt.Show();
            }
            else
            {
                WebBrowserTask wbt = new WebBrowserTask();
                wbt.Uri = new Uri(ServerConfig.LEAGUE_SUBSCRIPTION_RESUBSUBSCRIBE + SettingsMobile.Instance.User.CurrentLeagueId.ToString().Replace("-", ""));
                wbt.Show();
            }
        }

        private void DocsButton_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsMobile.Instance.AccountSettings.IsSubscriptionActive)
            {
                WebBrowserTask wbt = new WebBrowserTask();
                wbt.Uri = new Uri(ServerConfig.LEAGUE_DOCUMENTS_URL + SettingsMobile.Instance.User.CurrentLeagueId.ToString().Replace("-", ""));
                wbt.Show();
            }
            else
            {
                WebBrowserTask wbt = new WebBrowserTask();
                wbt.Uri = new Uri(ServerConfig.LEAGUE_SUBSCRIPTION_RESUBSUBSCRIBE + SettingsMobile.Instance.User.CurrentLeagueId.ToString().Replace("-", ""));
                wbt.Show();
            }
        }

        private void JobsButton_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsMobile.Instance.AccountSettings.IsSubscriptionActive)
            {
                WebBrowserTask wbt = new WebBrowserTask();
                wbt.Uri = new Uri(ServerConfig.LEAGUE_JOB_BOARD_URL);
                wbt.Show();
            }
            else
            {
                WebBrowserTask wbt = new WebBrowserTask();
                wbt.Uri = new Uri(ServerConfig.LEAGUE_SUBSCRIPTION_RESUBSUBSCRIBE + SettingsMobile.Instance.User.CurrentLeagueId.ToString().Replace("-", ""));
                wbt.Show();
            }
        }

        private void InventoryButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(ServerConfig.LEAGUE_INVENTORY_URL);
            wbt.Show();
        }

        private void SponsorsButton_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsMobile.Instance.AccountSettings.IsSubscriptionActive)
            {
                WebBrowserTask wbt = new WebBrowserTask();
                wbt.Uri = new Uri(ServerConfig.LEAGUE_SPONSORS_URL);
                wbt.Show();
            }
            else
            {
                WebBrowserTask wbt = new WebBrowserTask();
                wbt.Uri = new Uri(ServerConfig.LEAGUE_SUBSCRIPTION_RESUBSUBSCRIBE + SettingsMobile.Instance.User.CurrentLeagueId.ToString().Replace("-", ""));
                wbt.Show();
            }
        }

        private void SubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(ServerConfig.LEAGUE_SUBSCRIPTION_RESUBSUBSCRIBE + SettingsMobile.Instance.User.CurrentLeagueId.ToString().Replace("-", ""));
            wbt.Show();
        }

        private void ShopButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(ServerConfig.LEAGUE_SHOPS_URL + SettingsMobile.Instance.AccountSettings.ShopEndUrl);
            wbt.Show();
        }

        private void ChallengesButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(ServerConfig.GAMES_BOUT_CHALLENGES);
            wbt.Show();
        }

        private void Officials_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(ServerConfig.GAMES_OFFICIATING_REQUESTS_URL);
            wbt.Show();
        }

        private void MySettings_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(ServerConfig.MEMBER_SETTINGS_URL);
            wbt.Show();
        }

        private void MyMedical_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(ServerConfig.MEMBER_MEDICAL_URL);
            wbt.Show();
        }

        private void MyContacts_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(ServerConfig.MEMBER_CONTACTS_URL);
            wbt.Show();

        }

        private void SetupButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(ServerConfig.WEBSITE_SETUP_LEAGUE_URL);
            wbt.Show();

        }

        private void JoinButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(ServerConfig.LEAGUE_JOIN_URL);
            wbt.Show();
        }

    }
}