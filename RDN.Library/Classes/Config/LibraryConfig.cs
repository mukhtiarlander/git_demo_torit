using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using RDN.Library.Classes.Site.Enums;

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
        public static string FacebookUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["FacebookUrl"];
            }
        }

        public static string BlogSite
        {
            get
            {
                return ConfigurationManager.AppSettings["BlogSite"];
            }
        }

        public static string MainDomain
        {
            get
            {
                return ConfigurationManager.AppSettings["MainDomain"];
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

        public static string ApiSite
        {
            get
            {
                return ConfigurationManager.AppSettings["ApiSite"];
            }
        }

        public static string AdminSite
        {
            get
            {
                return ConfigurationManager.AppSettings["AdminSite"];
            }
        }

        public static SiteType SiteType
        {
            get
            {
                return (SiteType) Enum.Parse(typeof(SiteType), ConfigurationManager.AppSettings["SiteType"]);
            }
        }

        public static string StripeApiPublicKey
        {
            get
            {
                return ConfigurationManager.AppSettings["StripeApiPublicKey"];
            }
        }
        public static string StripeConnectKey
        {
            get
            {
                return ConfigurationManager.AppSettings["StripeConnectKey"];
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

        public static string CompanyAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["CompanyAddress"];
            }
        }

        public static string EmailSignature
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailSignature"];
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

        public static string ShopSite
        {
            get
            {
                return ConfigurationManager.AppSettings["ShopSite"];
            }
        }

        public static string MemberPublicUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["MemberPublicUrl"];
            }
        }


        public static string LogoUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["LogoUrl"];
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
        //public static string LogoURL
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["LogoURL"];
        //    }
        //}

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
        public static Guid DEFAULT_RDN_FORUM_ID
        {
            get
            {
                return new Guid( ConfigurationManager.AppSettings["DEFAULT_RDN_FORUM_ID"]);
            }
        }















        public static Guid STORE_ID
        {
            get
            {
                return new Guid(ConfigurationManager.AppSettings["RDNATION_STORE_ID"]);
            }
        }
        
        public static string CSS_VERSION
        {
            get
            {
                return ConfigurationManager.AppSettings["CSS_VERSION"];
            }
        }
        
        
        
        public static string JS_VERSION
        {
            get
            {
                return ConfigurationManager.AppSettings["JS_VERSION"];
            }
        }
        
        public static string WikiSite
        {
            get
            {
                return ConfigurationManager.AppSettings["WikiSite"];
            }
        }
        
        
        
        /// <summary>
        /// website data folder
        /// </summary>
        public static string DataFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["DataFolder"];
            }
        }
        public static string ImageFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["ImageFolder"];
            }
        }
       



    }
}
