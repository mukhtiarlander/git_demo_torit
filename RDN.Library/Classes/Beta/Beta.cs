using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Context;

namespace RDN.Library.Classes.Beta
{
    public class Beta
    {
        /// <summary>
        /// signs the email up for public beta.
        /// </summary>
        /// <param name="email"></param>
        public static void SignUpForBeta(string email)
        {
            var dc = new ManagementContext();
            DataModels.Beta.BetaSignUp signUp = new DataModels.Beta.BetaSignUp();
            signUp.Email = email;
            signUp.Emailed = false;
            dc.BetaEmails.Add(signUp);
            dc.SaveChanges();
        }

    }
}
