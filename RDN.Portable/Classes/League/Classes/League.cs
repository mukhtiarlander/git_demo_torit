using ProtoBuf;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Contacts;
using RDN.Portable.Classes.Federation;
using RDN.Portable.Classes.Imaging;
using RDN.Portable.Classes.League.Enums;
using RDN.Portable.Classes.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.League.Classes
{
    [ProtoContract]
    [DataContract]
    public class League : LeagueBase
    {
        /// <summary>
        /// if the member is being moved to a new league, this will be the id where the member is moving to.
        /// </summary>
        [ProtoMember(201)]
        [DataMember]
        public Guid LeagueMovedId { get; set; }
        [ProtoMember(202)]
        [DataMember]
        public string NameUrl { get; set; }

        [ProtoMember(203)]
        [DataMember]
        public string FoundedCultureString { get; set; }
        [ProtoMember(204)]
        [DataMember]
        public string ShopUrl { get; set; }

        [ProtoMember(205)]
        [DataMember]
        public PhotoItem Logo { get; set; }
        [ProtoMember(206)]
        [DataMember]
        public PhotoItem InternalWelcomeImage { get; set; }
        [ProtoMember(207)]
        [DataMember]
        public string InternalWelcomeMessage { get; set; }
        [ProtoMember(208)]
        [DataMember]
        public string InternalWelcomeMessageHtml { get; set; }
        [ProtoMember(209)]
        [DataMember]
        public DateTime SubscriptionPeriodEnds { get; set; }
        [ProtoMember(210)]
        [DataMember]
        public bool HasCurrentSubscription { get; set; }
        [ProtoMember(211)]
        [DataMember]
        public string LeagueOwnerType { get; set; }
        [ProtoMember(212)]
        [DataMember]
        public bool IsInactiveInLeague { get; set; }
        [ProtoMember(213)]
        [DataMember]
        public string InternalFederationIdForLeague { get; set; }
        [ProtoMember(124)]
        [DataMember]
        public Guid FederationIdForLeague { get; set; }

        //this is for editing members apart of the league
        [ProtoMember(215)]
        [DataMember]
        public DateTime? MembershipDate { get; set; }
        [ProtoMember(216)]
        [DataMember]
        public DateTime? DepartureDate { get; set; }
        [ProtoMember(217)]
        [DataMember]
        public bool HasLeftLeague { get; set; }
        [ProtoMember(218)]
        [DataMember]
        public DateTime? SkillsTestDate { get; set; }
        [ProtoMember(219)]
        [DataMember]
        public string NotesAboutMember { get; set; }
        ///newbie, begginner, pro etc..
        [ProtoMember(220)]
        [DataMember]
        public long SkaterClass { get; set; }
        [ProtoMember(221)]
        [DataMember]
        public string SkaterLevel { get; set; }
        [ProtoMember(222)]
        [DataMember]
        public LeagueOwnersEnum LeagueOwnersEnum { get; set; }
        [ProtoMember(223)]
        [DataMember]
        public List<MemberDisplay> LeagueMembers { get; set; }
        [ProtoMember(224)]
        [DataMember]
        public List<TeamDisplay> Teams { get; set; }
        [ProtoMember(125)]
        [DataMember]
        public List<ContactDisplayBasic> Contacts { get; set; }
        [ProtoMember(227)]
        [DataMember]
        public List<FederationDisplay> Federations { get; set; }
        [ProtoMember(228)]
        [DataMember]
        public List<LeagueGroup> Groups { get; set; }
        //[ProtoMember(229)]
        //[DataMember]
        //public List<LeagueDocument> Documents { get; set; }
        //[ProtoMember(230)]
        //[DataMember]
        //public List<LeagueFolder> Folders { get; set; }
        [ProtoMember(231)]
        [DataMember]
        public List<Colors.ColorDisplay> Colors { get; set; }
        [ProtoMember(232)]
        [DataMember]
        public string ColorsSelected { get; set; }
        [ProtoMember(233)]
        [DataMember]
        public string ColorTempSelected { get; set; }

        [ProtoMember(234)]
        [DataMember]
        public bool IsDuesManagementLocked { get; set; }

        [ProtoMember(235)]
        [DataMember]
        public DateTime? PassedWrittenExam { get; set; }

        public League()
        {
            LeagueMembers = new List<MemberDisplay>();
            //Owners = new List<MemberDisplay>();
            Teams = new List<TeamDisplay>();
            Federations = new List<FederationDisplay>();
            Groups = new List<LeagueGroup>();
            //Documents = new List<LeagueDocument>();
            Contacts = new List<ContactDisplayBasic>();
            Colors = new List<Colors.ColorDisplay>();
            //Folders = new List<LeagueFolder>();
        }
    }
}
