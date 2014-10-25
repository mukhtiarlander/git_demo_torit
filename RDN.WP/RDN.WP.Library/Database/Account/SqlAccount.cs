using RDN.Portable.Account;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.WP.Library.Database.Account
{
    public class SqlAccount : UserMobile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public SqlAccount()
        { }

        public SqlAccount(UserMobile u)
        {
            DerbyName = u.DerbyName;
            DidSignUp = u.DidSignUp;
            Error = u.Error;
            FirstName = u.FirstName;
            Gender = u.Gender;
            IsConnectedToDerby = u.IsConnectedToDerby;
            IsLoggedIn = u.IsLoggedIn;
            IsRegisteredForNotifications = u.IsRegisteredForNotifications;
            IsValidSub = u.IsValidSub;
            LoginId = u.LoginId;
            MemberId = u.MemberId;
            Password = u.Password;
            Position = u.Position;
            RegistrationIdForNotifications = u.RegistrationIdForNotifications;
            UserName = u.UserName;
            LastMobileLoginDate = u.LastMobileLoginDate;
            CurrentLeagueId = u.CurrentLeagueId;
        }
    }
}
