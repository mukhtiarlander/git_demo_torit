using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using RDN.Library.Classes.Site.Enums;
using RDN.Library.Cache;

namespace RDN.Library.Classes.Config
{
    public static class LibraryConfig
    {
         public static string PublicSite
        {
            get
            {
               // return ConfigurationManager.AppSettings["PublicSite"];
                return SiteCache.GetConfigurationValue("PublicSite");
            }
        }

        public static string WebsiteMainEmail
        {
            get
            {
               // return ConfigurationManager.AppSettings["WebsiteMainEmail"];
                return SiteCache.GetConfigurationValue("WebsiteMainEmail");
            }
        }

        public static string DefaultEmailSubject
        {
            get
            {
                return ConfigurationManager.AppSettings["DefaultEmailSubject"];
            }
        }

        public static string FacebookPageName2
        {
            get
            {
              //  return ConfigurationManager.AppSettings["FacebookPageName2"];
                return SiteCache.GetConfigurationValue("FacebookPageName2");
            }
        }

        public static string TwitterConsumerKey
        {
            get
            {
              //  return ConfigurationManager.AppSettings["TwitterConsumerKey"];
                return SiteCache.GetConfigurationValue("TwitterConsumerKey");
            }
        }
        public static string FacebookPageId2
        {
            get
            {
                //return ConfigurationManager.AppSettings["FacebookPageId2"];
                return SiteCache.GetConfigurationValue("FacebookPageId2");
            }
        }
        public static string FacebookUrl
        {
            get
            {
              //  return ConfigurationManager.AppSettings["FacebookUrl"];
                return SiteCache.GetConfigurationValue("FacebookUrl");
            }
        }

        public static string BlogSite
        {
            get
            {
                //return ConfigurationManager.AppSettings["BlogSite"];
                return SiteCache.GetConfigurationValue("BlogSite");
            }
        }

        public static string MainDomain
        {
            get
            {
               // return ConfigurationManager.AppSettings["MainDomain"];
                return SiteCache.GetConfigurationValue("MainDomain");
            }
        }

        public static string TwitterConsumerSecret
        {
            get
            {
                //return ConfigurationManager.AppSettings["TwitterConsumerSecret"];

                return SiteCache.GetConfigurationValue("TwitterConsumerSecret");
            }
        }
        public static string TwitterToken
        {
            get
            {
               // return ConfigurationManager.AppSettings["TwitterToken"];

                return SiteCache.GetConfigurationValue("TwitterToken");
            }
        }

        public static string TwitterTokenSecret
        {
            get
            {
               // return ConfigurationManager.AppSettings["TwitterTokenSecret"];
                return SiteCache.GetConfigurationValue("TwitterTokenSecret");
            }
        }

        public static string ThemeColors
        {
            get
            {
               //return ConfigurationManager.AppSettings["ThemeColors"];

                return SiteCache.GetConfigurationValue("ThemeColors");
            }
        }

