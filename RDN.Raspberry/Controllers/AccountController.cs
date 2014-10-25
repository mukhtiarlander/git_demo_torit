using System;
using System.Web.Mvc;
using System.Web.Security;
using RDN.Raspberry.Models.Account;
using RDN.Raspberry.Models.Utilities;
using RDN.Library.Cache;

namespace RDN.Raspberry.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult UpdateNonVerifiedUserEmail(string veriId, string email)
        {
            var work = Library.Classes.Account.User.UpdateEmailForNonVerifiedUser(new Guid(veriId), email);

            return Json(new { isSuccess = work }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewAllNonVerifiedUsers()
        {
            var users = Library.Classes.Account.User.GetAllNonVerifiedUsers();

            return View(users);
        }

        public ActionResult SendAllEmailVerifications()
        {

            return View();
        }
        [HttpPost]
        public ActionResult SendEmails()
        {

            int success = Library.Classes.Account.User.ResendAllEmailVerificationsInQueue();

            return RedirectToAction("SendAllEmailVerifications");
        }

        [HttpPost]
        public ActionResult Roles(Role role)
        {
            bool exists = System.Web.Security.Roles.RoleExists(role.RoleForMember);

            if (!exists)
            {
                System.Web.Security.Roles.CreateRole(role.RoleForMember);
            }
            System.Web.Security.Roles.AddUserToRole(role.EmailAddress, role.RoleForMember);
            role.Added = true;
            return View(role);
        }
        [Authorize]
        public ActionResult Roles()
        {
            Role role = new Role();
            role.Added = false;

            return View(role);
        }
        [Authorize]
        public ActionResult SwitchProfileForUser()
        {
            IdModel mod = new IdModel();
            mod.IsSuccess = false;
            return View(mod);
        }
        [Authorize]
        [HttpPost]
        public ActionResult SwitchProfileForUser(IdModel mod)
        {
            mod.IsSuccess = RDN.Library.Classes.Admin.Account.Member.AttachUserToProfile(mod.Id, new Guid(mod.Id2));
            MemberCache.ClearWebSitesCache(new Guid(mod.Id2));
            return View(mod);
        }

        [Authorize]
        public ActionResult Delete()
        {
            IdModel mod = new IdModel();
            mod.IsDeleted = false;
            return View(mod);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Delete(IdModel mod)
        {

            mod.IsDeleted = RDN.Library.Classes.Admin.Account.Member.DeleteMember(new Guid(mod.Id));
            return View(mod);
        }
        [Authorize]
        public ActionResult UnRetireUser()
        {
            IdModel mod = new IdModel();
            mod.IsDeleted = false;
            return View(mod);
        }
        [Authorize]
        [HttpPost]
        public ActionResult UnRetireUser(IdModel mod)
        {

            mod.IsDeleted = RDN.Library.Classes.Admin.Account.Member.UnRetireMember(mod.Id);
            return View(mod);
        }
        [Authorize]
        public ActionResult DeleteTempAccountOne()
        {
            IdModel mod = new IdModel();
            mod.IsDeleted = false;
            return View(mod);
        }
        [Authorize]
        [HttpPost]
        public ActionResult DeleteTempAccountOne(IdModel mod)
        {

            mod.IsDeleted = RDN.Library.Classes.Admin.Account.Member.DeleteTempMemberFromTwoEvils(new Guid(mod.Id));
            return View(mod);
        }
        [Authorize]
        public ActionResult DeleteUser()
        {
            IdModel mod = new IdModel();
            mod.IsDeleted = false;
            return View(mod);
        }
        [Authorize]
        [HttpPost]
        public ActionResult DeleteUser(IdModel mod)
        {

            mod.IsDeleted = RDN.Library.Classes.Account.User.DeleteUserByName(mod.Id);
            return View(mod);
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LogOn model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    if (System.Web.Security.Roles.IsUserInRole(model.UserName, "admin") || System.Web.Security.Roles.IsUserInRole(model.UserName, "moderator"))
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, false);
                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }

                        var member = Membership.GetUser(model.UserName);
                        if (Session["AdminId"] == null)
                            Session.Add("AdminId", (Guid)member.ProviderUserKey);
                        else
                            Session["AdminId"] = (Guid)member.ProviderUserKey;

                        return RedirectToAction("Index", "Admin");
                    }
                }

                ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}
