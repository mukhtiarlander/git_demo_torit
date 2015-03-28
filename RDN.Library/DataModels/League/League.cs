using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.Bouts;
using RDN.Library.DataModels.Federation;
using RDN.Library.DataModels.League.Documents;
using RDN.Library.DataModels.League.Report;
using RDN.Library.DataModels.League.OrganizationChart;
using RDN.Library.DataModels.Officials;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.League
{
    [Table("RDN_Leagues")]
    public class League : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LeagueId { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string LoweredName { get; set; }
        /// <summary>
        /// defaults to false, but false actually means true when checking if the league is public.
        /// </summary>
        public bool IsLeaguePublic { get; set; }
        /// <summary>
        /// internal welcome message for the league
        /// </summary>
        public string InternalWelcomeMessage { get; set; }
        /// <summary>
        /// date founded.
        /// </summary>
        public DateTime? Founded { get; set; }
        public string WebSite { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public long RuleSetsPlayedEnum { get; set; } 

        public DateTime? SubscriptionPeriodEnds { get; set; }
        public bool SubscriptionWillAutoRenew { get; set; }

        public Guid LeagueJoinCode { get; set; }

        public int CultureLCID { get; set; }
        public long IntroductoryEmailEnum { get; set; }
        public string ThemeColor { get; set; }

        public virtual TimeZone.TimeZone TimeZoneSelection { get; set; }
        public int TimeZone { get; set; }
        public bool IsLeagueInUTC { get; set; } 
        
        #region References
        public virtual ICollection<ItemInfo> ItemInfo { get; set; }
        public virtual ICollection<Organization> Organization { get; set; }
        public virtual ICollection<Organize> Organize { get; set; }
        public virtual ICollection<Designation> Designation { get; set; }
        public virtual ICollection<Sponsorship> Sponsorships { get; set; }
        public virtual ICollection<JobBoard> JobBoards { get; set; }
        public virtual List<BoutList> BoutChallenges { get; set; }
        public virtual ICollection<LeagueMember> Members { get; set; }
        public virtual LeaguePhoto InternalWelcomePicture { get; set; }
        public virtual LeaguePhoto Logo { get; set; }
        [Obsolete("this column is obsolete, please use collection of federations")]
        public virtual Federation.Federation Federation { get; set; }
        public virtual ICollection<Federation.FederationLeague> Federations { get; set; }
        public virtual ContactCard.ContactCard ContactCard { get; set; }
        public virtual ICollection<News> News { get; set; }
        //public virtual ICollection<Member.Member> Members { get; set; }
        public virtual ICollection<PendingMember> PendingMembers { get; set; }
        [Obsolete]
        public virtual ICollection<Team.Team> Teams { get; set; }
        public virtual ICollection<LeagueOwnership> Owners { get; set; }
        /// <summary>
        /// groups are the same thing as teams or committees.  Group of people
        /// trying to accomplish something within each league
        /// </summary>
        public virtual ICollection<Group.Group> Groups { get; set; }
        public virtual ICollection<Documents.LeagueDocument> Documents { get; set; }
        public virtual ICollection<Documents.DocumentCategory> Folders { get; set; }
        public virtual ICollection<LeagueContacts> Contacts { get; set; }
        public virtual ICollection<LeagueReport> Reports { get; set; }
        public virtual List<LeagueColor> Colors { get; set; }
        #endregion

        #region Methods

        public League()
        {
            Contacts = new Collection<LeagueContacts>();
            Owners = new Collection<LeagueOwnership>();
            News = new Collection<News>();
            Members = new Collection<LeagueMember>();
            PendingMembers = new Collection<PendingMember>();
            Teams = new Collection<Team.Team>();
            Federations = new Collection<Federation.FederationLeague>();
            Groups = new Collection<Group.Group>();
            Documents = new Collection<Documents.LeagueDocument>();
            Reports = new Collection<LeagueReport>();
            Colors = new List<LeagueColor>();
        }

        #endregion
    }
}
