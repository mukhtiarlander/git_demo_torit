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
    public partial class AddPost : PhoneApplicationPage
    {

        ForumModel forumModel;
        ForumGroupModel currentGroup;
        ProgressIndicator progressIndicator;
        public AddPost()
        {
            InitializeComponent();
            try
            {
                progressIndicator = SystemTray.ProgressIndicator;
                progressIndicator = new ProgressIndicator();

                SystemTray.SetProgressIndicator(this, progressIndicator);
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Pulling Categories...";

                forumModel = (ForumModel)(App.Current as App).SecondPageObject;
                Groups.ItemsSource = forumModel.Groups;
                currentGroup = forumModel.Groups.Where(x => x.GroupId == forumModel.GroupId).FirstOrDefault();
                if (currentGroup != null)
                {
                    ForumGroupName.Text = currentGroup.GroupName;
                    //groupName.Text = currentGroup.GroupName;
                    Groups.SelectedItem = currentGroup;
                    Categories.ItemsSource = currentGroup.Categories;
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }


        private void Groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    var group = (ForumGroupModel)e.AddedItems[0];
                    //reset category for the group
                    forumModel.CategoryId = 0;
                    progressIndicator.Text = "Pulling Categories...";
                    if (group.GroupId != forumModel.GroupId)
                    {
                        forumModel.GroupId = group.GroupId;
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
                    Categories.ItemsSource = model;
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
                ForumMobile.PullForumCategories(SettingsMobile.Instance.User.MemberId.ToString(), SettingsMobile.Instance.User.LoginId.ToString(), forumModel.GroupId.ToString(), new Action<List<ForumCategory>>(ReturnCategoriesModel));
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
                    forumModel.CategoryId = cat.CategoryId;

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
                AddForumTopicModel model = new AddForumTopicModel();
                model.BroadcastMessage = Broadcast.IsChecked.GetValueOrDefault();
                model.CategoryId = forumModel.CategoryId;
                model.ForumId = forumModel.ForumId;
                model.GroupId = forumModel.GroupId;
                model.MemberId = SettingsMobile.Instance.User.MemberId.ToString();
                model.UserId = SettingsMobile.Instance.User.LoginId.ToString();
                model.ForumType = ForumOwnerTypeEnum.league.ToString();
                model.LockMessage = Lock.IsChecked.GetValueOrDefault();
                model.PinMessage = Pinn.IsChecked.GetValueOrDefault();
                model.Text = MessageInput.Text;
                model.TopicTitle = Title.Text;

                Task.Run(new Action(() =>
                {
                    try
                    {
                        var m = ForumMobile.AddNewForumTopic(model);
                        ToastPrompt t = new ToastPrompt
                        {
                            Title = "Message Sent",
                            TextOrientation = System.Windows.Controls.Orientation.Vertical
                        };

                        (App.Current as App).SecondPageObject = forumModel;
                        Dispatcher.BeginInvoke(delegate
                                                {
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



    }
}