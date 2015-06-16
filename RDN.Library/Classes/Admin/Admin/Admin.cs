using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RDN.Library.Classes.Admin.Admin.Classes;
using RDN.Library.Classes.Admin.Admin.Enums;
using RDN.Library.DataModels.Admin.Download;
using RDN.Library.DataModels.Admin.LeagueContacts;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.EmailServer;
using RDN.Library.DataModels.EmailServer.Enums;
using RDN.Library.DataModels.Location;
using RDN.Library.Util;
using RDN.Utilities.Config;
using ContactLeague = RDN.Library.Classes.Admin.Admin.Classes.ContactLeague;
using System.Net.Mail;
using RDN.Library.Classes.Error;
using RDN.Utilities.Error;
using System.Web.Security;
using RDN.Portable.Config;
using RDN.Library.DataModels.Admin.Email;
using Common.EmailServer.Library.Classes.Email;
using RDN.Library.Classes.Config;


namespace RDN.Library.Classes.Admin.Admin
{
    public static class Admin
    {
        public static bool SaveNextAdminMessage(string message)
        {
            EmailServerManager email = new EmailServerManager();
            return email.SaveNextAdminMessage(message);
        }
        public static List<Common.EmailServer.Library.Database.Emails.AdminEmailMessage> GetLastAdminEmailMessages(int count)
        {
            EmailServerManager email = new EmailServerManager();
            return email.GetLastAdminEmailMessages(count);
        }

        public static int GetContactLeagueCount()
        {
            var dc = new ManagementContext();
            return dc.ContactLeagues.Count();
        }

        public static void DeleteContactLeague(int itemToDelete)
        {
            var dc = new ManagementContext();
            var item = dc.ContactLeagues.FirstOrDefault(x => x.LeagueId.Equals(itemToDelete));
            if (item == null)
                return;
            dc.ContactLeagues.Remove(item);
            dc.SaveChanges();
        }

        public static List<ContactLeague> GetContactLeague(int recordsToSkip, int numberOfRecordsToPull)
        {
            var dc = new ManagementContext();
            var leagues = dc.ContactLeagues.Include("Country").Include("LeagueAddresses").Include("LeagueAssociation").Include("LeagueType").OrderBy(x => x.Country.Name).OrderBy(x => x.Name).Skip(recordsToSkip).Take(numberOfRecordsToPull).ToList();
            return leagues.Select(league => new ContactLeague
                                                {
                                                    Addresses = league.LeagueAddresses.Select(x => new ContactLeagueAddress { Email = x.EmailAddress, IsMain = x.IsMain }).ToList(),
                                                    Association = string.Format("({0}) {1}", league.LeagueAssociation.Short, league.LeagueAssociation.Name),
                                                    City = league.City,
                                                    Country = league.Country.Name,
                                                    Created = league.Created,
                                                    Facebook = league.Facebook,
                                                    HomePage = league.HomePage,
                                                    LeagueId = league.LeagueId,
                                                    LeagueType = string.Format("({0}) {1}", league.LeagueType.Short, league.LeagueType.Name),
                                                    Name = league.Name,
                                                    Comment = league.Comments
                                                }).ToList();
        }

