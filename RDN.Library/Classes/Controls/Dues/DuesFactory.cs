using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.MemberFees;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Account.Classes;
using RDN.Utilities.Config;
using RDN.Library.Classes.Dues.Enums;
using RDN.Library.Cache;
using RDN.Portable.Config;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Controls.Dues.Enums;
using RDN.Portable.Classes.Controls.Dues;
using RDN.Portable.Classes.Controls.Dues.Classify;
using RDN.Library.Classes.Config;
using RDN.Portable.Classes.Url;

namespace RDN.Library.Classes.Dues
{
    public class DuesFactory
    {


        public DuesFactory()
        {
        }

        public static void PullClassifications(Guid duesManagementId)
        {
            try
            {
                var dc = new ManagementContext();
                var clas = (from xx in dc.FeeManagement
                            where xx.FeeManagementId == duesManagementId
                            select new
                            {
                                xx.FeeClassifications,
                                xx.LeagueOwner.Members
                            }).FirstOrDefault();

                for (int i = 0; i < clas.Members.Count; i++)
                {

                }


            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        public static bool RemoveDuesCollectionItem(long duesItemId, Guid duesManagementId, Guid memId, long duesCollectedId)
        {
            try
            {
                var dc = new ManagementContext();
                var item = (from xx in dc.FeesCollected
                            where xx.MemberPaid.MemberId == memId
                            where xx.FeeItem.FeeCollectionId == duesItemId
                            where xx.FeeItem.FeeManagedBy.FeeManagementId == duesManagementId
                            select xx);

                foreach (var fee in item)
                {
                    fee.IsPaidInFull = false;
                }
                var feed = item.Where(x => x.FeeCollectionId == duesCollectedId).FirstOrDefault();
                if (feed != null)
                    dc.FeesCollected.Remove(feed);

                int c = dc.SaveChanges();
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static DuesMemberItem GetDuesCollectionItemForMember(long duesItemId, Guid duesManagementId, Guid memberId)
        {
            DuesMemberItem memberPaidDues = new DuesMemberItem();
            try
            {
                var dc = new ManagementContext();
                var dues = (from xx in dc.FeeItem
                            where xx.FeeManagedBy.FeeManagementId == duesManagementId
                            where xx.FeeCollectionId == duesItemId
                            select new
                            {
                                xx.FeeManagedBy.DayOfMonthToCollectDefault,
                                xx.FeeManagedBy.DaysBeforeDeadlineToNotifyDefault,
                                xx.FeeManagedBy.FeeCostDefault,
                                xx.FeeManagedBy.FeeManagementId,
                                xx.CostOfFee,
                                xx.DaysBeforeDeadlineToNotify,
                                xx.FeeCollectionId,
                                FeesCollected = xx.FeesCollected.Where(x => x.MemberPaid.MemberId == memberId),
                                FeesRequired = xx.FeesRequired.Where(x => x.MemberRequiredFrom.MemberId == memberId),
                                xx.Notified,
                                xx.PayBy,
                                xx.FeeManagedBy.LeagueOwner
                            }).FirstOrDefault();

                if (dues != null)
                {
                    memberPaidDues.OwnerEntity = DuesOwnerEntityEnum.league.ToString();
                    memberPaidDues.DuesCostDisplay = dues.FeeCostDefault.ToString("N2");
                    memberPaidDues.DuesCost = dues.FeeCostDefault;
                    memberPaidDues.DuesId = duesManagementId;
                    memberPaidDues.OwnerId = dues.LeagueOwner.LeagueId;
                    memberPaidDues.DuesItem = new DuesItem();
                    if (dues.FeesRequired.FirstOrDefault() != null)
                        memberPaidDues.DuesItem.CostOfDues = dues.FeesRequired.FirstOrDefault().FeeRequired;
                    else
                        memberPaidDues.DuesItem.CostOfDues = dues.CostOfFee;
                    memberPaidDues.DuesItem.DuesItemId = dues.FeeCollectionId;
                    memberPaidDues.DuesItem.PayBy = dues.PayBy;
                    memberPaidDues.DuesItem.TotalPaid = 0.00;
                    memberPaidDues.DuesItem.TotalWithstanding = 0.00;
                    foreach (var fee in dues.FeesCollected)
                    {
                        DuesCollected col = new DuesCollected();
                        col.PaidDate = fee.Created;
                        col.DuesPaid = fee.FeeCollected;
                        memberPaidDues.DuesItem.TotalPaid += fee.FeeCollected;
                        col.DuesCollectedId = fee.FeeCollectionId;
                        col.IsPaidInFull = fee.IsPaidInFull;
                        col.MemberPaidId = fee.MemberPaid.MemberId;
                        col.MemberPaidName = fee.MemberPaid.DerbyName;
                        col.Note = fee.Note;
                        col.IsWaived = fee.IsFeeWaived;
                        memberPaidDues.DuesItem.DuesCollected.Add(col);
                    }
                    var member = dues.LeagueOwner.Members.Where(x => x.Member.MemberId == memberId).FirstOrDefault();
                    if (member != null)
                    {
                        //adds every member to the member list
                        memberPaidDues.Member = new MemberDisplayBasic();
                        memberPaidDues.Member.MemberId = member.Member.MemberId;
                        memberPaidDues.Member.DerbyName = member.Member.DerbyName;

                        if (member.Member.ContactCard != null && member.Member.ContactCard.Emails.FirstOrDefault() != null)
                            memberPaidDues.Member.UserId = Guid.NewGuid();
                        else
                            memberPaidDues.Member.UserId = member.Member.AspNetUserId;

                        //gets member in collection list
                        var collected = memberPaidDues.DuesItem.DuesCollected.Where(x => x.MemberPaidId == member.Member.MemberId);
                        if (collected.Count() == 0)
                            memberPaidDues.DuesItem.TotalWithstanding -= memberPaidDues.DuesItem.CostOfDues;
                        else
                        {
                            //gets the total dues paid by the member for this month.  Can pay in increments so thats
                            // why we check for all collected items.
                            double totalPaid = 0.00;
                            foreach (var c in collected)
                            {
                                totalPaid += c.DuesPaid;
                            }
                            //if the total dues paid isn't equal to the actual cost of the month,
                            // we subtract it from the total withstanding dues.
                            if (totalPaid != memberPaidDues.DuesItem.CostOfDues)
                                memberPaidDues.DuesItem.TotalWithstanding -= (memberPaidDues.DuesItem.CostOfDues - totalPaid);
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return memberPaidDues;
        }


        /// <summary>
        /// sets the notified flag to true that members were notified of collection.
        /// </summary>
        /// <param name="due"></param>
        public static void NotifiedMembersOfDuesItem(FeeItem due)
        {
            var dc = new ManagementContext();
            var dueUpdate = dc.FeeItem.Where(x => x.FeeCollectionId == due.FeeCollectionId).FirstOrDefault();
            dueUpdate.Notified = true;
            dc.SaveChanges();
        }
        /// <summary>
        /// creates a new fee item from the automatedtasks class.
        /// </summary>
        /// <param name="monthAhead"></param>
        /// <param name="fee"></param>
        public static void CreateNewFeeItem(DateTime monthAhead, FeeManagement fee)
        {
            var dc = new ManagementContext();
            var mana = dc.FeeManagement.Where(x => x.FeeManagementId == fee.FeeManagementId).FirstOrDefault();
            FeeItem fItem = new FeeItem();
            fItem.CostOfFee = fee.FeeCostDefault;
            fItem.FeeManagedBy = mana;
            fItem.PayBy = monthAhead;
            fItem.Notified = false;
            fItem.DaysBeforeDeadlineToNotify = fee.DaysBeforeDeadlineToNotifyDefault;
            mana.Fees.Add(fItem);
            dc.SaveChanges();
        }
        public static void CreateNewFeeItem(DateTime monthAhead, Guid managementId)
        {
            var dc = new ManagementContext();
            var mana = dc.FeeManagement.Where(x => x.FeeManagementId == managementId).FirstOrDefault();
            FeeItem fItem = new FeeItem();
            fItem.CostOfFee = mana.FeeCostDefault;
            fItem.FeeManagedBy = mana;
            fItem.PayBy = monthAhead;
            fItem.Notified = false;
            fItem.DaysBeforeDeadlineToNotify = mana.DaysBeforeDeadlineToNotifyDefault;
            mana.Fees.Add(fItem);
            dc.SaveChanges();
        }
        /// <summary>
        /// sends an email reminder to everyone who hasn't paid dues.
        /// </summary>
        /// <param name="duesItem"></param>
        /// <param name="duesManagementItem"></param>
        /// <returns></returns>
        public static bool SendEmailReminderAll(long duesItem, Guid duesManagementItem)
        {
            try
            {
                var dc = new ManagementContext();
                var dues = GetDuesCollectionItem(duesItem, duesManagementItem, RDN.Library.Classes.Account.User.GetMemberId());
                foreach (var member in dues.Members)
                {
                    try
                    {

                        if (member.due > 0.00 && !member.isWaived && !member.DoesNotPayDues)
                        {
                            string email = string.Empty;
                            var mem = dc.Members.Where(x => x.MemberId == member.MemberId).FirstOrDefault();
                            if (mem.AspNetUserId != new Guid())
                                email = System.Web.Security.Membership.GetUser((object)mem.AspNetUserId).UserName;
                            else
                            {
                                if (mem.ContactCard != null)
                                    if (mem.ContactCard.Emails.Where(x => x.IsDefault).FirstOrDefault() != null)
                                        email = mem.ContactCard.Emails.Where(x => x.IsDefault).FirstOrDefault().EmailAddress;
                            }
                            if (!String.IsNullOrEmpty(email))
                            {

                                var emailData = new Dictionary<string, string>
                                        {
                                            { "name", member.DerbyName}, 
                                            { "leaguename", dues.LeagueOwnerName}, 
                                            { "duedate", dues.DuesFees.FirstOrDefault().PayBy.ToShortDateString() },
                                            { "paymentneeded",member.due.ToString("N2")},
                                            { "paymentOnlineText",  GeneratePaymentOnlineText(dues.AcceptPaymentsOnline,dues.LeagueOwnerId) }
                                        };

                                if (!String.IsNullOrEmpty(dues.EmailResponse))
                                {
                                    emailData.Add("blanktext", dues.EmailResponse);
                                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, email, LibraryConfig.DefaultEmailSubject + " Dues Payment Requested", emailData, EmailServer.EmailServerLayoutsEnum.DuesCollectingNotificationBlank);
                                }
                                else
                                {
                                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, email, LibraryConfig.DefaultEmailSubject + " Dues Payment Requested", emailData, EmailServer.EmailServerLayoutsEnum.DuesCollectingNotification);
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool SendEmailReminder(long duesItem, Guid duesManagementItem, Guid memberId, Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var item = GetDuesCollectionItem(duesItem, duesManagementItem, RDN.Library.Classes.Account.User.GetMemberId());
                var mem = item.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                if (mem.due > 0.00 && !mem.DoesNotPayDues)
                {
                    var member = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                    string email = string.Empty;
                    if (member.AspNetUserId != new Guid())
                        email = System.Web.Security.Membership.GetUser((object)member.AspNetUserId).UserName;
                    else
                    {
                        if (member.ContactCard != null)
                            if (member.ContactCard.Emails.Where(x => x.IsDefault).FirstOrDefault() != null)
                                email = member.ContactCard.Emails.Where(x => x.IsDefault).FirstOrDefault().EmailAddress;
                    }
                    if (!String.IsNullOrEmpty(email))
                    {
                        var league = member.Leagues.Where(x => x.League.LeagueId == leagueId).FirstOrDefault();

                        var emailData = new Dictionary<string, string>
                                        {
                                            { "name", member.DerbyName }, 
                                            { "leaguename", league.League.Name}, 
                                            { "duedate", item.DuesFees.FirstOrDefault().PayBy.ToShortDateString() },
                                            { "paymentneeded",mem.due.ToString("N2")},
                                            { "paymentOnlineText",  GeneratePaymentOnlineText(item.AcceptPaymentsOnline, league.League.LeagueId) }
                                        };

                        if (!String.IsNullOrEmpty(item.EmailResponse))
                        {
                            emailData.Add("blanktext", item.EmailResponse);
                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, email, LibraryConfig.DefaultEmailSubject + " Dues Payment Requested", emailData, EmailServer.EmailServerLayoutsEnum.DuesCollectingNotificationBlank);
                        }
                        else
                        {
                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, email, LibraryConfig.DefaultEmailSubject + " Dues Payment Requested", emailData, EmailServer.EmailServerLayoutsEnum.DuesCollectingNotification);
                        }

                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static string GeneratePaymentOnlineText(bool acceptPaymetsOnline, Guid ownerId)
        {
            string paymentOnlineText = @RDN.Library.Classes.Config.LibraryConfig.WebsiteShortName + " allows members to pay their dues online to their league.  This feature isn't currently enabled for your league, but your managers can enable this feature through the <b>dues management portal settings</b>.";
            if (acceptPaymetsOnline)
                paymentOnlineText = "You can <a href='" + LibraryConfig.InternalSite + UrlManager.LEAGUE_DUES_MANAGEMENT_URL + ownerId.ToString().Replace("-", "") + "'>pay your dues online <b>NOW</b> at " + LibraryConfig.WebsiteShortName + "!</a>";
            return paymentOnlineText;
        }

        public static bool SendEmailReminderWithStanding(Guid duesManagementItem, Guid memberId, Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var dues = GetDuesObject(leagueId, memberId);

                var mem = dues.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                if (mem != null)
                {

                    var member = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                    string email = string.Empty;
                    if (member.AspNetUserId != new Guid())
                        email = System.Web.Security.Membership.GetUser((object)member.AspNetUserId).UserName;
                    else
                    {
                        if (member.ContactCard != null)
                            if (member.ContactCard.Emails.Where(x => x.IsDefault).FirstOrDefault() != null)
                                email = member.ContactCard.Emails.Where(x => x.IsDefault).FirstOrDefault().EmailAddress;
                    }
                    if (!String.IsNullOrEmpty(email))
                    {
                        var league = member.Leagues.Where(x => x.League.LeagueId == leagueId).FirstOrDefault();

                        var emailData = new Dictionary<string, string>
                                        {
                                            { "name", member.DerbyName }, 
                                            { "leaguename", league.League.Name}, 
                                            { "duedate", "This is Total Withstanding Dues You Owe the League" },
                                            { "paymentneeded",mem.TotalWithstanding.ToString("N2")},
                                            { "paymentOnlineText",  GeneratePaymentOnlineText(dues.AcceptPaymentsOnline, league.League.LeagueId) }
                                        };

                        if (!String.IsNullOrEmpty(dues.DuesEmailText))
                        {
                            emailData.Add("blanktext", dues.DuesEmailText);
                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, email, LibraryConfig.DefaultEmailSubject + " Dues Payments Withstanding", emailData, EmailServer.EmailServerLayoutsEnum.DuesCollectingNotificationBlank);
                        }
                        else
                        {
                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, email, LibraryConfig.DefaultEmailSubject + " Dues Payments Withstanding", emailData, EmailServer.EmailServerLayoutsEnum.DuesCollectingNotification);
                        }
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool SetDuesAmount(long duesItemId, Guid duesManagementId, double amountDue, Guid memberIdPaid)
        {
            try
            {
                var dc = new ManagementContext();
                var dues = (from xx in dc.FeeItem
                            where xx.FeeManagedBy.FeeManagementId == duesManagementId
                            where xx.FeeCollectionId == duesItemId
                            select xx).FirstOrDefault();
                if (dues != null)
                {
                    var re = dues.FeesRequired.Where(x => x.MemberRequiredFrom.MemberId == memberIdPaid).FirstOrDefault();
                    if (re == null)
                    {
                        FeesRequired collected = new FeesRequired();
                        collected.FeeRequired = amountDue;
                        collected.FeeItem = dues;
                        collected.MemberRequiredFrom = dc.Members.Where(x => x.MemberId == memberIdPaid).FirstOrDefault();
                        //collected.Note = note;
                        dues.FeesRequired.Add(collected);
                        dc.SaveChanges();
                    }
                    else
                    {
                        re.FeeRequired = amountDue;
                    }
                    //need to set the is paid in full marker on the fees collected if paid more than needed.
                    var col = dues.FeesCollected.Where(x => x.MemberPaid.MemberId == memberIdPaid);
                    double feeCollected = col.Sum(x => x.FeeCollected);
                    if (feeCollected >= amountDue)
                    {
                        foreach (var collect in col)
                        {
                            collect.IsPaidInFull = true;
                        }
                    }
                    else if (feeCollected <= amountDue)
                    {
                        foreach (var collect in col)
                        {
                            collect.IsPaidInFull = false;
                        }
                    }
                    int c = dc.SaveChanges();
                    return c > 0;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool PayDuesAmount(long duesItemId, Guid duesManagementId, double amountPaid, Guid memberIdPaid, string note)
        {
            try
            {
                var dc = new ManagementContext();
                var dues = (from xx in dc.FeeItem.Include("FeeManagedBy").Include("FeeManagedBy.FeeClassifications").Include("FeeManagedBy.FeeClassifications").Include("FeeManagedBy.FeeClassifications.MembersClassified").Include("FeeManagedBy.FeeClassifications.MembersClassified.Member")
                            where xx.FeeManagedBy.FeeManagementId == duesManagementId
                            where xx.FeeCollectionId == duesItemId
                            select xx).FirstOrDefault();
                if (dues != null)
                {
                    double due = 0.0;
                    bool DoesNotPayDues = false;

                    FeesCollected collected = new FeesCollected();
                    collected.FeeCollected = amountPaid;
                    collected.FeeItem = dues;
                    collected.MemberPaid = dc.Members.Where(x => x.MemberId == memberIdPaid).FirstOrDefault();
                    collected.Note = note;
                    double totalPaid = amountPaid;

                    var fees = dues.FeesCollected.Where(x => x.MemberPaid.MemberId == memberIdPaid);
                    var required = dues.FeesRequired.Where(x => x.MemberRequiredFrom.MemberId == memberIdPaid).FirstOrDefault();
                    var classification = dues.FeeManagedBy.FeeClassifications.Where(x => x.MembersClassified.Where(y => y.Member.MemberId == memberIdPaid).Count() > 0).FirstOrDefault();

                    if (required != null)
                        due = required.FeeRequired;
                    else if (classification != null)
                    {
                        DoesNotPayDues = classification.DoesNotPayDues;
                        due = classification.FeeRequired;
                    }
                    else
                        due = dues.CostOfFee;

                    foreach (var fee in fees)
                    {
                        totalPaid += fee.FeeCollected;
                    }
                    if (totalPaid == due || totalPaid > due)
                    {
                        collected.IsPaidInFull = true;
                        foreach (var fee in fees)
                        {
                            fee.IsPaidInFull = true;
                        }
                    }
                    dues.FeesCollected.Add(collected);
                    int c = dc.SaveChanges();
                    return c > 0;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool WaiveDuesAmount(long duesItemId, Guid duesManagementId, Guid memberIdPaid, string note)
        {
            try
            {
                var dc = new ManagementContext();
                var dues = (from xx in dc.FeeItem
                            where xx.FeeManagedBy.FeeManagementId == duesManagementId
                            where xx.FeeCollectionId == duesItemId
                            select xx).FirstOrDefault();
                if (dues != null)
                {
                    FeesCollected collected = new FeesCollected();
                    collected.FeeCollected = 0.00;
                    collected.IsFeeWaived = true;
                    collected.FeeItem = dues;
                    collected.MemberPaid = dc.Members.Where(x => x.MemberId == memberIdPaid).FirstOrDefault();
                    collected.Note = note;
                    collected.IsPaidInFull = true;
                    dues.FeesCollected.Add(collected);
                    int c = dc.SaveChanges();
                    return c > 0;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool RemoveDuesWaived(long duesItemId, Guid duesManagementId, Guid memberIdPaid)
        {
            try
            {
                var dc = new ManagementContext();
                var dues = (from xx in dc.FeeItem
                            where xx.FeeManagedBy.FeeManagementId == duesManagementId
                            where xx.FeeCollectionId == duesItemId
                            select xx).FirstOrDefault();
                if (dues != null)
                {
                    var feeCollected = dues.FeesCollected.Where(x => x.MemberPaid.MemberId == memberIdPaid).ToList();
                    foreach (var col in feeCollected)
                    {
                        col.IsFeeWaived = false;
                        col.IsPaidInFull = false;
                        col.WasClearedByUser = true;
                        //if (col.FeeCollected == 0.0)
                        //dues.FeesCollected.Remove(col);
                    }
                    if (feeCollected.Count == 0)
                    {
                        FeesCollected col = new FeesCollected();
                        col.MemberPaid = dc.Members.Where(x => x.MemberId == memberIdPaid).FirstOrDefault();
                        col.FeeCollected = 0.0;
                        col.FeeItem = dues;
                        col.WasClearedByUser = true;
                        dues.FeesCollected.Add(col);
                    }
                    int c = dc.SaveChanges();
                    return c > 0;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        /// <summary>
        /// creates a brand new dues object for a league.
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="ownerType"></param>
        public static void CreateDuesObject(Guid ownerId, DuesOwnerEntityEnum ownerType)
        {
            try
            {
                var dc = new ManagementContext();
                FeeManagement fee = new FeeManagement();
                fee.DayOfMonthToCollectDefault = 15;
                fee.DaysBeforeDeadlineToNotifyDefault = 10;
                fee.FeeCostDefault = 45.00;
                fee.LeagueOwner = dc.Leagues.Where(x => x.LeagueId == ownerId).FirstOrDefault();
                fee.EmailResponse = String.Empty;
                fee.AcceptPaymentsOnline = false;
                fee.WhoPaysProcessorFeesEnum = Convert.ToInt32(WhoPaysProcessorFeesEnum.Sender);
                fee.FeeTypeEnum = Convert.ToInt32(Enums.FeesTypeEnum.DuesType);

                FeeItem item = new FeeItem();
                item.CostOfFee = 45.00;
                item.DaysBeforeDeadlineToNotify = 10;
                item.FeeManagedBy = fee;
                item.Notified = false;
                item.PayBy = DateTime.UtcNow.AddDays(15);
                fee.Fees.Add(item);
                dc.FeeManagement.Add(fee);
                dc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        public static DuesPortableModel GetDuesSettings(Guid leagueId, Guid duesId)
        {
            DuesPortableModel due = new DuesPortableModel();
            try
            {
                var dc = new ManagementContext();
                var dues = (from xx in dc.FeeManagement.Include("LeagueOwner")
                            where xx.LeagueOwner.LeagueId == leagueId
                            where xx.FeeManagementId == duesId
                            select new
                            {
                                xx.DayOfMonthToCollectDefault,
                                xx.DaysBeforeDeadlineToNotifyDefault,
                                xx.FeeCostDefault,
                                xx.FeeManagementId,
                                xx.LeagueOwner,
                                xx.WhoPaysProcessorFeesEnum,
                                xx.PayPalEmailAddress,
                                xx.LockDownManagementToManagersOnly,
                                xx.AcceptPaymentsOnline,
                                xx.EmailResponse,
                                xx.CurrencyRate
                            }).FirstOrDefault();


                if (dues != null)
                {
                    if (dues.CurrencyRate != null)
                        due.Currency = dues.CurrencyRate.CurrencyAbbrName;
                    else
                        due.Currency = "USD";
                    due.DuesEmailText = dues.EmailResponse;
                    due.DayOfMonthToCollectDefault = dues.DayOfMonthToCollectDefault;
                    due.DaysBeforeDeadlineToNotifyDefault = dues.DaysBeforeDeadlineToNotifyDefault;
                    due.DuesCost = dues.FeeCostDefault;
                    due.DuesCostDisplay = dues.FeeCostDefault.ToString("N2");
                    due.DuesId = dues.FeeManagementId;
                    due.LeagueOwnerId = dues.LeagueOwner.LeagueId;
                    due.LeagueOwnerName = dues.LeagueOwner.Name;
                    due.OwnerEntity = Enums.DuesOwnerEntityEnum.league.ToString();
                    due.AcceptPaymentsOnline = dues.AcceptPaymentsOnline;
                    due.PayPalEmailAddress = dues.PayPalEmailAddress;
                    due.LockDownManagementToManagersOnly = dues.LockDownManagementToManagersOnly;
                    due.WhoPaysProcessorFeesEnum = (WhoPaysProcessorFeesEnum)Enum.Parse(typeof(WhoPaysProcessorFeesEnum), dues.WhoPaysProcessorFeesEnum.ToString());
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return due;
        }
        public static DuesPortableModel GetDuesSettings(Guid duesId)
        {
            try
            {
                var dc = new ManagementContext();
                var dues = (from xx in dc.FeeManagement.Include("LeagueOwner")
                            where xx.FeeManagementId == duesId
                            select xx).FirstOrDefault();

                if (dues != null)
                {
                    return DisplayDuesSettings(dues);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        /// <summary>
        /// we disable the leagues ability to collect dues via paypal.
        /// </summary>
        /// <param name="duesId"></param>
        /// <returns></returns>
        public static bool DisablePaypalDuesAccountForLeague(Guid duesId)
        {
            try
            {
                var dc = new ManagementContext();
                var dues = (from xx in dc.FeeManagement.Include("LeagueOwner")
                            where xx.FeeManagementId == duesId
                            select xx).FirstOrDefault();
                dues.AcceptPaymentsOnline = false;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static DuesPortableModel GetDuesSettingsByLeague(Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var dues = (from xx in dc.FeeManagement.Include("LeagueOwner")
                            where xx.LeagueOwner.LeagueId == leagueId
                            select xx).FirstOrDefault();

                if (dues != null)
                {
                    return DisplayDuesSettings(dues);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        private static DuesPortableModel DisplayDuesSettings(FeeManagement dues)
        {
            try
            {
                DuesPortableModel due = new DuesPortableModel();
                due.DayOfMonthToCollectDefault = dues.DayOfMonthToCollectDefault;
                due.DaysBeforeDeadlineToNotifyDefault = dues.DaysBeforeDeadlineToNotifyDefault;
                due.DuesCost = dues.FeeCostDefault;
                due.DuesCostDisplay = dues.FeeCostDefault.ToString("N2");
                due.DuesId = dues.FeeManagementId;
                due.LeagueOwnerId = dues.LeagueOwner.LeagueId;
                due.LeagueOwnerName = dues.LeagueOwner.Name;
                if (dues.LeagueOwner.ContactCard != null && dues.LeagueOwner.ContactCard.Emails.FirstOrDefault() != null)
                    due.LeagueEmailAddress = dues.LeagueOwner.ContactCard.Emails.Where(x => x.IsDefault == true).FirstOrDefault().EmailAddress;
                due.OwnerEntity = Enums.DuesOwnerEntityEnum.league.ToString();
                due.AcceptPaymentsOnline = dues.AcceptPaymentsOnline;
                due.PayPalEmailAddress = dues.PayPalEmailAddress;
                due.WhoPaysProcessorFeesEnum = (WhoPaysProcessorFeesEnum)Enum.Parse(typeof(WhoPaysProcessorFeesEnum), dues.WhoPaysProcessorFeesEnum.ToString());
                due.LockDownManagementToManagersOnly = dues.LockDownManagementToManagersOnly;
                return due;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }


        public static bool UpdateDuesSettings(DuesPortableModel duesObject)
        {
            try
            {
                var dc = new ManagementContext();
                var dues = (from xx in dc.FeeManagement.Include("LeagueOwner")
                            where xx.LeagueOwner.LeagueId == duesObject.LeagueOwnerId
                            select xx).FirstOrDefault();
                dues.CurrencyRate = dc.ExchangeRates.Where(x => x.CurrencyAbbrName == duesObject.Currency).FirstOrDefault();
                dues.DayOfMonthToCollectDefault = duesObject.DayOfMonthToCollectDefault;
                dues.DaysBeforeDeadlineToNotifyDefault = duesObject.DaysBeforeDeadlineToNotifyDefault;
                dues.FeeCostDefault = Convert.ToDouble(duesObject.DuesCostDisplay);
                dues.EmailResponse = duesObject.DuesEmailText;
                dues.AcceptPaymentsOnline = duesObject.AcceptPaymentsOnline;
                if (!String.IsNullOrEmpty(duesObject.PayPalEmailAddress))
                    duesObject.PayPalEmailAddress = duesObject.PayPalEmailAddress.Trim();
                dues.PayPalEmailAddress = duesObject.PayPalEmailAddress;
                dues.WhoPaysProcessorFeesEnum = Convert.ToInt32(duesObject.WhoPaysProcessorFeesEnum);
                dues.LockDownManagementToManagersOnly = duesObject.LockDownManagementToManagersOnly;
                dc.SaveChanges();
                MemberCache.ClearLeagueMembersCache(duesObject.LeagueOwnerId);
                MemberCache.ClearLeagueMembersApiCache(duesObject.LeagueOwnerId);
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool UpdateDuesCollectionItem(long duesItemId, Guid duesManagementId, DateTime payBy, double duesCost)
        {
            try
            {
                var dc = new ManagementContext();
                var dues = (from xx in dc.FeeItem
                            where xx.FeeManagedBy.FeeManagementId == duesManagementId
                            where xx.FeeCollectionId == duesItemId
                            select xx).FirstOrDefault();

                if (dues != null)
                {
                    dues.PayBy = payBy;
                    dues.CostOfFee = duesCost;
                    int c = dc.SaveChanges();
                    return c > 0;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }


        public static bool DeleteDuesCollectionItem(long duesItemId, Guid duesManagementId)
        {
            try
            {
                var dc = new ManagementContext();
                var dues = (from xx in dc.FeeItem
                            where xx.FeeManagedBy.FeeManagementId == duesManagementId
                            where xx.FeeCollectionId == duesItemId
                            select xx).FirstOrDefault();

                if (dues != null)
                {
                    List<long> ids = new List<long>();
                    foreach (var fee in dues.FeesRequired)
                        ids.Add(fee.FeeRequiredId);
                    foreach (var id in ids)
                        dc.FeesRequired.Remove(dc.FeesRequired.Where(x => x.FeeRequiredId == id).FirstOrDefault());

                    ids.Clear();

                    foreach (var fee in dues.FeesCollected)
                        ids.Add(fee.FeeCollectionId);
                    foreach (var id in ids)
                        dc.FeesCollected.Remove(dc.FeesCollected.Where(x => x.FeeCollectionId == id).FirstOrDefault());


                    dc.FeeItem.Remove(dues);
                    int changes = dc.SaveChanges();
                    if (changes == 0)
                        throw new Exception("Didn't Delete Fee Item For Dues");
                    return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        /// <summary>
        /// Get Due Collection added a optional param to identify to get DuesPortableModel with  Next and Previous due
        /// </summary>
        /// <param name="duesItemId"></param>
        /// <param name="duesManagementId"></param>
        /// <param name="currentMemberId"></param>
        /// <param name="isNextPreviousDueRequired"></param>
        /// <returns></returns>
        public static DuesPortableModel GetDuesCollectionItem(long duesItemId, Guid duesManagementId, Guid currentMemberId, bool isNextPreviousDueRequired = false)
        {
            try
            {
                DuesPortableModel due = new DuesPortableModel();
                var dc = new ManagementContext();
                var dues = (from xx in dc.FeeItem.Include("FeeManagedBy").Include("FeeManagedBy.FeeClassifications").Include("FeeManagedBy.FeeClassifications").Include("FeeManagedBy.FeeClassifications.MembersClassified").Include("FeeManagedBy.FeeClassifications.MembersClassified.Member")
                            where xx.FeeManagedBy.FeeManagementId == duesManagementId
                            where xx.FeeCollectionId == duesItemId
                            select xx).FirstOrDefault();

                if (dues != null)
                {
                    due.OwnerEntity = DuesOwnerEntityEnum.league.ToString();
                    due.DayOfMonthToCollectDefault = dues.FeeManagedBy.DayOfMonthToCollectDefault;
                    due.DaysBeforeDeadlineToNotifyDefault = dues.FeeManagedBy.DaysBeforeDeadlineToNotifyDefault;
                    due.DuesCostDisplay = dues.FeeManagedBy.FeeCostDefault.ToString("N2");
                    due.DuesCost = dues.FeeManagedBy.FeeCostDefault;
                    due.DuesId = duesManagementId;
                    due.LeagueOwnerId = dues.FeeManagedBy.LeagueOwner.LeagueId;
                    due.LeagueOwnerName = dues.FeeManagedBy.LeagueOwner.Name;
                    due.AcceptPaymentsOnline = dues.FeeManagedBy.AcceptPaymentsOnline;
                    due.PayPalEmailAddress = dues.FeeManagedBy.PayPalEmailAddress;
                    due.EmailResponse = dues.FeeManagedBy.EmailResponse;
                    due.WhoPaysProcessorFeesEnum = (WhoPaysProcessorFeesEnum)Enum.Parse(typeof(WhoPaysProcessorFeesEnum), dues.FeeManagedBy.WhoPaysProcessorFeesEnum.ToString());
                    if (dues.FeeManagedBy.CurrencyRate != null)
                        due.Currency = dues.FeeManagedBy.CurrencyRate.CurrencyAbbrName;
                    else
                        due.Currency = "USD";

                    foreach (var classification in dues.FeeManagedBy.FeeClassifications)
                    {
                        FeeClassified c = new FeeClassified();
                        c.FeeClassificationId = classification.FeeClassificationId;
                        c.FeeRequired = classification.FeeRequired;
                        c.Name = classification.Name;
                        c.DoesNotPayDues = classification.DoesNotPayDues;

                        foreach (var mem in classification.MembersClassified)
                            c.MembersInClass.Add(new MemberDisplayBasic() { DerbyName = mem.Member.DerbyName, MemberId = mem.Member.MemberId });

                        due.Classifications.Add(c);
                    }

                    DuesItem item = new DuesItem();
                    item.CostOfDues = dues.CostOfFee;
                    item.DuesItemId = dues.FeeCollectionId;
                    item.PayBy = dues.PayBy;
                    item.TotalPaid = 0.00;
                    item.TotalWithstanding = 0.00;
                    item.TotalPaymentNeededFromMember = dues.CostOfFee;



                    foreach (var fee in dues.FeesRequired)
                    {
                        DuesRequired col = new DuesRequired();
                        col.DuesRequire = fee.FeeRequired;
                        col.DuesRequiredId = fee.FeeRequiredId;
                        col.IsPaidInFull = fee.IsPaidInFull;
                        col.MemberRequiredId = fee.MemberRequiredFrom.MemberId;
                        col.MemberRequiredName = fee.MemberRequiredFrom.DerbyName;
                        col.Note = fee.Note;
                        col.IsWaived = fee.IsFeeWaived;
                        col.RequiredDate = fee.Created;

                        item.DuesRequired.Add(col);
                    }

                    foreach (var fee in dues.FeesCollected)
                    {
                        DuesCollected col = new DuesCollected();

                        col.DuesPaid = fee.FeeCollected;
                        item.TotalPaid += fee.FeeCollected;
                        col.DuesCollectedId = fee.FeeCollectionId;
                        col.IsPaidInFull = fee.IsPaidInFull;
                        col.MemberPaidId = fee.MemberPaid.MemberId;
                        col.MemberPaidName = fee.MemberPaid.DerbyName;
                        col.Note = fee.Note;
                        col.IsWaived = fee.IsFeeWaived;
                        col.WasDuesClearedByUser = fee.WasClearedByUser;
                        col.PaidDate = fee.Created;
                        if (currentMemberId == fee.MemberPaid.MemberId)
                        {
                            if (col.IsPaidInFull || col.IsWaived)
                                item.IsCurrentMemberPaidOrWaivedInFull = true;
                        }

                        item.DuesCollected.Add(col);
                    }
                    var members = dues.FeeManagedBy.LeagueOwner.Members.Where(x => x.IsInactiveForLeague == false && x.HasLeftLeague == false).OrderBy(x => x.Member.DerbyName).ToList();
                    foreach (var member in members)
                    {
                        //if the member joined after the dues was due.

                        //adds every member to the member list
                        MemberDisplayDues mem = new MemberDisplayDues();
                        mem.MemberId = member.Member.MemberId;
                        mem.DerbyName = member.Member.DerbyName;
                        mem.LastName = member.Member.Lastname;

                        if (member.Member.ContactCard != null && member.Member.ContactCard.Emails.FirstOrDefault() != null)
                            mem.UserId = Guid.NewGuid();
                        else
                            mem.UserId = member.Member.AspNetUserId;

                        var collection = item.DuesCollected.Where(x => x.MemberPaidId == mem.MemberId);
                        var required = item.DuesRequired.Where(x => x.MemberRequiredId == mem.MemberId).FirstOrDefault();
                        var classification = due.Classifications.Where(x => x.MembersInClass.Where(y => y.MemberId == mem.MemberId).Count() > 0).FirstOrDefault();

                        //set variables before defining them below.
                        mem.collected = 0.00;
                        mem.isPaidFull = false;
                        mem.isWaived = false;

                        if (required != null)
                            mem.due = required.DuesRequire;
                        else if (classification != null)
                        {
                            mem.DoesNotPayDues = classification.DoesNotPayDues;
                            mem.isWaived = classification.DoesNotPayDues;
                            mem.due = classification.FeeRequired;
                            if (mem.due <= 0)
                                mem.isPaidFull = true;
                        }
                        else
                            mem.due = item.CostOfDues;


                        foreach (var col in collection)
                        {
                            mem.collected += col.DuesPaid;
                            mem.due -= col.DuesPaid;
                            mem.isPaidFull = col.IsPaidInFull;
                            mem.isWaived = col.IsWaived;
                            mem.DatePaid = col.PaidDate;
                        }
                        bool wasDuesClearedByUser = false;
                        if (collection.Where(x => x.WasDuesClearedByUser == true).FirstOrDefault() != null)
                            wasDuesClearedByUser = true;
                        //waive the user since they were created after dues was due.
                        if (item.PayBy < member.Created && wasDuesClearedByUser == false)
                        {
                            mem.isWaived = true;
                            mem.isPaidFull = true;
                        }
                        mem.tempDue = mem.due;
                        due.Members.Add(mem);
                        //gets member in collection list
                        var collected = item.DuesCollected.Where(x => x.MemberPaidId == member.Member.MemberId);
                        if (collected.Count() == 0)
                        {
                            //if the member was created after payment, we don't require member to pay.
                            if (item.PayBy > member.Created)
                            {
                                item.TotalWithstanding -= mem.due;
                                if (member.Member.MemberId == currentMemberId)
                                {
                                    item.TotalPaidFromMember = 0.00;
                                    item.TotalPaymentNeededFromMember = mem.due;
                                    if (mem.due == 0.0)
                                        item.IsCurrentMemberPaidOrWaivedInFull = true;
                                }
                            }
                        }
                        else
                        {
                            //make sure member joined dues program before dues item is due.
                            if (item.PayBy > member.Created)
                            {
                                //gets the total dues paid by the member for this month.  Can pay in increments so thats
                                // why we check for all collected items.
                                double totalPaid = 0.00;
                                foreach (var c in collected)
                                {
                                    totalPaid += c.DuesPaid;
                                }
                                //if the total dues paid isn't equal to the actual cost of the month,
                                // we subtract it from the total withstanding dues.
                                if (totalPaid != item.CostOfDues)
                                {
                                    item.TotalWithstanding -= totalPaid;
                                }
                                //total withstanding for the current member accessing this page.
                                if (member.Member.MemberId == currentMemberId)
                                {
                                    item.TotalPaymentNeededFromMember = mem.due;
                                    item.TotalPaidFromMember = totalPaid;
                                }
                            }
                        }

                    }
                    due.DuesFees.Add(item);
                }
                //check to see Next or Previous Due Item required if so then make database call to get due collection 
                if (isNextPreviousDueRequired == true)
                {
                    GetNextPreviousDueItem(duesItemId, due);
                }

                return due;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        private static void GetNextPreviousDueItem(long duesItemId, DuesPortableModel due)
        {
            //get the collection of all the dues of the League
            var dueCollection = GetDuesItemCollection(due.LeagueOwnerId);

            if (dueCollection != null)
            {
                int currentItemIndex = dueCollection.FindIndex(item => item.Equals(Convert.ToInt64(duesItemId)));

                if (currentItemIndex >= 0)
                {
                    if (currentItemIndex == 0) //check to see its first element
                    {
                        if (dueCollection.Count > 1)
                            due.NextDueItem = dueCollection.ElementAt(currentItemIndex + 1);
                        else
                            due.NextDueItem = 0;
                        due.PreviousDueItem = 0;
                    }
                    else if (currentItemIndex != 0 && (currentItemIndex + 1 != dueCollection.Count))
                    {
                        due.NextDueItem = dueCollection.ElementAt(currentItemIndex + 1);
                        due.PreviousDueItem = dueCollection.ElementAt(currentItemIndex - 1);
                    }
                    else //check to see its last element
                    {
                        due.NextDueItem = 0;
                        due.PreviousDueItem = dueCollection.ElementAt(currentItemIndex - 1);
                    }
                }
            }
        }

        public static DuesPortableModel GetDuesObject(Guid leagueId, Guid currentMemberId)
        {
            DuesPortableModel due = new DuesPortableModel();
            try
            {
                var dc = new ManagementContext();
                var dues = (from xx in dc.FeeManagement.Include("LeagueOwner").Include("FeeClassifications").Include("FeeClassifications").Include("FeeClassifications.MembersClassified").Include("FeeClassifications.MembersClassified.Member")
                            where xx.LeagueOwner.LeagueId == leagueId
                            select xx).FirstOrDefault();

                if (dues != null)
                {
                    due.OwnerEntity = DuesOwnerEntityEnum.league.ToString();
                    due.DayOfMonthToCollectDefault = dues.DayOfMonthToCollectDefault;
                    due.DaysBeforeDeadlineToNotifyDefault = dues.DaysBeforeDeadlineToNotifyDefault;
                    due.DuesCostDisplay = dues.FeeCostDefault.ToString("N2");
                    due.DuesCost = dues.FeeCostDefault;
                    due.DuesId = dues.FeeManagementId;
                    due.LeagueOwnerId = dues.LeagueOwner.LeagueId;
                    due.LeagueOwnerName = dues.LeagueOwner.Name;
                    due.AcceptPaymentsOnline = dues.AcceptPaymentsOnline;
                    due.PayPalEmailAddress = dues.PayPalEmailAddress;
                    due.WhoPaysProcessorFeesEnum = (WhoPaysProcessorFeesEnum)Enum.Parse(typeof(WhoPaysProcessorFeesEnum), dues.WhoPaysProcessorFeesEnum.ToString());
                    due.DuesEmailText = dues.EmailResponse;

                    foreach (var classification in dues.FeeClassifications)
                    {
                        FeeClassified c = new FeeClassified();
                        c.FeeClassificationId = classification.FeeClassificationId;
                        c.FeeRequired = classification.FeeRequired;
                        c.Name = classification.Name;
                        c.DoesNotPayDues = classification.DoesNotPayDues;

                        foreach (var mem in classification.MembersClassified)
                            c.MembersInClass.Add(new MemberDisplayBasic() { DerbyName = mem.Member.DerbyName, MemberId = mem.Member.MemberId });

                        due.Classifications.Add(c);
                    }

                    var members = dues.LeagueOwner.Members.Where(x => x.IsInactiveForLeague == false && x.HasLeftLeague == false).OrderBy(x => x.Member.DerbyName).ToList();
                    foreach (var member in members)
                    {
                        MemberDisplayDues mem = new MemberDisplayDues();
                        mem.MemberId = member.Member.MemberId;
                        mem.DerbyName = member.Member.DerbyName;
                        mem.JoinedLeague = member.Created;
                        mem.LastName = member.Member.Lastname;
                        if (member.Member.ContactCard != null && member.Member.ContactCard.Emails.FirstOrDefault() != null)
                            mem.UserId = Guid.NewGuid();
                        else
                            mem.UserId = member.Member.AspNetUserId;

                        due.Members.Add(mem);
                    }

                    var duess = dues.Fees.OrderByDescending(x => x.PayBy);
                    foreach (var d in duess)
                    {
                        DuesItem item = new DuesItem();
                        item.CostOfDues = d.CostOfFee;
                        item.DuesItemId = d.FeeCollectionId;
                        item.PayBy = d.PayBy;
                        item.TotalPaid = 0.00;
                        item.TotalWithstanding = 0.00;
                        item.TotalPaymentNeededFromMember = d.CostOfFee;
                        item.ClassificationName = "";
                        item.MemberClassificationName = "";
                        foreach (var fee in d.FeesCollected)
                        {
                            DuesCollected col = new DuesCollected();
                            col.DuesPaid = fee.FeeCollected;
                            item.TotalPaid += fee.FeeCollected;
                            col.DuesCollectedId = fee.FeeCollectionId;
                            col.IsPaidInFull = fee.IsPaidInFull;
                            col.MemberPaidId = fee.MemberPaid.MemberId;
                            col.MemberPaidName = fee.MemberPaid.DerbyName;
                            col.Note = fee.Note;
                            col.IsWaived = fee.IsFeeWaived;
                            if (currentMemberId == fee.MemberPaid.MemberId)
                            {
                                if (col.IsPaidInFull || col.IsWaived)
                                    item.IsCurrentMemberPaidOrWaivedInFull = true;

                            }
                            item.DuesCollected.Add(col);
                        }
                        foreach (var fee in d.FeesRequired)
                        {
                            DuesRequired col = new DuesRequired();
                            col.DuesRequire = fee.FeeRequired;
                            col.DuesRequiredId = fee.FeeRequiredId;
                            col.IsPaidInFull = fee.IsPaidInFull;
                            col.MemberRequiredId = fee.MemberRequiredFrom.MemberId;
                            col.MemberRequiredName = fee.MemberRequiredFrom.DerbyName;
                            col.Note = fee.Note;
                            col.IsWaived = fee.IsFeeWaived;

                            item.DuesRequired.Add(col);
                        }

                        foreach (var member in due.Members)
                        {

                            //gets member in collection list
                            var collected = item.DuesCollected.Where(x => x.MemberPaidId == member.MemberId);
                            var required = item.DuesRequired.Where(x => x.MemberRequiredId == member.MemberId).FirstOrDefault();
                            var classification = due.Classifications.Where(x => x.MembersInClass.Where(y => y.MemberId == member.MemberId).Count() > 0).FirstOrDefault();

                            bool doesNotPayDues = false;
                            double duesDue = 0.0;
                            if (required != null)
                            {
                                duesDue = required.DuesRequire;
                                if (member.MemberId == currentMemberId)
                                {
                                    item.CostOfDuesFromMember = required.DuesRequire;
                                    item.MemberClassificationName = "Custom";
                                }
                            }
                            else if (classification != null)
                            {
                                doesNotPayDues = classification.DoesNotPayDues;
                                duesDue = classification.FeeRequired;
                                //if the user was classified and told they don't have to pay dues.
                                if (doesNotPayDues && duesDue <= 0)
                                {
                                    DuesCollected col = new DuesCollected();
                                    col.MemberPaidId = member.MemberId;
                                    col.IsWaived = true;
                                    if (currentMemberId == member.MemberId)
                                    {
                                        item.IsCurrentMemberPaidOrWaivedInFull = true;

                                    }
                                    item.DuesCollected.Add(col);
                                }

                                item.CostOfDuesFromMember = classification.FeeRequired;
                                //item.MemberClassificationName = classification.Name;
                                member.ClassificationId = classification.FeeClassificationId;
                                member.ClassificationName = classification.Name;
                                if (member.MemberId == currentMemberId)
                                    item.MemberClassificationName = classification.Name;
                            }
                            else
                            {
                                if (member.MemberId == currentMemberId)
                                {
                                    item.CostOfDuesFromMember = item.CostOfDues;
                                    item.MemberClassificationName = "Default";
                                }
                                duesDue = item.CostOfDues;
                            }

                            //no dues were collected
                            if (collected.Count() == 0)
                            {
                                //waive the user since they were created after dues was due.
                                if (item.PayBy < member.JoinedLeague)
                                {
                                    DuesCollected col = new DuesCollected();
                                    col.MemberPaidId = member.MemberId;
                                    col.IsWaived = true;
                                    if (currentMemberId == member.MemberId)
                                    {
                                        item.IsCurrentMemberPaidOrWaivedInFull = true;

                                    }
                                    item.DuesCollected.Add(col);
                                }
                                else
                                {
                                    item.TotalWithstanding -= duesDue;
                                    AddTotalWithstandingToMember(due, duesDue, member);
                                    if (member.MemberId == currentMemberId)
                                    {
                                        item.TotalPaidFromMember = 0.00;
                                        item.TotalPaymentNeededFromMember = duesDue;
                                        item.CostOfDuesFromMember = duesDue;
                                        if (duesDue <= 0)
                                            item.IsCurrentMemberPaidOrWaivedInFull = true;
                                    }
                                }

                            }
                            else
                            {
                                bool isWaived = false;
                                //gets the total dues paid by the member for this month.  Can pay in increments so thats
                                // why we check for all collected items.
                                double totalPaid = 0.00;
                                foreach (var c in collected)
                                {
                                    totalPaid += c.DuesPaid;
                                    isWaived = c.IsWaived;
                                    member.collected += c.DuesPaid;
                                }
                                //waive the user since they were created after dues was due.
                                if (item.PayBy < member.JoinedLeague)
                                    isWaived = true;

                                //if the total dues paid isn't equal to the actual cost of the month,
                                // we subtract it from the total withstanding dues.
                                if (totalPaid != item.CostOfDues && !isWaived)
                                {
                                    item.TotalWithstanding -= (duesDue - totalPaid);

                                    //generating a list of members who all have outstanding balances with the league.
                                    AddTotalWithstandingToMember(due, (duesDue - totalPaid), member);

                                }
                                //total withstanding for the current member accessing this page.
                                if (member.MemberId == currentMemberId)
                                {
                                    item.CostOfDuesFromMember = duesDue;
                                    item.TotalPaymentNeededFromMember = (duesDue - totalPaid);
                                    item.TotalPaidFromMember = totalPaid;
                                    if (item.TotalPaymentNeededFromMember <= 0)
                                        item.IsCurrentMemberPaidOrWaivedInFull = true;
                                }
                            }

                        }
                        due.DuesFees.Add(item);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return due;
        }

        private static void AddTotalWithstandingToMember(DuesPortableModel due, double feeToAdd, MemberDisplayBasic member)
        {
            var memberWhoOwes = due.Members.Where(x => x.MemberId == member.MemberId).FirstOrDefault();
            if (memberWhoOwes != null)
                memberWhoOwes.TotalWithstanding += feeToAdd;
            else
            {
                due.Members.Add(new MemberDisplayDues { MemberId = member.MemberId, DerbyName = member.DerbyName, TotalWithstanding = feeToAdd, UserId = member.UserId });
            }
        }

        /// <summary>
        /// Get Due Item Collection making request to get only Fee Management egnore other entities
        /// </summary>
        /// <param name="leagueId"></param>
        /// <returns></returns>
        public static List<long> GetDuesItemCollection(Guid leagueId)
        {
            List<long> due = new List<long>();
            try
            {
                var dc = new ManagementContext();
                var dues = (from xx in dc.FeeManagement
                            where xx.LeagueOwner.LeagueId == leagueId
                            select xx).FirstOrDefault();

                var duess = dues.Fees.OrderByDescending(x => x.PayBy);

                foreach (var d in duess)
                {
                    due.Add(d.FeeCollectionId);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return due;
        }
    }
}
