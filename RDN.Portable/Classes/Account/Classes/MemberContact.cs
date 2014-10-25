using ProtoBuf;
using RDN.Portable.Classes.Account.Enums;
using RDN.Portable.Classes.Communications.Enums;
using RDN.Portable.Classes.ContactCard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Account.Classes
{
    [ProtoContract]
    [DataContract]
    public class MemberContact
    {
        [ProtoMember(1)]
        [DataMember]
        public string Firstname { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string Lastname { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public long ContactId { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public MemberContactTypeEnum ContactType { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public string Email { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public string PhoneNumber { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public MobileServiceProvider Carrier { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public string SMSVerificationNum { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public List<Address> Addresses { get; set; }
        /// <summary>
        /// use to edit member information
        /// </summary>
        [ProtoMember(10)]
        [DataMember]
        public Address Address { get; set; }

        public MemberContact()
        {
            Addresses = new List<Address>();
        }
    }
}
