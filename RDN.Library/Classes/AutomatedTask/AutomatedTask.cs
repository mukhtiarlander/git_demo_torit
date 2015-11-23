using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Context;
using RDN.Library.Classes.AutomatedTask.Enums;
using RDN.Library.DataModels.AutomatedTasks;
using RDN.Library.DataModels.Member;
using RDN.Utilities.Config;
using RDN.Library.DataModels.EmailServer.Enums;
using RDN.Library.DataModels.MemberFees;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Messages.Classes;
using RDN.Library.Cache;
using RDN.Library.DataModels.PaymentGateway.Merchants;
using RDN.Library.Classes.Store;
using RDN.Utilities.Strings;
using RDN.Portable.Config;
using RDN.Library.Classes.Payment.Money;
using System.Net;
using RDN.Library.Classes.EmailServer;
using RDN.Library.DataModels.Site;
using System.Web.Security;
using RDN.Library.Classes.Imports.Rinxter;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Federation.Enums;
using RDN.Portable.Classes.Controls.Message;
using RDN.Portable.Classes.Payment.Enums;
using RDN.Portable.Classes.League.Enums;
using RDN.Portable.Classes.Federation.Enums;
using RDN.Library.Cache.Singletons;
using RDN.Library.Classes.Config;
using RDN.Portable.Classes.Url;

namespace RDN.Library.Classes.AutomatedTask
{
    /// <summary>
    /// works all the automated tasks for .
    /// </summary>
    public class AutomatedTask
    {
        public int massRollinNewsPaymentsProcessed, emailsNotFilled, messagesInbox, forumContent, duesItemsCreated, verificationTable, merchantItems, subscriptionsExpiring, storesNotPublished, storeWithFewItems;
        public bool CurrencyUpdated;

