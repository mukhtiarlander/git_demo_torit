using RDN.Library.Classes.Config;
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
                if (HttpContext.Request.Headers.Get("api_key") == LibraryConfig.ApiAuthenticationKey)
                    return true;
                else
                    return false;
            }

        }
    }
}