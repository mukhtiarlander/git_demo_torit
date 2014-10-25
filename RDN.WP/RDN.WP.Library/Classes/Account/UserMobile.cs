using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

using System.Text;
using System.Threading.Tasks;
using RDN.Portable.Config;
using RDN.Portable.Account;
using RDN.Portable.Settings;
using RDN.Portable.Classes.Utilities;
using RDN.Portable.Network;
using RDN.WP.Library.Database;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Account;
using System.Net.Http;

namespace RDN.WP.Library.Account
{

    [DataContract]
    public class UserMobileWP : UserMobile
    {
        public static MemberDisplayEdit SaveMemberEdit(Guid memId, Guid uid, MemberDisplayEdit member)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new MemberPassParams() { Mid = memId, Uid = uid, Member = member }), MobileConfig.MEMBER_EDIT_SAVE_GET_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<MemberDisplayEdit>(json);
        }
        public static MemberDisplayEdit GetMemberEdit(Guid memId, Guid uid)
        {

            var response = Network.SendPackage(Network.ConvertObjectToStream(new MemberPassParams() { Mid = memId, Uid = uid }), MobileConfig.MEMBER_EDIT_GET_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<MemberDisplayEdit>(json);
        }

        //public static UserMobile Login(UserMobile user)
        //{
        //    Random r = new Random();
        //    var response = Network.SendPackage1(Network.ConvertObjectToStream(user), MobileConfig.LOGIN_URL + "?r=" + r.Next());
        //    SettingsMobile.Instance.User = Json.DeserializeObject<UserMobile>(response.Result);
        //    return SettingsMobile.Instance.User;
        //}
        public static UserMobile Login(UserMobile user)
        {
            Random r = new Random();
            var response = Network.SendPackage(Network.ConvertObjectToStream(user), MobileConfig.LOGIN_URL + "?r=" + r.Next());
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
        public static AccountSettingsModel AccountSettings(UserMobile user)
        {
            Random r = new Random();
            var response = Network.SendPackage(Network.ConvertObjectToStream(user), MobileConfig.ACCOUNT_SETTINGS_URL + "?r=" + r.Next());
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();

            return Json.DeserializeObject<AccountSettingsModel>(json);
        }



    }
}
