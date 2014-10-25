using ProtoBuf;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.League.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.League.Classes
{
    [ProtoContract]
    [DataContract]  
    [DebuggerDisplay("[{this.MemberId} {this.DerbyName} apart {this.IsApartOfGroup}]")]
    public class LeagueGroupMember : MemberDisplayBasic
    {
        [ProtoMember(1)]
        [DataMember]
        public GroupMemberAccessLevelEnum MemberAccessLevelEnum { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public bool DoesReceiveNewPostGroupNotifications { get; set; }
        /// <summary>
        /// anytime the broadcast button is clicked.
        /// </summary>
        [ProtoMember(3)]
        [DataMember]
        public bool DoesReceiveGroupBroadcastNotifications { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string Notes { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public bool IsApartOfGroup { get; set; }
    }
}
