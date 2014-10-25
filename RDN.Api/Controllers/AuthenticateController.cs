using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RDN.Library.Cache;
using RDN.Library.Classes.Account;
using RDN.Library.Classes.Account.Enums;
using RDN.Library.Classes.Error;
using RDN.Mobile.Account;
using RDN.Portable.Account;
using RDN.Portable.Network;

namespace RDN.Api.Controllers
{
    public class AuthenticateController : Controller
    {
        [ValidateInput(false)]
        public ActionResult Login()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<UserMobile>(ref stream);
                    if (Membership.ValidateUser(ob.UserName, ob.Password))
                    {
                        var user = Membership.GetUser(ob.UserName);
                        ob.LoginId = (Guid)user.ProviderUserKey;
                        ob.IsLoggedIn = true;
                        var mem = RDN.Library.Classes.Account.User.GetMemberWithUserId(ob.LoginId);
                        if (mem != null)
                        {
                            ob.MemberId = mem.MemberId;
                            ob.DerbyName = mem.DerbyName;
                            ob.FirstName = mem.Firstname;
                            ob.Gender = mem.Gender;
                            ob.IsConnectedToDerby = !mem.IsNotConnectedToDerby;
                            ob.Position = mem.PositionType;
                            ob.CurrentLeagueId = mem.CurrentLeagueId;
                        }
                        ob.IsValidSub = MemberCache.CheckIsLeagueSubscriptionPaid(ob.MemberId);

                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());

            }
            return Json(new UserMobile() { IsLoggedIn = false }, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public ActionResult SignUp()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    List<NewUserEnum> result = new List<NewUserEnum>();
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<UserMobile>(ref stream);

                    if (ob.IsConnectedToDerby)
                        result = Library.Classes.Account.User.CreateMember(ob.UserName, ob.UserName, ob.Password, ob.FirstName, ob.DerbyName, ob.Gender, ob.Position, Request.UserHostAddress);
                    else
                    {
                        result = Library.Classes.Account.User.CreateUserNotConnectedToDerby(ob.UserName, ob.UserName, ob.Password);
                    }
                    if (result.Count > 0)
                    {
                        ob.Error = Register.RegisterErrors(result).FirstOrDefault();
                        return Json(new UserMobile() { IsLoggedIn = false, DidSignUp = false, Error = ob.Error }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ob.DidSignUp = true;
                        var user = Membership.GetUser(ob.UserName);
                        ob.LoginId = (Guid)user.ProviderUserKey;
                        ob.IsLoggedIn = true;
                        var mem = RDN.Library.Classes.Account.User.GetMemberWithUserId(ob.LoginId);
                        if (mem != null)
                        {
                            ob.MemberId = mem.MemberId;
                        }
                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());

            }
            return Json(new UserMobile() { IsLoggedIn = false, DidSignUp = false, Error = "Something Happened, Looking Into Error" }, JsonRequestBehavior.AllowGet);
        }
    }
}
