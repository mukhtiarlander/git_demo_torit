using System.Collections.Generic;
using System.Linq;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.EmailServer;
using RDN.Library.DataModels.EmailServer.Enums;
using RDN.Library.Classes.Error;
using System;

namespace RDN.Library.Classes.EmailServer
{
    public enum EmailServerLayoutsEnum
    {
        Blank = 0,
        Default = 1,
        FederationApproved = 2,
        LeagueApproved = 3,
        LeagueRejected = 4,
        NewFederationAdmin = 5,
        NewLeagueAdmin = 6,
        PasswordChanged = 7,
        RecoverLostPassword = 8,
        SendEmailVerificationWithoutPassword = 9,
        SendEmailVerificationWithPassword = 10,
        OwnershipTakenOfLeague = 11,
        AddedToANewGame = 12,
        EmailUnFilledProfilesTask = 13,
        AutomatedStats = 14,
        LeagueCreatedMemberProfile = 15,
        MemberAddedToManageDerbyGame = 16,
        //for the RDNation notification
        DuesCollectingNotification = 17,
        //for the nortification managers want to write out.
        DuesCollectingNotificationBlank = 18,
        //member was just assigned an ownership role for the league.
        MemberAddedToOwnershipGroupOfLeague = 19,
        SendMessageToUserFromOtherUser = 20,
        //sends the messages to the user from users in the past few hours.
        SendLatestConversationsThreadToUser = 21,
        //receipt for a league that subscribes to RDNation.
        ReceiptForLeagueSubscription = 22,
        SubscriptionIsAboutToRunOutTask = 23,
        SubscriptionHasExpiredTask = 24,
        SubscriptionForLeagueExpiringAdmin = 25,
        //user was added to the league group.
        UserAddedToLeagueGroup = 26,
        //sends forum post if broadcast message was checked.
        SendForumBroadcastMessageToUserGroup = 27,
        //the league payment failed.
        LeagueSubscriptionFailedPayment = 28,
        //dues payment made and this one gets sent to user.
        DuesPaymentMadeForUser = 29,
        //tell the league that a dues payment was made.
        DuesPaymentMadeForLeague = 30,
        //sends the latest forum topics to the user.
        SendLatestForumTopicsToUser = 31,
        //store item is expiring soon.
        StoreItemExpiringSoon = 32,
        //sends a receipt email to the payer of the store.
        StoreSendReceiptForOrder = 33,
        //sends email to seller about what just got bought from their store.
        StoreSendSellerAboutOrdersBought = 34,
        StoreSendShippedItemsForOrder = 35,
        //subscription will auto renew
        SubscriptionWillAutoRenew = 36,
        //this is where a dues payment is made and the email used isn't confirmed.
        PayPalEmailIsNotConfirmed = 37,
        //shop isn't published
        ShopIsNotPublished = 38,
        /// <summary>
        /// shop doesn't have any items.
        /// </summary>
        ShopHasNoItems = 39,
        /// <summary>
        /// paypal email is restricted and wrong.
        /// </summary>
        PaypalEmailIsRestricted = 40,
        /// <summary>
        /// user just paid for the paywall.
        /// </summary>
        PaywallPaid = 41,
        PaywallPaidMerchant = 42,
        /// <summary>
        /// refund was made.
        /// </summary>
        PaywallRefunded = 43,
        PaywallRefundedMerchant = 44,
        /// <summary>
        /// your username was changed.
        /// </summary>
        UsernameChanged = 45,
        CalendarSendNewEvent = 46,
        TextMessage = 47,
        TextMessageNotVerified = 48,
        PollNewPollCreated = 49,
        /// <summary>
        /// card was declinded for subscription
        /// </summary>
        SubscriptionCardWasDeclined = 50,
        SubscriptionWasCancelled = 51,
        TournamentMemberAddedToManage = 52,


        RNAddedAsEditor = 53,
        RNAddedAsTrustedEditor = 54,
        RNPaymentRequested = 55,
        RNPaymentJustPaid = 56,
        RNPaymentJustCompleted = 57,
        RNMemberLeagueOfTheWeek = 58,
        RNMoneyAddedToAccount = 59,
        RNNewPostPublished = 60,
        RNPostAwaitingApproval = 61,
        //TODO
        RNWriterRegistered = 62,

