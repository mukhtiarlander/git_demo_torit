using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using RDN.Library.Classes.Error;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class TagsController : ApiController
{
    readonly ITagRepository repository;

    public TagsController(ITagRepository repository)
    {
        this.repository = repository;
    }

    public IEnumerable<TagItem> Get(int take = 10, int skip = 0, string postId = "", string order = "")
    {
        try
        {
            return repository.Find(take, skip, postId, order);
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

    public HttpResponseMessage Put([FromBody]TagToUpdate tag)
    {
        try
        {
            repository.Save(tag.OldTag, tag.NewTag);
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
            repository.Delete(id);
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
    public HttpResponseMessage ProcessChecked([FromBody]List<TagItem> items)
    {
        try
        {
            if (items == null || items.Count == 0)
                throw new HttpResponseException(HttpStatusCode.ExpectationFailed);

            var action = Request.GetRouteData().Values["id"].ToString();

            if (action.ToLower() == "delete")
            {
                foreach (var item in items)
                {
                    if (item.IsChecked)
                    {
                        repository.Delete(item.TagName);
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

public class TagToUpdate
{
    public string OldTag { get; set; }
    public string NewTag { get; set; }
}
