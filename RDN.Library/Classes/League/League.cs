using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using RDN.Library.Classes.Account;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Account.Enums;
using RDN.Library.Classes.League.Enums;
using RDN.Library.Classes.Utilities;
using RDN.Library.DataModels.ContactCard;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.League;
using RDN.Utilities.Config;
using Scoreboard.Library.ViewModel;
using System.Collections.ObjectModel;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Team;
using RDN.Library.Classes.Federation.Enums;
using RDN.Library.DataModels.Member;
using RDN.Library.Cache;
using System.IO;
using RDN.Library.Classes.Payment.Classes.Invoice;
using RDN.Library.DataModels.Federation;
using RDN.Library.Classes.League.Classes;
using RDN.Utilities.Strings;

using RDN.Library.Classes.Account.Classes.Json;
using RDN.Library.DataModels.EmailServer.Enums;
using Scoreboard.Library.ViewModel.Members;
using MoreLinq;
using RDN.Library.Classes.League.Classes.Json;
using System.Drawing;
using RDN.Library.Classes.Location;
using RDN.Portable.Config;
using RDN.Portable.Models.Json.Public;
using RDN.Library.Classes.Store;
using System.Globalization;
using RDN.Portable.Classes.League.Classes;
using RDN.Portable.Classes.Federation.Enums;
using RDN.Portable.Classes.Imaging;
using RDN.Portable.Classes.League.Enums;
using RDN.Portable.Classes.Contacts.Enums;
using RDN.Portable.Classes.Contacts;
using RDN.Portable.Classes.Federation;
using RDN.Portable.Classes.Team;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Colors;
using System.Configuration;

namespace RDN.Library.Classes.League
{
    public class LeagueFactory
    {

