using BlogEngine.Core;
using BlogEngine.Core.Data;
using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Providers;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.RN.Funds;
using RDN.Portable.Config;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using System.Linq;
using BlogEngine.Core.Providers.CacheProvider;

public class FilesController : ApiController
{

    public FilesController()
    {
    }



    public List<FileItem> Get(string search)
    {
        try
        {
           
            List<FileItem> files = NewsCache.GetPictureArchive();

            if (!String.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                files = files.Where(x => x.Name.ToLower().Contains(search)).Take(30).ToList();
            }
            else
                files = files.Take(30).ToList();

            return files;
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
