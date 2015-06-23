using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using RDN.Library.DataModels.ContactCard;
using RDN.Library.DataModels.Context;
using RDN.Library.Classes.Account.Enums;
using RDN.Utilities.Config;
using RDN.Library.DataModels.League;
using RDN.Library.Classes.League.Enums;
using System.Net;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment.Classes.Invoice;
using RDN.Library.DataModels.Federation;
using RDN.Library.Classes.Location;
using RDN.Portable.Config;
using RDN.Portable.Classes.Forum.Enums;
using RDN.Portable.Classes.League.Enums;
using RDN.Library.Classes.Config;

namespace RDN.Library.Classes.Admin.League
{
    public static class League
    {
        public static bool DeleteLogoFromLeague(string url)
        {
            try
            {
                var dc = new ManagementContext();
                var forum = dc.Leagues.Include("Logo").Where(x => x.Logo.ImageUrl == url).FirstOrDefault();
                forum.Logo = null;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: url);
            }
            return false;
        }

        public static bool AttachLeagueToFederation(Guid id, Guid fedId)
        {
            try
            {
                var dc = new ManagementContext();
                var league = dc.Leagues.Where(x => x.LeagueId == id).FirstOrDefault();
                var federation = dc.Federations.Where(x => x.FederationId == fedId).FirstOrDefault();
                FederationLeague l = new FederationLeague();
                l.Federation = federation;
                l.League = league;
                l.MembershipDate = DateTime.UtcNow;
                league.Federations.Add(l);

                var u = dc.SaveChanges();
                if (u == 0)
                    throw new Exception("league wasn't attached to federation " + id + " " + fedId);
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool DeleteLeague(Guid id)
        {
            try
            {
                var dc = new ManagementContext();
                var forum = dc.Forums.Where(x => x.LeagueOwner.LeagueId == id).FirstOrDefault();
                if (forum != null)
                    dc.Forums.Remove(forum);
                var league = dc.Leagues.Where(x => x.LeagueId == id).FirstOrDefault();
                dc.Leagues.Remove(league);
                dc.SaveChanges();
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: id.ToString());
            }
            return false;
        }
        public static bool HideLeague(Guid id)
        {
            try
            {
                var dc = new ManagementContext();

                var league = dc.Leagues.Where(x => x.LeagueId == id).FirstOrDefault();
                league.IsLeaguePublic = true;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: id.ToString());
            }
            return false;
        }

