using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.Url
{
   public  class UrlManager
   {

       #region unused URLS save for later
       //public static readonly string ERROR_SUBMIT_URL_DEBUG = "http://localhost:52509/error/submit";
       //public static readonly string ERROR_SUBMIT_URL_DEBUG_ANDROID = "http://10.0.2.2:52509/error/submit";
       //public static readonly string ERROR_SUBMIT_URL_WINDOWS_PHONE = "/error/submitwp";
       //public static readonly string ERROR_SUBMIT_URL_WINDOWS_PHONE_DEBUG = "http://localhost:52509/error/submitwp";
       //public static readonly string ERROR_SUBMIT_URL_MOBILE = "https://api.rdnation.com/error/submitmobile";
       //public static readonly string PublicSite_HOST = "rdnation.com";
       //public static readonly string WEBSITE_PING_LOCATION = "api.rdnation.com";
       //public static readonly string PublicSite_HOST_API = "api.rdnation.com";
       //public static readonly string GAMES_OFFICIATING_REQUESTS_URL = "/officiating/requests";
       //public static readonly string GAMES_BOUT_CHALLENGES = "/boutchallenge/view/all";
       //public static readonly string LEAGUE_CONTACTS_URL = "/league/contacts";
       //public static readonly string LEAGUE_DOCUMENTS_URL = "/league/documents/";
       //public static readonly string LEAGUE_JOB_BOARD_URL = "/league/jobboard/";
       //public static readonly string LEAGUE_INVENTORY_URL = "/league/inventory/all";
       //public static readonly string LEAGUE_SPONSORS_URL = "/league/sponsors";
       //public static readonly string LEAGUE_SHOPS_URL = "/store/home/";
       //public static readonly string MEMBER_MEDICAL_URL = "/member/medical";
       //public static readonly string MEMBER_CONTACTS_URL = "/member/contacts";
       //public static readonly string LEAGUE_JOIN_URL = "/league/join";
       //public static readonly string WEBSITE_LOST_PASSWORD_RECOVER = "/recover";
       //public static readonly string PublicSite_FOR_ALL_LEAGUES = "/roller-derby-leagues";
       //public static readonly string PublicSite_FOR_MEMBER = "/roller-derby-skater/";
       //public static readonly string GAME_URL_FOR_SERVER = "/livegame/gamexml?id=";
       //public static string SAVE_LOGOS_WEBSITE_FOLDER = "/data/rollerderby/logos/";
       //public static readonly string WEBSITE_SCOREBOARD_URL = "/roller-derby-scoreboard";
       //public static readonly string WEBSITE_EVENTS_URL = "https://rdnation.com/roller-derby-events";
       //public static readonly string WEBSITE_SIGNUP_URL = "https://rdnation.com/SignUp";
       //public static readonly string WEBSITE_SETUP_LEAGUE_URL = "https://league.rdnation.com/league/setup";
       //public static readonly string WEBSITE_SHOPS_SELL_URL = "https://shops.rdnation.com/sell";
       #endregion

       
       public static readonly string ERROR_SUBMIT_URL = "/error/submit";


       public static readonly string WEBSITE_DEFAULT_LOGIN_LOCATION = "/login";
       public static readonly string WEBSITE_LOST_PASSWORD_RESET_LOCATION = "/lostpassword";

       public static readonly string WIKI_URL_FOR_CONFIRMED_PAYPAL_ACCOUNT = "/Verified_Paypal_Account";

       public static readonly string WEBSITE_CLEAR_TOURNAMENT_CACHE = "/utilities/ClearTournamentCache";
       public static readonly string WEBSITE_CLEAR_TOURNAMENT_CACHE_API = "/tournament/ClearTournamentCache";
        ///// <summary>
        ///// users get an email and we use this link to help them verify their account faster.
        ///// </summary>
       public static readonly string WEBSITE_VALIDATE_ACCOUNT_WITH_EMAIL_LOCATION = "/manager/validateemails";
       public static readonly string WEBSITE_VERIFY_EMAIL_LOGGED_IN_LOCATION = "/manager/verifyemail";
       public static readonly string WEBSITE_VALIDATE_DERBY_NAME = "/verifyderbyname/";

       public static readonly string PublicSite_FOR_LEAGUES = "/roller-derby-league/";
       
       public static readonly string PublicSite_FOR_PAST_GAMES = "/roller-derby-game";

       public static readonly string VIEW_MESSAGE_CONVERSATION = "/messages/view/";
       public static readonly string VIEW_MESSAGES_INBOX_MEMBER = "/messages/member/";
       
       public static readonly string WEBSITE_MEMBER_SETTINGS = "/member/settings";
       
       public static readonly string GAME_URL = "/roller-derby-game/";

       public static string SAVE_ADVERTISEMENTS_WEBSITE_FOLDER = "/data/rollerderby/advertisements/";

        public static readonly string LEAGUE_SUBSCRIPTION_SERVICES_URL = "/pricing";
        public static readonly string LEAGUE_SUBSCRIPTION_RESUBSUBSCRIBE = "/billing/league/";
        public static readonly string LEAGUE_SUBSCRIPTION_ADDSUBSUBSCRIBE =  "/billing/league/addsubscription/";
        public static readonly string LEAGUE_SUBSCRIPTION_UPDATESUBSUBSCRIBE =  "/billing/league/update/";
        public static readonly string LEAGUE_SUBSCRIPTION_RECIEPT = "/billing/league/receipt/";
        public static readonly string LEAGUE_DUES_MANAGEMENT_URL = "/dues/league/";
        public static readonly string LEAGUE_DUES_RECEIPT_URL = "/dues/receipt/league/";
        public static readonly string LEAGUE_DUES_SETTINGS_URL = "/dues/settings/league/";

        public static readonly string LEAGUE_SUBSCRIPTION_LINK_FOR_ADMIN = "/League/SubscriptionsOfAllLeagues";
        public static readonly string LEAGUE_FORUM_POSTS_URL = "/forum/post/view/";
        public static readonly string LEAGUE_FORUM_URL = "/forum/posts/league/";

        public static readonly string STORE_MERCHANT_CART_URL = "/checkout/";
        public static readonly string STORE_MERCHANT_RECEIPT_URL = "/receipt/";
        public static readonly string STORE_MERCHANT_SHOP_URL = "/roller-derby-shop/";
        public static readonly string STORE_CREATE_URL =  "/store/home";

        public static readonly string MEMBER_SETTINGS_URL = "/member/settings/";
       
        public static readonly string PAYWALL_RECEIPT_URL = "/streaming/receipt/";

        ///// <summary>
        ///// url to clear the memberCache object on the league website.  We use the for admin purposes so the cache for specific
        ///// users can be destroyed and recreated.
        ///// </summary>
        public static readonly string URL_TO_CLEAR_MEMBER_CACHE = "/utilities/clearmembercache1234?memberId=";
        public static readonly string URL_TO_CLEAR_MEMBER_CACHE_API = "/utilities/clearmembercache1234?memberId=";
        public static readonly string URL_TO_CLEAR_LEAGUE_MEMBER_CACHE = "/utilities/ClearLeagueMembersCache1234?lid=";
        public static readonly string URL_TO_CLEAR_LEAGUE_MEMBER_CACHE_API = "/utilities/ClearLeagueMembersCache1234?lid=";
        public static readonly string URL_TO_CLEAR_LIVE_GAME_CACHE = "/utilities/ClearLiveGameCache1234?gameId=";
        public static readonly string URL_TO_CLEAR_EXCHANGE_RATES = "/utilities/ClearCurrencyExchangeRates";
        public static readonly string URL_TO_CLEAR_EXCHANGE_RATES_API = "/utilities/ClearCurrencyExchangeRates";

        public static readonly string URL_TO_CLEAR_FORUM_TOPIC = "/forum/ClearForumTopicCache?";
        public static readonly string URL_TO_CLEAR_FORUM_TOPIC_API = "/league/ClearForumTopicCache?";
               
        public static readonly string WEBSITE_EVENT_URL = "/roller-derby-event";

        public static readonly string WEBSITE_TOURNAMENT_RENDER_URL = "/tournament/rendertournament?id=";
        public static readonly string WEBSITE_TOURNAMENT_RENDER_PERFORMANCE_URL = "/tournament/renderperformancetournament?id=";

    }
}
