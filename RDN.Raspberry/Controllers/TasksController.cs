using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.AutomatedTask;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Location;
using RDN.Library.Classes.Payment.Money;
using RDN.Library.Classes.EmailServer;
using RDN.Portable.Config;
using RDN.Library.Classes.Imports.Rinxter;
using RDN.Library.Classes.Federation.Enums;
using System.Net;
using RDN.Library.Classes.Messages;
using RDN.Library.Classes.Forum;

namespace RDN.Raspberry.Controllers
{
    public class TasksController : Controller
    {

        public ActionResult DoUpgrade()
        {
            return Json(new { result = Forum.UpdateForumPostCount() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RunAutomatedTasks()
        {
            AutomatedTask task = new AutomatedTask();
            try
            {
                //task.massRollinNewsPaymentsProcessed = AutomatedTask.ProcessRollinNewsMassPayments();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            try
            {
                //Commented By Khalid:on 10.01.2014 mm.dd.yyyy.In my solution the method does not exist.
                AutomatedTask.SetLeagueOfTheWeek();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            try
            {
                //Commented By Khalid:on 10.01.2014 mm.dd.yyyy.In my solution the method does not exist.
                AutomatedTask.SetSkaterOfTheWeek();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            try
            {
                task.emailsNotFilled = AutomatedTask.EmailNotFilledOutProfile();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            try
            {
                task.messagesInbox = AutomatedTask.EmailMembersAboutMessagesWaitingInInbox();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            try
            {
                AutomatedTask.EmailLeaguesWelcomeEmailAfterJoining();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            try
            {
                AutomatedTask.EmailAboutReviewingProductBought();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            try
            {
                task.forumContent = AutomatedTask.EmailMembersAboutNewForumContent();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            try
            {
                task.duesItemsCreated = AutomatedTask.CreateNewDuesCollectionDates();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            try
            {
                task.verificationTable = AutomatedTask.ResendAllEmailsStuckInVerificationTable();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            try
            {
                task.verificationTable = AutomatedTask.TextMessageAdminsToShowSMSIsWorking();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            try
            {
                task.merchantItems = AutomatedTask.EmailMerchantsAboutItemsExpiring();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            try
            {
                task.storesNotPublished = AutomatedTask.EmailMerchantsAboutStoreNotPublished();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            try
            {
                task.storeWithFewItems = AutomatedTask.EmailMerchantsAboutStoreNotPublished();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            try
            {
                //AutomatedTask.ChargeMerchantsNewListingFees();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            try
            {
                task.subscriptionsExpiring = AutomatedTask.EmailLeaguesAboutSubscriptionsExpiring();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            try
            {
                task.CurrencyUpdated = AutomatedTask.UpdateCurrencyExchangeRates();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            try
            {
                AutomatedTask.ImportRinxterGames();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            try
            {
                AutomatedTask.EmailAdminsAboutAutomationWorking(task);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            WebClient wc = new WebClient();
            wc.DownloadString("http://postsecretcollection.com/DownloadNewSecrets123USA");

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

    }
}
