using Common.EmailServer.Library.Classes.Email;
using Common.Site.Controllers;
using RDN.Api.Models.Filters;
using RDN.Library.Cache;
using RDN.Library.Classes.EmailServer;
using RDN.Library.DataModels.EmailServer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace RDN.Api.Controllers
{
    public class FederationController : BaseController
    {
        [HttpGet]
        [ApiAuthorize(IsApiAuthenticated = true)]
        public ActionResult GetMembers(Guid id)
        {
            return Json(ApiFederationCache.GetMembersOfFederation(id));

        }

        [HttpGet]
        [ApiAuthorize(IsApiAuthenticated = true)]
        public ActionResult GetFederation(Guid id)
        {
            return Json(ApiFederationCache.GetFederation(id));

        }

        [HttpGet]
        [ApiAuthorize(IsApiAuthenticated = true)]
        public ActionResult ClearCache(Guid id)
        {
            ApiFederationCache.Clear(id);
            return Json(true);

        }
    }
}