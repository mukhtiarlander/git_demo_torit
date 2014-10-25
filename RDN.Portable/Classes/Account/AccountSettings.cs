using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Account
{

    [ProtoContract]
    [DataContract]
    public class AccountSettingsModel
    {
        [ProtoMember(1)]
        [DataMember]
        public bool IsSubscriptionActive { get; set; }

        [ProtoMember(2)]
        [DataMember]
        public bool IsDuesManagementLockedDown { get; set; }

        [ProtoMember(3)]
        [DataMember]
        public bool IsTreasurer { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public bool IsSuccessful { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public bool IsManagerOrBetter{ get; set; }

        [ProtoMember(6)]
        [DataMember]
        public bool IsEventsCoordinatorOrBetter{ get; set; }

        [ProtoMember(7)]
        [DataMember]
        public Guid CalendarId{ get; set; }
        [ProtoMember(8)]
        [DataMember]
        public bool IsApartOfLeague{ get; set; }
        [ProtoMember(9)]
        [DataMember]
        public string ShopEndUrl { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public int UnreadMessagesCount { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public int ChallengeCount { get; set; }
        [ProtoMember(12)]
        [DataMember]
        public int OfficialsRequestCount { get; set; }
        [ProtoMember(13)]
        [DataMember]
        public string LeagueLogo { get; set; }
    }
}
