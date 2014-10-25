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

namespace RDN.Store.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class AccountController : Controller
    {

        //public ActionResult LogOn()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult LogOn(LogOnModel model, string returnUrl)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (Membership.ValidateUser(model.Email, model.Password))
        //        {
        //            FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);

        //            //modify the Domain attribute of the cookie to the second level domain
        //            System.Web.HttpCookie MyCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(model.Email, false);
        //            MyCookie.Domain = "rdnation.com";//the second level domain name
        //            Response.AppendCookie(MyCookie);

        //            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
        //            {
        //                return Redirect(returnUrl);
        //            }

        //            var member = Membership.GetUser(model.Email);
        //            if (Session["UserId"] == null)
        //                Session.Add("UserId", (Guid)member.ProviderUserKey);
        //            else
        //                Session["UserId"] = (Guid)member.ProviderUserKey;

        //            return Redirect(ServerConfig.WEBSITE_DEFAULT_LOGIN_LOCATION);
        //        }

        //        ModelState.AddModelError("", "The user name or password provided is incorrect.");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}


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

            return Redirect(ServerConfig.WEBSITE_STORE_DEFAULT_LOCATION);
        }





    }
}
