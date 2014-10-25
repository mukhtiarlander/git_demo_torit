using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.Portable.Classes.Controls.Message;
using RDN.Portable.Settings;
using RDN.WP.Library.Database;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.WP.Library.Classes.League;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RDN.WP.View.MyLeague.Messages
{
    public partial class MessagesView : PhoneApplicationPage
    {
        private DispatcherTimer _timer;
        private int _countdown;

        ProgressIndicator progressIndicator;
        ConversationModel conversation;
        public MessagesView()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                conversation = new ConversationModel();

                //(App.Current as App).SecondPageObject = null;
                progressIndicator = SystemTray.ProgressIndicator;
                progressIndicator = new ProgressIndicator();
                SystemTray.SetProgressIndicator(this, progressIndicator);
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Pulling Messages...";
                if (SettingsMobile.Instance.User == null)
                {
                    SqlFactory fact = new SqlFactory();
                    SettingsMobile.Instance.User = fact.GetProfile();
                }
                conversation.GroupMessageId = Convert.ToInt64(this.NavigationContext.QueryString["mid"]);
                PullTopic();

                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(20);
                _timer.Tick += (s, eee) => Tick();
                _timer.Start();
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }

        }
        private void Tick()
        {
            PullTopic();
        }
        void PullTopic()
        {
            Dispatcher.BeginInvoke(delegate
            {
                progressIndicator.IsVisible = true;
                progressIndicator.Text = "Pulling Messages...";
            });
            try
            {
                MessagesMobile.PullMessageTopic(SettingsMobile.Instance.User.MemberId.ToString(), SettingsMobile.Instance.User.LoginId.ToString(), conversation.GroupMessageId.ToString(), new Action<ConversationModel>(ReturnMessageTopic));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        void ReturnMessageTopic(ConversationModel model)
        {
            try
            {
                if (conversation.Messages.Count < model.Messages.Count)
                {
                    Dispatcher.BeginInvoke(delegate
                    {
                        try
                        {
                            topicName.Title = conversation.Title;
                            MessageList.ItemsSource = conversation.Messages;
                            RecipientsList.ItemsSource = conversation.Recipients;
                            MessageList.ScrollTo(conversation.Messages.LastOrDefault());
                        }
                        catch (Exception exception)
                        {
                            ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                        }
                    });
                }
                conversation = model;

                Dispatcher.BeginInvoke(delegate
                  {
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

        private void ReplyText_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ReplyText.Text.Equals("Write a Reply...", StringComparison.OrdinalIgnoreCase))
            {
                ReplyText.Text = string.Empty;
            }
        }

        private void ReplyText_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ReplyText.Text))
            {
                ReplyText.Text = "Write a Reply...";
            }
        }

        private void SendMessageBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                progressIndicator.IsVisible = true;
                progressIndicator.Text = "Sending Message...";
                conversation.Messages.Add(new MessageSingleModel()
                {
                    FromName = SettingsMobile.Instance.User.DerbyName,
                    MessageText = ReplyText.Text,
                });

                MessageList.ItemsSource = conversation.Messages;
                MessageList.ScrollTo(conversation.Messages.LastOrDefault());
                conversation.Message = ReplyText.Text;
                Task.Run(new Action(() =>
                {
                    try
                    {

                        conversation.MemberId = SettingsMobile.Instance.User.MemberId;
                        conversation.UserId = SettingsMobile.Instance.User.LoginId;
                        Dispatcher.BeginInvoke(delegate
                        {
                            ReplyText.Text = "Write a Reply...";
                        });
                        MessagesMobile.ReplyToMessage(conversation);
                        Dispatcher.BeginInvoke(delegate
                        {
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