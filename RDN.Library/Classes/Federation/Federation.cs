using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Context;
using RDN.Library.Classes.Federation.Enums;
using RDN.Library.Classes.Error;
using RDN.Utilities.Config;
using System.Web;
using RDN.Library.DataModels.Federation;
using RDN.Library.Classes.Account.Enums;
using RDN.Library.Classes.Account.Classes;
using System.Data.OleDb;
using System.Data;
using RDN.Portable.Config;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Imaging;
using RDN.Portable.Classes.Federation;
using RDN.Portable.Classes.Forum.Enums;
using RDN.Library.Classes.Config;

namespace RDN.Library.Classes.Federation
{
    public class Federation
    {
        public Guid FederationId { get; set; }
        public string FederationName { get; set; }


        public static List<string> SearchForFederationName(string q, int limit)
        {
            List<string> names = new List<string>();
            try
            {
                var dc = new ManagementContext();
                var name = (from xx in dc.Federations
                            where xx.Name.Contains(q)
                            select new
                            {
                                xx.Name,
                                xx.FederationId
                            }).Take(limit).ToList();

                for (int i = 0; i < name.Count; i++)
                    names.Add(name[i].Name + "~" + name[i].FederationId.ToString().Replace("-", ""));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return names;
        }



        /// <summary>
        /// removes the member from connection to the federation, but doesn't actually delete the member.
        /// </summary>
        /// <param name="federationId"></param>
        /// <param name="memberId"></param>
        public static void DisconnectMemberFromFederation(Guid federationId, Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var mem = dc.FederationMembers.Where(x => x.Member.MemberId == memberId).Where(x => x.Federation.FederationId == federationId).FirstOrDefault();
                mem.Member = mem.Member;
                mem.Federation = mem.Federation;
                mem.IsRemoved = true;
                var u = dc.SaveChanges();
                if (u == 0)
                    throw new Exception("changes weren't accepted " + memberId + ":" + federationId);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        public static void DisconnectLeagueFromFederation(Guid federationId, Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var league = dc.FederationLeagues.Where(x => x.League.LeagueId == leagueId && x.Federation.FederationId == federationId).FirstOrDefault();
                if (league != null)
                    dc.FederationLeagues.Remove(league);

                var u = dc.SaveChanges();
                if (u == 0)
                    throw new Exception("league didn't disconnect from federation " + leagueId + ":" + federationId);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }


        /// <summary>
        /// gets the new members added to all leagues within the federation since the last login of the federation owner.
        /// </summary>
        /// <param name="federationId"></param>
        /// <param name="lastLoginDate"></param>
        /// <returns></returns>
        [Obsolete]
        public static List<MemberDisplayFactory> GetNewMembersAddedToLeaguesSinceLogin(Guid federationId, DateTime lastLoginDate)
        {
            List<RDN.Library.Classes.Account.Classes.MemberDisplayFactory> membersTemp = new List<Account.Classes.MemberDisplayFactory>();
            try
            {
                //var dc = new ManagementContext();
                //var mems = dc.FederationLeagues.Where(x => x.Federation.FederationId == federationId).Where(x=>x.League.Members.Select(x=>x.Created)).ToList();
                //var members = MemberDisplay.IterateMembersForDisplay(mems);
                //return members;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }


        /// <summary>
        /// gets the total number of members in the federation
        /// </summary>
        /// <param name="federationId"></param>
        /// <returns></returns>
        public static int GetNumberOfMembersInFederation(Guid federationId)
        {
            try
            {
                var dc = new ManagementContext();
                return dc.FederationMembers.Where(x => x.Federation.FederationId == federationId && x.IsRemoved == false).Count();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }

        /// <summary>
        /// gets all the members of the federation for the paging view.
        /// </summary>
        /// <param name="federationId"></param>
        /// <returns></returns>
        public static List<MemberDisplayFederation> GetMembersOfFederation(Guid federationId, int recordsToSkip, int numberOfRecordsToPull)
        {
            List<MemberDisplayFederation> membersTemp = new List<MemberDisplayFederation>();
            try
            {
                var dc = new ManagementContext();
                var members = dc.FederationMembers.Include("Member").Where(x => x.Federation.FederationId == federationId && x.IsRemoved == false).OrderBy(x => x.Member.DerbyName).Skip(recordsToSkip).Take(numberOfRecordsToPull).ToList();

                membersTemp.AddRange(MemberDisplayFactory.IterateMembersForDisplay(members));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return membersTemp;
        }

        public static List<MemberDisplayFederation> GetMembersOfFederation(Guid federationId)
        {
            List<MemberDisplayFederation> membersTemp = new List<MemberDisplayFederation>();
            try
            {
                var dc = new ManagementContext();
                var members = dc.FederationMembers.Include("Member").Where(x => x.Federation.FederationId == federationId && x.IsRemoved == false).ToList();
                membersTemp.AddRange(MemberDisplayFactory.IterateMembersForDisplay(members));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return membersTemp;
        }

        /// <summary>
        /// gets the total number of leagues in the federation
        /// </summary>
        /// <param name="federationId"></param>
        /// <returns></returns>
        public static int GetNumberLeaguesInFederations(Guid federationId)
        {
            try
            {
                var dc = new ManagementContext();
                return dc.FederationLeagues.Where(x => x.Federation.FederationId == federationId).Count();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }
        public static int GetNumberOfFederations()
        {
            try
            {
                var dc = new ManagementContext();
                return dc.Federations.Where(x => x.IsVerified == true).AsParallel().Count();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }

        /// <summary>
        /// gets all the federations that still need approval.
        /// </summary>
        /// <returns></returns>
        public static List<DataModels.Federation.Federation> GetAllUnApprovedFederations(int recordsToSkip, int numberOfRecordsToPull)
        {
            try
            {
                var dc = new ManagementContext();
                return dc.Federations.Include("Owners").Where(x => x.IsVerified == false).OrderBy(x => x.Created).Skip(recordsToSkip).Take(numberOfRecordsToPull).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        /// <summary>
        /// gets the number of unapproved federations.
        /// </summary>
        /// <returns></returns>
        public static int GetNumberOfUnApprovedFederations()
        {
            try
            {
                var dc = new ManagementContext();
                return dc.Federations.Where(x => x.IsVerified == false).Count();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }

        public static void DeleteFederation(Guid federationId)
        {
            try
            {
                var dc = new ManagementContext();
                var federation = dc.Federations.Where(x => x.FederationId == federationId).FirstOrDefault();
                if (federation != null)
                {
                    dc.Federations.Remove(federation);
                    dc.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }
        /// <summary>
        /// approves the federation and emails the new owner
        /// </summary>
        /// <param name="federationId"></param>
        /// <returns>the guid of the member so we can clear their cache</returns>
        public static Guid ApproveFederation(Guid federationId)
        {
            try
            {
                var dc = new ManagementContext();
                var federation = dc.Federations.Where(x => x.FederationId == federationId).FirstOrDefault();
                if (federation != null)
                {
                    federation.IsVerified = true;

                    int ownerType = Convert.ToInt32(FederationOwnerEnum.Owner);
                    var owner = federation.Owners.Where(x => x.OwnerType == ownerType).FirstOrDefault();
                    if (owner != null)
                        owner.IsVerified = true;
                    if (owner.Member == null)
                        owner.Member = dc.FederationMembers.Where(x => x.Federation.FederationId == federationId).FirstOrDefault().Member;


                    dc.SaveChanges();
                    //sends and email to the approved user.
                    SendEmailAboutFederationApproved(federation.Name, owner.Member.DerbyName, federation.ContactCard.Emails.FirstOrDefault().EmailAddress);
                    //sends and email to us just so we know when we clicked the approval button, an email got sent.
                    SendEmailAboutFederationApproved(federation.Name, "Boss", LibraryConfig.DefaultInfoEmail);
                    return owner.Member.MemberId;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }

        /// <summary>
        /// checks if the member is the owner of a federation
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static bool IsOwnerOfFederation(Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var owners = dc.FederationOwners.Where(x => x.Member.MemberId == memberId).ToList();

                if (owners != null && owners.Count > 0)
                {
                    var check = owners.Where(x => x.OwnerType == Convert.ToInt32(FederationOwnerEnum.Owner)).FirstOrDefault();
                    if (check != null)
                        return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;

        }
        public static List<FederationOwnership> GetAllOwnedFederations(Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                return dc.FederationOwners.Where(x => x.Member.MemberId == memberId).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        /// <summary>
        /// gets all the owners/operators of the federation
        /// </summary>
        /// <param name="federationId"></param>
        /// <returns></returns>
        private static List<DataModels.Federation.FederationOwnership> GetAllOwnersOfFederation(Guid federationId)
        {
            var dc = new ManagementContext();
            return dc.FederationOwners.Where(x => x.Federation.FederationId == federationId).ToList();
        }
        /// <summary>
        /// gets the federation Datamodel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataModels.Federation.Federation GetFederation(Guid id)
        {
            var dc = new ManagementContext();
            return dc.Federations.Where(x => x.FederationId == id).FirstOrDefault();

        }
        public static FederationDisplay GetFederationDisplay(Guid id)
        {
            FederationDisplay fed = new FederationDisplay();
            var dc = new ManagementContext();
            var f = dc.Federations.Where(x => x.FederationId == id).FirstOrDefault();
            fed.FederationId = f.FederationId;
            fed.FederationName = f.Name;

            if (f.ContactCard != null)
            {
                if (f.ContactCard.Addresses.FirstOrDefault() != null)
                {
                    fed.Address = f.ContactCard.Addresses.FirstOrDefault().Address1;
                    fed.City = f.ContactCard.Addresses.FirstOrDefault().CityRaw;
                    fed.State = f.ContactCard.Addresses.FirstOrDefault().StateRaw;
                    if (f.ContactCard.Addresses.FirstOrDefault().Country != null)
                        fed.Country = f.ContactCard.Addresses.FirstOrDefault().Country.Code;
                }
                if (f.ContactCard.Emails.FirstOrDefault() != null)
                {
                    fed.Email = f.ContactCard.Emails.FirstOrDefault().EmailAddress;
                }
            }
            fed.Founded = f.Founded;
            fed.Website = f.Website;
            if (f.Logo != null)
                fed.Logo = new PhotoItem(f.Logo.ImageUrl, f.Logo.IsPrimaryPhoto, f.Logo.AlternativeText);

            fed.Members.AddRange(MemberDisplayFactory.IterateMembersForDisplay(f.Members.Where(x => x.IsRemoved == false).ToList()).OrderBy(x => x.DerbyName));
            fed.Leagues.AddRange(League.LeagueFactory.IterateThroughLeaguesForDisplay(f.Leagues.Select(x => x.League).ToList()));
            return fed;
        }
        /// <summary>
        /// gets the federation for the league.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<Federation> GetFederationsWithLeagueId(Guid id)
        {
            try
            {
                List<Federation> feds = new List<Federation>();
                var dc = new ManagementContext();
                var leagues = dc.FederationLeagues.Where(x => x.League.LeagueId == id).ToList();
                foreach (var league in leagues)
                {
                    Federation f = new Federation();
                    f.FederationId = league.Federation.FederationId;
                    f.FederationName = league.Federation.Name;
                    feds.Add(f);
                }
                return feds;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static Federation GetFederationWithLeagueId(Guid id)
        {
            try
            {
                List<Federation> feds = new List<Federation>();
                var dc = new ManagementContext();
                var leagues = dc.FederationLeagues.Where(x => x.League.LeagueId == id).ToList();
                if (leagues.FirstOrDefault() != null)
                {
                    Federation f = new Federation();
                    f.FederationId = leagues.FirstOrDefault().Federation.FederationId;
                    f.FederationName = leagues.FirstOrDefault().Federation.Name;
                    return f;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        /// <summary>
        /// gets all the federations we currently have registered.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Guid, string> GetFederations()
        {
            var dc = new ManagementContext();
            return dc.Federations.ToDictionary(key => key.FederationId, value => value.Name);
        }
        public static List<FederationDisplay> GetFederationsForDisplay()
        {
            var dc = new ManagementContext();
            var fed = (from xx in dc.Federations
                       where xx.IsVerified == true
                       select new FederationDisplay
                       {
                           FederationId = xx.FederationId,
                           LeaguesCount = xx.Leagues.Count,
                           MembersCount = xx.Members.Count,
                           FederationName = xx.Name
                       }).ToList();
            return fed;
        }
        /// <summary>
        /// creates a federation from the current member
        /// </summary>
        /// <param name="nameOfFederation"></param>
        /// <param name="emailAddress"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static DataModels.Federation.Federation CreateFederation(string nameOfFederation, string emailAddress, string phoneNumber, DataModels.Member.Member owner)
        {
            try
            {
                var dc = new ManagementContext();

                var email = new DataModels.ContactCard.Email();
                email.EmailAddress = emailAddress;
                email.IsDefault = true;

                var communication = new DataModels.ContactCard.Communication();
                communication.Data = phoneNumber;
                communication.IsDefault = true;
                int phoneNumberCast = Convert.ToInt32(CommunicationTypeEnum.PhoneNumber);
                //communication.CommunicationType = dc.CommunicationType.Where(x => x.CommunicationTypeId == phoneNumberCast).FirstOrDefault();
                communication.CommunicationTypeEnum = (byte)CommunicationTypeEnum.PhoneNumber;

                var card = new DataModels.ContactCard.ContactCard();
                card.Emails.Add(email);
                card.Communications.Add(communication);

                var fedOwner = new DataModels.Federation.FederationOwnership();
                fedOwner.IsVerified = false;
                var mem = dc.Members.Where(x => x.MemberId == owner.MemberId).FirstOrDefault();
                fedOwner.Member = mem;
                fedOwner.OwnerType = Convert.ToInt32(FederationOwnerEnum.Owner);

                var federation = new DataModels.Federation.Federation();
                federation.Name = nameOfFederation;
                federation.ContactCard = card;
                federation.IsVerified = false;
                federation.Owners.Add(fedOwner);

                //adds the current member to the federation they are creating.
                var fedMembers = new DataModels.Federation.FederationMember
                {
                    Federation = federation,
                    Member = mem,
                    FederationIdForMember = ""
                };

                federation.Members.Add(fedMembers);

                dc.Federations.Add(federation);
                dc.SaveChanges();

                Forum.Forum.CreateNewForum(federation.FederationId, ForumOwnerTypeEnum.league, nameOfFederation + "'s Forum");

                SendEmailAboutNewFederation(nameOfFederation, federation.FederationId, owner.DerbyName, emailAddress, phoneNumber);
                return federation;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: "Fed Name: " + nameOfFederation + " email:" + emailAddress + " phone:" + phoneNumber + " owner:" + owner.MemberId);
            }
            return null;
        }
        /// <summary>
        /// sens and email to the admins to let them know a new federation was created and needs approval.
        /// </summary>
        /// <param name="federationName"></param>
        /// <param name="memberName"></param>
        /// <param name="federationEmail"></param>
        /// <param name="federationPhone"></param>
        private static void SendEmailAboutNewFederation(string federationName, Guid federationId, string memberName, string federationEmail, string federationPhone)
        {
            try
            {
                var emailData = new Dictionary<string, string>
                                    {
                                        {"name", federationName},
                                        {"membername", memberName},
                                        {"phone", federationPhone},
                                        {"email", federationEmail},
                                        {"id", federationId.ToString()},
                                        {"link", LibraryConfig.AdminSite + FederationConfig.ADMIN_APPROVAL_LINK_FOR_NEW_FED}
                                    };

                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, LibraryConfig.DefaultInfoEmail, EmailServer.EmailServer.DEFAULT_SUBJECT + " New Federation Created", emailData, layout: EmailServer.EmailServerLayoutsEnum.NewFederationAdmin);


                // ToDo: To be removed
                //                var message = string.Format(@"Hey Boss,<br/><br/> A new Federation just registered at RDNation.com.<br/><br/>
                //So get your ASS over there and approve them!<br/><br/>
                //The Name of the Federation is: {0}<br/><br/>
                //The Person to Contact is: {1}<br/>
                //The Contact Number is: {2}<br/>
                //The Contact Email is: {3}<br/><br/>
                //The Federation Id is: {4}<br/>
                //Here is the approval link once you get your sweet ass out of bed to do it: {5}", federationName, memberName, federationPhone, federationEmail, federationId, FederationConfig.ADMIN_APPROVAL_LINK_FOR_NEW_FED);

                // 

                //Util.Email.SendEmail(false, LibraryConfig.DEFAULT_ADMIN_EMAIL, "New Federation Created - RDNation.com", message);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: "Fed Name: " + federationName + "Fed Id: " + federationId + " email:" + federationEmail + " phone:" + federationPhone + " owner:" + memberName);
            }
        }
        /// <summary>
        /// sends an email to the approved federations owner.
        /// </summary>
        /// <param name="federationName"></param>
        /// <param name="memberName"></param>
        /// <param name="federationEmail"></param>
        private static void SendEmailAboutFederationApproved(string federationName, string memberName, string federationEmail)
        {
            try
            {
                var emailData = new Dictionary<string, string>
                                    {
                                        {"membername", memberName},
                                        {"federationname", federationName},
                                        {"federationlink", LibraryConfig.InternalSite + FederationConfig.LINK_FOR_APPROVED_FEDERATION_OWNERS},
                                        {"contactemail", LibraryConfig.DefaultInfoEmail}
                                    };

                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, federationEmail, EmailServer.EmailServer.DEFAULT_SUBJECT + " Federation Approved", emailData, layout: EmailServer.EmailServerLayoutsEnum.FederationApproved);

                // ToDo: To be removed
                //                var message = string.Format(@"{0},<br/><br/> Your Federation '{1}' has just been approved by and RDNation.com!<br/><br/>
                //You may now login to http://RDNation.com and check it out.
                //The name of your Federation is: {1}<br/><br/>
                //Let us show you what your federation can now do at <a href='{2}'>{2}</a>.<br/><br/>
                //Thanks for choosing and bruising with RDNation.com.  If you have any questions, feel free to contact us at {3}.<br/><br/>
                //Veggie Delight and RDNation's Team.", memberName, federationName, FederationConfig.LINK_FOR_APPROVED_FEDERATION_OWNERS, LibraryConfig.DEFAULT_ADMIN_EMAIL);

                //Util.Email.SendEmail(false, federationEmail, federationName + " ", message);  
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: "Fed Name: " + federationName + " email:" + federationEmail + " owner:" + memberName);
            }
        }

        ///Used this to Import Members from a MADE members list
        //public static void InportMembersFromList()
        //{

        //    var fileName = @"C:\temp\Book1.xls";
        //    var connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", fileName);

        //    var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
        //    var ds = new DataSet();

        //    adapter.Fill(ds, "anyNameHere");
        //    List<MadeMember> mems = new List<MadeMember>();
        //    DataTable data = ds.Tables["anyNameHere"];
        //    foreach (DataRow row in data.Rows)
        //    {

        //        MadeMember member = new MadeMember();
        //        member.added = row[0].ToString();
        //        member.number = row[1].ToString();
        //        member.active = row[2].ToString();
        //        member.classs = row[3].ToString();
        //        member.sex = row[4].ToString();
        //        member.league = row[5].ToString();
        //        member.name = row[6].ToString();
        //        mems.Add(member);

        //    }

        //    var dc = new ManagementContext();
        //    int matches = 0;
        //    foreach (var mem in mems)
        //    {
        //        var memDb = dc.Members.Where(x => x.DerbyName.ToLower().Contains(mem.name.ToLower())).FirstOrDefault();
        //        if (memDb == null)
        //        {
        //            MemberDisplay dis = new MemberDisplay();
        //            dis.DerbyName = mem.name;
        //            dis.Email = "";
        //            dis.Firstname = "";
        //            dis.LastName = "";
        //            dis.PlayerNumber = "";
        //            if (!String.IsNullOrEmpty(mem.sex))
        //            {
        //                if (mem.sex.ToLower() == GenderEnum.Female.ToString().ToLower())
        //                    dis.Gender = GenderEnum.Female;
        //                else if (mem.sex.ToLower() == GenderEnum.Male.ToString().ToLower())
        //                    dis.Gender = GenderEnum.Male;
        //                else
        //                    dis.Gender = GenderEnum.None;
        //            }
        //            else
        //                dis.Gender = GenderEnum.None;

        //            if (!String.IsNullOrEmpty(mem.number))
        //                dis.FederationIdForMember = Convert.ToInt32(mem.number);
        //            dis.FederationMemberType = mem.active;
        //            dis.MadeClassRank = mem.classs;

        //            if (!String.IsNullOrEmpty(mem.league))
        //            {
        //                var league = dc.Leagues.Where(x => x.Name.ToLower().Contains(mem.league.ToLower())).FirstOrDefault();
        //                if (league != null)
        //                    dis.Team = league.LeagueId.ToString();
        //                else
        //                {
        //                    var leagueId = RDN.Library.Classes.League.League.CreateLeagueForImport(mem.league, "0", "Virginia", "Fairfax", -5, "info@rdnation.com", "0", new Guid("9323d367-ca5f-414d-bfce-666449bc94f3"));
        //                    dis.Team = leagueId.ToString();

        //                }
        //            }

        //            RDN.Library.Classes.Account.User.CreateMemberForFederation(dis, new Guid("9323d367-ca5f-414d-bfce-666449bc94f3"));
        //        }
        //    }
        //}


    }
}
