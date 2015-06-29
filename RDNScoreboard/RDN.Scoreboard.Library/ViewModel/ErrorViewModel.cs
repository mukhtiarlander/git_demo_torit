using System;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml.Serialization;

using RDN.Utilities.Error;
using System.Linq.Expressions;
using System.Collections.Generic;
using RDN.Utilities.Version;
using RDN.Utilities.Config;
using RDN.Portable.Config;

namespace Scoreboard.Library.ViewModel
{
    public class ErrorViewModel
    {
        public string ScoreboardId { get; set; }
        public DateTime DateTime { get; set; }
        public Guid GameId { get; set; }
        public string ExtraContent { get; set; }
        public string ExceptionMessege { get; set; }
        public string ExceptionSource { get; set; }
        public string ExceptionInner { get; set; }
        public string ExceptionStack { get; set; }
        public string Version { get; set; }
        public string Error_Target { get; set; }
        public string Error_Trace { get; set; }
        public string Last_Exception { get; set; }
        public string Log { get; set; }


        public ErrorViewModel()
        { }


        public static void Save(Exception e, Type type, ErrorGroupEnum? errorGroup = null, ErrorSeverityEnum? errorSeverity = null, IList<Expression<Func<object>>> parameters = null, string additionalInformation = null)
        {
            Task<bool>.Factory.StartNew(
                                    () =>
                                    {
                                        try
                                        {
                                            string dt = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                                            //we save the error to the computer so that its backed up before we send it off to the server.
                                            ErrorManager.SaveErrorObject(e, type, ScoreboardConfig.SAVE_ERRORS_FOLDER + "Error" + dt + ".data", errorGroup, errorSeverity, parameters, ScoreboardConfig.SCOREBOARD_VERSION_NUMBER + ": " + additionalInformation);
                                            Stream stream;
                                            ErrorManager.LoadErrorObject(out stream, ScoreboardConfig.SAVE_ERRORS_FOLDER + "Error" + dt + ".data");
                                            if (Network.Internet.CheckConnection())
                                            {
                                                try
                                                {
                                                    System.Net.HttpStatusCode response;
                                                    response = RDN.Utilities.Network.Network.SendPackage(stream, ScoreboardConfig.ERROR_SUBMIT_URL);

                                                    if (response == HttpStatusCode.OK)
                                                    {
                                                        FileInfo file = new FileInfo(ScoreboardConfig.SAVE_ERRORS_FOLDER + "Error" + dt + ".data");
                                                        file.Delete();
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    var d = ex;
                                                }
                                            }
                                        }
                                        catch
                                        {

                                        }
                                        return true;
                                    });
        }



        /// <summary>
        /// checks the errors folder for old errors so that they may be delivered to the server.
        /// </summary>
        public static void checkForOldErrors()
        {

            Task<bool>.Factory.StartNew(
                          () =>
                          {
                              try
                              {
                                  DirectoryInfo dir = new DirectoryInfo(ScoreboardConfig.SAVE_ERRORS_FOLDER);
                                  bool isUploaded = false;
                                  if (dir.Exists)
                                  {
                                      FileInfo[] files = dir.GetFiles();
                                      foreach (var file in files)
                                      {
                                          Stream stream;
                                          ErrorManager.LoadErrorObject(out stream, file.FullName);
                                          if (Network.Internet.CheckConnection())
                                          {
                                              RDN.Utilities.Network.Network.SendPackage(stream, ScoreboardConfig.UPLOAD_ERRORS_URL);

                                              isUploaded = true;
                                          }
                                          Thread.Sleep(5000);
                                          try
                                          {
                                              if (isUploaded)
                                                  file.Delete();
                                          }
                                          catch (Exception e)
                                          {
                                              Save(e, e.GetType());
                                          }
                                      }
                                  }
                              }
                              catch (Exception e)
                              {
                                  Save(e, e.GetType());
                              }
                              return true;
                          });
        }
    }
}
