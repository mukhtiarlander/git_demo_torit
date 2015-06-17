using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace RDN.Library.Classes.Config
{
    public static class LibraryConfig
    {
        public static string PublicSite
        {
            get
            {
                return ConfigurationManager.AppSettings["PublicSite"];
            }
        }

        public static string FacebookPageName2
        {
            get
            {
                return ConfigurationManager.AppSettings["FacebookPageName2"];
            }
        }

        public static string TwitterConsumerKey
        {
            get
            {
                return ConfigurationManager.AppSettings["TwitterConsumerKey"];
            }
        }
        public static string FacebookPageId2
        {
            get
            {
                return ConfigurationManager.AppSettings["FacebookPageId2"];
            }
        }

        public static string TwitterConsumerSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["TwitterConsumerSecret"];
            }
        }
        public static string TwitterToken
        {
            get
            {
                return ConfigurationManager.AppSettings["TwitterToken"];
            }
        }

        public static string TwitterTokenSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["TwitterTokenSecret"];
            }
        }

        public static string ThemeColors
        {
            get
            {
                return ConfigurationManager.AppSettings["ThemeColors"];
            }
        }

        public static bool IsProduction
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["IsProduction"]);
            }
        }
        public static string ConnectionStringName
        {
            get
            {
                return ConfigurationManager.AppSettings["ConnectionStringName"];
            }
        }

        public static string InternalSite
        {
            get
            {
                return ConfigurationManager.AppSettings["InternalSite"];
            }
        }

        public static string SiteType
        {
            get
            {
                return ConfigurationManager.AppSettings["SiteType"];
            }
        }

        public static string StripeApiPublicKey
        {
            get
            {
                return ConfigurationManager.AppSettings["StripeApiPublicKey"];
            }
        }

        public static string PaymentMode
        {
            get
            {
                return ConfigurationManager.AppSettings["PaymentMode"];
            }
        }

        public static string WebsiteMainEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["WebsiteMainEmail"];
            }
        }

        public static string WebsiteName
        {
            get
            {
                return ConfigurationManager.AppSettings["WebsiteName"];
            }
        }

        public static string ListLoadingCount
        {
            get
            {
                return ConfigurationManager.AppSettings["ListLoadingCount"];
            }
        }

        public static string LeagueUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["LeagueUrl"];
            }
        }

        public static string SportName
        {
            get
            {
                return ConfigurationManager.AppSettings["SportName"];
            }
        }

        public static string WebsiteShortName
        {
            get
            {
                return ConfigurationManager.AppSettings["WebsiteShortName"];
            }
        }

        public static string WebsiteSlogan
        {
            get
            {
                return ConfigurationManager.AppSettings["WebsiteSlogan"];
            }
        }

        public static string TournamentPublicUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["TournamentPublicUrl"];
            }
        }

        public static string DefaultThemeColor
        {
            get
            {
                return ConfigurationManager.AppSettings["DefaultThemeColor"];
            }
        }

        public static string EventPublicUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["EventPublicUrl"];
            }
        }

        public static string LeaguesPublicUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["LeaguesPublicUrl"];
            }
        }

        public static string MembersPublicUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["MembersPublicUrl"];
            }
        }

        public static string NameOfMembers
        {
            get
            {
                return ConfigurationManager.AppSettings["NameOfMembers"];
            }
        }

        public static string TournamentsPublicUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["TournamentsPublicUrl"];
            }
        }

        public static string ShopUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ShopUrl"];
            }
        }

        public static string MemberPublicUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["MemberPublicUrl"];
            }
        }

        public static string BaseSiteUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["BaseSiteUrl"];
            }
        }

        public static string SiteEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["SiteEmail"];
            }
        }

        public static string FromEmailName
        {
            get
            {
                return ConfigurationManager.AppSettings["FromEmailName"];
            }
        }

        public static string StripeApiKey
        {
            get
            {
                return ConfigurationManager.AppSettings["StripeApiKey"];
            }
        }

        public static string NameOfMember
        {
            get
            {
                return ConfigurationManager.AppSettings["NameOfMember"];
            }
        }

        public static string MemberName
        {
            get
            {
                return ConfigurationManager.AppSettings["MemberName"];
            }
        }

        public static string ApiAuthenticationKey
        {
            get
            {
                return ConfigurationManager.AppSettings["ApiAuthenticationKey"];
            }
        }

        public static string DefaultInfoEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["DefaultInfoEmail"];
            }
        }

        public static string DefaultAdminEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["DefaultAdminEmailAdmin"];
            }
        }

        public static string DefaultPhoneNumber
        {
            get
            {
                return ConfigurationManager.AppSettings["DefaultPhoneNumber"];
            }
        }

        public static string DefaultKrisWorlidgeEmailAdmin
        {
            get
            {
                return ConfigurationManager.AppSettings["DefaultKrisWorlidgeEmailAdmin"];
            }
        }

        public static string DefaultEmailFromName
        {
            get
            {
                return ConfigurationManager.AppSettings["DefaultEmailFromName"];
            }
        }

        public static string DefaultEmailMessage
        {
            get
            {
                return ConfigurationManager.AppSettings["DefaultEmailMessages"];
            }
        }

        public static string TextMessageEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["TextMessageEmail"];
            }
        }
        public static string AdminPhoneNumber
        {
            get
            {
                return ConfigurationManager.AppSettings["AdminPhoneNumber"];
            }
        }
        public static string WebsiteTitleDefault
        {
            get
            {
                return ConfigurationManager.AppSettings["WebsiteTitleDefault"];
            }
        }

        public static string WebsiteSloganDefault
        {
            get
            {
                return ConfigurationManager.AppSettings["WebsiteSloganDefault"];
            }
        }

        public static string WebsiteTitleDefaultStore
        {
            get
            {
                return ConfigurationManager.AppSettings["WebsiteTitleDefaultStore"];
            }
        }
        public static string LogoURL
        {
            get
            {
                return ConfigurationManager.AppSettings["LogoURL"];
            }
        }

        public static string PaypalIPNHandler
        {
            get
            {
                return ConfigurationManager.AppSettings["PaypalIPNHandler"];
            }
        }

        public static string PaypalIPNHandlerDebug
        {
            get
            {
                return ConfigurationManager.AppSettings["PaypalIPNHandlerDebug"];
            }
        }

        public static Guid DEFAULT_SCOTTS_USER_ID
        {
            get
            {
                return new Guid(ConfigurationManager.AppSettings["DEFAULT_SCOTTS_USER_ID"]);
            }
        }
        public static Guid DEFAULT_JAMIES_USER_ID
        {
            get
            {
                return new Guid(ConfigurationManager.AppSettings["DEFAULT_JAMIES_USER_ID"]);
            }
        }
        public static Guid DEFAULT_ADMIN_USER_ID
        {
            get
            {
                return new Guid(ConfigurationManager.AppSettings["DEFAULT_ADMIN_USER_ID"]);
            }
        }
        public static string DEFAULT_RDN_FORUM_ID
        {
            get
            {
                return ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"];
            }
        }















        public static Guid RDNATION_STORE_ID
        {
            get
            {
                return new Guid(ConfigurationManager.AppSettings["RDNATION_STORE_ID"]);
            }
        }
        public static string LEAGUE_SUBSCRIPTION_UPDATESUBSUBSCRIBE
        {
            get
            {
                return ConfigurationManager.AppSettings["LEAGUE_SUBSCRIPTION_UPDATESUBSUBSCRIBE"];
            }
        }
        public static string LEAGUE_DUES_MANAGEMENT_URL
        {
            get
            {
                return LeagueUrl + ConfigurationManager.AppSettings["LEAGUE_DUES_MANAGEMENT_URL"];
            }
        }
        public static string WEBSITE_VALIDATE_DERBY_NAME
        {
            get
            {
                return PublicSite + ConfigurationManager.AppSettings["WEBSITE_VALIDATE_DERBY_NAME"];
            }
        }
        public static string WikiUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["WikiUrl"];
            }
        }
        public static string WIKI_URL_FOR_CONFIRMED_PAYPAL_ACCOUNT
        {
            get
            {
                return WikiUrl + ConfigurationManager.AppSettings["WIKI_URL_FOR_CONFIRMED_PAYPAL_ACCOUNT"];
            }
        }
        public static string LEAGUE_DUES_RECEIPT_URL
        {
            get
            {
                return InternalSite + ConfigurationManager.AppSettings["LEAGUE_DUES_RECEIPT_URL"];
            }
        }
        public static string LEAGUE_DUES_SETTINGS_URL
        {
            get
            {
                return InternalSite + ConfigurationManager.AppSettings["LEAGUE_DUES_SETTINGS_URL"];
            }
        }
        public static string CSS_VERSION
        {
            get
            {
                return ConfigurationManager.AppSettings["CSS_VERSION"];
            }
        }
        public static string WEBSITE_VALIDATE_ACCOUNT_WITH_EMAIL_LOCATION
        {
            get
            {
                return ConfigurationManager.AppSettings["WEBSITE_VALIDATE_ACCOUNT_WITH_EMAIL_LOCATION"];
            }
        }
        public static string VIEW_MESSAGE_CONVERSATION
        {
            get
            {
                return ConfigurationManager.AppSettings["VIEW_MESSAGE_CONVERSATION"];
            }
        }
        public static string VIEW_MESSAGES_INBOX_MEMBER
        {
            get
            {
                return ConfigurationManager.AppSettings["VIEW_MESSAGES_INBOX_MEMBER"];
            }
        }
        public static string WEBSITE_EVENT_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["WEBSITE_EVENT_URL"];
            }
        }
        public static string LEAGUE_FORUM_POSTS_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["LEAGUE_FORUM_POSTS_URL"];
            }
        }
        public static string LEAGUE_FORUM_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["LEAGUE_FORUM_URL"];
            }
        }
        public static string JS_VERSION
        {
            get
            {
                return ConfigurationManager.AppSettings["JS_VERSION"];
            }
        }
        public static string GAME_URL_FOR_RDNATION
        {
            get
            {
                return ConfigurationManager.AppSettings["GAME_URL_FOR_RDNATION"];
            }
        }
        public static string LEAGUE_SUBSCRIPTION_SERVICES_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["LEAGUE_SUBSCRIPTION_SERVICES_URL"];
            }
        }
        public static string LEAGUE_SUBSCRIPTION_RESUBSUBSCRIBE
        {
            get
            {
                return ConfigurationManager.AppSettings["LEAGUE_SUBSCRIPTION_RESUBSUBSCRIBE"];
            }
        }
        public static string LEAGUE_SUBSCRIPTION_RECIEPT
        {
            get
            {
                return ConfigurationManager.AppSettings["LEAGUE_SUBSCRIPTION_RECIEPT"];
            }
        }
        public static string LEAGUE_SUBSCRIPTION_LINK_FOR_ADMIN
        {
            get
            {
                return ConfigurationManager.AppSettings["LEAGUE_SUBSCRIPTION_LINK_FOR_ADMIN"];
            }
        }
        public static string PAYWALL_RECEIPT_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["PAYWALL_RECEIPT_URL"];
            }
        }
        //public static string DEFAULT_RDN_FORUM_ID
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"];
        //    }
        //}
        //public static string DEFAULT_RDN_FORUM_ID
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"];
        //    }
        //}
        //public static string DEFAULT_RDN_FORUM_ID
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"];
        //    }
        //}
        //public static string DEFAULT_RDN_FORUM_ID
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"];
        //    }
        //}
        //public static string DEFAULT_RDN_FORUM_ID
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"];
        //    }
        //}
        //public static string DEFAULT_RDN_FORUM_ID
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"];
        //    }
        //}
        //public static string DEFAULT_RDN_FORUM_ID
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"];
        //    }
        //}
        //public static string DEFAULT_RDN_FORUM_ID
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"];
        //    }
        //}
        //public static string DEFAULT_RDN_FORUM_ID
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"];
        //    }
        //}
        //public static string DEFAULT_RDN_FORUM_ID
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"];
        //    }
        //}
        //public static string DEFAULT_RDN_FORUM_ID
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"];
        //    }
        //}
        //public static string DEFAULT_RDN_FORUM_ID
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"];
        //    }
        //}
        //public static string DEFAULT_RDN_FORUM_ID
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"];
        //    }
        //}
        //public static string DEFAULT_RDN_FORUM_ID
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"];
        //    }
        //}
        //public static string DEFAULT_RDN_FORUM_ID
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"];
        //    }
        //}
        //public static string DEFAULT_RDN_FORUM_ID
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"];
        //    }
        //}
        //public static string DEFAULT_RDN_FORUM_ID
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"];
        //    }
        //}



    }
}