        public static CreateLeagueContactEnum CreateContactLeague(string name, string associationIdRaw, string leagueTypeIdRaw, string countryIdRaw, string state, string city, string homePage, string facebook, string primaryEmails, string emails, string comments)
        {
            var dc = new ManagementContext();

            var countryId = Int32.Parse(countryIdRaw);
            var country = dc.Countries.FirstOrDefault(x => x.CountryId.Equals(countryId));
            var associationId = Int32.Parse(associationIdRaw);
            var association = dc.ContactLeagueAssociations.FirstOrDefault(x => x.LeagueAssociationId.Equals(associationId));
            var leagueTypeId = Int32.Parse(leagueTypeIdRaw);
            var type = dc.ContactLeagueTypes.FirstOrDefault(x => x.LeagueTypeId.Equals(leagueTypeId));

            var contactLeague = new Library.DataModels.Admin.LeagueContacts.ContactLeague
                                    {
                                        City = city,
                                        Comments = comments,
                                        Country = country,
                                        Facebook = facebook,
                                        HomePage = homePage,
                                        Name = name,
                                        LeagueAssociation = association,
                                        LeagueType = type,
                                        State = state
                                    };

            if (!string.IsNullOrEmpty(primaryEmails))
            {
                var primaryEmailsList = primaryEmails.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var email in primaryEmailsList)
                {
                    var emailToBeInserted = email.Trim();

                    if (String.IsNullOrEmpty(emailToBeInserted)) continue;
                    if (!Utilities.EmailValidator.Validate(emailToBeInserted))
                        return CreateLeagueContactEnum.InvalidEmailAddress;

                    contactLeague.LeagueAddresses.Add(new LeagueAddress
                    {
                        EmailAddress = emailToBeInserted,
                        IsMain = true
                    });
                }
            }

            if (!string.IsNullOrEmpty(emails))
            {
                var emailsList = emails.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var email in emailsList)
                {
                    var emailToBeInserted = email.Trim();

                    if (String.IsNullOrEmpty(emailToBeInserted)) continue;
                    if (!Utilities.EmailValidator.Validate(emailToBeInserted))
                        return CreateLeagueContactEnum.InvalidEmailAddress;

                    contactLeague.LeagueAddresses.Add(new LeagueAddress
                    {
                        EmailAddress = emailToBeInserted,
                        IsMain = false
                    });
                }
            }

            dc.ContactLeagues.Add(contactLeague);
            var result = dc.SaveChanges();

