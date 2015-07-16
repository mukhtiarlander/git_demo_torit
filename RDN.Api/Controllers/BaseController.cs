using RDN.Library.Classes.Api;
using RDN.Library.Classes.Config;
using RDN.Portable.Classes.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.Api.Controllers
{
    public class BaseController : Controller
    {
        public bool IsAuthenticated
        {
            get
            {
                if (HttpContext.Request.Headers.Get(ApiManager.ApiKey) == LibraryConfig.ApiKey)
                    return true;
                else
                    return false;
            }

        }
    }
}