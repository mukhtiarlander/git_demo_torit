using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.Portable.Classes.Voting;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using System.Collections.ObjectModel;
using RDN.Portable.Classes.Voting.Enums;

namespace RDN.WP.View.MyLeague.Polls
{
    public partial class QuestionEdit : PhoneApplicationPage
    {
        string questionText = "What Are You Asking?";
        string answerText = "My Favorite Color Is Blue";
        ObservableCollection<VotingAnswersClass> Answers;
        VotingQuestionClass question;
        public QuestionEdit()
        {
            InitializeComponent();

            try
            {
                Answers = new ObservableCollection<VotingAnswersClass>();
                if ((App.Current as App).SecondPageObject != null)
                {
                    question = ((VotingQuestionClass)(App.Current as App).SecondPageObject);
                    if (question.QuestionType == QuestionTypeEnum.Multiple)
                        IsMultipleChoice.IsChecked = true;
                    Question.Text = question.Question;
                    foreach (var answer in question.Answers)
                        Answers.Add(new VotingAnswersClass() { Answer = answer.Answer, AnswerId = answer.AnswerId });
                }
                else
                {
                    question = new VotingQuestionClass();
                    Answers.Add(new VotingAnswersClass() { Answer = answerText });
                    Answers.Add(new VotingAnswersClass());
                    Answers.Add(new VotingAnswersClass());

                }
                (App.Current as App).SecondPageObject = null;

                AnswerList.ItemsSource = Answers;
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }



        private void Question_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Question.Text.Equals(questionText, StringComparison.OrdinalIgnoreCase))
            {
                Question.Text = string.Empty;
            }

        }

        private void Question_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Question.Text))
            {
                Question.Text = questionText;
            }
        }

        private void Answer_GotFocus(object sender, RoutedEventArgs e)
        {
            if (((TextBox)sender).Text.Equals(answerText, StringComparison.OrdinalIgnoreCase))
            {
                ((TextBox)sender).Text = string.Empty;
            }
        }

        private void Answer_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void Done_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            question.Question = Question.Text;
            if (question.QuestionId == 0)
                question.QuestionId = r.Next();
            if (IsMultipleChoice.IsChecked.GetValueOrDefault())
                question.QuestionType = QuestionTypeEnum.Multiple;
            else
                question.QuestionType = QuestionTypeEnum.Single;

            //foreach(var item in AnswerList.i)
            question.Answers = Answers.ToList();


            (App.Current as App).SecondPageObject = question;
            NavigationService.GoBack();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AddAnswer_Click(object sender, RoutedEventArgs e)
        {
            Answers.Add(new VotingAnswersClass());
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            question.IsDeleted = true;
            (App.Current as App).SecondPageObject = question;
            NavigationService.GoBack();
        }

        private void AnswerDelete_Click(object sender, RoutedEventArgs e)
        {
            var btn = ((Button)sender).DataContext as VotingAnswersClass;
            Answers.Remove(btn);
        }

    }
}