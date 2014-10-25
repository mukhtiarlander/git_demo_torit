using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

using System.Text;
using RDN.Portable.Config;
using ProtoBuf;

namespace RDN.Portable.Account
{
    [ProtoContract]
    [DataContract]
    public class UserMobile
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid LoginId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public Guid MemberId { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string UserName { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string Password { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public bool IsLoggedIn { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public bool DidSignUp { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public bool IsConnectedToDerby { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public int Position { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public int Gender { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public string DerbyName { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public string FirstName { get; set; }
        [ProtoMember(12)]
        [DataMember]
        public string Error { get; set; }
        [ProtoMember(13)]
        [DataMember]
        public bool IsValidSub { get; set; }
        [ProtoMember(14)]
        [DataMember]
        public bool IsRegisteredForNotifications { get; set; }
        [ProtoMember(15)]
        [DataMember]
        public string RegistrationIdForNotifications { get; set; }
        [ProtoMember(16)]
        [DataMember]
        public DateTime LastMobileLoginDate { get; set; }
        [ProtoMember(17)]
        [DataMember]
        public bool WasSuccessful { get; set; }

        [ProtoMember(18)]
        [DataMember]
        public Guid CurrentLeagueId { get; set; }


    }
}
