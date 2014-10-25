using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.Portable.Config.Enums;
using RDN.WP.Classes.Error;
using RDN.Portable.Classes.Controls.Forum;
using RDN.WP.Library.Classes.League;
using RDN.Portable.Settings;
using System.Threading.Tasks;
using RDN.WP.Library.Database;

namespace RDN.WP.View.MyLeague.Forum
{
    public partial class ViewForumTopic : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        ForumTopicModel topic;
        public ViewForumTopic()
        {
            InitializeComponent();
           
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                topic = new ForumTopicModel();

                //(App.Current as App).SecondPageObject = null;
                progressIndicator = SystemTray.ProgressIndicator;
                progressIndicator = new ProgressIndicator();
                SystemTray.SetProgressIndicator(this, progressIndicator);
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Pulling Topic...";
                if (SettingsMobile.Instance.User == null)
                {
                    SqlFactory fact = new SqlFactory();
                    SettingsMobile.Instance.User = fact.GetProfile();
                }
                topic.TopicId = Convert.ToInt64(this.NavigationContext.QueryString["tid"]);
                PullTopic();
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }

        }
        void PullTopic()
        {
            Dispatcher.BeginInvoke(delegate
                        {
                            progressIndicator.IsVisible = true;
                        });
            try
            {
                ForumMobile.PullForumTopic(SettingsMobile.Instance.User.MemberId.ToString(), SettingsMobile.Instance.User.LoginId.ToString(), topic.TopicId.ToString(), new Action<ForumTopicModel>(ReturnForumTopic));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        void ReturnForumTopic(ForumTopicModel model)
        {
            try
            {
                topic = model;
                MessageList.ItemsSource = topic.Posts;

                Dispatcher.BeginInvoke(delegate
                {
                    topicName.Title = topic.TopicName;
                    if (topic.IsLocked)
                    {
                        ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
                        b.IsEnabled = false;
                    }
                    for (int i = 0; i < topic.TopicInbox.Count; i++)
                        Unread.Text += topic.TopicInbox[i].DerbyName + ", ";
                    Category.Text = topic.Category;
                    Locked.Text = topic.IsLocked.ToString();
                    Pinned.Text = topic.IsPinned.ToString();
                    Watching.Text = topic.IsWatching.ToString();
                    StartedBy.Text = topic.StartedByName;
                    ViewCount.Text = topic.ViewCount.ToString();
                    if (topic.IsWatching)
                    {
                        ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
                        b.Text = "UnWatch";
                    }
                    MessageList.ItemsSource = topic.Posts;
                    progressIndicator.IsVisible = false;
                });
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }


        private void MessageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ReplyPost_Click(object sender, EventArgs e)
        {
            (App.Current as App).SecondPageObject = topic;
            NavigationService.Navigate(new Uri("/View/MyLeague/Forum/ReplyToPost.xaml", UriKind.Relative));

        }

        private void WatchPostBar_Click(object sender, EventArgs e)
        {
            progressIndicator.IsVisible = true;
            progressIndicator.Text = "Watching...";
            try
            {
                AddForumTopicModel model = new AddForumTopicModel();
                model.ForumId = topic.ForumId;
                model.MemberId = SettingsMobile.Instance.User.MemberId.ToString();
                model.UserId = SettingsMobile.Instance.User.LoginId.ToString();
                model.TopicId = topic.TopicId;
                Task.Run(new Action(() =>
                {
                    try
                    {
                        var m = ForumMobile.ToggleWatchingForumTopic(model);
                        if (m)
                            topic.IsWatching = !topic.IsWatching;

                        Dispatcher.BeginInvoke(delegate
                        {
                            ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[1];

                            if (topic.IsWatching)
                                b.Text = "UnWatch";
                            else
                                b.Text = "Watch";
                            Watching.Text = topic.IsWatching.ToString();
                            progressIndicator.IsVisible = false;
                        });
                    }
                    catch (Exception exception)
                    {
                        ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                    }
                }));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }

        }
    }
}