        public static List<RDN.Portable.Classes.League.Classes.League> GetSubscriptionInformationOfAllLeagues()
        {
            try
            {
                var dc = new ManagementContext();
                var leagues = (from xx in dc.Leagues
                               select xx);

                var subscriptions = (from yy in dc.InvoiceSubsriptions
                                     select yy).ToList();

                List<RDN.Portable.Classes.League.Classes.League> ls = new List<RDN.Portable.Classes.League.Classes.League>();
                foreach (var league in leagues)
                {
                    var l = Library.Classes.League.LeagueFactory.DisplayLeague(league);
                    var sub = subscriptions.Where(x => x.InternalObject == league.LeagueId).OrderByDescending(x => x.ValidUntil).FirstOrDefault();
                    if (sub != null)
                    {
                        if (sub.ValidUntil > DateTime.UtcNow)
                            l.HasCurrentSubscription = true;
                    }
                    ls.Add(l);
                }

                return ls;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<RDN.Portable.Classes.League.Classes.League>();
        }

        public static List<Classes.League> GetPendingLeagues()
        {
            try
            {
                var dc = new ManagementContext();
                var leagues = dc.LeaguePendings.Include("Creator").Include("Country").Include("Federation").OrderBy(x => x.Created);

                return leagues.Select(league => new Classes.League
                                                    {
                                                        AdditionalInformation = league.AdditionalInformation,
                                                        City = league.CityRaw,
                                                        ContactEmail = league.ContactEmail,
                                                        ContactTelephone = league.ContactTelephone,
                                                        Country = league.Country.Name,
                                                        Created = league.Created,
                                                        LeagueName = league.LeagueName,
                                                        MemberFirstname = league.Creator.Firstname,
                                                        MemberDerbyname = league.Creator.DerbyName,
                                                        PendingLeagueId = league.LeagueId,
                                                        State = league.StateRaw,
                                                        LogInformation = league.LogInformation,
                                                        Federation = league.Federation.Name
                                                    }).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<Classes.League>();
        }
        /// <summary>
        /// approves the league for federation purposes.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool ApproveLeagueForFederation(string id)
        {
            Guid leagueId;
            var guidConversion = Guid.TryParse(id, out leagueId);
            if (!guidConversion)
                return false;

            var dc = new ManagementContext();
            var pendingLeague = dc.LeaguePendings.Include("Creator").Include("Federation").Include("Country").FirstOrDefault(x => x.LeagueId.Equals(leagueId));
            if (pendingLeague == null)
                return false;

            var contactCard = new DataModels.ContactCard.ContactCard();
            var add = new Address
                                          {
                                              Country = pendingLeague.Country,
                                              StateRaw = pendingLeague.StateRaw,
                                              CityRaw = pendingLeague.CityRaw,
                                              TimeZone = pendingLeague.TimeZone
                                          };

            var coords = OpenStreetMap.FindLatLongOfAddress(string.Empty, string.Empty, string.Empty, add.CityRaw, add.StateRaw, add.Country != null ? add.Country.Name : string.Empty);
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
                EmailAddress = pendingLeague.ContactEmail,
                IsDefault = true
            });


            if (!String.IsNullOrEmpty(pendingLeague.ContactTelephone))
            {
                var communication = new DataModels.ContactCard.Communication();
                communication.Data = pendingLeague.ContactTelephone;
                communication.IsDefault = true;
                //int comType = Convert.ToInt32(CommunicationTypeEnum.PhoneNumber);
                //communication.CommunicationType = dc.CommunicationType.Where(x => x.CommunicationTypeId == comType).FirstOrDefault();
                communication.CommunicationTypeEnum = (byte)CommunicationTypeEnum.PhoneNumber;
                contactCard.Communications.Add(communication);
            }

            var league = new DataModels.League.League
                             {
                                 ContactCard = contactCard,
                                 Name = pendingLeague.LeagueName,
                                 LoweredName = pendingLeague.LeagueName.ToLower(),
                                 SubscriptionPeriodEnds = DateTime.UtcNow.AddDays(InvoiceSubscription.NUMBER_OF_DAYS_FOR_TRIAL_SUBSCRIPTION),
                                 SubscriptionWillAutoRenew = false
                             };

            if (pendingLeague.Federation != null)
            {
                FederationLeague l = new FederationLeague();
                l.League = league;
                l.Federation = pendingLeague.Federation;
                l.MembershipDate = DateTime.UtcNow;
                league.Federations.Add(l);
            }
            //league.Members.Add(pendingLeague.Creator);
            dc.Leagues.Add(league);
            var result = dc.SaveChanges();

            if (result == 0)
                return false;

            var membership = Membership.Providers["LeagueManagement"];
            var roles = Roles.Providers["LeagueManagement"];
            var user = membership.GetUser(pendingLeague.Creator.MemberId, false);
            //roles.AddUsersToRoles(new[] { user.UserName }, new[] { "League_President", "League_Member" });

            var defaultEmail = pendingLeague.Creator.ContactCard.Emails.FirstOrDefault(x => x.IsDefault.Equals(true));
            if (defaultEmail != null)
            {
                var emailData = new Dictionary<string, string>();
                emailData.Add("name", pendingLeague.Creator.Firstname);
                emailData.Add("derbyname", pendingLeague.Creator.DerbyName);
                emailData.Add("leaguename", pendingLeague.LeagueName);
                EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, defaultEmail.EmailAddress, EmailServer.EmailServer.DEFAULT_SUBJECT + " Your league has been approved", emailData, EmailServer.EmailServerLayoutsEnum.LeagueApproved);
            }

            // ToDo: To be removed
            //            var message = string.Format(@"
            //                                Hello {0} {1},
            //                                The league '{2}' has now been created. You can now login and administer the league.
            //                            ", pendingLeague.Creator.Firstname, pendingLeague.Creator.DerbyName, pendingLeague.LeagueName);


            //            SendEmail(, "ContactLeague creation successful", message);

