using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using RDN.Library.DataModels.Admin.LeagueContacts;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.ContactCard;
using RDN.Library.DataModels.EmailServer;
using RDN.Library.DataModels.Location;
using RDN.Library.DataModels.Member;
using RDN.Library.DataModels.Game;
using RDN.Library.DataModels.PaymentGateway.Invoices;
using RDN.Library.DataModels.PaymentGateway.Merchants;
using RDN.Library.DataModels.Roster;
using RDN.Library.DataModels.Store;
using RDN.Library.DataModels.Team;
using RDN.Library.DataModels.Scoreboard;
using RDN.Library.DataModels.Utilities;
using System.Data.Entity.Validation;
using System.Diagnostics;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Dues;
using RDN.Library.DataModels.PaymentGateway.Stripe;
using RDN.Library.DataModels.Messages;
using RDN.Library.DataModels.PaymentGateway.Paypal;
using RDN.Library.DataModels.Game.Members;
using RDN.Library.DataModels.Game.Tournaments;
using RDN.Library.DataModels.Game.Officials;
using RDN.Library.DataModels.Calendar;
using RDN.Library.DataModels.Controls.Voting;
using RDN.Library.DataModels.TimeZone;
using RDN.Library.DataModels.Admin.Download;
using RDN.Library.DataModels.Admin.Email;
using RDN.Library.DataModels.PaymentGateway.Money;
using RDN.Library.DataModels.League.Report;
using RDN.Library.DataModels.Site;
using RDN.Library.DataModels.Bouts;
using RDN.Library.DataModels.Game.Teams;
using RDN.Library.DataModels.League;
using RDN.Library.DataModels.League.OrganizationChart;
using RDN.Library.DataModels.Officials;
using RDN.Library.DataModels.League.Task;
using RDN.Library.DataModels.RN.Posts;
using RDN.Library.DataModels.EmailServer.Subscriptions;
using RDN.Library.DataModels.Social;
using RDN.Library.DataModels.Controls.Forum;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using RDN.Library.DataModels.League.Links;

namespace RDN.Library.DataModels.Context
{
    internal class ManagementContext : DbContext
    {
        // **************** Federation management **************** \\
        public DbSet<Federation.Federation> Federations { get; set; }
        public DbSet<Federation.FederationOwnership> FederationOwners { get; set; }
        public DbSet<Federation.FederationMember> FederationMembers { get; set; }
        public DbSet<Federation.FederationLeague> FederationLeagues { get; set; }

        // **************** Organization Chart management **************** \\
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Organize> Organizes { get; set; }
        public DbSet<Organization> Organizations { get; set; }

