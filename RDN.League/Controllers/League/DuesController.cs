using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Dues;
using RDN.League.Models.Filters;
using RDN.Library.Classes.Error.Classes;
using RDN.Library.Classes.Error;
using RDN.League.Models.Dues;
using RDN.Library.Classes.AutomatedTask;
using RDN.League.Models.Enum;
using System.Collections.Specialized;
using RDN.League.Models.Utilities;
using RDN.Library.Classes.Dues.Enums;
using RDN.League.Classes.Enums;
using RDN.Library.Classes.Payment;
using RDN.Utilities.Config;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.DataModels.PaymentGateway.Invoices;
using RDN.Library.Cache;
using RDN.Library.Util.Enum;
using RDN.Library.Util;
using OfficeOpenXml;
using RDN.Portable.Config;
using RDN.Library.Classes.Controls.Dues;
using RDN.Library.Cache.Singletons;
using RDN.Portable.Classes.Controls.Dues.Enums;
using RDN.Portable.Classes.Controls.Dues.Classify;
using RDN.Portable.Classes.Controls.Dues;
using RDN.Portable.Classes.Payment.Enums;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class DuesController : BaseController
    {
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsManager = true, HasPaidSubscription = true, IsTreasurer = true)]
        public JsonResult RemoveDuesPayment(string duesItemId, string duesManagementId, string memId, string duesCollectedId)
        {
            try
            {
                bool isSuccessful = DuesFactory.RemoveDuesCollectionItem(Convert.ToInt64(duesItemId), new Guid(duesManagementId), new Guid(memId), Convert.ToInt64(duesCollectedId));

                return Json(new { isSuccess = isSuccessful }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsTreasurer = true, IsManager = true, HasPaidSubscription = true)]
        public ActionResult EditMemberDuesItem(string duesItemId, string duesManagementId, string memberId)
        {
            try
            {
                var duesMember = DuesFactory.GetDuesCollectionItemForMember(Convert.ToInt64(duesItemId), new Guid(duesManagementId), new Guid(memberId));

                return View(duesMember);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public JsonResult SendEmailReminderWithstanding(string duesManagementId, string memId, string leagueId)
        {
            try
            {
                bool isSuccessful = DuesFactory.SendEmailReminderWithStanding(new Guid(duesManagementId), new Guid(memId), new Guid(leagueId));

                return Json(new { isSuccess = isSuccessful }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public JsonResult SendEmailReminder(string duesItemId, string duesManagementId, string memId, string leagueId)
        {
            try
            {
                bool isSuccessful = DuesFactory.SendEmailReminder(Convert.ToInt64(duesItemId), new Guid(duesManagementId), new Guid(memId), new Guid(leagueId));

                return Json(new { isSuccess = isSuccessful }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public JsonResult SendEmailReminderAll(string duesItemId, string duesManagementId)
        {
            try
            {
                bool isSuccessful = DuesFactory.SendEmailReminderAll(Convert.ToInt64(duesItemId), new Guid(duesManagementId));

                return Json(new { isSuccess = isSuccessful }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public JsonResult PayDuesAmount(string duesId, string duesManagementId, string amountPaid, string memberId, string note)
        {
            try
            {
                bool isSuccessful = DuesFactory.PayDuesAmount(Convert.ToInt64(duesId), new Guid(duesManagementId), Convert.ToDouble(amountPaid), new Guid(memberId), note);

                return Json(new { isSuccess = isSuccessful }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true)]
        public JsonResult SetDuesAmount(string duesId, string duesManagementId, string amountDue, string memberId)
        {
            try
            {
                bool isSuccessful = DuesFactory.SetDuesAmount(Convert.ToInt64(duesId), new Guid(duesManagementId), Convert.ToDouble(amountDue), new Guid(memberId));

                return Json(new { isSuccess = isSuccessful }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public JsonResult WaiveDuesAmount(string duesId, string duesManagementId, string memberId, string note)
        {
            try
            {
                bool isSuccessful = DuesFactory.WaiveDuesAmount(Convert.ToInt64(duesId), new Guid(duesManagementId), new Guid(memberId), note);
                return Json(new { isSuccess = isSuccessful }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public JsonResult RemoveDuesWaived(string duesId, string duesManagementId, string memberId, string note)
        {
            try
            {
                bool isSuccessful = DuesFactory.RemoveDuesWaived(Convert.ToInt64(duesId), new Guid(duesManagementId), new Guid(memberId));
                return Json(new { isSuccess = isSuccessful }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult DuesItem(string duesItemId, string duesManagementId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                if (league != null)
                    SetCulture(league.CultureSelected);

                var dues = DuesFactory.GetDuesCollectionItem(Convert.ToInt64(duesItemId), new Guid(duesManagementId), RDN.Library.Classes.Account.User.GetMemberId());

                return View(dues);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        #region Classifications


        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult DuesClassification(string type, string id, string duesManagementId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string u = nameValueCollection["u"];

                if (u == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Added Classification.";
                    this.AddMessage(message);
                }
                else if (u == SiteMessagesEnum.su.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Updated Classification.";
                    this.AddMessage(message);
                }
                else if (u == SiteMessagesEnum.de.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Deleted Classification.";
                    this.AddMessage(message);
                }
                else if (u == SiteMessagesEnum.sww.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Something went wrong. Please contact info@rdnation.com if this continues not to work.";
                    this.AddMessage(message);
                }
                var classification = FeeClassificationFactory.PullClassifications(new Guid(duesManagementId), memId);


                return View(classification);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public JsonResult ChangeDuesClassification(string duesId, string memberId, string classification)
        {
            try
            {

                var isSuccessful = FeeClassificationFactory.ChangeClassificationForMember(new Guid(duesId), Convert.ToInt64(classification), new Guid(memberId));


                return Json(new { isSuccess = isSuccessful }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult DuesClassificationNew(string type, string id, string duesManagementId)
        {
            try
            {
                FeeClassified fee = new FeeClassified();
                fee.DuesId = new Guid(duesManagementId);
                fee.OwnerEntity = type;
                fee.LeagueOwnerId = new Guid(id);
                fee.FeeRequired = 45.00;
                fee.FeeRequiredInput = "45.00";

                return View(fee);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult DuesClassificationNew(FeeClassified fee)
        {
            try
            {
                fee.FeeRequired = Convert.ToDouble(fee.FeeRequiredInput);
                var isSuccess = FeeClassificationFactory.AddNewClassification(fee);

                if (isSuccess)
                    return Redirect(Url.Content("~/dues/classification/" + fee.OwnerEntity + "/" + fee.LeagueOwnerId.ToString().Replace("-", "") + "/" + fee.DuesId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.sac));
                return Redirect(Url.Content("~/dues/classification/" + fee.OwnerEntity + "/" + fee.LeagueOwnerId.ToString().Replace("-", "") + "/" + fee.DuesId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.sww));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult DuesClassificationEdit(string type, string id, string duesManagementId, string classId)
        {
            try
            {
                var classification = FeeClassificationFactory.PullClassification(new Guid(duesManagementId), Convert.ToInt64(classId));


                return View(classification);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult DuesClassificationEdit(FeeClassified fee)
        {
            try
            {
                fee.FeeRequired = Convert.ToDouble(fee.FeeRequiredInput);
                var isSuccess = FeeClassificationFactory.UpdateClassification(fee);


                if (isSuccess)
                    return Redirect(Url.Content("~/dues/classification/" + fee.OwnerEntity + "/" + fee.LeagueOwnerId.ToString().Replace("-", "") + "/" + fee.DuesId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.su));
                return Redirect(Url.Content("~/dues/classification/" + fee.OwnerEntity + "/" + fee.LeagueOwnerId.ToString().Replace("-", "") + "/" + fee.DuesId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.sww));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult DuesClassificationDelete(string type, string id, string duesManagementId, string classId)
        {
            try
            {
                var isSuccess = FeeClassificationFactory.DeleteClassification(new Guid(duesManagementId), Convert.ToInt64(classId));
                if (isSuccess)
                    return Redirect(Url.Content("~/dues/classification/" + type + "/" + id + "/" + duesManagementId + "?u=" + SiteMessagesEnum.de));
                return Redirect(Url.Content("~/dues/classification/" + type + "/" + id + "/" + duesManagementId + "?u=" + SiteMessagesEnum.sww));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        #endregion

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult EditDuesItem(string duesItemId, string duesManagementId)
        {
            try
            {
                var dues = DuesFactory.GetDuesCollectionItem(Convert.ToInt64(duesItemId), new Guid(duesManagementId), RDN.Library.Classes.Account.User.GetMemberId());
                ViewBag.Saved = false;
                return View(dues);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult EditDuesItem(DuesPortableModel duesItem)
        {
            try
            {
                DateTime dateTime = Convert.ToDateTime(HttpContext.Request.Form["PayBy"]);
                double payBy = Convert.ToDouble(HttpContext.Request.Form["DuesCost"]);
                long duesItemId = Convert.ToInt64(HttpContext.Request.Form["DuesItemId"]);

                DuesFactory.UpdateDuesCollectionItem(duesItemId, duesItem.DuesId, dateTime, payBy);
                var dues = DuesFactory.GetDuesCollectionItem(duesItemId, duesItem.DuesId, RDN.Library.Classes.Account.User.GetMemberId());
                ViewBag.Saved = true;

                return View(dues);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult DeleteDuesItem(DuesPortableModel duesItem)
        {
            try
            {
                long duesItemId = Convert.ToInt64(HttpContext.Request.Form["DuesItemId"]);
                bool isSuccess = DuesFactory.DeleteDuesCollectionItem(duesItemId, duesItem.DuesId);
                if (!isSuccess)
                    return Redirect(Url.Content("~/dues/" + duesItem.OwnerEntity + "/" + duesItem.LeagueOwnerId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.sww));
                return Redirect(Url.Content("~/dues/" + duesItem.OwnerEntity + "/" + duesItem.LeagueOwnerId.ToString().Replace("-", "")));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }


        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult CreateDuesManagement(DuesPortableModel dues)
        {
            try
            {
                DuesFactory.CreateDuesObject(dues.LeagueOwnerId, Library.Classes.Dues.Enums.DuesOwnerEntityEnum.league);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/dues/" + dues.OwnerEntity + "/" + dues.LeagueOwnerId.ToString().Replace("-", "")));
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public ActionResult DuesManagement(string type, string id)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(id)))
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sww.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Error;
                    message.Message = "Something went wrong. Error sent to developers, please try again later.";
                    this.AddMessage(message);
                }

                var league = MemberCache.GetLeagueOfMember(memId);
                if (league != null)
                    SetCulture(league.CultureSelected);

                DuesPortableModel dues = new DuesPortableModel();
                dues.LeagueOwnerId = new Guid(id);
                dues.OwnerEntity = type;
                var dues2 = DuesFactory.GetDuesObject(new Guid(id), memId);
                if (dues2.DuesId != new Guid())
                    dues = dues2;

                if (MemberCache.IsTreasurerOrBetterOfLeague(memId) && !dues.AcceptPaymentsOnline)
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Did You know you can accept Dues Payments online?  Go to the settings to get started.";
                    this.AddMessage(message);
                }

                return View(dues);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }

        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public ActionResult DuesManagementByMember(string type, string id)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sww.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Something went wrong. Error sent to developers, please try again later.";
                    this.AddMessage(message);
                }
                if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(id)))
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                var league = MemberCache.GetLeagueOfMember(memId);
                if (league != null)
                    SetCulture(league.CultureSelected);

                DuesPortableModel dues = new DuesPortableModel();
                dues.LeagueOwnerId = new Guid(id);
                dues.OwnerEntity = type;
                var dues2 = DuesFactory.GetDuesObject(new Guid(id), memId);
                if (dues2.DuesId != new Guid())
                    dues = dues2;
                return View(dues);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult DuesForMember(string leagueId, string memberId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                //member must belong to correct league.
                if (!MemberCache.IsMemberApartOfLeague(new Guid(memberId), new Guid(leagueId)))
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                //must be a manager or the actual person to access this page.
                if (!MemberCache.IsManagerOrBetterOfLeague(memId) && memId != new Guid(memberId))
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.ppldnc.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Error;
                    message.Message = "League hasn't yet verified their PayPal Account. They have been notified. Please pay another way until they have verified their account with PayPal.";
                    this.AddMessage(message);
                }

                var league = MemberCache.GetLeagueOfMember(memId);
                if (league != null)
                    SetCulture(league.CultureSelected);

                var dues = DuesFactory.GetDuesObject(new Guid(leagueId), new Guid(memberId));
                var mem = MemberCache.GetMemberDisplay(new Guid(memberId));
                dues.CurrentMemberDerbyName = mem.DerbyName;
                dues.CurrentMemberId = mem.MemberId;
                return View(dues);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsManager = true, HasPaidSubscription = true, IsTreasurer = true)]
        public ActionResult DuesSettings(string type, string id, string duesId)
        {
            DuesModel dues = new DuesModel();
            try
            {
                DuesPortableModel due = DuesFactory.GetDuesSettings(new Guid(id), new Guid(duesId));

                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string u = nameValueCollection["u"];

                if (u == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Success;
                    message.Message = "Successfully Updated.";
                    this.AddMessage(message);
                }

                dues.DayOfMonthToCollectDefault = due.DayOfMonthToCollectDefault;
                dues.DaysBeforeDeadlineToNotifyDefault = due.DaysBeforeDeadlineToNotifyDefault;
                dues.DuesCost = due.DuesCost;
                dues.DuesCostDisplay = due.DuesCostDisplay;
                dues.DuesFees = due.DuesFees;
                dues.DuesId = due.DuesId;
                dues.LeagueOwnerId = due.LeagueOwnerId;
                dues.LeagueOwnerName = due.LeagueOwnerName;
                dues.OwnerEntity = due.OwnerEntity;
                dues.DuesEmailDisplayText = due.DuesEmailText;
                dues.PayPalEmailAddress = due.PayPalEmailAddress;
                dues.WhoPaysProcessorFeesEnum = due.WhoPaysProcessorFeesEnum;
                dues.AcceptPaymentsOnline = due.AcceptPaymentsOnline;
                dues.LockDownManagementToManagersOnly = due.LockDownManagementToManagersOnly;
                dues.Currency = due.Currency;

                ViewData["whoPaysFeesSelectList"] = WhoPaysProcessorFeesEnum.Sender.ToSelectList();
                dues.ProcessorFeesTotal = (due.DuesCost * .029 + .50).ToString("N2");
                dues.CurrencyList = new SelectList(SiteCache.GetCurrencyExchanges(), "CurrencyAbbrName", "CurrencyNameDisplay", "USD");

                ViewBag.IsSuccessful = false;
                return View(dues);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, HasPaidSubscription = true, IsTreasurer = true)]
        public ActionResult DuesReceipt(string type, string invoiceId)
        {
            try
            {
                Guid iId = new Guid(invoiceId);
                var receipt = RDN.Library.Classes.Dues.DuesReceiptFactory.GetReceiptForDues(iId);
                return View(receipt);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsManager = true, HasPaidSubscription = true, IsTreasurer = true)]
        public ActionResult GenerateNewDuesItem(string type, string id, string duesId)
        {
            try
            {
                DuesFactory.CreateNewFeeItem(DateTime.UtcNow.AddMonths(1), new Guid(duesId));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/dues/" + type + "/" + id));
        }
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsManager = true, IsTreasurer = true, HasPaidSubscription = true)]
        public ActionResult DuesSettings(DuesModel duesModel)
        {
            DuesPortableModel dues = duesModel;
            try
            {
                dues.DuesEmailText = duesModel.DuesEmailDisplayText;
                ViewBag.IsSuccessful = DuesFactory.UpdateDuesSettings(dues);
                ViewData["whoPaysFeesSelectList"] = WhoPaysProcessorFeesEnum.Sender.ToSelectList();
                duesModel.ProcessorFeesTotal = (dues.DuesCost * .029 + .50).ToString("N2");
                return Redirect(Url.Content("~/dues/settings/" + duesModel.OwnerEntity + "/" + duesModel.LeagueOwnerId.ToString().Replace("-", "") + "/" + duesModel.DuesId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        //public ActionResult VerifyPaypalEmailAddress(string paypalEmail)
        //{
        //    return Json(new { isSuccess = isSuccessful }, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult PayDuesOnlinePayPal(DuesPortableModel duesModel)
        {
            try
            {
                if (!String.IsNullOrEmpty(Request.Form["DuesItemId"]))
                {
                    Guid memberId = RDN.Library.Classes.Account.User.GetMemberId();
                    var mem = MemberCache.GetMemberDisplay(memberId);
                    var dues2 = DuesFactory.GetDuesCollectionItem(Convert.ToInt64(Request.Form["DuesItemId"]), duesModel.DuesId, memberId);
                    if (dues2 != null)
                    {
                        PaymentGateway pg = new PaymentGateway();

                        var f = pg.StartInvoiceWizard()
                            .Initalize(ServerConfig.RDNATION_STORE_ID, dues2.Currency, PaymentProvider.Paypal, SiteSingleton.Instance.IsPayPalLive, ChargeTypeEnum.DuesItem)
                            .SetInvoiceId(Guid.NewGuid())
                            .AddDuesItem(new Library.Classes.Payment.Classes.Invoice.InvoiceDuesItem
                            {
                                BasePrice = (decimal)dues2.DuesFees.FirstOrDefault().TotalPaymentNeededFromMember,
                                WhoPaysFees = dues2.WhoPaysProcessorFeesEnum,
                                DuesId = dues2.DuesId,
                                DuesItemId = dues2.DuesFees.FirstOrDefault().DuesItemId,
                                MemberPaidId = memberId,
                                Name = "Dues For " + dues2.DuesFees.FirstOrDefault().PayBy.ToString("yyyy/MM/dd"),
                                PaidForDate = dues2.DuesFees.FirstOrDefault().PayBy,
                                Description = "Dues Payment",
                            })
                            .SetInvoiceContactData(new Library.Classes.Payment.Classes.Invoice.InvoiceContactInfo
                            {
                                Email = mem.Email,
                                FirstName = mem.Firstname,
                                LastName = mem.LastName,
                                Phone = mem.PhoneNumber,
                            })
                                                .FinalizeInvoice();
                        if (f.Status == InvoiceStatus.Paypal_Email_Not_Confirmed)
                            return Redirect(Url.Content("~/dues/member/" + duesModel.LeagueOwnerId.ToString().Replace("-", "") + "/" + memberId.ToString().Replace("-", "")) + "?u=" + SiteMessagesEnum.ppldnc.ToString());
                        else if (f.Status != InvoiceStatus.Failed)
                            return Redirect(f.RedirectLink);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Redirect(Url.Content("~/dues/league/" + duesModel.LeagueOwnerId.ToString().Replace("-", "")) + "?u=" + SiteMessagesEnum.sww.ToString());
        }
        #region reports
        /// <summary>
        /// exports one dues item.
        /// </summary>
        /// <param name="dues"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsManager = true, IsTreasurer = true, HasPaidSubscription = true)]
        public ActionResult DuesItemReport(DuesPortableModel dues)
        {
            var memId = RDN.Library.Classes.Account.User.GetMemberId();
            long duesItemId = Convert.ToInt64(HttpContext.Request.Form["DuesItemId"]);
            var duesItem = DuesFactory.GetDuesCollectionItem(duesItemId, dues.DuesId, memId);
            var Fee = duesItem.DuesFees.FirstOrDefault();
            var league = MemberCache.GetLeagueOfMember(memId);
            if (league != null)
                SetCulture(league.CultureSelected);

            using (ExcelPackage p = new ExcelPackage())
            {
                try
                {
                    p.Workbook.Properties.Author = "RDNation.com";

                    ExcelWorksheet reportSheet = p.Workbook.Worksheets.Add(Fee.PayBy.ToString("yyyy-MM-dd"));
                    reportSheet.Name = Fee.PayBy.ToString("yyyy-MM-dd"); //Setting Sheet's name
                    reportSheet.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                    reportSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
                    reportSheet.Cells[1, 1].Value = "Member";
                    reportSheet.Cells[1, 2].Value = "Due";
                    reportSheet.Cells[1, 3].Value = "Collected";
                    reportSheet.Cells[1, 4].Value = "Date Collected";

                    int rowReport = 2;
                    //create the remaining sheets with the names.
                    foreach (var attendee in duesItem.Members)
                    {
                        try
                        {
                            reportSheet.Cells[rowReport, 1].Value = attendee.DerbyName;
                            reportSheet.Cells[rowReport, 2].Value = attendee.due;
                            if (attendee.isWaived)
                                reportSheet.Cells[rowReport, 2].Value = "W";
                            reportSheet.Cells[rowReport, 3].Value = attendee.collected;
                            if (attendee.DatePaid != new DateTime() && attendee.DatePaid != null)
                                reportSheet.Cells[rowReport, 4].Value = attendee.DatePaid.ToShortDateString();
                            rowReport += 1;
                        }
                        catch (Exception exception)
                        {
                            ErrorDatabaseManager.AddException(exception, exception.GetType());
                        }
                    }
                    reportSheet.Cells[rowReport, 2].Formula = "=sum(b2:b" + (rowReport - 1) + ")";
                    reportSheet.Cells[rowReport, 3].Formula = "=sum(c2:c" + (rowReport - 1) + ")";
                    reportSheet.Cells["A1:K20"].AutoFitColumns();
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
                //Generate A File with Random name
                Byte[] bin = p.GetAsByteArray();
                string file = Fee.PayBy.ToString("yyyy-MM-dd") + RDN.Utilities.Strings.StringExt.ToExcelFriendly(" Dues Report " + duesItem.LeagueOwnerName) + "_" + Fee.PayBy.ToString("yyyymmdd") + ".xlsx";
                return File(bin, RDN.Utilities.IO.FileExt.GetMIMEType(file), file);
            }
        }
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsManager = true, IsTreasurer = true, HasPaidSubscription = true)]
        public ActionResult DuesManagementReport(DuesPortableModel dues)
        {

            var memId = RDN.Library.Classes.Account.User.GetMemberId();
            var duesItem = DuesFactory.GetDuesObject(dues.LeagueOwnerId, memId);

            var league = MemberCache.GetLeagueOfMember(memId);
            if (league != null)
                SetCulture(league.CultureSelected);

            using (ExcelPackage p = new ExcelPackage())
            {
                try
                {
                    p.Workbook.Properties.Author = "RDNation.com";

                    ExcelWorksheet duesPaidSheet = p.Workbook.Worksheets.Add(RDN.Utilities.Strings.StringExt.ToExcelFriendly("Dues Paid"));
                    ExcelWorksheet duesNotPaidSheet = p.Workbook.Worksheets.Add(RDN.Utilities.Strings.StringExt.ToExcelFriendly("Dues Not Paid"));
                    duesPaidSheet.Name = "Dues Paid"; //Setting Sheet's name
                    duesPaidSheet.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                    duesPaidSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
                    duesPaidSheet.Cells[1, 1].Value = "Member";
                    duesPaidSheet.Cells[1, 2].Value = "Last Name";

                    duesNotPaidSheet.Name = "Dues Not Paid"; //Setting Sheet's name
                    duesNotPaidSheet.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                    duesNotPaidSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
                    duesNotPaidSheet.Cells[1, 1].Value = "Member";
                    duesNotPaidSheet.Cells[1, 2].Value = "Last Name";

                    int column = 3;
                    int rowReport = 2;
                    //create the remaining sheets with the names.
                    foreach (var attendee in duesItem.Members)
                    {
                        try
                        {
                            duesPaidSheet.Cells[rowReport, 1].Value = attendee.DerbyName;
                            duesNotPaidSheet.Cells[rowReport, 1].Value = attendee.DerbyName;
                            duesPaidSheet.Cells[rowReport, 2].Value = attendee.LastName;
                            duesNotPaidSheet.Cells[rowReport, 2].Value = attendee.LastName;
                            rowReport += 1;
                        }
                        catch (Exception exception)
                        {
                            ErrorDatabaseManager.AddException(exception, exception.GetType());
                        }
                    }
                    rowReport = 2;
                    foreach (var due in duesItem.DuesFees)
                    {
                        duesPaidSheet.Cells[1, column].Value = due.PayBy.ToShortDateString();
                        duesNotPaidSheet.Cells[1, column].Value = due.PayBy.ToShortDateString();
                        foreach (var attendee in duesItem.Members)
                        {
                            if (attendee.DerbyName == "duesMem201312last")
                            { }

                            double paid = due.DuesCollected.Where(x => x.MemberPaidId == attendee.MemberId).Sum(x => x.DuesPaid);
                            var required = due.DuesRequired.Where(x => x.MemberRequiredId == attendee.MemberId).FirstOrDefault();
                            var classification = duesItem.Classifications.Where(x => x.MembersInClass.Where(y => y.MemberId == attendee.MemberId).Count() > 0).FirstOrDefault();

                            double duesDue = 0.0;
                            bool doesNotPayDues = false;
                            if (required != null)
                                duesDue = required.DuesRequire;
                            else if (classification != null)
                            {
                                doesNotPayDues = classification.DoesNotPayDues;
                                duesDue = classification.FeeRequired;
                            }
                            else
                                duesDue = due.CostOfDues;
                            double notPaid = duesDue - paid;
                            duesPaidSheet.Cells[rowReport, column].Value = paid;
                            duesNotPaidSheet.Cells[rowReport, column].Value = notPaid;
                            if (paid == 0)
                            {
                                var waived = due.DuesCollected.Where(x => x.MemberPaidId == attendee.MemberId).FirstOrDefault();
                                if (waived != null && waived.IsWaived)
                                {
                                    duesPaidSheet.Cells[rowReport, column].Value = "W";
                                    duesNotPaidSheet.Cells[rowReport, column].Value = "W";
                                }
                            }

                            rowReport += 1;
                        }
                        duesPaidSheet.Cells[rowReport, column].Formula = "=sum(" + RDN.Utilities.Strings.StringExt.GetExcelColumnName(column) + "2:" + RDN.Utilities.Strings.StringExt.GetExcelColumnName(column) + (rowReport - 1) + ")";
                        duesNotPaidSheet.Cells[rowReport, column].Formula = "=sum(" + RDN.Utilities.Strings.StringExt.GetExcelColumnName(column) + "2:" + RDN.Utilities.Strings.StringExt.GetExcelColumnName(column) + (rowReport - 1) + ")";
                        column += 1;
                        rowReport = 2;
                    }

                    duesPaidSheet.Cells["A1:K20"].AutoFitColumns();
                    duesNotPaidSheet.Cells["A1:K20"].AutoFitColumns();
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
                //Generate A File with Random name
                Byte[] bin = p.GetAsByteArray();
                string file = RDN.Utilities.Strings.StringExt.ToExcelFriendly(" Dues Report " + duesItem.LeagueOwnerName) + "_" + DateTime.UtcNow.ToString("yyyymmdd") + ".xlsx";
                return File(bin, RDN.Utilities.IO.FileExt.GetMIMEType(file), file);
            }
        }
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsManager = true, IsTreasurer = true, HasPaidSubscription = true)]
        public ActionResult DuesPersonalReport(DuesPortableModel dues)
        {

            var memId = RDN.Library.Classes.Account.User.GetMemberId();
            var duesItem = DuesFactory.GetDuesObject(dues.LeagueOwnerId, memId);


            using (ExcelPackage p = new ExcelPackage())
            {
                try
                {
                    p.Workbook.Properties.Author = "RDNation.com";

                    ExcelWorksheet duesPaidSheet = p.Workbook.Worksheets.Add(RDN.Utilities.Strings.StringExt.ToExcelFriendly("Dues Paid"));
                    ExcelWorksheet duesNotPaidSheet = p.Workbook.Worksheets.Add(RDN.Utilities.Strings.StringExt.ToExcelFriendly("Dues Not Paid"));
                    duesPaidSheet.Name = "Dues Paid"; //Setting Sheet's name
                    duesPaidSheet.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                    duesPaidSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
                    duesPaidSheet.Cells[1, 1].Value = "Member";

                    duesNotPaidSheet.Name = "Dues Not Paid"; //Setting Sheet's name
                    duesNotPaidSheet.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                    duesNotPaidSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
                    duesNotPaidSheet.Cells[1, 1].Value = "Member";

                    int column = 2;
                    int rowReport = 2;
                    //create the remaining sheets with the names.
                    foreach (var attendee in duesItem.Members.Where(x => x.MemberId == memId))
                    {
                        try
                        {
                            duesPaidSheet.Cells[rowReport, 1].Value = attendee.DerbyName;
                            duesNotPaidSheet.Cells[rowReport, 1].Value = attendee.DerbyName;
                            rowReport += 1;
                        }
                        catch (Exception exception)
                        {
                            ErrorDatabaseManager.AddException(exception, exception.GetType());
                        }
                    }
                    rowReport = 2;
                    foreach (var due in duesItem.DuesFees)
                    {
                        duesPaidSheet.Cells[1, column].Value = due.PayBy.ToShortDateString();
                        duesNotPaidSheet.Cells[1, column].Value = due.PayBy.ToShortDateString();
                        foreach (var attendee in duesItem.Members.Where(x => x.MemberId == memId))
                        {
                            if (attendee.DerbyName == "duesMem201312last")
                            { }

                            double paid = due.DuesCollected.Where(x => x.MemberPaidId == attendee.MemberId).Sum(x => x.DuesPaid);
                            var required = due.DuesRequired.Where(x => x.MemberRequiredId == attendee.MemberId).FirstOrDefault();
                            var classification = duesItem.Classifications.Where(x => x.MembersInClass.Where(y => y.MemberId == attendee.MemberId).Count() > 0).FirstOrDefault();

                            double duesDue = 0.0;
                            bool doesNotPayDues = false;
                            if (required != null)
                                duesDue = required.DuesRequire;
                            else if (classification != null)
                            {
                                doesNotPayDues = classification.DoesNotPayDues;
                                duesDue = classification.FeeRequired;
                            }
                            else
                                duesDue = due.CostOfDues;
                            double notPaid = duesDue - paid;
                            duesPaidSheet.Cells[rowReport, column].Value = paid;
                            duesNotPaidSheet.Cells[rowReport, column].Value = notPaid;
                            if (paid == 0)
                            {
                                var waived = due.DuesCollected.Where(x => x.MemberPaidId == attendee.MemberId).FirstOrDefault();
                                if (waived != null && waived.IsWaived)
                                {
                                    duesPaidSheet.Cells[rowReport, column].Value = "W";
                                    duesNotPaidSheet.Cells[rowReport, column].Value = "W";
                                }
                            }

                            rowReport += 1;
                        }
                        duesPaidSheet.Cells[rowReport, column].Formula = "=sum(" + RDN.Utilities.Strings.StringExt.GetExcelColumnName(column) + "2:" + RDN.Utilities.Strings.StringExt.GetExcelColumnName(column) + (rowReport - 1) + ")";
                        duesNotPaidSheet.Cells[rowReport, column].Formula = "=sum(" + RDN.Utilities.Strings.StringExt.GetExcelColumnName(column) + "2:" + RDN.Utilities.Strings.StringExt.GetExcelColumnName(column) + (rowReport - 1) + ")";
                        column += 1;
                        rowReport = 2;
                    }

                    duesPaidSheet.Cells["A1:K20"].AutoFitColumns();
                    duesNotPaidSheet.Cells["A1:K20"].AutoFitColumns();
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
                //Generate A File with Random name
                Byte[] bin = p.GetAsByteArray();
                string file = RDN.Utilities.Strings.StringExt.ToExcelFriendly(" Dues Report " + duesItem.LeagueOwnerName) + "_" + DateTime.UtcNow.ToString("yyyymmdd") + ".xlsx";
                return File(bin, RDN.Utilities.IO.FileExt.GetMIMEType(file), file);
            }
        }

        #endregion
    }
}
