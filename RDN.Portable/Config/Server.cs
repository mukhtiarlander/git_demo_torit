using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Config
{
    public class ServerConfig
    {
        public static string BaseApiUrl { get; set; }

        public static void SetApiUrl(string baseApiUrl)
        {
            BaseApiUrl = baseApiUrl;
        }

        public static string BaseInternalUrl { get; set; }

        public static void SetInternalUrl(string baseUrl)
        {
            BaseInternalUrl = baseUrl;
        }

        public static string BasePublicUrl { get; set; }

        public static void SetPublicUrl(string baseUrl)
        {
            BasePublicUrl = baseUrl;
        }

        public static string BaseShopsUrl { get; set; }

        public static void SetShopsUrl(string baseUrl)
        {
            BaseShopsUrl = baseUrl;
        }


        /// <summary>
        /// default email for admins
        /// </summary>

        public static Guid DEFAULT_SCOTTS_USER_ID = new Guid("C59EF7B7-CBDD-4663-B22A-5C7C11E2934F");
        public static Guid DEFAULT_JAMIES_USER_ID = new Guid("9C73F101-1F00-46AA-BD29-4B836F4E426C");
        public static Guid DEFAULT_SCOTTS_USER_ID_DEBUG = new Guid("737f127f-8d4f-4167-9d85-9ddc0cd8f708");
        public static Guid DEFAULT_ADMIN_USER_ID = new Guid("885E1E16-0F8E-44A8-BB36-4FCE12EF6360");
        public static Guid DEFAULT_ADMIN_USER_ID_DEBUG = new Guid("885E1E16-0F8E-44A8-BB36-4FCE12EF6360");

        public static readonly int CSS_VERSION = 25;
        public static readonly int JS_VERSION = CSS_VERSION;

        public static readonly Guid DEFAULT_RDN_FORUM_ID = new Guid("8059734d-511c-4511-8770-ee595c10bef5");

        public static readonly string ERROR_SUBMIT_URL_DEBUG = "http://localhost:52509/error/submit";
        public static readonly string ERROR_SUBMIT_URL_DEBUG_ANDROID = "http://10.0.2.2:52509/error/submit";
        public static readonly string ERROR_SUBMIT_URL = BaseApiUrl + "/error/submit";
        public static readonly string ERROR_SUBMIT_URL_WINDOWS_PHONE = BaseApiUrl + "/error/submitwp";
        public static readonly string ERROR_SUBMIT_URL_WINDOWS_PHONE_DEBUG = "http://localhost:52509/error/submitwp";
        public static readonly string ERROR_SUBMIT_URL_MOBILE = BaseApiUrl + "/error/submitmobile";
        //public static readonly string WEBSITE_DEFAULT_LOCATION_HOST = "rdnation.com";
        //public static readonly string WEBSITE_PING_LOCATION = "api.rdnation.com";
        //public static readonly string WEBSITE_DEFAULT_LOCATION_HOST_API = "api.rdnation.com";
        public static readonly string WEBSITE_DEFAULT_LOCATION_DEBUG = "http://localhost:26646";

        public static readonly string WEBSITE_DEFAULT_LOGIN_LOCATION = BasePublicUrl + "/login";
        public static readonly string WEBSITE_LOST_PASSWORD_RESET_LOCATION =BasePublicUrl +"/lostpassword";
        public static readonly string WEBSITE_LOST_PASSWORD_RECOVER = "https://rdnation.com/recover";
        public static readonly string WIKI_URL = "http://wiki.rdnation.com/";
        public static readonly string WIKI_URL_FOR_CONFIRMED_PAYPAL_ACCOUNT = "http://wiki.rdnation.com/Verified_Paypal_Account";

        public static readonly string WEBSITE_CLEAR_TOURNAMENT_CACHE = "https://rdnation.com/utilities/ClearTournamentCache";
        public static readonly string WEBSITE_CLEAR_TOURNAMENT_CACHE_API = "https://api.rdnation.com/tournament/ClearTournamentCache";
        /// <summary>
        /// users get an email and we use this link to help them verify their account faster.
        /// </summary>
        public static readonly string WEBSITE_VALIDATE_ACCOUNT_WITH_EMAIL_LOCATION = "https://league.rdnation.com/manager/validateemails";
        public static readonly string WEBSITE_VERIFY_EMAIL_LOGGED_IN_LOCATION = "https://league.rdnation.com/manager/verifyemail";
        public static readonly string WEBSITE_VALIDATE_DERBY_NAME = "https://rdnation.com/verifyderbyname/";

        public static readonly string WEBSITE_DEFAULT_LOCATION_FOR_LEAGUES = "https://rdnation.com/roller-derby-league/";
        public static readonly string WEBSITE_DEFAULT_LOCATION_FOR_ALL_LEAGUES = "https://rdnation.com/roller-derby-leagues";

        public static readonly string WEBSITE_DEFAULT_LOCATION_FOR_PAST_GAMES = "https://rdnation.com/roller-derby-game";

        public static readonly string VIEW_MESSAGE_CONVERSATION = "https://league.rdnation.com/messages/view/";
        public static readonly string VIEW_MESSAGES_INBOX_MEMBER = "https://league.rdnation.com/messages/member/";
        public static readonly string WEBSITE_DEFAULT_LOCATION_FOR_MEMBER = "https://rdnation.com/roller-derby-skater/";

        public static readonly string WEBSITE_MEMBER_SETTINGS = "https://league.rdnation.com/member/settings";

        public static readonly string EMAIL_LAYOUTS_LOCATION = @"C:\WebSites Services\EmailLayouts";

        public static readonly string GAME_URL_FOR_SERVER_DEBUG = "http://localhost:14202/livegame/gamexml?id=";
        public static readonly string GAME_URL_FOR_SERVER = "https://api.rdnation.com/livegame/gamexml?id=";
        public static readonly string GAME_URL_FOR_RDNATION = "https://rdnation.com/roller-derby-game/";

        public static string SAVE_ERRORS_FOLDER = "C:\\WebSites\\RDNation.com\\Data\\Scoreboard\\Errors\\";
        public static string SAVE_FEEDBACK_FOLDER = "C:\\WebSites\\RDNation.com\\Data\\Scoreboard\\Feedback\\";
        public static string SAVE_LIVE_GAMES_FOLDER = "C:\\WebSites\\RDNation.com\\Data\\Scoreboard\\LiveGames\\";
        public static string SAVE_OLD_GAMES_FOLDER = "C:\\WebSites\\RDNation.com\\Data\\Scoreboard\\OldGames\\";

        public static string SAVE_ADVERTISEMENTS_WEBSITE_FOLDER = "https://rdnation.com/data/rollerderby/advertisements/";
        public static string SAVE_ADVERTISEMENTS_FOLDER = "C:\\WebSites\\RDNation.com\\Data\\RollerDerby\\Advertisements\\";

        //public static string SAVE_LOGOS_WEBSITE_FOLDER = "https://rdnation.com/data/rollerderby/logos/";
        //public static string SAVE_LOGOS_FOLDER = @"C:\WebSites\codingforcharity.org\rdnation\logos\";

        public static Guid RDNATION_STORE_ID = new Guid("7b0c3da2-b58a-4b1a-b9a2-92d253ce0100");

        
        public static readonly string LEAGUE_SUBSCRIPTION_RESUBSUBSCRIBE = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/billing/league/";
        public static readonly string LEAGUE_SUBSCRIPTION_ADDSUBSUBSCRIBE = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/billing/league/addsubscription/";
        public static readonly string LEAGUE_SUBSCRIPTION_UPDATESUBSUBSCRIBE = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/billing/league/update/";
        public static readonly string LEAGUE_SUBSCRIPTION_RECIEPT = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/billing/league/receipt/";
        public static readonly string LEAGUE_DUES_MANAGEMENT_URL = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/dues/league/";
        public static readonly string LEAGUE_DUES_RECEIPT_URL = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/dues/receipt/league/";
        public static readonly string LEAGUE_DUES_SETTINGS_URL = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/dues/settings/league/";
        public static readonly string LEAGUE_SUBSCRIPTION_RESUBSUBSCRIBE_DEBUG = WEBSITE_INTERNAL_DEFAULT_LOCATION_DEBUG + "/billing/league/";
        public static readonly string LEAGUE_SUBSCRIPTION_ADDSUBSUBSCRIBE_DEBUG = WEBSITE_INTERNAL_DEFAULT_LOCATION_DEBUG + "/billing/league/addsubscription/";
        public static readonly string LEAGUE_SUBSCRIPTION_RECIEPT_DEBUG = WEBSITE_INTERNAL_DEFAULT_LOCATION_DEBUG + "/billing/league/receipt/";
        public static readonly string LEAGUE_DUES_RECEIPT_URL_DEBUG = WEBSITE_INTERNAL_DEFAULT_LOCATION_DEBUG + "/dues/receipt/league/";
        public static readonly string LEAGUE_DUES_SETTINGS_URL_DEBUG = WEBSITE_INTERNAL_DEFAULT_LOCATION_DEBUG + "/dues/settings/league/";

        public static readonly string LEAGUE_SUBSCRIPTION_LINK_FOR_ADMIN = "http://raspberry.rdnation.com/League/SubscriptionsOfAllLeagues";
        public static readonly string LEAGUE_FORUM_POSTS_URL = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/forum/post/view/";
        public static readonly string LEAGUE_FORUM_URL = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/forum/posts/league/";

        public static readonly string STORE_MERCHANT_CART_URL = WEBSITE_STORE_DEFAULT_LOCATION + "/checkout/";
        public static readonly string STORE_MERCHANT_RECEIPT_URL = WEBSITE_STORE_DEFAULT_LOCATION + "/receipt/";
        public static readonly string STORE_MERCHANT_SHOP_URL = WEBSITE_STORE_DEFAULT_LOCATION + "/roller-derby-shop/";
        public static readonly string STORE_CREATE_URL = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/store/home";

        public static readonly string MEMBER_SETTINGS_URL = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/member/settings/";
        public static readonly string GAMES_OFFICIATING_REQUESTS_URL = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/officiating/requests";
        public static readonly string GAMES_BOUT_CHALLENGES = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/boutchallenge/view/all";
        public static readonly string LEAGUE_CONTACTS_URL = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/league/contacts";
        public static readonly string LEAGUE_DOCUMENTS_URL = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/league/documents/";
        public static readonly string LEAGUE_JOB_BOARD_URL = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/league/jobboard/";
        public static readonly string LEAGUE_INVENTORY_URL = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/league/inventory/all";
        public static readonly string LEAGUE_SPONSORS_URL = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/league/sponsors";
        public static readonly string LEAGUE_SHOPS_URL = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/store/home/";
        public static readonly string MEMBER_MEDICAL_URL = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/member/medical";
        public static readonly string MEMBER_CONTACTS_URL = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/member/contacts";
        public static readonly string LEAGUE_JOIN_URL = WEBSITE_INTERNAL_DEFAULT_LOCATION + "/league/join";

        public static readonly string STRIPE_DEBUG_KEY = "pk_0D4hTiqyKAkZbsx8QCPpZcSljUYFO";
        public static readonly string STRIPE_LIVE_KEY = "pk_2k5dQ6cKsgfdC7Z7sfSPk1Yn1t3jx";
        public static readonly string STRIPE_CONNECT_DEBUG_KEY = "ca_1H0q11C8HbuHiAPfYEpgUIEhI9clgPJ7";
        public static readonly string STRIPE_CONNECT_LIVE_KEY = "ca_1H0qzzsbbbMWxgN41Oiscxmrc3fiTfe6";

        public static readonly string PAYPAL_SELLER_DEBUG_ADDRESS = "seller_1359430587_biz@gmail.com";
        public static readonly string PAYPAL_SELLER1_DEBUG_ADDRESS = "jb-us-seller@paypal.com";
        public static readonly string PAYWALL_RECEIPT_URL = WEBSITE_DEFAULT_LOCATION + "/streaming/receipt/";


        /// <summary>
        /// url to clear the memberCache object on the league website.  We use the for admin purposes so the cache for specific
        /// users can be destroyed and recreated.
        /// </summary>
        public static readonly string URL_TO_CLEAR_MEMBER_CACHE = "https://league.rdnation.com/utilities/clearmembercache1234?memberId=";
        public static readonly string URL_TO_CLEAR_MEMBER_CACHE_API = "https://league.rdnation.com/utilities/clearmembercache1234?memberId=";
        public static readonly string URL_TO_CLEAR_LEAGUE_MEMBER_CACHE = "https://league.rdnation.com/utilities/ClearLeagueMembersCache1234?lid=";
        public static readonly string URL_TO_CLEAR_LEAGUE_MEMBER_CACHE_API = "https://api.rdnation.com/utilities/ClearLeagueMembersCache1234?lid=";
        public static readonly string URL_TO_CLEAR_LIVE_GAME_CACHE = "https://api.rdnation.com/utilities/ClearLiveGameCache1234?gameId=";
        public static readonly string URL_TO_CLEAR_EXCHANGE_RATES = "https://league.rdnation.com/utilities/ClearCurrencyExchangeRates";
        public static readonly string URL_TO_CLEAR_EXCHANGE_RATES_API = "https://api.rdnation.com/utilities/ClearCurrencyExchangeRates";

        public static readonly string URL_TO_CLEAR_FORUM_TOPIC = "https://league.rdnation.com/forum/ClearForumTopicCache?";
        public static readonly string URL_TO_CLEAR_FORUM_TOPIC_API = "https://api.rdnation.com/league/ClearForumTopicCache?";


        public static readonly string WEBSITE_SCOREBOARD_URL = "https://rdnation.com/roller-derby-scoreboard";
        public static readonly string WEBSITE_EVENTS_URL = "https://rdnation.com/roller-derby-events";
        public static readonly string WEBSITE_EVENT_URL = "https://rdnation.com/roller-derby-event";
        public static readonly string WEBSITE_SIGNUP_URL = "https://rdnation.com/SignUp";
        public static readonly string WEBSITE_SETUP_LEAGUE_URL = "https://league.rdnation.com/league/setup";
        public static readonly string WEBSITE_SHOPS_SELL_URL = "https://shops.rdnation.com/sell";
        public static readonly string WEBSITE_SHOPS_URL = "https://shops.rdnation.com/";



        public static readonly string WEBSITE_TOURNAMENT_RENDER_URL = "https://api.rdnation.com/tournament/rendertournament?id=";
        public static readonly string WEBSITE_TOURNAMENT_RENDER_PERFORMANCE_URL = "https://api.rdnation.com/tournament/renderperformancetournament?id=";


    }
}
