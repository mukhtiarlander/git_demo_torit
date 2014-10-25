using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RDN.Mobile.Account;
using RDN.Mobile.Classes.Utilities;
using RDN.Portable.Account;
using RDN.Portable.Settings;

namespace RDNation.Droid.Classes.Configuration
{
    public class Settings
    {
        private static string RDNationSettingsString = "RDNationSettings";
        private static string UserMobileKey = "UserMobile";

        public static UserMobile GetUserPreferences(Context context)
        {
            if (SettingsMobile.Instance.User == null)
            {
                // this is an Activity
                ISharedPreferences prefs = context.GetSharedPreferences(RDNationSettingsString, FileCreationMode.WorldReadable);

                //ISharedPreferencesEditor editor = prefs.Edit();
                var s = prefs.GetString(UserMobileKey, String.Empty);
                if (!String.IsNullOrEmpty(s))
                {
                    SettingsMobile.Instance.User = Json.DeserializeObject<UserMobile>(s);
                }
                else
                {
                    SettingsMobile.Instance.User = new UserMobile() { IsLoggedIn = false };
                }
                //                editor.PutInt(("number_of_times_accessed", accessCount++);
                //editor.PutString("date_last_accessed", DateTime.Now.ToString("yyyy-MMM-dd"));
                //editor.Apply();
            }
            return SettingsMobile.Instance.User;
        }
        public static UserMobile SaveUserPreferences(Context context, UserMobile user)
        {
            SettingsMobile.Instance.User = user;
            ISharedPreferences prefs = context.GetSharedPreferences(RDNationSettingsString, FileCreationMode.WorldReadable);
            ISharedPreferencesEditor editor = prefs.Edit();
            string s = Json.ConvertToString<UserMobile>(user);
            editor.PutString(UserMobileKey, s);
            editor.Commit();
            return user;
        }



    }
}