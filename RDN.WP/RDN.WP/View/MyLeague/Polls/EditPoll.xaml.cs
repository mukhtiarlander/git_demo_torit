using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.WP.Classes.UI;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.Portable.Classes.Voting;
using System.Collections.ObjectModel;
using RDN.WP.Classes.Collections;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Settings;
using System.Threading.Tasks;
using RDN.WP.Library.Classes.League;
using Coding4Fun.Toolkit.Controls;
using RDN.WP.Library.Database;

namespace RDN.WP.View.MyLeague.Polls
{
    public partial class EditPoll : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        ObservableCollection<NameIdModel> Names;
        VotingClass Voting;
        TrulyObservableCollection<VotingQuestionClass> Questions;

        public EditPoll()
        {
            InitializeComponent();

            try
            {
                Names = new ObservableCollection<NameIdModel>();
                progressIndicator = SystemTray.ProgressIndicator;
                progressIndicator = new ProgressIndicator();

                Voting = new VotingClass();
                Questions = new TrulyObservableCollection<VotingQuestionClass>();

                QuestionList.ItemsSource = Questions;
                RecipientsList.ItemsSource = Names;

            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            try
            {






                var currentObject = (App.Current as App).SecondPageObject;
                if (currentObject != null)
                {
                    if (currentObject.GetType() == typeof(VotingQuestionClass))
                    {
                        var quest = ((VotingQuestionClass)currentObject);
                        var tempQuest = Questions.Where(x => x.QuestionId == quest.QuestionId).FirstOrDefault();
                        if (tempQuest == null)
                        {
                            Voting.Questions.Add(quest);
                            Questions.Add(quest);
                        }
                        else
                        {
                            if (quest.IsDeleted)
                            {
                                Questions.Remove(tempQuest);
                            }
                            else
                            {
                                int index = Questions.IndexOf(tempQuest);
                                Questions.Remove(tempQuest);
                                Questions.Insert(index, quest);
                                QuestionList.ItemsSource = null;
                                QuestionList.ItemsSource = Questions;
                            }
                        }
                    }
                    else if (currentObject.GetType() == typeof(ObservableCollection<NameIdModel>))
                        Names = ((ObservableCollection<NameIdModel>)currentObject);
                }
                else
                {
                    Voting = new VotingClass();
                    Questions = new TrulyObservableCollection<VotingQuestionClass>();
                    QuestionList.ItemsSource = Questions;
                    RecipientsList.ItemsSource = Names;
                    progressIndicator = SystemTray.ProgressIndicator;
                    progressIndicator = new ProgressIndicator();
                    SystemTray.SetProgressIndicator(this, progressIndicator);
                    progressIndicator.IsIndeterminate = true;
                    progressIndicator.Text = "Pulling Poll...";
                    if (SettingsMobile.Instance.User == null)
                    {
                        SqlFactory fact = new SqlFactory();
                        SettingsMobile.Instance.User = fact.GetProfile();
                    }
                    Voting.VotingId = Convert.ToInt64(this.NavigationContext.QueryString["pid"]);
                    PullTopic();
                }
                (App.Current as App).SecondPageObject = null;

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
                progressIndicator.Text = "Pulling Poll...";
            });

            Task.Run(new Action(() =>
            {
                try
                {
                    Voting = PollsMobile.GetPoll(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, Voting.VotingId);




                    Dispatcher.BeginInvoke(delegate
                    {
                        Title.Text = Voting.Title;
                        Description.Text = Voting.Description;
                        OpenResults.IsChecked = Voting.IsOpenToLeague;
                        for (int i = 0; i < Voting.Questions.Count; i++)
                            Questions.Add(Voting.Questions[i]);
                        for (int i = 0; i < Voting.Voters.Count; i++)
                        {
                            Names.Add(new NameIdModel() { Id = Voting.Voters[i].MemberId.ToString(), Name = Voting.Voters[i].DerbyName });
                        }

                        progressIndicator.IsVisible = false;
                    });
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                }
            }));


        }







