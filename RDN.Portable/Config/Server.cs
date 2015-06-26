using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Config
{
    public class ServerManager
    {

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

        //public static readonly string EMAIL_LAYOUTS_LOCATION = @"C:\WebSites Services\EmailLayouts";

        public static string SAVE_ERRORS_FOLDER = "\\Scoreboard\\Errors\\";
        public static string SAVE_FEEDBACK_FOLDER = "\\Scoreboard\\Feedback\\";
        public static string SAVE_LIVE_GAMES_FOLDER = "\\Scoreboard\\LiveGames\\";
        public static string SAVE_OLD_GAMES_FOLDER = "\\Scoreboard\\OldGames\\";

        public static string SAVE_ADVERTISEMENTS_FOLDER = "\\RollerDerby\\Advertisements\\";

        //public static string SAVE_LOGOS_FOLDER = @"C:\WebSites\codingforcharity.org\rdnation\logos\";

        public static Guid RDNATION_STORE_ID = new Guid("7b0c3da2-b58a-4b1a-b9a2-92d253ce0100");

        //public static readonly string STRIPE_DEBUG_KEY = "pk_0D4hTiqyKAkZbsx8QCPpZcSljUYFO";
        //public static readonly string STRIPE_LIVE_KEY = "pk_2k5dQ6cKsgfdC7Z7sfSPk1Yn1t3jx";
        //public static readonly string STRIPE_CONNECT_DEBUG_KEY = "ca_1H0q11C8HbuHiAPfYEpgUIEhI9clgPJ7";
        //public static readonly string STRIPE_CONNECT_LIVE_KEY = "ca_1H0qzzsbbbMWxgN41Oiscxmrc3fiTfe6";

        //public static readonly string PAYPAL_SELLER_DEBUG_ADDRESS = "seller_1359430587_biz@gmail.com";
        //public static readonly string PAYPAL_SELLER1_DEBUG_ADDRESS = "jb-us-seller@paypal.com";


    }
}
