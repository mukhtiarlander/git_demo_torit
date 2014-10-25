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
using System.Threading.Tasks;
using RDN.WP.Library.Classes.League;
using RDN.Portable.Settings;
using RDN.Portable.Classes.Forum.Enums;
using RDN.Portable.Classes.Utilities;
using System.Collections.ObjectModel;

namespace RDN.WP.View.MyLeague
{
    public partial class ForumMain : PhoneApplicationPage
    {
        private int PAGE_COUNT = 40;
        private int lastPagePulled = 0;
        bool isLoading = false;
        int _offsetKnob = 7;
        ForumModel forumModel = new ForumModel();
        ObservableCollection<ForumTopicModel> CurrentTopics;
        ProgressIndicator progressIndicator;
        //long currentGroupId = 0;
        public ForumMain()
        {
            InitializeComponent();
            try
            {
                TopicsList.ItemRealized += TopicsList_ItemRealized;
                progressIndicator = SystemTray.ProgressIndicator;
                progressIndicator = new ProgressIndicator();

                SystemTray.SetProgressIndicator(this, progressIndicator);
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Pulling Forum...";

                if ((App.Current as App).SecondPageObject != null)
                {
                    try
                    {
                        forumModel = (ForumModel)(App.Current as App).SecondPageObject;
                        (App.Current as App).SecondPageObject = null;
                        forumModel.CategoryId = 0;
                        ForumPivot.SelectedIndex = 1;
                    }
                    catch (Exception exception)
                    {
                        ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                    }
                }

                CurrentTopics = new ObservableCollection<ForumTopicModel>();
                PullGroups();

            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        void TopicsList_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            try
            {
                if (!isLoading && TopicsList.ItemsSource != null && TopicsList.ItemsSource.Count >= _offsetKnob)
                {
                    if (e.ItemKind == LongListSelectorItemKind.Item)
                    {
                        if ((e.Container.Content as ForumTopicModel).Equals(TopicsList.ItemsSource[TopicsList.ItemsSource.Count - _offsetKnob]))
                        {
                            lastPagePulled += 1;
                            PullGroups();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        /// <summary>
        /// returns the forum model from the api.
        /// </summary>
        /// <param name="model"></param>
        void ReturnForumModel(ForumModel model)
        {
            try
            {
                ForumGroupModel currentGroup = null;
                var pulledTopics = model.Groups.Where(x => x.GroupId == forumModel.GroupId).FirstOrDefault();
                if (lastPagePulled == 0)
                {
                    forumModel = model;
                    currentGroup = forumModel.Groups.Where(x => x.GroupId == forumModel.GroupId).FirstOrDefault();
                    CurrentTopics.Clear();
                    for (int i = 0; i < currentGroup.Topics.Count; i++)
                        CurrentTopics.Add(currentGroup.Topics[i]);
                }
                else
                {
                    currentGroup = model.Groups.Where(x => x.GroupId == forumModel.GroupId).FirstOrDefault();
                    for (int i = 0; i < currentGroup.Topics.Count; i++)
                        CurrentTopics.Add(currentGroup.Topics[i]);
                }

                Dispatcher.BeginInvoke(delegate
                                    {

                                        GroupNamesList.ItemsSource = model.Groups;
                                        if (currentGroup != null)
                                        {
                                            TopicsList.ItemsSource = CurrentTopics;
                                            CategoryNamesList.ItemsSource = currentGroup.Categories;
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

        void PullGroups()
        {
            progressIndicator.IsVisible = true;
            isLoading = true;
            
            try
            {
                ForumMobile.PullForumTopics(SettingsMobile.Instance.User.MemberId.ToString(), SettingsMobile.Instance.User.LoginId.ToString(), ForumOwnerTypeEnum.league.ToString(), forumModel.GroupId.ToString(), forumModel.CategoryId.ToString(), lastPagePulled, PAGE_COUNT, new Action<ForumModel>(ReturnForumModel));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        private void GroupNamesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    var group = (ForumGroupModel)e.AddedItems[0];
                    //reset category for the group
                    forumModel.CategoryId = 0;
                    lastPagePulled = 0;
                    if (group.GroupId != forumModel.GroupId)
                    {

                        forumModel.GroupId = group.GroupId;
                        PullGroups();
                    }
                    ForumPivot.SelectedIndex = 1;
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
                    //topic.ForumId = forumModel.ForumId;
                    //(App.Current as App).SecondPageObject = topic;
                    NavigationService.Navigate(new Uri("/View/MyLeague/Forum/ViewForumTopic.xaml?tid=" + topic.TopicId, UriKind.Relative));

                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void AddPost_Click(object sender, EventArgs e)
        {
            (App.Current as App).SecondPageObject = forumModel;
            NavigationService.Navigate(new Uri("/View/MyLeague/Forum/AddPost.xaml", UriKind.Relative));
        }

        private void CategoryNamesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    var category = (ForumCategory)e.AddedItems[0];
                    forumModel.CategoryId = category.CategoryId;
                    TopicsList.ItemsSource = forumModel.Groups.Where(x => x.GroupId == forumModel.GroupId).FirstOrDefault().Topics.Where(x => x.CategoryId == category.CategoryId).ToList();
                    ForumPivot.SelectedIndex = 1;
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
    }
}