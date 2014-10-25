using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RDN.Utilities.Config;

namespace RDN.Utilities.Util
{
    public enum LoggerEnum { error, message }
        /// <summary>
        /// Summary description for Logger.
        /// </summary>
        public class Logger
        {

            private System.IO.StreamWriter _Output = null;
            private string _logFile = ScoreboardConfig.LOG_SCOREBOARD_FOLDER + "Log" + DateTime.UtcNow.ToString("yyyyMMdd") + ".txt";

            static Logger instance = new Logger();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Logger()
            { }


            public static Logger Instance
            {
                get
                {
                    return instance;
                }
            }

            private Logger()
            {

            }

            public string getLoggedMessages()
            {
                FileInfo file = new FileInfo(_logFile);
                try
                {
                    if (file.Exists)
                    {
                        System.IO.StreamReader reader = new System.IO.StreamReader(_logFile);
                        return reader.ReadToEnd();
                    } return string.Empty;

                }
                catch
                {
                    try
                    {
                        if (file.Exists)
                            file.Delete();
                    }
                    catch { }
                    return string.Empty;
                }
            }

            public void deleteOldLogs()
            {
                DirectoryInfo dir = new DirectoryInfo(ScoreboardConfig.LOG_SCOREBOARD_FOLDER);
                if (dir.Exists)
                {
                    var files = dir.GetFiles();
                    for (int i = 0; i < files.Count(); i++)
                    {
                        if (files[i].FullName.ToLower() != _logFile.ToLower())
                        {
                            try
                            {
                                FileInfo file = new FileInfo(files[i].FullName);
                                file.Delete();
                            }
                            catch { }
                        }
                    }


                }


            }



            public void logMessage(string s, LoggerEnum severity)
            {

                try
                {
                    Console.WriteLine(s);
                    if (_Output == null)
                    {
                        _Output = new System.IO.StreamWriter(_logFile, true, System.Text.UnicodeEncoding.Default);
                    }

                    _Output.WriteLine(System.DateTime.Now + "\t" + severity.ToString() + "\t" + s + Environment.NewLine, new object[0]);

                    if (_Output != null)
                    {
                        _Output.Close();
                        _Output = null;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, new object[0]);
                }
            }

            public void closeLog()
            {
                try
                {
                    if (_Output != null)
                    {
                        _Output.Close();
                        _Output = null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, new object[0]);
                }
            }
        }
    }
