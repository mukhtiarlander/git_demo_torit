using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using RDN.League.Models.Utilities;
using RDN.Library.Cache;
using RDN.Library.Classes.Federation.Enums;
using RDN.Utilities.Config;
using RDN.Portable.Config;

namespace RDN.League.Models.Filters
{
    /// <summary>
    /// checks for authorization at the federation level.
    /// </summary>
    public class FederationAuthorize : AuthorizeAttribute
    {
        public bool RequireFederationManagerStatus { get; set; }
        public bool RequireFederationOwnerStatus { get; set; }


        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                return;

            if (RequireFederationManagerStatus)
            {

                var isOwner = MemberCache.IsManagerOrBetterOfFederation(RDN.Library.Classes.Account.User.GetMemberId());

                if (!isOwner)
                {
                    filterContext.Result = new RedirectResult(LibraryConfig.InternalSite);
                    return;
                }
            }
            if (RequireFederationOwnerStatus)
            {
                var isOwner = MemberCache.IsOwnerOfFederation(RDN.Library.Classes.Account.User.GetMemberId());

                if (!isOwner)
                {
                    filterContext.Result = new RedirectResult(LibraryConfig.InternalSite);
                    return;
                }
            }




        }



    }
}