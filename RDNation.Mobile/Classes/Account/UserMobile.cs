using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

using System.Text;
using System.Threading.Tasks;
using RDN.Mobile.Classes.Utilities;
using RDN.Mobile.ErrorHandling;
using RDN.Portable.Config;
using RDN.Portable.Account;
using RDN.Portable.Settings;
using RDN.Portable.Network;

namespace RDN.Mobile.Account
{

    [DataContract]
    public class UserMobileMb : UserMobile
    {
        public static UserMobile Login(UserMobile user)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(user), MobileConfig.LOGIN_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            SettingsMobile.Instance.User = Json.DeserializeObject<UserMobile>(json);
            return SettingsMobile.Instance.User;
        }
        public static UserMobile SignUp(UserMobile user)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(user), MobileConfig.SIGNUP_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            SettingsMobile.Instance.User = Json.DeserializeObject<UserMobile>(json);
            return SettingsMobile.Instance.User;
        }




    }
}
