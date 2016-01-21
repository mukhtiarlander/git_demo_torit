using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Cache;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Account.Enums;
using RDN.Library.Classes.EmailServer;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.League.Enums;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.EmailServer.Enums;
using RDN.Library.DataModels.League.Group;
using RDN.Utilities.Config;
using RDN.Portable.Config;
using RDN.Portable.Classes.League;
using RDN.Portable.Classes.Imaging;
using RDN.Portable.Classes.League.Enums;
using RDN.Portable.Classes.League.Classes;
using RDN.Library.Classes.Config;

namespace RDN.Library.Classes.League.Classes
{
    public class LeagueGroupFactory
    {


        public static bool RemoveMemberToGroup(long groupId, Guid memId)
        {
            try
            {
                var dc = new ManagementContext();
                var mem = dc.LeagueGroupMembers.Where(x => x.Member.MemberId == memId && x.Group.Id == groupId && x.IsMemRemoved == false).FirstOrDefault();
                if (mem != null)
                {
                    mem.Group = mem.Group;
                    mem.Member = mem.Member;
                    mem.IsMemRemoved = true;
                    int c = dc.SaveChanges();
                    MemberCache.Clear(memId);
                    MemberCache.ClearApiCache(memId);
                    if (c > 0)
                        return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool GetGroupOfMember(long groupId, Guid memId)
        {
            try
            {
                var dc = new ManagementContext();
                var mem = dc.LeagueGroupMembers.Where(x => x.Member.MemberId == memId && x.Group.Id == groupId && x.IsMemRemoved == false).FirstOrDefault();
                if (mem != null)
                {
                    return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool IsMemberInGroup(long groupId, Guid memId)
        {
            try
            {
                var dc = new ManagementContext();
                var mem = dc.LeagueGroupMembers.Where(x => x.Member.MemberId == memId && x.Group.Id == groupId && x.IsMemRemoved == false).FirstOrDefault();
                if (mem != null)
                {
                    return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool RemoveGroup(long groupId)
        {
            try
            {
                var dc = new ManagementContext();
                var mem = dc.LeagueGroups.Where(x => x.Id == groupId).FirstOrDefault();
                if (mem != null)
                {
                    mem.League = mem.League;
                    mem.IsGroupRemoved = true;
                    int c = dc.SaveChanges();
                    if (c > 0)
                        return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool UpdateMemberToGroup(long groupId, Guid memId, string memType)
        {
            try
            {
                var dc = new ManagementContext();
                var mem = dc.LeagueGroupMembers.Where(x => x.Member.MemberId == memId && x.Group.Id == groupId && x.IsMemRemoved == false).FirstOrDefault();
                if (mem != null)
                {
                    mem.Group = mem.Group;
                    mem.Member = mem.Member;
                    mem.MemberAccessLevelEnum = Convert.ToInt32((GroupMemberAccessLevelEnum)Enum.Parse(typeof(GroupMemberAccessLevelEnum), memType));
                    int c = dc.SaveChanges();
                    MemberCache.Clear(memId);
                    MemberCache.ClearApiCache(memId);
                    if (c > 0)
                        return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool AddMemberToGroup(long groupId, Guid leagueId, Guid memId, string memType)
        {
            try
            {
                var dc = new ManagementContext();
                var memDb = dc.LeagueGroupMembers.Where(x => x.Member.MemberId == memId && x.Group.Id == groupId).FirstOrDefault();
                if (memDb == null)
                {
                    GroupMember mem = new GroupMember();
                    mem.Group = dc.LeagueGroups.Where(x => x.Id == groupId && x.League.LeagueId == leagueId).FirstOrDefault();
                    mem.Member = dc.Members.Where(x => x.MemberId == memId).FirstOrDefault();
                    mem.MemberAccessLevelEnum = Convert.ToInt32((GroupMemberAccessLevelEnum)Enum.Parse(typeof(GroupMemberAccessLevelEnum), memType));
                    dc.LeagueGroupMembers.Add(mem);
                    EmailMemberAboutAddedToGroup(mem.Group.GroupName, memId);
                }
                else
                {
                    memDb.Group = memDb.Group;
                    memDb.Member = memDb.Member;
                    memDb.IsMemRemoved = false;
                    memDb.MemberAccessLevelEnum = Convert.ToInt32((GroupMemberAccessLevelEnum)Enum.Parse(typeof(GroupMemberAccessLevelEnum), memType));
                    EmailMemberAboutAddedToGroup(memDb.Group.GroupName, memId);
                }
                int c = dc.SaveChanges();
                MemberCache.Clear(memId);
                MemberCache.ClearApiCache(memId);
                if (c > 0)
                    return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool UpdateGroup(LeagueGroup group)
        {
            try
            {
                var dc = new ManagementContext();
                var g = dc.LeagueGroups.Include("GroupMembers").Include("ContactCard").Include("ContactCard.Emails").Where(x => x.Id == group.Id).FirstOrDefault();
                if (g != null)
                {
                    g.GroupName = group.GroupName;
                    g.GroupTypeEnum = Convert.ToInt32(group.GroupTypeEnum);
                    g.IsPublicToWorld = group.IsPublicToWorld;
                    g.League = g.League;
                }

                if (g.ContactCard != null && g.ContactCard.Emails.Count > 0 && !String.IsNullOrEmpty(group.EmailAddress))
                {
                    var e = g.ContactCard.Emails.FirstOrDefault();
                    e.EmailAddress = group.EmailAddress;
                }
                else if (!String.IsNullOrEmpty(group.EmailAddress))
                {
                    if (g.ContactCard == null)
                        g.ContactCard = new DataModels.ContactCard.ContactCard();
                    g.ContactCard.Emails.Add(new RDN.Library.DataModels.ContactCard.Email { EmailAddress = group.EmailAddress, IsDefault = true });
                }

                foreach (var mem in group.GroupMembers)
                {
                    var gm11 = g.GroupMembers.Where(x => x.Member.MemberId == mem.MemberId).FirstOrDefault();
                    if (mem.IsApartOfGroup)
                    {

                        if (gm11 == null)
                        {
                            GroupMember gm = new GroupMember();
                            gm.Group = g;
                            gm.Member = dc.Members.Where(x => x.MemberId == mem.MemberId).FirstOrDefault();
                            gm.MemberAccessLevelEnum = Convert.ToInt32(mem.MemberAccessLevelEnum);
                            g.GroupMembers.Add(gm);
                            LeagueGroupFactory.EmailMemberAboutAddedToGroup(group.GroupName, mem.MemberId);
                        }
                        else
                        {
                            gm11.MemberAccessLevelEnum = Convert.ToInt32(mem.MemberAccessLevelEnum);
                            gm11.IsMemRemoved = false;
                            gm11.Member = gm11.Member;
                            gm11.Group = gm11.Group;
                        }
                    }
                    else
                    {
                        if (gm11 != null)
                        {
                            gm11.IsMemRemoved = true;
                            gm11.Member = gm11.Member;
                            gm11.Group = gm11.Group;
                        }
                    }
                }

                int c = dc.SaveChanges();


                foreach (var mem in group.GroupMembers)
                {
                    if (mem.IsApartOfGroup)
                    {
                        MemberCache.Clear(mem.MemberId);
                        MemberCache.ClearApiCache(mem.MemberId);
                    }
                }


                if (c > 0)
                    return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool AddGroup(LeagueGroup group)
        {
            try
            {
                var dc = new ManagementContext();
                RDN.Library.DataModels.League.Group.Group g = new DataModels.League.Group.Group();
                g.GroupName = group.GroupName;
                g.GroupTypeEnum = Convert.ToInt32(group.GroupTypeEnum);
                g.IsPublicToWorld = group.IsPublicToWorld;
                g.League = dc.Leagues.Where(x => x.LeagueId == group.League.LeagueId).FirstOrDefault();

                if (!String.IsNullOrEmpty(group.EmailAddress))
                {
                    g.ContactCard = new DataModels.ContactCard.ContactCard();
                    g.ContactCard.Emails.Add(new RDN.Library.DataModels.ContactCard.Email { EmailAddress = group.EmailAddress, IsDefault = true });

                }
                foreach (var mem in group.GroupMembers)
                {
                    GroupMember gm = new GroupMember();
                    gm.Group = g;
                    gm.Member = dc.Members.Where(x => x.MemberId == mem.MemberId).FirstOrDefault();
                    gm.MemberAccessLevelEnum = Convert.ToInt32(mem.MemberAccessLevelEnum);
                    g.GroupMembers.Add(gm);
                }
                dc.LeagueGroups.Add(g);
                int c = dc.SaveChanges();
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                MemberCache.Clear(memId);
                MemberCache.ClearApiCache(memId);
                if (c > 0)
                    return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static void EmailMemberAboutAddedToGroup(string groupName, Guid memberId)
        {
            //send emails to all memmbers added to group.
            var member = MemberCache.GetMemberDisplay(memberId);
            if (!String.IsNullOrEmpty(member.Email))
            {
                var emailData = new Dictionary<string, string> {
                        { "derbyname", member.DerbyName },
                        { "groupname", groupName},
                        { "link", LibraryConfig.InternalSite} };

                //clear members cache for each member.
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, member.Email, LibraryConfig.DefaultEmailSubject + " Added To New Group", emailData, layout: EmailServerLayoutsEnum.UserAddedToLeagueGroup, priority: EmailPriority.Normal);
            }
            MemberCache.Clear(memberId);
            MemberCache.ClearApiCache(memberId);
        }

        public static LeagueGroup GetGroup(long groupId, Guid leagueId)
        {
            var dc = new ManagementContext();
            var group = dc.LeagueGroups.Where(x => x.League.LeagueId == leagueId && x.Id == groupId).FirstOrDefault();
            return DisplayGroup(group);
        }

        public static List<LeagueGroup> GetGroups(Guid leagueId)
        {
            var dc = new ManagementContext();
            var groups = dc.LeagueGroups.Include("ContactCard").Include("ContactCard.Emails").Where(x => x.League.LeagueId == leagueId && x.IsGroupRemoved == false).ToList();
            return DisplayGroups(groups);
        }
        public static List<LeagueGroup> GetGroups()
        {
            var dc = new ManagementContext();
            var groups = dc.LeagueGroups.Include("ContactCard").Include("ContactCard.Emails").Where(x => x.IsGroupRemoved == false).ToList();
            return DisplayGroups(groups);
        }

        public static List<LeagueGroup> DisplayGroups(List<DataModels.League.Group.Group> groups)
        {

            List<LeagueGroup> gs = new List<LeagueGroup>();
            try
            {
                foreach (var group in groups)
                {
                    gs.Add(DisplayGroup(group));
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return gs;
        }

        public static LeagueGroup DisplayGroup(DataModels.League.Group.Group group)
        {
            if (group == null)
                return null;
            LeagueGroup g = new LeagueGroup();
            try
            {
                g.DefaultBroadcastMessage = group.BroadcastToEveryone;
                g.GroupName = group.GroupName;
                g.GroupTypeEnum = (LeagueGroupTypeEnum)Enum.Parse(typeof(LeagueGroupTypeEnum), group.GroupTypeEnum.ToString());
                g.Id = group.Id;
                g.IsPublicToWorld = group.IsPublicToWorld;
                if (group.ContactCard != null && group.ContactCard.Emails.FirstOrDefault() != null)
                    g.EmailAddress = group.ContactCard.Emails.FirstOrDefault().EmailAddress;
                foreach (var logo in group.Logos)
                {
                    g.Logos.Add(new PhotoItem(logo.ImageUrl, logo.IsPrimaryPhoto, logo.AlternativeText));
                }
                var mems = group.GroupMembers.Where(x => x.IsMemRemoved == false).ToList();
                foreach (var member in mems)
                {
                    try
                    {
                        if (group.League.Members.Where(x => x.HasLeftLeague == false && member.Member != null && member.Member.MemberId == x.Member.MemberId).FirstOrDefault() != null)
                        {
                            RDN.Portable.Classes.League.Classes.LeagueGroupMember mem = new Portable.Classes.League.Classes.LeagueGroupMember();
                            mem.IsApartOfGroup = true;
                            mem.DerbyName = member.Member.DerbyName;
                            mem.PlayerNumber = member.Member.PlayerNumber;
                            mem.MemberId = member.Member.MemberId;
                            mem.Firstname = member.Member.Firstname;
                            mem.LastName = member.Member.Lastname;
                            mem.UserId = member.Member.AspNetUserId;
                            var email = member.Member.ContactCard.Emails.FirstOrDefault();
                            if (email != null)
                                mem.Email = email.EmailAddress;
                            if (member.Member.ContactCard.Communications.Count > 0 && member.Member.ContactCard.Communications.Where(x => x.IsDefault == true).Where(x => x.CommunicationTypeEnum == (byte)CommunicationTypeEnum.PhoneNumber).FirstOrDefault() != null)
                                mem.PhoneNumber = member.Member.ContactCard.Communications.Where(x => x.IsDefault == true).Where(x => x.CommunicationTypeEnum == (byte)CommunicationTypeEnum.PhoneNumber).FirstOrDefault().Data;
                            mem.Notes = member.Notes;
                            mem.DoesReceiveNewPostGroupNotifications = !member.TurnOffEmailNotifications;
                            mem.DoesReceiveGroupBroadcastNotifications = !member.TurnOffBroadcastNotifications;
                            mem.MemberAccessLevelEnum = (GroupMemberAccessLevelEnum)Enum.Parse(typeof(GroupMemberAccessLevelEnum), member.MemberAccessLevelEnum.ToString());
                            g.GroupMembers.Add(mem);
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }

                }
                return g;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return g;
        }
    }
}
