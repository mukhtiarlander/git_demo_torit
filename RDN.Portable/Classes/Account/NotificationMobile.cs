using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using RDN.Portable.Config;
using RDN.Portable.Config.Enums;
using ProtoBuf;

namespace RDN.Portable.Classes.Account
{
    [ProtoContract]
    [DataContract]
    public class NotificationMobileJson
    {
        [ProtoMember(1)]
        [DataMember]
        public long Id { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string NotificationId { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public bool CanSendGameNotifications { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string MemberId { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public MobileTypeEnum MobileTypeEnum { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public bool IsSuccessful { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public bool CanSendForumNotifications{ get; set; }
        
    }
}
