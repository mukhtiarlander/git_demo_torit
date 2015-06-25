using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
//using RDN.Models.Account;
using RDN.Library.Classes.Account.Enums;
using RDN.Library.Classes.Error;
using RDN.Utilities.Config;
using RDN.Portable.Config;
using RDN.Library.Classes.Config;

namespace RDN.Store.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class AccountController : Controller
    {

     
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);

            return Redirect(LibraryConfig.ShopSite);
        }





    }
}
