using Facebook;
using RDN.Library.Classes.Facebook;
using RDN.Library.Classes.Facebook.Enum;
using RDN.Utilities.Dates;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using TweetSharp;

namespace RDN.Library.Classes.Social.Twitter
{
    public class TwitterFactory
    {
        private string ScoresMessage = String.Empty;
        private TwitterService Service = null;
        public TwitterSearchResult Result;
        public TwitterStatus Status;
        //Service = new TwitterService("qtmKXlKuyrsXEBNwc6Q3Q", "1JOZnJxS6FvvhLVAudE8nKrntIML6PSPfybMdqzn5UY");
        //    Service.AuthenticateWith("213102432-8pKL9OEjRCaRNgcqhqe9xiQpz9xpW2uqfUW3CSBW", "aF18OXxk3JnOHKqWQNYn6JwvUdIdNpMYRSj5pxGaM");
        public static TwitterFactory Initialize(string consumerKey, string consumerSecret, string token, string tokenSecret)
        {
            var fact = new TwitterFactory();
           fact. Result = new TwitterSearchResult();
           fact. Service = new TwitterService(consumerKey, consumerSecret);
            fact.Service.AuthenticateWith(token, tokenSecret);
            return fact;
        }

        public TwitterFactory SearchHashTag(string tag, int count)
        {
            SearchOptions opt = new SearchOptions();
            opt.Count = count;
            opt.Q = "#derbyscores";
            Result = Service.Search(opt);
            foreach (var item in Result.Statuses)
            {
                item.TextAsHtml = item.TextAsHtml.Replace("\n", "").Replace("\"", "");
            }
            return this;
        }
        public TwitterFactory AddScores(string teamName1, string team1Score, string team2Name, string team2Score)
        {
            this.ScoresMessage =  teamName1 + " " + team1Score + " - " + team2Score + " " + team2Name +" ";
            return this;
        }
        public TwitterFactory SendMessage(string message)
        {
            var options = new SendTweetOptions();
            options.Status = ScoresMessage + message;
            
            Status = Service.SendTweet(options);
            return this;
        }

    }
}
