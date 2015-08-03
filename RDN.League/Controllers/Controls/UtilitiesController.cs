using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.League.Models.ActionResults;
using RDN.Library.Classes.Utilities;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Forum;
using RDN.Library.Classes.Colors;
using RDN.Library.Util.Enum;
using RDN.Library.Classes.Payment.Enums.Stripe;
using RDN.Library.Classes.Store;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Calendar;
using MoreLinq;
using RDN.Portable.Classes.Controls.Forum;
using RDN.Library.Classes.Messages;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class UtilitiesController : Controller
    {


        [Authorize]
        public ActionResult ConfirmFromStripe()
        {
            try
            {
                var stateId = HttpContext.Request.Params["state"];
                if (String.IsNullOrEmpty(stateId))
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                string returnType = stateId.Split('-')[0];
                string privId = stateId.Split('-')[1];

                StripeStateReturnCodeEnum typeOfReturn = (StripeStateReturnCodeEnum)Enum.Parse(typeof(StripeStateReturnCodeEnum), returnType);

                var errorCode = HttpContext.Request.Params["error"];
                if (!String.IsNullOrEmpty(errorCode))
                {
                    if (errorCode == StripeConnectCodes.access_denied.ToString())
                    {
                        var sg = new MerchantGateway();
                        var store = sg.GetMerchant(new Guid(privId));

                        if (store != null && typeOfReturn == StripeStateReturnCodeEnum.store)
                            return Redirect(Url.Content("~/store/settings/" + privId + "/" + store.MerchantId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.sced));
                        else if (store != null && typeOfReturn == StripeStateReturnCodeEnum.merchant)
                            return Redirect(Url.Content("~/merchant/settings?u=" + SiteMessagesEnum.sced));

                    }
                }
                var stripeCode = HttpContext.Request.Params["code"];
                if (!String.IsNullOrEmpty(stripeCode))
                {
                    var sg = new StoreGateway();
                    sg.UpdateStripeKey(stripeCode, new Guid(privId));
                    var mg = new MerchantGateway();
                    var store = mg.GetMerchant(new Guid(privId));

                    if (store != null && typeOfReturn == StripeStateReturnCodeEnum.store)
                        return Redirect(Url.Content("~/store/settings/" + privId + "/" + store.MerchantId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.sca));
                    else if (store != null && typeOfReturn == StripeStateReturnCodeEnum.merchant)
                        return Redirect(Url.Content("~/merchant/settings?u=" + SiteMessagesEnum.sca));

                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }


        [Authorize]
        public ActionResult sus(string email)
        {
            try
            {
                Guid id = RDN.Library.Classes.Account.User.SwitchMemberId(email);

                return Json(new { result = id }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType(), additionalInformation: email);
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// secret url so admins can clear the membership cache of a user.
        /// we hit this from the administration pages.
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public ActionResult ClearMemberCache1234(string memberId)
        {
            try
            {
                MemberCache.Clear(new Guid(memberId));
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType(), additionalInformation: memberId);
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ClearCurrencyExchangeRates()
        {
            try
            {
                SiteCache.ClearCurrencyExchanges();
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult AddColor(string nameOfColor, string hexOfColor)
        {
            try
            {
                bool re = ColorDisplayFactory.AddColor(nameOfColor, hexOfColor);
                return Json(new { isSuccess = re }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType(), additionalInformation: nameOfColor + ":" + hexOfColor);
                return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ClearLeagueMembersCache1234(string lid)
        {
            try
            {
                MemberCache.ClearLeagueMembersCache(new Guid(lid));
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType(), additionalInformation: lid);
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult SearchNames(string term)
        {
            var namesFound = RDN.Library.Classes.Account.User.SearchDerbyNames(term, 10);
            return Json(namesFound, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult SearchLeaguesNames(string q, int limit, string leagueId)
        //{
        //    List<string> namesFound = RDN.Library.Classes.League.League.SearchLeagueDerbyNames(q, limit, new Guid(leagueId));

        //    return Content(string.Join("\n", namesFound));
        //}
        public ActionResult SearchLeaguesNames(string term)
        {
            var leagueId = RDN.Library.Cache.MemberCache.GetLeagueIdOfMember(RDN.Library.Classes.Account.User.GetMemberId());
            var namesFound = RDN.Library.Classes.League.LeagueFactory.SearchLeagueDerbyNames(term, 10, leagueId);

            return Json(namesFound, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Search top 10 Members to be added in Message
        /// </summary>        
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult SearchNamesObjects(string q)
        {
            List<MemberJson> namesFound = new List<MemberJson>();
            var members = MemberCache.GetCurrentLeagueMembers(RDN.Library.Classes.Account.User.GetMemberId());
            var searchLeague = (from xx in members
                                where (xx.DerbyName != null && xx.DerbyName.ToLower().Contains(q))
                                || (xx.Firstname != null && xx.Firstname.ToLower().Contains(q))
                                || (xx.LastName != null && xx.LastName.ToLower().Contains(q))
                                select new MemberJson
                                       {
                                           name = xx.DerbyName,
                                           realname = xx.FullName,
                                           id = xx.MemberId
                                       }).Take(10).ToList();

            namesFound.AddRange(searchLeague);
            namesFound.AddRange(RDN.Library.Classes.Account.User.SearchDerbyNamesJson(q, 10));
            namesFound = namesFound.DistinctBy(x => x.id).Take(10).ToList();
            return Json(namesFound, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchNamesToAdd(string q, string messageGroupId)
        {
            List<MemberJson> new_members = new List<MemberJson>();
            var added_members = Messages.GetConversationMembers(Convert.ToInt64(messageGroupId));
            var total_members = MemberCache.GetCurrentLeagueMembers(RDN.Library.Classes.Account.User.GetMemberId());
            new_members = (from xx in total_members
                                where ((xx.DerbyName != null && xx.DerbyName.ToLower().Contains(q))
                                || (xx.Firstname != null && xx.Firstname.ToLower().Contains(q))
                                || (xx.LastName != null && xx.LastName.ToLower().Contains(q)))
                                && !added_members.Contains(xx.MemberId)
                                select new MemberJson
                                {
                                    link = Url.Content("~/Member/" + xx.MemberId.ToString().Replace("-", "") + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(xx.DerbyName)),
                                    picture= xx.Photos.Where(x => x.IsPrimaryPhoto == true).FirstOrDefault() != null  ? xx.Photos.Where(x => x.IsPrimaryPhoto == true).FirstOrDefault().ImageUrl : "",
                                    name = xx.DerbyName,
                                    id = xx.MemberId
                                }).Take(5).ToList();
            return Json(new_members, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchNamesForMention(string q, string messageGroupId)
        {
            List<MemberJson> namesFound = new List<MemberJson>();
            var members = MemberCache.GetCurrentLeagueMembers(RDN.Library.Classes.Account.User.GetMemberId());
            var searchLeague = (from xx in members
                                where (xx.DerbyName != null && xx.DerbyName.ToLower().Contains(q))
                                || (xx.Firstname != null && xx.Firstname.ToLower().Contains(q))
                                || (xx.LastName != null && xx.LastName.ToLower().Contains(q))
                                select new MemberJson
                                {
                                    picture = xx.Photos.Where(x => x.IsPrimaryPhoto == true).FirstOrDefault() != null ? xx.Photos.Where(x => x.IsPrimaryPhoto == true).FirstOrDefault().ImageUrl : "",
                                    name = xx.DerbyName,
                                    id = xx.MemberId
                                }).Take(10).ToList();
            namesFound.AddRange(searchLeague);
            namesFound.AddRange(RDN.Library.Classes.Account.User.SearchDerbyNamesJson(q, 10));
            namesFound = namesFound.DistinctBy(x => x.id).Take(10).ToList();
            return Json(namesFound, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchLeagues(string term)
        {
            var namesFound = RDN.Library.Classes.League.LeagueFactory.SearchForLeagueName(term, 10);

            return Json(namesFound, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchFederations(string q, int limit)
        {
            List<string> namesFound = RDN.Library.Classes.Federation.Federation.SearchForFederationName(q, limit);

            return Content(string.Join("\n", namesFound));
        }



        public JsonResult GetCountries()
        {
            var countries = RDN.Library.Classes.Location.LocationFactory.GetCountriesList();
            return Json(countries, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// searches for league names
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult SearchForLeagueName(string name)
        {
            var leagueNames = RDN.Library.Classes.League.LeagueFactory.SearchForLeagueName(name);
            return Json(new { result = true, names = leagueNames }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchLeagueCategories(string fId, string gId)
        {
            var cats = RDN.Library.Classes.Forum.Forum.GetCategoriesOfForum(new Guid(fId), Convert.ToInt64(gId));
            cats.Insert(0, new ForumCategory() { CategoryId = 0, CategoryName = "None" });
            return Json(new { result = true, names = cats }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// adds a plus one to the post view count.
        /// </summary>
        /// <param name="forumId"></param>
        /// <param name="topicId"></param>
        /// <returns></returns>
        public ActionResult AddPostViewToCount(string forumId, string topicId)
        {
            RDN.Library.Classes.Forum.Forum.UpdatePostViewCount(new Guid(forumId), Convert.ToInt64(topicId), RDN.Library.Classes.Account.User.GetMemberId());
            
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }
        



    }
}