using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Utilities.Util
{
    public class LoggerMobile
    {

        /// <summary>
        /// Summary description for Logger.
        /// </summary>
      
            public List<Log.Log> Log { get; set; }

            static LoggerMobile instance = new LoggerMobile();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static LoggerMobile()
            { }


            public static LoggerMobile Instance
            {
                get
                {
                    return instance;
                }
            }

            private LoggerMobile()
            {

            }

            public string getLoggedMessages()
            {
                if (instance.Log != null)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < instance.Log.Count; i++)
                    {
                        sb.Append(instance.Log[i].LogType.ToString() + ":" + instance.Log[i].LogMessage + "|");
                    }
                    return sb.ToString();
                }
                return String.Empty;
            }

            public void deleteOldLogs()
            {
                instance.Log = null;
            }



            public void logMessage(string s, RDN.Utilities.Util.LoggerEnum severity)
            {

                try
                {
                    if (instance.Log == null)
                        instance.Log = new List<Util.Log.Log>();

                    instance.Log.Add(new Log.Log() { LogMessage = s, LogType = severity });


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, new object[0]);
                }
            }

        }
    }