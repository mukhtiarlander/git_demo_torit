using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Payment.Paywall;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Social.Twitter;
using RDN.Library.Classes.Error;
using System.Configuration;
using System.Net;
using TweetSharp;
using RDN.Library.Classes.Site.Enums;

namespace RDN.Library.Cache.Singletons
{
    public class SiteSingleton
    {
        public SiteType SiteType { get;set; }
        public bool IsProduction { get; set; }
        public PaymentMode IsPayPalLive { get; set; }
        static SiteSingleton instance = new SiteSingleton();
        private List<TwitterStatus> GetTweets { get; set; }
        private DateTime LastRefresh { get; set; }

        public IEnumerable<TwitterStatus> GetTweetsStatus(string screenName)
        {
            if (instance.GetTweets == null || instance.LastRefresh < DateTime.UtcNow.AddHours(-2))
            {               
                var service = new TwitterService(RDN.Library.Classes.Config.LibraryConfig.TwitterConsumerKey, RDN.Library.Classes.Config.LibraryConfig.TwitterConsumerSecret);
                service.AuthenticateWith(RDN.Library.Classes.Config.LibraryConfig.TwitterToken,RDN.Library.Classes.Config.LibraryConfig.TwitterTokenSecret);

                instance.GetTweets = service.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions { Count = 150 }).ToList();
                instance.LastRefresh = DateTime.UtcNow;
            }
            return instance.GetTweets;
        }


        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static SiteSingleton()
        {

        }

        public static SiteSingleton Instance
        {
            get
            {
                if (instance == null)
                    instance = new SiteSingleton();
                return instance;
            }
        }




    }
}
