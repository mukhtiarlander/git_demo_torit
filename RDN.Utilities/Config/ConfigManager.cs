using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Utilities.Config
{
    public class ConfigManager
    {
        private static string Get(string key)
        {
            return GetConfigString(key);
        }

        private static string GetConfigString(string key)
        {
            var cleanedKey = key.Replace("get_", "");
            return System.Configuration.ConfigurationManager.AppSettings[cleanedKey];
        }

        public static string WebsiteShortName
        {
            get { return Get("WebsiteShortName"); }
        }
        public static string PrivacyUrl
        {
            get { return Get("PrivacyUrl"); }
        }
        public static string InternalSite
        {
            get { return Get("InternalSite"); }
        }

        public static string NameOfMembers
        {
            get { return Get("NameOfMembers"); }
        }
    }
}
