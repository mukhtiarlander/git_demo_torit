using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using RDN.League.Models.Account;
using RDN.League.Models.Filters;
using RDN.League.Models.League;
using RDN.League.Models.OutModel;
using RDN.League.Models.Team;
using RDN.League.Models.User;
using RDN.League.Models.Utilities;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Account.Enums;
using RDN.Library.Classes.Federation;
using RDN.Library.Classes.League.Enums;
using RDN.Library.Classes.Team.Classes;
using RDN.Library.Classes.Team.Enums;
using RDN.Library.Classes.Utilities;
using Team = RDN.Library.Classes.Team.TeamFactory;
using ViewMember = RDN.League.Models.User.ViewMember;
using RDN.Library.Cache;
using RDN.Library.Classes.Location;
using RDN.Library.Classes.Error;
using RDN.Library.Util;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class ManagerController : BaseController
    {
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult Home()
        {
            return View();

        }

        #region No authorization

        [HttpGet]
        public ActionResult ValidateEmailOld(string email, string code)
        {
            try
            {
                List<SiteMessage> result =
                    VerifyEmailErrors(Library.Classes.Account.User.VerifyEmailVerification(code,
                                                                                           Request.UserHostAddress));

                if (result.Count > 0)
                {
                    AddMessages(result);
                    return RedirectToAction("LogOn", "Account");
                }

                if (Session["IsVerified"] == null)
                    Session.Add("IsVerified", true);
                else
                    Session["IsVerified"] = true;
                AddMessage(new SiteMessage { Message = "Your email address has been verified.", MessageType = SiteMessageType.Success });

                //MemberCache.Clear(Library.Classes.Account.User.GetMemberId(), HttpContext.Cache);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return RedirectToAction("LogOn", "Account");
        }
        [HttpGet]
        public ActionResult ValidateEmail(string code)
        {
            try
            {
                List<SiteMessage> result =
                    VerifyEmailErrors(Library.Classes.Account.User.VerifyEmailVerification(code,
                                                                                           Request.UserHostAddress));

                if (result.Count > 0)
                {
                    AddMessages(result);
                    return RedirectToAction("LogOn", "Account");
                }

                if (Session["IsVerified"] == null)
                    Session.Add("IsVerified", true);
                else
                    Session["IsVerified"] = true;
                AddMessage(new SiteMessage { Message = "Your email address has been verified.", MessageType = SiteMessageType.Success });

                //MemberCache.Clear(Library.Classes.Account.User.GetMemberId(), HttpContext.Cache);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return RedirectToAction("LogOn", "Account");
        }

        #endregion

        #region No league membership required

        [LeagueAuthorize(EmailVerification = false, IsInLeague = false)]
        public ActionResult VerifyEmail()
        {

            var output = new GenericSingleModel<VerifyEmail>(new VerifyEmail());
            return View(output);
        }

        [LeagueAuthorize(EmailVerification = false, IsInLeague = false)]
        [HttpPost]
        public ActionResult VerifyEmail(VerifyEmail model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    List<SiteMessage> result = VerifyEmailErrors(Library.Classes.Account.User.VerifyEmailVerification(
                        string.IsNullOrEmpty(model.EmailVerificationCode)
                            ? string.Empty
                            : model.EmailVerificationCode.Trim(), Request.UserHostAddress
                                                                     ));

                    if (result.Count > 0)
                    {
                        AddMessages(result);
                        var output = new GenericSingleModel<VerifyEmail>(model);
                        return View(output);
                    }

                    if (Session["IsVerified"] == null)
                        Session.Add("IsVerified", true);
                    else
                        Session["IsVerified"] = true;
                    AddMessage(new SiteMessage
                                   {
                                       Message = "Your email address has been verified.",
                                       MessageType = SiteMessageType.Success
                                   });

                    return RedirectToAction("Index", "Home");
                }
                AddMessage(new SiteMessage { Message = "An unknown error occured.", MessageType = SiteMessageType.Error });
                return RedirectToAction("VerifyEmail");

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
                return RedirectToAction("VerifyEmail");
            }
        }

        [LeagueAuthorize(EmailVerification = false, IsInLeague = false)]
        public ActionResult ResendVerificationCode()
        {
            try
            {
                bool success = Library.Classes.Account.User.ResendEmailVerification();
                if (success)
                    AddMessage(new SiteMessage { Message = "A verification email has been sent.", MessageType = SiteMessageType.Success });
                else
                    AddMessage(new SiteMessage { Message = "A problem occured.", MessageType = SiteMessageType.Error });

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return RedirectToAction("VerifyEmail");

        }

        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult JoinLeague()
        {

            var output = new GenericSingleModel<JoinLeague>(new JoinLeague());
            return View(output);
        }



        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult CreateLeague()
        {
            try
            {
                Dictionary<Guid, string> federations = Federation.GetFederations();
                Dictionary<int, string> countries = LocationFactory.GetCountriesDictionary();

                var output = new GenericSingleModel<CreateLeague>(new CreateLeague
                                                                      {
                                                                          Federations =
                                                                              federations.Select(
                                                                                  item =>
                                                                                  new SelectListItem
                                                                                      {
                                                                                          Text = item.Value,
                                                                                          Value = item.Key.ToString()
                                                                                      }).ToList(),
                                                                          Countries =
                                                                              countries.Select(
                                                                                  item =>
                                                                                  new SelectListItem
                                                                                      {
                                                                                          Text = item.Value,
                                                                                          Value = item.Key.ToString()
                                                                                      }).ToList()
                                                                      });
                return View(output);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View();
        }

        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        [HttpPost]
        public ActionResult CreateLeague(CreateLeague model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    List<SiteMessage> result = CreateLeagueErrors(Library.Classes.League.LeagueFactory.CreateLeague(
                        model.Federation,
                        string.IsNullOrEmpty(model.LeagueName) ? string.Empty : model.LeagueName.Trim(),
                        string.IsNullOrEmpty(model.ContactPhone) ? string.Empty : model.ContactPhone.Trim(),
                        string.IsNullOrEmpty(model.ContactEmail) ? string.Empty : model.ContactEmail.Trim(),
                        string.IsNullOrEmpty(model.AdditionalInformation)
                            ? string.Empty
                            : model.AdditionalInformation.Trim(),
                        model.Country,
                        string.IsNullOrEmpty(model.State) ? string.Empty : model.State.Trim(),
                        string.IsNullOrEmpty(model.City) ? string.Empty : model.City.Trim(),
                                                                    0));

                    if (result.Count > 0)
                    {
                        AddMessages(result);
                        Dictionary<Guid, string> federations = Federation.GetFederations();
                        model.Federations =
                            federations.Select(item => new SelectListItem { Text = item.Value, Value = item.Key.ToString() }).
                                ToList();

                        Dictionary<int, string> countries = LocationFactory.GetCountriesDictionary();
                        model.Countries =
                            countries.Select(item => new SelectListItem { Text = item.Value, Value = item.Key.ToString() }).
                                ToList();

                        foreach (SelectListItem country in model.Countries)
                            country.Selected = country.Value.Equals(model.Country);

                        foreach (SelectListItem federation in model.Federations)
                            federation.Selected = federation.Value.Equals(model.Federation);

                        var output = new GenericSingleModel<CreateLeague>(model);
                        return View(output);
                    }

                    AddMessage(new SiteMessage
                                   {
                                       Message = "Your league creation request has been sent and will be reviewed shortly.",
                                       MessageType = SiteMessageType.Success
                                   });
                    return RedirectToAction("JoinLeague");
                }

                AddMessage(new SiteMessage { Message = "An unknown error occured.", MessageType = SiteMessageType.Error });

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return RedirectToAction("CreateLeague");

        }

        [LeagueAuthorize]
        public ActionResult ChangePassword()
        {
            var output = new GenericSingleModel<ChangePasswordModel>(new ChangePasswordModel());
            return View(output);
        }

        [LeagueAuthorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!model.Password.Equals(model.PasswordRepeat))
                    {
                        AddMessage(new SiteMessage
                                       {
                                           MessageType = SiteMessageType.Error,
                                           Message = "The password and the password repeat doesn't match."
                                       });
                        return RedirectToActionPermanent("ChangePassword");
                    }

                    bool result = Membership.GetUser().ChangePassword(model.OldPassword, model.Password);
                    if (!result)
                    {
                        AddMessage(new SiteMessage
                                       {
                                           MessageType = SiteMessageType.Error,
                                           Message =
                                               "Incorrect old password or the new password does not meet the password criterias."
                                       });
                        Library.Classes.Account.User.AddMemberLog("Change password failed",
                                                                  "A failed attempt to change the password was recorded.",
                                                                  MemberLogEnum.SystemFailedAttempt, Request.UserHostAddress);
                        return RedirectToActionPermanent("ChangePassword");
                    }

                    AddMessage(new SiteMessage
                                   {
                                       MessageType = SiteMessageType.Success,
                                       Message = "Your password was successfully changed"
                                   });
                    Library.Classes.Account.User.AddMemberLog("Password changed successfully",
                                                              "The password was successfully changed.",
                                                              MemberLogEnum.SystemDataChanged, Request.UserHostAddress);
                    return RedirectToActionPermanent("Roster");
                }

                AddMessage(new SiteMessage { MessageType = SiteMessageType.Error, Message = "Something was incorrect, try again" });
                Library.Classes.Account.User.AddMemberLog("Change password failed",
                                                          "A failed attempt to change the password was recorded.",
                                                          MemberLogEnum.SystemFailedAttempt, Request.UserHostAddress);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return RedirectToActionPermanent("ChangePassword");
        }

        #endregion

        #region Membership required

        #region League member status required

        //[LeagueAuthorize(RequireEmailVerification = true, RequireLeagueMembership = true)]
        //public ActionResult Roster()
        //{
        //    try
        //    {
        //        var output = new GenericEnumerableModel<Library.Classes.Account.Classes.ViewMember>();
        //        output.Model = Library.Classes.League.League.GetLeagueMembers();
        //        return View(output);
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }
        //    return View();
        //}

        //[LeagueAuthorize(RequireEmailVerification = true, RequireLeagueMembership = true)]
        //public ActionResult ViewMember(string id)
        //{
        //    try
        //    {
        //        Library.Classes.Account.Classes.ViewMember rawModel = Library.Classes.Account.User.GetViewMember(id);
        //        if (rawModel == null)
        //            return RedirectToActionPermanent("Roster");

        //        var output = new GenericSingleModel<ViewMember>();
        //        var model = new ViewMember
        //                        {
        //                            Email = rawModel.Email,
        //                            Firstname = rawModel.Firstname,
        //                            Information = rawModel.Information,
        //                            MemberId = rawModel.MemberId,
        //                            DerbyName = rawModel.DerbyName,
        //                            PlayerNumber = rawModel.PlayerNumber,
        //                            Team = rawModel.Team
        //                        };

        //        SetViewMemberTeamDropdownData(ref model);
        //        output.Model = model;
        //        return View(output);
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }
        //    return View();
        //}

        //[LeagueAuthorize(RequireEmailVerification = true, RequireLeagueMembership = true)]
        //public ActionResult ViewTeams()
        //{
        //    try
        //    {
        //        var output = new GenericEnumerableModel<ViewTeam>();
        //        output.Model = Team.GetTeamsForLeague();
        //        return View(output);
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }
        //    return View();
        //}

        //[LeagueAuthorize(RequireEmailVerification = true, RequireLeagueMembership = true, RequireManagerStatus = true)]
        //public ActionResult ViewTeam(string id)
        //{
        //    try
        //    {
        //        ViewTeam model = Team.GetViewTeam(id);
        //        if (model == null)
        //            return RedirectToActionPermanent("ViewTeams");

        //        var output = new GenericSingleModel<ViewTeam>(model);
        //        return View(output);
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }
        //    return View();
        //}

        #endregion

        #region League manager status required

        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true)]
        public ActionResult ApprovePendingMember(string id, string leagueId)
        {
            try
            {
                bool result = Library.Classes.League.LeagueFactory.ApprovePendingMember(id, new Guid(leagueId), Request.UserHostAddress);
                if (result)
                    AddMessage(new SiteMessage
                                   {
                                       Message = "The approval was successful and the member has been added to the Roster.",
                                       MessageType = SiteMessageType.Success
                                   });
                else
                    AddMessage(new SiteMessage
                                   {
                                       Message = "An error occured and the member could not be added to the Roster.",
                                       MessageType = SiteMessageType.Error
                                   });
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return RedirectToActionPermanent("ShowPendingMembers");
        }

        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true)]
        public ActionResult RemovePendingMember(string id, string leagueId)
        {
            try
            {
                bool result = Library.Classes.League.LeagueFactory.RemovePendingMember(id, new Guid(leagueId), Request.UserHostAddress);
                if (result)
                    AddMessage(new SiteMessage
                                   {
                                       Message = "The member has been removed from the pendings list.",
                                       MessageType = SiteMessageType.Success
                                   });
                else
                    AddMessage(new SiteMessage
                                   {
                                       Message = "An error occured and the member could not be removed from the pendings list.",
                                       MessageType = SiteMessageType.Error
                                   });
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return RedirectToActionPermanent("ShowPendingMembers");
        }

        //[LeagueAuthorize(RequireEmailVerification = true, RequireLeagueMembership = true, RequireManagerStatus = true)]
        //public ActionResult RemoveMemberFromLeague(string id)
        //{
        //    try
        //    {
        //        bool result = Library.Classes.League.League.RemoveMember(id, Request.UserHostAddress);
        //        if (result)
        //            AddMessage(new SiteMessage
        //                           {
        //                               Message = "The member has been successfully removed from the league.",
        //                               MessageType = SiteMessageType.Success
        //                           });
        //        else
        //            AddMessage(new SiteMessage
        //                           {
        //                               Message =
        //                                   "The member could not be removed. Check that the member exists, is in the league and that the supplied member id is correct",
        //                               MessageType = SiteMessageType.Error
        //                           });
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }

        //    return RedirectToActionPermanent("Roster");
        //}

        //[LeagueAuthorize(RequireEmailVerification = true, RequireLeagueMembership = true, RequireManagerStatus = true)]
        //public ActionResult ShowPendingMembers()
        //{
        //    try
        //    {
        //        var output = new GenericEnumerableModel<MemberDisplay>();
        //        output.Model = Library.Classes.League.League.GetLeaguePendingMembers();
        //        return View(output);
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }
        //    return View();
        //}

        //[LeagueAuthorize(RequireEmailVerification = true, RequireLeagueMembership = true, RequireManagerStatus = true)]
        //public ActionResult CreateAccount()
        //{
        //    var output = new GenericSingleModel<CreateAccount>();
        //    try
        //    {
        //        output.Model = new CreateAccount
        //                           {
        //                               League = Library.Classes.League.League.GetLeague().Name
        //                           };
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }
        //    return View(output);
        //}

        //    [LeagueAuthorize(RequireEmailVerification = true, RequireLeagueMembership = true, RequireManagerStatus = true)]
        //    [HttpPost]
        //    public ActionResult CreateAccount(CreateAccount model)
        //    {
        //        try
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                string psw = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);
        //                List<SiteMessage> result = CreateAccountErrors(Library.Classes.Account.User.CreateMember(
        //                    model.Email,
        //                    model.ConfirmEmail,
        //                    psw, model.Firstname,
        //model.Surname, 0, 0,
        //                    Request.UserHostAddress,
        //                    Library.Classes.League.League.GetLeague().LeagueId
        //                                                                   ));

        //                if (result.Count > 0)
        //                {
        //                    AddMessages(result);

        //                    model.League = Library.Classes.League.League.GetLeague().Name;

        //                    var output = new GenericSingleModel<CreateAccount>(model);
        //                    return View(output);
        //                }

        //                AddMessage(new SiteMessage
        //                               {
        //                                   Message =
        //                                       "The account has been created and an email has been sent to the user to let him/her verify the email address.",
        //                                   MessageType = SiteMessageType.Success
        //                               });
        //                return RedirectToAction("Roster");
        //            }

        //            AddMessage(new SiteMessage { Message = "An unknown error occured.", MessageType = SiteMessageType.Error });
        //        }
        //        catch (Exception exception)
        //        {
        //            ErrorDatabaseManager.AddException(exception, exception.GetType());
        //        }

        //        return RedirectToAction("Roster");

        //    }

        //[LeagueAuthorize(RequireEmailVerification = true, RequireLeagueMembership = true, RequireManagerStatus = true)]
        //[HttpPost]
        //public ActionResult ViewMember(ViewMember model)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            bool result = Library.Classes.League.League.UpdateMember(model.MemberId, model.DerbyName,
        //                                                                     model.PlayerNumber, model.Information,
        //                                                                     model.SelectedTeamId);

        //            if (result)
        //            {
        //                AddMessage(new SiteMessage
        //                               {
        //                                   MessageType = SiteMessageType.Success,
        //                                   Message = "The profile has been updated"
        //                               });
        //                return ViewMember(model.MemberId.ToString());
        //            }

        //            AddMessage(new SiteMessage
        //                           {
        //                               MessageType = SiteMessageType.Error,
        //                               Message = "Invalid input data"
        //                           });

        //            var output = new GenericSingleModel<ViewMember>();
        //            Library.Classes.Account.Classes.ViewMember rawModel =
        //                Library.Classes.Account.User.GetViewMember(model.MemberId.ToString());
        //            model.Email = rawModel.Email;
        //            model.Firstname = rawModel.Firstname;
        //            model.MemberId = rawModel.MemberId;
        //            model.DerbyName = rawModel.DerbyName;
        //            model.Team = rawModel.Team;

        //            SetViewMemberTeamDropdownData(ref model);
        //            output.Model = model;
        //            return View(output);
        //        }

        //        AddMessage(new SiteMessage
        //                       {
        //                           MessageType = SiteMessageType.Error,
        //                           Message = "Invalid user profile"
        //                       });
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }

        //    return RedirectToActionPermanent("Roster");
        //}

        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true)]
        public ActionResult CreateTeam()
        {
            try
            {
                var output = new GenericSingleModel<CreateTeam>(new CreateTeam());
                return View(output);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View();
        }

        //[LeagueAuthorize(RequireEmailVerification = true, RequireLeagueMembership = true, RequireManagerStatus = true)]
        //[HttpPost]
        //public ActionResult CreateTeam(CreateTeam model)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var result = Team.CreateTeam(model.Name, model.Description);

        //            if (result.Count == 0)
        //            {
        //                AddMessage(new SiteMessage { MessageType = SiteMessageType.Success, Message = "The team was successfully created" });
        //                return RedirectToActionPermanent("ViewTeams");
        //            }

        //            if (result[0] == CreateTeamEnum.Name_TooShort)
        //            {
        //                AddMessage(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The team name is too short" });
        //                var output = new GenericSingleModel<CreateTeam>(model);
        //                return View(output);
        //            }
        //            else
        //            {
        //                AddMessage(new SiteMessage
        //                               {
        //                                   MessageType = SiteMessageType.Error,
        //                                   Message = "An error occured and the information could not be saved"
        //                               });
        //                var output = new GenericSingleModel<CreateTeam>(model);
        //                return View(output);
        //            }
        //        }

        //        AddMessage(new SiteMessage { Message = "An unknown error occured.", MessageType = SiteMessageType.Error });
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }
        //    return View();
        //    return RedirectToAction("Roster");
        //}

        //[LeagueAuthorize(RequireEmailVerification = true, RequireLeagueMembership = true, RequireManagerStatus = true)]
        //[HttpPost]
        //public ActionResult ViewTeam(ViewTeam model)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var result = Team.UpdateTeam(model.TeamId, model.Name, model.Description);

        //            if (result.Count == 0)
        //            {
        //                AddMessage(new SiteMessage
        //                {
        //                    MessageType = SiteMessageType.Success,
        //                    Message = "The team has been updated"
        //                });
        //                return ViewTeam(model.TeamId.ToString());
        //            }

        //            if (result[0] == UpdateTeamEnum.Name_TooShort)
        //            {
        //                AddMessage(new SiteMessage
        //                    {
        //                        MessageType = SiteMessageType.Error,
        //                        Message = "The team name is too short"
        //                    });
        //            }
        //            else
        //            {
        //                AddMessage(new SiteMessage
        //                {
        //                    MessageType = SiteMessageType.Error,
        //                    Message = "An error occured and the information could not be saved"
        //                });
        //            }

        //            var output = new GenericSingleModel<ViewTeam>(model);
        //            return View(output);
        //        }

        //        AddMessage(new SiteMessage { Message = "An unknown error occured.", MessageType = SiteMessageType.Error });
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }

        //    return RedirectToAction("Roster");
        //}

        //[LeagueAuthorize(RequireEmailVerification = true, RequireLeagueMembership = true, RequireManagerStatus = true)]
        //public ActionResult RemoveMemberFromTeam(string id)
        //{
        //    try
        //    {
        //        var result = Team.RemoveMemberFromTeam(id, Request.UserHostAddress);
        //        if (result)
        //            AddMessage(new SiteMessage
        //            {
        //                Message = "The member has been successfully removed from the team.",
        //                MessageType = SiteMessageType.Success
        //            });
        //        else
        //            AddMessage(new SiteMessage
        //            {
        //                Message =
        //                    "The member could not be removed. Check that the member exists, is in the team and that the supplied member id is correct",
        //                MessageType = SiteMessageType.Error
        //            });
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }

        //    return RedirectToActionPermanent("ViewTeams");
        //}
        #endregion

        #endregion

        #region JSON
        public JsonResult GetLeagues(string id)
        {

            return Json(Library.Classes.League.LeagueFactory.GetLeagues(id), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Private Helpers

        private List<SiteMessage> VerifyEmailErrors(IEnumerable<VerifyEmailEnum> list)
        {
            var result = new List<SiteMessage>();
            try
            {
                foreach (VerifyEmailEnum item in list)
                {
                    switch (item)
                    {
                        case VerifyEmailEnum.Code_Email_Invalid:
                            result.Add(new SiteMessage
                                           {
                                               MessageType = SiteMessageType.Error,
                                               Message = "We couldn't find your verfication Id, Have you already Verified with RDNation? You should be able to use RDNation now."
                                           });
                            break;
                        case VerifyEmailEnum.Code_Invalid:
                            result.Add(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The code is invalid" });
                            break;
                        case VerifyEmailEnum.Email_Invalid:
                            result.Add(new SiteMessage
                                           {
                                               MessageType = SiteMessageType.Error,
                                               Message = "The email contains illegal characters"
                                           });
                            break;
                        case VerifyEmailEnum.Error_Save:
                            result.Add(new SiteMessage
                                           {
                                               MessageType = SiteMessageType.Error,
                                               Message = "An error occured when saving your data, try again in a little while"
                                           });
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return result;
        }

        private List<SiteMessage> JoinLeagueErrors(IEnumerable<JoinLeagueEnum> list)
        {
            var result = new List<SiteMessage>();
            try
            {
                foreach (JoinLeagueEnum item in list)
                {
                    switch (item)
                    {
                        case JoinLeagueEnum.League_NotFound:
                            result.Add(new SiteMessage
                                           {
                                               MessageType = SiteMessageType.Error,
                                               Message = "The league could not be found in the database"
                                           });
                            break;
                        case JoinLeagueEnum.League_InvalidId:
                            result.Add(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The league id is invalid" });
                            break;
                        case JoinLeagueEnum.Error_Save:
                            result.Add(new SiteMessage
                                           {
                                               MessageType = SiteMessageType.Error,
                                               Message = "An error occured when saving your data, try again in a little while"
                                           });
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return result;
        }

        private List<SiteMessage> CreateLeagueErrors(IEnumerable<CreateLeagueEnum> list)
        {
            var result = new List<SiteMessage>();
            try
            {
                foreach (CreateLeagueEnum item in list)
                {
                    switch (item)
                    {
                        case CreateLeagueEnum.Name_TooShort:
                            result.Add(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The league name is too short" });
                            break;
                        case CreateLeagueEnum.Name_Invalid:
                            result.Add(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The league name is invalid" });
                            break;
                        case CreateLeagueEnum.Email_Invalid:
                            result.Add(new SiteMessage
                                           {
                                               MessageType = SiteMessageType.Error,
                                               Message = "The email contains illegal characters"
                                           });
                            break;
                        case CreateLeagueEnum.Error_Save:
                            result.Add(new SiteMessage
                                           {
                                               MessageType = SiteMessageType.Error,
                                               Message = "An error occured when saving your data, try again in a little while"
                                           });
                            break;
                        case CreateLeagueEnum.Country_Invalid:
                            result.Add(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The country is invalid" });
                            break;
                        case CreateLeagueEnum.Federation_Invalid:
                            result.Add(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The federation is invalid" });
                            break;
                        case CreateLeagueEnum.State_TooShort:
                            result.Add(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The state is too short" });
                            break;
                        case CreateLeagueEnum.State_Invalid:
                            result.Add(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The state is invalid" });
                            break;
                        case CreateLeagueEnum.City_TooShort:
                            result.Add(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The city is too short" });
                            break;
                        case CreateLeagueEnum.City_Invalid:
                            result.Add(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The city is invalid" });
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return result;
        }

        private List<SiteMessage> CreateAccountErrors(IEnumerable<NewUserEnum> list)
        {
            var result = new List<SiteMessage>();
            try
            {
                foreach (NewUserEnum item in list)
                {
                    switch (item)
                    {
                        case NewUserEnum.Email_EmailRepeatIncorrect:
                            result.Add(new SiteMessage
                                           {
                                               MessageType = SiteMessageType.Error,
                                               Message = "The email address and confirmation email address do not match"
                                           });
                            break;
                        case NewUserEnum.Email_IllegalCharacters:
                            result.Add(new SiteMessage
                                           {
                                               MessageType = SiteMessageType.Error,
                                               Message = "The email contains illegal characters"
                                           });
                            break;
                        case NewUserEnum.Email_InUse:
                            result.Add(new SiteMessage
                                           {
                                               MessageType = SiteMessageType.Error,
                                               Message = "The email supplied is already in use"
                                           });
                            break;
                        case NewUserEnum.Email_IsEmpty:
                            result.Add(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The email field is empty" });
                            break;
                        case NewUserEnum.Error_Save:
                            result.Add(new SiteMessage
                                           {
                                               MessageType = SiteMessageType.Error,
                                               Message = "An error occured when saving your data, try again in a little while"
                                           });
                            break;
                        case NewUserEnum.Firstname_IllegalCharacters:
                            result.Add(new SiteMessage
                                           {
                                               MessageType = SiteMessageType.Error,
                                               Message = "The firstname contains illegal characters"
                                           });
                            break;
                        case NewUserEnum.Firstname_IsEmpty:
                            result.Add(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The firstname field is empty" });
                            break;
                        case NewUserEnum.Firstname_TooShort:
                            result.Add(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The firstname is too short" });
                            break;
                        case NewUserEnum.Password_IsEmpty:
                            result.Add(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The password field is empty" });
                            break;
                        case NewUserEnum.Password_PasswordRepeatIncorrect:
                            result.Add(new SiteMessage
                                           {
                                               MessageType = SiteMessageType.Error,
                                               Message = "The password and confirmation password do not match."
                                           });
                            break;
                        case NewUserEnum.Password_RuleViolated:
                            result.Add(new SiteMessage
                                           {
                                               MessageType = SiteMessageType.Error,
                                               Message =
                                                   "The number of special caracters/case letters and numbers is not fulfilled"
                                           });
                            break;
                        case NewUserEnum.Password_TooShort:
                            result.Add(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The password is too short" });
                            break;
                        case NewUserEnum.League_LeagueNotFound:
                            result.Add(new SiteMessage
                                           {
                                               MessageType = SiteMessageType.Error,
                                               Message = "The supplied league does not exist"
                                           });
                            break;
                        case NewUserEnum.UserName_IllegalCharacters:
                            result.Add(new SiteMessage
                                           {
                                               MessageType = SiteMessageType.Error,
                                               Message = "The username contains illegal characters"
                                           });
                            break;
                        case NewUserEnum.UserName_InUse:
                            result.Add(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The username is already in use" });
                            break;
                        case NewUserEnum.UserName_IsEmpty:
                            result.Add(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The username field is empty" });
                            break;
                        case NewUserEnum.UserName_TooShort:
                            result.Add(new SiteMessage { MessageType = SiteMessageType.Error, Message = "The username is too short" });
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return result;
        }

        //private void SetViewMemberTeamDropdownData(ref ViewMember model)
        //{
        //    try
        //    {
        //        List<Library.Classes.Team.Classes.Team> teams = Team.GetDropdownTeamsForLeague();
        //        foreach (Library.Classes.Team.Classes.Team team in teams)
        //        {
        //            var teamOut = new SelectListItem { Text = team.Name, Value = team.TeamId.ToString() };
        //            if (model.Team != null && model.Team.TeamId.Equals(team.TeamId))
        //                teamOut.Selected = true;
        //            else
        //                teamOut.Selected = false;
        //            model.Teams.Add(teamOut);
        //        }

        //        if (model.Team == null)
        //        {
        //            var teamOut = new SelectListItem { Text = "No team", Value = Guid.Empty.ToString(), Selected = true };
        //            model.Teams.Insert(0, teamOut);
        //        }
        //        else
        //        {
        //            var teamOut = new SelectListItem { Text = "No team", Value = Guid.Empty.ToString(), Selected = false };
        //            model.Teams.Insert(0, teamOut);
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }
        //}
        #endregion
    }
}