        public static int DoUpdates()
        {
            try
            {
                int c = 0;
                var dc = new ManagementContext();
                var mems = dc.LeagueMembers.Where(x => x.LeagueOwnersEnum > 0);
                foreach (var mem in mems)
                {
                    mem.LeagueOwnersEnums = (int)mem.LeagueOwnersEnum;
                    if (mem.LeagueOwnersEnum > 0)
                        Console.WriteLine(mem.LeagueOwnersEnum);
                    //leagueMember.LeagueOwnersEnums = (int)owner;
                    mem.League = mem.League;
                    mem.Member = mem.Member;

                }
                c += dc.SaveChanges();


                return c;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }


        public static int EmailAllLeaguesWhomSubscriptionNeedsReviewed()
        {

            int emailsSent = 0;
            try
            {
                int fiveDays = 5;
                int fifhteen = 15;
                int tenDays = 10;
                int thirtyDays = 30;
                int OneHundredDays = 100;

                DateTime now = DateTime.UtcNow;
                var dc = new ManagementContext();
                var leagues = dc.Leagues.Include("ContactCard").ToList();
                foreach (var league in leagues)
                {
                    bool sendEmailExpired = false;
                    bool sendEmailAboutToExpire = false;
                    int daysToExpire = 0;
                    //about to expire in 5 days
                    if (league.SubscriptionPeriodEnds.GetValueOrDefault().ToShortDateString() == now.AddDays(fiveDays).ToShortDateString() && league.SubscriptionWillAutoRenew == false)
                    {
                        sendEmailAboutToExpire = true;
                        daysToExpire = fiveDays;
                    }
                    else if (league.SubscriptionPeriodEnds.GetValueOrDefault().ToShortDateString() == now.AddDays(tenDays).ToShortDateString() && league.SubscriptionWillAutoRenew == false)
                    {//about to expire in ten days
                        sendEmailAboutToExpire = true;
                        daysToExpire = tenDays;
                    }
                    else if (league.SubscriptionPeriodEnds.GetValueOrDefault().ToShortDateString() == now.AddDays(fifhteen).ToShortDateString() && league.SubscriptionWillAutoRenew == false)
                    {//about to expire in 15 days
                        sendEmailAboutToExpire = true;
                        daysToExpire = fifhteen;
                    }
                    else if (league.SubscriptionPeriodEnds.GetValueOrDefault().ToShortDateString() == now.ToShortDateString() && league.SubscriptionWillAutoRenew == false)
                    { //will expire today
                        sendEmailAboutToExpire = true;
                        daysToExpire = 1;
                    }
                    if (league.SubscriptionPeriodEnds.GetValueOrDefault().ToShortDateString() == now.AddDays(-fiveDays).ToShortDateString())
                    {//subscrip expired 5 days ago
                        sendEmailExpired = true;
                        daysToExpire = fiveDays;
                    }
                    else if (league.SubscriptionPeriodEnds.GetValueOrDefault().ToShortDateString() == now.AddDays(-fifhteen).ToShortDateString())
                    {//subscription expired 15 days ago.
                        sendEmailExpired = true;
                        daysToExpire = fifhteen;
                    }
                    else if (league.SubscriptionPeriodEnds.GetValueOrDefault().ToShortDateString() == now.AddDays(-thirtyDays).ToShortDateString())
                    {//subscription expired 30 days ago.
                        sendEmailExpired = true;
                        daysToExpire = thirtyDays;
                    }
                    else if (league.SubscriptionPeriodEnds.GetValueOrDefault().ToShortDateString() == now.AddDays(-OneHundredDays).ToShortDateString())
                    {//subscription expired 30 days ago.
                        sendEmailExpired = true;
                        daysToExpire = OneHundredDays;
                    }
                    bool sendEmailForSubscriptionAutoRenewing = false;
                    if (league.SubscriptionPeriodEnds.GetValueOrDefault().ToShortDateString() == now.AddDays(fiveDays).ToShortDateString() && league.SubscriptionWillAutoRenew == true)
                    {
                        sendEmailForSubscriptionAutoRenewing = true;
                        daysToExpire = fiveDays;
                    }

                    if (sendEmailExpired)
                    {
                        List<string> emails = GetLeagueEmails(league);
                        string phoneNumber = GetLeaguePhoneNumber(league);

                        var emailData = new Dictionary<string, string>
                                        {
                                            { "days", daysToExpire.ToString() }, 
                                            { "infomormationalVideo", ServerConfig.LEAGUE_SUBSCRIPTION_SERVICES_URL }, 
                                            { "leagueSubscriptionLink", ServerConfig.LEAGUE_SUBSCRIPTION_RESUBSUBSCRIBE + league.LeagueId.ToString().Replace("-","") }
                                                                                   };
                        foreach (var email in emails)
                        {
                            if (email != String.Empty)
                            {
                                EmailServer.EmailServer.SendEmail(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, email, EmailServer.EmailServer.DEFAULT_SUBJECT + " League Subscription Has Expired", emailData, EmailServer.EmailServerLayoutsEnum.SubscriptionHasExpiredTask);
                                emailsSent += 1;
                            }
                        }
                    }
                    else if (sendEmailForSubscriptionAutoRenewing)
                    {
                        //subscription will auto renew in 5 days.
                        List<string> emails = GetLeagueEmails(league);
                        string phoneNumber = GetLeaguePhoneNumber(league);

                        var emailData = new Dictionary<string, string>
                                        {
                                            { "days", daysToExpire.ToString() }, 
                                            { "infomormationalVideo", ServerConfig.LEAGUE_SUBSCRIPTION_SERVICES_URL }, 
                                            { "leagueSubscriptionLink", ServerConfig.LEAGUE_SUBSCRIPTION_RESUBSUBSCRIBE + league.LeagueId.ToString().Replace("-","") }
                                                                                   };
                        foreach (var email in emails)
                        {
                            if (email != String.Empty)
                            {
                                EmailServer.EmailServer.SendEmail(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, email, EmailServer.EmailServer.DEFAULT_SUBJECT + " League Subscription Will AutoRenew", emailData, EmailServer.EmailServerLayoutsEnum.SubscriptionWillAutoRenew);
                                emailsSent += 1;
                            }
                        }
                    }
                    else if (sendEmailAboutToExpire)
                    {
                        List<string> emails = GetLeagueEmails(league);
                        string phoneNumber = GetLeaguePhoneNumber(league);

                        var emailData = new Dictionary<string, string>
                                        {
                                            { "days", daysToExpire.ToString() }, 
                                            { "infomormationalVideo", ServerConfig.LEAGUE_SUBSCRIPTION_SERVICES_URL }, 
                                            { "leagueSubscriptionLink", ServerConfig.LEAGUE_SUBSCRIPTION_RESUBSUBSCRIBE + league.LeagueId.ToString().Replace("-","") }
                                        };
                        foreach (var email in emails)
                        {
                            if (email != String.Empty)
                            {
                                EmailServer.EmailServer.SendEmail(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, email, EmailServer.EmailServer.DEFAULT_SUBJECT + " Expiring League Subscription", emailData, EmailServer.EmailServerLayoutsEnum.SubscriptionIsAboutToRunOutTask);
                                emailsSent += 1;
                            }
                        }

                        var adminEmailData = new Dictionary<string, string>
                                        {
                                            { "days", daysToExpire.ToString() }, 
                                            { "leagueName", league.Name}, 
                                            { "leagueEmail", emails.ToArray().ToString()},
                                            { "leaguePhoneNumber", phoneNumber},
                                            {"link", ServerConfig.LEAGUE_SUBSCRIPTION_LINK_FOR_ADMIN}
                                        };

                        EmailServer.EmailServer.SendEmail(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, EmailServer.EmailServer.DEFAULT_SUBJECT + " Expiring League Subscription", adminEmailData, EmailServer.EmailServerLayoutsEnum.SubscriptionForLeagueExpiringAdmin);
                        EmailServer.EmailServer.SendEmail(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_KRIS_WORLIDGE_EMAIL_ADMIN, EmailServer.EmailServer.DEFAULT_SUBJECT + " Expiring League Subscription", adminEmailData, EmailServer.EmailServerLayoutsEnum.SubscriptionForLeagueExpiringAdmin);

                    }

                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return emailsSent;
        }

        private static string GetLeagueEmail(DataModels.League.League league)
        {
            if (league.ContactCard != null && league.ContactCard.Emails != null && league.ContactCard.Emails.FirstOrDefault() != null)
            {
                string email = league.ContactCard.Emails.Where(x => x.IsDefault == true).FirstOrDefault().EmailAddress;
                if (email != ServerConfig.DEFAULT_EMAIL)
                    return email;
            }
            return String.Empty;
        }
        private static List<string> GetLeagueEmails(DataModels.League.League league)
        {
            List<string> emails = new List<string>();
            if (league.ContactCard.Emails.FirstOrDefault() != null)
            {
                string email = league.ContactCard.Emails.Where(x => x.IsDefault == true).FirstOrDefault().EmailAddress;
                if (email != ServerConfig.DEFAULT_EMAIL)
                    emails.Add(email);
                var owners = GetLeagueOwners(league.LeagueId);
                foreach (var owner in owners)
                {
                    if (!String.IsNullOrEmpty(owner.Email))
                        emails.Add(owner.Email);
                }
            } return emails;
        }
        /// <summary>
        /// gets the leagues phone number with the current league datamodel
        /// </summary>
        /// <param name="league"></param>
        /// <returns></returns>
        private static string GetLeaguePhoneNumber(DataModels.League.League league)
        {
            //int numberType = Convert.ToInt32(CommunicationTypeEnum.PhoneNumber);
            var phone = league.ContactCard.Communications.Where(x => x.CommunicationTypeEnum == (byte)CommunicationTypeEnum.PhoneNumber).FirstOrDefault();
            if (phone != null)
                return phone.Data;
            return String.Empty;
        }

        public static void UpdateLeagueSubscriptionPeriod(DateTime dateToSet, bool doesSubscriptionAutoRenew, Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var dbLeague = dc.Leagues.Where(x => x.LeagueId == leagueId).FirstOrDefault();
                dbLeague.SubscriptionPeriodEnds = dateToSet;
                dbLeague.SubscriptionWillAutoRenew = doesSubscriptionAutoRenew;
                int c = dc.SaveChanges();
                if (c <= 0)
                    ErrorDatabaseManager.AddException(new Exception("Stripe Not Updated Subscription: " + dateToSet.ToString() + ":" + dbLeague.SubscriptionPeriodEnds + ":" + doesSubscriptionAutoRenew + ":" + leagueId), new Exception().GetType());
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        public static bool UpdateLeagueSubscriptionPeriod(DateTime dateToSet, Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var dbLeague = dc.Leagues.Where(x => x.LeagueId == leagueId).FirstOrDefault();
                dbLeague.SubscriptionPeriodEnds = dateToSet;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static DateTime? GetLeagueSubscriptionDate(Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var dbLeague = dc.Leagues.Where(x => x.LeagueId == leagueId).FirstOrDefault();
                return dbLeague.SubscriptionPeriodEnds;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        //[Obsolete]
        //public static void AddTrialSubscriptionsToAllCurrentLeagues()
        //{
        //    try
        //    {
        //        var dc = new ManagementContext();
        //        var dbLeague = dc.Leagues.ToList();
        //        foreach (var league in dbLeague)
        //        {
        //            league.SubscriptionPeriodEnds = DateTime.UtcNow.AddDays(58);
        //        }
        //        dc.SaveChanges();
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }
        //}

        public static void UpdateLeagueForFederation(Guid federationId, RDN.Portable.Classes.League.Classes.League league)
        {
            try
            {
                var dc = new ManagementContext();
                var dbLeague = dc.Leagues.Where(x => x.LeagueId == league.LeagueId).FirstOrDefault();

                if (dbLeague != null)
                {
                    dbLeague.Name = league.Name;
                    var address = dbLeague.ContactCard.Addresses.FirstOrDefault();
                    address.CityRaw = league.City;
                    address.StateRaw = league.State;
                    var country = dc.Countries.Where(x => x.Name == league.Country).FirstOrDefault();
                    if (country != null)
                    {
                        address.Country = country;
                    }
                    dc.SaveChanges();
                }
                var fedLeague = dc.FederationLeagues.Where(x => x.Federation.FederationId == federationId && x.League.LeagueId == league.LeagueId).FirstOrDefault();
                if (fedLeague != null)
                {
                    fedLeague.InternalLeagueIdForFederation = league.InternalFederationIdForLeague;
                    dc.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        /// <summary>
        /// gets the number of members in a league
        /// </summary>
        /// <param name="leagueId"></param>
        /// <returns></returns>
        public static int GetNumberOfMembersInLeague(Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                return dc.LeagueMembers.Where(x => x.League.LeagueId == leagueId && x.HasLeftLeague == false).Count();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }
        public static bool ChangeCurrentLeague(Guid memberId, Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var mem = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                mem.CurrentLeagueId = leagueId;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static List<MemberDisplay> GetLeagueMembersDisplay(Guid leagueId, bool hasLeftLeague = false)
        {
            List<MemberDisplay> membersTemp = new List<MemberDisplay>();
            try
            {
                var dc = new ManagementContext();
                var members = dc.LeagueMembers.Where(x => x.League.LeagueId == leagueId && x.HasLeftLeague == hasLeftLeague).OrderBy(x => x.Member.DerbyName).ToList();

                membersTemp.AddRange(MemberDisplayFactory.IterateMembersForDisplay(members));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return membersTemp;
        }
        public static List<MemberDisplay> GetLeagueMembers(Guid leagueId, int recordsToSkip, int numberOfRecordsToPull)
        {

            List<MemberDisplay> membersTemp = new List<MemberDisplay>();
            try
            {
                var dc = new ManagementContext();
                var members = dc.LeagueMembers.Include("SkaterClass").Where(x => x.League.LeagueId == leagueId && x.HasLeftLeague == false).OrderBy(x => x.Member.DerbyName).Skip(recordsToSkip).Take(numberOfRecordsToPull).ToList();

                membersTemp.AddRange(MemberDisplayFactory.IterateMembersForDisplay(members));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return membersTemp;
        }



        /// <summary>
        /// gets the members of a league.
        /// </summary>
        /// <param name="leagueId"></param>
        /// <returns></returns>
        public static ObservableCollection<TeamMembersViewModel> GetLeagueMembersForGame(Guid leagueId)
        {
            ObservableCollection<TeamMembersViewModel> memberDis = new ObservableCollection<TeamMembersViewModel>();
            try
            {
                var dc = new ManagementContext();
                var members = dc.LeagueMembers.Where(x => x.League.LeagueId == leagueId).ToList();
                foreach (var mem in members)
                {
                    TeamMembersViewModel memDis = new TeamMembersViewModel();
                    memDis.SkaterId = mem.Member.MemberId;
                    memDis.SkaterName = mem.Member.DerbyName;
                    memberDis.Add(memDis);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return memberDis;
        }
        /// <summary>
        /// gets the owners of the league.
        /// </summary>
        /// <param name="leagueId"></param>
        /// <returns></returns>
        public static List<MemberDisplay> GetLeagueOwners(Guid leagueId)
        {
            List<MemberDisplay> memberDis = new List<MemberDisplay>();
            try
            {
                var dc = new ManagementContext();
                var owners = dc.LeagueMembers.Where(x => x.League.LeagueId == leagueId && x.HasLeftLeague == false && x.IsInactiveForLeague == false);
                foreach (var mem in owners)
                {
                    MemberDisplay memDis = new MemberDisplay();
                    memDis.LeagueOwnersEnum = (LeagueOwnersEnum)mem.LeagueOwnersEnums;
                    if (memDis.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Manager) || memDis.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner) || memDis.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Secretary) || memDis.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Treasurer))
                    {
                        memDis.MemberId = mem.Member.MemberId;
                        memDis.DerbyName = mem.Member.DerbyName;
                        memDis.Email = User.ExtractEmailFromContactCard(mem.Member);
                        memberDis.Add(memDis);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return memberDis;
        }

        public static List<LeagueJsonAutoComplete> SearchForLeagueName(string q, int limit)
        {
            List<LeagueJsonAutoComplete> names = new List<LeagueJsonAutoComplete>();
            try
            {
                var dc = new ManagementContext();
                var name = (from xx in dc.Leagues
                            where xx.Name.Contains(q)
                            select new
                            {
                                xx.Name,
                                xx.LeagueId
                            }).Take(limit).ToList();

                for (int i = 0; i < name.Count; i++)
                {
                    LeagueJsonAutoComplete j = new LeagueJsonAutoComplete();
                    j.LeagueName = name[i].Name;
                    j.LeagueId = name[i].LeagueId.ToString().Replace("-", "");
                    names.Add(j);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return names;
        }
        /// <summary>
        /// searches for the league with the name that has no owners..
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<RDN.Portable.Classes.League.Classes.League> SearchForLeagueName(string name)
        {
            List<RDN.Portable.Classes.League.Classes.League> leagues = new List<RDN.Portable.Classes.League.Classes.League>();
            try
            {
                var dc = new ManagementContext();
                var list = dc.Leagues.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
                foreach (var league in list)
                {
                    RDN.Portable.Classes.League.Classes.League basic = new RDN.Portable.Classes.League.Classes.League();
                    basic.LeagueId = league.LeagueId;
                    basic.Name = league.Name;
                    if (league.ContactCard != null && league.ContactCard.Addresses.FirstOrDefault() != null)
                    {
                        basic.City = league.ContactCard.Addresses.FirstOrDefault().CityRaw;
                        basic.State = league.ContactCard.Addresses.FirstOrDefault().StateRaw;
                        if (league.ContactCard.Addresses.FirstOrDefault().Country != null)
                        {
                            basic.Country = league.ContactCard.Addresses.FirstOrDefault().Country.Name;
                            basic.CountryId = league.ContactCard.Addresses.FirstOrDefault().Country.CountryId;
                        }
                    }
                    if (league.Members.Where(x => x.LeagueOwnersEnums > 0).Count() == 0)
                        leagues.Add(basic);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: name);
            }
            return leagues;

        }

        public static Guid GetLeagueAndJoinWithJoinCode(Guid joinCode)
        {
            try
            {
                var dc = new ManagementContext();

                var league = dc.Leagues.Where(x => x.LeagueJoinCode == joinCode).FirstOrDefault();
                if (league == null)
                    return new Guid();

                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var mem = dc.Members.Where(x => x.MemberId == memId).FirstOrDefault();
                var oldMem = league.Members.Where(x => x.Member.MemberId == memId).FirstOrDefault();
                mem.IsNotConnectedToDerby = false;
                //if they are rejoining the league...
                if (oldMem != null)
                {
                    oldMem.Member = oldMem.Member;
                    oldMem.League = oldMem.League;
                    oldMem.HasLeftLeague = false;
                    mem.CurrentLeagueId = oldMem.League.LeagueId;
                }
                else //if they are new to the league.
                {
                    LeagueMember m = new LeagueMember();
                    m.League = league;
                    m.Member = mem;
                    m.MembershipDate = DateTime.UtcNow;
                    mem.CurrentLeagueId = league.LeagueId;
                    mem.Leagues.Add(m);
                    league.Members.Add(m);
                    dc.LeagueMembers.Add(m);
                }
                dc.SaveChanges();


                var owners = LeagueFactory.GetLeagueOwners(league.LeagueId);
                var o = owners.Where(x => x.LeagueOwnersEnum.HasFlag(LeagueOwnersEnum.Owner)).ToList();
                //notify the owners a new member just joined their league.
                o = o.DistinctBy(x => x.Email).ToList();
                foreach (var owner in o)
                {
                    try
                    {
                        var emailData = new Dictionary<string, string> { { "body", mem.DerbyName + " just connected to your league with the join code. They have been updated in your list of members." } };

                        EmailServer.EmailServer.SendEmail(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, owner.Email, EmailServer.EmailServer.DEFAULT_SUBJECT + " " + mem.DerbyName + " joined your league", emailData, layout: EmailServer.EmailServerLayoutsEnum.Blank);
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }

                return league.LeagueId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }

        /// <summary>
        /// get league with the league id.
        /// </summary>
        /// <param name="leagueId"></param>
        /// <returns></returns>
        public static RDN.Portable.Classes.League.Classes.League GetLeague(Guid leagueId, bool isRemovedFromLeague = false)
        {
            try
            {
                var dc = new ManagementContext();
                var league = dc.Leagues.Include("Groups").Include("Federations").Include("Owners").Include("Teams").Include("Members").Include("Members.SkaterClass").Include("ContactCard").Include("Contacts").Where(x => x.LeagueId == leagueId).FirstOrDefault();

                if (league == null)
                    return null;

                return DisplayLeague(league, isRemovedFromLeague);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static RDN.Portable.Classes.League.Classes.League GetPublicLeague(Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var league = dc.Leagues.Include("Groups").Include("Federations").Include("Owners").Include("Teams").Include("Members").Include("Members.SkaterClass").Include("ContactCard").Include("Contacts").Where(x => x.LeagueId == leagueId).FirstOrDefault();

                if (league == null)
                    return null;

                return DisplayLeague(league, true);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static RDN.Portable.Classes.League.Classes.League DisplayLeague(DataModels.League.League league, bool isPublic = false, bool isRemovedFromLeague = false)
        {
            try
            {
                RDN.Portable.Classes.League.Classes.League leagueTemp = new Portable.Classes.League.Classes.League();
                leagueTemp.LeagueId = league.LeagueId;
                leagueTemp.Name = league.Name;
                leagueTemp.ThemeColor = league.ThemeColor;
                //leagues don't put the http in their profile.
                if (!String.IsNullOrEmpty(league.WebSite) && !league.WebSite.Contains("http://"))
                    leagueTemp.Website = "http://" + league.WebSite;
                else
                    leagueTemp.Website = league.WebSite;
                if (!String.IsNullOrEmpty(league.Twitter) && !league.Twitter.Contains("http://"))
                    leagueTemp.Twitter = "http://" + league.Twitter;
                else
                    leagueTemp.Twitter = league.Twitter;
                if (!String.IsNullOrEmpty(league.Instagram) && !league.Instagram.Contains("http://"))
                    leagueTemp.Instagram = "http://" + league.Instagram;
                else
                    leagueTemp.Instagram = league.Instagram;
                if (!String.IsNullOrEmpty(league.Facebook) && !league.Facebook.Contains("http://"))
                    leagueTemp.Facebook = "http://" + league.Facebook;
                else
                    leagueTemp.Facebook = league.Facebook;
                leagueTemp.RuleSetsPlayedEnum = (RuleSetsUsedEnum)league.RuleSetsPlayedEnum;
                leagueTemp.ShopUrl = StoreGateway.GetShopUrl(league.LeagueId);

                leagueTemp.SubscriptionPeriodEnds = league.SubscriptionPeriodEnds.GetValueOrDefault();
                leagueTemp.Groups = LeagueGroupFactory.DisplayGroups(league.Groups.Where(x => x.IsGroupRemoved == false).ToList());
                if (league.Founded.HasValue)
                    leagueTemp.Founded = league.Founded.Value;
                //we use the contact card Id as the join code for "making contact with the league" 
                //instead of creating a new id 
                //and when I don't want to release to use the public LeagueId as the join key.
                leagueTemp.JoinCode = league.LeagueJoinCode;

                if (league.ContactCard != null)
                {
                    if (league.ContactCard.Addresses.FirstOrDefault() != null)
                    {
                        leagueTemp.TimeZone = league.ContactCard.Addresses.FirstOrDefault().TimeZone;
                        leagueTemp.Address = league.ContactCard.Addresses.FirstOrDefault().Address1;
                        leagueTemp.City = league.ContactCard.Addresses.FirstOrDefault().CityRaw;
                        leagueTemp.State = league.ContactCard.Addresses.FirstOrDefault().StateRaw;
                        leagueTemp.ZipCode = league.ContactCard.Addresses.FirstOrDefault().Zip;
                        if (league.ContactCard.Addresses.FirstOrDefault().Country != null)
                        {
                            leagueTemp.Country = league.ContactCard.Addresses.FirstOrDefault().Country.Name;
                            leagueTemp.CountryId = league.ContactCard.Addresses.FirstOrDefault().Country.CountryId;
                        }
                    }
                    if (league.ContactCard.Emails.FirstOrDefault() != null)
                        leagueTemp.Email = league.ContactCard.Emails.Where(x => x.IsDefault == true).FirstOrDefault().EmailAddress;
                    //int numberType = Convert.ToInt32(CommunicationTypeEnum.PhoneNumber);
                    var phone = league.ContactCard.Communications.Where(x => x.CommunicationTypeEnum == (byte)CommunicationTypeEnum.PhoneNumber).FirstOrDefault();
                    if (phone != null)
                        leagueTemp.PhoneNumber = phone.Data;

                }
                if (league.Logo != null)
                    leagueTemp.Logo = new PhotoItem(league.Logo.ImageUrl, league.Logo.IsPrimaryPhoto, league.Logo.AlternativeText);

                if (league.InternalWelcomePicture != null)
                    leagueTemp.InternalWelcomeImage = new PhotoItem(league.InternalWelcomePicture.ImageUrl, league.InternalWelcomePicture.IsPrimaryPhoto, league.InternalWelcomePicture.AlternativeText);

                leagueTemp.InternalWelcomeMessage = league.InternalWelcomeMessage;

                //sets culture as united states english
                if (league.CultureLCID == 0)
                    leagueTemp.CultureSelected = 1033;
                else
                    leagueTemp.CultureSelected = league.CultureLCID;

                if (league.LastModified > new DateTime(2013, 11, 23) || league.Created > new DateTime(2013, 11, 23))
                {
                    leagueTemp.InternalWelcomeMessageHtml = leagueTemp.InternalWelcomeMessage;
                }
                else if (league.Created < new DateTime(2013, 11, 23))
                {
                    RDN.Library.Util.MarkdownSharp.Markdown markdown = new RDN.Library.Util.MarkdownSharp.Markdown();
                    markdown.AutoHyperlink = true;
                    markdown.AutoNewLines = true;
                    markdown.LinkEmails = true;
                    if (!String.IsNullOrEmpty(leagueTemp.InternalWelcomeMessage))
                    {
                        leagueTemp.InternalWelcomeMessageHtml = markdown.Transform(HtmlSanitize.FilterHtmlToWhitelist(leagueTemp.InternalWelcomeMessage));
                        leagueTemp.InternalWelcomeMessageHtml = leagueTemp.InternalWelcomeMessageHtml.Replace("</p>", "</p><br/>");
                    }
                }
                if (league.TimeZoneSelection != null)
                {
                    Portable.Classes.Location.TimeZone tz = new Portable.Classes.Location.TimeZone();
                    tz.ZoneId = league.TimeZoneSelection.ZoneId;
                    tz.Location= league.TimeZoneSelection.Location;
                    tz.GMTOffset = league.TimeZoneSelection.GMTOffset;
                    tz.GMT = league.TimeZoneSelection.GMT;
                    leagueTemp.TimeZoneSelection = tz;
                }

                List<LeagueMember> members = new List<LeagueMember>();
                if (isPublic)
                    members = league.Members.Where(x => x.HasLeftLeague == isRemovedFromLeague && x.Member.IsProfileRemovedFromPublic == false).ToList();
                else
                    members = league.Members.Where(x => x.HasLeftLeague == isRemovedFromLeague).ToList();

                leagueTemp.LeagueMembers.AddRange(MemberDisplayFactory.IterateMembersForDisplay(members).OrderBy(x => x.DerbyName));

                try
                {
                    foreach (var color in league.Colors)
                    {
                        if (color != null)
                        {
                            var c = Color.FromArgb(color.Color.ColorIdCSharp);
                            var hex = ColorTranslator.ToHtml(c);
                            ColorDisplay d = new ColorDisplay();
                            d.ColorId = color.ColorId;
                            d.CSharpColor = color.Color.ColorIdCSharp;
                            d.HexColor = hex;
                            d.NameOfColor = color.Color.ColorName;
                            leagueTemp.ColorTempSelected += d.HexColor + ";";
                            leagueTemp.ColorsSelected += d.HexColor + ";";
                            leagueTemp.Colors.Add(d);
                        }

                    }
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }


                foreach (var team in league.Teams)
                {
                    TeamDisplay teamTemp = new TeamDisplay();
                    teamTemp.TeamName = team.Name;
                    teamTemp.TeamId = team.TeamId;
                    teamTemp.Description = team.Description;
                    leagueTemp.Teams.Add(teamTemp);
                }
                foreach (var con in league.Contacts)
                {
                    try
                    {
                        ContactDisplayBasic c = new ContactDisplayBasic();
                        c.ContactId = con.Contact.ContactId;
                        c.FirstName = con.Contact.Firstname;
                        c.LastName = con.Contact.Lastname;
                        c.ContactTypeForOrg = (ContactTypeForOrganizationEnum)con.ContactTypeEnum;
                        c.ContactTypeSelected = c.ContactTypeForOrg.ToString();
                        if (con.Contact.ContactCard.Addresses.FirstOrDefault() != null)
                        {
                            c.ContactCard = new Portable.Classes.ContactCard.ContactCard();
                            Portable.Classes.ContactCard.Address add = new Portable.Classes.ContactCard.Address();

                            add.Address1 = con.Contact.ContactCard.Addresses.FirstOrDefault().Address1;
                            add.Address2 = con.Contact.ContactCard.Addresses.FirstOrDefault().Address2;
                            add.CityRaw = con.Contact.ContactCard.Addresses.FirstOrDefault().CityRaw;
                            add.Zip = con.Contact.ContactCard.Addresses.FirstOrDefault().Zip;
                            add.StateRaw = con.Contact.ContactCard.Addresses.FirstOrDefault().StateRaw;
                            if (con.Contact.ContactCard.Addresses.FirstOrDefault().Country != null)
                                add.Country = con.Contact.ContactCard.Addresses.FirstOrDefault().Country.Name;
                            c.Address1 = con.Contact.ContactCard.Addresses.FirstOrDefault().Address1;
                            c.Address2 = con.Contact.ContactCard.Addresses.FirstOrDefault().Address2;
                            c.CityRaw = con.Contact.ContactCard.Addresses.FirstOrDefault().CityRaw;
                            c.Zip = con.Contact.ContactCard.Addresses.FirstOrDefault().Zip;
                            c.StateRaw = con.Contact.ContactCard.Addresses.FirstOrDefault().StateRaw;
                            if (con.Contact.ContactCard.Addresses.FirstOrDefault().Country != null)
                            {
                                c.CountryId = con.Contact.ContactCard.Addresses.FirstOrDefault().Country.CountryId;
                                c.CountryName = con.Contact.ContactCard.Addresses.FirstOrDefault().Country.Name;
                            }
                            c.ContactCard.Addresses.Add(add);
                        }
                        if (con.Contact.ContactCard.Emails.FirstOrDefault() != null)
                            c.Email = con.Contact.ContactCard.Emails.FirstOrDefault().EmailAddress;

                        var phone = con.Contact.ContactCard.Communications.OrderByDescending(x => x.Created).FirstOrDefault();
                        if (phone != null)
                            c.PhoneNumber = phone.Data;
                        c.CompanyName = con.Contact.CompanyName;
                        c.Notes = con.Notes;
                        c.Link = con.Contact.Link;
                        leagueTemp.Contacts.Add(c);
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }


                foreach (var federation in league.Federations)
                {
                    FederationDisplay fed = new FederationDisplay();
                    fed.FederationId = federation.Federation.FederationId;
                    fed.FederationName = federation.Federation.Name;
                    fed.MembershipId = federation.InternalLeagueIdForFederation;

                    leagueTemp.Federations.Add(fed);
                }

                return leagueTemp;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }



        public static List<RDN.Portable.Classes.League.Classes.League> IterateThroughLeaguesForDisplay(ICollection<DataModels.League.League> leagues)
        {
            try
            {
                List<RDN.Portable.Classes.League.Classes.League> le = new List<RDN.Portable.Classes.League.Classes.League>();
                foreach (var league in leagues)
                {
                    RDN.Portable.Classes.League.Classes.League leagueTemp = new RDN.Portable.Classes.League.Classes.League();
                    leagueTemp.LeagueId = league.LeagueId;
                    leagueTemp.Name = league.Name;
                    leagueTemp.Website = league.WebSite;
                    leagueTemp.Twitter = league.Twitter;
                    leagueTemp.Instagram = league.Instagram;
                    leagueTemp.Facebook = league.Facebook;
                    //leagueTemp.RuleSetsPlayedEnum = league.RuleSetsPlayedEnum;

                    //we use the contact card Id as the join code for "making contact with the league" 
                    //instead of creating a new id 
                    //and when I don't want to release to use the public LeagueId as the join key.
                    leagueTemp.JoinCode = league.LeagueJoinCode;
                    leagueTemp.SubscriptionPeriodEnds = league.SubscriptionPeriodEnds.GetValueOrDefault();

                    if (league.Founded.HasValue)
                        leagueTemp.Founded = league.Founded.Value;
                    if (league.ContactCard.Addresses.FirstOrDefault() != null)
                    {
                        leagueTemp.TimeZone = league.ContactCard.Addresses.FirstOrDefault().TimeZone;
                        leagueTemp.Address = league.ContactCard.Addresses.FirstOrDefault().Address1;
                        leagueTemp.City = league.ContactCard.Addresses.FirstOrDefault().CityRaw;
                        leagueTemp.State = league.ContactCard.Addresses.FirstOrDefault().StateRaw;
                        if (league.ContactCard.Addresses.FirstOrDefault().Country != null)
                        {
                            leagueTemp.Country = league.ContactCard.Addresses.FirstOrDefault().Country.Name;
                            leagueTemp.CountryId = league.ContactCard.Addresses.FirstOrDefault().Country.CountryId;
                        }
                    }
                    if (league.ContactCard.Emails.FirstOrDefault() != null)
                        leagueTemp.Email = league.ContactCard.Emails.Where(x => x.IsDefault == true).FirstOrDefault().EmailAddress;
                    //int numberType = Convert.ToInt32(CommunicationTypeEnum.PhoneNumber);
                    var phone = league.ContactCard.Communications.Where(x => x.CommunicationTypeEnum == (byte)CommunicationTypeEnum.PhoneNumber).FirstOrDefault();
                    if (phone != null)
                        leagueTemp.PhoneNumber = phone.Data;

                    if (league.Logo != null)
                        leagueTemp.Logo = new PhotoItem(league.Logo.ImageUrl, league.Logo.IsPrimaryPhoto, league.Logo.AlternativeText);

                    if (league.InternalWelcomePicture != null)
                        leagueTemp.InternalWelcomeImage = new PhotoItem(league.InternalWelcomePicture.ImageUrl, league.InternalWelcomePicture.IsPrimaryPhoto, league.InternalWelcomePicture.AlternativeText);

                    leagueTemp.InternalWelcomeMessage = league.InternalWelcomeMessage;

                    var members = league.Members.Where(x => x.HasLeftLeague == false).ToList();

                    leagueTemp.LeagueMembers.AddRange(MemberDisplayFactory.IterateMembersForDisplay(members).OrderBy(x => x.DerbyName));

                    foreach (var team in league.Teams)
                    {
                        TeamDisplay teamTemp = new TeamDisplay();
                        teamTemp.TeamName = team.Name;
                        teamTemp.TeamId = team.TeamId;
                        teamTemp.Description = team.Description;
                        leagueTemp.Teams.Add(teamTemp);
                    }

                    //foreach (var owner in league.Owners)
                    //{
                    //    MemberDisplay mem = new MemberDisplay();
                    //    mem.DerbyName = owner.Member.DerbyName;
                    //    mem.MemberId = owner.Member.MemberId;
                    //    leagueTemp.Owners.Add(mem);
                    //}

                    foreach (var federation in league.Federations)
                    {
                        FederationDisplay fed = new FederationDisplay();
                        fed.FederationId = federation.Federation.FederationId;
                        fed.FederationName = federation.Federation.Name;
                        fed.MembershipId = federation.InternalLeagueIdForFederation;

                        leagueTemp.Federations.Add(fed);
                    }
                    le.Add(leagueTemp);
                }
                return le;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public static RDN.Portable.Classes.League.Classes.League GetLeagueForFederation(Guid federationId, Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var league = dc.FederationLeagues.Include("Federation").Include("League").Include("League.Owners").Include("League.Teams").Include("League.Members").Include("League.ContactCard").Where(x => x.League.LeagueId == leagueId && x.Federation.FederationId == federationId).FirstOrDefault();

                RDN.Portable.Classes.League.Classes.League leagueTemp = new RDN.Portable.Classes.League.Classes.League();
                leagueTemp.LeagueId = league.League.LeagueId;
                leagueTemp.Name = league.League.Name;
                leagueTemp.Website = league.League.WebSite;
                //we use the contact card Id as the join code for "making contact with the league" 
                //instead of creating a new id 
                //and when I don't want to release to use the public LeagueId as the join key.
                leagueTemp.JoinCode = league.League.LeagueJoinCode;
                leagueTemp.SubscriptionPeriodEnds = league.League.SubscriptionPeriodEnds.GetValueOrDefault();

                if (league.League.Founded.HasValue)
                    leagueTemp.Founded = league.League.Founded.Value;
                if (league.League.ContactCard.Addresses.FirstOrDefault() != null)
                {
                    leagueTemp.TimeZone = league.League.ContactCard.Addresses.FirstOrDefault().TimeZone;
                    leagueTemp.Address = league.League.ContactCard.Addresses.FirstOrDefault().Address1;
                    leagueTemp.City = league.League.ContactCard.Addresses.FirstOrDefault().CityRaw;
                    leagueTemp.State = league.League.ContactCard.Addresses.FirstOrDefault().StateRaw;
                    if (league.League.ContactCard.Addresses.FirstOrDefault().Country != null)
                    {
                        leagueTemp.Country = league.League.ContactCard.Addresses.FirstOrDefault().Country.Name;
                        leagueTemp.CountryId = league.League.ContactCard.Addresses.FirstOrDefault().Country.CountryId;
                    }
                }
                if (league.League.ContactCard.Emails.FirstOrDefault() != null)
                    leagueTemp.Email = league.League.ContactCard.Emails.Where(x => x.IsDefault == true).FirstOrDefault().EmailAddress;
                //int numberType = Convert.ToInt32(CommunicationTypeEnum.PhoneNumber);
                var phone = league.League.ContactCard.Communications.Where(x => x.CommunicationTypeEnum == (byte)CommunicationTypeEnum.PhoneNumber).FirstOrDefault();
                if (phone != null)
                    leagueTemp.PhoneNumber = phone.Data;

                if (league.League.Logo != null)
                    leagueTemp.Logo = new PhotoItem(league.League.Logo.ImageUrl, league.League.Logo.IsPrimaryPhoto, league.League.Logo.AlternativeText);

                if (league.League.InternalWelcomePicture != null)
                    leagueTemp.InternalWelcomeImage = new PhotoItem(league.League.InternalWelcomePicture.ImageUrl, league.League.InternalWelcomePicture.IsPrimaryPhoto, league.League.InternalWelcomePicture.AlternativeText);

                leagueTemp.InternalWelcomeMessage = league.League.InternalWelcomeMessage;

                var members = league.League.Members.ToList();

                leagueTemp.LeagueMembers.AddRange(MemberDisplayFactory.IterateMembersForDisplay(members).OrderBy(x => x.DerbyName));

                foreach (var team in league.League.Teams)
                {
                    TeamDisplay teamTemp = new TeamDisplay();
                    teamTemp.TeamName = team.Name;
                    teamTemp.TeamId = team.TeamId;
                    teamTemp.Description = team.Description;
                    leagueTemp.Teams.Add(teamTemp);
                }

                //foreach (var owner in league.League.Owners)
                //{
                //    MemberDisplay mem = new MemberDisplay();
                //    mem.DerbyName = owner.Member.DerbyName;
                //    mem.MemberId = owner.Member.MemberId;
                //    leagueTemp.Owners.Add(mem);
                //}

                FederationDisplay fed = new FederationDisplay();
                fed.FederationId = league.Federation.FederationId;
                fed.FederationName = league.Federation.Name;
                fed.MembershipId = league.InternalLeagueIdForFederation;
                leagueTemp.Federations.Add(fed);
                leagueTemp.InternalFederationIdForLeague = league.InternalLeagueIdForFederation;
                leagueTemp.FederationIdForLeague = league.Federation.FederationId;
                return leagueTemp;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }


        public static RDN.Portable.Classes.League.Classes.League GetLeagueTwoEvils(Guid leagueId)
        {
            var dc = new ManagementContext();
            var league = dc.LeaguesForTwoEvils.Include("Skaters").Where(x => x.TeamId == leagueId).FirstOrDefault();

            RDN.Portable.Classes.League.Classes.League leagueTemp = new RDN.Portable.Classes.League.Classes.League();
            leagueTemp.LeagueId = league.TeamId;
            leagueTemp.Name = league.Name;

            foreach (var skater in league.Skaters)
            {
                MemberDisplay member = new MemberDisplay();
                member.DerbyName = skater.Name;
                member.PlayerNumber = skater.Number;
                member.MemberId = skater.ProfileId;
                leagueTemp.LeagueMembers.Add(member);
            }

            return leagueTemp;
        }

        public static RDN.Portable.Classes.League.Classes.League GetLeaguePublicDerbyRoster(Guid leagueId)
        {
            var dc = new ManagementContext();
            var league = dc.LeaguesForDerbyRoster.Include("Skaters").Where(x => x.TeamId == leagueId).FirstOrDefault();

            RDN.Portable.Classes.League.Classes.League leagueTemp = new RDN.Portable.Classes.League.Classes.League();
            leagueTemp.LeagueId = league.TeamId;
            leagueTemp.Name = league.Name;
            leagueTemp.Country = league.Country;
            leagueTemp.State = league.State;
            leagueTemp.Website = league.WebSite;

            foreach (var skater in league.Skaters)
            {
                MemberDisplay member = new MemberDisplay();
                member.DerbyName = skater.Name;
                member.PlayerNumber = skater.Number;
                member.MemberId = skater.ProfileId;
                leagueTemp.LeagueMembers.Add(member);
            }

            return leagueTemp;
        }


        /// <summary>
        /// stuffs a copy of the league into the teamviewmodel object.
        /// </summary>
        /// <param name="leagueId"></param>
        /// <returns></returns>
        public static TeamViewModel GetLeagueInTeamViewModel(Guid leagueId)
        {
            try
            {
                var league = GetLeague(leagueId);
                if (league != null)
                {
                    TeamViewModel leag = new TeamViewModel();
                    leag.TeamLinkId = league.LeagueId;
                    leag.TeamName = league.Name;

                    foreach (var member in league.LeagueMembers)
                    {
                        TeamMembersViewModel mem = new TeamMembersViewModel();
                        mem.SkaterLinkId = member.MemberId;
                        mem.SkaterName = member.DerbyName;
                        mem.SkaterNumber = member.PlayerNumber;
                        leag.TeamMembers.Add(mem);
                    }
                    return leag;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="federationId"></param>
        /// <returns></returns>
        public static List<Portable.Classes.League.Classes.League> GetLeaguesInFederation(Guid federationId)
        {
            var dc = new ManagementContext();
            var getListOfLeagues = new List<Portable.Classes.League.Classes.League>();
            try
            {
                getListOfLeagues = (from xx in dc.FederationLeagues
                                    where xx.Federation.FederationId == federationId
                                    select new Portable.Classes.League.Classes.League
                                    {
                                        LeagueId = xx.League.LeagueId,
                                        Name = xx.League.Name
                                    }).OrderBy(x => x.Name).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return getListOfLeagues;
        }
        /// <summary>
        /// gets all the leagues in the federation.
        /// </summary>
        /// <param name="federationId"></param>
        /// <returns></returns>
        public static List<RDN.Portable.Classes.League.Classes.League> GetLeaguesInFederation(Guid federationId, int recordsToSkip, int numberOfRecordsToPull)
        {
            var output = new List<RDN.Portable.Classes.League.Classes.League>();

            var dc = new ManagementContext();
            // Get the leagues that matches the partial name, include all contactcard information in the query
            var leagues = dc.FederationLeagues.Include("League.ContactCard").Where(x => x.Federation.FederationId == federationId).OrderBy(x => x.Created).Skip(recordsToSkip).Take(numberOfRecordsToPull).ToList();

            foreach (var league in leagues)
            {
                // Create a league object to be returned
                //TODO: Remove logo path
                var leagueObj = new RDN.Portable.Classes.League.Classes.League { LeagueId = league.League.LeagueId, Name = league.League.Name }; //, LogoPath = league.LogoPath
                leagueObj.InternalFederationIdForLeague = league.InternalLeagueIdForFederation;
                // If contact information is found, add it
                if (league.League.ContactCard != null)
                {
                    var addresses = league.League.ContactCard.Addresses;
                    if (addresses != null && addresses.Count > 0)
                    {
                        if (addresses.Count > 0)
                        {
                            var address = addresses.FirstOrDefault(x => x.IsDefault);
                            if (address == null)
                                address = addresses.First();
                            leagueObj.State = address.StateRaw;
                            if (address.Country != null)
                                leagueObj.Country = address.Country.Name;
                        }
                        else
                        {
                            var address = addresses.First();
                            leagueObj.State = address.StateRaw;
                            leagueObj.Country = address.Country.Name;
                        }
                    }
                }
                output.Add(leagueObj);
            }
            return output;
        }

        public static int GetAllPublicLeaguesCount()
        {
            var dc = new ManagementContext();
            var leagues = dc.Leagues.Where(x => x.IsLeaguePublic == false).Count();
            return leagues;
        }

        public static List<LeagueJsonDataTable> SearchPublicLeagues(string qu, int count, int page)
        {
            List<LeagueJsonDataTable> membersTemp = new List<LeagueJsonDataTable>();
            try
            {
                qu = qu.ToLower();
                var mems = SiteCache.GetAllPublicLeagues();
                return mems.Where(x => x.LeagueName.ToLower().Contains(qu)).AsParallel().OrderBy(x => x.LeagueName).Skip(page * count).Take(count).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return membersTemp;
        }

        public static List<LeagueJsonDataTable> GetAllPublicLeagues()
        {
            var output = new List<LeagueJsonDataTable>();

            var dc = new ManagementContext();
            // Get the leagues that matches the partial name, include all contactcard information in the query
            var leagues = dc.Leagues.Include("Logo").Include("ContactCard").Where(x => x.IsLeaguePublic == false).AsParallel().OrderBy(x => x.Name).ToList();

            foreach (var league in leagues)
            {
                try
                {
                    // Create a league object to be returned
                    //TODO: Remove logo path
                    if (!String.IsNullOrEmpty(league.Name))
                    {
                        var leagueObj = new LeagueJsonDataTable { LeagueName = league.Name }; //, LogoPath = league.LogoPath

                        leagueObj.LeagueUrl = ConfigurationManager.AppSettings["LeagueUrl"] + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(league.Name) + "/" + league.LeagueId.ToString().Replace("-", "");
                        leagueObj.LeagueId = league.LeagueId.ToString().Replace("-", "");
                        leagueObj.Membercount = league.Members.Count;
                        leagueObj.DateFounded = league.Founded.GetValueOrDefault();
                        leagueObj.Twitter = league.Twitter;
                        leagueObj.WebSite = league.WebSite;
                        leagueObj.Instagram = league.Instagram;
                        leagueObj.Facebook = league.Facebook;
                        leagueObj.RuleSetsPlayed = ((RuleSetsUsedEnum)league.RuleSetsPlayedEnum).ToString();


                        // If contact information is found, add it
                        if (league.ContactCard != null)
                        {

                            var addresses = league.ContactCard.Addresses;

                            if (addresses != null && addresses.Count > 0)
                            {
                                if (addresses.Count > 0)
                                {
                                    var address = addresses.FirstOrDefault(x => x.IsDefault);
                                    if (address == null)
                                        address = addresses.First();

                                    leagueObj.State = address.StateRaw;
                                    if (address.Country != null)
                                        leagueObj.Country = address.Country.Name;
                                    leagueObj.City = address.CityRaw;
                                    leagueObj.lat = address.Coords.Latitude;
                                    leagueObj.lon = address.Coords.Longitude;
                                }
                                else
                                {
                                    var address = addresses.First();
                                    leagueObj.State = address.StateRaw;
                                    leagueObj.Country = address.Country.Name;
                                    leagueObj.lat = address.Coords.Latitude;
                                    leagueObj.lon = address.Coords.Longitude;
                                }
                            }
                        }

                        if (league.Logo != null)
                        {
                            leagueObj.LogoUrl = league.Logo.ImageUrl;
                            leagueObj.LogoUrlThumb = league.Logo.ImageUrlThumb;
                        }

                        output.Add(leagueObj);
                    }
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
            }
            return output.OrderBy(x => x.LeagueName).ToList();
        }

        public static List<RDN.Portable.Classes.League.Classes.League> GetAllPublicLeagues(int recordsToSkip, int numberOfRecordsToPull)
        {
            var output = new List<RDN.Portable.Classes.League.Classes.League>();

            var dc = new ManagementContext();
            // Get the leagues that matches the partial name, include all contactcard information in the query
            var leagues = dc.Leagues.Include("ContactCard").Where(x => x.IsLeaguePublic == false).OrderBy(x => x.Name).Skip(recordsToSkip).Take(numberOfRecordsToPull).ToList();

            foreach (var league in leagues)
            {
                // Create a league object to be returned
                //TODO: Remove logo path
                var leagueObj = new RDN.Portable.Classes.League.Classes.League { LeagueId = league.LeagueId, Name = league.Name }; //, LogoPath = league.LogoPath

                // If contact information is found, add it
                if (league.ContactCard != null)
                {
                    var addresses = league.ContactCard.Addresses;
                    if (addresses != null && addresses.Count > 0)
                    {
                        if (addresses.Count > 0)
                        {
                            var address = addresses.FirstOrDefault(x => x.IsDefault);
                            if (address == null)
                                address = addresses.First();
                            leagueObj.State = address.StateRaw;
                            if (address.Country != null)
                                leagueObj.Country = address.Country.Name;
                        }
                        else
                        {
                            var address = addresses.First();
                            leagueObj.State = address.StateRaw;
                            leagueObj.Country = address.Country.Name;
                        }
                    }
                }

                if (league.Logo != null)
                    leagueObj.Logo = new PhotoItem(league.Logo.ImageUrl, league.Logo.IsPrimaryPhoto, league.Logo.AlternativeText);
                output.Add(leagueObj);
            }
            return output.OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Returns a list of the leagues that matches the partialLeagueName
        /// </summary>
        /// <param name="partialLeagueName"></param>
        /// <returns></returns>
        public static List<Classes.LeagueList> GetLeagues(string partialLeagueName)
        {
            var output = new List<Classes.LeagueList>();

            var dc = new ManagementContext();
            // Get the leagues that matches the partial name, include all contactcard information in the query
            var leagues = dc.Leagues.Include("ContactCard").Where(x => x.LoweredName.Contains(partialLeagueName.ToLower()));

            foreach (var league in leagues)
            {
                // Create a league object to be returned
                //TODO: Remove logo path
                var leagueObj = new Classes.LeagueList { LeagueId = league.LeagueId, Name = league.Name }; //, LogoPath = league.LogoPath

                // If contact information is found, add it
                if (league.ContactCard != null)
                {
                    var addresses = league.ContactCard.Addresses;
                    if (addresses != null && addresses.Count > 0)
                    {
                        if (addresses.Count > 0)
                        {
                            var address = addresses.FirstOrDefault(x => x.IsDefault);
                            if (address == null)
                                address = addresses.First();
                            leagueObj.State = address.StateRaw;
                            leagueObj.Country = address.Country.Name;
                        }
                        else
                        {
                            var address = addresses.First();
                            leagueObj.State = address.StateRaw;
                            leagueObj.Country = address.Country.Name;
                        }
                    }
                }
                output.Add(leagueObj);
            }
            return output;
        }

        public static Guid CreateLeagueForImport(string leagueName, string country, string state, string city, double timeZone, string email, string telephoneNumber, Guid federationId)
        {
            int result = 0;
            try
            {
                var dc = new ManagementContext();
                var count = dc.Countries.Where(x => x.CountryId == 0).FirstOrDefault();

                var contactCard = new DataModels.ContactCard.ContactCard();
                var add = new Address
                  {
                      Country = count,
                      StateRaw = state,
                      CityRaw = city,
                      TimeZone = timeZone
                  };
                var coords = OpenStreetMap.FindLatLongOfAddress(add.Address1, add.Address2, add.Zip, add.CityRaw, add.StateRaw, add.Country != null ? add.Country.Name : string.Empty);
                if (coords != null)
                {
                    add.Coords = new System.Device.Location.GeoCoordinate();
                    add.Coords.Latitude = coords.Latitude;
                    add.Coords.Longitude = coords.Longitude;
                    add.Coords.Altitude = 0;
                    add.Coords.Course = 0;
                    add.Coords.HorizontalAccuracy = 1;
                    add.Coords.Speed = 0;
                    add.Coords.VerticalAccuracy = 1;
                }
                contactCard.Addresses.Add(add);


                contactCard.Emails.Add(new RDN.Library.DataModels.ContactCard.Email
                {
                    EmailAddress = email,
                    IsDefault = true
                });


                if (!String.IsNullOrEmpty(telephoneNumber))
                {
                    var communication = new DataModels.ContactCard.Communication();
                    communication.Data = telephoneNumber;
                    communication.IsDefault = true;
                    int comType = Convert.ToInt32(CommunicationTypeEnum.PhoneNumber);
                    communication.CommunicationTypeEnum = (byte)CommunicationTypeEnum.PhoneNumber;
                    contactCard.Communications.Add(communication);
                }

                var league = new DataModels.League.League
                {
                    ContactCard = contactCard,
                    Name = leagueName,
                    LoweredName = leagueName.ToLower()
                };

                //we clear it by hitting a URL setup to clear the cache.
                var fed = dc.Federations.Where(x => x.FederationId == federationId).FirstOrDefault();

                FederationLeague fl = new FederationLeague();
                fl.League = league;
                fl.Federation = fed;
                fed.Leagues.Add(fl);

                //league.Members.Add(pendingLeague.Creator);
                dc.Leagues.Add(league);
                result = dc.SaveChanges();
                //    var result = 1;

                return league.LeagueId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }

        /// <summary>
        /// Adds a league to the league pendings. No physical league is created. This has to be approved by an admin before it's turned into a physical league.
        /// </summary>
        /// <param name="federationIdRaw"></param>
        /// <param name="leagueName"></param>
        /// <param name="contactTelephone"></param>
        /// <param name="contactEmail"></param>
        /// <param name="additionalInformation"></param>
        /// <param name="countryIdRaw"></param>
        /// <param name="state"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        public static List<CreateLeagueEnum> CreateLeague(string federationIdRaw, string leagueName, string contactTelephone, string contactEmail, string additionalInformation, string countryIdRaw, string state, string city, double timeZone)
        {
            int countryId;
            Guid federationId;

            // Parse the ids and check that the required fields have values
            var output = CheckCreateLeague(federationIdRaw, leagueName, contactEmail, countryIdRaw, state, out countryId, out federationId);
            if (output.Count > 0) // Something was not ok, return
                return output;
            try
            {
                // Get the federation and country, if one of them fails, return
                var dc = new ManagementContext();
                var federation = dc.Federations.FirstOrDefault(x => x.FederationId.Equals(federationId));
                //if (federation == null)
                //{
                //    output.Add(CreateLeagueEnum.Federation_Invalid);
                //    return output;
                //}

                var country = dc.Countries.FirstOrDefault(x => x.CountryId.Equals(countryId));
                if (country == null)
                {
                    output.Add(CreateLeagueEnum.Country_Invalid);
                    return output;
                }

                // Get information about possible duplicates, for each league check the addresses to see if it matches addresses of leagues that already exists
                // If the country/state/city matches or close to matches, then it alerts the admin about this. Hopefully this will prevent duplicate leagues from being created.
                var logInformation = string.Empty;
                var league = dc.Leagues.Include("ContactCard").FirstOrDefault(x => x.LoweredName.Contains(leagueName));
                if (league != null)
                {
                    try
                    {
                        var contactCard = league.ContactCard;
                        if (contactCard != null)
                        {
                            var addresses = contactCard.Addresses;
                            var communicationToLeague = GenerateContactInformation(contactCard.Communications.OrderBy(x => x.IsDefault));

                            foreach (var address in addresses)
                            {
                                if (address.Country != null)
                                {
                                    if (address.Country.CountryId.Equals(countryId) && address.StateRaw.Contains(state) && address.CityRaw.Contains(city))
                                        logInformation =
                                            string.Format(
                                                "Strong possible match detected. The submitted league name matches {0}. The country, state and city partially matches: {1}, {2}, {3}. Contact information to this league: {4}",
                                                league.Name, address.Country.Name, address.StateRaw, address.CityRaw, communicationToLeague);
                                    else if (address.Country.CountryId.Equals(countryId))
                                        logInformation =
                                            string.Format(
                                                "Possible match detected. The submitted league name matches {0}. The country partially matches: {1}. Contact information to this league: {2}",
                                                league.Name, address.Country.Name, communicationToLeague);
                                    else if (address.StateRaw.Contains(state))
                                        logInformation =
                                            string.Format(
                                                "Possible match detected. The submitted league name matches {0}. The state partially matches: {1}. Contact information to this league: {2}",
                                                league.Name, address.StateRaw, communicationToLeague);
                                    else if (address.CityRaw.Contains(city))
                                        logInformation =
                                            string.Format(
                                                "Weak possible match detected. The submitted league name matches {0}. The city partially matches: {1}. Contact information to this league: {2}",
                                                league.Name, address.CityRaw, communicationToLeague);
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }

                // Add a new league to the pending leagues
                var pending = new Pending
                                  {
                                      LeagueName = leagueName,
                                      AdditionalInformation = additionalInformation,
                                      ContactEmail = contactEmail,
                                      ContactTelephone = contactTelephone,
                                      Creator = User.GetMember(ref dc),
                                      LogInformation = logInformation,
                                      Country = country,
                                      StateRaw = state,
                                      CityRaw = city,
                                      Federation = federation,
                                      TimeZone = timeZone
                                  };
                dc.LeaguePendings.Add(pending);

                var result = dc.SaveChanges();
                if (result == 0)
                    output.Add(CreateLeagueEnum.Error_Save);

                var emailData = new Dictionary<string, string>
                                {
                                    {"name", leagueName}
                                };

                EmailServer.EmailServer.SendEmail(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_INFO_EMAIL, EmailServer.EmailServer.DEFAULT_SUBJECT + " A league has been created", emailData, layout: EmailServer.EmailServerLayoutsEnum.NewLeagueAdmin);

                // ToDo: To be removed
                //Util.Email.SendEmail(false, ServerConfig.DEFAULT_ADMIN_EMAIL, "ContactLeague creation", "A new league has been created a needs to be verified, league name: " + leagueName);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return output;
        }
        public static List<CreateLeagueEnum> CreateLeagueFromFederation(string federationIdRaw, string leagueName, string contactTelephone, string contactEmail, string additionalInformation, string countryIdRaw, string state, string city)
        {
            int countryId;
            Guid federationId;

            // Parse the ids and check that the required fields have values
            var output = CheckCreateLeague(federationIdRaw, leagueName, contactEmail, countryIdRaw, state, out countryId, out federationId);
            if (output.Count > 0) // Something was not ok, return
                return output;

            // Get the federation and country, if one of them fails, return
            var dc = new ManagementContext();
            var federation = dc.Federations.FirstOrDefault(x => x.FederationId.Equals(federationId));
            if (federation == null)
            {
                output.Add(CreateLeagueEnum.Federation_Invalid);
                return output;
            }

            var country = dc.Countries.FirstOrDefault(x => x.CountryId.Equals(countryId));
            if (country == null)
            {
                output.Add(CreateLeagueEnum.Country_Invalid);
                return output;
            }

            // Get information about possible duplicates, for each league check the addresses to see if it matches addresses of leagues that already exists
            // If the country/state/city matches or close to matches, then it alerts the admin about this. Hopefully this will prevent duplicate leagues from being created.
            var logInformation = string.Empty;
            var league = dc.Leagues.Include("ContactCard").FirstOrDefault(x => x.LoweredName.Contains(leagueName));
            if (league != null)
            {
                var contactCard = league.ContactCard;
                if (contactCard != null)
                {
                    var addresses = contactCard.Addresses;
                    var communicationToLeague = GenerateContactInformation(contactCard.Communications.OrderBy(x => x.IsDefault));

                    foreach (var address in addresses)
                    {
                        if (address.Country.CountryId.Equals(countryId) && address.StateRaw.Contains(state) && address.CityRaw.Contains(city))
                            logInformation =
                                string.Format(
                                    "Strong possible match detected. The submitted league name matches {0}. The country, state and city partially matches: {1}, {2}, {3}. Contact information to this league: {4}",
                                    league.Name, address.Country.Name, address.StateRaw, address.CityRaw, communicationToLeague);
                        else if (address.Country.CountryId.Equals(countryId))
                            logInformation =
                                string.Format(
                                    "Possible match detected. The submitted league name matches {0}. The country partially matches: {1}. Contact information to this league: {2}",
                                    league.Name, address.Country.Name, communicationToLeague);
                        else if (address.StateRaw.Contains(state))
                            logInformation =
                                string.Format(
                                    "Possible match detected. The submitted league name matches {0}. The state partially matches: {1}. Contact information to this league: {2}",
                                    league.Name, address.StateRaw, communicationToLeague);
                        else if (address.CityRaw.Contains(city))
                            logInformation =
                                string.Format(
                                    "Weak possible match detected. The submitted league name matches {0}. The city partially matches: {1}. Contact information to this league: {2}",
                                    league.Name, address.CityRaw, communicationToLeague);
                    }
                }
            }

            // Add a new league to the pending leagues
            var pending = new Pending
            {
                LeagueName = leagueName,
                AdditionalInformation = additionalInformation,
                ContactEmail = contactEmail,
                ContactTelephone = contactTelephone,
                Creator = User.GetMember(ref dc),
                LogInformation = logInformation,
                Country = country,
                StateRaw = state,
                CityRaw = city,
                Federation = federation
            };
            dc.LeaguePendings.Add(pending);

            var result = dc.SaveChanges();


            if (result == 0)
                output.Add(CreateLeagueEnum.Error_Save);

            var emailData = new Dictionary<string, string>
                                {
                                    {"name", leagueName}
                                };

            EmailServer.EmailServer.SendEmail(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_INFO_EMAIL, EmailServer.EmailServer.DEFAULT_SUBJECT + " A league has been created", emailData, layout: EmailServer.EmailServerLayoutsEnum.NewLeagueAdmin);


            // ToDo: To be removed
            //Util.Email.SendEmail(false, ServerConfig.DEFAULT_ADMIN_EMAIL, "ContactLeague creation", "A new league has been created a needs to be verified, league name: " + leagueName);

            return output;
        }

        private static string GenerateContactInformation(IEnumerable<Communication> communications)
        {
            // Generates contact information from the Entity Communication. Db table: RDN_ContactCard_Communications
            var output = new StringBuilder();
            foreach (var item in communications)
            {
                if (item.IsDefault)
                    output.Append("Primary contact:");
                output.AppendLine(string.Format("{0}: {1}", (CommunicationTypeEnum)item.CommunicationTypeEnum, item.Data));
            }
            return output.ToString();
        }

        private static List<CreateLeagueEnum> CheckCreateLeague(string federationIdRaw, string leagueName, string contactEmail, string countryIdRaw, string state, out int countryId, out Guid federationId)
        {
            // Just validators/validations to see that all fields have been filled out correctly
            var output = new List<CreateLeagueEnum>();
            if (String.IsNullOrEmpty(leagueName))
                output.Add(CreateLeagueEnum.Name_Invalid);

            if (!String.IsNullOrEmpty(leagueName) && leagueName.Length < 2)
                output.Add(CreateLeagueEnum.Name_TooShort);
            else if (!String.IsNullOrEmpty(leagueName) && leagueName.Length > 255)
                output.Add(CreateLeagueEnum.Name_Invalid);

            var federationConvert = Guid.TryParse(federationIdRaw, out federationId);
            if (!federationConvert)
                output.Add(CreateLeagueEnum.Federation_Invalid);

            var countryConvert = Int32.TryParse(countryIdRaw, out countryId);
            if (!countryConvert)
                output.Add(CreateLeagueEnum.Country_Invalid);
            else if (countryId < 0)
                output.Add(CreateLeagueEnum.Country_Invalid);


            if (!EmailValidator.Validate(contactEmail))
                output.Add(CreateLeagueEnum.Email_Invalid);
            return output;
        }

        public static List<ViewMember> GetActiveLeagueMembers(Guid leagueId)
        {
            var output = new List<ViewMember>();
            var dc = new ManagementContext();
            var league = dc.Leagues.Include("Members").FirstOrDefault(x => x.LeagueId.Equals(leagueId));
            if (league == null)
                return output;

            var members = league.Members.Where(x => x.IsInactiveForLeague == false && x.HasLeftLeague == false);
            foreach (var member in members)
            {
                var email = member.Member.ContactCard.Emails.FirstOrDefault(x => x.IsDefault);
                if (email == null)
                    output.Add(new ViewMember
                    {
                        MemberId = member.Member.MemberId,
                        Firstname = member.Member.Firstname,
                        DerbyName = member.Member.DerbyName,
                        PlayerNumber = member.Member.PlayerNumber,
                        //Team = team
                    });
                else
                    output.Add(new ViewMember
                    {
                        MemberId = member.Member.MemberId,
                        Firstname = member.Member.Firstname,
                        DerbyName = member.Member.DerbyName,
                        Email = email.EmailAddress,
                        PlayerNumber = member.Member.PlayerNumber,
                        //Team = team
                    });
            }

            return output.OrderBy(x => x.DerbyName).ToList();
        }

        public static List<MemberJsonAutoComplete> SearchLeagueDerbyNames(string q, int limit, Guid leagueId)
        {
            List<MemberJsonAutoComplete> names = new List<MemberJsonAutoComplete>();
            var dc = new ManagementContext();
            var name = (from xx in dc.LeagueMembers
                        where xx.League.LeagueId == leagueId
                        where xx.Member.DerbyName.Contains(q)
                        where xx.HasLeftLeague == false
                        select new
                        {
                            xx.Member.DerbyName,
                            xx.Member.MemberId,
                            xx.Member.PlayerNumber
                        }).Take(limit).ToList();

            for (int i = 0; i < name.Count; i++)
            {
                MemberJsonAutoComplete j = new MemberJsonAutoComplete();
                j.DerbyName = name[i].DerbyName;
                if (!String.IsNullOrEmpty(name[i].PlayerNumber))
                    j.DerbyNumber = name[i].PlayerNumber;
                j.MemberId = name[i].MemberId.ToString().Replace("-", "");
                names.Add(j);
            }
            return names;
        }
        public static List<ViewMember> GetLeagueMembersNotificationSettings(Guid leagueId)
        {
            var output = new List<ViewMember>();
            try
            {
                var dc = new ManagementContext();
                var league = dc.Leagues.Include("Members").FirstOrDefault(x => x.LeagueId.Equals(leagueId));
                if (league == null)
                    return output;
                var members = league.Members.Where(x => x.HasLeftLeague == false);
                foreach (var member in members)
                {
                    var email = member.Member.Notifications;
                    if (email == null)
                        output.Add(new ViewMember
                        {
                            MemberId = member.Member.MemberId,
                            UserId = member.Member.AspNetUserId,
                            Firstname = member.Member.Firstname,
                            DerbyName = member.Member.DerbyName,
                            PlayerNumber = member.Member.PlayerNumber,
                            EmailCalendarNewEventBroadcast = true,
                            EmailForumBroadcasts = true,
                            EmailForumNewPost = true,
                            EmailForumWeeklyRoundup = true,
                            EmailMessagesReceived = true
                            //Team = team
                        });
                    else
                        output.Add(new ViewMember
                        {
                            MemberId = member.Member.MemberId,
                            UserId = member.Member.AspNetUserId,
                            Firstname = member.Member.Firstname,
                            DerbyName = member.Member.DerbyName,
                            PlayerNumber = member.Member.PlayerNumber,
                            EmailCalendarNewEventBroadcast = !email.EmailCalendarNewEventBroadcastTurnOff,
                            EmailForumBroadcasts = !email.EmailForumBroadcastsTurnOff,
                            EmailForumNewPost = !email.EmailForumNewPostTurnOff,
                            EmailForumWeeklyRoundup = !email.EmailForumWeeklyRoundupTurnOff,
                            EmailMessagesReceived = !email.EmailMessagesReceivedTurnOff
                        });
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return output.OrderByDescending(x => x.DerbyName).ToList();
        }

        public static List<ViewMember> GetLeagueMembers(Guid leagueId)
        {
            var output = new List<ViewMember>();
            try
            {
                var dc = new ManagementContext();
                var league = dc.Leagues.Include("Members").FirstOrDefault(x => x.LeagueId.Equals(leagueId));
                if (league == null)
                    return output;
                var members = league.Members.Where(x => x.HasLeftLeague == false);
                foreach (var member in members)
                {
                    var email = member.Member.ContactCard.Emails.FirstOrDefault(x => x.IsDefault);
                    if (email == null)
                        output.Add(new ViewMember
                                       {
                                           MemberId = member.Member.MemberId,
                                           Firstname = member.Member.Firstname,
                                           DerbyName = member.Member.DerbyName,
                                           PlayerNumber = member.Member.PlayerNumber,
                                           //Team = team
                                       });
                    else
                        output.Add(new ViewMember
                        {
                            MemberId = member.Member.MemberId,
                            Firstname = member.Member.Firstname,
                            DerbyName = member.Member.DerbyName,
                            Email = email.EmailAddress,
                            PlayerNumber = member.Member.PlayerNumber,
                            //Team = team
                        });
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return output.OrderBy(x => x.DerbyName).ToList();
        }

        public static List<MemberDisplay> GetLeaguePendingMembers(Guid leagueId)
        {
            var output = new List<MemberDisplay>();
            var dc = new ManagementContext();
            var pendings = dc.LeagueMemberPendings.Include("Member").Include("Member.ContactCard").Where(x => x.League.LeagueId.Equals(leagueId)).ToList();

            foreach (var member in pendings)
            {
                var email = member.Member.ContactCard.Emails.FirstOrDefault(x => x.IsDefault);
                if (email == null)
                    output.Add(new MemberDisplay
                    {
                        MemberId = member.Member.MemberId,
                        Firstname = member.Member.Firstname,
                        DerbyName = member.Member.DerbyName,
                        PlayerNumber = member.Member.PlayerNumber
                    });
                else
                    output.Add(new MemberDisplay
                    {
                        MemberId = member.Member.MemberId,
                        Firstname = member.Member.Firstname,
                        DerbyName = member.Member.DerbyName,
                        Email = email.EmailAddress,
                        PlayerNumber = member.Member.PlayerNumber
                    });
            }

            return output;
        }



        public static bool ApprovePendingMember(string idRaw, Guid leagueId, string ip)
        {
            Guid id;
            var result = Guid.TryParse(idRaw, out id);
            if (!result) return false;

            var dc = new ManagementContext();
            var league = dc.Leagues.FirstOrDefault(x => x.LeagueId.Equals(leagueId));
            if (league == null) return false;

            var pendingMember = league.PendingMembers.FirstOrDefault(x => x.Member.MemberId.Equals(id));
            if (pendingMember == null) return false;

            // Get the member object seperatly since we are about to delete the pending member object
            var member = dc.Members.FirstOrDefault(x => x.MemberId.Equals(id));
            if (member == null) return false;
            LeagueMember m = new LeagueMember();
            m.Member = member;
            m.League = league;
            m.MembershipDate = DateTime.UtcNow;
            member.Leagues.Add(m);

            var leagueName = league.Name;

            dc.LeagueMemberPendings.Remove(pendingMember);

            result = dc.SaveChanges() > 0;

            User.AddMemberLog(id, "Leagion request accepted", string.Format("The league ' {0} ' with id: {1}, accepted the request to join the league.", leagueName, leagueId), MemberLogEnum.SystemStatusChanged, ip);

            return result;
        }

        public static bool RemovePendingMember(string idRaw, Guid leagueId, string ip)
        {
            Guid id;
            var result = Guid.TryParse(idRaw, out id);
            if (!result) return false;

            var dc = new ManagementContext();


            var pendingMember = dc.LeagueMemberPendings.Include("League").FirstOrDefault(x => x.League.LeagueId.Equals(leagueId) && x.Member.MemberId.Equals(id));
            if (pendingMember == null) return false;

            var leagueName = pendingMember.League.Name;
            dc.LeagueMemberPendings.Remove(pendingMember);

            result = dc.SaveChanges() > 0;

            User.AddMemberLog(id, "Leagion request rejected", string.Format("The league ' {0} ' with id: {1}, rejected the request to join the league.", leagueName, leagueId), MemberLogEnum.SystemStatusChanged, ip);

            return result;
        }
        /// <summary>
        /// attaches a member of RDN to the league as the owner of the league
        /// </summary>
        /// <param name="leagueId"></param>
        /// <param name="ownerId"></param>
        //public static bool AttachOwnerToLeague(Guid leagueId, Guid memberId)
        //{
        //    try
        //    {
        //        var dc = new ManagementContext();
        //        var league = dc.Leagues.Where(x => x.LeagueId == leagueId).FirstOrDefault();
        //        var member = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();

        //        LeagueOwnership owner = new LeagueOwnership();
        //        owner.League = league;
        //        owner.OwnerType = Convert.ToInt32(LeagueOwnerEnum.Owner);
        //        owner.Member = member;
        //        dc.LeagueOwners.Add(owner);
        //        dc.SaveChanges();
        //        return true;
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }
        //    return false;
        //}
        public static bool ToggleOwnerToLeague(Guid leagueId, Guid memberId, LeagueOwnersEnum ownerType)
        {
            try
            {
                var dc = new ManagementContext();
                var leagueMember = dc.LeagueMembers.Where(x => x.League.LeagueId == leagueId && x.Member.MemberId == memberId).FirstOrDefault();
                LeagueOwnersEnum owner = (LeagueOwnersEnum)leagueMember.LeagueOwnersEnums;
                bool isType = owner.HasFlag(ownerType);
                if (isType)
                    owner &= ~ownerType;
                else
                    owner |= ownerType;

                leagueMember.LeagueOwnersEnums = (int)owner;
                leagueMember.League = leagueMember.League;
                leagueMember.Member = leagueMember.Member;
                int c = dc.SaveChanges();

                var mem = MemberCache.GetMemberDisplay(leagueMember.Member.MemberId);
                if (mem != null && !String.IsNullOrEmpty(mem.Email) && c > 0)
                {
                    var emailData = new Dictionary<string, string> { 
                        { "derbyname", leagueMember.Member.DerbyName }, 
                        { "leagueName", leagueMember.League.Name},
                        { "ownerType", RDN.Utilities.Enums.EnumExt.ToFreindlyName( ownerType) } };

                    EmailServer.EmailServer.SendEmail(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, mem.Email, EmailServer.EmailServer.DEFAULT_SUBJECT + " Added To The Management Team", emailData, layout: EmailServer.EmailServerLayoutsEnum.MemberAddedToOwnershipGroupOfLeague, priority: EmailPriority.Normal);
                }

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool AttachHeadNsoToLeague(Guid leagueId, Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var league = dc.Leagues.Where(x => x.LeagueId == leagueId).FirstOrDefault();
                var member = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();

                LeagueOwnership owner = new LeagueOwnership();
                owner.League = league;
                owner.OwnerType = Convert.ToInt32(LeagueOwnerEnum.HeadNso);
                owner.Member = member;
                dc.LeagueOwners.Add(owner);
                dc.SaveChanges();
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        /// <summary>
        /// adds a picture to the league and makes it the logo of the league.
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="nameOfFile"></param>
        /// <param name="dc"></param>
        /// <param name="leagueId"></param>
        /// <returns></returns>
        private static string AddLogoForLeague(Stream fileStream, string nameOfFile, Guid leagueId)
        {
            try
            {
                ManagementContext dc = new ManagementContext();
                var memDb = dc.Leagues.Where(x => x.LeagueId == leagueId).FirstOrDefault();
                //time stamp for the save location
                DateTime timeOfSave = DateTime.UtcNow;

                FileInfo info = new FileInfo(nameOfFile);

                //the file name when we save it
                string fileName = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(memDb.Name + " logo-") + timeOfSave.ToFileTimeUtc() + info.Extension;
                string fileNameThumb = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(memDb.Name + " logo-") + timeOfSave.ToFileTimeUtc() + "_thumb" + info.Extension;

                string url = "http://images.rdnation.com/leagues/" + timeOfSave.Year + "/" + timeOfSave.Month + "/" + timeOfSave.Day + "/";
                string imageLocationToSave = @"C:\WebSites\images.rdnation.com\leagues\" + timeOfSave.Year + @"\" + timeOfSave.Month + @"\" + timeOfSave.Day + @"\";
                //creates the directory for the image
                if (!Directory.Exists(imageLocationToSave))
                    Directory.CreateDirectory(imageLocationToSave);

                string urlMain = url + fileName;
                string urlThumb = url + fileNameThumb;
                string imageLocationToSaveMain = imageLocationToSave + fileName;
                string imageLocationToSaveThumb = imageLocationToSave + fileNameThumb;



                RDN.Library.DataModels.League.LeaguePhoto image = new RDN.Library.DataModels.League.LeaguePhoto();
                image.ImageUrl = urlMain;
                image.SaveLocation = imageLocationToSaveMain;
                image.SaveLocationThumb = imageLocationToSaveThumb;
                image.ImageUrlThumb = urlThumb;
                image.IsPrimaryPhoto = true;
                image.IsVisibleToPublic = true;
                image.League = memDb;
                dc.LeagueLogos.Add(image);

                using (var newfileStream = new FileStream(imageLocationToSaveMain, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fileStream.CopyTo(newfileStream);
                }
                try
                {
                    Image thumbImg = Image.FromStream(fileStream);
                    Image thumb = RDN.Utilities.Drawing.Images.ScaleDownImage(thumbImg, 300, 300);
                    thumb.Save(imageLocationToSaveThumb);
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: imageLocationToSaveThumb);
                }
                //saves the photo to the DB.
                dc.SaveChanges();
                memDb.Logo = image;
                dc.SaveChanges();

                return urlMain;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return string.Empty;
        }

        /// <summary>
        /// updates the league to the matching league item
        /// </summary>
        /// <param name="leagueToUpdate"></param>
        /// <returns></returns>
        public static bool UpdateLeague(RDN.Portable.Classes.League.Classes.League leagueToUpdate, Stream fileStream, string nameOfFile)
        {
            try
            {
                var dc = new ManagementContext();
                var league = dc.Leagues.Include("ContactCard").Where(x => x.LeagueId == leagueToUpdate.LeagueId).FirstOrDefault();
                league.Name = leagueToUpdate.Name;
                league.ThemeColor = leagueToUpdate.ThemeColor;

                league.TimeZoneSelection = dc.TimeZone.Where(x => x.ZoneId == leagueToUpdate.TimeZoneId).FirstOrDefault();
                if (league.TimeZoneSelection != null)
                    league.TimeZone = league.TimeZoneSelection.GMTOffset;

                league.WebSite = leagueToUpdate.Website;
                league.Twitter = leagueToUpdate.Twitter;
                league.Instagram = leagueToUpdate.Instagram;
                league.Facebook = leagueToUpdate.Facebook;
                league.RuleSetsPlayedEnum = (long)leagueToUpdate.RuleSetsPlayedEnum;
                league.CultureLCID = leagueToUpdate.CultureSelected;
                UpdateLeagueColors(leagueToUpdate, dc, league);

                if (leagueToUpdate.Founded != null && leagueToUpdate.Founded != new DateTime() && leagueToUpdate.Founded > DateTime.UtcNow.AddYears(-100))
                    league.Founded = leagueToUpdate.Founded;

                if (fileStream != null)
                    AddLogoForLeague(fileStream, nameOfFile, league.LeagueId);

                league.InternalWelcomeMessage = leagueToUpdate.InternalWelcomeMessage;
                var emailDb = league.ContactCard.Emails.Where(x => x.IsDefault == true).FirstOrDefault();
                if (emailDb != null)
                {
                    emailDb.EmailAddress = leagueToUpdate.Email;
                }
                else
                {
                    Email em = new Email();
                    em.ContactCard = league.ContactCard;
                    em.EmailAddress = leagueToUpdate.Email;
                    em.IsDefault = true;
                    league.ContactCard.Emails.Add(em);
                }
                //int numberType = Convert.ToInt32(CommunicationTypeEnum.PhoneNumber);
                var phone = league.ContactCard.Communications.Where(x => x.CommunicationTypeEnum == (byte)CommunicationTypeEnum.PhoneNumber).FirstOrDefault();
                if (phone != null)
                    phone.Data = leagueToUpdate.PhoneNumber;
                else
                {
                    Communication com = new Communication();
                    com.Data = leagueToUpdate.PhoneNumber;
                    //com.CommunicationType = dc.CommunicationType.Where(x => x.CommunicationTypeId == numberType).FirstOrDefault();
                    com.CommunicationTypeEnum = (byte)CommunicationTypeEnum.PhoneNumber;
                    com.ContactCard = league.ContactCard;
                    com.IsDefault = true;
                    league.ContactCard.Communications.Add(com);
                }

                var addresses = league.ContactCard.Addresses.Where(x => x.IsDefault == true).FirstOrDefault();
                if (addresses == null)
                    if (league.ContactCard.Addresses.Count > 0)
                        addresses = league.ContactCard.Addresses.FirstOrDefault();
                if (addresses == null)
                {
                    addresses = new Address();
                    addresses.ContactCard = league.ContactCard;
                }
                addresses.CityRaw = leagueToUpdate.City;
                addresses.StateRaw = leagueToUpdate.State;
                addresses.Zip = leagueToUpdate.ZipCode;



                var countryDb = dc.Countries.Where(x => x.Name.ToLower() == leagueToUpdate.Country.ToLower()).FirstOrDefault();
                if (countryDb == null)
                    countryDb = dc.Countries.Where(x => x.Code.ToLower() == leagueToUpdate.Country.ToLower()).FirstOrDefault();
                addresses.Country = countryDb;
                var coords = OpenStreetMap.FindLatLongOfAddress(addresses.Address1, addresses.Address2, addresses.Zip, addresses.CityRaw, addresses.StateRaw, addresses.Country != null ? addresses.Country.Name : string.Empty);
                if (coords != null)
                {
                    addresses.Coords = new System.Device.Location.GeoCoordinate();
                    addresses.Coords.Latitude = coords.Latitude;
                    addresses.Coords.Longitude = coords.Longitude;
                    addresses.Coords.Altitude = 0;
                    addresses.Coords.Course = 0;
                    addresses.Coords.HorizontalAccuracy = 1;
                    addresses.Coords.Speed = 0;
                    addresses.Coords.VerticalAccuracy = 1;
                }
                addresses.IsDefault = true;


                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool UpdateLeagueFromMobile(LeagueBase leagueToUpdate)
        {
            try
            {
                var dc = new ManagementContext();
                var league = dc.Leagues.Include("ContactCard").Where(x => x.LeagueId == leagueToUpdate.LeagueId).FirstOrDefault();
                league.Name = leagueToUpdate.Name;
                league.WebSite = leagueToUpdate.Website;
                league.Twitter = leagueToUpdate.Twitter;
                league.Instagram = leagueToUpdate.Instagram;
                league.Facebook = leagueToUpdate.Facebook;
                league.RuleSetsPlayedEnum = (long)leagueToUpdate.RuleSetsPlayedEnum;
                league.CultureLCID = leagueToUpdate.CultureSelected;

                if (leagueToUpdate.Founded != null && leagueToUpdate.Founded != new DateTime() && leagueToUpdate.Founded > DateTime.UtcNow.AddYears(-100))
                    league.Founded = leagueToUpdate.Founded;

                var emailDb = league.ContactCard.Emails.Where(x => x.IsDefault == true).FirstOrDefault();
                if (emailDb != null)
                {
                    emailDb.EmailAddress = leagueToUpdate.Email;
                }
                else
                {
                    Email em = new Email();
                    em.ContactCard = league.ContactCard;
                    em.EmailAddress = leagueToUpdate.Email;
                    em.IsDefault = true;
                    league.ContactCard.Emails.Add(em);
                }
                //int numberType = Convert.ToInt32(CommunicationTypeEnum.PhoneNumber);
                var phone = league.ContactCard.Communications.Where(x => x.CommunicationTypeEnum == (byte)CommunicationTypeEnum.PhoneNumber).FirstOrDefault();
                if (phone != null)
                    phone.Data = leagueToUpdate.PhoneNumber;
                else
                {
                    Communication com = new Communication();
                    com.Data = leagueToUpdate.PhoneNumber;
                    //com.CommunicationType = dc.CommunicationType.Where(x => x.CommunicationTypeId == numberType).FirstOrDefault();
                    com.CommunicationTypeEnum = (byte)CommunicationTypeEnum.PhoneNumber;
                    com.ContactCard = league.ContactCard;
                    com.IsDefault = true;
                    league.ContactCard.Communications.Add(com);
                }

                var addresses = league.ContactCard.Addresses.Where(x => x.IsDefault == true).FirstOrDefault();
                if (addresses == null)
                    if (league.ContactCard.Addresses.Count > 0)
                        addresses = league.ContactCard.Addresses.FirstOrDefault();
                if (addresses == null)
                {
                    addresses = new Address();
                    addresses.ContactCard = league.ContactCard;
                }
                addresses.CityRaw = leagueToUpdate.City;
                addresses.StateRaw = leagueToUpdate.State;
                addresses.Zip = leagueToUpdate.ZipCode;

                var countryDb = dc.Countries.Where(x => x.Name.ToLower() == leagueToUpdate.Country.ToLower()).FirstOrDefault();
                if (countryDb == null)
                    countryDb = dc.Countries.Where(x => x.Code.ToLower() == leagueToUpdate.Country.ToLower()).FirstOrDefault();
                addresses.Country = countryDb;
                var coords = OpenStreetMap.FindLatLongOfAddress(addresses.Address1, addresses.Address2, addresses.Zip, addresses.CityRaw, addresses.StateRaw, addresses.Country != null ? addresses.Country.Name : string.Empty);
                if (coords != null)
                {
                    addresses.Coords = new System.Device.Location.GeoCoordinate();
                    addresses.Coords.Latitude = coords.Latitude;
                    addresses.Coords.Longitude = coords.Longitude;
                    addresses.Coords.Altitude = 0;
                    addresses.Coords.Course = 0;
                    addresses.Coords.HorizontalAccuracy = 1;
                    addresses.Coords.Speed = 0;
                    addresses.Coords.VerticalAccuracy = 1;
                }
                addresses.IsDefault = true;


                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }


        private static void UpdateLeagueColors(RDN.Portable.Classes.League.Classes.League leagueToUpdate, ManagementContext dc, DataModels.League.League league)
        {
            //removes any colors not being used anymore.
            List<int> colors = new List<int>();
            if (!String.IsNullOrEmpty(leagueToUpdate.ColorsSelected))
            {
                foreach (string color in leagueToUpdate.ColorsSelected.Split(';'))
                {
                    if (color.Length > 3)
                    {
                        Color c = ColorTranslator.FromHtml(color);
                        int arb = c.ToArgb();
                        colors.Add(arb);
                    }
                }
            }
            var colorsNoLongerIn = league.Colors.Where(x => !colors.Contains(x.Color.ColorIdCSharp)).ToList();
            foreach (var removeColor in colorsNoLongerIn)
            {
                league.Colors.Remove(removeColor);
            }
            //adds colors that are not currently added to the storeitem.
            if (!String.IsNullOrEmpty(leagueToUpdate.ColorsSelected))
            {
                foreach (string color in leagueToUpdate.ColorsSelected.Split(';'))
                {
                    if (color.Length > 3)
                    {
                        Color c = ColorTranslator.FromHtml(color);
                        int arb = c.ToArgb();
                        if (league.Colors.Where(x => x.Color.ColorIdCSharp == arb).FirstOrDefault() == null)
                        {
                            var colorDb = dc.Colors.Where(x => x.ColorIdCSharp == arb).FirstOrDefault();
                            if (colorDb != null)
                            {
                                DataModels.League.LeagueColor cItem = new DataModels.League.LeagueColor();
                                cItem.Color = colorDb;
                                cItem.League = league;
                                league.Colors.Add(cItem);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// updates the leagues basic information basically coming from a new user registering a new league.
        /// </summary>
        /// <param name="leagueId"></param>
        /// <param name="leagueName"></param>
        /// <param name="emailOfLeague"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="city"></param>
        /// <param name="country"></param>

        /// <param name="state"></param>
        /// <param name="timezone"></param>
        public static bool UpdateLeagueForOwner(Guid leagueId, string leagueName, string emailOfLeague, string phoneNumber, string city, int country, string state, double timezone)
        {
            try
            {
                var dc = new ManagementContext();
                var league = dc.Leagues.Include("Owners").Include("Teams").Include("Members").Include("ContactCard").Where(x => x.LeagueId == leagueId).FirstOrDefault();
                league.Name = leagueName;
                league.SubscriptionPeriodEnds = DateTime.UtcNow.AddDays(InvoiceSubscription.NUMBER_OF_DAYS_FOR_TRIAL_SUBSCRIPTION);

                var emailDb = league.ContactCard.Emails.Where(x => x.IsDefault == true).FirstOrDefault();
                if (emailDb != null)
                {
                    emailDb.EmailAddress = emailOfLeague;
                }
                else
                {
                    Email em = new Email();
                    em.ContactCard = league.ContactCard;
                    em.EmailAddress = emailOfLeague;
                    em.IsDefault = true;
                }
                var addresses = league.ContactCard.Addresses.Where(x => x.IsDefault == true).FirstOrDefault();
                if (addresses != null)
                {
                    addresses.CityRaw = city;
                    addresses.StateRaw = state;
                    var countryDb = dc.Countries.Where(x => x.CountryId == country).FirstOrDefault();
                    addresses.Country = countryDb;
                    addresses.TimeZone = timezone;
                    var coords = OpenStreetMap.FindLatLongOfAddress(addresses.Address1, addresses.Address2, addresses.Zip, addresses.CityRaw, addresses.StateRaw, addresses.Country != null ? addresses.Country.Name : string.Empty);
                    if (coords != null)
                    {
                        addresses.Coords = new System.Device.Location.GeoCoordinate();
                        addresses.Coords.Latitude = coords.Latitude;
                        addresses.Coords.Longitude = coords.Longitude;
                        addresses.Coords.Altitude = 0;
                        addresses.Coords.Course = 0;
                        addresses.Coords.HorizontalAccuracy = 1;
                        addresses.Coords.Speed = 0;
                        addresses.Coords.VerticalAccuracy = 1;
                    }
                }
                else if (league.ContactCard.Addresses.Count > 0)
                {
                    addresses = league.ContactCard.Addresses.FirstOrDefault();
                    addresses.CityRaw = city;
                    addresses.StateRaw = state;
                    var countryDb = dc.Countries.Where(x => x.CountryId == country).FirstOrDefault();
                    addresses.Country = countryDb;
                    addresses.TimeZone = timezone;
                    addresses.IsDefault = true;
                    var coords = OpenStreetMap.FindLatLongOfAddress(addresses.Address1, addresses.Address2, addresses.Zip, addresses.CityRaw, addresses.StateRaw, addresses.Country != null ? addresses.Country.Name : string.Empty);
                    if (coords != null)
                    {
                        addresses.Coords = new System.Device.Location.GeoCoordinate();
                        addresses.Coords.Latitude = coords.Latitude;
                        addresses.Coords.Longitude = coords.Longitude;
                        addresses.Coords.Altitude = 0;
                        addresses.Coords.Course = 0;
                        addresses.Coords.HorizontalAccuracy = 1;
                        addresses.Coords.Speed = 0;
                        addresses.Coords.VerticalAccuracy = 1;
                    }
                }
                else
                {
                    Address add = new Address();
                    add.IsDefault = true;
                    add.CityRaw = city;
                    add.StateRaw = state;
                    var countryDb = dc.Countries.Where(x => x.CountryId == country).FirstOrDefault();
                    add.Country = countryDb;
                    add.TimeZone = timezone;
                    add.IsDefault = true;
                    var coords = OpenStreetMap.FindLatLongOfAddress(addresses.Address1, addresses.Address2, addresses.Zip, addresses.CityRaw, addresses.StateRaw, addresses.Country != null ? addresses.Country.Name : string.Empty);
                    if (coords != null)
                    {
                        add.Coords = new System.Device.Location.GeoCoordinate();
                        add.Coords.Latitude = coords.Latitude;
                        add.Coords.Longitude = coords.Longitude;
                        add.Coords.Altitude = 0;
                        add.Coords.Course = 0;
                        add.Coords.HorizontalAccuracy = 1;
                        add.Coords.Speed = 0;
                        add.Coords.VerticalAccuracy = 1;
                    }
                    add.ContactCard = league.ContactCard;
                }
                var member = RDN.Library.Classes.Account.User.GetMember();

                var emailData = new Dictionary<string, string>
                                        {
                                            { "name", leagueName },
                                            { "leagueId", leagueId.ToString() },
                                            { "email", emailOfLeague },
                                            { "phone", phoneNumber },
                                            { "country", country.ToString() },
                                            { "memberId", member.MemberId.ToString() },
                                            { "memberName", member.DerbyName}
                                          };
                EmailServer.EmailServer.SendEmail(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, ServerConfig.DEFAULT_INFO_EMAIL, EmailServer.EmailServer.DEFAULT_SUBJECT + " A league has found its owner", emailData, layout: EmailServer.EmailServerLayoutsEnum.OwnershipTakenOfLeague);


                dc.SaveChanges();
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        //public static DataModels.League.League GetLeague()
        //{
        //    return User.GetMember().League;
        //}
        /// <summary>
        /// disconnects the member from the league
        /// </summary>
        /// <param name="leagueId"></param>
        /// <param name="memberId"></param>
        public static bool DisconnectMemberFromLeague(Guid leagueId, Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var league = dc.LeagueMembers.Where(x => x.League.LeagueId == leagueId && x.Member.MemberId == memberId).FirstOrDefault();
                if (league != null)
                {
                    league.HasLeftLeague = true;
                    if (league.DepartureDate != new DateTime())
                        league.DepartureDate = DateTime.UtcNow;
                    league.Member = league.Member;
                    league.League = league.League;
                    var mem = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                    if (mem.CurrentLeagueId == leagueId)
                        mem.CurrentLeagueId = new Guid();
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
        public static bool RemoveMemberFromLeague(Guid leagueId, Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var league = dc.LeagueMembers.Where(x => x.League.LeagueId == leagueId && x.Member.MemberId == memberId).FirstOrDefault();
                if (league != null)
                {
                    dc.LeagueMembers.Remove(league);
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

        public static int RemoveLeagueMembersThatAreDisconnected()
        {
            try
            {
                var dc = new ManagementContext();
                var leagues = dc.LeagueMembers.Include("Member").Include("League").Where(x => x.HasLeftLeague == true);

                foreach (var mem in leagues)
                {
                    try
                    {
                        dc.LeagueMembers.Remove(mem);

                        var mems = dc.Members.Where(x => x.MemberId == mem.Member.MemberId).FirstOrDefault();
                        if (mems.CurrentLeagueId == mem.League.LeagueId)
                            mems.CurrentLeagueId = new Guid();
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }
                return dc.SaveChanges();

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }

        public static int FixMembersNotApartOfLeague()
        {
            var dc = new ManagementContext();
            var league = dc.LeagueMembers.Where(x => x.HasLeftLeague == false);
            foreach (var mem in league)
            {
                if (mem.Member.CurrentLeagueId == new Guid() || mem.Member.CurrentLeagueId == null)
                {
                    mem.Member.CurrentLeagueId = mem.League.LeagueId;
                }
            }
            int c = dc.SaveChanges();
            return c;
        }
        public static int PortLeagueOwnersTableOverToLeagueMembersOwner()
        {
            var dc = new ManagementContext();
            var leagues = dc.LeagueOwners;
            foreach (var l in leagues)
            {
                LeagueOwnerEnum owner = (LeagueOwnerEnum)Enum.Parse(typeof(LeagueOwnerEnum), l.OwnerType.ToString());
                var leagueMember = dc.LeagueMembers.Where(x => x.Member.MemberId == l.Member.MemberId && x.League.LeagueId == l.League.LeagueId).FirstOrDefault();
                if (leagueMember != null)
                {
                    LeagueOwnersEnum o = LeagueOwnersEnum.None;
                    if (owner == LeagueOwnerEnum.HeadNso)
                        o = LeagueOwnersEnum.Head_NSO;
                    else if (owner == LeagueOwnerEnum.Manager)
                        o = LeagueOwnersEnum.Manager;
                    else if (owner == LeagueOwnerEnum.Owner)
                        o = LeagueOwnersEnum.Owner;

                    LeagueOwnersEnum lowner = (LeagueOwnersEnum)leagueMember.LeagueOwnersEnums;
                    bool isType = lowner.HasFlag(o);
                    if (!isType)
                        lowner |= o;
                    leagueMember.LeagueOwnersEnums = (int)lowner;
                    leagueMember.League = leagueMember.League;
                    leagueMember.Member = leagueMember.Member;

                }
            }
            int c = dc.SaveChanges();
            return c;
        }

        public static int CreateNewLeagueJoinCodes()
        {
            var dc = new ManagementContext();
            var leagues = dc.Leagues;
            foreach (var le in leagues)
            {
                le.LeagueJoinCode = Guid.NewGuid();
            }
            int c = dc.SaveChanges();
            return c;
        }

        public static bool RefreshLeagueJoinCode(Guid leagueId)
        {
            var dc = new ManagementContext();
            var leagues = dc.Leagues.Where(x => x.LeagueId == leagueId).FirstOrDefault();

            leagues.LeagueJoinCode = Guid.NewGuid();
            int c = dc.SaveChanges();
            return c>0;
        }
    }
}
