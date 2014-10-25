using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Context;
using RDN.Library.Classes.Error;
using System.IO;
using System.Web.Security;

namespace RDN.Library.Classes.Admin.Account
{
    public class Member
    {
        public static bool UnRetireMember(string userName)
        {
            try
            {
                var user = Membership.GetUser(userName);
                Guid userId = (Guid)user.ProviderUserKey;
                var dc = new ManagementContext();
                var member = dc.Members.Where(x => x.AspNetUserId == userId);
                foreach (var mem in member)
                {
                    mem.Retired = false;
                }
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool AttachUserToProfile(string userName, Guid profileId)
        {
            try
            {
                var user = Membership.GetUser(userName);
                Guid userId = (Guid)user.ProviderUserKey;
                var dc = new ManagementContext();
                var member = dc.Members.Where(x => x.AspNetUserId == userId);
                foreach (var mem in member)
                {
                    mem.AspNetUserId = new Guid();
                }
                int c = dc.SaveChanges();
                var memNew = dc.Members.Where(x => x.MemberId == profileId).FirstOrDefault();
                memNew.AspNetUserId = userId;
                c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool DeleteMember(Guid id)
        {
            try
            {
                var dc = new ManagementContext();
                var member = dc.Members.Where(x => x.MemberId == id).FirstOrDefault();
                var lmembers = dc.LeagueMembers.Where(x => x.Member.MemberId == id);
                List<long> memIds = new List<long>();
                foreach (var mem in lmembers)
                    memIds.Add(mem.Id);
                foreach (var memId in memIds)
                    dc.LeagueMembers.Remove(dc.LeagueMembers.Where(x => x.Id == memId).FirstOrDefault());
                memIds.Clear();

                var fmembers = dc.FederationMembers.Where(x => x.Member.MemberId == id);
                foreach (var mem in fmembers)
                    memIds.Add(mem.Id);
                foreach (var memId in memIds)
                    dc.FederationMembers.Remove(dc.FederationMembers.Where(x => x.Id == memId).FirstOrDefault());
                memIds.Clear();

                List<Guid> pId = new List<Guid>();
                var pmembers = dc.MemberPhotos.Where(x => x.Member.MemberId == id);
                foreach (var mem in pmembers)
                    pId.Add(mem.MemberPhotoId);
                foreach (var memId in pId)
                {
                    var photo = dc.MemberPhotos.Where(x => x.MemberPhotoId == memId).FirstOrDefault();
                    FileInfo file = new FileInfo(photo.SaveLocation);
                    if (file.Exists)
                        file.Delete();
                    dc.MemberPhotos.Remove(photo);
                }
                memIds.Clear();

                var atten = dc.CalendarAttendance.Where(x => x.Attendant.MemberId == id);
                foreach (var mem in atten)
                    memIds.Add(mem.CalendarAttendanceId);
                foreach (var memId in memIds)
                {
                    var photo = dc.CalendarAttendance.Where(x => x.CalendarAttendanceId == memId).FirstOrDefault();
                    dc.CalendarAttendance.Remove(photo);
                }
                memIds.Clear();

                var rec = dc.MessagesRecipients.Where(x => x.Recipient.MemberId == id);
                foreach (var mem in rec)
                    memIds.Add(mem.RecipientId);
                foreach (var memId in memIds)
                {
                    var photo = dc.MessagesRecipients.Where(x => x.RecipientId == memId).FirstOrDefault();
                    dc.MessagesRecipients.Remove(photo);
                }
                memIds.Clear();

                var inboc = dc.ForumInbox.Where(x => x.ToUser != null && x.ToUser.MemberId == id);
                foreach (var mem in inboc)
                    memIds.Add(mem.InboxTopicId);
                foreach (var memId in memIds)
                {
                    var photo = dc.ForumInbox.Where(x => x.InboxTopicId == memId).FirstOrDefault();
                    dc.ForumInbox.Remove(photo);
                }
                memIds.Clear();

                var messages = dc.MessageInbox.Where(x => x.ToUser != null && x.ToUser.MemberId == id);
                foreach (var mem in messages)
                    memIds.Add(mem.InboxMessageId);
                foreach (var memId in memIds)
                {
                    var photo = dc.MessageInbox.Where(x => x.InboxMessageId == memId).FirstOrDefault();
                    dc.MessageInbox.Remove(photo);
                }
                memIds.Clear();

                var fees = dc.FeesCollected.Where(x => x.MemberPaid != null && x.MemberPaid.MemberId == id);
                foreach (var mem in fees)
                    memIds.Add(mem.FeeCollectionId);
                foreach (var memId in memIds)
                {
                    var photo = dc.FeesCollected.Where(x => x.FeeCollectionId == memId).FirstOrDefault();
                    dc.FeesCollected.Remove(photo);
                }
                memIds.Clear();

                var contacts = member.MemberContacts;
                foreach (var mem in contacts)
                    memIds.Add(mem.ContactId);
                foreach (var memId in memIds)
                {
                    var photo = dc.MemberContacts.Where(x => x.ContactId == memId).FirstOrDefault();
                    dc.MemberContacts.Remove(photo);
                }
                memIds.Clear();

                dc.Members.Remove(member);
                dc.SaveChanges();
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;

        }
        public static bool DeleteTempMemberFromTwoEvils(Guid id)
        {
            try
            {
                var dc = new ManagementContext();
                var member = dc.ProfilesForTwoEvils.Where(x => x.ProfileId == id).FirstOrDefault();
                dc.ProfilesForTwoEvils.Remove(member);
                dc.SaveChanges();
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;

        }

    }
}
