using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Providers;
using RDN.Library.Classes.Error;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Web.Http;

public class PingServicesController : ApiController
{
    public IEnumerable<SelectOption> Get()
    {
        try
        {
            var pings = new List<SelectOption>();

            foreach (var s in BlogService.LoadPingServices())
            {
                pings.Add(new SelectOption { OptionName = s, OptionValue = s });
            }

            return pings;
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

    public void Put([FromBody]List<SelectOption> pingServices, string action = "")
    {
        var sc = new StringCollection();
        try
        {
            foreach (var item in pingServices)
            {
                sc.Add(item.OptionValue);
            }
            BlogService.SavePingServices(sc);
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, GetType());
            throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