        public static bool IsProduction
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["IsProduction"]);
               // return Convert.ToBoolean(SiteCache.GetConfigurationValue("IsProduction"));
            }
        }

        public static string DefaultPictureName
        {
            get
            {
                return ConfigurationManager.AppSettings["DefaultPictureName"];
            }
        }

        public static string ConnectionStringName
        {
            get
            {
              //  return ConfigurationManager.AppSettings["ConnectionStringName"];
                return SiteCache.GetConfigurationValue("ConnectionStringName");
            }
        }

        public static string InternalSite
        {
            get
            {
            //    return ConfigurationManager.AppSettings["InternalSite"];
            //{   
                return SiteCache.GetConfigurationValue("InternalSite");
            }
        }

        public static string ImagesBaseUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ImagesBaseUrl"];
            }
        }


        public static string ImagesBaseSaveLocation
        {
            get
            {
                return ConfigurationManager.AppSettings["ImagesBaseSaveLocation"];
            }
        }


        public static string ApiSite
        {
            get
            {
               // return ConfigurationManager.AppSettings["ApiSite"];
                return SiteCache.GetConfigurationValue("ApiSite");
            }
        }

        public static string AdminSite
        {
            get
            {
            // return ConfigurationManager.AppSettings["AdminSite"];
            //{  
                return SiteCache.GetConfigurationValue("AdminSite");
            }
        }

        public static SiteType SiteType
        {
            get
            {
                return (SiteType) Enum.Parse(typeof(SiteType), ConfigurationManager.AppSettings["SiteType"]);
              //  return (SiteType) Enum.Parse(typeof(SiteType), SiteCache.GetConfigurationValue("SiteType"));
            }
        }

        public static string StripeApiPublicKey
        {
            get
            {
               // return ConfigurationManager.AppSettings["StripeApiPublicKey"];
                return SiteCache.GetConfigurationValue("StripeApiPublicKey");
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

        public static string WebsiteName
        {
            get
            {
                //return ConfigurationManager.AppSettings["WebsiteName"];
                return SiteCache.GetConfigurationValue("WebsiteName");
            }
        }

        public static string ListLoadingCount
        {
            get
            {
                //return ConfigurationManager.AppSettings["ListLoadingCount"];
                return SiteCache.GetConfigurationValue("ListLoadingCount");
            }
        }

        public static string LeagueUrl
        {
            get
            {
                //return ConfigurationManager.AppSettings["LeagueUrl"];
                return SiteCache.GetConfigurationValue("LeagueUrl");
            }
        }

        public static string SportName
        {
            get
            {
                //return ConfigurationManager.AppSettings["SportName"];
                return SiteCache.GetConfigurationValue("SportName");
            }
        }

        public static string WebsiteShortName
        {
            get
            {
               // return ConfigurationManager.AppSettings["WebsiteShortName"];
                return SiteCache.GetConfigurationValue("WebsiteShortName");
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
              //  return ConfigurationManager.AppSettings["WebsiteSlogan"];
                return SiteCache.GetConfigurationValue("WebsiteSlogan");
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
              //  return ConfigurationManager.AppSettings["DefaultThemeColor"];

                return SiteCache.GetConfigurationValue("DefaultThemeColor");
            }
        }

        public static string EventPublicUrl
        {
            get
            {
               // return ConfigurationManager.AppSettings["EventPublicUrl"];
                return SiteCache.GetConfigurationValue("EventPublicUrl");
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
               // return SiteCache.GetConfigurationValue("NameOfMembers");
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
               // return ConfigurationManager.AppSettings["ShopSite"];
                return SiteCache.GetConfigurationValue("ShopSite");
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
              //  return ConfigurationManager.AppSettings["StripeApiKey"];
              
                return SiteCache.GetConfigurationValue("StripeApiKey");
            }
        }

        public static string NameOfMember
        {
            get
            {
               // return ConfigurationManager.AppSettings["NameOfMember"];
                return SiteCache.GetConfigurationValue("NameOfMember");
            }
        }

        public static string MemberName
        {
            get
            {
            //    return ConfigurationManager.AppSettings["MemberName"];
                return SiteCache.GetConfigurationValue("MemberName");
            }
        }

        public static string ApiKey
        {
            get
            {
                return ConfigurationManager.AppSettings["ApiKey"];
            }
        }

        public static string DefaultInfoEmail
        {
            get
            {
                //return ConfigurationManager.AppSettings["DefaultInfoEmail"];

                return SiteCache.GetConfigurationValue("DefaultInfoEmail");
            }
        }

        public static string SupportSite
        {
            get
            {
               // return ConfigurationManager.AppSettings["SupportSite"];
                return SiteCache.GetConfigurationValue("SupportSite");
            }
        }

        public static string DefaultAdminEmail
        {
            get
            {
               // return ConfigurationManager.AppSettings["DefaultAdminEmailAdmin"];

                return SiteCache.GetConfigurationValue("DefaultAdminEmailAdmin");
            }
        }

        public static string DefaultPhoneNumber
        {
            get
            {
               // return ConfigurationManager.AppSettings["DefaultPhoneNumber"];

                return SiteCache.GetConfigurationValue("DefaultPhoneNumber");
            }
        }

        public static string DefaultKrisWorlidgeEmailAdmin
        {
            get
            {
               // return ConfigurationManager.AppSettings["DefaultKrisWorlidgeEmailAdmin"];

                return SiteCache.GetConfigurationValue("DefaultKrisWorlidgeEmailAdmin");
            }
        }

        public static string DefaultEmailFromName
        {
            get
            {
                //return ConfigurationManager.AppSettings["DefaultEmailFromName"];

                return SiteCache.GetConfigurationValue("DefaultEmailFromName");
            }
        }

        public static string DefaultEmailMessage
        {
            get
            {
             //   return ConfigurationManager.AppSettings["DefaultEmailMessage"];
                return SiteCache.GetConfigurationValue("DefaultEmailMessage");
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

        public static Guid SiteStoreID
        {
            get
            {
                return new Guid(ConfigurationManager.AppSettings["SiteStoreID"]);
            }
        }
        
        public static string CSS_VERSION
        {
            get
            {
                return ConfigurationManager.AppSettings["CSS_VERSION"];
            }
        }

        public static string DocumentsSaveFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["DocumentsSaveFolder"];
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
                return SiteCache.GetConfigurationValue("WikiSite");
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
                return SiteCache.GetConfigurationValue("DataFolder");
            }
        }
        public static string ImageFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["ImageFolder"];
            }
        }


        public static string SportNameForUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["SportNameForUrl"];
            }
        }
        public static string SportNamePlusMemberNameForUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["SportNamePlusMemberNameForUrl"];
            }
        }

        public static string SportNamePlusMembersNameForUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["SportNamePlusMembersNameForUrl"];
            }
        }
    }
}