        private static void testEmail()
        {
            try
            {
                var emailData = new Dictionary<string, string> {
                        { "derbyname", "Veggie D" },
                        { "sportAndProfileName", LibraryConfig.SportName + " "+ LibraryConfig.MemberName },
                        { "publicProfile", RDN.Library.Classes.Config.LibraryConfig.PublicSite +"/" + RDN.Library.Classes.Config.LibraryConfig.SportNamePlusMemberNameForUrl + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly("Veggie D") + "/0525b20627d849d2a881c9a42b42d53a"  },
                        { "editProfileLink",RDN.Library.Classes.Config.LibraryConfig.PublicSite + "/login?returnSite=league&ReturnUrl=%2fmember%2fedit" } };

                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, "spoiledtechie@gmail.com", "Your Profile Is Still Empty", emailData, layout: EmailServer.EmailServerLayoutsEnum.EmailUnFilledProfilesTask, priority: EmailPriority.Normal);
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        /// <summary>
        /// 30 days
        /// </summary>
        private static readonly int HOURS_BETWEEN_EMAILS_OF_NOT_FILLED_PROFILES = 720;
        private static readonly int HOURS_BETWEEN_EMAILS_FOR_ADMIN = 12;
        private static readonly int HOURS_BETWEEN_EMAILS_FOR_MERCH_ITEMS_EXPIRING = 12;
        private static readonly int HOURS_BETWEEN_EMAILS_FOR_MESSAGES_IN_INBOX = 12;
        private static readonly int HOURS_BETWEEN_EMAILS_FOR_FORUM_MESSAGES_FOR_CONTENT = 165;
        private static readonly int HOURS_BETWEEN_EMAILS_FOR_MERCHANT_STORE_NOT_PUBLISHED = 165;
        private static readonly int HOURS_BETWEEN_EMAILS_FOR_SUBSCRIPTIONS_EXPIRING = 12;
        private static readonly int HOURS_BETWEEN_CURRENCY_UPDATES = 12;
        private static readonly int HOURS_BETWEEN_TEXTMESSAGE_CHECK = 12;
        private static readonly int HOURS_BETWEEN_MASS_ROLLIN_NEWS_PAYMENTS = 12;
        private static readonly int HOURS_BETWEEN_NEW_SKATER_LEAGUE_PROFILE = 165;
        private static readonly int ONE_WEEK_WORTH_OF_HOURS = 165;
        private static readonly int TWO_WEEKS_WORTH_OF_HOURS = 330;
        private static readonly int ONE_MONTH_WORTH_OF_HOURS = 720;
        private static readonly int TWELVE_HOURS = 12;
        private static readonly int THREE_DAYS = 72;
        private static readonly int FORTY_EIGHT_HOURS = 48;
        private static readonly int RINXTER_IMPORT = 0; //720
        /// <summary>
        /// 30 days
        /// </summary>
        private static readonly int HOURS_BETWEEN_EMAILS_FOR_NON_VERIFIED_USERS = 720;
        public static int EmailLeaguesAboutSubscriptionsExpiring()
        {
            int emailsSent = 0;
            try
            {

                var dc = new ManagementContext();
                int type = Convert.ToInt32(TaskTypeEnum.EmailLeaguesWhichSubscriptionToWiteIsAboutToOrHasExpired);
                var emailTask = dc.AutomatedTasks.Where(x => x.TaskIdForDescription == type).FirstOrDefault();

                if (emailTask == null)
                {
                    TaskForRunning newTask = new TaskForRunning();
                    newTask.FirstRun = DateTime.UtcNow;
                    newTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_FOR_SUBSCRIPTIONS_EXPIRING;
                    newTask.LastRun = DateTime.UtcNow;
                    newTask.TaskIdForDescription = type;
                    dc.AutomatedTasks.Add(newTask);
                    dc.SaveChanges();
                }
                else
                {
                    emailTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_FOR_SUBSCRIPTIONS_EXPIRING;
                    if (emailTask.LastRun.AddHours(HOURS_BETWEEN_EMAILS_FOR_SUBSCRIPTIONS_EXPIRING) < DateTime.UtcNow)
                    {
                        emailTask.LastRun = DateTime.UtcNow;
                        try
                        {
                            emailsSent = RDN.Library.Classes.League.LeagueFactory.EmailAllLeaguesWhomSubscriptionNeedsReviewed();
                        }
                        catch (Exception exception)
                        {
                            Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
                        }
                        dc.SaveChanges();
                    }
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return emailsSent;
        }
        public static bool UpdateCurrencyExchangeRates()
        {
            bool hasWorked = false;
            try
            {

                var dc = new ManagementContext();
                int type = Convert.ToInt32(TaskTypeEnum.UpdateCurrencyExchangeRates);
                var emailTask = dc.AutomatedTasks.Where(x => x.TaskIdForDescription == type).FirstOrDefault();

                if (emailTask == null)
                {
                    TaskForRunning newTask = new TaskForRunning();
                    newTask.FirstRun = DateTime.UtcNow;
                    newTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_CURRENCY_UPDATES;
                    newTask.LastRun = DateTime.UtcNow;
                    newTask.TaskIdForDescription = type;
                    dc.AutomatedTasks.Add(newTask);
                    dc.SaveChanges();
                }
                else
                {
                    emailTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_CURRENCY_UPDATES;
                    if (emailTask.LastRun.AddHours(HOURS_BETWEEN_CURRENCY_UPDATES) < DateTime.UtcNow)
                    {
                        emailTask.LastRun = DateTime.UtcNow;
                        try
                        {
                            hasWorked = CurrencyExchangeFactory.UpdateCurrencyExchangeRates();
                        }
                        catch (Exception exception)
                        {
                            Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
                        }
                        dc.SaveChanges();
                    }
                }
                WebClient wc1 = new WebClient();
                wc1.DownloadStringAsync(new Uri(LibraryConfig.InternalSite + UrlManager.URL_TO_CLEAR_EXCHANGE_RATES));
                WebClient wc2 = new WebClient();
                wc2.DownloadStringAsync(new Uri(LibraryConfig.ApiSite + UrlManager.URL_TO_CLEAR_EXCHANGE_RATES_API));
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return hasWorked;
        }
        public static int TextMessageAdminsToShowSMSIsWorking()
        {
            int emailsSent = 0;
            try
            {
                var dc = new ManagementContext();
                int type = Convert.ToInt32(TaskTypeEnum.TextMessagesAreWorking);
                var emailTask = dc.AutomatedTasks.Where(x => x.TaskIdForDescription == type).FirstOrDefault();

                if (emailTask == null)
                {
                    TaskForRunning newTask = new TaskForRunning();
                    newTask.FirstRun = DateTime.UtcNow;
                    newTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_TEXTMESSAGE_CHECK;
                    newTask.LastRun = DateTime.UtcNow;
                    newTask.TaskIdForDescription = type;
                    dc.AutomatedTasks.Add(newTask);
                    dc.SaveChanges();
                }
                else
                {
                    emailTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_TEXTMESSAGE_CHECK;
                    if (emailTask.LastRun.AddHours(HOURS_BETWEEN_TEXTMESSAGE_CHECK) < DateTime.UtcNow)
                    {
                        var emailData = new Dictionary<string, string> { { "body", @RDN.Library.Classes.Config.LibraryConfig.WebsiteShortName + " Text Messages Still Running" } };

                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, "AdminEmail", LibraryConfig.TextMessageEmail, LibraryConfig.AdminPhoneNumber, emailData, EmailServerLayoutsEnum.TextMessage, Library.DataModels.EmailServer.Enums.EmailPriority.Important);
                    }
                    dc.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return emailsSent;
        }

        /// <summary>
        /// we need to email users who have currently not filled out their profiles every 30 days and tell them, come back to 
        /// fill it out!!
        /// </summary>
        public static int ProcessRollinNewsMassPayments()
        {
            try
            {
                var dc = new ManagementContext();
                int type = Convert.ToInt32(TaskTypeEnum.ProcessRollinNewsMassPayments);
                var emailTask = dc.AutomatedTasks.Where(x => x.TaskIdForDescription == type).FirstOrDefault();

                if (emailTask == null)
                {
                    TaskForRunning newTask = new TaskForRunning();
                    newTask.FirstRun = DateTime.UtcNow;
                    newTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_MASS_ROLLIN_NEWS_PAYMENTS;
                    newTask.LastRun = DateTime.UtcNow;
                    newTask.TaskIdForDescription = type;
                    dc.AutomatedTasks.Add(newTask);
                    dc.SaveChanges();
                }
                else
                {
                    emailTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_MASS_ROLLIN_NEWS_PAYMENTS;
                    if (emailTask.LastRun.AddHours(HOURS_BETWEEN_MASS_ROLLIN_NEWS_PAYMENTS) < DateTime.UtcNow)
                    {
                        var mode = LibraryConfig.IsProduction;

                        Payment.PaymentGateway pg = new Payment.PaymentGateway();
                        pg.StartInvoiceWizard()
                            .Initalize(RollinNewsConfig.MERCHANT_ID, "USD", Payment.Enums.PaymentProvider.Paypal, mode, Payment.Enums.ChargeTypeEnum.RollinNewsWriterPayouts)
                            .FinalizeInvoice();

                    }
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }

        /// <summary>
        /// we need to email users who have currently not filled out their profiles every 30 days and tell them, come back to 
        /// fill it out!!
        /// </summary>
        public static int EmailNotFilledOutProfile()
        {
            int emailsSent = 0;
            try
            {
                var dc = new ManagementContext();
                int type = Convert.ToInt32(TaskTypeEnum.EmailThoseMembersThatDidntFillProfileOut);
                var emailTask = dc.AutomatedTasks.Where(x => x.TaskIdForDescription == type).FirstOrDefault();

                if (emailTask == null)
                {
                    TaskForRunning newTask = new TaskForRunning();
                    newTask.FirstRun = DateTime.UtcNow;
                    newTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_OF_NOT_FILLED_PROFILES;
                    newTask.LastRun = DateTime.UtcNow;
                    newTask.TaskIdForDescription = type;
                    dc.AutomatedTasks.Add(newTask);
                    dc.SaveChanges();
                }
                else
                {
                    emailTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_OF_NOT_FILLED_PROFILES;
                    if (emailTask.LastRun.AddHours(HOURS_BETWEEN_EMAILS_OF_NOT_FILLED_PROFILES) < DateTime.UtcNow)
                    {
                        List<Member> newMembers = new List<Member>();

                        emailTask.LastRun = DateTime.UtcNow;
                        dc.SaveChanges();

                        var members = dc.Members.Where(x => x.AspNetUserId != null && x.AspNetUserId != new Guid()).Where(x => x.Retired == false).Where(x => x.PlayerNumber == null || x.PlayerNumber == "").ToList();

                        var secondMembers = dc.Members.Where(x => x.AspNetUserId != null && x.AspNetUserId != new Guid()).Where(x => x.Retired == false).Where(x => x.Photos.Count == 0).ToList();

                        newMembers.AddRange(members);
                        foreach (var mem in secondMembers)
                        {
                            if (members.Where(x => x.MemberId == mem.MemberId).FirstOrDefault() == null)
                            {
                                newMembers.Add(mem);
                            }
                        }




                        foreach (var mem in newMembers)
                        {
                            try
                            {
                                var user = System.Web.Security.Membership.GetUser((object)mem.AspNetUserId);
                                var emailData = new Dictionary<string, string> {
                        { "derbyname", mem.DerbyName },
                        { "sportAndProfileName", LibraryConfig.SportName + " "+ LibraryConfig.MemberName },
                        { "publicProfile", LibraryConfig.PublicSite +"/" + RDN.Library.Classes.Config.LibraryConfig.SportNamePlusMemberNameForUrl + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(mem.DerbyName) + "/" + mem.MemberId.ToString().Replace("-", "") },
                        { "editProfileLink",LibraryConfig.InternalSite +"/member/edit" } };

                                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, user.UserName, LibraryConfig.DefaultEmailSubject + " Your " + RDN.Library.Classes.Config.LibraryConfig.SportName + " Profile Is Empty", emailData, layout: EmailServer.EmailServerLayoutsEnum.EmailUnFilledProfilesTask, priority: EmailPriority.Normal);
                                emailsSent += 1;
                            }
                            catch (Exception exception)
                            {
                                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
                            }
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return emailsSent;
        }
        public static bool SetLeagueOfTheWeek()
        {
            try
            {
                var dc = new ManagementContext();
                int type = Convert.ToInt32(TaskTypeEnum.SkaterAndLeagueOfDay);
                var emailTask = dc.AutomatedTasks.Where(x => x.TaskIdForDescription == type).FirstOrDefault();

                if (emailTask == null)
                {
                    TaskForRunning newTask = new TaskForRunning();
                    newTask.FirstRun = DateTime.UtcNow;
                    newTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_NEW_SKATER_LEAGUE_PROFILE;
                    newTask.LastRun = DateTime.UtcNow;
                    newTask.TaskIdForDescription = type;
                    dc.AutomatedTasks.Add(newTask);
                    dc.SaveChanges();
                }
                else
                {
                    emailTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_NEW_SKATER_LEAGUE_PROFILE;
                    if (emailTask.LastRun.AddHours(HOURS_BETWEEN_NEW_SKATER_LEAGUE_PROFILE) < DateTime.UtcNow)
                    {

                        emailTask.LastRun = DateTime.UtcNow;

                        var league = (from xx in dc.Leagues
                                      where xx.Logo != null
                                      where xx.IsLeaguePublic == false
                                      where xx.Members.Count > 5
                                      where !String.IsNullOrEmpty(xx.WebSite)
                                      orderby Guid.NewGuid()
                                      select new { xx.LeagueId, xx.Name }).FirstOrDefault();




                        FrontPageHistory hist = new FrontPageHistory();
                        hist.LeagueId = league.LeagueId;
                        dc.FrontPageHistory.Add(hist);
                        int c = dc.SaveChanges();


                        var leagueEmail = League.LeagueFactory.GetLeagueOwners(league.LeagueId);
                        var emailDataLeague = new Dictionary<string, string>
                                        {
                                            { "derbyname", league.Name},
                                            { "text", "Your League has been named Rollin News League of the Week.  Congrats!"},
                                            { "link", RollinNewsConfig.WEBSITE_DEFAULT_LOCATION}
                                        };
                        for (int i = 0; i < leagueEmail.Count; i++)
                            EmailServer.EmailServer.SendEmail(RollinNewsConfig.DEFAULT_ADMIN_EMAIL, RollinNewsConfig.DEFAULT_EMAIL_FROM_NAME, leagueEmail[i].Email, LibraryConfig.DefaultEmailSubject + " League of the Week!", emailDataLeague, EmailServerLayoutsEnum.RNMemberLeagueOfTheWeek, EmailPriority.Normal);


                        return c > 0;
                    }
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool SetSkaterOfTheWeek()
        {
            try
            {
                var dc = new ManagementContext();
                int type = Convert.ToInt32(TaskTypeEnum.SkaterOfDay);
                var emailTask = dc.AutomatedTasks.Where(x => x.TaskIdForDescription == type).FirstOrDefault();

                if (emailTask == null)
                {
                    TaskForRunning newTask = new TaskForRunning();
                    newTask.FirstRun = DateTime.UtcNow;
                    newTask.HoursBetweenEachRunOfTask = FORTY_EIGHT_HOURS;
                    newTask.LastRun = DateTime.UtcNow;
                    newTask.TaskIdForDescription = type;
                    dc.AutomatedTasks.Add(newTask);
                    dc.SaveChanges();
                }
                else
                {
                    emailTask.HoursBetweenEachRunOfTask = FORTY_EIGHT_HOURS;
                    if (emailTask.LastRun.AddHours(FORTY_EIGHT_HOURS) < DateTime.UtcNow)
                    {

                        emailTask.LastRun = DateTime.UtcNow;
                        var mem = (from xx in dc.Members
                                   where xx.AspNetUserId != null
                                   where xx.AspNetUserId != new Guid()
                                   where xx.Retired == false
                                   where xx.IsNotConnectedToDerby == false
                                   where xx.IsProfileRemovedFromPublic == false
                                   where !String.IsNullOrEmpty(xx.Bio)
                                   where xx.Photos.Count > 0
                                   where !String.IsNullOrEmpty(xx.PlayerNumber)
                                   where !String.IsNullOrEmpty(xx.DerbyName)
                                   orderby Guid.NewGuid()
                                   select new { xx.MemberId, xx.AspNetUserId, xx.DerbyName }).FirstOrDefault();




                        FrontPageHistory hist = new FrontPageHistory();
                        hist.MemberId = mem.MemberId;
                        dc.FrontPageHistory.Add(hist);
                        int c = dc.SaveChanges();

                        var user = Membership.GetUser(mem.AspNetUserId);
                        var emailDataMember = new Dictionary<string, string>
                                        {
                                            { "derbyname", mem.DerbyName},
                                            { "text", "You have been named Rollin News Member of the Week.  Congrats!"},
                                            { "link", RollinNewsConfig.WEBSITE_DEFAULT_LOCATION}
                                        };
                        EmailServer.EmailServer.SendEmail(RollinNewsConfig.DEFAULT_ADMIN_EMAIL, RollinNewsConfig.DEFAULT_EMAIL_FROM_NAME, user.UserName, LibraryConfig.DefaultEmailSubject + " You are Member of the Week!", emailDataMember, EmailServerLayoutsEnum.RNMemberLeagueOfTheWeek, EmailPriority.Normal);



                        return c > 0;
                    }
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static int EmailAboutReviewingProductBought()
        {
            int emailsSent = 0;
            try
            {
                var dc = new ManagementContext();
                int type = Convert.ToInt32(TaskTypeEnum.EmailAboutReviewingProductBought);
                var emailTask = dc.AutomatedTasks.Where(x => x.TaskIdForDescription == type).FirstOrDefault();

                if (emailTask == null)
                {
                    TaskForRunning newTask = new TaskForRunning();
                    newTask.FirstRun = DateTime.UtcNow;
                    newTask.HoursBetweenEachRunOfTask = ONE_WEEK_WORTH_OF_HOURS;
                    newTask.LastRun = DateTime.UtcNow;
                    newTask.TaskIdForDescription = type;
                    dc.AutomatedTasks.Add(newTask);
                    dc.SaveChanges();
                }
                else
                {
                    emailTask.HoursBetweenEachRunOfTask = ONE_WEEK_WORTH_OF_HOURS;
                    if (emailTask.LastRun.AddHours(ONE_WEEK_WORTH_OF_HOURS) < DateTime.UtcNow)
                    {
                        List<Guid> membersToEmail = new List<Guid>();

                        emailTask.LastRun = DateTime.UtcNow;
                        var lastmodified = DateTime.UtcNow.AddDays(-7);
                        var to = (from xx in dc.Invoices.Include("InvoiceBilling").Include("InvoiceShipping").Include("Items")
                                  where xx.InvoiceStatus == (int)InvoiceStatus.Archived_Item_Completed || xx.InvoiceStatus == (int)InvoiceStatus.Shipped
                                  where xx.LastModified < lastmodified
                                  where xx.HasNotifiedAboutReviewingProduct == false
                                  select xx);

                        foreach (var message in to)
                        {
                            message.HasNotifiedAboutReviewingProduct = true;
                            message.Merchant = message.Merchant;
                            try
                            {
                                string email = string.Empty;
                                string firstName = string.Empty;
                                string lastName = string.Empty;

                                if (message.InvoiceShipping != null && !String.IsNullOrEmpty(message.InvoiceShipping.Email))
                                {
                                    if (!String.IsNullOrEmpty(message.InvoiceShipping.Email))
                                        email = message.InvoiceShipping.Email;
                                    if (!String.IsNullOrEmpty(message.InvoiceShipping.FirstName))
                                        firstName = message.InvoiceShipping.FirstName;
                                    if (!String.IsNullOrEmpty(message.InvoiceShipping.LastName))
                                        lastName = message.InvoiceShipping.LastName;
                                }
                                else if (message.InvoiceBilling != null && !String.IsNullOrEmpty(message.InvoiceBilling.Email))
                                {
                                    if (!String.IsNullOrEmpty(message.InvoiceBilling.Email))
                                        email = message.InvoiceBilling.Email;
                                    if (!String.IsNullOrEmpty(message.InvoiceBilling.FirstName))
                                        firstName = message.InvoiceBilling.FirstName;
                                    if (!String.IsNullOrEmpty(message.InvoiceBilling.LastName))
                                        lastName = message.InvoiceBilling.LastName;
                                }
                                if (!String.IsNullOrEmpty(email))
                                {
                                    if (String.IsNullOrEmpty(firstName) || String.IsNullOrEmpty(lastName))
                                    { firstName = "Sir/Madame"; lastName = ""; }
                                    ////http://localhost:8847/product-review/tdp1medium/10/e6cab01ce0044bd6a5d31970a7fe8dc0
                                    string link = LibraryConfig.ShopSite + "/product-review/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(message.Items.FirstOrDefault().Name) + "/" + message.Items.FirstOrDefault().StoreItemId + "/" + message.InvoiceId.ToString().Replace("-", "");

                                    var emailData = new Dictionary<string, string>
                                        {
                                            { "firstLastName",firstName +" " +lastName},
                                            { "linkforReview", link},
                                            { "nameOfItem",message.Items.FirstOrDefault().Name }
                                        };

                                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email, LibraryConfig.DefaultEmailSubject + " Review Your Purchase", emailData, EmailServer.EmailServerLayoutsEnum.RDNShopsReviewPurchaseMade);
                                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, LibraryConfig.DefaultEmailSubject + " Review Your Purchase", emailData, EmailServer.EmailServerLayoutsEnum.RDNShopsReviewPurchaseMade);
                                }
                            }
                            catch (Exception exception)
                            {
                                ErrorDatabaseManager.AddException(exception, exception.GetType());
                            }
                        }
                        int c = dc.SaveChanges();


                    }
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return emailsSent;
        }
        public static int EmailLeaguesWelcomeEmailAfterJoining()
        {
            int emailsSent = 0;
            try
            {
                var dc = new ManagementContext();
                int type = Convert.ToInt32(TaskTypeEnum.EmailIntroductoryEmails);
                var emailTask = dc.AutomatedTasks.Where(x => x.TaskIdForDescription == type).FirstOrDefault();

                if (emailTask == null)
                {
                    TaskForRunning newTask = new TaskForRunning();
                    newTask.FirstRun = DateTime.UtcNow;
                    newTask.HoursBetweenEachRunOfTask = THREE_DAYS;
                    newTask.LastRun = DateTime.UtcNow;
                    newTask.TaskIdForDescription = type;
                    dc.AutomatedTasks.Add(newTask);
                    dc.SaveChanges();
                }
                else
                {
                    emailTask.HoursBetweenEachRunOfTask = THREE_DAYS;
                    if (emailTask.LastRun.AddHours(THREE_DAYS) < DateTime.UtcNow)
                    {

                        emailTask.LastRun = DateTime.UtcNow;

                        var leagues = dc.Leagues.Include("Members");

                        foreach (var league in leagues)
                        {
                            try
                            {
                                var leagueTemp = League.LeagueFactory.DisplayLeague(league, false);
                                IntroductoryEmailEnum introductions = (IntroductoryEmailEnum)league.IntroductoryEmailEnum;
                                if (!introductions.HasFlag(IntroductoryEmailEnum.WelcomeAndAddMembers) &&
                                    league.Created < DateTime.UtcNow.AddDays(-3) &&
                                    league.Members.Count < 12)
                                {
                                    var mems = leagueTemp.LeagueMembers.Where(x => x.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner) || x.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager)).ToList();
                                    foreach (var mem in mems)
                                    {
                                        try
                                        {
                                            var user = System.Web.Security.Membership.GetUser((object)mem.UserId);
                                            if (user != null)
                                            {
                                                var emailData = new Dictionary<string, string>
                                        {
                                            { "derbyname",mem.DerbyName},
                                        };
                                                emailsSent += 1;
                                                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, user.UserName, LibraryConfig.DefaultEmailSubject + " Getting Started", emailData, EmailServer.EmailServerLayoutsEnum.RDNWelcomeMessageInviteMembers);
                                            }
                                        }
                                        catch (Exception exception)
                                        {
                                            Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
                                        }
                                    }
                                    introductions |= IntroductoryEmailEnum.WelcomeAndAddMembers;
                                    league.IntroductoryEmailEnum = (int)introductions;
                                }

                            }
                            catch (Exception exception)
                            {
                                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
                            }
                        }


                        int c = dc.SaveChanges();
                    }
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return emailsSent;
        }

        public static int EmailMembersAboutMessagesWaitingInInbox()
        {
            int emailsSent = 0;
            try
            {
                var dc = new ManagementContext();
                int type = Convert.ToInt32(TaskTypeEnum.EmailMembersThatHaveMessagesSittingInInbox);
                var emailTask = dc.AutomatedTasks.Where(x => x.TaskIdForDescription == type).FirstOrDefault();

                if (emailTask == null)
                {
                    TaskForRunning newTask = new TaskForRunning();
                    newTask.FirstRun = DateTime.UtcNow;
                    newTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_FOR_MESSAGES_IN_INBOX;
                    newTask.LastRun = DateTime.UtcNow;
                    newTask.TaskIdForDescription = type;
                    dc.AutomatedTasks.Add(newTask);
                    dc.SaveChanges();
                }
                else
                {
                    emailTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_FOR_MESSAGES_IN_INBOX;
                    if (emailTask.LastRun.AddHours(HOURS_BETWEEN_EMAILS_FOR_MESSAGES_IN_INBOX) < DateTime.UtcNow)
                    {
                        List<Guid> membersToEmail = new List<Guid>();
                        MessageModel mess = new MessageModel();

                        emailTask.LastRun = DateTime.UtcNow;

                        var to = (from xx in dc.MessageInbox.Include("Message").Include("Message.FromUser").Include("ToUser")
                                  where xx.UserNotifiedViaEmail == false
                                  where xx.IsRead == false
                                  select xx).ToList();

                        foreach (var message in to)
                        {
                            message.UserNotifiedViaEmail = true;
                            message.NotifiedEmailDateTime = DateTime.UtcNow;
                            try
                            {
                                MessageSingleModel ms = new MessageSingleModel();
                                ms.FromId = message.Message.FromUser.MemberId;
                                ms.MessageText = RDN.Portable.Util.Strings.StringExt.HtmlDecode(message.Message.MessageText);
                                membersToEmail.Add(message.ToUser.MemberId);
                                if (ms.MessageText.Length > 20)
                                    ms.MessageText = ms.MessageText.Remove(20);
                                var convo = mess.Conversations.Where(x => x.ConversationWithUser == ms.FromId && x.OwnerUserId == message.ToUser.MemberId).FirstOrDefault();
                                if (convo != null)
                                {
                                    convo.Messages.Add(ms);
                                }
                                else
                                {
                                    ConversationModel con = new ConversationModel();
                                    con.ConversationWithUser = ms.FromId;
                                    con.FromName = message.Message.FromUser.DerbyName;
                                    con.OwnerUserId = message.ToUser.MemberId;
                                    con.Messages.Add(ms);
                                    mess.Conversations.Add(con);
                                }
                            }
                            catch (Exception exception)
                            {
                                ErrorDatabaseManager.AddException(exception, exception.GetType());
                            }
                        }
                        dc.SaveChanges();
                        membersToEmail = membersToEmail.Distinct().ToList();

                        var members = dc.Members.Where(x => x.AspNetUserId != null && x.AspNetUserId != new Guid()).Where(x => membersToEmail.Contains(x.MemberId)).ToList();

                        foreach (var mem in members)
                        {
                            string membersWhoSentThisMemberAMessage = String.Empty;
                            foreach (var conv in mess.Conversations.Where(x => x.OwnerUserId == mem.MemberId))
                            {
                                membersWhoSentThisMemberAMessage += conv.FromName;
                                membersWhoSentThisMemberAMessage += "<br/>";
                            }

                            try
                            {
                                var user = System.Web.Security.Membership.GetUser((object)mem.AspNetUserId);
                                var emailData = new Dictionary<string, string>
                                        {
                                            { "derbyname",mem.DerbyName},
                                            { "membersWhoSentMessages", membersWhoSentThisMemberAMessage},
                                            { "viewConversationLink",LibraryConfig.InternalSite + UrlManager.VIEW_MESSAGES_INBOX_MEMBER + mem.MemberId.ToString().Replace("-","") }
                                        };

                                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, user.UserName, LibraryConfig.DefaultEmailSubject + " New Messages", emailData, EmailServer.EmailServerLayoutsEnum.SendLatestConversationsThreadToUser);
                                emailsSent += 1;
                            }
                            catch (Exception exception)
                            {
                                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
                            }
                        }
                        dc.SaveChanges();
                    }
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return emailsSent;
        }
        public static int EmailMembersAboutNewForumContent()
        {
            int emailsSent = 0;
            try
            {
                var dc = new ManagementContext();
                int type = Convert.ToInt32(TaskTypeEnum.EmailMembersAboutNewForumContent);
                var emailTask = dc.AutomatedTasks.Where(x => x.TaskIdForDescription == type).FirstOrDefault();

                if (emailTask == null)
                {
                    TaskForRunning newTask = new TaskForRunning();
                    newTask.FirstRun = DateTime.UtcNow;
                    newTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_FOR_FORUM_MESSAGES_FOR_CONTENT;
                    newTask.LastRun = DateTime.UtcNow;
                    newTask.TaskIdForDescription = type;
                    dc.AutomatedTasks.Add(newTask);
                    dc.SaveChanges();
                }
                else
                {
                    emailTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_FOR_FORUM_MESSAGES_FOR_CONTENT;
                    if (emailTask.LastRun.AddHours(-10) < DateTime.UtcNow)
                    {
                        emailTask.LastRun = DateTime.UtcNow;
                        Guid tempG = new Guid();
                        var to = (from xx in dc.ForumInbox.Where(x => x.UserNotifiedViaEmail == false && x.ToUser.AspNetUserId != tempG)
                                  group xx by xx.ToUser into yy
                                  select new
                                  {
                                      yy.Key,
                                      boxes = yy.Where(x => x.Topic.IsRemoved == false)
                                  });
                        var allGroups = SiteCache.GetAllGroups();
                        //we need to save the changes before we send out the emails.  We were getting modified changes before completing this task.
                        //so someone was posting at the same time this was being sent out.
                        foreach (var message in to)
                        {
                            var groups = message.boxes.GroupBy(x => x.Topic.GroupId);
                            foreach (var group in groups)
                            {
                                foreach (var box in group)
                                {
                                    box.UserNotifiedViaEmail = true;
                                    box.NotifiedEmailDateTime = DateTime.UtcNow;
                                }
                            }
                        }

                        int c = dc.SaveChanges();
                        var tempList = to.ToList();
                        foreach (var message in tempList)
                        {
                            List<Forum.ForumGroup> groupsForUser = new List<Forum.ForumGroup>();
                            var groups = message.boxes.GroupBy(x => x.Topic.GroupId);
                            foreach (var group in groups)
                            {
                                RDN.Library.Classes.Forum.ForumGroup g = new Forum.ForumGroup();
                                g.GroupId = group.Key;
                                if (g.GroupId > 0)
                                {
                                    var tempGroup = allGroups.Where(x => x.Id == group.Key).FirstOrDefault();
                                    g.GroupName = tempGroup.GroupName;
                                }
                                else
                                    g.GroupName = "League";

                                foreach (var box in group)
                                {
                                    if (Forum.Forum.DoesUserBelongToForum(box.ToUser.MemberId, box.Topic.Forum.ForumId))
                                    {
                                        if (box.Topic.GroupId == 0 || (box.Topic.GroupId > 0 && RDN.Library.Classes.League.Classes.LeagueGroupFactory.IsMemberInGroup(box.Topic.GroupId, box.ToUser.MemberId)))
                                        {
                                            string url = LibraryConfig.InternalSite + UrlManager.LEAGUE_FORUM_POSTS_URL + box.Topic.Forum.ForumId.ToString().Replace("-", "") + "/" + box.Topic.TopicId;
                                            g.Topics.Add(new Forum.ForumTopic() { Url = "<a href='" + url + "'>" + box.Topic.TopicTitle + "</a>" });
                                        }
                                    }
                                }
                                groupsForUser.Add(g);
                            }

                            if (message.Key.AspNetUserId != new Guid() && groupsForUser.Count > 0)
                            {
                                var forumTopics = new StringBuilder();
                                foreach (var g in groupsForUser)
                                {
                                    forumTopics.Append("<b>" + g.GroupName + "</b><br/><ul>");
                                    foreach (var conv in g.Topics)
                                    {
                                        forumTopics.Append("<li>" + conv.Url + "</li>");
                                    }
                                    forumTopics.Append("</ul><br/><br/>");
                                }
                                try
                                {
                                    if (message.Key.Notifications == null || !message.Key.Notifications.EmailForumWeeklyRoundupTurnOff)
                                    {
                                        var user = System.Web.Security.Membership.GetUser((object)message.Key.AspNetUserId);
                                        if (user != null)
                                        {
                                            var emailData = new Dictionary<string, string>
                                        {
                                            { "derbyname",message.Key.DerbyName},
                                            { "forumTopics", forumTopics.ToString()},
                                            { "viewConversationLink", LibraryConfig.InternalSite + UrlManager.LEAGUE_FORUM_URL + message.boxes.FirstOrDefault().Topic.Forum.ForumId.ToString().Replace("-", "") }
                                        };
                                            emailsSent += 1;
                                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, user.UserName, LibraryConfig.DefaultEmailSubject + " New Forum Topics", emailData, EmailServer.EmailServerLayoutsEnum.SendLatestForumTopicsToUser);
                                        }
                                    }
                                }
                                catch (Exception exception)
                                {
                                    Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
                                }
                            }
                        }


                    }
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return emailsSent;
        }
        /// <summary>
        /// goes through all the dues collection items.  Generates new dues items based on the dates set by teams to start collecting dues.
        /// </summary>
        public static int CreateNewDuesCollectionDates()
        {
            int emailsSent = 0;
            try
            {
                var dc = new ManagementContext();
                int duesType = Convert.ToInt32(Dues.Enums.FeesTypeEnum.DuesType);
                var getFeeManagements = (from xx in dc.FeeManagement
                                         where xx.FeeTypeEnum == duesType
                                         select xx);
                DateTime now = DateTime.UtcNow;
                DateTime monthAhead = now.AddMonths(1);

                foreach (var fee in getFeeManagements)
                {
                    string emailFromDuesSettings = fee.EmailResponse;

                    if (fee.Created < new DateTime(2013, 11, 23))
                    {
                        RDN.Library.Util.MarkdownSharp.Markdown markdown = new RDN.Library.Util.MarkdownSharp.Markdown();
                        markdown.AutoHyperlink = true;
                        markdown.LinkEmails = true;
                        if (!String.IsNullOrEmpty(fee.EmailResponse))
                        {
                            emailFromDuesSettings = HtmlSanitize.FilterHtmlToWhitelist(markdown.Transform(fee.EmailResponse)).Replace("</p>", "</p><br/>");
                        }
                    }
                    try
                    {
                        if (now.Day == fee.DayOfMonthToCollectDefault)
                        {
                            bool doesFeeExist = false;
                            //checks for the year, month and day of the certain fee to see if its already been added to the fee collection.
                            var feeYear = fee.Fees.Where(x => x.PayBy.Year == monthAhead.Year);
                            if (feeYear != null)
                            {
                                var feeMonth = feeYear.Where(x => x.PayBy.Month == monthAhead.Month);
                                if (feeMonth != null)
                                {
                                    if (feeMonth.Where(x => x.PayBy.Day == monthAhead.Day).FirstOrDefault() != null)
                                    {
                                        //if fee exists, we don't want to add a brand new one.  So we set the flag.
                                        doesFeeExist = true;
                                    }
                                }
                            }
                            //if no fee exists for the next month, we go ahead and add one.
                            if (!doesFeeExist)
                            {
                                Dues.DuesFactory.CreateNewFeeItem(monthAhead, fee);
                            }


                            var duesItems = fee.Fees.Where(x => x.PayBy > now && x.PayBy < monthAhead && x.Notified == false).ToList();
                            foreach (var due in duesItems)
                            {

                                DateTime dateDue = due.PayBy.AddDays(-due.DaysBeforeDeadlineToNotify);
                                if (dateDue.Month == now.Month && dateDue.Day == now.Day && due.Notified == false)
                                {
                                    //email members about payment
                                    var mems = fee.LeagueOwner.Members.Where(x => x.HasLeftLeague == false);
                                    foreach (var member in mems)
                                    {
                                        try
                                        {
                                            string email = string.Empty;
                                            if (member.Member.AspNetUserId != new Guid())
                                                email = System.Web.Security.Membership.GetUser((object)member.Member.AspNetUserId).UserName;
                                            else
                                            {
                                                if (member.Member.ContactCard != null)
                                                    if (member.Member.ContactCard.Emails.Where(x => x.IsDefault).FirstOrDefault() != null)
                                                        email = member.Member.ContactCard.Emails.Where(x => x.IsDefault).FirstOrDefault().EmailAddress;
                                            }
                                            if (!String.IsNullOrEmpty(email))
                                            {

                                                var emailData = new Dictionary<string, string>
                                        {
                                            { "name", member.Member.DerbyName },
                                            { "leaguename", fee.LeagueOwner.Name },
                                            { "duedate", due.PayBy.ToShortDateString() },
                                            { "paymentneeded", due.CostOfFee.ToString("N2")},
                                            { "paymentOnlineText",  Dues.DuesFactory.GeneratePaymentOnlineText(fee.AcceptPaymentsOnline, fee.LeagueOwner.LeagueId)}
                                        };
                                                if (!String.IsNullOrEmpty(fee.EmailResponse))
                                                {
                                                    emailData.Add("blanktext", emailFromDuesSettings);
                                                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email, LibraryConfig.DefaultEmailSubject + " Dues Payment Requested", emailData, EmailServer.EmailServerLayoutsEnum.DuesCollectingNotificationBlank);
                                                    emailsSent += 1;
                                                }
                                                else
                                                {
                                                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email, LibraryConfig.DefaultEmailSubject + " Dues Payment Requested", emailData, EmailServer.EmailServerLayoutsEnum.DuesCollectingNotification);
                                                    emailsSent += 1;
                                                }
                                            }
                                        }
                                        catch (Exception exception)
                                        {
                                            ErrorDatabaseManager.AddException(exception, exception.GetType());
                                        }
                                    }
                                    Dues.DuesFactory.NotifiedMembersOfDuesItem(due);
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return emailsSent;
        }

        public static void ImportRinxterGames()
        {
            try
            {
                var dc = new ManagementContext();
                int type = Convert.ToInt32(TaskTypeEnum.RinxterGamesTournamentsImport);
                var emailTask = dc.AutomatedTasks.Where(x => x.TaskIdForDescription == type).FirstOrDefault();

                if (emailTask == null)
                {
                    TaskForRunning newTask = new TaskForRunning();
                    newTask.FirstRun = DateTime.UtcNow;
                    newTask.HoursBetweenEachRunOfTask = TWO_WEEKS_WORTH_OF_HOURS;
                    newTask.LastRun = DateTime.UtcNow;
                    newTask.TaskIdForDescription = type;
                    dc.AutomatedTasks.Add(newTask);
                    dc.SaveChanges();
                }
                else
                {
                    emailTask.HoursBetweenEachRunOfTask = TWO_WEEKS_WORTH_OF_HOURS;
                    if (emailTask.LastRun.AddHours(TWO_WEEKS_WORTH_OF_HOURS) < DateTime.UtcNow)
                    {

                        emailTask.LastRun = DateTime.UtcNow;

                        try
                        {
                            RinxterImportFactory riW = new RinxterImportFactory();
                            riW.Initialize(FederationsEnum.WFTDA).RunRinxterImports();

                            RinxterImportFactory riM = new RinxterImportFactory();
                            riM.Initialize(FederationsEnum.MRDA).RunRinxterImports();

                        }
                        catch (Exception exception)
                        {
                            Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
                        }
                        dc.SaveChanges();
                    }
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }


        }

        public static void EmailAdminsAboutAutomationWorking(AutomatedTask task)
        {
            try
            {
                var dc = new ManagementContext();
                int type = Convert.ToInt32(TaskTypeEnum.EmailAdminToTellThemAutomationIsWorking);
                var emailTask = dc.AutomatedTasks.Where(x => x.TaskIdForDescription == type).FirstOrDefault();

                if (emailTask == null)
                {
                    TaskForRunning newTask = new TaskForRunning();
                    newTask.FirstRun = DateTime.UtcNow;
                    newTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_FOR_ADMIN;
                    newTask.LastRun = DateTime.UtcNow;
                    newTask.TaskIdForDescription = type;
                    dc.AutomatedTasks.Add(newTask);
                    dc.SaveChanges();
                }
                else
                {
                    emailTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_FOR_ADMIN;
                    if (emailTask.LastRun.AddHours(HOURS_BETWEEN_EMAILS_FOR_ADMIN) < DateTime.UtcNow)
                    {

                        emailTask.LastRun = DateTime.UtcNow;

                        try
                        {
                            var emailData = new Dictionary<string, string> {
                        { "errorCount", dc.ErrorExceptions.Count().ToString()+" "+LibraryConfig.AdminSite+"/Admin/Errors" },
                        { "feedbackCount", dc.ScoreboardFeedback.Count().ToString() +" "+LibraryConfig.AdminSite+"/Admin/Feedback"},
                         { "memberCount", dc.Members.Count().ToString() },
                          { "userCount", dc.Members.Where(x=>x.AspNetUserId != new Guid()).Count().ToString() },
                          { "ownedLeagueCount", dc.Leagues.Where(x=>x.Owners.Count> 0).Count().ToString() },
                          { "leagueCount", dc.Leagues.Count().ToString() },
                          { "unverifiedUsers", dc.EmailVerifications.Count().ToString() },
                          { "ScoreboardDownloads", dc.ScoreboardDownloads.Count().ToString() },
                          { "ScoreboardInstances", dc.ScoreboardInstance.Count().ToString() },
                          { "duesItems",task.duesItemsCreated.ToString() },
                          { "emailsNotFilled",task.emailsNotFilled.ToString() },
                          { "forumContent",task.forumContent.ToString() },
                          { "merchantItems",task.merchantItems.ToString() },
                          { "messageInbox",task.messagesInbox.ToString() },
                          { "subscriptions",task.subscriptionsExpiring.ToString() },
                          { "verificationTable",task.verificationTable.ToString() },
                          { "shopNotPublished",task.storesNotPublished.ToString() },
                          { "ShopWithFewItems",task.storeWithFewItems.ToString() },
                          { "CurrencyUpdated",task.CurrencyUpdated.ToString() },
                        };

                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultAdminEmail, LibraryConfig.DefaultEmailSubject + " Automation Still Working", emailData, layout: EmailServer.EmailServerLayoutsEnum.AutomatedStats, priority: EmailPriority.Normal);
                        }
                        catch (Exception exception)
                        {
                            Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
                        }
                        dc.SaveChanges();
                    }
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }


        }
        public static int EmailMerchantsAboutItemsExpiring()
        {
            int emailsSent = 0;
            try
            {
                var dc = new ManagementContext();
                int type = Convert.ToInt32(TaskTypeEnum.EmailMerchantsAboutItemsExpiring);
                var emailTask = dc.AutomatedTasks.Where(x => x.TaskIdForDescription == type).FirstOrDefault();

                if (emailTask == null)
                {
                    TaskForRunning newTask = new TaskForRunning();
                    newTask.FirstRun = DateTime.UtcNow;
                    newTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_FOR_MERCH_ITEMS_EXPIRING;
                    newTask.LastRun = DateTime.UtcNow;
                    newTask.TaskIdForDescription = type;
                    dc.AutomatedTasks.Add(newTask);
                    dc.SaveChanges();
                }
                else
                {
                    emailTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_FOR_MERCH_ITEMS_EXPIRING;
                    if (emailTask.LastRun.AddHours(HOURS_BETWEEN_EMAILS_FOR_MERCH_ITEMS_EXPIRING) < DateTime.UtcNow)
                    {
                        emailTask.LastRun = DateTime.UtcNow;
                        DateTime ThreeMonthsAgo = DateTime.UtcNow.AddMonths(-3);
                        DateTime FiveMonthsAgo = DateTime.UtcNow.AddMonths(-5);

                        var items = dc.StoreItems.Where(x => x.LastPublished > FiveMonthsAgo && x.LastPublished < ThreeMonthsAgo && x.NotifiedOfExpiration == false && x.IsPublished == true);
                        //giving the merchants 5 days notice so they know the items will resubscribe.
                        DateTime publishedDate = DateTime.UtcNow.AddDays(5).AddMonths(-4);
                        foreach (var item in items)
                        {
                            if (publishedDate.Month == item.LastPublished.Value.Month && publishedDate.Day == item.LastPublished.Value.Day)
                            {
                                try
                                {
                                    string name = String.Empty;
                                    string email = String.Empty;
                                    var league = League.LeagueFactory.GetLeague(item.Merchant.InternalReference);
                                    if (league != null)
                                    {
                                        name = league.Name;
                                        email = league.Email;
                                    }
                                    else
                                    {
                                        var mem = MemberCache.GetMemberDisplay(item.Merchant.InternalReference);
                                        if (mem != null)
                                        {
                                            name = mem.DerbyName;
                                            email = mem.Email;
                                        }
                                    }
                                    if (!String.IsNullOrEmpty(email))
                                    {
                                        var emailData = new Dictionary<string, string> {
                        { "FeeDate", DateTime.UtcNow.AddDays(5).ToLongDateString()},
                        { "FeeCost", "$.50 Cents"},
                         { "ItemName",item.Name },
                          { "name",  name },
                          { "monthOfEachFee", "four" },
                                                };

                                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email, LibraryConfig.DefaultEmailSubject + " Shop Item Expiring Soon", emailData, layout: EmailServer.EmailServerLayoutsEnum.StoreItemExpiringSoon, priority: EmailPriority.Normal);
                                        emailsSent += 1;
                                    }
                                    item.NotifiedOfExpiration = true;
                                    //resets the charge, so in 5 days we can charge the merch for the listing fee.
                                    item.ChargedNewListingFee = false;
                                    item.Merchant = item.Merchant;
                                }
                                catch (Exception exception)
                                {
                                    Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
                                }
                            }
                        }
                        dc.SaveChanges();
                    }
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return emailsSent;
        }
        public static int EmailMerchantsAboutStoreNotPublished()
        {
            int emailsSent = 0;
            try
            {
                var dc = new ManagementContext();
                int type = Convert.ToInt32(TaskTypeEnum.EmailMerchantsAboutStoreClosed);
                var emailTask = dc.AutomatedTasks.Where(x => x.TaskIdForDescription == type).FirstOrDefault();

                if (emailTask == null)
                {
                    TaskForRunning newTask = new TaskForRunning();
                    newTask.FirstRun = DateTime.UtcNow;
                    newTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_FOR_MERCHANT_STORE_NOT_PUBLISHED;
                    newTask.LastRun = DateTime.UtcNow;
                    newTask.TaskIdForDescription = type;
                    dc.AutomatedTasks.Add(newTask);
                    dc.SaveChanges();
                }
                else
                {
                    emailTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_FOR_MERCHANT_STORE_NOT_PUBLISHED;
                    if (emailTask.LastRun.AddHours(HOURS_BETWEEN_EMAILS_FOR_MERCHANT_STORE_NOT_PUBLISHED) < DateTime.UtcNow)
                    {
                        emailTask.LastRun = DateTime.UtcNow;

                        var items = dc.Merchants.Where(x => x.IsPublished == false);

                        foreach (var item in items)
                        {
                            if (!String.IsNullOrEmpty(item.OrderPayedNotificationEmail))
                            {
                                try
                                {
                                    string name = String.Empty;
                                    string email = String.Empty;
                                    var league = League.LeagueFactory.GetLeague(item.InternalReference);
                                    if (league != null)
                                    {
                                        name = league.Name;
                                    }
                                    else
                                    {
                                        var mem = MemberCache.GetMemberDisplay(item.InternalReference);
                                        if (mem != null)
                                        {
                                            name = mem.DerbyName;
                                        }
                                    }
                                    if (!String.IsNullOrEmpty(email))
                                    {
                                        var emailData = new Dictionary<string, string> {
                                                 { "name",  name },
                                                 { "link",  LibraryConfig.InternalSite},
                                                };

                                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, email, LibraryConfig.DefaultEmailSubject + " Your Shop Isn't Published", emailData, layout: EmailServer.EmailServerLayoutsEnum.ShopIsNotPublished, priority: EmailPriority.Normal);
                                        emailsSent += 1;
                                    }
                                }
                                catch (Exception exception)
                                {
                                    Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
                                }
                            }
                        }
                        dc.SaveChanges();
                    }
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return emailsSent;
        }
        public static int EmailMerchantsAboutStoreNoItems()
        {
            int emailsSent = 0;
            try
            {
                int AmountOfItemsNotPublished = 5;
                var dc = new ManagementContext();
                int type = Convert.ToInt32(TaskTypeEnum.EmailMerchantsAboutNoItemsOnStore);
                var emailTask = dc.AutomatedTasks.Where(x => x.TaskIdForDescription == type).FirstOrDefault();

                if (emailTask == null)
                {
                    TaskForRunning newTask = new TaskForRunning();
                    newTask.FirstRun = DateTime.UtcNow;
                    newTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_FOR_MERCHANT_STORE_NOT_PUBLISHED;
                    newTask.LastRun = DateTime.UtcNow;
                    newTask.TaskIdForDescription = type;
                    dc.AutomatedTasks.Add(newTask);
                    dc.SaveChanges();
                }
                else
                {
                    emailTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_FOR_MERCHANT_STORE_NOT_PUBLISHED;
                    if (emailTask.LastRun.AddHours(HOURS_BETWEEN_EMAILS_FOR_MERCHANT_STORE_NOT_PUBLISHED) < DateTime.UtcNow)
                    {
                        emailTask.LastRun = DateTime.UtcNow;

                        var items = dc.Merchants.Where(x => x.IsPublished == true && x.Items.Count < AmountOfItemsNotPublished);

                        foreach (var item in items)
                        {
                            if (!String.IsNullOrEmpty(item.OrderPayedNotificationEmail))
                            {
                                try
                                {
                                    string name = String.Empty;
                                    string email = String.Empty;
                                    var league = League.LeagueFactory.GetLeague(item.InternalReference);
                                    if (league != null)
                                    {
                                        name = league.Name;
                                    }
                                    else
                                    {
                                        var mem = MemberCache.GetMemberDisplay(item.InternalReference);
                                        if (mem != null)
                                        {
                                            name = mem.DerbyName;
                                        }
                                    }
                                    if (!String.IsNullOrEmpty(email))
                                    {
                                        var emailData = new Dictionary<string, string> {
                                                 { "name",  name },
                                                 { "link", LibraryConfig.ShopSite + UrlManager.STORE_MERCHANT_SHOP_URL + item.MerchantId.ToString().Replace("-","") +"/"+RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly( item.ShopName)},
                                                 { "itemsCount",  AmountOfItemsNotPublished.ToString() }
                                                };

                                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, email, LibraryConfig.DefaultEmailSubject + " Your Shop Isn't Full", emailData, layout: EmailServer.EmailServerLayoutsEnum.ShopHasNoItems, priority: EmailPriority.Normal);
                                        emailsSent += 1;
                                    }
                                }
                                catch (Exception exception)
                                {
                                    Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
                                }
                            }
                        }
                        dc.SaveChanges();
                    }
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return emailsSent;
        }

        public static void ChargeMerchantsNewListingFees()
        {
            try
            {
                var dc = new ManagementContext();
                int type = Convert.ToInt32(TaskTypeEnum.ChargeMerchantsForItemsExpiring);
                var emailTask = dc.AutomatedTasks.Where(x => x.TaskIdForDescription == type).FirstOrDefault();

                if (emailTask == null)
                {
                    TaskForRunning newTask = new TaskForRunning();
                    newTask.FirstRun = DateTime.UtcNow;
                    newTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_FOR_MERCH_ITEMS_EXPIRING;
                    newTask.LastRun = DateTime.UtcNow;
                    newTask.TaskIdForDescription = type;
                    dc.AutomatedTasks.Add(newTask);
                    dc.SaveChanges();
                }
                else
                {
                    emailTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_FOR_MERCH_ITEMS_EXPIRING;
                    if (emailTask.LastRun.AddHours(HOURS_BETWEEN_EMAILS_FOR_MERCH_ITEMS_EXPIRING) < DateTime.UtcNow)
                    {
                        emailTask.LastRun = DateTime.UtcNow;
                        DateTime ThreeMonthsAgo = DateTime.UtcNow.AddMonths(-3);
                        DateTime FiveMonthsAgo = DateTime.UtcNow.AddMonths(-5);

                        var items = dc.StoreItems.Where(x => x.LastPublished > FiveMonthsAgo && x.LastPublished < ThreeMonthsAgo && x.IsPublished == true && x.ChargedNewListingFee == false);
                        //giving the merchants 5 days notice so they know the items will resubscribe.
                        DateTime publishedDate = DateTime.UtcNow.AddMonths(-4);
                        foreach (var item in items)
                        {
                            if (publishedDate.Month == item.LastPublished.Value.Month && publishedDate.Day == item.LastPublished.Value.Day)
                            {
                                try
                                {
                                    item.NotifiedOfExpiration = false;
                                    //resets the charge, so in 5 days we can charge the merch for the listing fee.
                                    item.ChargedNewListingFee = true;
                                    item.LastPublished = DateTime.UtcNow;
                                    item.Merchant = item.Merchant;

                                    StoreGateway.AddSiteFeeToMerchant(item);

                                }
                                catch (Exception exception)
                                {
                                    Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
                                }
                            }
                        }
                        dc.SaveChanges();
                    }
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        public static int ResendAllEmailsStuckInVerificationTable()
        {
            int emailsSent = 0;
            try
            {
                var dc = new ManagementContext();
                int type = Convert.ToInt32(TaskTypeEnum.ResendAllEmailsStuckWaitingToBeVerified);
                var emailTask = dc.AutomatedTasks.Where(x => x.TaskIdForDescription == type).FirstOrDefault();

                if (emailTask == null)
                {
                    TaskForRunning newTask = new TaskForRunning();
                    newTask.FirstRun = DateTime.UtcNow;
                    newTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_FOR_NON_VERIFIED_USERS;
                    newTask.LastRun = DateTime.UtcNow;
                    newTask.TaskIdForDescription = type;
                    dc.AutomatedTasks.Add(newTask);
                    dc.SaveChanges();
                }
                else
                {
                    emailTask.HoursBetweenEachRunOfTask = HOURS_BETWEEN_EMAILS_FOR_NON_VERIFIED_USERS;
                    if (emailTask.LastRun.AddHours(HOURS_BETWEEN_EMAILS_FOR_NON_VERIFIED_USERS) < DateTime.UtcNow)
                    {

                        emailTask.LastRun = DateTime.UtcNow;
                        dc.SaveChanges();
                        try
                        {
                            emailsSent = RDN.Library.Classes.Account.User.ResendAllEmailVerificationsInQueue();
                        }
                        catch (Exception exception)
                        {
                            Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return emailsSent;
        }

    }
}
