using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using RDN.Library.Classes.Error;
using System;
using System.Net;
using System.Web.Http;

public class LookupsController : ApiController
{
    readonly ILookupsRepository  repository;

    public LookupsController(ILookupsRepository repository)
    {
        this.repository = repository;
    }

    public Lookups Get()
    {
        try
        {
            return repository.GetLookups();
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
}
