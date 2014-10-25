﻿using BlogEngine.Core.Data;
using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Game;
using RDN.Library.Classes.RN.RSS;
using RDN.Portable.Classes.Federation.Enums;
using RDN.Portable.Classes.Games;
using RDN.Portable.Classes.RN;
using RDN.Portable.Models.Json.Games;
using RDN.Portable.Util.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class FederationsController : ApiController
{


    public FederationsController()
    {
        //this.repository = repository;
    }

    public List<EnumClass> Get(int take = 10, int skip = 0, string filter = "", string order = "")
    {
        try
        {
            return EnumExt.ToList<FederationsEnum>();
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

    //public HttpResponseMessage Get(string id)
    //{
    //    try
    //    {
    //        var result = GameFactory.Initialize().GetManualGame(Convert.ToInt64(id));
    //        if (result == null)
    //            return Request.CreateResponse(HttpStatusCode.NotFound);

    //        return Request.CreateResponse(HttpStatusCode.OK, result);
    //    }
    //    catch (UnauthorizedAccessException)
    //    {
    //        return Request.CreateResponse(HttpStatusCode.Unauthorized);
    //    }
    //    catch (Exception exception)
    //    {
    //        ErrorDatabaseManager.AddException(exception, GetType());
    //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
    //    }
    //}

    //public HttpResponseMessage Post([FromBody]ManualGame item)
    //{
    //    try
    //    {
    //        GameFactory.Initialize().AddRNScore(item).SaveChanges();

    //        return Request.CreateResponse(HttpStatusCode.Created, true);
    //    }
    //    catch (UnauthorizedAccessException)
    //    {
    //        return Request.CreateResponse(HttpStatusCode.Unauthorized);
    //    }
    //    catch (Exception exception)
    //    {
    //        ErrorDatabaseManager.AddException(exception, GetType());
    //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
    //    }
    //}

    //[HttpPut]
    //public HttpResponseMessage Update([FromBody]RSSFeedItem item)
    //{
    //    try
    //    {
    //        RSSFactory.Initilize().UpdateFeed(item);
    //        return Request.CreateResponse(HttpStatusCode.OK);
    //    }
    //    catch (UnauthorizedAccessException)
    //    {
    //        return Request.CreateResponse(HttpStatusCode.Unauthorized);
    //    }
    //    catch (Exception exception)
    //    {
    //        ErrorDatabaseManager.AddException(exception, GetType());
    //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
    //    }
    //}

    //public HttpResponseMessage RuleSets(string r)
    //{
    //    try
    //    {
    //        return Request.CreateResponse(HttpStatusCode.OK, EnumExt.GetValues<RuleSetsUsedEnum>());
    //    }
    //    catch (UnauthorizedAccessException)
    //    {
    //        return Request.CreateResponse(HttpStatusCode.Unauthorized);
    //    }
    //    catch (Exception exception)
    //    {
    //        ErrorDatabaseManager.AddException(exception, GetType());
    //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
    //    }
    //}

    //public HttpResponseMessage Federations(string r)
    //{
    //    try
    //    {
    //        return Request.CreateResponse(HttpStatusCode.OK, EnumExt.GetValues<FederationsEnum>());
    //    }
    //    catch (UnauthorizedAccessException)
    //    {
    //        return Request.CreateResponse(HttpStatusCode.Unauthorized);
    //    }
    //    catch (Exception exception)
    //    {
    //        ErrorDatabaseManager.AddException(exception, GetType());
    //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
    //    }
    //}

    //public HttpResponseMessage Delete(string id)
    //{
    //    try
    //    {
    //        RSSFactory.Initilize().RemoveFeed(Convert.ToInt64(id));
    //        return Request.CreateResponse(HttpStatusCode.OK);
    //    }
    //    catch (UnauthorizedAccessException)
    //    {
    //        return Request.CreateResponse(HttpStatusCode.Unauthorized);
    //    }
    //    catch (Exception exception)
    //    {
    //        ErrorDatabaseManager.AddException(exception, GetType());
    //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
    //    }
    //}

    //[HttpPut]
    //public HttpResponseMessage ProcessChecked([FromBody]List<RSSFeedItem> items)
    //{
    //    try
    //    {
    //        if (items == null || items.Count == 0)
    //            throw new HttpResponseException(HttpStatusCode.ExpectationFailed);

    //        var action = Request.GetRouteData().Values["id"].ToString();

    //        if (action.ToLower() == "delete")
    //        {
    //            var fact = RSSFactory.Initilize();

    //            foreach (var item in items)
    //            {
    //                if (item.IsChecked)
    //                {
    //                    fact.RemoveFeed(item.FeedId);
    //                }
    //            }
    //        }
    //        return Request.CreateResponse(HttpStatusCode.OK);
    //    }
    //    catch (UnauthorizedAccessException)
    //    {
    //        return Request.CreateResponse(HttpStatusCode.Unauthorized);
    //    }
    //    catch (Exception exception)
    //    {
    //        ErrorDatabaseManager.AddException(exception, GetType());
    //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
    //    }
    //}
}
