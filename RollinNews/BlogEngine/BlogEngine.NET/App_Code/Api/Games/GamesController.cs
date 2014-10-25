using BlogEngine.Core.Data;
using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Facebook;
using RDN.Library.Classes.Game;
using RDN.Library.Classes.RN.RSS;
using RDN.Library.Classes.Social.Facebook;
using RDN.Library.Classes.Social.Twitter;
using RDN.Portable.Classes.Federation.Enums;
using RDN.Portable.Classes.Games;
using RDN.Portable.Classes.RN;
using RDN.Portable.Models.Json.Games;
using RDN.Portable.Util.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

public class GamesController : ApiController
{


    public GamesController()
    {
        //this.repository = repository;
    }

    public GamesJson Get(int take = 10, int skip = 0, string filter = "", string order = "")
    {
        try
        {
            GamesJson game = new GamesJson();
            game.ManualGames = GameFactory.Initialize().GetManualGames(take, skip);
            return game;
        }
        catch (UnauthorizedAccessException)
        {
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, GetType());
            throw new HttpResponseException(HttpStatusCode.InternalServerError);
        }
    }

    public HttpResponseMessage Get(string id)
    {
        try
        {
            var result = GameFactory.Initialize().GetManualGame(Convert.ToInt64(id));
            if (result == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        catch (UnauthorizedAccessException)
        {
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, GetType());
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    public HttpResponseMessage Post([FromBody]ManualGame item)
    {
        try
        {
            if (!string.IsNullOrEmpty(item.SanctionedByFederationEnumDisplay))
                item.SanctionedByFederationEnum = (FederationsEnum)Enum.Parse(typeof(FederationsEnum), item.SanctionedByFederationEnumDisplay);
            if (!String.IsNullOrEmpty(item.RuleSetEnumDisplay))
                item.RuleSetEnum = (RuleSetsUsedEnum)Enum.Parse(typeof(RuleSetsUsedEnum), item.RuleSetEnumDisplay);

            GameFactory.Initialize().AddRNScore(item).SaveChanges();

            var token = FacebookData.GetLatestAccessToken();
            string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/');
            FacebookFactory.Initialize(token)
                    .GetPageAuthorization(ConfigurationManager.AppSettings["FacebookPageName"].ToString(), ConfigurationManager.AppSettings["FacebookPageId"].ToString())
                    .AddScores(item.Team1Name, item.Team1Score.ToString(), item.Team2Name, item.Team2Score.ToString())
                                                .PostToFanPage("#derbyscores " + baseUrl, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty);

            TwitterFactory.Initialize(ConfigurationManager.AppSettings["TwitterConsumerKey"].ToString(), ConfigurationManager.AppSettings["TwitterConsumerSecret"].ToString(), ConfigurationManager.AppSettings["TwitterToken"].ToString(), ConfigurationManager.AppSettings["TwitterTokenSecret"].ToString())
                .AddScores(item.Team1Name, item.Team1Score.ToString(), item.Team2Name, item.Team2Score.ToString())
                .SendMessage(" #rollerderby #derbyscores " + baseUrl);



            return Request.CreateResponse(HttpStatusCode.Created, true);
        }
        catch (UnauthorizedAccessException)
        {
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, GetType());
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    [HttpPut]
    public HttpResponseMessage Update([FromBody]RSSFeedItem item)
    {
        try
        {
            RSSFactory.Initilize().UpdateFeed(item);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        catch (UnauthorizedAccessException)
        {
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, GetType());
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    public HttpResponseMessage RuleSets(string r)
    {
        try
        {
            return Request.CreateResponse(HttpStatusCode.OK, EnumExt.GetValues<RuleSetsUsedEnum>());
        }
        catch (UnauthorizedAccessException)
        {
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, GetType());
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    public HttpResponseMessage Federations(string r)
    {
        try
        {
            return Request.CreateResponse(HttpStatusCode.OK, EnumExt.GetValues<FederationsEnum>());
        }
        catch (UnauthorizedAccessException)
        {
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, GetType());
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    public HttpResponseMessage Delete(string id)
    {
        try
        {
            RSSFactory.Initilize().RemoveFeed(Convert.ToInt64(id));
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        catch (UnauthorizedAccessException)
        {
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, GetType());
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    [HttpPut]
    public HttpResponseMessage ProcessChecked([FromBody]List<RSSFeedItem> items)
    {
        try
        {
            if (items == null || items.Count == 0)
                throw new HttpResponseException(HttpStatusCode.ExpectationFailed);

            var action = Request.GetRouteData().Values["id"].ToString();

            if (action.ToLower() == "delete")
            {
                var fact = RSSFactory.Initilize();

                foreach (var item in items)
                {
                    if (item.IsChecked)
                    {
                        fact.RemoveFeed(item.FeedId);
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        catch (UnauthorizedAccessException)
        {
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, GetType());
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}
