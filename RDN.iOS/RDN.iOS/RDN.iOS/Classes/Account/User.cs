using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using RDN.Portable.Account;
using RDN.iOS.Classes.Network;
using RDN.Mobile.Account;
using RDN.iOS.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.Portable.Settings;
using RDN.Mobile.Database;
using RDN.Mobile.Database.Account;

namespace RDN.iOS.Classes.Account
{
    public class User
    {


        public static bool Authenticate(UserMobile user)
        {

            var status = Reachability.IsHostReachable(Reachability.HostName);
            if (status)
            {
                try
                {
                    var u = UserMobileMb.Login(user);
                    if (u.IsLoggedIn)
                    {
                        //u.LastMobileLoginDate = DateTime.UtcNow;
                        SqlAccount account = new SqlAccount(u);
                        new SqlFactory().DeleteProfile().InsertProfile(account);
                        SettingsMobile.Instance.User = u;
                    }
                    return u.IsLoggedIn;
                }
                catch (Exception ex)
                {
                    ErrorHandler.Save(ex, MobileTypeEnum.iPhone);
                }
            }
            return false;

        }
        public static UserMobile SignUp(UserMobile user)
        {
            var status = Reachability.IsHostReachable(Reachability.HostName);
            if (status)
            {
                try
                {
                    var u = UserMobileMb.SignUp(user);
                    //Settings.SaveUserPreferences(context, u);
                    return u;
                }
                catch (Exception ex)
                {
                    ErrorHandler.Save(ex, MobileTypeEnum.iPhone);
                }
            }
            return user;

        }

    }
}