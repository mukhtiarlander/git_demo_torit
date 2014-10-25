using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.Portable.Settings;
using RDN.WP.Library.Database;
using RDN.Portable.Classes.Voting;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.WP.Library.Classes.League;
using System.Threading.Tasks;
using RDN.Portable.Classes.Voting.Enums;
using Coding4Fun.Toolkit.Controls;

namespace RDN.WP.View.MyLeague.Polls
{
    public partial class VotePoll : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        VotingClass Voting;
        public VotePoll()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                Voting = new VotingClass();
                //(App.Current as App).SecondPageObject = null;
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
                        Pivot.Title = Voting.Title;
                        description.Text = Voting.Description;
                        for (int i = 0; i < Voting.Questions.Count; i++)
                        {
                            PivotItem p = new PivotItem();
                            p.Header = "Question " + (i + 1);
                            StackPanel sp = new StackPanel();
                            TextBlock question = new TextBlock();
                            question.Text = Voting.Questions[i].Question;
                            question.FontSize = 30;
                            question.TextWrapping = TextWrapping.Wrap;

                            sp.Children.Add(question);

                            if (Voting.Questions[i].QuestionType == QuestionTypeEnum.Multiple)
                            {
                                foreach (var answer in Voting.Questions[i].Answers)
                                {
                                    CheckBox cb = new CheckBox();
                                    cb.Name = Voting.Questions[i].QuestionId + "-" + answer.AnswerId;
                                    cb.Checked += cb_Checked;
                                    cb.Unchecked += cb_Unchecked;
                                    TextBlock tb = new TextBlock();
                                    tb.Text = answer.Answer;
                                    cb.Content = tb;
                                    sp.Children.Add(cb);
                                }
                            }
                            else if (Voting.Questions[i].QuestionType == QuestionTypeEnum.Single)
                            {
                                foreach (var answer in Voting.Questions[i].Answers)
                                {
                                    RadioButton cb = new RadioButton();
                                    cb.Checked += rb_Checked;
                                    cb.GroupName = "g-" + Voting.Questions[i].QuestionId;
                                    cb.Name = Voting.Questions[i].QuestionId + "-" + answer.AnswerId;
                                    TextBlock tb = new TextBlock();
                                    tb.Text = answer.Answer;
                                    cb.Content = tb;
                                    sp.Children.Add(cb);
                                }
                            }
                            TextBlock comment = new TextBlock();
                            comment.Text = "Comment:";
                            TextBox commentTb = new TextBox();
                            commentTb.Name = "c-" + Voting.Questions[i].QuestionId;
                            commentTb.LostFocus += commentTb_LostFocus;
                            sp.Children.Add(comment);
                            sp.Children.Add(commentTb);

                            p.Content = sp;
                            Pivot.Items.Add(p);
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

        void commentTb_LostFocus(object sender, RoutedEventArgs e)
        {
            var currentCheckBoxItem = sender as TextBox;
            var text = currentCheckBoxItem.Text;
            if (!String.IsNullOrEmpty(currentCheckBoxItem.Text))
            {
                string name = currentCheckBoxItem.Name;

                var questionAnswer = name.Split('-');
                long qId = Convert.ToInt64(questionAnswer[1]);

                Task.Run(new Action(() =>
                {
                    try
                    {
                        var question = Voting.Questions.Where(x => x.QuestionId == qId).FirstOrDefault();
                        var vote = question.Votes.FirstOrDefault();
                        if (vote != null)
                        {
                            vote.OtherText = text;
                        }
                        else
                        {
                            vote = new VotesClass();
                            vote.OtherText = text;
                            question.Votes.Add(vote);
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                    }
                }));

            }

        }

        void cb_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentCheckBoxItem = sender as CheckBox;
                string name = currentCheckBoxItem.Name;

                var questionAnswer = name.Split('-');
                long qId = Convert.ToInt64(questionAnswer[0]);
                long aId = Convert.ToInt64(questionAnswer[1]);

                if (currentCheckBoxItem.IsChecked == false)
                {
                    Task.Run(new Action(() =>
                    {
                        try
                        {
                            var question = Voting.Questions.Where(x => x.QuestionId == qId).FirstOrDefault();
                            var vote = question.Votes.FirstOrDefault();
                            if (vote != null)
                            {
                                vote.AnswerIds.Remove(aId);
                            }
                        }
                        catch (Exception exception)
                        {
                            ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                        }
                    }));
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        void cb_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentCheckBoxItem = sender as CheckBox;
                string name = currentCheckBoxItem.Name;

                var questionAnswer = name.Split('-');
                long qId = Convert.ToInt64(questionAnswer[0]);
                long aId = Convert.ToInt64(questionAnswer[1]);

                if (currentCheckBoxItem.IsChecked == true)
                {
                    Task.Run(new Action(() =>
                    {
                        try
                        {
                            var question = Voting.Questions.Where(x => x.QuestionId == qId).FirstOrDefault();
                            var vote = question.Votes.FirstOrDefault();
                            if (vote == null)
                            {
                                vote = new VotesClass();
                                vote.AnswerIds.Add(aId);
                                question.Votes.Add(vote);
                            }
                            else
                            {
                                vote.AnswerIds.Add(aId);
                            }
                        }
                        catch (Exception exception)
                        {
                            ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                        }
                    }));

                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        void rb_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentCheckBoxItem = sender as RadioButton;
                string name = currentCheckBoxItem.Name;

                var questionAnswer = name.Split('-');
                long qId = Convert.ToInt64(questionAnswer[0]);
                long aId = Convert.ToInt64(questionAnswer[1]);

                if (currentCheckBoxItem.IsChecked == true)
                {
                    Task.Run(new Action(() =>
                    {
                        try
                        {
                            var question = Voting.Questions.Where(x => x.QuestionId == qId).FirstOrDefault();
                            var vote = question.Votes.FirstOrDefault();
                            if (vote == null)
                            {
                                vote = new VotesClass();
                                vote.AnswerIds.Add(aId);
                                question.Votes.Add(vote);
                            }
                            else
                            {
                                vote.AnswerIds.Clear();
                                vote.AnswerIds.Add(aId);
                            }
                        }
                        catch (Exception exception)
                        {
                            ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                        }
                    }));

                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {

                progressIndicator.Text = "Voting...";
                progressIndicator.IsVisible = true;
                Voting.Mid = SettingsMobile.Instance.User.MemberId;
                Voting.Uid = SettingsMobile.Instance.User.LoginId;
                Voting.LeagueId = SettingsMobile.Instance.User.CurrentLeagueId.ToString();


                Task.Run(new Action(() =>
                {
                    try
                    {
                        var m = PollsMobile.VoteOnPoll(Voting);
                        if (m.IsSuccessful)
                        {
                            Dispatcher.BeginInvoke(delegate
                            {
                                progressIndicator.IsVisible = false;
                                ToastPrompt t = new ToastPrompt
                                {
                                    Title = "Successfully Voted...",
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

        private void Cancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

    }
}