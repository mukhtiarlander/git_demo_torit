using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.League.Models.Filters;
using RDN.League.Models.Federation;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Federation;
using RDN.Library.Cache;
using RDN.League.Models.Utilities;
using RDN.League.Models.OutModel;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Federation.Enums;
using System.Web.Security;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.API.Federation;
using RDN.Library.Classes.Config;
using RDN.Library.Util.Enum;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    [HandleError]
    public class FederationController : BaseController
    {
        /// <summary>
        /// default page size for table contents.
        /// </summary>
        private int PAGE_SIZE = 50;

        //public ActionResult ImportMadeMembers()
        //{
        //    Federation.InportMembersFromList();

        //    return View();
        //}

        FederationManager _manager = new FederationManager(LibraryConfig.ApiSite, LibraryConfig.ApiKey);
        /// <summary>
        /// home page for the federation.
        /// </summary>
        /// <returns></returns>
        [FederationAuthorize(RequireFederationManagerStatus = true)]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult Home()
        {

            FederationHome home = new FederationHome();
            try
            {
                var federation = MemberCache.GetAllOwnedFederations(RDN.Library.Classes.Account.User.GetMemberId()).FirstOrDefault().Federation;
                //var members = Federation.GetNewMembersAddedToLeaguesSinceLogin(federation.FederationId, DateTime.UtcNow.AddDays(-5));
                //home.NewMembersSinceLastLogin = members.Count;
                home.FederationName = federation.Name;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return View(home);
        }


        /// <summary>
        /// the view for adding members to the federation.
        /// </summary>
        /// <returns></returns>
        [FederationAuthorize(RequireFederationManagerStatus = true)]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult AddMembers()
        {
            ViewBag.Federation = MemberCache.GetAllOwnedFederations(RDN.Library.Classes.Account.User.GetMemberId()).FirstOrDefault().Federation;

            return View();
        }
        /// <summary>
        /// bulk adds members to the federation.
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        [HttpPost]
        [FederationAuthorize(RequireFederationManagerStatus = true)]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public JsonResult AddMembers(List<MemberDisplay> members)
        {
            try
            {
                var fed = MemberCache.GetAllOwnedFederations(RDN.Library.Classes.Account.User.GetMemberId()).FirstOrDefault();
                foreach (var mem in members)
                {
                    RDN.Library.Classes.Account.User.CreateMemberForFederation(mem, fed.Federation.FederationId);
                }

                var url = Url.Content("~/Federation/Home");
                return Json(new { result = "true", url = url });
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new { result = "false" });
            }
        }

        /// <summary>
        /// displays the league for the federation.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="leagueName"></param>
        /// <returns></returns>
        [FederationAuthorize(RequireFederationManagerStatus = true)]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult League(string id, string name)
        {
            ViewBag.FederationName = MemberCache.GetAllOwnedFederations(RDN.Library.Classes.Account.User.GetMemberId()).FirstOrDefault().Federation.Name;

            var league = RDN.Library.Classes.League.LeagueFactory.GetLeague(new Guid(id));

            return View(league);
        }
        [FederationAuthorize(RequireFederationManagerStatus = true)]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult EditLeague(string id, string name)
        {
            var fed = MemberCache.GetAllOwnedFederations(RDN.Library.Classes.Account.User.GetMemberId()).FirstOrDefault().Federation;
            ViewBag.FederationName = fed.Name;

            var league = RDN.Library.Classes.League.LeagueFactory.GetLeagueForFederation(fed.FederationId, new Guid(id));
            ViewBag.Saved = false;

            return View(league);
        }
        [HttpPost]
        [FederationAuthorize(RequireFederationManagerStatus = true)]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult EditLeague(RDN.Portable.Classes.League.Classes.League league)
        {
            var federation = MemberCache.GetAllOwnedFederations(RDN.Library.Classes.Account.User.GetMemberId()).FirstOrDefault().Federation;

            ViewBag.FederationName = federation.Name;
            RDN.Library.Classes.League.LeagueFactory.UpdateLeagueForFederation(federation.FederationId, league);

            league = RDN.Library.Classes.League.LeagueFactory.GetLeagueForFederation(federation.FederationId, league.LeagueId);
            ViewBag.Saved = true;

            return View(league);
        }

        [HttpPost]
        [FederationAuthorize(RequireFederationManagerStatus = true)]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult RemoveLeague(RDN.Portable.Classes.League.Classes.League model)
        {
            try
            {
                var federation = MemberCache.GetAllOwnedFederations(RDN.Library.Classes.Account.User.GetMemberId()).FirstOrDefault().Federation;
                Federation.DisconnectLeagueFromFederation(federation.FederationId, model.LeagueId);
                return RedirectToAction("ViewLeagues");
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return RedirectToAction("ViewLeagues");
            }
        }


        /// <summary>
        /// allows the federation to view the leagues attached to it.
        /// </summary>
        /// <returns></returns>
        [FederationAuthorize(RequireFederationManagerStatus = true)]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult ViewLeagues()
        {
            try
            {
                var federation = MemberCache.GetAllOwnedFederations(RDN.Library.Classes.Account.User.GetMemberId()).FirstOrDefault().Federation;
                ViewBag.FederationName = federation.Name;

                var model = new SimpleModPager<RDN.Portable.Classes.League.Classes.League>();
                model.CurrentPage = 1;
                model.NumberOfRecords = Federation.GetNumberLeaguesInFederations(federation.FederationId);
                model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / PAGE_SIZE);

                var output = FillLeagueModel(model, federation.FederationId);
                return View(output);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());

            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ViewLeagues(SimpleModPager<RDN.Portable.Classes.League.Classes.League> model)
        {
            try
            {
                var federation = MemberCache.GetAllOwnedFederations(RDN.Library.Classes.Account.User.GetMemberId()).FirstOrDefault().Federation;

                var output = FillLeagueModel(model, federation.FederationId);
                return View(output);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return null;
        }

        [FederationAuthorize(RequireFederationManagerStatus = true)]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult ViewMembers()
        {
            try
            {
                var federation = MemberCache.GetAllOwnedFederations(RDN.Library.Classes.Account.User.GetMemberId()).FirstOrDefault().Federation;
                ViewBag.FederationName = federation.Name;

                var model = new SimpleModPager<MemberDisplayFederation>();
                model.CurrentPage = 1;
                model.NumberOfRecords = Federation.GetNumberOfMembersInFederation(federation.FederationId);

                model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / model.NumberOfRecords);

                var output = FillMembersModel(model, federation.FederationId);
                return View(output);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return View();
        }
        [FederationAuthorize(RequireFederationManagerStatus = true)]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult ViewMembersJson(string sEcho)
        {
            try
            {
                var federation = MemberCache.GetAllOwnedFederations(RDN.Library.Classes.Account.User.GetMemberId()).FirstOrDefault().Federation;
                var mems = _manager.GetMembersAsync(federation.FederationId).Result;

                return Json(new
                {
                    sEcho = sEcho,
                    iTotalRecords = mems.Count,
                    iTotalDisplayRecords = 50,
                    aaData = mems
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// creates a post from the page and allows us to page the members page.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult ViewMembers(SimpleModPager<MemberDisplayFederation> model)
        {
            try
            {
                var federation = MemberCache.GetAllOwnedFederations(RDN.Library.Classes.Account.User.GetMemberId()).FirstOrDefault();
                if (federation != null)
                {
                    var output = FillMembersModel(model, federation.Federation.FederationId);
                    return View(output);
                }
                return Redirect(Url.Content("~/"));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());

            }
            return View();
        }
        /// <summary>
        /// allows the federation to edit certain things about this member.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [FederationAuthorize(RequireFederationManagerStatus = true)]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult EditMember(string id, string name)
        {
            var member = RDN.Library.Cache.MemberCache.GetMemberDisplay(new Guid(id));
            try
            {
                var federation = MemberCache.GetAllOwnedFederations(RDN.Library.Classes.Account.User.GetMemberId()).FirstOrDefault().Federation;
                var list = member.FederationsApartOf.Where(x => x.FederationId != federation.FederationId).ToList();
                foreach (var fed in list)
                    member.FederationsApartOf.Remove(fed);

                member.LeaguesToChooseFrom = RDN.Library.Classes.League.LeagueFactory.GetLeaguesInFederation(federation.FederationId);
                member.LeaguesToChooseFrom.Add(new RDN.Portable.Classes.League.Classes.League { Name = "", LeagueId = new Guid() });
                if (member.Leagues.Count == 0)
                    member.Leagues.Add(new RDN.Portable.Classes.League.Classes.League());
                ViewBag.Saved = false;
                ViewBag.Removed = false;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return View();
            }
            return View(member);
        }
        [HttpPost]
        [FederationAuthorize(RequireFederationManagerStatus = true)]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult EditMember(MemberDisplay model, HttpPostedFileBase file)
        {
            var mem = RDN.Library.Cache.MemberCache.GetMemberDisplay(model.MemberId);
            var federation = MemberCache.GetAllOwnedFederations(RDN.Library.Classes.Account.User.GetMemberId()).FirstOrDefault().Federation;
            model.LeaguesToChooseFrom = RDN.Library.Classes.League.LeagueFactory.GetLeaguesInFederation(federation.FederationId);
            if (mem.Leagues.Count == 0)
                mem.Leagues.Add(new RDN.Portable.Classes.League.Classes.League());

            var list = mem.FederationsApartOf.Where(x => x.FederationId != federation.FederationId).ToList();
            foreach (var fed in list)
                mem.FederationsApartOf.Remove(fed);
            try
            {
                foreach (var feds in mem.FederationsApartOf)
                {
                    var rank = HttpContext.Request.Form["CLASSRANK[" + feds.FederationId + "]"].ToString();
                    var owner = HttpContext.Request.Form["FEDOwner[" + feds.FederationId + "]"].ToString();
                    var memberType = HttpContext.Request.Form["MEMBERTYPE[" + feds.FederationId + "]"].ToString();
                    var memberDate = HttpContext.Request.Form["MembershipDate-" + feds.FederationId].ToString();
                    feds.MembershipId = HttpContext.Request.Form["MembershipId" + feds.FederationId].ToString();

                    feds.MADEClassRank = ((MADEClassRankEnum)Enum.ToObject(typeof(MADEClassRankEnum), Convert.ToInt32(rank))).ToString();
                    feds.OwnerType = ((FederationOwnerEnum)Enum.ToObject(typeof(FederationOwnerEnum), Convert.ToInt32(owner))).ToString();
                    feds.MemberType = ((MemberTypeFederationEnum)Enum.ToObject(typeof(MemberTypeFederationEnum), Convert.ToInt32(memberType))).ToString();
                    if (String.IsNullOrEmpty(memberDate))
                        feds.MembershipDate = new DateTime();
                    else
                        feds.MembershipDate = Convert.ToDateTime(memberDate);
                    _manager.ClearCacheAsync(feds.FederationId);

                }
                foreach (var league in mem.Leagues)
                {
                    string newLeague = HttpContext.Request.Form["NEWLEAGUE[" + league.LeagueId + "]"].ToString();
                    league.LeagueMovedId = new Guid(newLeague);
                }

                if (model.DerbyName != null)
                    mem.DerbyName = model.DerbyName;
                if (model.PlayerNumber != null)
                    mem.PlayerNumber = model.PlayerNumber;
                if (model.Firstname != null)
                    mem.Firstname = model.Firstname;
                if (model.LastName != null)
                    mem.LastName = model.LastName;
                mem.StartedSkating = model.StartedSkating;
                mem.StoppedSkating = model.StoppedSkating;
                mem.Email = model.Email;
                mem.PhoneNumber = model.PhoneNumber;

                if (file == null)
                    RDN.Library.Classes.Account.User.UpdateMemberDisplayForFederation(mem);
                else
                    RDN.Library.Classes.Account.User.UpdateMemberDisplayForFederation(mem, file.InputStream, file.FileName);

                MemberCache.Clear(mem.MemberId);
                MemberCache.ClearApiCache(mem.MemberId);

                mem = RDN.Library.Cache.MemberCache.GetMemberDisplay(model.MemberId);

                mem.LeaguesToChooseFrom = model.LeaguesToChooseFrom;

                ViewBag.Removed = false;
                ViewBag.Saved = true;
                return View(mem);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return View(model);
        }
        /// <summary>
        /// disconnects the member from the federation without actually removing the member.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [FederationAuthorize(RequireFederationManagerStatus = true)]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult RemoveMember(MemberDisplay model)
        {
            try
            {
                var federation = MemberCache.GetAllOwnedFederations(RDN.Library.Classes.Account.User.GetMemberId()).FirstOrDefault().Federation;
                Federation.DisconnectMemberFromFederation(federation.FederationId, model.MemberId);
                RDN.Library.Cache.MemberCache.Clear(model.MemberId);
                MemberCache.ClearApiCache(model.MemberId);
                _manager.ClearCacheAsync(federation.FederationId);
                ViewBag.Removed = true;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return RedirectToAction("ViewMembers");
        }

        /// <summary>
        /// converts the large members list into a single model to display on the view.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="federationId"></param>
        /// <returns></returns>
        private GenericSingleModel<SimpleModPager<MemberDisplayFederation>> FillMembersModel(SimpleModPager<MemberDisplayFederation> model, Guid federationId)
        {
            var output = new GenericSingleModel<SimpleModPager<MemberDisplayFederation>> { Model = model };

            try
            {
                for (var i = 1; i <= model.NumberOfPages; i++)
                    model.Pages.Add(new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString(),
                        Selected = i == model.CurrentPage
                    });

                output.Model.Items = _manager.GetMembersAsync(federationId).Result.ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return output;
        }

        /// <summary>
        /// converts the large league list into a single model to display on the view.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="federationId"></param>
        /// <returns></returns>
        private GenericSingleModel<SimpleModPager<RDN.Portable.Classes.League.Classes.League>> FillLeagueModel(SimpleModPager<RDN.Portable.Classes.League.Classes.League> model, Guid federationId)
        {
            var output = new GenericSingleModel<SimpleModPager<RDN.Portable.Classes.League.Classes.League>> { Model = model };

            try
            {
                for (var i = 1; i <= model.NumberOfPages; i++)
                    model.Pages.Add(new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString(),
                        Selected = i == model.CurrentPage
                    });

                output.Model.Items = RDN.Library.Classes.League.LeagueFactory.GetLeaguesInFederation(federationId, (model.CurrentPage - 1) * PAGE_SIZE, PAGE_SIZE);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());

            }
            return output;

        }


        /// <summary>
        /// allows the user to set up their federation.
        /// </summary>
        /// <returns></returns>
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult Setup()
        {
            return View();
        }
        /// <summary>
        /// allows owners to setup their new federations.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = false)]
        public ActionResult Setup(RegisterFederation model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var member = Library.Classes.Account.User.GetMember();

                    var federation = Federation.CreateFederation(model.FederationName, model.FederationEmail, model.FederationPhoneNumber, member);
                    if (federation == null)
                        model.FederationCreated = false;
                    else
                        model.FederationCreated = true;

                    MemberCache.Clear(member.MemberId);
                    MemberCache.ClearApiCache(member.MemberId);
                    return View(model);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return View();
            }
            return View(model);
        }

        public ActionResult Join()
        {

            FederationJoin federationJoin = new FederationJoin();

            var memberid = Library.Classes.Account.User.GetMemberId();
            var league = MemberCache.GetLeagueOfMember(memberid);

            var federations = Federation.GetLeagueFederationsByLeagueId(league.LeagueId);

            federationJoin.Federations = new SelectList(federations.Federations, "FederationId", "FederationName");

            federationJoin.League.Federations = federations.League.Federations;

            return View(federationJoin);
        }

        [HttpPost]
        public ActionResult Join(FederationJoin model)
        {
            FederationJoin federationJoin = new FederationJoin();
            var memberid = Library.Classes.Account.User.GetMemberId();
            var league = MemberCache.GetLeagueOfMember(memberid);  

            bool isSuccess=Federation.JoinFederation(new Guid(model.SelectedFederation), league.LeagueId);

            if (isSuccess)
               return  RedirectToAction("Join");

            return View();
        }

        public ActionResult DeleteJoinedFederation(Guid federationId, Guid leagueId)
        {
            var issuccess = false;
            try
            {
                 issuccess = Federation.DeleteLeagueFederation(federationId, leagueId);               
            }

            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = issuccess }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ValidateFederationAlradyJoin(Guid federationId, Guid leagueId)
        {
            bool isFederationAlreadyJoin = Federation.ValidateFederationAlradyJoin(federationId, leagueId);

           return Json(new { isExists = isFederationAlreadyJoin }, JsonRequestBehavior.AllowGet);           
        }
    }
}
