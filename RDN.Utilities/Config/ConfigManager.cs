using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Utilities.Config
{
    public class ConfigManager 
    {
        public string Get(string key)
        {
            return GetConfigString(key);
        }

        private static string GetConfigString(string key)
        {
            var cleanedKey = key.Replace("get_", "");
            return System.Configuration.ConfigurationManager.AppSettings[cleanedKey];
        }

        public string ProductionCalllistTableName
        {
            get { return Get("ProductionCallListTable"); }
        }

      
    }
}
