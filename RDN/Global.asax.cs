using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.DatabaseInitializers;
using RDN.Utilities.Config;
using StackExchange.Profiling;
using RDN.Portable.Config;
using RDN.Library.Cache.Singletons;
using System.Configuration;
using StackExchange.Profiling.EntityFramework6;

namespace RDN
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //try
            //{
            //    if (!HttpContext.Current.Request.IsLocal)
            //    {
            //        filters.Add(new RequireHttpsAttribute());
            //    }
            //}
            //catch
            //{ }
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region Paywalls
            routes.MapRoute(
              "paywallReceiptForStreaming", // Route name
              "streaming/receipt/{id}", // URL with parameters
              new { controller = "Paywall", action = "ViewPaywallReceipt" }, // Parameter defaults
              new { id = "[a-zA-Z0-9]+" }
          );
            routes.MapRoute(
           "embedForStreaming", // Route name
           "embed/tournament/{id}", // URL with parameters
           new { controller = "Iframe", action = "StreamingVideo" }, // Parameter defaults
           new { id = "[a-zA-Z0-9]+" }
       );
            #endregion

            #region Games
            routes.MapRoute(
              "tournamentHome", // Route name
              "roller-derby-tournament/{id}/{name}", // URL with parameters
              new { controller = "Tournament", action = "TournamentHome" }, // Parameter defaults
              new { id = "[a-zA-Z0-9]+" }
          );
            routes.MapRoute(
          "tournaments", // Route name
          "roller-derby-tournaments", // URL with parameters
          new { controller = "Tournament", action = "Tournaments" } // Parameter defaults
                );

            routes.MapRoute(
              "apiGame", // Route name
              "game/api/{id}", // URL with parameters
              new { controller = "Game", action = "GameApi" }, // Parameter defaults
              new { id = "[a-zA-Z0-9]+" }
          );
            routes.MapRoute(
           "apiGameCurrent", // Route name
           "game/currentgames", // URL with parameters
           new { controller = "Game", action = "CurrentGames" } // Parameter defaults
                  );
            routes.MapRoute(
      "picturesOfGame", // Route name
      "game/picturesOfGame/{id}", // URL with parameters
      new { controller = "Game", action = "PicturesOfGame" }, // Parameter defaults
      new { id = "[a-zA-Z0-9]+" }
  );

            routes.MapRoute(
           "getConversationGame", // Route name
           "game/picturesOfGame/{id}", // URL with parameters
           new { controller = "Game", action = "GetConversation" }, // Parameter defaults
           new { id = "[a-zA-Z0-9]+" }
       );

            routes.MapRoute(
              "Gameid", // Route name
              "roller-derby-game/{id}", // URL with parameters
              new { controller = "Game", action = "Index" }, // Parameter defaults
              new { id = "[a-zA-Z0-9]+" }
          );

            routes.MapRoute(
                "Game", // Route name
                "roller-derby-game/{id}/{gameName}/{team1}/{team2}", // URL with parameters
                new { controller = "Game", action = "Index", team1 = UrlParameter.Optional, team2 = UrlParameter.Optional }, // Parameter defaults
                new { id = "[a-zA-Z0-9]+" }
            );


            routes.MapRoute(
                "Games", // Route name
                "roller-derby-games", // URL with parameters
                new { controller = "Games", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
    "Games2", // Route name
    "roller-derby-game", // URL with parameters
    new { controller = "Games", action = "Index", id = UrlParameter.Optional } // Parameter defaults
);

            routes.MapRoute(
    "OfficialsInLeague", // Route name
    "officiating-requests/view/all", // URL with parameters
    new { controller = "Games", action = "OfficiatingRequests" } // Parameter defaults
);
            routes.MapRoute(
   "ViewRequest", // Route name
   "officiating-requests/view/{id}", // URL with parameters
   new { controller = "Games", action = "ViewRequest"} // Parameter defaults
);
            routes.MapRoute(
   "ViewAllChallenges", // Route name
   "bout-challenge/view/all", // URL with parameters
   new { controller = "Games", action = "AllBoutList" } // Parameter defaults
);
            routes.MapRoute(
   "ViewChallengeDetails", // Route name
   "bout-challenge/view/{ChallengeId}", // URL with parameters
   new { controller = "Games", action = "ViewBoutEvent"} // Parameter defaults
);




            #endregion

            #region homeController

            routes.MapRoute(
                "PrivacyController", // Route name
                "privacy", // URL with parameters
                new { controller = "Home", action = "Privacy" } // Parameter defaults
            );
            routes.MapRoute(
             "HiringController", // Route name
             "hiring", // URL with parameters
             new { controller = "Home", action = "Hiring" } // Parameter defaults
         );
            routes.MapRoute(
            "PricingController", // Route name
            "pricing", // URL with parameters
            new { controller = "Home", action = "Pricing" } // Parameter defaults
        );

            routes.MapRoute(
        "scoreboard", // Route name
        "roller-derby-scoreboard", // URL with parameters
        new { controller = "Scoreboard", action = "Index" } // Parameter defaults
    );
            routes.MapRoute(
    "scoreboard2", // Route name
    "scoreboard", // URL with parameters
    new { controller = "Scoreboard", action = "Index" } // Parameter defaults
    );
            routes.MapRoute(
            "About", // Route name
            "about/{action}", // URL with parameters
            new { controller = "About", action = "Index" } // Parameter defaults
        );
            routes.MapRoute(
     "Sitemap", // Route name
     "sitemap.xml", // URL with parameters
     new { controller = "Home", action = "Sitemap" } // Parameter defaults
 );
            //I have no idea what a cross domain is for, im just cheating here and using our sitemap instead cause I don't care.
            routes.MapRoute(
"CrossDomain", // Route name
"crossdomain.xml", // URL with parameters
new { controller = "Home", action = "Sitemap" } // Parameter defaults
);
            routes.MapRoute(
        "SignUp", // Route name
        "signupbeta", // URL with parameters
        new { controller = "Home", action = "SignUpBeta" } // Parameter defaults
    );
            routes.MapRoute(
"Error", // Route name
"problem.error", // URL with parameters
new { controller = "Home", action = "Error" } // Parameter defaults
);
            routes.MapRoute(
            "lostPassword", // Route name
            "lostPassword/{code}/{email}", // URL with parameters
            new { controller = "Home", action = "LostPassword" } // Parameter defaults
        );
            routes.MapRoute(
                        "verifyDerbyName", // Route name
                        "verifyderbyname/{id}/{name}", // URL with parameters
                        new { controller = "Home", action = "VerifyDerbyName", name = UrlParameter.Optional } // Parameter defaults
                    );

            routes.MapRoute(
                      "blogPosts", // Route name
                      "post/{id}/{name}", // URL with parameters
                      new { controller = "Home", action = "BlogPosts" },
                      new { id = @"[0-9]+" } // Parameter defaults
                  );


            #endregion

            #region CalendarRoutes
            routes.MapRoute(
         "CalendarEvent", // Route name
         "roller-derby-event/{name}/{id}", // URL with parameters
         new { controller = "Calendar", action = "EventCalendar" } // Parameter defaults
                 );
            routes.MapRoute(
      "CalendarEvents", // Route name
      "roller-derby-events/{year}/{month}", // URL with parameters
      new { controller = "Calendar", action = "CalendarEvents", year = UrlParameter.Optional, month = UrlParameter.Optional } // Parameter defaults
              );
            #endregion

            #region PublicProfiles
            routes.MapRoute(
            "AllLogos", // Route name
            "roller-derby-logos", // URL with parameters
            new { controller = "PublicLogos", action = "AllLogos" } // Parameter defaults
                    );

            
            routes.MapRoute(
            "AllSkaters", // Route name
            "roller-derby-skaters", // URL with parameters
            new { controller = "PublicProfile", action = "AllSkaters" } // Parameter defaults
                    );
            routes.MapRoute(
          "PublicSkaterRedirect", // Route name
          "roller-derby-skater/{id}", // URL with parameters
          new { controller = "PublicProfile", action = "SkaterRedirect" } // Parameter defaults
                  );
            routes.MapRoute(
            "PublicSkater", // Route name
            "roller-derby-skater/{name}/{id}", // URL with parameters
            new { controller = "PublicProfile", action = "Skater" } // Parameter defaults
                    );
           
            routes.MapRoute(
           "PublicSkaterTwoEvils", // Route name
           "roller-derby-skater/1/{name}/{id}", // URL with parameters
           new { controller = "PublicProfile", action = "SkaterTwoEvils" } // Parameter defaults
                   );


            routes.MapRoute(
            "AllRefs", // Route name
            "roller-derby-referees", // URL with parameters
            new { controller = "PublicProfile", action = "AllRefs" } // Parameter defaults
                    );

            routes.MapRoute(
            "PublicRef", // Route name
            "roller-derby-referee/{name}/{id}", // URL with parameters
            new { controller = "PublicProfile", action = "Ref", league = UrlParameter.Optional } // Parameter defaults
                    );
            routes.MapRoute(
            "PublicFederation", // Route name
            "roller-derby-federation/{name}/{id}", // URL with parameters
            new { controller = "PublicFederation", action = "Federation" } // Parameter defaults
                    );
            routes.MapRoute(
            "PublicFederations", // Route name
            "roller-derby-federations", // URL with parameters
            new { controller = "PublicFederation", action = "AllFederations" } // Parameter defaults
                    );

            routes.MapRoute(
            "PublicLeagues", // Route name
            "roller-derby-leagues", // URL with parameters
            new { controller = "PublicLeague", action = "AllLeagues" } // Parameter defaults
                    );

            // routes.MapRoute(
            //"PublicLeaguesPager", // Route name
            //"roller-derby-leagues/{page}", // URL with parameters
            //new { controller = "PublicLeague", action = "AllLeagues" } // Parameter defaults
            //        );
            routes.MapRoute(
            "RegularDerbyLeague", // Route name
            "roller-derby-league/{name}/{id}", // URL with parameters
            new { controller = "PublicLeague", action = "League" } // Parameter defaults
                    );
            routes.MapRoute(
            "PublicLeagueTwoEvils", // Route name
            "roller-derby-league/1/{name}/{id}", // URL with parameters
            new { controller = "PublicLeague", action = "LeagueTwoEvils" } // Parameter defaults
                    );

            routes.MapRoute(
                        "PublicLeagueDerbyRoster", // Route name
                        "roller-derby-league/2/{name}/{id}", // URL with parameters
                        new { controller = "PublicLeague", action = "LeagueDerbyRoster" } // Parameter defaults
                                );





            #endregion

            #region payments

            routes.MapRoute(
           "RDContentPayments", // Route name
           "payments/rncontent", // URL with parameters
           new { controller = "Payments", action = "RNContent" } // Parameter defaults
                   );

            #endregion

            routes.MapRoute(
             "Default", // Route name
             "{action}", // URL with parameters
             new { controller = "Home", action = "Index" } // Parameter defaults
         );

            routes.MapRoute(
                "DefaultController", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            #region Bashregion


            routes.MapRoute(
 "BruiseBashDefault", // Route name
 "bruisebash", // URL with parameters
 new { controller = "BruiseBash", action = "Index" } // Parameter defaults
 );
            routes.MapRoute(
                //bruisebase/about
"BruiseBash", // Route name
"bruisebash/{action}", // URL with parameters
new { controller = "BruiseBash", action = "Index" } // Parameter defaults
);

            routes.MapRoute(
                //bruisebase/about
"BruiseBashViewBruise", // Route name
"bruisebash/bruise/{id}/{title}", // URL with parameters
new { controller = "BruiseBash", action = "ViewBruise" } // Parameter defaults
);

            #endregion

        }

        protected void Application_Start()
        {

            MiniProfilerEF6.Initialize();
            ErrorInitializer.Initialize();
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            
            //SiteCache.StartSite(HttpContext.Current.Cache);
            SiteSingleton.Instance.IsProduction = Convert.ToBoolean(ConfigurationManager.AppSettings["IsProduction"].ToString());
        }
        void Application_Error(object sender, EventArgs e)
        {
            string url = HttpContext.Current.Request.Url.AbsoluteUri;
            // Code that runs when an unhandled error occurs
            if (!url.Contains("wiki.rdnation.com") && !url.Contains("Utilities/AddNodeToSiteMap") && !url.Contains("com/member/register") && !url.Contains(".php") && !url.Contains(".cgi") && !url.Contains(".asp") && !url.Contains("blogs/load/recent") && !url.Contains("user/register") && !url.Contains("Scoreboard/Download"))
            {
                // Get the exception object.
                Exception exc = Server.GetLastError();
                ErrorDatabaseManager.AddException(exc, GetType(), additionalInformation: "Application Error");
            }
            Response.Redirect(ServerConfig.WEBSITE_DEFAULT_LOCATION);
        }
        protected void Application_BeginRequest()
        {
            if (Request.IsLocal)
            {
                MiniProfiler.Start();
            }
            else
            {
                if (!Context.Request.IsSecureConnection)
                    Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));

            }
        }
        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }
    }
}