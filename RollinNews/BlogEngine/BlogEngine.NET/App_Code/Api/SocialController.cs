using BlogEngine.Core;
using BlogEngine.Core.Data;
using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using Newtonsoft.Json;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Facebook;
using RDN.Library.Classes.Facebook.Classes;
using RDN.Library.Classes.Facebook.Enum;
using RDN.Library.Classes.Game;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.RN.Funds;
using RDN.Library.Classes.Social.Facebook;
using RDN.Library.Classes.Social.Twitter;
using RDN.Portable.Classes.Games;
using RDN.Portable.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using System.Linq;

public class SocialController : ApiController
{

    public SocialController()
    {
    }

    public FacebookMessage Get()
    {
        try
        {
            return new FacebookMessage();
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
    public HttpResponseMessage Put([FromBody]FacebookMessage message, string action)
    {
        try
        {

            if (action == "updateLongTermToken")
            {
                var token = FacebookFactory.Initialize(message.UserAccessToken)
                           .GetLongTermAccessToken(ConfigurationManager.AppSettings["FacebookAppId"].ToString(), ConfigurationManager.AppSettings["FacebookAppIdSecret"].ToString());

                if (!String.IsNullOrEmpty(token.LongTermAccessToken))
                {
                    message.UserAccessToken = token.LongTermAccessToken;
                    //the access token will time to time not have an expire listed on it.  So we only insert when we know it expires.
                    //if (token.LongTermAccessTokenExpires > DateTime.UtcNow)
                    FacebookData.InsertNewAccessToken(token.LongTermAccessToken, token.LongTermAccessTokenExpires);
                }
            }
            else if (action == "postToFacebook")
            {
                FacebookFactory.Initialize(message.UserAccessToken)
                    .GetPageAuthorization(ConfigurationManager.AppSettings["FacebookPageName"].ToString(), ConfigurationManager.AppSettings["FacebookPageId"].ToString())
                    .PostToFanPage(message.Message, message.Link, message.PictureUrl, message.Name, message.Caption, message.DateForMessage);



            }
            //bitcoinWithdraw
            else if (action == "saveAndPostToFacebook")
            {
                ManualGame score = new ManualGame();
                score.Team1Name = message.Team1Name;
                if (!String.IsNullOrEmpty(message.Team1Score))
                    score.Team1Score = Convert.ToInt32(message.Team1Score);
                score.Team2Name = message.Team2Name;
                if (!String.IsNullOrEmpty(message.Team2Score))
                    score.Team2Score = Convert.ToInt32(message.Team2Score);
                if (!String.IsNullOrEmpty(message.DateOfGame))
                    score.TimeEntered = Convert.ToDateTime(message.DateOfGame);
                else
                    score.TimeEntered = DateTime.UtcNow;
                GameFactory.Initialize().AddRNScore(score).SaveChanges();

                SiteCache.ClearPastGames();
                if (!String.IsNullOrEmpty(message.UserAccessToken))
                {
                    FacebookFactory.Initialize(message.UserAccessToken)
                        .GetPageAuthorization(ConfigurationManager.AppSettings["FacebookPageName"].ToString(), ConfigurationManager.AppSettings["FacebookPageId"].ToString())
                            .AddScores(message.Team1Name, message.Team1Score, message.Team2Name, message.Team2Score)
                            .PostToFanPage(message.Message, message.Link, message.PictureUrl, message.Name, message.Caption, message.DateForMessage);
                }
            }
            else if (action == "postScoreToFacebook")
            {

                FacebookFactory.Initialize(message.UserAccessToken)
                    .GetPageAuthorization(ConfigurationManager.AppSettings["FacebookPageName"].ToString(), ConfigurationManager.AppSettings["FacebookPageId"].ToString())
                    .AddScores(message.Team1Name, message.Team1Score, message.Team2Name, message.Team2Score)
                    .PostToFanPage(message.Message, message.Link, message.PictureUrl, message.Name, message.Caption, message.DateForMessage);



            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
        catch (UnauthorizedAccessException)
        {
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, GetType(), additionalInformation: JsonConvert.SerializeObject(message));
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    public HttpResponseMessage PublishPostToSocial(string r)
    {
        try
        {
            var dateAfter = DateTime.UtcNow.AddDays(-90);
            PostRepository repo = new PostRepository();
            var posts = repo.FindPublic(1000).Select(x => x.Id).ToList();

            var postId = FacebookData.GetRandomPostToSocialize(posts, dateAfter);
            var token = FacebookData.GetLatestAccessToken();
            if (String.IsNullOrEmpty(token))
                FacebookData.ResetFacebookConnection();
            else
            {
                var post = repo.FindById(postId);
                string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/');
                
                FacebookFactory.Initialize(token)
                                    .GetPageAuthorization(ConfigurationManager.AppSettings["FacebookPageName"].ToString(), ConfigurationManager.AppSettings["FacebookPageId"].ToString())
                                            .PostToFanPage(post.Title, baseUrl + post.RelativeLink, baseUrl + post.MainImageUrl, post.Title, post.Title, string.Empty);

                FacebookFactory.Initialize(token).GetPageAuthorization(ConfigurationManager.AppSettings["FacebookPageName2"].ToString(), ConfigurationManager.AppSettings["FacebookPageId2"].ToString())
                                .PostToFanPage(post.Title, baseUrl + post.RelativeLink, baseUrl + post.MainImageUrl, post.Title, post.Title, string.Empty);


                TwitterFactory.Initialize(ConfigurationManager.AppSettings["TwitterConsumerKey"].ToString(), ConfigurationManager.AppSettings["TwitterConsumerSecret"].ToString(), ConfigurationManager.AppSettings["TwitterToken"].ToString(), ConfigurationManager.AppSettings["TwitterTokenSecret"].ToString())
                           .SendMessage(post.Title + " #rollerderby " + baseUrl + post.RelativeLink);
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
