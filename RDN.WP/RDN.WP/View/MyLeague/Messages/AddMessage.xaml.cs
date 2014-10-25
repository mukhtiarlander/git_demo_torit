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
using RDN.Portable.Classes.Controls.Message;
using RDN.WP.Classes.UI;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Controls.Message.Enums;
using System.Collections.ObjectModel;
using Coding4Fun.Toolkit.Controls;

namespace RDN.WP.View.MyLeague.Messages
{
    public partial class AddMessage : PhoneApplicationPage
    {

        ConversationModel conversationModel;
        ForumGroupModel currentGroup;
        ProgressIndicator progressIndicator;
        ObservableCollection<NameIdModel> Names;
        public AddMessage()
        {
            InitializeComponent();
            try
            {
                Names = new ObservableCollection<NameIdModel>();
                progressIndicator = SystemTray.ProgressIndicator;
                progressIndicator = new ProgressIndicator();

                SystemTray.SetProgressIndicator(this, progressIndicator);
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Pulling Groups...";
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Names = new ObservableCollection<NameIdModel>();
            if ((App.Current as App).SecondPageObject != null)
                Names = ((ObservableCollection<NameIdModel>)(App.Current as App).SecondPageObject);

            Dispatcher.BeginInvoke(delegate
                                                 {
                                                     NamesOfSending.Text = String.Empty;
                                                     for (int i = 0; i < Names.Count; i++)
                                                         NamesOfSending.Text += Names[i].Name + ", ";

                                                 });
        }


        private void Groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    var group = (ForumGroupModel)e.AddedItems[0];
                    //reset category for the group

                    progressIndicator.Text = "Pulling Categories...";
                    if (group.GroupId != conversationModel.GroupId)
                    {
                        conversationModel.GroupId = group.GroupId;
                        PullCategories();
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        void ReturnCategoriesModel(List<ForumCategory> model)
        {
            try
            {
                model.Insert(0, new ForumCategory() { CategoryName = "Latest", CategoryId = 0 });
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

        void PullCategories()
        {
            progressIndicator.IsVisible = true;
            try
            {
                ForumMobile.PullForumCategories(SettingsMobile.Instance.User.MemberId.ToString(), SettingsMobile.Instance.User.LoginId.ToString(), conversationModel.GroupId.ToString(), new Action<List<ForumCategory>>(ReturnCategoriesModel));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void Categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    var cat = (ForumCategory)e.AddedItems[0];
                    //reset category for the group
                    //conversationModel.CategoryId = cat.CategoryId;

                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void SendPost_Click(object sender, EventArgs e)
        {
            progressIndicator.Text = "Sending Message...";
            progressIndicator.IsVisible = true;
            try
            {
                ConversationModel con = new ConversationModel();
                if (Regular.IsChecked.GetValueOrDefault())
                    con.MessageTypeEnum = MessageTypeEnum.Regular;
                else if (Text.IsChecked.GetValueOrDefault())
                    con.MessageTypeEnum = MessageTypeEnum.Text;
                con.MemberId = SettingsMobile.Instance.User.MemberId;
                con.Message = MessageInput.Text;
                con.Title = Title.Text;
                con.UserId = SettingsMobile.Instance.User.LoginId;
                con.FromId = SettingsMobile.Instance.User.MemberId;
                foreach (var id in Names)
                {
                    long idOfGroup = 0;
                    Guid tempId = new Guid();
                    if (Int64.TryParse(id.Id, out idOfGroup))
                    {
                        con.GroupIds.Add(idOfGroup);
                    }
                    else if (Guid.TryParse(id.Id, out tempId))
                    {
                        con.Recipients.Add(new MemberDisplayMessage() { MemberId = tempId });
                    }
                }
                con.SendEmailForMessage = SendEmail.IsChecked.GetValueOrDefault();

                Task.Run(new Action(() =>
                {
                    try
                    {
                        var m = MessagesMobile.SendNewMessage(con);

                        (App.Current as App).SecondPageObject = conversationModel;
                        Dispatcher.BeginInvoke(delegate
                                                {
                                                    progressIndicator.IsVisible = false;
                                                    ToastPrompt t = new ToastPrompt
                                                    {
                                                        Title = "Message Sent",
                                                        TextOrientation = System.Windows.Controls.Orientation.Vertical

                                                    };
                                                    t.Show();
                                                    NavigationService.Navigate(new Uri("/View/MyLeague/MyLeague.xaml", UriKind.RelativeOrAbsolute));
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

        private void Title_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Title.Text.Equals("Title Of Message...", StringComparison.OrdinalIgnoreCase))
            {
                Title.Text = string.Empty;
            }
        }

        private void Title_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Title.Text))
            {
                Title.Text = "Title Of Message...";
            }
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            (App.Current as App).SecondPageObject = Names;
            NavigationService.Navigate(new Uri("/View/MyLeague/Generic/MemberGroupChooser.xaml?type=AddMessage", UriKind.RelativeOrAbsolute));
        }

        private void Text_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Regular_Click(object sender, RoutedEventArgs e)
        {

        }



    }
}