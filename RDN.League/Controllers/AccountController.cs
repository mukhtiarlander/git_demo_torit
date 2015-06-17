using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RDN.League.Models.Filters;
using RDN.League.Models.OutModel;
using RDN.Library.Classes.Account.Enums;
using RDN.League.Models.Account;
using RDN.Utilities.Config;
using RDN.Library.Classes.Error;
using RDN.Portable.Config;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View(new LogOnModel { RememberMe = true });
        }

        public ActionResult LogOn()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect(LibraryConfig.WEBSITE_DEFAULT_LOGIN_LOCATION);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            try
            {
                FormsAuthentication.SignOut();
                RDN.Library.Classes.Account.User.ClearMemberId();
                Session.Abandon();


                foreach (var cookie in Request.Cookies.AllKeys)
                {
                    Request.Cookies.Remove(cookie);
                }
                foreach (var cookie in Response.Cookies.AllKeys)
                {
                    Response.Cookies.Remove(cookie);
                }

                // clear authentication cookie
                HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                cookie1.Expires = DateTime.Now.AddYears(-1);
                cookie1.Path = FormsAuthentication.FormsCookiePath;
                cookie1.Domain = FormsAuthentication.CookieDomain;
                Response.Cookies.Add(cookie1);

                // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
                HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
                cookie2.Expires = DateTime.Now.AddYears(-1);
                cookie2.Path = FormsAuthentication.FormsCookiePath;
                cookie2.Domain = FormsAuthentication.CookieDomain;
                Response.Cookies.Add(cookie2);

                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(RDN.Library.Classes.Config.LibraryConfig.PublicSite + "/account/logout");
        }





    }
}
