using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Security;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Account.Enums;
using RDN.Library.Classes.Utilities;
using RDN.Library.DataModels.ContactCard;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Member;
using RDN.Library.Classes.Error;
using RDN.Utilities.Error;
using RDN.Library.DataModels.Federation;
using Scoreboard.Library.ViewModel;
using RDN.Library.Classes.Federation.Enums;
using RDN.Utilities.Config;
using System.IO;
using RDN.Library.DataModels.EmailServer.Enums;
using RDN.Library.DataModels.League;
using RDN.Library.Classes.Payment.Classes.Invoice;
using RDN.Utilities.Strings;
using RDN.Library.Classes.Account.Classes.Json;
using RDN.Library.Cache;
using RDN.Library.Classes.League.Enums;
using Scoreboard.Library.ViewModel.Members;
using RDN.Library.DataModels.Game.Members;
using RDN.Library.Classes.Communications.Enums;
using Scoreboard.Library.ViewModel.Members.Enums;
using RDN.Library.DataModels.Game.Officials;
using System.Drawing;
using System.Text.RegularExpressions;
using RDN.Portable.Config;
using RDN.Portable.Models.Json;
using System.Text;
using RDN.Portable.Classes.Account.Enums;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.ContactCard.Enums;
using RDN.Portable.Classes.Imaging;
using RDN.Portable.Classes.Communications.Enums;
using RDN.Portable.Classes.League.Enums;
using RDN.Portable.Classes.Federation;
using RDN.Portable.Classes.Games.Scoreboard;
using RDN.Portable.Classes.Games.Scoreboard.Game;
using System.Configuration;
using RDN.Library.Classes.Config;
using RDN.Portable.Classes.Url;
using RDN.Portable.Classes.Insurance;

