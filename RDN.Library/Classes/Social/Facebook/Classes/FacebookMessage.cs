using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Facebook.Classes
{
    public class FacebookMessage
    {
        public FacebookMessage()
        { }
        public string Message { get; set; }
        public string Link { get; set; }
        public string PictureUrl { get; set; }
        public string Name { get; set; }
        public string Caption { get; set; }
        public string Team1Name { get; set; }
        public string Team1Score { get; set; }
        public string Team2Name { get; set; }
        public string Team2Score { get; set; }
        public string DateOfGame { get; set; }
        public string DateForMessage { get; set; }
        public string UserAccessToken { get; set; }
        public bool IsLoggedIntoFacebook { get; set; }
    }
}