        // **************** League management **************** \\
        public DbSet<League.League> Leagues { get; set; }
        public DbSet<League.LeaguePhoto> LeagueLogos { get; set; }
        public DbSet<Team.Team> Teams { get; set; }
        public DbSet<League.Pending> LeaguePendings { get; set; }
        public DbSet<League.PendingMember> LeagueMemberPendings { get; set; }
        public DbSet<TeamLogo> TeamLogos { get; set; }
        public DbSet<League.LeagueOwnership> LeagueOwners { get; set; }
        public DbSet<League.LeagueMember> LeagueMembers { get; set; }
        public DbSet<League.LeagueMemberClass> LeagueMemberClasses { get; set; }
        public DbSet<League.Group.Group> LeagueGroups { get; set; }
        public DbSet<League.Group.GroupMember> LeagueGroupMembers { get; set; }
        public DbSet<League.Group.GroupPhoto> LeagueGroupPhotos { get; set; }
        public DbSet<League.Documents.LeagueDocument> LeagueDocuments { get; set; }
        public DbSet<League.Documents.DocumentComment> DocumentComments { get; set; }
        public DbSet<League.Documents.DocumentCategory> LeagueDocumentFolders { get; set; }
        public DbSet<League.Documents.DocumentTag> DocumentTags { get; set; }
        public DbSet<League.LeagueContacts> LeagueContacts { get; set; }
        public DbSet<League.LeagueColor> LeagueColors { get; set; }
        public DbSet<LeagueReport> LeagueReports { get; set; }
        public DbSet<JobBoard> JobBoards { get; set; }
        public DbSet<Sponsorship> Sponsorships { get; set; }
        public DbSet<ItemInfo> ItemInfos { get; set; }
        public DbSet<RefereeRequest> Requests { get; set; }
        public DbSet<TaskList> TaskLists { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Links> Links { get; set; }

        // **************** Document management **************** \\     
        public DbSet<DataModels.Document.Document> Documents { get; set; }

        // ****************Tag Management **************** \\   
        public DbSet<DataModels.Tags.Tag> Tags { get; set; }
       

        // **************** Member management **************** \\                
        public DbSet<Member.Member> Members { get; set; }
        public DbSet<Member.MemberPhoto> MemberPhotos { get; set; }
        public DbSet<EmailVerification> EmailVerifications { get; set; }
        public DbSet<MemberLog> MemberLog { get; set; }
        public DbSet<MemberLogReason> MemberLogReasons { get; set; }
        public DbSet<Communication> Communications { get; set; }
        public DbSet<CommunicationType> CommunicationType { get; set; }
        public DbSet<MemberMedical> MemberMedical { get; set; }
        public DbSet<MemberContact> MemberContacts { get; set; }
        public DbSet<MemberSettings> MemberSettings { get; set; }
        public DbSet<MemberNotifications> MemberNotifications { get; set; }


        // **************** Admin **************** \\
        public DbSet<ContactLeague> ContactLeagues { get; set; }
        public DbSet<LeagueAddress> ContactLeagueAddresses { get; set; }
        public DbSet<LeagueAssociation> ContactLeagueAssociations { get; set; }
        public DbSet<LeagueType> ContactLeagueTypes { get; set; }
        public DbSet<Admin.Download.ScoreboardDownload> ScoreboardDownloads { get; set; }
        public DbSet<Admin.RefContacts.RefMasterRoster> RefRoster { get; set; }
        public DbSet<ContactCard.Email> EmailsForAllEntities { get; set; }
        public DbSet<NonSubscribersList> NonSubscribersList { get; set; }
        public DbSet<SubscribersList> SubscribersList { get; set; }
        public DbSet<AdminEmailMessages> AdminEmailMessages { get; set; }
        [Obsolete("Use Common")]
        public DbSet<EmailSendItem> EmailServer{ get; set; }
        [Obsolete("Use Common")]
        public DbSet<EmailProperty> EmailServerProperties { get; set; }


        // **************** Game **************** \\
        public DbSet<Game.Game> Games { get; set; }
        public DbSet<GameAdvertisement> GameAdvertisements { get; set; }
        public DbSet<GameJam> GameJams { get; set; }
        public DbSet<GameLeadJammer> GameJamsLeadJammer { get; set; }
        public DbSet<GameMember> GameMembers { get; set; }
        public DbSet<GameTeam> GameTeam { get; set; }
        public DbSet<GameMemberPenaltyBox> GameMemberPenaltyBox { get; set; }
        public DbSet<GameMemberPenalty> GameMemberPenalty { get; set; }
        public DbSet<GameMemberAssist> GameMemberAssist { get; set; }
        public DbSet<GameMemberBlock> GameMemberBlock { get; set; }
        public DbSet<GamePolicy> GamePolicy { get; set; }
        public DbSet<GameScore> GameScore { get; set; }
        public DbSet<GameStopwatch> GameStopWatch { get; set; }
        public DbSet<GameTimeout> GameTimeOut { get; set; }
        public DbSet<GameLink> GameLinks { get; set; }
        public DbSet<GameMemberPhoto> GameMemberPhotos { get; set; }
        public DbSet<GameConversation> GameConversations { get; set; }
        public DbSet<GameTournament> GameTournaments { get; set; }
        public DbSet<TournamentOwner> TournamentOwners { get; set; }
        public DbSet<TournamentRound> TournamentRounds { get; set; }
        public DbSet<TournamentPairing> TournamentPairings { get; set; }
        public DbSet<TournamentPairingTeam> TournamentTeams { get; set; }
        public DbSet<TournamentConversation> TournamentConversations { get; set; }
        public DbSet<GameTournamentLogo> GameTournamentsLogo { get; set; }
        public DbSet<GameLeagueOwnership> GameLeagueOwners { get; set; }
        public DbSet<GameFederationOwnership> GameFederationOwners { get; set; }
        public DbSet<GameMemberOwnership> GameMemberOwners { get; set; }
        public DbSet<GameOfficial> GameOfficials { get; set; }
        public DbSet<GameOfficialPhoto> GameOfficialPhotos { get; set; }
        public DbSet<BoutList> BoutLists { get; set; }
        public DbSet<GameTeamLineupSettings> GameTeamLineUpSettings { get; set; }


        public DbSet<RNScore> RNScores { get; set; }


        // **************** Scoreboard **************** \\
        public DbSet<ScoreboardFeedback> ScoreboardFeedback { get; set; }
        public DbSet<ScoreboardInstance> ScoreboardInstance { get; set; }

        // **************** Locations **************** \\
        public DbSet<Location.Location> Locations { get; set; }
        public DbSet<ContactCard.Address> Addresses { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CountryTimeZone> CountriesTimeZone { get; set; }
        public DbSet<TimeZone.TimeZone> TimeZone { get; set; }
        public DbSet<Zone> Zone { get; set; }

        // **************** Misc **************** \\
        public DbSet<Sitemap> SiteMap { get; set; }


        // **************** Email **************** \\
        //public DbSet<EmailSendItem> EmailServer { get; set; }
        public DbSet<Subscriber> SubscriptionSubscriber { get; set; }
        public DbSet<SubscriptionOwner> SubscriptionOwners { get; set; }
        public DbSet<SubscriptionList> SubscriptionLists { get; set; }
        public DbSet<EmailBlast> EmailBlasts { get; set; }

        // **************** Communication **************** \\
        public DbSet<Contacts.Contact> Contacts { get; set; }

        // **************** Error **************** \\
        public DbSet<Exception.Exception> ErrorExceptions { get; set; }

        // **************** BruiseBash **************** \\
        public DbSet<BruiseBash.BruiseBashImage> BruiseBashImages { get; set; }
        public DbSet<BruiseBash.BruiseBashComment> BruiseBashComments { get; set; }
        public DbSet<BruiseBash.BruiseBashItem> BruiseBashItem { get; set; }
        public DbSet<BruiseBash.BruiseBashRating> BruiseBashRatings { get; set; }

        // **************** Forum **************** \\
        public DbSet<Forum.Forum> Forums { get; set; }
        public DbSet<Forum.ForumMessage> ForumMessages { get; set; }
        public DbSet<Forum.ForumTopic> ForumTopics { get; set; }
        public DbSet<Forum.ForumCategories> ForumCetegories { get; set; }
        public DbSet<Forum.ForumTopicInbox> ForumInbox { get; set; }
        public DbSet<Forum.ForumTopicWatchList> ForumWatchList { get; set; }
        public DbSet<ForumMessageAgree> ForumMessageAgree { get; set; }
        public DbSet<ForumMessageLike> ForumMessageLike { get; set; }



        // **************** Beta **************** \\
        public DbSet<Beta.BetaSignUp> BetaEmails { get; set; }

        // **************** Payments **************** \\
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceContactInfo> InvoiceAddresses { get; set; }
        public DbSet<InvoiceSubscription> InvoiceSubsriptions { get; set; }
        public DbSet<InvoicePaywall> InvoicePaywalls { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<StoreItem> StoreItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<StoreItemPhoto> StoreItemPhotos { get; set; }
        public DbSet<StoreItemCategory> StoreItemCategories { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<ShippingTable> ShippingTable { get; set; }
        public DbSet<IPNNotification> PaypalIPN { get; set; }
        public DbSet<PaymentGateway.Paywall.Paywall> Paywalls { get; set; }
        public DbSet<CurrencyExchangeRate> ExchangeRates { get; set; }

        // **************** Stripe **************** \\
        public DbSet<StripeEventDb> StripeEvents { get; set; }
        public DbSet<StripeCustomerDb> StripeCustomers { get; set; }
        public DbSet<StripeInvoiceDb> StripeInvoices { get; set; }
        public DbSet<StripeCardDb> StripeCards { get; set; }
        public DbSet<StripeChargeDb> StripeCharges { get; set; }
        public DbSet<StripePlanDb> StripePlans { get; set; }
        public DbSet<StripeSubscriptionDb> StripeSubscriptions { get; set; }


        // **************** TwoEvils **************** \\
        public DbSet<TwoEvilsProfile> ProfilesForTwoEvils { get; set; }
        public DbSet<TwoEvilsLeague> LeaguesForTwoEvils { get; set; }
        public DbSet<DerbyRosterLeague> LeaguesForDerbyRoster { get; set; }

        // **************** AutomatedTasks **************** \\
        public DbSet<AutomatedTasks.TaskForRunning> AutomatedTasks { get; set; }

        // **************** Rosters **************** \\
        public DbSet<Roster.Roster> Rosters { get; set; }
        public DbSet<Roster.RosterMember> RosterMembers { get; set; }


        // **************** Calendar **************** \\
        public DbSet<Calendar.Calendar> Calendar { get; set; }
        public DbSet<Calendar.CalendarAttendance> CalendarAttendance { get; set; }
        public DbSet<Calendar.CalendarEvent> CalendarEvents { get; set; }
        public DbSet<Calendar.EventsCalendar.EventCalendarConversation> CalendarEventsConversation { get; set; }
        public DbSet<Calendar.CalendarEventReoccuring> CalendarEventsReocurring { get; set; }
        public DbSet<Calendar.CalendarEventPoint> CalendarEventPoints { get; set; }
        public DbSet<Calendar.CalendarEventType> CalendarEventTypes { get; set; }
        public DbSet<Calendar.CalendarFederationOwnership> CalendarFederationOwners { get; set; }
        public DbSet<Calendar.CalendarLeagueOwnership> CalendarLeagueOwners { get; set; }

        // **************** FeesCollected **************** \\
        public DbSet<MemberFees.FeeManagement> FeeManagement { get; set; }
        public DbSet<MemberFees.FeeItem> FeeItem { get; set; }
        public DbSet<MemberFees.FeesCollected> FeesCollected { get; set; }
        public DbSet<MemberFees.FeeClassification> FeeClassification { get; set; }
        public DbSet<MemberFees.FeesClassificationByMember> FeeClassificationByMember { get; set; }
        public DbSet<MemberFees.FeesRequired> FeesRequired { get; set; }

        // **************** Messages **************** \\
        public DbSet<Messages.GroupMessage> GroupMessages { get; set; }
        public DbSet<Messages.MessageRecipient> MessagesRecipients { get; set; }
        public DbSet<Messages.Message> Message { get; set; }
        public DbSet<Messages.MessageInbox> MessageInbox { get; set; }

        // **************** Colors **************** \\
        public DbSet<Color.Color> Colors { get; set; }

        // **************** Mobile **************** \\
        public DbSet<MobileNotificationSettings> MobileSettings { get; set; }

        // **************** Social **************** \\
        public DbSet<SocialAuthKeys> SocialAuthKeys { get; set; }

        // **************** SiteHistory **************** \\
        public DbSet<FrontPageHistory> FrontPageHistory { get; set; }

        // **************** Polling and Voting **************** \\
        public DbSet<Voting> Voting { get; set; }
        public DbSet<Votes> Votes { get; set; }
        public DbSet<VotingAnswer> Answers { get; set; }
        public DbSet<VotingV2> VotingV2 { get; set; }
        public DbSet<VotingVoters> VotingVoters { get; set; }
        public DbSet<VotingQuestion> VotingQuestions { get; set; }


        // **************** Rinxter **************** \\
        public DbSet<RDN.Library.DataModels.Imports.Rinxter.RBouts> RBouts { get; set; }
        public DbSet<RDN.Library.DataModels.Imports.Rinxter.RLeague> RLeague { get; set; }
        public DbSet<RDN.Library.DataModels.Imports.Rinxter.RLineUps> RLineUps { get; set; }
        public DbSet<RDN.Library.DataModels.Imports.Rinxter.RPenalties> RPenalties { get; set; }
        public DbSet<RDN.Library.DataModels.Imports.Rinxter.RPosition> RPosition { get; set; }
        public DbSet<RDN.Library.DataModels.Imports.Rinxter.RRegions> RRegions { get; set; }
        public DbSet<RDN.Library.DataModels.Imports.Rinxter.RScores> RScores { get; set; }
        public DbSet<RDN.Library.DataModels.Imports.Rinxter.RSeasons> RSeasons { get; set; }
        public DbSet<RDN.Library.DataModels.Imports.Rinxter.RSkaters> RSkaters { get; set; }
        public DbSet<RDN.Library.DataModels.Imports.Rinxter.RSkaterTeam> RSkaterTeams { get; set; }
        public DbSet<RDN.Library.DataModels.Imports.Rinxter.RTeams> RTeams { get; set; }
        public DbSet<RDN.Library.DataModels.Imports.Rinxter.RTeamTournaments> RTeamTournaments { get; set; }
        public DbSet<RDN.Library.DataModels.Imports.Rinxter.RTournaments> RTournaments { get; set; }
        public DbSet<RDN.Library.DataModels.Imports.Rinxter.RLeaguePhoto> RLeaguePhoto { get; set; }
        public DbSet<RDN.Library.DataModels.Imports.Rinxter.RTeamPhoto> RTeamPhoto { get; set; }
        public DbSet<RDN.Library.DataModels.Imports.Rinxter.RTournamentPhoto> RTournamentPhoto { get; set; }
        public DbSet<RDN.Library.DataModels.Imports.Rinxter.RSkaterPhoto> RSkaterPhoto { get; set; }



        public ManagementContext()
            : base("RDN")
        {
        }

        public ManagementContext(string connectionStringName)
            : base(connectionStringName)
        {
        }

        public static void SetDataContext(string databaseConnectionName)
        {
            System.Web.HttpContext.Current.Items["ManagementDatabaseConnectionName"] = databaseConnectionName;
        }

        public static ManagementContext DataContext
        {
            get
            {
                if (System.Web.HttpContext.Current.Items["ManagementContext"] == null)
                {
                    if (System.Web.HttpContext.Current.Items["ManagementDatabaseConnectionName"] != null)
                        System.Web.HttpContext.Current.Items["ManagementContext"] = new ManagementContext(System.Web.HttpContext.Current.Items["ManagementDatabaseConnectionName"].ToString());
                    else
                        System.Web.HttpContext.Current.Items["ManagementContext"] = new ManagementContext();
                }

                return (ManagementContext)System.Web.HttpContext.Current.Items["ManagementContext"];
            }
        }

        // Automatically add the times the entity got created/modified
        public override int SaveChanges()
        {
            string tempInfo = String.Empty;
            try
            {
                try
                {
                    var entries = ChangeTracker.Entries().ToList();
                    for (int i = 0; i < entries.Count; i++)
                    {
                        if (entries[i].State == EntityState.Unchanged || entries[i].State == EntityState.Detached || entries[i].State == EntityState.Deleted) continue;

                        var hasInterfaceInheritDb = entries[i].Entity as InheritDb;
                        if (hasInterfaceInheritDb == null) continue;

                        if (entries[i].State == EntityState.Added)
                        {
                            var created = entries[i].Property("Created");
                            if (created != null)
                            {
                                created.CurrentValue = DateTime.UtcNow;
                            }
                        }
                        if (entries[i].State == EntityState.Modified)
                        {
                            var modified = entries[i].Property("LastModified");
                            if (modified != null)
                            {
                                modified.CurrentValue = DateTime.UtcNow;
                            }
                        }
                    }
                }
                catch (System.Exception dbEx)
                {
                    string additionalInfo = string.Empty;


                    ErrorDatabaseManager.AddException(dbEx, dbEx.GetType(), additionalInformation: additionalInfo);
                }
                return base.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                string additionalInfo = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        additionalInfo += "Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage + "";
                    }
                }
                ErrorDatabaseManager.AddException(dbEx, dbEx.GetType(), additionalInformation: additionalInfo);
                return 0;
            }

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove(new IncludeMetadataConvention());

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<GameMemberAssist>()
                   .HasRequired(e => e.Game)
                   .WithMany()
                   .WillCascadeOnDelete(false);

            modelBuilder.Entity<GameMemberBlock>()
                   .HasRequired(e => e.Game)
                   .WithMany()
                   .WillCascadeOnDelete(false);

            modelBuilder.Entity<GameMemberPenalty>()
                   .HasRequired(e => e.Game)
                   .WithMany()
                   .WillCascadeOnDelete(false);

            modelBuilder.Entity<GameJamPasses>()
                   .HasRequired(e => e.GameJam)
                   .WithMany()
                   .WillCascadeOnDelete(false);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //modelBuilder.Entity<Group>().HasMany(x => x.Members).WithMany(c => c.Groups)
            //    .Map(y =>
            //             {
            //                 y.MapLeftKey("GroupId");
            //                 y.MapRightKey("MemberId");
            //                 y.ToTable("RDN_Team_Group_to_Member");
            //             });            
        }
    }
}
