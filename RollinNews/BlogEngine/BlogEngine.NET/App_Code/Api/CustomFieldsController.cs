using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using RDN.Library.Classes.Error;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class CustomFieldsController : ApiController
{
    readonly ICustomFieldRepository repository;

    public CustomFieldsController(ICustomFieldRepository repository)
    {
        this.repository = repository;
    }

    public IEnumerable<CustomField> Get(string filter = "")
    {
        // filter example: 'CustomType == "THEME" && ObjectId == "standard"'
        try
        {
            return repository.Find(filter);
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

    public HttpResponseMessage Post([FromBody]CustomField item)
    {
        try
        {
            var result = repository.Add(item);
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

    public HttpResponseMessage Put([FromBody]List<CustomField> items)
    {
        try
        {
            if (items != null && items.Count > 0)
            {
                foreach (var item in items)
                {
                    item.BlogId = BlogEngine.Core.Blog.CurrentInstance.Id;
                    repository.Update(item);
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