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
using RDN.WP.Library.Database;
using RDN.WP.Library.Classes.RSS;
using System.ServiceModel.Syndication;
using System.Windows.Media.Imaging;
using RDN.Portable.Settings;
using Microsoft.Phone.Notification;
using System.Text;
using RDN.Portable.Classes.Account;
using RDN.WP.Library.Classes.Account;
using System.Threading.Tasks;

namespace RDN.WP
{
    public partial class MainPage : PhoneApplicationPage
    {
        List<SyndicationItemModel> items = new List<SyndicationItemModel>();

        public MainPage()
        {
            InitializeComponent();


            loadFeedButton_Click();

            SqlFactory fact = new SqlFactory();
            SettingsMobile.Instance.User = fact.GetProfile();
            LoadPushNotifications();
        }

        void LoadPushNotifications()
        {
            /// Holds the push channel that is created or found.
            HttpNotificationChannel pushChannel;

            // The name of our push channel.
            string channelName = "RDNation";

            // Try to find the push channel.
            pushChannel = HttpNotificationChannel.Find(channelName);


            // If the channel was not found, then create a new connection to the push service.
            if (pushChannel == null)
            {
                pushChannel = new HttpNotificationChannel(channelName);

                // Register for all the events before attempting to open the channel.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                pushChannel.Open();

                // Bind this new channel for toast events.
                pushChannel.BindToShellToast();

            }
            else
            {
                // The channel was already open, so just register for all the events.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                // Display the URI for testing purposes. Normally, the URI would be passed back to your web service at this point.
                //System.Diagnostics.Debug.WriteLine(pushChannel.ChannelUri.ToString());
                //MessageBox.Show(String.Format("Channel Uri is {0}",
                //    pushChannel.ChannelUri.ToString()));
                UpdateChannelForRDNation(pushChannel.ChannelUri.ToString());
            }


        }

        private static void UpdateChannelForRDNation(string channelUri)
        {
            Task.Run(() =>
            {
                NotificationMobileJson json = new NotificationMobileJson();
                json.CanSendGameNotifications = true;
                json.CanSendForumNotifications = true;
                json.MobileTypeEnum = MobileTypeEnum.WP8;
                json.NotificationId = channelUri;
                
                if (SettingsMobile.Instance.User != null)
                {
                    json.MemberId = SettingsMobile.Instance.User.MemberId.ToString();
                    NotificationMobileJsonWP.SendNotificationId(json);
                }
                SqlFactory fact = new SqlFactory();
                fact.InsertNotificationSettings(json);
            });
        }

        private void PushChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            StringBuilder message = new StringBuilder();
            string relativeUri = string.Empty;

            message.AppendFormat("Received Toast {0}:\n", DateTime.Now.ToShortTimeString());

            // Parse out the information that was part of the message.
            foreach (string key in e.Collection.Keys)
            {
                message.AppendFormat("{0}: {1}\n", key, e.Collection[key]);

                if (string.Compare(
                    key,
                    "wp:Param",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.CompareOptions.IgnoreCase) == 0)
                {
                    relativeUri = e.Collection[key];
                }
            }

        }

        private void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {

            Exception exception = new Exception(e.Message);
            ErrorHandler.Save(exception, MobileTypeEnum.WP8, additionalInformation: e.ErrorCode + ":" + e.ErrorAdditionalData);
            MessageBox.Show(String.Format("A push notification {0} error occurred.  {1} ({2}) {3}",
                e.ErrorType, e.Message, e.ErrorCode, e.ErrorAdditionalData));
        }

        private void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            UpdateChannelForRDNation(e.ChannelUri.ToString());

        }
        /// <summary>
        /// loads RN feeds.
        /// </summary>
        private async void loadFeedButton_Click()
        {
            var rssItems = await RssService.Execute("http://rollinnews.com/syndication.axd?maxitems=10&format=Rss");
            items.AddRange(rssItems);

            SlideList.ItemsSource = items.Take(8);


        }

        private void MyLeague_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlFactory fact = new SqlFactory();
                var profile = fact.GetProfile();

                if (profile == null || profile.LastMobileLoginDate < DateTime.UtcNow.AddDays(-30))
                    NavigationService.Navigate(new Uri("/View/Account/Login.xaml", UriKind.Relative));
                else
                    NavigationService.Navigate(new Uri("/View/MyLeague/MyLeague.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        
        private void Scores_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new Uri("/View/Public/Game/Games.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void Events_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new Uri("/View/Public/Events.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void Skaters_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/Skaters.xaml", UriKind.Relative));
        }

        private void Leagues_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/Public/Leagues.xaml", UriKind.Relative));
        }

        private void Shop_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/Public/Shop/ShopsItems.xaml", UriKind.Relative));
        }
        /// <summary>
        /// go to the blog post.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SlideList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var task = new Microsoft.Phone.Tasks.WebBrowserTask
                {
                    Uri = new Uri(((SyndicationItemModel)e.AddedItems[0]).Url)
                };

                task.Show();

            }
        }

        private void FeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new Uri("/View/Feedback.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

    }
}