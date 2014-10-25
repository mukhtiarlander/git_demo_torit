using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using RDN.Library.Classes.Error;
using System;
using System.Net;
using System.Web.Http;

public class StatsController : ApiController
{
    readonly IStatsRepository repository;

    public StatsController(IStatsRepository repository)
    {
        this.repository = repository;
    }

    public Stats Get()
    {
        try
        {
            return repository.Get();
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
