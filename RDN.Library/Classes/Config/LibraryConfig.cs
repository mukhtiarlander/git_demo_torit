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
                return SiteCache.GetConfigurationValue("PublicSite");
            }
        }

        public static string WebsiteMainEmail
        {
            get
            {
                return SiteCache.GetConfigurationValue("WebsiteMainEmail");
            }
        }

        public static string DefaultEmailSubject
        {
            get
            {               
                return SiteCache.GetConfigurationValue("DefaultEmailSubject");
            }
        }

        public static string FacebookPageName2
        {
            get
            {
                return SiteCache.GetConfigurationValue("FacebookPageName2");
            }
        }

        public static string TwitterConsumerKey
        {
            get
            {                
                return SiteCache.GetConfigurationValue("TwitterConsumerKey");
            }
        }
        public static string FacebookPageId2
        {
            get
            {   
                return SiteCache.GetConfigurationValue("FacebookPageId2");
            }
        }
        public static string FacebookUrl
        {
            get
            {   
                return SiteCache.GetConfigurationValue("FacebookUrl");
            }
        }

        public static string BlogSite
        {
            get
            {
                return SiteCache.GetConfigurationValue("BlogSite");
            }
        }

        public static string MainDomain
        {
            get
            {   
                return SiteCache.GetConfigurationValue("MainDomain");
            }
        }

        public static string GoogleAnalyticsTrackingId
        {
            get
            {
                return SiteCache.GetConfigurationValue("GoogleAnalyticsTrackingId");
            }
        }

        public static string TwitterConsumerSecret
        {
            get
            {                
                return SiteCache.GetConfigurationValue("TwitterConsumerSecret");
            }
        }
        public static string TwitterToken
        {
            get
            {
               return SiteCache.GetConfigurationValue("TwitterToken");
            }
        }

        public static string TwitterTokenSecret
        {
            get
            {   
                return SiteCache.GetConfigurationValue("TwitterTokenSecret");
            }
        }

        public static string ThemeColors
        {
            get
            {
                return SiteCache.GetConfigurationValue("ThemeColors");
            }
        }

        public static bool IsProduction
        {
            get
            { 
                return Convert.ToBoolean(ConfigurationManager.AppSettings["IsProduction"]);
            }
        }

        public static string DefaultPictureName
        {
            get
            {  
                return SiteCache.GetConfigurationValue("DefaultPictureName");
            }
        }
        public static string ConnectionStringName
        {
            get
            {
                return SiteCache.GetConfigurationValue("ConnectionStringName");             
            }
        }

        public static string InternalSite
        {
            get
            {
                return SiteCache.GetConfigurationValue("InternalSite");     
            }
        }

        public static string ImagesBaseUrl
        {
            get
            {
               return SiteCache.GetConfigurationValue("ImagesBaseUrl");     
            }
        }


        public static string ImagesBaseSaveLocation
        {
            get
            {
                return SiteCache.GetConfigurationValue("ImagesBaseSaveLocation");                
            }
        }


        public static string ApiSite
        {
            get
            {
                return SiteCache.GetConfigurationValue("ApiSite");                  
            }
        }

        public static string AdminSite
        {
            get
            {   
                return SiteCache.GetConfigurationValue("AdminSite");   
            }
        }

        public static SiteType SiteType
        {
            get
            {
                return (SiteType)Enum.Parse(typeof(SiteType), SiteCache.GetConfigurationValue("SiteType"));
            }
        }

        public static string StripeApiPublicKey
        {
            get
            { 
                return SiteCache.GetConfigurationValue("StripeApiPublicKey");     
            }
        }
        public static string StripeConnectKey
        {
            get
            {
                return SiteCache.GetConfigurationValue("StripeConnectKey");                   
            }
        }

        public static string PaymentMode
        {
            get
            {
                return SiteCache.GetConfigurationValue("PaymentMode");                  
            }
        }

        public static string WebsiteName
        {
            get
            {
                return SiteCache.GetConfigurationValue("WebsiteName");                 
            }
        }

        public static string ListLoadingCount
        {
            get
            {
                return SiteCache.GetConfigurationValue("ListLoadingCount");                   
            }
        }

        public static string LeagueUrl
        {
            get
            {
                return SiteCache.GetConfigurationValue("LeagueUrl");                      
            }
        }

        public static string LeaguesUrl
        {
            get
            {
                return SiteCache.GetConfigurationValue("LeaguesUrl");
            }
        }

        public static string TeamsName       {
            get
            {
                return SiteCache.GetConfigurationValue("TeamsName");
            }
        }


        public static string SportName
        {
            get
            {
                return SiteCache.GetConfigurationValue("SportName");                    
            }
        }

        public static string WebsiteShortName
        {
            get
            {
                return SiteCache.GetConfigurationValue("WebsiteShortName");                     
            }
        }

        public static string CompanyAddress
        {
            get
            {
                return SiteCache.GetConfigurationValue("CompanyAddress");   
            }
        }

        public static string EmailSignature
        {
            get
            {
                return SiteCache.GetConfigurationValue("EmailSignature");                   
            }
        }

        public static string WebsiteSlogan
        {
            get
            {
                return SiteCache.GetConfigurationValue("WebsiteSlogan");                   
            }
        }

        public static string TournamentPublicUrl
        {
            get
            {
                return SiteCache.GetConfigurationValue("TournamentPublicUrl");  
            }
        }

        public static string DefaultThemeColor
        {
            get
            {
                return SiteCache.GetConfigurationValue("DefaultThemeColor");  
            }
        }

        public static string EventPublicUrl
        {
            get
            {
                return SiteCache.GetConfigurationValue("EventPublicUrl");  
            }
        }

        public static string LeaguesPublicUrl
        {
            get
            {
                return SiteCache.GetConfigurationValue("LeaguesPublicUrl");                 
            }
        }

        public static string MembersPublicUrl
        {
            get
            {
                return SiteCache.GetConfigurationValue("MembersPublicUrl");                
            }
        }

        public static string NameOfMembers
        {
            get
            {               
                return SiteCache.GetConfigurationValue("NameOfMembers");    
            }
        }

        public static string TournamentsPublicUrl
        {
            get
            {
                return SiteCache.GetConfigurationValue("TournamentsPublicUrl");                
            }
        }

        public static string ShopSite
        {
            get
            {
                return SiteCache.GetConfigurationValue("ShopSite");                   
            }
        }

        public static string MemberPublicUrl
        {
            get
            {
                return SiteCache.GetConfigurationValue("MemberPublicUrl");                 
            }
        }


        public static string LogoUrl
        {
            get
            {
                return SiteCache.GetConfigurationValue("LogoURL");                     
            }
        }

        public static string LogoUrl500
        {
            get
            {
                return SiteCache.GetConfigurationValue("LogoUrl500");
            }
        }

        public static string LogoUrl250
        {
            get
            {
                return SiteCache.GetConfigurationValue("LogoUrl250");
            }
        }

        public static string FaviIcon
        {
            get
            {
                return SiteCache.GetConfigurationValue("FaviIcon");
            }
        }

        
        public static string SiteEmail
        {
            get
            {
                return SiteCache.GetConfigurationValue("SiteEmail");                      
            }
        }

        public static string FromEmailName
        {
            get
            {
                return SiteCache.GetConfigurationValue("FromEmailName");                  
            }
        }

        public static string StripeApiKey
        {
            get
            {
                return SiteCache.GetConfigurationValue("StripeApiKey");                 
            }
        }

        public static string NameOfMember
        {
            get
            {
                return SiteCache.GetConfigurationValue("NameOfMember");                
            }
        }

        public static string MemberName
        {
            get
            {
                return SiteCache.GetConfigurationValue("MemberName");                  
            }
        }

        public static string ApiKey
        {
            get
            {
                return SiteCache.GetConfigurationValue("ApiKey");                       
            }
        }

        public static string DefaultInfoEmail
        {
            get
            {
                return SiteCache.GetConfigurationValue("DefaultInfoEmail");               
            }
        }

        public static string SupportSite
        {
            get
            {
                return SiteCache.GetConfigurationValue("SupportSite");              
            }
        }

        public static string DefaultAdminEmail
        {
            get
            {
               return SiteCache.GetConfigurationValue("DefaultAdminEmail");                             
            }
        }

        public static string PaypalPaymentEmail
        {
            get
            {
                return SiteCache.GetConfigurationValue("PaypalPaymentEmail");
            }
        }

        public static string DefaultPhoneNumber
        {
            get
            {
                return SiteCache.GetConfigurationValue("DefaultPhoneNumber");                             
            }
        }

        public static string DefaultKrisWorlidgeEmailAdmin
        {
            get
            {
                return SiteCache.GetConfigurationValue("DefaultKrisWorlidgeEmailAdmin"); 
            }
        }

        public static string DefaultEmailFromName
        {
            get
            {
                return SiteCache.GetConfigurationValue("DefaultEmailFromName"); 
            }
        }

        public static string DefaultEmailMessage
        {
            get
            {
                return SiteCache.GetConfigurationValue("DefaultEmailMessage");              
            }
        }

        public static string TextMessageEmail
        {
            get
            {
                return SiteCache.GetConfigurationValue("TextMessageEmail");                             
            }
        }
        public static string AdminPhoneNumber
        {
            get
            {
                return SiteCache.GetConfigurationValue("AdminPhoneNumber");                             
            }
        }
        public static string WebsiteTitleDefault
        {
            get
            {
                return SiteCache.GetConfigurationValue("WebsiteTitleDefault");                                           
            }
        }

        public static string WebsiteSloganDefault
        { 
            get
            {
                return SiteCache.GetConfigurationValue("WebsiteSloganDefault");                  
            }
        }

        public static string WebsiteTitleDefaultStore
        {
            get
            {
                return SiteCache.GetConfigurationValue("WebsiteTitleDefaultStore");                  
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
                return SiteCache.GetConfigurationValue("PaypalIPNHandler"); 
            }
        }

        public static Guid DEFAULT_SCOTTS_USER_ID
        {
            get
            {
                return new Guid(SiteCache.GetConfigurationValue("DEFAULT_SCOTTS_USER_ID"));
            }
        }
        public static Guid DEFAULT_JAMIES_USER_ID
        {
            get
            {
                return new Guid(SiteCache.GetConfigurationValue("DEFAULT_JAMIES_USER_ID"));
            }
        }
        public static Guid DEFAULT_ADMIN_USER_ID
        {
            get
            {
                return new Guid(SiteCache.GetConfigurationValue("DEFAULT_ADMIN_USER_ID"));
            }
        }
        public static Guid DEFAULT_RDN_FORUM_ID
        {
            get
            {
                return new Guid( SiteCache.GetConfigurationValue("DEFAULT_RDN_FORUM_ID"));
            }
        }

        public static Guid SiteStoreID
        {
            get
            {
                return new Guid(SiteCache.GetConfigurationValue("SiteStoreID"));
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
                return SiteCache.GetConfigurationValue("DocumentsSaveFolder");          
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
                return SiteCache.GetConfigurationValue("SportNameForUrl");
            }
        }

        public static string SportNamePlusMemberNameForUrl
        {
            get
            {
                return SiteCache.GetConfigurationValue("SportNamePlusMemberNameForUrl");
            }
        }

        public static string SportNamePlusMembersNameForUrl
        {
            get
            {
                return SiteCache.GetConfigurationValue("SportNamePlusMembersNameForUrl");
            }
        }


        public static string GameUrl
        {
            get
            {
                return SiteCache.GetConfigurationValue("GameUrl");
            }
        }


        public static string GamesUrl
        {
            get
            {
                return SiteCache.GetConfigurationValue("GamesUrl");
            }
        }


        public static string RefereeUrl
        {
            get
            {
                return SiteCache.GetConfigurationValue("RefereeUrl");
            }
        }


        public static string RefereesUrl
        {
            get
            {
                return SiteCache.GetConfigurationValue("RefereesUrl");
            }
        }

        public static string GamesName
        {
            get
            {
                return SiteCache.GetConfigurationValue("GamesName");
            }
        }
        public static string GameName
        {
            get
            {
                return SiteCache.GetConfigurationValue("GameName");
            }
        }
        public static string ImageSlidesLocation
        {
            get
            {
                return SiteCache.GetConfigurationValue("ImageSlidesLocation");
            }
        }


    }
}
