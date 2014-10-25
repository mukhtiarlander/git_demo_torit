using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Util.Enum;
using RDN.League.Classes.Enums;
using System.Collections.Specialized;
using RDN.Library.Util;
using RDN.Portable.Classes.Contacts;
using RDN.Portable.Classes.Contacts.Enums;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class ContactController : BaseController
    {
        [Authorize]
        public ActionResult AddContact(string type)
        {
            try
            {
                ContactDisplayBasic con = new ContactDisplayBasic();
                con.ContactType = (ContactTypeEnum)Enum.Parse(typeof(ContactTypeEnum), type);
                var countries = RDN.Library.Classes.Location.LocationFactory.GetCountries();
                ViewBag.Countries = new SelectList(countries, "CountryId", "Name");
                ViewData["ContactTypeForOrg"] = con.ContactTypeForOrg.ToSelectList();
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string u = nameValueCollection["u"];
                if (u == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Added New Contact.";
                    this.AddMessage(message);
                }
                return View(con);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect("~/?u=" + SiteMessagesEnum.sww);
        }
        [Authorize]
        public ActionResult EditContact(string type, Guid contactId)
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string u = nameValueCollection["u"];
                if (u == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Edited Contact.";
                    this.AddMessage(message);
                }

                Dictionary<int, string> countries = RDN.Library.Classes.Location.LocationFactory.GetCountriesDictionary();
                var counTemp = countries.Select(item => new SelectListItem { Text = item.Value, Value = item.Key.ToString() }).ToList();

                if (type == ContactTypeEnum.league.ToString())
                {
                    var memId = RDN.Library.Classes.Account.User.GetMemberId();
                    var league = MemberCache.GetLeagueOfMember(memId);
                    ContactDisplayBasic con = league.Contacts.Where(x => x.ContactId == contactId).FirstOrDefault();
                    ViewBag.Countries = new SelectList(counTemp, "Value", "Text", con.CountryId);
                    ViewData["ContactTypeForOrg"] = con.ContactTypeForOrg.ToSelectList();

                    return View(con);
                }


            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect("~/?u=" + SiteMessagesEnum.sww);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddContact(ContactDisplayBasic contact)
        {
            try
            {
                bool createAnother = false;
                if (Request.Form["createAnother"] != null)
                    createAnother = true;

                bool isSuccess = false;

                if (contact.ContactType == ContactTypeEnum.league)
                {
                    var memId = RDN.Library.Classes.Account.User.GetMemberId();
                    var league = MemberCache.GetLeagueOfMember(memId);
                    isSuccess = RDN.Library.Classes.Contacts.Contact.AddLeagueContact(league.LeagueId, contact);
                    MemberCache.ClearLeagueMembersCache(league.LeagueId);
                    MemberCache.ClearLeagueMembersApiCache(league.LeagueId);
                }

                if (createAnother && isSuccess)
                    return Redirect(Url.Content("~/contact/add/" + contact.ContactType + "?u=" + SiteMessagesEnum.s));
                else if (isSuccess)
                    return Redirect(Url.Content("~/" + contact.ContactType + "/contacts?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect("~/?u=" + SiteMessagesEnum.sww);
        }
        [Authorize]
        [HttpPost]
        public ActionResult EditContact(ContactDisplayBasic contact)
        {
            try
            {

                bool isSuccess = false;

                if (contact.ContactType == ContactTypeEnum.league)
                {
                    var memId = RDN.Library.Classes.Account.User.GetMemberId();
                    var league = MemberCache.GetLeagueOfMember(memId);
                    isSuccess = RDN.Library.Classes.Contacts.Contact.EditLeagueContact(league.LeagueId, contact);
                    MemberCache.ClearLeagueMembersCache(league.LeagueId);
                    MemberCache.ClearLeagueMembersApiCache(league.LeagueId);
                }

                if (isSuccess)
                    return Redirect(Url.Content("~/" + contact.ContactType + "/contacts?u=" + SiteMessagesEnum.su));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect("~/?u=" + SiteMessagesEnum.sww);
        }

    }
}
