using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDN.Raspberry.Models.Admin
{
    public enum MassEmailEnum
    {
        //sends a mass email to all leagues
        AllLeaguesWorldWide = 1,
        //sends a email to all those who downloaded scoreboard and gave feedback.
        AllScoreboardDownloadersAndFeedbackers = 2,
        RefMasterList = 3,
        AllEmailsAvailable = 4,
        AllRegisteredUsers = 5,
        AllRegisteredLeagues = 6,
        AllRegisteredEmails = 7,
        AllLeaguesThatDontExistWithinSite = 8,
        AllLeagueOwners = 9,
        AllEmailsToSendMontlyUpdatesTo = 10,
        Role = 11
    }

    public class MassEmail
    {
        public bool IsMassSendVerified { get; set; }
        public string TestEmail { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }

        public string Role { get; set; }

        public MassEmailEnum MassEmailType { get; set; }
    }
}