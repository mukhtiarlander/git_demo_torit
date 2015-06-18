using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.Url
{
    public class UrlManager
    {
        public static readonly string LEAGUE_SUBSCRIPTION_SERVICES_URL = "https://rdnation.com/pricing";
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
    }
}