namespace RDN.Library.Classes.Account
{
    public static class User
    {
        public static Regex htmlStrip = new Regex(@"<[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static bool UpdatePhotosOfMembers()
        {
            try
            {
                var dc = new ManagementContext();
                var email = dc.TeamLogos;
                foreach (var photo in email)
                {
                    try
                    {
                        photo.ImageUrlThumb = photo.ImageUrl.Replace(Path.GetFileNameWithoutExtension(photo.ImageUrl) + Path.GetExtension(photo.ImageUrl), "") + Path.GetFileNameWithoutExtension(photo.ImageUrl) + "_thumb" + Path.GetExtension(photo.ImageUrl);
                        photo.SaveLocationThumb = photo.SaveLocation.Replace(Path.GetFileNameWithoutExtension(photo.SaveLocation) + Path.GetExtension(photo.SaveLocation), "") + Path.GetFileNameWithoutExtension(photo.SaveLocation) + "_thumb" + Path.GetExtension(photo.ImageUrl);

                        if (!(new FileInfo(photo.SaveLocationThumb).Exists))
                        {
                            Image thumbImg = Image.FromFile(photo.SaveLocation);
                            Image thumb = RDN.Utilities.Drawing.Images.ScaleDownImage(thumbImg, 300, 300);

                            thumb.Save(photo.SaveLocationThumb);
                        }
                    }
                    catch (Exception exception)
                    {
                        dc.TeamLogos.Remove(photo);
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }
                dc.SaveChanges();

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool UpdateEmailForNonVerifiedUser(Guid verificationId, string emailNew)
        {
            try
            {
                var dc = new ManagementContext();
                var email = dc.EmailVerifications.Where(x => x.VerificationId == verificationId).FirstOrDefault();
                if (email != null)
                {
                    email.EmailAddress = emailNew;
                    email.Member = email.Member;
                    dc.SaveChanges();
                    return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static List<MemberDisplayBasic> GetAllNonVerifiedUsers()
        {
            List<MemberDisplayBasic> members = new List<MemberDisplayBasic>();
            var dc = new ManagementContext();
            var emails = dc.EmailVerifications.ToList();
            foreach (var user in emails)
            {
                MemberDisplayBasic mem = new MemberDisplayBasic();
                mem.Email = user.EmailAddress;
                mem.UserId = user.VerificationId;
                mem.DerbyName = user.Member.DerbyName;
                members.Add(mem);
            }
            return members;
        }

        public static bool DeleteUserByName(string userName)
        {
            try
            {
                var user = Membership.GetUser(userName, false);
                bool deleted = Membership.DeleteUser(user.UserName, true);
                if (deleted)
                {
                    var dc = new ManagementContext();
                    var id = new Guid(user.ProviderUserKey.ToString());
                    var member = dc.Members.Where(x => x.AspNetUserId == id).FirstOrDefault();
                    if (member != null)
                        member.AspNetUserId = new Guid();
                    int c = dc.SaveChanges();
                    return (c > 0);
                }
                return deleted;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;

        }
        /// <summary>
        /// makes the person connected to
        /// </summary>
        /// <param name="memId"></param>
        /// <returns></returns>
        public static bool ToggleConnectionToDerby(Guid memId)
        {
            try
            {
                var dc = new ManagementContext();
                var member = dc.Members.Where(x => x.MemberId == memId).FirstOrDefault();
                if (member != null)
                    member.IsNotConnectedToDerby = !member.IsNotConnectedToDerby;
                int c = dc.SaveChanges();
                return (c > 0);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;

        }

        /// <summary>
        /// search for finding names with autocomplete.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static List<MemberJsonAutoComplete> SearchDerbyNames(string q, int limit)
        {
            List<MemberJsonAutoComplete> names = new List<MemberJsonAutoComplete>();
            var dc = new ManagementContext();
            var name = (from xx in dc.Members
                        where xx.DerbyName.Contains(q)
                        select new
                        {
                            xx.DerbyName,
                            xx.MemberId,
                            xx.PlayerNumber
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
        public static List<MemberJson> SearchDerbyNamesJson(string q, int limit)
        {
            var dc = new ManagementContext();
            var name = (from xx in dc.Members
                        where xx.DerbyName.ToLower().Contains(q) || xx.Firstname.Contains(q) || xx.Lastname.Contains(q)
                        select new MemberJson
                        {
                            name = xx.DerbyName,
                            realname = (xx.Firstname + " " + xx.Lastname).Trim(),
                            id = xx.MemberId,
                            picture = xx.Photos.Where(x => x.IsPrimaryPhoto == true).FirstOrDefault() != null ? xx.Photos.Where(x => x.IsPrimaryPhoto == true).FirstOrDefault().ImageUrlThumb : "",
                        }).Take(limit).ToList();

            return name;
        }

        /// <summary>
        /// this method should only be called once.  TODO delete method once done with it.
        /// </summary>
        /// <param name="oldId"></param>
        /// <param name="newId"></param>
        public static void PutMemberIdsIntoUserIdColumns()
        {
            var dc = new ManagementContext();
            var members = dc.Members;
            foreach (var mem in members)
            {
                mem.AspNetUserId = mem.MemberId;
            }
            dc.SaveChanges();

        }
        /// <summary>
        /// changes the userId of the member.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="newUserId"></param>
        public static void ChangeUserIdOfMember(Guid memberId, Guid newUserId)
        {
            try
            {
                var dc = new ManagementContext();
                var member = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                member.AspNetUserId = newUserId;
                dc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        /// <summary>
        /// searches for the name and see if its already entered in the DB by someone else.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<MemberDisplayBasic> SearchForDerbyName(string name)
        {
            List<MemberDisplayBasic> members = new List<MemberDisplayBasic>();
            var dc = new ManagementContext();
            var list = dc.Members.Where(x => x.DerbyName.ToLower().Contains(name.ToLower())).Where(x => x.AspNetUserId == new Guid()).ToList();
            foreach (var mem in list)
            {
                MemberDisplayBasic basic = new MemberDisplayBasic();
                basic.DerbyName = mem.DerbyName;
                basic.Firstname = mem.Firstname;
                basic.MemberId = mem.MemberId;
                basic.PlayerNumber = mem.PlayerNumber;
                members.Add(basic);
            }
            return members;

        }

        /// <summary>
        /// sends an email to the user the requests the lost password.
        /// </summary>
        /// <param name="user"></param>
        public static bool UserLostPassword(MembershipUser user)
        {
            try
            {
                var dc = new ManagementContext();
                Member mem = GetMember(ref dc, user.UserName);
                if (mem != null)
                {
                    Guid verificationCode = AddUserEmailVerification(ref dc, ref mem, user.UserName);
                    SendEmailForLostPassword(verificationCode, user.UserName, mem.DerbyName);
                    return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool ChangeUserName(Guid userId, string userName, string derbyName)
        {
            try
            {
                if (!String.IsNullOrEmpty(userName))
                {
                    using (var context = new MembershipModel())
                    {
                        // Get the membership record from the database
                        var membershipUser = context.aspnet_Users.Where(u => u.UserId == userId).FirstOrDefault();

                        if (membershipUser != null)
                        {
                            // Ensure that the new user name is not already being used
                            string newUserNameLowered = userName.ToLower();
                            if (!context.aspnet_Users.Any(u => u.LoweredUserName == newUserNameLowered))
                            {
                                membershipUser.UserName = userName;
                                membershipUser.LoweredUserName = newUserNameLowered;
                                int c = context.SaveChanges();
                                if (c > 0)
                                {
                                    var user = Membership.GetUser((object)userId);
                                    string oldEmail = user.Email;
                                    user.Email = userName;
                                    Membership.UpdateUser(user);
                                    SendEmailForEmailUsernameChanged(userName, oldEmail, derbyName);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: userName);
            }
            return false;
        }
        public static bool ChangeUserPassword(Guid userId, string oldPassword, string newPassword, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                var user = Membership.GetUser((object)userId);
                bool changed = user.ChangePassword(oldPassword, newPassword);
                if (changed)
                {
                    var tempUser = GetMember(user.UserName);
                    SendEmailForPasswordChanged(user.Email, tempUser.DerbyName);
                }
                return changed;
            }
            catch (Exception exception)
            {
                if (exception.Message.Contains("cannot be null"))
                {
                    errorMessage = "Please fill all the fields";
                }
                else if (exception.Message.Contains("cannot be empty"))
                {
                    errorMessage = "Please fill all the fields";
                }
                else if (exception.Message.Contains("length of parameter"))
                {
                    errorMessage = "The new password must have at least 4 characters";
                }
                else
                {
                    errorMessage = exception.Message;
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
            }
            return false;
        }

        public static bool ChangeUserPassword(MembershipUser user, string newPassword, string verificationId)
        {
            try
            {
                string newPass = string.Empty;
                try
                {
                    newPass = user.ResetPassword();
                }
                catch (Exception exception)
                {
                    if (exception.Message.Contains("user account has been locked out"))
                    {
                        user.UnlockUser();
                        newPass = user.ResetPassword();
                    }
                    else
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
                bool changed = user.ChangePassword(newPass, newPassword);
                var tempUser = GetMember(user.UserName);

                SendEmailForPasswordChanged(user.Email, tempUser.DerbyName);
                var dc = new ManagementContext();
                var verify = dc.EmailVerifications.Where(x => x.VerificationId == new Guid(verificationId)).FirstOrDefault();
                if (verify != null)
                {
                    dc.EmailVerifications.Remove(verify);
                    int c = dc.SaveChanges();
                }
                return changed;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());

            }
            return false;
        }
        public static bool ChangeUserPassword(string EmailOfUser, string newPassword)
        {
            try
            {
                MembershipUser user = Membership.GetUser(EmailOfUser);
                if (user == null)
                    return false;
                string newPass = user.ResetPassword();
                bool changed = user.ChangePassword(newPass, newPassword);
                if (changed)
                {
                    var tempUser = GetMember(user.UserName);
                    SendEmailForPasswordChanged(user.Email, tempUser.DerbyName);
                }
                return changed;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool UnlockAccount(string EmailOfUser)
        {
            try
            {
                MembershipUser user = Membership.GetUser(EmailOfUser);
                return user.UnlockUser();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        /// <summary>
        /// creates a member for the federation
        /// </summary>
        /// <param name="member"></param>
        /// <param name="federationId"></param>
        public static void CreateMemberForFederation(MemberDisplay member, Guid federationId)
        {
            try
            {
                var dc = new ManagementContext();
                // ContactLeague is set to null. We need to see if the leagueId is filled out and if so, if that league exists
                DataModels.League.League league = null;
                Guid teamId;
                if (Guid.TryParse(member.Team, out teamId))
                {
                    league = dc.Leagues.FirstOrDefault(x => x.LeagueId.Equals(teamId));
                }
                DataModels.Federation.Federation federation = dc.Federations.Where(x => x.FederationId == federationId).FirstOrDefault();
                var dbMember = CreateMemberAccount(ref dc, ref league, new Guid(), member.Firstname, member.DerbyName, Convert.ToInt32(member.Gender), 0, member.LastName, member.PlayerNumber, member.Email, member.PhoneNumber);

                //if the federation is creating the member, we need to create a member of the federation.
                FederationMember memberFed = new FederationMember
                {
                    Member = dbMember,
                    Federation = federation,
                    MADEClassRankForMember = String.IsNullOrEmpty(member.MadeClassRank) ? 0 : Convert.ToInt32(member.MadeClassRank),
                    MemberType = 0,
                    FederationIdForMember = member.FederationIdForMember.ToString()
                };

                dbMember.Federations.Add(memberFed);
                var result = dc.SaveChanges();

                // The save resultet in 0 records. Indicating that something went wrong. Delete the already inserted username in the membership system and return
                if (result == 0)
                {

                }
                else
                {
                    AddMemberLog(dbMember.MemberId, "(manager) Account created for Federation", string.Format("The user account was created successfully with the following data:\nFirstname: {0}\nDerbyname:{1}\nEmail:{2}", member.Firstname, member.DerbyName, member.Email), MemberLogEnum.SystemDataAdded);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        /// <summary>
        /// creates a member for the league
        /// </summary>
        /// <param name="member"></param>
        /// <param name="federationId"></param>
        public static void CreateMemberForLeague(MemberDisplay member, Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                // ContactLeague is set to null. We need to see if the leagueId is filled out and if so, if that league exists
                DataModels.League.League league = dc.Leagues.Where(x => x.LeagueId == leagueId).FirstOrDefault();
                var dbMember = CreateMemberAccount(ref dc, ref league, new Guid(), member.Firstname.Trim(), member.DerbyName.Trim(), Convert.ToInt32(member.Gender), 0, member.LastName, member.PlayerNumber, member.Email.Trim(), member.PhoneNumber);
                var result = dc.SaveChanges();

                SendEmailInviteOnProfileCreation(member.DerbyName, league.Name, dbMember.MemberId, member.Email);

                // The save resultet in 0 records. Indicating that something went wrong. Delete the already inserted username in the membership system and return
                if (result == 0)
                { }
                else
                {
                    AddMemberLog(dbMember.MemberId, "member Account created for League", string.Format("The user account was created successfully with the following data:\nFirstname: {0}\nDerbyname:{1}\nEmail:{2}", member.Firstname, member.DerbyName, member.Email), MemberLogEnum.SystemDataAdded);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        public static string SendEmailInviteOnProfileCreation(string leagueName, Guid memberId)
        {
            try
            {
                var mem = GetMemberDisplay(memberId);
                if (!String.IsNullOrEmpty(mem.Email))
                {
                    return SendEmailInviteOnProfileCreation(mem.DerbyName, leagueName, memberId, mem.Email);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return String.Empty;
        }
        public static string SendEmailInviteOnProfileCreation(string derbyName, string leagueName, Guid memberId, string memberEmail)
        {
            try
            {
                string link = LibraryConfig.PublicSite + UrlManager.WEBSITE_VALIDATE_DERBY_NAME + memberId.ToString().Replace("-", "") + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(derbyName);
                var emailData = new Dictionary<string, string> {
                        { "derbyname", derbyName },
                        { "leaguename", leagueName},
                                { "WebsiteName", LibraryConfig.WebsiteName},
                        { "link", link  } };

                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, memberEmail, LibraryConfig.DefaultEmailSubject + " " + LibraryConfig.NameOfMember + " Profile Created", emailData, layout: EmailServer.EmailServerLayoutsEnum.LeagueCreatedMemberProfile, priority: EmailPriority.Normal);
                return link;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return String.Empty;
        }


        public static List<NewUserEnum> VerifyDerbyNameAndCreateAccount(Guid memberId, string email, string repeatEmail, string password, string ip)
        {
            List<NewUserEnum> newUserConfirm = new List<NewUserEnum>();
            try
            {
                MemberDisplay member = RDN.Library.Classes.Account.User.GetMemberDisplay(memberId);
                //verifying the right person is signing up to the right acount.

                if (!String.IsNullOrEmpty(member.Email) && member.Email.Trim().ToLower() != email.Trim().ToLower())
                    newUserConfirm.Add(NewUserEnum.Email_EmailNotRightIncorrect);





                if (member.UserId != new Guid())
                    newUserConfirm.Add(NewUserEnum.User_AlreadyAttachedToAccount);

                if (newUserConfirm.Count > 0)
                    return newUserConfirm;


                var dc = new ManagementContext();
                var memberDb = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();

                // Create an asp.net membership account
                var userId = CreateMemberMembershipAccount(email, password, email);
                if (!userId.HasValue)
                {
                    newUserConfirm.Add(NewUserEnum.Error_Save);
                    return newUserConfirm;
                }
                else if (userId.Value == new Guid())
                {
                    newUserConfirm.Add(NewUserEnum.Email_InUse);
                    return newUserConfirm;
                }
                ChangeUserIdOfMember(memberId, userId.Value);

                // Generate an email validation guid code
                var emailVerificationCode = AddUserEmailVerification(ref dc, ref memberDb, email);

                // The user will not be added to the role member until he has verified his email.
                var result = dc.SaveChanges();


                SendEmailVerification(emailVerificationCode, email, member.DerbyName, password);
                AddMemberLog(memberId, "(manager) Account created", string.Format("The user account was created successfully with the following data:\nDerbyname:{0}\nEmail:{1}", member.DerbyName, email), MemberLogEnum.SystemDataAdded);


            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: memberId + ":" + email + ":" + repeatEmail + ":" + password);
            }

            return newUserConfirm;
        }


        /// <summary>
        /// creates a new member during the sign up process.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="repeatEmail"></param>
        /// <param name="password"></param>
        /// <param name="firstname"></param>
        /// <param name="derbyName"></param>
        /// <param name="gender"></param>
        /// <param name="positionType"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static List<NewUserEnum> CreateMember(string email, string repeatEmail, string password, string firstname, string derbyName, int gender, int positionType, string ip)
        {
            return CreateMember(email, repeatEmail, password, firstname, derbyName, gender, positionType, ip, null);
        }
        /// <summary>
        /// creates a new user during the sign up process.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="repeatEmail"></param>
        /// <param name="password"></param>
        /// <param name="firstname"></param>
        /// <param name="derbyName"></param>
        /// <param name="gender"></param>
        /// <param name="positionType"></param>
        /// <param name="ip"></param>
        /// <param name="leagueId"></param>
        /// <returns></returns>
        public static List<NewUserEnum> CreateMember(string email, string repeatEmail, string password, string firstname, string derbyName, int gender, int positionType, string ip, Guid? leagueId)
        {

            // Check the data against validators to see that the required information has been filled out
            var output = CheckCreateNewMember(email, repeatEmail, password, firstname, derbyName, positionType, gender);
            try
            {
                // Error found, return
                if (output.Count > 0)
                    return output;

                var dc = new ManagementContext();
                // ContactLeague is set to null. We need to see if the leagueId is filled out and if so, if that league exists
                DataModels.League.League league = null;

                if (leagueId.HasValue)
                {
                    league = dc.Leagues.FirstOrDefault(x => x.LeagueId.Equals(leagueId.Value));
                    if (league == null)
                    {
                        output.Add(NewUserEnum.League_LeagueNotFound);
                        return output;
                    }
                }

                // Create an asp.net membership account
                var memberId = CreateMemberMembershipAccount(email, password, email);
                if (!memberId.HasValue)
                {
                    output.Add(NewUserEnum.Error_Save);
                    return output;
                }

                // Create a member account
                var member = CreateMemberAccount(ref dc, ref league, memberId.Value, firstname, derbyName, gender, positionType, string.Empty, string.Empty, email, string.Empty);
                // Generate an email validation guid code
                var emailVerificationCode = AddUserEmailVerification(ref dc, ref member, email);

                // The user will not be added to the role member until he has verified his email.
                // The save resultet in 0 records. Indicating that something went wrong. Delete the already inserted username in the membership system and return
                SendEmailVerification(emailVerificationCode, email, derbyName, password);

                if (positionType == Convert.ToInt32(DefaultPositionEnum.Writer_For_Sport_News))
                {
                    SendEmailAboutNewRollinNewsWriter(email, derbyName);
                }

                dc.SaveChanges();
                AddMemberLog(memberId.Value, "(manager) Account created", string.Format("The user account was created successfully with the following data:\nFirstname: {0}\nDerbyname:{1}\nEmail:{2}", firstname, derbyName, email), MemberLogEnum.SystemDataAdded);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return output;
        }

        public static List<NewUserEnum> CreateUserNotConnectedToDerby(string email, string repeatEmail, string password)
        {

            var output = new List<NewUserEnum>();
            try
            {
                if (!String.IsNullOrEmpty(email))
                    email = email.Trim();
                if (!String.IsNullOrEmpty(repeatEmail))
                    repeatEmail = repeatEmail.Trim();

                if (string.IsNullOrEmpty(email))
                    output.Add(NewUserEnum.Email_IsEmpty);

                if (!EmailValidator.Validate(email))
                    output.Add(NewUserEnum.Email_IllegalCharacters);

                if (!email.Equals(repeatEmail))
                    output.Add(NewUserEnum.Email_EmailRepeatIncorrect);

                password = password.Trim();
                if (string.IsNullOrEmpty(password))
                    output.Add(NewUserEnum.Password_IsEmpty);

                if (password.Length < 6)
                    output.Add(NewUserEnum.Password_TooShort);

                // Check the asp.net membership management to see if the email is taken
                if (!string.IsNullOrEmpty(Membership.GetUserNameByEmail(email)))
                    output.Add(NewUserEnum.Email_InUse);

                // Error found, return
                if (output.Count > 0)
                    return output;

                var dc = new ManagementContext();
                // ContactLeague is set to null. We need to see if the leagueId is filled out and if so, if that league exists
                // Create an asp.net membership account
                var memberId = CreateMemberMembershipAccount(email, password, email);
                if (!memberId.HasValue)
                {
                    output.Add(NewUserEnum.Error_Save);
                    return output;
                }

                // Create a member account
                var member = CreateMemberAccountForNonDerbyFolks(ref dc, memberId.Value, email);
                // Generate an email validation guid code
                var emailVerificationCode = AddUserEmailVerification(ref dc, ref member, email);
                dc.SaveChanges();
                // The user will not be added to the role member until he has verified his email.
                // The save resultet in 0 records. Indicating that something went wrong. Delete the already inserted username in the membership system and return
                SendEmailVerification(emailVerificationCode, email, email, password);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: email + ":" + repeatEmail + ":" + password);
            }
            return output;
        }

        private static List<NewUserEnum> CheckCreateNewMember(string email, string repeatEmail, string password, string firstname, string derbyName, int positionType, int gender)
        {
            // Check required fields
            var output = new List<NewUserEnum>();
            try
            {

                if (string.IsNullOrEmpty(email))
                {
                    output.Add(NewUserEnum.Email_IsEmpty);
                    return output;
                }
                else
                    email = email.Trim();

                if (!EmailValidator.Validate(email))
                    output.Add(NewUserEnum.Email_IllegalCharacters);



                if (positionType == 0)
                    output.Add(NewUserEnum.Position_IsntSelected);

                if (!email.Equals(repeatEmail))
                    output.Add(NewUserEnum.Email_EmailRepeatIncorrect);
                else
                    repeatEmail = repeatEmail.Trim();

                if (string.IsNullOrEmpty(password))
                    output.Add(NewUserEnum.Password_IsEmpty);
                else
                    if (password.Length < 6)
                    output.Add(NewUserEnum.Password_TooShort);


                if (string.IsNullOrEmpty(firstname))
                    output.Add(NewUserEnum.Firstname_IsEmpty);
                else
                    firstname = firstname.Trim();

                if (firstname.Length < 2)
                    output.Add(NewUserEnum.Firstname_TooShort);

                if (string.IsNullOrEmpty(derbyName))
                    output.Add(NewUserEnum.Nickname_IsEmpty);

                // Check the asp.net membership management to see if the email is taken
                if (!string.IsNullOrEmpty(Membership.GetUserNameByEmail(email)))
                    output.Add(NewUserEnum.Email_InUse);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: email + ":" + repeatEmail + ":" + firstname + ":" + password + ":" + derbyName);
            }
            return output;
        }

        private static Guid? CreateMemberMembershipAccount(string username, string password, string email)
        {
            try
            {
                MembershipCreateStatus createStatus;
                var memberCard = Membership.CreateUser(username, password, email, "secret question", "secret answer", true, out createStatus);

                if (createStatus == MembershipCreateStatus.DuplicateUserName)
                    return new Guid();
                if (createStatus != MembershipCreateStatus.Success || memberCard == null)
                    return null;
                //adds user to the registered role for YAF.
                Roles.AddUserToRole(memberCard.UserName, "Registered");

                return (Guid)memberCard.ProviderUserKey;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }

        private static DataModels.Member.Member CreateMemberAccount(ref ManagementContext dc, ref DataModels.League.League league, Guid userId, string firstname, string derbyName, int gender, int positionType, string lastName, string playerNumber, string email, string phoneNumber)
        {
            // Creates the member record (also adds the user to the league specified if one was found)
            Member member = new DataModels.Member.Member();
            try
            {
                member.MemberId = Guid.NewGuid();
                member.AspNetUserId = userId;
                member.Firstname = firstname;
                member.Lastname = lastName;
                member.PlayerNumber = playerNumber;
                member.DerbyName = derbyName;
                member.Gender = gender;
                member.PositionType = positionType;
                member.ContactCard = new DataModels.ContactCard.ContactCard();
                member.IsVerified = false;

                if (league != null)
                {
                    LeagueMember lm = new LeagueMember();
                    lm.Member = member;
                    lm.League = league;
                    lm.MembershipDate = DateTime.UtcNow;
                    member.Leagues.Add(lm);
                    member.League = league;
                    member.CurrentLeagueId = league.LeagueId;
                }

                if (!String.IsNullOrEmpty(email))
                {
                    member.ContactCard.Emails.Add(new RDN.Library.DataModels.ContactCard.Email { EmailAddress = email, IsDefault = true });

                }
                if (!String.IsNullOrEmpty(phoneNumber))
                {
                    int phoneType = Convert.ToInt32(CommunicationTypeEnum.PhoneNumber);
                    member.ContactCard.Communications.Add(new Communication
                    {
                        Data = phoneNumber,
                        IsDefault = true,
                        //CommunicationType = dc.CommunicationType.Where(x => x.CommunicationTypeId == phoneType).FirstOrDefault()
                        CommunicationTypeEnum = (byte)CommunicationTypeEnum.PhoneNumber
                    });
                }

                member = dc.Members.Add(member);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return member;
        }

        private static DataModels.Member.Member CreateMemberAccountForNonDerbyFolks(ref ManagementContext dc, Guid userId, string email)
        {
            // Creates the member record (also adds the user to the league specified if one was found)
            Member member = new DataModels.Member.Member();
            try
            {
                member.MemberId = Guid.NewGuid();
                member.AspNetUserId = userId;
                member.ContactCard = new DataModels.ContactCard.ContactCard();
                member.IsVerified = false;
                member.IsNotConnectedToDerby = true;

                if (!String.IsNullOrEmpty(email))
                {
                    member.ContactCard.Emails.Add(new RDN.Library.DataModels.ContactCard.Email { EmailAddress = email, IsDefault = true });

                }

                dc.Members.Add(member);
                int c = dc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return member;
        }


        private static Guid AddUserEmailVerification(ref ManagementContext dc, ref DataModels.Member.Member member, string email)
        {
            var emailVerificationCode = Guid.NewGuid();
            try
            {
                // Add email verification
                var emailVerification = new EmailVerification
                {
                    EmailAddress = email,
                    Expires = DateTime.Now.AddDays(7),
                    VerificationId = emailVerificationCode,
                    Member = member
                };
                dc.EmailVerifications.Add(emailVerification);
                dc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return emailVerificationCode;
        }
        private static void SendEmailForPasswordChanged(string email, string derbyName)
        {
            try
            {
                var emailData = new Dictionary<string, string>
                                        {
                                            { "name", derbyName }
                                          };
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email, LibraryConfig.DefaultEmailSubject + " Password Changed", emailData, EmailServer.EmailServerLayoutsEnum.PasswordChanged);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: " email:" + email);
            }
        }
        private static void SendEmailForEmailUsernameChanged(string email, string oldEmail, string derbyName)
        {
            try
            {
                var emailData = new Dictionary<string, string>
                                        {
                                            { "name", derbyName },
                                            { "oldEmail", oldEmail},
                                            { "newEmail", email}

                                          };


                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email, LibraryConfig.DefaultEmailSubject + " Username/Email Changed", emailData, EmailServer.EmailServerLayoutsEnum.UsernameChanged);
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, oldEmail, LibraryConfig.DefaultEmailSubject + " Username/Email Changed", emailData, EmailServer.EmailServerLayoutsEnum.UsernameChanged);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: " email:" + email);
            }
        }


        private static void SendEmailForLostPassword(Guid emailVerificationCode, string email, string derbyName)
        {
            try
            {
                var emailData = new Dictionary<string, string>
                                        {
                                            { "name", derbyName },
                                            { "email", email },
                                            { "code", emailVerificationCode.ToString() },
                                            { "validationurl", LibraryConfig.PublicSite + UrlManager.WEBSITE_LOST_PASSWORD_RESET_LOCATION }
                                        };

                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email, LibraryConfig.DefaultEmailSubject + " Recover Password", emailData, EmailServer.EmailServerLayoutsEnum.RecoverLostPassword);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: "code:" + emailVerificationCode.ToString() + " email:" + email);
            }
        }
        private static bool SendEmailAboutNewRollinNewsWriter(string email, string derbyName)
        {
            try
            {

                var emailData = new Dictionary<string, string>
                                        {
                                            { "name", derbyName },
                                            { "email", email },
                                            { "text", "Please Go to Rollin News and Add as Contributor"},
 { "link", RollinNewsConfig.WEBSITE_DEFAULT_LOCATION},

                                        };
                var users = Roles.GetUsersInRole(RolesConfig.CHIEF_ROLE);

                foreach (var user in users)
                {
                    return EmailServer.EmailServer.SendEmail(RollinNewsConfig.DEFAULT_EMAIL, RollinNewsConfig.DEFAULT_EMAIL_FROM_NAME, user, LibraryConfig.DefaultEmailSubject + " New Writer Registered", emailData, EmailServer.EmailServerLayoutsEnum.RNWriterRegistered);
                }



            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: " email:" + email);
            }
            return false;
        }

        private static bool SendEmailVerification(Guid emailVerificationCode, string email, string derbyName, string password = null)
        {
            try
            {

                if (password == null)
                {
                    var emailData = new Dictionary<string, string>
                                        {
                                            { "name", derbyName },
                                            { "email", email },
                                            { "code", emailVerificationCode.ToString() },
                                            { "validationurl", LibraryConfig.InternalSite + UrlManager.WEBSITE_VALIDATE_ACCOUNT_WITH_EMAIL_LOCATION +"/"+ emailVerificationCode.ToString().Replace("-","") }
                                        };
                    return EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email, LibraryConfig.DefaultEmailSubject + " Validate your account", emailData, EmailServer.EmailServerLayoutsEnum.SendEmailVerificationWithoutPassword);
                }
                else
                {
                    var emailData = new Dictionary<string, string>
                                        {
                                            { "name", derbyName },
                                            { "email", email },
                                            { "code", emailVerificationCode.ToString().Replace("-","") },
                                            { "validationurl", LibraryConfig.InternalSite + UrlManager.WEBSITE_VALIDATE_ACCOUNT_WITH_EMAIL_LOCATION +"/"+ emailVerificationCode.ToString().Replace("-","") },
                                            { "password", password }
                                        };
                    return EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, email, LibraryConfig.DefaultEmailSubject + " Validate your account", emailData, EmailServer.EmailServerLayoutsEnum.SendEmailVerificationWithoutPassword);
                }


            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: "code:" + emailVerificationCode.ToString() + " email:" + email);
            }
            return false;
        }

        public static bool CheckLostPasswordVerificationCode(string emailVerificationCode, string email)
        {
            try
            {
                // Try to get the post from the database using the specified code and email
                var dc = new ManagementContext();
                var record = dc.EmailVerifications.Where(x => x.VerificationId == new Guid(emailVerificationCode)).Where(x => x.EmailAddress.ToLower() == email.ToLower()).FirstOrDefault();
                if (record == null) // No post found, return
                    return false;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return true;
        }


        /// <summary>
        /// Verifies that the code and email that was sent to the user matches the database entries.
        /// </summary>
        /// <param name="emailVerificationCode"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static List<VerifyEmailEnum> VerifyEmailVerification(string emailVerificationCode, string ip)
        {
            // Validate that the code and email are correct
            var output = new List<VerifyEmailEnum>();
            try
            {
                Guid verificationCode;
                var isCodeGuid = Guid.TryParse(emailVerificationCode, out verificationCode);
                if (!isCodeGuid)
                    output.Add(VerifyEmailEnum.Code_Invalid);

                // Something was wrong, return
                if (output.Count > 0)
                    return output;

                // Try to get the post from the database using the specified code and email
                var dc = new ManagementContext();
                var record = dc.EmailVerifications.Include("Member").FirstOrDefault(x => x.VerificationId.Equals(verificationCode));
                if (record == null) // No post found, return
                {
                    output.Add(VerifyEmailEnum.Code_Email_Invalid);
                    return output;
                }
                var memberId = record.Member.MemberId;

                var memberEmail = new RDN.Library.DataModels.ContactCard.Email
                {
                    ContactCard = record.Member.ContactCard,
                    EmailAddress = record.EmailAddress,
                    IsDefault = true
                };

                // Set all (current) emails default to false. Default means that this is this member primary contact email.
                var emails = record.Member.ContactCard.Emails.Where(x => x.IsDefault.Equals(true));
                foreach (var emailObj in emails)
                    emailObj.IsDefault = false;

                // Add the new email and also verify the member if the member is unverified
                record.Member.ContactCard.Emails.Add(memberEmail);
                record.Member.IsVerified = true;

                // Remove the verification email
                dc.EmailVerifications.Remove(record);

                var result = dc.SaveChanges();
                if (result == 0)
                {
                    output.Add(VerifyEmailEnum.Error_Save);
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return output;
        }

        public static int ResendAllEmailVerificationsInQueue()
        {
            int emailsSent = 0;
            try
            {
                var dc = new ManagementContext();
                var memberIds = dc.EmailVerifications.Include("Member").ToList();

                foreach (var mem in memberIds)
                {
                    try
                    {
                        ResendEmailVerification(mem.Member.MemberId);
                        emailsSent += 1;
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return emailsSent;
        }

        public static bool ResendEmailVerification()
        {
            try
            {
                return ResendEmailVerification(GetMemberId());
            }
            catch (Exception exception)
            { ErrorDatabaseManager.AddException(exception, exception.GetType()); }
            return false;
        }

        public static bool ResendEmailVerification(Guid memberId)
        {
            try
            {
                // Get the member
                var member = GetMemberWithMemberId(memberId);
                if (member == null)
                    return false;

                // get the email verification code, name and email
                var dc = new ManagementContext();
                var emailVerification = dc.EmailVerifications.Include("Member").FirstOrDefault(x => x.Member.MemberId.Equals(memberId));
                if (emailVerification == null)
                    return false;

                var firstname = member.Firstname;
                var derbyName = member.DerbyName;
                var email = emailVerification.EmailAddress;
                var emailCode = emailVerification.VerificationId;
                var mem = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                if (mem == null)
                    return false;
                emailVerification.Member = mem;

                // Update the expired date
                emailVerification.Expires = DateTime.Now.AddDays(100);
                dc.SaveChanges();
                // Send the email
                SendEmailVerification(emailCode, email, derbyName);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return true;
        }

        /// <summary>
        /// Gets the league id of the current logged in member
        /// </summary>
        /// <returns></returns>
        public static List<RDN.Portable.Classes.League.Classes.League> GetLeaguesOfMember(Guid memId)
        {
            List<RDN.Portable.Classes.League.Classes.League> les = new List<RDN.Portable.Classes.League.Classes.League>();
            try
            {
                var dc = new ManagementContext();
                var leagues = dc.LeagueMembers.Where(x => x.Member.MemberId == memId).Select(x => x.League).ToList();

                for (int i = 0; i < leagues.Count; i++)
                {
                    les.Add(League.LeagueFactory.DisplayLeague(leagues[i]));
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return les;
        }

        public static bool AddMemberToLeague(Guid memberId, Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var mem = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                if (mem != null)
                {
                    mem.CurrentLeagueId = leagueId;
                    mem.IsNotConnectedToDerby = false;
                    var le = dc.Leagues.Where(x => x.LeagueId == leagueId).FirstOrDefault();
                    LeagueMember l = new LeagueMember();
                    l.League = le;
                    l.Member = mem;
                    l.LeagueOwnersEnums = (int)LeagueOwnersEnum.Owner;
                    l.MembershipDate = DateTime.UtcNow;
                    mem.Leagues.Add(l);
                    var u = dc.SaveChanges();
                    if (u == 0)
                        throw new Exception("Member Didn't Get Added " + memberId + " " + leagueId);
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
        /// Gets the member id of the current logged in member, not to be confused with the user id for the membership provider
        /// </summary>
        /// <returns></returns>
        public static Guid GetMemberId()
        {
            try
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session["MemberId"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["MemberId"].ToString()))
                    return (Guid)HttpContext.Current.Session["MemberId"];

                var user = Membership.GetUser(HttpContext.Current.User.Identity.Name);
                if (user == null)
                    return new Guid();
                var member = GetMemberWithUserId((Guid)user.ProviderUserKey);
                if (member != null)
                {
                    if (HttpContext.Current.Session != null)
                        HttpContext.Current.Session["MemberId"] = member.MemberId;
                    return member.MemberId;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }
        public static Guid GetMemberId(string userName)
        {
            try
            {

                var user = Membership.GetUser(userName);
                if (user == null)
                    return new Guid();
                var member = GetMemberWithUserId((Guid)user.ProviderUserKey);
                if (member != null)
                {
                    return member.MemberId;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }
        public static void ClearMemberId()
        {
            try
            {
                HttpContext.Current.Session.Remove("MemberId");
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

        }
        public static Guid SwitchMemberId(string emailAddress)
        {
            try
            {
                var user = Membership.GetUser(emailAddress);
                var member = GetMemberWithUserId((Guid)user.ProviderUserKey);
                if (HttpContext.Current.Session != null)
                    HttpContext.Current.Session["MemberId"] = member.MemberId;
                return member.MemberId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }


        public static Guid GetUserId()
        {
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["UserId"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["UserId"].ToString()))
                return (Guid)HttpContext.Current.Session["UserId"];

            var member = Membership.GetUser(HttpContext.Current.User.Identity.Name);
            if (member == null)
                return new Guid();
            if (HttpContext.Current.Session != null)
                HttpContext.Current.Session["UserId"] = (Guid)member.ProviderUserKey;
            return (Guid)member.ProviderUserKey;
        }

        /// <summary>
        /// Gets the member entity
        /// </summary>                
        /// <returns></returns>
        public static DataModels.Member.Member GetMember()
        {
            return GetMemberWithMemberId(GetMemberId());
        }

        /// <summary>
        /// Gets the member entity
        /// </summary>        
        /// <param name="username"></param>
        /// <returns></returns>
        public static DataModels.Member.Member GetMember(string username)
        {
            var user = Membership.GetUser(username);

            if (user == null)
                return null;

            var userId = (Guid)user.ProviderUserKey;
            return GetMemberWithUserId(userId);
        }

        /// <summary>
        /// Gets the member entity with the user id from the ASPNET tables.
        /// </summary>        
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DataModels.Member.Member GetMemberWithUserId(Guid userId)
        {
            var dc = new ManagementContext();
            return dc.Members.FirstOrDefault(x => x.AspNetUserId.Equals(userId));
        }
        public static DataModels.Member.Member GetMemberWithLoginEmail(string email)
        {
            MembershipUser user = Membership.GetUser(email);
            Guid userId = new Guid(user.ProviderUserKey.ToString());
            var dc = new ManagementContext();
            return dc.Members.FirstOrDefault(x => x.AspNetUserId.Equals(userId));
        }
        /// <summary>
        /// gets number of members in our system that are public facing.
        /// </summary>
        /// <returns></returns>
        public static int GetNumberOfPublicMembers()
        {
            var dc = new ManagementContext();
            return dc.Members.Where(x => x.IsProfileRemovedFromPublic == false).Count();
        }
        /// <summary>
        /// get the member with the member id.
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static DataModels.Member.Member GetMemberWithMemberId(Guid memberId)
        {
            var dc = new ManagementContext();
            return dc.Members.FirstOrDefault(x => x.MemberId.Equals(memberId));
        }

        /// <summary>
        /// Gets the member entity using the specified context
        /// </summary>
        /// <param name="dc"></param>        
        /// <returns></returns>
        internal static DataModels.Member.Member GetMember(ref ManagementContext dc)
        {
            return GetMemberWithMemberId(ref dc, GetMemberId());
        }

        /// <summary>
        /// Gets the member entity using the specified context
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        internal static DataModels.Member.Member GetMember(ref ManagementContext dc, string username)
        {
            var user = Membership.GetUser(username);
            if (user == null)
                return null;

            var userId = (Guid)user.ProviderUserKey;
            return GetMemberWithUserId(ref dc, userId);
        }

        public static List<SkaterJson> GetAllPublicMembers(int recordsToSkip, int numberOfRecordsToPull)
        {
            List<SkaterJson> membersTemp = new List<SkaterJson>();
            try
            {
                var mems = SiteCache.GetAllPublicMembers();
                var m = mems.Skip(recordsToSkip).Take(numberOfRecordsToPull).ToList();

                return m;

                //membersTemp.AddRange(MemberDisplay.IterateMembersForDisplay(members));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return membersTemp;
        }

        public static List<SkaterJson> GetAllPublicMembers()
        {
            List<SkaterJson> membersTemp = new List<SkaterJson>();
            try
            {
                var dc = new ManagementContext();
                var members = dc.Members.Include("Leagues.League.Logo").Where(x => x.IsNotConnectedToDerby == false && x.IsProfileRemovedFromPublic == false).ToList().OrderBy(x => x.DerbyName);

                List<SkaterJson> mems = new List<SkaterJson>();
                try
                {
                    foreach (var mem in members)
                    {
                        SkaterJson m = DisplaySkaterJson(mem);
                        if (!String.IsNullOrEmpty(m.DerbyName))
                            mems.Add(m);
                    }
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }


                membersTemp.AddRange(mems);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return membersTemp;
        }

        private static SkaterJson DisplaySkaterJson(Member mem)
        {
            SkaterJson m = new SkaterJson();
            var photos = mem.Photos.OrderByDescending(x => x.Created).Where(x => x.IsPrimaryPhoto == true).FirstOrDefault();
            if (photos != null)
            {
                m.photoUrl = photos.ImageUrl;
                m.ThumbUrl = photos.ImageUrlThumb;
            }
            else if (mem.Gender == (int)GenderEnum.Female)
            {
                m.photoUrl = LibraryConfig.PublicSite + "/content/" + LibraryConfig.DefaultPictureName;
                m.ThumbUrl = m.photoUrl;
                m.Gender = GenderEnum.Female.ToString();
            }
            else if (mem.Gender == (int)GenderEnum.Male)
            {
                m.photoUrl = LibraryConfig.PublicSite + "/content/" + LibraryConfig.DefaultPictureName;
                m.ThumbUrl = m.photoUrl;
                m.Gender = GenderEnum.Male.ToString();
            }
            m.DerbyNameUrl = RDN.Library.Classes.Config.LibraryConfig.MemberPublicUrl + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(mem.DerbyName) + "/" + mem.MemberId.ToString().Replace("-", "");
            m.DerbyName = mem.DerbyName;
            m.DerbyNumber = mem.PlayerNumber;
            m.MemberId = mem.MemberId.ToString().Replace("-", "");
            if (!String.IsNullOrEmpty(mem.Bio))
                m.Bio = htmlStrip.Replace(mem.Bio, String.Empty);
            m.DOB = mem.DateOfBirth.GetValueOrDefault();
            if (mem.HeightInches != 0)
            {
                m.HeightFeet = (int)(mem.HeightInches / 12);
                m.HeightInches = (int)(mem.HeightInches % 12);
            }
            m.Weight = mem.WeightInLbs.ToString();
            m.FirstName = mem.Firstname;

            var leag = mem.Leagues.Where(x => x.League.LeagueId == mem.CurrentLeagueId).FirstOrDefault();
            if (leag != null)
            {
                m.LeagueUrl = RDN.Library.Classes.Config.LibraryConfig.MainDomain + "/" + RDN.Library.Classes.Config.LibraryConfig.LeagueUrl + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(leag.League.Name) + "/" + mem.CurrentLeagueId.ToString().Replace("-", "");
                m.LeagueName = leag.League.Name;
                m.LeagueId = leag.League.LeagueId.ToString().Replace("-", "");
                if (leag.League.Logo != null)
                    m.LeagueLogo = leag.League.Logo.ImageUrl;
            }
            return m;
        }
        public static List<SkaterJson> SearchPublicMembers(string qu, int count)
        {
            List<SkaterJson> membersTemp = new List<SkaterJson>();
            try
            {
                qu = qu.ToLower();
                var mems = SiteCache.GetAllPublicMembers();
                return mems.Where(x => !String.IsNullOrEmpty(x.DerbyName)).Where(x => x.DerbyName.ToLower().Contains(qu)).OrderBy(x => x.DerbyName).Take(count).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return membersTemp;
        }

        /// <summary>
        /// Gets the member entity using the specified context
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal static DataModels.Member.Member GetMemberWithUserId(ref ManagementContext dc, Guid userId)
        {
            return dc.Members.FirstOrDefault(x => x.AspNetUserId.Equals(userId));
        }
        internal static DataModels.Member.Member GetMemberWithMemberId(ref ManagementContext dc, Guid userId)
        {
            return dc.Members.FirstOrDefault(x => x.MemberId.Equals(userId));
        }

        public static MemberDisplay GetMemberDisplay()
        {
            return GetMemberDisplay(GetMemberId());
        }

        public static bool ClearMedicalRecords(Guid memberId)
        {
            ManagementContext dc = new ManagementContext();
            var mem = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
            if (mem.MedicalInformation != null)
                dc.MemberMedical.Remove(mem.MedicalInformation);
            mem.MedicalInformation = null;
            int c = dc.SaveChanges();
            if (c > 0)
                return true;
            return false;
        }

        public static bool UpdateMedicalRecords(MemberDisplay member)
        {
            ManagementContext dc = new ManagementContext();
            var mem = dc.Members.Where(x => x.MemberId == member.MemberId).FirstOrDefault();
            if (mem.MedicalInformation == null)
            {
                MemberMedical medical = new MemberMedical();
                medical.AdditionalDetailsText = member.Medical.AdditionalDetailsText;
                medical.AsthmaBronchitis = member.Medical.AsthmaBronchitis;
                medical.BackNeckPain = member.Medical.BackNeckPain;
                medical.Concussion = member.Medical.Concussion;
                medical.ContactLenses = member.Medical.ContactLenses;
                medical.Diabetes = member.Medical.Diabetes;
                medical.Dislocation = member.Medical.Dislocation;
                medical.DislocationText = member.Medical.DislocationText;
                medical.DoAnyConditionsAffectPerformanceText = member.Medical.DoAnyConditionsAffectPerformanceText;
                medical.Epilepsy = member.Medical.Epilepsy;
                medical.FractureInThreeYears = member.Medical.FractureInThreeYears;
                medical.FractureText = member.Medical.FractureText;
                medical.HardSoftLensesEnum = Convert.ToInt32(member.Medical.HardSoftLensesEnum);
                medical.HeartMurmur = member.Medical.HeartMurmur;
                medical.HeartProblems = member.Medical.HeartProblems;
                medical.Hernia = member.Medical.Hernia;
                medical.RegularMedsText = member.Medical.RegularMedsText;
                medical.ReoccurringPain = member.Medical.ReoccurringPain;
                medical.ReoccurringPainText = member.Medical.ReoccurringPainText;
                medical.SportsInjuriesText = member.Medical.SportsInjuriesText;
                medical.TreatedForInjury = member.Medical.TreatedForInjury;
                medical.WearGlasses = member.Medical.WearGlasses;
                mem.MedicalInformation = medical;
            }
            else
            {
                mem.MedicalInformation.AdditionalDetailsText = member.Medical.AdditionalDetailsText;
                mem.MedicalInformation.AsthmaBronchitis = member.Medical.AsthmaBronchitis;
                mem.MedicalInformation.BackNeckPain = member.Medical.BackNeckPain;
                mem.MedicalInformation.Concussion = member.Medical.Concussion;
                mem.MedicalInformation.ContactLenses = member.Medical.ContactLenses;
                mem.MedicalInformation.Diabetes = member.Medical.Diabetes;
                mem.MedicalInformation.Dislocation = member.Medical.Dislocation;
                mem.MedicalInformation.DislocationText = member.Medical.DislocationText;
                mem.MedicalInformation.DoAnyConditionsAffectPerformanceText = member.Medical.DoAnyConditionsAffectPerformanceText;
                mem.MedicalInformation.Epilepsy = member.Medical.Epilepsy;
                mem.MedicalInformation.FractureInThreeYears = member.Medical.FractureInThreeYears;
                mem.MedicalInformation.FractureText = member.Medical.FractureText;
                mem.MedicalInformation.HardSoftLensesEnum = Convert.ToInt32(member.Medical.HardSoftLensesEnum);
                mem.MedicalInformation.HeartMurmur = member.Medical.HeartMurmur;
                mem.MedicalInformation.HeartProblems = member.Medical.HeartProblems;
                mem.MedicalInformation.Hernia = member.Medical.Hernia;
                mem.MedicalInformation.RegularMedsText = member.Medical.RegularMedsText;
                mem.MedicalInformation.ReoccurringPain = member.Medical.ReoccurringPain;
                mem.MedicalInformation.ReoccurringPainText = member.Medical.ReoccurringPainText;
                mem.MedicalInformation.SportsInjuriesText = member.Medical.SportsInjuriesText;
                mem.MedicalInformation.TreatedForInjury = member.Medical.TreatedForInjury;
                mem.MedicalInformation.WearGlasses = member.Medical.WearGlasses;
            }

            int c = dc.SaveChanges();
            if (c > 0)
                return true;
            return false;
        }

        public static MemberDisplay UpdateMemberDisplayForLeague(MemberDisplay mem, Stream fileStream, string nameOfFile)
        {
            UpdateMemberDisplayForLeague(mem);
            AddPrimaryMemberPhoto(mem, fileStream, nameOfFile);
            return mem;
        }
        /// <summary>
        /// Adds the primary member photo and returns the url of the photo.
        /// </summary>
        /// <param name="mem"></param>
        /// <param name="fileStream"></param>
        /// <param name="nameOfFile"></param>
        /// <returns></returns>
        private static string AddPrimaryMemberPhoto(MemberDisplay mem, Stream fileStream, string nameOfFile)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                var dc = new ManagementContext();
                var memDb = dc.Members.Where(x => x.MemberId == mem.MemberId).FirstOrDefault();
                //time stamp for the save location
                DateTime timeOfSave = DateTime.UtcNow;

                FileInfo info = new FileInfo(nameOfFile);

                //the file name when we save it
                string fileName = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(mem.DerbyName + " " + LibraryConfig.SportNameForUrl + "-") + timeOfSave.ToFileTimeUtc() + info.Extension;
                string fileNameThumb = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(mem.DerbyName + " " + LibraryConfig.SportNameForUrl + "-") + timeOfSave.ToFileTimeUtc() + "_thumb" + info.Extension;


                string url = LibraryConfig.ImagesBaseUrl + "/members/" + timeOfSave.Year + "/" + timeOfSave.Month + "/" + timeOfSave.Day + "/";
                string imageLocationToSave = LibraryConfig.ImagesBaseSaveLocation + @"\members\" + timeOfSave.Year + @"\" + timeOfSave.Month + @"\" + timeOfSave.Day + @"\";
                //creates the directory for the image
                if (!Directory.Exists(imageLocationToSave))
                    Directory.CreateDirectory(imageLocationToSave);

                string urlMain = url + fileName;
                string urlThumb = url + fileNameThumb;
                string imageLocationToSaveMain = imageLocationToSave + fileName;
                string imageLocationToSaveThumb = imageLocationToSave + fileNameThumb;

                foreach (var memberPhoto in memDb.Photos)
                {
                    memberPhoto.IsPrimaryPhoto = false;
                }
                RDN.Library.DataModels.Member.MemberPhoto image = new RDN.Library.DataModels.Member.MemberPhoto();
                image.ImageUrl = urlMain;
                image.SaveLocation = imageLocationToSaveMain;
                image.SaveLocationThumb = imageLocationToSaveThumb;
                image.ImageUrlThumb = urlThumb;
                image.IsPrimaryPhoto = true;
                image.IsVisibleToPublic = true;
                image.Member = memDb;
                memDb.Photos.Add(image);

                sb.Append(imageLocationToSaveMain);
                using (var newfileStream = new FileStream(imageLocationToSaveMain, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fileStream.CopyTo(newfileStream);
                }

                Image thumbImg = Image.FromStream(fileStream);
                Image thumb = RDN.Utilities.Drawing.Images.ScaleDownImage(thumbImg, 300, 300);
                sb.Append(nameOfFile);
                sb.Append(imageLocationToSaveThumb);
                thumb.Save(imageLocationToSaveThumb);

                //saves the photo to the DB.
                int c = dc.SaveChanges();

                mem.Photos.Add(new PhotoItem(url, true, "Profile"));
                return urlMain;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: sb.ToString());
            }
            return string.Empty;
        }


        public static bool AddMemberPhotoForGame(Guid gameId, Stream fileStream, string nameOfFile, Guid gameMemberId, MemberTypeEnum memberType)
        {
            var dc = new ManagementContext();
            string name = String.Empty;
            Guid id = new Guid();
            GameMember member = null;
            GameOfficial official = null;
            if (memberType == MemberTypeEnum.Skater)
            {
                member = dc.GameMembers.Where(x => x.GameMemberId == gameMemberId && x.Team.Game.GameId == gameId).FirstOrDefault();
                if (member == null)
                    return false;
                name = member.MemberName;
                id = member.MemberLinkId;
            }
            else if (memberType == MemberTypeEnum.Referee)
            {
                official = dc.GameOfficials.Where(x => x.GameOfficialId == gameMemberId && x.Game.GameId == gameId).FirstOrDefault();
                if (official == null)
                    return false;
                name = official.MemberName;
                id = official.MemberLinkId;
            }

            //time stamp for the save location
            DateTime timeOfSave = DateTime.UtcNow;
            FileInfo info = new FileInfo(nameOfFile);
            //the file name when we save it
            string url = LibraryConfig.ImagesBaseUrl + "/games/" + timeOfSave.Year + "/" + timeOfSave.Month + "/" + timeOfSave.Day + "/";
            string imageLocationToSave = LibraryConfig.ImagesBaseSaveLocation + @"\games\" + timeOfSave.Year + @"\" + timeOfSave.Month + @"\" + timeOfSave.Day + @"\";
            //creates the directory for the image
            if (!Directory.Exists(imageLocationToSave))
                Directory.CreateDirectory(imageLocationToSave);

            string fileName = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(name + " " + LibraryConfig.SportNameForUrl + "-") + timeOfSave.ToFileTimeUtc() + info.Extension;
            string fileNameThumb = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(name + " " + LibraryConfig.SportNameForUrl + "-thumb-") + timeOfSave.ToFileTimeUtc() + info.Extension;

            string urlMain = url + fileName;
            string urlThumb = url + fileNameThumb;
            string imageLocationToSaveMain = imageLocationToSave + fileName;
            string imageLocationToSaveThumb = imageLocationToSave + fileNameThumb;

            bool isLinkedToMainMember = false;

            if (id != new Guid())
            {
                var memMain = dc.Members.Where(x => x.MemberId == id).FirstOrDefault();
                if (memMain != null)
                {
                    fileName = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(memMain.DerbyName + " " + LibraryConfig.SportNameForUrl + "-") + timeOfSave.ToFileTimeUtc() + info.Extension;

                    fileNameThumb = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(memMain.DerbyName + " " + LibraryConfig.SportNameForUrl + "-thumb-") + timeOfSave.ToFileTimeUtc() + info.Extension;

                    urlMain = url + fileName;
                    urlThumb = url + fileNameThumb;
                    imageLocationToSaveMain = imageLocationToSave + fileName;
                    imageLocationToSaveThumb = imageLocationToSave + fileNameThumb;

                    isLinkedToMainMember = true;

                    RDN.Library.DataModels.Member.MemberPhoto image = new RDN.Library.DataModels.Member.MemberPhoto();
                    image.ImageUrl = urlMain;
                    image.SaveLocation = imageLocationToSaveMain;
                    image.SaveLocationThumb = imageLocationToSaveThumb;
                    image.ImageUrlThumb = urlThumb;
                    image.IsPrimaryPhoto = false;
                    image.IsVisibleToPublic = true;
                    image.Member = memMain;
                    memMain.Photos.Add(image);
                }
            }
            //if it is, we won't actually save the picture in the game member table..
            if (isLinkedToMainMember == false && member != null)
            {
                url += fileName;
                imageLocationToSave += fileName;

                GameMemberPhoto image = new GameMemberPhoto();
                image.ImageUrl = urlMain;
                image.SaveLocation = imageLocationToSaveMain;
                image.SaveLocationThumb = imageLocationToSaveThumb;
                image.ImageUrlThumb = urlThumb;

                image.IsPrimaryPhoto = true;
                image.IsVisibleToPublic = true;
                image.Member = member;
                member.Photos.Add(image);
            }

            if (isLinkedToMainMember == false && official != null)
            {
                url += fileName;
                imageLocationToSave += fileName;

                GameOfficialPhoto image = new GameOfficialPhoto();
                image.ImageUrl = urlMain;
                image.SaveLocation = imageLocationToSaveMain;
                image.SaveLocationThumb = imageLocationToSaveThumb;
                image.ImageUrlThumb = urlThumb;
                image.IsPrimaryPhoto = true;
                image.IsVisibleToPublic = true;
                image.Official = official;
                official.Photos.Add(image);
            }

            using (var newfileStream = new FileStream(imageLocationToSaveMain, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fileStream.CopyTo(newfileStream);
            }
            Image thumbImg = Image.FromStream(fileStream);
            Image thumb = RDN.Utilities.Drawing.Images.ScaleDownImage(thumbImg, 300, 300);
            thumb.Save(imageLocationToSaveThumb);
            //saves the photo to the DB.
            dc.SaveChanges();

            return true;

        }

        public static bool UnRetireMember(Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var member = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                if (member == null)
                    return false;

                member.Retired = false;
                dc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return true;
        }
        public static bool RetireMember(Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var member = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                if (member == null)
                    return false;

                member.Retired = true;
                dc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return true;
        }


        public static MemberDisplay UpdateMemberDisplayForLeague(MemberDisplay mem)
        {
            try
            {
                var dc = new ManagementContext();
                var member = dc.Members.Include("InsuranceNumbers").Include("ContactCard").Include("ContactCard.Emails").Include("ContactCard.Communications").Include("Federations").FirstOrDefault(x => x.MemberId.Equals(mem.MemberId));
                if (member == null)
                    return mem;
                member.DayJob = mem.DayJob;
                member.DerbyName = mem.DerbyName;
                member.PlayerNumber = mem.PlayerNumber;
                member.Firstname = mem.Firstname;
                member.Lastname = mem.LastName;
                if (mem.DOB != null && mem.DOB > DateTime.Now.AddYears(-100))
                    member.DateOfBirth = mem.DOB;
                if (mem.StoppedSkating != new DateTime())
                    member.YearStoppedSkating = mem.StoppedSkating;
                if (mem.StartedSkating != new DateTime())
                    member.YearStartedSkating = mem.StartedSkating;

                //int phoneType = Convert.ToInt32(CommunicationTypeEnum.PhoneNumber);
                var phone = member.ContactCard.Communications.Where(x => x.CommunicationTypeEnum == (byte)CommunicationTypeEnum.PhoneNumber).FirstOrDefault();
                if (phone != null)
                    phone.Data = mem.PhoneNumber;
                else
                {
                    member.ContactCard.Communications.Add(new Communication
                    {
                        Data = mem.PhoneNumber,
                        IsDefault = true,
                        //CommunicationType = dc.CommunicationType.Where(x => x.CommunicationTypeId == phoneType).FirstOrDefault()
                        CommunicationTypeEnum = (byte)CommunicationTypeEnum.PhoneNumber
                    });
                }

                var email = member.ContactCard.Emails.FirstOrDefault();
                if (email != null)
                    email.EmailAddress = mem.Email;
                else
                    member.ContactCard.Emails.Add(new RDN.Library.DataModels.ContactCard.Email { EmailAddress = mem.Email, IsDefault = true });

                foreach (var league in mem.Leagues)
                {
                    var leagueMember = dc.LeagueMembers.Where(x => x.League.LeagueId == league.LeagueId && x.Member.MemberId == mem.MemberId).FirstOrDefault();
                    if (leagueMember != null)
                    {
                        leagueMember.IsInactiveForLeague = league.IsInactiveInLeague;
                        leagueMember.League = leagueMember.League;
                        leagueMember.Member = leagueMember.Member;
                        leagueMember.Notes = league.NotesAboutMember;
                        if (league.SkaterClass != null && league.SkaterClass > 0)
                            leagueMember.SkaterClass = dc.LeagueMemberClasses.Where(x => x.ClassId == league.SkaterClass).FirstOrDefault();
                        else
                            leagueMember.SkaterClass = null;
                        if (league.SkillsTestDate != new DateTime())
                            leagueMember.SkillsTestDate = league.SkillsTestDate;
                        if (league.MembershipDate != new DateTime())
                            leagueMember.MembershipDate = league.MembershipDate;
                        if (league.DepartureDate != new DateTime())
                            leagueMember.DepartureDate = league.DepartureDate;
                        if (league.PassedWrittenExam != new DateTime())
                            leagueMember.PassedWrittenExam = league.PassedWrittenExam;
                    }
                }
                for (int i = 0; i < mem.InsuranceNumbers.Count; i++)
                {
                    int other = Convert.ToInt32(mem.InsuranceNumbers[i].Type);
                    var insurance = member.InsuranceNumbers.Where(x => x.InsuranceType == other).FirstOrDefault();
                    if (insurance == null)
                        AddInsuranceProvider(mem.InsuranceNumbers[i].Number, mem.InsuranceNumbers[i].Expires, member, other);
                    else
                    {
                        insurance.InsuranceNumber = mem.InsuranceNumbers[i].Number;
                        insurance.Expires = mem.InsuranceNumbers[i].Expires;
                    }
                }

                dc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return mem;
        }
        /// <summary>
        /// adds a insurance provider to a member
        /// </summary>
        /// <param name="insuranceNumber"></param>
        /// <param name="member"></param>
        /// <param name="other"></param>
        private static void AddInsuranceProvider(string insuranceNumber, DateTime? expires, Member member, int other)
        {
            MemberInsurance insure = new MemberInsurance();
            insure.InsuranceNumber = insuranceNumber;
            insure.InsuranceType = other;
            insure.Member = member;
            insure.Expires = expires;
            member.InsuranceNumbers.Add(insure);
        }

        public static MemberDisplay UpdateMemberDisplayForMember(MemberDisplay mem, Stream fileStream, string nameOfFile)
        {
            UpdateMemberDisplayForMember(mem);

            AddPrimaryMemberPhoto(mem, fileStream, nameOfFile);
            //saves the photo to the member cache object we return to the user.

            return mem;
        }
        public static MemberDisplay UpdateMemberDisplayForMemberMobile(MemberDisplay mem)
        {
            try
            {
                var dc = new ManagementContext();
                var member = dc.Members.Include("InsuranceNumbers").Include("ContactCard").Include("ContactCard.Emails").Include("ContactCard.Communications").Include("Federations").FirstOrDefault(x => x.MemberId.Equals(mem.MemberId));
                if (member == null)
                    return mem;
                member.IsProfileRemovedFromPublic = mem.IsProfileRemovedFromPublicView;
                member.DayJob = mem.DayJob;
                member.DerbyName = mem.DerbyName;
                member.PlayerNumber = mem.PlayerNumber;
                member.Firstname = mem.Firstname;
                member.Lastname = mem.LastName;
                member.Gender = Convert.ToInt32(mem.Gender);
                member.WeightInLbs = mem.WeightLbs;
                if (mem.DOB != null && mem.DOB > DateTime.Now.AddYears(-200))
                    member.DateOfBirth = mem.DOB;
                else
                    member.DateOfBirth = null;
                if (mem.StartedSkating != null && mem.StartedSkating > DateTime.Now.AddYears(-100))
                    member.YearStartedSkating = mem.StartedSkating;
                if (mem.StoppedSkating != null && mem.StoppedSkating > DateTime.Now.AddYears(-100))
                    member.YearStoppedSkating = mem.StoppedSkating;
                member.HeightInches = (mem.HeightFeet * 12) + mem.HeightInches;

                for (int i = 0; i < mem.InsuranceNumbers.Count; i++)
                {
                    int other = Convert.ToInt32(mem.InsuranceNumbers[i].Type);
                    var insurance = member.InsuranceNumbers.Where(x => x.InsuranceType == other).FirstOrDefault();
                    if (insurance == null)
                        AddInsuranceProvider(mem.InsuranceNumbers[i].Number, mem.InsuranceNumbers[i].Expires, member, other);
                    else
                    {
                        insurance.InsuranceNumber = mem.InsuranceNumbers[i].Number;
                        insurance.Expires = mem.InsuranceNumbers[i].Expires;
                    }
                }

                var add = member.ContactCard.Addresses.FirstOrDefault();
                if (add == null)
                {
                    ContactCard.ContactCardFactory.AddAddressToContact(mem.Address, mem.Address2, mem.City, mem.State, mem.ZipCode, AddressTypeEnum.None, member.ContactCard, dc.Countries.Where(x => x.CountryId == mem.CountryId).FirstOrDefault());
                }
                else
                {
                    ContactCard.ContactCardFactory.UpdateAddressToContact(mem.Address, mem.Address2, mem.City, mem.State, mem.ZipCode, AddressTypeEnum.None, add, dc.Countries.Where(x => x.CountryId == mem.CountryId).FirstOrDefault());
                }
                UpdatePhoneNumberForMember(mem, dc, member);
                UpdateEmailForMember(mem, member);

                int c = dc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return mem;
        }

        public static MemberDisplay UpdateMemberDisplayForMember(MemberDisplay mem)
        {
            try
            {
                var dc = new ManagementContext();
                var member = dc.Members.Include("InsuranceNumbers").Include("ContactCard").Include("ContactCard.Emails").Include("ContactCard.Communications").Include("Federations").FirstOrDefault(x => x.MemberId.Equals(mem.MemberId));
                if (member == null)
                    return mem;
                member.IsProfileRemovedFromPublic = mem.IsProfileRemovedFromPublicView;
                member.DayJob = mem.DayJob;
                member.DerbyName = mem.DerbyName;
                member.PlayerNumber = mem.PlayerNumber;
                member.Firstname = mem.Firstname;
                member.Lastname = mem.LastName;
                member.Bio = mem.Bio;
                member.Gender = Convert.ToInt32(mem.Gender);
                member.WeightInLbs = mem.WeightLbs;
                member.MemberType = (long)mem.MemberType;

                member.Website = mem.Website;
                member.Twitter = mem.Twitter;
                member.Instagram = mem.Instagram;
                member.Facebook = mem.Facebook;

                if (mem.DOB != null && mem.DOB > DateTime.Now.AddYears(-200))
                    member.DateOfBirth = mem.DOB;
                else
                    member.DateOfBirth = null;
                if (mem.StartedSkating != null && mem.StartedSkating > DateTime.Now.AddYears(-100))
                    member.YearStartedSkating = mem.StartedSkating;
                if (mem.StoppedSkating != null && mem.StoppedSkating > DateTime.Now.AddYears(-100))
                    member.YearStoppedSkating = mem.StoppedSkating;
                member.HeightInches = (mem.HeightFeet * 12) + mem.HeightInches;

                for (int i = 0; i < mem.InsuranceNumbers.Count; i++)
                {
                    int other = Convert.ToInt32(mem.InsuranceNumbers[i].Type);
                    var insurance = member.InsuranceNumbers.Where(x => x.InsuranceType == other).FirstOrDefault();
                    if (insurance == null)
                        AddInsuranceProvider(mem.InsuranceNumbers[i].Number, mem.InsuranceNumbers[i].Expires, member, other);
                    else
                    {
                        insurance.InsuranceNumber = mem.InsuranceNumbers[i].Number;
                        insurance.Expires = mem.InsuranceNumbers[i].Expires;
                    }
                }

                foreach (var league in mem.Leagues)
                {
                    var leagueMember = dc.LeagueMembers.Where(x => x.League.LeagueId == league.LeagueId && x.Member.MemberId == mem.MemberId).FirstOrDefault();
                    if (leagueMember != null)
                    {
                        leagueMember.League = leagueMember.League;
                        leagueMember.Member = leagueMember.Member;
                        if (league.DepartureDate != new DateTime())
                            leagueMember.DepartureDate = league.DepartureDate;
                    }
                }
                var add = member.ContactCard.Addresses.FirstOrDefault();
                if (add == null)
                {
                    ContactCard.ContactCardFactory.AddAddressToContact(mem.Address, mem.Address2, mem.City, mem.State, mem.ZipCode, AddressTypeEnum.None, member.ContactCard, dc.Countries.Where(x => x.CountryId == mem.CountryId).FirstOrDefault());
                }
                else
                {
                    ContactCard.ContactCardFactory.UpdateAddressToContact(mem.Address, mem.Address2, mem.City, mem.State, mem.ZipCode, AddressTypeEnum.None, add, dc.Countries.Where(x => x.CountryId == mem.CountryId).FirstOrDefault());
                }
                UpdatePhoneNumberForMember(mem, dc, member);
                UpdateEmailForMember(mem, member);

                int c = dc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return mem;
        }

        public static MemberDisplay UpdateMemberDisplayForFederation(MemberDisplay mem, Stream fileStream, string nameOfFile)
        {
            UpdateMemberDisplayForFederation(mem);
            AddPrimaryMemberPhoto(mem, fileStream, nameOfFile);
            return mem;
        }

        /// <summary>
        /// only updates certain things, so if we need to update more things of the member display, you need to add
        /// it to this method.  In the interest of time right now, thats how its working.
        /// </summary>
        /// <param name="member"></param>
        public static MemberDisplay UpdateMemberDisplayForFederation(MemberDisplay mem)
        {
            try
            {
                var dc = new ManagementContext();
                var member = dc.Members.Include("ContactCard").Include("ContactCard.Emails").Include("ContactCard.Communications").Include("Federations").FirstOrDefault(x => x.MemberId.Equals(mem.MemberId));
                if (member == null)
                    return mem;

                foreach (var league in mem.Leagues)
                {
                    if (league.LeagueMovedId != new Guid())
                    {
                        var le = dc.LeagueMembers.Where(x => x.Member.MemberId == mem.MemberId && x.League.LeagueId == league.LeagueId).FirstOrDefault();
                        if (le != null)
                            le.League = dc.Leagues.Where(x => x.LeagueId == league.LeagueMovedId).FirstOrDefault();
                        else
                        {
                            LeagueMember lm = new LeagueMember();
                            lm.League = dc.Leagues.Where(x => x.LeagueId == league.LeagueMovedId).FirstOrDefault();
                            lm.Member = dc.Members.Where(x => x.MemberId == mem.MemberId).FirstOrDefault();
                            dc.LeagueMembers.Add(lm);
                        }
                    }
                }

                member.DerbyName = mem.DerbyName;
                member.PlayerNumber = mem.PlayerNumber;
                member.Firstname = mem.Firstname;
                member.Lastname = mem.LastName;
                if (mem.StoppedSkating != new DateTime())
                    member.YearStoppedSkating = mem.StoppedSkating;
                if (mem.StartedSkating != new DateTime())
                    member.YearStartedSkating = mem.StartedSkating;

                UpdatePhoneNumberForMember(mem, dc, member);
                UpdateEmailForMember(mem, member);

                //updates the federation ownership and class rank for the federation
                foreach (var feds in mem.FederationsApartOf)
                {
                    try
                    {
                        var fed = member.Federations.Where(x => x.Federation.FederationId == feds.FederationId).FirstOrDefault();
                        fed.MADEClassRankForMember = Convert.ToInt32((MADEClassRankEnum)Enum.Parse(typeof(MADEClassRankEnum), feds.MADEClassRank));
                        fed.MemberType = Convert.ToInt32((MemberTypeFederationEnum)Enum.Parse(typeof(MemberTypeFederationEnum), feds.MemberType));
                        if (feds.MembershipDate != new DateTime())
                            fed.MembershipDate = feds.MembershipDate;
                        fed.FederationIdForMember = feds.MembershipId;
                        var fedOwner = member.FederationOwnership.Where(x => x.Federation.FederationId == feds.FederationId).FirstOrDefault();
                        if (fedOwner == null)
                        {
                            fedOwner = new FederationOwnership();
                            fedOwner.Federation = dc.Federations.Where(x => x.FederationId == feds.FederationId).FirstOrDefault();
                            fedOwner.IsVerified = true;
                            fedOwner.Member = member;
                            fedOwner.OwnerType = Convert.ToInt32((FederationOwnerEnum)Enum.Parse(typeof(FederationOwnerEnum), feds.OwnerType));
                            dc.FederationOwners.Add(fedOwner);
                        }
                        else
                        {
                            fedOwner.OwnerType = Convert.ToInt32((FederationOwnerEnum)Enum.Parse(typeof(FederationOwnerEnum), feds.OwnerType));
                        }

                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }
                int c = dc.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return mem;
        }

        private static void UpdateEmailForMember(MemberDisplay mem, Member member)
        {
            var email = member.ContactCard.Emails.Where(x => x.IsDefault == true).FirstOrDefault();
            if (email != null)
                email.EmailAddress = mem.Email;
            else
                member.ContactCard.Emails.Add(new RDN.Library.DataModels.ContactCard.Email { EmailAddress = mem.Email, IsDefault = true });
        }

        private static void UpdatePhoneNumberForMember(MemberDisplay mem, ManagementContext dc, Member member)
        {
            int phoneType = Convert.ToInt32(CommunicationTypeEnum.PhoneNumber);
            var phone = member.ContactCard.Communications.Where(x => x.CommunicationTypeEnum == (byte)CommunicationTypeEnum.PhoneNumber).FirstOrDefault();
            if (phone != null)
                phone.Data = mem.PhoneNumber;
            else
            {
                member.ContactCard.Communications.Add(new Communication
                {
                    Data = mem.PhoneNumber,
                    IsDefault = true,
                    //CommunicationType = dc.CommunicationType.Where(x => x.CommunicationTypeId == phoneType).FirstOrDefault()
                    CommunicationTypeEnum = (byte)CommunicationTypeEnum.PhoneNumber
                });
            }
        }

        public static MemberDisplay GetMemberDisplay(string userName, bool isPublicProfile = false)
        {
            var memId = GetMemberId(userName);
            return GetMemberDisplay(memId, isPublicProfile);
        }

        /// <summary>
        /// builds up the member object to display out to the world.
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static MemberDisplay GetMemberDisplay(Guid memberId, bool isPublicProfile = false, bool pullGameData = true)
        {
            try
            {
                var dc = new ManagementContext();
                var member = dc.Members.Include("Notifications").Include("Settings").Include("Leagues").Include("Leagues.SkaterClass").Include("InsuranceNumbers").Include("ContactCard").Include("ContactCard.Emails").Include("ContactCard.Communications").Include("Photos").Include("Federations").Include("MedicalInformation").FirstOrDefault(x => x.MemberId.Equals(memberId));
                if (member == null)
                    return null;



                MemberDisplay mem = new MemberDisplay();

                mem.IsProfileRemovedFromPublicView = member.IsProfileRemovedFromPublic;
                if (member.IsProfileRemovedFromPublic && isPublicProfile)
                    return null;

                foreach (var photo in member.Photos.OrderByDescending(x => x.Created))
                {
                    mem.Photos.Add(new PhotoItem(photo.ImageUrl, photo.IsPrimaryPhoto, member.DerbyName));
                }

                if (member.YearStartedSkating != null && member.YearStartedSkating != new DateTime())
                    mem.StartedSkating = member.YearStartedSkating.Value;
                else
                    mem.StartedSkating = null;
                if (member.YearStoppedSkating != null && member.YearStoppedSkating != new DateTime())
                    mem.StoppedSkating = member.YearStoppedSkating.Value;
                else
                    mem.StoppedSkating = null;

                mem.CurrentLeagueId = member.CurrentLeagueId;
                mem.IsRetired = member.Retired;
                mem.Firstname = member.Firstname;
                mem.DerbyName = member.DerbyName;
                mem.DerbyNameUrl = LibraryConfig.PublicSite + "/" + RDN.Library.Classes.Config.LibraryConfig.SportNamePlusMemberNameForUrl + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(member.DerbyName) + "/" + member.MemberId.ToString().Replace("-", "");
                mem.MemberId = memberId;
                mem.UserId = member.AspNetUserId;
                mem.DayJob = member.DayJob;
                var user = Membership.GetUser((object)member.AspNetUserId);
                if (user != null)
                    mem.UserName = user.UserName;
                mem.IsNotConnectedToDerby = member.IsNotConnectedToDerby;
                mem.Website = member.Website;
                mem.Twitter = member.Twitter;
                mem.Instagram = member.Instagram;
                mem.Facebook = member.Facebook;
                mem.YearStartedSkating = member.YearStartedSkating;
                mem.MemberType = (MemberType)member.MemberType;

                mem.Email = ExtractEmailFromContactCard(member);
                if (member.ContactCard.Addresses.Count > 0)
                {
                    foreach (var add in member.ContactCard.Addresses)
                    {
                        var address = new RDN.Portable.Classes.ContactCard.Address();
                        address.AddressId = add.AddressId;
                        address.Address1 = add.Address1;
                        address.Address2 = add.Address2;
                        address.CityRaw = add.CityRaw;
                        if (add.Country != null)
                            address.Country = add.Country.Code;
                        address.StateRaw = add.StateRaw;
                        address.Zip = add.Zip;
                        address.Type = (AddressTypeEnum)add.AddressType;

                        mem.ContactCard.Addresses.Add(address);
                    }
                }
                GenderEnum gen;
                if (Enum.TryParse<GenderEnum>(member.Gender.ToString(), out gen))
                    mem.Gender = gen;
                mem.LastName = member.Lastname;
                mem.Bio = member.Bio;
                mem.DefaultPositionType = (DefaultPositionEnum)Enum.Parse(typeof(DefaultPositionEnum), member.PositionType.ToString());
                mem.BioHtml = member.Bio;


                if (member.DateOfBirth.HasValue)
                    mem.DOB = member.DateOfBirth.Value;

                mem.WeightLbs = member.WeightInLbs;

                if (member.HeightInches != 0)
                {
                    mem.HeightFeet = (int)(member.HeightInches / 12);
                    mem.HeightInches = (int)(member.HeightInches % 12);
                }

                DisplayInsuranceNumbers(member.InsuranceNumbers, mem);
                DisplayMemberNotifications(member.Notifications, mem);

                mem.Settings = MemberSettingsFactory.DisplayMemberSettings(member.Settings);

                foreach (var memberContact in member.MemberContacts)
                {
                    try
                    {
                        RDN.Portable.Classes.Account.Classes.MemberContact contact = new RDN.Portable.Classes.Account.Classes.MemberContact();
                        contact.ContactId = memberContact.ContactId;
                        contact.Firstname = memberContact.Firstname;
                        contact.Lastname = memberContact.Lastname;
                        contact.ContactType = (MemberContactTypeEnum)Enum.Parse(typeof(MemberContactTypeEnum), memberContact.ContactTypeEnum.ToString());
                        if (memberContact.ContactCard != null)
                        {
                            if (memberContact.ContactCard.Emails.Count > 0)
                                contact.Email = memberContact.ContactCard.Emails.Where(x => x.IsDefault == true).FirstOrDefault().EmailAddress;
                            if (memberContact.ContactCard.Communications.Count > 0)
                            {
                                contact.PhoneNumber = memberContact.ContactCard.Communications.FirstOrDefault().Data;
                                contact.SMSVerificationNum = memberContact.ContactCard.Communications.FirstOrDefault().SMSVerificationCode;
                                contact.Carrier = (MobileServiceProvider)memberContact.ContactCard.Communications.FirstOrDefault().CarrierType;
                            }
                            if (memberContact.ContactCard.Addresses.Count > 0)
                            {
                                contact.Addresses = new List<Portable.Classes.ContactCard.Address>();

                                foreach (var add in memberContact.ContactCard.Addresses)
                                {
                                    var address = new RDN.Portable.Classes.ContactCard.Address();
                                    address.Address1 = add.Address1;
                                    address.Address2 = add.Address2;
                                    address.CityRaw = add.CityRaw;
                                    if (add.Country != null)
                                        address.Country = add.Country.Code;
                                    address.StateRaw = add.StateRaw;
                                    address.Zip = add.Zip;
                                    address.Type = (AddressTypeEnum)add.AddressType;
                                    contact.Addresses.Add(address);
                                }
                            }
                        }
                        mem.MemberContacts.Add(contact);
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }

                if (member.ContactCard.Communications.Count > 0)
                {
                    var com = member.ContactCard.Communications.Where(x => x.IsDefault == true).Where(x => x.CommunicationTypeEnum == (byte)CommunicationTypeEnum.PhoneNumber).FirstOrDefault();
                    if (com != null)
                    {
                        mem.PhoneNumber = com.Data;
                        mem.SMSVerificationNum = com.SMSVerificationCode;
                        mem.Carrier = (MobileServiceProvider)com.CarrierType;
                        mem.IsCarrierVerified = com.IsCarrierVerified;
                    }
                }
                mem.PlayerNumber = member.PlayerNumber;
                if (member.Leagues.Count > 0)
                {
                    foreach (var league in member.Leagues)
                    {
                        RDN.Portable.Classes.League.Classes.League l = new Portable.Classes.League.Classes.League();
                        l.LeagueId = league.League.LeagueId;
                        l.Name = league.League.Name;
                        l.IsInactiveInLeague = league.IsInactiveForLeague;
                        l.DepartureDate = league.DepartureDate;
                        l.PassedWrittenExam = league.PassedWrittenExam;
                        l.MembershipDate = league.MembershipDate;
                        l.SkillsTestDate = league.SkillsTestDate;
                        l.HasLeftLeague = league.HasLeftLeague;
                        l.NotesAboutMember = league.Notes;
                        l.NameUrl = LibraryConfig.PublicSite + UrlManager.PublicSite_FOR_LEAGUES + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(l.Name) + "/" + l.LeagueId.ToString().Replace("-", "");
                        l.LeagueOwnersEnum = (LeagueOwnersEnum)league.LeagueOwnersEnums;
                        if (l.LeagueId == mem.CurrentLeagueId)
                        {
                            mem.LeagueOwnersEnum = l.LeagueOwnersEnum;
                            mem.DoesReceiveLeagueNotifications = !league.TurnOffEmailNotifications;
                            mem.Settings.ForumGroupOrder = league.ForumGroupOrder;
                        }
                        if (league.SkaterClass != null)
                            l.SkaterClass = league.SkaterClass.ClassId;
                        else
                            l.SkaterClass = 0;

                        mem.Leagues.Add(l);
                    }
                }

                MedicalInformationFactory.AttachMemberMedicalInformation(member, mem);

                foreach (var fed in member.Federations)
                {
                    FederationDisplay fedDisplay = new FederationDisplay();
                    fedDisplay.FederationId = fed.Federation.FederationId;
                    fedDisplay.FederationName = fed.Federation.Name;
                    fedDisplay.MADEClassRank = ((MADEClassRankEnum)Enum.ToObject(typeof(MADEClassRankEnum), fed.MADEClassRankForMember)).ToString();
                    fedDisplay.MemberType = ((MemberTypeFederationEnum)Enum.ToObject(typeof(MemberTypeFederationEnum), fed.MemberType)).ToString();
                    var ownersOfFed = fed.Federation.Owners.Where(x => x.Member == member).FirstOrDefault();
                    if (ownersOfFed == null)
                        fedDisplay.OwnerType = FederationOwnerEnum.None.ToString();
                    else
                        fedDisplay.OwnerType = ((FederationOwnerEnum)Enum.ToObject(typeof(FederationOwnerEnum), ownersOfFed.OwnerType)).ToString();

                    if (fed.MembershipDate != null && fed.MembershipDate != new DateTime())
                        fedDisplay.MembershipDate = fed.MembershipDate.Value;
                    else
                        fedDisplay.MembershipDate = DateTime.UtcNow;
                    fedDisplay.MembershipId = fed.FederationIdForMember;
                    mem.FederationsApartOf.Add(fedDisplay);
                }
                if (pullGameData)
                    GetGameStats(memberId, dc, mem);
                return mem;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static SkaterJson GetMemberDisplayJson(Guid memberId, bool isPublicProfile = false)
        {
            try
            {
                var dc = new ManagementContext();
                var member = dc.Members.Include("Notifications").Include("Settings").Include("Leagues").Include("Leagues.SkaterClass").Include("InsuranceNumbers").Include("ContactCard").Include("ContactCard.Emails").Include("ContactCard.Communications").Include("Photos").Include("Federations").Include("MedicalInformation").FirstOrDefault(x => x.MemberId.Equals(memberId));
                if (member == null)
                    return null;

                var mem = DisplaySkaterJson(member);
                return mem;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        private static void GetGameStats(Guid memberId, ManagementContext dc, MemberDisplay mem)
        {

            try
            {
                //looking for some information to fill out the stats we currently collect
                var memberGame = (from yy in dc.GameMembers
                                  where yy.MemberLinkId == memberId
                                  where yy.Team.Game.ScoreboardType == (int)ScoreboardModeEnum.AddedOldGame || yy.Team.Game.ScoreboardType == (int)ScoreboardModeEnum.Live
                                  select new
                                  {
                                      yy.Team.Game.GameId,
                                      yy.Created,
                                      yy.Team.TeamId,
                                      //yy.Team.TeamScoreboardId,
                                      yy.Team.TeamIdLink,
                                      JamsCount = yy.Team.Game.GameJams,

                                      game = (from xx in dc.Games
                                              where xx.GameId == yy.Team.Game.GameId
                                              where xx.ScoreboardType == (int)ScoreboardModeEnum.AddedOldGame || xx.ScoreboardType == (int)ScoreboardModeEnum.Live
                                              select new RDN.Portable.Classes.Games.Game
                                              {
                                                  //we are making sure, that the member is always apart of 
                                                  // team 1 so when we do our calculations below
                                                  // we also know the skater is on team 1.

                                                  GameDate = xx.GameDate,
                                                  GameId = xx.GameId,
                                                  GameName = xx.GameName,
                                                  Team1Id = xx.GameTeams.Where(x => x.TeamId == yy.Team.TeamId).FirstOrDefault().TeamId,
                                                  Team1LinkId = xx.GameTeams.Where(x => x.TeamId == yy.Team.TeamId).FirstOrDefault().TeamIdLink,
                                                  Team2Id = xx.GameTeams.Where(x => x.TeamId != yy.Team.TeamId).FirstOrDefault().TeamId,
                                                  Team2LinkId = xx.GameTeams.Where(x => x.TeamId != yy.Team.TeamId).FirstOrDefault().TeamIdLink,
                                                  Team2Name = (dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.Where(x => x.TeamId != yy.Team.TeamId).FirstOrDefault().TeamIdLink).FirstOrDefault() == null) ? xx.GameTeams.Where(x => x.TeamId != yy.Team.TeamId).FirstOrDefault().TeamName : dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.Where(x => x.TeamId != yy.Team.TeamId).FirstOrDefault().TeamIdLink).FirstOrDefault().Name,
                                                  Team1Name = (dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.Where(x => x.TeamId == yy.Team.TeamId).FirstOrDefault().TeamIdLink).FirstOrDefault() == null) ? xx.GameTeams.Where(x => x.TeamId == yy.Team.TeamId).FirstOrDefault().TeamName : dc.Leagues.Where(g => g.LeagueId == xx.GameTeams.Where(x => x.TeamId == yy.Team.TeamId).FirstOrDefault().TeamIdLink).FirstOrDefault().Name,
                                                  Team1Score = (from ss in dc.GameScore
                                                                where ss.GameTeam == xx.GameTeams.Where(z => z.TeamId == yy.Team.TeamId).FirstOrDefault()
                                                                select new GameScore
                                                                {
                                                                    DateTimeScored = ss.DateTimeScored,
                                                                    GameScoreId = ss.GameScoreId,
                                                                    JamId = ss.JamId,
                                                                    JamNumber = ss.JamNumber,
                                                                    PeriodNumber = ss.PeriodNumber,
                                                                    PeriodTimeRemainingMilliseconds = ss.PeriodTimeRemainingMilliseconds,
                                                                    Point = ss.Point,
                                                                    ScoreId = ss.ScoreId
                                                                }),
                                                  Team2Score = (from ss in dc.GameScore
                                                                where ss.GameTeam == xx.GameTeams.Where(z => z.TeamId != yy.Team.TeamId).FirstOrDefault()
                                                                select new GameScore
                                                                {
                                                                    DateTimeScored = ss.DateTimeScored,
                                                                    GameScoreId = ss.GameScoreId,
                                                                    JamId = ss.JamId,
                                                                    JamNumber = ss.JamNumber,
                                                                    PeriodNumber = ss.PeriodNumber,
                                                                    PeriodTimeRemainingMilliseconds = ss.PeriodTimeRemainingMilliseconds,
                                                                    Point = ss.Point,
                                                                    ScoreId = ss.ScoreId
                                                                }),
                                              }).FirstOrDefault()
                                  }).OrderByDescending(x => x.Created).ToList();

                //total games
                mem.TotalGamesPlayed = memberGame.Count;
                mem.JammerPosition.TotalGamesPlayed = 0;
                mem.BlockerPosition.TotalGamesPlayed = 0;
                mem.PivotPosition.TotalGamesPlayed = 0;
                mem.TotalJamsPlayed = 0;
                mem.JammerPosition.TotalJamsPlayed = 0;
                mem.BlockerPosition.TotalJamsPlayed = 0;
                mem.PivotPosition.TotalJamsPlayed = 0;
                mem.TotalJamsInAllGames = 0;
                mem.JammerPosition.PointsSeason = 0;
                mem.JammerPosition.PointsCareer = 0;
                mem.JammerPosition.PointsAgainstSeason = 0;
                mem.JammerPosition.PointsAgainstCareer = 0;
                mem.BlockerPosition.PointsSeason = 0;
                mem.BlockerPosition.PointsCareer = 0;
                mem.BlockerPosition.PointsAgainstSeason = 0;
                mem.BlockerPosition.PointsAgainstCareer = 0;
                mem.PivotPosition.PointsSeason = 0;
                mem.PivotPosition.PointsCareer = 0;
                mem.PivotPosition.PointsAgainstSeason = 0;
                mem.PivotPosition.PointsAgainstCareer = 0;
                mem.JamsBeenIn = new List<Portable.Classes.Games.Scoreboard.JamModel>();
                mem.GamesWon = 0;
                mem.GamesLost = 0;
                mem.GamesToDisplay = new List<Portable.Classes.Games.Game>();



                //iterate through all the games.
                foreach (var game in memberGame)
                {
                    bool PlayedAsJammerInGame = false;
                    bool PlayedAsPivotInGame = false;
                    bool PlayedAsBlockerInGame = false;
                    //sets total jams in allgames.
                    mem.TotalJamsInAllGames += game.JamsCount.Count;
                    //jam numbers for jammers/pivots and blockers skater was in.  So we can use this info for points.
                    List<int> jamNumbersAsJammers = new List<int>();
                    List<int> jamNumbersAsPivots = new List<int>();
                    List<int> jamNumbersAsBlockers = new List<int>();
                    foreach (var jam in game.JamsCount)
                    {
                        //checks if skater was in this jam
                        //adds the jam for the skater.
                        JamModel model = new JamModel();
                        model.JamNumber = jam.JamNumber;
                        if (jam.PivotTeam1 != null && jam.PivotTeam1.MemberLinkId == memberId)
                        {
                            model.PivotT1 = new TeamMembersModel(jam.PivotTeam1.MemberLinkId);
                            jamNumbersAsPivots.Add(model.JamNumber);
                            mem.PivotPosition.TotalJamsPlayed += 1;
                            mem.TotalJamsPlayed += 1;
                            PlayedAsPivotInGame = true;
                        }
                        if (jam.JammerTeam1 != null && jam.JammerTeam1.MemberLinkId == memberId)
                        {
                            model.JammerT1 = new TeamMembersModel(jam.JammerTeam1.MemberLinkId);
                            jamNumbersAsJammers.Add(model.JamNumber);
                            mem.JammerPosition.TotalJamsPlayed += 1;
                            mem.TotalJamsPlayed += 1;
                            PlayedAsJammerInGame = true;
                        }
                        if (jam.Blocker3Team1 != null && jam.Blocker3Team1.MemberLinkId == memberId)
                        {
                            model.Blocker3T1 = new TeamMembersModel(jam.Blocker3Team1.MemberLinkId);
                            jamNumbersAsBlockers.Add(model.JamNumber);
                            mem.BlockerPosition.TotalJamsPlayed += 1;
                            mem.TotalJamsPlayed += 1;
                            PlayedAsBlockerInGame = true;
                        }
                        if (jam.Blocker4Team1 != null && jam.Blocker4Team1.MemberLinkId == memberId)
                        {
                            model.Blocker4T1 = new TeamMembersModel(jam.Blocker4Team1.MemberLinkId);
                            jamNumbersAsBlockers.Add(model.JamNumber);
                            mem.BlockerPosition.TotalJamsPlayed += 1;
                            mem.TotalJamsPlayed += 1;
                            PlayedAsBlockerInGame = true;
                        }
                        if (jam.Blocker2Team1 != null && jam.Blocker2Team1.MemberLinkId == memberId)
                        {
                            model.Blocker2T1 = new TeamMembersModel(jam.Blocker2Team1.MemberLinkId);
                            jamNumbersAsBlockers.Add(model.JamNumber);
                            mem.BlockerPosition.TotalJamsPlayed += 1;
                            mem.TotalJamsPlayed += 1;
                            PlayedAsBlockerInGame = true;
                        }
                        if (jam.Blocker1Team1 != null && jam.Blocker1Team1.MemberLinkId == memberId)
                        {
                            model.Blocker1T1 = new TeamMembersModel(jam.Blocker1Team1.MemberLinkId);
                            jamNumbersAsBlockers.Add(model.JamNumber);
                            mem.BlockerPosition.TotalJamsPlayed += 1;
                            mem.TotalJamsPlayed += 1;
                            PlayedAsBlockerInGame = true;
                        }
                        model.GameTimeElapsedMillisecondsStart = jam.GameTimeElapsedMillisecondsStart;
                        model.GameTimeElapsedMillisecondsEnd = jam.GameTimeElapsedMillisecondsEnd;
                        mem.JamsBeenIn.Add(model);
                    }
                    //after counting through all the jams, we see which position the skater played for each game and 
                    //we count how many games the played with that position
                    if (PlayedAsJammerInGame)
                        mem.JammerPosition.TotalGamesPlayed += 1;
                    if (PlayedAsPivotInGame)
                        mem.PivotPosition.TotalGamesPlayed += 1;
                    if (PlayedAsBlockerInGame)
                        mem.BlockerPosition.TotalGamesPlayed += 1;

                    int totalPointsTeam1 = 0;
                    int totalPointsTeam2 = 0;
                    foreach (var point in game.game.Team1Score)
                    {
                        //sets points scored for the game.
                        totalPointsTeam1 += point.Point;

                        //also sets points scored while member was in the present position
                        if (jamNumbersAsJammers.Contains(point.JamNumber))
                        {
                            mem.JammerPosition.PointsCareer += point.Point;
                            mem.JammerPosition.PointsSeason += point.Point;
                        }
                        if (jamNumbersAsPivots.Contains(point.JamNumber))
                        {
                            mem.PivotPosition.PointsCareer += point.Point;
                            mem.PivotPosition.PointsSeason += point.Point;
                        }
                        if (jamNumbersAsBlockers.Contains(point.JamNumber))
                        {
                            mem.BlockerPosition.PointsCareer += point.Point;
                            mem.BlockerPosition.PointsSeason += point.Point;
                        }
                    }
                    foreach (var point in game.game.Team2Score)
                    {
                        //sets points scored for the game.
                        totalPointsTeam2 += point.Point;
                        //also sets points scored while member was in the present position
                        if (jamNumbersAsJammers.Contains(point.JamNumber))
                        {
                            mem.JammerPosition.PointsAgainstCareer += point.Point;
                            mem.JammerPosition.PointsAgainstSeason += point.Point;
                        }
                        if (jamNumbersAsPivots.Contains(point.JamNumber))
                        {
                            mem.PivotPosition.PointsAgainstCareer += point.Point;
                            mem.PivotPosition.PointsAgainstSeason += point.Point;
                        }
                        if (jamNumbersAsBlockers.Contains(point.JamNumber))
                        {
                            mem.BlockerPosition.PointsAgainstCareer += point.Point;
                            mem.BlockerPosition.PointsAgainstSeason += point.Point;
                        }
                    }

                    //total games this skater won and lost.
                    if (totalPointsTeam1 > totalPointsTeam2)
                        mem.GamesWon += 1;
                    else
                        mem.GamesLost += 1;

                    game.game.Team1ScoreTotal = 0;
                    foreach (var score in game.game.Team1Score)
                        game.game.Team1ScoreTotal += score.Point;

                    game.game.Team2ScoreTotal = 0;
                    foreach (var score in game.game.Team2Score)
                        game.game.Team2ScoreTotal += score.Point;

                    game.game.Score1Score2Delta = CalculateDeltaOfTwoNumbers((double)game.game.Team1ScoreTotal, (double)game.game.Team2ScoreTotal);

                    mem.GamesToDisplay.Add(game.game);

                }
                //average jam per game.
                mem.AverageJamsPerGame = CalculateAverage(mem.JamsBeenIn.Count, mem.TotalGamesPlayed);
                CalculateJammerPositionForMemberDisplay(mem);
                CalculateBlockerPositionForMemberDisplay(mem);
                CalculatePivotPositionForMemberDisplay(mem);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        private static void DisplayInsuranceNumbers(ICollection<MemberInsurance> insuranceNumbers, MemberDisplay mem)
        {
            foreach (var numb in insuranceNumbers)
            {
                try
                {
                    mem.InsuranceNumbers.Add(new InsuranceNumber() { Expires = numb.Expires, Number = numb.InsuranceNumber, Type = (InsuranceType)numb.InsuranceType });
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
            }
        }
        private static void DisplayMemberNotifications(MemberNotifications notifications, MemberDisplay mem)
        {
            if (notifications != null)
            {
                mem.EmailCalendarNewEventBroadcast = !notifications.EmailCalendarNewEventBroadcastTurnOff;
                mem.EmailForumBroadcasts = !notifications.EmailForumBroadcastsTurnOff;
                mem.EmailForumNewPost = !notifications.EmailForumNewPostTurnOff;
                mem.EmailForumWeeklyRoundup = !notifications.EmailForumWeeklyRoundupTurnOff;
                mem.EmailMessagesReceived = !notifications.EmailMessagesReceivedTurnOff;
            }
            else
            {
                mem.EmailCalendarNewEventBroadcast = true;
                mem.EmailForumBroadcasts = true;
                mem.EmailForumNewPost = true;
                mem.EmailForumWeeklyRoundup = true;
                mem.EmailMessagesReceived = true;

            }
        }

        public static string ExtractEmailFromContactCard(Member member)
        {
            if (member.ContactCard.Emails.Count > 0)
                return member.ContactCard.Emails.Where(x => x.IsDefault == true).FirstOrDefault().EmailAddress;
            return String.Empty;
        }



        public static MemberDisplay GetMemberDisplayTwoEvils(Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var member = dc.ProfilesForTwoEvils.Where(x => x.ProfileId == memberId).FirstOrDefault();
                if (member == null)
                    return null;

                MemberDisplay mem = new MemberDisplay();

                mem.MemberId = member.ProfileId;
                mem.PlayerNumber = member.Number;
                mem.DerbyName = member.Name;
                mem.MemberId = memberId;

                if (member.LeagueDerbyRoster != null)
                {
                    mem.Team1ScoreTotal = 2;
                    RDN.Portable.Classes.League.Classes.League l = new Portable.Classes.League.Classes.League();
                    l.Name = member.LeagueDerbyRoster.Name;
                    l.LeagueId = member.LeagueDerbyRoster.TeamId;
                    mem.Leagues.Add(l);
                }
                else if (member.League != null)
                {
                    mem.Team1ScoreTotal = 1;
                    RDN.Portable.Classes.League.Classes.League l = new RDN.Portable.Classes.League.Classes.League();
                    l.Name = member.League.Name;
                    l.LeagueId = member.League.TeamId;
                    mem.Leagues.Add(l);
                }
                return mem;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }


        private static double CalculateDeltaOfTwoNumbers(double number1, double number2)
        {
            if (number1 != 0 && number2 != 0)
            {
                if (number2 > number1)
                    return ((double)number1 / (double)number2) * -1;
                else
                    return (double)number2 / (double)number1;
            }
            else
                return 0;
        }

        /// <summary>
        /// calculates all the jammer numbers for the member
        /// </summary>
        /// <param name="mem"></param>
        private static void CalculateJammerPositionForMemberDisplay(MemberDisplay mem)
        {
            try
            {
                mem.JammerPosition.AveragePointsScoredPerGame = CalculateAverage(mem.JammerPosition.PointsCareer, mem.TotalGamesPlayed);

                mem.JammerPosition.AveragePointsScoredPerJam = CalculateAverage(mem.JammerPosition.PointsCareer, mem.JamsBeenIn.Count);

                //average points scored against jammer during game in career.
                mem.JammerPosition.AveragePointsScoredAgainstPerGame = CalculateAverage(mem.JammerPosition.PointsAgainstCareer, mem.TotalGamesPlayed);

                //average points scored against jammer in jams in career
                mem.JammerPosition.AveragePointsScoredAgainstPerJam = CalculateAverage(mem.JammerPosition.PointsAgainstCareer, mem.JamsBeenIn.Count);

                mem.JammerPosition.AveragePointsScoredPerJamDelta = CalculateDeltaOfTwoNumbers((double)mem.JammerPosition.AveragePointsScoredPerJam, (double)mem.JammerPosition.AveragePointsScoredAgainstPerJam);

                mem.JammerPosition.AveragePointsScoredPerGameDelta = CalculateDeltaOfTwoNumbers((double)mem.JammerPosition.AveragePointsScoredPerGame, (double)mem.JammerPosition.AveragePointsScoredAgainstPerGame);

                mem.JammerPosition.PointsCareerDelta = CalculateDeltaOfTwoNumbers((double)mem.JammerPosition.PointsCareer, (double)mem.JammerPosition.PointsAgainstCareer);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        private static void CalculateBlockerPositionForMemberDisplay(MemberDisplay mem)
        {
            try
            {
                mem.BlockerPosition.AveragePointsScoredPerGame = CalculateAverage(mem.BlockerPosition.PointsCareer, mem.TotalGamesPlayed);

                mem.BlockerPosition.AveragePointsScoredPerJam = CalculateAverage(mem.BlockerPosition.PointsCareer, mem.JamsBeenIn.Count);

                //average points scored against jammer during game in career.
                mem.BlockerPosition.AveragePointsScoredAgainstPerGame = CalculateAverage(mem.BlockerPosition.PointsAgainstCareer, mem.TotalGamesPlayed);

                //average points scored against jammer in jams in career
                mem.BlockerPosition.AveragePointsScoredAgainstPerJam = CalculateAverage(mem.BlockerPosition.PointsAgainstCareer, mem.JamsBeenIn.Count);

                mem.BlockerPosition.AveragePointsScoredPerJamDelta = CalculateDeltaOfTwoNumbers((double)mem.BlockerPosition.AveragePointsScoredPerJam, (double)mem.BlockerPosition.AveragePointsScoredAgainstPerJam);

                mem.BlockerPosition.AveragePointsScoredPerGameDelta = CalculateDeltaOfTwoNumbers((double)mem.BlockerPosition.AveragePointsScoredPerGame, (double)mem.BlockerPosition.AveragePointsScoredAgainstPerGame);

                mem.BlockerPosition.PointsCareerDelta = CalculateDeltaOfTwoNumbers((double)mem.BlockerPosition.PointsCareer, (double)mem.BlockerPosition.PointsAgainstCareer);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        private static void CalculatePivotPositionForMemberDisplay(MemberDisplay mem)
        {
            try
            {
                mem.PivotPosition.AveragePointsScoredPerGame = CalculateAverage(mem.PivotPosition.PointsCareer, mem.TotalGamesPlayed);

                mem.PivotPosition.AveragePointsScoredPerJam = CalculateAverage(mem.PivotPosition.PointsCareer, mem.JamsBeenIn.Count);

                //average points scored against jammer during game in career.
                mem.PivotPosition.AveragePointsScoredAgainstPerGame = CalculateAverage(mem.PivotPosition.PointsAgainstCareer, mem.TotalGamesPlayed);

                //average points scored against jammer in jams in career
                mem.PivotPosition.AveragePointsScoredAgainstPerJam = CalculateAverage(mem.PivotPosition.PointsAgainstCareer, mem.JamsBeenIn.Count);

                mem.PivotPosition.AveragePointsScoredPerJamDelta = CalculateDeltaOfTwoNumbers((double)mem.PivotPosition.AveragePointsScoredPerJam, (double)mem.PivotPosition.AveragePointsScoredAgainstPerJam);

                mem.PivotPosition.AveragePointsScoredPerGameDelta = CalculateDeltaOfTwoNumbers((double)mem.PivotPosition.AveragePointsScoredPerGame, (double)mem.PivotPosition.AveragePointsScoredAgainstPerGame);

                mem.PivotPosition.PointsCareerDelta = CalculateDeltaOfTwoNumbers((double)mem.PivotPosition.PointsCareer, (double)mem.PivotPosition.PointsAgainstCareer);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        /// <summary>
        /// gets the average of a number and the total of a specific number
        /// </summary>
        /// <param name="number1"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        private static double CalculateAverage(double number1, double total)
        {
            if (number1 != 0 && total != 0)
                return number1 / total;
            else
                return 0;
        }

        //public static MemberPhoto GetMemberPhoto()
        //{
        //    return GetMemberPhoto(GetMemberId());
        //}

        //public static MemberPhoto GetMemberPhoto(Guid memberId)
        //{
        //    var output = new MemberPhoto();

        //    var dc = new ManagementContext();
        //    var member = dc.Members.FirstOrDefault(x => x.MemberId.Equals(memberId));
        //    if (member == null) return null;
        //    var memberPhoto = member.Photos.FirstOrDefault(x => x.IsPrimaryPhoto.Equals(true));
        //    if (memberPhoto == null) return null;
        //    output.ImageFormat = new ImageFormat(memberPhoto.Format);
        //    output.ImageData = new byte[memberPhoto.PhotoData.Length];
        //    Buffer.BlockCopy(memberPhoto.PhotoData, 0, output.ImageData, 0, memberPhoto.PhotoData.Length);
        //    dc.Dispose();

        //    return output;
        //}

        /// <summary>
        /// Adds a note in the member log
        /// </summary>        
        /// <param name="title">Title of the entry</param>
        /// <param name="log">Description of the entry</param>
        /// <param name="logType">Type of log entry</param>
        /// <param name="ip">The ip of the person executing this command. Can be optained from Request.UserHostAddress or Request.ServerVariables</param>
        public static void AddMemberLog(string title, string log, MemberLogEnum logType, string ip = null)
        {
            AddMemberLog(GetMemberId(), title, log, logType, ip);
        }

        /// <summary>
        /// Adds a note in the member log
        /// </summary>
        /// <param name="userId">The member id of the targetted user</param>
        /// <param name="title">Title of the entry</param>
        /// <param name="log">Description of the entry</param>
        /// <param name="logType">Type of log entry</param>
        /// <param name="ip">The ip of the person executing this command. Can be optained from Request.UserHostAddress or Request.ServerVariables</param>
        public static void AddMemberLog(Guid memberId, string title, string log, MemberLogEnum logType, string ip = null)
        {
            try
            {
                var dc = new ManagementContext();
                var member = GetMemberWithMemberId(ref dc, memberId);
                var issuedBy = string.Empty;
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    //var currentMemberId = GetMemberId();

                    //if (!currentMemberId.Equals(memberId))
                    //{
                    var currentMember = GetMember(ref dc);
                    if (currentMember == null)
                        issuedBy = "\n\nIssued by: Admin, with email: " + Membership.GetUser().Email;
                    else
                        issuedBy = string.Format("\n\nIssued by: {0} - {1}, with user id: {2}", currentMember.Firstname,
                                                 currentMember.DerbyName, currentMember.MemberId);
                    //}
                }
                if (member != null)
                {
                    member.Logs.Add(new MemberLog
                    {
                        Member = member,
                        Details = log + issuedBy,
                        LogReason = dc.MemberLogReasons.First(x => x.MemberLogReasonId.Equals((byte)logType)),
                        Ip = ip,
                        LogTitle = title
                    });
                }
                dc.SaveChanges();
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, e.GetType(), errorGroup: ErrorGroupEnum.Database);
            }
        }
        /// <summary>
        /// adds billing information to the member contact information.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        public static bool AddContactToMember(Guid memberId, InvoiceContactInfo contact, AddressTypeEnum type)
        {
            try
            {
                var dc = new ManagementContext();
                int country = Convert.ToInt32(contact.Country);
                var mem = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                if (mem.ContactCard != null && mem.ContactCard.Addresses.Count > 0)
                {
                    bool addAddress = true;
                    foreach (var address in mem.ContactCard.Addresses)
                    {
                        //checking if address exists.
                        if (address.Address1 == contact.Street && address.Address2 == contact.Street2 && address.Zip == contact.Zip)
                            addAddress = false;
                    }
                    if (addAddress)
                    {
                        ContactCard.ContactCardFactory.AddAddressToContact(contact.Street, contact.Street2, contact.City, contact.State, contact.Zip, type, mem.ContactCard, dc.Countries.Where(x => x.CountryId == country).FirstOrDefault());
                    }
                }
                else if (mem.ContactCard != null && mem.ContactCard.Addresses.Count == 0)
                {

                    ContactCard.ContactCardFactory.AddAddressToContact(contact.Street, contact.Street2, contact.City, contact.State, contact.Zip, type, mem.ContactCard, dc.Countries.Where(x => x.CountryId == country).FirstOrDefault());
                }
                else if (mem.ContactCard == null)
                {

                    mem.ContactCard = new DataModels.ContactCard.ContactCard();
                    ContactCard.ContactCardFactory.AddAddressToContact(contact.Street, contact.Street2, contact.City, contact.State, contact.Zip, type, mem.ContactCard, dc.Countries.Where(x => x.CountryId == country).FirstOrDefault());

                }
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, e.GetType(), errorGroup: ErrorGroupEnum.Database);
            }
            return false;
        }
    }
}
