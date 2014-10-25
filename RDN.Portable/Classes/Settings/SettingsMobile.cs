using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RDN.Portable.Account;
using RDN.Portable.Classes.Account;

namespace RDN.Portable.Settings
{
    public class SettingsMobile
    {

        public UserMobile User { get; set; }

        public AccountSettingsModel AccountSettings { get; set; }

        public bool IsSentGameNotifications { get; set; }
        public Guid CurrentForumId { get; set; }

        static SettingsMobile instance = new SettingsMobile();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static SettingsMobile()
        {

        }

        public static SettingsMobile Instance
        {
            get
            {
                return instance;
            }
        }



    }
}



