using System;
using System.Collections.Generic;
using RDN.Library.Classes.EmailServer;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.EmailServer.Enums;
using RDN.Library.DataModels.Scoreboard;
using RDN.Utilities.Config;
using RDN.Portable.Config;
using RDN.Portable.Classes.Utilities.Enums;
using RDN.Library.Classes.Config;
using Common.EmailServer.Library.Classes.Subscribe;

namespace RDN.Library.Classes.Scoreboard
{
    public class ScoreboardFeedbackClass
    {
        /// <summary>
        /// inserts new feedback from the scoreboard into the DB.
        /// </summary>
        /// <param name="feedback"></param>
        /// <param name="league"></param>
        /// <param name="email"></param>
        /// <param name="scoreboardId"></param>
        public static void commitScoreboardFeedback(string feedback, string league, string email, string scoreboardId)
        {
            try
            {
                var instance = Scoreboard.getScoreboardId(scoreboardId);

                ManagementContext db = new ManagementContext();

                ScoreboardFeedback fb = new ScoreboardFeedback();
                fb.Created = DateTime.UtcNow;
                fb.Feedback = feedback;
                fb.League = league;
                fb.Email = email;
                SubscriberManager.AddSubscriber(SubscriberType.ScoreboardFeedback, email);
                fb.FeedbackTypeEnum = (byte)FeedbackTypeEnum.None;
                fb.Instance = instance;
                db.ScoreboardFeedback.Add(fb);
                db.SaveChanges();

                string body = "New Feedback Boss:<br/><br/>";
                body += feedback + "<br/><br/>";
                body += "from: " + email + "<br/>";
                body += "instanceId: " + instance.InstanceId + "<br/>";
                body += "instance MAC: " + instance.InstanceMacAddress + "<br/>";
                body += "instance Loads Count: " + instance.LoadsCount + "<br/><br/>";
                body += "url: http://raspberry.rdnation.com/Admin/Feedback<br/>";
                body += "You Da Man!";


                var emailData = new Dictionary<string, string> { { "body", body } };
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailSubject + " New Feedback", emailData, layout: EmailServerLayoutsEnum.Blank, priority: EmailPriority.Normal);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        public static void CommitFeedback(string feedback, string league, string email, FeedbackTypeEnum type)
        {
            try
            {

                ManagementContext db = new ManagementContext();

                ScoreboardFeedback fb = new ScoreboardFeedback();
                fb.Created = DateTime.UtcNow;
                fb.Feedback = feedback;
                fb.League = league;
                SubscriberManager.AddSubscriber(SubscriberType.ScoreboardDownloads, email);
                fb.Email = email;
                fb.FeedbackTypeEnum = (byte)FeedbackTypeEnum.None;
                db.ScoreboardFeedback.Add(fb);
                db.SaveChanges();

                string body = "New Feedback Boss:<br/><br/>";
                body += feedback + "<br/><br/>";
                body += "from: " + email + "<br/>";
                body += "url: http://raspberry.rdnation.com/Admin/Feedback<br/>";
                body += "You Da Man!";


                var emailData = new Dictionary<string, string> { { "body", body } };
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailSubject + " New Feedback", emailData, layout: EmailServerLayoutsEnum.Blank, priority: EmailPriority.Normal);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
    }
}
