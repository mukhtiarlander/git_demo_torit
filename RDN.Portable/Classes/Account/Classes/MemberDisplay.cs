using ProtoBuf;
using RDN.Portable.Classes.Account.Enums;
using RDN.Portable.Classes.Communications.Enums;
using RDN.Portable.Classes.Federation;
using RDN.Portable.Classes.Games;
using RDN.Portable.Classes.Games.Scoreboard;
using RDN.Portable.Classes.Imaging;
using RDN.Portable.Classes.Insurance;
using RDN.Portable.Classes.League.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Account.Classes
{
    [ProtoContract]
    [DataContract]
    public class MemberDisplay : MemberDisplayBasic
    {

        public MemberDisplay()
        {
            JammerPosition = new SkaterPositionDisplay(SkaterPositionDisplayEnum.Jammer);
            BlockerPosition = new SkaterPositionDisplay(SkaterPositionDisplayEnum.Blocker);
            PivotPosition = new SkaterPositionDisplay(SkaterPositionDisplayEnum.Pivot);
            FederationsApartOf = new List<FederationDisplay>();
            Photos = new List<PhotoItem>();
            Leagues = new List<League.Classes.League>();
            Teams = new List<Team.TeamDisplay>();
            MemberContacts = new List<MemberContact>();
            ContactCard = new ContactCard.ContactCard();
            Settings = new MemberSettingsClass();
            InsuranceNumbers = new List<InsuranceNumber>();
        }

        /// <summary>
        /// this is the current league id the user has selected.
        /// users can be apart of many leagues, but should only be able to connect to one at a time.
        /// </summary>
        [ProtoMember(101)]
        [DataMember]
        public Guid CurrentLeagueId { get; set; }
        [Obsolete("Use the Leagues List Instead")]
        public string Team { get; set; }
        [Obsolete("Use the Leagues List Instead")]
        public string TeamName { get; set; }
        [ProtoMember(102)]
        [DataMember]
        public List<RDN.Portable.Classes.League.Classes.League> Leagues { get; set; }
        [ProtoMember(103)]
        [DataMember]
        public List<Team.TeamDisplay> Teams { get; set; }
        [ProtoMember(104)]
        [DataMember]
        public List<MemberContact> MemberContacts { get; set; }
        [ProtoMember(105)]
        [DataMember]
        public ContactCard.ContactCard ContactCard { get; set; }
        [ProtoMember(106)]
        [DataMember]
        public MemberSettingsClass Settings { get; set; }
        [ProtoMember(107)]
        [DataMember]
        public MobileServiceProvider Carrier { get; set; }
        [ProtoMember(108)]
        [DataMember]
        public bool IsCarrierVerified { get; set; }
        [ProtoMember(109)]
        [DataMember]
        public string SMSVerificationNum { get; set; }
        [ProtoMember(110)]
        [DataMember]
        public LeagueOwnersEnum LeagueOwnersEnum { get; set; }
        [ProtoMember(111)]
        [DataMember]
        public bool DoesReceiveLeagueNotifications { get; set; }

        /// <summary>
        /// notified via email of forum broadcasts
        /// </summary>
        [ProtoMember(112)]
        [DataMember]
        public bool EmailForumBroadcasts { get; set; }
        [ProtoMember(113)]
        [DataMember]
        public bool EmailForumWeeklyRoundup { get; set; }
        /// <summary>
        /// email notify about brand new forum post.
        /// </summary>
        [ProtoMember(114)]
        [DataMember]
        public bool EmailForumNewPost { get; set; }

        [ProtoMember(115)]
        [DataMember]
        public bool EmailCalendarNewEventBroadcast { get; set; }
        [ProtoMember(116)]
        [DataMember]
        public bool EmailMessagesReceived { get; set; }


        [ProtoMember(117)]
        [DataMember]
        public bool IsInactiveFromCurrentLeague { get; set; }
        /// <summary>
        /// this is set to TRUE if the user isn't a member of the  crowd.
        /// just someone who wanted to sign up.
        /// </summary>
        [ProtoMember(118)]
        [DataMember]
        public bool IsNotConnectedToDerby { get; set; }

        [ProtoMember(119)]
        [DataMember]
        public bool IsProfileRemovedFromPublicView { get; set; }

        /// <summary>
        /// used to change the password of the member
        /// </summary>
        [ProtoMember(120)]
        [DataMember]
        public string OldPassword { get; set; }
        [ProtoMember(121)]
        [DataMember]
        public string Password { get; set; }
        [ProtoMember(122)]
        [DataMember]
        public string NewPassword { get; set; }
        [ProtoMember(123)]
        [DataMember]
        public bool IsRetired { get; set; }
        [ProtoMember(124)]
        [DataMember]
        public string DayJob { get; set; }

        [ProtoMember(133)]
        [DataMember]
        public string LeagueClassificationOfSkatingLevel { get; set; }

        [ProtoMember(134)]
        [DataMember]
        public MedicalInformation Medical { get; set; }

        [ProtoMember(135)]
        [DataMember]
        public List<PhotoItem> Photos { get; set; }
        /// <summary>
        /// is connected to with a user profile
        /// </summary>
        [ProtoMember(136)]
        [DataMember]
        public bool IsConnected { get; set; }
        /// <summary>
        /// verification code to send out to user to verify connection to league
        /// </summary>
        [ProtoMember(137)]
        [DataMember]
        public string VerificationCode { get; set; }

        /// <summary>
        /// member bio for them selves.  fill in anything they want.
        /// </summary>
        [ProtoMember(138)]
        [DataMember]
        public string Bio { get; set; }

        public string BioShort
        {
            get
            {
                if (!String.IsNullOrEmpty(Bio) && Bio.Length > 20)
                    return Bio.Remove(20);
                return Bio;
            }
        }


        [ProtoMember(139)]
        [DataMember]
        public string BioHtml { get; set; }

        [ProtoMember(140)]
        [DataMember]
        public DefaultPositionEnum DefaultPositionType { get; set; }

        [ProtoMember(141)]
        [DataMember]
        public string Information { get; set; }
        /// <summary>
        /// DateTime Started 
        /// </summary>
        [ProtoMember(142)]
        [DataMember]
        public DateTime? StartedSkating { get; set; }
        [ProtoMember(143)]
        [DataMember]
        public DateTime? StoppedSkating { get; set; }

        /// <summary>
        /// use this variable to edit the member and allow the editor to change the member from one league to another.
        /// </summary>
        [ProtoMember(144)]
        [DataMember]
        public List<RDN.Portable.Classes.League.Classes.League> LeaguesToChooseFrom { get; set; }
        [ProtoMember(145)]
        [DataMember]
        public List<FederationDisplay> FederationsApartOf { get; set; }
        [ProtoMember(146)]
        [DataMember]
        public List<Game> GamesToDisplay { get; set; }
        /// <summary>
        /// total games played by skater
        /// </summary>
        [ProtoMember(147)]
        [DataMember]
        public int TotalGamesPlayed { get; set; }

        /// <summary>
        /// total jams played in
        /// </summary>
        [ProtoMember(148)]
        [DataMember]
        public int TotalJamsPlayed { get; set; }
        /// <summary>
        /// all the jams the skater has ever been in.
        /// </summary>
        [ProtoMember(149)]
        [DataMember]
        public List<JamModel> JamsBeenIn { get; set; }
        /// <summary>
        /// total jams in all games count
        /// </summary>
        [ProtoMember(150)]
        [DataMember]
        public int TotalJamsInAllGames { get; set; }
        /// <summary>
        /// all games won this skater has been apart of
        /// </summary>
        [ProtoMember(151)]
        [DataMember]
        public int GamesWon { get; set; }
        /// <summary>
        /// of games lost this skater has been apart of
        /// </summary>
        [ProtoMember(152)]
        [DataMember]
        public int GamesLost { get; set; }
        /// <summary>
        /// team score for teams they played on so we can calculate percentages.
        /// </summary>
        [ProtoMember(153)]
        [DataMember]
        public int Team1ScoreTotal { get; set; }
        /// <summary>
        /// total score for teams they havent played on.
        /// </summary>
        [ProtoMember(154)]
        [DataMember]
        public int Team2ScoreTotal { get; set; }
        /// <summary>
        /// MADE federation attribute for this member
        /// </summary>
        [ProtoMember(155)]
        [DataMember]
        public string MadeClassRank { get; set; }
        /// <summary>
        /// member type of the federation
        /// </summary>
        [ProtoMember(156)]
        [DataMember]
        public string FederationMemberType { get; set; }
        /// <summary>
        /// the id that the federation uses to track their members
        /// </summary>
        [ProtoMember(157)]
        [DataMember]
        public int FederationIdForMember { get; set; }

        [ProtoMember(158)]
        [DataMember]
        public double AverageJamsPerGame { get; set; }

        [ProtoMember(159)]
        [DataMember]
        public SkaterPositionDisplay JammerPosition { get; set; }
        [ProtoMember(160)]
        [DataMember]
        public SkaterPositionDisplay BlockerPosition { get; set; }
        [ProtoMember(161)]
        [DataMember]
        public SkaterPositionDisplay PivotPosition { get; set; }


        /// <summary>
        /// selected position when they first sign up so we can present them with the right information.
        /// </summary>
        [ProtoMember(162)]
        [DataMember]
        public DefaultPositionEnum DefaultSelectedPosition { get; set; }
        [ProtoMember(163)]
        [DataMember]
        public string Country { get; set; }
        [ProtoMember(164)]
        [DataMember]
        public int CountryId { get; set; }
        [ProtoMember(165)]
        [DataMember]
        public string State { get; set; }
        [ProtoMember(166)]
        [DataMember]
        public string Address { get; set; }
        [ProtoMember(167)]
        [DataMember]
        public string Address2 { get; set; }
        [ProtoMember(168)]
        [DataMember]
        public string City { get; set; }
        [ProtoMember(169)]
        [DataMember]
        public string ZipCode { get; set; }
        [ProtoMember(170)]
        [DataMember]
        public long TotalForumPostsCount { get; set; }

        [ProtoMember(171)]
        [DataMember]
        public string Website { get; set; }

        [ProtoMember(172)]
        [DataMember]
        public string Instagram { get; set; }

        [ProtoMember(173)]
        [DataMember]
        public string Twitter { get; set; }

        [ProtoMember(174)]
        [DataMember]
        public string Facebook { get; set; }

        [ProtoMember(175)]
        [DataMember]
        public DateTime? YearStartedSkating { get; set; }

        public List<InsuranceNumber> InsuranceNumbers { get; set; }
    }
}
