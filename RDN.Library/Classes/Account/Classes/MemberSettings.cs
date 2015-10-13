using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Cache;
using RDN.Library.Classes.Communications.Enums;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Utilities.Config;
using RDN.Portable.Config;
using RDN.Library.Classes.League.Classes;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Account.Enums.Settings;
using RDN.Portable.Classes.Account.Enums;
using RDN.Portable.Classes.Communications.Enums;
using RDN.Library.Classes.Config;

namespace RDN.Library.Classes.Account.Classes
{
    public class MemberSettingsFactory  {

        public MemberSettingsFactory()
        { }

      

        public static bool TogglePrivacySettingsForMember(Guid memberId, MemberPrivacySettingsEnum privacySetting)
        {
            try
            {
                var dc = new ManagementContext();
                var leagueMember = dc.Members.Include("Settings").Where(x => x.MemberId == memberId).FirstOrDefault();
                if (leagueMember.Settings == null)
                    leagueMember.Settings = new DataModels.Member.MemberSettings();


                MemberPrivacySettingsEnum owner = (MemberPrivacySettingsEnum)leagueMember.Settings.MemberPrivacySettingsEnum;
                bool isType = owner.HasFlag(privacySetting);
                if (isType)
                    owner &= ~privacySetting;
                else
                    owner |= privacySetting;

                leagueMember.Settings.MemberPrivacySettingsEnum = (long)owner;
                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool ChangeEmailNotificationForumBroadcastsOff(Guid memberId, bool onOrOff)
        {
            try
            {
                var dc = new ManagementContext();
                var settings = dc.Members.Include("Notifications").Where(x => x.MemberId == memberId).FirstOrDefault();
                if (settings == null)
                    return false;
                if (settings.Notifications == null)
                    settings.Notifications = new DataModels.Member.MemberNotifications();
                settings.Notifications.EmailForumBroadcastsTurnOff = !onOrOff;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool ChangeEmailNotificationForumNewPosts(Guid memberId, bool onOrOff)
        {
            try
            {
                var dc = new ManagementContext();
                var settings = dc.Members.Include("Notifications").Where(x => x.MemberId == memberId).FirstOrDefault();
                if (settings == null)
                    return false;
                if (settings.Notifications == null)
                    settings.Notifications = new DataModels.Member.MemberNotifications();
                settings.Notifications.EmailForumNewPostTurnOff = !onOrOff;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool ChangeEmailNotificationForumWeeklyRoundUpOff(Guid memberId, bool onOrOff)
        {
            try
            {
                var dc = new ManagementContext();
                var settings = dc.Members.Include("Notifications").Where(x => x.MemberId == memberId).FirstOrDefault();
                if (settings == null)
                    return false;
                if (settings.Notifications == null)
                    settings.Notifications = new DataModels.Member.MemberNotifications();
                settings.Notifications.EmailForumWeeklyRoundupTurnOff = !onOrOff;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool ChangeEmailNotificationCalendarNewEventOff(Guid memberId, bool onOrOff)
        {
            try
            {
                var dc = new ManagementContext();
                var settings = dc.Members.Include("Notifications").Where(x => x.MemberId == memberId).FirstOrDefault();
                if (settings == null)
                    return false;
                if (settings.Notifications == null)
                    settings.Notifications = new DataModels.Member.MemberNotifications();
                settings.Notifications.EmailCalendarNewEventBroadcastTurnOff = !onOrOff;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool ChangeEmailNotificationMessageNewOff(Guid memberId, bool onOrOff)
        {
            try
            {
                var dc = new ManagementContext();
                var settings = dc.Members.Include("Notifications").Where(x => x.MemberId == memberId).FirstOrDefault();
                if (settings == null)
                    return false;
                if (settings.Notifications == null)
                    settings.Notifications = new DataModels.Member.MemberNotifications();
                settings.Notifications.EmailMessagesReceivedTurnOff = !onOrOff;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        /// <summary>
        /// turns the notification for broadcasting a group forum post.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="groupId"></param>
        /// <param name="onOrOff"></param>
        /// <returns></returns>
        public static bool ChangeEmailNotificationSettingForGroupBroadcasted(Guid memberId, long groupId, bool onOrOff)
        {
            try
            {
                var dc = new ManagementContext();
                var settings = dc.LeagueGroupMembers.Where(x => x.Member.MemberId == memberId && x.Group.Id == groupId).FirstOrDefault();
                if (settings == null)
                    return false;
                settings.TurnOffBroadcastNotifications = !onOrOff;
                settings.Group = settings.Group;
                settings.Member = settings.Member;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        /// <summary>
        /// turns the NEW POSTS for the forum GROUP on and off.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="groupId"></param>
        /// <param name="onOrOff"></param>
        /// <returns></returns>
        public static bool ChangeEmailNotificationSettingForGroup(Guid memberId, long groupId, bool onOrOff)
        {
            try
            {
                var dc = new ManagementContext();
                var settings = dc.LeagueGroupMembers.Where(x => x.Member.MemberId == memberId && x.Group.Id == groupId).FirstOrDefault();
                if (settings == null)
                    return false;
                settings.TurnOffEmailNotifications = !onOrOff;
                settings.Group = settings.Group;
                settings.Member = settings.Member;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool ChangeEmailNotificationSettingForLeague(Guid memberId, Guid leagueId, bool onOrOff)
        {
            try
            {
                var dc = new ManagementContext();
                var settings = dc.LeagueMembers.Where(x => x.Member.MemberId == memberId && x.League.LeagueId == leagueId).FirstOrDefault();
                if (settings == null)
                    return false;
                settings.TurnOffEmailNotifications = !onOrOff;
                settings.League = settings.League;
                settings.Member = settings.Member;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        /// <summary>
        /// Change Sort Order of Forum Messages
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="groupId"></param>
        /// <param name="onOrOff"></param>
        /// <returns></returns>
        public static bool ChangeForumMessageOrderSetting(Guid memberId, bool onOrOff)
        {
            try
            {
                var dc = new ManagementContext();
                var leagueMember = dc.Members.Include("Notifications").Include("Settings").Include("Leagues").Include("Leagues.SkaterClass").Include("InsuranceNumbers").Include("ContactCard").Include("ContactCard.Emails").Include("ContactCard.Communications").Include("Photos").Include("Federations").Include("MedicalInformation").Where(x => x.MemberId == memberId).FirstOrDefault();
                if (leagueMember.Settings == null)
                    leagueMember.Settings = new DataModels.Member.MemberSettings();

                leagueMember.Settings.ForumDescending = onOrOff;
                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
            return false;
        }

        public static bool ChangeCalendarViewSetting(CalendarDefaultViewEnum viewType, Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var settings = dc.Members.Include("Settings").Where(x => x.MemberId == memberId).FirstOrDefault();
                if (settings == null)
                    return false;
                if (settings.Settings == null)
                    settings.Settings = new DataModels.Member.MemberSettings();

                settings.Settings.CalendarViewSetting = (byte)viewType;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        /// <summary>
        /// sends user text message to verify account
        /// </summary>
        /// <param name="number"></param>
        /// <param name="provider"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static bool VerifiySMSCarrier(long number, MobileServiceProvider provider, Guid memberId)
        {
            try
            {
                var mem = MemberCache.GetMemberDisplay(memberId);
                if (String.IsNullOrEmpty(mem.SMSVerificationNum))
                {
                    Random rand = new Random();
                    mem.SMSVerificationNum = rand.Next(10000, 99999).ToString();
                }
                mem.PhoneNumber = number.ToString();
                mem.Carrier = provider;
                string body = "Your Code: " + mem.SMSVerificationNum + " (If sent to you in error, our apologies, please ignore.)";

                var emailData = new Dictionary<string, string> { { "body", body } };

                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.WebsiteShortName, LibraryConfig.TextMessageEmail, mem.PhoneNumber, emailData, EmailServer.EmailServerLayoutsEnum.TextMessage, DataModels.EmailServer.Enums.EmailPriority.Important);
                var dc = new ManagementContext();
                var memDb = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();

                ContactCard.ContactCardFactory.UpdatePhoneNumberToCard(mem.PhoneNumber, memDb.ContactCard, mem.SMSVerificationNum, mem.Carrier);
                int c = dc.SaveChanges();

                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool VerifiySMSCarrierCode(long number, MobileServiceProvider provider, string code, Guid memberId)
        {
            try
            {
                var mem = MemberCache.GetMemberDisplay(memberId);
                if (mem.SMSVerificationNum == code)
                {
                    string body = "You are now Verified to receive " + @RDN.Library.Classes.Config.LibraryConfig.WebsiteShortName + " text messages.";


                    var emailData = new Dictionary<string, string> { { "body", body } };

                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, @RDN.Library.Classes.Config.LibraryConfig.WebsiteShortName, LibraryConfig.TextMessageEmail, mem.PhoneNumber, emailData, EmailServer.EmailServerLayoutsEnum.TextMessage, DataModels.EmailServer.Enums.EmailPriority.Important);

                    var dc = new ManagementContext();
                    var memDb = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();

                    ContactCard.ContactCardFactory.UpdatePhoneNumberToCard(mem.PhoneNumber, memDb.ContactCard, mem.SMSVerificationNum, mem.Carrier, true);
                    int c = dc.SaveChanges();

                    return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static MemberSettingsClass DisplayMemberSettings(DataModels.Member.MemberSettings settings)
        {
            try
            {
                if (settings == null)
                    return new MemberSettingsClass();
                MemberSettingsClass s = new MemberSettingsClass();
                s.CalendarViewDefault = (CalendarDefaultViewEnum)settings.CalendarViewSetting;
                s.PrivacySettings = (MemberPrivacySettingsEnum)settings.MemberPrivacySettingsEnum;
                s.Hide_DOB_From_League = s.PrivacySettings.HasFlag(MemberPrivacySettingsEnum.Hide_DOB_From_League);
                s.Hide_DOB_From_Public = s.PrivacySettings.HasFlag(MemberPrivacySettingsEnum.Hide_DOB_From_Public);
                s.Hide_Email_From_League = s.PrivacySettings.HasFlag(MemberPrivacySettingsEnum.Hide_Email_From_League);
                s.Hide_Phone_Number_From_League = s.PrivacySettings.HasFlag(MemberPrivacySettingsEnum.Hide_Phone_Number_From_League);
                s.Hide_Address_From_League = s.PrivacySettings.HasFlag(MemberPrivacySettingsEnum.Hide_Address_From_League);
                s.ForumDescending = settings.ForumDescending;
                return s;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new MemberSettingsClass();
        }
    }
}
