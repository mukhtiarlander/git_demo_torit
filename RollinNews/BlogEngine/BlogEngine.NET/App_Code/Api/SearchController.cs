using BlogEngine.Core.Data;
using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using RDN.Library.Classes.Error;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class SearchController : ApiController
{

    public SearchController()
    {
    }

    public IEnumerable<SearchResultItem> Get(string search,string type)
    {
        try
        {
            if (!String.IsNullOrEmpty(search))
                return SearchRepository.Search(search.ToLower(), type);
            
        }
        catch (UnauthorizedAccessException)
        {
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, GetType());
            
        }
        return new List<SearchResultItem>();
    }


}
