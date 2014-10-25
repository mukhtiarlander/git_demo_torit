using BlogEngine.Core.Data;
using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Utilities;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.RN.RSS;
using RDN.Portable.Classes.RN;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class RSSFeedController : ApiController
{


    public RSSFeedController()
    {
        //this.repository = repository;
    }

    public IEnumerable<RSSFeedItem> Get(int take = 2000, int skip = 0, string filter = "", string order = "")
    {
        try
        {
            return RSSFactory.Initilize().PullAllFeeds(take, skip);
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
            var result = RSSFactory.Initilize().PullFeed(Convert.ToInt64(id));
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

    public HttpResponseMessage Post([FromBody]RSSFeedItem item)
    {
        try
        {
            var result = RSSFactory.Initilize().AddNewFeed(item);
            if (result == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            return Request.CreateResponse(HttpStatusCode.Created, result);
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

    public HttpResponseMessage Scan(string r)
    {
        try
        {
            RSSRepository.ScanRSSFeeds();
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

    public HttpResponseMessage DownloadDNN(string s)
    {
        try
        {
            new DNNScraper().ScanAllPages();
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
