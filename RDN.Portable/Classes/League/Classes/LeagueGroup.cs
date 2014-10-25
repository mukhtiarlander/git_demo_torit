using ProtoBuf;
using RDN.Portable.Classes.Imaging;
using RDN.Portable.Classes.League.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.League.Classes
{
    [ProtoContract]
    [DataContract]
    public class LeagueGroup : LeagueGroupBasic
    {
        [ProtoMember(101)]
        [DataMember]
        public bool DefaultBroadcastMessage { get; set; }
        [ProtoMember(102)]
        [DataMember]
        public int DocumentCount { get; set; }
        [ProtoMember(103)]
        [DataMember]
        public LeagueGroupTypeEnum GroupTypeEnum { get; set; }
        [ProtoMember(104)]
        [DataMember]
        public bool IsPublicToWorld { get; set; }
        [ProtoMember(105)]
        [DataMember]
        public string EmailAddress { get; set; }
        [ProtoMember(106)]
        [DataMember]
        public List<PhotoItem> Logos { get; set; }
        [ProtoMember(107)]
        [DataMember]
        public League League { get; set; }
        /// <summary>
        /// used for the current member of the group to see if they get email notifications.
        /// </summary>
        [ProtoMember(108)]
        [DataMember]
        public bool DoesReceiveGroupNotificationsCurrentMember { get; set; }
        [ProtoMember(109)]
        [DataMember]
        public bool DoesReceiveGroupBroadcastNotificationsCurrentMember { get; set; }
        [ProtoMember(110)]
        [DataMember]
        public List<LeagueGroupMember> GroupMembers { get; set; }
        public LeagueGroup()
        {
            Logos = new List<PhotoItem>();
            GroupMembers = new List<LeagueGroupMember>();
        }
    }
}
