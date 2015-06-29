using RDN.Library.Classes.Account;
using RDN.Library.Classes.EmailServer;
using RDN.Library.DataModels.EmailServer.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace RDN.Library.Classes.RN.Communication
{
    public class Authors
    {
        public static bool SendAutomatedPostingEmailToAuthors(List<Guid> userIds)
        {

            for (int i = 0; i < userIds.Count; i++)
            {
                var mem = User.GetMemberWithUserId(userIds[i]);

                var user = Membership.GetUser(userIds[i]);



                Dictionary<string, string> emailProps = new Dictionary<string, string>(){
                {"Name",mem.Name}   ,
                {"link",RDN.Library.Classes.Config.LibraryConfig.PublicSite  + "admin/#/"}   
               };

                EmailServer.EmailServer.SendEmail(RDN.Library.Classes.Config.LibraryConfig.SiteEmail, RDN.Library.Classes.Config.LibraryConfig.FromEmailName, user.UserName, "Rollin News Reminder", emailProps, EmailServerLayoutsEnum.RNAutomatedEmailForWriter, EmailPriority.Important);
            }

            return true;
        }
    }
}
