using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Config
{
    public class ScoreboardConfig
    {
        public static string WEBSITE_PING_LOCATION
        {
            get
            {
                return ConfigurationManager.AppSettings["WEBSITE_PING_LOCATION"];
            }
        }
    }
}
