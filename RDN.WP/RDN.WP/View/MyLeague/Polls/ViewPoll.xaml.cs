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
    public partial class ViewPoll : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        VotingClass Voting;
        public ViewPoll()
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
                        IsAnonymous.Text = Voting.IsPollAnonymous.ToString();
                        RecipientsList.ItemsSource = Voting.Voters;
                        for (int i = 0; i < Voting.Questions.Count; i++)
                        {
                            PivotItem p = new PivotItem();
                            p.Header = "question " + (i + 1);
                            ScrollViewer sv = new ScrollViewer();
                            StackPanel sp = new StackPanel();
                            TextBlock question = new TextBlock();
                            question.Text = Voting.Questions[i].Question;
                            question.FontSize = 30;
                            question.Margin = new Thickness(0, 0, 0, 15);
                            question.TextWrapping = TextWrapping.Wrap;
                            if (Voting.Questions[i].QuestionType == QuestionTypeEnum.Multiple)
                            {
                                question.Text += " - MC";
                            }
                            sp.Children.Add(question);


                            foreach (var answer in Voting.Questions[i].Answers)
                            {
                                TextBlock tbAnswer = new TextBlock();
                                tbAnswer.Text = answer.Answer;
                                tbAnswer.FontSize = 25;
                                tbAnswer.TextWrapping = TextWrapping.Wrap;
                                sp.Children.Add(tbAnswer);

                                StackPanel answerStack = new StackPanel();
                                answerStack.Margin = new Thickness(15, 0, 0, 10);
                                TextBlock tbVotesCount = new TextBlock();
                                tbVotesCount.Text = Voting.Questions[i].Votes.Where(x => x.AnswerIds.Contains(answer.AnswerId)).Count() + " vote(s) " + Voting.Questions[i].GetPercentage(answer.AnswerId) + "%";
                                answerStack.Children.Add(tbVotesCount);

                                switch (Voting.IsPollAnonymous)
                                {
                                    case true:
                                        foreach (var vote in Voting.Questions[i].Votes.Where(x => x.AnswerIds.Contains(answer.AnswerId)).OrderBy(x => x.DerbyName))
                                        {
                                            if (!String.IsNullOrEmpty(vote.OtherText))
                                            {
                                                TextBlock comment = new TextBlock();
                                                comment.Text =" - "+ vote.OtherText;
                                                comment.FontSize = 20;
                                                comment.Margin = new Thickness(10, 0, 0, 10);
                                                comment.TextWrapping = TextWrapping.Wrap;
                                                answerStack.Children.Add(comment);
                                            }
                                        }
                                        break;
                                    case false:
                                        foreach (var vote in Voting.Questions[i].Votes.Where(x => x.AnswerIds.Contains(answer.AnswerId)).OrderBy(x => x.DerbyName))
                                        {
                                            TextBlock name = new TextBlock();
                                            name.Text = vote.DerbyName;
                                            answerStack.Children.Add(name);
                                            if (!String.IsNullOrEmpty(vote.OtherText))
                                            {
                                                TextBlock comment = new TextBlock();
                                                comment.Text = " - " + vote.OtherText;
                                                comment.FontSize = 20;
                                                comment.Margin = new Thickness(10, 0, 0, 10);
                                                comment.TextWrapping = TextWrapping.Wrap;
                                                answerStack.Children.Add(comment);
                                            }
                                        }
                                        break;
                                }
                                sp.Children.Add(answerStack);
                                ProgressBar bar = new ProgressBar();
                                bar.Margin = new Thickness(0, 0, 0, 15);
                                bar.Maximum = Voting.Questions[i].Votes.Count;
                                bar.Minimum = 0;
                                bar.Value = Voting.Questions[i].GetPercentage(answer.AnswerId);
                                sp.Children.Add(bar);

                            }
                            sv.Content = sp;
                            p.Content = sv;

                            Pivot.Items.Insert(i + 1, p);
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


       
    }
}