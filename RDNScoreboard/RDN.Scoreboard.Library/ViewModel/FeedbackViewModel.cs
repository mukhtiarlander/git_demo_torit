using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scoreboard.Library.Static;
using System.Threading.Tasks;
using System.IO;
using System.Net;

using System.Xml;
using System.Threading;
using System.Xml.Serialization;

using RDN.Utilities.Version;
using RDN.Utilities.Config;
using Scoreboard.Library.Util;
using RDN.Utilities.Util;

namespace Scoreboard.Library.ViewModel
{
    public class FeedbackViewModel
    {
        public string ScoreboardMacId { get; set; }
        public DateTime DateTime { get; set; }
        public string League { get; set; }
        public string Email { get; set; }
        public string Feedback { get; set; }



        public FeedbackViewModel()
        { }

        public FeedbackViewModel(string feedback, string league, string email)
        {
            this.Feedback = feedback;
            this.League = league;
            this.Email = email;
            this.DateTime = DateTime.UtcNow;
            this.ScoreboardMacId = Network.Client.GetMacAddress();
        }





        public static FeedbackViewModel deserialize(Stream stream)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(FeedbackViewModel));
            FeedbackViewModel feedback = (FeedbackViewModel)mySerializer.Deserialize(stream);
            return feedback;
        }


        /// <summary>
        /// saves any error created by the software.
        /// </summary>
        /// <param name="e"></param>
        public static void saveFeedback(FeedbackViewModel feedback)
        {
            Task<bool>.Factory.StartNew(
                          () =>
                          {
                              DirectoryInfo dir = new DirectoryInfo(ScoreboardConfig.SAVE_FEEDBACK_FOLDER);
                              if (!dir.Exists)
                                  dir.Create();

                              System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(feedback.GetType());
                              string filePath = ScoreboardConfig.SAVE_FEEDBACK_FOLDER + "Feedback" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".xml";
                              string filePathEncrypted = ScoreboardConfig.SAVE_FEEDBACK_FOLDER + "Feedback" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".xml";
                              System.IO.StreamWriter file = new System.IO.StreamWriter(filePath);

                              writer.Serialize(file, feedback);
                              file.Close();
                              file.Dispose();

                              Encryption.EncryptFiletoFile(filePath, filePathEncrypted);
                              string filePathCompressed = Compression.Compress(new FileInfo(filePathEncrypted));

                              bool isUploaded = uploadFeedbackToServer(filePathCompressed);
                              if (isUploaded)
                              {
                                  Thread.Sleep(2000);
                                  FileInfo fileInfo = new FileInfo(filePathCompressed);
                                  if (fileInfo.Exists)
                                      fileInfo.Delete();

                              }

                              FileInfo file1 = new FileInfo(filePathEncrypted);
                              if (file1.Exists)
                                  file1.Delete();
                              FileInfo file2 = new FileInfo(filePath);
                              if (file2.Exists)
                                  file2.Delete();
                              return true;
                          });

        }
        /// <summary>
        /// uploads the error to the server.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool uploadFeedbackToServer(string filePath)
        {
            if (!Network.Internet.CheckConnection())
                return false;

            WebClient client = new WebClient();
            try
            {
                client.UploadFile(new Uri(ScoreboardConfig.FEEDBACK_URL), filePath);
                return true;
            }
            catch
            { }
            return false;
        }


        /// <summary>
        /// checks the errors folder for old errors so that they may be delivered to the server.
        /// </summary>
        public static void checkForOldFeedbacks()
        {
            //DirectoryInfo dir1 = new DirectoryInfo(@"C:\Documents and Settings\pioh");
            //FileInfo[] filesss = dir1.GetFiles();

            Task<bool>.Factory.StartNew(
                          () =>
                          {
                              DirectoryInfo dir = new DirectoryInfo(ScoreboardConfig.SAVE_FEEDBACK_FOLDER);

                              if (dir.Exists)
                              {
                                  FileInfo[] files = dir.GetFiles();
                                  foreach (var file in files)
                                  {
                                      bool isUploaded = uploadFeedbackToServer(file.FullName);
                                      Thread.Sleep(5000);
                                      try
                                      {
                                          if (isUploaded)
                                              file.Delete();
                                      }
                                      catch (Exception e)
                                      {
                                          ErrorViewModel.Save(e, e.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
                                      }
                                  }
                              }
                              return true;
                          });
        }
    }
}
