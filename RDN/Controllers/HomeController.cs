using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using RDN.Library.Util;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Models.Account;
using RDN.Library.Classes.Account.Enums;
using System.Web.Security;
using System.IO;
using System.Xml;
using RDN.Library.Classes.Utilities;
using RDN.Utilities.Config;
using RDN.Library.Classes.Account;
using RDN.Library.Classes.Account.Classes;
using System.Web.Routing;
using System.Net;
using System.Text.RegularExpressions;
using RDN.Library.Cache;
using System.Collections.Specialized;
using RDN.Library.Classes.Store;
using RDN.Models.Home;
using RDN.Library.Classes.Store.Display;
using RDN.Portable.Config;
using RDN.Portable.Classes.Account.Enums;
using RDN.Portable.Classes.Account.Classes;
using RDN.Library.Classes.Config;

namespace RDN.Controllers
{
    public class HomeController : Controller
    {
        private static int TOTALSITEMAPRECORDSALLOWED = 30000;

        public ActionResult BlogPosts(string id, string name)
        {

            return Redirect(LibraryConfig.BlogSite + "/post/" + id + "/" + name);
        }

        public ActionResult Error()
        {
            return View();
        }
        public ActionResult Pricing()
        {
            return View();
        }
        public ActionResult Hiring()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SignUpBeta(Models.Account.SignUpBeta beta)
        {
            if (beta.Email.Length > 2)
            {
                beta.SignedUp = true;
                RDN.Library.Classes.Beta.Beta.SignUpForBeta(beta.Email);
            }
            return View(beta);
        }

        public ActionResult SignUpBeta()
        {
            Models.Account.SignUpBeta beta = new SignUpBeta();

            return View(beta);
        }

        /// <summary>
        /// the view to verify if the user that is signing up is connected to a name 
        /// already in the system.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult VerifyName(string id, string name)
        {
            RegisterModel register = PopulateRegisterModel(new Guid(id));

            return View(register);
        }

        private static RegisterModel PopulateRegisterModel(Guid id)
        {
            MemberDisplayBasic member = RDN.Library.Classes.Account.User.GetMemberDisplay(id);
            RegisterModel register = new RegisterModel();
            register.DerbyName = member.DerbyName;
            register.GenderName = member.Gender;
            register.MemberId = member.MemberId;

            if (member.Email != null)
                register.EmailVerify = member.Email.Split('@').LastOrDefault();
            return register;
        }

        [HttpPost]
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult VerifyName(RegisterModel model)
        {
            try
            {

                var result = Library.Classes.Account.User.VerifyDerbyNameAndCreateAccount(model.MemberId, model.Email, model.ConfirmEmail, model.Password, Request.UserHostAddress);

                if (result.Count > 0)
                {
                    RegisterModel register = PopulateRegisterModel(model.MemberId);
                    register.Errors = Register.RegisterErrors(result);
                    return View(register);
                }

                setCookie(model.Email, true);


                return Redirect(LibraryConfig.InternalSite);



            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());
                return View();
            }
        }

        public ActionResult Index()
        {
            HomeModel model = new HomeModel();
            return View(model);
        }

        public ActionResult Privacy()
        {

            return View();
        }


