using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace Common.CordovaModules
{
    public class CordovaHelper
    {
        private static volatile CordovaHelper _instance = null;

        private const string DefaultCordovaScriptsFolder = "~/CordovaScripts";
        private const string DefaultPlatformCookieKey = "cdva_platfrm";
        private readonly List<string> _platformList;

        public static CordovaHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(CordovaHelper))
                    {
                        _instance = new CordovaHelper();
                    }
                }
                return _instance;
            }
        }

        public CordovaHelper()
        {
            CordovaScriptsPath = DefaultCordovaScriptsFolder;
            PlatformCookieKey = DefaultPlatformCookieKey;
            _platformList = new List<string>() { "ios", "android", "windows" };
        }

        public string CordovaScriptsPath { get; set; }

        public string PlatformCookieKey { get; set; }

        public IHtmlString GetCordovaScript(string platform)
        {
            try
            {
                string cordovaPath = "", cordovaAppPath = "";
                if (_platformList.Any(p => p == platform))
                {
                    cordovaPath = CordovaScriptsPath+ "/cordovadist/" + platform + "/cordova.js";
                    cordovaAppPath = CordovaScriptsPath + "/cordovaApp/app.js";
                    //cordovaPluginsPath = CordovaScriptsPath + "/cordovadist/" + platform + "/cordova_plugins.js";
                }
                return Scripts.Render(cordovaPath, cordovaAppPath);
            }
            catch(Exception ex)
            {
                return null;
            }
        }


        public IHtmlString GetCordovaScript(HttpRequestBase request)
        {
            try
            {
                var platform = request.Cookies.Get(PlatformCookieKey).Value;
                return GetCordovaScript(platform);
            }
            catch
            {
                return MvcHtmlString.Empty;
            }
        }
    }
}