            dc.LeaguePendings.Remove(pendingLeague);
            result = dc.SaveChanges();
            return result != 0;
        }
        /// <summary>
        /// approves the league and attaches owners and members to the league.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool ApproveLeague(string id)
        {
            int result = 0;
            try
            {
                Guid leagueId;
                var guidConversion = Guid.TryParse(id, out leagueId);
                if (!guidConversion)
                    return false;

                var dc = new ManagementContext();
                var pendingLeague = dc.LeaguePendings.Include("Creator").Include("Federation").Include("Country").FirstOrDefault(x => x.LeagueId.Equals(leagueId));
                if (pendingLeague == null)
                    return false;

                var contactCard = new DataModels.ContactCard.ContactCard();
                var add = new Address
                {
                    Country = pendingLeague.Country,
                    StateRaw = pendingLeague.StateRaw,
                    CityRaw = pendingLeague.CityRaw,
                    TimeZone = pendingLeague.TimeZone
                };

                var coords = OpenStreetMap.FindLatLongOfAddress(string.Empty, string.Empty, string.Empty, add.CityRaw, add.StateRaw, add.Country != null ? add.Country.Name : string.Empty);
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
                else
                {
                    add.Coords = new System.Device.Location.GeoCoordinate();
                    add.Coords.Latitude = 0.0;
                    add.Coords.Longitude = 0.0;
                    add.Coords.Altitude = 0;
                    add.Coords.Course = 0;
                    add.Coords.HorizontalAccuracy = 1;
                    add.Coords.Speed = 0;
                    add.Coords.VerticalAccuracy = 1;
                }
                contactCard.Addresses.Add(add);

                contactCard.Emails.Add(new RDN.Library.DataModels.ContactCard.Email
                {
                    EmailAddress = pendingLeague.ContactEmail,
                    IsDefault = true
                });


                if (!String.IsNullOrEmpty(pendingLeague.ContactTelephone))
                {
                    var communication = new DataModels.ContactCard.Communication();
                    communication.Data = pendingLeague.ContactTelephone;
                    communication.IsDefault = true;
                    //int comType = Convert.ToInt32(CommunicationTypeEnum.PhoneNumber);
                    //communication.CommunicationType = dc.CommunicationType.Where(x => x.CommunicationTypeId == comType).FirstOrDefault();
                    communication.CommunicationTypeEnum = (byte)CommunicationTypeEnum.PhoneNumber;
                    contactCard.Communications.Add(communication);
                }

                var league = new DataModels.League.League
                {
                    ContactCard = contactCard,
                    Name = pendingLeague.LeagueName,
                    LoweredName = pendingLeague.LeagueName.ToLower(),
                    SubscriptionPeriodEnds = DateTime.UtcNow.AddDays(InvoiceSubscription.NUMBER_OF_DAYS_FOR_TRIAL_SUBSCRIPTION),
                    LeagueJoinCode = Guid.NewGuid(),
                    SubscriptionWillAutoRenew = false

                };


                //we clear it by hitting a URL setup to clear the cache.
                LeagueMember me = new LeagueMember();
                me.League = league;
                me.Member = pendingLeague.Creator;
                me.MembershipDate = DateTime.UtcNow;
                me.LeagueOwnersEnums = (int)LeagueOwnersEnum.Owner;
                league.Members.Add(me);

                if (pendingLeague.Federation != null)
                {
                    FederationLeague l = new FederationLeague();
                    l.Federation = pendingLeague.Federation;
                    l.League = league;
                    l.MembershipDate = DateTime.UtcNow;
                    league.Federations.Add(l);
                }

                //league.Members.Add(pendingLeague.Creator);
                dc.Leagues.Add(league);
                result = dc.SaveChanges();
                pendingLeague.Creator.CurrentLeagueId = league.LeagueId;
                result = dc.SaveChanges();

                //    var result = 1;
                if (result == 0)
                    return false;

                Forum.Forum.CreateNewForum(league.LeagueId, ForumOwnerTypeEnum.league, league.Name + "'s Forum");

                var defaultEmail = pendingLeague.Creator.ContactCard.Emails.FirstOrDefault(x => x.IsDefault.Equals(true));
                if (defaultEmail != null)
                {
                    var emailData = new Dictionary<string, string>();
                    emailData.Add("name", pendingLeague.Creator.Firstname);
                    emailData.Add("derbyname", pendingLeague.Creator.DerbyName);
                    emailData.Add("leaguename", pendingLeague.LeagueName);
                    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, defaultEmail.EmailAddress, EmailServer.EmailServer.DEFAULT_SUBJECT + " Your league has been approved", emailData, EmailServer.EmailServerLayoutsEnum.LeagueApproved);
                }

                try
                {
                    WebClient client = new WebClient();
                    client.DownloadStringAsync(new Uri(ServerConfig.URL_TO_CLEAR_MEMBER_CACHE + pendingLeague.Creator.MemberId.ToString()));
                    WebClient client1 = new WebClient();
                    client1.DownloadStringAsync(new Uri(ServerConfig.URL_TO_CLEAR_MEMBER_CACHE_API + pendingLeague.Creator.MemberId.ToString()));
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
                dc.LeaguePendings.Remove(pendingLeague);
                result = dc.SaveChanges();

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return result != 0;
        }
        public static bool RejectLeague(string id, string rejectMessage)
        {
            Guid leagueId;
            var guidConversion = Guid.TryParse(id, out leagueId);
            if (!guidConversion)
                return false;

            var dc = new ManagementContext();
            var pendingLeague = dc.LeaguePendings.Include("Creator").FirstOrDefault(x => x.LeagueId.Equals(leagueId));
            if (pendingLeague == null)
                return false;

            //var defaultEmail = pendingLeague.Creator.ContactCard.Emails.FirstOrDefault(x => x.IsDefault.Equals(true));
            //if (defaultEmail != null)
            //{
            //    var emailData = new Dictionary<string, string>();
            //    emailData.Add("name", pendingLeague.Creator.Firstname);
            //    emailData.Add("derbyname", pendingLeague.Creator.DerbyName);
            //    emailData.Add("leaguename", pendingLeague.LeagueName);
            //    emailData.Add("rejectmessage", rejectMessage);
            //    EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultInfoEmail, LibraryConfig.DefaultEmailFromName, defaultEmail.EmailAddress, "Your league was rejected at RDNation.com", emailData, EmailServer.EmailServerLayoutsEnum.LeagueRejected);
            //}


            dc.LeaguePendings.Remove(pendingLeague);
            var result = dc.SaveChanges();
            return result != 0;
        }
    }
}
