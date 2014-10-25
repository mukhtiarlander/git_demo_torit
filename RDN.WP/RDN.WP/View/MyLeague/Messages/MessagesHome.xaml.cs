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
using System.Collections.ObjectModel;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.Portable.Settings;
using RDN.WP.Library.Classes.League;
using RDN.Portable.Classes.Controls.Message.Enums;

namespace RDN.WP.View.MyLeague.Messages
{
    public partial class MessagesHome : PhoneApplicationPage
    {
        private int PAGE_COUNT = 15;
        private int lastPagePulled = 0;
        bool isLoading = false;
        int _offsetKnob = 7;
        MessageModel messageModel = new MessageModel();
        ObservableCollection<ConversationModel> CurrentConversations;
        ProgressIndicator progressIndicator;
        public MessagesHome()
        {
            InitializeComponent();


            try
            {
                MessagesList.ItemRealized += MessageList_ItemRealized;
                progressIndicator = SystemTray.ProgressIndicator;
                progressIndicator = new ProgressIndicator();

                SystemTray.SetProgressIndicator(this, progressIndicator);
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Pulling Messages...";

                CurrentConversations = new ObservableCollection<ConversationModel>();
                PullMessages();

            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        void MessageList_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            //try
            //{
            //    if (!isLoading && MessagesList.ItemsSource != null && MessagesList.ItemsSource.Count >= _offsetKnob)
            //    {
            //        if (e.ItemKind == LongListSelectorItemKind.Item)
            //        {
            //            if ((e.Container.Content as ConversationModel).Equals(MessagesList.ItemsSource[MessagesList.ItemsSource.Count - _offsetKnob]))
            //            {
            //                lastPagePulled += 1;
            //                PullMessages();
            //            }
            //        }
            //    }
            //}
            //catch (Exception exception)
            //{
            //    ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            //}
        }
        void PullMessages()
        {
            progressIndicator.IsVisible = true;
            isLoading = true;

            try
            {
                MessagesMobile.PullMessages(SettingsMobile.Instance.User.MemberId.ToString(), SettingsMobile.Instance.User.LoginId.ToString(), lastPagePulled, PAGE_COUNT, GroupOwnerTypeEnum.league, new Action<MessageModel>(ReturnMessageModel));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        void ReturnMessageModel(MessageModel model)
        {
            try
            {
                messageModel = model;

                Dispatcher.BeginInvoke(delegate
                {

                    if (messageModel != null)
                    {
                        MessagesList.ItemsSource = model.Conversations;
                    }
                    progressIndicator.IsVisible = false;
                    isLoading = false;
                });
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void MessagesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    var topic = (ConversationModel)e.AddedItems[0];
                    //topic.ForumId = forumModel.ForumId;
                    //(App.Current as App).SecondPageObject = topic;
                    NavigationService.Navigate(new Uri("/View/MyLeague/Messages/MessagesView.xaml?mid=" + topic.GroupMessageId, UriKind.Relative));

                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void AddPost_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/MyLeague/Messages/AddMessage.xaml", UriKind.Relative));

        }

    }
}