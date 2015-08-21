using RDN.Library.Cache;
using RDN.Library.Cache.Singletons;
using RDN.Library.Classes.Config;
using RDN.Library.Classes.Dues;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Enums;
using RDN.Portable.Classes.Controls.Dues;
using RDN.Portable.Classes.Payment.Classes;
using RDN.Portable.Classes.Payment.Enums;
using RDN.Portable.Config;
using RDN.Portable.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RDN.Api.Controllers.League
{
    public class DuesController : Controller
    {

        public ActionResult DuesManagementByMember()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        DuesPortableModel dues = new DuesPortableModel();
                        dues.LeagueOwnerId = mem.CurrentLeagueId;
                        var dues2 = DuesFactory.GetDuesObject(mem.CurrentLeagueId, ob.CurrentMemberId);
                        if (dues2.DuesId != new Guid())
                            dues = dues2;
                        dues.IsSuccessful = true;
                        return Json(dues, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, GetType());

            }
            return Json(new DuesPortableModel() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoveDuesPayment()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        ob.IsSuccessful = DuesFactory.RemoveDuesCollectionItem(ob.DuesItemId, ob.DuesId, ob.MemberId, ob.DuesCollectedId);

                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new DuesSendParams() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SendEmailReminderWithstanding()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        ob.IsSuccessful = DuesFactory.SendEmailReminderWithStanding(ob.DuesId, mem.MemberId, mem.CurrentLeagueId);

                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new DuesSendParams() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SendEmailReminder()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        ob.IsSuccessful = DuesFactory.SendEmailReminder(ob.DuesItemId, ob.DuesId, mem.MemberId, mem.CurrentLeagueId);

                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new DuesSendParams() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SendEmailReminderAll()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        ob.IsSuccessful = DuesFactory.SendEmailReminderAll(ob.DuesItemId, ob.DuesId);

                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new DuesSendParams() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PayDuesAmount()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        ob.IsSuccessful = DuesFactory.PayDuesAmount(ob.DuesItemId, ob.DuesId, ob.Amount, ob.MemberId, ob.Note);

                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new DuesSendParams() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SetDuesAmount()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        ob.IsSuccessful = DuesFactory.SetDuesAmount(ob.DuesItemId, ob.DuesId, ob.Amount, ob.MemberId);

                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new DuesSendParams() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult WaiveDuesAmount()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        ob.IsSuccessful = DuesFactory.WaiveDuesAmount(ob.DuesItemId, ob.DuesId, ob.MemberId, ob.Note);
                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new DuesSendParams() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoveDuesWaived()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        ob.IsSuccessful = DuesFactory.RemoveDuesWaived(ob.DuesItemId, ob.DuesId, ob.MemberId);
                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new DuesSendParams() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DuesItem()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        var dues = DuesFactory.GetDuesCollectionItem(ob.DuesItemId, ob.DuesId, ob.CurrentMemberId);
                        return Json(dues, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new DuesPortableModel() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditDuesItem()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        ob.IsSuccessful = DuesFactory.UpdateDuesCollectionItem(ob.DuesItemId, ob.DuesId, ob.PayBy, ob.Amount);
                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new DuesPortableModel() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteDuesItem()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        ob.IsSuccessful = DuesFactory.DeleteDuesCollectionItem(ob.DuesItemId, ob.DuesId);
                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new DuesPortableModel() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DuesManagement()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        DuesPortableModel dues = new DuesPortableModel();
                        dues.LeagueOwnerId = mem.CurrentLeagueId;
                        var dues2 = DuesFactory.GetDuesObject(mem.CurrentLeagueId, ob.CurrentMemberId);
                        if (dues2.DuesId != new Guid())
                            dues = dues2;
                        dues.IsSuccessful = true;
                        return Json(dues, JsonRequestBehavior.AllowGet);
                        //if (MemberCache.IsTreasurerOrBetterOfLeague(memId) && !dues.AcceptPaymentsOnline)
                        //{
                        //    SiteMessage message = new SiteMessage();
                        //    message.MessageType = SiteMessageType.Info;
                        //    message.Message = "Did You know you can accept Dues Payments online?  Go to the settings to get started.";
                        //    this.AddMessage(message);
                        //}
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }

            return Json(new DuesPortableModel() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DuesForMember()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        var dues = DuesFactory.GetDuesObject(mem.CurrentLeagueId, ob.MemberId);
                        dues.CurrentMemberDerbyName = mem.DerbyName;
                        dues.CurrentMemberId = mem.MemberId;
                        return Json(dues, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new DuesPortableModel() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditMemberDuesItem()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        var duesMember = DuesFactory.GetDuesCollectionItemForMember(ob.DuesItemId, ob.DuesId, ob.MemberId);
                        return Json(duesMember, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new DuesMemberItem() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DuesSettings()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        DuesPortableModel dues = DuesFactory.GetDuesSettings(mem.CurrentLeagueId, ob.DuesId);
                        dues.Currencies = SiteCache.GetCurrencyExchanges();
                        dues.ProcessorFeesTotal = (dues.DuesCost * .029 + .50).ToString("N2");
                        return Json(dues, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new DuesPortableModel() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult DuesReceipt()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        var receipt = DuesReceiptFactory.GetReceiptForDues(ob.DuesId);
                        return Json(receipt, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new DuesReceipt() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GenerateNewDuesItem()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        DuesFactory.CreateNewFeeItem(DateTime.UtcNow.AddMonths(1), ob.DuesId);
                        ob.IsSuccessful = true;
                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new DuesSendParams() { IsSuccessful = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveDuesSettings()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesPortableModel>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.CurrentUserId == mem.UserId)
                    {
                        ob.IsSuccessful = DuesFactory.UpdateDuesSettings(ob);
                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new DuesPortableModel() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PayDuesOnlinePayPal()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<DuesSendParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.CurrentMemberId);
                    if (ob.UserId == mem.UserId)
                    {
                        var dues2 = DuesFactory.GetDuesCollectionItem(ob.DuesItemId, ob.DuesId, ob.CurrentMemberId);
                        if (dues2 != null)
                        {
                            
                            PaymentGateway pg = new PaymentGateway();

                            var f = pg.StartInvoiceWizard()
                                .Initalize(LibraryConfig.SiteStoreID, dues2.Currency, PaymentProvider.Paypal, LibraryConfig.IsProduction, ChargeTypeEnum.DuesItem)
                                .SetInvoiceId(Guid.NewGuid())
                                .AddDuesItem(new Library.Classes.Payment.Classes.Invoice.InvoiceDuesItem
                                {
                                    BasePrice = (decimal)dues2.DuesFees.FirstOrDefault().TotalPaymentNeededFromMember,
                                    WhoPaysFees = dues2.WhoPaysProcessorFeesEnum,
                                    DuesId = dues2.DuesId,
                                    DuesItemId = dues2.DuesFees.FirstOrDefault().DuesItemId,
                                    MemberPaidId = ob.CurrentMemberId,
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

                            //if (f.Status == InvoiceStatus.Paypal_Email_Not_Confirmed)
                            //    return Redirect(Url.Content("~/dues/member/" + duesModel.LeagueOwnerId.ToString().Replace("-", "") + "/" + memberId.ToString().Replace("-", "")) + "?u=" + SiteMessagesEnum.ppldnc.ToString());
                            //else if (f.Status != InvoiceStatus.Failed)
                            //    return Redirect(f.RedirectLink);
                            return Json(f, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new CreateInvoiceReturn() { Status = InvoiceStatus.Failed });
        }



    }
}