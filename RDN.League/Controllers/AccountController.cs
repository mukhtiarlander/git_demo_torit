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
using RDN.Library.Classes.Config;
using RDN.Portable.Classes.Url;
using RDN.Library.Classes.Store;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class AccountController : Controller
    {
        #region Private Methods
        private void setCookie(string email, bool rememberMe)
        {
            try
            {
                FormsAuthentication.SetAuthCookie(email, rememberMe);
                //modify the Domain attribute of the cookie to the second level domain
                System.Web.HttpCookie MyCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(email, rememberMe);
                MyCookie.Domain = LibraryConfig.MainDomain;//the second level domain name
                Response.AppendCookie(MyCookie);
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());
            }
        }
        #endregion

        #region Public Methods
        public ActionResult LogOn()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect(LibraryConfig.PublicSite + UrlManager.WEBSITE_DEFAULT_LOGIN_LOCATION);
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

        //RDN-12345 -- Login Functionality Added Like RDN
        public ActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect(LibraryConfig.InternalSite);
            }
            return View(new LogOnModel { RememberMe = true });
        }

        //RDN-12345 -- Login Functionality Added Like RDN
        [HttpPost]
        public ActionResult Login(LogOnModel model, string returnSite, string returnUrl)
        {
            try
            {
                var id = StoreGateway.GetShoppingCartId(HttpContext);
                if (ModelState.IsValid)
                {
                    if (model.Email.Contains("@163.com") || model.Email.Contains("@tom.com") || model.Email.Contains("@126.com"))
                    {
                        ModelState.AddModelError("", "That Domain name has been banned from " + LibraryConfig.WebsiteShortName + ", if you think this is in Error, please contact us.");
                        return View(model);
                    }
                    if (Membership.ValidateUser(model.Email, model.Password))
                    {
                        setCookie(model.Email, model.RememberMe);

                        if (id != null)
                            StoreGateway.SetShoppingCartSession(id.Value, HttpContext);

                        if (!String.IsNullOrEmpty(returnSite))
                        {
                            string url;
                            if (returnSite == "league")
                            {
                                url = LibraryConfig.InternalSite;
                                if (!String.IsNullOrEmpty(returnUrl))
                                    url += returnUrl;
                                return Redirect(url);
                            }
                            if (returnSite == "shops")
                            {
                                url = LibraryConfig.ShopSite;
                                if (!String.IsNullOrEmpty(returnUrl))
                                    url += returnUrl;
                                return Redirect(url);
                            }
                            if (returnSite == "zebras")
                            {
                                url = "http://zebras." + LibraryConfig.MainDomain;
                                if (!String.IsNullOrEmpty(returnUrl))
                                    url += returnUrl;
                                return Redirect(url);
                            }
                        }
                        else if (!String.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return Redirect(LibraryConfig.InternalSite);
                    }

                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion
    }
}