        /// <summary>
        /// submission from the user to change their lost password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult LostPassword(LostAccountModel model)
        {
            try
            {

                model.CanChangePassword = true;
                if (model.NewPasswordConfirm != model.NewPassword)
                    return View(model);

                if (ModelState.IsValid)
                {
                    var user = Membership.GetUser(model.Email);
                    if (user != null)
                    {

                        bool changed = RDN.Library.Classes.Account.User.ChangeUserPassword(user, model.NewPassword, model.VerificationCode);
                        if (changed == true)
                            model.ConfirmationMessage = "Your Password Has Been Changed.";
                        else
                            model.ConfirmationMessage = "Your Password Has NOT Been Changed. If this problem persists, please email " + LibraryConfig.DefaultInfoEmail;
                        return View(model);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return View(model);
        }

        /// <summary>
        /// page loads from an email the user can go to so they can change their password.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="email"></param>
        /// <returns></returns>
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult LostPassword(string code, string email)
        {
            LostAccountModel accountModel = new LostAccountModel();
            try
            {

                var user = Membership.GetUser(email);
                accountModel.Email = email;
                accountModel.VerificationCode = code;
                if (user != null)
                {
                    bool check = RDN.Library.Classes.Account.User.CheckLostPasswordVerificationCode(code, email);

                    if (!check)
                        accountModel.ConfirmationMessage = "Code and Email could not be found.";
                    else
                    {
                        accountModel.CanChangePassword = true;
                        return View(accountModel);
                    }
                }
                else
                {
                    accountModel.ConfirmationMessage = "It Seems Your Password Change Request Has Expired.";
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }


            return View(accountModel);
        }
        /// <summary>
        /// a view to request a password change.
        /// </summary>
        /// <returns></returns>
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult Recover()
        {
            return View();
        }
        /// <summary>
        /// the post from recover to change the users password by sending an email from the user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult Recover(LostAccountModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = Membership.GetUser(model.Email);
                    if (user != null)
                    {
                        if (RDN.Library.Classes.Account.User.UserLostPassword(user))
                            model.ConfirmationMessage = "An Email Has Been Sent to: " + model.Email;
                        else
                            model.ConfirmationMessage = "Something went wrong, if this problem persists please contact " + LibraryConfig.DefaultInfoEmail;
                        return View(model);
                    }

                    ViewBag.IsEmailExists = false;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return View();
        }
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult SignUp()
        {
            RegisterModel model = new RegisterModel();
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
            string updated = nameValueCollection["u"];
            List<DefaultPositionEnum> positions = Enum.GetValues(typeof(DefaultPositionEnum)).Cast<DefaultPositionEnum>().ToList();

            //need to remove the first position from the list.
            positions.RemoveAt(0);
            var list = (from a in positions
                        select new SelectListItem
                        {
                            Text = RDN.Utilities.Enums.EnumExt.ToFreindlyName(a),
                            Value = ((int)a).ToString()
                        });

            ViewBag.Positions = new SelectList(list, "Value", "Text");

            List<GenderEnum> genders = Enum.GetValues(typeof(GenderEnum)).Cast<GenderEnum>().ToList();
            genders.RemoveAt(0);

            if (LibraryConfig.SiteType != Library.Classes.Site.Enums.SiteType.RollerDerby)
            {
                genders.Clear();
                genders.Add(GenderEnum.Male);
                genders.Add(GenderEnum.Female);
            }

            var listGenders = (from a in genders
                               select new SelectListItem
                               {
                                   Text = RDN.Utilities.Enums.EnumExt.ToFreindlyName(a),
                                   Value = ((int)a).ToString()
                               });
            ViewBag.Genders = new SelectList(listGenders, "Value", "Text");

            if (!String.IsNullOrEmpty(updated) && updated == "f")
                model.IsConnectedToRollerDerby = false;
            else
                model.IsConnectedToRollerDerby = true;
            return View(model);
        }

        [HttpPost]
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult SignUp(RegisterModel model)
        {
            try
            {

                if (!String.IsNullOrEmpty(model.Email) && (model.Email.Contains("@163.com") || model.Email.Contains("@tom.com") || model.Email.Contains("@126.com")))
                {

                    ModelState.AddModelError("", "That Domain name has been banned from " + LibraryConfig.WebsiteShortName + ", if you think this is in Error, please contact us.");
                    return View(model);
                }
                var id = StoreGateway.GetShoppingCartId(HttpContext);

                List<DefaultPositionEnum> positions = Enum.GetValues(typeof(DefaultPositionEnum))
                                                          .Cast<DefaultPositionEnum>().ToList();
                //need to remove the first position from the list.
                positions.RemoveAt(0);
                var list = (from a in positions
                            select new SelectListItem
                            {
                                Text = RDN.Utilities.Enums.EnumExt.ToFreindlyName(a),
                                Value = ((int)a).ToString()
                            });

                ViewBag.Positions = new SelectList(list, "Value", "Text");

                List<GenderEnum> genders = Enum.GetValues(typeof(GenderEnum)).Cast<GenderEnum>().ToList();
                genders.RemoveAt(0);

                if (LibraryConfig.SiteType != Library.Classes.Site.Enums.SiteType.RollerDerby)
                {
                    genders.Clear();
                    genders.Add(GenderEnum.Male);
                    genders.Add(GenderEnum.Female);
                }

                var listGenders = (from a in genders
                                   select new SelectListItem
                                   {
                                       Text = RDN.Utilities.Enums.EnumExt.ToFreindlyName(a),
                                       Value = ((int)a).ToString()
                                   });
                ViewBag.Genders = new SelectList(listGenders, "Value", "Text");

                List<NewUserEnum> result = new List<NewUserEnum>();
                //p=f&returnSite=store&ReturnUrl
                if (model.IsConnectedToRollerDerby)
                    result = Library.Classes.Account.User.CreateMember(model.Email, model.Email, model.Password, model.Firstname, model.DerbyName, model.Gender, model.PositionType, Request.UserHostAddress);
                else
                {
                    result = Library.Classes.Account.User.CreateUserNotConnectedToDerby(model.Email, model.Email, model.Password);
                }
                if (result.Count > 0)
                {
                    model.Errors = Register.RegisterErrors(result);
                    return View(model);
                }
                setCookie(model.Email, true);

                if (id != null)
                    StoreGateway.SetShoppingCartSession(id.Value, HttpContext);

                if (Request.UrlReferrer != null)
                {
                    NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.UrlReferrer.Query);
                    string site = nameValueCollection["returnSite"];
                    string url = nameValueCollection["ReturnUrl"];

                    if (site == "store")
                    {
                        if (!String.IsNullOrEmpty(site) && !String.IsNullOrEmpty(url))
                            return Redirect(LibraryConfig.ShopSite + url);
                        else if (!String.IsNullOrEmpty(site))
                            return Redirect(LibraryConfig.ShopSite);
                    }
                    else if (site == "rollinNews")
                    {
                        if (!String.IsNullOrEmpty(site) && !String.IsNullOrEmpty(url))
                            return Redirect(RollinNewsConfig.WEBSITE_DEFAULT_LOCATION + url);
                        else if (!String.IsNullOrEmpty(site))
                            return Redirect(RollinNewsConfig.WEBSITE_DEFAULT_LOCATION);

                    }

                }
                return Redirect(LibraryConfig.InternalSite);
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());
                return View();
            }
        }
        /// <summary>
        /// sets the cookie for the logged in session
        /// </summary>
        /// <param name="email"></param>
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


#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult Login()
        {
            return View(new LogOnModel { RememberMe = true });
        }

        /// <summary>
        /// we have a returnst
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnSite">Used for returning to a subdomain like </param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
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


        public ActionResult Logout()
        {
            try
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
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());
            }
            return Redirect(LibraryConfig.PublicSite);
        }

        public ActionResult SiteMap()
        {
            using (TextWriter textWriter = new StreamWriter(HttpContext.Response.OutputStream, System.Text.Encoding.UTF8))
            {
                try
                {
                    XmlTextWriter writer = new XmlTextWriter(textWriter);
                    writer.Formatting = Formatting.Indented;
                    writer.WriteStartDocument();
                    writer.WriteStartElement("sitemapindex");
                    writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
                    int sitemap = SiteCache.GetSiteMap().Count;
                    //total records allowed is 50k, but not to exceed the limit in MBs, we are shrinking this down to 30k
                    sitemap /= TOTALSITEMAPRECORDSALLOWED;

                    for (int i = 0; i < sitemap + 1; i++)
                    {
                        writer.WriteStartElement("sitemap");
                        writer.WriteElementString("loc", LibraryConfig.PublicSite + "/sitemaps?p=" + i);
                        writer.WriteElementString("lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd"));
                        writer.WriteEndElement(); // url
                    }
                    writer.WriteEndElement(); // urlset
                }
                catch (Exception e)
                {
                    ErrorDatabaseManager.AddException(e, GetType());
                }
                return Content("");
            }
        }
        public ActionResult SiteMaps(int p)
        {
            using (TextWriter textWriter = new StreamWriter(HttpContext.Response.OutputStream, System.Text.Encoding.UTF8))
            {
                try
                {
                    XmlTextWriter writer = new XmlTextWriter(textWriter);
                    writer.Formatting = Formatting.Indented;
                    writer.WriteStartDocument();
                    writer.WriteStartElement("urlset");
                    writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                    writer.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");
                    writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
                    var sitemap = SiteCache.GetSiteMap();
                    sitemap = sitemap.Skip(p * TOTALSITEMAPRECORDSALLOWED).Take(TOTALSITEMAPRECORDSALLOWED).ToList();
                    foreach (var item in sitemap)
                    {
                        writer.WriteStartElement("url");
                        writer.WriteElementString("loc", item.Url);
                        SitemapHelperEnum sit;
                        if (Enum.TryParse<SitemapHelperEnum>(item.ChangeFrequency.ToString(), out sit))
                            writer.WriteElementString("changefreq", sit.ToString());
                        writer.WriteElementString("lastmod", item.LastModified.Value.ToString("yyyy-MM-dd"));
                        writer.WriteEndElement(); // url
                    }
                    writer.WriteEndElement(); // urlset
                }
                catch (Exception e)
                {
                    ErrorDatabaseManager.AddException(e, GetType());
                }
                return Content("");
            }
        }




        protected override void HandleUnknownAction(string actionName)
        {
            try
            {
                //if its a post from the zebra forum.
                if (actionName.Contains("yaf_"))
                    Response.Redirect("http://zebras." + LibraryConfig.MainDomain + "/" + actionName);
                else if (HttpContext.Request.Url.AbsoluteUri.Contains("wiki." + LibraryConfig.MainDomain))
                    Response.Redirect(LibraryConfig.WikiSite);
                else
                    Response.Redirect(LibraryConfig.PublicSite);
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());
            }
        }

    }
}
