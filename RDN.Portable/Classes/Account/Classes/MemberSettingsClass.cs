using ProtoBuf;
using RDN.Portable.Classes.Account.Enums;
using RDN.Portable.Classes.Account.Enums.Settings;
using RDN.Portable.Classes.Communications.Enums;
using RDN.Portable.Classes.League.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Account.Classes
{
    [ProtoContract]
    [DataContract]
    public class MemberSettingsClass : MemberDisplayBasic
    {

        public MemberSettingsClass()
        { }
        [ProtoMember(601)]
        [DataMember]
        public CalendarDefaultViewEnum CalendarViewDefault { get; set; }
        [ProtoMember(602)]
        [DataMember]
        public string MobileNumber { get; set; }
        [ProtoMember(603)]
        [DataMember]
        public MobileServiceProvider ServiceProvider { get; set; }
        [ProtoMember(604)]
        [DataMember]
        public MemberPrivacySettingsEnum PrivacySettings { get; set; }
        [ProtoMember(605)]
        [DataMember]
        public bool IsCarrierVerified { get; set; }
        [ProtoMember(606)]
        [DataMember]
        public bool DoesReceiveLeagueNotifications { get; set; }
        [ProtoMember(607)]
        [DataMember]
        public bool DoYouDerby { get; set; }
        [ProtoMember(608)]
        [DataMember]
        public bool Hide_Phone_Number_From_League { get; set; }
        [ProtoMember(609)]
        [DataMember]
        public bool Hide_Email_From_League { get; set; }
        [ProtoMember(610)]
        [DataMember]
        public bool Hide_DOB_From_League { get; set; }
        [ProtoMember(611)]
        [DataMember]
        public bool Hide_DOB_From_Public { get; set; }

        /// <summary>
        /// notified via email of forum broadcasts
        /// </summary>
        [ProtoMember(612)]
        [DataMember]
        public bool EmailForumBroadcasts { get; set; }
        [ProtoMember(613)]
        [DataMember]
        public bool EmailForumWeeklyRoundup { get; set; }
        /// <summary>
        /// email notify about brand new forum post.
        /// </summary>
        [ProtoMember(614)]
        [DataMember]
        public bool EmailForumNewPost { get; set; }

        [ProtoMember(615)]
        [DataMember]
        public bool EmailCalendarNewEventBroadcast { get; set; }
        [ProtoMember(616)]
        [DataMember]
        public bool EmailMessagesReceived { get; set; }

        [ProtoMember(617)]
        [DataMember]
        public List<LeagueGroup> GroupsApartOf { get; set; }
        [ProtoMember(618)]
        [DataMember]
        public Guid CurrentLeagueId { get; set; }

        /// <summary>
        /// Sort order of Forum Message
        /// </summary>
        [ProtoMember(619)]
        [DataMember]
        public bool ForumDescending { get; set; }

        
            
        [ProtoMember(620)]
        [DataMember]
        public bool Hide_Address_From_League { get; set; }

		/// <summary>
		/// Sort order of Forum Groups
		/// </summary>
		[ProtoMember(621)]
		[DataMember]
		public string ForumGroupOrder { get; set; }

        [ProtoMember(622)]
        [DataMember]
        public NavigationDefaultViewEnum NavigationDirection { get; set; }
    }
}
