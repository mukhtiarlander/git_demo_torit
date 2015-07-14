using System;
using System.Linq;
using RDN.Library.Classes.Account.Enums;
using Scoreboard.Library.ViewModel;
using System.Collections.Generic;
using RDN.Library.DataModels.Member;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Federation;
using RDN.Library.Classes.Federation.Enums;
using RDN.Library.Classes.League.Enums;
using RDN.Library.Classes.Communications.Enums;
using RDN.Portable.Classes.Account.Enums;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Communications.Enums;
using RDN.Portable.Classes.Imaging;
using RDN.Portable.Classes.League.Enums;
using RDN.Library.Classes.Config;
using RDN.Portable.Classes.Insurance;

namespace RDN.Library.Classes.Account.Classes
{
    /// <summary>
    /// member class for adding members to the DB.  We use this to serialize JSON.
    /// </summary>
    public class MemberDisplayFactory
    {

        /// <summary>
        /// iterates throught the collection of members and sets them up for display to the end user.
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        public static List<MemberDisplay> IterateMembersForDisplay(ICollection<DataModels.League.LeagueMember> members)
        {
            List<MemberDisplay> membersReturned = new List<MemberDisplay>();
            foreach (var mem in members)
            {
                try
                {
                    MemberDisplay memTemp = new MemberDisplay();
                    memTemp.IsProfileRemovedFromPublicView = mem.Member.IsProfileRemovedFromPublic;
                    memTemp.DerbyName = mem.Member.DerbyName;
                    memTemp.StartedSkating = mem.Member.YearStartedSkating;
                    memTemp.Bio = mem.Member.Bio;
                    memTemp.DOB = mem.Member.DateOfBirth.GetValueOrDefault();
                    if (mem.Member.ContactCard.Emails.Count > 0)
                        memTemp.Email = mem.Member.ContactCard.Emails.Where(x => x.IsDefault == true).FirstOrDefault().EmailAddress;
                    memTemp.Firstname = mem.Member.Firstname;
                    GenderEnum gen;
                    if (Enum.TryParse<GenderEnum>(mem.Member.Gender.ToString(), out gen))
                        memTemp.Gender = gen;
                    memTemp.LastName = mem.Member.Lastname;
                    memTemp.MemberId = mem.Member.MemberId;

                    memTemp.PlayerNumber = mem.Member.PlayerNumber;
                    memTemp.DayJob = mem.Member.DayJob;

                    if (mem.Member.ContactCard != null && mem.Member.ContactCard.Communications.Count > 0)
                    {
                        var com = mem.Member.ContactCard.Communications.Where(x => x.IsDefault == true).Where(x => x.CommunicationTypeEnum == (byte)CommunicationTypeEnum.PhoneNumber).FirstOrDefault();
                        if (com != null)
                        {
                            memTemp.PhoneNumber = com.Data;
                            memTemp.SMSVerificationNum = com.SMSVerificationCode;
                            memTemp.Carrier = (MobileServiceProvider)com.CarrierType;
                            memTemp.IsCarrierVerified = com.IsCarrierVerified;
                        }

                    }
                    if (mem.Member.ContactCard != null && mem.Member.ContactCard.Addresses.Count > 0)
                    {
                        foreach (var add in mem.Member.ContactCard.Addresses)
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
                            if (add.Coords != null)
                            {
                                address.Coords = new Portable.Classes.Location.GeoCoordinate();
                                address.Coords.Latitude = add.Coords.Latitude;
                                address.Coords.Longitude = add.Coords.Longitude;
                            }

                            memTemp.ContactCard.Addresses.Add(address);
                        }
                    }

                    if (mem.League != null)
                    {
                        RDN.Portable.Classes.League.Classes.League l = new RDN.Portable.Classes.League.Classes.League();
                        l.LeagueId = mem.League.LeagueId;
                        l.Name = mem.League.Name;
                        l.DepartureDate = mem.DepartureDate;
                        l.SkillsTestDate = mem.SkillsTestDate;
                        if (mem.Member.CurrentLeagueId == l.LeagueId)
                        {
                            memTemp.IsInactiveFromCurrentLeague = mem.IsInactiveForLeague;
                            if (mem.SkaterClass != null)
                                memTemp.LeagueClassificationOfSkatingLevel = mem.SkaterClass.ClassName;
                        }

                        l.LeagueOwnersEnum = (LeagueOwnersEnum)mem.LeagueOwnersEnums;
                        if (mem.Member.CurrentLeagueId == l.LeagueId)
                            memTemp.LeagueOwnersEnum = l.LeagueOwnersEnum;
                        l.IsInactiveInLeague = mem.IsInactiveForLeague;
                        if (mem.SkaterClass != null)
                        {
                            l.SkaterClass = mem.SkaterClass.ClassId;
                            //l.SkaterLevel = mem.SkaterClass.ClassName;
                        }
                        memTemp.Leagues.Add(l);
                    }


                    if (mem.Member.AspNetUserId != null && mem.Member.AspNetUserId != new Guid())
                    {
                        memTemp.UserId = mem.Member.AspNetUserId;
                        memTemp.IsConnected = true;
                    }
                    else
                    {
                        memTemp.IsConnected = false;
                        memTemp.VerificationCode = mem.Member.MemberId.ToString().Replace("-", "") + mem.Member.Gender.ToString();
                    }

                    foreach (var photo in mem.Member.Photos.OrderBy(x => x.Created))
                    {
                        memTemp.Photos.Add(new PhotoItem(photo.ImageUrl, photo.ImageUrlThumb, photo.IsPrimaryPhoto, mem.Member.DerbyName));
                    }

                    foreach (var numb in mem.Member.InsuranceNumbers)
                    {
                        memTemp.InsuranceNumbers.Add(new InsuranceNumber()
                        {
                            Expires = numb.Expires,
                            Number = numb.InsuranceNumber,
                            Type = (InsuranceType)numb.InsuranceType
                        });
                    }

                    MedicalInformationFactory.AttachMemberMedicalInformation(mem.Member, memTemp);
                    memTemp.Settings = MemberSettingsFactory.DisplayMemberSettings(mem.Member.Settings);
                    membersReturned.Add(memTemp);

                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }

            }
            return membersReturned;
        }
        public static List<MemberDisplayFederation> IterateMembersForDisplay(ICollection<FederationMember> members)
        {
            List<MemberDisplayFederation> membersReturned = new List<MemberDisplayFederation>();
            foreach (var mem in members)
            {
                try
                {
                    MemberDisplayFederation memTemp = new MemberDisplayFederation();
                    memTemp.DerbyName = mem.Member.DerbyName;

                    if (mem.Member.ContactCard.Emails.Count > 0)
                        memTemp.Email = mem.Member.ContactCard.Emails.Where(x => x.IsDefault == true).FirstOrDefault().EmailAddress;
                    memTemp.Firstname = mem.Member.Firstname;
                    GenderEnum gen;
                    if (Enum.TryParse<GenderEnum>(mem.Member.Gender.ToString(), out gen))
                        memTemp.Gender = gen;
                    memTemp.LastName = mem.Member.Lastname;
                    memTemp.MemberId = mem.Member.MemberId;
                    memTemp.MembershipId = mem.FederationIdForMember;
                    memTemp.MadeClassRank = ((MADEClassRankEnum)Enum.ToObject(typeof(MADEClassRankEnum), mem.MADEClassRankForMember)).ToString();
                    memTemp.FederationMemberType = ((MemberTypeFederationEnum)Enum.ToObject(typeof(MemberTypeFederationEnum), mem.MemberType)).ToString();

                    if (mem.Member.ContactCard.Communications.Count > 0)
                        memTemp.PhoneNumber = mem.Member.ContactCard.Communications.Where(x => x.CommunicationTypeEnum == (byte)CommunicationTypeEnum.PhoneNumber).Where(x => x.IsDefault == true).FirstOrDefault().Data;
                    memTemp.PlayerNumber = mem.Member.PlayerNumber;
                    foreach (var league in mem.Member.Leagues)
                    {
                        RDN.Portable.Classes.League.Classes.League l = new Portable.Classes.League.Classes.League();
                        l.LeagueId = league.League.LeagueId;
                        l.Name = league.League.Name;
                        memTemp.Leagues.Add(l);
                    }
                    foreach (var photo in mem.Member.Photos.OrderBy(x => x.Created))
                    {
                        memTemp.Photos.Add(new PhotoItem(photo.ImageUrl, photo.IsPrimaryPhoto, mem.Member.DerbyName));
                    }


                    memTemp.Edit = "<a href='" + LibraryConfig.InternalSite + "/Federation/Member/Edit/" + memTemp.MemberId.ToString().Replace("-", "") + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(memTemp.DerbyName) + "'>Edit</a>";

                    memTemp.MemberUrl = "<a href='" + LibraryConfig.InternalSite + "/member/" + memTemp.MemberId.ToString().Replace("-", "") + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(memTemp.DerbyName) + "'>" + memTemp.DerbyName + "</a>";

                    membersReturned.Add(memTemp);
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
            }
            return membersReturned;
        }
    }
}