        RDNShopsReviewPurchaseMade = 63,
        RDNJobNewCreated= 64,
        RDNWelcomeMessageInviteMembers= 65,

    }
    public static class EmailServer
    {
        public static readonly string DEFAULT_SUBJECT = "[RDNation]";
        public static readonly string DEFAULT_SUBJECT_ROLLIN_NEWS = "[RollinNews]";
        public static bool ChangeEmailFromThenTo(string oldEmail, string newEmail)
        {
            var dc = new ManagementContext();

            var emails = dc.ScoreboardFeedback.Where(x => x.Email == oldEmail).FirstOrDefault();
            if (emails != null)
            {
                emails.Email = newEmail;
                dc.SaveChanges();
                return true;
            }
            var email2 = dc.ScoreboardDownloads.Where(x => x.Email == oldEmail).FirstOrDefault();
            if (email2 != null)
            {
                email2.Email = newEmail;
                dc.SaveChanges();
                return true;
            }
            var emails3 = dc.RefRoster.Where(x => x.FacebookLink == oldEmail).FirstOrDefault();
            if (emails3 != null)
            {
                emails3.FacebookLink = newEmail;
                dc.SaveChanges();
                return true;
            }
            var emails4 = dc.ContactLeagueAddresses.Where(x => x.EmailAddress == oldEmail).FirstOrDefault();
            if (emails4 != null)
            {
                emails4.EmailAddress = newEmail;
                emails4.ContactLeague = emails4.ContactLeague;
                dc.SaveChanges();
                return true;
            }

            var emails5 = dc.EmailsForAllEntities.Where(x => x.EmailAddress == oldEmail).FirstOrDefault();
            if (emails4 != null)
            {
                emails4.EmailAddress = newEmail;
                dc.SaveChanges();
                return true;
            }
            return false;
        }

        public static bool AddEmailToUnsubscribeList(string email)
        {
            var dc = new ManagementContext();
            NonSubscribersList non = new NonSubscribersList();
            non.EmailToRemoveFromList = email;
            dc.NonSubscribersList.Add(non);
            dc.SaveChanges();
            return true;
        }

        public static bool AddEmailToSubscriberList(string email)
        {
            var dc = new ManagementContext();
            SubscribersList non = new SubscribersList();
            non.EmailToAddToList = email;
            dc.SubscribersList.Add(non);
            dc.SaveChanges();
            return true;
        }

        /// <summary>
        /// Send the email to the email server for processing
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="priority"></param>
        public static void SendEmail(string from, string displayNameFrom, string to, string subject, string body, EmailPriority priority = EmailPriority.Important)
        {
            try
            {
                var dc = new ManagementContext();

                var email = new EmailSendItem(priority);
                email.EmailLayout = "Default";
                email.From = from;
                email.DisplayNameFrom = displayNameFrom;
                email.Reciever = to;
                email.Subject = subject;
                email.Properties.Add(new EmailProperty { Key = "body", Value = body });
                if (!String.IsNullOrEmpty(email.Reciever))
                    dc.EmailServer.Add(email);
                dc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        /// <summary>
        /// Send the email to the email server for processing
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="properties">Email properties. Each property is defined as a key/value item. A property can for instance be body/{value] and then that will be parsed into the layout at the place of the %body% tag</param>
        /// <param name="layout">The layout name. For available names, open the database and look inside the table RDN_EmailServer_EmailLayouts</param>
        /// <param name="priority"></param>
        public static bool SendEmail(string from, string displayNameFrom, string to, string subject, Dictionary<string, string> properties, EmailServerLayoutsEnum layout = EmailServerLayoutsEnum.Default, EmailPriority priority = EmailPriority.Important)
        {
            try
            {
                var dc = new ManagementContext();

                if (layout == EmailServerLayoutsEnum.TextMessage)
                    priority = EmailPriority.Important;

                var email = new EmailSendItem(priority);
                email.EmailLayout = layout.ToString();
                email.From = from;
                email.DisplayNameFrom = displayNameFrom;
                email.Reciever = to;
                email.Subject = subject;
                foreach (var property in properties)
                    email.Properties.Add(new EmailProperty { Key = property.Key, Value = property.Value });

                if (!String.IsNullOrEmpty(email.Reciever))
                    dc.EmailServer.Add(email);

                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: to);
            }
            return false;
        }

        internal static List<EmailSendItem> PullPriorityItemsToSend()
        {
            var dc = new ManagementContext();
            return dc.EmailServer.Include("Properties").Where(x => x.Prio.Equals(1)).OrderBy(x => x.Created).ToList();
        }

        internal static List<EmailSendItem> PullItemsToSend(int numberOfItemsToPull)
        {
            var dc = new ManagementContext();
            return dc.EmailServer.Include("Properties").Where(x => x.Prio.Equals(0)).OrderBy(x => x.Created).Take(numberOfItemsToPull).ToList();
        }

        internal static string PullLatestEmailMessage()
        {
            var dc = new EmailServerContext();
            var message = dc.EmailMessages.OrderByDescending(x => x.EmailId).FirstOrDefault();
            if (message != null)
                return message.Message;
            return string.Empty;
        }

        internal static void DeleteItem(int itemId)
        {
            var dc = new ManagementContext();
            var item = dc.EmailServer.FirstOrDefault(x => x.EmailSendItemId.Equals(itemId));
            if (item == null)
                return;
            dc.EmailServer.Remove(item);
            dc.SaveChanges();
        }

        internal static void DeleteItems(IList<int> itemIds)
        {
            var dc = new ManagementContext();
            var items = dc.EmailServer.Where(x => itemIds.Contains(x.EmailSendItemId)).ToList();
            if (!items.Any())
                return;
            foreach (var item in items)
                dc.EmailServer.Remove(item);
            dc.SaveChanges();
        }
    }
}
