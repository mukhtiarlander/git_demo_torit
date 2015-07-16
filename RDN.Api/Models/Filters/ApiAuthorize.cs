using RDN.Library.Classes.Api;
using RDN.Library.Classes.Config;
using RDN.Portable.Classes.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace RDN.Api.Models.Filters
{
    public class ApiAuthorize : AuthorizeAttribute
    {

        public bool IsApiAuthenticated { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (IsApiAuthenticated)
            {
                if (httpContext.Request.Headers[ApiManager.ApiKey] == null)
                {
                    return false;
                }
                else
                {
                    var key = HttpContext.Current.Request.Headers[ApiManager.ApiKey];
                    if (key != LibraryConfig.ApiKey)
                    {
                        return false;
                    }
                }
            }


            return true;
        }
       


    }
}