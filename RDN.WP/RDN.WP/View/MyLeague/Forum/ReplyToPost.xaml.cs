using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.Portable.Classes.Controls.Forum;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.WP.Library.Classes.League;
using RDN.Portable.Settings;
using RDN.Portable.Classes.Forum.Enums;
using System.Threading.Tasks;
using Coding4Fun.Toolkit.Controls;

namespace RDN.WP.View.MyLeague.Forum
{
    public partial class ReplyToPost : PhoneApplicationPage
    {

        ForumTopicModel forumModel;
        ProgressIndicator progressIndicator;
        public ReplyToPost()
        {
            InitializeComponent();
            try
            {
                progressIndicator = SystemTray.ProgressIndicator;
                progressIndicator = new ProgressIndicator();

                SystemTray.SetProgressIndicator(this, progressIndicator);
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Posting Reply...";

                forumModel = (ForumTopicModel)(App.Current as App).SecondPageObject;
                (App.Current as App).SecondPageObject = null;

                TopicName.Text = forumModel.TopicName;

            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void SendPost_Click(object sender, EventArgs e)
        {
            progressIndicator.IsVisible = true;
            try
            {
                AddForumTopicModel model = new AddForumTopicModel();
                model.BroadcastMessage = Broadcast.IsChecked.GetValueOrDefault();
                model.ForumId = forumModel.ForumId;
                model.MemberId = SettingsMobile.Instance.User.MemberId.ToString();
                model.UserId = SettingsMobile.Instance.User.LoginId.ToString();
                model.TopicId = forumModel.TopicId;
                model.Text = MessageInput.Text;
                model.CategoryId = forumModel.CategoryId;
                Task.Run(new Action(() =>
                {
                    try
                    {
                        var m = ForumMobile.ReplyToForumTopic(model);
                        (App.Current as App).SecondPageObject = new ForumModel() { ForumId = model.ForumId, CategoryId = model.CategoryId };
                        
                        Dispatcher.BeginInvoke(delegate
                                                {
                                                    ToastPrompt t = new ToastPrompt
                                                    {
                                                        Title = "Message Sent",
                                                        TextOrientation = System.Windows.Controls.Orientation.Vertical
                                                    };
                                                    progressIndicator.IsVisible = false;
                                                    t.Show();
                                                    NavigationService.Navigate(new Uri("/View/MyLeague/ForumMain.xaml", UriKind.RelativeOrAbsolute));
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

        private void MessageInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (MessageInput.Text.Equals("What do you have to say?...", StringComparison.OrdinalIgnoreCase))
            {
                MessageInput.Text = string.Empty;
            }
        }

        private void MessageInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MessageInput.Text))
            {
                MessageInput.Text = "What do you have to say?...";
            }
        }
    }
}