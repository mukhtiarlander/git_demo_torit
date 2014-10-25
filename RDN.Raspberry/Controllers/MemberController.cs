using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Raspberry.Models.Utilities;

namespace RDN.Raspberry.Controllers
{
    public class MemberController : Controller
    {
      
        [Authorize]
        public ActionResult ResetUserPassword()
        {
            IdModel mod = new IdModel();
            mod.IsSuccess = false;
            return View(mod);
        }
        [Authorize]
        [HttpPost]
        public ActionResult ResetUserPassword(IdModel mod)
        {
            mod.IsSuccess = RDN.Library.Classes.Account.User.ChangeUserPassword(mod.Id, mod.Id2);
            return View(mod);
        }

        [Authorize]
        public ActionResult UnlockUser()
        {
            IdModel mod = new IdModel();
            mod.IsSuccess = false;
            return View(mod);
        }
        [Authorize]
        [HttpPost]
        public ActionResult UnlockUser(IdModel mod)
        {
            mod.IsSuccess = RDN.Library.Classes.Account.User.UnlockAccount(mod.Id);
            return View(mod);
        }

    }
}
