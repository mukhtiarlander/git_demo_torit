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

namespace RDN.WP.View.MyLeague.Polls
{
    public partial class CreatePoll : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        ObservableCollection<NameIdModel> Names;
        VotingClass Voting;
        TrulyObservableCollection<VotingQuestionClass> Questions;

        public CreatePoll()
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
                (App.Current as App).SecondPageObject = null;
                //Dispatcher.BeginInvoke(delegate
                //{
                //    NamesOfSending.Text = String.Empty;
                //    for (int i = 0; i < Names.Count; i++)
                //        NamesOfSending.Text += Names[i].Name + ", ";

                //});
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void Title_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Title.Text.Equals("Title Of Poll...", StringComparison.OrdinalIgnoreCase))
            {
                Title.Text = string.Empty;
            }
        }

        private void Title_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Title.Text))
            {
                Title.Text = "Title Of Poll...";
            }

        }

        private void Description_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Description.Text.Equals("Description of Poll...", StringComparison.OrdinalIgnoreCase))
            {
                Description.Text = string.Empty;
            }
        }

        private void Description_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Description.Text))
            {
                Description.Text = "Description of Poll...";
            }
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
                if (Names.Count == 0)
                {
                    MessageBox.Show("No Members Added...");
                    return;
                }
                progressIndicator.Text = "Creating Poll...";
                progressIndicator.IsVisible = true;

                Voting.Title = Title.Text;
                Voting.Description = Description.Text;
                Voting.IsOpenToLeague = OpenResults.IsChecked.GetValueOrDefault();
                Voting.IsPollAnonymous = IsAnonymous.IsChecked.GetValueOrDefault();
                Voting.BroadcastPoll = BroadcastPoll.IsChecked.GetValueOrDefault();
                Voting.Questions = Questions.ToList();

                for (int i = 0; i < Names.Count; i++)
                {
                    Guid tempId = new Guid();
                    if (Guid.TryParse(Names[i].Id, out tempId))
                        Voting.Voters.Add(new MemberDisplayBasic() { DerbyName = Names[i].Name, MemberId = tempId });
                }
                Voting.Mid = SettingsMobile.Instance.User.MemberId;
                Voting.Uid = SettingsMobile.Instance.User.LoginId;
                Voting.LeagueId = SettingsMobile.Instance.User.CurrentLeagueId.ToString();


                Task.Run(new Action(() =>
                {
                    try
                    {
                        var m = PollsMobile.SendNewPoll(Voting);
                        if (m.IsSuccessful)
                        {
                         
                            //(App.Current as App).SecondPageObject = conversationModel;
                            Dispatcher.BeginInvoke(delegate
                            {
                                progressIndicator.IsVisible = false;
                                ToastPrompt t = new ToastPrompt
                                {
                                    Title = "Poll Created...",
                                    TextOrientation = System.Windows.Controls.Orientation.Vertical
                                                                    };
                                t.Show();
                                NavigationService.Navigate(new Uri("/View/MyLeague/Polls/Polls.xaml", UriKind.RelativeOrAbsolute));
                            });
                        }
                        else
                        {
                            MessageBox.Show("Something went wrong, please check values and try again.");
                        }
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

        private void AddMembers_Click(object sender, EventArgs e)
        {
            (App.Current as App).SecondPageObject = Names;
            NavigationService.Navigate(new Uri("/View/MyLeague/Generic/MemberGroupChooser.xaml?type=CreatePoll&g=off", UriKind.RelativeOrAbsolute));
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
                    ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
                    b.IsEnabled = false;
                    b = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
                    b.IsEnabled = true;
                }
                else if (pivotContainer.SelectedIndex == 2)//members
                {
                    ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
                    b.IsEnabled = false;
                    b = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
                    b.IsEnabled = true;
                }
                else //main
                {
                    ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
                    b.IsEnabled = false;
                    b = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
                    b.IsEnabled = false;
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
    }
}