            return result > 0 ? CreateLeagueContactEnum.Saved : CreateLeagueContactEnum.Error;
        }

        public static Dictionary<int, string> GetLeagueAssociations()
        {
            var dc = new ManagementContext();
            return dc.ContactLeagueAssociations.ToDictionary(key => key.LeagueAssociationId,
                                                             value =>
                                                             string.Format("({0}) {1}", value.Short, value.Name));
        }

        public static Dictionary<int, string> GetLeagueTypes()
        {
            var dc = new ManagementContext();
            return dc.ContactLeagueTypes.ToDictionary(key => key.LeagueTypeId,
                                                             value =>
                                                             string.Format("({0}) {1}", value.Short, value.Name));
        }
        /// <summary>
        /// inserts a download in the DB.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="email"></param>
        /// <param name="ip"></param>
        /// <param name="httpRaw"></param>
        public static void ScoreboardDownloadClick(string version, string email, string ip, string httpRaw)
        {
            try
            {
                var dc = new ManagementContext();
                var scoreboardDownload = new ScoreboardDownload();
                if (Utilities.EmailValidator.Validate(email))
                    scoreboardDownload.Email = email;

                scoreboardDownload.IP = ip;
                scoreboardDownload.HttpRaw = httpRaw;
                scoreboardDownload.Version = version;
                dc.ScoreboardDownloads.Add(scoreboardDownload);
                dc.SaveChanges();
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, e.GetType(), errorGroup: ErrorGroupEnum.Database);
            }
        }



        public static bool SendMassScoreboardEmailsForLeaguesWorldWide(bool isMassSendVerified, string subject, string body, string testEmail)
        {
            if (isMassSendVerified)
            {
                var dc = new ManagementContext();
                var UnSubscribedEmails = dc.NonSubscribersList.Select(x => x.EmailToRemoveFromList).ToList();
                var emails = dc.ContactLeagueAddresses.Where(x => x.IsMain.Equals(true)).Select(x => new { x.EmailAddress });
                foreach (var email in emails)
                {
                    if (!UnSubscribedEmails.Contains(email.EmailAddress))
                    {
                        if (Utilities.EmailValidator.Validate(email.EmailAddress))
                        {
                            var emailData = new Dictionary<string, string> { { "body", body } };

                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email.EmailAddress, EmailServer.EmailServer.DEFAULT_SUBJECT + " " + subject, emailData, layout: EmailServer.EmailServerLayoutsEnum.Blank, priority: EmailPriority.Normal);
                        }
                    }
                }
                return true;
            }
            else
            {
                var emailData = new Dictionary<string, string> { { "body", body } };

                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, testEmail, EmailServer.EmailServer.DEFAULT_SUBJECT + " " + subject, emailData, layout: EmailServer.EmailServerLayoutsEnum.Blank, priority: EmailPriority.Normal);
                return true;
            }
        }

        public static bool SendMassEmailsForMonthlyBroadcasts(string subject, string body, string testEmail)
        {
            try
            {
                var dc = new ManagementContext();
                var UnSubscribedEmails = dc.NonSubscribersList.Select(x => x.EmailToRemoveFromList.ToLower()).ToList();

                var emails = dc.ScoreboardFeedback.Where(x => x.Email.Length > 2).Select(x => x.Email.ToLower()).ToList();

                var email10 = dc.SubscribersList.Where(x => x.EmailToAddToList.Length > 2).Select(x => x.EmailToAddToList.ToLower()).ToList();
                emails.AddRange(email10);

                var email2 = dc.ScoreboardDownloads.Where(x => x.Email.Length > 2).Select(x => x.Email.ToLower()).ToList();
                emails.AddRange(email2);

                var leagueEmails = dc.ContactLeagueAddresses.Where(x => x.IsMain.Equals(true)).Select(x => x.EmailAddress.ToLower());
                emails.AddRange(leagueEmails);

                var refRoster = dc.RefRoster.Where(x => x.FacebookLink.Length > 2).Select(x => x.FacebookLink.ToLower()).ToList();
                emails.AddRange(refRoster);

                MembershipUserCollection users = System.Web.Security.Membership.GetAllUsers();
                foreach (MembershipUser user in users)
                    if (!String.IsNullOrEmpty(user.Email))
                        emails.Add(user.Email.ToLower());

                var emailsRegLeagues = dc.Leagues.Include("ContactCard").Include("ContactCard.Emails").ToList();
                foreach (var lea in emailsRegLeagues)
                {
                    if (lea.ContactCard != null)
                        foreach (var e in lea.ContactCard.Emails)
                            if (!String.IsNullOrEmpty(e.EmailAddress))
                                emails.Add(e.EmailAddress.ToLower());
                }
                var emailsRegMembers = dc.Members.Include("ContactCard").Include("ContactCard.Emails").Where(x => x.Retired == false).ToList();
                foreach (var lea in emailsRegMembers)
                {
                    if (lea.ContactCard != null)
                        foreach (var e in lea.ContactCard.Emails)
                            if (!String.IsNullOrEmpty(e.EmailAddress))
                                emails.Add(e.EmailAddress.ToLower());
                }
                var invoiceAddresses = dc.InvoiceAddresses.Select(x => x.Email.ToLower()).ToList();
                emails.AddRange(invoiceAddresses);

                emails = emails.Distinct().ToList();
                for (int i = 0; i < emails.Count; i++)
                {
                    try
                    {

                        if (!UnSubscribedEmails.Contains(emails[i].Trim()))
                        {
                            if (Utilities.EmailValidator.Validate(emails[i]))
                            {
                                var emailData = new Dictionary<string, string> { { "body", body } };

                                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, emails[i], EmailServer.EmailServer.DEFAULT_SUBJECT + " " + subject, emailData, layout: EmailServer.EmailServerLayoutsEnum.Blank, priority: EmailPriority.Normal);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ErrorDatabaseManager.AddException(e, e.GetType(), errorGroup: ErrorGroupEnum.Database, additionalInformation: emails[i] + ":" + i);
                    }

                }
                return true;
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, e.GetType(), errorGroup: ErrorGroupEnum.Database);
            }
            return false;
        }

        public static bool SendMassEmailsForLeaguesNotSignedUpToRDNation(string subject, string body, string testEmail)
        {
            var dc = new ManagementContext();
            var UnSubscribedEmails = dc.NonSubscribersList.Select(x => x.EmailToRemoveFromList).ToList();
            var emails = dc.ContactLeagueAddresses.Where(x => x.IsMain.Equals(true)).Select(x => new { x.EmailAddress });
            List<string> ems = new List<string>();
            var leaguesSignedUp = dc.Leagues.Include("ContactCard").Include("ContactCard.Emails").ToList();
            foreach (var lea in leaguesSignedUp)
            {
                if (lea.ContactCard != null)
                {
                    if (lea.ContactCard.Emails.Count > 0)
                    {
                        if (lea.ContactCard.Emails.FirstOrDefault() != null)
                        {
                            var ema = lea.ContactCard.Emails.FirstOrDefault().EmailAddress;
                            ems.Add(ema);
                        }
                    }
                }
            }
            foreach (var email in emails)
            {
                if (!UnSubscribedEmails.Contains(email.EmailAddress) && !ems.Contains(email.EmailAddress))
                {
                    if (Utilities.EmailValidator.Validate(email.EmailAddress))
                    {
                        var emailData = new Dictionary<string, string> { { "body", body } };

                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email.EmailAddress, EmailServer.EmailServer.DEFAULT_SUBJECT + " " + subject, emailData, layout: EmailServer.EmailServerLayoutsEnum.Blank, priority: EmailPriority.Normal);
                    }
                }
            }
            return true;
        }


        public static bool SendTestEmail(string subject, string body, string testEmail)
        {

            var emailData = new Dictionary<string, string> { { "body", body } };

            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, testEmail, subject, emailData, layout: EmailServer.EmailServerLayoutsEnum.Blank, priority: EmailPriority.Normal);
            return true;

        }

        public static bool SendMassEmailsToAllContacts(bool isMassSendVerified, string subject, string body, string testEmail)
        {
            if (isMassSendVerified)
            {
                var dc = new ManagementContext();
                var UnSubscribedEmails = dc.NonSubscribersList.Select(x => x.EmailToRemoveFromList).ToList();

                var emails = dc.ScoreboardFeedback.Where(x => x.Email.Length > 2).Select(x => x.Email).ToList();
                var email2 = dc.ScoreboardDownloads.Where(x => x.Email.Length > 2).Select(x => x.Email).ToList();
                emails.AddRange(email2);
                var emails3 = dc.RefRoster.Where(x => x.FacebookLink.Length > 2).Select(x => x.FacebookLink).ToList();
                emails.AddRange(emails3);
                MembershipUserCollection users = System.Web.Security.Membership.GetAllUsers();
                foreach (MembershipUser user in users)
                    emails.Add(user.Email);
                var emails4 = dc.ContactLeagueAddresses.Where(x => x.IsMain.Equals(true)).Select(x => x.EmailAddress).ToList();
                emails.AddRange(emails4);

                var emails5 = dc.EmailsForAllEntities.Select(x => x.EmailAddress).ToList();
                emails.AddRange(emails5);
                emails = emails.Distinct().ToList();
                foreach (var email in emails)
                {
                    if (!UnSubscribedEmails.Contains(email))
                    {
                        if (Utilities.EmailValidator.Validate(email))
                        {
                            var emailData = new Dictionary<string, string> { { "body", body } };

                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email, EmailServer.EmailServer.DEFAULT_SUBJECT + " " + subject, emailData, layout: EmailServer.EmailServerLayoutsEnum.Blank, priority: EmailPriority.Normal);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public static bool SendMassScoreboardEmailsForFeedback(bool isMassSendVerified, string subject, string body, string testEmail)
        {
            if (isMassSendVerified)
            {
                var dc = new ManagementContext();
                var UnSubscribedEmails = dc.NonSubscribersList.Select(x => x.EmailToRemoveFromList).ToList();

                var emails = dc.ScoreboardFeedback.Where(x => x.Email.Length > 2).Select(x => x.Email).ToList();
                var email2 = dc.ScoreboardDownloads.Where(x => x.Email.Length > 2).Select(x => x.Email).ToList();
                emails.AddRange(email2);
                foreach (var email in emails.Distinct())
                {
                    if (!UnSubscribedEmails.Contains(email))
                    {
                        if (Utilities.EmailValidator.Validate(email))
                        {
                            var emailData = new Dictionary<string, string> { { "body", body } };

                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email, EmailServer.EmailServer.DEFAULT_SUBJECT + " " + subject, emailData, layout: EmailServer.EmailServerLayoutsEnum.Blank, priority: EmailPriority.Normal);
                        }
                    }

                }
                return true;
            }
            else
            {
                var emailData = new Dictionary<string, string> { { "body", body } };

                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, testEmail, EmailServer.EmailServer.DEFAULT_SUBJECT + " " + subject, emailData, layout: EmailServer.EmailServerLayoutsEnum.Blank, priority: EmailPriority.Normal);
                return true;
            }
        }
        public static bool SendMassScoreboardEmailsForMasterRefRoster(bool isMassSendVerified, string subject, string body, string testEmail)
        {
            if (isMassSendVerified)
            {
                var dc = new ManagementContext();
                //must use the facebook link because the import killed a column...  Ugh.
                var emails = dc.RefRoster.Where(x => x.FacebookLink.Length > 2).Select(x => x.FacebookLink).ToList();
                var UnSubscribedEmails = dc.NonSubscribersList.Select(x => x.EmailToRemoveFromList).ToList();

                foreach (var email in emails)
                {
                    if (!UnSubscribedEmails.Contains(email))
                    {
                        if (Utilities.EmailValidator.Validate(email))
                        {
                            var emailData = new Dictionary<string, string> { { "body", body } };

                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email, EmailServer.EmailServer.DEFAULT_SUBJECT + " " + subject, emailData, layout: EmailServer.EmailServerLayoutsEnum.Blank, priority: EmailPriority.Normal);
                        }
                    }
                }
                return true;
            }
            return false;
        }
        public static bool SendMassEmailsForAllRegisteredEmails(bool isMassSendVerified, string subject, string body)
        {
            var dc = new ManagementContext();
            var UnSubscribedEmails = dc.NonSubscribersList.Select(x => x.EmailToRemoveFromList).ToList();
            List<string> ems = new List<string>();

            if (isMassSendVerified)
            {
                MembershipUserCollection users = System.Web.Security.Membership.GetAllUsers();
                foreach (MembershipUser email in users)
                {
                    ems.Add(email.Email);
                }
                var emails = dc.Leagues.Include("ContactCard").Include("ContactCard.Emails").ToList();
                foreach (var lea in emails)
                {
                    if (lea.ContactCard != null)
                    {
                        if (lea.ContactCard.Emails.Count > 0)
                        {
                            if (lea.ContactCard.Emails.FirstOrDefault() != null)
                            {
                                var ema = lea.ContactCard.Emails.FirstOrDefault().EmailAddress;
                                ems.Add(ema);
                            }
                        }
                    }
                }
                ems = ems.Distinct().ToList();
                foreach (string email in ems)
                {
                    if (!UnSubscribedEmails.Contains(email))
                    {
                        if (Utilities.EmailValidator.Validate(email))
                        {
                            var emailData = new Dictionary<string, string> { { "body", body } };
                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email, EmailServer.EmailServer.DEFAULT_SUBJECT + " " + subject, emailData, layout: EmailServer.EmailServerLayoutsEnum.Blank, priority: EmailPriority.Normal);
                        }
                    }
                }
                return true;
            }
            return false;
        }
        public static bool SendMassScoreboardEmailsForAllUsers(bool isMassSendVerified, string subject, string body, string testEmail)
        {
            var dc = new ManagementContext();
            var UnSubscribedEmails = dc.NonSubscribersList.Select(x => x.EmailToRemoveFromList).ToList();

            if (isMassSendVerified)
            {
                MembershipUserCollection users = System.Web.Security.Membership.GetAllUsers();
                foreach (MembershipUser email in users)
                {
                    if (!UnSubscribedEmails.Contains(email.Email))
                    {
                        if (Utilities.EmailValidator.Validate(email.Email))
                        {
                            var emailData = new Dictionary<string, string> { { "body", body } };
                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email.Email, EmailServer.EmailServer.DEFAULT_SUBJECT + " " + subject, emailData, layout: EmailServer.EmailServerLayoutsEnum.Blank, priority: EmailPriority.Normal);
                        }
                    }
                }
                return true;
            }
            return false;
        }
        public static bool SendMassEmailsToOwnersOfLeagues(string subject, string body, string testEmail)
        {

            var dc = new ManagementContext();
            var UnSubscribedEmails = dc.NonSubscribersList.Select(x => x.EmailToRemoveFromList).ToList();

            List<string> ems = new List<string>();
            var emails = dc.LeagueOwners.Include("Member.ContactCard").Include("Member.ContactCard.Emails").ToList();
            foreach (var lea in emails)
            {
                if (lea.Member.ContactCard != null)
                {
                    if (lea.Member.ContactCard.Emails.Count > 0)
                    {
                        if (lea.Member.ContactCard.Emails.FirstOrDefault() != null)
                        {
                            var ema = lea.Member.ContactCard.Emails.FirstOrDefault().EmailAddress;
                            ems.Add(ema);
                        }
                    }
                }
            }

            foreach (var email in ems)
            {
                if (!UnSubscribedEmails.Contains(email))
                {
                    if (Utilities.EmailValidator.Validate(email))
                    {
                        var emailData = new Dictionary<string, string> { { "body", body } };

                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email, EmailServer.EmailServer.DEFAULT_SUBJECT + " " + subject, emailData, layout: EmailServer.EmailServerLayoutsEnum.Blank, priority: EmailPriority.Normal);
                    }
                }
            }
            return true;

        }
        public static bool SendMassEmailsToRole(string subject, string body, string testEmail, string roleName)
        {

            var dc = new ManagementContext();
            var UnSubscribedEmails = dc.NonSubscribersList.Select(x => x.EmailToRemoveFromList).ToList();

            var users = Roles.GetUsersInRole(roleName);

            foreach (var email in users)
            {
                if (!UnSubscribedEmails.Contains(email))
                {
                    if (Utilities.EmailValidator.Validate(email))
                    {
                        var emailData = new Dictionary<string, string> { { "body", body } };

                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email, EmailServer.EmailServer.DEFAULT_SUBJECT + " " + subject, emailData, layout: EmailServer.EmailServerLayoutsEnum.Blank, priority: EmailPriority.Normal);
                    }
                }
            }
            return true;

        }
        public static bool SendMassEmailsToRegisteredLeagues(bool isMassSendVerified, string subject, string body, string testEmail)
        {
            if (isMassSendVerified)
            {
                var dc = new ManagementContext();
                var UnSubscribedEmails = dc.NonSubscribersList.Select(x => x.EmailToRemoveFromList).ToList();

                List<string> ems = new List<string>();
                var emails = dc.Leagues.Include("ContactCard").Include("ContactCard.Emails").ToList();
                foreach (var lea in emails)
                {
                    if (lea.ContactCard != null)
                    {
                        if (lea.ContactCard.Emails.Count > 0)
                        {
                            if (lea.ContactCard.Emails.FirstOrDefault() != null)
                            {
                                var ema = lea.ContactCard.Emails.FirstOrDefault().EmailAddress;
                                ems.Add(ema);
                            }
                        }
                    }
                }

                foreach (var email in ems)
                {
                    if (!UnSubscribedEmails.Contains(email))
                    {
                        if (Utilities.EmailValidator.Validate(email))
                        {
                            var emailData = new Dictionary<string, string> { { "body", body } };

                            EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email, EmailServer.EmailServer.DEFAULT_SUBJECT + " " + subject, emailData, layout: EmailServer.EmailServerLayoutsEnum.Blank, priority: EmailPriority.Normal);
                        }
                    }
                }
                return true;
            }
            return false;
        }

    }
}
