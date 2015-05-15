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

        public static string IsProduction
        {
            get
            {
                return ConfigurationManager.AppSettings["IsProduction"];
            }
        }


        public static string IsPayPalLive
        {
            get
            {
                return ConfigurationManager.AppSettings["IsPayPalLive"];
            }
        }

        public static string PaypalLiveTest
        {
            get
            {
                return ConfigurationManager.AppSettings["PaypalLiveTest"];
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
    }
}
