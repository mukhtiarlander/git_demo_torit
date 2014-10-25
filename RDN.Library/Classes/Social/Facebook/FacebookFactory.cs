using Facebook;
using RDN.Library.Classes.Facebook.Enum;
using RDN.Utilities.Dates;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Facebook
{
    public class FacebookFactory
    {
        //https://developers.facebook.com/tools/explorer/?method=GET&path=37400746%3Ffields%3Did%2Cname
        private string ScoresMessage = String.Empty;
        private FacebookClient Client = null;
        dynamic accountsForUser = null;
        string _userAccessToken { get; set; }
        public string LongTermAccessToken { get; set; }
        public DateTime LongTermAccessTokenExpires { get; set; }
        string _siteId = string.Empty;
        string _siteName = string.Empty;

        public static FacebookFactory Initialize(string userAccessToken)
        {
            var fact = new FacebookFactory();

            fact.Client = new FacebookClient(userAccessToken);
            fact._userAccessToken = userAccessToken;
            fact.accountsForUser = fact.Client.Get("/me/accounts");


            return fact;

        }

        public FacebookFactory GetLongTermAccessToken(string clientId, string clientSecret)
        {

            dynamic result = Client.Post("oauth/access_token", new
            {
                client_id = clientId,
                client_secret = clientSecret,
                grant_type = "fb_exchange_token",
                fb_exchange_token = _userAccessToken
            });
            this.LongTermAccessToken = result.access_token;
            this._userAccessToken = this.LongTermAccessToken;
            long expire = 0;
            if (Int64.TryParse(result.expires, out expire))
                this.LongTermAccessTokenExpires = FromUnixTime(expire);
            return this;
        }

        static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public FacebookFactory GetPageAuthorization(string siteName, string siteId)
        {
            this._siteId = siteId;
            this._siteName = siteName;
            string pageAccessToken = "";
            foreach (var acct in accountsForUser.data)
            {
                if (acct.id == siteId)
                {
                    pageAccessToken = acct.access_token;
                    break;
                }
            }

            Client = new FacebookClient(pageAccessToken);
            return this;
        }




        public FacebookFactory AddScores(string teamName1, string team1Score, string team2Name, string team2Score)
        {
            this.ScoresMessage = " \n\n " + teamName1 + " " + team1Score + " - " + team2Score + " " + team2Name;
            return this;
        }

        public FacebookFactory PostToFanPage(string message, string link, string pictureLink, string name, string captionOfLink, string dateTimeForMessage)
        {

            dynamic parameters = new ExpandoObject();
            if (!String.IsNullOrEmpty(message))
                parameters.message = message + ScoresMessage;
            if (!String.IsNullOrEmpty(link))
                parameters.link = link;
            if (!String.IsNullOrEmpty(pictureLink))
                parameters.picture = pictureLink;
            if (!String.IsNullOrEmpty(name))
                parameters.name = name;
            if (!String.IsNullOrEmpty(captionOfLink))
                parameters.caption = captionOfLink;
            else
            {
                parameters.caption = "";
            }
            if (!String.IsNullOrEmpty(dateTimeForMessage))
            {
                parameters.published = false;
                parameters.scheduled_publish_time = DateTimeExt.ToUnixTime(Convert.ToDateTime(dateTimeForMessage));
            }
            this.Client.Post("/" + this._siteId + "/feed", parameters);


            return this;
        }


    }
}