        private void SendPost_Click(object sender, EventArgs e)
        {

            try
            {
                if (String.IsNullOrEmpty(Title.Text))
                {
                    MessageBox.Show("Title Is Empty");
                    return;
                }
                if (Questions.Count == 0)
                {
                    MessageBox.Show("No Questions Added...");
                    return;
                }

                progressIndicator.Text = "Updating Poll...";
                progressIndicator.IsVisible = true;

                Voting.Title = Title.Text;
                Voting.Description = Description.Text;
                Voting.IsOpenToLeague = OpenResults.IsChecked.GetValueOrDefault();
                Voting.Questions = Questions.ToList();


                Voting.Mid = SettingsMobile.Instance.User.MemberId;
                Voting.Uid = SettingsMobile.Instance.User.LoginId;
                Voting.LeagueId = SettingsMobile.Instance.User.CurrentLeagueId.ToString();


                Task.Run(new Action(() =>
                {
                    try
                    {
                        var m = PollsMobile.SendUpdatedPoll(Voting);

                        //(App.Current as App).SecondPageObject = conversationModel;
                        Dispatcher.BeginInvoke(delegate
                        {
                            progressIndicator.IsVisible = false;
                            if (m.IsSuccessful)
                            {

                                ToastPrompt t = new ToastPrompt
                                {
                                    Title = "Poll Updated...",
                                    TextOrientation = System.Windows.Controls.Orientation.Vertical
                                };
                                t.Show();
                                NavigationService.Navigate(new Uri("/View/MyLeague/Polls/Polls.xaml", UriKind.RelativeOrAbsolute));
                            }
                            else
                            {
                                MessageBox.Show("Something went wrong, please check values and try again.");
                            }
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


        private void AddQuestion_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/MyLeague/Polls/QuestionEdit.xaml", UriKind.RelativeOrAbsolute));
        }



        private void QuestionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                if (e.AddedItems.Count > 0)
                {
                    var topic = (VotingQuestionClass)e.AddedItems[0];
                    //topic.ForumId = forumModel.ForumId;
                    (App.Current as App).SecondPageObject = topic;
                    NavigationService.Navigate(new Uri("/View/MyLeague/Polls/QuestionEdit.xaml", UriKind.Relative));

                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }

        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (pivotContainer.SelectedIndex == 1)//questions
                {
                    ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
                    b.IsEnabled = false;
                    b = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
                    b.IsEnabled = true;
                }

                else //main
                {
                    ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
                    b.IsEnabled = false;
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }



        private void QuestionDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = ((MenuItem)sender).DataContext as VotingQuestionClass;
                Questions.Remove(btn);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }


        private void EditQuestion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var topic = ((MenuItem)sender).DataContext as VotingQuestionClass;
                //topic.ForumId = forumModel.ForumId;
                (App.Current as App).SecondPageObject = topic;
                NavigationService.Navigate(new Uri("/View/MyLeague/Polls/QuestionEdit.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }

        }

        private void Close_Click(object sender, EventArgs e)
        {
            progressIndicator.Text = "Closing Poll...";
            progressIndicator.IsVisible = true;
            Voting.Mid = SettingsMobile.Instance.User.MemberId;
            Voting.Uid = SettingsMobile.Instance.User.LoginId;
            Voting.LeagueId = SettingsMobile.Instance.User.CurrentLeagueId.ToString();


            Task.Run(new Action(() =>
            {
                try
                {
                    var m = PollsMobile.ClosePoll(Voting);

                    //(App.Current as App).SecondPageObject = conversationModel;
                    Dispatcher.BeginInvoke(delegate
                    {
                        progressIndicator.IsVisible = false;
                        if (m.IsSuccessful)
                        {
                            ToastPrompt t = new ToastPrompt
                            {
                                Title = "Poll Closed...",
                                TextOrientation = System.Windows.Controls.Orientation.Vertical
                            };
                            t.Show();
                            NavigationService.Navigate(new Uri("/View/MyLeague/Polls/Polls.xaml", UriKind.RelativeOrAbsolute));
                        }
                        else
                        {
                            MessageBox.Show("Something went wrong, please check values and try again.");
                        }
                    });

                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                }
            }));

        }

        private void Delete_Click(object sender, EventArgs e)
        {
            progressIndicator.Text = "Deleting Poll...";
            progressIndicator.IsVisible = true;
            Voting.Mid = SettingsMobile.Instance.User.MemberId;
            Voting.Uid = SettingsMobile.Instance.User.LoginId;
            Voting.LeagueId = SettingsMobile.Instance.User.CurrentLeagueId.ToString();


            Task.Run(new Action(() =>
            {
                try
                {
                    var m = PollsMobile.DeletePoll(Voting);

                    //(App.Current as App).SecondPageObject = conversationModel;
                    Dispatcher.BeginInvoke(delegate
                    {
                        progressIndicator.IsVisible = false;
                        if (m.IsSuccessful)
                        {
                            ToastPrompt t = new ToastPrompt
                            {
                                Title = "Poll Deleted...",
                                TextOrientation = System.Windows.Controls.Orientation.Vertical
                            };
                            t.Show();
                            NavigationService.Navigate(new Uri("/View/MyLeague/Polls/Polls.xaml", UriKind.RelativeOrAbsolute));
                        }
                        else
                        {
                            MessageBox.Show("Something went wrong, please check values and try again.");
                        }
                    });

                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                }
            }));
        }
    }
}