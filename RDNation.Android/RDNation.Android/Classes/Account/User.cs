    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RDN.Mobile.Account;
using RDNation.Droid.Classes.Configuration;
using RDN.Portable.Account;
using RDN.Portable.Config.Enums;

namespace RDNation.Droid.Classes.Account
{
    public class User
    {


        public static bool Authenticate(UserMobile user, Context context)
        {

            var connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            var activeConnection = connectivityManager.ActiveNetworkInfo;
            if ((activeConnection != null) && activeConnection.IsConnected)
            {
                try
                {
                    var u = UserMobileMb.Login(user);
                    Settings.SaveUserPreferences(context, u);
                    return u.IsLoggedIn;
                }
                catch (Exception ex)
                {
                    ErrorHandler.Save(ex, MobileTypeEnum.Android, context);
                }
            }
            return false;

        }
        public static UserMobile SignUp(UserMobile user, Context context)
        {
            var connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            var activeConnection = connectivityManager.ActiveNetworkInfo;
            if ((activeConnection != null) && activeConnection.IsConnected)
            {
                try
                {
                    var u = UserMobileMb.SignUp(user);
                    Settings.SaveUserPreferences(context, u);
                    return u;
                }
                catch (Exception ex)
                {
                    ErrorHandler.Save(ex, MobileTypeEnum.Android, context);
                }
            }
            return user;

        }

    }
}